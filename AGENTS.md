# Weatheria AI Development Instructions

## Project

Weatheria is a small weather application built as a technical assessment.

The implementation has an approximately six-hour development window.

The objective is to produce the smallest complete implementation that satisfies the requirements while demonstrating clean architecture, TDD, maintainability, and practical engineering judgment.

---

## Technology

- OS: Windows 10
- IDE: Visual Studio Code
- Runtime: .NET 9
- Language: C#
- API: ASP.NET Core
- Persistence: Entity Framework Core + SQLite
- CQRS: MediatR
- Validation: FluentValidation
- Testing: xUnit
- Source control: Git
- CI/CD: GitHub Actions

---

## Architecture

Weatheria follows a lightweight Onion Architecture.

Logical layers:

- Domain
- Application
- Infrastructure
- API

Dependency direction:

```text
API
 ↓
Application
 ↓
Domain

Infrastructure
 ↓
Application / Domain
```

Infrastructure must not leak implementation details into Domain.

Application defines abstractions for external dependencies where appropriate.

Controllers must remain thin.

Business logic belongs in Domain or Application, not controllers.

---

## Repository Guidelines

Use focused repository interfaces for application-owned persistent data.

Example:

IWeatherNoteRepository

Do not introduce generic repository abstractions such as:

- IRepository<T>
- GenericRepository<T>
- RepositoryFactory
- UnitOfWork

unless a concrete requirement demonstrates that they are necessary.

Repositories should expose only operations required by the application's use cases.

---

## Persistence Strategy

Weatheria intentionally distinguishes reference data from application-owned persistent data.

Countries and cities are reference/catalog data and are not persisted in SQLite.

They should be provided through a catalog abstraction, for example:

ICountryCatalog
    ↓
CountryCatalog

Do not introduce EF Core entities, repositories, database tables, or migrations for countries/cities unless explicitly required.

Weather notes are application-owned persistent data and use:

IWeatherNoteRepository
    ↓
WeatherNoteRepository
    ↓
EF Core
    ↓
SQLite

This is an intentional design decision. Production quality does not require every model to be persisted; persistence should be applied where the application owns the data lifecycle.

---

## CQRS

Use CQRS through MediatR.

Queries:

- Read data
- Must not change application state

Commands:

- Change application state
- Return an appropriate result

Each use case should have a focused request and handler.

Do not introduce unnecessary generic CQRS abstractions.

---

## Test-Driven Development

Use pragmatic TDD for new behavior.

Follow:

```text
RED
 ↓
GREEN
 ↓
REFACTOR
```

Workflow:

1. Define the smallest behavior.
2. Write a failing test.
3. Implement the minimum required code.
4. Make the test pass.
5. Refactor while keeping tests green.
6. Continue to the next behavior.

Tests should verify behavior rather than implementation details.

Do not write tests solely to increase code coverage.

---

## Testing Rules

Tests must run offline.

Never make live OpenWeather API calls from automated tests.

Use mocks, fakes, or deterministic test implementations for external services.

Use real SQLite in-memory persistence tests when testing EF Core persistence behavior.

Use API integration tests to verify HTTP behavior and the application pipeline.

Every significant domain behavior and application use case should have automated tests.

---

## External Weather Service

The external weather provider must be accessed through:

```csharp
IWeatherService
```

Application code must not directly create or use an OpenWeather HTTP client.

The OpenWeather implementation belongs in Infrastructure.

External API models should not leak into Domain.

---

## Validation

Use FluentValidation for request validation.

Validation should happen at the application boundary rather than being duplicated across controllers and handlers.

Do not create unnecessary validation abstractions.

---

## Error Handling

Use centralized exception handling.

Prefer standard ASP.NET Core `ProblemDetails` responses.

Do not expose internal exceptions, stack traces, secrets, or infrastructure details to API consumers.

---

## Async Code

Use asynchronous APIs for I/O-bound operations.

Propagate `CancellationToken` where appropriate.

Do not introduce artificial asynchronous wrappers around synchronous operations.

---

## Code Quality

Prefer:

- Small focused classes
- Explicit dependencies
- Clear naming
- Immutable DTOs where appropriate
- Guard clauses
- Dependency injection
- Async/await for I/O
- Simple implementations

Avoid:

- God classes
- Static service locators
- Hidden dependencies
- Unnecessary inheritance
- Premature abstractions
- Speculative framework code
- Excessive design patterns

---

## Scope Control

This is a six-hour implementation.

Do not introduce:

- Microservices
- Message brokers
- Redis
- Kubernetes
- Event sourcing
- Distributed transactions
- Background workers
- Authentication
- Authorization
- User management
- Separate read/write databases
- Complex caching infrastructure

unless explicitly required later.

Complete required functionality before optional improvements.

---

## AI Change Discipline

Before changing code:

1. Understand the requirement.
2. Inspect the relevant existing code.
3. Identify the smallest change required.
4. Write or update tests first when adding behavior.
5. Implement the change.
6. Run relevant tests.
7. Refactor only when justified.

Do not:

- Rewrite unrelated files
- Change architecture without justification
- Add dependencies without need
- Remove tests to make the build pass
- Disable compiler warnings merely to hide problems
- Introduce speculative features

When there are multiple valid solutions, prefer the simplest solution that satisfies the requirement and preserves architectural boundaries.

---

## Requirement Priority

Priority order:

1. Explicit assessment requirements
2. Correctness
3. Architecture boundaries
4. Automated tests
5. Error handling
6. CI/CD
7. Documentation
8. Code quality refinements
9. Optional enhancements

Never sacrifice a required feature for an optional architectural improvement.

---

## Verification

After meaningful changes, run:

```powershell
dotnet build
dotnet test
```

Before considering the project complete:

- Build must succeed.
- All tests must pass.
- Tests must run offline.
- Required API endpoints must work.
- Frontend workflow must work.
- CI must pass.
- Deployment must succeed.
- README must reflect actual implementation status.