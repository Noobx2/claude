# C# (.NET)

C# es un lenguaje **estático y orientado a objetos** de Microsoft, muy completo y moderno. Se ejecuta sobre .NET (multiplataforma). Se usa en APIs web (ASP.NET Core), aplicaciones de escritorio (WPF, WinForms), videojuegos (Unity), servicios y herramientas. Visual Studio, que ya tienes en tu equipo anfitrión, es su entorno natural.

*Verificado: 2026-10-27. Todos los ejemplos se han compilado y ejecutado con el SDK de .NET 10.0.113 en Linux (xUnit 2.9, ASP.NET Core 10). En tu VM Ubuntu **no hay** `dotnet`; este módulo funciona en tu anfitrión con Visual Studio o tras instalar el SDK en la VM. Referencia: [learn.microsoft.com/dotnet/csharp](https://learn.microsoft.com/es-es/dotnet/csharp/).*

## 1. Para qué sirve

| Uso | Ejemplo |
|---|---|
| APIs y servicios web | ASP.NET Core |
| Aplicaciones de escritorio de Windows | WPF, WinForms |
| Videojuegos | Unity |
| Control de hardware en Windows | Puerto serie, USB, bibliotecas del fabricante (ver [Hardware](#hardware)) |
| Herramientas y automatización | Aplicaciones de consola |

## 2. Instalación y versiones

- **En tu anfitrión**: Visual Studio incluye el SDK de .NET; el instalador permite elegir la carga de trabajo *Desarrollo de ASP.NET y web* y *Desarrollo de escritorio de .NET*.
- **En tu VM Ubuntu** (si la necesitas): el SDK está en los repositorios de Ubuntu (`dotnet-sdk-10.0`) y en Microsoft. Antes de instalar nada, confirma el método vigente en [learn.microsoft.com/dotnet/core/install/linux](https://learn.microsoft.com/es-es/dotnet/core/install/linux).

```bash
dotnet --version
dotnet --list-sdks
dotnet --info
```

| Comando | Significado |
|---|---|
| `dotnet --version` | Versión del SDK activo |
| `dotnet --list-sdks` | SDK instalados |
| `dotnet --list-runtimes` | Entornos de ejecución instalados |
| `dotnet --info` | Diagnóstico completo (SO, SDK, rutas) |

SDK frente a runtime: el **SDK** compila y trae las herramientas; el **runtime** solo ejecuta. Para desarrollar necesitas el SDK.

## 3. Archivos y convenciones de nombres

| Elemento | Convención |
|---|---|
| Archivos | `PascalCase.cs`, normalmente uno por tipo |
| Clases, interfaces, records, métodos, propiedades | `PascalCase` |
| Interfaces | Prefijo `I`: `ITaskRepository` |
| Variables locales y parámetros | `camelCase` |
| Campos privados | `_camelCase` |
| Espacios de nombres | `PascalCase` (`TasksApi.Services`) |
| Proyecto | `.csproj` (manifiesto del proyecto) |
| Solución | `.sln` / `.slnx` (agrupa varios proyectos) |

## 4. Hola mundo

```bash
dotnet new console -n HelloCs -o HelloCs
cd HelloCs
dotnet run
```

```csharp
// Program.cs
Console.WriteLine("Hola mundo");
```

| Pieza | Significado |
|---|---|
| `dotnet new console` | Crea un proyecto de consola desde una plantilla |
| `-n HelloCs` | Nombre del proyecto |
| `-o HelloCs` | Carpeta de salida |
| `dotnet run` | Compila y ejecuta |
| Instrucciones de nivel superior | Desde C# 9 no hace falta `class Program` ni `Main` en programas sencillos |

En Visual Studio: *Archivo → Nuevo → Proyecto → Aplicación de consola*; **F5** ejecuta con depurador, **Ctrl+F5** sin depurador.

### Ejecutar un solo archivo (.NET 10)

```bash
dotnet run basics.cs
```

Perfecto para probar ideas sin crear un proyecto. Se pueden fijar propiedades con directivas al principio del archivo:

```csharp
#:property PublishAot=false
```

> **Ojo:** en este modo, la serialización JSON basada en reflexión puede estar desactivada y fallar con `Reflection-based serialization has been disabled`. La línea `#:property PublishAot=false` lo resuelve; en un proyecto normal (`dotnet new console`) no ocurre.

## 5. Variables, tipos y conversiones

```csharp
// basics.cs: variables, tipos, condiciones, bucles y métodos
// Se ejecuta con: dotnet run basics.cs  (archivo único, .NET 10)

const int MaxTries = 3;              // constante

string name = "Ana";                 // string
int age = 31;                        // int (32 bits)
long big = 3_000_000_000L;           // long (64 bits)
double height = 1.68;                // double
decimal price = 19.99m;              // decimal: dinero y cálculos exactos (sufijo m)
bool student = false;
char letter = 'A';
var inferred = 42;                   // var: tipo deducido (sigue siendo estático)
string? maybe = null;                // el ? permite null de forma explícita

Console.WriteLine($"{name} {age} {big} {height} {price} {student} {letter} {inferred} {MaxTries}");
Console.WriteLine($"{name} mide {height:F1} m");        // F1: un decimal
Console.WriteLine(maybe ?? "sin valor");                // ?? valor por defecto si es null
Console.WriteLine(maybe?.Length);                       // ?. no falla si es null (imprime vacío)

// Conversiones
int parsed = int.Parse("42");
bool ok = int.TryParse("abc", out int result);          // TryParse: no lanza excepción
double ratio = (double)7 / 2;
Console.WriteLine($"{parsed} {ok} {result} {ratio} {7 / 2} {7 % 2}");

// Condiciones
int score = 72;
string grade = score switch                              // switch como expresión con patrones
{
    >= 90 => "A",
    >= 70 => "B",
    _ => "C",
};
Console.WriteLine(grade);

// Bucles
for (int i = 1; i <= 3; i++)
{
    Console.WriteLine($"vuelta {i}");
}
foreach (char c in "abc")
{
    Console.Write(c);
}
Console.WriteLine();
int remaining = 3;
while (remaining > 0)
{
    remaining--;
}

Console.WriteLine(Add(2, 3) + " " + Add(5) + " " + Add(b: 1, a: 2));
Console.WriteLine(Sum(1, 2, 3));

// Local functions / métodos de nivel superior (van tras el código)
static int Add(int a, int b = 0) => a + b;               // parámetro opcional y cuerpo de expresión
static int Sum(params int[] numbers) => numbers.Sum();   // params: número variable de argumentos
```

| Tipo | Ejemplo | Notas |
|---|---|---|
| `int`, `long` | `42`, `3_000_000_000L` | 32 y 64 bits |
| `double` | `1.68` | Decimal binario aproximado |
| `decimal` | `19.99m` | Decimal exacto: **úsalo para dinero** |
| `bool` | `true` | |
| `char` | `'A'` | |
| `string` | `"texto"` | Inmutable; `$"Hola {name}"` interpola |
| `T?` | `string?`, `int?` | Admite `null` (con *nullable reference types* activos, `string` no admite `null`) |
| `var` | `var n = 42;` | El compilador deduce el tipo |
| `record` | `record P(int X, int Y);` | Clase inmutable con igualdad por valor |

## 6. Colecciones y LINQ

```csharp
// collections.cs: colecciones y LINQ con el ejemplo de empleados
using System.Linq;

var employees = new List<Employee>
{
    new(1, "Ana", "ventas", 2400, 31, true),
    new(2, "Luis", "tecnología", 3600, 28, true),
    new(3, "Marta", "tecnología", 4100, 41, false),
    new(4, "Pablo", "ventas", 2100, 24, true),
    new(5, "Sara", "recursos", 2800, 36, true),
    new(6, "Iván", "tecnología", 3300, 33, true),
};

// LINQ: consultas sobre colecciones (Where = filter, Select = map, Aggregate = reduce)
var active = employees.Where(e => e.Active).Select(e => e.Name).ToList();
Console.WriteLine(string.Join(", ", active));

int total = employees.Sum(e => e.Salary);
Console.WriteLine($"{total} {employees.Average(e => e.Salary)}");

var top3 = employees.OrderByDescending(e => e.Salary).Take(3).Select(e => e.Name);
Console.WriteLine(string.Join(", ", top3));

Console.WriteLine(employees.FirstOrDefault(e => e.Age > 40)?.Name ?? "nadie");
Console.WriteLine($"{employees.All(e => e.Salary > 2000)} {employees.Any(e => !e.Active)}");

// Agrupar
var payroll = employees
    .GroupBy(e => e.Department)
    .OrderBy(g => g.Key)
    .ToDictionary(g => g.Key, g => g.Sum(e => e.Salary));
foreach (var (department, amount) in payroll)
{
    Console.WriteLine($"{department}: {amount}");
}

// Colecciones clásicas
var numbers = new List<int> { 3, 1, 4, 1, 5, 9, 2, 6, 5, 3, 5 };
var unique = numbers.Distinct().Order().ToList();
var frequency = numbers.GroupBy(n => n).OrderBy(g => g.Key).ToDictionary(g => g.Key, g => g.Count());
Console.WriteLine($"{string.Join(",", unique)} {numbers.Max()} {numbers.Min()}");
Console.WriteLine(string.Join(" ", frequency.Select(p => $"{p.Key}:{p.Value}")));

var ages = new Dictionary<string, int> { ["Ana"] = 31, ["Luis"] = 28 };
ages["Sara"] = 36;
if (ages.TryGetValue("Pedro", out var pedroAge)) Console.WriteLine(pedroAge);
else Console.WriteLine("Pedro no está");

// record: clase inmutable con igualdad por valor, ToString y desconstrucción
record Employee(int Id, string Name, string Department, int Salary, int Age, bool Active);
```

| Operación | LINQ |
|---|---|
| Filtrar | `.Where(e => …)` |
| Transformar | `.Select(e => …)` |
| Sumar / media | `.Sum(…)`, `.Average(…)` |
| Ordenar | `.OrderBy(…)`, `.OrderByDescending(…)`, `.ThenBy(…)` |
| Alguno / todos | `.Any(…)`, `.All(…)` |
| Primero | `.First(…)` (falla si no hay), `.FirstOrDefault(…)` |
| Agrupar | `.GroupBy(e => e.Department)` |
| Quitar repetidos | `.Distinct()` |
| Acumular | `.Aggregate(…)` |
| Materializar | `.ToList()`, `.ToArray()`, `.ToDictionary(…)` |

Las consultas LINQ son **diferidas**: no se ejecutan hasta que las recorres o llamas a `ToList()`.

## 7. Clases, interfaces y excepciones

```csharp
#:property PublishAot=false
// classes.cs: clases, herencia, interfaces, excepciones y archivos
class InsufficientFundsException(string message) : Exception(message);

interface IShape
{
    double Area();
}

record Rectangle(double Width, double Height) : IShape
{
    public double Area() => Width * Height;
}

record Circle(double Radius) : IShape
{
    public double Area() => Math.PI * Radius * Radius;
}

class Account
{
    public string Owner { get; }                    // propiedad de solo lectura
    public decimal Balance { get; protected set; }  // solo la clase y sus derivadas pueden cambiarla

    public Account(string owner, decimal balance = 0)
    {
        Owner = owner;
        Balance = balance;
    }

    public void Deposit(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("El importe debe ser positivo", nameof(amount));
        Balance += amount;
    }

    public virtual void Withdraw(decimal amount)    // virtual: las derivadas pueden redefinirlo
    {
        if (amount > Balance) throw new InsufficientFundsException($"Saldo insuficiente: {Balance}");
        Balance -= amount;
    }

    public override string ToString() => $"{Owner}: {Balance:F2} €";
}

class SavingsAccount : Account                      // herencia
{
    public decimal Rate { get; }
    public SavingsAccount(string owner, decimal balance, decimal rate) : base(owner, balance) => Rate = rate;
    public void AddInterest() => Deposit(Balance * Rate);
}

class Program
{
    static void Main()
    {
        var account = new SavingsAccount("Ana", 1000m, 0.02m);
        account.Deposit(500m);
        account.AddInterest();
        Console.WriteLine(account);

        try
        {
            account.Withdraw(5000m);
        }
        catch (InsufficientFundsException e)
        {
            Console.WriteLine($"No se pudo: {e.Message}");
        }
        finally
        {
            Console.WriteLine("finally se ejecuta siempre");
        }

        IShape[] shapes = [new Rectangle(3, 4), new Circle(1)];
        foreach (var shape in shapes)
        {
            Console.WriteLine($"{shape.GetType().Name} {shape.Area():F2}");
        }

        // Archivos y JSON
        var path = "demo.json";
        var task = new TaskItem(1, "Estudiar C#", false);
        File.WriteAllText(path, System.Text.Json.JsonSerializer.Serialize(task));
        var text = File.ReadAllText(path);
        Console.WriteLine(text);
        var back = System.Text.Json.JsonSerializer.Deserialize<TaskItem>(text);
        Console.WriteLine(back);
        File.Delete(path);

        if (!int.TryParse("abc", out _)) Console.WriteLine("no es un número");
    }
}

record TaskItem(int Id, string Title, bool Done);
```

- Las **propiedades** (`{ get; set; }`) sustituyen a los getters/setters manuales.
- Modificadores de acceso: `public`, `private`, `protected`, `internal` (visible en el mismo proyecto).
- `virtual` / `override` permiten redefinir comportamiento en clases derivadas.
- Constructores principales (`class X(string message)`) y colecciones con `[...]` son novedades recientes del lenguaje.
- `try / catch / finally` funciona como en Java; `throw` lanza; `using` cierra recursos (`IDisposable`).
- Usa `int.TryParse` en lugar de `int.Parse` cuando la entrada puede ser inválida.

## 8. Asincronía y concurrencia

```csharp
// async.cs: asincronía con async/await y Task
using System.Diagnostics;

async Task<string> FetchAsync(string name, int milliseconds)
{
    await Task.Delay(milliseconds);              // no bloquea el hilo mientras espera
    return $"{name} listo";
}

var watch = Stopwatch.StartNew();
string[] results = await Task.WhenAll(FetchAsync("a", 200), FetchAsync("b", 200), FetchAsync("c", 200));
Console.WriteLine($"{string.Join(", ", results)} {watch.ElapsedMilliseconds / 100}00 ms aprox.");

// Hilos y bloqueo
var counter = 0;
var gate = new object();
Parallel.For(0, 100, _ =>
{
    lock (gate) { counter++; }                   // lock: acceso exclusivo
});
Console.WriteLine($"contador: {counter}");
```

| Concepto | Significado |
|---|---|
| `async` / `await` | Esperar sin bloquear el hilo |
| `Task<T>` | Una operación asíncrona que devolverá `T` |
| `Task.WhenAll(…)` | Esperar varias en paralelo |
| `lock` | Acceso exclusivo a un recurso compartido |
| `Parallel.For` | Reparte iteraciones entre hilos |
| `CancellationToken` | Cancelar operaciones largas |

> **Regla:** nunca uses `.Result` ni `.Wait()` en código asíncrono: pueden bloquear el programa. Propaga `async` hasta arriba.

## 9. Proyectos, soluciones y NuGet

```bash
dotnet new sln -n TasksApp
dotnet new web -n TasksApi -o src/TasksApi
dotnet new xunit -n TasksApi.Tests -o tests/TasksApi.Tests
dotnet sln add src/TasksApi tests/TasksApi.Tests
dotnet add tests/TasksApi.Tests reference src/TasksApi
dotnet add tests/TasksApi.Tests package Microsoft.AspNetCore.Mvc.Testing
```

| Comando | Significado |
|---|---|
| `dotnet new <plantilla>` | Crea desde plantilla (`console`, `web`, `webapi`, `classlib`, `xunit`, `sln`; `dotnet new list` las muestra) |
| `dotnet sln add ruta` | Añade proyectos a la solución |
| `dotnet add A reference B` | El proyecto A depende del proyecto B |
| `dotnet add package X` | Añade una dependencia de **NuGet** (el «npm» de .NET); `--version` fija versión |
| `dotnet remove package X` | Quita la dependencia |
| `dotnet restore` | Descarga las dependencias (lo hacen solos `build` y `run`) |
| `dotnet build` | Compila (`-c Release` en modo Release) |
| `dotnet run` | Compila y ejecuta; `--project ruta` elige proyecto; `-- args` pasa argumentos al programa |
| `dotnet test` | Ejecuta las pruebas |
| `dotnet publish -c Release -o out` | Genera la versión para desplegar en la carpeta `out` |
| `dotnet format` | Formatea el código |

### El archivo `.csproj`

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

</Project>
```

| Elemento | Significado |
|---|---|
| `Sdk="Microsoft.NET.Sdk.Web"` | Tipo de proyecto: `Microsoft.NET.Sdk` (consola, librería) o `.Web` (ASP.NET Core) |
| `TargetFramework` | Versión de .NET (`net10.0`) |
| `Nullable` | Activa el análisis de `null` |
| `ImplicitUsings` | Importa automáticamente los espacios de nombres habituales |
| `PackageReference` | Dependencia NuGet con versión |
| `ProjectReference` | Dependencia de otro proyecto |

| Archivo | Qué es | ¿Git? |
|---|---|---|
| `*.csproj` | **Manifiesto**: framework, dependencias | Sí |
| `*.sln` / `*.slnx` | Lista de proyectos | Sí |
| `packages.lock.json` | **Bloqueo** (opcional; se activa con `RestorePackagesWithLockFile`) | Sí si existe |
| `obj/`, `bin/` | Generados | **No** |
| `appsettings.json` | Configuración de la aplicación web | Sí (sin secretos) |
| `Properties/launchSettings.json` | Perfiles de ejecución en desarrollo (puertos) | Sí |

## 10. Una API REST con ASP.NET Core

Estructura:

```text
TasksApp/
  TasksApp.slnx
  src/TasksApi/
    TasksApi.csproj
    Program.cs               configuración, servicios, rutas
    Models.cs                entidad y DTOs
    TaskRepository.cs        contrato + implementación en memoria
    TaskService.cs           reglas de negocio
    appsettings.json
  tests/TasksApi.Tests/
    TasksApi.Tests.csproj
    TasksApiTests.cs
```

```csharp
namespace TasksApi;

/// <summary>Entidad. Un record es una clase inmutable con igualdad por valor.</summary>
public record TaskItem(string Id, string Title, bool Done);

/// <summary>DTO de entrada para crear.</summary>
public record CreateTaskRequest(string? Title);

/// <summary>Cambios parciales: un campo null significa "no modificar".</summary>
public record UpdateTaskRequest(string? Title, bool? Done);
```

```csharp
namespace TasksApi;

/// <summary>Contrato de acceso a datos. El servicio depende de la interfaz, no de la implementación.</summary>
public interface ITaskRepository
{
    IReadOnlyList<TaskItem> FindAll();
    TaskItem? FindById(string id);
    TaskItem Save(TaskItem task);
    bool Delete(string id);
    string NextId();
}

public class InMemoryTaskRepository : ITaskRepository
{
    private readonly List<TaskItem> _tasks = [];
    private readonly object _gate = new();
    private int _lastId;

    public IReadOnlyList<TaskItem> FindAll()
    {
        lock (_gate) { return _tasks.ToList(); }
    }

    public TaskItem? FindById(string id)
    {
        lock (_gate) { return _tasks.FirstOrDefault(t => t.Id == id); }
    }

    public TaskItem Save(TaskItem task)
    {
        lock (_gate)
        {
            var index = _tasks.FindIndex(t => t.Id == task.Id);
            if (index >= 0) _tasks[index] = task; else _tasks.Add(task);
            return task;
        }
    }

    public bool Delete(string id)
    {
        lock (_gate) { return _tasks.RemoveAll(t => t.Id == id) > 0; }
    }

    public string NextId() => Interlocked.Increment(ref _lastId).ToString();
}
```

```csharp
namespace TasksApi;

public class NotFoundException(string message) : Exception(message);
public class ValidationException(string message) : Exception(message);

/// <summary>Reglas de negocio. No conoce HTTP.</summary>
public class TaskService(ITaskRepository repository)   // constructor principal: inyección de dependencias
{
    private static string ValidateTitle(string? title)
    {
        var trimmed = title?.Trim() ?? "";
        if (trimmed.Length < 2) throw new ValidationException("title debe tener al menos 2 caracteres");
        return trimmed;
    }

    public IReadOnlyList<TaskItem> List() => repository.FindAll();

    public TaskItem Get(string id) =>
        repository.FindById(id) ?? throw new NotFoundException($"No existe la tarea {id}");

    public TaskItem Create(CreateTaskRequest request) =>
        repository.Save(new TaskItem(repository.NextId(), ValidateTitle(request.Title), false));

    public TaskItem Update(string id, UpdateTaskRequest request)
    {
        var task = Get(id);
        if (request.Title is not null) task = task with { Title = ValidateTitle(request.Title) };
        if (request.Done is not null) task = task with { Done = request.Done.Value };   // "with": copia modificada
        return repository.Save(task);
    }

    public void Delete(string id)
    {
        if (!repository.Delete(id)) throw new NotFoundException($"No existe la tarea {id}");
    }
}
```

```csharp
using TasksApi;

var builder = WebApplication.CreateBuilder(args);

// Inyección de dependencias: se registran los servicios y .NET los entrega donde se pidan
builder.Services.AddSingleton<ITaskRepository, InMemoryTaskRepository>();
builder.Services.AddSingleton<TaskService>();

var corsOrigin = builder.Configuration["CORS_ORIGIN"] ?? "http://localhost:5173";
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy => policy
        .WithOrigins(corsOrigin)
        .WithMethods("GET", "POST", "PATCH", "DELETE")
        .WithHeaders("Content-Type")));

var app = builder.Build();
app.UseCors();

// Un único sitio donde las excepciones del negocio se convierten en respuestas HTTP
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (NotFoundException e)
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;
        await context.Response.WriteAsJsonAsync(new { error = e.Message });
    }
    catch (ValidationException e)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(new { error = e.Message });
    }
});

