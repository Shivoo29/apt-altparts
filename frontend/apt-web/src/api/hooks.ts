import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { api } from './client'
import { ProblemReportListItemDto, ProblemReportDetailDto, PRStatus } from './types'

export const usePRs = (params: { reasonCode?: string; status?: PRStatus; skip?: number; take?: number } = {}) =>
  useQuery({
    queryKey: ['prs', params],
    queryFn: async () => {
      const r = await api.get<ProblemReportListItemDto[]>('/prs', { params })
      return r.data
    }
  })

export const usePR = (id: number) =>
  useQuery({
    queryKey: ['pr', id],
    queryFn: async () => (await api.get<ProblemReportDetailDto>(`/prs/${id}`)).data
  })

export const useUpdatePRStatus = (id: number) => {
  const qc = useQueryClient()
  return useMutation({
    mutationFn: async (body: { newStatus: PRStatus; comments?: string }) =>
      (await api.patch(`/prs/${id}/status`, body)).data,
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['pr', id] })
      qc.invalidateQueries({ queryKey: ['prs'] })
    }
  })
}