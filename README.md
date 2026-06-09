# HelpMe

**HelpMe** е Bulgarian marketplace платформа, която свързва клиенти с верифицирани майстори за домашни услуги — ВиК, електро, боядисване, шпакловка, паркет, климатизация и много други.

Клиентите публикуват заявка с описание и приблизителен бюджет. Майсторите разглеждат отворените поръчки, изразяват интерес и предлагат цена. Клиентът преглежда профилите и оценките на кандидатите и избира един. Майсторът потвърждава — работата започва.

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend | ASP.NET Core 8 Web API, C# |
| Database | MS SQL Server LocalDB, Entity Framework Core 8 |
| Auth | ASP.NET Core Identity + JWT Bearer |
| Frontend | Vite 8 + React 19 (JavaScript) |
| Testing | NUnit, Moq |

---

## Solution Structure

```
HelpMe/
├── HelpMe.Domain/          # Entities, enums
├── HelpMe.Application/     # Services, DTOs, interfaces
├── HelpMe.Infrastructure/  # EF Core DbContext, migrations, seed data
├── HelpMe.Web/             # ASP.NET Core 8 Web API controllers
├── HelpMe.Tests/           # NUnit + Moq unit tests
└── helpme-frontend/        # Vite + React SPA
```

---

## Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server LocalDB (included with Visual Studio)
- Node.js 18+

### Backend

```bash
cd HelpMe.Web
dotnet restore
dotnet ef database update --project ../HelpMe.Infrastructure
dotnet run --launch-profile http
```

API runs at: `http://localhost:5079`

### Frontend

```bash
cd helpme-frontend
npm install
npm run dev
```

Frontend runs at: `http://localhost:5173`

> The Vite proxy is pre-configured to forward `/api/*` to `http://localhost:5079`.

---

## Test Accounts

All accounts use password: **`Test1234!`**

| Email | Role | Notes |
|-------|------|-------|
| `admin@helpme.bg` | Administrator | Full admin access |
| `client@helpme.bg` | Client | Георги Петров |
| `maria.ivanova@helpme.bg` | Client | Мария Иванова |
| `stefan.nikolov@helpme.bg` | Client | Стефан Николов |
| `elena.dimitrova@helpme.bg` | Client | Елена Димитрова |
| `plamen.stoyanov@helpme.bg` | Client | Пламен Стоянов |
| `handyman@helpme.bg` | Handyman | Димитър Колев — Електро (верифициран) |
| `handyman2@helpme.bg` | Handyman | Иван Георгиев — Облицовки (верифициран) |
| `handyman3@helpme.bg` | Handyman | Стоян Христов — ВиК (верифициран) |
| `nikolay.todorov@helpme.bg` | Handyman | Николай Тодоров — Шпакловка/Боядисване (верифициран) |
| `borislav.marinov@helpme.bg` | Handyman | Борислав Маринов — Климатизация (верифициран) |
| `atanas.popov@helpme.bg` | Handyman | Атанас Попов — Дограма (чака верификация) |

---

## Seed Data

| Ресурс | Брой |
|--------|------|
| Потребители | 12 (1 admin, 5 clients, 6 handymen) |
| Категории / подкатегории | 10 категории, 50+ подкатегории |
| Региони / градове | Всички български области и главни градове |
| Поръчки (Jobs) | 31 (18 отворени, 3 в процес, 9 завършени, 1 отменена) |
| Отзиви | 9 (3–5 звезди) |

---

## Implemented Features

### Authentication & Roles
- Регистрация и вход с JWT токен
- Три роли: **Client**, **Handyman**, **Administrator**
- Защитени маршрути по роля (frontend + backend)
- Rate limiting на auth ендпоинтите

### Client
- Публикуване на поръчка (3-стъпков wizard: локация → детайли → преглед)
- Табло с поръчки + статистики (отворени, в процес, завършени)
- Преглед на детайли на поръчка
- Избор на майстор от кандидатите (с профили, рейтинги, предложени цени)
- Оставяне на отзив след приключена работа
- Директория на верифицирани майстори с търсене
- Редактиране на профил (лична информация + смяна на парола)

