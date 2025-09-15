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
    [Required] 
    [MaxLength(100)] 
    public string Title { get; set; } = "Empty title";
    [MaxLength(500)]
    public string? Description { get; set; }
    public bool IsCompleted { get; set; } = false;
    public PriorityLevel Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
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