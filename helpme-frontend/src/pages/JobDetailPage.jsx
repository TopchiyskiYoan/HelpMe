import { useState, useEffect, useCallback } from 'react'
import { useParams, useNavigate, Link } from 'react-router-dom'
import { api } from '../services/api.js'
import { useAuth } from '../context/AuthContext.jsx'
import InterestForm from '../components/InterestForm.jsx'
import InterestedHandymanCard from '../components/InterestedHandymanCard.jsx'
import ReviewForm from '../components/ReviewForm.jsx'
import { STATUS_LABELS, STATUS_COLORS } from '../utils/jobStatus.js'
import { t } from '../theme.js'

const s = {
  page: { ...t.page, maxWidth: '760px' },
  back: {
    fontSize: '13px', fontWeight: 500, color: t.amberDark,
    textDecoration: 'none', display: 'inline-flex', alignItems: 'center',
    gap: '6px', marginBottom: '24px', cursor: 'pointer',
    background: 'none', border: 'none', padding: 0, fontFamily: 'inherit',
  },
  card: {
    background: t.card, border: `1px solid ${t.border}`,
    borderRadius: t.radiusLg, padding: '24px',
    boxShadow: t.shadow, marginBottom: '16px',
  },
  headerRow: {
    display: 'flex', justifyContent: 'space-between',
    alignItems: 'flex-start', gap: '1rem', marginBottom: '14px', flexWrap: 'wrap',
  },
  title: {
    fontFamily: "'Syne', system-ui, sans-serif",
    fontSize: '24px', fontWeight: 800, color: t.text,
    letterSpacing: '-0.03em', flex: 1,
  },
  badge: (status) => ({
    fontSize: '12px', fontWeight: 600, padding: '5px 14px',
    borderRadius: t.radiusFull, flexShrink: 0,
    background: STATUS_COLORS[status]?.bg ?? t.amberBg,
    color: STATUS_COLORS[status]?.color ?? t.amberDark,
    border: `1px solid ${STATUS_COLORS[status]?.border ?? t.amberBorder}`,
    whiteSpace: 'nowrap',
  }),
  metaRow: { display: 'flex', flexWrap: 'wrap', gap: '7px', marginBottom: '18px' },
  desc: {
    fontSize: '15px', color: t.text, lineHeight: 1.7,
    padding: '16px 18px', background: t.bg,
    border: `1px solid ${t.border}`, borderRadius: t.radius,
  },
  sectionCard: {
    background: t.card, border: `1px solid ${t.border}`,
    borderRadius: t.radiusLg, padding: '20px',
    boxShadow: t.shadow, marginBottom: '14px',
  },
  sectionTitle: {
    fontFamily: "'Syne', system-ui, sans-serif",
    fontSize: '16px', fontWeight: 700, color: t.text,
    marginBottom: '14px', letterSpacing: '-0.01em',
  },
  actions: { display: 'flex', gap: '10px', flexWrap: 'wrap', marginTop: '16px' },
  btnDanger: { ...t.btnDanger },
  btnSuccess: {
    padding: '10px 18px', borderRadius: '8px', border: 'none',
    background: '#16a34a', color: '#fff', fontSize: '14px',
    fontWeight: 600, cursor: 'pointer', boxShadow: '0 2px 6px rgba(22,163,74,0.3)',
  },
  btnPrimary: { ...t.btnPrimary },
  btnSecondary: { ...t.btnSecondary },
  interestsList: { display: 'flex', flexDirection: 'column', gap: '10px' },
  empty: { fontSize: '14px', color: t.textMuted, padding: '4px 0' },
  error: {
    fontSize: '13px', color: t.redText, marginBottom: '14px',
    padding: '11px 14px', background: t.redBg,
    border: `1px solid ${t.redBorder}`, borderRadius: t.radius,
  },
  alreadyInterested: {
    padding: '13px 16px', borderRadius: t.radius,
    background: t.greenBg, border: `1px solid ${t.greenBorder}`,
    color: t.greenText, fontSize: '14px', fontWeight: 500,
  },
  alreadyReviewed: {
    padding: '13px 16px', borderRadius: t.radius,
    background: t.amberBg, border: `1px solid ${t.amberBorder}`,
    color: t.amberText, fontSize: '14px', fontWeight: 500,
  },
  loading: { ...t.fullPage, display: 'flex', alignItems: 'center', justifyContent: 'center', color: t.textMuted },
}

