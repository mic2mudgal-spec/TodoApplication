using NUnit.Framework;
using Moq;
using Todo.Application.Exceptions;
using Todo.Tests.Fixtures;
using Todo.Domain.Entities;
using TaskStatus = Todo.Domain.Enums.TaskStatus;
using Todo.Application.DTOs.Requests;

namespace Todo.Tests.Services.Validation
{
    /// <summary>
    /// Test cases for UI Validation scenarios
    /// These tests cover positive and negative scenarios for form validation
    /// matching the Angular UI validation rules
    /// </summary>
    [TestFixture]
    public class UIValidationScenarioTests
    {
        private TodoServiceFixture _fixture = null!;

        [SetUp]
        public void SetUp() => _fixture = new TodoServiceFixture();

        #region Task Name Validation Tests

        [Test]
        [Category("Validation")]
        [Description("Positive: Valid task name with standard characters")]
        public async Task CreateTask_Positive_ValidTaskName_WithStandardCharacters()
        {
            // Arrange
            var request = BuildCreateRequest(name: "Complete Project Documentation");
            MockRepositoryNameExists(false);

            // Act
            var result = await _fixture.Service.CreateAsync(request);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Complete Project Documentation"));
            _fixture.RepositoryMock.Verify(r => r.AddAsync(It.IsAny<TodoTask>()), Times.Once);
        }

        [Test]
        [Category("Validation")]
        [Description("Positive: Task name with special characters")]
        public async Task CreateTask_Positive_TaskName_WithSpecialCharacters()
        {
            // Arrange
            var request = BuildCreateRequest(name: "Review PR #123 - Fix: Bug fix");
            MockRepositoryNameExists(false);

            // Act
            var result = await _fixture.Service.CreateAsync(request);

            // Assert
            Assert.That(result.Name, Is.EqualTo("Review PR #123 - Fix: Bug fix"));
        }

        [Test]
        [Category("Validation")]
        [Description("Positive: Task name with maximum allowed length (100 characters)")]
        public async Task CreateTask_Positive_TaskName_WithMaximumLength()
        {
            // Arrange
            var maxLengthName = new string('A', 100);
            var request = BuildCreateRequest(name: maxLengthName);
            MockRepositoryNameExists(false);

            // Act
            var result = await _fixture.Service.CreateAsync(request);

            // Assert
            Assert.That(result.Name.Length, Is.EqualTo(100));
            Assert.That(result.Name, Is.EqualTo(maxLengthName));
        }

        [Test]
        [Category("Validation")]
        [Description("Positive: Task name with leading and trailing whitespace gets trimmed")]
        public async Task CreateTask_Positive_TaskName_WithWhitespace_GetsTrimmed()
        {
            // Arrange
            var request = BuildCreateRequest(name: "   Task with spaces   ");
            MockRepositoryNameExists(false);

            // Act
            var result = await _fixture.Service.CreateAsync(request);

            // Assert
            Assert.That(result.Name, Is.EqualTo("Task with spaces"));
            Assert.That(result.Name.Length, Is.EqualTo(16));
        }

        [Test]
        [Category("Validation")]
        [Description("Negative: Empty task name should be rejected")]
        public void CreateTask_Negative_EmptyTaskName_ThrowsValidationException()
        {
            // Arrange
            var request = BuildCreateRequest(name: "");
            MockRepositoryNameExists(false);

            // Act & Assert
            Assert.ThrowsAsync<ValidationException>(async () =>
                await _fixture.Service.CreateAsync(request));
        }

        [Test]
        [Category("Validation")]
        [Description("Negative: Only whitespace task name should be rejected")]
        public void CreateTask_Negative_WhitespaceOnlyTaskName_ThrowsValidationException()
        {
            // Arrange
            var request = BuildCreateRequest(name: "   ");
            MockRepositoryNameExists(false);

            // Act & Assert
            // Service should trim and treat as empty
            Assert.ThrowsAsync<ValidationException>(async () =>
                await _fixture.Service.CreateAsync(request));
        }

        [Test]
        [Category("Validation")]
        [Description("Negative: Task name exceeding 100 characters should be rejected")]
        public void CreateTask_Negative_TaskName_ExceedsMaxLength_ThrowsValidationException()
        {
            // Arrange
            var toolongName = new string('A', 101);
            var request = BuildCreateRequest(name: toolongName);
            MockRepositoryNameExists(false);

            // Act & Assert
            Assert.ThrowsAsync<ValidationException>(async () =>
                await _fixture.Service.CreateAsync(request));
        }

