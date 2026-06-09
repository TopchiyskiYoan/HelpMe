# HelpMe — Technical Requirements

## Solution Structure

```
HelpMe/
├── HelpMe.Domain/           # Entities, enums, interfaces — zero dependencies
├── HelpMe.Application/      # Services, DTOs, validators — business logic only
├── HelpMe.Infrastructure/   # EF Core, DbContext, migrations, seed, file system
├── HelpMe.Web/              # ASP.NET Core 8 Web API — controllers, DI, middleware
├── HelpMe.Tests/            # NUnit + Moq unit tests
└── helpme-frontend/         # Vite + React SPA (outside .NET solution)
```

**Technology Stack:**
- Backend: ASP.NET Core 8, Entity Framework Core 8, MS SQL Server (LocalDB)
- Auth: ASP.NET Core Identity + JWT Bearer tokens (stored in localStorage)
- Frontend: Vite 8 + React 19 (JavaScript), native fetch API
- Testing: NUnit, Moq (Application layer unit tests)
- Source control: GitHub, public repo, 25+ commits, 7+ different days

**Ports:**
- API: `http://localhost:5079`
- Frontend: `http://localhost:5173` (Vite proxy forwards `/api/*` to API)

---

## Phase 1 — Foundation: Authentication & Core Structure

### Feature 1.1 — Solution & Project Setup

**Infrastructure / Config:**
- Blank solution `HelpMe` with 5 projects:
  - Web → Application → Domain
  - Web → Infrastructure → Domain
  - Tests → Application, Infrastructure
- NuGet packages: EF Core SqlServer, Identity.EntityFrameworkCore, EF Tools, AutoMapper, NUnit, Moq
- `appsettings.json` with LocalDB connection string
- Register DbContext, Identity, JWT, CORS in `Program.cs`

**Frontend:**
- Vite + React project in `helpme-frontend/`
- Dependencies: `react-router-dom`
- Folder structure: `src/pages/`, `src/components/`, `src/services/`, `src/context/`
- `vite.config.js` proxy: `/api/*` → `http://localhost:5079`
- `src/theme.js` — shared design tokens (colors, typography, spacing)

---

### Feature 1.2 — ApplicationUser Entity

**Domain:**
- `ApplicationUser : IdentityUser`
  - `FirstName` (string, required)
  - `LastName` (string, required)
  - `PhoneNumber` (inherited, required)
  - `ProfilePictureUrl` (string, nullable)
  - `CreatedAt` (DateTime)
  - `IsBanned` (bool, default false)

**Infrastructure:**
- `ApplicationDbContext : IdentityDbContext<ApplicationUser>`
- Migration: `InitialCreate`
- Seed (via `DbSeeder`): 1 admin, 5 clients, 6 handymen — all with password `Test1234!`

**Application:**
- `IUserService` / `UserService`:
  - `GetUserByIdAsync(string id)`
  - `UpdateProfileAsync(string id, UpdateProfileDto dto)`
  - `ChangePasswordAsync(string id, ChangePasswordDto dto)`

---

### Feature 1.3 — Authentication Endpoints (JWT)

**Web — Controller:** `AuthController`

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register (Client or Handyman role) |
| POST | `/api/auth/login` | Login, return JWT token |
| GET | `/api/auth/me` | Get current user info from token |

**JWT Configuration:**
- Token contains: userId, email, role, firstName, lastName
- Stored in `localStorage` by frontend `AuthContext`
- `Authorization: Bearer <token>` header on all protected calls

**Rate limiting:** applied to register + login endpoints

**Frontend:**
- `LoginPage.jsx`, `RegisterPage.jsx` (with role selection dropdown)
- `AuthContext.jsx` — stores current user, exposes `login()`, `logout()`, `user`
- `ProtectedRoute.jsx` — redirects to `/login`; optional `roles` prop for role-gating
- `DashboardPage.jsx` — redirects based on role: Admin → `/admin`, Client → `/dashboard`, Handyman → `/handyman/feed`
- `Navbar.jsx` — dark slate `#0f172a`, amber logo, avatar with initials, notification bell, dropdown

---

## Phase 2 — Handyman Profiles & Categories

### Feature 2.1 — Service Categories & Subcategories

**Domain:**
- `ServiceCategory`
  - `Id` (int), `Name` (string), `Icon` (string), `Description` (string, nullable)
  - `SubCategories` (ICollection of `ServiceSubCategory`)

