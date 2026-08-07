import { useState, useEffect } from 'react'
import { useQuery, useMutation } from '@tanstack/react-query'
import { accountApi, type Profile } from '../../../api/shop/account'
import { useShopAuth } from '../../../store/shopAuth'
import { useNavigate } from 'react-router-dom'
import { shopAuthApi } from '../../../api/onlineshop-auth'

export default function CustomerProfile() {
  const { user, logout } = useShopAuth()
  const navigate = useNavigate()

  const { data: profile, isLoading } = useQuery({
    queryKey: ['profile'],
    queryFn: accountApi.getProfile,
  })

  const [form, setForm] = useState<Partial<Profile>>({})
  const [saved, setSaved] = useState(false)

  useEffect(() => {
    if (profile) setForm(profile)
  }, [profile])

  const updateMut = useMutation({
    mutationFn: () => accountApi.updateProfile(form),
    onSuccess: () => {
      setSaved(true)
      setTimeout(() => setSaved(false), 2000)
    },
  })

  const handleLogout = async () => {
    try { await shopAuthApi.logout() } catch {}
    logout()
    navigate('/shop/login')
  }

  if (isLoading) return (
    <div className="flex justify-center py-20 text-ink-300">
      <span className="text-3xl animate-spin">⏳</span>
    </div>
  )

  return (
    <div className="space-y-4">
      <h1 className="text-xl font-extrabold text-ink-900">پروفایل من</h1>

      <div className="bg-gradient-to-l from-pet-500 to-pet-600 rounded-2xl p-5 flex items-center gap-4">
        <div className="w-14 h-14 bg-white/20 rounded-full flex items-center justify-center text-3xl">
          👤
        </div>
        <div>
          <p className="text-white font-extrabold text-base">
            {profile?.firstName} {profile?.lastName}
          </p>
          <p className="text-white/70 text-sm">{user?.phoneNumber}</p>
        </div>
      </div>

      <div className="bg-white rounded-2xl border border-ink-100 shadow-sm p-4 space-y-4">
        <h2 className="font-bold text-ink-900 text-sm">ویرایش اطلاعات</h2>

        {[
          { key: 'firstName',   label: 'نام',          type: 'text'  },
          { key: 'lastName',    label: 'نام خانوادگی', type: 'text'  },
          { key: 'email',       label: 'ایمیل',         type: 'email' },
          { key: 'nationalCode',label: 'کد ملی',        type: 'text'  },
          { key: 'address',     label: 'آدرس',          type: 'text'  },
        ].map(({ key, label, type }) => (
          <div key={key}>
            <label className="block text-xs text-ink-400 mb-1">{label}</label>
            <input
              type={type}
              value={(form as Record<string, string>)[key] ?? ''}
              onChange={e => setForm(prev => ({ ...prev, [key]: e.target.value }))}
              className="w-full px-3.5 py-2.5 rounded-xl border border-ink-100 bg-white text-sm outline-none focus:border-pet-400 focus:ring-2 focus:ring-pet-100 transition-all"
              dir="rtl"
            />
          </div>
        ))}

        {saved && (
          <div className="bg-forest-50 text-forest-600 text-sm text-center py-2 rounded-xl font-semibold">
            ✓ اطلاعات ذخیره شد
          </div>
        )}

        <button
          onClick={() => updateMut.mutate()}
          disabled={updateMut.isPending}
          className="w-full py-2.5 rounded-xl bg-pet-500 text-white font-semibold text-sm hover:bg-pet-600 disabled:opacity-50 transition-colors"
        >
          {updateMut.isPending ? 'در حال ذخیره…' : 'ذخیره تغییرات'}
        </button>
      </div>

      <button
        onClick={handleLogout}
        className="w-full py-3 rounded-2xl border border-red-200 text-red-400 font-semibold text-sm hover:bg-red-50 transition-colors"
      >
        🚪 خروج از حساب
      </button>
    </div>
  )
}
