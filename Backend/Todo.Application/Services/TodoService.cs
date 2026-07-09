using Todo.Application.DTOs.Requests;
using Todo.Application.DTOs.Responses;
using Todo.Application.Exceptions;
using Todo.Application.Interfaces;
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
            throw new ValidationException("Task name already exists.");

        var task = new TodoTask
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Priority = request.Priority,
            Status = request.Status
        };

        await _repository.AddAsync(task);

        return MapToResponse(task);
    }

    public async Task UpdateAsync(UpdateTodoTaskRequest request)
    {
        var existing = await _repository.GetByIdAsync(request.Id);

        if (existing == null)
            throw new NotFoundException("Task not found.");

        if (await _repository.ExistsByNameAsync(request.Name, request.Id))
            throw new ValidationException("Task name already exists.");

        existing.Name = request.Name.Trim();
        existing.Priority = request.Priority;
        existing.Status = request.Status;

        await _repository.UpdateAsync(existing);
    }

    public async Task DeleteAsync(Guid id)
    {
        var task = await _repository.GetByIdAsync(id);

        Console.WriteLine("========== DELETE ==========");
        Console.WriteLine($"Id: {id}");

        if (task == null)
        {
            Console.WriteLine("Task NOT FOUND");
            throw new NotFoundException("Task not found.");
        }

        Console.WriteLine($"Name   : {task.Name}");
        Console.WriteLine($"Status : {(int)task.Status} ({task.Status})");

        if (task.Status != Todo.Domain.Enums.TaskStatus.Completed)
        {
            Console.WriteLine("Validation failed - Task is not completed.");
            throw new ValidationException(
                "Only completed tasks can be deleted.");
        }

        Console.WriteLine("Calling Repository.DeleteAsync...");

        await _repository.DeleteAsync(id);

        Console.WriteLine("Task deleted successfully.");
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