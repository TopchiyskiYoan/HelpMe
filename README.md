# HelpMe

**HelpMe** is a Bulgarian marketplace platform connecting clients with verified handymen for home services — plumbing, electrical work, painting, cleaning, moving, gardening, AC installation, and more.

Clients post a service request with a description and approximate budget. Handymen in the matching area browse open jobs, express interest, and propose their price. The client reviews all interested handymen — their profiles, ratings, and offered prices — and selects one. The handyman then confirms the assignment, and the job begins.

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend | ASP.NET Core 8, C# |
| Database | MS SQL Server (LocalDB), Entity Framework Core 8 |
| Auth | ASP.NET Core Identity + JWT |
| Frontend | Vite + React (JavaScript) |
| Testing | NUnit, Moq |

## Solution Structure

```
HelpMe/
├── HelpMe.Domain/        # Entities, enums, interfaces
├── HelpMe.Application/   # Business logic services, DTOs
├── HelpMe.Infrastructure/# EF Core, DbContext, migrations, seed data
├── HelpMe.Web/           # ASP.NET Core 8 Web API
├── HelpMe.Tests/         # NUnit + Moq unit tests
└── helpme-frontend/      # Vite + React SPA
```

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
dotnet run
```

API runs at: `https://localhost:7000`

### Frontend

```bash
cd helpme-frontend
npm install
npm run dev
```

Frontend runs at: `http://localhost:5173`

## Development Phases

See [PHASES.md](PHASES.md) for a full breakdown of the development roadmap.

## Project Requirements

- [Functional Requirements](HM_Functional_Requirements.md)
- [Technical Requirements](HM_Technical_Requirements.md)
