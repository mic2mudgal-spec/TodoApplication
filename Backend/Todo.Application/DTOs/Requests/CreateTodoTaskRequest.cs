using Todo.Domain.Enums;

namespace Todo.Application.DTOs.Requests;

public class CreateTodoTaskRequest
{
    public string Name { get; set; } = string.Empty;

    public int Priority { get; set; }

    public Todo.Domain.Enums.TaskStatus Status { get; set; }
}