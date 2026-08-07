import { useQuery } from '@tanstack/react-query'
import { doctorApi, ScheduleDay, TodayPatient } from '../../api/doctor'
import Spinner from '../../components/Spinner'

export default function DoctorSchedule() {
  const { data, isLoading } = useQuery({ queryKey: ['doctor-schedule'], queryFn: () => doctorApi.schedule() })

  return (
    <div className="space-y-4">
      <h1 className="text-xl font-bold">برنامه هفتگی</h1>
      {isLoading ? <Spinner /> : !data?.length ? (
        <p className="text-gray-400 text-center py-8">برنامه‌ای ثبت نشده</p>
      ) : (
        data.map((day: ScheduleDay) => (
          <div key={day.date} className="card">
            <div className="flex justify-between items-center mb-3">
              <h2 className="font-bold">{day.dayName}</h2>
              <span className="text-sm text-gray-400">{day.date}</span>
            </div>
            {!day.reservations.length ? (
              <p className="text-sm text-gray-400">نوبتی ندارید</p>
            ) : (
              <ul className="divide-y">
                {day.reservations.map((r: TodayPatient) => (
                  <li key={r.reservationId} className="py-2 flex justify-between text-sm">
                    <span>{r.patientName}</span>
                    <span className="text-gray-500">{r.startTime}</span>
                  </li>
                ))}
              </ul>
            )}
          </div>
        ))
      )}
    </div>
  )
}
