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
  link: {
    fontSize: '13px',
    fontWeight: 600,
    color: '#d97706',
    textDecoration: 'none',
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
    textAlign: 'center',
  },
}

export default function HandymanFeedPage() {
  const [jobs, setJobs] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  useEffect(() => {
    api.get('/jobs/feed')
      .then(setJobs)
      .catch(() => setError('Грешка при зареждане. Уверете се, че профилът ви е верифициран.'))
      .finally(() => setLoading(false))
  }, [])

  return (
    <div style={s.page}>
      <div style={s.header}>
        <div style={s.title}>Налични поръчки</div>
        <Link to="/handymen/me/interests" style={s.link}>Моите оферти →</Link>
      </div>

      {loading && <div style={s.empty}>Зареждане...</div>}
      {error && <div style={s.error}>{error}</div>}
      {!loading && !error && jobs.length === 0 && (
        <div style={s.empty}>
          <span style={s.emptyIcon}>📋</span>
          Няма отворени поръчки, съответстващи на вашия профил.<br />
          <span style={{ fontSize: '13px', color: '#78350f' }}>
            Уверете се, че сте добавили подкатегории и населени места в профила си.
          </span>
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
