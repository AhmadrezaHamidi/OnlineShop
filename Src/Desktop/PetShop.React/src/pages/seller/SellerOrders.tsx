import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { ordersApi } from '../../api/orders'
import { ORDER_STATUS_LABEL, ORDER_STATUS_PILL } from '../../types'

const STATUS_OPTIONS = [
  { value: '', label: 'همه وضعیت‌ها' },
  { value: '0', label: 'پیش‌نویس' },  { value: '1', label: 'ثبت‌شده' },
  { value: '2', label: 'تأیید شده' }, { value: '3', label: 'ارسال شده' },
  { value: '4', label: 'تحویل شده' }, { value: '5', label: 'لغو شده' },
]

export default function SellerOrders() {
  const qc = useQueryClient()
  const [page, setPage] = useState(1)
  const [statusFilter, setStatusFilter] = useState('')

  const { data, isLoading } = useQuery({
    queryKey: ['seller-orders-list', page, statusFilter],
    queryFn: () => ordersApi.getAll({ page, pageSize: 12, status: statusFilter ? Number(statusFilter) as any : undefined }),
  })

  const confirm  = useMutation({ mutationFn: (id: number) => ordersApi.confirm(id),  onSuccess: () => qc.invalidateQueries({ queryKey: ['seller-orders-list'] }) })
  const ship     = useMutation({ mutationFn: (id: number) => ordersApi.ship(id),     onSuccess: () => qc.invalidateQueries({ queryKey: ['seller-orders-list'] }) })
  const deliver  = useMutation({ mutationFn: (id: number) => ordersApi.deliver(id),  onSuccess: () => qc.invalidateQueries({ queryKey: ['seller-orders-list'] }) })
  const cancel   = useMutation({ mutationFn: (id: number) => ordersApi.cancel(id),   onSuccess: () => qc.invalidateQueries({ queryKey: ['seller-orders-list'] }) })

  const orders     = data?.data?.data?.items ?? data?.data?.items ?? []
  const totalPages = data?.data?.data?.totalPages ?? data?.data?.totalPages ?? 1
  const total      = data?.data?.data?.totalCount ?? data?.data?.totalCount ?? 0

  return (
    <>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 20 }}>
        <div>
          <h2 style={{ margin: 0, fontSize: '1.4rem', fontWeight: 700 }}>سفارش‌ها</h2>
          <p style={{ margin: '4px 0 0', color: '#64748b', fontSize: '.85rem' }}>
            مجموع: {total.toLocaleString('fa-IR')} سفارش
          </p>
        </div>
        <select
          value={statusFilter}
          onChange={e => { setStatusFilter(e.target.value); setPage(1) }}
          style={{ padding: '.5rem .8rem', border: '1.5px solid #E2E8F0', borderRadius: 10, fontFamily: 'inherit', fontSize: '.9rem', background: '#fff' }}
        >
          {STATUS_OPTIONS.map(o => <option key={o.value} value={o.value}>{o.label}</option>)}
        </select>
      </div>

      <div className="card">
        {isLoading ? (
          <div style={{ textAlign: 'center', padding: 48 }}><div className="spinner" /></div>
        ) : (
          <table>
            <thead>
              <tr>
                <th>شناسه</th>
                <th>مبلغ (تومان)</th>
                <th>آیتم‌ها</th>
                <th>وضعیت</th>
                <th>تاریخ</th>
                <th>عملیات</th>
              </tr>
            </thead>
            <tbody>
              {orders.map(o => (
                <tr key={o.id}>
                  <td style={{ fontWeight: 700, color: 'var(--p)' }}>#{o.id}</td>
                  <td style={{ fontWeight: 700 }}>{o.totalAmount.toLocaleString('fa-IR')}</td>
                  <td>{o.items?.length ?? '—'} آیتم</td>
                  <td>
                    <span className={`pill ${ORDER_STATUS_PILL[o.status] ?? 'pill-gray'}`}>
                      {ORDER_STATUS_LABEL[o.status] ?? o.status}
                    </span>
                  </td>
                  <td style={{ fontSize: 12, color: '#64748b' }}>
                    {o.placedAt ? new Date(o.placedAt).toLocaleDateString('fa-IR') : '—'}
                  </td>
                  <td>
                    <div style={{ display: 'flex', gap: 4, flexWrap: 'wrap' }}>
                      {o.status === 1 && <button className="btn btn-teal btn-sm"    onClick={() => confirm.mutate(o.id)}>تأیید</button>}
                      {o.status === 2 && <button className="btn btn-primary btn-sm" onClick={() => ship.mutate(o.id)}>ارسال</button>}
                      {o.status === 3 && <button className="btn btn-ghost btn-sm"   onClick={() => deliver.mutate(o.id)}>تحویل</button>}
                      {o.status !== 4 && o.status !== 5 && (
                        <button className="btn btn-danger btn-sm" onClick={() => cancel.mutate(o.id)}>لغو</button>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
              {orders.length === 0 && (
                <tr><td colSpan={6} style={{ textAlign: 'center', padding: 40, color: '#94a3b8' }}>سفارشی یافت نشد</td></tr>
              )}
            </tbody>
          </table>
        )}

        {totalPages > 1 && (
          <div style={{ display: 'flex', gap: 6, justifyContent: 'center', marginTop: 16 }}>
            <button className="btn btn-ghost btn-sm" disabled={page <= 1} onClick={() => setPage(p => p - 1)}>
              <i className="ti ti-chevron-right" />
            </button>
            {Array.from({ length: Math.min(totalPages, 7) }, (_, i) => i + 1).map(p => (
              <button key={p} className={`btn btn-sm ${p === page ? 'btn-primary' : 'btn-ghost'}`} onClick={() => setPage(p)}>{p}</button>
            ))}
            <button className="btn btn-ghost btn-sm" disabled={page >= totalPages} onClick={() => setPage(p => p + 1)}>
              <i className="ti ti-chevron-left" />
            </button>
          </div>
        )}
      </div>
    </>
  )
}
