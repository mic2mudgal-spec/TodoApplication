using Todo.Domain.Entities;

namespace Todo.Domain.Interfaces;

public interface ITodoRepository
{
    Task<IEnumerable<TodoTask>> GetAllAsync();

    Task<TodoTask?> GetByIdAsync(Guid id);

    Task AddAsync(TodoTask task);

    Task UpdateAsync(TodoTask task);

    Task DeleteAsync(Guid id);

    Task<bool> ExistsByNameAsync(string name);

    Task<bool> ExistsByNameAsync(string name, Guid excludeId);
}