import { useState, useEffect, useRef } from 'react'
import { useNavigate } from 'react-router-dom'
import { useMutation } from '@tanstack/react-query'
import { shopAuthApi } from '../../api/onlineshop-auth'
import { useShopAuth } from '../../store/shopAuth'
import OtpInput from '../../components/OtpInput'

type Step = 'phone' | 'otp'

const RESEND_SECONDS = 90

export default function ShopLoginPage() {
  const navigate = useNavigate()
  const { setAuth } = useShopAuth()

  const [step, setStep] = useState<Step>('phone')
  const [phone, setPhone] = useState('')
  const [otp, setOtp] = useState('')
  const [devCode, setDevCode] = useState('')
  const [countdown, setCountdown] = useState(0)
  const timerRef = useRef<ReturnType<typeof setInterval> | null>(null)

  // countdown timer
  useEffect(() => {
    if (countdown <= 0) {
      if (timerRef.current) clearInterval(timerRef.current)
      return
    }
    timerRef.current = setInterval(() => setCountdown(c => c - 1), 1000)
    return () => { if (timerRef.current) clearInterval(timerRef.current) }
  }, [countdown])

  // auto-verify when 6 digits entered
  useEffect(() => {
    const filled = otp.replace(/\s/g, '').length
    if (filled === 6 && step === 'otp') verify.mutate()
  }, [otp])

  const requestOtp = useMutation({
    mutationFn: () => shopAuthApi.requestOtp(phone),
    onSuccess: (data) => {
      // در DEV کد برمی‌گردد
      const match = data.message?.match(/\d{6}/)
      if (match) setDevCode(match[0])
      else if (data.devCode) setDevCode(data.devCode)
      setStep('otp')
      setCountdown(RESEND_SECONDS)
    },
  })

  const verify = useMutation({
    mutationFn: () => shopAuthApi.verifyOtp(phone, otp.replace(/\s/g, '')),
    onSuccess: (data) => {
      setAuth(data.token, data.refreshToken, data.user)
      const role = data.user.role
      if (role === 'Admin')    navigate('/shop/admin')
      else if (role === 'Seller') navigate('/shop/seller')
      else                        navigate('/shop/customer')
    },
  })

  const handleResend = () => {
    setOtp('')
    setDevCode('')
    requestOtp.mutate()
  }

  const fmt = (s: number) =>
    `${String(Math.floor(s / 60)).padStart(2, '0')}:${String(s % 60).padStart(2, '0')}`

  return (
    <div className="min-h-screen bg-pet-50 flex items-center justify-center p-4" dir="rtl">

      {/* background paw prints */}
      <div className="fixed inset-0 overflow-hidden pointer-events-none select-none" aria-hidden>
        <span className="absolute top-10 right-10 text-7xl opacity-5 rotate-[-20deg]">🐾</span>
        <span className="absolute bottom-20 left-10 text-8xl opacity-5 rotate-[15deg]">🐾</span>
        <span className="absolute top-1/2 right-4 text-5xl opacity-5 rotate-[30deg]">🐾</span>
        <span className="absolute top-1/3 left-1/4 text-6xl opacity-5 rotate-[-10deg]">🐾</span>
      </div>

      <div className="w-full max-w-sm relative">

        {/* Card */}
        <div className="bg-white rounded-3xl shadow-pet-lg border border-pet-100 overflow-hidden">

          {/* Header */}
          <div className="bg-gradient-to-br from-pet-500 to-pet-600 px-8 pt-10 pb-8 text-center relative overflow-hidden">
            <div className="absolute inset-0 opacity-10 text-[120px] flex items-center justify-center select-none">🐾</div>
            <div className="relative">
              <div className="text-5xl mb-3 drop-shadow-sm">🐶</div>
              <h1 className="text-2xl font-extrabold text-white leading-tight">
                {step === 'phone' ? 'خوش اومدی!' : 'کد تأیید'}
              </h1>
              <p className="text-white/75 text-sm mt-1">
                {step === 'phone'
                  ? 'به پت‌شاپ آنلاین وارد شو'
                  : `کد ارسال‌شده به ${phone} رو وارد کن`}
              </p>
            </div>
          </div>

          {/* Body */}
          <div className="px-8 py-7 space-y-5">

            {/* ── STEP: PHONE ── */}
            {step === 'phone' && (
              <form
                onSubmit={e => { e.preventDefault(); requestOtp.mutate() }}
                className="space-y-4"
              >
                <div>
                  <label className="block text-sm font-semibold text-ink-500 mb-2">
                    شماره موبایل
                  </label>
                  <div className="relative">
                    <span className="absolute right-3.5 top-1/2 -translate-y-1/2 text-lg">📱</span>
                    <input
                      type="tel"
                      placeholder="09xxxxxxxxx"
                      value={phone}
                      onChange={e => setPhone(e.target.value.replace(/\D/g, '').slice(0, 11))}
                      dir="ltr"
                      className="w-full pr-10 pl-4 py-3 rounded-2xl border-2 border-ink-100 bg-pet-50
                                 text-sm font-medium tracking-wide outline-none
                                 focus:border-pet-400 focus:bg-white focus:shadow-pet transition-all"
                      required
                    />
                  </div>
                </div>

                {requestOtp.error && (
                  <div className="bg-red-50 border border-red-100 rounded-xl px-4 py-2.5 text-red-600 text-sm flex items-center gap-2">
                    <span>⚠️</span>{(requestOtp.error as Error).message}
                  </div>
                )}

                <button
                  type="submit"
                  disabled={requestOtp.isPending || phone.length < 11}
                  className="w-full bg-pet-500 hover:bg-pet-600 active:scale-[.98] disabled:opacity-50
                             text-white font-bold py-3.5 rounded-2xl transition-all shadow-pet text-sm"
                >
                  {requestOtp.isPending
                    ? <span className="flex items-center justify-center gap-2">
                        <span className="w-4 h-4 border-2 border-white/40 border-t-white rounded-full animate-spin"/>
                        در حال ارسال...
                      </span>
                    : '📨 دریافت کد تأیید'}
                </button>

                {/* role hint */}
                <div className="grid grid-cols-3 gap-2 pt-1">
                  {[
                    { icon: '🛒', label: 'مشتری', color: 'bg-blue-50 text-blue-600' },
                    { icon: '🏪', label: 'فروشنده', color: 'bg-forest-50 text-forest-500' },
                    { icon: '🔑', label: 'ادمین', color: 'bg-pet-100 text-pet-600' },
                  ].map(r => (
                    <div key={r.label} className={`${r.color} rounded-xl py-2 text-center text-xs font-semibold`}>
                      <div className="text-lg">{r.icon}</div>
                      {r.label}
                    </div>
                  ))}
                </div>
                <p className="text-center text-xs text-ink-300">
                  نقش شما بر اساس حساب کاربری تشخیص داده می‌شود
                </p>
              </form>
            )}

            {/* ── STEP: OTP ── */}
            {step === 'otp' && (
              <form
                onSubmit={e => { e.preventDefault(); verify.mutate() }}
                className="space-y-5"
              >
                {/* DEV code hint */}
                {devCode && (
                  <div className="bg-honey-50 border border-honey-400/30 rounded-xl px-4 py-3 text-center">
                    <p className="text-xs text-honey-600 font-semibold mb-0.5">🧪 کد DEV (فقط در حالت توسعه)</p>
                    <p className="text-2xl font-extrabold text-honey-500 tracking-[.2em]">{devCode}</p>
                  </div>
                )}

                <OtpInput
                  value={otp}
                  onChange={setOtp}
                  disabled={verify.isPending}
                />

                {verify.error && (
                  <div className="bg-red-50 border border-red-100 rounded-xl px-4 py-2.5 text-red-600 text-sm flex items-center gap-2">
                    <span>⚠️</span>{(verify.error as Error).message}
                  </div>
                )}

                <button
                  type="submit"
                  disabled={verify.isPending || otp.replace(/\s/g, '').length < 6}
                  className="w-full bg-forest-400 hover:bg-forest-500 active:scale-[.98] disabled:opacity-50
                             text-white font-bold py-3.5 rounded-2xl transition-all text-sm"
                >
                  {verify.isPending
                    ? <span className="flex items-center justify-center gap-2">
                        <span className="w-4 h-4 border-2 border-white/40 border-t-white rounded-full animate-spin"/>
                        در حال تأیید...
                      </span>
                    : '✅ تأیید و ورود'}
                </button>

                {/* resend / change phone */}
                <div className="flex items-center justify-between text-xs">
                  <button
                    type="button"
                    onClick={() => { setStep('phone'); setOtp(''); setDevCode('') }}
                    className="text-ink-300 hover:text-ink-500 transition-colors"
                  >
                    ← تغییر شماره
                  </button>

                  {countdown > 0 ? (
                    <span className="text-ink-300 font-mono font-semibold">{fmt(countdown)}</span>
                  ) : (
                    <button
                      type="button"
                      onClick={handleResend}
                      disabled={requestOtp.isPending}
                      className="text-pet-500 font-semibold hover:text-pet-600 transition-colors disabled:opacity-50"
                    >
                      {requestOtp.isPending ? 'در حال ارسال...' : '🔄 ارسال مجدد'}
                    </button>
                  )}
                </div>
              </form>
            )}

          </div>
        </div>

        {/* footer */}
        <p className="text-center text-xs text-ink-300 mt-4">
          پت‌شاپ آنلاین — پوشش سراسری کشور 🐾
        </p>
      </div>
    </div>
  )
}
