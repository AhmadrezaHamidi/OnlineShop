import { useState } from 'react'
import { useMutation, useQueryClient } from '@tanstack/react-query'
import { doctorApi } from '../../api/doctor'
import PersianDatePicker from '../../components/PersianDatePicker'

export default function DoctorPrescriptions() {
  const qc = useQueryClient()
  const [patientId, setPatientId] = useState('')
  const [nextVisitDate, setNextVisitDate] = useState('')
  const [form, setForm] = useState({ content: '', diagnosis: '', notes: '' })
  const [success, setSuccess] = useState(false)

  const write = useMutation({
    mutationFn: () => doctorApi.writePrescription(+patientId, { ...form, nextVisitDate: nextVisitDate || undefined }),
    onSuccess: () => { setSuccess(true); setForm({ content: '', diagnosis: '', notes: '' }) },
  })

  const set = (k: keyof typeof form) => (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) =>
    setForm(f => ({ ...f, [k]: e.target.value }))

  return (
    <div className="space-y-6 max-w-lg">
      <h1 className="text-xl font-bold">صدور نسخه</h1>
      {success && <div className="bg-green-50 border border-green-200 text-green-700 rounded p-3 text-sm">نسخه ثبت شد</div>}
      <div className="card space-y-4">
        <div>
          <label className="block text-sm mb-1">شناسه بیمار</label>
          <input className="input" type="number" value={patientId} onChange={e => setPatientId(e.target.value)} dir="ltr" />
        </div>
        <div>
          <label className="block text-sm mb-1">تشخیص</label>
          <input className="input" value={form.diagnosis} onChange={set('diagnosis')} />
        </div>
        <div>
          <label className="block text-sm mb-1">متن نسخه</label>
          <textarea className="input h-32 resize-none" value={form.content} onChange={set('content')} required />
        </div>
        <div>
          <label className="block text-sm mb-1">توضیحات</label>
          <textarea className="input h-20 resize-none" value={form.notes} onChange={set('notes')} />
        </div>
        <div>
          <label className="block text-sm mb-1">تاریخ ویزیت بعدی</label>
          <PersianDatePicker value={nextVisitDate} onChange={setNextVisitDate} placeholder="اختیاری" />
        </div>
        {write.error && <p className="text-red-600 text-sm">{(write.error as Error).message}</p>}
        <button className="btn btn-primary w-full" disabled={write.isPending || !patientId || !form.content}
          onClick={() => write.mutate()}>
          {write.isPending ? 'در حال ثبت...' : 'ثبت نسخه'}
        </button>
      </div>
    </div>
  )
}
