import { useState, useEffect, useCallback } from 'react'
import { useParams, useNavigate } from 'react-router-dom'
import { api } from '../services/api.js'
import { useAuth } from '../context/AuthContext.jsx'
import InterestForm from '../components/InterestForm.jsx'
import InterestedHandymanCard from '../components/InterestedHandymanCard.jsx'
import ReviewForm from '../components/ReviewForm.jsx'
import { STATUS_LABELS, STATUS_COLORS } from '../utils/jobStatus.js'

const s = {
  page: {
    padding: '2rem',
    flex: 1,
    maxWidth: '720px',
    margin: '0 auto',
    width: '100%',
    boxSizing: 'border-box',
    background: '#fffbf0',
  },
  back: {
    fontSize: '13px',
    fontWeight: 600,
    color: '#d97706',
    textDecoration: 'none',
    display: 'inline-flex',
    alignItems: 'center',
    gap: '4px',
    marginBottom: '20px',
  },
  header: {
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'flex-start',
    gap: '1rem',
    marginBottom: '12px',
    flexWrap: 'wrap',
  },
  title: {
    fontFamily: "'Syne', system-ui, sans-serif",
    fontSize: '26px',
    fontWeight: 800,
    color: '#1c1917',
    letterSpacing: '-0.03em',
    textAlign: 'left',
    flex: 1,
  },
  badge: (status) => ({
    fontSize: '12px',
    fontWeight: 600,
    padding: '4px 14px',
    borderRadius: '20px',
    flexShrink: 0,
    background: STATUS_COLORS[status]?.bg ?? '#fef3c7',
    color: STATUS_COLORS[status]?.color ?? '#d97706',
    border: `1px solid ${STATUS_COLORS[status]?.border ?? '#fde68a'}`,
    whiteSpace: 'nowrap',
  }),
  metaBar: {
    display: 'flex',
    flexWrap: 'wrap',
    gap: '8px',
    fontSize: '13px',
    marginBottom: '20px',
    textAlign: 'left',
  },
  metaChip: {
    background: '#fff',
    border: '1px solid #fde68a',
    borderRadius: '20px',
    padding: '4px 12px',
    color: '#78350f',
    fontWeight: 500,
  },
  budgetChip: {
    background: '#fef3c7',
    border: '1px solid #fde68a',
    borderRadius: '20px',
    padding: '4px 12px',
    color: '#d97706',
    fontWeight: 700,
  },
  desc: {
    fontSize: '15px',
    color: '#1c1917',
    textAlign: 'left',
    lineHeight: '1.65',
    marginBottom: '24px',
    padding: '16px 18px',
    background: '#fff',
    border: '1px solid #fde68a',
    borderRadius: '12px',
    boxShadow: '0 1px 4px rgba(217,119,6,0.06)',
  },
  section: { marginBottom: '24px' },
  sectionTitle: {
    fontFamily: "'Syne', system-ui, sans-serif",
    fontSize: '16px',
    fontWeight: 700,
    color: '#1c1917',
    marginBottom: '10px',
    textAlign: 'left',
    letterSpacing: '-0.02em',
  },
  actions: { display: 'flex', gap: '10px', flexWrap: 'wrap', marginBottom: '20px' },
  btnDanger: {
    padding: '9px 18px', borderRadius: '8px',
    border: '1px solid rgba(220,38,38,0.35)', background: 'rgba(220,38,38,0.07)',
    color: '#dc2626', fontSize: '14px', fontWeight: 600, cursor: 'pointer',
  },
  btnSuccess: {
    padding: '9px 18px', borderRadius: '8px', border: 'none',
    background: '#15803d', color: '#fff', fontSize: '14px', fontWeight: 600, cursor: 'pointer',
    boxShadow: '0 3px 10px rgba(21,128,61,0.25)',
  },
  btnPrimary: {
    padding: '9px 18px', borderRadius: '8px', border: 'none',
    background: 'linear-gradient(135deg, #d97706, #f59e0b)', color: '#fff',
    fontSize: '14px', fontWeight: 700, cursor: 'pointer',
    boxShadow: '0 3px 10px rgba(217,119,6,0.3)',
  },
  btnSecondary: {
    padding: '9px 18px', borderRadius: '8px',
    border: '1px solid #fde68a', background: 'transparent',
    color: '#78350f', fontSize: '14px', fontWeight: 500, cursor: 'pointer',
  },
  interestsList: { display: 'flex', flexDirection: 'column', gap: '0.75rem' },
  empty: { fontSize: '14px', color: '#78350f', textAlign: 'left', padding: '12px 0' },
  error: {
    color: '#dc2626', fontSize: '14px', marginBottom: '14px', textAlign: 'left',
    padding: '10px 13px', background: '#fee2e2', border: '1px solid #fecaca', borderRadius: '8px',
  },
  alreadyInterested: {
    padding: '12px 16px', borderRadius: '10px',
    background: '#dcfce7', border: '1px solid #bbf7d0',
    color: '#15803d', fontSize: '14px', fontWeight: 500, textAlign: 'left',
  },
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

  const selectHandyman = async (interestId) => {
    await doAction(`/jobs/${id}/select/${interestId}`)
  }

  if (loading) return (
    <div style={{ padding: '4rem', textAlign: 'center', color: '#78350f', background: '#fffbf0', flex: 1 }}>
      Зареждане...
    </div>
  )
  if (!job) return null

  const isClient = user?.role === 'Client'
  const isHandyman = user?.role === 'Handyman'
  const isOwner = isClient && job.clientId === user?.id
  const isSelectedHandyman = isHandyman && job.selectedHandymanId === user?.id

  return (
    <div style={s.page}>
      <a style={s.back} onClick={() => navigate(-1)} href="#">← Назад</a>

      <div style={s.header}>
        <div style={s.title}>{job.title}</div>
        <span style={s.badge(job.status)}>{STATUS_LABELS[job.status] ?? job.status}</span>
      </div>

      <div style={s.metaBar}>
        {job.subCategoryName && <span style={s.metaChip}>⚙ {job.subCategoryName}</span>}
        {job.cityName && <span style={s.metaChip}>📍 {job.cityName}</span>}
        {job.clientName && <span style={s.metaChip}>👤 {job.clientName}</span>}
        {job.approximateBudget != null && <span style={s.budgetChip}>~{job.approximateBudget} лв.</span>}
        {job.selectedHandymanName && <span style={s.metaChip}>🔧 {job.selectedHandymanName}</span>}
      </div>

      <div style={s.desc}>{job.description}</div>

      {actionError && <div style={s.error}>{actionError}</div>}

      {isOwner && (
        <div style={s.actions}>
          {(job.status === 'Open' || job.status === 'AwaitingConfirmation') && (
            <button style={s.btnDanger} onClick={() => doAction(`/jobs/${id}/cancel`)}>Отмени поръчката</button>
          )}
          {job.status === 'InProgress' && (
            <button style={s.btnSuccess} onClick={() => doAction(`/jobs/${id}/complete`)}>Маркирай като завършена ✓</button>
          )}
        </div>
      )}

      {isSelectedHandyman && job.status === 'AwaitingConfirmation' && (
        <div style={s.actions}>
          <button style={s.btnSuccess} onClick={() => doAction(`/jobs/${id}/confirm`)}>Потвърди ✓</button>
          <button style={s.btnSecondary} onClick={() => doAction(`/jobs/${id}/decline`)}>Откажи</button>
        </div>
      )}
      {isHandyman && job.status === 'InProgress' && job.selectedHandymanId === user?.id && (
        <div style={s.actions}>
          <button style={s.btnSuccess} onClick={() => doAction(`/jobs/${id}/complete`)}>Маркирай като завършена ✓</button>
        </div>
      )}

      {isOwner && (
        <div style={s.section}>
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
                    onSelect={selectHandyman}
                  />
                ))}
              </div>
            )}
        </div>
      )}

      {isHandyman && job.status === 'Open' && (
        <div style={s.section}>
          {interestSubmitted
            ? <div style={s.alreadyInterested}>✓ Вече сте изразили интерес към тази поръчка.</div>
            : <InterestForm jobId={Number(id)} onSubmitted={() => { setInterestSubmitted(true); load() }} />
          }
        </div>
      )}

      {isOwner && job.status === 'Completed' && !hasReview && (
        <div style={s.section}>
          <div style={s.sectionTitle}>Оставете отзив</div>
          <ReviewForm jobId={job.id} onSubmitted={() => { setHasReview(true); load() }} />
        </div>
      )}
      {isOwner && job.status === 'Completed' && hasReview && (
        <div style={{ ...s.alreadyInterested, background: '#fef3c7', border: '1px solid #fde68a', color: '#78350f' }}>
          ✓ Вече сте оставили отзив за тази поръчка.
        </div>
      )}
    </div>
  )
}
