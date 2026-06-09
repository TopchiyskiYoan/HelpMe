import { useState, useEffect } from 'react'
import { api } from '../services/api.js'
import AdminLayout from '../components/AdminLayout.jsx'
import LoadingSpinner from '../components/LoadingSpinner.jsx'
import EmptyState from '../components/EmptyState.jsx'
import ErrorMessage from '../components/ErrorMessage.jsx'

const s = {
  header: { marginBottom: '1.5rem' },
  title: { fontFamily: "'Syne', system-ui, sans-serif", fontSize: '22px', fontWeight: 800, color: '#1c1917', letterSpacing: '-0.03em' },
  card: {
    background: '#fff',
    borderRadius: '12px',
    border: '1px solid #fde68a',
    padding: '1.25rem',
    marginBottom: '0.75rem',
    display: 'flex',
    justifyContent: 'space-between',
    alignItems: 'flex-start',
    gap: '1rem',
    flexWrap: 'wrap',
  },
  name: { fontWeight: 700, fontSize: '15px', color: '#1c1917', marginBottom: '4px' },
  bio: { fontSize: '13px', color: '#57534e', lineHeight: 1.5, maxWidth: '480px' },
  meta: { fontSize: '12px', color: '#78350f', marginTop: '6px' },
  actions: { display: 'flex', gap: '8px', flexShrink: 0 },
  btnApprove: {
    padding: '7px 16px', borderRadius: '8px', border: 'none', cursor: 'pointer',
    fontSize: '13px', fontWeight: 700, background: 'linear-gradient(135deg, #d97706, #f59e0b)', color: '#fff',
  },
  btnReject: {
    padding: '7px 16px', borderRadius: '8px', border: '1px solid #fecaca', cursor: 'pointer',
    fontSize: '13px', fontWeight: 600, background: '#fee2e2', color: '#dc2626',
  },
}

export default function AdminVerificationPage() {
  const [profiles, setProfiles] = useState([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')

  const load = () => {
    setLoading(true)
    api.get('/admin/handymen/pending')
      .then(setProfiles)
      .catch(() => setError('Грешка при зареждане.'))
      .finally(() => setLoading(false))
  }

  useEffect(() => { load() }, [])

  const handleVerify = async (userId, approved) => {
    await api.post(`/admin/handymen/${userId}/verify`, { approved }).catch(() => {})
    load()
  }

  return (
    <AdminLayout>
      <div style={s.header}>
        <div style={s.title}>Изчакващи верификация</div>
      </div>

      <ErrorMessage message={error} />
      {loading && <LoadingSpinner />}
      {!loading && !error && profiles.length === 0 && (
        <EmptyState icon="✅" message="Няма изчакващи верификация." />
      )}
      {!loading && !error && profiles.map(p => (
        <div key={p.userId} style={s.card}>
          <div>
            <div style={s.name}>{p.firstName} {p.lastName}</div>
            {p.bio && <div style={s.bio}>{p.bio}</div>}
            <div style={s.meta}>
              Опит: {p.yearsOfExperience} г. &bull;&nbsp;
              Категории: {p.subCategories?.map(c => c.name).join(', ') || '—'} &bull;&nbsp;
              Градове: {p.cities?.map(c => c.name).join(', ') || '—'}
            </div>
          </div>
          <div style={s.actions}>
            <button style={s.btnApprove} onClick={() => handleVerify(p.userId, true)}>Одобри</button>
            <button style={s.btnReject} onClick={() => handleVerify(p.userId, false)}>Откажи</button>
          </div>
        </div>
      ))}
    </AdminLayout>
  )
}
