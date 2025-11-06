
// frontend/apt-web/src/components/StatusChip.tsx
import { PRStatus } from '../api/types';

interface StatusChipProps {
    status: PRStatus;
}

const statusStyles: Record<PRStatus, { text: string; bg: string; dot: string }> = {
    [PRStatus.New]: { text: 'text-blue-700', bg: 'bg-blue-100', dot: 'bg-blue-500' },
    [PRStatus.InAnalysis]: { text: 'text-yellow-700', bg: 'bg-yellow-100', dot: 'bg-yellow-500' },
    [PRStatus.AwaitingDeviation]: { text: 'text-purple-700', bg: 'bg-purple-100', dot: 'bg-purple-500' },
    [PRStatus.Approved]: { text: 'text-green-700', bg: 'bg-green-100', dot: 'bg-green-500' },
    [PRStatus.Implemented]: { text: 'text-indigo-700', bg: 'bg-indigo-100', dot: 'bg-indigo-500' },
    [PRStatus.Closed]: { text: 'text-gray-700', bg: 'bg-gray-100', dot: 'bg-gray-500' },
};

export const StatusChip = ({ status }: StatusChipProps) => {
    const style = statusStyles[status];
    const statusName = PRStatus[status];

    return (
        <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${style.bg} ${style.text}`}>
            <svg className={`-ml-0.5 mr-1.5 h-2 w-2 ${style.dot}`} fill="currentColor" viewBox="0 0 8 8">
                <circle cx="4" cy="4" r="3" />
            </svg>
            {statusName}
        </span>
    );
};
