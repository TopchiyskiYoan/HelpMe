import { useState, useEffect, useCallback } from 'react'
import { api } from '../services/api.js'
import AdminLayout from '../components/AdminLayout.jsx'
import LoadingSpinner from '../components/LoadingSpinner.jsx'
import EmptyState from '../components/EmptyState.jsx'
import ErrorMessage from '../components/ErrorMessage.jsx'

const ROLE_LABEL = { Client: 'Клиент', Handyman: 'Майстор', Administrator: 'Администратор' }

const s = {
  header: { display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem', flexWrap: 'wrap', gap: '0.75rem' },
  title: { fontFamily: "'Syne', system-ui, sans-serif", fontSize: '22px', fontWeight: 800, color: '#1c1917', letterSpacing: '-0.03em' },
  searchWrap: { display: 'flex', gap: '8px' },
  input: { padding: '8px 14px', borderRadius: '8px', border: '1px solid #fde68a', fontSize: '13px', outline: 'none', background: '#fffbf0', width: '220px' },
  table: { width: '100%', borderCollapse: 'collapse', fontSize: '13px' },
  th: { textAlign: 'left', padding: '10px 12px', background: '#fef3c7', color: '#78350f', fontWeight: 700, fontSize: '12px' },
  td: { padding: '10px 12px', borderBottom: '1px solid #fef3c7', color: '#1c1917', verticalAlign: 'middle' },
  badge: (isBanned) => ({
    display: 'inline-block', padding: '2px 8px', borderRadius: '20px', fontSize: '11px', fontWeight: 700,
    background: isBanned ? '#fee2e2' : '#d1fae5', color: isBanned ? '#dc2626' : '#059669',
  }),
  btnBan: { padding: '4px 12px', borderRadius: '6px', border: 'none', cursor: 'pointer', fontSize: '12px', fontWeight: 600, background: '#fee2e2', color: '#dc2626' },
  btnUnban: { padding: '4px 12px', borderRadius: '6px', border: 'none', cursor: 'pointer', fontSize: '12px', fontWeight: 600, background: '#d1fae5', color: '#059669' },
  pager: { display: 'flex', gap: '8px', marginTop: '1.5rem', justifyContent: 'center', alignItems: 'center' },
  pageBtn: (active) => ({
    padding: '5px 12px', borderRadius: '6px', border: '1px solid #fde68a', cursor: 'pointer',
    background: active ? '#d97706' : '#fff', color: active ? '#fff' : '#78350f', fontWeight: 600, fontSize: '12px',
  }),
  overlay: { position: 'fixed', inset: 0, background: 'rgba(0,0,0,0.4)', zIndex: 50, display: 'flex', justifyContent: 'flex-end' },
  drawer: { width: '380px', maxWidth: '95vw', background: '#fff', height: '100%', overflowY: 'auto', padding: '28px 24px', boxShadow: '-4px 0 24px rgba(0,0,0,0.12)' },
  drawerClose: { float: 'right', background: 'none', border: 'none', fontSize: '22px', cursor: 'pointer', color: '#78350f', lineHeight: 1 },
  drawerName: { fontFamily: "'Syne', system-ui, sans-serif", fontSize: '20px', fontWeight: 800, color: '#1c1917', marginBottom: '4px' },
  drawerRole: { fontSize: '13px', color: '#78350f', marginBottom: '20px' },
  dRow: { display: 'flex', justifyContent: 'space-between', padding: '9px 0', borderBottom: '1px solid #fef3c7', fontSize: '13px', gap: '12px' },
  dLabel: { color: '#92400e', fontWeight: 500, flexShrink: 0 },
  dVal: { color: '#1c1917', fontWeight: 600, textAlign: 'right', flex: 1 },
  chip: { display: 'inline-block', padding: '2px 8px', borderRadius: '20px', fontSize: '11px', background: '#fef3c7', color: '#92400e', margin: '2px' },
  section: { fontSize: '11px', fontWeight: 700, color: '#78350f', marginTop: '18px', marginBottom: '8px', textTransform: 'uppercase', letterSpacing: '0.07em' },
  drawerActions: { display: 'flex', gap: '10px', marginTop: '24px' },
}

function UserDrawer({ userId, onClose, onBanToggle }) {
  const [user, setUser] = useState(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    setLoading(true)
    api.get(`/admin/users/${userId}`).then(setUser).finally(() => setLoading(false))
  }, [userId])

  return (
    <div style={s.overlay} onClick={onClose}>
      <div style={s.drawer} onClick={e => e.stopPropagation()}>
        <button style={s.drawerClose} onClick={onClose}>✕</button>
        {loading && <div style={{ paddingTop: '3rem', textAlign: 'center', color: '#78350f' }}>Зареждане...</div>}
        {!loading && user && (
          <>
            <div style={s.drawerName}>{user.firstName} {user.lastName}</div>
            <div style={s.drawerRole}>
              {ROLE_LABEL[user.role] ?? user.role}
              {user.isBanned ? ' · 🚫 Блокиран' : ' · ✓ Активен'}
            </div>

            <div style={s.section}>Лична информация</div>
            {[
              ['Имейл', user.email],
              ['Телефон', user.phoneNumber ?? '—'],
              ['Регистрация', new Date(user.createdAt).toLocaleDateString('bg-BG')],
            ].map(([l, v]) => (
              <div key={l} style={s.dRow}>
                <span style={s.dLabel}>{l}</span>
                <span style={s.dVal}>{v}</span>
              </div>
            ))}

            {user.role === 'Handyman' && (
              <>
                <div style={s.section}>Профил на майстора</div>
                {[
                  ['Верифициран', user.isVerified ? 'Да' : 'Не'],
                  ['Оценка', user.reviewCount > 0 ? `${user.averageRating} ★ (${user.reviewCount})` : 'Без отзиви'],
                  ['Опит', user.yearsOfExperience != null ? `${user.yearsOfExperience} год.` : '—'],
                ].map(([l, v]) => (
                  <div key={l} style={s.dRow}>
                    <span style={s.dLabel}>{l}</span>
                    <span style={s.dVal}>{v}</span>
                  </div>
                ))}
                {user.subCategories?.length > 0 && (
                  <div style={{ paddingTop: '8px' }}>
                    <div style={{ ...s.dLabel, fontSize: '12px', marginBottom: '4px' }}>Специалности</div>
                    <div>{user.subCategories.map(sc => <span key={sc} style={s.chip}>{sc}</span>)}</div>
                  </div>
                )}
                {user.cities?.length > 0 && (
                  <div style={{ paddingTop: '8px' }}>
                    <div style={{ ...s.dLabel, fontSize: '12px', marginBottom: '4px' }}>Градове</div>
                    <div>{user.cities.map(c => <span key={c} style={s.chip}>📍 {c}</span>)}</div>
                  </div>
                )}
              </>
            )}

            {user.role === 'Client' && (
              <>
                <div style={s.section}>Активност</div>
                {[
                  ['Общо поръчки', user.totalJobs ?? 0],
                  ['Завършени', user.completedJobs ?? 0],
                ].map(([l, v]) => (
                  <div key={l} style={s.dRow}>
                    <span style={s.dLabel}>{l}</span>
                    <span style={s.dVal}>{v}</span>
                  </div>
                ))}
              </>
            )}

            <div style={s.drawerActions}>
              {user.isBanned
                ? <button style={s.btnUnban} onClick={() => onBanToggle(user.id, false)}>Разблокирай</button>
                : <button style={s.btnBan} onClick={() => onBanToggle(user.id, true)}>Блокирай</button>}
            </div>
          </>
        )}
      </div>
    </div>
  )
}