- `ServiceSubCategory`
  - `Id` (int), `Name` (string), `CategoryId` (int, FK)
  - `Category` (navigation)

**Infrastructure:**
- DbSets: `ServiceCategories`, `ServiceSubCategories`
- Migration: `AddServiceCategories`
- Seed: 10 categories, 50+ subcategories (ВиК, Електро, Шпакловка/Боядисване, Дограма, Облицовки, Климатизация, Сухо строителство, Отопление, Дърводелство, Паркет)

**Web — Controller:** `CategoriesController`

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/categories` | Public | All categories with subcategories |

---

### Feature 2.2 — Regions & Cities

**Domain:**
- `Region`
  - `Id` (int), `Name` (string) — Bulgarian oblast (e.g. "Софийска")
  - `Cities` (ICollection of `City`)

- `City`
  - `Id` (int), `Name` (string), `RegionId` (int, FK)
  - `Region` (navigation)

**Infrastructure:**
- DbSets: `Regions`, `Cities`
- Migration: `AddRegions`
- Seed: all 28 Bulgarian oblasti with major cities

**Web — Controller:** `RegionsController`

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/regions` | Public | All regions with cities |

---

### Feature 2.3 — Handyman Profile

**Domain:**
- `HandymanProfile`
  - `Id` (int)
  - `UserId` (string, FK to ApplicationUser, 1-to-1)
  - `Bio` (string), `YearsOfExperience` (int)
  - `IsVerified` (bool, default false)
  - `AverageRating` (double, cached), `ReviewCount` (int)
  - `ProfilePictureUrl` (string, nullable)
  - `CreatedAt` (DateTime)
  - Navigation: `User`, `SubCategories`, `Cities`

- `HandymanSubCategory` (join table)
  - `HandymanProfileId` (int, FK), `SubCategoryId` (int, FK)

- `HandymanCity` (join table)
  - `HandymanProfileId` (int, FK), `CityId` (int, FK)

**Infrastructure:**
- DbSets: `HandymanProfiles`, `HandymanSubCategories`, `HandymanCities`
- Migration: `AddHandymanProfile`
- Composite PKs on join tables
- Seed: 5 verified handymen + 1 unverified (pending)

**Application:**
- `IHandymanService` / `HandymanService`:
  - `GetProfileAsync(string userId)`
  - `GetPublicProfileAsync(string userId)` — includes subcategories, cities, rating
  - `CreateOrUpdateProfileAsync(string userId, UpdateHandymanProfileDto dto)`
  - `GetAllVerifiedAsync()` — public directory
  - `GetPendingVerificationAsync()` — Admin only
  - `VerifyHandymanAsync(string userId, bool approved)` — Admin only

