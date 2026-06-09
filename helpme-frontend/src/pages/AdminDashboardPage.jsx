import { useState, useEffect } from 'react'
import { Link } from 'react-router-dom'
import { api } from '../services/api.js'
import AdminLayout from '../components/AdminLayout.jsx'
import LoadingSpinner from '../components/LoadingSpinner.jsx'
import ErrorMessage from '../components/ErrorMessage.jsx'

const s = {
  title: {
    fontFamily: "'Syne', system-ui, sans-serif",
    fontSize: '24px', fontWeight: 800, color: '#1c1917',
    letterSpacing: '-0.03em', marginBottom: '6px',
  },
  sub: { fontSize: '13px', color: '#78350f', marginBottom: '2rem' },
  grid: {
    display: 'grid',
    gridTemplateColumns: 'repeat(auto-fit, minmax(160px, 1fr))',
    gap: '14px',
    marginBottom: '2rem',
  },
  card: {
    background: '#fff',
    border: '1px solid #fde68a',
    borderRadius: '12px',
    padding: '20px 18px',
    boxShadow: '0 1px 4px rgba(0,0,0,0.06)',
  },
  num: {
    fontFamily: "'Syne', system-ui, sans-serif",
    fontSize: '36px', fontWeight: 800, color: '#1c1917',
    letterSpacing: '-0.04em', lineHeight: 1,
  },
  numAmber: {
    fontFamily: "'Syne', system-ui, sans-serif",
    fontSize: '36px', fontWeight: 800, color: '#d97706',
    letterSpacing: '-0.04em', lineHeight: 1,
  },
  label: { fontSize: '12px', color: '#92400e', marginTop: '6px', fontWeight: 500 },
  sectionTitle: {
    fontFamily: "'Syne', system-ui, sans-serif",
    fontSize: '16px', fontWeight: 800, color: '#1c1917',
    letterSpacing: '-0.02em', marginBottom: '14px', marginTop: '0',
  },
  row2: { display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '20px' },
  panel: {
    background: '#fff', border: '1px solid #fde68a',
    borderRadius: '12px', padding: '20px',
    boxShadow: '0 1px 4px rgba(0,0,0,0.06)',
  },
  jobRow: {
    display: 'flex', justifyContent: 'space-between', alignItems: 'center',
    padding: '9px 0', borderBottom: '1px solid #fef3c7', fontSize: '13px', gap: '1rem',
  },
  jobTitle: { color: '#1c1917', fontWeight: 600, flex: 1, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' },
  badge: (status) => {
    const map = {
      Open: { bg: '#dbeafe', color: '#1d4ed8' },
      InProgress: { bg: '#d1fae5', color: '#059669' },
      Completed: { bg: '#f0fdf4', color: '#16a34a' },
      Cancelled: { bg: '#fee2e2', color: '#dc2626' },
      AwaitingConfirmation: { bg: '#fef3c7', color: '#d97706' },
    }
    const c = map[status] || { bg: '#f3f4f6', color: '#6b7280' }
    return { padding: '2px 8px', borderRadius: '20px', fontSize: '11px', fontWeight: 700, background: c.bg, color: c.color, flexShrink: 0 }
  },
  STATUS_LABEL: {
    Open: 'Отворена', InProgress: 'В процес', Completed: 'Завършена',
    Cancelled: 'Отказана', AwaitingConfirmation: 'Чаква',
  },
  quickLinks: { display: 'flex', flexWrap: 'wrap', gap: '10px', marginTop: '1rem' },
  ql: {
    padding: '9px 18px', borderRadius: '8px', border: '1px solid #fde68a',
    background: '#fffbf0', color: '#92400e', fontSize: '13px', fontWeight: 600,
    textDecoration: 'none', cursor: 'pointer',
  },
  stars: { color: '#f59e0b', fontSize: '14px' },
}

const LABEL = s.STATUS_LABEL

function StatCard({ num, label, amber }) {
  return (
    <div style={s.card}>
      <div style={amber ? s.numAmber : s.num}>{num}</div>
      <div style={s.label}>{label}</div>
    </div>
  )
}

export default function AdminDashboardPage() {
  const [stats, setStats] = useState(null)
  const [recentJobs, setRecentJobs] = useState([])
  const [recentReviews, setRecentReviews] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    Promise.all([
      api.get('/admin/stats'),
      api.get('/admin/jobs?page=1&sortDir=desc'),
      api.get('/admin/reviews?page=1&sortDir=desc'),
    ])
      .then(([st, jobs, reviews]) => {
        setStats(st)
        setRecentJobs((jobs.items ?? []).slice(0, 5))
        setRecentReviews((reviews.items ?? []).slice(0, 5))
      })
      .catch(() => setError('Грешка при зареждане на статистиките.'))
      .finally(() => setLoading(false))
  }, [])

  return (
    <AdminLayout>
      <div style={s.title}>Добре дошли в администрацията</div>
      <div style={s.sub}>Преглед на платформата в реално време</div>

      <ErrorMessage message={error} />
      {loading && <LoadingSpinner />}

      {!loading && !error && stats && (
        <>
          <div style={s.grid}>
            <StatCard num={stats.totalUsers} label="Общо потребители" />
            <StatCard num={stats.totalClients} label="Клиенти" />
            <StatCard num={stats.totalHandymen} label="Майстори" />
            <StatCard num={stats.verifiedHandymen} label="Верифицирани" amber />
            <StatCard num={stats.pendingVerifications} label="Чакат верификация" />
            <StatCard num={stats.totalJobs} label="Общо поръчки" />
            <StatCard num={stats.openJobs} label="Отворени поръчки" amber />
            <StatCard num={stats.inProgressJobs} label="В процес" />
            <StatCard num={stats.completedJobs} label="Завършени" />
            <StatCard num={stats.totalReviews} label="Отзиви" />
            <StatCard num={stats.averageRating > 0 ? `${stats.averageRating} ★` : '—'} label="Средна оценка" amber />
          </div>

          <div style={s.row2}>
            <div style={s.panel}>
              <div style={s.sectionTitle}>Последни поръчки</div>
              {recentJobs.length === 0 && <div style={{ fontSize: '13px', color: '#78350f' }}>Няма поръчки</div>}
              {recentJobs.map(j => (
                <div key={j.id} style={s.jobRow}>
                  <span style={s.jobTitle}>{j.title}</span>
                  <span style={s.badge(j.status)}>{LABEL[j.status] ?? j.status}</span>
                </div>
              ))}
              <div style={s.quickLinks}>
                <Link to="/admin/jobs" style={s.ql}>Всички поръчки →</Link>
              </div>
            </div>

            <div style={s.panel}>
              <div style={s.sectionTitle}>Последни отзиви</div>
              {recentReviews.length === 0 && <div style={{ fontSize: '13px', color: '#78350f' }}>Няма отзиви</div>}
              {recentReviews.map(r => (
                <div key={r.id} style={s.jobRow}>
                  <span style={s.jobTitle}>{r.clientName} → {r.handymanName}</span>
                  <span style={{ ...s.badge('Open'), background: '#fef3c7', color: '#92400e' }}>
                    {'★'.repeat(r.rating)}
                  </span>
                </div>
              ))}
              <div style={s.quickLinks}>
                <Link to="/admin/reviews" style={s.ql}>Всички отзиви →</Link>
              </div>
            </div>
          </div>

          <div style={{ marginTop: '1.5rem' }}>
            <div style={s.sectionTitle}>Бързи действия</div>
            <div style={s.quickLinks}>
              <Link to="/admin/users" style={s.ql}>Потребители</Link>
              <Link to="/admin/handymen/pending" style={s.ql}>Верификации ({stats.pendingVerifications})</Link>
              <Link to="/admin/jobs" style={s.ql}>Поръчки</Link>
              <Link to="/admin/reviews" style={s.ql}>Отзиви</Link>
            </div>
          </div>
        </>
      )}
    </AdminLayout>
  )
}
