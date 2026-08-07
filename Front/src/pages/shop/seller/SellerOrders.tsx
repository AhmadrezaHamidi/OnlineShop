import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { ordersApi, type Order, type OrderStatus } from '../../../api/shop/orders'

const STATUS_META: Record<OrderStatus, { label: string; color: string; bg: string }> = {
  Pending:    { label: 'در انتظار',      color: 'text-honey-600',  bg: 'bg-honey-50'   },
  Processing: { label: 'در حال پردازش', color: 'text-blue-600',   bg: 'bg-blue-50'    },
  Shipped:    { label: 'ارسال شده',      color: 'text-forest-600', bg: 'bg-forest-50'  },
  Delivered:  { label: 'تحویل شده',     color: 'text-forest-700', bg: 'bg-forest-100' },
  Cancelled:  { label: 'لغو شده',       color: 'text-red-500',    bg: 'bg-red-50'     },
}

const NEXT_ACTION: Partial<Record<OrderStatus, { next: 'confirm' | 'ship' | 'deliver'; label: string }>> = {
  Pending:    { next: 'confirm', label: 'تأیید و شروع پردازش' },
  Processing: { next: 'ship',    label: 'علامت‌گذاری ارسال شد' },
  Shipped:    { next: 'deliver', label: 'تأیید تحویل' },
}

function fDate(iso: string) {
  return new Date(iso).toLocaleDateString('fa-IR')
}

export default function SellerOrders() {
  const [filter, setFilter]   = useState<OrderStatus | 'All'>('All')
  const [expanded, setExpanded] = useState<number | null>(null)
  const qc = useQueryClient()

  const { data: orders = [], isLoading } = useQuery({
    queryKey: ['seller-orders'],
    queryFn: ordersApi.list,
  })

  const advanceMut = useMutation({
    mutationFn: ({ id, action }: { id: number; action: 'confirm' | 'ship' | 'deliver' }) => {
      if (action === 'confirm') return ordersApi.confirm(id)
      if (action === 'ship')    return ordersApi.ship(id)
      return ordersApi.deliver(id)
    },
    onSuccess: () => qc.invalidateQueries({ queryKey: ['seller-orders'] }),
  })

  const visible = filter === 'All' ? orders : orders.filter((o: Order) => o.status === filter)

  if (isLoading) return (
    <div className="flex items-center justify-center py-20 text-ink-300">
      <span className="text-3xl animate-spin">⏳</span>
    </div>
  )

  return (
    <div className="space-y-4">
      <h1 className="text-xl font-extrabold text-ink-900">سفارشات</h1>

      <div className="flex gap-2 overflow-x-auto pb-1 no-scrollbar">
        {(['All','Pending','Processing','Shipped','Delivered','Cancelled'] as const).map(k => (
          <button
            key={k}
            onClick={() => setFilter(k)}
            className={`shrink-0 px-3 py-1.5 rounded-full text-xs font-semibold border transition-all ${
              filter === k
                ? 'bg-forest-500 text-white border-forest-500'
                : 'bg-white text-ink-500 border-ink-100 hover:border-forest-300'
            }`}
          >
            {k === 'All' ? 'همه' : STATUS_META[k].label}
          </button>
        ))}
      </div>

      {visible.length === 0 && (
        <div className="text-center py-16 text-ink-300">
          <div className="text-5xl mb-3">📭</div>
          <p className="text-sm">سفارشی یافت نشد</p>
        </div>
      )}

      {visible.map((order: Order) => {
        const meta   = STATUS_META[order.status]
        const action = NEXT_ACTION[order.status]
        const isOpen = expanded === order.id

        return (
          <div key={order.id} className="bg-white rounded-2xl border border-ink-100 shadow-sm overflow-hidden">
            <button
              className="w-full flex items-center justify-between px-4 py-3.5 hover:bg-forest-50/40 transition-colors"
              onClick={() => setExpanded(isOpen ? null : order.id)}
            >
              <div className="text-right">
                <p className="font-bold text-ink-900 text-sm">ORD-{order.id}</p>
                <p className="text-ink-300 text-xs">{order.customerName} · {fDate(order.createdAt)}</p>
              </div>
              <div className="flex items-center gap-3">
                <span className={`text-xs font-semibold px-2 py-1 rounded-lg ${meta.bg} ${meta.color}`}>
                  {meta.label}
                </span>
                <span className={`text-ink-300 text-xs transition-transform ${isOpen ? 'rotate-180' : ''}`}>▼</span>
              </div>
            </button>

            {isOpen && (
              <div className="border-t border-ink-100 px-4 py-3 space-y-3">
                <div className="flex items-center gap-2 bg-forest-50 rounded-xl px-3 py-2">
                  <span className="text-lg">👤</span>
                  <div>
                    <p className="text-sm font-semibold text-ink-900">{order.customerName}</p>
                    <p className="text-xs text-ink-300">{order.customerPhone}</p>
                  </div>
                </div>

                {order.items.map(item => (
                  <div key={item.id} className="flex justify-between items-center text-sm">
                    <span className="text-ink-700">{item.productName} × {item.quantity}</span>
                    <span className="font-semibold text-ink-900">
                      {item.totalPrice.toLocaleString('fa-IR')} ت
                    </span>
                  </div>
                ))}

                <div className="flex justify-between items-center border-t border-ink-100 pt-3">
                  <span className="text-sm text-ink-400">جمع</span>
                  <span className="font-extrabold text-forest-600">
                    {order.totalAmount.toLocaleString('fa-IR')} تومان
                  </span>
                </div>

                {action && (
                  <button
                    onClick={() => advanceMut.mutate({ id: order.id, action: action.next })}
                    disabled={advanceMut.isPending}
                    className="w-full py-2.5 rounded-xl bg-forest-500 text-white text-sm font-semibold hover:bg-forest-600 transition-colors disabled:opacity-50"
                  >
                    {advanceMut.isPending ? '...' : action.label}
                  </button>
                )}
              </div>
            )}
          </div>
        )
      })}
    </div>
  )
}
