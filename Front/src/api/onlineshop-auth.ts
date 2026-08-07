import { http } from './client'

export type UserRole = 'Customer' | 'Seller' | 'Admin'

export interface ShopUser {
  id: number
  phoneNumber: string
  firstName: string | null
  lastName: string | null
  role: UserRole
}

export interface TokenResponse {
  token: string
  refreshToken: string
  expiresAt: string
  user: ShopUser
}

export const shopAuthApi = {
  requestOtp: (phoneNumber: string) =>
    http.post<{ message: string; devCode?: string }>(
      '/v1/Identity/Auth/RequestOtp',
      { phoneNumber }
    ).then(r => r.data),

  verifyOtp: (phoneNumber: string, otp: string) =>
    http.post<TokenResponse>(
      '/v1/Identity/Auth/VerifyOtp',
      { phoneNumber, otp }
    ).then(r => r.data),

  refreshToken: (refreshToken: string) =>
    http.post<TokenResponse>(
      '/v1/Identity/Auth/RefreshToken',
      { refreshToken }
    ).then(r => r.data),

  logout: () =>
    http.post('/v1/Identity/Auth/Logout').then(r => r.data),
}
