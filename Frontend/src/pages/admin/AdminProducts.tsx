import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { productsApi } from '../../api/products'
import type { ProductStatus } from '../../types'
import styles from './AdminProducts.module.css'

const STATUS_PILL: Record<ProductStatus, string> = {
  Active: 'pill-green', Inactive: 'pill-amber', Archived: 'pill-gray',
}
const STATUS_LABEL: Record<ProductStatus, string> = {
  Active: 'فعال', Inactive: 'غیرفعال', Archived: 'آرشیو',
}

export default function AdminProducts() {
  const qc = useQueryClient()
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState('')
  const [searchInput, setSearchInput] = useState('')

  const { data, isLoading } = useQuery({
    queryKey: ['products', page, search],
    queryFn: () => productsApi.getAll({ page, pageSize: 15, search: search || undefined }),
  })

  const activate   = useMutation({ mutationFn: (id: number) => productsApi.activate(id),   onSuccess: () => qc.invalidateQueries({ queryKey: ['products'] }) })
  const deactivate = useMutation({ mutationFn: (id: number) => productsApi.deactivate(id), onSuccess: () => qc.invalidateQueries({ queryKey: ['products'] }) })
  const archive    = useMutation({ mutationFn: (id: number) => productsApi.archive(id),    onSuccess: () => qc.invalidateQueries({ queryKey: ['products'] }) })

  const products   = data?.data?.data?.items ?? []
  const total      = data?.data?.data?.totalCount ?? 0
  const totalPages = data?.data?.data?.totalPages ?? 1

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
            <i className="ti ti-search" /> جستجو
          </button>
        </div>
      </div>

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
                  <td><span className={`pill ${STATUS_PILL[p.status]}`}>{STATUS_LABEL[p.status]}</span></td>
                  <td>
                    <div style={{ display: 'flex', gap: 4 }}>
                      {p.status !== 'Active'    && <button className="btn btn-ghost btn-sm"  onClick={() => activate.mutate(p.id)}>فعال</button>}
                      {p.status === 'Active'    && <button className="btn btn-danger btn-sm" onClick={() => deactivate.mutate(p.id)}>غیرفعال</button>}
                      {p.status !== 'Archived'  && <button className="btn btn-sm" style={{ background: '#F1F5F9', color: '#475569' }} onClick={() => archive.mutate(p.id)}>آرشیو</button>}
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
