namespace Todo.Application.Validation;

public interface ITaskValidator
{
    Task ValidateAsync(ValidationContext context);
}