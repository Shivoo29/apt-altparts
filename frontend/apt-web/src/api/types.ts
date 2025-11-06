
// frontend/apt-web/src/api/types.ts
export enum PRStatus { New, InAnalysis, AwaitingDeviation, Approved, Implemented, Closed }

export interface ProblemReportListItem {
    id: number;
    partNumber: string;
    reasonCode: string;
    status: PRStatus;
    ownerUpn: string;
    openedDate: string;
    slaDue: string;
}

export interface ProblemReportDetail {
    id: number;
    partNumber: string;
    description: string;
    reasonCode: string;
    status: PRStatus;
    priority: string;
    ownerUpn: string;
    openedDate: string;
    closedDate: string;
    slaDue: string;
    history: StatusHistoryItem[];
}

export interface StatusHistoryItem {
    changedAt: string;
    newStatus: PRStatus;
    changedByUpn: string;
    comments: string;
}

export interface UpdateStatusRequest {
    newStatus: PRStatus;
    comments: string;
}
