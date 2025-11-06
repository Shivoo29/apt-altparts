import { PRStatus } from '@api/types'

const colors: Record<PRStatus, string> = {
  New: 'gray',
  InAnalysis: 'blue',
  AwaitingDeviation: 'amber',
  Approved: 'green',
  Implemented: 'purple',
  Closed: 'gray'
}

export default function StatusChip({ status }: { status: PRStatus }) {
  return <span className={`badge ${colors[status]}`}>{status}</span>
}