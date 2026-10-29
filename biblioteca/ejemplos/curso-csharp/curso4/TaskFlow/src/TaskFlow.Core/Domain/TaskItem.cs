namespace TaskFlow.Domain;

public enum Priority { Low = 1, Medium = 2, High = 3 }

/// <summary>
/// Una tarea. Es un record: inmutable (para cambiarla se crea una copia con "with").
/// El dominio no sabe nada de consola, archivos ni bases de datos.
/// </summary>
public sealed record TaskItem(
    int Id,
    string Title,
    string? Description,
    Priority Priority,
    DateOnly? DueDate,
    bool Done,
    DateTime CreatedAt)
{
    /// <summary>Una tarea está vencida si tiene fecha, no está hecha y esa fecha ya pasó.</summary>
    public bool IsOverdue(DateOnly today) => !Done && DueDate is { } due && due < today;
}
