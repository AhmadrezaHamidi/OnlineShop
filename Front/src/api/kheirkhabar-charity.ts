import { kheirHttp } from './kheirkhabarClient'
import type { PagedResult } from './kheirkhabar-news'

export enum CampaignCategory {
  Medical = 0,
  Educational = 1,
  Emergency = 2,
  General = 3,
}

export enum CampaignStatus {
  Active = 0,
  Completed = 1,
  Cancelled = 2,
}

export interface CampaignSummary {
  id: number
  title: string
  category: CampaignCategory
  goalAmount: number
  raisedAmount: number
  progressPercent: number
  coverImageUrl: string | null
  daysLeft: number
  status: CampaignStatus
  isFeatured: boolean
}

export interface Donation {
  id: string
  amount: number
  donorDisplayName: string
  createdAt: string
}

export interface CampaignDetail extends CampaignSummary {
  description: string
  endDate: string
  donorCount: number
  recentDonations: Donation[]
}

export interface DonateResponse {
  donationId: string
  amount: number
  newRaisedAmount: number
  progressPercent: number
}

export const charityApi = {
  list: (page = 1, pageSize = 20, category?: CampaignCategory, featuredOnly?: boolean) =>
    kheirHttp
      .get<PagedResult<CampaignSummary>>('/v1/Campaigns/', { params: { page, pageSize, category, featuredOnly } })
      .then(r => r.data),

  get: (id: number) =>
    kheirHttp.get<CampaignDetail>(`/v1/Campaigns/${id}`).then(r => r.data),

  donate: (id: number, amount: number, donorDisplayName: string) =>
    kheirHttp
      .post<DonateResponse>(`/v1/Campaigns/${id}/Donate`, { amount, donorDisplayName })
      .then(r => r.data),
}
