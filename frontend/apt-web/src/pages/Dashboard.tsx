
// frontend/apt-web/src/pages/Dashboard.tsx
import { usePRs } from '../api/hooks';
import { PRStatus } from '../api/types';

const KpiCard = ({ title, value, className }: { title: string; value: string | number; className?: string }) => (
    <div className={`p-4 bg-white rounded-lg shadow-md ${className}`}>
        <h3 className="text-sm font-medium text-gray-500">{title}</h3>
        <p className="mt-1 text-3xl font-semibold text-gray-900">{value}</p>
    </div>
);

export const Dashboard = () => {
    const { data: prs, isLoading, error } = usePRs('ALT_PART', undefined, 0, 10000); // Fetch all PRs for dashboard

    if (isLoading) return <div>Loading Dashboard...</div>;
    if (error) return <div>Error: {error.message}</div>;

    const openPrs = prs?.filter(pr => pr.status !== PRStatus.Closed);
    const statusCounts = openPrs?.reduce((acc, pr) => {
        const statusName = PRStatus[pr.status];
        acc[statusName] = (acc[statusName] || 0) + 1;
        return acc;
    }, {} as Record<string, number>) ?? {};

    const now = new Date();
    const aging = {
        '<30 days': openPrs?.filter(pr => (now.getTime() - new Date(pr.openedDate).getTime()) / (1000 * 3600 * 24) < 30).length ?? 0,
        '30-60 days': openPrs?.filter(pr => {
            const age = (now.getTime() - new Date(pr.openedDate).getTime()) / (1000 * 3600 * 24);
            return age >= 30 && age < 60;
        }).length ?? 0,
        '>60 days': openPrs?.filter(pr => (now.getTime() - new Date(pr.openedDate).getTime()) / (1000 * 3600 * 24) >= 60).length ?? 0,
    };

    const slaBreached = openPrs?.filter(pr => pr.slaDue && new Date(pr.slaDue) < now).length ?? 0;
    const slaBreachPercentage = openPrs && openPrs.length > 0 ? ((slaBreached / openPrs.length) * 100).toFixed(1) : 0;

    return (
        <div>
            <h2 className="text-2xl font-bold mb-4">Dashboard</h2>
            <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
                <KpiCard title="Total Open PRs" value={openPrs?.length ?? 0} />
                <KpiCard title="SLA Breach" value={`${slaBreachPercentage}%`} />
            </div>
            <div className="grid grid-cols-1 gap-4 mt-4 sm:grid-cols-2">
                <div className="p-4 bg-white rounded-lg shadow-md">
                    <h3 className="text-sm font-medium text-gray-500">Open PRs by Status</h3>
                    <ul>
                        {Object.entries(statusCounts).map(([status, count]) => (
                            <li key={status} className="flex justify-between py-1">
                                <span>{status}</span>
                                <span>{count}</span>
                            </li>
                        ))}
                    </ul>
                </div>
                <div className="p-4 bg-white rounded-lg shadow-md">
                    <h3 className="text-sm font-medium text-gray-500">Aging Backlog</h3>
                    <ul>
                        {Object.entries(aging).map(([bucket, count]) => (
                            <li key={bucket} className="flex justify-between py-1">
                                <span>{bucket}</span>
                                <span>{count}</span>
                            </li>
                        ))}
                    </ul>
                </div>
            </div>
        </div>
    );
};
