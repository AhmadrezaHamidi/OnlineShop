import { kheirHttp } from './kheirkhabarClient'

export enum NewsCategory {
  General = 0,
  Social = 1,
  Charity = 2,
  Emergency = 3,
  Educational = 4,
}

export interface NewsArticleSummary {
  id: number
  title: string
  summary: string
  category: NewsCategory
  coverImageUrl: string | null
  viewCount: number
  publishedAt: string
}

export interface NewsArticleDetail extends NewsArticleSummary {
  body: string
  authorUserId: number
}

export interface PagedResult<T> {
  items: T[]
  totalCount: number
  page: number
  pageSize: number
  totalPages: number
  hasNextPage: boolean
  hasPreviousPage: boolean
}

export const newsApi = {
  list: (page = 1, pageSize = 20, category?: NewsCategory) =>
    kheirHttp
      .get<PagedResult<NewsArticleSummary>>('/v1/News/', { params: { page, pageSize, category } })
      .then(r => r.data),

  get: (id: number) =>
    kheirHttp.get<NewsArticleDetail>(`/v1/News/${id}`).then(r => r.data),

  incrementView: (id: number) =>
    kheirHttp.post<boolean>(`/v1/News/${id}/View`).then(r => r.data),
}
