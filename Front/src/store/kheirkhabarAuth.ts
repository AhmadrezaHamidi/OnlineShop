import { create } from 'zustand'
import { persist } from 'zustand/middleware'
import type { KheirUser } from '../api/kheirkhabar-auth'

interface KheirAuthState {
  token: string | null
  refreshToken: string | null
  user: KheirUser | null
  setAuth: (token: string, refreshToken: string, user: KheirUser) => void
  logout: () => void
}

export const useKheirAuth = create<KheirAuthState>()(
  persist(
    set => ({
      token: null,
      refreshToken: null,
      user: null,
      setAuth: (token, refreshToken, user) => {
        localStorage.setItem('kheirkhabar_token', token)
        set({ token, refreshToken, user })
      },
      logout: () => {
        localStorage.removeItem('kheirkhabar_token')
        set({ token: null, refreshToken: null, user: null })
      },
    }),
    { name: 'kheirkhabar-auth' }
  )
)
