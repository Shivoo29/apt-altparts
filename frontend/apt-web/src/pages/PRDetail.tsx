import { useParams, Link } from 'react-router-dom'
import { usePR, useUpdatePRStatus } from '@api/hooks'
import { PRStatus } from '@api/types'
import { useState } from 'react'
import StatusChip from '@components/StatusChip'

const STATUSES: PRStatus[] = ['New','InAnalysis','AwaitingDeviation','Approved','Implemented','Closed']

export default function PRDetail() {
  const { id } = useParams()
  const prId = Number(id)
  const { data, isLoading, isError } = usePR(prId)
  const update = useUpdatePRStatus(prId)
  const [newStatus, setNewStatus] = useState<PRStatus>('InAnalysis')
  const [comments, setComments] = useState('')

  if (isLoading) return <div className="card">Loading…</div>
  if (isError || !data) return <div className="card" style={{color:'crimson'}}>Failed to load PR</div>

  return (
    <div>
      <p><Link to="/">← Back</Link></p>

      <div className="card" style={{marginBottom:16}}>
        <h2 style={{marginTop:0}}>PR #{data.id} — {data.partNumber}</h2>
        <p><b>Reason:</b> {data.reasonCode}</p>
        <p><b>Status:</b> <StatusChip status={data.status} /></p>
        <p><b>Owner:</b> {data.ownerUpn ?? '-'} | <b>Opened:</b> {new Date(data.openedDate).toLocaleString()}</p>
        {data.slaDue && <p><b>SLA Due:</b> {new Date(data.slaDue).toLocaleDateString()}</p>}
      </div>

      <div className="card" style={{marginBottom:16}}>
        <h3 style={{marginTop:0}}>Update Status</h3>
        <div className="toolbar">
          <select value={newStatus} onChange={e => setNewStatus(e.target.value as PRStatus)}>
            {STATUSES.map(s => <option key={s} value={s}>{s}</option>)}
          </select>
          <input className="input" placeholder="Comments" value={comments} onChange={e => setComments(e.target.value)} />
          <button className="button" onClick={() => update.mutate({ newStatus, comments })}>Update</button>
        </div>
        {update.isError && <div style={{color:'crimson'}}>Update failed: {(update.error as any)?.response?.data?.error ?? 'Unknown error'}</div>}
        {update.isSuccess && <div style={{color:'green'}}>Status updated.</div>}
      </div>

      <div className="card">
        <h3 style={{marginTop:0}}>History</h3>
        <ul>
          {data.history.map((h, i) => (
            <li key={i}>
              {new Date(h.changedAt).toLocaleString()} → <b>{h.newStatus}</b> by {h.changedByUpn} {h.comments ? `(${h.comments})` : ''}
            </li>
          ))}
        </ul>
      </div>
    </div>
  )
}