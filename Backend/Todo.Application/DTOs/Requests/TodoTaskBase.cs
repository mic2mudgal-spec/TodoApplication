using System.ComponentModel.DataAnnotations;
using TaskStatus = Todo.Domain.Enums.TaskStatus;

public abstract class TodoTaskBase
{
    [Required(ErrorMessage = "Task Name is required.")]
    [MaxLength(100)]
    public string? Name { get; set; }

    [Required]
    [Range(1, 10)]
    public int? Priority { get; set; }

    [Required]
    public TaskStatus Status { get; set; }
}