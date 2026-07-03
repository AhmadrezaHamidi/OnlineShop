import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { productsApi } from '../../api/products'
import { PRODUCT_STATUS_LABEL, PRODUCT_STATUS_PILL } from '../../types'
import styles from './AdminProducts.module.css'

const EMPTY_FORM = { name: '', description: '', price: '', categoryId: '', imageUrl: '' }

export default function AdminProducts() {
  const qc = useQueryClient()
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState('')
  const [searchInput, setSearchInput] = useState('')
  const [showAdd, setShowAdd] = useState(false)
  const [form, setForm] = useState(EMPTY_FORM)
  const [error, setError] = useState('')

  const { data, isLoading } = useQuery({
    queryKey: ['products', page, search],
    queryFn: () => productsApi.getAll({ page, pageSize: 15, search: search || undefined }),
  })

  const { data: catData } = useQuery({
    queryKey: ['categories'],
    queryFn: () => productsApi.getCategories(),
  })

  const activate   = useMutation({ mutationFn: (id: number) => productsApi.activate(id),   onSuccess: () => qc.invalidateQueries({ queryKey: ['products'] }) })
  const deactivate = useMutation({ mutationFn: (id: number) => productsApi.deactivate(id), onSuccess: () => qc.invalidateQueries({ queryKey: ['products'] }) })
  const archive    = useMutation({ mutationFn: (id: number) => productsApi.archive(id),    onSuccess: () => qc.invalidateQueries({ queryKey: ['products'] }) })

  const createMut = useMutation({
    mutationFn: async () => {
      const res = await productsApi.create({
        name: form.name,
        description: form.description,
        price: Number(form.price),
        categoryId: Number(form.categoryId),
      })
      const newId = (res.data as any)?.data ?? res.data
      if (form.imageUrl && newId) {
        await productsApi.addImage(Number(newId), form.imageUrl)
      }
    },
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['products'] })
      setShowAdd(false)
      setForm(EMPTY_FORM)
      setError('')
    },
    onError: () => setError('خطا در ثبت محصول. لطفاً دوباره تلاش کنید.'),
  })

  const products   = data?.data?.items ?? []
  const total      = data?.data?.totalCount ?? 0
  const totalPages = data?.data?.totalPages ?? 1
  const categories = (catData?.data as any)?.data ?? catData?.data ?? []

  return (
    <>
      <div className={styles.header}>
        <h2>محصولات</h2>
        <div style={{ display: 'flex', gap: 8 }}>
          <input
            placeholder="جستجو..."
            value={searchInput}
            onChange={e => setSearchInput(e.target.value)}
            onKeyDown={e => { if (e.key === 'Enter') { setSearch(searchInput); setPage(1) } }}
            style={{ width: 200 }}
          />
          <button className="btn btn-primary btn-sm" onClick={() => { setSearch(searchInput); setPage(1) }}>
            <i className="ti ti-search" />
          </button>
          <button className="btn btn-primary" onClick={() => { setShowAdd(true); setError('') }}>
            <i className="ti ti-plus" /> محصول جدید
          </button>
        </div>
      </div>

      {/* Modal */}
      {showAdd && (
        <div style={{
          position: 'fixed', inset: 0, background: 'rgba(0,0,0,.5)',
          display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 1000,
        }}>
          <div className="card" style={{ width: 500, maxWidth: '92vw', maxHeight: '90vh', overflowY: 'auto' }}>
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: 20 }}>
              <h3 style={{ margin: 0 }}>افزودن محصول جدید</h3>
              <button className="btn btn-ghost btn-sm" onClick={() => { setShowAdd(false); setError('') }}>
                <i className="ti ti-x" />
              </button>
            </div>

            <div style={{ display: 'flex', flexDirection: 'column', gap: 14 }}>
              <div>
                <label style={{ display: 'block', marginBottom: 4, fontSize: 13, fontWeight: 600 }}>نام محصول *</label>
                <input value={form.name} onChange={e => setForm(f => ({ ...f, name: e.target.value }))}
                  placeholder="مثال: غذای سگ رویال کنین" style={{ width: '100%' }} />
              </div>

              <div>
                <label style={{ display: 'block', marginBottom: 4, fontSize: 13, fontWeight: 600 }}>توضیحات</label>
                <textarea value={form.description} onChange={e => setForm(f => ({ ...f, description: e.target.value }))}
                  rows={3} placeholder="توضیح کوتاه درباره محصول..."
                  style={{ width: '100%', resize: 'vertical', padding: '.6rem .8rem', border: '1.5px solid #E2E8F0', borderRadius: 10, fontFamily: 'inherit', fontSize: '.9rem' }} />
              </div>

              <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 12 }}>
                <div>
                  <label style={{ display: 'block', marginBottom: 4, fontSize: 13, fontWeight: 600 }}>قیمت (تومان) *</label>
                  <input type="number" value={form.price} onChange={e => setForm(f => ({ ...f, price: e.target.value }))}
                    placeholder="مثال: 250000" style={{ width: '100%' }} />
                </div>
                <div>
                  <label style={{ display: 'block', marginBottom: 4, fontSize: 13, fontWeight: 600 }}>دسته‌بندی *</label>
                  <select value={form.categoryId} onChange={e => setForm(f => ({ ...f, categoryId: e.target.value }))}
                    style={{ width: '100%', padding: '.6rem .8rem', border: '1.5px solid #E2E8F0', borderRadius: 10, fontFamily: 'inherit', fontSize: '.9rem', background: '#fff' }}>
                    <option value="">انتخاب دسته‌بندی</option>
                    {categories.map((c: { id: number; name: string }) => (
                      <option key={c.id} value={c.id}>{c.name}</option>
                    ))}
                  </select>
                </div>
              </div>

              <div>
                <label style={{ display: 'block', marginBottom: 4, fontSize: 13, fontWeight: 600 }}>
                  <i className="ti ti-photo" style={{ marginLeft: 4 }} />
                  آدرس تصویر (URL)
                </label>
                <input value={form.imageUrl} onChange={e => setForm(f => ({ ...f, imageUrl: e.target.value }))}
                  placeholder="https://example.com/image.jpg" dir="ltr" style={{ width: '100%' }} />
                {form.imageUrl && (
                  <div style={{ marginTop: 8, borderRadius: 10, overflow: 'hidden', height: 120, background: '#f1f5f9', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                    <img src={form.imageUrl} alt="پیش‌نمایش" style={{ maxHeight: '100%', maxWidth: '100%', objectFit: 'contain' }}
                      onError={e => { (e.target as HTMLImageElement).style.display = 'none' }} />
                  </div>
                )}
              </div>

              {error && <p style={{ color: '#ef4444', fontSize: 13, margin: 0 }}>{error}</p>}

              <div style={{ display: 'flex', gap: 8, justifyContent: 'flex-end', marginTop: 4 }}>
                <button className="btn btn-ghost" onClick={() => { setShowAdd(false); setError('') }}>لغو</button>
                <button className="btn btn-primary"
                  disabled={!form.name || !form.price || !form.categoryId || createMut.isPending}
                  onClick={() => createMut.mutate()}>
                  {createMut.isPending ? 'در حال ثبت...' : 'ثبت محصول'}
                </button>
              </div>
            </div>
          </div>
        </div>
      )}

      <div className="card">
        <div style={{ marginBottom: 14, fontSize: 13, color: 'var(--text2)' }}>
          مجموع: <strong>{total.toLocaleString('fa-IR')}</strong> محصول
        </div>
        {isLoading ? (
          <div className="spinner-center"><div className="spinner" /></div>
        ) : (
          <table>
            <thead>
              <tr>
                <th>شناسه</th>
                <th>نام محصول</th>
                <th>دسته‌بندی</th>
                <th>قیمت (تومان)</th>
                <th>وضعیت</th>
                <th>عملیات</th>
              </tr>
            </thead>
            <tbody>
              {products.map(p => (
                <tr key={p.id}>
                  <td><span style={{ fontWeight: 700, color: 'var(--p)' }}>#{p.id}</span></td>
                  <td style={{ fontWeight: 600 }}>{p.name}</td>
                  <td style={{ color: 'var(--text2)' }}>{p.categoryName}</td>
                  <td style={{ fontWeight: 700 }}>{p.price.toLocaleString('fa-IR')}</td>
                  <td><span className={`pill ${PRODUCT_STATUS_PILL[p.status] ?? 'pill-gray'}`}>{PRODUCT_STATUS_LABEL[p.status] ?? p.status}</span></td>
                  <td>
                    <div style={{ display: 'flex', gap: 4 }}>
                      {p.status !== 0 && <button className="btn btn-ghost btn-sm"  onClick={() => activate.mutate(p.id)}>فعال</button>}
                      {p.status === 0 && <button className="btn btn-danger btn-sm" onClick={() => deactivate.mutate(p.id)}>غیرفعال</button>}
                      {p.status !== 2 && <button className="btn btn-sm" style={{ background: '#F1F5F9', color: '#475569' }} onClick={() => archive.mutate(p.id)}>آرشیو</button>}
                    </div>
                  </td>
                </tr>
              ))}
              {products.length === 0 && (
                <tr><td colSpan={6} style={{ textAlign: 'center', color: 'var(--text3)', padding: 32 }}>محصولی یافت نشد</td></tr>
              )}
            </tbody>
          </table>
        )}

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
