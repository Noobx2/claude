using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TaskFlow.Web.Services;

// El cliente tiene SUS PROPIOS modelos: es el "contrato" de la API visto desde fuera.
// No referencia el proyecto Core del servidor (así cliente y servidor pueden evolucionar y desplegarse por separado).
public enum Prioridad { Low = 1, Medium = 2, High = 3 }

public sealed record Tarea(int Id, string Title, string? Description, string Priority, DateOnly? DueDate, bool Done, DateTime CreatedAt, bool IsOverdue);
public sealed record Estadisticas(int Total, int Done, int Pending, int Overdue);
public sealed record Token(string AccessToken, string TokenType, int ExpiresInMinutes);

/// <summary>Modelo del formulario. Los atributos de validación se aplican en el navegador antes de molestar al servidor.</summary>
public sealed class NuevaTareaModelo
{
    [Required(ErrorMessage = "El título es obligatorio")]
    [StringLength(80, MinimumLength = 3, ErrorMessage = "El título debe tener entre 3 y 80 caracteres")]
    public string Title { get; set; } = "";

    [StringLength(500, ErrorMessage = "Máximo 500 caracteres")]
    public string? Description { get; set; }

    public Prioridad Priority { get; set; } = Prioridad.Medium;
    public DateOnly? DueDate { get; set; }
}

public sealed class CredencialesModelo
{
    [Required(ErrorMessage = "Escribe tu usuario")] public string UserName { get; set; } = "";
    [Required(ErrorMessage = "Escribe tu contraseña"), MinLength(8, ErrorMessage = "Mínimo 8 caracteres")] public string Password { get; set; } = "";
}

/// <summary>Error de la API ya "traducido" a algo que se puede mostrar al usuario.</summary>
public sealed class ApiException(string mensaje, int estado) : Exception(mensaje) { public int Estado { get; } = estado; }

public static class Json
{
    public static readonly JsonSerializerOptions Opciones = new(JsonSerializerDefaults.Web) { Converters = { new JsonStringEnumConverter() } };
}
