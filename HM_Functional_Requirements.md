# HelpMe — Functional Requirements

## Overview

**HelpMe** is a Bulgarian marketplace platform that connects clients with verified handymen (professionals offering home services). Clients post service requests, handymen express interest and propose a price, and the client selects the best fit. The platform covers a wide range of home services — plumbing, electrical, painting, cleaning, moving, gardening, AC installation, and more.

---

## User Roles

| Role | Description |
|---|---|
| **Client** | Posts service requests, communicates with handymen, selects and reviews handymen |
| **Handyman** | Creates a professional profile, expresses interest in jobs, proposes prices, confirms assignments |
| **Admin** | Verifies handymen, manages categories and areas, moderates users and content, views platform activity |

All users must provide: first name, last name, phone number, and email upon registration.

---

## Job Status Flow

```
Отворена
  └─► Изчакване на потвърждение  (client selected a handyman)
        └─► В процес              (handyman confirmed)
              ├─► Завършена
              └─► Отменена
  └─► Отменена                   (client cancels before selection)
```

- A job remains **Отворена** until the client selects a handyman — multiple handymen can express interest simultaneously.
- A client can have **multiple active jobs** at the same time.
- A handyman can be interested in or assigned to **multiple jobs** at the same time.
- If no handyman expresses interest within **48 hours**, the system automatically expands the geographic zone and notifies the client.
- After the client selects a handyman, the handyman must **confirm** within a reasonable window — only then does the job move to "В процес".

---

## Not-For-Build Features
*(Documented for future development — must NOT be implemented in current scope)*

- **Real-time chat** (SignalR) — planned as a future communication layer
- **Document upload** for handyman verification — Admin verification is manual for now
- **Stripe subscription payments** — handyman subscriptions are managed manually/externally for now
- **Email notifications** — in-app notifications only
- **Auto-expand geographic zone** after 48h inactivity — logic defined but not implemented
- **Grace period** (2 days post-subscription expiry) — defined but not enforced
- **Admin analytics dashboard** (charts, revenue stats) — Admin has table views only
- **Mobile application**
- **SMS notifications**
- **Dispute resolution system**
- **Insurance/guarantee integrations**
- **Promotions and featured listings for handymen**

The platform must function **fully and correctly** without any of the above features.

---

## Phase 1 — Foundation: Authentication & Core Structure

### Goal
A working, navigable application with user registration, login, and role-based access.

### Functional Requirements

**Registration & Login**
- User can register as a Client or Handyman
- User provides: first name, last name, email, phone number, password, role
- User can log in and log out
- Sessions are maintained securely
- Passwords are hashed

**Role-based Access**
- Clients, Handymen, and Admins see different navigation and pages
- Protected routes redirect unauthenticated users to login
- Admin account is seeded in the database

**Frontend**
- Register page with role selection
- Login page
- Basic navigation bar (role-aware)
- Protected route handling
- Auth state management (context/store)

---

## Phase 2 — Handyman Profiles & Categories

### Goal
Handymen can build their professional profile. Admins manage the service taxonomy. The platform has browsable categories and handyman listings.

### Functional Requirements

**Handyman Profile**
- After registration, handyman completes their profile: bio, years of experience, profile picture
- Handyman selects one or more **service categories** they work in
- Handyman selects one or more **areas/cities** they serve
- Profile is visible publicly once the Admin has verified the handyman
- Unverified handymen can log in but cannot receive job requests

**Service Categories**
- Categories have a name, icon/emoji, and optional description
- Categories support a parent-child hierarchy (e.g. "Ремонти" → "ВиК", "Електро")
- Admin can create, edit, and deactivate categories

**Areas**
- Areas represent Bulgarian cities/regions (seeded in DB)
- Handyman selects which areas they are willing to work in
- Admin can manage the list of areas

**Admin — Handyman Verification**
- Admin sees a list of unverified handymen
- Admin can approve or reject a handyman
- Rejected handymen receive an in-app notification

