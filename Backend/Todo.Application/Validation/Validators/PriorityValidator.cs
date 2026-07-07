using Todo.Application.Exceptions;

namespace Todo.Application.Validation.Validators;

public class PriorityValidator : ITaskValidator
{
    public Task ValidateAsync(ValidationContext context)
    {
        var priority = context.IsCreate
            ? context.CreateRequest!.Priority
            : context.UpdateRequest!.Priority;

        if (priority < 1)
            throw new ValidationException(
                "Priority must be greater than zero.");

        return Task.CompletedTask;
    }
}
