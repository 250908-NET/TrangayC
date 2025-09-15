public class TaskItemService : ITaskItemService
{
    public IEnumerable<TaskItem> FilterTasks(
        IEnumerable<TaskItem> tasks,
        bool? isCompleted = null,
        PriorityLevel? priority = null,
        DateTime? dueBefore = null)
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

        return query.ToList();
    }
}
