import { create } from 'zustand'
import { persist } from 'zustand/middleware'
import type { ShopUser } from '../api/onlineshop-auth'

interface ShopAuthState {
  token: string | null
  refreshToken: string | null
  user: ShopUser | null
  setAuth: (token: string, refreshToken: string, user: ShopUser) => void
  logout: () => void
}

export const useShopAuth = create<ShopAuthState>()(
  persist(
    set => ({
      token: null,
      refreshToken: null,
      user: null,
      setAuth: (token, refreshToken, user) => {
        localStorage.setItem('shop_token', token)
        set({ token, refreshToken, user })
      },
      logout: () => {
        localStorage.removeItem('shop_token')
        set({ token: null, refreshToken: null, user: null })
      },
    }),
    { name: 'shop-auth' }
  )
)
