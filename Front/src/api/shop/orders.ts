import { shopHttp } from '../shopClient'

export type OrderStatus = 'Pending' | 'Processing' | 'Shipped' | 'Delivered' | 'Cancelled'
export type PaymentMethod = 'Online' | 'BNPL' | 'Cash'

export interface OrderItem {
  id: number
  productId: number
  productName: string
  quantity: number
  unitPrice: number
  totalPrice: number
}

export interface Payment {
  id: number
  method: PaymentMethod
  amount: number
  status: 'Pending' | 'Completed' | 'Failed'
}

export interface Order {
  id: number
  customerId: number
  customerName: string
  customerPhone: string
  sellerId?: number
  sellerName?: string
  status: OrderStatus
  totalAmount: number
  createdAt: string
  items: OrderItem[]
  payments: Payment[]
}

export const ordersApi = {
  list: () => shopHttp.get<Order[]>('/v1/Order/').then(r => r.data),
  get: (id: number) => shopHttp.get<Order>(`/v1/Order/${id}`).then(r => r.data),
  create: () => shopHttp.post<Order>('/v1/Order/').then(r => r.data),
  addItem: (orderId: number, productId: number, quantity: number) =>
    shopHttp.post(`/v1/Order/${orderId}/Items`, { productId, quantity }).then(r => r.data),
  removeItem: (orderId: number, itemId: number) =>
    shopHttp.delete(`/v1/Order/${orderId}/Items/${itemId}`).then(r => r.data),
  place: (id: number) => shopHttp.put(`/v1/Order/${id}/Place`).then(r => r.data),
  confirm: (id: number) => shopHttp.put(`/v1/Order/${id}/Confirm`).then(r => r.data),
  ship: (id: number) => shopHttp.put(`/v1/Order/${id}/Ship`).then(r => r.data),
  deliver: (id: number) => shopHttp.put(`/v1/Order/${id}/Deliver`).then(r => r.data),
  cancel: (id: number) => shopHttp.put(`/v1/Order/${id}/Cancel`).then(r => r.data),
  addPayment: (orderId: number, method: PaymentMethod, amount: number) =>
    shopHttp.post(`/v1/Order/${orderId}/Payments`, { method, amount }).then(r => r.data),
  completePayment: (orderId: number, paymentId: number) =>
    shopHttp.put(`/v1/Order/${orderId}/Payments/${paymentId}/Complete`).then(r => r.data),
  failPayment: (orderId: number, paymentId: number) =>
    shopHttp.put(`/v1/Order/${orderId}/Payments/${paymentId}/Fail`).then(r => r.data),
}
