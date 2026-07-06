import axios from 'axios'

export const API_VERSION = '1'
export const BASE_URL = import.meta.env.VITE_API_URL ?? 'https://localhost:7001'

const client = axios.create({
  baseURL: BASE_URL,
  headers: { 'Content-Type': 'application/json' },
})

// درج توکن در همه درخواست‌ها
client.interceptors.request.use((config) => {
  const token = localStorage.getItem('accessToken')
  if (token) config.headers.Authorization = `Bearer ${token}`
  return config
})

// رفرش توکن خودکار در صورت 401
client.interceptors.response.use(
  (res) => res,
  async (err) => {
    const original = err.config
    if (err.response?.status === 401 && !original._retry) {
      original._retry = true
      const refreshToken = localStorage.getItem('refreshToken')
      if (!refreshToken) {
        localStorage.clear()
        window.location.href = '/login'
        return Promise.reject(err)
      }
      try {
        const { data } = await axios.post(
          `${BASE_URL}/api/v${API_VERSION}/Identity/Auth/RefreshToken`,
          { refreshToken },
        )
        const { accessToken, refreshToken: newRefresh } = data.data ?? data
        localStorage.setItem('accessToken', accessToken)
        localStorage.setItem('refreshToken', newRefresh)
        original.headers.Authorization = `Bearer ${accessToken}`
        return client(original)
      } catch {
        localStorage.clear()
        window.location.href = '/login'
      }
    }
    return Promise.reject(err)
  },
)

export default client
