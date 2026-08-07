import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { patientApi, Reservation } from '../../api/patient'
import Spinner from '../../components/Spinner'
import ErrorMessage from '../../components/ErrorMessage'

const STATUS_LABELS: Record<string, string> = {
  Pending: 'در انتظار', Confirmed: 'تأیید شده', Cancelled: 'لغو شده', Completed: 'انجام شده'
}
const STATUS_BADGE: Record<string, string> = {
  Pending: 'badge-warning', Confirmed: 'badge-success', Cancelled: 'badge-danger', Completed: 'badge-info'
}

export default function PatientAppointments() {
  const qc = useQueryClient()
  const [status, setStatus] = useState('')

  const { data, isLoading, error } = useQuery({
    queryKey: ['my-reservations', status],
    queryFn: () => patientApi.myReservations(status ? { status } : undefined),
  })

  const cancel = useMutation({
    mutationFn: (id: number) => patientApi.cancel(id),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['my-reservations'] }),
  })

  return (
    <div className="space-y-4">
      <div className="flex items-center justify-between">
        <h1 className="text-xl font-bold">نوبت‌هایم</h1>
        <select className="input w-40" value={status} onChange={e => setStatus(e.target.value)}>
          <option value="">همه</option>
          {Object.entries(STATUS_LABELS).map(([k, v]) => <option key={k} value={k}>{v}</option>)}
        </select>
      </div>

      {isLoading && <Spinner />}
      {error && <ErrorMessage message={(error as Error).message} />}

      {data?.length === 0 && <p className="text-gray-400 text-center py-8">نوبتی یافت نشد</p>}

      <div className="space-y-3">
        {data?.map((r: Reservation) => (
          <div key={r.id} className="card flex justify-between items-start">
            <div className="space-y-1">
              <p className="font-bold">{r.specialtyTitle}</p>
              <p className="text-sm text-gray-500">{r.reservationDate} از {r.startTime} تا {r.endTime}</p>
              {r.doctorName && <p className="text-sm text-gray-500">دکتر {r.doctorName}</p>}
            </div>
            <div className="flex flex-col items-end gap-2">
              <span className={`badge ${STATUS_BADGE[r.status]}`}>{STATUS_LABELS[r.status]}</span>
              {(r.status === 'Pending' || r.status === 'Confirmed') && (
                <button className="btn text-xs text-red-600 border border-red-200 hover:bg-red-50"
                  onClick={() => cancel.mutate(r.id)} disabled={cancel.isPending}>
                  لغو نوبت
                </button>
              )}
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}
