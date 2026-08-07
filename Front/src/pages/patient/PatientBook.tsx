import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { patientApi, Specialty, TimeSlot } from '../../api/patient'
import Spinner from '../../components/Spinner'
import PersianDatePicker from '../../components/PersianDatePicker'

export default function PatientBook() {
  const qc = useQueryClient()
  const today = new Date().toISOString().split('T')[0]
  const [specialtyId, setSpecialtyId] = useState<number | null>(null)
  const [date, setDate] = useState(today)
  const [selected, setSelected] = useState<TimeSlot | null>(null)
  const [success, setSuccess] = useState(false)

  const { data: specialties } = useQuery({
    queryKey: ['specialties'],
    queryFn: patientApi.specialties,
  })

  const { data: slots, isLoading: loadingSlots } = useQuery({
    queryKey: ['slots', date, specialtyId],
    queryFn: () => patientApi.availableTimes(date, specialtyId!),
    enabled: !!specialtyId,
  })

  const book = useMutation({
    mutationFn: () => patientApi.book({
      specialtyId: specialtyId!,
      reservationDate: date,
      startTime: selected!.startTime,
      endTime: selected!.endTime,
    }),
    onSuccess: () => {
      setSuccess(true)
      setSelected(null)
      qc.invalidateQueries({ queryKey: ['my-reservations'] })
    },
  })

  return (
    <div className="space-y-6 max-w-lg">
      <h1 className="text-xl font-bold">رزرو نوبت</h1>

      {success && (
        <div className="bg-green-50 border border-green-200 text-green-700 rounded-lg p-4">
          نوبت با موفقیت رزرو شد!
        </div>
      )}

      <div className="card space-y-4">
        <div>
          <label className="block text-sm mb-1">تخصص</label>
          <select className="input" value={specialtyId ?? ''} onChange={e => { setSpecialtyId(+e.target.value); setSelected(null) }}>
            <option value="">انتخاب کنید</option>
            {specialties?.map((s: Specialty) => <option key={s.id} value={s.id}>{s.name}</option>)}
          </select>
        </div>
        <div>
          <label className="block text-sm mb-1">تاریخ</label>
          <PersianDatePicker
            value={date}
            minDate={today}
            onChange={v => { setDate(v); setSelected(null) }}
          />
        </div>
      </div>

      {specialtyId && (
        <div className="card">
          <h2 className="font-bold mb-3">انتخاب ساعت</h2>
          {loadingSlots ? <Spinner /> : (
            <div className="grid grid-cols-3 gap-2">
              {slots?.map((slot: TimeSlot) => (
                <button key={slot.startTime}
                  disabled={!slot.isAvailable}
                  onClick={() => setSelected(slot)}
                  className={`rounded-lg border py-2 text-sm transition
                    ${!slot.isAvailable ? 'opacity-40 cursor-not-allowed bg-gray-100' :
                      selected?.startTime === slot.startTime ? 'bg-teal-600 text-white border-teal-600' :
                        'hover:border-teal-400'}`}>
                  {slot.startTime}
                </button>
              ))}
              {slots?.length === 0 && <p className="col-span-3 text-gray-400 text-center py-4">وقتی خالی نیست</p>}
            </div>
          )}
        </div>
      )}

      {selected && (
        <button className="btn btn-primary w-full" onClick={() => book.mutate()} disabled={book.isPending}>
          {book.isPending ? 'در حال رزرو...' : `رزرو ساعت ${selected.startTime}`}
        </button>
      )}
      {book.error && <p className="text-red-600 text-sm">{(book.error as Error).message}</p>}
    </div>
  )
}
