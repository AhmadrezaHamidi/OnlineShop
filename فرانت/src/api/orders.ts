import client, { API_VERSION } from './client'
import type { Order, OrderStatus, PagedResult } from '../types'

const base = `/api/v${API_VERSION}/Order`

export const ordersApi = {
  getAll: (params?: { page?: number; pageSize?: number; userId?: number; status?: OrderStatus }) =>
    client.get<{ data: PagedResult<Order> }>(base, { params }),

  getById: (id: number) =>
    client.get<{ data: Order }>(`${base}/${id}`),

  create: (payload: { userId: number; shippingAddress: string }) =>
    client.post<{ data: number }>(base, payload),

  addItem: (orderId: number, productId: number, quantity: number) =>
    client.post(`${base}/${orderId}/Items`, { productId, quantity }),

  removeItem: (orderId: number, itemId: number) =>
    client.delete(`${base}/${orderId}/Items/${itemId}`),

  place: (id: number) =>
    client.post(`${base}/${id}/Place`),

  confirm: (id: number) =>
    client.post(`${base}/${id}/Confirm`),

  ship: (id: number) =>
    client.post(`${base}/${id}/Ship`),

  deliver: (id: number) =>
    client.post(`${base}/${id}/Deliver`),

  cancel: (id: number, reason: string) =>
    client.post(`${base}/${id}/Cancel`, { reason }),

  recordPayment: (orderId: number, amount: number, method: string) =>
    client.post(`${base}/${orderId}/Payments`, { amount, method }),
}
