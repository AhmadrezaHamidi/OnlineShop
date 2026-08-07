import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { charityApi, type CampaignSummary } from '../../api/kheirkhabar-charity'
import { newsApi } from '../../api/kheirkhabar-news'
import { useKheirAuth } from '../../store/kheirkhabarAuth'

function fmtToman(n: number) {
  return n.toLocaleString('fa-IR') + ' تومان'
}

function HeartIllustration({ className }: { className?: string }) {
  return (
    <svg viewBox="0 0 200 140" preserveAspectRatio="xMidYMid slice" className={className}>
      <rect width="200" height="140" fill="#EAF3DE" />
      <path
        d="M100 108 C68 86 52 62 52 42 C52 24 66 12 82 12 C92 12 100 19 100 30 C100 19 109 12 119 12 C134 12 148 24 148 42 C148 62 132 86 100 108 Z"
        fill="#27500A"
      />
      <path d="M22 132 C22 106 48 94 100 94 C152 94 178 106 178 132 L178 138 L22 138 Z" fill="#97C459" />
      <path d="M162 26 L166 34 L175 35 L168 41 L170 50 L162 45 L154 50 L156 41 L149 35 L158 34 Z" fill="#EF9F27" />
    </svg>
  )
}

function NewsIllustration({ className, bg = '#FAECE7', dark = '#D85A30', light = '#F0997B', dot = '#993C1D' }:
  { className?: string; bg?: string; dark?: string; light?: string; dot?: string }) {
  return (
    <svg viewBox="0 0 200 140" preserveAspectRatio="xMidYMid slice" className={className}>
      <rect width="200" height="140" fill={bg} />
      <rect x="26" y="20" width="148" height="100" rx="6" fill="#FFFFFF" />
      <rect x="42" y="36" width="70" height="9" rx="2" fill={dark} />
      <rect x="42" y="53" width="116" height="5" rx="2" fill={light} />
      <rect x="42" y="64" width="116" height="5" rx="2" fill={light} />
      <rect x="42" y="75" width="90" height="5" rx="2" fill={light} />
      <circle cx="150" cy="42" r="11" fill={dot} />
    </svg>
  )
}

function ProgressBar({ pct }: { pct: number }) {
  return (
    <div className="h-2 rounded-full bg-kheir-50 overflow-hidden">
      <div className="h-full bg-kheir-500" style={{ width: `${pct}%` }} />
    </div>
  )
}

const newsPalette = [
  { bg: '#FAECE7', dark: '#D85A30', light: '#F0997B', dot: '#993C1D' },
  { bg: '#FBEAF0', dark: '#993556', light: '#ED93B1', dot: '#993556' },
  { bg: '#FAEEDA', dark: '#BA7517', light: '#EF9F27', dot: '#BA7517' },
]

function timeAgo(iso: string) {
  const diffMs = Date.now() - new Date(iso).getTime()
  const hours = Math.floor(diffMs / 3_600_000)
  if (hours < 1) return 'چند لحظه پیش'
  if (hours < 24) return `${hours.toLocaleString('fa-IR')} ساعت پیش`
  const days = Math.floor(hours / 24)
  return days === 1 ? 'دیروز' : `${days.toLocaleString('fa-IR')} روز پیش`
}