**Web — Controller:** `HandymenController`

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/handymen` | Public | All verified handymen |
| GET | `/api/handymen/{userId}` | Public | Public profile by user ID |
| GET | `/api/handymen/me` | Handyman | Own profile |
| PUT | `/api/handymen/me` | Handyman | Update profile (bio, exp, subcategories, cities) |
| GET | `/api/admin/handymen/pending` | Admin | Pending verification queue |
| POST | `/api/admin/handymen/{userId}/verify` | Admin | Approve |
| POST | `/api/admin/handymen/{userId}/reject` | Admin | Reject |

**Frontend:**
- `HandymanPublicProfilePage.jsx` — bio, rating, subcategories, cities, reviews
- `HandymanListPage.jsx` — directory with name/specialty/city search, card grid
- `AdminVerificationPage.jsx` — pending queue with approve/reject

---

## Phase 3 — Jobs: Core Flow

### Feature 3.1 — Job Entity & Creation

**Domain:**
- `Job`
  - `Id` (int), `Title` (string), `Description` (string)
  - `ApproximateBudget` (decimal, nullable)
  - `Status` (`JobStatus` enum)
  - `ClientId` (string, FK), `SubCategoryId` (int, FK), `CityId` (int, FK)
  - `SelectedHandymanId` (int?, FK to HandymanProfile)
  - `CreatedAt`, `UpdatedAt` (DateTime)
  - Navigation: `Client`, `SubCategory`, `City`, `SelectedHandyman`, `Interests`

- `JobStatus` enum: `Open`, `AwaitingConfirmation`, `InProgress`, `Completed`, `Cancelled`

**Infrastructure:**
- DbSet: `Jobs`
- Migration: `AddJob`
- Seed: 31 jobs — 18 open, 3 in progress, 9 completed, 1 cancelled

**Application:**
- `IJobService` / `JobService`:
  - `CreateAsync(string clientId, CreateJobDto dto)`
  - `GetByIdAsync(int id)`
  - `GetClientJobsAsync(string clientId)`
  - `GetOpenJobsForHandymanAsync(string userId)` — matches handyman's subcategories + cities
  - `SelectHandymanAsync(int jobId, int interestId, string clientId)`
  - `ConfirmJobAsync(int jobId, string userId)`
  - `DeclineJobAsync(int jobId, string userId)`
  - `CancelAsync(int jobId, string clientId)`
  - `CompleteAsync(int jobId, string userId)`

**Web — Controller:** `JobsController`

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/jobs` | Client | Create job |
| GET | `/api/jobs/my` | Client | Own jobs |
| GET | `/api/jobs/feed` | Handyman | Matching open jobs |
| GET | `/api/jobs/{id}` | Authenticated | Job details |
| POST | `/api/jobs/{id}/interests` | Handyman | Submit interest |
| POST | `/api/jobs/{id}/select/{handymanId}` | Client | Select handyman |
| POST | `/api/jobs/{id}/confirm` | Handyman | Confirm assignment |
| POST | `/api/jobs/{id}/cancel` | Client | Cancel job |
| POST | `/api/jobs/{id}/complete` | Client or Handyman | Complete job |

**Frontend:**
- `JobCreatePage.jsx` — 3-step wizard: (1) subcategory + city, (2) title + description + budget, (3) review + publish
- `ClientDashboardPage.jsx` — stat cards + job list; clicking entire `JobCard` navigates to detail
- `HandymanFeedPage.jsx` — feed with text search + city filter dropdown + result count
- `JobCard.jsx` — full card is clickable via `useNavigate` + `onClick` (not just the title link)
- `PendingConfirmationsPage.jsx` — handyman's pending assignments

---

### Feature 3.2 — Job Interest

**Domain:**
- `JobInterest`
  - `Id` (int), `JobId` (int, FK), `HandymanId` (int, FK)
  - `ProposedPrice` (decimal), `Note` (string, nullable)
  - `SubmittedAt` (DateTime)
  - `Status` (`JobInterestStatus` enum: Pending, Selected, Rejected)

**Infrastructure:**
- DbSet: `JobInterests`
- Migration: `AddJobInterest`

**Frontend:**
- `JobDetailPage.jsx` — handyman sees interest form; client sees interested candidates list
- `InterestForm.jsx` — proposed price + optional note
- `InterestedHandymanCard.jsx` — name, rating, price, select button (client view)

---

## Phase 4 — Reviews, Ratings & Admin Dashboard

### Feature 4.1 — Reviews & Ratings

**Domain:**
- `Review`
  - `Id` (int), `JobId` (int, FK, unique), `ClientId` (string, FK), `HandymanId` (int, FK)
  - `Rating` (int, 1–5), `Comment` (string), `CreatedAt` (DateTime)

**Infrastructure:**
- DbSet: `Reviews`
- Migration: `AddReviews`
- Seed: 9 reviews on completed jobs

**Application:**
- `IReviewService` / `ReviewService`:
  - `CreateReviewAsync(string clientId, CreateReviewDto dto)`
  - `GetHandymanReviewsAsync(int handymanId)`
  - `UpdateHandymanRatingAsync(int handymanId)` — recalculates avg + count

