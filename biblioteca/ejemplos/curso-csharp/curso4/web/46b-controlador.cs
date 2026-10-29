#:sdk Microsoft.NET.Sdk.Web
#:property PublishAot=false

using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5102");
builder.Services.AddControllers();     // activa el modelo MVC: clases "Controller" descubiertas por convención
var app = builder.Build();
app.MapControllers();
app.Run();

// [ApiController] activa: validación automática del modelo (400) y enlace de parámetros desde el cuerpo.
[ApiController]
[Route("api/[controller]")]           // [controller] se sustituye por "Notas" (el nombre de la clase sin "Controller")
public class NotasController : ControllerBase
{
    private static readonly List<Nota> _notas = [new(1, "Primera nota")];

    [HttpGet]
    public IEnumerable<Nota> Listar() => _notas;

    [HttpGet("{id:int}")]
    public ActionResult<Nota> Obtener(int id) =>
        _notas.FirstOrDefault(n => n.Id == id) is { } nota ? nota : NotFound();

    [HttpPost]
    public ActionResult<Nota> Crear(NuevaNota entrada)    // si entrada no cumple los atributos, MVC responde 400 sin llegar aquí
    {
        var nota = new Nota(_notas.Max(n => n.Id) + 1, entrada.Texto);
        _notas.Add(nota);
        return CreatedAtAction(nameof(Obtener), new { id = nota.Id }, nota);
    }
}

public record Nota(int Id, string Texto);
// En MVC los atributos de validación van en el parámetro del record (en Minimal API hacía falta "property:").
public record NuevaNota([Required, MinLength(3)] string Texto);
