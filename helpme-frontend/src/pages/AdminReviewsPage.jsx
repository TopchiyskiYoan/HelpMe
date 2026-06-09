import { useState, useEffect, useCallback } from 'react'
import { api } from '../services/api.js'
import AdminLayout from '../components/AdminLayout.jsx'
import LoadingSpinner from '../components/LoadingSpinner.jsx'
import EmptyState from '../components/EmptyState.jsx'
import ErrorMessage from '../components/ErrorMessage.jsx'
import StarRating from '../components/StarRating.jsx'

const s = {
  header: { display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem', flexWrap: 'wrap', gap: '0.75rem' },
  title: { fontFamily: "'Syne', system-ui, sans-serif", fontSize: '22px', fontWeight: 800, color: '#1c1917', letterSpacing: '-0.03em' },
  select: { padding: '8px 14px', borderRadius: '8px', border: '1px solid #fde68a', fontSize: '13px', background: '#fffbf0', outline: 'none', cursor: 'pointer' },
  table: { width: '100%', borderCollapse: 'collapse', fontSize: '13px' },
  th: { textAlign: 'left', padding: '10px 12px', background: '#fef3c7', color: '#78350f', fontWeight: 700, fontSize: '12px' },
  td: { padding: '10px 12px', borderBottom: '1px solid #fef3c7', color: '#1c1917', verticalAlign: 'top' },
  comment: { fontSize: '12px', color: '#57534e', maxWidth: '280px', lineHeight: 1.5 },
  btnDelete: { padding: '4px 12px', borderRadius: '6px', border: 'none', cursor: 'pointer', fontSize: '12px', fontWeight: 600, background: '#fee2e2', color: '#dc2626' },
  pager: { display: 'flex', gap: '8px', marginTop: '1.5rem', justifyContent: 'center' },
  pageBtn: (active) => ({ padding: '5px 12px', borderRadius: '6px', border: '1px solid #fde68a', cursor: 'pointer', background: active ? '#d97706' : '#fff', color: active ? '#fff' : '#78350f', fontWeight: 600, fontSize: '12px' }),
}

export default function AdminReviewsPage() {
  const [data, setData] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [page, setPage] = useState(1)
  const [sort, setSort] = useState('createdAt:desc')

  const [sortBy, sortDir] = sort.split(':')

  const load = useCallback(() => {
    setLoading(true)
    api.get(`/admin/reviews?page=${page}&sortBy=${sortBy}&sortDir=${sortDir}`)
      .then(setData)
      .catch(() => setError('Грешка при зареждане.'))
      .finally(() => setLoading(false))
  }, [page, sortBy, sortDir])

  useEffect(() => { load() }, [load])

  const handleDelete = async (id) => {
    if (!window.confirm('Изтриете ли този отзив?')) return
    await api.delete(`/admin/reviews/${id}`).catch(() => {})
    load()
  }

  return (
    <AdminLayout>
      <div style={s.header}>
        <div style={s.title}>Отзиви {data ? `(${data.totalCount})` : ''}</div>
        <select style={s.select} value={sort} onChange={e => { setSort(e.target.value); setPage(1) }}>
          <option value="createdAt:desc">Дата ↓ (нови)</option>
          <option value="createdAt:asc">Дата ↑ (стари)</option>
          <option value="rating:desc">Оценка ↓ (5→1)</option>
          <option value="rating:asc">Оценка ↑ (1→5)</option>
          <option value="handyman:asc">Майстор А-Я</option>
          <option value="handyman:desc">Майстор Я-А</option>
        </select>
      </div>

      <ErrorMessage message={error} />
      {loading && <LoadingSpinner />}
      {!loading && !error && data?.items?.length === 0 && <EmptyState icon="⭐" message="Няма отзиви." />}

      {!loading && !error && data?.items?.length > 0 && (
        <>
          <table style={s.table}>
            <thead>
              <tr>
                <th style={s.th}>Клиент</th>
                <th style={s.th}>Майстор</th>
                <th style={s.th}>Оценка</th>
                <th style={s.th}>Коментар</th>
                <th style={s.th}>Дата</th>
                <th style={s.th}>Действие</th>
              </tr>
            </thead>
            <tbody>
              {data.items.map(r => (
                <tr key={r.id}>
                  <td style={s.td}>{r.clientName}</td>
                  <td style={s.td}>{r.handymanName}</td>
                  <td style={s.td}><StarRating value={r.rating} /></td>
                  <td style={s.td}><div style={s.comment}>{r.comment}</div></td>
                  <td style={s.td}>{new Date(r.createdAt).toLocaleDateString('bg-BG')}</td>
                  <td style={s.td}>
                    <button style={s.btnDelete} onClick={() => handleDelete(r.id)}>Изтрий</button>
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