**Web — Controller:** `ReviewsController`

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/reviews` | Client | Leave review (completed jobs only) |
| GET | `/api/handymen/{userId}/reviews` | Public | Handyman's reviews |
| DELETE | `/api/admin/reviews/{id}` | Admin | Remove review |

**Frontend:**
- `ReviewForm.jsx` — star selector (1–5) + comment textarea
- `StarRating.jsx` — reusable display (read-only and interactive)
- `ReviewList.jsx` — list with rating, comment, date

---

### Feature 4.2 — Admin Dashboard

**Application — `IAdminService` / `AdminService`:**
- `GetAllUsersAsync(string? search, int page)` — paginated, searchable
- `BanUserAsync(string userId)` / `UnbanUserAsync(string userId)`
- `GetAllJobsAsync(JobStatus? status, int page, string? sortBy, string? sortDir, int pageSize)`
  - Sort options: `title`, `budget`, `status`, `createdAt` (each ↑/↓)
- `GetAllReviewsAsync(int page, string? sortBy, string? sortDir, int pageSize)`
  - Sort options: `createdAt`, `rating`, `handymanName`
- `DeleteReviewAsync(int reviewId)`
- `GetStatsAsync()` → `AdminStatsDto`
- `GetUserDetailAsync(string id)` → `AdminUserDetailDto`

**DTOs:**
```csharp
AdminStatsDto {
    TotalUsers, TotalClients, TotalHandymen, VerifiedHandymen, PendingVerifications,
    TotalJobs, OpenJobs, InProgressJobs, CompletedJobs, TotalReviews, AverageRating
}

AdminUserDetailDto {
    Id, FirstName, LastName, Email, PhoneNumber, ProfilePictureUrl,
    Role, IsBanned, CreatedAt,
    // Handyman only
    AverageRating?, ReviewCount?, IsVerified?, YearsOfExperience?,
    SubCategories (List<string>), Cities (List<string>),
    // Client only
    TotalJobs?, CompletedJobs?
}
```

**Web — Controller:** `AdminController`

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/admin/stats` | Admin | Platform statistics |
| GET | `/api/admin/users` | Admin | All users (search + paging) |
| GET | `/api/admin/users/{id}` | Admin | Detailed user profile |
| POST | `/api/admin/users/{id}/ban` | Admin | Ban user |
| POST | `/api/admin/users/{id}/unban` | Admin | Unban user |
| GET | `/api/admin/jobs` | Admin | All jobs (filter + sort + paging) |
| GET | `/api/admin/reviews` | Admin | All reviews (sort + paging) |
| DELETE | `/api/admin/reviews/{id}` | Admin | Delete review |

**Frontend:**
- `AdminLayout.jsx` — sidebar with links to Табло, Потребители, Верификация, Поръчки, Отзиви
- `AdminDashboardPage.jsx` — stat cards grid + recent jobs panel + recent reviews panel + quick action links; admin lands here on login
- `AdminUsersPage.jsx` — searchable table; click any row opens `UserDrawer` (right-side panel with full profile, role-specific data, ban/unban button)
- `AdminJobsPage.jsx` — filterable by status + sortable (date↑↓, title A-Z, budget↑↓, status)
- `AdminReviewsPage.jsx` — sortable (date↑↓, rating↑↓, handyman A-Z) + delete

---

## Phase 5 — In-App Notifications & UI Polish

### Feature 5.1 — Notifications

**Domain:**
- `Notification`
  - `Id` (int), `UserId` (string, FK)
  - `Title` (string), `Message` (string)
  - `IsRead` (bool, default false), `CreatedAt` (DateTime)
  - `Type` (`NotificationType` enum)

- `NotificationType` enum: `JobInterestReceived`, `HandymanSelected`, `JobConfirmed`, `JobDeclined`, `JobCompleted`, `ReviewReceived`, `VerificationResult`

**Application — `INotificationService` / `NotificationService`:**
- `CreateAsync(string userId, NotificationType type, string title, string message)`
- `GetUserNotificationsAsync(string userId)` — last 20, newest first
- `MarkAsReadAsync(int id, string userId)`
- `MarkAllAsReadAsync(string userId)`

