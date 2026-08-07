import { NavLink, Outlet, useNavigate } from 'react-router-dom'
import { useAuth } from '../../store/auth'

const links = [
  { to: '/admin', label: 'داشبورد', end: true },
  { to: '/admin/patients', label: 'بیماران' },
  { to: '/admin/doctors', label: 'پزشکان' },
  { to: '/admin/reservations', label: 'نوبت‌ها' },
  { to: '/admin/campaigns', label: 'کمپین‌ها' },
  { to: '/admin/settings', label: 'تنظیمات' },
]

export default function AdminLayout() {
  const { user, logout } = useAuth()
  const navigate = useNavigate()

  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-gray-900 text-white shadow">
        <div className="max-w-6xl mx-auto px-4 flex items-center justify-between h-14">
          <span className="font-bold text-amber-400">پنل مدیریت</span>
          <div className="flex gap-4 text-sm">
            {links.map(l => (
              <NavLink key={l.to} to={l.to} end={l.end}
                className={({ isActive }) => isActive ? 'text-amber-400' : 'opacity-70 hover:opacity-100'}>
                {l.label}
              </NavLink>
            ))}
          </div>
          <div className="flex items-center gap-3 text-sm">
            <span className="opacity-70">{user?.firstName}</span>
            <button onClick={() => { logout(); navigate('/login') }} className="opacity-50 hover:opacity-100">خروج</button>
          </div>
        </div>
      </nav>
      <main className="max-w-6xl mx-auto px-4 py-6"><Outlet /></main>
    </div>
  )
}
