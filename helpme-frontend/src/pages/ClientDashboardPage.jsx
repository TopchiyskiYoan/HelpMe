import { useState, useEffect } from 'react'
import { Link } from 'react-router-dom'
import { api } from '../services/api.js'
import JobCard from '../components/JobCard.jsx'
import { t } from '../theme.js'

const s = {
  page: { ...t.fullPage },
  inner: { maxWidth: '860px', margin: '0 auto' },
  header: {
    display: 'flex', justifyContent: 'space-between', alignItems: 'center',
    marginBottom: '28px', flexWrap: 'wrap', gap: '0.75rem',
  },
  title: {
    fontFamily: "'Syne', system-ui, sans-serif",
    fontSize: '26px', fontWeight: 800, color: t.text, letterSpacing: '-0.03em',
  },
  btnNew: {
    ...t.btnPrimary, textDecoration: 'none', display: 'inline-flex',
    alignItems: 'center', gap: '5px', fontSize: '13.5px',
  },
  list: { display: 'flex', flexDirection: 'column', gap: '10px' },
  empty: {
    textAlign: 'center', padding: '5rem 2rem',
    color: t.textMuted, fontSize: '15px', lineHeight: 1.8,
  },
  emptyIcon: { fontSize: '44px', marginBottom: '14px', display: 'block' },
  error: {
    color: t.redText, fontSize: '14px', padding: '11px 14px',
    background: t.redBg, borderRadius: t.radius, border: `1px solid ${t.redBorder}`,
  },
  statsRow: {
    display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(140px, 1fr))',
    gap: '14px', marginBottom: '28px',
  },
  stat: {
    background: t.card, border: `1px solid ${t.border}`,
    borderRadius: t.radius, padding: '16px 18px',
    boxShadow: t.shadow,
  },
  statNum: {
    fontFamily: "'Syne', system-ui, sans-serif",
    fontSize: '28px', fontWeight: 800, color: t.text, letterSpacing: '-0.03em',
  },
  statLabel: { fontSize: '12px', color: t.textMuted, marginTop: '2px' },
}

const STATUS_LABELS = {
  Open: 'Отворени',
  InProgress: 'В процес',
  Completed: 'Завършени',
  Cancelled: 'Отменени',
  AwaitingConfirmation: 'Чакат потвърждение',
}

export default function ClientDashboardPage() {
  const [jobs, setJobs] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    api.get('/jobs/my')
      .then(setJobs)
      .catch(() => setError('Грешка при зареждане на поръчките.'))
      .finally(() => setLoading(false))
  }, [])

  const counts = jobs.reduce((acc, j) => ({ ...acc, [j.status]: (acc[j.status] || 0) + 1 }), {})

  return (
    <div style={s.page}>
      <div style={s.inner}>
        <div style={s.header}>
          <div style={s.title}>Моите поръчки</div>
          <Link to="/jobs/create" style={s.btnNew}>+ Нова поръчка</Link>
        </div>

        {!loading && !error && jobs.length > 0 && (
          <div style={s.statsRow}>
            {Object.entries(counts).map(([status, count]) => (
              <div key={status} style={s.stat}>
                <div style={s.statNum}>{count}</div>
                <div style={s.statLabel}>{STATUS_LABELS[status] ?? status}</div>
              </div>
            ))}
          </div>
        )}

        {loading && <div style={s.empty}>Зареждане...</div>}
        {error && <div style={s.error}>{error}</div>}
        {!loading && !error && jobs.length === 0 && (
          <div style={s.empty}>
            <span style={s.emptyIcon}>📋</span>
            Нямате поръчки все още.<br />
            <Link to="/jobs/create" style={{ color: t.amberDark, fontWeight: 600, textDecoration: 'none' }}>
              Публикувайте първата си поръчка →
            </Link>
          </div>
        )}
        {!loading && !error && (
          <div style={s.list}>
            {jobs.map(job => <JobCard key={job.id} job={job} />)}
          </div>
        )}
      </div>
    </div>
  )
}
