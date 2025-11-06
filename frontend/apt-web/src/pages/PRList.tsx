import { useState } from 'react';
import { Link } from 'react-router-dom';
import { usePRs } from '../api/hooks';
import { PRStatus } from '../api/types';
import { StatusChip } from '../components/StatusChip';

export const PRList = () => {
    const [status, setStatus] = useState<PRStatus | undefined>();
    const { data: prs, isLoading, error } = usePRs('ALT_PART', status);

    return (
        <div>
            <div className="filter-toolbar">
                <select className="form-select" onChange={(e) => setStatus(e.target.value ? Number(e.target.value) : undefined)} value={status ?? ''}>
                    <option value="">All Statuses</option>
                    {Object.keys(PRStatus).filter(k => !isNaN(Number(k))).map(key => (
                        <option key={key} value={key}>{PRStatus[Number(key)]}</option>
                    ))}
                </select>
            </div>

            <div className="table-container">
                <table className="table">
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Part Number</th>
                            <th>Status</th>
                            <th>Owner</th>
                            <th>Opened</th>
                            <th>SLA Due</th>
                        </tr>
                    </thead>
                    <tbody>
                        {isLoading && <tr><td colSpan={6}>Loading...</td></tr>}
                        {error && <tr><td colSpan={6}>Error: {error.message}</td></tr>}
                        {prs?.map(pr => (
                            <tr key={pr.id}>
                                <td><Link to={`/prs/${pr.id}`}>{pr.id}</Link></td>
                                <td>{pr.partNumber}</td>
                                <td><StatusChip status={pr.status} /></td>
                                <td>{pr.ownerUpn}</td>
                                <td>{new Date(pr.openedDate).toLocaleDateString()}</td>
                                <td>{pr.slaDue ? new Date(pr.slaDue).toLocaleDateString() : 'N/A'}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
};