export default function AdminUsersPage() {
  const [data, setData] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [search, setSearch] = useState('')
  const [page, setPage] = useState(1)
  const [selectedId, setSelectedId] = useState(null)

  const load = useCallback(() => {
    setLoading(true)
    const params = new URLSearchParams({ page })
    if (search) params.set('search', search)
    api.get(`/admin/users?${params}`)
      .then(setData)
      .catch(() => setError('Грешка при зареждане.'))
      .finally(() => setLoading(false))
  }, [page, search])

  useEffect(() => { load() }, [load])

  const handleBanToggle = async (id, ban) => {
    await api.post(`/admin/users/${id}/${ban ? 'ban' : 'unban'}`).catch(() => {})
    setSelectedId(null)
    load()
  }

  return (
    <AdminLayout>
      {selectedId && (
        <UserDrawer
          userId={selectedId}
          onClose={() => setSelectedId(null)}
          onBanToggle={handleBanToggle}
        />
      )}

      <div style={s.header}>
        <div style={s.title}>Потребители {data ? `(${data.totalCount})` : ''}</div>
        <form style={s.searchWrap} onSubmit={e => { e.preventDefault(); setPage(1); load() }}>
          <input
            style={s.input}
            placeholder="Търси по име или имейл..."
            value={search}
            onChange={e => setSearch(e.target.value)}
          />
        </form>
      </div>

      <ErrorMessage message={error} />
      {loading && <LoadingSpinner />}
      {!loading && !error && data?.items?.length === 0 && <EmptyState icon="👤" message="Няма намерени потребители." />}

      {!loading && !error && data?.items?.length > 0 && (
        <>
          <table style={s.table}>
            <thead>
              <tr>
                <th style={s.th}>Потребител</th>
                <th style={s.th}>Имейл</th>
                <th style={s.th}>Телефон</th>
                <th style={s.th}>Роля</th>
                <th style={s.th}>Статус</th>
                <th style={s.th}>Регистрация</th>
              </tr>
            </thead>
            <tbody>
              {data.items.map(u => (
                <tr
                  key={u.id}
                  style={{ cursor: 'pointer' }}
                  onClick={() => setSelectedId(u.id)}
                >
                  <td style={s.td}><strong>{u.firstName} {u.lastName}</strong></td>
                  <td style={s.td}>{u.email}</td>
                  <td style={s.td}>{u.phoneNumber ?? '—'}</td>
                  <td style={s.td}>{ROLE_LABEL[u.role] ?? u.role}</td>
                  <td style={s.td}>
                    <span style={s.badge(u.isBanned)}>{u.isBanned ? 'Блокиран' : 'Активен'}</span>
                  </td>
                  <td style={s.td}>{new Date(u.createdAt).toLocaleDateString('bg-BG')}</td>
                </tr>
              ))}
            </tbody>
          </table>

          {data.totalPages > 1 && (
            <div style={s.pager}>
              {Array.from({ length: data.totalPages }, (_, i) => i + 1).map(p => (
                <button key={p} style={s.pageBtn(p === page)} onClick={() => setPage(p)}>{p}</button>
              ))}
            </div>
          )}
        </>
      )}
    </AdminLayout>
  )
}
