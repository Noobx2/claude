using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace TaskFlow.Web.Services;

/// <summary>Toda la comunicación con la API en un solo sitio: los componentes no conocen URLs ni cabeceras.</summary>
public sealed class ApiClient(HttpClient http, Sesion sesion)
{
    public async Task RegistrarAsync(CredencialesModelo c) =>
        await Enviar(HttpMethod.Post, "auth/register", new { userName = c.UserName, password = c.Password });

    public async Task<Token> EntrarAsync(CredencialesModelo c) =>
        (await Enviar<Token>(HttpMethod.Post, "auth/login", new { userName = c.UserName, password = c.Password }))!;

    public async Task<List<Tarea>> ListarAsync(bool? hechas = null, Prioridad? prioridad = null, string? texto = null)
    {
        var partes = new List<string>();
        if (hechas is not null) partes.Add($"done={hechas.ToString()!.ToLower()}");
        if (prioridad is not null) partes.Add($"priority={prioridad}");
        if (!string.IsNullOrWhiteSpace(texto)) partes.Add($"q={Uri.EscapeDataString(texto)}");
        string consulta = partes.Count > 0 ? "?" + string.Join("&", partes) : "";
        return await Enviar<List<Tarea>>(HttpMethod.Get, "tasks" + consulta) ?? [];
    }

    public Task<Estadisticas?> EstadisticasAsync() => Enviar<Estadisticas>(HttpMethod.Get, "tasks/stats");
    public Task<Tarea?> CrearAsync(NuevaTareaModelo m) => Enviar<Tarea>(HttpMethod.Post, "tasks",
        new { title = m.Title, description = m.Description, priority = m.Priority, dueDate = m.DueDate });
    public Task<Tarea?> CompletarAsync(int id) => Enviar<Tarea>(HttpMethod.Post, $"tasks/{id}/done");
    public Task EliminarAsync(int id) => Enviar(HttpMethod.Delete, $"tasks/{id}");

    private async Task Enviar(HttpMethod metodo, string ruta, object? cuerpo = null) => await Enviar<object>(metodo, ruta, cuerpo);

    private async Task<T?> Enviar<T>(HttpMethod metodo, string ruta, object? cuerpo = null)
    {
        using var peticion = new HttpRequestMessage(metodo, ruta);
        if (sesion.Token is not null) peticion.Headers.Authorization = new AuthenticationHeaderValue("Bearer", sesion.Token);
        if (cuerpo is not null) peticion.Content = JsonContent.Create(cuerpo, options: Json.Opciones);

        HttpResponseMessage respuesta;
        try { respuesta = await http.SendAsync(peticion); }
        catch (HttpRequestException) { throw new ApiException("No se pudo conectar con el servidor. ¿Está en marcha la API?", 0); }

        if (respuesta.IsSuccessStatusCode)
            return respuesta.StatusCode == HttpStatusCode.NoContent || respuesta.Content.Headers.ContentLength == 0
                ? default : await respuesta.Content.ReadFromJsonAsync<T>(Json.Opciones);

        // Traducimos los códigos HTTP a mensajes para personas
        string detalle = await LeerDetalleAsync(respuesta);
        throw new ApiException(respuesta.StatusCode switch
        {
            HttpStatusCode.Unauthorized => "Credenciales incorrectas o sesión caducada.",
            HttpStatusCode.Forbidden => "No tienes permiso para hacer esto (se necesita el rol Admin).",
            HttpStatusCode.NotFound => "Eso ya no existe.",
            _ => detalle,
        }, (int)respuesta.StatusCode);
    }

    private static async Task<string> LeerDetalleAsync(HttpResponseMessage r)
    {
        try
        {
            using var doc = JsonDocument.Parse(await r.Content.ReadAsStringAsync());
            var raiz = doc.RootElement;
            if (raiz.TryGetProperty("errors", out var errores))   // validación: { errors: { campo: [mensajes] } }
                return string.Join(" ", errores.EnumerateObject().SelectMany(e => e.Value.EnumerateArray().Select(m => m.GetString())));
            if (raiz.TryGetProperty("detail", out var d)) return d.GetString() ?? "Error desconocido.";
            if (raiz.TryGetProperty("error", out var er)) return er.GetString() ?? "Error desconocido.";
        }
        catch (JsonException) { }
        return $"Error {(int)r.StatusCode}.";
    }
}
