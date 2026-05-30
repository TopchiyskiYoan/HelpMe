import { useState, useEffect } from 'react'
import { Link } from 'react-router-dom'
import { api } from '../services/api.js'
import { STATUS_LABELS, STATUS_COLORS } from '../utils/jobStatus.js'

const s = {
  page: { padding: '2rem', flex: 1, background: '#fffbf0' },
  title: {
    fontFamily: "'Syne', system-ui, sans-serif",
    fontSize: '24px',
    fontWeight: 800,
    color: '#1c1917',
    letterSpacing: '-0.03em',
    marginBottom: '1.5rem',
  },
  list: { display: 'flex', flexDirection: 'column', gap: '0.75rem' },
  empty: {
    textAlign: 'center',
    padding: '4rem 2rem',
    color: '#78350f',
    fontSize: '15px',
    lineHeight: 1.7,
  },
  emptyIcon: { fontSize: '40px', marginBottom: '12px', display: 'block' },
}

export default function PendingConfirmationsPage() {
  const [interests, setInterests] = useState([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    api.get('/handymen/me/interests')
      .then(data => setInterests(data.filter(i => i.status === 'Selected')))
      .catch(() => {})
      .finally(() => setLoading(false))
  }, [])

  return (
    <div style={s.page}>
      <div style={s.title}>Чакащи потвърждение</div>

      {loading && <div style={s.empty}>Зареждане...</div>}
      {!loading && interests.length === 0 && (
        <div style={s.empty}>
          <span style={s.emptyIcon}>✅</span>
          Няма поръчки, чакащи вашето потвърждение.
        </div>
      )}
      {!loading && (
        <div style={s.list}>
          {interests.map(i => (
            <InterestCard key={i.id} interest={i} />
          ))}
        </div>
      )}
    </div>
  )
}

function InterestCard({ interest }) {
  const [hovered, setHovered] = useState(false)

  const card = {
    border: `1px solid ${hovered ? '#fbbf24' : '#fde68a'}`,
    borderRadius: '12px',
    padding: '16px 20px',
    background: '#ffffff',
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    gap: '1rem',
    flexWrap: 'wrap',
    textAlign: 'left',
    boxShadow: hovered ? '0 6px 20px rgba(217,119,6,0.14)' : '0 2px 8px rgba(217,119,6,0.07)',
    transition: 'all 0.18s ease',
    transform: hovered ? 'translateY(-1px)' : 'none',
  }

  return (
    <div
      style={card}
      onMouseEnter={() => setHovered(true)}
      onMouseLeave={() => setHovered(false)}
    >
      <div style={{ display: 'flex', flexDirection: 'column', gap: '4px' }}>
        <Link
          to={`/jobs/${interest.jobId}`}
          style={{ fontSize: '15px', fontWeight: 700, color: '#1c1917', letterSpacing: '-0.01em', textDecoration: 'none' }}
        >
          Поръчка #{interest.jobId}
        </Link>
        <div style={{ fontSize: '13px', color: '#78350f' }}>Вашата оферта: <strong>{interest.proposedPrice} лв.</strong></div>
      </div>
      <span style={{
        fontSize: '11px',
        fontWeight: 600,
        padding: '4px 12px',
        borderRadius: '20px',
        background: STATUS_COLORS['AwaitingConfirmation'].bg,
        color: STATUS_COLORS['AwaitingConfirmation'].color,
        border: `1px solid ${STATUS_COLORS['AwaitingConfirmation'].border}`,
        whiteSpace: 'nowrap',
      }}>
        {STATUS_LABELS['AwaitingConfirmation']}
      </span>
    </div>
  )
}
