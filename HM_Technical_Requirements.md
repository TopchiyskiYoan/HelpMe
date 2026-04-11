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
- Backend: ASP.NET Core 8, Entity Framework Core, MS SQL Server (LocalDB via SSMS)
- Auth: ASP.NET Core Identity (cookie or JWT)
- Frontend: Vite + React, fetch API for all HTTP calls
- Testing: NUnit, Moq (target: 70%+ coverage on Application layer)
- Source control: GitHub (public repo, README.md, 25+ commits, 7+ different days)

---

## Phase 1 — Foundation: Authentication & Core Structure

### Feature 1.1 — Solution & Project Setup

**Infrastructure / Config:**
- Create blank solution `HelpMe`
- Add all 5 projects with correct project references:
  - Web → Application → Domain
  - Web → Infrastructure → Domain
  - Tests → Application, Infrastructure
- Install NuGet packages:
  - `Microsoft.EntityFrameworkCore.SqlServer` (Infrastructure)
  - `Microsoft.AspNetCore.Identity.EntityFrameworkCore` (Infrastructure)
  - `Microsoft.EntityFrameworkCore.Tools` (Infrastructure)
  - `AutoMapper` (Application)
  - `NUnit`, `Moq`, `NUnit3TestAdapter` (Tests)
- Configure `appsettings.json` with LocalDB connection string
- Register DbContext and Identity in `Program.cs`

**Frontend:**
- Initialize Vite + React project in `helpme-frontend/`
- Install: `react-router-dom`, `axios` (or use native fetch)
- Set up folder structure: `src/pages/`, `src/components/`, `src/services/`, `src/context/`
- Configure proxy to ASP.NET Core Web API (vite.config.js)

---

### Feature 1.2 — ApplicationUser Entity

**Domain:**
- `ApplicationUser : IdentityUser`
  - `FirstName` (string, required)
  - `LastName` (string, required)
  - `PhoneNumber` (inherited, required)
  - `ProfilePictureUrl` (string, nullable)
  - `CreatedAt` (DateTime)

**Infrastructure:**
- `ApplicationDbContext : IdentityDbContext<ApplicationUser>`
- Add `ApplicationUser` to DbContext
- Migration: `InitialCreate`
- Seed: Admin user with role `Administrator`, and sample Client + Handyman users

**Application:**
- `IUserService` interface in Domain
- `UserService` in Application:
  - `GetUserByIdAsync(string id)`
  - `UpdateProfileAsync(string id, UpdateProfileDto dto)`

**Web — DI:**
- Register `IUserService → UserService` in `Program.cs`

---

### Feature 1.3 — Authentication Endpoints

**Web — Controller:** `AuthController`

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register new user (Client or Handyman role) |
| POST | `/api/auth/login` | Login, return auth cookie/token |
| POST | `/api/auth/logout` | Logout |
| GET | `/api/auth/me` | Get current user info |

**Validation:**
- `RegisterDto`: FirstName, LastName, Email, PhoneNumber, Password, Role — all required, email format, password min length 6
- Return `400` with validation errors on invalid input
- Return `409` if email already exists

**Security:**
- Passwords hashed by Identity
- CSRF protection enabled (antiforgery middleware or SameSite cookies)
- Input sanitized to prevent XSS

**Frontend:**
- `RegisterPage.jsx` — form with role selection (Client / Handyman), calls `POST /api/auth/register`
- `LoginPage.jsx` — email + password form, calls `POST /api/auth/login`
- `AuthContext.jsx` — stores current user, exposes `login()`, `logout()`, `user`
- `ProtectedRoute.jsx` — redirects to `/login` if not authenticated
- `App.jsx` — set up React Router with routes: `/login`, `/register`, `/` (protected)
- Navbar component (basic): shows user name + logout button if logged in

---

## Phase 2 — Handyman Profiles & Categories

### Feature 2.1 — Service Categories

