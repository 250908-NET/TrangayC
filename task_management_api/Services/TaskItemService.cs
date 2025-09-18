public class TaskItemService : ITaskItemService
{
    public IEnumerable<TaskItem> FilterTasks(
        IEnumerable<TaskItem> tasks,
        bool? isCompleted = null,
        PriorityLevel? priority = null,
        DateTime? dueBefore = null,
        string? title = null,
        string? description = null)
    {
        var query = tasks;

        if (isCompleted is not null)
        {
            query = query.Where(task => task.IsCompleted == isCompleted.Value);
        }

        if (priority is not null)
        {
            query = query.Where(task => task.Priority == priority.Value);
        }

        if (dueBefore is not null)
        {
            query = query.Where(task => task.DueDate is not null && task.DueDate < dueBefore.Value);
        }

        if (!string.IsNullOrWhiteSpace(title))
        {
            query = query.Where(task => task.Title?.Contains(title, StringComparison.OrdinalIgnoreCase) == true);
        }

        if (!string.IsNullOrWhiteSpace(description))
        {
            query = query.Where(task => task.Description?.Contains(description, StringComparison.OrdinalIgnoreCase) == true);
        }

        return query.ToList();
    }

    public IEnumerable<TaskItem> Sort(IEnumerable<TaskItem> tasks, string? sortBy)
    {
        if (tasks is null) return Array.Empty<TaskItem>();

        return (sortBy?.ToLowerInvariant()) switch
        {
            "duedate" => tasks.OrderBy(t => t.DueDate ?? DateTime.MaxValue),
            "priority" => tasks.OrderBy(t => t.Priority),
            "createdat" => tasks.OrderBy(t => t.CreatedAt),
            _ => tasks.OrderBy(t => t.CreatedAt)
        };
    }
}
