import { useQuery } from '@tanstack/react-query'
import { adminApi } from '../../api/admin'
import Spinner from '../../components/Spinner'

export default function AdminDashboard() {
  const { data, isLoading } = useQuery({ queryKey: ['admin-today'], queryFn: adminApi.today })

  const stats = [
    { label: 'نوبت امروز', value: data?.todayCount ?? 0, color: 'bg-teal-500' },
    { label: 'در انتظار', value: data?.pendingCount ?? 0, color: 'bg-amber-500' },
    { label: 'تأیید شده', value: data?.confirmedCount ?? 0, color: 'bg-green-500' },
  ]

  return (
    <div className="space-y-6">
      <h1 className="text-xl font-bold">داشبورد</h1>

      {isLoading ? <Spinner /> : (
        <div className="grid grid-cols-3 gap-4">
          {stats.map(s => (
            <div key={s.label} className={`${s.color} text-white rounded-xl p-5`}>
              <p className="text-3xl font-bold">{s.value}</p>
              <p className="text-sm mt-1 opacity-90">{s.label}</p>
            </div>
          ))}
        </div>
      )}

      {data?.reservations?.length > 0 && (
        <div className="card">
          <h2 className="font-bold mb-3">نوبت‌های امروز</h2>
          <table className="w-full text-sm">
            <thead className="border-b">
              <tr className="text-right text-gray-500">
                <th className="pb-2">بیمار</th>
                <th className="pb-2">ساعت</th>
                <th className="pb-2">وضعیت</th>
              </tr>
            </thead>
            <tbody className="divide-y">
              {data.reservations.map((r: any) => (
                <tr key={r.reservationId}>
                  <td className="py-2">{r.patientName}</td>
                  <td className="py-2">{r.startTime}</td>
                  <td className="py-2">{r.status}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  )
}
