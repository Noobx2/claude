# Curso de C# · Parte VII: desarrollo web con ASP.NET Core

Aquí **TaskFlow** (la aplicación de la Parte II, con su capa `Core` que no sabe nada de consolas ni de bases de datos) se convierte en **una API web real**: HTTP, validación, base de datos PostgreSQL con Entity Framework Core, migraciones, seguridad con JWT, configuración por entornos, pruebas de integración y despliegue. El `Core` **no se tocó**: la arquitectura por capas de la Parte III se paga aquí.

*Verificado: 2026-10-29 con .NET SDK 10.0.113 y PostgreSQL 16 en Linux. La API se ejecutó, se llamó con `curl` y se probó con 40 pruebas automatizadas. **No se ejecutó** `docker` (no hay demonio Docker en este entorno): el `Dockerfile` y el `docker-compose.yml` se escribieron y se validó su sintaxis, pero la imagen no se construyó.*

## Mapa de la Parte VII

| Lección | Tema | Lo verás ejecutado |
|---|---|---|
| 46 | La web, HTTP, middleware, MVC y arquitecturas (capas, hexagonal, rebanadas verticales) | Cookies, HTTP/2, middleware, controladores, rebanadas |
| 47 | La API de TaskFlow: Minimal API, DTO, validación, errores estándar | Todas las rutas con `curl` |
| 48 | Entity Framework Core y PostgreSQL: modelo, LINQ→SQL, migraciones | SQL generado, N+1, concurrencia, inyección SQL, migración y *rollback* |
| 49 | Seguridad: contraseñas, JWT, roles, secretos | Hash, token abierto y manipulado, 401/403 |
| 50 | Configuración, entornos, pruebas de integración y despliegue | Capas de configuración, `WebApplicationFactory`, publicación, *bundle* |

**Estructura final de TaskFlow** (la capa `Core` es la misma de la Parte II; `Api` e `Infrastructure/Persistence` son lo nuevo):

```text
$ tree (sin bin/ ni obj/)
.dockerignore
.github/workflows/ci.yml
.gitignore
Dockerfile
TaskFlow.slnx
docker-compose.yml
src/TaskFlow.Api/Auth.cs
src/TaskFlow.Api/Dtos.cs
src/TaskFlow.Api/Program.cs
src/TaskFlow.Api/TaskFlow.Api.csproj
src/TaskFlow.Api/appsettings.Development.json
src/TaskFlow.Api/appsettings.json
src/TaskFlow.Console/ConsoleApp.cs
src/TaskFlow.Console/IConsoleIO.cs
src/TaskFlow.Console/Program.cs
src/TaskFlow.Console/Prompts.cs
src/TaskFlow.Console/TaskFlow.Console.csproj
src/TaskFlow.Core/Application/Ports.cs
src/TaskFlow.Core/Application/TaskService.cs
src/TaskFlow.Core/Domain/Errors.cs
src/TaskFlow.Core/Domain/TaskItem.cs
src/TaskFlow.Core/TaskFlow.Core.csproj
src/TaskFlow.Infrastructure/DependencyInjection.cs
src/TaskFlow.Infrastructure/InMemoryTaskRepository.cs
src/TaskFlow.Infrastructure/JsonTaskRepository.cs
src/TaskFlow.Infrastructure/Persistence/EfTaskRepository.cs
src/TaskFlow.Infrastructure/Persistence/Migrations/20261029175255_InitialCreate.cs
src/TaskFlow.Infrastructure/Persistence/Migrations/20261029181438_AddUserCreatedAt.cs
src/TaskFlow.Infrastructure/Persistence/TaskFlowDbContext.cs
src/TaskFlow.Infrastructure/Security/PasswordHasher.cs
src/TaskFlow.Infrastructure/TaskFlow.Infrastructure.csproj
tests/TaskFlow.Api.Tests/ApiFactory.cs
tests/TaskFlow.Api.Tests/ApiTests.cs
tests/TaskFlow.Api.Tests/ArquitecturaTests.cs
tests/TaskFlow.Api.Tests/PasswordHasherTests.cs
tests/TaskFlow.Api.Tests/TaskFlow.Api.Tests.csproj
tests/TaskFlow.Tests/ConsoleAppTests.cs
tests/TaskFlow.Tests/Fakes.cs
tests/TaskFlow.Tests/RepositoryContractTests.cs
tests/TaskFlow.Tests/TaskFlow.Tests.csproj
tests/TaskFlow.Tests/TaskServiceTests.cs
```

---

## Lección 46 · La web, HTTP y las arquitecturas

### Qué vas a aprender

- Cómo funciona una petición web: **HTTP**, métodos, códigos de estado, cabeceras y **cookies**.
- Qué es el **middleware** y por qué el orden importa.
- El modelo **MVC** y cuándo usar controladores frente a *Minimal APIs*.
- Las arquitecturas que nombran los temarios: **N capas**, **Onion / Clean / Hexagonal (puertos y adaptadores)** y **rebanadas verticales**.

### La idea

#### Cómo funciona la web

Un **cliente** (navegador, aplicación móvil, otro servidor) envía una **petición** a un servidor y recibe una **respuesta**. Todo es texto con estructura:

```text
GET /tasks/7 HTTP/1.1                      ← método, ruta y versión
Host: api.ejemplo.com                      ← cabeceras
Authorization: Bearer eyJhbGciOi…
                                           ← línea vacía y, si hay, cuerpo

HTTP/1.1 200 OK                            ← código de estado
Content-Type: application/json             ← cabeceras de la respuesta

{"id":7,"title":"Estudiar EF Core"}        ← cuerpo
```

| Método | Para qué | ¿Cambia datos? | ¿Idempotente? (repetirlo = mismo efecto) |
|---|---|---|---|
| `GET` | Leer | No | Sí |
| `POST` | Crear / acciones | Sí | No |
| `PUT` | Reemplazar/actualizar | Sí | Sí |
| `PATCH` | Cambio parcial | Sí | Depende |
| `DELETE` | Borrar | Sí | Sí |

| Códigos | Significado | Los que más usarás |
|---|---|---|
| **2xx** éxito | Salió bien | `200 OK`, `201 Created` (con cabecera `Location`), `204 No Content` |
| **3xx** redirección | Está en otro sitio | `301`, `302`, `304 Not Modified` |
| **4xx** error del **cliente** | La petición es incorrecta | `400` datos inválidos, `401` sin identificar, `403` sin permiso, `404`, `409` conflicto |
| **5xx** error del **servidor** | Falló el servidor | `500`, `503` |

> **401 y 403 no son lo mismo:** **401** = «no sé quién eres» (falta o es inválido el token). **403** = «sé quién eres, pero no puedes hacer esto».

**HTTP/1.1, HTTP/2 y HTTP/3:** HTTP/1.1 es texto, una petición por conexión a la vez. **HTTP/2** es binario y **multiplexa** muchas peticiones por una sola conexión. **HTTP/3** usa **QUIC** (sobre UDP) y evita que una pérdida de paquetes frene todo. Para ti, programando, el código es el mismo: cambia la configuración del servidor.

**Cookies:** el servidor envía `Set-Cookie`; el navegador la **devuelve en cada petición** al mismo sitio. Sirven para recordar una sesión. Atributos de seguridad que **debes** conocer:

| Atributo | Qué hace |
|---|---|
| `HttpOnly` | JavaScript no puede leerla (limita el robo por XSS) |
| `Secure` | Solo se envía por HTTPS |
| `SameSite=Strict/Lax` | No se envía desde otros sitios (limita CSRF) |
| `Max-Age` / `Expires` | Cuánto dura |

#### Middleware: la cebolla

Cada petición atraviesa una **cadena** de piezas (autenticación, registro, compresión, errores…). Cada una puede hacer algo **antes**, llamar a la **siguiente** (`next()`), y hacer algo **después**, o **cortar** la cadena. **El orden de registro es el orden de ejecución**, y un orden equivocado (por ejemplo, autorizar antes de autenticar) es un fallo clásico.

#### MVC y APIs

- **MVC** (Modelo-Vista-Controlador): el **controlador** recibe la petición, usa el **modelo** (datos y reglas) y devuelve una **vista** (HTML) o datos. Útil en aplicaciones que generan HTML en el servidor.
- En una **API** no hay vista: se devuelven datos (JSON). Hay dos estilos en ASP.NET Core: **controladores** (clases con `[ApiController]`) y **Minimal APIs** (rutas como funciones, menos ceremonia). **Ambos son válidos**; Minimal APIs es cada vez más común en proyectos nuevos y es lo que usa TaskFlow.
- **Validación del modelo:** comprobar la entrada **en la frontera**, antes de llegar a la lógica. Si es inválida: **400**.

### Código 1 (`46a-http-cookies.cs`): HTTP/2, cookies y middleware

```csharp
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
```

#### Resultado real

```text
$ curl -i http://localhost:5101/visita
HTTP/1.1 200 OK
Content-Type: text/plain; charset=utf-8
Date: Thu, 29 Oct 2026 17:57:10 GMT
Server: Kestrel
Set-Cookie: visitas=1; max-age=3600; path=/; samesite=strict; httponly
Transfer-Encoding: chunked
X-Protocolo: HTTP/1.1
X-Duracion-ms: 13.4

Visita número 1

$ curl con el "tarro de cookies" (-c guarda, -b envía)
Visita número 1
Visita número 2
Visita número 3

$ cat jar.txt  (la cookie que guardó curl)
localhost	FALSE	/	FALSE	1793300231	visitas	3

$ curl --http2-prior-knowledge -i http://localhost:5103/visita  (HTTP/2)
HTTP/2 200 
content-type: text/plain; charset=utf-8
date: Thu, 29 Oct 2026 17:57:10 GMT
server: Kestrel
set-cookie: visitas=1; max-age=3600; path=/; samesite=strict; httponly
x-protocolo: HTTP/2
x-duracion-ms: 0.1

Visita número 1

$ curl -i /privado  (el 2.º middleware corta la cadena; el 1.º igualmente añade su cabecera)
HTTP/1.1 401 Unauthorized
Date: Thu, 29 Oct 2026 17:57:10 GMT
Server: Kestrel
Transfer-Encoding: chunked
X-Protocolo: HTTP/1.1
X-Duracion-ms: 0.1

Falta X-Clave

$ curl -H "X-Clave: 1" /privado
Contenido privado
```

**Qué observar:**

1. **Cookie:** la primera respuesta incluye `Set-Cookie: visitas=1; …; samesite=strict; httponly`. Con el «tarro de cookies» de `curl` (`-c` guarda, `-b` envía) el contador sube 1, 2, 3: **el navegador devuelve la cookie y el servidor la lee**.
2. **HTTP/2:** `curl --http2-prior-knowledge` recibe `HTTP/2 200` y la cabecera propia `x-protocolo: HTTP/2` (en HTTP/2 los nombres de cabecera van en minúsculas). **Un fallo real que aparece al probarlo:** con `Http1AndHttp2` en un puerto **sin TLS** el servidor *no puede negociar* y responde siempre HTTP/1.1; hay que escuchar en dos puertos o usar HTTPS (la negociación ALPN solo existe dentro de TLS).
3. **Middleware:** el segundo corta con `401` y **no llama a `next()`**, pero la cabecera `X-Protocolo` del primero **sí aparece**: el primero «envuelve» al segundo.
4. **HTTP/3 no se ejecutó:** requiere QUIC (biblioteca `libmsquic`) y TLS; no está disponible aquí.

### Código 2 (`46b-controlador.cs`): la misma idea con controladores

```csharp
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
```

```text
$ GET /api/notas
[{"id":1,"texto":"Primera nota"}]

$ POST /api/notas
HTTP/1.1 201 Created
Location: http://localhost:5102/api/Notas/2
{"id":2,"texto":"Segunda nota"}

$ POST con texto corto -> 400 automático
{"type":"https://tools.ietf.org/html/rfc9110#section-15.5.1","title":"One or more validation errors occurred.","status":400,"errors":{"Texto":["The field Texto must be a string or array type with a minimum length of '3'."]},"traceId":"00-00e8de7a80d05b27beda8d6ba377106c-9fd7d3d286a55777-00"}

$ GET /api/notas/9 -> 404
404
```

**Qué observar:** `[ApiController]` valida el modelo **solo** (la `400` sale sin escribir código) y `CreatedAtAction` construye la cabecera `Location`. **Detalle que cuesta caro:** en MVC los atributos de validación de un `record` van **en el parámetro** (`[Required] string Texto`); en las Minimal APIs de este proyecto se usó `[property: Required]` y funcionó. Mi primera versión usó `property:` en MVC y devolvió un **500** con el mensaje *«validation metadata defined on property… will be ignored»*: ASP.NET te avisa, pero hay que leer el mensaje.