var tasks = app.MapGroup("/api/tasks");

tasks.MapGet("/", (TaskService service) => service.List());
tasks.MapGet("/{id}", (string id, TaskService service) => service.Get(id));
tasks.MapPost("/", (CreateTaskRequest request, TaskService service) =>
    Results.Created("/api/tasks", service.Create(request)));
tasks.MapPatch("/{id}", (string id, UpdateTaskRequest request, TaskService service) =>
    service.Update(id, request));
tasks.MapDelete("/{id}", (string id, TaskService service) =>
{
    service.Delete(id);
    return Results.NoContent();
});

app.Run();

// Permite que el proyecto de pruebas cree la aplicación con WebApplicationFactory<Program>
public partial class Program;
```

| Pieza | Significado |
|---|---|
| `builder.Services.AddSingleton<I, C>()` | Registra una dependencia en el contenedor de **inyección de dependencias** (una sola instancia). `AddScoped` (una por petición) y `AddTransient` (una por uso) son las alternativas |
| Constructor principal `TaskService(ITaskRepository repository)` | .NET entrega el repositorio registrado |
| `app.MapGroup("/api/tasks")` | Agrupa rutas bajo un prefijo |
| `MapGet/MapPost/MapPatch/MapDelete` | Rutas **Minimal API**; los parámetros del delegado se rellenan solos (ruta, cuerpo JSON, servicios) |
| `Results.Created(…)`, `Results.NoContent()` | Códigos `201` y `204` |
| Middleware con `try/catch` | Convierte excepciones del negocio en `404` y `400` |
| `AddCors` / `UseCors` | CORS con métodos enumerados |
| `builder.Configuration["CORS_ORIGIN"]` | Lee configuración de variables de entorno, `appsettings.json` o argumentos |
| `public partial class Program;` | Permite a las pruebas arrancar la aplicación |

La alternativa a Minimal API son los **controladores** (`[ApiController]`, clases `TasksController`), más estructurados para APIs grandes. Esta API es **intercambiable** con las de [Node.js](#node), [Python](#python), [Go](#go), [Rust](#rust) y [Spring Boot](#spring): mismas rutas y mismo JSON.

Ejecutar:

```bash
dotnet run --project src/TasksApi --urls http://localhost:3000
```

| Pieza | Significado |
|---|---|
| `--project` | Qué proyecto ejecutar de la solución |
| `--urls` | Dirección y puerto en que escucha (también `ASPNETCORE_URLS`) |
| `ASPNETCORE_ENVIRONMENT=Development` | Entorno; activa `appsettings.Development.json` |

**Configuración y secretos**: variables de entorno > `appsettings.{Entorno}.json` > `appsettings.json`. Los secretos en desarrollo van a *User Secrets* (`dotnet user-secrets set "Clave" "valor"`), nunca al repositorio.

## 11. Pruebas con xUnit

```csharp
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TasksApi;

