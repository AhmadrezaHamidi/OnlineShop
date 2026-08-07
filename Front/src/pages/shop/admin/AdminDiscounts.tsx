import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { discountsApi, type Discount } from '../../../api/shop/discounts'

interface DiscountForm { code: string; type: 'Percentage' | 'Fixed'; value: number; minOrderAmount: number; maxUsage: number }
const emptyForm: DiscountForm = { code: '', type: 'Percentage', value: 0, minOrderAmount: 0, maxUsage: 0 }

export default function AdminDiscounts() {
  const [showForm, setShowForm] = useState(false)
  const [form, setForm]         = useState<DiscountForm>(emptyForm)
  const qc = useQueryClient()

  const { data: discounts = [], isLoading } = useQuery({
    queryKey: ['discounts'],
    queryFn: discountsApi.list,
  })

  const createMut = useMutation({
    mutationFn: () => discountsApi.create(form),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['discounts'] })
      setForm(emptyForm)
      setShowForm(false)
    },
  })

  const toggleMut = useMutation({
    mutationFn: ({ id, active }: { id: number; active: boolean }) =>
      active ? discountsApi.deactivate(id) : discountsApi.activate(id),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['discounts'] }),
  })

  if (isLoading) return (
    <div className="flex justify-center py-20 text-ink-300">
      <span className="text-3xl animate-spin">⏳</span>
    </div>
  )

  return (
    <div className="space-y-5">
      <div className="flex items-center justify-between">
        <h1 className="text-xl font-extrabold text-ink-900">تخفیف‌ها</h1>
        <button
          onClick={() => setShowForm(v => !v)}
          className="px-4 py-2 rounded-xl bg-pet-500 text-white text-sm font-semibold hover:bg-pet-600 transition-colors"
        >
          + تخفیف جدید
        </button>
      </div>

      {showForm && (
        <div className="bg-white rounded-2xl border border-pet-200 shadow-sm p-4 space-y-3" dir="rtl">
          <h2 className="font-bold text-ink-900 text-sm">ایجاد کد تخفیف</h2>
          {[
            { key: 'code',           label: 'کد تخفیف',        type: 'text'   },
            { key: 'value',          label: 'مقدار',             type: 'number' },
            { key: 'minOrderAmount', label: 'حداقل مبلغ سفارش', type: 'number' },
            { key: 'maxUsage',       label: 'حداکثر استفاده',   type: 'number' },
          ].map(({ key, label, type }) => (
            <div key={key}>
              <label className="block text-xs text-ink-400 mb-1">{label}</label>
              <input
                type={type}
                value={(form as unknown as Record<string, string | number>)[key] as string}
                onChange={e => setForm(p => ({ ...p, [key]: type === 'number' ? +e.target.value : e.target.value }))}
                className="w-full px-3.5 py-2.5 rounded-xl border border-ink-100 text-sm outline-none focus:border-pet-400 focus:ring-2 focus:ring-pet-100 transition-all"
              />
            </div>
          ))}
          <div>
            <label className="block text-xs text-ink-400 mb-1">نوع تخفیف</label>
            <select
              value={form.type}
              onChange={e => setForm(p => ({ ...p, type: e.target.value as 'Percentage' | 'Fixed' }))}
              className="w-full px-3.5 py-2.5 rounded-xl border border-ink-100 text-sm outline-none focus:border-pet-400 transition-all"
            >
              <option value="Percentage">درصدی</option>
              <option value="Fixed">مبلغ ثابت</option>
            </select>
          </div>
          <div className="flex gap-2 pt-1">
            <button
              onClick={() => createMut.mutate()}
              disabled={createMut.isPending || !form.code}
              className="flex-1 py-2.5 rounded-xl bg-pet-500 text-white text-sm font-semibold hover:bg-pet-600 disabled:opacity-50 transition-colors"
            >
              {createMut.isPending ? '…' : 'ذخیره'}
            </button>
            <button
              onClick={() => setShowForm(false)}
              className="px-4 py-2.5 rounded-xl border border-ink-100 text-ink-500 text-sm font-semibold hover:bg-ink-50 transition-colors"
            >
              لغو
            </button>
          </div>
        </div>
      )}

      <div className="space-y-3">
        {discounts.length === 0 && (
          <div className="text-center py-16 text-ink-300">
            <div className="text-5xl mb-3">🏷️</div>
            <p className="text-sm">کد تخفیفی ثبت نشده</p>
          </div>
        )}
        {discounts.map((d: Discount) => (
          <div key={d.id} className="bg-white rounded-2xl border border-ink-100 shadow-sm px-4 py-3 flex items-center justify-between">
            <div>
              <div className="flex items-center gap-2">
                <span className="font-extrabold text-ink-900 tracking-widest">{d.code}</span>
                <span className={`text-xs px-2 py-0.5 rounded-full font-semibold ${d.isActive ? 'bg-forest-50 text-forest-600' : 'bg-red-50 text-red-500'}`}>
                  {d.isActive ? 'فعال' : 'غیرفعال'}
                </span>
              </div>
              <p className="text-xs text-ink-400 mt-1">
                {d.type === 'Percentage' ? `${d.value}٪ تخفیف` : `${d.value.toLocaleString('fa-IR')} تومان`}
                {d.maxUsage ? ` · ${d.usedCount}/${d.maxUsage} بار استفاده` : ''}
              </p>
            </div>
            <button
              onClick={() => toggleMut.mutate({ id: d.id, active: d.isActive })}
              disabled={toggleMut.isPending}
              className={`px-3 py-1.5 rounded-xl text-xs font-semibold transition-colors disabled:opacity-40 ${
                d.isActive
                  ? 'border border-red-200 text-red-400 hover:bg-red-50'
                  : 'border border-forest-200 text-forest-500 hover:bg-forest-50'
              }`}
            >
              {d.isActive ? 'غیرفعال' : 'فعال‌سازی'}
            </button>
          </div>
        ))}
      </div>
    </div>
  )
}
