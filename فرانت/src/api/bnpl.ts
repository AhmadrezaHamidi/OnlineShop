import client, { API_VERSION } from './client'
import type { CreditInfo, BnplContract } from '../types'

const base = `/api/v${API_VERSION}/Bnpl`

export const bnplApi = {
  getCreditLimit: (userId: number) =>
    client.get<{ data: CreditInfo }>(`${base}/Users/${userId}/Credit`),

  increaseCreditLimit: (userId: number, amount: number) =>
    client.put(`${base}/Users/${userId}/Credit/Increase`, { amount }),

  blockCredit: (userId: number, amount: number) =>
    client.patch(`${base}/Users/${userId}/Credit/Block`, { amount }),

  releaseCredit: (userId: number, amount: number) =>
    client.patch(`${base}/Users/${userId}/Credit/Release`, { amount }),

  createContract: (orderId: number, installmentCount: number) =>
    client.post<{ data: number }>(`${base}/Contracts`, { orderId, installmentCount }),

  getContract: (id: number) =>
    client.get<{ data: BnplContract }>(`${base}/Contracts/${id}`),

  getUserContracts: (userId: number) =>
    client.get<{ data: BnplContract[] }>(`${base}/Users/${userId}/Contracts`),

  getInstallments: (contractId: number) =>
    client.get(`${base}/Contracts/${contractId}/Installments`),

  payInstallment: (contractId: number, installmentId: number) =>
    client.post(`${base}/Contracts/${contractId}/Installments/${installmentId}/Pay`),

  cancelContract: (id: number) =>
    client.post(`${base}/Contracts/${id}/Cancel`),
}
