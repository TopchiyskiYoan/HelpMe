import { useState, useEffect, useCallback } from 'react'
import { api } from '../services/api.js'
import AdminLayout from '../components/AdminLayout.jsx'
import LoadingSpinner from '../components/LoadingSpinner.jsx'
import EmptyState from '../components/EmptyState.jsx'
import ErrorMessage from '../components/ErrorMessage.jsx'

const s = {
  header: { display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem', flexWrap: 'wrap', gap: '0.75rem' },
  title: { fontFamily: "'Syne', system-ui, sans-serif", fontSize: '22px', fontWeight: 800, color: '#1c1917', letterSpacing: '-0.03em' },
  searchWrap: { display: 'flex', gap: '8px' },
  input: {
    padding: '8px 14px', borderRadius: '8px', border: '1px solid #fde68a',
    fontSize: '13px', outline: 'none', background: '#fffbf0', width: '220px',
  },
  table: { width: '100%', borderCollapse: 'collapse', fontSize: '13px' },
  th: { textAlign: 'left', padding: '10px 12px', background: '#fef3c7', color: '#78350f', fontWeight: 700, fontSize: '12px' },
  td: { padding: '10px 12px', borderBottom: '1px solid #fef3c7', color: '#1c1917', verticalAlign: 'middle' },
  badge: (isBanned) => ({
    display: 'inline-block', padding: '2px 8px', borderRadius: '20px', fontSize: '11px', fontWeight: 700,
    background: isBanned ? '#fee2e2' : '#d1fae5', color: isBanned ? '#dc2626' : '#059669',
  }),
  btnBan: {
    padding: '4px 12px', borderRadius: '6px', border: 'none', cursor: 'pointer',
    fontSize: '12px', fontWeight: 600, background: '#fee2e2', color: '#dc2626',
  },
  btnUnban: {
    padding: '4px 12px', borderRadius: '6px', border: 'none', cursor: 'pointer',
    fontSize: '12px', fontWeight: 600, background: '#d1fae5', color: '#059669',
  },
  pager: { display: 'flex', gap: '8px', marginTop: '1.5rem', justifyContent: 'center', alignItems: 'center' },
  pageBtn: (active) => ({
    padding: '5px 12px', borderRadius: '6px', border: '1px solid #fde68a', cursor: 'pointer',
    background: active ? '#d97706' : '#fff', color: active ? '#fff' : '#78350f', fontWeight: 600, fontSize: '12px',
  }),
}

export default function AdminUsersPage() {
  const [data, setData] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [search, setSearch] = useState('')
  const [page, setPage] = useState(1)

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

  const handleSearch = (e) => {
    e.preventDefault()
    setPage(1)
    load()
  }

  const handleBan = async (id) => {
    await api.post(`/admin/users/${id}/ban`).catch(() => {})
    load()
  }

  const handleUnban = async (id) => {
    await api.post(`/admin/users/${id}/unban`).catch(() => {})
    load()
  }

  return (
    <AdminLayout>
      <div style={s.header}>
        <div style={s.title}>Потребители</div>
        <form style={s.searchWrap} onSubmit={handleSearch}>
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
                <th style={s.th}>Роля</th>
                <th style={s.th}>Статус</th>
                <th style={s.th}>Действие</th>
              </tr>
            </thead>
            <tbody>
              {data.items.map(u => (
                <tr key={u.id}>
                  <td style={s.td}>{u.firstName} {u.lastName}</td>
                  <td style={s.td}>{u.email}</td>
                  <td style={s.td}>{u.role}</td>
                  <td style={s.td}>
                    <span style={s.badge(u.isBanned)}>{u.isBanned ? 'Блокиран' : 'Активен'}</span>
                  </td>
                  <td style={s.td}>
                    {u.isBanned ? (
                      <button style={s.btnUnban} onClick={() => handleUnban(u.id)}>Разблокирай</button>
                    ) : (
                      <button style={s.btnBan} onClick={() => handleBan(u.id)}>Блокирай</button>
                    )}
                  </td>
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
