import client, { API_VERSION } from './client'
import type { Product, Category, Inventory, PagedResult, ProductStatus } from '../types'

const base = `/api/v${API_VERSION}/Product`

export const productsApi = {
  // ── Queries ──────────────────────────────────────────────────────────
  getAll: (params?: { page?: number; pageSize?: number; search?: string; categoryId?: number; status?: ProductStatus }) =>
    client.get<{ data: PagedResult<Product> }>(base, { params }),

  getById: (id: number) =>
    client.get<{ data: Product }>(`${base}/${id}`),

  getInventory: (id: number) =>
    client.get<{ data: Inventory }>(`${base}/${id}/Inventory`),

  getImages: (id: number) =>
    client.get<{ data: { id: number; url: string; isPrimary: boolean }[] }>(`${base}/${id}/Images`),

  getCategories: () =>
    client.get<{ data: Category[] }>(`${base}/Categories`),

  // ── Commands ─────────────────────────────────────────────────────────
  create: (payload: { name: string; description: string; price: number; categoryId: number }) =>
    client.post<{ data: number }>(base, payload),

  update: (id: number, payload: Partial<Product>) =>
    client.put<{ data: number }>(`${base}/${id}`, payload),

  changePrice: (id: number, newPrice: number) =>
    client.patch(`${base}/${id}/Price`, { newPrice }),

  activate: (id: number) =>
    client.patch(`${base}/${id}/Activate`),

  deactivate: (id: number) =>
    client.patch(`${base}/${id}/Deactivate`),

  archive: (id: number) =>
    client.patch(`${base}/${id}/Archive`),

  // ── Inventory ─────────────────────────────────────────────────────────
  replenish: (id: number, quantity: number) =>
    client.post(`${base}/${id}/Inventory/Replenish`, { quantity }),

  reserve: (id: number, quantity: number) =>
    client.post(`${base}/${id}/Inventory/Reserve`, { quantity }),

  release: (id: number, quantity: number) =>
    client.post(`${base}/${id}/Inventory/Release`, { quantity }),

  confirmStock: (id: number, quantity: number) =>
    client.post(`${base}/${id}/Inventory/Confirm`, { quantity }),

  // ── Categories ────────────────────────────────────────────────────────
  createCategory: (name: string, parentId?: number) =>
    client.post(`${base}/Categories`, { name, parentId }),

  updateCategory: (id: number, name: string) =>
    client.put(`${base}/Categories/${id}`, { name }),
}
