using Todo.Application.DTOs.Requests;

namespace Todo.Application.Validation;

public class ValidationContext
{
    public CreateTodoTaskRequest? CreateRequest { get; init; }

    public UpdateTodoTaskRequest? UpdateRequest { get; init; }

    public bool IsCreate => CreateRequest is not null;

    public bool IsUpdate => UpdateRequest is not null;
}