### Arquitecturas: dónde poner cada cosa

| Arquitectura | Idea | Dependencias | Cuándo |
|---|---|---|---|
| **N capas** (N-tier) | Presentación → Lógica → Datos, de arriba abajo | Cada capa conoce **a la de debajo** (incluida la de datos) | Aplicaciones sencillas |
| **Onion / Clean / Hexagonal** (puertos y adaptadores) | **El dominio en el centro**; todo lo demás depende de él | Hacia **dentro**: infraestructura e interfaz dependen del dominio, nunca al revés | Lógica de negocio que importa y debe poder probarse sola |
| **Rebanadas verticales** (*vertical slice*) | Una carpeta por **funcionalidad**, no por tipo de pieza | Cada rebanada contiene lo suyo de arriba a abajo | Equipos que cambian funcionalidades sueltas |

Todos esos nombres (Onion, Clean, Hexagonal, Puertos/Adaptadores) describen **la misma regla de fondo**: *el negocio no depende de los detalles técnicos*. En TaskFlow:

- **Puertos** (lo que el negocio necesita): `ITaskRepository`, `IClock`. Viven en `Core`.
- **Adaptadores** (cómo se consigue): `InMemoryTaskRepository`, `JsonTaskRepository`, `EfTaskRepository`. Viven en `Infrastructure`.
- **Raíz de composición** (el único sitio que conoce todo y lo conecta): `Program.cs` de la API (y el de la consola).

Esta regla se puede **comprobar con una prueba automática** (lección 50). Y se puede **ver** con una rebanada vertical:

#### Código 3 (`46c-vertical-slice.cs`)

```csharp
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
```

```text
$ POST /tareas  {"titulo":"Aprender rebanadas"}
{"id":1,"titulo":"Aprender rebanadas"}

$ POST /tareas  {"titulo":"x"}   → 400
{"type":"https://tools.ietf.org/html/rfc9110#section-15.5.1","title":"One or more validation errors occurred.","status":400,"errors":{"titulo":["Mínimo 3 caracteres"]}}

$ POST /tareas/1/completar
204

$ GET /tareas?hechas=true
[{"id":1,"titulo":"Aprender rebanadas","hecha":true}]

$ GET /tareas
[{"id":1,"titulo":"Aprender rebanadas","hecha":true},{"id":2,"titulo":"Segunda tarea","hecha":false}]
```

**Cómo se compara:** por capas, añadir «completar tarea» toca controlador, servicio y repositorio (tres carpetas). Por rebanadas, toca **un archivo**. A cambio, hay más riesgo de duplicar lógica entre rebanadas. **No hay una arquitectura «correcta»**: la de capas con dominio protegido (la de TaskFlow) compensa cuando las reglas son complejas; las rebanadas, cuando el trabajo son muchas funcionalidades independientes. **Mucha gente combina ambas** (rebanadas por fuera, dominio protegido por dentro).

### Errores típicos

| Síntoma | Causa | Solución |
|---|---|---|
| Todo devuelve `200` aunque falle | Se devuelve el error dentro de un cuerpo con `200` | Usa el código de estado correcto (`400`, `404`, `409`…) |
| `401` cuando debería ser `403` (o al revés) | Confusión entre identificar y autorizar | `401` = sin identidad; `403` = sin permiso |
| Orden de middleware roto | `UseAuthorization` antes de `UseAuthentication` | Autenticación **primero** |
| Cookie robada con JavaScript | Falta `HttpOnly` | Actívalo siempre en cookies de sesión |
| El dominio importa EF Core o ASP.NET | Atajo «temporal» | Prueba de arquitectura (lección 50) |

> **Ejercicio 46.1:** añade un middleware que mida el tiempo y escriba un `Console.WriteLine` con el método, la ruta y la duración.
>
> **Ejercicio 46.2:** añade a `46c` una rebanada `EliminarTarea` y comprueba que **no toca** ningún archivo existente salvo el registro en el arranque.

---

## Lección 47 · La API de TaskFlow: Minimal API, DTO, validación y errores

### Qué vas a aprender

- Convertir los casos de uso de `TaskService` en **rutas HTTP**.
- Qué es un **DTO** y por qué no se devuelve el modelo del dominio directamente.
- **Validación en dos niveles:** forma de los datos (atributos) y reglas del negocio (dominio).
- **Errores estándar** con `ProblemDetails` (RFC 9457) y un manejador global de excepciones.

### La idea

Una API REST modela **recursos** con rutas y métodos:

| Operación (CRUD) | Método y ruta | Éxito | Errores |
|---|---|---|---|
| Crear | `POST /tasks` | `201 Created` + `Location` | `400` |
| Listar (con filtros) | `GET /tasks?priority=High&done=false` | `200` | — |
| Leer una | `GET /tasks/{id}` | `200` | `404` |
| Actualizar | `PUT /tasks/{id}` | `200` | `400`, `404` |
| Completar | `POST /tasks/{id}/done` | `200` | `404` |
| Borrar | `DELETE /tasks/{id}` | `204` | `403`, `404` |
| Estadísticas | `GET /tasks/stats` | `200` | — |

**DTO** (*Data Transfer Object*): una clase hecha **solo para la frontera HTTP**. Si devolvieras `TaskItem` directamente, el día que cambies el dominio **rompes a todos los clientes**; con un DTO el contrato de la API es independiente. Aquí `TaskResponse` añade `isOverdue` y convierte la prioridad en texto.

**Dos niveles de validación (no los confundas):**

1. **Forma** (¿el JSON tiene sentido?): atributos `[Required]`, `[StringLength]` en el DTO → `400` automático.
2. **Negocio** (¿esto se puede hacer?): `TaskService` lanza `ValidationException` («la fecha límite no puede estar en el pasado»). **Esta regla sigue viviendo en el dominio**, no en la API.

**`ProblemDetails`** es el formato estándar para errores en JSON: `type`, `title`, `status`, `detail`. Todos los clientes pueden tratarlos igual.

### Código (los archivos de la API)

`Program.cs`: el punto de entrada y **raíz de composición**

```csharp
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskFlow.Api;
using TaskFlow.Application;
using TaskFlow.Domain;
using TaskFlow.Infrastructure;
using TaskFlow.Infrastructure.Persistence;
using TaskFlow.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

// ---- Servicios (contenedor de DI) ----
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSingleton<IClock, UtcClock>();
builder.Services.AddScoped<TaskService>();
// Los enums viajan como texto ("High") y no como número: más legible para quien consume la API.
builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<DomainExceptionHandler>();
builder.Services.AddHealthChecks().AddDbContextCheck<TaskFlowDbContext>("postgres");
builder.Services.AddOpenApi();
builder.Services.AddValidation();   // .NET 10: valida los atributos [Required], [StringLength]... de los DTO y responde 400

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<JwtTokenService>();
var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? new();
if (jwt.Key.Length < 32) throw new InvalidOperationException("Jwt:Key debe tener al menos 32 caracteres (configúrala con user-secrets o variable de entorno).");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = jwt.Issuer, ValidAudience = jwt.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
        ClockSkew = TimeSpan.FromSeconds(10),
    });
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", p => p.RequireRole("Admin"));

var app = builder.Build();

// ---- Pipeline de middleware (el orden importa) ----
app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.MapHealthChecks("/health");

// ---- Autenticación ----
var auth = app.MapGroup("/auth");
auth.MapPost("/register", (RegisterRequest req, TaskFlowDbContext db) =>
{
    if (db.Users.Any(u => u.UserName == req.UserName)) return Results.Conflict(new { error = "Ese usuario ya existe." });
    // El primer usuario registrado es Admin; el resto, User.
    string role = db.Users.Any() ? "User" : "Admin";
    db.Users.Add(new UserAccount { UserName = req.UserName, PasswordHash = PasswordHasher.Hash(req.Password), Role = role });
    db.SaveChanges();
    return Results.Created($"/auth/{req.UserName}", new { req.UserName, role });
});

auth.MapPost("/login", (LoginRequest req, TaskFlowDbContext db, JwtTokenService tokens) =>
{
    var user = db.Users.FirstOrDefault(u => u.UserName == req.UserName);
    // Mismo mensaje si el usuario no existe o la contraseña falla: no revelamos cuál de los dos fue.
    if (user is null || !PasswordHasher.Verify(req.Password, user.PasswordHash)) return Results.Unauthorized();
    return Results.Ok(new TokenResponse(tokens.Create(user.Id, user.UserName, user.Role), "Bearer", jwt.ExpiresMinutes));
});

// ---- CRUD de tareas (requiere token) ----
var tasks = app.MapGroup("/tasks").RequireAuthorization();

tasks.MapGet("/", (TaskService svc, IClock clock, bool? done, Priority? priority, string? q, bool overdue = false) =>
    svc.List(new TaskFilter(done, priority, q, overdue)).Select(t => TaskResponse.From(t, clock.Today)));

tasks.MapGet("/stats", (TaskService svc) => svc.GetStats());

tasks.MapGet("/{id:int}", (int id, TaskService svc, IClock clock) => TaskResponse.From(svc.Get(id), clock.Today));

tasks.MapPost("/", (CreateTaskRequest req, TaskService svc, IClock clock) =>
{
    var created = svc.Add(new NewTask(req.Title, req.Description, req.Priority, req.DueDate));
    return Results.Created($"/tasks/{created.Id}", TaskResponse.From(created, clock.Today));
});

tasks.MapPut("/{id:int}", (int id, UpdateTaskRequest req, TaskService svc, IClock clock) =>
    TaskResponse.From(svc.Update(id, new TaskChanges(req.Title, req.Description, req.Priority, req.DueDate)), clock.Today));

tasks.MapPost("/{id:int}/done", (int id, TaskService svc, IClock clock) => TaskResponse.From(svc.SetDone(id, true), clock.Today));

// Borrar es una acción sensible: solo el rol Admin (política "AdminOnly").
tasks.MapDelete("/{id:int}", (int id, TaskService svc) => { svc.Delete(id); return Results.NoContent(); })
     .RequireAuthorization("AdminOnly");

app.Run();

// ---- Tipos auxiliares ----
public sealed class UtcClock : IClock { public DateTime Now => DateTime.UtcNow; }

/// <summary>Traduce excepciones del dominio a respuestas HTTP estándar (ProblemDetails, RFC 9457).</summary>
public sealed class DomainExceptionHandler(ILogger<DomainExceptionHandler> log, IProblemDetailsService problems) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext ctx, Exception ex, CancellationToken ct)
    {
        (int status, string title) = ex switch
        {
            ValidationException => (400, "Datos inválidos"),
            NotFoundException   => (404, "No encontrado"),
            BadHttpRequestException => (400, "Solicitud mal formada"),   // JSON roto, enum desconocido, etc.
            _                   => (500, "Error interno"),
        };
        if (status == 500) log.LogError(ex, "Excepción no controlada en {Path}", ctx.Request.Path);   // solo los 500 son "errores" del servidor
        ctx.Response.StatusCode = status;
        return await problems.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = ctx,
            ProblemDetails = { Status = status, Title = title, Detail = status == 500 ? "Ocurrió un error inesperado." : ex.Message },
        });
    }
}

public partial class Program;   // permite que las pruebas de integración hagan WebApplicationFactory<Program>
```

`Dtos.cs`

```csharp
using System.ComponentModel.DataAnnotations;
using TaskFlow.Domain;

namespace TaskFlow.Api;

// DTO = Data Transfer Object: la forma de los datos EN LA FRONTERA HTTP. No se expone el dominio directamente.
public sealed record CreateTaskRequest(
    [property: Required, StringLength(80, MinimumLength = 3)] string Title,
    [property: StringLength(500)] string? Description,
    Priority Priority = Priority.Medium,
    DateOnly? DueDate = null);

public sealed record UpdateTaskRequest(string? Title, string? Description, Priority? Priority, DateOnly? DueDate);

public sealed record TaskResponse(int Id, string Title, string? Description, string Priority, DateOnly? DueDate, bool Done, DateTime CreatedAt, bool IsOverdue)
{
    public static TaskResponse From(TaskItem t, DateOnly today) =>
        new(t.Id, t.Title, t.Description, t.Priority.ToString(), t.DueDate, t.Done, t.CreatedAt, t.IsOverdue(today));
}

public sealed record RegisterRequest([property: Required, StringLength(40, MinimumLength = 3)] string UserName,
                                     [property: Required, StringLength(100, MinimumLength = 8)] string Password);
public sealed record LoginRequest(string UserName, string Password);
public sealed record TokenResponse(string AccessToken, string TokenType, int ExpiresInMinutes);
```

### Cómo se lee, paso a paso

