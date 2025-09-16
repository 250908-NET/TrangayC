using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public enum PriorityLevel
{
    Low,
    Medium,
    High,
    Critical
}

public class TaskItem
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required (ErrorMessage = "Title is required")]
    [StringLength(100, ErrorMessage = "Title must not exceed 100 characters")] 
    public required string Title { get; set; }
    [StringLength(500, ErrorMessage = "Description must not exceed 500 characters")]
    public string? Description { get; set; }
    public bool IsCompleted { get; set; } = false;
    [EnumDataType(typeof(PriorityLevel))]
    public PriorityLevel Priority { get; set; }
    [DataType(DataType.DateTime)]
    public DateTime? DueDate { get; set; }
    [DataType(DataType.DateTime)]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    [DataType(DataType.DateTime)]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public TaskItem() { }

    public TaskItem(int id, string title, string description, bool isCompleted, PriorityLevel priority, DateTime dueDate, DateTime createdAt, DateTime updatedAt) {
        this.Id = id;
        this.Title = title;
        this.Description = description;
        this.IsCompleted = isCompleted;
        this.Priority = priority;
        this.DueDate = dueDate;
        this.CreatedAt = createdAt;
        this.UpdatedAt = updatedAt;
    }
}