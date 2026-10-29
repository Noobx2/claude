using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

// CreateApplicationBuilder apila las fuentes de configuración en este orden (la ÚLTIMA gana):
//   1) appsettings.json  2) appsettings.{Entorno}.json  3) user-secrets (solo en Development)
//   4) variables de entorno  5) argumentos de la línea de comandos
var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddOptions<TiendaOptions>()
    .Bind(builder.Configuration.GetSection("Tienda"))      // enlaza la sección "Tienda" con la clase
    .ValidateDataAnnotations()                              // aplica [Required], [Range]…
    .ValidateOnStart();                                     // y falla AL ARRANCAR, no cuando alguien la use por primera vez

using var host = builder.Build();                           // Build() solo construye; todavía no valida
await host.StartAsync();                                    // AL ARRANCAR el host se ejecutan las validaciones: si algo está mal, falla aquí y no más tarde

var tienda = host.Services.GetRequiredService<IOptions<TiendaOptions>>().Value;
var config = host.Services.GetRequiredService<IConfiguration>();

Console.WriteLine($"Entorno            : {builder.Environment.EnvironmentName}");
Console.WriteLine($"Nombre             : {tienda.Nombre}");
Console.WriteLine($"MaxPedidosPorDia   : {tienda.MaxPedidosPorDia}");
Console.WriteLine($"Moneda             : {tienda.Moneda}");
Console.WriteLine($"Cadena de conexión : {Ocultar(config.GetConnectionString("Principal"))}");
Console.WriteLine($"Clave de API       : {Ocultar(config["ApiKey"])}");
await host.StopAsync();

static string Ocultar(string? secreto) => string.IsNullOrEmpty(secreto) ? "(no definida)"
    : secreto.Length <= 6 ? "***" : secreto[..6] + "… (" + secreto.Length + " caracteres, oculto)";

public class TiendaOptions
{
    [Required, MinLength(3)] public string Nombre { get; set; } = "";
    [Range(1, 100_000)] public int MaxPedidosPorDia { get; set; }
    [RegularExpression("^[A-Z]{3}$", ErrorMessage = "La moneda debe ser un código de 3 letras mayúsculas (ISO 4217)")]
    public string Moneda { get; set; } = "EUR";
}
