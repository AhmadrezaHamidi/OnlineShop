import client, { API_VERSION } from './client'
import type { LoginResponse } from '../types'

const base = `/api/v${API_VERSION}/Identity/Auth`

export const authApi = {
  requestOtp: (phoneNumber: string) =>
    client.post(`${base}/RequestOtp`, { phoneNumber }),

  verifyOtp: (phoneNumber: string, otp: string) =>
    client.post<{ data: LoginResponse }>(`${base}/VerifyOtp`, { phoneNumber, otp }),

  refreshToken: (refreshToken: string) =>
    client.post<{ data: LoginResponse }>(`${base}/RefreshToken`, { refreshToken }),

  logout: (refreshToken: string) =>
    client.post(`${base}/Logout`, { refreshToken }),
}
