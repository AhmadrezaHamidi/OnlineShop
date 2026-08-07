import { useRef, KeyboardEvent, ClipboardEvent } from 'react'

interface Props {
  value: string
  onChange: (v: string) => void
  length?: number
  disabled?: boolean
}

export default function OtpInput({ value, onChange, length = 6, disabled }: Props) {
  const refs = useRef<(HTMLInputElement | null)[]>([])
  const digits = value.padEnd(length, '').split('').slice(0, length)

  const update = (idx: number, char: string) => {
    const arr = digits.map(d => d === ' ' ? '' : d)
    arr[idx] = char
    onChange(arr.join(''))
    if (char && idx < length - 1) refs.current[idx + 1]?.focus()
  }

  const handleKey = (idx: number, e: KeyboardEvent<HTMLInputElement>) => {
    if (e.key === 'Backspace') {
      e.preventDefault()
      if (digits[idx] && digits[idx] !== ' ') {
        update(idx, '')
      } else if (idx > 0) {
        refs.current[idx - 1]?.focus()
        update(idx - 1, '')
      }
    } else if (e.key === 'ArrowRight' && idx > 0) {
      refs.current[idx - 1]?.focus()
    } else if (e.key === 'ArrowLeft' && idx < length - 1) {
      refs.current[idx + 1]?.focus()
    }
  }

  const handlePaste = (e: ClipboardEvent) => {
    e.preventDefault()
    const text = e.clipboardData.getData('text').replace(/\D/g, '').slice(0, length)
    onChange(text.padEnd(length, ' ').slice(0, length))
    const focusIdx = Math.min(text.length, length - 1)
    refs.current[focusIdx]?.focus()
  }

  return (
    <div className="flex gap-2 justify-center" dir="ltr">
      {Array.from({ length }).map((_, i) => {
        const d = digits[i] === ' ' ? '' : digits[i]
        const filled = d !== ''
        return (
          <input
            key={i}
            ref={el => { refs.current[i] = el }}
            type="text"
            inputMode="numeric"
            maxLength={1}
            value={d}
            disabled={disabled}
            onChange={e => {
              const v = e.target.value.replace(/\D/g, '')
              if (v) update(i, v.slice(-1))
            }}
            onKeyDown={e => handleKey(i, e)}
            onPaste={handlePaste}
            onFocus={e => e.target.select()}
            className={[
              'w-12 h-14 text-center text-xl font-bold rounded-2xl border-2 outline-none transition-all',
              'text-ink-900 bg-pet-50',
              filled
                ? 'border-pet-500 bg-pet-100 shadow-pet'
                : 'border-ink-100 focus:border-pet-400 focus:bg-white focus:shadow-pet',
              disabled ? 'opacity-50 cursor-not-allowed' : '',
            ].join(' ')}
          />
        )
      })}
    </div>
  )
}
