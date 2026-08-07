import DatePicker, { DateObject } from 'react-multi-date-picker'
import persian from 'react-date-object/calendars/persian'
import persian_fa from 'react-date-object/locales/persian_fa'

interface Props {
  value: string        // میلادی ISO: "2026-07-13"
  onChange: (iso: string) => void
  minDate?: string
  placeholder?: string
  className?: string
}

export default function PersianDatePicker({ value, onChange, minDate, placeholder = 'انتخاب تاریخ', className }: Props) {
  const dateObj = value ? new DateObject({ date: value, calendar: persian, locale: persian_fa }) : undefined

  return (
    <DatePicker
      value={dateObj}
      onChange={(d: DateObject | null) => {
        if (!d) return
        // تبدیل به میلادی ISO برای ارسال به بک‌اند
        const greg = d.convert('gregorian' as any)
        const y = greg.year
        const m = String(greg.month.number).padStart(2, '0')
        const day = String(greg.day).padStart(2, '0')
        onChange(`${y}-${m}-${day}`)
      }}
      calendar={persian}
      locale={persian_fa}
      minDate={minDate ? new DateObject({ date: minDate, calendar: persian, locale: persian_fa }) : undefined}
      inputClass={`input ${className ?? ''}`}
      placeholder={placeholder}
      style={{ width: '100%' }}
    />
  )
}
