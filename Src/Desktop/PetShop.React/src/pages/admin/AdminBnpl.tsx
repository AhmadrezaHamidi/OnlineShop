import { useState } from 'react'
import { useMutation } from '@tanstack/react-query'
import { bnplApi } from '../../api/bnpl'
import type { CreditInfo } from '../../types'
import styles from './AdminBnpl.module.css'

export default function AdminBnpl() {
  const [userId, setUserId]   = useState('')
  const [credit, setCredit]   = useState<CreditInfo | null>(null)
  const [amount, setAmount]   = useState('')
  const [loading, setLoading] = useState(false)
  const [msg, setMsg]         = useState('')

  async function fetchCredit() {
    if (!userId) return
    setLoading(true)
    try {
      const { data } = await bnplApi.getCreditLimit(Number(userId))
      setCredit(data.data)
      setMsg('')
    } catch { setMsg('کاربر یافت نشد') }
    finally { setLoading(false) }
  }

  const increase = useMutation({
    mutationFn: () => bnplApi.increaseCreditLimit(Number(userId), Number(amount)),
    onSuccess: () => { setMsg('سقف اعتبار افزایش یافت'); fetchCredit() },
  })
  const block = useMutation({
    mutationFn: () => bnplApi.blockCredit(Number(userId), Number(amount)),
    onSuccess: () => { setMsg('اعتبار مسدود شد'); fetchCredit() },
  })
  const release = useMutation({
    mutationFn: () => bnplApi.releaseCredit(Number(userId), Number(amount)),
    onSuccess: () => { setMsg('اعتبار آزاد شد'); fetchCredit() },
  })

  return (
    <>
      <h2 style={{ fontSize: 20, fontWeight: 700, marginBottom: 4 }}>مدیریت اعتبار BNPL</h2>

      <div className="card">
        <h3 className={styles.sectionTitle}>جستجوی کاربر</h3>
        <div style={{ display: 'flex', gap: 10, marginBottom: 20 }}>
          <input
            placeholder="شناسه کاربر (User ID)"
            value={userId}
            onChange={e => setUserId(e.target.value)}
            onKeyDown={e => e.key === 'Enter' && fetchCredit()}
            style={{ width: 200 }}
            type="number"
          />
          <button className="btn btn-primary" onClick={fetchCredit} disabled={loading}>
            {loading ? <span className="spinner" style={{ width: 16, height: 16, borderWidth: 2 }} /> : <i className="ti ti-search" />}
            جستجو
          </button>
        </div>

        {msg && <p className={styles.msg}>{msg}</p>}

        {credit && (
          <div className={styles.creditCard}>
            <div className={styles.creditRow}>
              <div className={styles.creditItem}>
                <div className={styles.creditLabel}>سقف اعتبار</div>
                <div className={styles.creditVal}>{credit.creditLimit.toLocaleString('fa-IR')} تومان</div>
              </div>
              <div className={styles.creditItem}>
                <div className={styles.creditLabel}>اعتبار مصرف‌شده</div>
                <div className={styles.creditVal} style={{ color: '#DC2626' }}>{credit.usedCredit.toLocaleString('fa-IR')} تومان</div>
              </div>
              <div className={styles.creditItem}>
                <div className={styles.creditLabel}>اعتبار در دسترس</div>
                <div className={styles.creditVal} style={{ color: '#059669' }}>{credit.availableCredit.toLocaleString('fa-IR')} تومان</div>
              </div>
              <div className={styles.creditItem}>
                <div className={styles.creditLabel}>وضعیت</div>
                <span className={`pill ${credit.isBlocked ? 'pill-red' : 'pill-green'}`}>
                  {credit.isBlocked ? 'مسدود' : 'فعال'}
                </span>
              </div>
            </div>

            <div className={styles.actions}>
              <input
                type="number"
                placeholder="مبلغ (تومان)"
                value={amount}
                onChange={e => setAmount(e.target.value)}
                style={{ width: 180 }}
              />
              <button className="btn btn-ghost" onClick={() => increase.mutate()}>
                <i className="ti ti-plus" /> افزایش سقف
              </button>
              <button className="btn btn-danger" onClick={() => block.mutate()}>
                <i className="ti ti-lock" /> مسدود کردن
              </button>
              <button className="btn btn-teal" onClick={() => release.mutate()} disabled={!credit.isBlocked}>
                <i className="ti ti-lock-open" /> آزادسازی
              </button>
            </div>
          </div>
        )}
      </div>
    </>
  )
}