**Domain:**
- `ServiceCategory`
  - `Id` (int)
  - `Name` (string, required)
  - `Icon` (string, emoji or icon name)
  - `Description` (string, nullable)
  - `ParentCategoryId` (int?, self-referencing FK)
  - `IsActive` (bool)
  - `ParentCategory` (navigation)
  - `SubCategories` (ICollection)

**Infrastructure:**
- Add `ServiceCategories` DbSet to DbContext
- Migration: `AddServiceCategory`
- Seed: 6–8 top-level categories (ВиК, Електро, Бояджийство, Почистване, Преместване, Градина, Климатици, ИТ поддръжка) with subcategories

**Application:**
- `ICategoryService` / `CategoryService`:
  - `GetAllAsync()` — returns tree structure
  - `GetByIdAsync(int id)`
  - `CreateAsync(CreateCategoryDto dto)` — Admin only
  - `UpdateAsync(int id, UpdateCategoryDto dto)` — Admin only
  - `DeactivateAsync(int id)` — Admin only

**Web — Controller:** `CategoriesController`

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/categories` | Public | Get all active categories |
| GET | `/api/categories/{id}` | Public | Get category by id |
| POST | `/api/categories` | Admin | Create category |
| PUT | `/api/categories/{id}` | Admin | Update category |
| DELETE | `/api/categories/{id}` | Admin | Deactivate category |

**Frontend:**
- Admin page: `AdminCategoriesPage.jsx` — table with all categories, add/edit/deactivate actions
- `CategoryForm.jsx` — reusable form for create/edit (name, icon, description, parent)
- Categories are fetched and cached in a context for use throughout the app

---

### Feature 2.2 — Areas

**Domain:**
- `Area`
  - `Id` (int)
  - `Name` (string) — e.g. "София", "Пловдив", "Варна"
  - `Region` (string) — e.g. "Южна България"
  - `IsActive` (bool)

**Infrastructure:**
- Add `Areas` DbSet
- Migration: `AddArea`
- Seed: 28 Bulgarian oblasti + major cities

**Application:**
- `IAreaService` / `AreaService`:
  - `GetAllAsync()`
  - `GetByIdAsync(int id)`
  - `CreateAsync(CreateAreaDto dto)` — Admin only
  - `UpdateAsync(int id, UpdateAreaDto dto)` — Admin only

**Web — Controller:** `AreasController`

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/areas` | Public | Get all active areas |
| POST | `/api/areas` | Admin | Create area |
| PUT | `/api/areas/{id}` | Admin | Update area |

---

### Feature 2.3 — Handyman Profile

**Domain:**
- `HandymanProfile`
  - `Id` (int)
  - `UserId` (string, FK to ApplicationUser, 1-to-1)
  - `Bio` (string)
  - `YearsOfExperience` (int)
  - `IsVerified` (bool, default false)
  - `AverageRating` (double, computed/cached)
  - `ReviewCount` (int)
  - `CreatedAt` (DateTime)
  - `User` (navigation)
  - `Categories` (ICollection of `HandymanCategory`)
  - `Areas` (ICollection of `HandymanArea`)

- `HandymanCategory` (join table)
  - `HandymanProfileId` (int, FK)
  - `CategoryId` (int, FK)

- `HandymanArea` (join table)
  - `HandymanProfileId` (int, FK)
  - `AreaId` (int, FK)

**Infrastructure:**
- Add DbSets: `HandymanProfiles`, `HandymanCategories`, `HandymanAreas`
- Migration: `AddHandymanProfile`
- Configure composite PKs for join tables
- Seed: 3–5 sample handyman profiles (verified)