1. **`WebApplication.CreateBuilder`** crea la configuración, el registro y el contenedor de DI. Se **registran servicios** (`builder.Services…`) y luego `Build()`.
2. **`AddInfrastructure(config)`** (un método de extensión, lección 36) registra la base de datos. La API no sabe que existe PostgreSQL.
3. **`AddScoped<TaskService>`**: un `TaskService` **por petición**, que comparte el `DbContext` de esa petición. `IClock` es **Singleton**: no tiene estado.
4. **`ConfigureHttpJsonOptions` + `JsonStringEnumConverter`**: el enum viaja como `"High"` y no como `3`. **Lo descubrí por un fallo real:** mi primer `POST` con `"priority":"High"` devolvió `500`.
5. **`AddValidation()`** (novedad de .NET 10): valida los atributos de los DTO en Minimal APIs.
6. **`app.Use…`**: la cadena de middleware, **en orden**: errores → autenticación → autorización.
7. **Rutas:** `MapGroup("/tasks").RequireAuthorization()` protege todo el grupo. Cada ruta es una función; los parámetros (`TaskService svc`, `CreateTaskRequest req`) **se inyectan o se enlazan solos**: los simples vienen de la ruta o la consulta, el objeto complejo del cuerpo JSON y los servicios del contenedor.
8. **`DomainExceptionHandler`** implementa `IExceptionHandler`: traduce `ValidationException`→`400`, `NotFoundException`→`404`, `BadHttpRequestException`→`400` y cualquier otra→`500` **sin revelar detalles** (solo se registra en el log). **También lo descubrí por un fallo:** mi primera versión devolvía `500` para un JSON mal formado; un error del cliente **no puede** ser un `500`.

### Resultado real: la API completa, recorrida con `curl`

```text
$ curl -i /tasks   (sin token)
HTTP/1.1 401 Unauthorized

$ curl /health
Healthy

$ POST /auth/register (primer usuario = Admin)
{"userName":"ana","role":"Admin"}

$ POST /auth/register (contraseña corta -> 400)
{"type":"https://tools.ietf.org/html/rfc9110#section-15.5.1","title":"One or more validation errors occurred.","status":400,"errors":{"Password":["The field Password must be a string with a minimum length of 8 and a maximum length of 100."]},"traceId":"00-e6894dece3fe6e7c175b1dbff6263428-2d0c2451fc8de799-00"}

$ POST /auth/login (mala contraseña)
HTTP/1.1 401 Unauthorized

token (recortado): eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ...

$ POST /tasks
{"id":1,"title":"Estudiar EF Core","description":null,"priority":"High","dueDate":"2030-01-15","done":false,"createdAt":"2026-10-29T17:54:08.0853164Z","isOverdue":false}

$ POST /tasks con prioridad inválida -> 400
{"type":"https://tools.ietf.org/html/rfc9110#section-15.5.1","title":"Solicitud mal formada","status":400,"detail":"Failed to read parameter \"CreateTaskRequest req\" from the request body as JSON.","traceId":"00-c512e033f2585e10cb08bf1a54ac2071-e71ed487e8eed815-00"}

$ POST /tasks con título corto -> 400
{"type":"https://tools.ietf.org/html/rfc9110#section-15.5.1","title":"One or more validation errors occurred.","status":400,"errors":{"Title":["The field Title must be a string with a minimum length of 3 and a maximum length of 80."]},"traceId":"00-40a9d6569643c829e4580dcba32aca1e-a841a881d96c16d8-00"}

$ POST /tasks con fecha pasada -> 400 (regla del dominio)
{"type":"https://tools.ietf.org/html/rfc9110#section-15.5.1","title":"Datos inválidos","status":400,"detail":"La fecha límite no puede estar en el pasado.","traceId":"00-ecaf81dd2ea41e86286889502ef1baba-65696cbdc6799e2b-00"}

$ GET /tasks?priority=High
[{"id":1,"title":"Estudiar EF Core","description":null,"priority":"High","dueDate":"2030-01-15","done":false,"createdAt":"2026-10-29T17:54:08.085316Z","isOverdue":false}]

$ PUT /tasks/2
{"id":2,"title":"Leer sobre JWT","description":"Revisar claims y roles","priority":"High","dueDate":null,"done":false,"createdAt":"2026-10-29T17:54:08.154747Z","isOverdue":false}

$ POST /tasks/1/done
{"id":1,"title":"Estudiar EF Core","description":null,"priority":"High","dueDate":"2030-01-15","done":true,"createdAt":"2026-10-29T17:54:08.085316Z","isOverdue":false}

$ GET /tasks/stats
{"total":2,"done":1,"pending":1,"overdue":0}

$ GET /tasks/99 -> 404
{"type":"https://tools.ietf.org/html/rfc9110#section-15.5.5","title":"No encontrado","status":404,"detail":"No existe la tarea 99.","traceId":"00-7066fc5244bdffd8d28ba1d86bc4e439-2e71f0a8efb28b91-00"}

$ DELETE /tasks/2 con rol User -> 403
403

$ DELETE /tasks/2 con rol Admin -> 204
204

$ token manipulado -> 401
401

$ psql: lo que guarda la base (hash, nunca la contraseña)
 Id | UserName | Role  |                  hash                   
----+----------+-------+-----------------------------------------
  1 | ana      | Admin | 210000.l+m9lO+Aa/ciwYQHL6YczA==.FMRY...
  2 | luis     | User  | 210000.Of27n8SLu+S/r8WwGqo2/g==.yNmv...
(2 rows)
```

**Qué observar** (cada línea ilustra una decisión de diseño):

- Sin token → `401`. El primer usuario registrado es `Admin`.
- Contraseña corta → `400` **con el campo concreto** (validación de forma, por atributos).
- Prioridad `"Urgente"` (inexistente) → `400 Solicitud mal formada`.
- Fecha de 2020 → `400 Datos inválidos` con **el mensaje del dominio**: la regla vive en `TaskService`.
- `GET /tasks?priority=High`: el filtro llega como parámetro de consulta y se convierte en `TaskFilter`.
- `GET /tasks/99` → `404` con `ProblemDetails`.
- `DELETE` con rol `User` → `403`; con `Admin` → `204`.
- Token manipulado → `401`.
- `psql`: la base guarda **el hash** de la contraseña, nunca la contraseña.

### Una limitación honesta de esta versión

`TaskService` pide `repository.GetAll()` y **filtra en memoria** con LINQ. Con 10 tareas no importa; con 10 millones, **cargaría la tabla entera**. El puerto `ITaskRepository` pensado para la consola no tiene filtros. La mejora natural (Parte XI, proyectos integradores) es añadir al puerto una consulta con filtros que EF traduzca a `WHERE` (lección 48 muestra cómo). **Es un coste real de reutilizar una abstracción hecha para otro propósito**; se dice aquí para que lo sepas, no se oculta.

Tampoco usa `async`: el repositorio es síncrono. Con una base remota y muchas peticiones, lo apropiado es `async` de punta a punta (lección 44).

### Errores típicos

| Síntoma | Causa | Solución |
|---|---|---|
| `500` al enviar un enum como texto | Falta `JsonStringEnumConverter` | `ConfigureHttpJsonOptions` |
| Los atributos del DTO no validan | `AddValidation()` ausente (Minimal API) | Regístralo |
| El cliente ve el mensaje de una excepción interna | Se devuelve `ex.Message` en los `500` | Mensaje genérico + registro en el log |
| El dominio devuelve `string` de error en vez de lanzar | Mezcla de estilos | Un solo estilo: excepciones de dominio traducidas en la frontera |
| Cambiar el dominio rompe a los clientes | Se expone el modelo directamente | DTO |

> **Ejercicio 47.1:** añade `PATCH /tasks/{id}` que solo cambie el título.
>
> **Ejercicio 47.2:** añade paginación (`?page=2&size=10`) a `GET /tasks` y cabecera `X-Total-Count`.
>
> **Ejercicio 47.3:** añade una versión `v2` de la ruta que devuelva las fechas en formato ISO completo.

---

## Lección 48 · Entity Framework Core y PostgreSQL

### Qué vas a aprender

- Qué es un **ORM** y qué hace **Entity Framework Core**.
- Definir un **modelo** y un `DbContext`; *Code First*; **migraciones** (crear, aplicar, revertir).
- Cómo **LINQ se convierte en SQL** (y cómo verlo).
- **Seguimiento de cambios**, el problema **N+1**, **relaciones**, **transacciones**, **concurrencia optimista** y **operaciones masivas**.
- Por qué **nunca** se concatena texto del usuario en SQL.

### La idea

Un **ORM** (*Object-Relational Mapper*) conecta **objetos** de C# con **filas** de tablas. Escribes LINQ y clases; EF Core genera el SQL. Ventajas: menos código repetitivo y seguridad de tipos. Riesgo: **no ver el SQL que genera** y escribir consultas ineficientes. Por eso esta lección **muestra el SQL**.

| Concepto de EF Core | Qué es |
|---|---|
| `DbContext` | La «sesión» con la base: consulta y guarda. **Barato de crear, de vida corta** (uno por petición) |
| `DbSet<T>` | Una tabla |
| **Entidad** | Una clase mapeada a una tabla |
| **Code First** | Escribes las clases; EF crea la base |
| **Migración** | Un archivo que describe un **cambio de esquema**, con `Up` (aplicar) y `Down` (revertir) |
| **Change tracker** | EF recuerda las entidades que cargaste y detecta qué cambió al llamar a `SaveChanges` |
| **Propiedad de navegación** | `libro.Autor`, `autor.Libros`: seguir una relación como objeto |

### Código 1: el modelo de TaskFlow (`TaskFlowDbContext.cs`)

```csharp
using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain;

namespace TaskFlow.Infrastructure.Persistence;

/// <summary>Una cuenta de usuario. Nunca guardamos la contraseña, solo su hash.</summary>
public sealed class UserAccount
{
    public int Id { get; set; }
    public string UserName { get; set; } = "";
    public string PasswordHash { get; set; } = "";   // formato: iteraciones.sal.hash (base64)
    public string Role { get; set; } = "User";        // "User" o "Admin"
    public DateTime CreatedAt { get; set; }           // lo rellena la base de datos (ver OnModelCreating)
}

/// <summary>El DbContext es la "sesión" con la base: EF Core lo usa para consultar y guardar.</summary>
public sealed class TaskFlowDbContext(DbContextOptions<TaskFlowDbContext> options) : DbContext(options)
{
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<UserAccount> Users => Set<UserAccount>();

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<TaskItem>(e =>
        {
            e.ToTable("tasks");
            e.HasKey(t => t.Id);
            e.Property(t => t.Id).ValueGeneratedOnAdd();
            e.Property(t => t.Title).HasMaxLength(80).IsRequired();
            e.Property(t => t.Description).HasMaxLength(500);
            e.Property(t => t.Priority).HasConversion<int>();
            e.HasIndex(t => new { t.Done, t.DueDate });     // para filtrar pendientes por fecha
        });

        model.Entity<UserAccount>(e =>
        {
            e.ToTable("users");
            e.HasIndex(u => u.UserName).IsUnique();         // dos usuarios no pueden llamarse igual
            e.Property(u => u.UserName).HasMaxLength(40).IsRequired();
            e.Property(u => u.Role).HasMaxLength(20).IsRequired();
            e.Property(u => u.CreatedAt).HasDefaultValueSql("now()");   // valor por defecto calculado por PostgreSQL
        });
    }
}
```

`EfTaskRepository.cs`: el mismo puerto `ITaskRepository`, otro adaptador

```csharp
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application;
using TaskFlow.Domain;

namespace TaskFlow.Infrastructure.Persistence;

/// <summary>El mismo puerto ITaskRepository, ahora respaldado por PostgreSQL a través de EF Core.</summary>
public sealed class EfTaskRepository(TaskFlowDbContext db) : ITaskRepository
{
    // AsNoTracking: solo leemos, así EF no gasta memoria vigilando cambios.
    public IReadOnlyList<TaskItem> GetAll() => db.Tasks.AsNoTracking().OrderBy(t => t.Id).ToList();

    public TaskItem? Find(int id) => db.Tasks.AsNoTracking().FirstOrDefault(t => t.Id == id);

    public TaskItem Add(TaskItem task)
    {
        db.Tasks.Add(task);          // Id = 0 → la base genera el Id (columna identity)
        db.SaveChanges();
        return task;
    }

    public void Update(TaskItem task)
    {
        db.Tasks.Update(task);       // marca todas las columnas como modificadas
        db.SaveChanges();
        db.ChangeTracker.Clear();    // soltamos la entidad para poder volver a cargar otra copia
    }

    public bool Remove(int id) => db.Tasks.Where(t => t.Id == id).ExecuteDelete() > 0;
}
```

