import { useState } from 'react'
import { useMutation } from '@tanstack/react-query'
import { marketingApi } from '../../api/marketing'
import styles from './AdminMarketing.module.css'

export default function AdminMarketing() {
  const [message, setMessage] = useState('')
  const [result, setResult]   = useState<{ sent: number } | null>(null)

  const send = useMutation({
    mutationFn: () => marketingApi.sendBulkSms(message),
    onSuccess: ({ data }) => {
      setResult({ sent: data.data })
      setMessage('')
    },
  })

  return (
    <>
      <h2 style={{ fontSize: 20, fontWeight: 700, marginBottom: 4 }}>بازاریابی — پیامک انبوه</h2>

      <div className="card">
        <div className={styles.smsBadge}>
          <i className="ti ti-message" />
          SendBulkSms — ارسال به همه مشتریان
        </div>

        <div className={styles.field}>
          <label>متن پیامک</label>
          <textarea
            rows={5}
            placeholder="متن پیامک را اینجا بنویسید..."
            value={message}
            onChange={e => setMessage(e.target.value)}
            style={{ resize: 'vertical', borderRadius: 16 }}
          />
          <div className={styles.charCount}>{message.length} / ۱۶۰ کاراکتر</div>
        </div>

        {result && (
          <div className={styles.success}>
            <i className="ti ti-check" />
            پیامک با موفقیت به <strong>{result.sent.toLocaleString('fa-IR')}</strong> نفر ارسال شد
          </div>
        )}
        {send.isError && (
          <div className={styles.errorMsg}>
            <i className="ti ti-alert-circle" />
            ارسال ناموفق بود. دوباره تلاش کنید.
          </div>
        )}

        <button
          className="btn btn-primary"
          onClick={() => send.mutate()}
          disabled={!message.trim() || send.isPending}
          style={{ marginTop: 4 }}
        >
          {send.isPending
            ? <span className="spinner" style={{ width: 16, height: 16, borderWidth: 2 }} />
            : <i className="ti ti-send" />
          }
          ارسال پیامک انبوه
        </button>
      </div>
    </>
  )
}
