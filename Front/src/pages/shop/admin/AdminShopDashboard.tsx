import { useQuery } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { useShopAuth } from '../../../store/shopAuth'
import { ordersApi, type Order } from '../../../api/shop/orders'
import { adminUsersApi, type AdminUser } from '../../../api/shop/adminUsers'

const TODAY = new Date().toDateString()

function deriveAdminStats(orders: Order[], users: AdminUser[]) {
  const todayOrders   = orders.filter(o => new Date(o.createdAt).toDateString() === TODAY).length
  const pendingOrders = orders.filter(o => o.status === 'Pending').length
  const activeUsers   = users.filter(u => u.isActive && !u.isSuspended).length
  const todayRevenue  = orders
    .filter(o => o.status !== 'Cancelled' && new Date(o.createdAt).toDateString() === TODAY)
    .reduce((s, o) => s + o.totalAmount, 0)
  return { todayOrders, pendingOrders, activeUsers, todayRevenue }
}

const STATUS_BADGE: Record<string, string> = {
  Pending:    'bg-honey-50 text-honey-600',
  Processing: 'bg-blue-50 text-blue-600',
  Shipped:    'bg-forest-50 text-forest-500',
  Delivered:  'bg-forest-100 text-forest-700',
  Cancelled:  'bg-red-50 text-red-500',
}
const STATUS_LABEL: Record<string, string> = {
  Pending: 'در انتظار', Processing: 'پردازش', Shipped: 'ارسال شد', Delivered: 'تحویل', Cancelled: 'لغو'
}

const WEEK_DAYS = ['ش', 'ی', 'د', 'س', 'چ', 'پ', 'ج']

function weeklyRevenue(orders: Order[]): number[] {
  const buckets = Array(7).fill(0)
  const now = new Date()
  orders
    .filter(o => o.status !== 'Cancelled')
    .forEach(o => {
      const diff = Math.floor((now.getTime() - new Date(o.createdAt).getTime()) / 86_400_000)
      if (diff >= 0 && diff < 7) buckets[6 - diff] += o.totalAmount
    })
  return buckets
}

export default function AdminShopDashboard() {
  const { user } = useShopAuth()
  const navigate = useNavigate()

  const { data: orders = [], isLoading: loadO } = useQuery({ queryKey: ['admin-orders'],  queryFn: ordersApi.list })
  const { data: users  = [], isLoading: loadU } = useQuery({ queryKey: ['admin-users'],   queryFn: adminUsersApi.list })

  const stats   = deriveAdminStats(orders, users)
  const revenue = weeklyRevenue(orders)
  const maxRev  = Math.max(...revenue, 1)
  const recentOrders = [...orders].sort((a, b) => b.id - a.id).slice(0, 5)

  const STAT_CARDS = [
    { icon: '👥', label: 'کاربران فعال',      value: stats.activeUsers,                                    up: true  },
    { icon: '📦', label: 'سفارشات امروز',     value: stats.todayOrders,                                   up: true  },
    { icon: '⏳', label: 'در انتظار تأیید',   value: stats.pendingOrders,                                 up: false },
    { icon: '💰', label: 'فروش امروز',         value: (stats.todayRevenue / 1_000_000).toFixed(1) + 'م', up: true  },
  ]

  return (
    <div className="space-y-5 max-w-4xl">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-xl font-extrabold text-ink-900">داشبورد 📊</h1>
          <p className="text-xs text-ink-300 mt-0.5">سلام {user?.firstName ?? 'ادمین'} — امروز چه خبره؟</p>
        </div>
        <button
          onClick={() => navigate('/shop/admin/orders')}
          className="bg-pet-500 hover:bg-pet-600 text-white text-sm font-bold px-4 py-2.5 rounded-xl shadow-pet transition-all active:scale-95"
        >
          مشاهده سفارشات
        </button>
      </div>

      {(loadO || loadU) ? (
        <div className="grid grid-cols-2 md:grid-cols-4 gap-3">
          {[0,1,2,3].map(i => <div key={i} className="h-24 bg-white rounded-2xl border border-pet-100 animate-pulse" />)}
        </div>
      ) : (
        <div className="grid grid-cols-2 md:grid-cols-4 gap-3" data-testid="admin-stats">
          {STAT_CARDS.map(s => (
            <div key={s.label} className="bg-white rounded-2xl border border-pet-100 p-4 shadow-sm">
              <div className="text-2xl mb-2">{s.icon}</div>
              <div className="text-2xl font-extrabold text-ink-900" data-testid={`stat-${s.label}`}>{s.value}</div>
              <div className="text-[10px] text-ink-300 font-semibold mt-0.5">{s.label}</div>
            </div>
          ))}
        </div>
      )}

      {/* weekly bar chart — real data */}
      <div className="bg-white rounded-2xl border border-pet-100 p-5 shadow-sm">
        <h3 className="font-bold text-ink-900 mb-4">فروش ۷ روز اخیر 📈</h3>
        {loadO ? (
          <div className="h-20 bg-ink-100/30 rounded-xl animate-pulse" />
        ) : (
          <div className="flex items-end gap-2 h-20" data-testid="weekly-chart">
            {revenue.map((val, i) => {
              const h = Math.round((val / maxRev) * 100)
              const isToday = i === 6
              return (
                <div key={i} className="flex-1 flex flex-col items-center gap-1" title={`${val.toLocaleString('fa-IR')} ت`}>
                  <div
                    className="w-full rounded-t-lg transition-all"
                    style={{
                      height: `${Math.max(h, 4)}%`,
                      background: isToday ? '#E07B39' : val > 0 ? '#FDE8D4' : '#F3F4F6',
                    }}
                  />
                  <span className="text-[9px] text-ink-300">{WEEK_DAYS[i]}</span>
                </div>
              )
            })}
          </div>
        )}
      </div>

      {/* recent orders — real data */}
      <div className="bg-white rounded-2xl border border-pet-100 shadow-sm overflow-hidden">
        <div className="flex items-center justify-between px-5 py-4 border-b border-pet-50">
          <h3 className="font-bold text-ink-900">آخرین سفارشات</h3>
          <button
            onClick={() => navigate('/shop/admin/orders')}
            className="text-xs text-pet-500 font-semibold hover:underline"
          >
            مشاهده همه
          </button>
        </div>
        {loadO ? (
          <div className="divide-y divide-pet-50">
            {[0,1,2].map(i => <div key={i} className="h-14 mx-5 my-2 bg-ink-100/30 rounded-xl animate-pulse" />)}
          </div>
        ) : recentOrders.length === 0 ? (
          <p className="text-center py-8 text-ink-300 text-sm">سفارشی وجود ندارد</p>
        ) : (
          <div className="divide-y divide-pet-50" data-testid="admin-recent-orders">
            {recentOrders.map(o => (
              <div key={o.id} className="flex items-center gap-3 px-5 py-3 hover:bg-pet-50 transition-colors">
                <div className="w-8 h-8 bg-pet-100 rounded-xl flex items-center justify-center text-sm font-bold text-pet-600 shrink-0">
                  {o.customerName?.[0] ?? '؟'}
                </div>
                <div className="flex-1 min-w-0">
                  <p className="text-sm font-semibold text-ink-900 truncate">{o.customerName}</p>
                  <p className="text-xs text-ink-300">ORD-{o.id} · {o.totalAmount.toLocaleString('fa-IR')} ت</p>
                </div>
                <span className={`text-[10px] font-bold px-2.5 py-1 rounded-xl whitespace-nowrap ${STATUS_BADGE[o.status] ?? ''}`}>
                  {STATUS_LABEL[o.status] ?? o.status}
                </span>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  )
}
