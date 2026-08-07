import { useState } from 'react'
import { useNavigate, Link } from 'react-router-dom'
import { useMutation } from '@tanstack/react-query'
import { authApi } from '../../api/auth'
import { useAuth } from '../../store/auth'

type Step = 'phone' | 'otp'

export default function LoginPage() {
  const navigate = useNavigate()
  const { setAuth } = useAuth()
  const [step, setStep] = useState<Step>('phone')
  const [phone, setPhone] = useState('')
  const [otp, setOtp] = useState('')
  const [devCode, setDevCode] = useState('')

  const sendCode = useMutation({
    mutationFn: () => authApi.login(phone),
    onSuccess: (data) => {
      // در حالت DEV کد در پیام برمی‌گردد
      const match = data.message?.match(/\d{6}/)
      if (match) setDevCode(`کد DEV: ${match[0]}`)
      setStep('otp')
    },
  })

  const verify = useMutation({
    mutationFn: () => authApi.verify(phone, otp),
    onSuccess: (data) => {
      setAuth(data.token, data.user)
      const role = data.user.role
      if (role === 'Admin') navigate('/admin')
      else if (role === 'Doctor') navigate('/doctor')
      else navigate('/patient')
    },
  })

  return (
    <div className="min-h-screen bg-gradient-to-br from-teal-50 to-teal-100 flex items-center justify-center p-4">
      <div className="card w-full max-w-md">
        <h1 className="text-2xl font-bold text-center text-teal-700 mb-8">ورود به سیستم</h1>

        {step === 'phone' && (
          <form onSubmit={e => { e.preventDefault(); sendCode.mutate() }} className="space-y-4">
            <div>
              <label className="block text-sm mb-1">شماره موبایل</label>
              <input
                className="input"
                type="tel"
                placeholder="09xxxxxxxxx"
                value={phone}
                onChange={e => setPhone(e.target.value)}
                dir="ltr"
                required
              />
            </div>
            {sendCode.error && (
              <p className="text-red-600 text-sm">{(sendCode.error as Error).message}</p>
            )}
            <button className="btn btn-primary w-full" disabled={sendCode.isPending}>
              {sendCode.isPending ? 'در حال ارسال...' : 'ارسال کد تأیید'}
            </button>
            <p className="text-center text-sm text-gray-500">
              حساب ندارید؟ <Link to="/register" className="text-teal-600">ثبت‌نام</Link>
            </p>
          </form>
        )}

        {step === 'otp' && (
          <form onSubmit={e => { e.preventDefault(); verify.mutate() }} className="space-y-4">
            {devCode && (
              <div className="bg-amber-50 border border-amber-200 rounded p-3 text-amber-800 text-sm text-center">
                {devCode}
              </div>
            )}
            <div>
              <label className="block text-sm mb-1">کد تأیید ارسال شده به {phone}</label>
              <input
                className="input text-center tracking-widest"
                type="text"
                placeholder="123456"
                maxLength={6}
                value={otp}
                onChange={e => setOtp(e.target.value)}
                dir="ltr"
                required
              />
            </div>
            {verify.error && (
              <p className="text-red-600 text-sm">{(verify.error as Error).message}</p>
            )}
            <button className="btn btn-primary w-full" disabled={verify.isPending}>
              {verify.isPending ? 'در حال تأیید...' : 'ورود'}
            </button>
            <button type="button" className="btn btn-outline w-full" onClick={() => setStep('phone')}>
              تغییر شماره
            </button>
          </form>
        )}
      </div>
    </div>
  )
}
