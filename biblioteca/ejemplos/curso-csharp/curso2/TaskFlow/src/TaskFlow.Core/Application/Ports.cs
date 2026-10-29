using TaskFlow.Domain;

namespace TaskFlow.Application;

/// <summary>
/// "Puerto": el contrato que el servicio NECESITA. La implementación (memoria, JSON, base de datos)
/// vive fuera, así que se puede cambiar sin tocar la lógica.
/// </summary>
public interface ITaskRepository
{
    IReadOnlyList<TaskItem> GetAll();
    TaskItem? Find(int id);
    TaskItem Add(TaskItem task);     // asigna el Id
    void Update(TaskItem task);
    bool Remove(int id);
}

/// <summary>La hora actual, como dependencia: así las pruebas controlan el tiempo.</summary>
public interface IClock
{
    DateTime Now { get; }
    DateOnly Today => DateOnly.FromDateTime(Now);
}
