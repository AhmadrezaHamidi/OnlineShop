import { shopHttp } from '../shopClient'

export interface AdminUser {
  id: number
  phoneNumber: string
  firstName: string
  lastName: string
  isActive: boolean
  isSuspended: boolean
  roles: string[]
  createdAt: string
}

export interface Role {
  id: number
  name: string
}

export const adminUsersApi = {
  list: () =>
    shopHttp.get<AdminUser[]>('/v1/Identity/Users').then(r => r.data),

  get: (id: number) =>
    shopHttp.get<AdminUser>(`/v1/Identity/Users/${id}`).then(r => r.data),

  roles: () =>
    shopHttp.get<Role[]>('/v1/Identity/Roles').then(r => r.data),

  addRole: (userId: number, roleId: number) =>
    shopHttp.post(`/v1/Identity/Users/${userId}/Roles/${roleId}`).then(r => r.data),

  removeRole: (userId: number, roleId: number) =>
    shopHttp.delete(`/v1/Identity/Users/${userId}/Roles/${roleId}`).then(r => r.data),

  activate: (id: number) =>
    shopHttp.put(`/v1/Identity/Users/${id}/Activate`).then(r => r.data),

  deactivate: (id: number) =>
    shopHttp.put(`/v1/Identity/Users/${id}/Deactivate`).then(r => r.data),

  suspend: (id: number) =>
    shopHttp.put(`/v1/Identity/Users/${id}/Suspend`).then(r => r.data),

  updateProfile: (id: number, data: Partial<AdminUser>) =>
    shopHttp.put(`/v1/Identity/Users/${id}/Profile`, data).then(r => r.data),
}
