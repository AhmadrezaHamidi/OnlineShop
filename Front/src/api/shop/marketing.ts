import { shopHttp } from '../shopClient'

export const marketingApi = {
  broadcastSms: (message: string) =>
    shopHttp.post('/v1/Marketing/sms/broadcast', { message }).then(r => r.data),
}
