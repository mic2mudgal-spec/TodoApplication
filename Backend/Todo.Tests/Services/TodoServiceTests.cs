using Moq;
using NUnit.Framework;
using Todo.Application.DTOs.Requests;
using Todo.Application.DTOs.Responses;
using Todo.Application.Exceptions;
using Todo.Application.Validation;
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
    public void SetUp() => _fixture = new TodoServiceFixture();

    #region Create Tests

    [Test]
    public async Task CreateTask_Positive_WithValidRequest_ReturnsTaskResponse()
    {
        // Arrange
        var request = BuildCreateRequest(name: "New Task", priority: 5);
        MockRepositoryNameExists(false);

        // Act
        var result = await _fixture.Service.CreateAsync(request);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("New Task"));
        Assert.That(result.Priority, Is.EqualTo(5));
        _fixture.RepositoryMock.Verify(r => r.AddAsync(It.IsAny<TodoTask>()), Times.Once);
    }

    [Test]
    public void CreateTask_Negative_WhenNameExists_ThrowsValidationException()
    {
        // Arrange
        var request = BuildCreateRequest(name: "Existing");
        MockRepositoryNameExists(true);

        // Act & Assert
        Assert.ThrowsAsync<ValidationException>(async () =>
            await _fixture.Service.CreateAsync(request));
    }

    [Test]
    public void CreateTask_Negative_WhenNameEmpty_ThrowsValidationException()
    {
        // Arrange
        var request = BuildCreateRequest(name: "   ");
        MockRepositoryNameExists(false);

        // Act & Assert
        Assert.ThrowsAsync<ValidationException>(async () =>
            await _fixture.Service.CreateAsync(request));
    }

    [TestCase(1)]
    [TestCase(5)]
    [TestCase(10)]
    public async Task CreateTask_Positive_WithValidPriority_ReturnsTaskResponse(int priority)
    {
        // Arrange
        var request = BuildCreateRequest(priority: priority);
        MockRepositoryNameExists(false);

        // Act
        var result = await _fixture.Service.CreateAsync(request);

        // Assert
        Assert.That(result.Priority, Is.EqualTo(priority));
    }

    [Test]
    public async Task CreateTask_Positive_WithNullPriority_UsesDefaultPriority()
    {
        // Arrange
        var request = BuildCreateRequest(priority: null);
        MockRepositoryNameExists(false);

        // Act
        var result = await _fixture.Service.CreateAsync(request);

        // Assert
        Assert.That(result.Priority, Is.EqualTo(5));
    }

    [Test]
    public async Task CreateTask_Positive_WithWhitespaceName_TrimsSuccessfully()
    {
        // Arrange
        var request = BuildCreateRequest(name: "  Task Name  ");
        MockRepositoryNameExists(false);

        // Act
        var result = await _fixture.Service.CreateAsync(request);

        // Assert
        Assert.That(result.Name, Is.EqualTo("Task Name"));
    }

    [TestCase(TaskStatus.NotStarted)]
    [TestCase(TaskStatus.InProgress)]
    [TestCase(TaskStatus.Completed)]
    public async Task CreateTask_Positive_WithVariousStatuses_ReturnsCorrectStatus(TaskStatus status)
    {
        // Arrange
        var request = BuildCreateRequest(status: status);
        MockRepositoryNameExists(false);

        // Act
        var result = await _fixture.Service.CreateAsync(request);

        // Assert
        Assert.That(result.Status, Is.EqualTo(status));
    }

    #endregion

    #region Update Tests

    [Test]
    public async Task UpdateTask_Positive_WithAllFieldsChanged_UpdatesSuccessfully()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existing = BuildTodoTask(taskId, "Old", 3, TaskStatus.NotStarted);
        var request = BuildUpdateRequest(taskId, "New", 8, TaskStatus.InProgress);
        MockGetById(taskId, existing);
        MockRepositoryNameExists(false);

        // Act
        await _fixture.Service.UpdateAsync(request);

        // Assert
        VerifyUpdateAsyncCalled("New", 8, TaskStatus.InProgress);
    }

    [Test]
    public void UpdateTask_Negative_WhenTaskNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var request = BuildUpdateRequest(taskId);
        MockGetById(taskId, null);

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () =>
            await _fixture.Service.UpdateAsync(request));
    }

    //[Test]
    public void UpdateTask_Negative_WhenNameAlreadyExists_ThrowsValidationException()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existing = BuildTodoTask(taskId, "Old", 5, TaskStatus.NotStarted);
        var request = BuildUpdateRequest(taskId, "Taken Name");
        MockGetById(taskId, existing);
        MockRepositoryNameExists(true, excludeId: taskId);

        // Act & Assert
        Assert.ThrowsAsync<ValidationException>(async () =>
            await _fixture.Service.UpdateAsync(request));
    }

    [TestCase(1)]
    [TestCase(5)]
    [TestCase(10)]
    public async Task UpdateTask_Positive_WithValidPriority_UpdatesSuccessfully(int priority)
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existing = BuildTodoTask(taskId);
        var request = BuildUpdateRequest(taskId, priority: priority);
        MockGetById(taskId, existing);
        MockRepositoryNameExists(false);

        // Act
        await _fixture.Service.UpdateAsync(request);

        // Assert
        VerifyUpdateAsyncCalledWithPriority(priority);
    }

    [Test]
    public async Task UpdateTask_Positive_WithNullPriority_KeepsExistingPriority()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existing = BuildTodoTask(taskId, priority: 7);
        var request = new UpdateTodoTaskRequest
        {
            Id = taskId,
            Name = "Updated",
            Priority = null,
            Status = TaskStatus.NotStarted
        };
        MockGetById(taskId, existing);
        MockRepositoryNameExists(false);

        // Act
        await _fixture.Service.UpdateAsync(request);

        // Assert
        VerifyUpdateAsyncCalledWithPriority(7);
    }

    #endregion

    #region Delete Tests

    [Test]
    public async Task DeleteTask_Positive_WhenTaskCompleted_DeletesSuccessfully()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = BuildTodoTask(taskId, status: TaskStatus.Completed);
        MockGetById(taskId, task);

        // Act
        await _fixture.Service.DeleteAsync(taskId);

        // Assert
        _fixture.RepositoryMock.Verify(r => r.DeleteAsync(taskId), Times.Once);
    }

    [Test]
    public void DeleteTask_Negative_WhenTaskNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        MockGetById(taskId, null);

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () =>
            await _fixture.Service.DeleteAsync(taskId));
    }

    [TestCase(TaskStatus.NotStarted)]
    [TestCase(TaskStatus.InProgress)]
    public void DeleteTask_Negative_WhenTaskIncomplete_ThrowsValidationException(TaskStatus status)
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = BuildTodoTask(taskId, status: status);
        MockGetById(taskId, task);

        // Act & Assert
        Assert.ThrowsAsync<ValidationException>(async () =>
            await _fixture.Service.DeleteAsync(taskId));
    }

    #endregion

    #region Get Tests

    [Test]
    public async Task GetTaskById_Positive_WhenTaskExists_ReturnsTask()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = BuildTodoTask(taskId, "Found", 7, TaskStatus.InProgress);
        MockGetById(taskId, task);

        // Act
        var result = await _fixture.Service.GetByIdAsync(taskId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(taskId));
        Assert.That(result.Name, Is.EqualTo("Found"));
        Assert.That(result.Priority, Is.EqualTo(7));
    }

    [Test]
    public async Task GetTaskById_Negative_WhenTaskNotFound_ReturnsNull()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        MockGetById(taskId, null);

        // Act
        var result = await _fixture.Service.GetByIdAsync(taskId);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetAllTasks_Positive_WhenMultipleExist_ReturnsAllTasks()
    {
        // Arrange
        var task1 = BuildTodoTask(Guid.NewGuid(), "Task1", 1, TaskStatus.NotStarted);
        var task2 = BuildTodoTask(Guid.NewGuid(), "Task2", 2, TaskStatus.InProgress);
        var task3 = BuildTodoTask(Guid.NewGuid(), "Task3", 3, TaskStatus.Completed);
        var tasks = new List<TodoTask> { task1, task2, task3 };
        _fixture.RepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(tasks);

        // Act
        var result = await _fixture.Service.GetAllAsync();
        var resultList = result.ToList();

        // Assert
        Assert.That(resultList.Count, Is.EqualTo(3));
        Assert.That(resultList[0].Name, Is.EqualTo("Task1"));
        Assert.That(resultList[1].Name, Is.EqualTo("Task2"));
        Assert.That(resultList[2].Name, Is.EqualTo("Task3"));
    }

    [Test]
    public async Task GetAllTasks_Positive_WhenNoTasks_ReturnsEmptyList()
    {
        // Arrange
        _fixture.RepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<TodoTask>());

        // Act
        var result = await _fixture.Service.GetAllAsync();
        var resultList = result.ToList();

        // Assert
        Assert.That(resultList.Count, Is.EqualTo(0));
    }

    #endregion

    #region Invalid GUID Tests

    [Test]
    public async Task GetTaskById_WithEmptyGuid_CallsRepository()
    {
        // Arrange
        var emptyGuid = Guid.Empty;
        MockGetById(emptyGuid, null);

        // Act
        var result = await _fixture.Service.GetByIdAsync(emptyGuid);

        // Assert
        Assert.That(result, Is.Null);
        _fixture.RepositoryMock.Verify(r => r.GetByIdAsync(emptyGuid), Times.Once);
    }

    [Test]
    public void UpdateTask_WithEmptyGuid_ThrowsNotFoundException()
    {
        // Arrange
        var emptyGuid = Guid.Empty;
        var request = BuildUpdateRequest(emptyGuid);
        MockGetById(emptyGuid, null);

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () =>
            await _fixture.Service.UpdateAsync(request));
    }

    [Test]
    public void DeleteTask_WithEmptyGuid_ThrowsNotFoundException()
    {
        // Arrange
        var emptyGuid = Guid.Empty;
        MockGetById(emptyGuid, null);

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(async () =>
            await _fixture.Service.DeleteAsync(emptyGuid));
    }

    #endregion

    #region Incomplete JSON Tests

    [Test]
    public async Task CreateTask_Positive_IncompleteJson_OnlyNameProvided_UseDefaults()
    {
        // Arrange
        var request = new CreateTodoTaskRequest
        {
            Name = "Task",
            Priority = null,
            Status = TaskStatus.NotStarted
        };
        MockRepositoryNameExists(false);

        // Act
        var result = await _fixture.Service.CreateAsync(request);

        // Assert
        Assert.That(result.Name, Is.EqualTo("Task"));
        Assert.That(result.Priority, Is.EqualTo(5));
        Assert.That(result.Status, Is.EqualTo(TaskStatus.NotStarted));
    }

    [Test]
    public async Task CreateTask_Positive_IncompleteJson_NameAndPriorityProvided_UseDefaults()
    {
        // Arrange
        var request = new CreateTodoTaskRequest
        {
            Name = "Task",
            Priority = 7,
            Status = TaskStatus.NotStarted
        };
        MockRepositoryNameExists(false);

        // Act
        var result = await _fixture.Service.CreateAsync(request);

        // Assert
        Assert.That(result.Name, Is.EqualTo("Task"));
        Assert.That(result.Priority, Is.EqualTo(7));
        Assert.That(result.Status, Is.EqualTo(TaskStatus.NotStarted));
    }

    [Test]
    public async Task CreateTask_Positive_IncompleteJson_NameAndStatusProvided_UseDefaultPriority()
    {
        // Arrange
        var request = new CreateTodoTaskRequest
        {
            Name = "Task",
            Priority = null,
            Status = TaskStatus.InProgress
        };
        MockRepositoryNameExists(false);

        // Act
        var result = await _fixture.Service.CreateAsync(request);

        // Assert
        Assert.That(result.Name, Is.EqualTo("Task"));
        Assert.That(result.Priority, Is.EqualTo(5));
        Assert.That(result.Status, Is.EqualTo(TaskStatus.InProgress));
    }

    [Test]
    public async Task UpdateTask_Positive_IncompleteJson_OnlyNameChanged()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existing = BuildTodoTask(taskId, "Old", 5, TaskStatus.NotStarted);
        var request = new UpdateTodoTaskRequest
        {
            Id = taskId,
            Name = "Updated",
            Priority = null,
            Status = TaskStatus.NotStarted
        };
        MockGetById(taskId, existing);
        MockRepositoryNameExists(false);

        // Act
        await _fixture.Service.UpdateAsync(request);

        // Assert
        VerifyUpdateAsyncCalled("Updated", 5, TaskStatus.NotStarted);
    }

    [Test]
    public async Task UpdateTask_Positive_IncompleteJson_NameAndStatusChanged_PriorityUnchanged()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existing = BuildTodoTask(taskId, "Old", 7, TaskStatus.NotStarted);
        var request = new UpdateTodoTaskRequest
        {
            Id = taskId,
            Name = "Updated",
            Priority = null,
            Status = TaskStatus.Completed
        };
        MockGetById(taskId, existing);
        MockRepositoryNameExists(false);

        // Act
        await _fixture.Service.UpdateAsync(request);

        // Assert
        VerifyUpdateAsyncCalled("Updated", 7, TaskStatus.Completed);
    }

    #endregion

    #region Helpers

    private CreateTodoTaskRequest BuildCreateRequest(
        string name = "Test Task",
        int? priority = 5,
        TaskStatus status = TaskStatus.NotStarted) =>
        new()
        {
            Name = name,
            Priority = priority,
            Status = status
        };

    private UpdateTodoTaskRequest BuildUpdateRequest(
        Guid id,
        string name = "Updated Task",
        int? priority = 5,
        TaskStatus status = TaskStatus.NotStarted) =>
        new()
        {
            Id = id,
            Name = name,
            Priority = priority,
            Status = status
        };

    private TodoTask BuildTodoTask(
        Guid? id = null,
        string name = "Task",
        int priority = 5,
        TaskStatus status = TaskStatus.NotStarted) =>
        new()
        {
            Id = id ?? Guid.NewGuid(),
            Name = name,
            Priority = priority,
            Status = status
        };

    private void MockGetById(Guid id, TodoTask? task) =>
        _fixture.RepositoryMock
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(task);

    private void MockRepositoryNameExists(bool exists, Guid? excludeId = null)
    {
        if (excludeId.HasValue)
        {
            _fixture.RepositoryMock
                .Setup(r => r.ExistsByNameAsync(It.IsAny<string>(), excludeId.Value))
                .ReturnsAsync(exists);
        }
        else
        {
            _fixture.RepositoryMock
                .Setup(r => r.ExistsByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(exists);
        }
    }

    private void VerifyUpdateAsyncCalled(string expectedName, int expectedPriority, TaskStatus expectedStatus)
    {
        _fixture.RepositoryMock.Verify(
            r => r.UpdateAsync(It.Is<TodoTask>(t =>
                t.Name == expectedName &&
                t.Priority == expectedPriority &&
                t.Status == expectedStatus)),
            Times.Once);
    }

    private void VerifyUpdateAsyncCalledWithPriority(int expectedPriority)
    {
        _fixture.RepositoryMock.Verify(
            r => r.UpdateAsync(It.Is<TodoTask>(t =>
                t.Priority == expectedPriority)),
            Times.Once);
    }

    #endregion
}
