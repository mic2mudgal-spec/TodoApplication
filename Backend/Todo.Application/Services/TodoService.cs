using Todo.Application.DTOs.Requests;
using Todo.Application.DTOs.Responses;
using Todo.Application.Exceptions;
using Todo.Application.Interfaces;
using Todo.Application.Validation;
using Todo.Domain.Entities;
using Todo.Domain.Interfaces;

namespace Todo.Application.Services;

public class TodoService : ITodoService
{
    private readonly ITodoRepository _repository;

    public TodoService(ITodoRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<TodoTaskResponse>> GetAllAsync()
    {
        var tasks = await _repository.GetAllAsync();

        return tasks.Select(MapToResponse);
    }

    public async Task<TodoTaskResponse?> GetByIdAsync(Guid id)
    {
        var task = await _repository.GetByIdAsync(id);

        return task == null
            ? null
            : MapToResponse(task);
    }

    public async Task<TodoTaskResponse> CreateAsync(CreateTodoTaskRequest request)
    {
        if (await _repository.ExistsByNameAsync(request.Name))
            throw new ValidationException(ErrorMessages.TaskNameAlreadyExists);

        var task = new TodoTask
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Priority = request.Priority ?? 5,
            Status = request.Status
        };

        await _repository.AddAsync(task);

        return MapToResponse(task);
    }

    public async Task UpdateAsync(UpdateTodoTaskRequest request)
    {
        var existing = await _repository.GetByIdAsync(request.Id);

        if (existing == null)
            throw new NotFoundException(ErrorMessages.TaskNotFound);

        if (await _repository.ExistsByNameAsync(request.Name))
            throw new ValidationException(ErrorMessages.TaskNameAlreadyExists);

        existing.Name = request.Name.Trim();
        existing.Priority = request.Priority ?? existing.Priority;
        existing.Status = request.Status;

        await _repository.UpdateAsync(existing);
    }

    public async Task DeleteAsync(Guid id)
    {
        var task = await _repository.GetByIdAsync(id);

        if (task == null)
            throw new NotFoundException(ErrorMessages.TaskNotFound);

        if (task.Status != Todo.Domain.Enums.TaskStatus.Completed)
            throw new ValidationException(ErrorMessages.OnlyCompletedTasksCanBeDeleted);

        await _repository.DeleteAsync(id);
    }

    private static TodoTaskResponse MapToResponse(TodoTask task)
    {
        return new TodoTaskResponse
        {
            Id = task.Id,
            Name = task.Name,
            Priority = task.Priority,
            Status = task.Status
        };
    }
}