import { describe, it, expect } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import SellerDashboard from '../../pages/shop/seller/SellerDashboard'

function wrap(ui: React.ReactNode) {
  const qc = new QueryClient({ defaultOptions: { queries: { retry: false } } })
  return render(
    <QueryClientProvider client={qc}>
      <MemoryRouter>{ui}</MemoryRouter>
    </QueryClientProvider>
  )
}

describe('SellerDashboard', () => {
  it('renders seller header', async () => {
    wrap(<SellerDashboard />)
    await waitFor(() => expect(screen.getByText('پنل فروشنده')).toBeTruthy())
  })

  it('shows stat cards after load', async () => {
    wrap(<SellerDashboard />)
    await waitFor(() => expect(screen.getByTestId('seller-stats')).toBeTruthy())
    expect(screen.getByText('سفارشات امروز')).toBeTruthy()
    expect(screen.getByText('محصولات فعال')).toBeTruthy()
    expect(screen.getByText('ناموجود')).toBeTruthy()
    expect(screen.getByText('فروش این ماه')).toBeTruthy()
  })

  it('computes todayOrders = 3 (all 3 mock orders are today)', async () => {
    wrap(<SellerDashboard />)
    await waitFor(() => expect(screen.getByTestId('stat-سفارشات امروز')).toBeTruthy())
    expect(screen.getByTestId('stat-سفارشات امروز').textContent).toBe('3')
  })

  it('computes activeProducts = 2 (products with isActive=true)', async () => {
    wrap(<SellerDashboard />)
    await waitFor(() => expect(screen.getByTestId('stat-محصولات فعال')).toBeTruthy())
    expect(screen.getByTestId('stat-محصولات فعال').textContent).toBe('2')
  })

  it('computes outOfStock = 1 (product with stock=0)', async () => {
    wrap(<SellerDashboard />)
    await waitFor(() => expect(screen.getByTestId('stat-ناموجود')).toBeTruthy())
    expect(screen.getByTestId('stat-ناموجود').textContent).toBe('1')
  })

  it('shows recent orders list', async () => {
    wrap(<SellerDashboard />)
    await waitFor(() => expect(screen.getByTestId('recent-orders')).toBeTruthy())
    expect(screen.getByText('علی رضایی')).toBeTruthy()
    expect(screen.getByText('مریم احمدی')).toBeTruthy()
  })

  it('shows link to orders page', async () => {
    wrap(<SellerDashboard />)
    await waitFor(() => expect(screen.getByText('مشاهده همه')).toBeTruthy())
  })
})
