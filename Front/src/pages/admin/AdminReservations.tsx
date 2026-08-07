import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { adminApi, ReservationStatusKey } from '../../api/admin'
import Spinner from '../../components/Spinner'

const STATUSES = ['', 'Pending', 'Confirmed', 'Cancelled', 'Completed']
const STATUS_LABELS: Record<string, string> = {
  '': 'همه', Pending: 'در انتظار', Confirmed: 'تأیید', Cancelled: 'لغو', Completed: 'انجام شده'
}

export default function AdminReservations() {
  const qc = useQueryClient()
  const [status, setStatus] = useState('')

  const { data, isLoading } = useQuery({
    queryKey: ['admin-reservations', status],
    queryFn: () => adminApi.reservations(status ? { status } : {}),
  })

  const update = useMutation({
    mutationFn: ({ id, status }: { id: number; status: ReservationStatusKey }) => adminApi.updateStatus(id, status),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['admin-reservations'] }),
  })

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h1 className="text-xl font-bold">نوبت‌ها</h1>
        <select className="input w-40" value={status} onChange={e => setStatus(e.target.value)}>
          {STATUSES.map(s => <option key={s} value={s}>{STATUS_LABELS[s]}</option>)}
        </select>
      </div>

      {isLoading ? <Spinner /> : (
        <div className="card overflow-x-auto">
          <table className="w-full text-sm">
            <thead className="border-b">
              <tr className="text-right text-gray-500">
                <th className="pb-2 px-2">بیمار</th>
                <th className="pb-2 px-2">تاریخ</th>
                <th className="pb-2 px-2">ساعت</th>
                <th className="pb-2 px-2">وضعیت</th>
                <th className="pb-2 px-2">عملیات</th>
              </tr>
            </thead>
            <tbody className="divide-y">
              {data?.map((r: any) => (
                <tr key={r.id} className="hover:bg-gray-50">
                  <td className="py-2 px-2">{r.patientName}</td>
                  <td className="py-2 px-2">{r.reservationDate}</td>
                  <td className="py-2 px-2">{r.startTime}</td>
                  <td className="py-2 px-2">{STATUS_LABELS[r.status] ?? r.status}</td>
                  <td className="py-2 px-2">
                    {r.status === 'Pending' && (
                      <div className="flex gap-1">
                        <button className="text-xs text-green-600 hover:underline"
                          onClick={() => update.mutate({ id: r.id, status: 'Confirmed' })}>تأیید</button>
                        <button className="text-xs text-red-600 hover:underline"
                          onClick={() => update.mutate({ id: r.id, status: 'Cancelled' })}>لغو</button>
                      </div>
                    )}
                  </td>
                </tr>
              ))}
              {data?.length === 0 && (
                <tr><td colSpan={5} className="py-6 text-center text-gray-400">نوبتی یافت نشد</td></tr>
              )}
            </tbody>
          </table>
        </div>
      )}
    </div>
  )
}
