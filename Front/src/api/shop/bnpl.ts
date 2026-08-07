import { shopHttp } from '../shopClient'

export interface Installment {
  id: number
  dueDate: string
  amount: number
  isPaid: boolean
  paidAt?: string
}

export interface BnplContract {
  id: number
  userId: number
  orderId: number
  totalAmount: number
  installmentCount: number
  status: 'Active' | 'Completed' | 'Defaulted' | 'Cancelled'
  createdAt: string
  installments: Installment[]
}

export interface UserCredit {
  userId: number
  limit: number
  used: number
  available: number
  isBlocked: boolean
}

export const bnplApi = {
  createContract: (orderId: number, installmentCount: number) =>
    shopHttp.post<BnplContract>('/v1/Bnpl/Contracts', { orderId, installmentCount }).then(r => r.data),

  getContract: (id: number) =>
    shopHttp.get<BnplContract>(`/v1/Bnpl/Contracts/${id}`).then(r => r.data),

  getUserContracts: (userId: number) =>
    shopHttp.get<BnplContract[]>(`/v1/Bnpl/Users/${userId}/Contracts`).then(r => r.data),

  getInstallments: (contractId: number) =>
    shopHttp.get<Installment[]>(`/v1/Bnpl/Contracts/${contractId}/Installments`).then(r => r.data),

  payInstallment: (contractId: number, installmentId: number) =>
    shopHttp.post(`/v1/Bnpl/Contracts/${contractId}/Installments/${installmentId}/Pay`).then(r => r.data),

  defaultContract: (id: number) =>
    shopHttp.put(`/v1/Bnpl/Contracts/${id}/Default`).then(r => r.data),

  cancelContract: (id: number) =>
    shopHttp.put(`/v1/Bnpl/Contracts/${id}/Cancel`).then(r => r.data),

  getUserCredit: (userId: number) =>
    shopHttp.get<UserCredit>(`/v1/Bnpl/Users/${userId}/Credit`).then(r => r.data),

  increaseCredit: (userId: number, amount: number) =>
    shopHttp.put(`/v1/Bnpl/Users/${userId}/Credit/Increase`, { amount }).then(r => r.data),

  blockCredit: (userId: number) =>
    shopHttp.put(`/v1/Bnpl/Users/${userId}/Credit/Block`).then(r => r.data),

  releaseCredit: (userId: number) =>
    shopHttp.put(`/v1/Bnpl/Users/${userId}/Credit/Release`).then(r => r.data),
}
