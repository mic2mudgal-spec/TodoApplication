using Todo.Application.DTOs.Requests;
using Todo.Application.DTOs.Responses;

namespace Todo.Application.Interfaces;

public interface ITodoService
{
    Task<IEnumerable<TodoTaskResponse>> GetAllAsync();

    Task<TodoTaskResponse?> GetByIdAsync(Guid id);

    Task<TodoTaskResponse> CreateAsync(CreateTodoTaskRequest request);

    Task UpdateAsync(UpdateTodoTaskRequest request);

    Task DeleteAsync(Guid id);
}