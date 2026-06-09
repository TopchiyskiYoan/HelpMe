import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { STATUS_LABELS, STATUS_COLORS } from '../utils/jobStatus.js'
import { t } from '../theme.js'

export default function JobCard({ job }) {
  const [hovered, setHovered] = useState(false)
  const navigate = useNavigate()

  return (
    <div
      role="button"
      tabIndex={0}
      onClick={() => navigate(`/jobs/${job.id}`)}
      onKeyDown={(e) => e.key === 'Enter' && navigate(`/jobs/${job.id}`)}
      style={{
        border: `1px solid ${hovered ? t.amberBorder : t.border}`,
        borderRadius: t.radius,
        padding: '16px 20px',
        background: t.card,
        display: 'flex',
        flexDirection: 'column',
        gap: '10px',
        textAlign: 'left',
        boxShadow: hovered ? t.shadowMd : t.shadow,
        transform: hovered ? 'translateY(-1px)' : 'none',
        transition: 'all 0.18s ease',
        cursor: 'pointer',
      }}
      onMouseEnter={() => setHovered(true)}
      onMouseLeave={() => setHovered(false)}
    >
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', gap: '1rem' }}>
        <span style={{
          fontSize: '15px', fontWeight: 700, color: t.text,
          letterSpacing: '-0.01em', lineHeight: 1.3, flex: 1,
        }}>
          {job.title}
        </span>
        <span style={{
          fontSize: '11px', fontWeight: 600, padding: '3px 11px', borderRadius: t.radiusFull,
          whiteSpace: 'nowrap', flexShrink: 0,
          background: STATUS_COLORS[job.status]?.bg ?? t.amberBg,
          color: STATUS_COLORS[job.status]?.color ?? t.amberDark,
          border: `1px solid ${STATUS_COLORS[job.status]?.border ?? t.amberBorder}`,
        }}>
          {STATUS_LABELS[job.status] ?? job.status}
        </span>
      </div>

      <div style={{ display: 'flex', flexWrap: 'wrap', gap: '6px', alignItems: 'center' }}>
        {job.subCategoryName && (
          <span style={{ ...t.chip, fontSize: '12px' }}>⚙ {job.subCategoryName}</span>
        )}
        {job.cityName && (
          <span style={{ ...t.chip, fontSize: '12px' }}>📍 {job.cityName}</span>
        )}
        {job.clientName && (
          <span style={{ ...t.chip, fontSize: '12px' }}>👤 {job.clientName}</span>
        )}
        {job.approximateBudget != null && (
          <span style={{ ...t.chipAmber, fontSize: '13px', fontWeight: 700 }}>~{job.approximateBudget} лв.</span>
        )}
      </div>
    </div>
  )
}
