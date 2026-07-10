using Todo.Application.Exceptions;
using Todo.Domain.Interfaces;

namespace Todo.Application.Validation.Validators;

public class DuplicateNameValidator : ITaskValidator
{
    private readonly ITodoRepository _repository;

    public DuplicateNameValidator(ITodoRepository repository)
    {
        _repository = repository;
    }

    public async Task ValidateAsync(ValidationContext context)
    {
        if (context.IsCreate)
        {
            if (await _repository.ExistsByNameAsync(
                context.CreateRequest!.Name))
            {
                throw new ValidationException(
                    ErrorMessages.TaskNameAlreadyExists);
            }
        }
        else
        {
            if (await _repository.ExistsByNameAsync(
                context.UpdateRequest!.Name,
                context.UpdateRequest.Id))
            {
                throw new ValidationException(
                    ErrorMessages.TaskNameAlreadyExists);
            }
        }
    }
}
