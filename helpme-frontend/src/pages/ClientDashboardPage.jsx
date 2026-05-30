import { useState, useEffect } from 'react'
import { Link } from 'react-router-dom'
import { api } from '../services/api.js'
import JobCard from '../components/JobCard.jsx'

const s = {
  page: { padding: '2rem', flex: 1, background: '#fffbf0' },
  header: {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'center',
    marginBottom: '1.5rem',
    flexWrap: 'wrap',
    gap: '0.75rem',
  },
  title: {
    fontFamily: "'Syne', system-ui, sans-serif",
    fontSize: '24px',
    fontWeight: 800,
    color: '#1c1917',
    letterSpacing: '-0.03em',
  },
  btnNew: {
    padding: '9px 20px',
    borderRadius: '8px',
    border: 'none',
    background: 'linear-gradient(135deg, #d97706, #f59e0b)',
    color: '#fff',
    fontSize: '13px',
    fontWeight: 700,
    cursor: 'pointer',
    textDecoration: 'none',
    display: 'inline-flex',
    alignItems: 'center',
    gap: '4px',
    boxShadow: '0 3px 10px rgba(217,119,6,0.3)',
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
  error: {
    color: '#dc2626',
    fontSize: '14px',
    padding: '10px 14px',
    background: '#fee2e2',
    borderRadius: '8px',
    border: '1px solid #fecaca',
  },
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

  return (
    <div style={s.page}>
      <div style={s.header}>
        <div style={s.title}>Моите поръчки</div>
        <Link to="/jobs/create" style={s.btnNew}>+ Нова поръчка</Link>
      </div>

      {loading && <div style={s.empty}>Зареждане...</div>}
      {error && <div style={s.error}>{error}</div>}
      {!loading && !error && jobs.length === 0 && (
        <div style={s.empty}>
          <span style={s.emptyIcon}>🔧</span>
          Нямате поръчки все още.<br />
          <Link to="/jobs/create" style={{ color: '#d97706', fontWeight: 600 }}>
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
  )
}
