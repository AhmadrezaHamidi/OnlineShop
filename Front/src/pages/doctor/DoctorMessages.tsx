import { useState } from 'react'
import { useMutation } from '@tanstack/react-query'
import { doctorApi } from '../../api/doctor'

export default function DoctorMessages() {
  const [patientId, setPatientId] = useState('')
  const [message, setMessage] = useState('')
  const [success, setSuccess] = useState(false)

  const notify = useMutation({
    mutationFn: () => doctorApi.notify(+patientId, message),
    onSuccess: () => { setSuccess(true); setMessage('') },
  })

  return (
    <div className="space-y-6 max-w-lg">
      <h1 className="text-xl font-bold">ارسال پیام به بیمار</h1>
      {success && <div className="bg-green-50 border border-green-200 text-green-700 rounded p-3 text-sm">پیام ارسال شد</div>}
      <div className="card space-y-4">
        <div>
          <label className="block text-sm mb-1">شناسه بیمار</label>
          <input className="input" type="number" value={patientId} onChange={e => setPatientId(e.target.value)} dir="ltr" />
        </div>
        <div>
          <label className="block text-sm mb-1">متن پیام</label>
          <textarea className="input h-32 resize-none" value={message} onChange={e => setMessage(e.target.value)} />
        </div>
        {notify.error && <p className="text-red-600 text-sm">{(notify.error as Error).message}</p>}
        <button className="btn btn-primary w-full" disabled={notify.isPending || !patientId || !message}
          onClick={() => notify.mutate()}>
          {notify.isPending ? 'در حال ارسال...' : 'ارسال پیام (Bale + SMS)'}
        </button>
      </div>
    </div>
  )
}
