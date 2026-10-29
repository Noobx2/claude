using TaskFlow.Application;
using TaskFlow.Domain;

namespace TaskFlow.Infrastructure;

/// <summary>Implementación en memoria: ideal para pruebas.</summary>
public class InMemoryTaskRepository : ITaskRepository
{
    protected readonly List<TaskItem> Items = [];
    private int _lastId;

    public IReadOnlyList<TaskItem> GetAll() => Items.ToList();          // copia: nadie modifica la lista interna

    public TaskItem? Find(int id) => Items.FirstOrDefault(t => t.Id == id);

    public virtual TaskItem Add(TaskItem task)
    {
        var saved = task with { Id = ++_lastId };
        Items.Add(saved);
        return saved;
    }

    public virtual void Update(TaskItem task)
    {
        int index = Items.FindIndex(t => t.Id == task.Id);
        if (index < 0) throw new NotFoundException($"No existe la tarea {task.Id}.");
        Items[index] = task;
    }

    public virtual bool Remove(int id) => Items.RemoveAll(t => t.Id == id) > 0;

    protected void SetLastId(int id) => _lastId = id;
}
