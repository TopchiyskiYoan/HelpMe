import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { api } from '../services/api.js'
import { t } from '../theme.js'

const s = {
  wrapper: {
    display: 'flex', justifyContent: 'center', flex: 1,
    padding: '2rem', background: t.bg,
  },
  card: {
    width: '100%', maxWidth: '580px',
    padding: '36px 32px',
    border: `1px solid ${t.border}`,
    borderRadius: t.radiusLg,
    background: t.card, textAlign: 'left',
    boxShadow: t.shadowMd,
    alignSelf: 'flex-start',
  },
  title: {
    fontFamily: "'Syne', system-ui, sans-serif",
    fontSize: '24px', fontWeight: 800, color: t.text,
    letterSpacing: '-0.03em', marginBottom: '3px',
  },
  subtitle: { fontSize: '13px', color: t.textMuted, marginBottom: '24px' },
  steps: { display: 'flex', gap: '6px', marginBottom: '28px' },
  step: (active, done) => ({
    flex: 1, height: '3px', borderRadius: '2px',
    background: done ? t.amber : active ? t.amberBorder : t.border,
    transition: 'background 0.2s',
  }),
  stepLabels: { display: 'flex', gap: '6px', marginTop: '-18px', marginBottom: '24px' },
  stepLabel: (active, done) => ({
    flex: 1, fontSize: '11px', fontWeight: done || active ? 600 : 400,
    color: done || active ? t.amberDark : t.textLight,
    textAlign: 'center', textTransform: 'uppercase', letterSpacing: '0.04em',
  }),
  field: { marginBottom: '16px' },
  label: { ...t.label },
  input: { ...t.input },
  textarea: { ...t.input, resize: 'vertical', minHeight: '110px', fontFamily: 'inherit' },
  select: { ...t.input, cursor: 'pointer', appearance: 'auto' },
  row: { display: 'flex', gap: '10px', justifyContent: 'flex-end', marginTop: '24px' },
  btnPrimary: { ...t.btnPrimary, padding: '10px 24px' },
  btnSecondary: { ...t.btnSecondary, padding: '10px 20px' },
  reviewRow: {
    display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start',
    padding: '12px 0', borderBottom: `1px solid ${t.border}`,
    fontSize: '14px', gap: '1rem',
  },
  reviewLabel: { color: t.textMuted, fontWeight: 500, flexShrink: 0 },
  reviewValue: { color: t.text, fontWeight: 600, textAlign: 'right' },
  error: {
    fontSize: '13px', color: t.redText, marginBottom: '16px',
    padding: '11px 14px', background: t.redBg,
    border: `1px solid ${t.redBorder}`, borderRadius: t.radius,
  },
}

const STEPS = ['Локация', 'Детайли', 'Преглед']

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

  const focusIn = (e) => { e.target.style.borderColor = t.amber }
  const focusOut = (e) => { e.target.style.borderColor = t.border }

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
              <label style={s.label}>Вид услуга</label>
              <select style={s.select} value={form.subCategoryId} onChange={set('subCategoryId')}
                onFocus={focusIn} onBlur={focusOut}>
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
              <select style={s.select} value={form.cityId} onChange={set('cityId')}
                onFocus={focusIn} onBlur={focusOut}>
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
                placeholder="напр. Смяна на кран в кухнята"
                onFocus={focusIn} onBlur={focusOut} />
            </div>
            <div style={s.field}>
              <label style={s.label}>Описание</label>
              <textarea style={s.textarea} value={form.description} onChange={set('description')}
                placeholder="Опишете проблема, размерите, спешността..."
                onFocus={focusIn} onBlur={focusOut} />
            </div>
            <div style={s.field}>
              <label style={s.label}>Примерен бюджет (лв., незадължително)</label>
              <input style={s.input} type="number" min="0" step="1" value={form.approximateBudget}
                onChange={set('approximateBudget')} placeholder="напр. 150"
                onFocus={focusIn} onBlur={focusOut} />
            </div>
          </>
        )}

        {step === 3 && (
          <div style={{ marginBottom: '8px' }}>
            {[
              ['Вид услуга', selectedSub ? `${selectedSub.categoryName} › ${selectedSub.name}` : '—'],
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
            <button style={s.btnSecondary} onClick={() => setStep(p => p - 1)}>← Назад</button>
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
