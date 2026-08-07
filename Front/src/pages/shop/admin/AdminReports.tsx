import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { backofficeApi, type Report } from '../../../api/shop/backoffice'

const REPORT_TYPES = [
  { value: 'Sales',     label: 'گزارش فروش' },
  { value: 'Users',     label: 'گزارش کاربران' },
  { value: 'Orders',    label: 'گزارش سفارشات' },
  { value: 'Inventory', label: 'گزارش موجودی' },
  { value: 'Revenue',   label: 'گزارش درآمد' },
]

const STATUS_META = {
  Pending:   { label: 'در حال پردازش', color: 'text-honey-600',  bg: 'bg-honey-50'   },
  Completed: { label: 'آماده',          color: 'text-forest-600', bg: 'bg-forest-50'  },
  Failed:    { label: 'ناموفق',         color: 'text-red-500',    bg: 'bg-red-50'     },
}

function fDate(iso: string) { return new Date(iso).toLocaleDateString('fa-IR') }

export default function AdminReports() {
  const qc = useQueryClient()

  const { data: reports = [], isLoading } = useQuery({
    queryKey: ['reports'],
    queryFn: backofficeApi.listReports,
  })

  const requestMut = useMutation({
    mutationFn: (type: string) => backofficeApi.requestReport(type),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['reports'] }),
  })

  if (isLoading) return (
    <div className="flex justify-center py-20 text-ink-300">
      <span className="text-3xl animate-spin">⏳</span>
    </div>
  )

  return (
    <div className="space-y-5">
      <h1 className="text-xl font-extrabold text-ink-900">گزارشات</h1>

      <div className="bg-white rounded-2xl border border-ink-100 shadow-sm p-4">
        <h2 className="font-bold text-ink-900 text-sm mb-3">درخواست گزارش جدید</h2>
        <div className="grid grid-cols-2 sm:grid-cols-3 gap-2">
          {REPORT_TYPES.map(rt => (
            <button
              key={rt.value}
              onClick={() => requestMut.mutate(rt.value)}
              disabled={requestMut.isPending}
              className="py-2.5 px-3 rounded-xl border border-pet-200 text-pet-600 text-xs font-semibold hover:bg-pet-50 disabled:opacity-40 transition-colors"
            >
              {rt.label}
            </button>
          ))}
        </div>
      </div>

      <div className="space-y-3">
        {reports.length === 0 && (
          <div className="text-center py-12 text-ink-300">
            <div className="text-4xl mb-2">📋</div>
            <p className="text-sm">گزارشی وجود ندارد</p>
          </div>
        )}
        {reports.map((r: Report) => {
          const meta = STATUS_META[r.status]
          return (
            <div key={r.id} className="bg-white rounded-2xl border border-ink-100 shadow-sm px-4 py-3 flex items-center justify-between">
              <div>
                <p className="font-bold text-ink-900 text-sm">
                  {REPORT_TYPES.find(t => t.value === r.type)?.label ?? r.type}
                </p>
                <p className="text-xs text-ink-400 mt-0.5">{fDate(r.requestedAt)}</p>
              </div>
              <div className="flex items-center gap-3">
                <span className={`text-xs font-semibold px-2.5 py-1 rounded-full ${meta.bg} ${meta.color}`}>
                  {meta.label}
                </span>
                {r.status === 'Completed' && r.fileUrl && (
                  <a
                    href={r.fileUrl}
                    target="_blank"
                    rel="noreferrer"
                    className="text-xs text-pet-500 hover:underline font-semibold"
                  >
                    دانلود
                  </a>
                )}
              </div>
            </div>
          )
        })}
      </div>
    </div>
  )
}
