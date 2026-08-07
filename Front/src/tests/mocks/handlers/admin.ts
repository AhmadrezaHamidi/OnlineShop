import { http, HttpResponse } from 'msw'

export const adminHandlers = [
  http.get('/api/backoffice/today', () =>
    HttpResponse.json({ todayCount: 5, pendingCount: 2, confirmedCount: 3, reservations: [] })
  ),
  http.get('/api/backoffice/patients', () =>
    HttpResponse.json([])
  ),
  http.get('/api/admin/doctors', () =>
    HttpResponse.json([])
  ),
]
