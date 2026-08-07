import { useState, useEffect, useRef } from 'react'
import { useNavigate } from 'react-router-dom'
import { useMutation } from '@tanstack/react-query'
import { kheirAuthApi } from '../../api/kheirkhabar-auth'
import { useKheirAuth } from '../../store/kheirkhabarAuth'
import OtpInput from '../../components/OtpInput'

type Step = 'phone' | 'otp'

const RESEND_SECONDS = 90

export default function KheirKhabarLoginPage() {
  const navigate = useNavigate()
  const { setAuth } = useKheirAuth()

  const [step, setStep] = useState<Step>('phone')
  const [phone, setPhone] = useState('')
  const [otp, setOtp] = useState('')
  const [countdown, setCountdown] = useState(0)
  const timerRef = useRef<ReturnType<typeof setInterval> | null>(null)

  useEffect(() => {
    if (countdown <= 0) {
      if (timerRef.current) clearInterval(timerRef.current)
      return
    }
    timerRef.current = setInterval(() => setCountdown(c => c - 1), 1000)
    return () => { if (timerRef.current) clearInterval(timerRef.current) }
  }, [countdown])

  useEffect(() => {
    const filled = otp.replace(/\s/g, '').length
    if (filled === 6 && step === 'otp') verify.mutate()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [otp])

  const requestOtp = useMutation({
    mutationFn: () => kheirAuthApi.requestOtp(phone),
    onSuccess: () => {
      setStep('otp')
      setCountdown(RESEND_SECONDS)
    },
  })

  const verify = useMutation({
    mutationFn: () => kheirAuthApi.verifyOtp(phone, otp.replace(/\s/g, '')),
    onSuccess: (data) => {
      setAuth(data.accessToken, data.refreshToken, {
        userId: data.userId,
        fullName: data.fullName,
        email: data.email,
      })
      navigate('/kheirkhabar')
    },
  })

  const handleResend = () => {
    setOtp('')
    requestOtp.mutate()
  }

  const fmt = (s: number) =>
    `${String(Math.floor(s / 60)).padStart(2, '0')}:${String(s % 60).padStart(2, '0')}`

  return (
    <div className="min-h-screen bg-kheir-50 flex items-center justify-center p-4" dir="rtl">
      <div className="w-full max-w-sm">
        <div className="bg-white rounded-3xl shadow-lg border border-kheir-100 overflow-hidden">

          <div className="bg-kheir-600 px-8 pt-10 pb-8 text-center">
            <div className="text-3xl mb-2" aria-hidden>🕌</div>
            <h1 className="text-2xl font-extrabold text-white leading-tight">
              {step === 'phone' ? 'خوش آمدید' : 'کد تأیید'}
            </h1>
            <p className="text-white/75 text-sm mt-1">
              {step === 'phone'
                ? 'برای ورود به خیرخبر شماره موبایل خود را وارد کنید'
                : `کد ارسال‌شده به ${phone} را وارد کنید`}
            </p>
          </div>

          <div className="px-8 py-7 space-y-5">
            {step === 'phone' && (
              <form onSubmit={e => { e.preventDefault(); requestOtp.mutate() }} className="space-y-4">
                <div>
                  <label className="block text-sm font-semibold text-ink-500 mb-2">شماره موبایل</label>
                  <input
                    type="tel"
                    placeholder="09xxxxxxxxx"
                    value={phone}
                    onChange={e => setPhone(e.target.value.replace(/\D/g, '').slice(0, 11))}
                    dir="ltr"
                    className="w-full px-4 py-3 rounded-2xl border-2 border-ink-100 bg-kheir-50
                               text-sm font-medium tracking-wide outline-none text-center
                               focus:border-kheir-400 focus:bg-white transition-all"
                    required
                  />
                </div>

                {requestOtp.error && (
                  <div className="bg-red-50 border border-red-100 rounded-xl px-4 py-2.5 text-red-600 text-sm">
                    {(requestOtp.error as Error).message}
                  </div>
                )}

                <button
                  type="submit"
                  disabled={requestOtp.isPending || phone.length < 11}
                  className="w-full bg-kheir-600 hover:bg-kheir-500 active:scale-[.98] disabled:opacity-50
                             text-white font-bold py-3.5 rounded-2xl transition-all text-sm"
                >
                  {requestOtp.isPending ? 'در حال ارسال...' : 'دریافت کد تأیید'}
                </button>

                {import.meta.env.DEV && (
                  <p className="text-center text-xs text-gold-500">
                    حالت توسعه: کد همیشه ۰۰۰۰۰۰ است
                  </p>
                )}
              </form>
            )}

            {step === 'otp' && (
              <form onSubmit={e => { e.preventDefault(); verify.mutate() }} className="space-y-5">
                <OtpInput value={otp} onChange={setOtp} disabled={verify.isPending} />

                {verify.error && (
                  <div className="bg-red-50 border border-red-100 rounded-xl px-4 py-2.5 text-red-600 text-sm">
                    {(verify.error as Error).message}
                  </div>
                )}

                <button
                  type="submit"
                  disabled={verify.isPending || otp.replace(/\s/g, '').length < 6}
                  className="w-full bg-kheir-600 hover:bg-kheir-500 active:scale-[.98] disabled:opacity-50
                             text-white font-bold py-3.5 rounded-2xl transition-all text-sm"
                >
                  {verify.isPending ? 'در حال تأیید...' : 'تأیید و ورود'}
                </button>

                <div className="flex items-center justify-between text-xs">
                  <button
                    type="button"
                    onClick={() => { setStep('phone'); setOtp('') }}
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
                      className="text-kheir-600 font-semibold hover:text-kheir-500 transition-colors disabled:opacity-50"
                    >
                      {requestOtp.isPending ? 'در حال ارسال...' : 'ارسال مجدد کد'}
                    </button>
                  )}
                </div>
              </form>
            )}
          </div>
        </div>
      </div>
    </div>
  )
}