**Observa:** `TaskItem` (el record del dominio) se mapea **tal cual**, sin atributos ni referencias a EF en el dominio: toda la configuración está en `OnModelCreating` (API fluida). `ITaskRepository` no cambió ni una línea: **solo se añadió un adaptador**.

### Migraciones: crear, aplicar y revertir

| Comando | Qué hace |
|---|---|
| `dotnet tool install --global dotnet-ef` | Instala la herramienta de EF (una vez) |
| `dotnet ef migrations add NOMBRE --project Infraestructura --startup-project Api -o Persistence/Migrations` | Compara el modelo con la última migración y **genera el código del cambio** |
| `dotnet ef migrations script DESDE HASTA` | Muestra el **SQL** que se ejecutaría |
| `dotnet ef migrations list` | Lista las migraciones y cuáles están pendientes |
| `dotnet ef database update [MIGRACIÓN]` | Aplica hasta la última (o hasta la indicada, **también hacia atrás**) |
| `dotnet ef migrations remove` | Borra la última migración **si no se aplicó** |

**Primera migración (`InitialCreate`)**, el SQL que genera para PostgreSQL:

```text
Build started...
Build succeeded.
CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);
START TRANSACTION;
CREATE TABLE tasks (
    "Id" integer GENERATED BY DEFAULT AS IDENTITY,
    "Title" character varying(80) NOT NULL,
    "Description" character varying(500),
    "Priority" integer NOT NULL,
    "DueDate" date,
    "Done" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_tasks" PRIMARY KEY ("Id")
);
CREATE TABLE users (
    "Id" integer GENERATED BY DEFAULT AS IDENTITY,
    "UserName" character varying(40) NOT NULL,
    "PasswordHash" text NOT NULL,
    "Role" character varying(20) NOT NULL,
    CONSTRAINT "PK_users" PRIMARY KEY ("Id")
);
CREATE INDEX "IX_tasks_Done_DueDate" ON tasks ("Done", "DueDate");
CREATE UNIQUE INDEX "IX_users_UserName" ON users ("UserName");
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20261029175255_InitialCreate', '10.0.14');
COMMIT;
```

Tras aplicarla, la base real:

```text
                                       Table "public.tasks"
   Column    |           Type           | Collation | Nullable |             Default              
-------------+--------------------------+-----------+----------+----------------------------------
 Id          | integer                  |           | not null | generated by default as identity
 Title       | character varying(80)    |           | not null | 
 Description | character varying(500)   |           |          | 
 Priority    | integer                  |           | not null | 
 DueDate     | date                     |           |          | 
 Done        | boolean                  |           | not null | 
 CreatedAt   | timestamp with time zone |           | not null | 
Indexes:
    "PK_tasks" PRIMARY KEY, btree ("Id")
    "IX_tasks_Done_DueDate" btree ("Done", "DueDate")
```

**Evolución real del esquema:** se añadió `CreatedAt` a `UserAccount` y se creó, aplicó, **revirtió** y volvió a aplicar una migración:

```text
$ (editamos UserAccount: nueva propiedad CreatedAt)
$ dotnet ef migrations add AddUserCreatedAt
Done. To undo this action, use 'ef migrations remove'

$ cat Persistence/Migrations/*_AddUserCreatedAt.cs   (solo Up y Down)
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "users");
        }

$ dotnet ef migrations script InitialCreate AddUserCreatedAt   (el SQL que se aplicaría)
START TRANSACTION;
ALTER TABLE users ADD "CreatedAt" timestamp with time zone NOT NULL DEFAULT (now());
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20261029181438_AddUserCreatedAt', '10.0.14');
COMMIT;

$ dotnet ef migrations list
(el log de EF Core con la consulta de __EFMigrationsHistory se omite)
20261029175255_InitialCreate
20261029181438_AddUserCreatedAt (Pending)

$ dotnet ef database update
Applying migration '20261029181438_AddUserCreatedAt'.
Done.

$ psql: \d users
                                       Table "public.users"
    Column    |           Type           | Collation | Nullable |             Default              
--------------+--------------------------+-----------+----------+----------------------------------
 Id           | integer                  |           | not null | generated by default as identity
 UserName     | character varying(40)    |           | not null | 
 PasswordHash | text                     |           | not null | 
 Role         | character varying(20)    |           | not null | 
 CreatedAt    | timestamp with time zone |           | not null | now()
Indexes:
    "PK_users" PRIMARY KEY, btree ("Id")
    "IX_users_UserName" UNIQUE, btree ("UserName")

$ psql: select "UserName","CreatedAt" from users   (los usuarios que ya existían recibieron la fecha por defecto)
 UserName |           CreatedAt           
----------+-------------------------------
 ana      | 2026-10-29 18:14:57.639662+00
 luis     | 2026-10-29 18:14:57.639662+00
(2 rows)

$ dotnet ef database update InitialCreate   (VOLVER ATRÁS: ejecuta Down)
Reverting migration '20261029181438_AddUserCreatedAt'.
Done.
columnas CreatedAt tras revertir: 0

$ dotnet ef database update   (y volver a avanzar)
Applying migration '20261029181438_AddUserCreatedAt'.
Done.
```

**Qué observar:**

- `Up` y `Down` son simétricos: **toda migración debería poder deshacerse**.
- Los usuarios que ya existían recibieron un `CreatedAt` por el valor por defecto `now()`: **una columna `NOT NULL` nueva necesita un valor por defecto**, o la migración falla con filas existentes.
- Tras `database update InitialCreate` la columna desaparece (`columnas CreatedAt tras revertir: 0`).
- **Reglas de oro:** (1) las migraciones **se suben a Git**; (2) **nunca edites** una migración ya aplicada en producción: crea otra nueva; (3) en producción **no** apliques migraciones al arrancar la app desde varias instancias a la vez (carreras): usa un paso de despliegue (lección 50, *bundle*).

### Código 2 (`48-efcore.cs`): consultas, N+1, transacciones y concurrencia

Un programa aparte (biblioteca de autores, libros y etiquetas) que **cuenta los comandos SQL** que EF envía de verdad con un interceptor.

```csharp
#:property PublishAot=false
#:package Npgsql.EntityFrameworkCore.PostgreSQL@10.0.3

using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

const string Conexion = "Host=localhost;Database=efdemo;Username=taskflow;Password=taskflow_dev";
var contador = new ContadorDeComandos();                         // cuenta cuántas consultas SQL se envían de verdad
DbContextOptions<Biblioteca> Opciones() => new DbContextOptionsBuilder<Biblioteca>().UseNpgsql(Conexion).AddInterceptors(contador).Options;

// ---- Crear el esquema desde el modelo y sembrar datos ----
await using (var db = new Biblioteca(Opciones()))
{
    await db.Database.EnsureDeletedAsync();
    await db.Database.EnsureCreatedAsync();                       // en proyectos reales: migraciones (lección 47); aquí basta para experimentar
    var tags = new[] { new Etiqueta { Nombre = "novela" }, new Etiqueta { Nombre = "clásico" }, new Etiqueta { Nombre = "ciencia" } };
    db.Autores.AddRange(
        new Autor { Nombre = "Cervantes",  Libros = { new Libro { Titulo = "El Quijote", Paginas = 1200, Etiquetas = { tags[0], tags[1] } },
                                                      new Libro { Titulo = "La Galatea", Paginas = 400, Etiquetas = { tags[0] } } } },
        new Autor { Nombre = "Borges",     Libros = { new Libro { Titulo = "Ficciones", Paginas = 200, Etiquetas = { tags[0] } },
                                                      new Libro { Titulo = "El Aleph", Paginas = 180 } } },
        new Autor { Nombre = "Sagan",      Libros = { new Libro { Titulo = "Cosmos", Paginas = 365, Etiquetas = { tags[2] } } } },
        new Autor { Nombre = "Sin libros" });
    int guardados = await db.SaveChangesAsync();
    Console.WriteLine($"Sembrado: {guardados} filas guardadas en una sola transacción");
}

// ================= 1) De LINQ a SQL =================
Console.WriteLine("\n=== 1) LINQ → SQL (ToQueryString muestra el SQL sin ejecutarlo) ===");
await using (var db = new Biblioteca(Opciones()))
{
    Mostrar("Libros de más de 300 páginas, ordenados",
        db.Libros.Where(l => l.Paginas > 300).OrderBy(l => l.Titulo));
    Mostrar("Proyección: solo título y nombre del autor",
        db.Libros.Select(l => new { l.Titulo, Autor = l.Autor.Nombre }));
    Mostrar("Agrupar: páginas totales por autor",
        db.Libros.GroupBy(l => l.Autor.Nombre).Select(g => new { Autor = g.Key, Total = g.Sum(l => l.Paginas) }));
    Mostrar("Paginación: página 2 de tamaño 2",
        db.Libros.OrderBy(l => l.Id).Skip(2).Take(2));
    Mostrar("Búsqueda sin distinguir mayúsculas",
        db.Libros.Where(l => EF.Functions.ILike(l.Titulo, "%quij%")));
}

// ================= 2) Seguimiento de cambios =================
Console.WriteLine("\n=== 2) Change tracking ===");
await using (var db = new Biblioteca(Opciones()))
{
    var libro = await db.Libros.FirstAsync(l => l.Titulo == "Cosmos");
    Console.WriteLine($"Tras cargar: estado = {db.Entry(libro).State}");
    libro.Paginas += 10;
    Console.WriteLine($"Tras cambiar Paginas: estado = {db.Entry(libro).State} (EF detectó el cambio solo)");
    contador.Reiniciar(); await db.SaveChangesAsync();
    Console.WriteLine($"SaveChanges envió {contador.Cuenta} comando(s); solo actualiza la columna modificada: {contador.Ultimo!.Replace("\n", " ")[..80]}…");
    var soloLectura = await db.Libros.AsNoTracking().FirstAsync();
    Console.WriteLine($"Con AsNoTracking: estado = {db.Entry(soloLectura).State} (más rápido para consultas de solo lectura)");
}

// ================= 3) El problema N+1 =================
Console.WriteLine("\n=== 3) El problema N+1 ===");
await using (var db = new Biblioteca(Opciones()))
{
    contador.Reiniciar();
    var autores = await db.Autores.ToListAsync();                       // 1 consulta
    foreach (var a in autores)
        a.Libros = await db.Libros.Where(l => l.AutorId == a.Id).ToListAsync();   // +1 consulta POR autor
    Console.WriteLine($"Cargar libros autor por autor     : {contador.Cuenta} consultas  (1 + {autores.Count})");
}
await using (var db = new Biblioteca(Opciones()))
{
    contador.Reiniciar();
    var autores = await db.Autores.Include(a => a.Libros).ToListAsync();      // 1 consulta con JOIN
    Console.WriteLine($"Con Include (un JOIN)               : {contador.Cuenta} consulta   ({autores.Sum(a => a.Libros.Count)} libros)");
}
await using (var db = new Biblioteca(Opciones()))
{
    contador.Reiniciar();
    var resumen = await db.Autores.Select(a => new { a.Nombre, Libros = a.Libros.Count, Paginas = a.Libros.Sum(l => (int?)l.Paginas) ?? 0 }).ToListAsync();
    Console.WriteLine($"Proyección (solo lo que se muestra) : {contador.Cuenta} consulta");
    foreach (var r in resumen) Console.WriteLine($"    {r.Nombre,-11} {r.Libros} libro(s), {r.Paginas} páginas");
}

// ================= 4) Muchos a muchos =================
Console.WriteLine("\n=== 4) Muchos a muchos ===");
await using (var db = new Biblioteca(Opciones()))
{
    var novelas = await db.Libros.Where(l => l.Etiquetas.Any(e => e.Nombre == "novela")).OrderBy(l => l.Titulo).Select(l => l.Titulo).ToListAsync();
    Console.WriteLine("Libros etiquetados «novela»: " + string.Join(", ", novelas));
    Console.WriteLine("Tabla intermedia creada por EF: " + string.Join(", ", db.Model.GetEntityTypes().Where(t => t.ClrType == typeof(Dictionary<string, object>)).Select(t => t.GetTableName())));
}

// ================= 5) Transacciones =================
Console.WriteLine("\n=== 5) Transacciones ===");
await using (var db = new Biblioteca(Opciones()))
{
    await using var tx = await db.Database.BeginTransactionAsync();
    db.Autores.Add(new Autor { Nombre = "Temporal" });
    await db.SaveChangesAsync();
    Console.WriteLine($"Dentro de la transacción hay {await db.Autores.CountAsync()} autores");
    await tx.RollbackAsync();                                            // deshace todo
}
await using (var db = new Biblioteca(Opciones()))
    Console.WriteLine($"Tras el rollback hay {await db.Autores.CountAsync()} autores (el «Temporal» desapareció)");

// ================= 6) Concurrencia optimista =================
Console.WriteLine("\n=== 6) Concurrencia optimista (dos usuarios editan lo mismo) ===");
await using (var db1 = new Biblioteca(Opciones()))
await using (var db2 = new Biblioteca(Opciones()))
{
    var porAna = await db1.Libros.FirstAsync(l => l.Titulo == "Ficciones");
    var porLuis = await db2.Libros.FirstAsync(l => l.Titulo == "Ficciones");
    porAna.Paginas = 210; await db1.SaveChangesAsync();                  // Ana guarda primero
    Console.WriteLine("Ana guarda: correcto");
    porLuis.Paginas = 999;
    try { await db2.SaveChangesAsync(); }
    catch (DbUpdateConcurrencyException) { Console.WriteLine("Luis guarda: DbUpdateConcurrencyException → la fila cambió desde que la leyó (no se pisan los cambios de Ana)"); }
}

// ================= 7) Operaciones masivas =================
Console.WriteLine("\n=== 7) ExecuteUpdate / ExecuteDelete (sin cargar entidades) ===");
await using (var db = new Biblioteca(Opciones()))
{
    contador.Reiniciar();
    int cambiados = await db.Libros.Where(l => l.Paginas < 300).ExecuteUpdateAsync(s => s.SetProperty(l => l.Paginas, l => l.Paginas + 1));
    Console.WriteLine($"ExecuteUpdate: {cambiados} filas en {contador.Cuenta} comando · SQL: {contador.Ultimo!.Replace("\n", " ")}");
    int borrados = await db.Autores.Where(a => !a.Libros.Any()).ExecuteDeleteAsync();
    Console.WriteLine($"ExecuteDelete: {borrados} autor(es) sin libros eliminados");
}

// ================= 8) SQL propio y la inyección SQL =================
Console.WriteLine("\n=== 8) SQL propio e inyección SQL ===");
await using (var db = new Biblioteca(Opciones()))
{
    string entrada = "x' OR '1'='1";                                      // lo que escribiría un atacante
    var mal = await db.Libros.FromSqlRaw("SELECT *, xmin FROM \"Libros\" WHERE \"Titulo\" = '" + entrada + "'").ToListAsync();   // MAL: concatenar. (Se añade xmin porque FromSql debe devolver TODAS las columnas del modelo, incluida la de concurrencia)
    Console.WriteLine($"CONCATENANDO el texto del usuario : {mal.Count} filas devueltas  ← ¡devolvió TODA la tabla!");
    var bien = await db.Libros.FromSql($"SELECT *, xmin FROM \"Libros\" WHERE \"Titulo\" = {entrada}").ToListAsync();   // BIEN: parámetro
    Console.WriteLine($"Con FromSql (interpolación segura): {bien.Count} filas devueltas  ← el texto viaja como parámetro, no como SQL");
}

static void Mostrar<T>(string titulo, IQueryable<T> consulta)
    => Console.WriteLine($"\n-- {titulo}\n{consulta.ToQueryString().Replace("\r", "")}");

// ---------------------------------------------------------------- modelo
public class Autor { public int Id { get; set; } public string Nombre { get; set; } = ""; public List<Libro> Libros { get; set; } = []; }
public class Libro
{
    public int Id { get; set; } public string Titulo { get; set; } = ""; public int Paginas { get; set; }
    public int AutorId { get; set; } public Autor Autor { get; set; } = null!;
    public List<Etiqueta> Etiquetas { get; set; } = [];                 // muchos a muchos: EF crea la tabla intermedia sola
}
public class Etiqueta { public int Id { get; set; } public string Nombre { get; set; } = ""; public List<Libro> Libros { get; set; } = []; }

public class Biblioteca(DbContextOptions<Biblioteca> opciones) : DbContext(opciones)
{
    public DbSet<Autor> Autores => Set<Autor>();
    public DbSet<Libro> Libros => Set<Libro>();
    protected override void OnModelCreating(ModelBuilder m)
    {
        m.Entity<Libro>().Property<uint>("xmin").HasColumnType("xid").ValueGeneratedOnAddOrUpdate().IsConcurrencyToken();   // token de concurrencia de PostgreSQL
        m.Entity<Autor>().HasMany(a => a.Libros).WithOne(l => l.Autor).HasForeignKey(l => l.AutorId).OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>Interceptor: se entera de cada comando SQL que EF envía a la base. Muy útil para detectar N+1.</summary>
public class ContadorDeComandos : DbCommandInterceptor
{
    public int Cuenta { get; private set; } public string? Ultimo { get; private set; }
    public void Reiniciar() { Cuenta = 0; Ultimo = null; }
    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand c, CommandEventData e, InterceptionResult<DbDataReader> r, CancellationToken ct = default)
    { Cuenta++; Ultimo = c.CommandText; return base.ReaderExecutingAsync(c, e, r, ct); }
    public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand c, CommandEventData e, InterceptionResult<int> r, CancellationToken ct = default)
    { Cuenta++; Ultimo = c.CommandText; return base.NonQueryExecutingAsync(c, e, r, ct); }
}
```

