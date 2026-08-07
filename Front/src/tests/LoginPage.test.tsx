import { describe, it, expect } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import LoginPage from '../pages/auth/LoginPage'

function wrap(ui: React.ReactNode) {
  const qc = new QueryClient({ defaultOptions: { queries: { retry: false } } })
  return render(
    <QueryClientProvider client={qc}>
      <MemoryRouter><>{ui}</></MemoryRouter>
    </QueryClientProvider>
  )
}

describe('LoginPage', () => {
  it('renders phone input', () => {
    wrap(<LoginPage />)
    expect(screen.getByPlaceholderText('09xxxxxxxxx')).toBeTruthy()
  })

  it('shows otp step after send code', async () => {
    wrap(<LoginPage />)
    fireEvent.change(screen.getByPlaceholderText('09xxxxxxxxx'), { target: { value: '09123456789' } })
    fireEvent.click(screen.getByText('ارسال کد تأیید'))
    await waitFor(() => expect(screen.getByPlaceholderText('123456')).toBeTruthy())
  })

  it('shows dev otp code in amber box', async () => {
    wrap(<LoginPage />)
    fireEvent.change(screen.getByPlaceholderText('09xxxxxxxxx'), { target: { value: '09123456789' } })
    fireEvent.click(screen.getByText('ارسال کد تأیید'))
    await waitFor(() => expect(screen.getByText(/DEV: 123456/)).toBeTruthy())
  })
})