namespace TasksApi.Tests;

// WebApplicationFactory arranca la aplicación completa en memoria
public class TasksApiTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task CrudFlow()
    {
        var client = factory.CreateClient();

        var created = await client.PostAsJsonAsync("/api/tasks", new { title = "Estudiar C#" });
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var task = await created.Content.ReadFromJsonAsync<TaskItem>();
        Assert.Equal("Estudiar C#", task!.Title);

        var invalid = await client.PostAsJsonAsync("/api/tasks", new { title = "x" });
        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);

        var patched = await client.PatchAsJsonAsync($"/api/tasks/{task.Id}", new { done = true });
        Assert.True((await patched.Content.ReadFromJsonAsync<TaskItem>())!.Done);

        var deleted = await client.DeleteAsync($"/api/tasks/{task.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleted.StatusCode);

        var missing = await client.GetAsync($"/api/tasks/{task.Id}");
        Assert.Equal(HttpStatusCode.NotFound, missing.StatusCode);
    }

    [Theory]                       // Theory + InlineData: la misma prueba con varios datos
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("a")]
    public async Task RejectsShortTitles(string title)
    {
        var client = factory.CreateClient();
        var response = await client.PostAsJsonAsync("/api/tasks", new { title });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
```

```bash
dotnet test
dotnet test --filter "FullyQualifiedName~CrudFlow"
dotnet test -v n
```

| Pieza | Significado |
|---|---|
| `[Fact]` | Una prueba |
| `[Theory]` + `[InlineData(…)]` | La misma prueba con varios datos |
| `Assert.Equal(esperado, real)` | Comparación (esperado primero) |
| `WebApplicationFactory<Program>` | Arranca la aplicación en memoria y crea un `HttpClient` |
| `--filter` | Ejecuta solo las pruebas cuyo nombre encaje |
| `-v n` | Verbosidad normal (detallado: `d`) |

Depuración: puntos de interrupción y **F5** en Visual Studio; en VS Code, la extensión *C# Dev Kit*.

## 12. Compilar y empaquetar

```bash
dotnet publish src/TasksApi -c Release -o out
dotnet out/TasksApi.dll --urls http://localhost:3000
```

Opciones útiles: `--self-contained -r linux-x64` (incluye el runtime), `-p:PublishSingleFile=true`. Imagen Docker: usa `mcr.microsoft.com/dotnet/sdk:10.0` para compilar y `mcr.microsoft.com/dotnet/aspnet:10.0` para ejecutar.

## 13. Errores habituales

| Mensaje | Causa | Solución |
|---|---|---|
| `The type or namespace name 'X' could not be found` | Falta `using` o paquete | Añade el `using` o `dotnet add package` |
| `CS8602: Dereference of a possibly null reference` | Análisis de `null` | Comprueba `null` o usa `?.` / `??` |
| `NullReferenceException` | Usas una referencia nula | Depura; comprueba el origen |
| `FormatException` al convertir | `int.Parse` con texto inválido | `int.TryParse` |
| `Reflection-based serialization has been disabled` | Modo de archivo único con JSON | `#:property PublishAot=false` o proyecto normal |
| `Unable to resolve service for type …` | Falta registrar la dependencia | `builder.Services.AddSingleton<…>()` |
| `Failed to bind to address … address already in use` | Puerto ocupado | `ss -ltnp \| grep 3000` o cambia `--urls` |
| Error de CORS con `PATCH`/`DELETE` | Métodos no permitidos | `.WithMethods(…)` |
| `No project was found` | Estás fuera de la carpeta del proyecto | `cd` o `--project` |
| El SDK no es el esperado | `global.json` fija otra versión | `dotnet --list-sdks`; revisa `global.json` |

## 14. Ejercicios

> **Ejercicio:** resuelve los ejercicios 1–15 de [lógica y programación](#ejercicios-basicos) en C# y comprueba cada uno con xUnit (`[Theory]`).

> **Ejercicio:** reproduce el análisis de empleados con LINQ; compara con la versión de Python y con la de Java.

> **Ejercicio:** añade `GET /api/tasks?done=true` leyendo un parámetro de consulta (`bool? done`), con prueba.

> **Ejercicio:** escribe una aplicación de consola con argumentos `--name` y `--times` y ejecútala desde Visual Studio (argumentos de depuración) y desde la terminal.

> **Ejercicio:** cambia `InMemoryTaskRepository` por una implementación que guarde un archivo JSON y registra la nueva con una línea en `Program.cs`.

## 15. Documentación oficial

- [Tour de C#](https://learn.microsoft.com/es-es/dotnet/csharp/tour-of-csharp/)
- [Guía de .NET](https://learn.microsoft.com/es-es/dotnet/)
- [ASP.NET Core](https://learn.microsoft.com/es-es/aspnet/core/)
- [NuGet](https://www.nuget.org/)
- [xUnit](https://xunit.net/)
