#:sdk Microsoft.NET.Sdk.Web
#:property PublishAot=false

using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);
// Sin TLS el servidor NO puede negociar el protocolo, así que usamos dos puertos (solo para esta demo local):
//   5101 → HTTP/1.1      5103 → HTTP/2 sin cifrar ("h2c"). Con HTTPS, un solo puerto atiende ambos (negociación ALPN).
builder.WebHost.ConfigureKestrel(k =>
{
    k.ListenLocalhost(5101, o => o.Protocols = HttpProtocols.Http1);
    k.ListenLocalhost(5103, o => o.Protocols = HttpProtocols.Http2);
});
var app = builder.Build();

// Middleware 1: mide cuánto tarda cada petición. Envuelve a todo lo que viene después ("cebolla").
app.Use(async (ctx, next) =>
{
    var inicio = System.Diagnostics.Stopwatch.GetTimestamp();
    ctx.Response.OnStarting(() =>
    {
        ctx.Response.Headers["X-Protocolo"] = ctx.Request.Protocol;                       // HTTP/1.1 o HTTP/2
        ctx.Response.Headers["X-Duracion-ms"] = System.Diagnostics.Stopwatch.GetElapsedTime(inicio).TotalMilliseconds.ToString("F1");
        return Task.CompletedTask;
    });
    await next();                                                                          // pasa al siguiente middleware
});

// Middleware 2: un "cortafuegos" mínimo que corta la cadena si falta una cabecera.
app.Use(async (ctx, next) =>
{
    if (ctx.Request.Path.StartsWithSegments("/privado") && !ctx.Request.Headers.ContainsKey("X-Clave"))
    {
        ctx.Response.StatusCode = 401;
        await ctx.Response.WriteAsync("Falta X-Clave");
        return;                                                                            // NO llama a next(): la petición termina aquí
    }
    await next();
});

// Cookie: el servidor la pide con Set-Cookie, el navegador la devuelve en cada petición con Cookie.
app.MapGet("/visita", (HttpContext ctx) =>
{
    int visitas = int.TryParse(ctx.Request.Cookies["visitas"], out int n) ? n + 1 : 1;
    ctx.Response.Cookies.Append("visitas", visitas.ToString(), new CookieOptions
    {
        HttpOnly = true,                    // JavaScript no puede leerla (frena el robo por XSS)
        SameSite = SameSiteMode.Strict,     // no se envía desde otros sitios (frena CSRF)
        MaxAge = TimeSpan.FromHours(1),
    });
    return $"Visita número {visitas}";
});

app.MapGet("/privado", () => "Contenido privado");
app.Run();
