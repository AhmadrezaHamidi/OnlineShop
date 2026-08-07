import { http } from './client'

export interface DoctorProfile {
  specialtyTitle: string; medicalLicenseNumber?: string; bio?: string; isActive: boolean
  user: { id: number; firstName: string; lastName: string; phoneNumber: string }
}
export interface TodayPatient {
  reservationId: number; patientId: number; patientName: string
  phone: string; specialty: string; startTime: string; endTime: string; status: string
}
export interface ScheduleDay { date: string; dayName: string; reservations: TodayPatient[] }

export const doctorApi = {
  profile: () => http.get<DoctorProfile>('/doctor/profile').then(r => r.data),

  today: () => http.get<TodayPatient[]>('/doctor/today').then(r => r.data),

  schedule: (from?: string) =>
    http.get<ScheduleDay[]>('/doctor/schedule', { params: from ? { from } : {} }).then(r => r.data),

  prescriptions: (patientId: number) =>
    http.get(`/doctor/patients/${patientId}/prescriptions`).then(r => r.data),

  writePrescription: (patientId: number, data: {
    content: string; diagnosis?: string; notes?: string
    reservationId?: number; nextVisitDate?: string
  }) => http.post(`/doctor/patients/${patientId}/prescriptions`, data).then(r => r.data),

  messages: (patientId: number) =>
    http.get(`/doctor/patients/${patientId}/messages`).then(r => r.data),

  notify: (patientId: number, message: string) =>
    http.post(`/doctor/patients/${patientId}/notify`, { message }).then(r => r.data),
}
