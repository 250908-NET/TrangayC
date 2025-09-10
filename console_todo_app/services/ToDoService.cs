using System.Text;

public class ToDoService : IToDoService
{
    private List<ToDoItem> toDoItems = new List<ToDoItem>();

    public void addItem(string item)
    {
        // Generate a unique, increasing ID even after deletions
        int nextId = toDoItems.Count == 0 ? 1 : toDoItems.Max(i => i.Id) + 1;
        toDoItems.Add(new ToDoItem(nextId, item, false, DateTime.Now));
    }

    public string listItems()
    {
        bool hasItems = toDoItems.Any();
        if (!hasItems)
        {
            return "No items.";
        }
        var Result = new StringBuilder();
        Result.AppendLine("==== ToDo Items ====");
        foreach (var item in toDoItems)
        {
            var status = item.IsCompleted ? "Done" : "Pending";
            Result.AppendLine($"{item.Id}. {item.Title}");
            Result.AppendLine($"   Status : {status}");
            Result.AppendLine($"   Created: {item.CreatedDate:g}");
            Result.AppendLine();
        }
        return Result.ToString();
    }

    public bool markItemComplete(int itemId)
    {
        var itemExists = toDoItems.FirstOrDefault(i => i.Id == itemId);
        if (itemExists == null)
        {
            return false;
        }
        itemExists.IsCompleted = true;
        return true;
    }

    public void deleteItem(int itemId)
    {
        var itemExists = toDoItems.FirstOrDefault(i => i.Id == itemId);
        if (itemExists != null)
        {
            toDoItems.Remove(itemExists);
        }
    }

    public void exit()
    {
        // No-op for console; handled in Program.cs loop
    }
}