import { describe, it, expect } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import AdminShopDashboard from '../../pages/shop/admin/AdminShopDashboard'

function wrap(ui: React.ReactNode) {
  const qc = new QueryClient({ defaultOptions: { queries: { retry: false } } })
  return render(
    <QueryClientProvider client={qc}>
      <MemoryRouter>{ui}</MemoryRouter>
    </QueryClientProvider>
  )
}

describe('AdminShopDashboard', () => {
  it('renders dashboard heading', async () => {
    wrap(<AdminShopDashboard />)
    await waitFor(() => expect(screen.getByText('داشبورد 📊')).toBeTruthy())
  })

  it('shows 4 stat cards after load', async () => {
    wrap(<AdminShopDashboard />)
    await waitFor(() => expect(screen.getByTestId('admin-stats')).toBeTruthy())
    expect(screen.getByText('کاربران فعال')).toBeTruthy()
    expect(screen.getByText('سفارشات امروز')).toBeTruthy()
    expect(screen.getByText('در انتظار تأیید')).toBeTruthy()
    expect(screen.getByText('فروش امروز')).toBeTruthy()
  })

  it('computes activeUsers = 2 (isActive=true and isSuspended=false)', async () => {
    wrap(<AdminShopDashboard />)
    await waitFor(() => expect(screen.getByTestId('stat-کاربران فعال')).toBeTruthy())
    expect(screen.getByTestId('stat-کاربران فعال').textContent).toBe('2')
  })

  it('computes todayOrders = 3 (all mock orders are today)', async () => {
    wrap(<AdminShopDashboard />)
    await waitFor(() => expect(screen.getByTestId('stat-سفارشات امروز')).toBeTruthy())
    expect(screen.getByTestId('stat-سفارشات امروز').textContent).toBe('3')
  })

  it('computes pendingOrders = 1 (only order 101 is Pending)', async () => {
    wrap(<AdminShopDashboard />)
    await waitFor(() => expect(screen.getByTestId('stat-در انتظار تأیید')).toBeTruthy())
    expect(screen.getByTestId('stat-در انتظار تأیید').textContent).toBe('1')
  })

  it('renders weekly chart bars', async () => {
    wrap(<AdminShopDashboard />)
    await waitFor(() => expect(screen.getByTestId('weekly-chart')).toBeTruthy())
    const bars = screen.getByTestId('weekly-chart').querySelectorAll('[style]')
    expect(bars.length).toBe(7)
  })

  it('renders recent orders with customer names', async () => {
    wrap(<AdminShopDashboard />)
    await waitFor(() => expect(screen.getByTestId('admin-recent-orders')).toBeTruthy())
    expect(screen.getByText('علی رضایی')).toBeTruthy()
    expect(screen.getByText('مریم احمدی')).toBeTruthy()
    expect(screen.getByText('رضا کریمی')).toBeTruthy()
  })

  it('shows status badges on recent orders', async () => {
    wrap(<AdminShopDashboard />)
    await waitFor(() => expect(screen.getByTestId('admin-recent-orders')).toBeTruthy())
    expect(screen.getByText('در انتظار')).toBeTruthy()
    expect(screen.getByText('تحویل')).toBeTruthy()
    expect(screen.getByText('ارسال شد')).toBeTruthy()
  })
})
