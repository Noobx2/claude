namespace Mvvm.Core;

public sealed record Tarea(int Id, string Titulo, bool Hecha);

/// <summary>Lo que el ViewModel necesita del mundo exterior. En producción llamaría a la API de TaskFlow; en las pruebas es un doble.</summary>
public interface ITareasApi
{
    Task<IReadOnlyList<Tarea>> ListarAsync(CancellationToken ct = default);
    Task<Tarea> CrearAsync(string titulo, CancellationToken ct = default);
    Task CompletarAsync(int id, CancellationToken ct = default);
}
