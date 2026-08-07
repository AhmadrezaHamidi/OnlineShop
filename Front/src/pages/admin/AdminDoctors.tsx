import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { adminApi } from '../../api/admin'
import Spinner from '../../components/Spinner'

export default function AdminDoctors() {
  const qc = useQueryClient()
  const [showAdd, setShowAdd] = useState(false)
  const [form, setForm] = useState({ userId: '', specialtyTitle: '', medicalLicenseNumber: '' })

  const { data, isLoading } = useQuery({ queryKey: ['admin-doctors'], queryFn: adminApi.doctors })

  const toggle = useMutation({
    mutationFn: (id: number) => adminApi.toggleDoctor(id),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['admin-doctors'] }),
  })

  const add = useMutation({
    mutationFn: () => adminApi.addDoctor(form),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['admin-doctors'] }); setShowAdd(false) },
  })

  const set = (k: keyof typeof form) => (e: React.ChangeEvent<HTMLInputElement>) =>
    setForm(f => ({ ...f, [k]: e.target.value }))

  return (
    <div className="space-y-4">
      <div className="flex justify-between items-center">
        <h1 className="text-xl font-bold">پزشکان</h1>
        <button className="btn btn-primary" onClick={() => setShowAdd(v => !v)}>+ افزودن پزشک</button>
      </div>

      {showAdd && (
        <div className="card space-y-3">
          <h2 className="font-bold">افزودن پزشک جدید</h2>
          <input className="input" placeholder="شناسه کاربر" type="number" dir="ltr" value={form.userId} onChange={set('userId')} />
          <input className="input" placeholder="تخصص" value={form.specialtyTitle} onChange={set('specialtyTitle')} />
          <input className="input" placeholder="شماره نظام پزشکی" dir="ltr" value={form.medicalLicenseNumber} onChange={set('medicalLicenseNumber')} />
          {add.error && <p className="text-red-600 text-sm">{(add.error as Error).message}</p>}
          <button className="btn btn-primary" onClick={() => add.mutate()} disabled={add.isPending}>ثبت</button>
        </div>
      )}

      {isLoading ? <Spinner /> : (
        <div className="card space-y-3">
          {data?.map((d: any) => (
            <div key={d.id} className="flex justify-between items-center border-b pb-3 last:border-0 last:pb-0">
              <div>
                <p className="font-bold">{d.firstName} {d.lastName}</p>
                <p className="text-sm text-gray-500">{d.specialtyTitle}</p>
              </div>
              <button
                className={`btn text-xs ${d.isActive ? 'border-red-200 text-red-600 hover:bg-red-50' : 'border-green-200 text-green-600 hover:bg-green-50'}`}
                onClick={() => toggle.mutate(d.id)} disabled={toggle.isPending}>
                {d.isActive ? 'غیرفعال' : 'فعال'}
              </button>
            </div>
          ))}
          {data?.length === 0 && <p className="text-gray-400 text-center py-4">پزشکی ثبت نشده</p>}
        </div>
      )}
    </div>
  )
}
