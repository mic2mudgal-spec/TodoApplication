using Todo.Domain.Entities;
using Todo.Domain.Interfaces;
using TaskStatus = Todo.Domain.Enums.TaskStatus;

namespace Todo.Infrastructure.Repositories;

public class InMemoryTodoRepository : ITodoRepository
{
    private readonly List<TodoTask> _tasks = [];
    private readonly object _lock = new();


    public InMemoryTodoRepository()
    {
        _tasks.Add(new TodoTask() {
            Id = Guid.NewGuid(),
            Name = "Task 1",
        Priority = 1,
        Status= TaskStatus.NotStarted
        });
        _tasks.Add(new TodoTask()
        {
            Id= Guid.NewGuid(),
            Name = "Task 2",
            Priority = 1,
            Status=TaskStatus.InProgress
        });
        _tasks.Add(new TodoTask()
        {
            Id = Guid.NewGuid(),
            Name = "Task 3",
            Priority = 1,
            Status = TaskStatus.Completed
        });
    }
    public Task<IEnumerable<TodoTask>> GetAllAsync()
    {
        lock (_lock)
        {
            return Task.FromResult(_tasks.AsEnumerable());
        }
    }

    public Task<TodoTask?> GetByIdAsync(Guid id)
    {
        lock (_lock)
        {
            return Task.FromResult(
                _tasks.FirstOrDefault(t => t.Id == id));
        }
    }

    public Task AddAsync(TodoTask task)
    {
        lock (_lock)
        {
            _tasks.Add(task);
        }

        return Task.CompletedTask;
    }

    public Task UpdateAsync(TodoTask task)
    {
        lock (_lock)
        {
            var existing = _tasks.First(t => t.Id == task.Id);

            existing.Name = task.Name;
            existing.Priority = task.Priority;
            existing.Status = task.Status;
        }

        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        lock (_lock)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);

            if (task != null)
                _tasks.Remove(task);
        }

        return Task.CompletedTask;
    }

    public Task<bool> ExistsByNameAsync(string name)
    {
        lock (_lock)
        {
            return Task.FromResult(
                _tasks.Any(t =>
                    t.Name.Equals(name,
                        StringComparison.OrdinalIgnoreCase)));
        }
    }

    public Task<bool> ExistsByNameAsync(string name, Guid excludeId)
    {
        lock (_lock)
        {
            return Task.FromResult(
                _tasks.Any(t =>
                    t.Id != excludeId &&
                    t.Name.Equals(name,
                        StringComparison.OrdinalIgnoreCase)));
        }
    }
}