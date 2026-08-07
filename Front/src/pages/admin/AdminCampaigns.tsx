import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { adminApi } from '../../api/admin'
import Spinner from '../../components/Spinner'

export default function AdminCampaigns() {
  const qc = useQueryClient()
  const [form, setForm] = useState({ title: '', message: '' })
  const [showAdd, setShowAdd] = useState(false)

  const { data, isLoading } = useQuery({ queryKey: ['campaigns'], queryFn: adminApi.campaigns })

  const create = useMutation({
    mutationFn: () => adminApi.createCampaign(form),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['campaigns'] }); setShowAdd(false) },
  })

  const send = useMutation({
    mutationFn: (id: number) => adminApi.sendCampaign(id),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['campaigns'] }),
  })

  return (
    <div className="space-y-4">
      <div className="flex justify-between items-center">
        <h1 className="text-xl font-bold">کمپین‌های پیامکی</h1>
        <button className="btn btn-primary" onClick={() => setShowAdd(v => !v)}>+ کمپین جدید</button>
      </div>

      {showAdd && (
        <div className="card space-y-3">
          <input className="input" placeholder="عنوان کمپین" value={form.title} onChange={e => setForm(f => ({ ...f, title: e.target.value }))} />
          <textarea className="input h-24 resize-none" placeholder="متن پیام" value={form.message} onChange={e => setForm(f => ({ ...f, message: e.target.value }))} />
          <button className="btn btn-primary" onClick={() => create.mutate()} disabled={create.isPending}>ذخیره</button>
        </div>
      )}

      {isLoading ? <Spinner /> : (
        <div className="space-y-3">
          {data?.map((c: any) => (
            <div key={c.id} className="card flex justify-between items-start">
              <div>
                <p className="font-bold">{c.title}</p>
                <p className="text-sm text-gray-500 mt-1">{c.message}</p>
              </div>
              <button className="btn btn-primary text-xs" onClick={() => send.mutate(c.id)} disabled={send.isPending}>
                ارسال
              </button>
            </div>
          ))}
          {data?.length === 0 && <p className="text-gray-400 text-center py-6">کمپینی ثبت نشده</p>}
        </div>
      )}
    </div>
  )
}