        [Test]
        [Category("Validation")]
        [Description("Negative: Duplicate task name should be rejected")]
        public void CreateTask_Negative_DuplicateTaskName_ThrowsValidationException()
        {
            // Arrange
            var request = BuildCreateRequest(name: "Existing Task");
            MockRepositoryNameExists(true);

            // Act & Assert
            Assert.ThrowsAsync<ValidationException>(async () =>
                await _fixture.Service.CreateAsync(request));
        }

        [Test]
        [Category("Validation")]
        [Description("Negative: Duplicate task name on update (same task excluded) should succeed")]
        public async Task UpdateTask_Positive_DuplicateCheck_ExcludesCurrentTask()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var existing = BuildTodoTask(taskId, "Old Name", 5, TaskStatus.NotStarted);
            var request = BuildUpdateRequest(taskId, "Old Name", 5, TaskStatus.NotStarted);
            MockGetById(taskId, existing);
            MockRepositoryNameExists(false, taskId);

            // Act
            await _fixture.Service.UpdateAsync(request);

            // Assert
            VerifyUpdateAsyncCalled("Old Name", 5, TaskStatus.NotStarted);
        }

        #endregion

        #region Priority Validation Tests

        [Test]
        [Category("Validation")]
        [Description("Positive: Valid priority values (1, 5, 10)")]
        [TestCase(1)]
        [TestCase(5)]
        [TestCase(10)]
        public async Task CreateTask_Positive_ValidPriority_AcceptsAnyPositiveInteger(int priority)
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
        [Category("Validation")]
        [Description("Positive: Null priority defaults to 5")]
        public async Task CreateTask_Positive_NullPriority_DefaultsToFive()
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
        [Category("Validation")]
        [Description("Positive: Large priority value (999) is accepted")]
        public async Task CreateTask_Positive_LargePriorityValue_Accepted()
        {
            // Arrange
            var request = BuildCreateRequest(priority: 999);
            MockRepositoryNameExists(false);

            // Act
            var result = await _fixture.Service.CreateAsync(request);

            // Assert
            Assert.That(result.Priority, Is.EqualTo(999));
        }

        [Test]
        [Category("Validation")]
        [Description("Negative: Zero priority should be rejected")]
        public void CreateTask_Negative_ZeroPriority_ThrowsValidationException()
        {
            // Arrange
            var request = BuildCreateRequest(priority: 0);
            MockRepositoryNameExists(false);

            // Act & Assert
            Assert.ThrowsAsync<ValidationException>(async () =>
                await _fixture.Service.CreateAsync(request));
        }

        [Test]
        [Category("Validation")]
        [Description("Negative: Negative priority should be rejected")]
        public void CreateTask_Negative_NegativePriority_ThrowsValidationException()
        {
            // Arrange
            var request = BuildCreateRequest(priority: -5);
            MockRepositoryNameExists(false);

            // Act & Assert
            Assert.ThrowsAsync<ValidationException>(async () =>
                await _fixture.Service.CreateAsync(request));
        }

        #endregion

        #region Status Validation Tests

        [Test]
        [Category("Validation")]
        [Description("Positive: Valid task statuses")]
        [TestCase(TaskStatus.NotStarted)]
        [TestCase(TaskStatus.InProgress)]
        [TestCase(TaskStatus.Completed)]
        public async Task CreateTask_Positive_ValidStatus_AllStatusesAccepted(TaskStatus status)
        {
            // Arrange
            var request = BuildCreateRequest(status: status);
            MockRepositoryNameExists(false);

            // Act
            var result = await _fixture.Service.CreateAsync(request);

            // Assert
            Assert.That(result.Status, Is.EqualTo(status));
        }

        [Test]
        [Category("Validation")]
        [Description("Positive: Status can be changed from NotStarted to InProgress")]
        public async Task UpdateTask_Positive_StatusChange_FromNotStartedToInProgress()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var existing = BuildTodoTask(taskId, "Task", 5, TaskStatus.NotStarted);
            var request = BuildUpdateRequest(taskId, "Task", 5, TaskStatus.InProgress);
            MockGetById(taskId, existing);
            MockRepositoryNameExists(false, taskId);

            // Act
            await _fixture.Service.UpdateAsync(request);

