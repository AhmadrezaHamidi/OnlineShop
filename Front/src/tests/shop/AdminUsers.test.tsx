import { describe, it, expect } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import AdminUsers from '../../pages/shop/admin/AdminUsers'

function wrap(ui: React.ReactNode) {
  const qc = new QueryClient({ defaultOptions: { queries: { retry: false } } })
  return render(
    <QueryClientProvider client={qc}>
      <MemoryRouter>{ui}</MemoryRouter>
    </QueryClientProvider>
  )
}

describe('AdminUsers', () => {
  it('renders heading', async () => {
    wrap(<AdminUsers />)
    await waitFor(() => expect(screen.getByText('مدیریت کاربران')).toBeTruthy())
  })

  it('shows 3 stat cards', async () => {
    wrap(<AdminUsers />)
    await waitFor(() => {
      expect(screen.getByText('کل کاربران')).toBeTruthy()
      // "فعال" appears in both stat card and table badges — use getAllByText
      expect(screen.getAllByText('فعال').length).toBeGreaterThanOrEqual(1)
      expect(screen.getAllByText('معلق').length).toBeGreaterThanOrEqual(1)
    })
  })

  it('lists all 4 users', async () => {
    wrap(<AdminUsers />)
    await waitFor(() => {
      expect(screen.getByText('علی رضایی')).toBeTruthy()
      expect(screen.getByText('مریم احمدی')).toBeTruthy()
      expect(screen.getByText('رضا کریمی')).toBeTruthy()
      expect(screen.getByText('سارا محمدی')).toBeTruthy()
    })
  })

  it('filters by search', async () => {
    wrap(<AdminUsers />)
    await waitFor(() => expect(screen.getByText('علی رضایی')).toBeTruthy())
    fireEvent.change(screen.getByPlaceholderText('جستجو نام یا شماره موبایل…'), {
      target: { value: 'مریم' },
    })
    await waitFor(() => expect(screen.getByText('مریم احمدی')).toBeTruthy())
    expect(screen.queryByText('علی رضایی')).toBeNull()
  })

  it('opens manage modal on click', async () => {
    wrap(<AdminUsers />)
    // wait for table to load before clicking
    await waitFor(() => expect(screen.getAllByText('مدیریت').length).toBeGreaterThan(0))
    fireEvent.click(screen.getAllByText('مدیریت')[0]) // علی رضایی — active user
    // "غیرفعال‌سازی" button only exists inside the modal, not in the table
    await waitFor(() => expect(screen.getByText('غیرفعال‌سازی')).toBeTruthy())
  })

  it('shows suspend button in modal for active user', async () => {
    wrap(<AdminUsers />)
    await waitFor(() => expect(screen.getAllByText('مدیریت')[0]).toBeTruthy())
    fireEvent.click(screen.getAllByText('مدیریت')[0]) // علی رضایی — active, not suspended
    await waitFor(() => expect(screen.getByText('تعلیق کاربر')).toBeTruthy())
  })
})
