#:sdk Microsoft.NET.Sdk.Web
#:property PublishAot=false

// ARQUITECTURA POR CAPAS (lo que hace TaskFlow):    Controllers/ Services/ Repositories/  → una carpeta por TIPO de pieza
// ARQUITECTURA POR REBANADAS VERTICALES (aquí):     Features/CrearTarea, Features/ListarTareas → una carpeta por FUNCIONALIDAD
// Cada rebanada contiene todo lo que necesita (petición, validación, lógica y ruta); cambiar una funcionalidad toca UN sitio.

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5104");
builder.Services.AddSingleton<Almacen>();
var app = builder.Build();

CrearTarea.Mapear(app);          // cada rebanada registra sus propias rutas
ListarTareas.Mapear(app);
CompletarTarea.Mapear(app);
app.Run();

// ---------------------------------------------------------- Rebanada 1
static class CrearTarea
{
    public record Peticion(string Titulo);
    public record Respuesta(int Id, string Titulo);

    public static void Mapear(WebApplication app) => app.MapPost("/tareas", (Peticion p, Almacen almacen) =>
    {
        if (string.IsNullOrWhiteSpace(p.Titulo) || p.Titulo.Length < 3)          // la validación vive AQUÍ, junto a la funcionalidad
            return Results.ValidationProblem(new Dictionary<string, string[]> { ["titulo"] = ["Mínimo 3 caracteres"] });
        var tarea = almacen.Añadir(p.Titulo.Trim());
        return Results.Created($"/tareas/{tarea.Id}", new Respuesta(tarea.Id, tarea.Titulo));
    });
}

// ---------------------------------------------------------- Rebanada 2
static class ListarTareas
{
    public record Elemento(int Id, string Titulo, bool Hecha);
    public static void Mapear(WebApplication app) => app.MapGet("/tareas", (Almacen almacen, bool? hechas) =>
        almacen.Todas().Where(t => hechas is null || t.Hecha == hechas).Select(t => new Elemento(t.Id, t.Titulo, t.Hecha)));
}

// ---------------------------------------------------------- Rebanada 3
static class CompletarTarea
{
    public static void Mapear(WebApplication app) => app.MapPost("/tareas/{id:int}/completar", (int id, Almacen almacen) =>
        almacen.Completar(id) ? Results.NoContent() : Results.NotFound());
}

// ---------------------------------------------------------- Lo compartido (mínimo)
public record Tarea(int Id, string Titulo, bool Hecha);
public class Almacen
{
    private readonly List<Tarea> _tareas = []; private int _siguiente = 1;
    public Tarea Añadir(string titulo) { var t = new Tarea(_siguiente++, titulo, false); lock (_tareas) _tareas.Add(t); return t; }
    public IReadOnlyList<Tarea> Todas() { lock (_tareas) return _tareas.ToList(); }
    public bool Completar(int id) { lock (_tareas) { int i = _tareas.FindIndex(t => t.Id == id); if (i < 0) return false; _tareas[i] = _tareas[i] with { Hecha = true }; return true; } }
}
