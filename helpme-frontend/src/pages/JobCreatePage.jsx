import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { api } from '../services/api.js'

const inp = {
  padding: '11px 13px',
  borderRadius: '8px',
  border: '1px solid #fde68a',
  background: '#fffbf0',
  color: '#1c1917',
  fontSize: '14px',
  outline: 'none',
  width: '100%',
  boxSizing: 'border-box',
}

const s = {
  wrapper: { display: 'flex', justifyContent: 'center', flex: 1, padding: '2rem', background: '#fffbf0' },
  card: {
    width: '100%',
    maxWidth: '560px',
    padding: '32px 28px',
    border: '1px solid #fde68a',
    borderRadius: '16px',
    background: '#fff',
    textAlign: 'left',
    boxShadow: '0 8px 32px rgba(217,119,6,0.14)',
  },
  title: {
    fontFamily: "'Syne', system-ui, sans-serif",
    fontSize: '24px',
    fontWeight: 800,
    color: '#1c1917',
    letterSpacing: '-0.03em',
    marginBottom: '3px',
  },
  subtitle: { fontSize: '13px', color: '#78350f', marginBottom: '20px' },
  steps: { display: 'flex', gap: '6px', marginBottom: '24px' },
  step: (active, done) => ({
    flex: 1,
    height: '4px',
    borderRadius: '2px',
    background: done ? '#d97706' : active ? '#fde68a' : 'rgba(253,230,138,0.4)',
    transition: 'background 0.2s',
  }),
  field: { display: 'flex', flexDirection: 'column', gap: '5px', marginBottom: '14px' },
  label: {
    fontSize: '11px',
    fontWeight: 600,
    color: '#78350f',
    textTransform: 'uppercase',
    letterSpacing: '0.06em',
  },
  input: inp,
  textarea: { ...inp, resize: 'vertical', minHeight: '100px', fontFamily: 'inherit' },
  select: { ...inp, cursor: 'pointer' },
  row: { display: 'flex', gap: '0.75rem', justifyContent: 'flex-end', marginTop: '20px' },
  btnPrimary: {
    padding: '10px 24px',
    borderRadius: '8px',
    border: 'none',
    background: 'linear-gradient(135deg, #d97706, #f59e0b)',
    color: '#fff',
    fontSize: '14px',
    fontWeight: 700,
    cursor: 'pointer',
    boxShadow: '0 3px 10px rgba(217,119,6,0.3)',
  },
  btnSecondary: {
    padding: '10px 20px',
    borderRadius: '8px',
    border: '1px solid #fde68a',
    background: 'transparent',
    color: '#78350f',
    fontSize: '14px',
    fontWeight: 500,
    cursor: 'pointer',
  },
  reviewRow: {
    display: 'flex',
    justifyContent: 'space-between',
    padding: '10px 0',
    borderBottom: '1px solid #fde68a',
    fontSize: '14px',
  },
  reviewLabel: { color: '#78350f', fontWeight: 500 },
  reviewValue: { color: '#1c1917', fontWeight: 600, textAlign: 'right', maxWidth: '55%' },
  error: {
    fontSize: '13px',
    color: '#dc2626',
    marginBottom: '14px',
    padding: '10px 13px',
    background: '#fee2e2',
    border: '1px solid #fecaca',
    borderRadius: '8px',
  },
}