#### Resultado real

```text
Sembrado: 17 filas guardadas en una sola transacción

=== 1) LINQ → SQL (ToQueryString muestra el SQL sin ejecutarlo) ===

-- Libros de más de 300 páginas, ordenados
SELECT l."Id", l."AutorId", l."Paginas", l."Titulo", l.xmin
FROM "Libros" AS l
WHERE l."Paginas" > 300
ORDER BY l."Titulo"

-- Proyección: solo título y nombre del autor
SELECT l."Titulo", a."Nombre" AS "Autor"
FROM "Libros" AS l
INNER JOIN "Autores" AS a ON l."AutorId" = a."Id"

-- Agrupar: páginas totales por autor
SELECT a."Nombre" AS "Autor", COALESCE(sum(l."Paginas"), 0)::int AS "Total"
FROM "Libros" AS l
INNER JOIN "Autores" AS a ON l."AutorId" = a."Id"
GROUP BY a."Nombre"

-- Paginación: página 2 de tamaño 2
-- @p='2'
SELECT l."Id", l."AutorId", l."Paginas", l."Titulo", l.xmin
FROM "Libros" AS l
ORDER BY l."Id"
LIMIT @p OFFSET @p

-- Búsqueda sin distinguir mayúsculas
SELECT l."Id", l."AutorId", l."Paginas", l."Titulo", l.xmin
FROM "Libros" AS l
WHERE l."Titulo" ILIKE '%quij%'

=== 2) Change tracking ===
Tras cargar: estado = Unchanged
Tras cambiar Paginas: estado = Modified (EF detectó el cambio solo)
SaveChanges envió 1 comando(s); solo actualiza la columna modificada: UPDATE "Libros" SET "Paginas" = @p0 WHERE "Id" = @p1 AND xmin = @p2 RETURNING xm…
Con AsNoTracking: estado = Detached (más rápido para consultas de solo lectura)

=== 3) El problema N+1 ===
Cargar libros autor por autor     : 5 consultas  (1 + 4)
Con Include (un JOIN)               : 1 consulta   (5 libros)
Proyección (solo lo que se muestra) : 1 consulta
    Cervantes   2 libro(s), 1600 páginas
    Borges      2 libro(s), 380 páginas
    Sagan       1 libro(s), 375 páginas
    Sin libros  0 libro(s), 0 páginas

=== 4) Muchos a muchos ===
Libros etiquetados «novela»: El Quijote, Ficciones, La Galatea
Tabla intermedia creada por EF: EtiquetaLibro

=== 5) Transacciones ===
Dentro de la transacción hay 5 autores
Tras el rollback hay 4 autores (el «Temporal» desapareció)

=== 6) Concurrencia optimista (dos usuarios editan lo mismo) ===
Ana guarda: correcto
Luis guarda: DbUpdateConcurrencyException → la fila cambió desde que la leyó (no se pisan los cambios de Ana)

=== 7) ExecuteUpdate / ExecuteDelete (sin cargar entidades) ===
ExecuteUpdate: 2 filas en 1 comando · SQL: UPDATE "Libros" AS l SET "Paginas" = l."Paginas" + 1 WHERE l."Paginas" < 300
ExecuteDelete: 1 autor(es) sin libros eliminados

=== 8) SQL propio e inyección SQL ===
CONCATENANDO el texto del usuario : 5 filas devueltas  ← ¡devolvió TODA la tabla!
Con FromSql (interpolación segura): 0 filas devueltas  ← el texto viaja como parámetro, no como SQL
```

### Cómo se lee, paso a paso

1. **`ToQueryString()`** muestra el SQL **sin ejecutarlo**. El hábito más útil con EF Core: **mira el SQL**.
   - `Where` → `WHERE`; `OrderBy` → `ORDER BY`; `Select` con propiedades de otra entidad → `INNER JOIN`.
   - `GroupBy` + `Sum` → `GROUP BY` con `COALESCE(sum(…), 0)`.
   - `Skip/Take` → `LIMIT … OFFSET`. (En la salida `LIMIT @p OFFSET @p` usa el **mismo parámetro** porque ambos valen 2: no es un error.)
   - `EF.Functions.ILike` → el `ILIKE` de PostgreSQL (búsqueda sin distinguir mayúsculas), que es una función **de PostgreSQL**, no de LINQ estándar.
2. **Change tracking:** al cargar, el estado es `Unchanged`; al modificar `Paginas`, `Modified`. `SaveChanges` envía **un solo `UPDATE` con solo la columna cambiada** y `WHERE Id = … AND xmin = …` (el token de concurrencia). `AsNoTracking()` deja la entidad `Detached`: **más rápido para consultas de solo lectura**.
3. **El problema N+1:** cargar autores y luego sus libros **uno a uno** = `1 + 4 = 5` consultas. Con `Include` = **1** (un `JOIN`). Con una **proyección** (`Select` de solo lo que se muestra, incluyendo `Count` y `Sum`) = **1** y sin traer columnas que no usas. Con 4 autores da igual; con 4000 son **4001 viajes a la base**.
4. **Muchos a muchos:** `Libro.Etiquetas` y `Etiqueta.Libros`; **EF creó la tabla intermedia `EtiquetaLibro` solo**. `Any()` sobre la colección se convierte en un `EXISTS`.
5. **Transacción:** dentro hay 5 autores; tras `Rollback`, 4. `SaveChanges` por sí mismo ya es **atómico** (todo o nada); una transacción explícita sirve para agrupar **varios** `SaveChanges`.
6. **Concurrencia optimista:** Ana y Luis leen el mismo libro. Ana guarda; cuando Luis intenta guardar, EF detecta que `xmin` cambió y lanza `DbUpdateConcurrencyException`: **no se pisan los cambios en silencio**. Qué hacer entonces es decisión de negocio (reintentar, avisar, fusionar). «Optimista» porque **no bloquea** al leer y asume que los conflictos son raros.
7. **`ExecuteUpdate`/`ExecuteDelete`:** una sola sentencia `UPDATE … WHERE`, **sin cargar** las entidades. Imprescindibles para cambios masivos.
8. **Inyección SQL:** el texto `x' OR '1'='1`. **Concatenado** al SQL devolvió **las 5 filas** (el atacante cambió la lógica de la consulta). **Con `FromSql` interpolado** devolvió **0**: EF lo convierte en un **parámetro**, que la base trata siempre como dato y nunca como código. Fíjate también en el aviso del compilador `EF1003` en `FromSqlRaw` con concatenación: **no lo silencies**.

> **Un detalle que costó un error real:** `FromSql` exige devolver **todas** las columnas del modelo; como el libro tiene la columna de concurrencia `xmin`, el `SELECT *` falló con *«The required column 'xmin' was not present»*. Se corrigió con `SELECT *, xmin`.

### Errores típicos

| Síntoma | Causa | Solución |
|---|---|---|
| Miles de consultas por una pantalla | N+1 | `Include`, proyección o una consulta con `Where … Contains` |
| Aplicación lenta al listar | `ToList()` y luego `Where` en memoria | Filtra **antes** de `ToList()` (que lo haga la base) |
| `ObjectDisposedException` | Usar el `DbContext` después de liberarlo | Vida corta, uno por petición (`AddDbContext` lo hace) |
| «Entity is already being tracked» | Dos instancias de la misma fila en un contexto | `AsNoTracking`, o `ChangeTracker.Clear()` |
| Migración que falla con datos existentes | Columna `NOT NULL` sin valor por defecto | `defaultValue` o migrar en dos pasos |
| Modelo y base «desincronizados» | Cambios hechos a mano en la base | Todo cambio de esquema por **migraciones** |
| Cadena de conexión con contraseña en Git | Copia en `appsettings.json` | User-secrets / variables de entorno (lección 49) |

