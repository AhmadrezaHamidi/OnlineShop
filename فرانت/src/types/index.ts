// ── Auth ──────────────────────────────────────────────────────────────────
export interface LoginResponse {
  accessToken: string
  refreshToken: string
  expiresIn: number
}

export interface User {
  id: number
  phoneNumber: string
  fullName: string
  role: 'Customer' | 'Seller' | 'Admin'
}

// ── Common ────────────────────────────────────────────────────────────────
export interface PagedResult<T> {
  items: T[]
  totalCount: number
  page: number
  pageSize: number
  totalPages: number
}

export interface ApiResponse<T> {
  data: T
  success: boolean
  message?: string
}

// ── Product ───────────────────────────────────────────────────────────────
export type ProductStatus = 'Active' | 'Inactive' | 'Archived'

export interface Product {
  id: number
  name: string
  description: string
  price: number
  categoryId: number
  categoryName: string
  status: ProductStatus
  sellerId: number
  createdAt: string
}

export interface Category {
  id: number
  name: string
  parentId?: number
}

export interface Inventory {
  productId: number
  available: number
  reserved: number
  total: number
}

// ── Order ─────────────────────────────────────────────────────────────────
export type OrderStatus =
  | 'Draft'
  | 'Placed'
  | 'Confirmed'
  | 'Shipped'
  | 'Delivered'
  | 'Cancelled'

export interface OrderItem {
  id: number
  productId: number
  productName: string
  quantity: number
  unitPrice: number
  totalPrice: number
}

export interface Order {
  id: number
  userId: number
  status: OrderStatus
  totalAmount: number
  items: OrderItem[]
  createdAt: string
}

// ── BNPL ──────────────────────────────────────────────────────────────────
export interface CreditInfo {
  userId: number
  creditLimit: number
  usedCredit: number
  availableCredit: number
  isBlocked: boolean
}

export interface BnplContract {
  id: number
  orderId: number
  userId: number
  totalAmount: number
  installmentCount: number
  status: 'Active' | 'Completed' | 'Defaulted' | 'Cancelled'
  createdAt: string
}
