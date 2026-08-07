import { kheirHttp } from './kheirkhabarClient'

export interface KheirUser {
  userId: number
  fullName: string
  email: string
}

export interface TokenResponse {
  accessToken: string
  refreshToken: string
  accessTokenExpiresAt: string
  refreshTokenExpiresAt: string
  userId: number
  fullName: string
  email: string
}

export const kheirAuthApi = {
  requestOtp: (phoneNumber: string) =>
    kheirHttp.post<boolean>('/v1/Identity/Auth/RequestOtp', { phoneNumber }).then(r => r.data),

  verifyOtp: (phoneNumber: string, code: string) =>
    kheirHttp.post<TokenResponse>('/v1/Identity/Auth/VerifyOtp', { phoneNumber, code }).then(r => r.data),

  refreshToken: (accessToken: string, refreshToken: string) =>
    kheirHttp.post<TokenResponse>('/v1/Identity/Auth/RefreshToken', { accessToken, refreshToken }).then(r => r.data),

  logout: (refreshToken: string) =>
    kheirHttp.post<boolean>('/v1/Identity/Auth/Logout', { refreshToken }).then(r => r.data),
}