export default function JobDetailPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const { user } = useAuth()
  const [job, setJob] = useState(null)
  const [interests, setInterests] = useState([])
  const [loading, setLoading] = useState(true)
  const [actionError, setActionError] = useState('')
  const [interestSubmitted, setInterestSubmitted] = useState(false)
  const [hasReview, setHasReview] = useState(false)

  const load = useCallback(async () => {
    try {
      const j = await api.get(`/jobs/${id}`)
      setJob(j)
      if (user?.role === 'Client' && j.clientId === user.id) {
        if (j.status !== 'Completed') {
          const ints = await api.get(`/jobs/${id}/interests`)
          setInterests(ints)
        }
        if (j.status === 'Completed' && j.selectedHandymanId) {
          const reviews = await api.get(`/reviews/handyman/${j.selectedHandymanId}`)
          setHasReview(reviews.some(r => r.jobId === j.id))
        }
      }
      if (user?.role === 'Handyman') {
        const myInts = await api.get('/handymen/me/interests')
        setInterestSubmitted(myInts.some(i => i.jobId === j.id))
      }
    } catch {
      navigate('/')
    } finally {
      setLoading(false)
    }
  }, [id, user, navigate])

  useEffect(() => { load() }, [load])

  const doAction = async (endpoint, method = 'post') => {
    setActionError('')
    try {
      await api[method](endpoint)
      await load()
    } catch (err) {
      setActionError(err.message || 'Грешка.')
    }
  }

  if (loading) return <div style={s.loading}>Зареждане...</div>
  if (!job) return null

  const isClient = user?.role === 'Client'
  const isHandyman = user?.role === 'Handyman'
  const isOwner = isClient && job.clientId === user?.id
  const isSelectedHandyman = isHandyman && job.selectedHandymanId === user?.id

  return (
    <div style={s.page}>
      <button style={s.back} onClick={() => navigate(-1)}>← Назад</button>

      <div style={s.card}>
        <div style={s.headerRow}>
          <div style={s.title}>{job.title}</div>
          <span style={s.badge(job.status)}>{STATUS_LABELS[job.status] ?? job.status}</span>
        </div>

        <div style={s.metaRow}>
          {job.subCategoryName && <span style={t.chip}>⚙ {job.subCategoryName}</span>}
          {job.cityName && <span style={t.chip}>📍 {job.cityName}</span>}
          {job.clientName && <span style={t.chip}>👤 {job.clientName}</span>}
          {job.approximateBudget != null && <span style={{ ...t.chipAmber, fontWeight: 700 }}>~{job.approximateBudget} лв.</span>}
          {job.selectedHandymanName && <span style={t.chip}>🔧 {job.selectedHandymanName}</span>}
        </div>

        <div style={s.desc}>{job.description}</div>

        {actionError && <div style={{ ...s.error, marginTop: '14px', marginBottom: 0 }}>{actionError}</div>}

        <div style={s.actions}>
          {isOwner && (job.status === 'Open' || job.status === 'AwaitingConfirmation') && (
            <button style={s.btnDanger} onClick={() => doAction(`/jobs/${id}/cancel`)}>Отмени поръчката</button>
          )}
          {isOwner && job.status === 'InProgress' && (
            <button style={s.btnSuccess} onClick={() => doAction(`/jobs/${id}/complete`)}>Маркирай като завършена ✓</button>
          )}
          {isSelectedHandyman && job.status === 'AwaitingConfirmation' && (
            <>
              <button style={s.btnSuccess} onClick={() => doAction(`/jobs/${id}/confirm`)}>Потвърди ✓</button>
              <button style={s.btnSecondary} onClick={() => doAction(`/jobs/${id}/decline`)}>Откажи</button>
            </>
          )}
          {isHandyman && job.status === 'InProgress' && job.selectedHandymanId === user?.id && (
            <button style={s.btnSuccess} onClick={() => doAction(`/jobs/${id}/complete`)}>Маркирай като завършена ✓</button>
          )}
        </div>
      </div>

      {isOwner && (
        <div style={s.sectionCard}>
          <div style={s.sectionTitle}>Заинтересувани майстори ({interests.length})</div>
          {interests.length === 0
            ? <div style={s.empty}>Все още няма изразен интерес.</div>
            : (
              <div style={s.interestsList}>
                {interests.map(i => (
                  <InterestedHandymanCard
                    key={i.id}
                    interest={i}
                    canSelect={job.status === 'Open'}
                    onSelect={(interestId) => doAction(`/jobs/${id}/select/${interestId}`)}
                  />
                ))}
              </div>
            )}
        </div>
      )}

      {isHandyman && job.status === 'Open' && (
        <div style={s.sectionCard}>
          <div style={s.sectionTitle}>Изразете интерес</div>
          {interestSubmitted
            ? <div style={s.alreadyInterested}>✓ Вече сте изразили интерес към тази поръчка.</div>
            : <InterestForm jobId={Number(id)} onSubmitted={() => { setInterestSubmitted(true); load() }} />
          }
        </div>
      )}

      {isOwner && job.status === 'Completed' && (
        <div style={s.sectionCard}>
          {!hasReview ? (
            <>
              <div style={s.sectionTitle}>Оставете отзив</div>
              <ReviewForm jobId={job.id} onSubmitted={() => { setHasReview(true); load() }} />
            </>
          ) : (
            <div style={s.alreadyReviewed}>✓ Вече сте оставили отзив за тази поръчка.</div>
          )}
        </div>
      )}

      {isOwner && job.selectedHandymanId && (
        <div style={{ fontSize: '13px', color: t.textMuted, paddingTop: '4px' }}>
          <Link to={`/handymen/${job.selectedHandymanId}`} style={{ color: t.amberDark, fontWeight: 600, textDecoration: 'none' }}>
            Виж профила на майстора →
          </Link>
        </div>
      )}
    </div>
  )
}