### Handyman
- Feed с отворени поръчки, съответстващи на специалностите и локацията (търсене + филтър по град)
- Изразяване на интерес с предложена цена и бележка
- Потвърждение/отказ при избор от клиент
- Публичен профил с биография, оценки, специалности и градове
- Редактиране на профил

### Administrator
- **Dashboard с реално-времеви статистики** — потребители, майстори, верификации, поръчки по статус, средна оценка, последни поръчки и отзиви
- **Управление на потребители** — търсене, ban/unban, разглеждане на пълен профил (drawer с детайли, специалности, активност)
- **Верификация на майстори** — одобряване или отхвърляне на нови профили
- **Поръчки** — таблица с филтър по статус и сортиране (по дата, заглавие, бюджет)
- **Отзиви** — таблица с сортиране (по оценка, дата, майстор) и изтриване

### Platform
- In-app нотификации (bell с unread badge) за всички ключови събития
- Глобален error handling middleware
- Loading / Empty / Error state компоненти
- 404 страница
- Responsive design

---

## API Endpoints

### Auth
| Method | Path | Description |
|--------|------|-------------|
| POST | `/api/auth/register` | Регистрация |
| POST | `/api/auth/login` | Вход (връща JWT) |

### Users
| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/users/me` | Текущ потребител |
| PUT | `/api/users/me` | Обновяване на профил |
| PUT | `/api/users/me/password` | Смяна на парола |

### Handymen
| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/handymen` | Всички верифицирани майстори |
| GET | `/api/handymen/{userId}` | Публичен профил на майстор |
| GET | `/api/handymen/me` | Профил на текущия майстор |
| PUT | `/api/handymen/me` | Редактиране на профил |

### Jobs
| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/jobs/my` | Поръчки на текущия клиент |
| GET | `/api/jobs/feed` | Feed за майстора |
| POST | `/api/jobs` | Нова поръчка |
| GET | `/api/jobs/{id}` | Детайли на поръчка |
| POST | `/api/jobs/{id}/interests` | Изрази интерес |
| POST | `/api/jobs/{id}/select/{handymanId}` | Избери майстор |
| POST | `/api/jobs/{id}/confirm` | Майсторът потвърждава |
| POST | `/api/jobs/{id}/cancel` | Отмени поръчка |
| POST | `/api/jobs/{id}/complete` | Завърши поръчка |

### Reviews
| Method | Path | Description |
|--------|------|-------------|
| POST | `/api/reviews` | Остави отзив |
| GET | `/api/handymen/{userId}/reviews` | Отзиви за майстор |

### Admin
| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/admin/stats` | Статистики на платформата |
| GET | `/api/admin/users` | Всички потребители (с търсене) |
| GET | `/api/admin/users/{id}` | Детайлен профил на потребител |
| POST | `/api/admin/users/{id}/ban` | Блокиране |
| POST | `/api/admin/users/{id}/unban` | Разблокиране |
| GET | `/api/admin/jobs` | Всички поръчки (с филтър + сортиране) |
| GET | `/api/admin/reviews` | Всички отзиви (с сортиране) |
| DELETE | `/api/admin/reviews/{id}` | Изтриване на отзив |
| GET | `/api/admin/handymen/pending` | Чакащи верификация |
| POST | `/api/admin/handymen/{userId}/verify` | Верифициране |
| POST | `/api/admin/handymen/{userId}/reject` | Отхвърляне |

### Other
| Method | Path | Description |
|--------|------|-------------|
| GET | `/api/categories` | Всички категории с подкатегории |
| GET | `/api/regions` | Всички региони с градове |
| GET | `/api/notifications` | Нотификации на текущия потребител |
| PUT | `/api/notifications/{id}/read` | Маркирай като прочетена |
| PUT | `/api/notifications/read-all` | Маркирай всички като прочетени |

---

## Design System

Платформата използва споделен `theme.js` файл:

- **Background**: `#f8fafc` (cool gray)
- **Cards**: `#ffffff` с `#e2e8f0` border
- **Brand accent**: `#f59e0b` (amber)
- **Navbar**: `#0f172a` (dark slate)
- **Typography**: Inter (body) + Syne (headings)
