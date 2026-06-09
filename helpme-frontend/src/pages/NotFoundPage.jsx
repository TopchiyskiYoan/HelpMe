import { Link } from 'react-router-dom'

const s = {
  page: {
    display: 'flex',
    flexDirection: 'column',
    alignItems: 'center',
    justifyContent: 'center',
    flex: 1,
    padding: '4rem 2rem',
    background: '#fffbf0',
    textAlign: 'center',
  },
  code: {
    fontFamily: "'Syne', system-ui, sans-serif",
    fontSize: '80px',
    fontWeight: 800,
    color: '#d97706',
    lineHeight: 1,
    marginBottom: '12px',
  },
  title: {
    fontSize: '22px',
    fontWeight: 700,
    color: '#1c1917',
    marginBottom: '8px',
  },
  sub: {
    fontSize: '15px',
    color: '#78350f',
    marginBottom: '28px',
  },
  link: {
    padding: '10px 24px',
    borderRadius: '8px',
    background: 'linear-gradient(135deg, #d97706, #f59e0b)',
    color: '#fff',
    fontWeight: 700,
    fontSize: '14px',
    textDecoration: 'none',
    boxShadow: '0 3px 10px rgba(217,119,6,0.3)',
  },
}

export default function NotFoundPage() {
  return (
    <div style={s.page}>
      <div style={s.code}>404</div>
      <div style={s.title}>Страницата не е намерена</div>
      <div style={s.sub}>Търсената от вас страница не съществува.</div>
      <Link to="/" style={s.link}>Към началото</Link>
    </div>
  )
}
