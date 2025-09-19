# Task Management API

A minimal ASP.NET Core 9 Web API for managing in-memory task items. The API demonstrates clean minimal endpoints, validation with Data Annotations, filtering/sorting via a service, and logging with Serilog (including custom middleware for automatic request log formatting).

## Prerequisites
- .NET SDK 9.0
- Windows, macOS, or Linux

## Getting Started

1. Restore and build
```bash
 dotnet restore
 dotnet build
```

2. Run the API
```bash
 dotnet run
```

3. Default URLs
- HTTPS: https://localhost:5237 (port may vary)

Check `Properties/launchSettings.json` for the exact ports.

## Logging
- Serilog is configured via `Program.cs` with:
  - Logs shown in Console 
  - Logs stored in `Logs/log-<date>.txt`
- A custom middleware (`Middleware/RequestLoggingMiddleware.cs`) logs every request in the format:
  - `Method: (GET|POST|PUT|DELETE), Path: (/api/... ), Response Time: (X ms)`
  - Logs are information level for 2xx/3xx; warning for 4xx/5xx.

## OpenAPI
- OpenAPI is enabled in Development via `app.MapOpenApi()`.
- Visit `/openapi/v1.json` when running in Development to view the JSON description.

## Data Model
`Models/TaskItem.cs`
```csharp
public enum PriorityLevel { Low, Medium, High, Critical }

public class TaskItem
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public PriorityLevel Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

## Services
`Services/TaskItemService.cs`
- `FilterTasks(...)` supports filters: `isCompleted`, `priority`, `dueBefore`, `title` (case-insensitive contains), `description` (case-insensitive contains).
- `Sort(...)` supports: `createdAt` (default), `dueDate`, `priority`.

## Endpoints
Base path group: `/api`

### 1) GET /api/tasks
Retrieve all tasks with optional filtering and sorting.

Query parameters (all optional):
- `isCompleted` (bool)
- `priority` (Low = 1, Medium = 2, High = 3, Critical = 4)
- `dueBefore` (datetime)
- `title` (string, case-insensitive)
- `description` (string, case-insensitive)
- `sortBy` (createdAt|dueDate|priority)

Example (no filters):
```
GET /api/tasks
```
Expected 200 OK response body:
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "title": "Documentation",
      "description": null,
      "isCompleted": true,
      "priority": 0,
      "dueDate": null,
      "createdAt": "2025-09-18T19:42:29.8058436-04:00",
      "updatedAt": "2025-09-18T19:42:29.8058834-04:00"
    }
  ],
  "message": "Operation completed successfully"
}
```
Example (filter and sort):
```
GET /api/tasks?isCompleted=false
```
Expected 200 OK response body:
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "title": "Documentation",
      "description": null,
      "isCompleted": false,
      "priority": 0,
      "dueDate": null,
      "createdAt": "2025-09-18T19:42:29.8058436-04:00",
      "updatedAt": "2025-09-18T19:42:29.8058834-04:00"
    }
  ],
  "message": "Operation completed successfully"
}
```

### 2) GET /api/tasks/{id}
Retrieve a single task by its id.

Example
```
GET /api/tasks/4
```
Expected 200 OK (found):
```json
{
  "success": true,
  "data": { /* TaskItem */ },
  "message": "Operation completed successfully"
}
```
Expected 404 Not Found (missing):
```json
{
  "success": false,
  "error": "Task does not exist for provided id: 4",
  "message": "Operation failed"
}
```

### 3) POST /api/tasks
Create a new task.

Example
```json
POST /api/tasks
{
  "title": "Finish Documentation",
  "description": "Review Documentation",
  "isCompleted": false,
  "priority": 1,
  "dueDate": "2025-09-30T17:00:00Z"
}
```
Expected 201 Created:
```json
{
  "success": true,
  "data": { /* TaskItem */ },
  "message": "Operation completed successfully"
}
```
Expected 400 Bad Request (validation errors):
```json
if title: ""
{
  "success": false,
  "errors": [
    "Title is required"
  ],
  "message": "Operation failed"
}
```

### 4) PUT /api/tasks/{id}
Update an existing task.

Example
```json
PUT /api/tasks/3
{
  "title": "Finish Documentation - Updated",
  "description": "Review Documentation - Updated",
  "isCompleted": true,
  "priority": 2,
  "dueDate": "2025-10-01T10:00:00Z"
}
```
Expected 200 OK:
```json
{
  "success": true,
  "data": { /* Updated TaskItem */ },
  "message": "Operation completed successfully"
}
```
Expected 404 Not Found:
```json
{
  "success": false,
  "error": "Task does not exist for provided id: 3",
  "message": "Operation failed"
}
```
Expected 400 Bad Request (validation errors):
```json
{
  "success": false,
  "errors": [ /* validation error messages */ ],
  "message": "Operation failed"
}
```

### 5) DELETE /api/tasks/{id}
Delete an existing task by id.

Example
```json
DELETE /api/tasks/3
```
Expected 200 OK:
```json
{
  "success": true,
  "data": { /* Deleted TaskItem */ },
  "message": "Operation completed successfully"
}
```
Expected 404 Not Found:
```json
{
  "success": false,
  "error": "Task does not exist for provided id: 3",
  "message": "Operation failed"
}
```

### 6) GET /api/tasks/statistics
Return aggregate statistics for tasks.

Example
```json
GET /api/tasks/statistics
```
Expected 200 OK response body:
```json
{
  "success": true,
  "data": {
    "totalTasks": 10,
    "completedTasks": 4,
    "overdueTasks": 2,
    "tasksByPriority": [
      { "priority": "Low", "count": 2 },
      { "priority": "Medium", "count": 3 },
      { "priority": "High", "count": 4 },
      { "priority": "Critical", "count": 1 }
    ]
  },
  "message": "Operation completed successfully"
}
```

## Testing Tips
- Use any REST client (e.g., Postman, Thunder Client).
- Since the data store is in-memory (`List<TaskItem>` in `Program.cs`), data resets when the program restarts.

## Running Tests
This repository includes an xUnit test project (`task_management_api.Tests/`)

Run all tests from the solution root:
```bash
dotnet test
```

## Project Structure
```
.
├── Models/
│   └── TaskItem.cs
├── Services/
│   ├── ITaskItemService.cs
│   └── TaskItemService.cs
├── Middleware/
│   └── RequestLoggingMiddleware.cs
├── helpers/
│   └── ModelValidationHelper.cs
├── Program.cs
├── task_management_api.csproj
└── README.md
```

## Contributing
- Use conventional commits (e.g., `feat: ...`, `fix: ...`).
- Follow .NET coding conventions and the style used in this repository.