> **Ejercicio 48.1:** añade a la biblioteca una consulta «autores con más de 500 páginas en total» y mira el SQL.
>
> **Ejercicio 48.2:** provoca un N+1 en tu propio código con un contador de comandos y arréglalo con una proyección.
>
> **Ejercicio 48.3:** añade `Description` a las tareas con una nueva migración y revierte con `database update` a la anterior.

---

## Lección 49 · Seguridad: contraseñas, JWT, roles y secretos

### Qué vas a aprender

- Por qué **jamás** se guarda una contraseña y qué se guarda en su lugar (**hash con sal**).
- Qué es un **JWT**, qué contiene, **qué NO protege** y cómo se valida.
- **Autenticación** (¿quién eres?) frente a **autorización** (¿qué puedes hacer?): roles y políticas.
- Dónde guardar **secretos** (y dónde no).

### La idea

| Concepto | Pregunta que responde | En TaskFlow |
|---|---|---|
| **Autenticación** | ¿Quién eres? | `POST /auth/login` → token |
| **Autorización** | ¿Puedes hacer esto? | Política `AdminOnly` en `DELETE` |
| **Hash** | ¿Es esta la contraseña? (sin guardarla) | PBKDF2-SHA512 con sal |
| **Sal** (*salt*) | Hacer único cada hash | 16 bytes aleatorios por usuario |
| **JWT** | Prueba firmada de identidad | `Authorization: Bearer …` |

**Hash de contraseñas.** Una función hash es de **un solo sentido**: de la contraseña sale un valor, pero del valor no se puede recuperar la contraseña. Para validar, se vuelve a calcular y se compara. Reglas:

1. **Algoritmo lento a propósito** para dificultar la fuerza bruta: PBKDF2 (con muchas iteraciones), bcrypt, scrypt o **Argon2**. Nunca SHA-256 «a secas» ni MD5.
2. **Sal aleatoria** distinta por usuario: dos personas con la misma contraseña tienen hashes distintos y las «tablas arcoíris» no sirven.
3. **Comparación en tiempo constante** (`CryptographicOperations.FixedTimeEquals`): comparar byte a byte y cortar al primer fallo filtra información por el tiempo que tarda.
4. **Guardar las iteraciones** en el propio hash para poder subirlas con los años.

> **En producción** la recomendación habitual es no escribir esto a mano: **ASP.NET Core Identity** incluye hashing, bloqueo por intentos, confirmación de correo, doble factor y gestión de roles. Aquí se escribe a mano **para entender qué hace por dentro**.

**JWT** (*JSON Web Token*): tres trozos en Base64URL separados por puntos: **cabecera** (algoritmo), **contenido** (*claims*: quién eres, rol, caducidad) y **firma**. El servidor firma con una **clave secreta**; al recibir el token **recalcula la firma**: si alguien cambió algo, no coincide.

**Lo que un JWT NO hace:** **no cifra**. Cualquiera puede leer el contenido (se verá abajo). **Nunca metas datos secretos en un token.** Tampoco se puede «anular» antes de que caduque sin infraestructura extra, por eso los tokens deben caducar pronto (30 minutos aquí) y se complementan con *refresh tokens* (no cubiertos aquí).

### Código

`PasswordHasher.cs`

```csharp
using System.Security.Cryptography;

namespace TaskFlow.Infrastructure.Security;

/// <summary>
/// Hash de contraseñas con PBKDF2. Cada contraseña lleva su propia sal aleatoria, por eso dos
/// usuarios con la misma contraseña tienen hashes distintos. Nunca se "descifra": se recalcula y compara.
/// </summary>
public static class PasswordHasher
{
    private const int Iterations = 210_000;   // recomendación OWASP para PBKDF2-HMAC-SHA512 (sube con los años)
    private const int SaltSize = 16, HashSize = 32;

    public static string Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA512, HashSize);
        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public static bool Verify(string password, string stored)
    {
        string[] parts = stored.Split('.');
        if (parts.Length != 3 || !int.TryParse(parts[0], out int iterations)) return false;
        byte[] salt = Convert.FromBase64String(parts[1]);
        byte[] expected = Convert.FromBase64String(parts[2]);
        byte[] actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA512, expected.Length);
        return CryptographicOperations.FixedTimeEquals(actual, expected);   // tiempo constante: no filtra información
    }
}
```

`Auth.cs`: opciones y generación del token

```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace TaskFlow.Api;

/// <summary>Opciones del JWT, enlazadas a la sección "Jwt" de la configuración (patrón Options).</summary>
public sealed class JwtOptions
{
    public string Issuer { get; set; } = "TaskFlow";
    public string Audience { get; set; } = "TaskFlow.Clients";
    public string Key { get; set; } = "";            // SECRETO: viene de user-secrets / variable de entorno, nunca del repo
    public int ExpiresMinutes { get; set; } = 30;
}

public sealed class JwtTokenService(IOptions<JwtOptions> options)
{
    private readonly JwtOptions _o = options.Value;

    public string Create(int userId, string userName, string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_o.Key));
        var token = new JwtSecurityToken(
            issuer: _o.Issuer, audience: _o.Audience,
            claims: [new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                     new Claim(ClaimTypes.Name, userName),
                     new Claim(ClaimTypes.Role, role)],
            expires: DateTime.UtcNow.AddMinutes(_o.ExpiresMinutes),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
```

### Cómo se lee, paso a paso

1. **`Hash`**: sal aleatoria (`RandomNumberGenerator`), `Pbkdf2` con 210 000 iteraciones y SHA-512 (valor recomendado por OWASP para esa combinación; **sube con el tiempo**), y devuelve `iteraciones.sal.hash` en Base64. Todo lo necesario para verificar viaja **dentro** del texto guardado.
2. **`Verify`**: separa las tres partes, vuelve a calcular con **la misma sal e iteraciones** y compara en tiempo constante. Si el formato es basura, devuelve `false` sin lanzar excepción (lo prueban tres casos).
3. **`JwtOptions`**: la sección `"Jwt"` de la configuración. **`Key` no está en `appsettings.json`**: viene de *user-secrets* en desarrollo y de una variable de entorno en producción.
4. **`JwtTokenService.Create`**: crea los *claims* (id, nombre, **rol**), la caducidad (UTC) y firma con HMAC-SHA256.
5. **En `Program.cs`:** `AddJwtBearer` define cómo **validar** (emisor, audiencia, clave, margen de reloj de 10 s); `RequireAuthorization()` exige token; `AddPolicy("AdminOnly", p => p.RequireRole("Admin"))` exige un rol.
6. **El login responde igual** si el usuario no existe o la contraseña es incorrecta (`401` sin explicación): **no revela qué usuarios existen**.
7. **La aplicación se niega a arrancar** si `Jwt:Key` mide menos de 32 caracteres: *fallar rápido* es mejor que funcionar con una clave débil.

### Resultado real: un token abierto, y uno manipulado

```text
$ Un JWT son tres trozos en Base64 separados por puntos: cabecera.contenido.firma
cabecera : {"alg": "HS256", "typ": "JWT"}
contenido: nameidentifier   2
contenido: name             luis
contenido: role             User
contenido: exp              1793299454
contenido: iss              TaskFlow
contenido: aud              TaskFlow.Clients
firma    : dk_GrvZICR94qCDMxqTS…  (HMAC-SHA256 de cabecera+contenido con la clave secreta del servidor)

Cualquiera puede LEER el contenido (Base64 no es cifrado). Lo que nadie puede es modificarlo sin romper la firma.

$ DELETE /tasks/1 con el token auténtico de «luis» (rol User)      → 403
$ El atacante cambia el rol a «Admin» y conserva la firma antigua → 401 (firma inválida)
$ Token con la firma recortada                                    → 401
$ Sin cabecera Authorization                                      → 401
```

**Qué observar:**

- El contenido del token **se lee sin ninguna clave**: nombre, rol y caducidad (`exp` es una fecha en segundos desde 1970).
- Con el token auténtico de `luis` (rol `User`), `DELETE` da **403** (identificado, pero sin permiso).
- El atacante cambió `role` a `Admin` **manteniendo la firma antigua**: **401**, porque la firma ya no corresponde al contenido.
- Firma recortada o sin cabecera: **401**.

### Secretos: dónde sí y dónde no

| Lugar | ¿Secretos aquí? | Comentario |
|---|---|---|
| Código fuente | **Nunca** | Queda en Git para siempre |
| `appsettings.json` en el repositorio | **Nunca** | Igual |
| **User-secrets** (`dotnet user-secrets`) | Solo en **desarrollo** | Se guardan **fuera** del proyecto, en tu perfil de usuario |
| **Variables de entorno** | Sí (producción básica) | Contenedores, servidores |
| **Gestor de secretos** (Azure Key Vault, AWS Secrets Manager, HashiCorp Vault) | **Sí (producción seria)** | Rotación, auditoría y control de acceso. *No se probó aquí.* |

Si una contraseña llegó a Git, **cámbiala ya** (rótala): borrar el commit no sirve, ya se pudo copiar.

### Errores típicos

| Síntoma | Causa | Solución |
|---|---|---|
| Contraseñas en texto o con hash rápido | SHA-256/MD5 | PBKDF2/bcrypt/Argon2 con sal, o Identity |
| Token con datos sensibles | Creer que es cifrado | Solo identificadores y roles |
| Clave de firma en el repositorio | Comodidad | user-secrets / entorno / gestor |
| `401` en todas las llamadas con token válido | Orden de middleware o `Issuer`/`Audience` distintos | `UseAuthentication` antes de `UseAuthorization`; mismos valores al emitir y validar |
| Token que no caduca | Sin `exp` | Caducidad corta |
| Mensajes que revelan si existe el usuario | «Usuario no encontrado» | Mismo mensaje y mismo tiempo |
| Sin HTTPS | Token interceptable | HTTPS siempre fuera de local |

> **Ejercicio 49.1:** mide cuánto tarda `Hash` con 210 000 iteraciones y con 1 000. ¿Por qué se quiere *lento*?
>
> **Ejercicio 49.2:** añade una política `OwnerOrAdmin` que permita borrar solo al creador de la tarea (necesitarás añadir `OwnerId` y una migración).
>
> **Ejercicio 49.3:** investiga cómo añadirías *refresh tokens* y escribe el diseño en un párrafo.

---

## Lección 50 · Configuración, entornos, pruebas de integración y despliegue

### Qué vas a aprender

- **Configuración por capas** y **entornos** (`Development`, `Production`), `IOptions` y validación al arrancar.
- **Pruebas de integración** con `WebApplicationFactory` y **pruebas de arquitectura**.
- **Publicar**, ejecutar como en producción, *migration bundle*, **Docker** y **CI** (estos dos últimos, sin ejecutar).

### La idea: la configuración

Una aplicación lee ajustes de **varias fuentes**, apiladas en este orden (**la última gana**):

| # | Fuente | Para qué |
|---|---|---|
| 1 | `appsettings.json` | Valores por defecto, **sin secretos** |
| 2 | `appsettings.{Entorno}.json` | Cambios por entorno |
| 3 | **User-secrets** (solo `Development`) | Secretos de tu máquina |
| 4 | **Variables de entorno** | Producción y contenedores (`Tienda__MaxPedidos` = sección `Tienda`, clave `MaxPedidos`) |
| 5 | **Argumentos de la línea de comandos** | Ajustes puntuales |

El **entorno** lo marca `ASPNETCORE_ENVIRONMENT` (web) o `DOTNET_ENVIRONMENT` (consola). Si no se define, es **`Production`**: por seguridad, el valor por defecto es el más estricto.

**Patrón Options:** en vez de leer `config["Tienda:Max"]` por todas partes, se **enlaza** una sección con una clase tipada (`IOptions<T>`), se valida con atributos y con **`ValidateOnStart()`** se detecta una configuración mala **al arrancar**, no cuando alguien la use por primera vez a las tres de la madrugada.

### Código 1: la configuración en acción (`Program.cs` de `Config`)

```csharp
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
```

#### Resultado real: cinco ejecuciones y los secretos

