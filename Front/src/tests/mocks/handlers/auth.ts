import { http, HttpResponse } from 'msw'

export const authHandlers = [
  http.post('/api/Auth/send-verification', () =>
    HttpResponse.json({ message: 'کد تأیید (DEV): 123456' })
  ),
  http.post('/api/Auth/login', () =>
    HttpResponse.json({ message: 'کد تأیید (DEV): 123456' })
  ),
  http.post('/api/Auth/verify-phone', () =>
    HttpResponse.json({
      token: 'fake-jwt-token',
      user: { id: 1, firstName: 'احمد', lastName: 'تست', phoneNumber: '09123456789', role: 'User' }
    })
  ),
  http.post('/api/Auth/register', () =>
    HttpResponse.json({ message: 'ثبت‌نام موفق' })
  ),
]