**Application:**
- `IHandymanService` / `HandymanService`:
  - `GetProfileAsync(string userId)`
  - `GetPublicProfileAsync(int handymanId)` — includes rating, categories, areas
  - `CreateProfileAsync(string userId, CreateHandymanProfileDto dto)`
  - `UpdateProfileAsync(string userId, UpdateHandymanProfileDto dto)`
  - `UpdateCategoriesAsync(string userId, List<int> categoryIds)`
  - `UpdateAreasAsync(string userId, List<int> areaIds)`
  - `GetAllVerifiedAsync(int? categoryId, int? areaId)` — with filtering
  - `GetPendingVerificationAsync()` — Admin only
  - `VerifyHandymanAsync(int handymanId, bool approved)` — Admin only

**Web — Controller:** `HandymanController`

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/handymen` | Public | List verified handymen (filter by category, area) |
| GET | `/api/handymen/{id}` | Public | Get public handyman profile |
| GET | `/api/handymen/me` | Handyman | Get own profile |
| POST | `/api/handymen/me` | Handyman | Create own profile |
| PUT | `/api/handymen/me` | Handyman | Update own profile |
| PUT | `/api/handymen/me/categories` | Handyman | Update selected categories |
| PUT | `/api/handymen/me/areas` | Handyman | Update selected areas |
| GET | `/api/admin/handymen/pending` | Admin | Get pending verification list |
| POST | `/api/admin/handymen/{id}/verify` | Admin | Approve or reject handyman |

**Frontend:**
- `HandymanProfileEditPage.jsx` — bio, experience, multi-select categories, multi-select areas
- `HandymanPublicProfilePage.jsx` — shows bio, categories, areas, rating, reviews
- `HandymanListPage.jsx` — browsable list with filters (category dropdown, area dropdown)
- `HandymanCard.jsx` — reusable card component (name, rating, categories, areas)
- Admin page: `AdminVerificationPage.jsx` — list of unverified handymen with approve/reject buttons

---

## Phase 3 — Jobs: Core Flow

### Feature 3.1 — Job Entity & Creation

**Domain:**
- `Job`
  - `Id` (int)
  - `Title` (string, required)
  - `Description` (string, required)
  - `ApproximateBudget` (decimal, nullable)
  - `Status` (enum: Open, AwaitingConfirmation, InProgress, Completed, Cancelled)
  - `ClientId` (string, FK to ApplicationUser)
  - `CategoryId` (int, FK)
  - `AreaId` (int, FK)
  - `SelectedHandymanId` (int?, FK to HandymanProfile)
  - `CreatedAt` (DateTime)
  - `UpdatedAt` (DateTime)
  - Navigation: `Client`, `Category`, `Area`, `SelectedHandyman`, `Interests`

- `JobStatus` enum: `Open`, `AwaitingConfirmation`, `InProgress`, `Completed`, `Cancelled`

**Infrastructure:**
- Add `Jobs` DbSet
- Migration: `AddJob`
- Seed: 5 sample jobs in various statuses

**Application:**
- `IJobService` / `JobService`:
  - `CreateAsync(string clientId, CreateJobDto dto)`
  - `GetByIdAsync(int id)`
  - `GetClientJobsAsync(string clientId)` — all jobs by this client
  - `GetOpenJobsForHandymanAsync(int handymanId)` — jobs matching handyman's categories + areas
  - `CancelAsync(int id, string requesterId)` — client can cancel Open or AwaitingConfirmation

**Web — Controller:** `JobsController`

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/jobs` | Client | Create new job |
| GET | `/api/jobs/my` | Client | Get client's own jobs |
| GET | `/api/jobs/feed` | Handyman | Get open jobs matching handyman's profile |
| GET | `/api/jobs/{id}` | Authenticated | Get job details |
| POST | `/api/jobs/{id}/cancel` | Client | Cancel a job |

**Frontend:**
- `JobCreatePage.jsx` — multi-step form: select category → fill details → select area → review & submit
- `ClientDashboardPage.jsx` — list of client's jobs with status badges
- `HandymanFeedPage.jsx` — list of open jobs matching handyman's categories/areas
- `JobCard.jsx` — reusable card (title, category, area, budget, status)

---

### Feature 3.2 — Job Interest (Handyman Expresses Interest)

