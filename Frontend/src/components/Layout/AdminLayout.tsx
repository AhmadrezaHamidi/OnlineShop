import { NavLink, Outlet, useNavigate } from 'react-router-dom'
import { useAuth } from '../../hooks/useAuth'
import styles from './AdminLayout.module.css'

const navItems = [
  { to: '/admin',          icon: 'ti-layout-dashboard', label: 'داشبورد',     end: true },
  { to: '/admin/orders',   icon: 'ti-shopping-cart',    label: 'سفارش‌ها'            },
  { to: '/admin/products', icon: 'ti-box',              label: 'محصولات'             },
  { to: '/admin/users',    icon: 'ti-users',            label: 'کاربران'             },
  { to: '/admin/bnpl',     icon: 'ti-credit-card',      label: 'اعتبار BNPL'         },
  { to: '/admin/marketing',icon: 'ti-message',          label: 'بازاریابی'           },
  { to: '/admin/reports',  icon: 'ti-chart-pie',        label: 'گزارشات'             },
]

export default function AdminLayout() {
  const { user, logout } = useAuth()
  const navigate = useNavigate()

  async function handleLogout() {
    await logout()
    navigate('/login')
  }

  return (
    <div className={styles.layout}>
      {/* Sidebar */}
      <aside className={styles.sidebar}>
        <div className={styles.logo}>
          <span>🐾</span>
          <div>
            <div className={styles.logoTitle}>PetShop</div>
            <div className={styles.logoSub}>پنل مدیریت</div>
          </div>
        </div>

        <nav className={styles.nav}>
          <div className={styles.navGroup}>
            <div className={styles.navGroupLabel}>اصلی</div>
            {navItems.slice(0, 5).map(item => (
              <NavLink
                key={item.to}
                to={item.to}
                end={item.end}
                className={({ isActive }) =>
                  `${styles.navItem} ${isActive ? styles.active : ''}`
                }
              >
                <i className={`ti ${item.icon}`} aria-hidden="true" />
                {item.label}
              </NavLink>
            ))}
          </div>
          <div className={styles.navGroup}>
            <div className={styles.navGroupLabel}>مدیریت</div>
            {navItems.slice(5).map(item => (
              <NavLink
                key={item.to}
                to={item.to}
                className={({ isActive }) =>
                  `${styles.navItem} ${isActive ? styles.active : ''}`
                }
              >
                <i className={`ti ${item.icon}`} aria-hidden="true" />
                {item.label}
              </NavLink>
            ))}
          </div>
        </nav>

        <div className={styles.sidebarFooter}>
          <div className={styles.userRow}>
            <div className={styles.avatar}>
              {user?.fullName?.[0] ?? user?.phoneNumber?.[0] ?? 'A'}
            </div>
            <div>
              <div className={styles.userName}>{user?.fullName || 'ادمین'}</div>
              <div className={styles.userRole}>Super Admin</div>
            </div>
          </div>
          <button className={styles.logoutBtn} onClick={handleLogout} title="خروج">
            <i className="ti ti-logout" aria-hidden="true" />
          </button>
        </div>
      </aside>

      {/* Main */}
      <div className={styles.main}>
        <Outlet />
      </div>
    </div>
  )
}
