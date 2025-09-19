# Task Management API – Test Suite

This project contains the automated test suite for the Task Management API located in `../task_management_api/`. 

## Contents
- `EndpointsTests.cs` – Integration tests for `/api/tasks` endpoints.
- `Usings.cs` – Global usings for refactoring.
- `task_management_api.Tests.csproj` – Test project configuration and package references.

## Tech Stack
- .NET 9 (C# 13)
- xUnit
- FluentAssertions
- Microsoft.AspNetCore.Mvc.Testing
## What’s Covered
The suite in `EndpointsTests.cs` validates the API endpoints in `task_management_api/Program.cs`:
- GET `/api/tasks` returns 200 OK with an empty list when no data exists.
- GET `/api/tasks` supports filtering and sorting (e.g., `isCompleted`, `priority`, `dueBefore`, `title`, `description`, `sortBy`).
- GET `/api/tasks/{id}` returns 404 with an error body when not found; 200 with the expected payload when found.
- POST `/api/tasks` validates input (title required, max lengths) and returns 201 Created with the new resource.
- PUT `/api/tasks/{id}` validates input, returns 404 when missing, and 200 OK when updated.
- DELETE `/api/tasks/{id}` returns 404 when missing, and 200 OK when deleted.

These tests assert both HTTP status codes and the unified response envelope shape:
```json
{
  "success": true,
  "data": {},
  "message": "Operation completed successfully"
}
or 
{
  "success": false,
  "errors": [],
  "message": "Operation failed"
}
```

## Prerequisites
- .NET SDK 9.0+

No external database or services are required; the API under test uses an in-memory list and is bootstrapped via `WebApplicationFactory<Program>`. The API exposes `public partial class Program` which enables testing.

## Running Tests
```csharp
dotnet test
```

## Test Conventions
- Arrange/Act/Assert structure.
- Use `WebApplicationFactory<Program>` per test to ensure isolation.
- Prefer `FluentAssertions` for readable, intention-revealing assertions.
- Validate both status codes and response body shape (success flag, message, and error details when applicable).

## Adding New Tests
1. Create a new `[Fact]` (or `[Theory]`) in `EndpointsTests.cs` or a new test file.
2. Use the factory helper to spin up the app and an `HttpClient`:
   ```csharp
   using var factory = new WebApplicationFactory<Program>();
   using var client = factory.CreateClient();
   ```
3. Mock the desired API route with `client.GetAsync`, `PostAsJsonAsync`, etc.
4. Assert on `StatusCode` and parse the JSON body with `ReadFromJsonAsync<JsonElement>()` for flexible checks.

## Troubleshooting
- Build failures: Ensure you can build the API project `../task_management_api/` (the test project references it).
- Missing endpoints: Confirm the route definitions in `task_management_api/Program.cs` and that `public partial class Program { }` exists.
- HTTPS errors: The tests use the default test server configuration via `WebApplicationFactory`; no additional HTTPS setup is required.