            // Assert
            VerifyUpdateAsyncCalled("Task", 5, TaskStatus.InProgress);
        }

        [Test]
        [Category("Validation")]
        [Description("Positive: Status can be changed from InProgress to Completed")]
        public async Task UpdateTask_Positive_StatusChange_FromInProgressToCompleted()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var existing = BuildTodoTask(taskId, "Task", 5, TaskStatus.InProgress);
            var request = BuildUpdateRequest(taskId, "Task", 5, TaskStatus.Completed);
            MockGetById(taskId, existing);
            MockRepositoryNameExists(false, taskId);

            // Act
            await _fixture.Service.UpdateAsync(request);

            // Assert
            VerifyUpdateAsyncCalled("Task", 5, TaskStatus.Completed);
        }

        #endregion

        #region Combined Validation Tests

        [Test]
        [Category("Validation")]
        [Description("Positive: All fields valid with standard values")]
        public async Task CreateTask_Positive_AllFieldsValid_StandardScenario()
        {
            // Arrange
            var request = BuildCreateRequest(
                name: "Implement Authentication",
                priority: 7,
                status: TaskStatus.NotStarted
            );
            MockRepositoryNameExists(false);

            // Act
            var result = await _fixture.Service.CreateAsync(request);

            // Assert
            Assert.That(result.Name, Is.EqualTo("Implement Authentication"));
            Assert.That(result.Priority, Is.EqualTo(7));
            Assert.That(result.Status, Is.EqualTo(TaskStatus.NotStarted));
        }

        [Test]
        [Category("Validation")]
        [Description("Negative: Invalid name with valid priority and status")]
        public void CreateTask_Negative_InvalidName_ValidOtherFields()
        {
            // Arrange
            var request = BuildCreateRequest(
                name: "",
                priority: 5,
                status: TaskStatus.NotStarted
            );
            MockRepositoryNameExists(false);

            // Act & Assert
            Assert.ThrowsAsync<ValidationException>(async () =>
                await _fixture.Service.CreateAsync(request));
        }

        [Test]
        [Category("Validation")]
        [Description("Negative: Valid name with invalid priority")]
        public void CreateTask_Negative_ValidName_InvalidPriority()
        {
            // Arrange
            var request = BuildCreateRequest(
                name: "Valid Task",
                priority: -1,
                status: TaskStatus.NotStarted
            );
            MockRepositoryNameExists(false);

            // Act & Assert
            Assert.ThrowsAsync<ValidationException>(async () =>
                await _fixture.Service.CreateAsync(request));
        }

        #endregion

        #region Update Partial Fields Tests

        [Test]
        [Category("Validation")]
        [Description("Positive: Update only name field, priority and status unchanged")]
        public async Task UpdateTask_Positive_PartialUpdate_OnlyNameChanged()
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
        [Category("Validation")]
        [Description("Positive: Update priority field, name and status unchanged")]
        public async Task UpdateTask_Positive_PartialUpdate_OnlyPriorityChanged()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var existing = BuildTodoTask(taskId, "Task", 5, TaskStatus.NotStarted);
            var request = new UpdateTodoTaskRequest
            {
                Id = taskId,
                Name = null,
                Priority = 8,
                Status = TaskStatus.NotStarted
            };
            MockGetById(taskId, existing);
            MockRepositoryNameExists(false);

            // Act
            await _fixture.Service.UpdateAsync(request);

            // Assert
            VerifyUpdateAsyncCalledWithPriority(8);
        }

        [Test]
        [Category("Validation")]
        [Description("Positive: Update multiple fields - name and status")]
        public async Task UpdateTask_Positive_PartialUpdate_NameAndStatusChanged()
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

        #region Error Handling Tests

        [Test]
        [Category("Validation")]
        [Description("Negative: Update non-existent task returns 404")]
        public void UpdateTask_Negative_TaskNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var request = BuildUpdateRequest(taskId);
            MockGetById(taskId, null);

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _fixture.Service.UpdateAsync(request));
        }

        [Test]
        [Category("Validation")]
        [Description("Negative: Delete non-existent task returns 404")]
        public void DeleteTask_Negative_TaskNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            MockGetById(taskId, null);

            // Act & Assert
            Assert.ThrowsAsync<NotFoundException>(async () =>
                await _fixture.Service.DeleteAsync(taskId));
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
            string? name = "Updated Task",
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
}
