# HelpMe — Development Phases

A quick overview of the build roadmap. Each phase produces a fully working product — the next phase adds to it.

For the full technical breakdown of each phase, see [HM_Technical_Requirements.md](HM_Technical_Requirements.md).

---

## Phase 1 — Foundation: Auth & Core Structure
**Goal:** A working app with registration, login, and role-based access.

- Solution setup (5 projects, NuGet packages, LocalDB connection)
- `ApplicationUser` with custom fields (FirstName, LastName, Phone)
- ASP.NET Identity + JWT authentication
- Register / Login / Logout / Me endpoints
- React frontend: Register page, Login page, protected routes, auth context

[Full details →](HM_Technical_Requirements.md#phase-1--foundation-authentication--core-structure)

---

## Phase 2 — Handyman Profiles & Categories
**Goal:** Handymen build profiles. Admins manage the service taxonomy.

- `ServiceCategory` with hierarchy (parent/subcategories)
- `Area` model (Bulgarian cities/regions)
- `HandymanProfile` with categories and areas (many-to-many)
- Admin verifies handymen before they can receive jobs
- React: profile edit, public profile view, handyman listing with filters, admin verification panel

[Full details →](HM_Technical_Requirements.md#phase-2--handyman-profiles--categories)

---

## Phase 3 — Jobs: Core Flow
**Goal:** The full job lifecycle works end-to-end.

- `Job` entity with status flow: Open → Awaiting Confirmation → In Progress → Completed / Cancelled
- `JobInterest` — handyman expresses interest + proposes price
- Client sees all interested handymen and selects one
- Handyman confirms (or declines) → job starts
- Multiple active jobs per client and per handyman supported
- React: job creation form, client dashboard, handyman feed, job detail page, interest/selection/confirmation UI

[Full details →](HM_Technical_Requirements.md#phase-3--jobs-the-core-flow)

---

## Phase 4 — Reviews, Ratings & Admin Dashboard
**Goal:** Trust signals visible across platform. Admin has full control.

- `Review` after completed job (client → handyman, 1–5 stars + comment)
- `ReviewResponse` — handyman replies publicly
- Handyman rating auto-calculated from all reviews
- Admin dashboard: manage users (ban/unban), view all jobs, moderate reviews
- React: review form, star rating component, review list on profiles, admin tables

[Full details →](HM_Technical_Requirements.md#phase-4--reviews-ratings--admin-dashboard)

---

## Phase 5 — In-App Notifications & UI Polish
**Goal:** Users are informed of key events. Platform feels complete.

- `Notification` entity with typed events (interest received, selected, confirmed, etc.)
- Notifications triggered automatically at each key action
- Notification bell with unread badge in navbar
- Full responsive design, loading/empty/error states, 404 page
- Rate limiting on auth endpoints, global error handling middleware

[Full details →](HM_Technical_Requirements.md#phase-5--in-app-notifications--ui-polish)

---

## Not For Build (Documented for Future Development)

The following features are planned but will **not** be implemented in the current scope. The platform works fully without them.

| Feature | Reason deferred |
|---------|----------------|
| Real-time chat (SignalR) | Significant complexity, not core to MVP |
| Document upload for verification | Admin verifies manually for now |
| Stripe subscription payments | Handled externally for now |
| Email notifications | In-app notifications are sufficient for MVP |
| Auto-expand geographic zone after 48h | Background job complexity |
| Grace period after subscription expiry | Payment system dependency |
| Admin analytics/charts dashboard | Phase 4 covers table views |
| Mobile application | Future milestone |
| SMS notifications | Twilio integration deferred |
| Dispute resolution system | Future milestone |
