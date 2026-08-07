import axios from 'axios'

export const kheirHttp = axios.create({ baseURL: '/kheirkhabar-api' })

kheirHttp.interceptors.request.use(cfg => {
  const token = localStorage.getItem('kheirkhabar_token')
  if (token) cfg.headers.Authorization = `Bearer ${token}`
  return cfg
})

kheirHttp.interceptors.response.use(
  r => r,
  err => {
    const msg = err.response?.data?.errors?.exception?.[0] ?? err.response?.data?.message ?? err.message ?? 'خطای سرور'
    return Promise.reject(new Error(msg))
  }
)
