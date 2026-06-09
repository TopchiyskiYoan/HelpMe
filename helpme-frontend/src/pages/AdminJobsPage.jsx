import { useState, useEffect, useCallback } from 'react'
import { api } from '../services/api.js'
import AdminLayout from '../components/AdminLayout.jsx'
import LoadingSpinner from '../components/LoadingSpinner.jsx'
import EmptyState from '../components/EmptyState.jsx'
import ErrorMessage from '../components/ErrorMessage.jsx'

const STATUS_OPTIONS = ['', 'Open', 'AwaitingConfirmation', 'InProgress', 'Completed', 'Cancelled']
const STATUS_BG = {
  Open: { bg: '#dbeafe', color: '#1d4ed8', label: 'Отворена' },
  AwaitingConfirmation: { bg: '#fef3c7', color: '#d97706', label: 'Изчаква' },
  InProgress: { bg: '#d1fae5', color: '#059669', label: 'В процес' },
  Completed: { bg: '#f0fdf4', color: '#16a34a', label: 'Завършена' },
  Cancelled: { bg: '#fee2e2', color: '#dc2626', label: 'Отказана' },
}

const s = {
  header: { display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem', flexWrap: 'wrap', gap: '0.75rem' },
  title: { fontFamily: "'Syne', system-ui, sans-serif", fontSize: '22px', fontWeight: 800, color: '#1c1917', letterSpacing: '-0.03em' },
  filters: { display: 'flex', gap: '8px', flexWrap: 'wrap', alignItems: 'center' },
  select: { padding: '8px 14px', borderRadius: '8px', border: '1px solid #fde68a', fontSize: '13px', background: '#fffbf0', outline: 'none', cursor: 'pointer' },
  table: { width: '100%', borderCollapse: 'collapse', fontSize: '13px' },
  th: { textAlign: 'left', padding: '10px 12px', background: '#fef3c7', color: '#78350f', fontWeight: 700, fontSize: '12px', cursor: 'pointer', userSelect: 'none' },
  thStatic: { textAlign: 'left', padding: '10px 12px', background: '#fef3c7', color: '#78350f', fontWeight: 700, fontSize: '12px' },
  td: { padding: '10px 12px', borderBottom: '1px solid #fef3c7', color: '#1c1917', verticalAlign: 'middle' },
  badge: (status) => {
    const c = STATUS_BG[status] || { bg: '#f3f4f6', color: '#6b7280' }
    return { display: 'inline-block', padding: '2px 8px', borderRadius: '20px', fontSize: '11px', fontWeight: 700, background: c.bg, color: c.color }
  },
  pager: { display: 'flex', gap: '8px', marginTop: '1.5rem', justifyContent: 'center', alignItems: 'center' },
  pageBtn: (active) => ({ padding: '5px 12px', borderRadius: '6px', border: '1px solid #fde68a', cursor: 'pointer', background: active ? '#d97706' : '#fff', color: active ? '#fff' : '#78350f', fontWeight: 600, fontSize: '12px' }),
}

export default function AdminJobsPage() {
  const [data, setData] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [status, setStatus] = useState('')
  const [page, setPage] = useState(1)
  const [sortBy, setSortBy] = useState('createdAt')
  const [sortDir, setSortDir] = useState('desc')

  const load = useCallback(() => {
    setLoading(true)
    const params = new URLSearchParams({ page, sortBy, sortDir })
    if (status) params.set('status', status)
    api.get(`/admin/jobs?${params}`)
      .then(setData)
      .catch(() => setError('Грешка при зареждане.'))
      .finally(() => setLoading(false))
  }, [page, status, sortBy, sortDir])

  useEffect(() => { load() }, [load])

  const handleSort = (col) => {
    if (sortBy === col) setSortDir(d => d === 'asc' ? 'desc' : 'asc')
    else { setSortBy(col); setSortDir('desc') }
    setPage(1)
  }

  const sortIcon = (col) => sortBy === col ? (sortDir === 'asc' ? ' ↑' : ' ↓') : ' ⇅'

  return (
    <AdminLayout>
      <div style={s.header}>
        <div style={s.title}>Поръчки {data ? `(${data.totalCount})` : ''}</div>
        <div style={s.filters}>
          <select style={s.select} value={status} onChange={e => { setStatus(e.target.value); setPage(1) }}>
            <option value="">Всички статуси</option>
            {STATUS_OPTIONS.filter(Boolean).map(st => (
              <option key={st} value={st}>{STATUS_BG[st]?.label || st}</option>
            ))}
          </select>
          <select style={s.select} value={`${sortBy}:${sortDir}`} onChange={e => {
            const [col, dir] = e.target.value.split(':')
            setSortBy(col); setSortDir(dir); setPage(1)
          }}>
            <option value="createdAt:desc">Дата ↓</option>
            <option value="createdAt:asc">Дата ↑</option>
            <option value="title:asc">Заглавие А-Я</option>
            <option value="title:desc">Заглавие Я-А</option>
            <option value="budget:desc">Бюджет ↓</option>
            <option value="budget:asc">Бюджет ↑</option>
            <option value="status:asc">Статус</option>
          </select>
        </div>
      </div>

      <ErrorMessage message={error} />
      {loading && <LoadingSpinner />}
      {!loading && !error && data?.items?.length === 0 && <EmptyState icon="📋" message="Няма намерени поръчки." />}

      {!loading && !error && data?.items?.length > 0 && (
        <>
          <table style={s.table}>
            <thead>
              <tr>
                <th style={s.thStatic}>#</th>
                <th style={s.thStatic}>Заглавие</th>
                <th style={s.thStatic}>Клиент</th>
                <th style={s.thStatic}>Категория</th>
                <th style={s.thStatic}>Град</th>
                <th style={s.thStatic}>Статус</th>
                <th style={s.thStatic}>Дата</th>
              </tr>
            </thead>
            <tbody>
              {data.items.map(j => (
                <tr key={j.id}>
                  <td style={s.td}>{j.id}</td>
                  <td style={s.td}>{j.title}</td>
                  <td style={s.td}>{j.clientName}</td>
                  <td style={s.td}>{j.subCategoryName}</td>
                  <td style={s.td}>{j.cityName}</td>
                  <td style={s.td}>
                    <span style={s.badge(j.status)}>{STATUS_BG[j.status]?.label || j.status}</span>
                  </td>
                  <td style={s.td}>{new Date(j.createdAt).toLocaleDateString('bg-BG')}</td>
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
