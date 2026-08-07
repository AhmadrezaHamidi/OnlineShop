import { shopHttp } from '../shopClient'

export interface Category {
  id: number
  name: string
  parentId?: number
}

export interface ProductImage {
  id: number
  url: string
  isPrimary: boolean
  order: number
}

export interface Product {
  id: number
  name: string
  description: string
  price: number
  discountedPrice?: number
  categoryId: number
  categoryName: string
  isActive: boolean
  stock: number
  images: ProductImage[]
}

export interface Inventory {
  productId: number
  available: number
  reserved: number
  total: number
}

export const productsApi = {
  list: (categoryId?: number) =>
    shopHttp.get<Product[]>('/v1/Product/', { params: categoryId ? { categoryId } : {} }).then(r => r.data),

  get: (id: number) =>
    shopHttp.get<Product>(`/v1/Product/${id}`).then(r => r.data),

  create: (data: Partial<Product>) =>
    shopHttp.post<Product>('/v1/Product/', data).then(r => r.data),

  update: (id: number, data: Partial<Product>) =>
    shopHttp.put<Product>(`/v1/Product/${id}`, data).then(r => r.data),

  setPrice: (id: number, price: number) =>
    shopHttp.put(`/v1/Product/${id}/Price`, { price }).then(r => r.data),

  activate: (id: number) =>
    shopHttp.put(`/v1/Product/${id}/Activate`).then(r => r.data),

  deactivate: (id: number) =>
    shopHttp.put(`/v1/Product/${id}/Deactivate`).then(r => r.data),

  archive: (id: number) =>
    shopHttp.put(`/v1/Product/${id}/Archive`).then(r => r.data),

  getInventory: (id: number) =>
    shopHttp.get<Inventory>(`/v1/Product/${id}/Inventory`).then(r => r.data),

  replenish: (id: number, quantity: number) =>
    shopHttp.put(`/v1/Product/${id}/Inventory/Replenish`, { quantity }).then(r => r.data),

  getImages: (id: number) =>
    shopHttp.get<ProductImage[]>(`/v1/Product/${id}/Images`).then(r => r.data),

  addImage: (id: number, url: string) =>
    shopHttp.post(`/v1/Product/${id}/Images`, { url }).then(r => r.data),

  deleteImage: (id: number, imageId: number) =>
    shopHttp.delete(`/v1/Product/${id}/Images/${imageId}`).then(r => r.data),

  setPrimaryImage: (id: number, imageId: number) =>
    shopHttp.put(`/v1/Product/${id}/Images/${imageId}/Primary`).then(r => r.data),

  categories: () =>
    shopHttp.get<Category[]>('/v1/Product/Categories').then(r => r.data),

  createCategory: (name: string, parentId?: number) =>
    shopHttp.post('/v1/Product/Categories', { name, parentId }).then(r => r.data),
}
