import { useState } from 'react'
import { useNavigate, Link } from 'react-router-dom'
import { useMutation } from '@tanstack/react-query'
import { authApi } from '../../api/auth'

export default function RegisterPage() {
  const navigate = useNavigate()
  const [form, setForm] = useState({ firstName: '', lastName: '', phoneNumber: '', nationalId: '' })

  const register = useMutation({
    mutationFn: () => authApi.register(form),
    onSuccess: () => navigate('/login'),
  })

  const set = (k: keyof typeof form) => (e: React.ChangeEvent<HTMLInputElement>) =>
    setForm(f => ({ ...f, [k]: e.target.value }))

  return (
    <div className="min-h-screen bg-gradient-to-br from-teal-50 to-teal-100 flex items-center justify-center p-4">
      <div className="card w-full max-w-md">
        <h1 className="text-2xl font-bold text-center text-teal-700 mb-8">ثبت‌نام</h1>
        <form onSubmit={e => { e.preventDefault(); register.mutate() }} className="space-y-4">
          <div className="grid grid-cols-2 gap-3">
            <div>
              <label className="block text-sm mb-1">نام</label>
              <input className="input" value={form.firstName} onChange={set('firstName')} required />
            </div>
            <div>
              <label className="block text-sm mb-1">نام خانوادگی</label>
              <input className="input" value={form.lastName} onChange={set('lastName')} required />
            </div>
          </div>
          <div>
            <label className="block text-sm mb-1">شماره موبایل</label>
            <input className="input" type="tel" dir="ltr" placeholder="09xxxxxxxxx"
              value={form.phoneNumber} onChange={set('phoneNumber')} required />
          </div>
          <div>
            <label className="block text-sm mb-1">کد ملی</label>
            <input className="input" dir="ltr" placeholder="0000000000"
              value={form.nationalId} onChange={set('nationalId')} required />
          </div>
          {register.error && <p className="text-red-600 text-sm">{(register.error as Error).message}</p>}
          <button className="btn btn-primary w-full" disabled={register.isPending}>
            {register.isPending ? 'در حال ثبت...' : 'ثبت‌نام'}
          </button>
          <p className="text-center text-sm text-gray-500">
            حساب دارید؟ <Link to="/login" className="text-teal-600">ورود</Link>
          </p>
        </form>
      </div>
    </div>
  )
}
