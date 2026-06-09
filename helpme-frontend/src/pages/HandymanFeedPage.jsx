import { useState, useEffect, useMemo } from 'react'
import { Link } from 'react-router-dom'
import { api } from '../services/api.js'
import JobCard from '../components/JobCard.jsx'
import { t } from '../theme.js'

const s = {
  page: { ...t.fullPage },
  inner: { maxWidth: '860px', margin: '0 auto' },
  header: {
    display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start',
    marginBottom: '20px', flexWrap: 'wrap', gap: '0.75rem',
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
    background: t.amberBg, flexShrink: 0,
  },
  filterRow: {
    display: 'flex', gap: '10px', marginBottom: '20px', flexWrap: 'wrap',
  },
  search: {
    ...t.input, flex: 1, minWidth: '200px', maxWidth: '340px',
  },
  select: {
    ...t.input, width: 'auto', minWidth: '160px', cursor: 'pointer', appearance: 'auto',
  },
  count: {
    fontSize: '13px', color: t.textMuted, marginBottom: '14px',
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
  const [search, setSearch] = useState('')
  const [cityFilter, setCityFilter] = useState('')

  useEffect(() => {
    api.get('/jobs/feed')
      .then(setJobs)
      .catch((err) => {
        if (err.status === 403) setNotVerified(true)
        else setError('Грешка при зареждане.')
      })
      .finally(() => setLoading(false))
  }, [])

  const cities = useMemo(() => {
    const set = new Set(jobs.map(j => j.cityName).filter(Boolean))
    return [...set].sort()
  }, [jobs])

  const filtered = useMemo(() => {
    const q = search.toLowerCase().trim()
    return jobs.filter(j => {
      const matchCity = !cityFilter || j.cityName === cityFilter
      const matchSearch = !q ||
        j.title?.toLowerCase().includes(q) ||
        j.subCategoryName?.toLowerCase().includes(q) ||
        j.cityName?.toLowerCase().includes(q)
      return matchCity && matchSearch
    })
  }, [jobs, search, cityFilter])

  const focusIn = (e) => { e.target.style.borderColor = t.amber }
  const focusOut = (e) => { e.target.style.borderColor = t.border }

  return (
    <div style={s.page}>
      <div style={s.inner}>
        <div style={s.header}>
          <div style={s.titleBlock}>
            <div style={s.title}>Налични поръчки</div>
            <div style={s.subtitle}>Поръчки, подходящи за вашите услуги</div>
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

        {!loading && !notVerified && !error && (
          <>
            <div style={s.filterRow}>
              <input
                style={s.search}
                placeholder="Търси по заглавие или категория..."
                value={search}
                onChange={e => setSearch(e.target.value)}
                onFocus={focusIn} onBlur={focusOut}
              />
              <select style={s.select} value={cityFilter} onChange={e => setCityFilter(e.target.value)}
                onFocus={focusIn} onBlur={focusOut}>
                <option value="">Всички градове</option>
                {cities.map(c => <option key={c} value={c}>{c}</option>)}
              </select>
            </div>

            <div style={s.count}>
              {filtered.length} от {jobs.length} поръчки
            </div>

            {filtered.length === 0 ? (
              <div style={s.empty}>
                <span style={s.emptyIcon}>🔍</span>
                Няма намерени поръчки по зададените критерии.
              </div>
            ) : (
              <div style={s.list}>
                {filtered.map(job => <JobCard key={job.id} job={job} />)}
              </div>
            )}
          </>
        )}
      </div>
    </div>
  )
}
