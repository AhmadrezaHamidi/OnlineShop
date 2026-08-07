import { describe, it, expect } from 'vitest'
import { render, screen, fireEvent, waitFor } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import CustomerOrders from '../../pages/shop/customer/CustomerOrders'

function wrap(ui: React.ReactNode) {
  const qc = new QueryClient({ defaultOptions: { queries: { retry: false } } })
  return render(
    <QueryClientProvider client={qc}>
      <MemoryRouter>{ui}</MemoryRouter>
    </QueryClientProvider>
  )
}

describe('CustomerOrders', () => {
  it('shows heading', async () => {
    wrap(<CustomerOrders />)
    await waitFor(() => expect(screen.getByText('سفارشات من')).toBeTruthy())
  })

  it('renders filter chips', async () => {
    wrap(<CustomerOrders />)
    await waitFor(() => {
      expect(screen.getByText('همه')).toBeTruthy()
      expect(screen.getByText('در انتظار')).toBeTruthy()
      expect(screen.getByText('تحویل شده')).toBeTruthy()
    })
  })

  it('lists 3 orders from API', async () => {
    wrap(<CustomerOrders />)
    await waitFor(() => {
      expect(screen.getByText('ORD-101')).toBeTruthy()
      expect(screen.getByText('ORD-102')).toBeTruthy()
      expect(screen.getByText('ORD-103')).toBeTruthy()
    })
  })

  it('expands order to show items on click', async () => {
    wrap(<CustomerOrders />)
    await waitFor(() => expect(screen.getByText('ORD-101')).toBeTruthy())
    fireEvent.click(screen.getByText('ORD-101').closest('button')!)
    await waitFor(() => expect(screen.getByText('غذای گربه')).toBeTruthy())
  })

  it('shows cancel button only for Pending orders', async () => {
    wrap(<CustomerOrders />)
    await waitFor(() => expect(screen.getByText('ORD-101')).toBeTruthy())
    fireEvent.click(screen.getByText('ORD-101').closest('button')!)
    await waitFor(() => expect(screen.getByText('لغو سفارش')).toBeTruthy())
  })

  it('filters by Pending status', async () => {
    wrap(<CustomerOrders />)
    await waitFor(() => expect(screen.getAllByText(/ORD-/).length).toBe(3))
    fireEvent.click(screen.getByText('در انتظار'))
    await waitFor(() => expect(screen.getAllByText(/ORD-/).length).toBe(1))
    expect(screen.getByText('ORD-101')).toBeTruthy()
  })

  it('shows empty state for Cancelled filter (no cancelled in mock)', async () => {
    wrap(<CustomerOrders />)
    await waitFor(() => expect(screen.getByText('لغو شده')).toBeTruthy())
    fireEvent.click(screen.getByText('لغو شده'))
    await waitFor(() => expect(screen.getByText('سفارشی در این دسته وجود ندارد')).toBeTruthy())
  })
})
