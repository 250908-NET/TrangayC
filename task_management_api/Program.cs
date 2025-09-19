using Microsoft.AspNetCore.Mvc;
using Serilog;
ITaskItemService service = new TaskItemService();
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// Configure Serilog for structured logging
builder.Host.UseSerilog((ctx, services, lc) => lc
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day));

var app = builder.Build();  

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
// Use custom request logging middleware (replaces Serilog's default request logging)
app.UseMiddleware<RequestLoggingMiddleware>();

List<TaskItem> taskList = new List<TaskItem>();

var apiPath = app.MapGroup("api");
//GET /api/tasks - Get all tasks with optional filtering and sorting
//Query parameters: isCompleted, priority, dueBefore, sortBy (createdAt|dueDate|priority)
//Example query for sorting: /api/tasks?sortBy=dueDate
//Example query for filtering: /api/tasks?isCompleted=false&dueBefore=2025-09-30
//Example query for filtering by title: /api/tasks?title=Documentation
apiPath.MapGet("/tasks", (bool? isCompleted, PriorityLevel? priority, DateTime? dueBefore, string? title, string? description, string? sortBy) => 
{
    var filteredTasks = service.FilterTasks(taskList, isCompleted, priority, dueBefore, title, description);
    var sortedTasks = service.Sort(filteredTasks, sortBy);
    var body = new { success = true, data = sortedTasks, message = "Operation completed successfully" };
    return Results.Ok(body);
});

//GET /api/tasks/{id} - Get specific task by ID
apiPath.MapGet("/tasks/{id:int}", (int id) =>
{
    var task = taskList.FirstOrDefault(t => t.Id == id);
    if (task == null) 
    {
        var errorMessage = $"Task does not exist for provided id: {id}";
        var errorBody = new { success = false, error = errorMessage, message = "Operation failed" };
        return Results.NotFound(errorBody);
    }
    var body = new { success = true, data = task, message = "Operation completed successfully" };
    return Results.Ok(body);
});

//POST /api/tasks - Create new task
/*
Example body:
{
  "title": "Finish Documentation",
  "description": "Review Documentation",
  "isCompleted": false,
  "priority": 1,
  "dueDate": "2025-09-30"
}
*/
apiPath.MapPost("/tasks", ([FromBody] TaskItem task) => 
{
    // Assign a new ID if not provided
    if (task.Id <= 0)
    {
        var nextId = taskList.Count == 0 ? 1 : taskList.Max(t => t.Id) + 1;
        task.Id = nextId;
    }

    // Validate using Data Annotations
    var (isValid, validationResults) = ModelValidationHelper.Validate(task);
    if (!isValid)
    {
        var errorMessages = ModelValidationHelper.GetErrorMessages(validationResults);
        var errorBody = new { success = false, errors = errorMessages, message = "Operation failed" };
        return Results.BadRequest(errorBody);
    }

    task.CreatedAt = DateTime.Now;
    task.UpdatedAt = DateTime.Now;

    taskList.Add(task);
    var body = new { success = true, data = task, message = "Operation completed successfully" };
    return Results.Created($"/api/tasks/{task.Id}", body);
});

//PUT /api/tasks/{id} - Update existing task
apiPath.MapPut("/tasks/{id:int}", (int id, TaskItem updatedTask) =>
{
    var task = taskList.FirstOrDefault(t => t.Id == id);
    if (task is null) 
    {
        var errorMessage = $"Task does not exist for provided id: {id}";
        var errorBody = new { success = false, error = errorMessage, message = "Operation failed" };
        return Results.NotFound(errorBody);
    }

    // Validate using Data Annotations
    var (isValid, validationResults) = ModelValidationHelper.Validate(updatedTask);
    if (!isValid)
    {
        var errorMessages = ModelValidationHelper.GetErrorMessages(validationResults);
        var errorBody = new { success = false, errors = errorMessages, message = "Operation failed" };
        return Results.BadRequest(errorBody);
    }

    task.Title = updatedTask.Title;
    task.Description = updatedTask.Description;
    task.IsCompleted = updatedTask.IsCompleted;
    task.Priority = updatedTask.Priority;
    task.DueDate = updatedTask.DueDate;
    task.UpdatedAt = DateTime.Now;

    var body = new { success = true, data = task, message = "Operation completed successfully" };
    return Results.Ok(body);
});

//DELETE /api/tasks/{id} - Delete task
apiPath.MapDelete("/tasks/{id:int}", (int id) =>
{
    var task = taskList.FirstOrDefault(t => t.Id == id);
    
    if (task is null) 
    {
        var errorMessage = $"Task does not exist for provided id: {id}";
        var errorBody = new { success = false, error = errorMessage, message = "Operation failed" };
        return Results.NotFound(errorBody);
    }

    taskList.Remove(task);
    var body = new { success = true, data = task, message = "Operation completed successfully" };    
    return Results.Ok(body);
});

app.Run();

public partial class Program { }