**Frontend**
- Handyman profile edit page
- Public handyman profile view page (bio, categories, rating, reviews)
- Admin panel: category management (CRUD)
- Admin panel: handyman verification queue
- Browsable handyman listing (filterable by category and area)

---

## Phase 3 — Jobs: The Core Flow

### Goal
The full job lifecycle works end-to-end: posting, interest, selection, confirmation, and completion.

### Functional Requirements

**Creating a Job (Client)**
- Client fills in: title, description, approximate budget, service category, area
- Client can have multiple active jobs simultaneously
- Job is created with status "Отворена"
- System identifies handymen who match the category and area and notifies them (in-app)

**Expressing Interest (Handyman)**
- Handyman sees a list of open jobs matching their categories and areas
- Handyman can express interest in a job by submitting a proposed price and optional note
- Handyman can express interest in multiple jobs simultaneously

**Selecting a Handyman (Client)**
- Client sees a list of handymen who expressed interest in their job (with proposed price, profile summary, rating)
- Client selects one handyman — job status moves to "Изчакване на потвърждение"
- Other interested handymen are notified that the job was filled

**Confirming the Job (Handyman)**
- Selected handyman sees pending confirmation requests
- Handyman confirms → job moves to "В процес"
- Handyman declines → job returns to "Отворена", client is notified

**Completing a Job**
- Either party can mark the job as complete (with the other party's agreement)
- Job moves to "Завършена"
- Client is prompted to leave a review

**Cancellation**
- Client can cancel a job at any status before "В процес"
- After "В процес", cancellation requires a reason

**Frontend**
- Job creation form (multi-step: category → details → location → review)
- Client's active jobs dashboard
- Handyman's job feed (filtered by their categories and areas)
- Job detail page
- List of interested handymen with prices (client view)
- Pending confirmations list (handyman view)

---

## Phase 4 — Reviews, Ratings & Admin Dashboard

### Goal
Trust and quality signals are visible across the platform. Admin has full moderation control.

### Functional Requirements

**Reviews**
- After a job is marked "Завършена", the client can leave one review per job
- Review includes: star rating (1–5) and written comment
- Handyman can publicly respond to a review (one response per review)
- Reviews are visible on the handyman's public profile
- Handyman's overall rating is the average of all received ratings, displayed with review count

**Admin Dashboard**
- Admin sees all users (Clients and Handymen) with ability to ban/unban
- Admin sees all jobs with their current status
- Admin sees all reviews with ability to remove inappropriate content
- Admin can manage (add/edit/deactivate) service categories and areas

**Frontend**
- Review form (shown after job completion)
- Star rating display on handyman profiles and listings
- Review list on handyman profile (with responses)
- Admin panel: user management table (ban/unban)
- Admin panel: jobs overview table
- Admin panel: reviews moderation table

---

## Phase 5 — In-App Notifications & UI Polish

### Goal
Users are always informed of important events. The platform feels complete and polished.

### Functional Requirements

**Notifications**
- In-app notifications are triggered by the following events:

| Event | Recipient |
|---|---|
| Handyman expresses interest in a job | Client |
| Client selects a handyman | Selected handyman |
| Client selects a handyman | Other interested handymen (job filled) |
| Handyman confirms the job | Client |
| Handyman declines the job | Client |
| Job is marked as complete | Both parties |
| Admin verifies/rejects handyman | Handyman |
| Client leaves a review | Handyman |

- Notifications are shown in a dropdown bell icon in the navbar
- Unread notifications show a badge count
- User can mark individual notifications as read or mark all as read

**UI Polish**
- Fully responsive design (mobile-friendly)
- Consistent design system across all pages
- Loading states and empty states for all lists
- Error messages for failed actions
- Form validation feedback
- 404 and error pages

**Frontend**
- Notification bell with badge in navbar
- Notification dropdown with list of recent notifications
- Mark as read functionality
- Responsive layout across all existing pages
- Unified component library (buttons, cards, forms, badges)
