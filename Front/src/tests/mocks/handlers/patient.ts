import { http, HttpResponse } from 'msw'

export const patientHandlers = [
  http.get('/api/specialties', () =>
    HttpResponse.json([
      { id: 1, name: 'عمومی', hasInstallments: false },
      { id: 2, name: 'دندانپزشکی', hasInstallments: true },
    ])
  ),
  http.get('/api/reservations/MyReserves', () =>
    HttpResponse.json([])
  ),
  http.get('/api/patient/prescriptions', () =>
    HttpResponse.json([])
  ),
]
