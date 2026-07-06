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
import SellerLayout     from './components/Layout/SellerLayout'
import SellerDashboard  from './pages/seller/SellerDashboard'
import SellerProducts   from './pages/seller/SellerProducts'
import SellerInventory  from './pages/seller/SellerInventory'
import SellerOrders     from './pages/seller/SellerOrders'

function ProtectedRoute({ children, role }: { children: React.ReactNode; role?: string }) {
  const { isAuthenticated, user } = useAuth()
  if (!isAuthenticated) return <Navigate to="/login" replace />
  if (role && user?.role !== role) return <Navigate to="/login" replace />
  return <>{children}</>
}

function DefaultRedirect() {
  const { isAuthenticated, user } = useAuth()
  if (!isAuthenticated) return <Navigate to="/login" replace />
  if (user?.role === 'Seller') return <Navigate to="/seller" replace />
  if (user?.role === 'Admin')  return <Navigate to="/admin"  replace />
  return <Navigate to="/login" replace />
}

function AppRoutes() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />

      {/* Admin */}
      <Route
        path="/admin"
        element={<ProtectedRoute role="Admin"><AdminLayout /></ProtectedRoute>}
      >
        <Route index           element={<AdminDashboard />} />
        <Route path="orders"   element={<AdminOrders />}    />
        <Route path="products" element={<AdminProducts />}  />
        <Route path="users"    element={<AdminUsers />}     />
        <Route path="bnpl"     element={<AdminBnpl />}      />
        <Route path="marketing"element={<AdminMarketing />} />
        <Route path="reports"  element={<AdminReports />}   />
      </Route>

      {/* Seller */}
      <Route
        path="/seller"
        element={<ProtectedRoute role="Seller"><SellerLayout /></ProtectedRoute>}
      >
        <Route index             element={<SellerDashboard />} />
        <Route path="products"   element={<SellerProducts />}  />
        <Route path="inventory"  element={<SellerInventory />} />
        <Route path="orders"     element={<SellerOrders />}    />
      </Route>

      <Route path="*" element={<DefaultRedirect />} />
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
