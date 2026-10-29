using System.Net.Http;
using System.Net.Http.Json;
using Mvvm.Core;

namespace Mvvm.Wpf;

/// <summary>
/// La implementación REAL de ITareasApi: habla con la API de TaskFlow (lecciones 47–50).
/// Traduce entre el JSON de la API (title/done) y el modelo del ViewModel (Titulo/Hecha).
/// </summary>
public sealed class HttpTareasApi(HttpClient http) : ITareasApi
{
    private sealed record TareaDto(int Id, string Title, bool Done);

    public async Task<IReadOnlyList<Tarea>> ListarAsync(CancellationToken ct = default) =>
        (await http.GetFromJsonAsync<List<TareaDto>>("tasks", ct) ?? []).Select(Convertir).ToList();

    public async Task<Tarea> CrearAsync(string titulo, CancellationToken ct = default)
    {
        var r = await http.PostAsJsonAsync("tasks", new { title = titulo }, ct);
        r.EnsureSuccessStatusCode();
        return Convertir((await r.Content.ReadFromJsonAsync<TareaDto>(ct))!);
    }

    public async Task CompletarAsync(int id, CancellationToken ct = default) =>
        (await http.PostAsync($"tasks/{id}/done", null, ct)).EnsureSuccessStatusCode();

    private static Tarea Convertir(TareaDto d) => new(d.Id, d.Title, d.Done);
}
