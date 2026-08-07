import axios from 'axios'

export const shopHttp = axios.create({ baseURL: '/api' })

shopHttp.interceptors.request.use(cfg => {
  const token = localStorage.getItem('shop_token')
  if (token) cfg.headers.Authorization = `Bearer ${token}`
  return cfg
})

shopHttp.interceptors.response.use(
  r => r,
  err => {
    const msg = err.response?.data?.message ?? err.message ?? 'خطای سرور'
    return Promise.reject(new Error(msg))
  }
)
