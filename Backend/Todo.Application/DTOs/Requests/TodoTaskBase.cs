using System.ComponentModel.DataAnnotations;
using Todo.Application.Validation;
using TaskStatus = Todo.Domain.Enums.TaskStatus;

public abstract class TodoTaskBase
{
    [Required(ErrorMessage = "Task Name is required.")]
    [MaxLength(100, ErrorMessage = "Name max length is 100")]
    public string? Name { get; set; }

    [Required]
    [Range(1, 10, ErrorMessage = "Priority must be between 1 and 10.")]
    public int? Priority { get; set; }

    [Required]
    [EnumValidation(typeof(TaskStatus),
        ErrorMessage = "Invalid task status.")]
    public TaskStatus Status { get; set; }
}