import { shopHttp } from '../shopClient'

export interface Profile {
  id: number
  phoneNumber: string
  firstName: string
  lastName: string
  email?: string
  nationalCode?: string
  address?: string
}

export const accountApi = {
  getProfile: () =>
    shopHttp.get<Profile>('/v1/Account/Profile').then(r => r.data),

  updateProfile: (data: Partial<Profile>) =>
    shopHttp.put('/v1/Account/Profile', data).then(r => r.data),
}
