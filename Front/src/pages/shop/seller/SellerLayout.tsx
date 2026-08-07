import { NavLink, Outlet } from 'react-router-dom'
import { useShopAuth } from '../../../store/shopAuth'
import { useNavigate } from 'react-router-dom'
import { shopAuthApi } from '../../../api/onlineshop-auth'

const NAV = [
  { to: '/shop/seller',             icon: '📊', label: 'داشبورد'  },
  { to: '/shop/seller/products',    icon: '🛍️', label: 'محصولات'  },
  { to: '/shop/seller/orders',      icon: '📦', label: 'سفارشات'  },
  { to: '/shop/seller/inventory',   icon: '🏬', label: 'انبار'    },
  { to: '/shop/seller/profile',     icon: '👤', label: 'پروفایل'  },
]

export default function SellerLayout() {
  const { user, logout } = useShopAuth()
  const navigate = useNavigate()

  const handleLogout = async () => {
    try { await shopAuthApi.logout() } catch {}
    logout()
    navigate('/shop/login')
  }

  return (
    <div className="min-h-screen bg-forest-50 flex flex-col" dir="rtl">
      <header className="bg-white border-b border-forest-100 sticky top-0 z-20 shadow-sm">
        <div className="max-w-2xl mx-auto px-4 h-14 flex items-center justify-between">
          <div className="flex items-center gap-2">
            <span className="text-2xl">🏪</span>
            <span className="font-extrabold text-forest-500 text-lg">پنل فروشنده</span>
          </div>
          <div className="flex items-center gap-3">
            <span className="text-xs text-ink-300 hidden sm:block">{user?.firstName ?? user?.phoneNumber}</span>
            <button onClick={handleLogout} className="text-xs text-ink-300 hover:text-red-400 transition-colors">خروج</button>
          </div>
        </div>
      </header>

      <main className="flex-1 max-w-2xl w-full mx-auto px-4 py-5">
        <Outlet />
      </main>

      <nav className="bg-white border-t border-forest-100 sticky bottom-0 z-20">
        <div className="max-w-2xl mx-auto flex">
          {NAV.map(({ to, icon, label }) => (
            <NavLink
              key={to}
              to={to}
              end={to === '/shop/seller'}
              className={({ isActive }) =>
                `flex-1 flex flex-col items-center py-2 gap-0.5 text-xs font-semibold transition-colors ${
                  isActive ? 'text-forest-500' : 'text-ink-300 hover:text-ink-500'
                }`
              }
            >
              <span className="text-xl">{icon}</span>
              {label}
            </NavLink>
          ))}
        </div>
      </nav>
    </div>
  )
}
