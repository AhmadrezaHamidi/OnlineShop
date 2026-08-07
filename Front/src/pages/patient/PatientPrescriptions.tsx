import { useQuery } from '@tanstack/react-query'
import { patientApi, Prescription } from '../../api/patient'
import Spinner from '../../components/Spinner'

export default function PatientPrescriptions() {
  const { data, isLoading } = useQuery({ queryKey: ['prescriptions'], queryFn: patientApi.prescriptions })

  return (
    <div className="space-y-4">
      <h1 className="text-xl font-bold">نسخه‌های من</h1>
      {isLoading ? <Spinner /> : !data?.length ? (
        <p className="text-gray-400 text-center py-8">نسخه‌ای ثبت نشده</p>
      ) : (
        data.map((p: Prescription) => (
          <div key={p.id} className="card space-y-2">
            <div className="flex justify-between text-sm text-gray-500">
              <span>تاریخ صدور: {p.issuedAt.split('T')[0]}</span>
              {p.nextVisitDate && <span>ویزیت بعدی: {p.nextVisitDate.split('T')[0]}</span>}
            </div>
            {p.diagnosis && <p className="font-medium">تشخیص: {p.diagnosis}</p>}
            <p className="bg-gray-50 rounded p-3 text-sm whitespace-pre-line">{p.content}</p>
            {p.notes && <p className="text-sm text-gray-500">توضیحات: {p.notes}</p>}
          </div>
        ))
      )}
    </div>
  )
}
