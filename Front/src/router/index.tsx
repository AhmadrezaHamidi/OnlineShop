import { createBrowserRouter, Navigate, Outlet } from 'react-router-dom'
import { useAuth } from '../store/auth'
import { useShopAuth } from '../store/shopAuth'
import LoginPage from '../pages/auth/LoginPage'
import RegisterPage from '../pages/auth/RegisterPage'

// ── KheirKhabar ──
import KheirKhabarLayout from '../pages/kheirkhabar/KheirKhabarLayout'
import KheirKhabarHome from '../pages/kheirkhabar/KheirKhabarHome'
import KheirKhabarLoginPage from '../pages/kheirkhabar/KheirKhabarLoginPage'

// ── Clinic pages ──
import PatientHome from '../pages/patient/PatientHome'
import PatientAppointments from '../pages/patient/PatientAppointments'
import PatientBook from '../pages/patient/PatientBook'
import PatientPrescriptions from '../pages/patient/PatientPrescriptions'
import PatientProfile from '../pages/patient/PatientProfile'
import DoctorHome from '../pages/doctor/DoctorHome'
import DoctorSchedule from '../pages/doctor/DoctorSchedule'
import DoctorPrescriptions from '../pages/doctor/DoctorPrescriptions'
import DoctorMessages from '../pages/doctor/DoctorMessages'
import AdminDashboard from '../pages/admin/AdminDashboard'
import AdminPatients from '../pages/admin/AdminPatients'
import AdminDoctors from '../pages/admin/AdminDoctors'
import AdminReservations from '../pages/admin/AdminReservations'
import AdminCampaigns from '../pages/admin/AdminCampaigns'
import AdminSettings from '../pages/admin/AdminSettings'
import AdminLayout from '../pages/admin/AdminLayout'
import PatientLayout from '../pages/patient/PatientLayout'
import DoctorLayout from '../pages/doctor/DoctorLayout'

// ── PetShop auth ──
import ShopLoginPage from '../pages/auth/ShopLoginPage'

// ── Customer ──
import CustomerLayout from '../pages/shop/customer/CustomerLayout'
import CustomerHome from '../pages/shop/customer/CustomerHome'
import CustomerProducts from '../pages/shop/customer/CustomerProducts'
import CustomerCart from '../pages/shop/customer/CustomerCart'
import CustomerOrders from '../pages/shop/customer/CustomerOrders'
import CustomerProfile from '../pages/shop/customer/CustomerProfile'

// ── Seller ──
import SellerLayout from '../pages/shop/seller/SellerLayout'
import SellerDashboard from '../pages/shop/seller/SellerDashboard'
import SellerOrders from '../pages/shop/seller/SellerOrders'

// ── Admin Shop ──
import AdminShopLayout from '../pages/shop/admin/AdminShopLayout'
import AdminShopDashboard from '../pages/shop/admin/AdminShopDashboard'
import AdminOrders from '../pages/shop/admin/AdminOrders'
import AdminUsers from '../pages/shop/admin/AdminUsers'
import AdminDiscounts from '../pages/shop/admin/AdminDiscounts'
import AdminBnpl from '../pages/shop/admin/AdminBnpl'
import AdminMarketing from '../pages/shop/admin/AdminMarketing'
import AdminReports from '../pages/shop/admin/AdminReports'

function RequireAuth({ role }: { role?: string }) {
  const { token, user } = useAuth()
  if (!token || !user) return <Navigate to="/login" replace />
  if (role && user.role !== role) return <Navigate to="/login" replace />
  return <Outlet />
}

function RequireShopAuth({ role }: { role?: string }) {
  const { token, user } = useShopAuth()
  if (!token || !user) return <Navigate to="/shop/login" replace />
  if (role && user.role !== role) return <Navigate to="/shop/login" replace />
  return <Outlet />
}

