import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { bnplApi, type BnplContract } from '../../../api/shop/bnpl'

const STATUS_META = {
  Active:    { label: 'فعال',      color: 'text-forest-600', bg: 'bg-forest-50'  },
  Completed: { label: 'تکمیل',     color: 'text-blue-600',   bg: 'bg-blue-50'    },
  Defaulted: { label: 'نکول',      color: 'text-red-500',    bg: 'bg-red-50'     },
  Cancelled: { label: 'لغو شده',  color: 'text-ink-400',    bg: 'bg-ink-100/50' },
}

function fDate(iso: string) { return new Date(iso).toLocaleDateString('fa-IR') }

export default function AdminBnpl() {
  const [userId, setUserId]     = useState('')
  const [selected, setSelected] = useState<BnplContract | null>(null)
  const qc = useQueryClient()

  const { data: contracts = [], isLoading, refetch } = useQuery({
    queryKey: ['bnpl-contracts', userId],
    queryFn: () => userId ? bnplApi.getUserContracts(+userId) : Promise.resolve([]),
    enabled: !!userId,
  })

  const mut = useMutation({
    mutationFn: ({ id, action }: { id: number; action: 'default' | 'cancel' }) =>
      action === 'default' ? bnplApi.defaultContract(id) : bnplApi.cancelContract(id),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['bnpl-contracts'] })
      setSelected(null)
    },
  })

  return (
    <div className="space-y-5">
      <h1 className="text-xl font-extrabold text-ink-900">مدیریت BNPL / اقساط</h1>

      <div className="bg-white rounded-2xl border border-ink-100 shadow-sm p-4">
        <label className="block text-xs text-ink-400 mb-2">شناسه کاربر</label>
        <div className="flex gap-2">
          <input
            value={userId}
            onChange={e => setUserId(e.target.value)}
            placeholder="User ID را وارد کنید…"
            className="flex-1 px-4 py-2.5 rounded-xl border border-ink-100 text-sm outline-none focus:border-pet-400 focus:ring-2 focus:ring-pet-100 transition-all"
            dir="ltr"
          />
          <button
            onClick={() => refetch()}
            className="px-4 py-2.5 rounded-xl bg-pet-500 text-white text-sm font-semibold hover:bg-pet-600 transition-colors"
          >
            جستجو
          </button>
        </div>
      </div>

      {isLoading && (
        <div className="flex justify-center py-10 text-ink-300">
          <span className="text-3xl animate-spin">⏳</span>
        </div>
      )}

      {userId && !isLoading && contracts.length === 0 && (
        <div className="text-center py-12 text-ink-300">
          <div className="text-4xl mb-2">💳</div>
          <p className="text-sm">قراردادی یافت نشد</p>
        </div>
      )}

      {contracts.map((c: BnplContract) => {
        const meta = STATUS_META[c.status]
        return (
          <div key={c.id} className="bg-white rounded-2xl border border-ink-100 shadow-sm p-4">
            <div className="flex justify-between items-start mb-3">
              <div>
                <p className="font-bold text-ink-900">قرارداد #{c.id}</p>
                <p className="text-xs text-ink-400 mt-0.5">{fDate(c.createdAt)}</p>
              </div>
              <span className={`text-xs font-semibold px-2.5 py-1 rounded-full ${meta.bg} ${meta.color}`}>
                {meta.label}
              </span>
            </div>

            <div className="grid grid-cols-3 gap-2 text-center mb-3">
              {[
                { label: 'مبلغ کل', value: c.totalAmount.toLocaleString('fa-IR') + ' ت' },
                { label: 'تعداد اقساط', value: c.installmentCount },
                { label: 'سفارش', value: `#${c.orderId}` },
              ].map(s => (
                <div key={s.label} className="bg-ink-900/5 rounded-xl p-2">
                  <p className="font-bold text-ink-900 text-sm">{s.value}</p>
                  <p className="text-xs text-ink-400">{s.label}</p>
                </div>
              ))}
            </div>

            {c.status === 'Active' && (
              <div className="flex gap-2">
                <button
                  onClick={() => mut.mutate({ id: c.id, action: 'default' })}
                  disabled={mut.isPending}
                  className="flex-1 py-2 rounded-xl border border-red-200 text-red-400 text-xs font-semibold hover:bg-red-50 disabled:opacity-40 transition-colors"
                >
                  ثبت نکول
                </button>
                <button
                  onClick={() => mut.mutate({ id: c.id, action: 'cancel' })}
                  disabled={mut.isPending}
                  className="flex-1 py-2 rounded-xl border border-ink-200 text-ink-400 text-xs font-semibold hover:bg-ink-50 disabled:opacity-40 transition-colors"
                >
                  لغو قرارداد
                </button>
              </div>
            )}
          </div>
        )
      })}
    </div>
  )
}
