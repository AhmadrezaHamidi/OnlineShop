import { useQuery } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { useShopAuth } from '../../../store/shopAuth'
import { ordersApi, type Order } from '../../../api/shop/orders'
import { productsApi, type Product } from '../../../api/shop/products'

const TODAY = new Date().toDateString()

function deriveSellerStats(orders: Order[], products: Product[]) {
  const todayOrders   = orders.filter(o => new Date(o.createdAt).toDateString() === TODAY)
  const pendingOrders = orders.filter(o => o.status === 'Pending')
  const monthStart    = new Date(); monthStart.setDate(1); monthStart.setHours(0,0,0,0)
  const monthRevenue  = orders
    .filter(o => o.status === 'Delivered' && new Date(o.createdAt) >= monthStart)
    .reduce((s, o) => s + o.totalAmount, 0)
  const activeProducts   = products.filter(p => p.isActive).length
  const outOfStock       = products.filter(p => p.stock === 0).length
  return { todayOrders: todayOrders.length, pendingOrders: pendingOrders.length, monthRevenue, activeProducts, outOfStock }
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

export default function SellerDashboard() {
  const { user }  = useShopAuth()
  const navigate  = useNavigate()

  const { data: orders   = [], isLoading: loadO } = useQuery({ queryKey: ['seller-orders'],   queryFn: ordersApi.list })
  const { data: products = [], isLoading: loadP } = useQuery({ queryKey: ['seller-products'], queryFn: () => productsApi.list() })

  const stats = deriveSellerStats(orders, products)
  const recentOrders = [...orders].sort((a, b) => b.id - a.id).slice(0, 5)

  const STAT_CARDS = [
    { icon: '📦', label: 'سفارشات امروز',  value: stats.todayOrders,                                   color: 'text-pet-500'    },
    { icon: '🛍️', label: 'محصولات فعال',  value: stats.activeProducts,                                color: 'text-forest-500' },
    { icon: '⚠️', label: 'ناموجود',        value: stats.outOfStock,                                    color: 'text-red-400'    },
    { icon: '💰', label: 'فروش این ماه',   value: (stats.monthRevenue / 1_000_000).toFixed(1) + 'م',  color: 'text-honey-500'  },
  ]

  return (
    <div className="space-y-5">
      <div className="bg-gradient-to-br from-forest-500 to-forest-600 rounded-3xl p-6 text-white relative overflow-hidden">
        <div className="absolute left-4 top-1/2 -translate-y-1/2 text-6xl opacity-20 select-none">🏪</div>
        <p className="text-white/80 text-sm mb-1">سلام {user?.firstName ?? '👋'}</p>
        <h2 className="text-2xl font-extrabold">پنل فروشنده</h2>
        <p className="text-white/70 text-xs mt-1">
          {stats.pendingOrders > 0 ? `${stats.pendingOrders} سفارش در انتظار تأیید` : 'همه سفارشات پردازش شده'}
        </p>
      </div>

      {(loadO || loadP) ? (
        <div className="grid grid-cols-2 gap-3">
          {[0,1,2,3].map(i => (
            <div key={i} className="bg-white rounded-2xl border border-forest-100 p-4 shadow-sm h-24 animate-pulse" />
          ))}
        </div>
      ) : (
        <div className="grid grid-cols-2 gap-3" data-testid="seller-stats">
          {STAT_CARDS.map(s => (
            <div key={s.label} className="bg-white rounded-2xl border border-forest-100 p-4 shadow-sm">
              <div className="text-2xl mb-1">{s.icon}</div>
              <div className={`text-2xl font-extrabold ${s.color}`} data-testid={`stat-${s.label}`}>{s.value}</div>
              <div className="text-xs text-ink-300 mt-0.5 font-medium">{s.label}</div>
            </div>
          ))}
        </div>
      )}

      <div className="bg-white rounded-2xl border border-forest-100 p-4 shadow-sm">
        <div className="flex items-center justify-between mb-3">
          <h3 className="font-bold text-ink-900">آخرین سفارشات</h3>
          <button
            onClick={() => navigate('/shop/seller/orders')}
            className="text-xs text-forest-500 font-semibold hover:underline"
          >
            مشاهده همه
          </button>
        </div>
        {loadO ? (
          <div className="space-y-3">
            {[0,1,2].map(i => <div key={i} className="h-10 bg-ink-100/40 rounded-xl animate-pulse" />)}
          </div>
        ) : recentOrders.length === 0 ? (
          <p className="text-center py-6 text-ink-300 text-sm">سفارشی ثبت نشده</p>
        ) : (
          <div className="space-y-2" data-testid="recent-orders">
            {recentOrders.map(o => (
              <div key={o.id} className="flex items-center justify-between py-2 border-b border-pet-50 last:border-0">
                <div>
                  <p className="text-sm font-semibold text-ink-900">{o.customerName}</p>
                  <p className="text-xs text-ink-300">ORD-{o.id} · {o.totalAmount.toLocaleString('fa-IR')} ت</p>
                </div>
                <span className={`text-xs font-semibold px-3 py-1 rounded-xl ${STATUS_BADGE[o.status] ?? ''}`}>
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
