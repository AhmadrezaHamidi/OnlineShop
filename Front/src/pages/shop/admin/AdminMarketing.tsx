import { useState } from 'react'
import { useMutation } from '@tanstack/react-query'
import { marketingApi } from '../../../api/shop/marketing'

export default function AdminMarketing() {
  const [message, setMessage] = useState('')
  const [sent, setSent]       = useState(false)

  const broadcastMut = useMutation({
    mutationFn: () => marketingApi.broadcastSms(message),
    onSuccess: () => {
      setSent(true)
      setMessage('')
      setTimeout(() => setSent(false), 4000)
    },
  })

  const charCount = message.length
  const smsCount  = Math.ceil(charCount / 70) || 1

  return (
    <div className="space-y-5 max-w-xl">
      <h1 className="text-xl font-extrabold text-ink-900">بازاریابی — پیامک انبوه</h1>

      <div className="bg-honey-50 border border-honey-200 rounded-2xl p-4 flex gap-3">
        <span className="text-2xl shrink-0">⚠️</span>
        <p className="text-sm text-honey-700 font-medium leading-relaxed">
          این پیامک برای <strong>همه مشتریان</strong> ارسال می‌شود.
          قبل از ارسال متن را با دقت بررسی کنید.
        </p>
      </div>

      <div className="bg-white rounded-2xl border border-ink-100 shadow-sm p-4 space-y-4">
        <div>
          <label className="block text-xs text-ink-400 mb-2">متن پیامک</label>
          <textarea
            value={message}
            onChange={e => setMessage(e.target.value)}
            rows={5}
            placeholder="پیام خود را اینجا بنویسید…"
            className="w-full px-4 py-3 rounded-xl border border-ink-100 text-sm outline-none focus:border-pet-400 focus:ring-2 focus:ring-pet-100 resize-none transition-all"
            dir="rtl"
          />
          <div className="flex justify-between mt-1 text-xs text-ink-300">
            <span>{charCount} کاراکتر</span>
            <span>{smsCount} پیامک</span>
          </div>
        </div>

        {sent && (
          <div className="bg-forest-50 text-forest-600 text-sm font-semibold text-center py-3 rounded-xl">
            ✓ پیامک با موفقیت به همه مشتریان ارسال شد
          </div>
        )}

        {broadcastMut.isError && (
          <div className="bg-red-50 text-red-500 text-sm font-semibold text-center py-3 rounded-xl">
            ⚠️ خطا در ارسال پیامک
          </div>
        )}

        <button
          onClick={() => broadcastMut.mutate()}
          disabled={broadcastMut.isPending || !message.trim()}
          className="w-full py-3 rounded-xl bg-pet-500 text-white font-bold text-sm hover:bg-pet-600 active:scale-[.98] transition-all disabled:opacity-50"
        >
          {broadcastMut.isPending ? 'در حال ارسال…' : '📣 ارسال پیامک انبوه'}
        </button>
      </div>
    </div>
  )
}
