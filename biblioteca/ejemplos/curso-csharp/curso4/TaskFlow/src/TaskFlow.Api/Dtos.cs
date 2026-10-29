using System.ComponentModel.DataAnnotations;
using TaskFlow.Domain;

namespace TaskFlow.Api;

// DTO = Data Transfer Object: la forma de los datos EN LA FRONTERA HTTP. No se expone el dominio directamente.
public sealed record CreateTaskRequest(
    [property: Required, StringLength(80, MinimumLength = 3)] string Title,
    [property: StringLength(500)] string? Description,
    Priority Priority = Priority.Medium,
    DateOnly? DueDate = null);

public sealed record UpdateTaskRequest(string? Title, string? Description, Priority? Priority, DateOnly? DueDate);

public sealed record TaskResponse(int Id, string Title, string? Description, string Priority, DateOnly? DueDate, bool Done, DateTime CreatedAt, bool IsOverdue)
{
    public static TaskResponse From(TaskItem t, DateOnly today) =>
        new(t.Id, t.Title, t.Description, t.Priority.ToString(), t.DueDate, t.Done, t.CreatedAt, t.IsOverdue(today));
}

public sealed record RegisterRequest([property: Required, StringLength(40, MinimumLength = 3)] string UserName,
                                     [property: Required, StringLength(100, MinimumLength = 8)] string Password);
public sealed record LoginRequest(string UserName, string Password);
public sealed record TokenResponse(string AccessToken, string TokenType, int ExpiresInMinutes);
