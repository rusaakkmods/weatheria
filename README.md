# Weatheria 🌦️

> A small, production-conscious weather application built with ASP.NET Core, CQRS, TDD, and Onion Architecture.

**Weatheria** is a lightweight weather application that allows users to explore countries and cities, retrieve current weather information, and save personal weather notes.

The name **Weatheria** is inspired by *Weatheria* from *One Piece* Manga/Anime — the weather science island where Nami spent time learning and mastering meteorology.

The reference is a small nod to the project's purpose: **understanding, observing, and working with weather**.

---

## 🚧 Implementation Status

**Current status: 🟡 Development — 6-hour MVP**

Weatheria is being developed as a focused technical assessment with an approximately **six-hour implementation window**.

The goal is not to build a large production platform. The goal is to deliver the **smallest complete implementation that satisfies all requirements while demonstrating good engineering judgment**.

### Progress

| Area | Status | Notes |
|---|:---:|---|
| Requirements | ✅ | Scope defined from assessment |
| Architecture | ✅ | Onion Architecture + CQRS |
| Development strategy | ✅ | Pragmatic TDD |
| Technology stack | ✅ | .NET 9 / ASP.NET Core |
| Project structure | ✅ | Single .NET project with logical architecture folders |
| Domain layer | ✅ | `WeatherNote` and temperature conversion covered by tests |
| Application / CQRS | 🟡 | Create Weather Note and country, city, and weather queries implemented |
| Infrastructure | 🟡 | SQLite persistence, static country/city catalog, and typed OpenWeather client implemented |
| EF Core persistence | ✅ | SQLite + `WeatherNoteRepository` + integration test |
| OpenWeather integration | ✅ | Typed client implemented; configure `OpenWeather:ApiKey` before live use |
| API | 🟡 | Country/city/weather reads and `POST /api/weather/notes` implemented and integration-tested |
| Frontend | 🟡 | Country → City → Weather → Save Weather Note flow implemented; favorites are not implemented |
| Automated tests | 🟡 | Unit, application, persistence, and API tests built continuously |
| Offline test execution | ✅ | Current tests do not require live weather API access |
| GitHub repository | 🟡 | Repository setup in progress |
| GitHub Actions CI | ⬜ | Planned |
| Deployment | ⬜ | Planned |
| Documentation | 🟡 | README + `AGENTS.md`; updated as implementation progresses |

### Status Legend

- ✅ **Completed** — implemented and verified
- 🟡 **In Progress** — currently being implemented
- ⬜ **Not Started** — planned
- 🔴 **Blocked** — requires a decision, dependency, or external action

> This status reflects the actual state of the repository and should be updated as implementation progresses.

---

# Table of Contents