**Domain:**
- `JobInterest`
  - `Id` (int)
  - `JobId` (int, FK)
  - `HandymanId` (int, FK to HandymanProfile)
  - `ProposedPrice` (decimal, required)
  - `Note` (string, nullable)
  - `SubmittedAt` (DateTime)
  - `Status` (enum: Pending, Selected, Rejected)
  - Navigation: `Job`, `Handyman`

- `JobInterestStatus` enum: `Pending`, `Selected`, `Rejected`

**Infrastructure:**
- Add `JobInterests` DbSet
- Migration: `AddJobInterest`

**Application:**
- `IJobInterestService` / `JobInterestService`:
  - `SubmitInterestAsync(int handymanId, int jobId, SubmitInterestDto dto)`
  - `GetInterestsForJobAsync(int jobId)` — for client to see who is interested
  - `GetHandymanInterestsAsync(int handymanId)` — handyman's submitted interests
  - Validation: handyman cannot submit interest twice for the same job; job must be Open

**Web — Controller:** `JobInterestsController`

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/jobs/{jobId}/interests` | Handyman | Submit interest + proposed price |
| GET | `/api/jobs/{jobId}/interests` | Client (owner) | Get all interested handymen |
| GET | `/api/handymen/me/interests` | Handyman | Get own submitted interests |

**Frontend:**
- `JobDetailPage.jsx` — shows job info; handyman sees "Express Interest" button + price input; client sees list of interested handymen
- `InterestForm.jsx` — proposed price input + optional note
- `InterestedHandymanCard.jsx` — shows handyman name, rating, proposed price, "Select" button (client view)

---

### Feature 3.3 — Selection & Confirmation

**Application (added to `JobService` and `JobInterestService`):**
- `SelectHandymanAsync(int jobId, int interestId, string clientId)`
  - Sets `Job.SelectedHandymanId`
  - Changes `Job.Status` → `AwaitingConfirmation`
  - Changes selected `JobInterest.Status` → `Selected`
  - Changes all other interests → `Rejected`
  - Creates notifications (see Phase 5)
- `ConfirmJobAsync(int jobId, int handymanId)`
  - Validates handyman is the selected one
  - Changes `Job.Status` → `InProgress`
- `DeclineJobAsync(int jobId, int handymanId)`
  - Changes `Job.Status` → `Open`
  - Resets `Job.SelectedHandymanId` to null
  - Resets all `JobInterest` statuses back to `Pending`
  - Notifies client

**Web — added to `JobsController`:**

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/jobs/{jobId}/select/{interestId}` | Client | Select a handyman |
| POST | `/api/jobs/{jobId}/confirm` | Handyman | Confirm assignment |
| POST | `/api/jobs/{jobId}/decline` | Handyman | Decline assignment |
| POST | `/api/jobs/{jobId}/complete` | Client or Handyman | Mark job as complete |

**Frontend:**
- `JobDetailPage.jsx` — updated: client sees "Select" button per interested handyman; handyman sees "Confirm" / "Decline" buttons when selected
- `PendingConfirmationsPage.jsx` — handyman sees all jobs awaiting their confirmation
- Status badge component shows current job status with color coding

---

## Phase 4 — Reviews, Ratings & Admin Dashboard

### Feature 4.1 — Reviews & Ratings

**Domain:**
- `Review`
  - `Id` (int)
  - `JobId` (int, FK, unique — one review per job)
  - `ClientId` (string, FK to ApplicationUser)
  - `HandymanId` (int, FK to HandymanProfile)
  - `Rating` (int, 1–5)
  - `Comment` (string)
  - `CreatedAt` (DateTime)
  - Navigation: `Job`, `Client`, `Handyman`, `Response`

- `ReviewResponse`
  - `Id` (int)
  - `ReviewId` (int, FK, unique)
  - `Content` (string, required)
  - `CreatedAt` (DateTime)
  - Navigation: `Review`

