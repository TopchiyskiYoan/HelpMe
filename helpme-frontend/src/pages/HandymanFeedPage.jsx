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
    marginBottom: '24px', flexWrap: 'wrap', gap: '0.75rem',
  },
  titleBlock: {},
  title: {
    fontFamily: "'Syne', system-ui, sans-serif",
    fontSize: '26px', fontWeight: 800, color: t.text, letterSpacing: '-0.03em',
  },
  subtitle: { fontSize: '13px', color: t.textMuted, marginTop: '3px' },
  link: {
    fontSize: '13px', fontWeight: 600, color: t.amberDark, textDecoration: 'none',
    padding: '7px 14px', border: `1px solid ${t.amberBorder}`, borderRadius: t.radius,
    background: t.amberBg,
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
  notVerified: {
    padding: '16px 20px', borderRadius: t.radius,
    background: t.amberBg, border: `1px solid ${t.amberBorder}`,
    color: t.amberText, fontSize: '14px', fontWeight: 500,
  },
}

export default function HandymanFeedPage() {
  const [jobs, setJobs] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [notVerified, setNotVerified] = useState(false)

  useEffect(() => {
    api.get('/jobs/feed')
      .then(setJobs)
      .catch((err) => {
        if (err.status === 403) setNotVerified(true)
        else setError('Грешка при зареждане.')
      })
      .finally(() => setLoading(false))
  }, [])

  return (
    <div style={s.page}>
      <div style={s.inner}>
        <div style={s.header}>
          <div style={s.titleBlock}>
            <div style={s.title}>Налични поръчки</div>
            <div style={s.subtitle}>{jobs.length > 0 ? `${jobs.length} поръчки, съответстващи на профила ви` : 'Поръчки, подходящи за вашите услуги'}</div>
          </div>
          <Link to="/handymen/me/interests" style={s.link}>Моите оферти →</Link>
        </div>

        {loading && <div style={s.empty}>Зареждане...</div>}
        {error && <div style={s.error}>{error}</div>}
        {notVerified && (
          <div style={s.notVerified}>
            ⏳ Вашият профил е в процес на верификация. Ще получите известие, когато бъде одобрен.
          </div>
        )}
        {!loading && !notVerified && !error && jobs.length === 0 && (
          <div style={s.empty}>
            <span style={s.emptyIcon}>🔍</span>
            Няма отворени поръчки за вашите специалности в момента.<br />
            <span style={{ fontSize: '13px', color: t.textMuted }}>
              Уверете се, че сте добавили подкатегории и населените места, в които работите.
            </span>
          </div>
        )}
        {!loading && !notVerified && !error && (
          <div style={s.list}>
            {jobs.map(job => <JobCard key={job.id} job={job} />)}
          </div>
        )}
      </div>
    </div>
  )
}