export const router = createBrowserRouter([
  { path: '/',        element: <Navigate to="/shop/login" replace /> },
  { path: '/login',   element: <LoginPage /> },
  { path: '/register',element: <RegisterPage /> },

  // ── KheirKhabar ──
  { path: '/kheirkhabar/login', element: <KheirKhabarLoginPage /> },
  {
    element: <KheirKhabarLayout />,
    children: [
      { path: '/kheirkhabar', element: <KheirKhabarHome /> },
    ],
  },

  // ── PetShop ──
  { path: '/shop/login', element: <ShopLoginPage /> },
  { path: '/shop',       element: <Navigate to="/shop/login" replace /> },

  {
    element: <RequireShopAuth role="Customer" />,
    children: [{
      element: <CustomerLayout />,
      children: [
        { path: '/shop/customer',          element: <CustomerHome /> },
        { path: '/shop/customer/products', element: <CustomerProducts /> },
        { path: '/shop/customer/cart',     element: <CustomerCart /> },
        { path: '/shop/customer/orders',   element: <CustomerOrders /> },
        { path: '/shop/customer/profile',  element: <CustomerProfile /> },
      ],
    }],
  },

  {
    element: <RequireShopAuth role="Seller" />,
    children: [{
      element: <SellerLayout />,
      children: [
        { path: '/shop/seller',           element: <SellerDashboard /> },
        { path: '/shop/seller/products',  element: <SellerDashboard /> },
        { path: '/shop/seller/orders',    element: <SellerOrders /> },
        { path: '/shop/seller/inventory', element: <SellerDashboard /> },
        { path: '/shop/seller/profile',   element: <SellerDashboard /> },
      ],
    }],
  },

  {
    element: <RequireShopAuth role="Admin" />,
    children: [{
      element: <AdminShopLayout />,
      children: [
        { path: '/shop/admin',           element: <AdminShopDashboard /> },
        { path: '/shop/admin/orders',    element: <AdminOrders /> },
        { path: '/shop/admin/products',  element: <AdminShopDashboard /> },
        { path: '/shop/admin/users',     element: <AdminUsers /> },
        { path: '/shop/admin/discounts', element: <AdminDiscounts /> },
        { path: '/shop/admin/bnpl',      element: <AdminBnpl /> },
        { path: '/shop/admin/marketing', element: <AdminMarketing /> },
        { path: '/shop/admin/reports',   element: <AdminReports /> },
      ],
    }],
  },

  // ── Clinic ──
  {
    element: <RequireAuth role="User" />,
    children: [{
      element: <PatientLayout />,
      children: [
        { path: '/patient',               element: <PatientHome /> },
        { path: '/patient/appointments',  element: <PatientAppointments /> },
        { path: '/patient/book',          element: <PatientBook /> },
        { path: '/patient/prescriptions', element: <PatientPrescriptions /> },
        { path: '/patient/profile',       element: <PatientProfile /> },
      ],
    }],
  },
  {
    element: <RequireAuth role="Doctor" />,
    children: [{
      element: <DoctorLayout />,
      children: [
        { path: '/doctor',               element: <DoctorHome /> },
        { path: '/doctor/schedule',      element: <DoctorSchedule /> },
        { path: '/doctor/prescriptions', element: <DoctorPrescriptions /> },
        { path: '/doctor/messages',      element: <DoctorMessages /> },
      ],
    }],
  },
  {
    element: <RequireAuth role="Admin" />,
    children: [{
      element: <AdminLayout />,
      children: [
        { path: '/admin',               element: <AdminDashboard /> },
        { path: '/admin/patients',      element: <AdminPatients /> },
        { path: '/admin/doctors',       element: <AdminDoctors /> },
        { path: '/admin/reservations',  element: <AdminReservations /> },
        { path: '/admin/campaigns',     element: <AdminCampaigns /> },
        { path: '/admin/settings',      element: <AdminSettings /> },
      ],
    }],
  },
])
