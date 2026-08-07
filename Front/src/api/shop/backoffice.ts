import { shopHttp } from '../shopClient'

export interface Report {
  id: number
  type: string
  status: 'Pending' | 'Completed' | 'Failed'
  requestedAt: string
  completedAt?: string
  fileUrl?: string
}

export const backofficeApi = {
  listReports: () =>
    shopHttp.get<Report[]>('/v1/BackOffice/Reports').then(r => r.data),

  requestReport: (type: string) =>
    shopHttp.post<Report>('/v1/BackOffice/Reports', { type }).then(r => r.data),

  completeReport: (id: number) =>
    shopHttp.put(`/v1/BackOffice/Reports/${id}/Complete`).then(r => r.data),

  failReport: (id: number) =>
    shopHttp.put(`/v1/BackOffice/Reports/${id}/Fail`).then(r => r.data),
}
