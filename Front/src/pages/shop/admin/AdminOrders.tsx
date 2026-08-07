import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { ordersApi, type Order, type OrderStatus } from '../../../api/shop/orders'

const STATUS_META: Record<OrderStatus, { label: string; color: string; bg: string; dot: string }> = {
  Pending:    { label: 'در انتظار',  color: 'text-honey-600',  bg: 'bg-honey-50',   dot: 'bg-honey-500'  },
  Processing: { label: 'پردازش',    color: 'text-blue-600',   bg: 'bg-blue-50',    dot: 'bg-blue-500'   },
  Shipped:    { label: 'ارسال شده', color: 'text-forest-600', bg: 'bg-forest-50',  dot: 'bg-forest-500' },
  Delivered:  { label: 'تحویل شده',color: 'text-forest-700', bg: 'bg-forest-100', dot: 'bg-forest-600' },
  Cancelled:  { label: 'لغو شده',  color: 'text-red-500',    bg: 'bg-red-50',     dot: 'bg-red-500'    },
}

const STATUSES: (OrderStatus | 'All')[] = ['All','Pending','Processing','Shipped','Delivered','Cancelled']

function fDate(iso: string) {
  return new Date(iso).toLocaleDateString('fa-IR')
}

export default function AdminOrders() {
  const [filter, setFilter]   = useState<OrderStatus | 'All'>('All')
  const [search, setSearch]   = useState('')
  const [selected, setSelected] = useState<Order | null>(null)
  const qc = useQueryClient()

  const { data: orders = [], isLoading } = useQuery({
    queryKey: ['admin-orders'],
    queryFn: ordersApi.list,
  })

  const mut = useMutation({
    mutationFn: ({ id, action }: { id: number; action: 'confirm' | 'ship' | 'deliver' | 'cancel' }) => {
      if (action === 'confirm') return ordersApi.confirm(id)
      if (action === 'ship')    return ordersApi.ship(id)
      if (action === 'deliver') return ordersApi.deliver(id)
      return ordersApi.cancel(id)
    },
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['admin-orders'] })
      setSelected(null)
    },
  })

  const visible = orders
    .filter((o: Order) => filter === 'All' || o.status === filter)
    .filter((o: Order) =>
      !search ||
      String(o.id).includes(search) ||
      o.customerName?.includes(search) ||
      o.sellerName?.includes(search)
    )

  const stats = {
    total:   orders.length,
    pending: orders.filter((o: Order) => o.status === 'Pending').length,
    shipped: orders.filter((o: Order) => o.status === 'Shipped').length,
    revenue: orders
      .filter((o: Order) => o.status !== 'Cancelled')
      .reduce((s: number, o: Order) => s + o.totalAmount, 0),
  }

  if (isLoading) return (
    <div className="flex items-center justify-center py-20 text-ink-300">
      <span className="text-3xl animate-spin">⏳</span>
    </div>
  )

  return (
    <div className="space-y-5">
      <h1 className="text-xl font-extrabold text-ink-900">مدیریت سفارشات</h1>

      <div className="grid grid-cols-2 md:grid-cols-4 gap-3">
        {[
          { label: 'کل سفارشات',    value: stats.total,                          icon: '📦', cls: 'bg-pet-50   text-pet-600'    },
          { label: 'در انتظار',     value: stats.pending,                         icon: '⏳', cls: 'bg-honey-50  text-honey-600'  },
          { label: 'ارسال شده',     value: stats.shipped,                         icon: '🚚', cls: 'bg-forest-50 text-forest-600' },
          { label: 'درآمد',         value: stats.revenue.toLocaleString('fa-IR'), icon: '💰', cls: 'bg-blue-50   text-blue-600'   },
        ].map(s => (
          <div key={s.label} className={`rounded-2xl p-4 ${s.cls}`}>
            <div className="text-2xl mb-1">{s.icon}</div>
            <div className="text-xl font-extrabold">{s.value}</div>
            <div className="text-xs opacity-70 mt-0.5">{s.label}</div>
          </div>
        ))}
      </div>

      <input
        value={search}
        onChange={e => setSearch(e.target.value)}
        placeholder="جستجو شماره سفارش، مشتری، فروشنده…"
        className="w-full px-4 py-2.5 rounded-xl border border-ink-100 bg-white text-sm outline-none focus:border-pet-400 focus:ring-2 focus:ring-pet-100 transition-all"
        dir="rtl"
      />

      <div className="flex gap-2 overflow-x-auto pb-1 no-scrollbar">
        {STATUSES.map(k => (
          <button
            key={k}
            onClick={() => setFilter(k)}
            className={`shrink-0 px-3 py-1.5 rounded-full text-xs font-semibold border transition-all ${
              filter === k
                ? 'bg-pet-500 text-white border-pet-500'
                : 'bg-white text-ink-500 border-ink-100 hover:border-pet-300'
            }`}
          >
            {k === 'All' ? 'همه' : STATUS_META[k].label}
          </button>
        ))}
      </div>

      <div className="bg-white rounded-2xl border border-ink-100 shadow-sm overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full text-sm" dir="rtl">
            <thead>
              <tr className="border-b border-ink-100 bg-ink-900/5">
                {['شماره','مشتری','فروشنده','تاریخ','مبلغ','وضعیت',''].map(h => (
                  <th key={h} className="text-right px-4 py-3 text-xs font-semibold text-ink-400 whitespace-nowrap">{h}</th>
                ))}
              </tr>
            </thead>
            <tbody className="divide-y divide-ink-100">
              {visible.map((order: Order) => {
                const meta = STATUS_META[order.status]
                return (
                  <tr key={order.id} className="hover:bg-pet-50/30 transition-colors">
                    <td className="px-4 py-3 font-bold text-ink-900 whitespace-nowrap">ORD-{order.id}</td>
                    <td className="px-4 py-3 text-ink-700 whitespace-nowrap">{order.customerName}</td>
                    <td className="px-4 py-3 text-ink-500 whitespace-nowrap">{order.sellerName ?? '—'}</td>
                    <td className="px-4 py-3 text-ink-400 whitespace-nowrap">{fDate(order.createdAt)}</td>
                    <td className="px-4 py-3 font-semibold text-ink-900 whitespace-nowrap">
                      {order.totalAmount.toLocaleString('fa-IR')}
                    </td>
                    <td className="px-4 py-3 whitespace-nowrap">
                      <span className={`inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-semibold ${meta.bg} ${meta.color}`}>
                        <span className={`w-1.5 h-1.5 rounded-full ${meta.dot}`} />
                        {meta.label}
                      </span>
                    </td>
                    <td className="px-4 py-3">
                      <button
                        onClick={() => setSelected(order)}
                        className="text-xs text-pet-500 hover:underline font-semibold whitespace-nowrap"
                      >
                        جزئیات
                      </button>
                    </td>
                  </tr>
                )
              })}
              {visible.length === 0 && (
                <tr><td colSpan={7} className="text-center py-12 text-ink-300 text-sm">سفارشی یافت نشد</td></tr>
              )}
            </tbody>
          </table>
        </div>
      </div>

      {selected && (
        <div className="fixed inset-0 bg-black/40 z-50 flex items-center justify-center p-4" onClick={() => setSelected(null)}>
          <div className="bg-white rounded-2xl shadow-xl w-full max-w-sm" onClick={e => e.stopPropagation()} dir="rtl">
            <div className="flex items-center justify-between px-5 py-4 border-b border-ink-100">
              <h3 className="font-extrabold text-ink-900">ORD-{selected.id}</h3>
              <button onClick={() => setSelected(null)} className="text-ink-300 hover:text-ink-500 text-lg">✕</button>
            </div>
            <div className="px-5 py-4 space-y-3 text-sm">
              {[
                ['مشتری',   selected.customerName],
                ['فروشنده', selected.sellerName ?? '—'],
                ['تاریخ',   fDate(selected.createdAt)],
                ['اقلام',   `${selected.items.length} ردیف`],
                ['مبلغ کل', `${selected.totalAmount.toLocaleString('fa-IR')} تومان`],
              ].map(([k,v]) => (
                <div key={k} className="flex justify-between">
                  <span className="text-ink-400">{k}</span>
                  <span className="font-semibold text-ink-900">{v}</span>
                </div>
              ))}
              <div className="flex justify-between items-center">
                <span className="text-ink-400">وضعیت</span>
                <span className={`text-xs font-semibold px-2.5 py-1 rounded-full ${STATUS_META[selected.status].bg} ${STATUS_META[selected.status].color}`}>
                  {STATUS_META[selected.status].label}
                </span>
              </div>
            </div>
            <div className="px-5 pb-5 space-y-2">
              {selected.status === 'Pending' && (
                <button
                  onClick={() => mut.mutate({ id: selected.id, action: 'confirm' })}
                  disabled={mut.isPending}
                  className="w-full py-2.5 rounded-xl bg-forest-500 text-white text-sm font-semibold hover:bg-forest-600 disabled:opacity-50 transition-colors"
                >
                  تأیید سفارش
                </button>
              )}
              {selected.status === 'Processing' && (
                <button
                  onClick={() => mut.mutate({ id: selected.id, action: 'ship' })}
                  disabled={mut.isPending}
                  className="w-full py-2.5 rounded-xl bg-blue-500 text-white text-sm font-semibold hover:bg-blue-600 disabled:opacity-50 transition-colors"
                >
                  ثبت ارسال
                </button>
              )}
              {selected.status === 'Shipped' && (
                <button
                  onClick={() => mut.mutate({ id: selected.id, action: 'deliver' })}
                  disabled={mut.isPending}
                  className="w-full py-2.5 rounded-xl bg-forest-600 text-white text-sm font-semibold hover:bg-forest-700 disabled:opacity-50 transition-colors"
                >
                  تأیید تحویل
                </button>
              )}
              {!['Delivered','Cancelled'].includes(selected.status) && (
                <button
                  onClick={() => mut.mutate({ id: selected.id, action: 'cancel' })}
                  disabled={mut.isPending}
                  className="w-full py-2.5 rounded-xl border border-red-200 text-red-500 text-sm font-semibold hover:bg-red-50 disabled:opacity-50 transition-colors"
                >
                  لغو سفارش
                </button>
              )}
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
