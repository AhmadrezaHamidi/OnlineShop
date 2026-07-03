import client, { API_VERSION } from './client'

const base = `/api/v${API_VERSION}/Marketing`

export const marketingApi = {
  sendBulkSms: (message: string) =>
    client.post<{ data: number }>(`${base}/sms/broadcast`, { message }),
}