**Infrastructure:**
- Add `Reviews`, `ReviewResponses` DbSets
- Migration: `AddReviews`
- Seed: 3–5 sample reviews on seeded completed jobs

**Application:**
- `IReviewService` / `ReviewService`:
  - `CreateReviewAsync(string clientId, CreateReviewDto dto)`
    - Validates: job must be Completed, client must be owner, no existing review
  - `GetHandymanReviewsAsync(int handymanId)` — paginated
  - `RespondToReviewAsync(int handymanId, int reviewId, string content)`
  - `UpdateHandymanRatingAsync(int handymanId)` — recalculates and saves avg rating + count

**Web — Controller:** `ReviewsController`

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/reviews` | Client | Leave review on completed job |
| GET | `/api/reviews/handyman/{handymanId}` | Public | Get handyman's reviews (paged) |
| POST | `/api/reviews/{id}/respond` | Handyman | Respond to a review |
| DELETE | `/api/reviews/{id}` | Admin | Remove inappropriate review |

**Frontend:**
- `ReviewForm.jsx` — star rating (1–5) + comment textarea, shown after job completion
- `ReviewList.jsx` — list of reviews with rating, comment, date, handyman response
- `StarRating.jsx` — reusable display component (read-only and interactive modes)
- `HandymanPublicProfilePage.jsx` — updated: shows avg rating, review count, review list

---

### Feature 4.2 — Admin Dashboard

**Application:**
- `IAdminService` / `AdminService`:
  - `GetAllUsersAsync(string? search, int page)` — paginated, searchable
  - `BanUserAsync(string userId)`
  - `UnbanUserAsync(string userId)`
  - `GetAllJobsAsync(JobStatus? status, int page)` — paginated, filterable
  - `GetAllReviewsAsync(int page)` — paginated
  - `DeleteReviewAsync(int reviewId)`

**Web — Controller:** `AdminController`

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/admin/users` | Admin | List all users (search + paging) |
| POST | `/api/admin/users/{id}/ban` | Admin | Ban user |
| POST | `/api/admin/users/{id}/unban` | Admin | Unban user |
| GET | `/api/admin/jobs` | Admin | List all jobs (filter by status + paging) |
| GET | `/api/admin/reviews` | Admin | List all reviews (paging) |
| DELETE | `/api/admin/reviews/{id}` | Admin | Delete review |

**Frontend:**
- `AdminLayout.jsx` — sidebar navigation for admin pages
- `AdminUsersPage.jsx` — searchable, paginated table of users with ban/unban action
- `AdminJobsPage.jsx` — filterable, paginated table of all jobs with status
- `AdminReviewsPage.jsx` — paginated table of reviews with delete action
- Admin pages are protected — redirect to home if not in Administrator role

---

## Phase 5 — In-App Notifications & UI Polish

### Feature 5.1 — Notifications

**Domain:**
- `Notification`
  - `Id` (int)
  - `UserId` (string, FK to ApplicationUser)
  - `Title` (string)
  - `Message` (string)
  - `IsRead` (bool, default false)
  - `CreatedAt` (DateTime)
  - `Type` (enum: JobInterestReceived, HandymanSelected, JobConfirmed, JobDeclined, JobCompleted, ReviewReceived, VerificationResult)

- `NotificationType` enum (listed above)

**Infrastructure:**
- Add `Notifications` DbSet
- Migration: `AddNotifications`

**Application:**
- `INotificationService` / `NotificationService`:
  - `CreateAsync(string userId, NotificationType type, string title, string message)`
  - `GetUserNotificationsAsync(string userId)` — last 20, newest first
  - `MarkAsReadAsync(int id, string userId)`
  - `MarkAllAsReadAsync(string userId)`
  - `GetUnreadCountAsync(string userId)`

- Notification creation is called from other services at key events:

