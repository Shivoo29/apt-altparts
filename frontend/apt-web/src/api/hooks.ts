
// frontend/apt-web/src/api/hooks.ts
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { apiClient } from './client';
import { ProblemReportListItem, ProblemReportDetail, UpdateStatusRequest, PRStatus } from './types';

export const usePRs = (reasonCode?: string, status?: PRStatus, skip = 0, take = 100) => {
    return useQuery({
        queryKey: ['problem-reports', { reasonCode, status, skip, take }],
        queryFn: async () => {
            const response = await apiClient.get<ProblemReportListItem[]>('/prs', {
                params: { reasonCode, status, skip, take },
            });
            return response.data;
        },
    });
};

export const usePR = (id: number) => {
    return useQuery({
        queryKey: ['problem-report', id],
        queryFn: async () => {
            const response = await apiClient.get<ProblemReportDetail>(`/prs/${id}`);
            return response.data;
        },
    });
};

export const useUpdatePRStatus = (id: number) => {
    const queryClient = useQueryClient();
    return useMutation({
        mutationFn: async (data: UpdateStatusRequest) => {
            const response = await apiClient.patch(`/prs/${id}/status`, data);
            return response.data;
        },
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['problem-report', id] });
            queryClient.invalidateQueries({ queryKey: ['problem-reports'] });
        },
    });
};
