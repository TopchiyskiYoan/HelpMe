import { useState } from 'react'
import { Link } from 'react-router-dom'
import { STATUS_LABELS, STATUS_COLORS } from '../utils/jobStatus.js'

export default function JobCard({ job }) {
  const [hovered, setHovered] = useState(false)

  const card = {
    border: `1px solid ${hovered ? '#fbbf24' : '#fde68a'}`,
    borderRadius: '12px',
    padding: '16px 20px',
    background: '#ffffff',
    display: 'flex',
    flexDirection: 'column',
    gap: '8px',
    textAlign: 'left',
    boxShadow: hovered
      ? '0 6px 20px rgba(217,119,6,0.16)'
      : '0 2px 8px rgba(217,119,6,0.08)',
    transform: hovered ? 'translateY(-1px)' : 'none',
    transition: 'all 0.18s ease',
    cursor: 'default',
  }

  const st = {
    top: {
      display: 'flex',
      justifyContent: 'space-between',
      alignItems: 'flex-start',
      gap: '1rem',
    },
    title: {
      fontSize: '15px',
      fontWeight: 700,
      color: '#1c1917',
      textDecoration: 'none',
      letterSpacing: '-0.01em',
      lineHeight: 1.3,
    },
    badge: (status) => ({
      fontSize: '11px',
      fontWeight: 600,
      padding: '3px 11px',
      borderRadius: '20px',
      whiteSpace: 'nowrap',
      flexShrink: 0,
      background: STATUS_COLORS[status]?.bg ?? '#fef3c7',
      color: STATUS_COLORS[status]?.color ?? '#d97706',
      border: `1px solid ${STATUS_COLORS[status]?.border ?? '#fde68a'}`,
    }),
    meta: {
      display: 'flex',
      flexWrap: 'wrap',
      gap: '10px',
      fontSize: '12px',
      color: '#78350f',
    },
    budget: {
      fontSize: '14px',
      fontWeight: 600,
      color: '#d97706',
    },
  }

  return (
    <div
      style={card}
      onMouseEnter={() => setHovered(true)}
      onMouseLeave={() => setHovered(false)}
    >
      <div style={st.top}>
        <Link to={`/jobs/${job.id}`} style={st.title}>{job.title}</Link>
        <span style={st.badge(job.status)}>{STATUS_LABELS[job.status] ?? job.status}</span>
      </div>
      <div style={st.meta}>
        {job.subCategoryName && <span>⚙ {job.subCategoryName}</span>}
        {job.cityName && <span>📍 {job.cityName}</span>}
        {job.clientName && <span>👤 {job.clientName}</span>}
      </div>
      {job.approximateBudget != null && (
        <div style={st.budget}>~{job.approximateBudget} лв.</div>
      )}
    </div>
  )
}
