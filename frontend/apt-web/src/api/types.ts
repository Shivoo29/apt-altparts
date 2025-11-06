export type PRStatus = 'New' | 'InAnalysis' | 'AwaitingDeviation' | 'Approved' | 'Implemented' | 'Closed'

export interface ProblemReportListItemDto {
  id: number
  partNumber: string
  reasonCode: string
  status: PRStatus
  ownerUpn?: string
  openedDate: string
  slaDue?: string
}

export interface ProblemReportDetailDto extends ProblemReportListItemDto {
  description?: string
  priority?: string
  closedDate?: string
  history: { changedAt: string; newStatus: PRStatus; changedByUpn: string; comments?: string }[]
}