export default function JobCreatePage() {
  const navigate = useNavigate()
  const [step, setStep] = useState(1)
  const [categories, setCategories] = useState([])
  const [regions, setRegions] = useState([])
  const [form, setForm] = useState({ subCategoryId: '', cityId: '', title: '', description: '', approximateBudget: '' })
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  useEffect(() => {
    api.get('/categories').then(setCategories).catch(() => {})
    api.get('/regions').then(setRegions).catch(() => {})
  }, [])

  const allSubs = categories.flatMap(c => (c.subCategories ?? []).map(sub => ({ ...sub, categoryName: c.name })))
  const allCities = regions.flatMap(r => (r.cities ?? []).map(c => ({ ...c, regionName: r.name })))
  const selectedSub = allSubs.find(sub => String(sub.id) === String(form.subCategoryId))
  const selectedCity = allCities.find(c => String(c.id) === String(form.cityId))

  const set = (field) => (e) => setForm(f => ({ ...f, [field]: e.target.value }))

  const goNext = () => {
    setError('')
    if (step === 1) {
      if (!form.subCategoryId) { setError('Изберете подкатегория.'); return }
      if (!form.cityId) { setError('Изберете населено място.'); return }
    }
    if (step === 2) {
      if (!form.title.trim()) { setError('Заглавието е задължително.'); return }
      if (!form.description.trim()) { setError('Описанието е задължително.'); return }
    }
    setStep(s => s + 1)
  }

  const handleSubmit = async () => {
    setError('')
    setLoading(true)
    try {
      const body = {
        title: form.title.trim(),
        description: form.description.trim(),
        subCategoryId: Number(form.subCategoryId),
        cityId: Number(form.cityId),
        approximateBudget: form.approximateBudget ? Number(form.approximateBudget) : null,
      }
      const created = await api.post('/jobs', body)
      navigate(`/jobs/${created.id}`)
    } catch (err) {
      setError(err.message || 'Грешка при създаване.')
      setStep(2)
    } finally {
      setLoading(false)
    }
  }

  const STEPS = ['Локация', 'Детайли', 'Преглед']

  return (
    <div style={s.wrapper}>
      <div style={s.card}>
        <div style={s.title}>Нова поръчка</div>
        <div style={s.subtitle}>Стъпка {step} от 3 — {STEPS[step - 1]}</div>
        <div style={s.steps}>
          {[1, 2, 3].map(n => <div key={n} style={s.step(n === step, n < step)} />)}
        </div>

        {error && <div style={s.error}>{error}</div>}

        {step === 1 && (
          <>
            <div style={s.field}>
              <label style={s.label}>Подкатегория</label>
              <select style={s.select} value={form.subCategoryId} onChange={set('subCategoryId')}>
                <option value="">— изберете —</option>
                {categories.map(cat => (
                  <optgroup key={cat.id} label={cat.name}>
                    {(cat.subCategories ?? []).map(sub => (
                      <option key={sub.id} value={sub.id}>{sub.name}</option>
                    ))}
                  </optgroup>
                ))}
              </select>
            </div>
            <div style={s.field}>
              <label style={s.label}>Населено място</label>
              <select style={s.select} value={form.cityId} onChange={set('cityId')}>
                <option value="">— изберете —</option>
                {regions.map(r => (
                  <optgroup key={r.id} label={r.name}>
                    {(r.cities ?? []).map(c => (
                      <option key={c.id} value={c.id}>{c.name}</option>
                    ))}
                  </optgroup>
                ))}
              </select>
            </div>
          </>
        )}

        {step === 2 && (
          <>
            <div style={s.field}>
              <label style={s.label}>Заглавие</label>
              <input style={s.input} value={form.title} onChange={set('title')}
                placeholder="напр. Смяна на кран в кухнята" />
            </div>
            <div style={s.field}>
              <label style={s.label}>Описание</label>
              <textarea style={s.textarea} value={form.description} onChange={set('description')}
                placeholder="Опишете проблема, размерите, спешността..." />
            </div>
            <div style={s.field}>
              <label style={s.label}>Примерен бюджет (лв., незадължително)</label>
              <input style={s.input} type="number" min="0" step="1" value={form.approximateBudget}
                onChange={set('approximateBudget')} placeholder="напр. 150" />
            </div>
          </>
        )}

        {step === 3 && (
          <div style={{ marginBottom: '8px' }}>
            {[
              ['Подкатегория', selectedSub ? `${selectedSub.categoryName} › ${selectedSub.name}` : '—'],
              ['Населено място', selectedCity?.name ?? '—'],
              ['Заглавие', form.title],
              ['Описание', form.description],
              ['Бюджет', form.approximateBudget ? `~${form.approximateBudget} лв.` : 'Не е посочен'],
            ].map(([lbl, val]) => (
              <div key={lbl} style={s.reviewRow}>
                <span style={s.reviewLabel}>{lbl}</span>
                <span style={s.reviewValue}>{val}</span>
              </div>
            ))}
          </div>
        )}

        <div style={s.row}>
          {step > 1 && (
            <button style={s.btnSecondary} onClick={() => setStep(p => p - 1)}>Назад</button>
          )}
          {step < 3 && (
            <button style={s.btnPrimary} onClick={goNext}>Напред →</button>
          )}
          {step === 3 && (
            <button style={s.btnPrimary} onClick={handleSubmit} disabled={loading}>
              {loading ? 'Изпращане...' : 'Публикувай →'}
            </button>
          )}
        </div>
      </div>
    </div>
  )
}
