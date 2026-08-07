import { describe, it, expect } from 'vitest'
import { render, screen, waitFor } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import AdminDashboard from '../pages/admin/AdminDashboard'

function wrap(ui: React.ReactNode) {
  const qc = new QueryClient({ defaultOptions: { queries: { retry: false } } })
  return render(
    <QueryClientProvider client={qc}>
      <MemoryRouter><>{ui}</></MemoryRouter>
    </QueryClientProvider>
  )
}

describe('AdminDashboard', () => {
  it('shows داشبورد heading', () => {
    wrap(<AdminDashboard />)
    expect(screen.getByText('داشبورد')).toBeTruthy()
  })

  it('shows stats after load', async () => {
    wrap(<AdminDashboard />)
    await waitFor(() => expect(screen.getByText('نوبت امروز')).toBeTruthy())
  })
})