**Web — Controller:** `NotificationsController`

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/notifications` | Authenticated | Recent notifications |
| PUT | `/api/notifications/{id}/read` | Authenticated | Mark one as read |
| PUT | `/api/notifications/read-all` | Authenticated | Mark all as read |

**Frontend:**
- `NotificationBell.jsx` — in navbar, polls unread count every 30s
- `NotificationDropdown.jsx` — list with mark-as-read on click

---

### Feature 5.2 — Profile Editing & UI Polish

**Application:**
- `UsersController`: `GET /api/users/me`, `PUT /api/users/me`, `PUT /api/users/me/password`

**Frontend:**
- `ProfilePage.jsx` — edit name, phone, profile picture URL + password change form
- `LoadingSpinner.jsx`, `EmptyState.jsx`, `ErrorMessage.jsx` — reusable state components
- `NotFoundPage.jsx` — custom 404

**Backend:**
- Global exception handling middleware — consistent JSON error responses
- Rate limiting on auth endpoints

---

## Phase 5.3 — Admin Dashboard Expansion

### Feature 5.3.1 — Platform Statistics

**Backend:** `GET /api/admin/stats` → `AdminStatsDto`
- Loads all users via `UserManager.Users`, categorizes by role
- Counts `HandymanProfile` records where `IsVerified = false` (pending)
- Counts jobs by status from `DbContext.Jobs`
- Calculates platform average rating from all reviews

**Frontend:** `AdminDashboardPage.jsx`
- Stat cards: Потребители, Майстори, Верифицирани, Чакащи, Поръчки, Отворени, В процес, Завършени, Отзиви, Средна оценка
- Recent jobs panel: first 5 from `GET /api/admin/jobs`
- Recent reviews panel: first 5 from `GET /api/admin/reviews`
- Quick action links to all admin sub-pages
- Admin redirected here from `DashboardPage.jsx` when role === 'Administrator'

---

### Feature 5.3.2 — User Detail Drawer

**Backend:** `GET /api/admin/users/{id}` → `AdminUserDetailDto`
- If Handyman: load `HandymanProfile` with `Include(SubCategories).ThenInclude(SubCategory)` and `.Include(Cities).ThenInclude(City)`
- If Client: count `Jobs` where `ClientId == id` grouped by status

**Frontend:** `UserDrawer` sub-component inside `AdminUsersPage.jsx`
- Renders as a fixed right-side panel (400px) when `selectedId` is set
- Shows: avatar with initials, full name, role badge, ban status, email, phone, registered date
- Handyman section: verified status, avg rating with stars, years exp, subcategory chips (amber), city chips (blue)
- Client section: total jobs, completed jobs
- Ban/Unban button calls existing ban/unban endpoints; updates table row inline

---

### Feature 5.3.3 — Sorting on Admin Tables

**Backend:** query params `sortBy` + `sortDir` on jobs and reviews endpoints
- Jobs sort switch: `(sortBy.ToLower(), sortDir == "asc")` → `title/budget/status/createdAt`
- Reviews sort switch: `createdAt/rating/handymanName`

**Frontend:**
- `AdminJobsPage.jsx` — sort dropdown: Дата ↓, Дата ↑, Заглавие А-Я, Заглавие Я-А, Бюджет ↓, Бюджет ↑, Статус ↓
- `AdminReviewsPage.jsx` — sort dropdown: Дата ↓, Дата ↑, Оценка ↓, Оценка ↑, Майстор А-Я

---

## Unit Tests (HelpMe.Tests)

Tests written with NUnit + Moq targeting the Application layer.

**Services to test:**
- `JobService` — create, cancel, select, confirm, decline, complete
- `JobInterestService` — submit interest, duplicate prevention
- `HandymanService` — create/update profile, verify
- `ReviewService` — create review, rating recalculation
- `NotificationService` — creation, mark as read

**Pattern:**
```
[SetUp] — create mock dependencies with Moq
[Test]  — arrange → act → assert
[Test]  — test failure case (throws or returns error)
```

---

## Security Checklist

| Threat | Mitigation |
|--------|-----------|
| SQL Injection | EF Core parameterized queries — never raw SQL with user input |
| XSS | React escapes output by default; input sanitization middleware |
| Parameter Tampering | Ownership validated server-side (userId from JWT, not request body) |
| Unauthorized Access | `[Authorize]` + `[Authorize(Roles = "...")]` on all protected endpoints |
| Mass Assignment | DTOs only — never bind directly to entity models |
| Token Security | JWT with expiry; stored in localStorage |

---

## Design System (`helpme-frontend/src/theme.js`)

| Token | Value |
|-------|-------|
| Background | `#f8fafc` |
| Card background | `#ffffff` |
| Card border | `#e2e8f0` |
| Brand accent | `#f59e0b` (amber) |
| Navbar | `#0f172a` (dark slate) |
| Body font | Inter |
| Heading font | Syne |

---

## GitHub Workflow

- Public repository, `main` branch, 25+ commits across 7+ days
- `README.md` includes: project description, tech stack, how to run, test accounts, seed data, API reference
- Commit message convention: `feat: <what was done> (Phase X.X)`
