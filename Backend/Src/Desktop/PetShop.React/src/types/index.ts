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
export type ProductStatus = 0 | 1 | 2

export const PRODUCT_STATUS_LABEL: Record<number, string> = {
  0: 'فعال', 1: 'غیرفعال', 2: 'آرشیو',
}
export const PRODUCT_STATUS_PILL: Record<number, string> = {
  0: 'pill-green', 1: 'pill-amber', 2: 'pill-gray',
}

export interface Product {
  id: number
  name: string
  description: string
  price: number
  categoryId: number
  categoryName?: string
  status: ProductStatus
  sellerId?: number
  createdAt?: string
  images?: { id: string; url: string; type: number }[]
  inventory?: { availableQuantity: number; quantity: number; reservedQuantity: number; isOutOfStock: boolean }
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
export type OrderStatus = 0 | 1 | 2 | 3 | 4 | 5

export const ORDER_STATUS_LABEL: Record<number, string> = {
  0: 'پیش‌نویس', 1: 'ثبت‌شده', 2: 'تأیید شده',
  3: 'ارسال شده', 4: 'تحویل شده', 5: 'لغو شده',
}
export const ORDER_STATUS_PILL: Record<number, string> = {
  0: 'pill-gray', 1: 'pill-purple', 2: 'pill-teal',
  3: 'pill-amber', 4: 'pill-green', 5: 'pill-red',
}

export interface OrderItem {
  id: number
  productId: number
  productName?: string
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
  placedAt?: string
  paymentMethod?: number
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
