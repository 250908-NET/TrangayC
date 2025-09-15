using Microsoft.AspNetCore.Mvc;
ITaskItemService service = new TaskItemService();
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();  

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

List<TaskItem> taskList = new List<TaskItem>();

var apiPath = app.MapGroup("api");
//GET /api/tasks - Get all tasks with optional filtering
//Query parameters: IsCompleted, Priority, DueBefore
apiPath.MapGet("/tasks", (bool? isCompleted, PriorityLevel? priority, DateTime? dueBefore) => 
{
    var allTasks = service.FilterTasks(taskList, isCompleted, priority, dueBefore);
    return allTasks;
});

//GET /api/tasks/{id} - Get specific task by ID
apiPath.MapGet("/tasks/{id:int}", (int id) =>
{
    var task = taskList.FirstOrDefault(t => t.Id == id);
    if (task == null) return Results.NotFound();

    return Results.Ok(task);
});

//POST /api/tasks - Create new task
/*
Example body:
{
  "title": "Finish API Documentation",
  "description": "Complete the Swagger docs for all endpoints",
  "isCompleted": false,
  "priority": 1,
  "dueDate": "2025-09-30T17:00:00Z"
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

    // Basic validation for required fields
    if (string.IsNullOrWhiteSpace(task.Title)) return Results.BadRequest("Title is required.");
    
    task.CreatedAt = task.CreatedAt == default ? DateTime.Now : task.CreatedAt;
    task.UpdatedAt = DateTime.Now;

    taskList.Add(task);
    return Results.Created($"/api/tasks/{task.Id}", task);
});

//PUT /api/tasks/{id} - Update existing task
apiPath.MapPut("/tasks/{id:int}", (int id, TaskItem updatedTask) =>
{
    var task = taskList.FirstOrDefault(t => t.Id == id);
    if (task is null) return Results.NotFound();

    if (string.IsNullOrWhiteSpace(updatedTask.Title)) return Results.BadRequest("Title is required.");

    task.Title = updatedTask.Title;
    task.Description = updatedTask.Description;
    task.IsCompleted = updatedTask.IsCompleted;
    task.Priority = updatedTask.Priority;
    task.DueDate = updatedTask.DueDate;
    task.UpdatedAt = DateTime.Now;

    return Results.Ok(task);
});

//DELETE /api/tasks/{id} - Delete task
apiPath.MapDelete("/tasks/{id:int}", (int id) =>
{
    var task = taskList.FirstOrDefault(t => t.Id == id);
    if (task is null) return Results.NotFound();

    taskList.Remove(task);
    return Results.NoContent();
});

app.Run();
