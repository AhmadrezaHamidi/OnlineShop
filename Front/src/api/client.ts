import axios from 'axios'

export const BASE = '/api'

export const http = axios.create({ baseURL: BASE })

http.interceptors.request.use(cfg => {
  const token = localStorage.getItem('token')
  if (token) cfg.headers.Authorization = `Bearer ${token}`
  return cfg
})

http.interceptors.response.use(
  r => r,
  err => {
    const msg = err.response?.data?.message ?? err.message ?? 'خطای سرور'
    return Promise.reject(new Error(msg))
  }
)
