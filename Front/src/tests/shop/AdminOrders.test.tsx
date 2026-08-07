import { describe, it, expect } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import AdminOrders from '../../pages/shop/admin/AdminOrders'

function wrap(ui: React.ReactNode) {
  const qc = new QueryClient({ defaultOptions: { queries: { retry: false } } })
  return render(
    <QueryClientProvider client={qc}>
      <MemoryRouter>{ui}</MemoryRouter>
    </QueryClientProvider>
  )
}

describe('AdminOrders', () => {
  it('renders heading', async () => {
    wrap(<AdminOrders />)
    await waitFor(() => expect(screen.getByText('مدیریت سفارشات')).toBeTruthy())
  })

  it('shows 4 stat cards', async () => {
    wrap(<AdminOrders />)
    await waitFor(() => {
      expect(screen.getByText('کل سفارشات')).toBeTruthy()
      expect(screen.getAllByText('در انتظار').length).toBeGreaterThanOrEqual(1)
      expect(screen.getAllByText('ارسال شده').length).toBeGreaterThanOrEqual(1)
      expect(screen.getByText('درآمد')).toBeTruthy()
    })
  })

  it('renders all 3 orders in table', async () => {
    wrap(<AdminOrders />)
    await waitFor(() => {
      expect(screen.getByText('ORD-101')).toBeTruthy()
      expect(screen.getByText('ORD-102')).toBeTruthy()
      expect(screen.getByText('ORD-103')).toBeTruthy()
    })
  })

  it('shows customer names in table', async () => {
    wrap(<AdminOrders />)
    await waitFor(() => {
      expect(screen.getByText('علی رضایی')).toBeTruthy()
      expect(screen.getByText('مریم احمدی')).toBeTruthy()
    })
  })

  it('opens detail modal on click', async () => {
    wrap(<AdminOrders />)
    // wait for table to load
    await waitFor(() => expect(screen.getAllByText('جزئیات').length).toBeGreaterThan(0))
    fireEvent.click(screen.getAllByText('جزئیات')[0])
    // modal shows "لغو سفارش" button for Pending order — unique to modal
    await waitFor(() => expect(screen.getByText('لغو سفارش')).toBeTruthy())
  })

  it('filters by Shipped status', async () => {
    wrap(<AdminOrders />)
    // wait for all 3 orders to load
    await waitFor(() => expect(screen.getAllByText('جزئیات').length).toBe(3))
    // use getByRole to target the filter chip button specifically (stats card also has "ارسال شده")
    fireEvent.click(screen.getByRole('button', { name: 'ارسال شده' }))
    // after filter: only ORD-103 (Shipped) stays, ORD-101 (Pending) disappears
    await waitFor(() => expect(screen.getAllByText('جزئیات').length).toBe(1))
    expect(screen.getByText('رضا کریمی')).toBeTruthy()   // customer of ORD-103
    expect(screen.queryByText('علی رضایی')).toBeNull()   // customer of ORD-101
  })

  it('searches by customer name', async () => {
    wrap(<AdminOrders />)
    await waitFor(() => expect(screen.getByText('ORD-101')).toBeTruthy())
    fireEvent.change(screen.getByPlaceholderText('جستجو شماره سفارش، مشتری، فروشنده…'), {
      target: { value: 'مریم' },
    })
    await waitFor(() => expect(screen.getByText('ORD-102')).toBeTruthy())
    expect(screen.queryByText('ORD-101')).toBeNull()
  })
})
