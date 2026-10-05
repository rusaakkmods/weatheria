# Integration and Regression Test Report

Date: 2026-10-05

## Migration-backed API integration test

Command:

```powershell
dotnet test Weatheria.Tests/Weatheria.Tests.csproj --filter FullyQualifiedName~CreateWeatherNoteEndpointTests
```

Result: **Passed, 1 test**.

The API test host starts with a fresh SQLite in-memory database. Application startup applied migration `20261005113311_InitialCreate`, created the `WeatherNotes` table, and `POST /api/weather/notes` returned `201 Created` after persisting a note.

## Full regression suite

Command:

```powershell
dotnet test
```

Result: **Passed, 24 tests; 0 failed; 0 skipped**. The build completed successfully as part of the test run.

Coverage includes domain temperature conversion, CQRS handlers, SQLite persistence, API endpoints, and frontend hosting. OpenWeather tests use mocks or a stub HTTP handler; tests make no live network calls.
