import { useParams } from 'react-router-dom';
import { usePR, useUpdatePRStatus } from '../api/hooks';
import { PRStatus, UpdateStatusRequest } from '../api/types';
import { StatusChip } from '../components/StatusChip';
import { useForm } from 'react-hook-form';

export const PRDetail = () => {
    const { id } = useParams<{ id: string }>();
    const { data: pr, isLoading, error } = usePR(Number(id));
    const updateStatus = useUpdatePRStatus(Number(id));
    const { register, handleSubmit } = useForm<UpdateStatusRequest>();

    const onSubmit = (data: UpdateStatusRequest) => {
        updateStatus.mutate({ ...data, newStatus: Number(data.newStatus) });
    };

    if (isLoading) return <div>Loading...</div>;
    if (error) return <div>Error: {error.message}</div>;
    if (!pr) return <div>Problem Report not found</div>;

    return (
        <div style={{ display: 'grid', gridTemplateColumns: '2fr 1fr', gap: '2rem' }}>
            <div>
                <h2 style={{ fontSize: '1.5rem', fontWeight: 600, marginBottom: '1rem' }}>{pr.partNumber}</h2>
                <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '1rem', marginBottom: '1.5rem' }}>
                    <div><strong>Status:</strong> <StatusChip status={pr.status} /></div>
                    <div><strong>Priority:</strong> {pr.priority}</div>
                    <div><strong>Owner:</strong> {pr.ownerUpn}</div>
                    <div><strong>Opened:</strong> {new Date(pr.openedDate).toLocaleString()}</div>
                    <div><strong>SLA Due:</strong> {pr.slaDue ? new Date(pr.slaDue).toLocaleString() : 'N/A'}</div>
                    <div><strong>Reason Code:</strong> {pr.reasonCode}</div>
                </div>

                <h3 style={{ fontSize: '1.25rem', fontWeight: 600, marginTop: '2rem', marginBottom: '1rem' }}>Status History</h3>
                <div className="table-container">
                    <table className="table">
                        <thead>
                            <tr>
                                <th>Changed At</th>
                                <th>New Status</th>
                                <th>Changed By</th>
                                <th>Comments</th>
                            </tr>
                        </thead>
                        <tbody>
                            {pr.history.map(h => (
                                <tr key={h.changedAt}>
                                    <td>{new Date(h.changedAt).toLocaleString()}</td>
                                    <td><StatusChip status={h.newStatus} /></td>
                                    <td>{h.changedByUpn}</td>
                                    <td>{h.comments}</td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </div>
            <div>
                <h3 style={{ fontSize: '1.25rem', fontWeight: 600, marginBottom: '1rem' }}>Update Status</h3>
                <form onSubmit={handleSubmit(onSubmit)} style={{ display: 'flex', flexDirection: 'column', gap: '1rem' }}>
                    <select {...register('newStatus')} className="form-select">
                        {Object.keys(PRStatus).filter(k => !isNaN(Number(k))).map(key => (
                            <option key={key} value={key}>{PRStatus[Number(key)]}</option>
                        ))}
                    </select>
                    <textarea {...register('comments')} className="form-input" placeholder="Comments..." />
                    <button type="submit" className="btn btn-primary" disabled={updateStatus.isPending}>
                        {updateStatus.isPending ? 'Updating...' : 'Update'}
                    </button>
                    {updateStatus.isError && <div style={{ color: 'red' }}>{updateStatus.error.message}</div>}
                </form>
            </div>
        </div>
    );
};