import { http } from './client'

export interface Specialty { id: number; name: string; description?: string; hasInstallments: boolean }
export interface TimeSlot { startTime: string; endTime: string; isAvailable: boolean }
export interface Reservation {
  id: number; specialtyId: number; specialtyTitle?: string
  reservationDate: string; startTime: string; endTime: string
  status: 'Pending' | 'Confirmed' | 'Cancelled' | 'Completed'
  doctorName?: string; notes?: string; hasPrescription?: boolean; createdAt: string
}
export interface Prescription {
  id: number; content: string; diagnosis?: string; notes?: string
  issuedAt: string; nextVisitDate?: string; reservationId?: number
}
export interface DoctorMessage { id: number; messageText: string; sentAt: string; isDelivered: boolean }

export const patientApi = {
  specialties: () => http.get<Specialty[]>('/specialties').then(r => r.data),

  availableTimes: (date: string, specialtyId: number) =>
    http.get<TimeSlot[]>('/reservations/available-times', { params: { date, specialtyId } }).then(r => r.data),

  myReservations: (params?: { specialtyId?: number; status?: string; date?: string }) =>
    http.get<Reservation[]>('/reservations/MyReserves', { params }).then(r => r.data),

  book: (data: { specialtyId: number; reservationDate: string; startTime: string; endTime: string }) =>
    http.post<Reservation>('/reservations', data).then(r => r.data),

  cancel: (id: number) =>
    http.delete(`/reservations/${id}`).then(r => r.data),

  prescriptions: () =>
    http.get<Prescription[]>('/patient/prescriptions').then(r => r.data),

  messages: () =>
    http.get<DoctorMessage[]>('/patient/messages').then(r => r.data),
}