export default function KheirKhabarHome() {
  const navigate = useNavigate()
  const { user } = useKheirAuth()
  const queryClient = useQueryClient()

  const { data: campaigns, isLoading: loadingCampaigns } = useQuery({
    queryKey: ['kheirkhabar-campaigns'],
    queryFn: () => charityApi.list(1, 6),
  })

  const { data: news, isLoading: loadingNews } = useQuery({
    queryKey: ['kheirkhabar-news'],
    queryFn: () => newsApi.list(1, 3),
  })

  const donate = useMutation({
    mutationFn: ({ id, amount, donorDisplayName }: { id: number; amount: number; donorDisplayName: string }) =>
      charityApi.donate(id, amount, donorDisplayName),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: ['kheirkhabar-campaigns'] }),
  })

  const handleDonate = (campaign: CampaignSummary) => {
    if (!user) {
      navigate('/kheirkhabar/login')
      return
    }
    const raw = window.prompt(`مبلغ کمک به «${campaign.title}» (تومان):`)
    if (!raw) return
    const amount = Number(raw.replace(/[^\d]/g, ''))
    if (!amount || amount <= 0) return
    donate.mutate({ id: campaign.id, amount, donorDisplayName: user.fullName || 'حامی' })
  }

  const items = campaigns?.items ?? []
  const featured = items[0]
  const rest = items.slice(1, 3)

  return (
    <div className="space-y-10">

      {/* ── Charity section (primary) ── */}
      <section>
        <div className="flex items-center gap-2 mb-1">
          <span aria-hidden>🌙</span>
          <h1 className="text-lg font-bold text-ink-900">کمک به خیریه</h1>
          <span className="text-xs text-ink-300">— دست‌های کوچک، کمک‌های بزرگ</span>
        </div>
        <p className="text-xs text-ink-300 mb-4">کمپین‌های در حال اجرا، تأییدشده و شفاف</p>

        {loadingCampaigns && <p className="text-xs text-ink-300">در حال بارگذاری کمپین‌ها...</p>}
        {!loadingCampaigns && items.length === 0 && (
          <p className="text-xs text-ink-300">فعلاً کمپینی ثبت نشده است.</p>
        )}

        {featured && (
          <div className="rounded-2xl border border-kheir-100 overflow-hidden mb-4">
            <div className="grid grid-cols-1 sm:grid-cols-2">
              <HeartIllustration className="w-full aspect-[4/3] sm:aspect-auto sm:h-full" />
              <div className="p-4 flex flex-col">
                {featured.isFeatured && (
                  <span className="text-[11px] w-fit px-3 py-1 rounded-full bg-gold-50 text-gold-500 mb-2">کمپین ویژه</span>
                )}
                <h2 className="font-bold text-ink-900 mb-3">{featured.title}</h2>
                <ProgressBar pct={featured.progressPercent} />
                <p className="text-xs text-ink-300 mt-2 mb-4">
                  {featured.progressPercent}٪ از {fmtToman(featured.goalAmount)}
                </p>
                <button
                  onClick={() => handleDonate(featured)}
                  disabled={donate.isPending}
                  className="mt-auto w-full bg-kheir-600 hover:bg-kheir-500 disabled:opacity-50 text-white font-bold py-3 rounded-xl text-sm transition-colors"
                >
                  کمک می‌کنم
                </button>
              </div>
            </div>
          </div>
        )}

        {rest.length > 0 && (
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-3 mb-3">
            {rest.map(c => (
              <div key={c.id} className="rounded-xl border border-kheir-100 overflow-hidden">
                <HeartIllustration className="w-full aspect-[16/8]" />
                <div className="p-3">
                  <p className="text-sm font-semibold text-ink-900 mb-2 line-clamp-2">{c.title}</p>
                  <ProgressBar pct={c.progressPercent} />
                  <div className="flex justify-between text-[11px] text-ink-300 mt-1.5 mb-3">
                    <span>{c.progressPercent}٪ تامین شده</span>
                    <span>{c.daysLeft} روز مانده</span>
                  </div>
                  <button
                    onClick={() => handleDonate(c)}
                    disabled={donate.isPending}
                    className="w-full border border-kheir-200 text-kheir-600 font-semibold py-2 rounded-lg text-xs hover:bg-kheir-50 disabled:opacity-50 transition-colors"
                  >
                    حمایت می‌کنم
                  </button>
                </div>
              </div>
            ))}
          </div>
        )}
      </section>

      {/* ── News section (secondary) ── */}
      <section className="border-t border-kheir-100 pt-6">
        <div className="flex items-center justify-between mb-3">
          <h2 className="text-sm font-semibold text-ink-500">آخرین اخبار</h2>
        </div>

        {loadingNews && <p className="text-xs text-ink-300">در حال بارگذاری اخبار...</p>}
        {!loadingNews && (news?.items.length ?? 0) === 0 && (
          <p className="text-xs text-ink-300">فعلاً خبری ثبت نشده است.</p>
        )}

        <div className="grid grid-cols-1 sm:grid-cols-3 gap-3">
          {news?.items.map((n, i) => (
            <div key={n.id} className="rounded-lg border border-ink-100 overflow-hidden">
              <NewsIllustration className="w-full aspect-[16/10]" {...newsPalette[i % 3]} />
              <div className="p-2.5">
                <p className="text-xs font-medium text-ink-900 line-clamp-2 mb-1">{n.title}</p>
                <span className="text-[10px] text-ink-300">{timeAgo(n.publishedAt)}</span>
              </div>
            </div>
          ))}
        </div>
      </section>
    </div>
  )
}
