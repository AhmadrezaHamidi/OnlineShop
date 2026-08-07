import { NavLink, Outlet } from 'react-router-dom'
import { useShopAuth } from '../../../store/shopAuth'
import { useNavigate } from 'react-router-dom'
import { shopAuthApi } from '../../../api/onlineshop-auth'

const SIDEBAR = [
  { to: '/shop/admin',          icon: '📊', label: 'داشبورد'   },
  { to: '/shop/admin/orders',   icon: '📦', label: 'سفارشات'   },
  { to: '/shop/admin/products', icon: '🛍️', label: 'محصولات'   },
  { to: '/shop/admin/users',    icon: '👥', label: 'کاربران'   },
  { to: '/shop/admin/discounts',icon: '🏷️', label: 'تخفیف‌ها'  },
  { to: '/shop/admin/bnpl',     icon: '💳', label: 'BNPL'      },
  { to: '/shop/admin/marketing',icon: '📣', label: 'بازاریابی' },
  { to: '/shop/admin/reports',  icon: '📋', label: 'گزارشات'   },
]

export default function AdminShopLayout() {
  const { user, logout } = useShopAuth()
  const navigate = useNavigate()

  const handleLogout = async () => {
    try { await shopAuthApi.logout() } catch {}
    logout()
    navigate('/shop/login')
  }

  return (
    <div className="min-h-screen flex" dir="rtl">
      {/* Sidebar desktop */}
      <aside className="hidden md:flex w-52 bg-ink-900 flex-col fixed top-0 bottom-0 right-0 z-30">
        <div className="flex items-center gap-3 px-5 py-5 border-b border-white/10">
          <div className="w-9 h-9 bg-pet-500 rounded-xl flex items-center justify-center text-xl">🐾</div>
          <div>
            <p className="text-white font-extrabold text-sm leading-tight">پت‌شاپ</p>
            <p className="text-white/40 text-[10px]">پنل مدیریت</p>
          </div>
        </div>

        <nav className="flex-1 py-3 px-2 space-y-0.5 overflow-y-auto">
          {SIDEBAR.map(({ to, icon, label }) => (
            <NavLink
              key={to}
              to={to}
              end={to === '/shop/admin'}
              className={({ isActive }) =>
                `flex items-center gap-3 px-3 py-2.5 rounded-xl text-sm font-semibold transition-all ${
                  isActive
                    ? 'bg-pet-500/20 text-pet-400'
                    : 'text-white/50 hover:bg-white/5 hover:text-white'
                }`
              }
            >
              <span className="text-base">{icon}</span>
              {label}
            </NavLink>
          ))}
        </nav>

        <div className="border-t border-white/10 p-3 space-y-0.5">
          <div className="px-3 py-2 text-xs text-white/30 truncate">{user?.firstName ?? user?.phoneNumber}</div>
          <button
            onClick={handleLogout}
            className="w-full flex items-center gap-3 px-3 py-2.5 rounded-xl text-sm font-semibold text-white/40 hover:bg-white/5 hover:text-red-400 transition-all"
          >
            <span>🚪</span> خروج
          </button>
        </div>
      </aside>

      {/* Main */}
      <div className="flex-1 md:mr-52 flex flex-col min-h-screen bg-pet-50">
        {/* mobile header */}
        <header className="md:hidden bg-ink-900 text-white px-4 h-14 flex items-center justify-between sticky top-0 z-20">
          <div className="flex items-center gap-2">
            <span>🐾</span>
            <span className="font-bold text-sm">پنل ادمین</span>
          </div>
          <button onClick={handleLogout} className="text-xs text-white/50">خروج</button>
        </header>

        <main className="flex-1 p-4 md:p-6">
          <Outlet />
        </main>
      </div>
    </div>
  )
}
