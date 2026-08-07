import { http } from './client'

export interface AuthUser {
  id: number; firstName: string; lastName: string
  phoneNumber: string; nationalId: string
  isPhoneVerified: boolean; role: 'User' | 'Admin' | 'Doctor'
}

export interface AuthResponse { token: string; expiresAt: string; user: AuthUser }

export const authApi = {
  login:    (phoneNumber: string) =>
    http.post<{ message: string }>('/Auth/login', { phoneNumber }).then(r => r.data),

  register: (data: { phoneNumber: string; firstName: string; lastName: string; nationalId: string }) =>
    http.post('/Auth/register', data).then(r => r.data),

  verify:   (phoneNumber: string, verificationCode: string) =>
    http.post<AuthResponse>('/Auth/verify-phone', { phoneNumber, verificationCode }).then(r => r.data),

  sendCode: (phoneNumber: string) =>
    http.post<{ message: string }>('/Auth/send-verification', { phoneNumber }).then(r => r.data),
}