```text
$ dotnet Config.dll                                            # sin entorno → Production por defecto
Entorno            : Production
Nombre             : Tienda REAL
MaxPedidosPorDia   : 10000
Moneda             : EUR
Cadena de conexión : Host=l… (27 caracteres, oculto)
Clave de API       : (no definida)

$ DOTNET_ENVIRONMENT=Development dotnet Config.dll             # appsettings.Development.json sobrescribe
Entorno            : Development
Nombre             : Tienda (desarrollo)
MaxPedidosPorDia   : 5
Moneda             : EUR
Cadena de conexión : Host=l… (27 caracteres, oculto)
Clave de API       : (no definida)

$ DOTNET_ENVIRONMENT=Development Tienda__MaxPedidosPorDia=77 dotnet Config.dll   # variable de entorno (__ = nivel)
Entorno            : Development
Nombre             : Tienda (desarrollo)
MaxPedidosPorDia   : 77
Moneda             : EUR
Cadena de conexión : Host=l… (27 caracteres, oculto)
Clave de API       : (no definida)

$ DOTNET_ENVIRONMENT=Development Tienda__MaxPedidosPorDia=77 dotnet Config.dll --Tienda:MaxPedidosPorDia=3 --Tienda:Moneda=USD   # argumentos: ganan a todo
Entorno            : Development
Nombre             : Tienda (desarrollo)
MaxPedidosPorDia   : 3
Moneda             : USD
Cadena de conexión : Host=l… (27 caracteres, oculto)
Clave de API       : (no definida)

$ Tienda__Moneda=euros dotnet Config.dll                       # configuración INVÁLIDA → falla al arrancar el host
fail: Microsoft.Extensions.Hosting.Internal.Host[11]
      Hosting failed to start
      Microsoft.Extensions.Options.OptionsValidationException: DataAnnotation validation failed for 'TiendaOptions' members: 'Moneda' with the error: 'La moneda debe ser un código de 3 letras mayúsculas (ISO 4217)'.
      --- End of stack trace from previous location ---
         at Microsoft.Extensions.Hosting.Internal.Host.StartAsync(CancellationToken cancellationToken)
Unhandled exception. Microsoft.Extensions.Options.OptionsValidationException: DataAnnotation validation failed for 'TiendaOptions' members: 'Moneda' with the error: 'La moneda debe ser un código de 3 letras mayúsculas (ISO 4217)'.
(el proceso aborta con código 134, es decir, SIGABRT: NO llega a funcionar con datos malos)

$ dotnet user-secrets set "ApiKey" "sk-demo-1234567890abcdef"   # se guarda FUERA del proyecto, en el perfil del usuario
$ DOTNET_ENVIRONMENT=Development dotnet Config.dll   # los secretos solo se leen en Development
Cadena de conexión : Host=l… (27 caracteres, oculto)
Clave de API       : sk-dem… (24 caracteres, oculto)

$ dotnet Config.dll   # en Production NO se leen los user-secrets: allí se usa variable de entorno o un almacén (Key Vault…)
Clave de API       : (no definida)

$ ApiKey=desde-entorno dotnet Config.dll
Clave de API       : desde-… (13 caracteres, oculto)

$ dotnet user-secrets list   # dónde están
ApiKey = sk-demo-1234567890abcdef
$ ls ~/.microsoft/usersecrets/curso-csharp-config-demo
secrets.json
```

**Qué observar:**

- Sin entorno: `Production` y `Tienda REAL` (por defecto, el más estricto).
- `Development` hace ganar a `appsettings.Development.json` (`MaxPedidosPorDia` = 5).
- La variable de entorno `Tienda__MaxPedidosPorDia=77` **sobrescribe** el archivo; el argumento `--Tienda:MaxPedidosPorDia=3` sobrescribe a todos (y `--Tienda:Moneda=USD` también).
- `Moneda=euros` no cumple la expresión regular: el host **no arranca** (código de salida 134, aborto). **Una corrección:** mi primera versión comentaba que `Build()` ya validaba; **no**: la validación ocurre al **arrancar** el host (`StartAsync`/`Run`). Lo reveló la ejecución.
- Los secretos solo se leen en `Development`; en `Production` hay que dar `ApiKey` por variable de entorno (última prueba). La función `Ocultar` **no imprime nunca un secreto completo**: haz lo mismo en tus registros.

### Código 2: pruebas de integración y de arquitectura

Las **pruebas unitarias** (Parte VI) verifican piezas aisladas. Las **de integración** levantan **la aplicación entera** (rutas, middleware, EF Core, PostgreSQL) y la llaman por HTTP. `WebApplicationFactory<Program>` la arranca **en memoria**, sin red ni puertos.

`ApiFactory.cs`

```csharp
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Api.Tests;

/// <summary>
/// Arranca la API REAL en memoria (sin red ni puertos) contra una base de pruebas distinta a la de desarrollo.
/// </summary>
public sealed class ApiFactory : WebApplicationFactory<Program>
{
    public ApiFactory()
    {
        // Antes de usar la app: crear el esquema con las migraciones y dejar las tablas vacías.
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TaskFlowDbContext>();
        db.Database.Migrate();
        db.Database.ExecuteSqlRaw("TRUNCATE tasks, users RESTART IDENTITY");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");   // distinto de "Development": no carga los user-secrets de desarrollo
        builder.UseSetting("ConnectionStrings:TaskFlow", "Host=localhost;Database=taskflow_test;Username=taskflow;Password=taskflow_dev");
        builder.UseSetting("Jwt:Key", "clave-solo-para-pruebas-0123456789-abcdef");
    }
}
```

`ApiTests.cs`

```csharp
using System.Net.Http.Headers;
using System.Text.Json;

namespace TaskFlow.Api.Tests;

/// <summary>Pruebas de integración: cada una hace peticiones HTTP reales contra la API y la base PostgreSQL.</summary>
public sealed class ApiTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;
    public ApiTests(ApiFactory factory) => _factory = factory;

    // Helpers: cada prueba crea un usuario con nombre único, así no dependen del orden de ejecución.
    private async Task<HttpClient> LoginAsync(string role = "User")
    {
        var client = _factory.CreateClient();
        string name = $"{role.ToLower()}-{Guid.NewGuid():N}"[..20];
        await client.PostAsJsonAsync("/auth/register", new { userName = name, password = "clave-segura-1" });
        var response = await client.PostAsJsonAsync("/auth/login", new { userName = name, password = "clave-segura-1" });
        var token = (await response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("accessToken").GetString();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    [Fact]
    public async Task Tasks_WithoutToken_Returns401()
    {
        var response = await _factory.CreateClient().GetAsync("/tasks");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithWrongPassword_Returns401()
    {
        var client = _factory.CreateClient();
        await client.PostAsJsonAsync("/auth/register", new { userName = "marta-wrong", password = "clave-segura-1" });
        var response = await client.PostAsJsonAsync("/auth/login", new { userName = "marta-wrong", password = "incorrecta" });
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Create_ThenGet_RoundTripsThroughPostgres()
    {
        var client = await LoginAsync();
        var created = await client.PostAsJsonAsync("/tasks", new { title = "Probar la API", priority = "High" });
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);

        var body = await created.Content.ReadFromJsonAsync<JsonElement>();
        int id = body.GetProperty("id").GetInt32();

        var fetched = await client.GetFromJsonAsync<JsonElement>($"/tasks/{id}");
        Assert.Equal("Probar la API", fetched.GetProperty("title").GetString());
        Assert.Equal("High", fetched.GetProperty("priority").GetString());
    }

    [Theory]
    [InlineData("{\"title\":\"ab\"}")]                                  // título demasiado corto (DataAnnotations)
    [InlineData("{\"title\":\"Tarea\",\"priority\":\"Urgente\"}")]       // enum inexistente (JSON)
    [InlineData("{\"title\":\"Tarea\",\"dueDate\":\"2001-01-01\"}")]     // fecha en el pasado (regla del dominio)
    public async Task Create_WithInvalidData_Returns400(string json)
    {
        var client = await LoginAsync();
        var response = await client.PostAsync("/tasks", new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Get_Missing_Returns404WithProblemDetails()
    {
        var client = await LoginAsync();
        var response = await client.GetAsync("/tasks/999999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(404, problem.GetProperty("status").GetInt32());
    }

    [Fact]
    public async Task Delete_AsNormalUser_Returns403()
    {
        // El primer usuario de la base es Admin; nos aseguramos de que exista alguno antes del usuario normal.
        await LoginAsync("admin");
        var user = await LoginAsync("user");
        var created = await user.PostAsJsonAsync("/tasks", new { title = "No borrable" });
        int id = (await created.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetInt32();

        var response = await user.DeleteAsync($"/tasks/{id}");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Health_ReportsHealthy()
    {
        var response = await _factory.CreateClient().GetAsync("/health");
        Assert.Equal("Healthy", await response.Content.ReadAsStringAsync());
    }
}
```

`ArquitecturaTests.cs`: hace **cumplir** la regla de dependencias

```csharp
using System.Reflection;
using TaskFlow.Application;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Api.Tests;

/// <summary>
/// «Pruebas de arquitectura»: hacen cumplir la regla de dependencias. Si alguien añade por error
/// EF Core al dominio, la prueba falla ANTES de que el diseño se degrade.
/// </summary>
public class ArquitecturaTests
{
    private static string[] Referencias(Assembly a) => a.GetReferencedAssemblies().Select(r => r.Name!).ToArray();

    [Fact]
    public void Core_NoDependeDeInfraestructuraNiDeFrameworks()
    {
        var referencias = Referencias(typeof(TaskService).Assembly);
        Assert.DoesNotContain(referencias, r => r.StartsWith("Microsoft.EntityFrameworkCore"));
        Assert.DoesNotContain(referencias, r => r.StartsWith("Npgsql"));
        Assert.DoesNotContain(referencias, r => r.StartsWith("Microsoft.AspNetCore"));
        Assert.DoesNotContain(referencias, r => r == "TaskFlow.Infrastructure" || r == "TaskFlow.Api");
    }

    [Fact]
    public void ElDetector_SiVeEfCoreDondeSiEstaPresente()      // control: si esto fallara, las pruebas anteriores serían vacías
    {
        Assert.Contains(Referencias(typeof(TaskFlowDbContext).Assembly), r => r.StartsWith("Microsoft.EntityFrameworkCore"));
    }

    [Fact]
    public void Infraestructura_NoDependeDeLaApi()
    {
        Assert.DoesNotContain(Referencias(typeof(TaskFlowDbContext).Assembly), r => r == "TaskFlow.Api");
    }

    [Fact]
    public void Api_ConoceTodasLasCapas_PorqueEsLaRaizDeComposicion()
    {
        var referencias = Referencias(typeof(Program).Assembly);
        Assert.Contains("TaskFlow.Core", referencias);
        Assert.Contains("TaskFlow.Infrastructure", referencias);
    }
}
```

**Cómo se lee:**

1. **`ApiFactory`** usa el entorno `Testing` (no `Development`, para **no cargar tus secretos de desarrollo**), apunta a **otra base** (`taskflow_test`), aplica las migraciones y vacía las tablas. `public partial class Program;` (al final de `Program.cs`) permite referenciar el punto de entrada.
2. **Cada prueba** crea su usuario con nombre único (`Guid`), así **no dependen del orden**.
3. **`Create_WithInvalidData_Returns400`** usa `[Theory]` con tres entradas inválidas **de tres capas distintas** (atributo, JSON, regla del dominio).
4. **Pruebas de arquitectura:** `Core` no puede referenciar EF Core, Npgsql ni ASP.NET; `Infrastructure` no puede referenciar la API. **Incluyen una prueba de control** (`ElDetector_SiVeEfCoreDondeSiEstaPresente`) que demuestra que el detector realmente detecta; sin ella, las otras podrían pasar sin comprobar nada.

#### Resultado real: las 40 pruebas de la solución

```text
Passed TaskFlow.Api.Tests.ArquitecturaTests.Core_NoDependeDeInfraestructuraNiDeFrameworks
Passed TaskFlow.Api.Tests.ArquitecturaTests.Infraestructura_NoDependeDeLaApi [< 1 ms]
Passed TaskFlow.Api.Tests.ArquitecturaTests.ElDetector_SiVeEfCoreDondeSiEstaPresente
Passed TaskFlow.Api.Tests.ArquitecturaTests.Api_ConoceTodasLasCapas_PorqueEsLaRaizDeComposicion
Passed TaskFlow.Api.Tests.PasswordHasherTests.Hash_SamePasswordTwice_ProducesDifferentHashes
Passed TaskFlow.Api.Tests.PasswordHasherTests.Verify_WithGarbageStoredValue_ReturnsFalse(stored: "") [< 1 ms]
Passed TaskFlow.Api.Tests.PasswordHasherTests.Verify_WithGarbageStoredValue_ReturnsFalse(stored: "no-es-un-hash") [< 1 ms]
Passed TaskFlow.Api.Tests.PasswordHasherTests.Verify_WithGarbageStoredValue_ReturnsFalse(stored: "1.2") [< 1 ms]
Passed TaskFlow.Api.Tests.PasswordHasherTests.Verify_AcceptsCorrectPassword_RejectsWrongOne
Passed TaskFlow.Api.Tests.ApiTests.Login_WithWrongPassword_Returns401
Passed TaskFlow.Api.Tests.ApiTests.Create_WithInvalidData_Returns400(json: "{\"title\":\"Tarea\",\"dueDate\":\"2001-01-01\"}")
Passed TaskFlow.Api.Tests.ApiTests.Create_WithInvalidData_Returns400(json: "{\"title\":\"Tarea\",\"priority\":\"Urgente\"}")
Passed TaskFlow.Api.Tests.ApiTests.Create_WithInvalidData_Returns400(json: "{\"title\":\"ab\"}")
Passed TaskFlow.Api.Tests.ApiTests.Health_ReportsHealthy
Passed TaskFlow.Api.Tests.ApiTests.Tasks_WithoutToken_Returns401
Passed TaskFlow.Api.Tests.ApiTests.Get_Missing_Returns404WithProblemDetails
Passed TaskFlow.Api.Tests.ApiTests.Delete_AsNormalUser_Returns403
Passed TaskFlow.Api.Tests.ApiTests.Create_ThenGet_RoundTripsThroughPostgres
```

