import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../../hooks/useAuth'
import styles from './LoginPage.module.css'

export default function LoginPage() {
  const { requestOtp, verifyOtp } = useAuth()
  const navigate = useNavigate()

  const [step, setStep] = useState<'phone' | 'otp'>('phone')
  const [phone, setPhone] = useState('')
  const [otp, setOtp] = useState('')
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')

  async function handleSendOtp() {
    if (!/^09\d{9}$/.test(phone)) { setError('شماره موبایل معتبر نیست'); return }
    setError('')
    setLoading(true)
    try {
      await requestOtp(phone)
      setStep('otp')
    } catch {
      setError('ارسال کد ناموفق بود. دوباره تلاش کنید.')
    } finally { setLoading(false) }
  }

  async function handleVerify() {
    if (otp.length !== 6) { setError('کد ۶ رقمی وارد کنید'); return }
    setError('')
    setLoading(true)
    try {
      await verifyOtp(phone, otp)
      navigate('/')
    } catch {
      setError('کد اشتباه است یا منقضی شده.')
    } finally { setLoading(false) }
  }

  return (
    <div className={styles.page}>
      <div className={styles.card}>
        {/* Logo */}
        <div className={styles.logo}>
          <span className={styles.logoIcon}>🛍️</span>
          <h1>OnlineShop</h1>
          <p>ورود به سیستم</p>
        </div>

        {step === 'phone' ? (
          <>
            <div className={styles.field}>
              <label>شماره موبایل</label>
              <input
                type="tel"
                placeholder="09XXXXXXXXX"
                maxLength={11}
                value={phone}
                onChange={e => setPhone(e.target.value)}
                onKeyDown={e => e.key === 'Enter' && handleSendOtp()}
                dir="ltr"
                style={{ textAlign: 'center', letterSpacing: '3px', fontSize: '16px' }}
              />
            </div>
            {error && <p className={styles.error}>{error}</p>}
            <button className="btn btn-primary" style={{ width: '100%' }} onClick={handleSendOtp} disabled={loading}>
              {loading ? <span className="spinner" style={{ width: 18, height: 18, borderWidth: 2 }} /> : null}
              ارسال کد تأیید
            </button>
          </>
        ) : (
          <>
            <p className={styles.hint}>
              کد ۶ رقمی ارسال شده به <span dir="ltr">{phone}</span> را وارد کنید
            </p>
            <div className={styles.field}>
              <input
                type="text"
                inputMode="numeric"
                maxLength={6}
                placeholder="------"
                value={otp}
                onChange={e => setOtp(e.target.value.replace(/\D/g, ''))}
                onKeyDown={e => e.key === 'Enter' && handleVerify()}
                dir="ltr"
                style={{ textAlign: 'center', letterSpacing: '8px', fontSize: '20px' }}
                autoFocus
              />
            </div>
            {error && <p className={styles.error}>{error}</p>}
            <button className="btn btn-primary" style={{ width: '100%' }} onClick={handleVerify} disabled={loading}>
              {loading ? <span className="spinner" style={{ width: 18, height: 18, borderWidth: 2 }} /> : null}
              ورود
            </button>
            <button className="btn btn-ghost" style={{ width: '100%', marginTop: 8 }} onClick={() => { setStep('phone'); setOtp(''); setError('') }}>
              <i className="ti ti-arrow-right" /> بازگشت
            </button>
          </>
        )}
      </div>
    </div>
  )
}
