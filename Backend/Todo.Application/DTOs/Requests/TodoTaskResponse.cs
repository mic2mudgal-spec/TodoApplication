using System.ComponentModel.DataAnnotations;
using TaskStatus = Todo.Domain.Enums.TaskStatus;

namespace Todo.Application.DTOs.Responses;

public class TodoTaskResponse : TodoTaskBase
{
    public Guid Id { get; set; }

}