using Moq;
using Todo.Application.DTOs.Requests;
using Todo.Application.Exceptions;
using Todo.Domain.Entities;
using Todo.Domain.Enums;
using Todo.Tests.Fixtures;
using Xunit;
using TaskStatus = Todo.Domain.Enums.TaskStatus;

namespace Todo.Tests.Services;

public class TodoServiceTests
{
    private readonly TodoServiceFixture _fixture;

    public TodoServiceTests()
    {
        _fixture = new TodoServiceFixture();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllTasks()
    {
        // Arrange
        var tasks = new List<TodoTask>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Task 1",
                Priority = 1,
                Status = TaskStatus.NotStarted
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Task 2",
                Priority = 2,
                Status = TaskStatus.Completed
            }
        };

        _fixture.RepositoryMock
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(tasks);

        // Act
        var result = await _fixture.Service.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnTask_WhenTaskExists()
    {
        // Arrange
        var id = Guid.NewGuid();

        var task = new TodoTask
        {
            Id = id,
            Name = "Learn Testing",
            Priority = 1,
            Status = TaskStatus.NotStarted
        };

        _fixture.RepositoryMock
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(task);

        // Act
        var result = await _fixture.Service.GetByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result!.Id);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateTask_WhenRequestIsValid()
    {
        var request = new CreateTodoTaskRequest
        {
            Name = "Learn .NET",
            Priority = 1,
            Status = TaskStatus.NotStarted
        };

        _fixture.RepositoryMock
            .Setup(r => r.ExistsByNameAsync(request.Name))
            .ReturnsAsync(false);

        var result = await _fixture.Service.CreateAsync(request);

        Assert.NotNull(result);

        _fixture.RepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<TodoTask>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowValidationException_WhenDuplicateExists()
    {
        var request = new CreateTodoTaskRequest
        {
            Name = "Task 1",
            Priority = 1,
            Status = TaskStatus.NotStarted
        };

        _fixture.RepositoryMock
            .Setup(r => r.ExistsByNameAsync(request.Name))
            .ReturnsAsync(true);

        await Assert.ThrowsAsync<ValidationException>(
            () => _fixture.Service.CreateAsync(request));
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateTask()
    {
        var id = Guid.NewGuid();

        var entity = new TodoTask
        {
            Id = id,
            Name = "Old",
            Priority = 1,
            Status = TaskStatus.NotStarted
        };

        var request = new UpdateTodoTaskRequest
        {
            Id = id,
            Name = "New",
            Priority = 3,
            Status = TaskStatus.Completed
        };

        _fixture.RepositoryMock
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(entity);

        _fixture.RepositoryMock
            .Setup(r => r.ExistsByNameAsync(request.Name, request.Id))
            .ReturnsAsync(false);

        await _fixture.Service.UpdateAsync(request);

        _fixture.RepositoryMock.Verify(
            r => r.UpdateAsync(It.IsAny<TodoTask>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteCompletedTask()
    {
        var id = Guid.NewGuid();

        var entity = new TodoTask
        {
            Id = id,
            Name = "Completed",
            Priority = 1,
            Status = TaskStatus.Completed
        };

        _fixture.RepositoryMock
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(entity);

        await _fixture.Service.DeleteAsync(id);

        _fixture.RepositoryMock.Verify(
            r => r.DeleteAsync(id),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowValidationException_WhenTaskIsNotCompleted()
    {
        var id = Guid.NewGuid();

        var entity = new TodoTask
        {
            Id = id,
            Name = "Task",
            Priority = 1,
            Status = TaskStatus.InProgress
        };

        _fixture.RepositoryMock
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(entity);

        await Assert.ThrowsAsync<ValidationException>(
            () => _fixture.Service.DeleteAsync(id));
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowNotFoundException_WhenTaskDoesNotExist()
    {
        var id = Guid.NewGuid();

        _fixture.RepositoryMock
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((TodoTask?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _fixture.Service.DeleteAsync(id));
    }
}