- [Overview](#overview)
- [Scope and Time Constraint](#scope-and-time-constraint)
- [Requirements](#requirements)
- [Development Environment](#development-environment)
- [Architecture](#architecture)
- [CQRS](#cqrs)
- [Test-Driven Development](#test-driven-development)
- [Weather Integration](#weather-integration)
- [Persistence](#persistence)
- [Error Handling](#error-handling)
- [Testing Strategy](#testing-strategy)
- [Frontend](#frontend)
- [GitHub and CI/CD](#github-and-cicd)
- [Development Principles](#development-principles)
- [AI-Assisted Development](#ai-assisted-development)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [API Overview](#api-overview)
- [Design Decisions](#design-decisions)
- [Future Improvements](#future-improvements)
- [License](#license)

---

# Overview

Weatheria is a small web application designed around clear separation of concerns, testability, and maintainable code.

The application provides:

1. A list of available countries.
2. A list of cities for a selected country.
3. Current weather information for a selected city.
4. The ability to save a custom weather note.
5. A lightweight web interface.
6. Automated build and test validation through GitHub Actions.
7. Automated deployment after successful validation.

The project is intentionally implemented as a **small modular monolith** rather than a collection of distributed services.

The architecture is designed to be appropriate for the application's size while still demonstrating professional engineering practices.

---

# Scope and Time Constraint

Weatheria is intentionally scoped as a small application with an approximately **six-hour development window**.

The implementation prioritizes:

- Complete functional requirements
- Clear architectural boundaries
- Test-driven development
- Meaningful automated tests
- Maintainable and readable code
- Appropriate error handling
- CI/CD
- Clear documentation

The project intentionally avoids infrastructure that is not required by the assessment, such as:

- Microservices
- Message brokers
- Distributed transactions
- Redis or external caching infrastructure
- Authentication and authorization
- Background processing
- Kubernetes
- Container orchestration
- Event sourcing
- Separate read/write databases

This is a deliberate engineering decision rather than a limitation of the architecture.

> **The goal is not to build the largest possible system. The goal is to build the smallest complete system that demonstrates production-quality engineering judgment.**

---

# Requirements

Weatheria implements the following core workflow:

```text
Country
   ↓
City
   ↓
Current Weather
   ↓
Optional Weather Note
```

## Required API Operations

### Countries

```http
GET /api/countries
```

Returns available countries.

### Cities

```http
GET /api/countries/{countryCode}/cities
```

Returns cities belonging to the selected country.

### Weather

```http
GET /api/weather/{cityName}
```

Returns current weather information for the selected city.

### Weather Notes

```http
POST /api/weather/notes
```

Creates and persists a custom weather note.

---

# Development Environment

Weatheria is developed using:

| Tool | Version / Choice |
|---|---|
| Operating System | Windows 10 |
| IDE | Visual Studio Code |
| Runtime / SDK | .NET 9.0 |
| Language | C# |
| Source Control | Git |
| Repository | GitHub |
| CI/CD | GitHub Actions |

The project is intentionally designed to work using the standard .NET CLI.

Typical commands:

```powershell
dotnet restore
dotnet build
dotnet test
dotnet run
```

---

# Architecture

Weatheria follows a lightweight **Onion Architecture**.

```text
┌──────────────────────────────────────────┐
│                   API                    │
│ Controllers / Middleware / HTTP         │
└────────────────────┬─────────────────────┘
                     │
                     ▼
┌──────────────────────────────────────────┐
│              APPLICATION                 │
│ CQRS / MediatR / DTOs / Validation      │
│ Application Abstractions                │
└────────────────────┬─────────────────────┘
                     │
                     ▼
┌──────────────────────────────────────────┐
│                 DOMAIN                   │
│ Entities / Business Rules              │
│ No Infrastructure Dependencies          │
└──────────────────────────────────────────┘
                     ▲
                     │
┌────────────────────┴─────────────────────┐
│             INFRASTRUCTURE               │
│ EF Core / SQLite / HTTP / Weather API   │
└──────────────────────────────────────────┘
```

The important architectural rule is:

> **Dependencies point toward the application core.**

Infrastructure implements abstractions defined by the Application layer.

The Domain does not depend on Infrastructure.

---

## Domain

The Domain contains business concepts and deterministic business logic.

Examples:

- `WeatherNote`
- Temperature conversion

The Domain must not depend on:

- ASP.NET Core
- Entity Framework Core
- HTTP
- OpenWeather
- Database providers
- MediatR

---

## Application

The Application layer contains use cases.

Responsibilities include:

- CQRS commands
- CQRS queries
- MediatR handlers
- DTOs
- Validation
- Application abstractions
- Pipeline behaviors where useful

The Application layer depends on the Domain.

It does not directly depend on Infrastructure.

---

## Infrastructure

Infrastructure contains technical implementations.

Examples:

- Entity Framework Core
- SQLite
- Weather provider integration
- HTTP clients
- Persistence implementations
- Static country/city catalog

Infrastructure implements abstractions required by the Application layer.

---

## API

The API is responsible for HTTP concerns.

Responsibilities include:

- Routing
- Controllers
- HTTP request/response handling
- Dependency injection
- Exception handling
- API configuration

Controllers should remain thin.

The expected flow is:

```text
HTTP Request
     ↓
Controller
     ↓
MediatR
     ↓
Application Handler
     ↓
Domain / Infrastructure
     ↓
Response
```

Business logic should not be placed inside controllers.

---

# CQRS

Weatheria uses **Command Query Responsibility Segregation (CQRS)** through MediatR.

## Queries

Queries retrieve information without changing application state.

Examples:

```text
GetCountriesQuery
GetCitiesQuery
GetWeatherQuery
```

Example flow:

```text
GET /api/weather/{cityName}
          ↓
   GetWeatherQuery
          ↓
 GetWeatherQueryHandler
          ↓
    IWeatherService
          ↓
    Weather DTO
```

---

## Commands

Commands represent operations that change application state.

Example:

```text
POST /api/weather/notes
          ↓
CreateWeatherNoteCommand
          ↓
CreateWeatherNoteCommandHandler
          ↓
IWeatherNoteRepository
          ↓
       SQLite
```

The command returns the created resource identifier.

---

# Test-Driven Development

Weatheria follows a **pragmatic TDD workflow**.

For new behavior:

```text
Requirement
     ↓
Write failing test
     ↓
🔴 RED
     ↓
Minimum implementation
     ↓
🟢 GREEN
     ↓
Refactor
     ↓
🔵 CLEAN
```

The development process prioritizes behavior over implementation details.

Tests should verify what the system does rather than simply increase code coverage.

## TDD Rules

1. Define the smallest expected behavior.
2. Write a failing test.
3. Implement the minimum required code.
4. Make the test pass.
5. Refactor while keeping tests green.
6. Continue to the next behavior.

TDD is applied continuously throughout development rather than being postponed until the end of the project.

---

# Weather Integration

Weatheria isolates the external weather provider behind an application abstraction:

```csharp
IWeatherService
```

The dependency direction is:

```text
Application
     │
     │ IWeatherService
     ▼
Infrastructure
     │
     ▼
OpenWeather API
```

The Application layer does not know how OpenWeather works.

External API response models remain inside Infrastructure and are mapped into Weatheria's own application models.

This allows the external provider implementation to be changed without changing the application use cases.

The Infrastructure adapter calls OpenWeather's current-weather endpoint with imperial units. Set the API key through user secrets or the `OpenWeather__ApiKey` environment variable; `OpenWeather:BaseUrl` can optionally override the default API base URL. The adapter maps the provider payload into Application data and derives Fahrenheit dew point from temperature and relative humidity because the current-weather payload does not include dew point.

---

# Temperature Conversion

The weather provider supplies temperature information in Fahrenheit.

Weatheria converts Fahrenheit to Celsius using:

```text
°C = (°F - 32) × 5 / 9
```

The conversion is deterministic business logic and is tested independently.

Important edge cases include:

```text
32°F   → 0°C
212°F  → 100°C
-40°F  → -40°C
```

---

# Persistence

Weatheria uses **Entity Framework Core with SQLite** for persistence.

Only the weather note requires persistent storage in the initial scope.

The country and city catalog does not require a full geographic database. A small static catalog is sufficient for the assessment and keeps the implementation focused.

Persistence concerns remain in Infrastructure.

The Domain does not depend on Entity Framework Core.

---

# Error Handling

Weatheria uses centralized exception handling.

API errors should use standard `ProblemDetails` responses where appropriate.

Expected categories include:

- Validation errors
- Invalid country/city requests
- Resource not found
- External weather provider failures
- Unexpected application errors

Internal implementation details and sensitive information must not be exposed through API responses.

---

# Testing Strategy

Testing is an integral part of implementation.

```text
Tests/
├── Domain/
├── Application/
├── Infrastructure/
└── Api/
```

## Domain Tests

Test deterministic business behavior.

Example:

```text
TemperatureConverter
    ├── 32°F → 0°C
    ├── 212°F → 100°C
    └── -40°F → -40°C
```

---

## Application Tests

Test CQRS handlers independently from external infrastructure.

Examples:

```text
GetCountriesQueryHandler
GetCitiesQueryHandler
GetWeatherQueryHandler
CreateWeatherNoteCommandHandler
```

External services are replaced with mocks or fakes.

---

## Infrastructure Tests

Test actual infrastructure behavior where it matters.

For persistence, SQLite in-memory can be used to verify:

```text
Create Weather Note
        ↓
EF Core
        ↓
SaveChangesAsync()
        ↓
Record exists
```

---

## API Integration Tests

API tests verify the HTTP boundary and application pipeline.

Examples:

```text
GET /api/countries
GET /api/countries/{countryCode}/cities
GET /api/weather/{cityName}
POST /api/weather/notes
```

---

## Offline Testing

Automated tests must not require live access to OpenWeather.

Tests use mocks, fakes, or deterministic test implementations for external services.

The complete test suite must be executable offline.

---

# Frontend

The frontend is intentionally lightweight.

No frontend framework is required for the initial scope.

The UI uses:

- HTML
- CSS
- JavaScript

The primary workflow is:

```text
Country
   ↓
City
   ↓
Get Weather
   ↓
Display Weather
   ↓
Enter Note
   ↓
Save Note
```

The frontend communicates with the Weatheria API using HTTP.

The UI prioritizes usability and completion of the required workflow over visual complexity.

---

# GitHub and CI/CD

Weatheria is maintained in a GitHub repository and uses **GitHub Actions** for automated validation and deployment.

## Continuous Integration

Every pull request and push to the main branch should execute:

```text
Checkout
   ↓
Setup .NET 9
   ↓
Restore
   ↓
Build
   ↓
Test
```

The CI pipeline should fail if:

- Dependencies cannot be restored
- The project does not compile
- Tests fail

This provides a basic quality gate before changes are merged or deployed.

---

## Deployment

Deployment is handled through a separate GitHub Actions workflow.

The intended flow is:

```text
main
 ↓
Build
 ↓
Test
 ↓
Publish
 ↓
Deploy
```

Deployment must only proceed after the application successfully builds and tests pass.

The hosting platform is intentionally kept separate from the application architecture so that deployment infrastructure does not leak into the application code.

---

# Development Principles

### Keep boundaries explicit

Dependencies should point toward the application core.

### Prefer simple solutions

Architecture should solve real problems rather than introduce patterns for their own sake.

### Keep handlers focused

A handler should represent one application use case.

### Keep controllers thin

Controllers are HTTP adapters, not business logic containers.

### Avoid infrastructure leakage

Infrastructure-specific models and implementations should not leak into Domain or Application layers.

### Make dependencies explicit

Dependencies should be provided through dependency injection.

### Prefer deterministic tests

Tests should not depend on external services, network connectivity, or developer-specific environments.

### Use asynchronous APIs

I/O-bound operations should use asynchronous APIs and propagate `CancellationToken` where appropriate.

### Avoid premature abstractions

Introduce abstractions when they represent a meaningful architectural boundary.

Do not create abstractions solely because they are considered "enterprise patterns."

### Complete required work before optional work

Optional improvements must never delay or replace required functionality.

---

# AI-Assisted Development

Weatheria is intentionally developed with AI assistance.

AI-generated code must follow the project's requirements and architecture rather than define them.

The repository contains:

```text
AGENTS.md
```

This file defines:

- Architecture rules
- Layer boundaries
- CQRS conventions
- TDD requirements
- Naming conventions
- Testing requirements
- Dependency rules
- Code quality standards
- AI change discipline
- Scope limitations

## AI Delivery Rules

Because Weatheria has a strict six-hour implementation window:

1. Complete explicit requirements first.
2. Prefer the simplest correct implementation.
3. Follow TDD for new behavior.
4. Do not introduce speculative features.
5. Do not introduce unnecessary infrastructure.
6. Do not refactor unrelated code.
7. Do not bypass existing tests.
8. Keep changes small and verifiable.
9. Run relevant tests after changes.
10. Preserve architectural boundaries.

### Requirement Priority

```text
1. Explicit assessment requirements
2. Correctness
3. Architecture boundaries
4. Automated tests
5. Error handling
6. CI/CD
7. Documentation
8. Code elegance
9. Optional enhancements
```

Never sacrifice a required feature for an optional architectural improvement.

---

# Project Structure

Weatheria intentionally uses a **single .NET project with logical architectural folders**.

This keeps the project simple while making the architecture explicit.

```text
Weatheria/
│
├── Domain/
│   ├── Entities/
│   └── Services/
│
├── Application/
│   ├── Abstractions/
│   ├── Features/
│   │   ├── Countries/
│   │   ├── Weather/
│   │   └── WeatherNotes/
│   └── Common/
│
├── Infrastructure/
│   ├── Persistence/
│   ├── Weather/
│   └── Catalog/
│
├── Api/
│   ├── Controllers/
│   └── Middleware/
│
├── Tests/
│   ├── Domain/
│   ├── Application/
│   ├── Infrastructure/
│   └── Api/
│
├── frontend/
│   ├── index.html
│   ├── app.js
│   └── style.css
│
├── .github/
│   └── workflows/
│       ├── ci.yml
│       └── deploy.yml
│
├── AGENTS.md
├── README.md
├── .editorconfig
├── .gitignore
└── Weatheria.csproj
```

The single-project approach is deliberate. The architectural boundaries are logical rather than unnecessarily split into multiple projects.

---

# Getting Started

## Prerequisites

- Windows 10
- .NET 9 SDK
- Visual Studio Code
- Git

## Clone

```powershell
git clone <repository-url>
cd Weatheria
```

## Restore

```powershell
dotnet restore
```

## Build

```powershell
dotnet build
```

## Test

```powershell
dotnet test
```

## Run

```powershell
dotnet run
```

The application will expose the API and frontend according to the configured ASP.NET Core environment.

---

# API Overview

| Method | Endpoint | Purpose | Status |
|---|---|---|:---:|
| GET | `/api/countries` | Retrieve available countries | ✅ |
| GET | `/api/countries/{countryCode}/cities` | Retrieve cities for a country | ✅ |
| GET | `/api/weather/{cityName}` | Retrieve current weather | ✅ |
| POST | `/api/weather/notes` | Save a weather note | ✅ |

---

# Design Decisions

## Why Onion Architecture?

The application needs to demonstrate clear separation between business logic, use cases, infrastructure, and HTTP concerns.

Onion Architecture provides that separation without requiring a distributed or multi-service architecture.

---

## Why CQRS?

The application contains distinct read and write operations.

Queries retrieve information.

Commands change state.

Separating these responsibilities keeps individual use cases focused.

---

## Why MediatR?

MediatR provides a consistent mechanism for dispatching CQRS requests and allows application-level pipeline behaviors such as validation to be introduced without coupling controllers to handlers.

---

## Why TDD?

The project has a short implementation window and explicit testing requirements.

TDD provides a fast feedback loop:

```text
Test
 ↓
Implementation
 ↓
Refactor
```

It also helps prevent unnecessary abstractions because new architecture is introduced in response to observable requirements.

---

## Why SQLite?

SQLite provides relational persistence without requiring a separate database server.

This keeps local development and deployment simple while still demonstrating real EF Core persistence.

---

## Why a Static Country/City Catalog?

The assessment requires country and city selection but does not require a geographic database.

A small static catalog satisfies the requirement without spending the limited implementation time building unrelated geographic infrastructure.

---

## Repository Strategy

Weatheria uses focused repository abstractions only where application-owned data requires persistence.

For example:

```text
IWeatherNoteRepository
        ↓
WeatherNoteRepository
        ↓
EF Core
        ↓
SQLite
```

The project intentionally does **not** introduce a generic repository abstraction such as `IRepository<T>`, a generic repository implementation, or a Unit of Work abstraction. These patterns would add indirection without solving a current requirement.

Repositories should expose only the operations required by the application's use cases.

---

## Why Countries and Cities Are Not Persisted

Weatheria distinguishes between **reference data** and **application-owned persistent data**.

Countries and cities are treated as reference/catalog data. They are not user-owned resources and do not have an application lifecycle that requires database persistence.

Therefore:

```text
Country / City
      ↓
Static Catalog
```

rather than:

```text
Country / City
      ↓
EF Core
      ↓
SQLite
```

This avoids introducing database tables, migrations, repositories, and seed-data management for information that does not require runtime persistence.

The application-owned weather notes are different. They have a lifecycle and must survive application restarts:

```text
Weather Note
      ↓
IWeatherNoteRepository
      ↓
EF Core
      ↓
SQLite
```

This results in a deliberate separation:

| Data | Storage | Reason |
|---|---|---|
| Countries | Static catalog | Reference data |
| Cities | Static catalog | Reference data |
| Current weather | External weather provider | External/volatile data |
| Weather notes | SQLite via EF Core | Application-owned persistent data |

This is an intentional architectural decision. **Production quality does not mean persisting every model; it means selecting an appropriate persistence strategy for each type of data.**

---

## Why a Lightweight Frontend?

The assessment primarily evaluates backend architecture, CQRS, persistence, testing, and integration.

A simple HTML/CSS/JavaScript frontend is sufficient to demonstrate the required user workflow without introducing unnecessary frontend infrastructure.

---

## Why GitHub Actions?

Automated build and test validation provides a basic quality gate.

Deployment is separated from the application code and executed only after successful validation.

This demonstrates practical CI/CD without introducing unnecessary DevOps infrastructure.

---

# Future Improvements

The architecture leaves room for future improvements if the project grows.

Potential future work includes:

- Weather response caching
- Multiple weather providers
- Weather forecasts
- Historical weather
- Favorite cities
- User accounts
- Authentication and authorization
- Rate limiting
- Structured logging
- OpenTelemetry
- PostgreSQL
- Background weather refresh
- More advanced frontend presentation

These are intentionally **outside the initial six-hour scope**.

---

# Project Philosophy

Weatheria is built around a simple idea:

> **Keep the core clean, keep the boundaries clear, and keep the complexity where it belongs.**

The goal is not to demonstrate how many technologies can be placed into one application.

The goal is to demonstrate that a small application can be built with:

- Good architecture
- TDD
- Meaningful tests
- Clean code
- Appropriate abstractions
- Automated CI/CD
- Clear documentation

while remaining conscious of time, scope, and actual requirements.

---

# License

This project is created as a technical assessment and learning project.

License information will be added as appropriate.