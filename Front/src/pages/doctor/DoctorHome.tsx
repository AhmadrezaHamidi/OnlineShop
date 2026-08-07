import { useQuery } from '@tanstack/react-query'
import { doctorApi, TodayPatient } from '../../api/doctor'
import Spinner from '../../components/Spinner'

const STATUS_LABEL: Record<string, string> = {
  Pending: 'در انتظار', Confirmed: 'تأیید', Completed: 'انجام شده', Cancelled: 'لغو'
}

export default function DoctorHome() {
  const { data, isLoading } = useQuery({ queryKey: ['doctor-today'], queryFn: doctorApi.today })

  return (
    <div className="space-y-4">
      <h1 className="text-xl font-bold">بیماران امروز</h1>
      {isLoading ? <Spinner /> : !data?.length ? (
        <p className="text-gray-400 text-center py-8">امروز نوبتی ندارید</p>
      ) : (
        <div className="space-y-3">
          {data.map((p: TodayPatient) => (
            <div key={p.reservationId} className="card flex justify-between items-center">
              <div>
                <p className="font-bold">{p.patientName}</p>
                <p className="text-sm text-gray-500" dir="ltr">{p.phone}</p>
                <p className="text-sm text-teal-600">{p.startTime} — {p.endTime}</p>
              </div>
              <span className="badge badge-info">{STATUS_LABEL[p.status] ?? p.status}</span>
            </div>
          ))}
        </div>
      )}
    </div>
  )
}
