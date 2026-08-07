import { http } from './client'

export const ReservationStatus = { Pending: 1, Confirmed: 2, Cancelled: 3, Completed: 4 } as const
export type ReservationStatusKey = keyof typeof ReservationStatus

export const adminApi = {
  // Dashboard
  today: () => http.get('/backoffice/today').then(r => r.data),
  weeklyReport: (from?: string) => http.get('/backoffice/weekly-report', { params: from ? { from } : {} }).then(r => r.data),

  // Patients
  patients: (q?: string) => http.get('/backoffice/patients', { params: q ? { q } : {} }).then(r => r.data),
  patient: (id: number) => http.get(`/backoffice/patients/${id}`).then(r => r.data),
  patientNotes: (id: number) => http.get(`/patients/${id}/notes`).then(r => r.data),
  addNote: (id: number, noteText: string, nextVisitDate?: string) =>
    http.post(`/patients/${id}/notes`, { noteText, nextVisitDate }).then(r => r.data),
  deleteNote: (patientId: number, noteId: number) =>
    http.delete(`/patients/${patientId}/notes/${noteId}`).then(r => r.data),

  // Reservations
  reservations: (params?: object) => http.get('/reservations/all', { params }).then(r => r.data),
  updateStatus: (id: number, status: ReservationStatusKey) =>
    http.patch(`/reservations/${id}/status`, { status: ReservationStatus[status] }).then(r => r.data),

  // Doctors
  doctors: () => http.get('/admin/doctors').then(r => r.data),
  addDoctor: (data: object) => http.post('/admin/doctors', data).then(r => r.data),
  toggleDoctor: (id: number) => http.patch(`/admin/doctors/${id}/toggle-active`).then(r => r.data),

  // Campaigns
  campaigns: () => http.get('/campaigns').then(r => r.data),
  createCampaign: (data: object) => http.post('/campaigns', data).then(r => r.data),
  sendCampaign: (id: number) => http.post(`/campaigns/${id}/send`).then(r => r.data),
  previewCampaign: (id: number) => http.get(`/campaigns/${id}/preview`).then(r => r.data),

  // Settings
  settings: () => http.get('/settings').then(r => r.data),
  setSetting: (key: string, value: string) => http.put(`/settings/${key}`, { value }).then(r => r.data),
  bulkSettings: (data: Record<string, string>) => http.put('/settings/bulk', data).then(r => r.data),

  // Working hours
  workingHours: (specialtyId: number) => http.get(`/workinghours/specialty/${specialtyId}`).then(r => r.data),
  addWorkingHours: (data: object) => http.post('/workinghours', data).then(r => r.data),
  updateWorkingHours: (id: number, data: object) => http.put(`/workinghours/${id}`, data).then(r => r.data),
  deleteWorkingHours: (id: number) => http.delete(`/workinghours/${id}`).then(r => r.data),

  // Holidays
  holidays: (from?: string) => http.get('/backoffice/holidays', { params: from ? { from } : {} }).then(r => r.data),
  addHoliday: (data: object) => http.post('/backoffice/holidays', data).then(r => r.data),
  deleteHoliday: (id: number) => http.delete(`/backoffice/holidays/${id}`).then(r => r.data),
  cancelDay: (data: object) => http.post('/backoffice/cancel-day', data).then(r => r.data),
}
