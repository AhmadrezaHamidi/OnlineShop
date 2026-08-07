import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { ordersApi, type Order, type OrderStatus } from '../../../api/shop/orders'

const STATUS_META: Record<OrderStatus, { label: string; color: string; bg: string; icon: string }> = {
  Pending:    { label: 'در انتظار تأیید', color: 'text-honey-600',  bg: 'bg-honey-50',   icon: '⏳' },
  Processing: { label: 'در حال پردازش',   color: 'text-blue-500',   bg: 'bg-blue-50',    icon: '⚙️' },
  Shipped:    { label: 'ارسال شده',        color: 'text-forest-600', bg: 'bg-forest-50',  icon: '🚚' },
  Delivered:  { label: 'تحویل داده شد',   color: 'text-forest-700', bg: 'bg-forest-100', icon: '✅' },
  Cancelled:  { label: 'لغو شده',          color: 'text-red-500',    bg: 'bg-red-50',     icon: '❌' },
}

const FILTERS: { key: OrderStatus | 'All'; label: string }[] = [
  { key: 'All',        label: 'همه' },
  { key: 'Pending',    label: 'در انتظار' },
  { key: 'Processing', label: 'پردازش' },
  { key: 'Shipped',    label: 'ارسال شده' },
  { key: 'Delivered',  label: 'تحویل شده' },
  { key: 'Cancelled',  label: 'لغو شده' },
]

function fDate(iso: string) {
  return new Date(iso).toLocaleDateString('fa-IR')
}

export default function CustomerOrders() {
  const [filter, setFilter]   = useState<OrderStatus | 'All'>('All')
  const [expanded, setExpanded] = useState<number | null>(null)
  const qc = useQueryClient()

  const { data: orders = [], isLoading, isError } = useQuery({
    queryKey: ['orders'],
    queryFn: ordersApi.list,
  })

  const cancelMut = useMutation({
    mutationFn: (id: number) => ordersApi.cancel(id),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['orders'] }),
  })

  const visible = filter === 'All' ? orders : orders.filter((o: Order) => o.status === filter)

  if (isLoading) return (
    <div className="flex items-center justify-center py-20 text-ink-300">
      <span className="text-3xl animate-spin">⏳</span>
    </div>
  )

  if (isError) return (
    <div className="text-center py-20 text-red-400">
      <div className="text-4xl mb-2">⚠️</div>
      <p className="text-sm">خطا در بارگذاری سفارشات</p>
    </div>
  )

  return (
    <div className="space-y-4">
      <h1 className="text-xl font-extrabold text-ink-900">سفارشات من</h1>

      <div className="flex gap-2 overflow-x-auto pb-1 no-scrollbar">
        {FILTERS.map(({ key, label }) => (
          <button
            key={key}
            onClick={() => setFilter(key)}
            className={`shrink-0 px-3 py-1.5 rounded-full text-xs font-semibold border transition-all ${
              filter === key
                ? 'bg-pet-500 text-white border-pet-500'
                : 'bg-white text-ink-500 border-ink-100 hover:border-pet-300'
            }`}
          >
            {label}
          </button>
        ))}
      </div>

      {visible.length === 0 && (
        <div className="text-center py-16 text-ink-300">
          <div className="text-5xl mb-3">📭</div>
          <p className="text-sm">سفارشی در این دسته وجود ندارد</p>
        </div>
      )}

      {visible.map((order: Order) => {
        const meta  = STATUS_META[order.status]
        const isOpen = expanded === order.id
        return (
          <div key={order.id} className="bg-white rounded-2xl border border-ink-100 shadow-sm overflow-hidden">
            <button
              className="w-full flex items-center justify-between px-4 py-3.5 hover:bg-pet-50/40 transition-colors"
              onClick={() => setExpanded(isOpen ? null : order.id)}
            >
              <div className="flex items-center gap-3">
                <span className={`w-9 h-9 rounded-xl flex items-center justify-center text-lg ${meta.bg}`}>
                  {meta.icon}
                </span>
                <div className="text-right">
                  <p className="font-bold text-ink-900 text-sm">ORD-{order.id}</p>
                  <p className="text-ink-300 text-xs">{fDate(order.createdAt)}</p>
                </div>
              </div>
              <div className="flex items-center gap-3">
                <div className="text-right">
                  <p className={`text-xs font-semibold ${meta.color}`}>{meta.label}</p>
                  <p className="text-ink-900 font-bold text-sm">{order.totalAmount.toLocaleString('fa-IR')} ت</p>
                </div>
                <span className={`text-ink-300 text-xs transition-transform ${isOpen ? 'rotate-180' : ''}`}>▼</span>
              </div>
            </button>

            {isOpen && (
              <div className="border-t border-ink-100 px-4 py-3 space-y-3">
                {order.items.map(item => (
                  <div key={item.id} className="flex items-center gap-3">
                    <div className="w-11 h-11 bg-pet-50 rounded-xl flex items-center justify-center text-2xl shrink-0">
                      🛍️
                    </div>
                    <div className="flex-1 min-w-0">
                      <p className="text-sm font-semibold text-ink-900 truncate">{item.productName}</p>
                      <p className="text-xs text-ink-300">{item.quantity} عدد</p>
                    </div>
                    <p className="text-sm font-bold text-ink-700 shrink-0">
                      {item.totalPrice.toLocaleString('fa-IR')} ت
                    </p>
                  </div>
                ))}

                <div className="flex justify-between items-center border-t border-ink-100 pt-3">
                  <span className="text-sm text-ink-400">جمع کل</span>
                  <span className="font-extrabold text-pet-500">{order.totalAmount.toLocaleString('fa-IR')} تومان</span>
                </div>

                {order.status === 'Pending' && (
                  <button
                    onClick={() => cancelMut.mutate(order.id)}
                    disabled={cancelMut.isPending}
                    className="w-full py-2.5 rounded-xl border border-red-200 text-red-400 text-sm font-semibold hover:bg-red-50 transition-colors disabled:opacity-50"
                  >
                    {cancelMut.isPending ? '...' : 'لغو سفارش'}
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
