import { NavLink, Outlet, useNavigate } from 'react-router-dom'
import { useAuth } from '../../store/auth'

const links = [
  { to: '/doctor', label: 'امروز', end: true },
  { to: '/doctor/schedule', label: 'برنامه' },
  { to: '/doctor/prescriptions', label: 'نسخه‌ها' },
  { to: '/doctor/messages', label: 'پیام‌ها' },
]

export default function DoctorLayout() {
  const { user, logout } = useAuth()
  const navigate = useNavigate()

  return (
    <div className="min-h-screen bg-gray-50">
      <nav className="bg-teal-800 text-white shadow">
        <div className="max-w-5xl mx-auto px-4 flex items-center justify-between h-14">
          <span className="font-bold">پنل پزشک</span>
          <div className="flex gap-4 text-sm">
            {links.map(l => (
              <NavLink key={l.to} to={l.to} end={l.end}
                className={({ isActive }) => isActive ? 'border-b-2 border-amber-400 pb-0.5' : 'opacity-80 hover:opacity-100'}>
                {l.label}
              </NavLink>
            ))}
          </div>
          <div className="flex items-center gap-3 text-sm">
            <span>دکتر {user?.firstName}</span>
            <button onClick={() => { logout(); navigate('/login') }} className="opacity-70 hover:opacity-100">خروج</button>
          </div>
        </div>
      </nav>
      <main className="max-w-5xl mx-auto px-4 py-6"><Outlet /></main>
    </div>
  )
}
