public class ToDoItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedDate { get; set; }

    public ToDoItem(int id, string title, bool isCompleted, DateTime createdDate)
    {
        this.Id = id;
        this.Title = title;
        this.IsCompleted = isCompleted;
        this.CreatedDate = createdDate;
    }
}
