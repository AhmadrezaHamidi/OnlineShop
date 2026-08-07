import { Link, Outlet, useNavigate } from 'react-router-dom'
import { useKheirAuth } from '../../store/kheirkhabarAuth'
import { kheirAuthApi } from '../../api/kheirkhabar-auth'

export default function KheirKhabarLayout() {
  const { user, refreshToken, logout } = useKheirAuth()
  const navigate = useNavigate()

  const handleLogout = async () => {
    try { if (refreshToken) await kheirAuthApi.logout(refreshToken) } catch { /* ignore */ }
    logout()
    navigate('/kheirkhabar')
  }

  return (
    <div className="min-h-screen bg-white" dir="rtl">
      <header className="border-b border-kheir-100">
        <div className="max-w-3xl mx-auto px-4 h-16 flex items-center justify-between">
          <Link to="/kheirkhabar" className="flex items-center gap-2">
            <span className="text-xl" aria-hidden>🕌</span>
            <span className="font-bold text-lg text-ink-900">خیرخبر</span>
          </Link>
          <nav className="hidden sm:flex items-center gap-6 text-sm text-ink-500">
            <Link to="/kheirkhabar" className="text-kheir-600 font-semibold">کمک به خیریه</Link>
          </nav>
          {user ? (
            <div className="flex items-center gap-3">
              <span className="text-xs text-ink-300 hidden sm:block">{user.fullName || user.email}</span>
              <button onClick={handleLogout} className="text-xs text-ink-300 hover:text-red-400 transition-colors">خروج</button>
            </div>
          ) : (
            <Link
              to="/kheirkhabar/login"
              className="text-xs font-semibold border border-kheir-200 rounded-full px-4 py-2 text-kheir-600 hover:bg-kheir-50 transition-colors"
            >
              ورود
            </Link>
          )}
        </div>
        <svg width="100%" height="8" viewBox="0 0 240 8" preserveAspectRatio="none" className="block">
          <defs>
            <pattern id="girih-nav" width="16" height="8" patternUnits="userSpaceOnUse">
              <path d="M8 0 L16 4 L8 8 L0 4 Z" fill="none" stroke="#BA7517" strokeWidth="1" />
              <circle cx="8" cy="4" r="1.1" fill="#3B6D11" />
            </pattern>
          </defs>
          <rect width="240" height="8" fill="url(#girih-nav)" />
        </svg>
      </header>

      <main className="max-w-3xl mx-auto px-4 py-6">
        <Outlet />
      </main>

      <footer className="border-t border-kheir-100 mt-10">
        <div className="max-w-3xl mx-auto px-4 py-6 text-center text-xs text-ink-300">
          خیرخبر — دست‌های کوچک، کمک‌های بزرگ
        </div>
      </footer>
    </div>
  )
}
