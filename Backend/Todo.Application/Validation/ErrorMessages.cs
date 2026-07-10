namespace Todo.Application.Validation;

/// <summary>
/// Centralized error messages to follow DRY principle
/// </summary>
public static class ErrorMessages
{
    public const string TaskNotFound = "Task not found.";
    
    public const string TaskNameAlreadyExists = "Task name already exists.";
    
    public const string TaskNameRequired = "Task name is required.";
    
    public const string OnlyCompletedTasksCanBeDeleted = "Only completed tasks can be deleted.";
}
