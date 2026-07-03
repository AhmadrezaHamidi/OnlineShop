import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import client, { API_VERSION } from '../../api/client'

interface UserItem {
  id: number
  fullName: string
  phoneNumber: string
  status: string
  role: string
}

export default function AdminUsers() {
  const [page, setPage] = useState(1)

  const { data, isLoading } = useQuery({
    queryKey: ['users', page],
    queryFn: () =>
      client.get<{ data: { items: UserItem[]; totalCount: number; totalPages: number } }>(
        `/api/v${API_VERSION}/Identity/Users`,
        { params: { page, pageSize: 15 } },
      ),
  })

  const users      = data?.data?.data?.items ?? []
  const total      = data?.data?.data?.totalCount ?? 0
  const totalPages = data?.data?.data?.totalPages ?? 1

  const STATUS_PILL: Record<string, string> = {
    Active: 'pill-green', Inactive: 'pill-amber', Suspended: 'pill-red',
  }
  const STATUS_LABEL: Record<string, string> = {
    Active: 'فعال', Inactive: 'غیرفعال', Suspended: 'تعلیق',
  }

  return (
    <>
      <h2 style={{ fontSize: 20, fontWeight: 700 }}>کاربران</h2>

      <div className="card">
        <div style={{ marginBottom: 14, fontSize: 13, color: 'var(--text2)' }}>
          مجموع: <strong>{total.toLocaleString('fa-IR')}</strong> کاربر
        </div>
        {isLoading ? (
          <div className="spinner-center"><div className="spinner" /></div>
        ) : (
          <table>
            <thead>
              <tr>
                <th>شناسه</th>
                <th>نام</th>
                <th>شماره موبایل</th>
                <th>نقش</th>
                <th>وضعیت</th>
              </tr>
            </thead>
            <tbody>
              {users.map(u => (
                <tr key={u.id}>
                  <td><span style={{ fontWeight: 700, color: 'var(--p)' }}>#{u.id}</span></td>
                  <td style={{ fontWeight: 600 }}>{u.fullName}</td>
                  <td dir="ltr" style={{ textAlign: 'right' }}>{u.phoneNumber}</td>
                  <td><span className="pill pill-purple">{u.role}</span></td>
                  <td><span className={`pill ${STATUS_PILL[u.status] ?? 'pill-gray'}`}>{STATUS_LABEL[u.status] ?? u.status}</span></td>
                </tr>
              ))}
              {users.length === 0 && (
                <tr><td colSpan={5} style={{ textAlign: 'center', color: 'var(--text3)', padding: 32 }}>کاربری یافت نشد</td></tr>
              )}
            </tbody>
          </table>
        )}

        {totalPages > 1 && (
          <div style={{ display: 'flex', justifyContent: 'center', gap: 6, marginTop: 16 }}>
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
