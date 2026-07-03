import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { ordersApi } from '../../api/orders'
import type { OrderStatus } from '../../types'
import styles from './AdminOrders.module.css'

const STATUS_OPTIONS: { value: OrderStatus | ''; label: string }[] = [
  { value: '', label: 'همه وضعیت‌ها' },
  { value: 'Draft',     label: 'پیش‌نویس' },
  { value: 'Placed',    label: 'ثبت‌شده' },
  { value: 'Confirmed', label: 'تأیید شده' },
  { value: 'Shipped',   label: 'ارسال شده' },
  { value: 'Delivered', label: 'تحویل شده' },
  { value: 'Cancelled', label: 'لغو شده' },
]
const PILL: Record<string, string> = {
  Draft: 'pill-gray', Placed: 'pill-purple', Confirmed: 'pill-teal',
  Shipped: 'pill-amber', Delivered: 'pill-green', Cancelled: 'pill-red',
}

export default function AdminOrders() {
  const qc = useQueryClient()
  const [page, setPage] = useState(1)
  const [status, setStatus] = useState<OrderStatus | ''>('')

  const { data, isLoading } = useQuery({
    queryKey: ['orders', page, status],
    queryFn: () => ordersApi.getAll({ page, pageSize: 15, status: status || undefined }),
  })

  const confirm = useMutation({ mutationFn: (id: number) => ordersApi.confirm(id), onSuccess: () => qc.invalidateQueries({ queryKey: ['orders'] }) })
  const ship    = useMutation({ mutationFn: (id: number) => ordersApi.ship(id),    onSuccess: () => qc.invalidateQueries({ queryKey: ['orders'] }) })
  const deliver = useMutation({ mutationFn: (id: number) => ordersApi.deliver(id), onSuccess: () => qc.invalidateQueries({ queryKey: ['orders'] }) })
  const cancel  = useMutation({ mutationFn: (id: number) => ordersApi.cancel(id, 'لغو توسط ادمین'), onSuccess: () => qc.invalidateQueries({ queryKey: ['orders'] }) })

  const orders = data?.data?.data?.items ?? []
  const total  = data?.data?.data?.totalCount ?? 0
  const totalPages = data?.data?.data?.totalPages ?? 1

  return (
    <>
      <div className={styles.header}>
        <h2>سفارش‌ها</h2>
        <div className={styles.filters}>
          <select value={status} onChange={e => { setStatus(e.target.value as OrderStatus | ''); setPage(1) }} style={{ width: 180 }}>
            {STATUS_OPTIONS.map(o => <option key={o.value} value={o.value}>{o.label}</option>)}
          </select>
        </div>
      </div>

      <div className="card">
        <div className={styles.tableInfo}>
          <span>مجموع: <strong>{total.toLocaleString('fa-IR')}</strong> سفارش</span>
        </div>
        {isLoading ? (
          <div className="spinner-center"><div className="spinner" /></div>
        ) : (
          <table>
            <thead>
              <tr>
                <th>شناسه</th>
                <th>کاربر</th>
                <th>مبلغ (تومان)</th>
                <th>تعداد آیتم</th>
                <th>وضعیت</th>
                <th>تاریخ</th>
                <th>عملیات</th>
              </tr>
            </thead>
            <tbody>
              {orders.map(o => (
                <tr key={o.id}>
                  <td><span style={{ fontWeight: 700, color: 'var(--p)' }}>#{o.id}</span></td>
                  <td>{o.userId}</td>
                  <td style={{ fontWeight: 700 }}>{o.totalAmount.toLocaleString('fa-IR')}</td>
                  <td>{o.items?.length ?? '—'}</td>
                  <td><span className={`pill ${PILL[o.status] ?? 'pill-gray'}`}>{STATUS_OPTIONS.find(s => s.value === o.status)?.label ?? o.status}</span></td>
                  <td style={{ color: 'var(--text2)', fontSize: 12 }}>{new Date(o.createdAt).toLocaleDateString('fa-IR')}</td>
                  <td>
                    <div style={{ display: 'flex', gap: 4 }}>
                      {o.status === 'Placed'    && <button className="btn btn-ghost btn-sm" onClick={() => confirm.mutate(o.id)}>تأیید</button>}
                      {o.status === 'Confirmed' && <button className="btn btn-teal btn-sm"  onClick={() => ship.mutate(o.id)}>ارسال</button>}
                      {o.status === 'Shipped'   && <button className="btn btn-primary btn-sm" onClick={() => deliver.mutate(o.id)}>تحویل</button>}
                      {!['Delivered','Cancelled'].includes(o.status) && (
                        <button className="btn btn-danger btn-sm" onClick={() => cancel.mutate(o.id)}>لغو</button>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
              {orders.length === 0 && (
                <tr><td colSpan={7} style={{ textAlign: 'center', color: 'var(--text3)', padding: 32 }}>سفارشی یافت نشد</td></tr>
              )}
            </tbody>
          </table>
        )}

        {/* Pagination */}
        {totalPages > 1 && (
          <div className={styles.pagination}>
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
