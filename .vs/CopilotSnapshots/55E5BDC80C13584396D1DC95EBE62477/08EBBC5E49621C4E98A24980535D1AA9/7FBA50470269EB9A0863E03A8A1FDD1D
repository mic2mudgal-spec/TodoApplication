using Moq;
using NUnit.Framework;
using Todo.Application.DTOs.Requests;
using Todo.Application.Exceptions;
using Todo.Domain.Entities;
using Todo.Domain.Enums;
using Todo.Tests.Fixtures;
using TaskStatus = Todo.Domain.Enums.TaskStatus;

namespace Todo.Tests.Services;

[TestFixture]
public class TodoServiceTests
{
    private TodoServiceFixture _fixture = null!;

    [SetUp]
    public void SetUp()
    {
        _fixture = new TodoServiceFixture();
    }

    [Test]
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
        Assert.That(result.Count(), Is.EqualTo(2));
    }

    [Test]
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
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(id));
    }

    [Test]
    public async Task CreateAsync_ShouldCreateTask_WhenRequestIsValid()
    {
        // Arrange
        var request = new CreateTodoTaskRequest
        {
            Name = "Learn .NET",
            Priority = 1,
            Status = TaskStatus.NotStarted
        };

        _fixture.RepositoryMock
            .Setup(r => r.ExistsByNameAsync(request.Name))
            .ReturnsAsync(false);

        _fixture.RepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<TodoTask>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _fixture.Service.CreateAsync(request);

        // Assert
        Assert.That(result, Is.Not.Null);

        _fixture.RepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<TodoTask>()),
            Times.Once);
    }

    [Test]
    public async Task CreateAsync_ShouldThrowValidationException_WhenDuplicateExists()
    {
        // Arrange
        var request = new CreateTodoTaskRequest
        {
            Name = "Task 1",
            Priority = 1,
            Status = TaskStatus.NotStarted
        };

        _fixture.RepositoryMock
            .Setup(r => r.ExistsByNameAsync(request.Name))
            .ReturnsAsync(true);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ValidationException>(
            async () => await _fixture.Service.CreateAsync(request));
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public async Task UpdateAsync_ShouldUpdateTask()
    {
        // Arrange
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

        _fixture.RepositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<TodoTask>()))
            .Returns(Task.CompletedTask);

        // Act
        await _fixture.Service.UpdateAsync(request);

        // Assert
        _fixture.RepositoryMock.Verify(
            r => r.UpdateAsync(It.IsAny<TodoTask>()),
            Times.Once);
    }

    [Test]
    public async Task DeleteAsync_ShouldDeleteCompletedTask()
    {
        // Arrange
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

        _fixture.RepositoryMock
            .Setup(r => r.DeleteAsync(id))
            .Returns(Task.CompletedTask);

        // Act
        await _fixture.Service.DeleteAsync(id);

        // Assert
        _fixture.RepositoryMock.Verify(
            r => r.DeleteAsync(id),
            Times.Once);
    }

    [Test]
    public async Task DeleteAsync_ShouldThrowValidationException_WhenTaskIsNotCompleted()
    {
        // Arrange
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

        // Act & Assert
        var ex = Assert.ThrowsAsync<ValidationException>(
            async () => await _fixture.Service.DeleteAsync(id));
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public async Task DeleteAsync_ShouldThrowNotFoundException_WhenTaskDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();

        _fixture.RepositoryMock
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((TodoTask?)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<NotFoundException>(
            async () => await _fixture.Service.DeleteAsync(id));
        Assert.That(ex, Is.Not.Null);
    }
}