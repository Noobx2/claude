using TaskFlow.Domain;

namespace TaskFlow.Application;

public sealed record NewTask(string Title, string? Description, Priority Priority, DateOnly? DueDate);
public sealed record TaskChanges(string? Title = null, string? Description = null, Priority? Priority = null, DateOnly? DueDate = null);
public sealed record TaskFilter(bool? Done = null, Priority? Priority = null, string? Text = null, bool OnlyOverdue = false);
public sealed record TaskStats(int Total, int Done, int Pending, int Overdue);

/// <summary>
/// Las reglas del negocio y las operaciones CRUD. Recibe sus dependencias por el constructor
/// (inyección de dependencias) y no conoce ni la consola ni cómo se guardan los datos.
/// </summary>
public sealed class TaskService(ITaskRepository repository, IClock clock)
{
    public TaskItem Add(NewTask input)
    {
        string title = ValidateTitle(input.Title);
        if (input.DueDate is { } due && due < clock.Today)
            throw new ValidationException("La fecha límite no puede estar en el pasado.");

        return repository.Add(new TaskItem(0, title, NullIfEmpty(input.Description), input.Priority, input.DueDate, false, clock.Now));
    }

    public TaskItem Get(int id) =>
        repository.Find(id) ?? throw new NotFoundException($"No existe la tarea {id}.");

    public IReadOnlyList<TaskItem> List(TaskFilter? filter = null)
    {
        filter ??= new TaskFilter();
        IEnumerable<TaskItem> query = repository.GetAll();
        if (filter.Done is { } done) query = query.Where(t => t.Done == done);
        if (filter.Priority is { } priority) query = query.Where(t => t.Priority == priority);
        if (!string.IsNullOrWhiteSpace(filter.Text))
            query = query.Where(t => t.Title.Contains(filter.Text, StringComparison.CurrentCultureIgnoreCase)
                                  || (t.Description?.Contains(filter.Text, StringComparison.CurrentCultureIgnoreCase) ?? false));
        if (filter.OnlyOverdue) query = query.Where(t => t.IsOverdue(clock.Today));

        // Primero las pendientes, luego por prioridad (alta antes) y por fecha límite
        return query.OrderBy(t => t.Done)
                    .ThenByDescending(t => t.Priority)
                    .ThenBy(t => t.DueDate ?? DateOnly.MaxValue)
                    .ThenBy(t => t.Id)
                    .ToList();
    }

    public TaskItem Update(int id, TaskChanges changes)
    {
        TaskItem current = Get(id);
        TaskItem updated = current with
        {
            Title = changes.Title is null ? current.Title : ValidateTitle(changes.Title),
            Description = changes.Description is null ? current.Description : NullIfEmpty(changes.Description),
            Priority = changes.Priority ?? current.Priority,
            DueDate = changes.DueDate ?? current.DueDate,
        };
        repository.Update(updated);
        return updated;
    }

    public TaskItem SetDone(int id, bool done)
    {
        TaskItem updated = Get(id) with { Done = done };
        repository.Update(updated);
        return updated;
    }

    public void Delete(int id)
    {
        if (!repository.Remove(id)) throw new NotFoundException($"No existe la tarea {id}.");
    }

    public TaskStats GetStats()
    {
        var all = repository.GetAll();
        int done = all.Count(t => t.Done);
        return new TaskStats(all.Count, done, all.Count - done, all.Count(t => t.IsOverdue(clock.Today)));
    }

    private static string ValidateTitle(string? title)
    {
        string trimmed = title?.Trim() ?? "";
        if (trimmed.Length < 3) throw new ValidationException("El título necesita al menos 3 caracteres.");
        if (trimmed.Length > 80) throw new ValidationException("El título no puede superar 80 caracteres.");
        return trimmed;
    }

    private static string? NullIfEmpty(string? text) => string.IsNullOrWhiteSpace(text) ? null : text.Trim();
}
