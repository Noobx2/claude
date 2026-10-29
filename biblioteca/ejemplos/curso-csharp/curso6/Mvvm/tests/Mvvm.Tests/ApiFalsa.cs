using Mvvm.Core;

namespace Mvvm.Tests;

/// <summary>API de mentira, controlable desde la prueba: se le puede pedir que se quede esperando o que falle.</summary>
public sealed class ApiFalsa : ITareasApi
{
    private int _siguiente = 1;
    public List<Tarea> Datos { get; } = [];
    public Exception? FallarCon { get; set; }
    public TaskCompletionSource? Puerta { get; set; }          // si no es null, las llamadas esperan hasta que la prueba lo permita
    public int Llamadas { get; private set; }

    public async Task<IReadOnlyList<Tarea>> ListarAsync(CancellationToken ct = default)
    { Llamadas++; await Esperar(); return Datos.ToList(); }

    public async Task<Tarea> CrearAsync(string titulo, CancellationToken ct = default)
    { Llamadas++; await Esperar(); var t = new Tarea(_siguiente++, titulo, false); Datos.Add(t); return t; }

    public async Task CompletarAsync(int id, CancellationToken ct = default)
    { Llamadas++; await Esperar(); }

    private async Task Esperar()
    {
        if (Puerta is not null) await Puerta.Task;
        if (FallarCon is not null) throw FallarCon;
    }
}
