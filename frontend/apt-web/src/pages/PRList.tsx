import { useState } from 'react'
import { usePRs } from '@api/hooks'
import { PRStatus } from '@api/types'
import { Link } from 'react-router-dom'
import StatusChip from '@components/StatusChip'

const STATUSES: PRStatus[] = ['New','InAnalysis','AwaitingDeviation','Approved','Implemented','Closed']

export default function PRList() {
  const [status, setStatus] = useState<PRStatus | undefined>()
  const { data, isLoading, isError, error } = usePRs({ reasonCode: 'ALT_PART', status })

  return (
    <div>
      <div className="toolbar">
        <label>Status:&nbsp;</label>
        <select value={status ?? ''} onChange={e => setStatus((e.target.value || undefined) as PRStatus)}>
          <option value="">All</option>
          {STATUSES.map(s => <option key={s} value={s}>{s}</option>)}
        </select>
      </div>

      {isLoading && <div className="card">Loading…</div>}
      {isError && <div className="card" style={{color:'crimson'}}>Error: {(error as any)?.message ?? 'failed to load'}</div>}

      {data && (
        <table className="table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Part</th>
              <th>Status</th>
              <th>Owner</th>
              <th>Opened</th>
            </tr>
          </thead>
          <tbody>
            {data.map(pr => (
              <tr key={pr.id}>
                <td><Link className="mono" to={`/prs/${pr.id}`}>{pr.id}</Link></td>
                <td>{pr.partNumber}</td>
                <td><StatusChip status={pr.status} /></td>
                <td>{pr.ownerUpn ?? '-'}</td>
                <td>{new Date(pr.openedDate).toLocaleDateString()}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </div>
  )
}