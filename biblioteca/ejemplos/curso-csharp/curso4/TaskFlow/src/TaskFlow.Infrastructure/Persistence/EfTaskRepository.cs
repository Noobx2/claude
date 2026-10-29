using Microsoft.EntityFrameworkCore;
using TaskFlow.Application;
using TaskFlow.Domain;

namespace TaskFlow.Infrastructure.Persistence;

/// <summary>El mismo puerto ITaskRepository, ahora respaldado por PostgreSQL a través de EF Core.</summary>
public sealed class EfTaskRepository(TaskFlowDbContext db) : ITaskRepository
{
    // AsNoTracking: solo leemos, así EF no gasta memoria vigilando cambios.
    public IReadOnlyList<TaskItem> GetAll() => db.Tasks.AsNoTracking().OrderBy(t => t.Id).ToList();

    public TaskItem? Find(int id) => db.Tasks.AsNoTracking().FirstOrDefault(t => t.Id == id);

    public TaskItem Add(TaskItem task)
    {
        db.Tasks.Add(task);          // Id = 0 → la base genera el Id (columna identity)
        db.SaveChanges();
        return task;
    }

    public void Update(TaskItem task)
    {
        db.Tasks.Update(task);       // marca todas las columnas como modificadas
        db.SaveChanges();
        db.ChangeTracker.Clear();    // soltamos la entidad para poder volver a cargar otra copia
    }

    public bool Remove(int id) => db.Tasks.Where(t => t.Id == id).ExecuteDelete() > 0;
}
