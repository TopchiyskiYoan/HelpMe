# HelpMe — Development Phases

A quick overview of the build roadmap. Each phase produces a fully working product — the next adds to it.

For the full technical breakdown, see [HM_Technical_Requirements.md](HM_Technical_Requirements.md).

---

## Phase 1 — Foundation: Auth & Core Structure ✅

**Goal:** A working app with registration, login, and role-based access.

- Solution setup — 5 projects, NuGet packages, LocalDB connection string
- `ApplicationUser : IdentityUser` with FirstName, LastName, Phone, ProfilePictureUrl, CreatedAt
- ASP.NET Core Identity + JWT Bearer authentication
- `POST /api/auth/register`, `POST /api/auth/login`, `GET /api/auth/me`
- Rate limiting on auth endpoints
- React: LoginPage, RegisterPage (with role selection), AuthContext, ProtectedRoute, Navbar

---

## Phase 2 — Handyman Profiles & Categories ✅

**Goal:** Handymen build profiles. Admins verify them. Platform has browsable categories.

- `ServiceCategory` → `ServiceSubCategory` hierarchy (10 categories, 50+ subcategories seeded)
- `Region` → `City` geographic model (all Bulgarian oblasti + major cities seeded)
- `HandymanProfile` with bio, years of experience, IsVerified, AverageRating, ReviewCount
- Many-to-many: `HandymanSubCategory`, `HandymanCity`
- Admin verifies handymen before they can receive jobs
- React: HandymanPublicProfilePage, HandymanListPage (directory with search), AdminVerificationPage

---

## Phase 3 — Jobs: Core Flow ✅

**Goal:** Full job lifecycle works end-to-end.

- `Job` entity with status flow: Open → AwaitingConfirmation → InProgress → Completed / Cancelled
- `JobInterest` — handyman expresses interest with proposed price and note
- Client selects one handyman from all interested candidates
- Handyman confirms (or declines) → job starts / returns to Open
- React: 3-step JobCreatePage, ClientDashboardPage with stats, HandymanFeedPage with search + city filter, JobDetailPage, InterestForm, InterestedHandymanCard, PendingConfirmationsPage

---

## Phase 4 — Reviews, Ratings & Admin Dashboard ✅

**Goal:** Trust signals visible across platform. Admin has full control.

- `Review` after completed job (client → handyman, 1–5 stars + comment)
- Handyman rating auto-calculated and cached on `HandymanProfile`
- Admin dashboard: users, jobs, reviews management
- React: ReviewForm, StarRating, ReviewList, AdminLayout, AdminUsersPage, AdminJobsPage, AdminReviewsPage

---

## Phase 5 — Notifications, Profile Editing & Platform Polish ✅

**Goal:** Users informed of key events. Admin has full oversight. Platform feels complete.

- `Notification` entity with typed events (interest received, selected, confirmed, etc.)
- Notification bell with unread badge; dropdown with mark-as-read
- Profile editing for all user types (name, phone, profile picture, password change)
- `UsersController` — `GET/PUT /api/users/me`, `PUT /api/users/me/password`
- Global exception handling middleware, consistent error responses
- Loading / Empty / Error state components, 404 page
- React: NotificationBell, NotificationDropdown, ProfilePage (all roles)

---

## Phase 5.3 — Admin Dashboard Expansion ✅

**Goal:** Admin gets a proper operations center with real-time overview and drill-down.

- `GET /api/admin/stats` — platform statistics (users, jobs, reviews, ratings)
- `GET /api/admin/users/{id}` — detailed user profile (role-specific: handyman subcategories/cities/rating, client job counts)
- Sorting on jobs (date, title, budget, status) and reviews (date, rating, handyman)
- **AdminDashboardPage** — stat cards, recent jobs & reviews, quick links; admin redirects here on login
- **User detail drawer** — click any user row to open right-side panel with full profile + ban/unban
- Entire job card clickable (not just title link)
- Rich seed data: 12 users, 31 jobs (18 open), 9 reviews

---

## Not For Build (Documented for Future Development)

| Feature | Reason deferred |
|---------|----------------|
| Real-time chat (SignalR) | Significant complexity, not core to MVP |
| Document upload for verification | Admin verifies manually for now |
| Stripe subscription payments | Handled externally for now |
| Email notifications | In-app notifications sufficient for MVP |
| Auto-expand geographic zone after 48h | Background job complexity |
| Admin charts / analytics | Table views cover current needs |
| Mobile application | Future milestone |
| SMS notifications | Twilio integration deferred |
| Dispute resolution system | Future milestone |
