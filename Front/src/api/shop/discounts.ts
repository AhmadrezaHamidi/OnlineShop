import { shopHttp } from '../shopClient'

export interface Discount {
  id: number
  code: string
  type: 'Percentage' | 'Fixed'
  value: number
  minOrderAmount?: number
  maxUsage?: number
  usedCount: number
  isActive: boolean
  expiresAt?: string
}

export const discountsApi = {
  list: () =>
    shopHttp.get<Discount[]>('/v1/Discounts').then(r => r.data),

  get: (id: number) =>
    shopHttp.get<Discount>(`/v1/Discounts/${id}`).then(r => r.data),

  getByCode: (code: string) =>
    shopHttp.get<Discount>(`/v1/Discounts/code/${code}`).then(r => r.data),

  create: (data: Partial<Discount>) =>
    shopHttp.post<Discount>('/v1/Discounts', data).then(r => r.data),

  activate: (id: number) =>
    shopHttp.put(`/v1/Discounts/${id}/activate`).then(r => r.data),

  deactivate: (id: number) =>
    shopHttp.put(`/v1/Discounts/${id}/deactivate`).then(r => r.data),

  apply: (code: string, orderId: number) =>
    shopHttp.post('/v1/Discounts/apply', { code, orderId }).then(r => r.data),
}
