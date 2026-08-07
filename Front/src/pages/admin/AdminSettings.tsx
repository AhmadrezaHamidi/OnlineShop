import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { adminApi } from '../../api/admin'
import Spinner from '../../components/Spinner'

export default function AdminSettings() {
  const qc = useQueryClient()
  const [edited, setEdited] = useState<Record<string, string>>({})
  const [saved, setSaved] = useState(false)

  const { data, isLoading } = useQuery({ queryKey: ['settings'], queryFn: adminApi.settings })

  const bulk = useMutation({
    mutationFn: () => adminApi.bulkSettings(edited),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['settings'] }); setSaved(true); setEdited({}) },
  })

  const settings: { key: string; value: string }[] = data ?? []

  return (
    <div className="space-y-4 max-w-xl">
      <h1 className="text-xl font-bold">تنظیمات</h1>
      {saved && <div className="bg-green-50 border border-green-200 text-green-700 rounded p-3 text-sm">ذخیره شد</div>}
      {isLoading ? <Spinner /> : (
        <div className="card space-y-4">
          {settings.map((s) => (
            <div key={s.key}>
              <label className="block text-sm text-gray-500 mb-1">{s.key}</label>
              <input className="input" defaultValue={s.value}
                onChange={e => setEdited(ed => ({ ...ed, [s.key]: e.target.value }))} dir="ltr" />
            </div>
          ))}
          {settings.length === 0 && <p className="text-gray-400 text-center py-4">تنظیماتی یافت نشد</p>}
          {Object.keys(edited).length > 0 && (
            <button className="btn btn-primary w-full" onClick={() => bulk.mutate()} disabled={bulk.isPending}>
              {bulk.isPending ? 'در حال ذخیره...' : 'ذخیره تغییرات'}
            </button>
          )}
        </div>
      )}
    </div>
  )
}
