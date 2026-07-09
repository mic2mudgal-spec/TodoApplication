using Microsoft.Extensions.DependencyInjection;
using Todo.Domain.Interfaces;
using Todo.Infrastructure.Repositories;

namespace Todo.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services)
    {
        services.AddSingleton<ITodoRepository, InMemoryTodoRepository>();

        return services;
    }
}