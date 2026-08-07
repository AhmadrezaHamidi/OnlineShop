import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { adminUsersApi, type AdminUser } from '../../../api/shop/adminUsers'

export default function AdminUsers() {
  const [search, setSearch]     = useState('')
  const [selected, setSelected] = useState<AdminUser | null>(null)
  const qc = useQueryClient()

  const { data: users = [], isLoading } = useQuery({
    queryKey: ['admin-users'],
    queryFn: adminUsersApi.list,
  })

  const mut = useMutation({
    mutationFn: ({ id, action }: { id: number; action: 'activate' | 'deactivate' | 'suspend' }) => {
      if (action === 'activate')   return adminUsersApi.activate(id)
      if (action === 'deactivate') return adminUsersApi.deactivate(id)
      return adminUsersApi.suspend(id)
    },
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['admin-users'] })
      setSelected(null)
    },
  })

  const visible = users.filter((u: AdminUser) =>
    !search ||
    u.phoneNumber.includes(search) ||
    u.firstName?.includes(search) ||
    u.lastName?.includes(search)
  )

  if (isLoading) return (
    <div className="flex justify-center py-20 text-ink-300">
      <span className="text-3xl animate-spin">⏳</span>
    </div>
  )

  return (
    <div className="space-y-5">
      <h1 className="text-xl font-extrabold text-ink-900">مدیریت کاربران</h1>

      <div className="grid grid-cols-3 gap-3">
        {[
          { label: 'کل کاربران',  value: users.length,                                       cls: 'bg-pet-50   text-pet-600'   },
          { label: 'فعال',        value: users.filter((u: AdminUser) => u.isActive && !u.isSuspended).length,  cls: 'bg-forest-50 text-forest-600'},
          { label: 'معلق',        value: users.filter((u: AdminUser) => u.isSuspended).length,                cls: 'bg-honey-50  text-honey-600' },
        ].map(s => (
          <div key={s.label} className={`rounded-2xl p-4 ${s.cls}`}>
            <div className="text-2xl font-extrabold">{s.value}</div>
            <div className="text-xs opacity-70 mt-0.5">{s.label}</div>
          </div>
        ))}
      </div>

      <input
        value={search}
        onChange={e => setSearch(e.target.value)}
        placeholder="جستجو نام یا شماره موبایل…"
        className="w-full px-4 py-2.5 rounded-xl border border-ink-100 bg-white text-sm outline-none focus:border-pet-400 focus:ring-2 focus:ring-pet-100 transition-all"
        dir="rtl"
      />

      <div className="bg-white rounded-2xl border border-ink-100 shadow-sm overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full text-sm" dir="rtl">
            <thead>
              <tr className="border-b border-ink-100 bg-ink-900/5">
                {['نام','موبایل','نقش‌ها','وضعیت',''].map(h => (
                  <th key={h} className="text-right px-4 py-3 text-xs font-semibold text-ink-400 whitespace-nowrap">{h}</th>
                ))}
              </tr>
            </thead>
            <tbody className="divide-y divide-ink-100">
              {visible.map((u: AdminUser) => (
                <tr key={u.id} className="hover:bg-pet-50/30 transition-colors">
                  <td className="px-4 py-3 font-semibold text-ink-900 whitespace-nowrap">
                    {u.firstName} {u.lastName}
                  </td>
                  <td className="px-4 py-3 text-ink-500 whitespace-nowrap">{u.phoneNumber}</td>
                  <td className="px-4 py-3">
                    <div className="flex flex-wrap gap-1">
                      {u.roles.map(r => (
                        <span key={r} className="px-2 py-0.5 bg-pet-50 text-pet-600 rounded-full text-xs font-semibold">
                          {r}
                        </span>
                      ))}
                    </div>
                  </td>
                  <td className="px-4 py-3 whitespace-nowrap">
                    {u.isSuspended ? (
                      <span className="inline-flex items-center gap-1 px-2.5 py-1 rounded-full text-xs font-semibold bg-honey-50 text-honey-600">
                        معلق
                      </span>
                    ) : u.isActive ? (
                      <span className="inline-flex items-center gap-1 px-2.5 py-1 rounded-full text-xs font-semibold bg-forest-50 text-forest-600">
                        فعال
                      </span>
                    ) : (
                      <span className="inline-flex items-center gap-1 px-2.5 py-1 rounded-full text-xs font-semibold bg-red-50 text-red-500">
                        غیرفعال
                      </span>
                    )}
                  </td>
                  <td className="px-4 py-3">
                    <button
                      onClick={() => setSelected(u)}
                      className="text-xs text-pet-500 hover:underline font-semibold whitespace-nowrap"
                    >
                      مدیریت
                    </button>
                  </td>
                </tr>
              ))}
              {visible.length === 0 && (
                <tr><td colSpan={5} className="text-center py-12 text-ink-300 text-sm">کاربری یافت نشد</td></tr>
              )}
            </tbody>
          </table>
        </div>
      </div>

      {selected && (
        <div className="fixed inset-0 bg-black/40 z-50 flex items-center justify-center p-4" onClick={() => setSelected(null)}>
          <div className="bg-white rounded-2xl shadow-xl w-full max-w-sm" onClick={e => e.stopPropagation()} dir="rtl">
            <div className="flex items-center justify-between px-5 py-4 border-b border-ink-100">
              <h3 className="font-extrabold text-ink-900">{selected.firstName} {selected.lastName}</h3>
              <button onClick={() => setSelected(null)} className="text-ink-300 hover:text-ink-500 text-lg">✕</button>
            </div>
            <div className="px-5 py-4 space-y-2 text-sm">
              <div className="flex justify-between">
                <span className="text-ink-400">موبایل</span>
                <span className="font-semibold">{selected.phoneNumber}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-ink-400">نقش‌ها</span>
                <span className="font-semibold">{selected.roles.join('، ')}</span>
              </div>
              <div className="flex justify-between">
                <span className="text-ink-400">وضعیت</span>
                <span className="font-semibold">
                  {selected.isSuspended ? 'معلق' : selected.isActive ? 'فعال' : 'غیرفعال'}
                </span>
              </div>
            </div>
            <div className="px-5 pb-5 space-y-2">
              {!selected.isActive && !selected.isSuspended && (
                <button
                  onClick={() => mut.mutate({ id: selected.id, action: 'activate' })}
                  disabled={mut.isPending}
                  className="w-full py-2.5 rounded-xl bg-forest-500 text-white text-sm font-semibold hover:bg-forest-600 disabled:opacity-50 transition-colors"
                >
                  فعال‌سازی
                </button>
              )}
              {selected.isActive && !selected.isSuspended && (
                <button
                  onClick={() => mut.mutate({ id: selected.id, action: 'deactivate' })}
                  disabled={mut.isPending}
                  className="w-full py-2.5 rounded-xl border border-ink-200 text-ink-500 text-sm font-semibold hover:bg-ink-50 disabled:opacity-50 transition-colors"
                >
                  غیرفعال‌سازی
                </button>
              )}
              {!selected.isSuspended && (
                <button
                  onClick={() => mut.mutate({ id: selected.id, action: 'suspend' })}
                  disabled={mut.isPending}
                  className="w-full py-2.5 rounded-xl border border-red-200 text-red-500 text-sm font-semibold hover:bg-red-50 disabled:opacity-50 transition-colors"
                >
                  تعلیق کاربر
                </button>
              )}
            </div>
          </div>
        </div>
      )}
    </div>
  )
}
