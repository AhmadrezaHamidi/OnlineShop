import { useQuery } from '@tanstack/react-query'
import { Link } from 'react-router-dom'
import { patientApi } from '../../api/patient'
import { useAuth } from '../../store/auth'
import Spinner from '../../components/Spinner'

export default function PatientHome() {
  const { user } = useAuth()
  const { data: reservations, isLoading } = useQuery({
    queryKey: ['my-reservations'],
    queryFn: () => patientApi.myReservations({ status: 'Confirmed' }),
  })

  return (
    <div className="space-y-6">
      <h1 className="text-xl font-bold text-gray-800">سلام، {user?.firstName} عزیز</h1>

      <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
        {[
          { label: 'رزرو نوبت', to: '/patient/book', color: 'bg-teal-500' },
          { label: 'نوبت‌هایم', to: '/patient/appointments', color: 'bg-amber-500' },
          { label: 'نسخه‌ها', to: '/patient/prescriptions', color: 'bg-coral-500 bg-red-400' },
          { label: 'پروفایل', to: '/patient/profile', color: 'bg-gray-500' },
        ].map(c => (
          <Link key={c.to} to={c.to}
            className={`${c.color} text-white rounded-xl p-5 text-center font-bold hover:opacity-90 transition`}>
            {c.label}
          </Link>
        ))}
      </div>

      <div className="card">
        <h2 className="font-bold text-gray-700 mb-4">نوبت‌های تأیید شده</h2>
        {isLoading ? <Spinner /> : !reservations?.length ? (
          <p className="text-gray-400 text-sm text-center py-4">نوبتی ندارید</p>
        ) : (
          <ul className="divide-y">
            {reservations.map((r: any) => (
              <li key={r.id} className="py-3 flex justify-between items-center">
                <div>
                  <p className="font-medium">{r.specialtyTitle}</p>
                  <p className="text-sm text-gray-500">{r.reservationDate} — {r.startTime}</p>
                </div>
                <span className="badge badge-success">تأیید شده</span>
              </li>
            ))}
          </ul>
        )}
      </div>
    </div>
  )
}
