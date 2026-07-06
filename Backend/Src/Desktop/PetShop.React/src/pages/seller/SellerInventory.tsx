import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { productsApi } from '../../api/products'

export default function SellerInventory() {
  const qc = useQueryClient()
  const [page, setPage] = useState(1)
  const [replenishModal, setReplenishModal] = useState<{ id: number; name: string } | null>(null)
  const [qty, setQty] = useState('')

  const { data, isLoading } = useQuery({
    queryKey: ['inventory-products', page],
    queryFn: () => productsApi.getAll({ page, pageSize: 12 }),
  })

  const replenish = useMutation({
    mutationFn: ({ id, quantity }: { id: number; quantity: number }) => productsApi.replenish(id, quantity),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['inventory-products'] })
      setReplenishModal(null)
      setQty('')
    },
  })

  const products   = data?.data?.data?.items ?? []
  const totalPages = data?.data?.data?.totalPages ?? 1

  return (
    <>
      <div style={{ marginBottom: 20 }}>
        <h2 style={{ margin: 0, fontSize: '1.4rem', fontWeight: 700 }}>مدیریت انبار</h2>
        <p style={{ margin: '4px 0 0', color: '#64748b', fontSize: '.85rem' }}>موجودی محصولات و تأمین مجدد</p>
      </div>

      {/* Replenish Modal */}
      {replenishModal && (
        <div style={{
          position: 'fixed', inset: 0, background: 'rgba(0,0,0,.5)',
          display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 1000,
        }}>
          <div className="card" style={{ width: 380 }}>
            <h3 style={{ margin: '0 0 16px' }}>تأمین موجودی</h3>
            <p style={{ margin: '0 0 12px', color: '#64748b', fontSize: .9 + 'rem' }}>
              محصول: <strong>{replenishModal.name}</strong>
            </p>
            <label style={{ display: 'block', marginBottom: 4, fontSize: 13, fontWeight: 500 }}>تعداد</label>
            <input type="number" value={qty} onChange={e => setQty(e.target.value)} placeholder="مثال: 50" style={{ width: '100%', marginBottom: 16 }} />
            <div style={{ display: 'flex', gap: 8, justifyContent: 'flex-end' }}>
              <button className="btn btn-ghost" onClick={() => { setReplenishModal(null); setQty('') }}>لغو</button>
              <button className="btn btn-primary"
                disabled={!qty || replenish.isPending}
                onClick={() => replenish.mutate({ id: replenishModal.id, quantity: Number(qty) })}>
                {replenish.isPending ? 'در حال ثبت...' : 'ثبت تأمین'}
              </button>
            </div>
          </div>
        </div>
      )}

      <div className="card">
        {isLoading ? (
          <div style={{ textAlign: 'center', padding: 48 }}><div className="spinner" /></div>
        ) : (
          <table>
            <thead>
              <tr>
                <th>#</th>
                <th>نام محصول</th>
                <th>دسته‌بندی</th>
                <th>قیمت</th>
                <th>وضعیت</th>
                <th>عملیات انبار</th>
              </tr>
            </thead>
            <tbody>
              {products.map(p => (
                <tr key={p.id}>
                  <td style={{ color: 'var(--p)', fontWeight: 700 }}>#{p.id}</td>
                  <td style={{ fontWeight: 600 }}>{p.name}</td>
                  <td style={{ color: '#64748b', fontSize: 13 }}>{p.categoryName}</td>
                  <td>{p.price.toLocaleString('fa-IR')} ت</td>
                  <td>
                    <span className={`pill ${p.status === 'Active' ? 'pill-green' : p.status === 'Inactive' ? 'pill-amber' : 'pill-gray'}`}>
                      {p.status === 'Active' ? 'فعال' : p.status === 'Inactive' ? 'غیرفعال' : 'آرشیو'}
                    </span>
                  </td>
                  <td>
                    <div style={{ display: 'flex', gap: 4 }}>
                      <button className="btn btn-teal btn-sm" onClick={() => setReplenishModal({ id: p.id, name: p.name })}>
                        <i className="ti ti-plus" /> تأمین موجودی
                      </button>
                      <button className="btn btn-ghost btn-sm" onClick={() => productsApi.reserve(p.id, 1)}>
                        <i className="ti ti-lock" /> رزرو
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
              {products.length === 0 && (
                <tr><td colSpan={6} style={{ textAlign: 'center', padding: 40, color: '#94a3b8' }}>محصولی یافت نشد</td></tr>
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
