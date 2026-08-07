import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { adminApi } from '../../api/admin'
import Spinner from '../../components/Spinner'

export default function AdminPatients() {
  const [q, setQ] = useState('')
  const [search, setSearch] = useState('')

  const { data, isLoading } = useQuery({
    queryKey: ['admin-patients', search],
    queryFn: () => adminApi.patients(search || undefined),
  })

  return (
    <div className="space-y-4">
      <h1 className="text-xl font-bold">بیماران</h1>
      <form onSubmit={e => { e.preventDefault(); setSearch(q) }} className="flex gap-2">
        <input className="input flex-1" placeholder="جستجو..." value={q} onChange={e => setQ(e.target.value)} />
        <button className="btn btn-primary px-6">جستجو</button>
      </form>

      {isLoading ? <Spinner /> : (
        <div className="card overflow-x-auto">
          <table className="w-full text-sm">
            <thead className="border-b">
              <tr className="text-right text-gray-500">
                <th className="pb-2 px-2">نام</th>
                <th className="pb-2 px-2">موبایل</th>
                <th className="pb-2 px-2">کد ملی</th>
                <th className="pb-2 px-2">تعداد نوبت</th>
              </tr>
            </thead>
            <tbody className="divide-y">
              {data?.map((p: any) => (
                <tr key={p.id} className="hover:bg-gray-50">
                  <td className="py-2 px-2">{p.firstName} {p.lastName}</td>
                  <td className="py-2 px-2" dir="ltr">{p.phoneNumber}</td>
                  <td className="py-2 px-2">{p.nationalCode ?? '—'}</td>
                  <td className="py-2 px-2">{p.reservationCount ?? 0}</td>
                </tr>
              ))}
              {data?.length === 0 && (
                <tr><td colSpan={4} className="py-6 text-center text-gray-400">بیماری یافت نشد</td></tr>
              )}
            </tbody>
          </table>
        </div>
      )}
    </div>
  )
}
