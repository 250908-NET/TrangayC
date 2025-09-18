public interface ITaskItemService
{
    IEnumerable<TaskItem> FilterTasks(
        IEnumerable<TaskItem> tasks,
        bool? isCompleted = null,
        PriorityLevel? priority = null,
        DateTime? dueBefore = null);

    IEnumerable<TaskItem> Sort(IEnumerable<TaskItem> tasks, string? sortBy);
}
