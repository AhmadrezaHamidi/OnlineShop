import { createContext, useContext, useState, useCallback, type ReactNode } from 'react'
import { authApi } from '../api/auth'
import type { User } from '../types'

interface AuthState {
  user: User | null
  isAuthenticated: boolean
  role: User['role'] | null
}

interface AuthContextValue extends AuthState {
  requestOtp: (phone: string) => Promise<void>
  verifyOtp: (phone: string, otp: string) => Promise<void>
  logout: () => Promise<void>
}

const AuthContext = createContext<AuthContextValue | null>(null)

function parseJwt(token: string): Partial<User> {
  try {
    const payload = JSON.parse(atob(token.split('.')[1]))
    return {
      id: Number(payload.sub ?? payload.nameid),
      phoneNumber: payload.phone_number ?? payload.unique_name ?? '',
      fullName: payload.name ?? '',
      role: payload.role ?? 'Customer',
    }
  } catch {
    return {}
  }
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [state, setState] = useState<AuthState>(() => {
    const token = localStorage.getItem('accessToken')
    if (!token) return { user: null, isAuthenticated: false, role: null }
    const parsed = parseJwt(token)
    return {
      user: parsed as User,
      isAuthenticated: true,
      role: (parsed.role as User['role']) ?? null,
    }
  })

  const requestOtp = useCallback(async (phone: string) => {
    await authApi.requestOtp(phone)
  }, [])

  const verifyOtp = useCallback(async (phone: string, otp: string) => {
    const { data } = await authApi.verifyOtp(phone, otp)
    const tokens = data.data ?? (data as any)
    localStorage.setItem('accessToken', tokens.accessToken)
    localStorage.setItem('refreshToken', tokens.refreshToken)
    const parsed = parseJwt(tokens.accessToken)
    setState({
      user: parsed as User,
      isAuthenticated: true,
      role: (parsed.role as User['role']) ?? null,
    })
  }, [])

  const logout = useCallback(async () => {
    const rt = localStorage.getItem('refreshToken') ?? ''
    try { await authApi.logout(rt) } catch { /* ignore */ }
    localStorage.clear()
    setState({ user: null, isAuthenticated: false, role: null })
  }, [])

  return (
    <AuthContext.Provider value={{ ...state, requestOtp, verifyOtp, logout }}>
      {children}
    </AuthContext.Provider>
  )
}

export function useAuth() {
  const ctx = useContext(AuthContext)
  if (!ctx) throw new Error('useAuth must be inside AuthProvider')
  return ctx
}
