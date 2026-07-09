using Moq;
using Todo.Application.Services;
using Todo.Domain.Interfaces;

namespace Todo.Tests.Fixtures;

public class TodoServiceFixture
{
    public Mock<ITodoRepository> RepositoryMock { get; }

    public TodoService Service { get; }

    public TodoServiceFixture()
    {
        RepositoryMock = new Mock<ITodoRepository>();

        Service = new TodoService(RepositoryMock.Object);
    }
}