| Event | Triggered in | Recipient |
|-------|-------------|-----------|
| Handyman submits interest | `JobInterestService.SubmitInterestAsync` | Client |
| Client selects handyman | `JobService.SelectHandymanAsync` | Selected handyman |
| Client selects handyman | `JobService.SelectHandymanAsync` | Rejected handymen |
| Handyman confirms | `JobService.ConfirmJobAsync` | Client |
| Handyman declines | `JobService.DeclineJobAsync` | Client |
| Job completed | `JobService.CompleteJobAsync` | Both parties |
| Admin verifies handyman | `AdminService` / `HandymanService` | Handyman |
| Client leaves review | `ReviewService.CreateReviewAsync` | Handyman |

**Web — Controller:** `NotificationsController`

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/notifications` | Authenticated | Get recent notifications |
| GET | `/api/notifications/unread-count` | Authenticated | Get unread count |
| POST | `/api/notifications/{id}/read` | Authenticated | Mark one as read |
| POST | `/api/notifications/read-all` | Authenticated | Mark all as read |

**Frontend:**
- `NotificationBell.jsx` — icon with unread badge in navbar, polls `/api/notifications/unread-count` every 30s
- `NotificationDropdown.jsx` — dropdown list of recent notifications, mark as read on click
- Notifications are fetched on dropdown open

---

### Feature 5.2 — UI Polish & Cross-Cutting

**Frontend:**
- Consistent design system: unified color palette, typography, spacing
- `LoadingSpinner.jsx` — shown during API calls
- `EmptyState.jsx` — shown when lists are empty (with helpful message)
- `ErrorMessage.jsx` — shown on API errors
- All forms show inline validation errors
- `NotFoundPage.jsx` — custom 404 page
- `ErrorPage.jsx` — generic error boundary page
- Fully responsive layout (mobile-first, works on 320px+)
- Navbar is responsive (hamburger menu on mobile)

**Backend:**
- Global exception handling middleware — catches unhandled exceptions, returns consistent JSON error response
- `ProblemDetails` format for all API errors
- Input sanitization middleware for XSS prevention
- Rate limiting on auth endpoints (login, register)
- Paging, sorting, and searching implemented on: `GET /api/handymen`, `GET /api/admin/users`, `GET /api/admin/jobs`

---

## Unit Tests (HelpMe.Tests)

Tests are written alongside each phase. Target: **70%+ coverage on Application layer.**

**Per service, test:**
- Happy path (expected successful outcome)
- Invalid input (validation failures)
- Unauthorized access (wrong user trying to act)
- Edge cases (e.g. duplicate interest, review on non-completed job)

**Services to test (minimum):**
- `JobService` — create, cancel, select, confirm, decline, complete
- `JobInterestService` — submit interest, duplicate prevention
- `HandymanService` — create/update profile, verify
- `ReviewService` — create review, respond, rating calculation
- `NotificationService` — creation, mark as read
- `CategoryService` — CRUD, deactivate

**Pattern:**
```
[SetUp] — create mock dependencies with Moq
[Test]  — arrange → act → assert
[Test]  — test failure case (throws or returns error)
```

---

## Security Checklist (applied throughout all phases)

| Threat | Mitigation |
|--------|-----------|
| SQL Injection | EF Core parameterized queries — never raw SQL with user input |
| XSS | Input sanitization middleware; React escapes output by default |
| CSRF | SameSite cookie policy; antiforgery tokens on state-changing endpoints |
| Parameter Tampering | Always validate ownership server-side (clientId from token, not request body) |
| Unauthorized Access | `[Authorize]` + role checks on every protected endpoint |
| Mass Assignment | Always use DTOs — never bind directly to entity models |

---

## GitHub Workflow

- Public repository from day 1 with `README.md` (project description, setup instructions)
- Commit after completing each feature — aim for 1–3 commits per feature
- Minimum: 25 commits across 7+ different days
- Branch strategy: `main` (stable) + feature branches per phase (optional but recommended)
- README includes: project description, tech stack, how to run locally, screenshots (added progressively)
