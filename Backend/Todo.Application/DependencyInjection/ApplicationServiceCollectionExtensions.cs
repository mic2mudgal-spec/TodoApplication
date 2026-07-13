using Microsoft.Extensions.DependencyInjection;
using Todo.Application.Interfaces;
using Todo.Application.Services;
//using Todo.Application.Validation;
//using Todo.Application.Validation.Validators;

namespace Todo.Application.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Services
        services.AddScoped<ITodoService, TodoService>();

        // Validators
        //services.AddScoped<ITaskValidator, RequiredNameValidator>();
        //services.AddScoped<ITaskValidator, PriorityValidator>();
        //services.AddScoped<ITaskValidator, DuplicateNameValidator>();

        return services;
    }
}