```text
$ dotnet test   (toda la solución)
Passed!  - Failed:     0, Passed:    22, Total:    22,  - TaskFlow.Tests.dll
Passed!  - Failed:     0, Passed:    18, Total:    18,  - TaskFlow.Api.Tests.dll
Total de pruebas que pasan: 40
```

### Código 3: publicar, ejecutar como en producción y migrar

```text
$ dotnet publish src/TaskFlow.Api -c Release -o publish    (lo mismo que hace la etapa «build» del Dockerfile)
  TaskFlow.Api -> /tmp/claude-0/-home-user-claude/ddd43a70-8f87-5490-9d3f-755ecbba3013/scratchpad/curso4/TaskFlow/src/TaskFlow.Api/bin/Release/net10.0/TaskFlow.Api.dll
  TaskFlow.Api -> /tmp/tf-publish/

$ ls publish | grep -v -E "\\.(pdb|xml)$" | head -12
Microsoft.AspNetCore.Authentication.JwtBearer.dll
Microsoft.AspNetCore.OpenApi.dll
Microsoft.Bcl.Cryptography.dll
Microsoft.EntityFrameworkCore.Abstractions.dll
Microsoft.EntityFrameworkCore.Relational.dll
Microsoft.EntityFrameworkCore.dll
Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore.dll
Microsoft.IdentityModel.Abstractions.dll
Microsoft.IdentityModel.JsonWebTokens.dll
Microsoft.IdentityModel.Logging.dll
Microsoft.IdentityModel.Protocols.OpenIdConnect.dll
Microsoft.IdentityModel.Protocols.dll

$ du -sh publish
9.0M

$ ... y se ejecuta como lo haría el contenedor: Production + variables de entorno
$ curl localhost:5120/health
Healthy
$ curl -o /dev/null -w "%{http_code}" localhost:5120/openapi/v1.json   (en Production la documentación NO se expone)
404
$ curl -o /dev/null -w "%{http_code}" localhost:5120/tasks   (sigue protegido)
401

$ primeras líneas del log de Production
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5120
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Production

--- Sin los secretos (cadena de conexión, Jwt:Key) la aplicación NO arranca: falla rápido, a propósito ---
$ dotnet TaskFlow.Api.dll   (sin secretos)
Unhandled exception. System.InvalidOperationException: Falta ConnectionStrings:TaskFlow (usa user-secrets o una variable de entorno).
   at TaskFlow.Infrastructure.DependencyInjection.AddInfrastructure(IServiceCollection services, IConfiguration config) in /tmp/claude-0/-home-user-claude/ddd43a70-8f87-5
```

**Qué observar:**

- `dotnet publish` produce una carpeta autosuficiente de **9 MB** (la API y sus paquetes). Es **exactamente lo que hace la etapa `build` del Dockerfile**.
- Ejecutada con `ASPNETCORE_ENVIRONMENT=Production` y variables de entorno: `/health` responde `Healthy`; `/openapi/v1.json` da **404** (la documentación no se expone en producción); `/tasks` sigue dando **401**.
- Sin secretos **la aplicación se niega a arrancar** con un mensaje claro: *falla rápido*.

**Migrar en producción con un *bundle*:**

```text
$ dotnet ef migrations bundle --self-contained -r linux-x64 -o /tmp/efbundle   (un ejecutable que aplica las migraciones; útil en CI/CD sin instalar dotnet-ef)
Done. Migrations Bundle: /tmp/efbundle
Don't forget to copy appsettings.json alongside your bundle if you need it to apply migrations.
tamaño del bundle: 128 MB

$ /tmp/efbundle --connection "..."  (contra una base NUEVA taskflow_bundle)
Applying migration '20261029175255_InitialCreate'.
Applying migration '20261029181438_AddUserCreatedAt'.
Done.

$ psql taskflow_bundle -c "\dt"
                 List of relations
 Schema |         Name          | Type  |  Owner   
--------+-----------------------+-------+----------
 public | __EFMigrationsHistory | table | taskflow
 public | tasks                 | table | taskflow
 public | users                 | table | taskflow
(3 rows)
```

`dotnet ef migrations bundle` genera **un ejecutable** que aplica las migraciones pendientes: se ejecuta en el paso de despliegue **sin instalar `dotnet-ef`** ni el SDK, y **una sola vez** (no desde cada instancia al arrancar). Pesa 128 MB porque es autocontenido. Aquí se aplicó a una base nueva y creó las tres tablas.

### Docker y CI: escritos, **no ejecutados**

No hay demonio Docker en este entorno, por lo que **la imagen nunca se construyó ni se ejecutó**. Se validó solo que los dos archivos YAML tengan sintaxis correcta. Lo que sí se probó es cada paso que ejecutaría (publicar, ejecutar en Production, migrar).

`Dockerfile` (dos etapas: compilar con el SDK, ejecutar con una imagen pequeña)

```text
# ---------- Etapa 1: compilar y publicar (imagen grande con el SDK) ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
# Primero solo los .csproj: así Docker reutiliza la capa de "restore" mientras no cambien las dependencias
COPY src/TaskFlow.Core/TaskFlow.Core.csproj                       src/TaskFlow.Core/
COPY src/TaskFlow.Infrastructure/TaskFlow.Infrastructure.csproj   src/TaskFlow.Infrastructure/
COPY src/TaskFlow.Api/TaskFlow.Api.csproj                         src/TaskFlow.Api/
RUN dotnet restore src/TaskFlow.Api/TaskFlow.Api.csproj
COPY src/ src/
RUN dotnet publish src/TaskFlow.Api/TaskFlow.Api.csproj -c Release -o /app --no-restore

# ---------- Etapa 2: imagen final pequeña (solo el runtime de ASP.NET) ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app .
# No ejecutar como root: las imágenes oficiales traen el usuario "app"
USER app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "TaskFlow.Api.dll"]
```

`docker-compose.yml` (API + PostgreSQL)

```yaml
# Arranque local: docker compose up --build
services:
  db:
    image: postgres:16
    environment:
      POSTGRES_USER: taskflow
      POSTGRES_PASSWORD: ${DB_PASSWORD:?define DB_PASSWORD en un archivo .env (no se sube a Git)}
      POSTGRES_DB: taskflow
    volumes:
      - dbdata:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U taskflow -d taskflow"]
      interval: 5s
      timeout: 3s
      retries: 10

  api:
    build: .
    depends_on:
      db:
        condition: service_healthy      # espera a que PostgreSQL responda
    environment:
      ASPNETCORE_ENVIRONMENT: Production
      # "__" separa niveles de la configuración: ConnectionStrings:TaskFlow
      ConnectionStrings__TaskFlow: Host=db;Database=taskflow;Username=taskflow;Password=${DB_PASSWORD}
      Jwt__Key: ${JWT_KEY:?define JWT_KEY (mínimo 32 caracteres) en el archivo .env}
    ports:
      - "8080:8080"

volumes:
  dbdata:
```

`.github/workflows/ci.yml` (compilar, probar con PostgreSQL real y revisar vulnerabilidades)

```yaml
name: CI
on:
  push:
    branches: [main]
  pull_request:

jobs:
  build-test:
    runs-on: ubuntu-latest
    services:
      postgres:                              # una base PostgreSQL real para las pruebas de integración
        image: postgres:16
        env:
          POSTGRES_USER: taskflow
          POSTGRES_PASSWORD: taskflow_dev
          POSTGRES_DB: taskflow_test
        ports: ["5432:5432"]
        options: >-
          --health-cmd "pg_isready -U taskflow" --health-interval 5s --health-timeout 3s --health-retries 10
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: 10.0.x
      - run: dotnet restore
      - run: dotnet build --no-restore -c Release
      - run: dotnet test --no-build -c Release --collect:"XPlat Code Coverage"
      - name: Revisar paquetes con vulnerabilidades conocidas
        run: dotnet list package --vulnerable --include-transitive
      - uses: actions/upload-artifact@v4
        if: always()
        with:
          name: cobertura
          path: "**/coverage.cobertura.xml"
```

**Por qué está escrito así:**

- **Dos etapas:** la imagen final no lleva el SDK (cientos de MB) ni el código fuente.
- **Copiar primero los `.csproj`** y restaurar: Docker guarda en caché esa capa y **no reinstala los paquetes** mientras no cambien las dependencias.
- **`USER app`:** el proceso **no corre como root**. Si lo comprometen, tiene menos poder.
- **Contraseñas con `${DB_PASSWORD:?…}`:** el compose **se niega a arrancar** si faltan; se leen de un `.env` que **está en `.gitignore`**.
- **`depends_on` con `service_healthy`:** la API espera a que PostgreSQL esté listo, no solo a que su contenedor haya arrancado.
- **CI:** compila en Release, ejecuta las pruebas con un PostgreSQL real como servicio y revisa `dotnet list package --vulnerable`.

> **También posible (no probado):** `dotnet publish /t:PublishContainer` construye la imagen directamente desde .NET, sin Dockerfile, pero necesita Docker o un registro.

### Errores típicos

| Síntoma | Causa | Solución |
|---|---|---|
| Funciona en local y falla desplegada | Configuración distinta, entorno por defecto `Production` | Revisa variables de entorno y `ASPNETCORE_ENVIRONMENT` |
| Las pruebas de integración se pisan | Estado compartido en la base | Datos únicos por prueba, o base limpia por ejecución |
| Las pruebas usan la base de desarrollo | Entorno `Development` carga los secretos | Entorno `Testing` + base propia |
| Migraciones aplicadas por varias instancias a la vez | `Migrate()` al arrancar con réplicas | *Bundle* en el despliegue |
| Contraseñas en `docker-compose.yml` | Comodidad | `.env` fuera de Git o secretos del orquestador |
| Imagen enorme | Una sola etapa con el SDK | Multi-etapa |
| Documentación de la API expuesta en producción | `MapOpenApi()` sin condición | Solo en `Development` (o protégela) |

> **Ejercicio 50.1:** añade una configuración tipada `Seguridad:IntentosMaximos` con rango 1–10 y `ValidateOnStart`, y comprueba que un valor 0 impide arrancar.
>
> **Ejercicio 50.2:** escribe una prueba de integración de `PUT /tasks/{id}` y otra de `POST /tasks/{id}/done`.
>
> **Ejercicio 50.3:** si tienes Docker, construye la imagen y anota qué falló a la primera: este texto no lo pudo verificar por ti.

---

## Resumen de la Parte VII

| Si quieres… | Usa… |
|---|---|
| Entender una petición | Método, ruta, cabeceras, código de estado |
| Meter lógica transversal | Middleware (el **orden** importa) |
| Rutas sin ceremonia | Minimal API (o controladores si prefieres MVC) |
| Frontera estable de la API | DTO + validación + `ProblemDetails` |
| Guardar en PostgreSQL | EF Core: `DbContext`, migraciones |
| Ver qué SQL se genera | `ToQueryString()` / interceptor |
| Evitar consultas en bucle | `Include` o proyección |
| Detectar ediciones simultáneas | Concurrencia optimista (`xmin`) |
| Nunca montar SQL con texto del usuario | `FromSql` / parámetros |
| Guardar contraseñas | Hash lento con sal (o ASP.NET Core Identity) |
| Identificar peticiones | JWT con caducidad corta, por HTTPS |
| Secretos | user-secrets (desarrollo), entorno / gestor (producción) |
| Detectar configuración mala | `ValidateOnStart` |
| Probar la aplicación entera | `WebApplicationFactory` + base de pruebas |
| Que el diseño no se degrade | Pruebas de arquitectura |
| Migrar en producción | *Migration bundle* |

**Sigue con la Parte VIII:** una interfaz web para TaskFlow con **Blazor**, consumiendo esta API.
