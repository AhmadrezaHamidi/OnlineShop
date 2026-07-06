import { Navigate, Route, Routes } from 'react-router-dom'
import { AuthProvider, useAuth } from './hooks/useAuth'
import LoginPage        from './pages/auth/LoginPage'
import AdminLayout      from './components/Layout/AdminLayout'
import AdminDashboard   from './pages/admin/AdminDashboard'
import AdminOrders      from './pages/admin/AdminOrders'
import AdminProducts    from './pages/admin/AdminProducts'
import AdminUsers       from './pages/admin/AdminUsers'
import AdminBnpl        from './pages/admin/AdminBnpl'
import AdminMarketing   from './pages/admin/AdminMarketing'
import AdminReports     from './pages/admin/AdminReports'

function ProtectedRoute({ children }: { children: React.ReactNode }) {
  const { isAuthenticated } = useAuth()
  return isAuthenticated ? <>{children}</> : <Navigate to="/login" replace />
}

function AppRoutes() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route
        path="/admin"
        element={<ProtectedRoute><AdminLayout /></ProtectedRoute>}
      >
        <Route index           element={<AdminDashboard />} />
        <Route path="orders"   element={<AdminOrders />}    />
        <Route path="products" element={<AdminProducts />}  />
        <Route path="users"    element={<AdminUsers />}     />
        <Route path="bnpl"     element={<AdminBnpl />}      />
        <Route path="marketing"element={<AdminMarketing />} />
        <Route path="reports"  element={<AdminReports />}   />
      </Route>
      <Route path="*" element={<Navigate to="/admin" replace />} />
    </Routes>
  )
}

export default function App() {
  return (
    <AuthProvider>
      <AppRoutes />
    </AuthProvider>
  )
}
