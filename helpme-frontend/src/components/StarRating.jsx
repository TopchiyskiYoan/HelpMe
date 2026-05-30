const s = {
  container: { display: 'inline-flex', gap: '2px', alignItems: 'center' },
  star: (filled, interactive) => ({
    fontSize: '20px',
    color: filled ? '#d97706' : '#d1d5db',
    cursor: interactive ? 'pointer' : 'default',
    transition: 'color 0.1s',
    lineHeight: 1,
    userSelect: 'none',
  }),
}

export default function StarRating({ rating, max = 5, onChange }) {
  const interactive = typeof onChange === 'function'

  return (
    <div style={s.container}>
      {Array.from({ length: max }, (_, i) => {
        const value = i + 1
        return (
          <span
            key={i}
            style={s.star(value <= rating, interactive)}
            onClick={interactive ? () => onChange(value) : undefined}
            title={interactive ? `${value} звезди` : undefined}
          >
            ★
          </span>
        )
      })}
    </div>
  )
}
