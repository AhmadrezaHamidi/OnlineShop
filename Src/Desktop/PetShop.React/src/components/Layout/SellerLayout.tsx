import { NavLink, Outlet, useNavigate } from 'react-router-dom'
import { useAuth } from '../../hooks/useAuth'

const NAV = [
  { to: '/seller',           icon: 'ti-home',           label: 'داشبورد'       },
  { to: '/seller/products',  icon: 'ti-package',        label: 'محصولات'       },
  { to: '/seller/inventory', icon: 'ti-clipboard-list', label: 'انبار'         },
  { to: '/seller/orders',    icon: 'ti-shopping-cart',  label: 'سفارشات'       },
]

export default function SellerLayout() {
  const { user, logout } = useAuth()
  const navigate = useNavigate()

  async function handleLogout() {
    await logout()
    navigate('/login')
  }

  return (
    <div style={{ display: 'flex', minHeight: '100vh', background: 'var(--bg, #f1f5f9)' }}>
      {/* Sidebar */}
      <aside style={{
        width: 240, background: 'var(--dk)', color: '#fff',
        display: 'flex', flexDirection: 'column', padding: '1.5rem 0',
        position: 'sticky', top: 0, height: '100vh',
      }}>
        <div style={{ padding: '0 1.5rem 1.5rem', borderBottom: '1px solid rgba(255,255,255,.1)' }}>
          <div style={{ fontSize: 28, marginBottom: 4 }}>🐾</div>
          <div style={{ fontWeight: 700, fontSize: '1.1rem' }}>PetShop</div>
          <div style={{ fontSize: '.78rem', opacity: .6, marginTop: 2 }}>پنل فروشنده</div>
        </div>

        <nav style={{ flex: 1, padding: '1rem 0' }}>
          {NAV.map(n => (
            <NavLink
              key={n.to}
              to={n.to}
              end={n.to === '/seller'}
              style={({ isActive }) => ({
                display: 'flex', alignItems: 'center', gap: 12,
                padding: '.7rem 1.5rem', textDecoration: 'none',
                color: isActive ? '#fff' : 'rgba(255,255,255,.6)',
                background: isActive ? 'rgba(255,255,255,.12)' : 'transparent',
                borderRight: isActive ? '3px solid var(--p)' : '3px solid transparent',
                fontSize: '.9rem', fontWeight: isActive ? 600 : 400,
              })}
            >
              <i className={`ti ${n.icon}`} style={{ fontSize: 18 }} />
              {n.label}
            </NavLink>
          ))}
        </nav>

        <div style={{ padding: '1rem 1.5rem', borderTop: '1px solid rgba(255,255,255,.1)' }}>
          <div style={{ fontSize: '.82rem', opacity: .6, marginBottom: 8 }}>{user?.name ?? user?.mobile ?? 'فروشنده'}</div>
          <button className="btn btn-ghost" onClick={handleLogout} style={{ width: '100%', color: '#fff', border: '1px solid rgba(255,255,255,.2)' }}>
            <i className="ti ti-logout" /> خروج
          </button>
        </div>
      </aside>

      {/* Main content */}
      <main style={{ flex: 1, padding: '2rem', overflow: 'auto' }}>
        <Outlet />
      </main>
    </div>
  )
}
