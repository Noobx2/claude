# Curso de C# · Parte VIII: interfaces web con Blazor

Hasta ahora la API devolvía JSON y se probaba con `curl`. Aquí se construye **la interfaz**: una aplicación **Blazor WebAssembly** que consume la API de TaskFlow, con inicio de sesión, formularios validados y un CRUD completo. **Todo en C#**: sin escribir JavaScript para la lógica.

*Verificado: 2026-10-29 con .NET SDK 10.0.113. La aplicación se ejecutó en **Chromium real controlado por Playwright**: 17 pasos de extremo a extremo (incluida una vista de móvil) y la API con PostgreSQL detrás. **No se ejecutó** en Visual Studio, ni en Safari/Firefox, ni se hicieron pruebas de accesibilidad con un lector de pantalla.*

## Mapa de la Parte VIII

| Lección | Tema | Lo verás ejecutado |
|---|---|---|
| 51 | Fundamentos de Blazor: componentes, enlace de datos, eventos, formularios, modelos de ejecución | Blazor Server y WebAssembly medidos, **Hot Reload** en vivo |
| 52 | TaskFlow.Web: el CRUD completo contra la API | Recorrido de 17 pasos en un navegador, y 3 defectos reales encontrados |

---

## Lección 51 · Fundamentos de Blazor

### Qué vas a aprender

- Qué es **Blazor** y qué es un **componente**.
- Sintaxis **Razor**: mezclar HTML y C#.
- **Enlace de datos** (`@bind`), **eventos** (`@onclick`), **parámetros** y `EventCallback`.
- **Formularios** con validación (`EditForm`).
- **Blazor Server frente a WebAssembly** (y los modos de .NET 8+), con **medidas reales**.
- **Hot Reload**: editar código con la aplicación en marcha.

### La idea

**Blazor** es el framework de interfaz web de .NET. Una página se construye con **componentes**: pedazos reutilizables de interfaz hechos de HTML y C#, en archivos `.razor`. Un componente **puede contener otros componentes**.

```text
Tareas.razor  (página)
 ├── EditForm        (formulario de «Nueva tarea»)
 ├── input de filtros
 └── FilaTarea.razor  × N   (un componente por fila)
```

#### Dónde se ejecuta el C#

| Modelo | Dónde corre tu C# | Cómo llega al navegador | Necesita |
|---|---|---|---|
| **Blazor Server** (`InteractiveServer`) | **En el servidor** | Un clic viaja por **SignalR** (WebSocket); el servidor devuelve **solo la diferencia** del HTML | Conexión permanente |
| **Blazor WebAssembly** (`InteractiveWebAssembly`) | **En el navegador**, con el runtime de .NET compilado a **WebAssembly** | Se descarga el runtime + tus DLL la primera vez | Descarga inicial; luego puede funcionar sin servidor |
| **Auto** | Primero Server (arranque rápido) y, cuando termina la descarga, WebAssembly | Las dos cosas | Más complejidad |
| **Renderizado estático del servidor** (SSR) | Solo en el servidor, **sin interactividad** (HTML plano, como una página clásica) | HTML | Nada especial |

> **Desde .NET 8 todo esto se unificó en la «Blazor Web App»**: una sola aplicación donde cada componente puede elegir su modo de renderizado (`@rendermode`). La plantilla **«Blazor WebAssembly Standalone App»** (la que usa TaskFlow.Web) es una aplicación **solo cliente** que se sirve como archivos estáticos.

**Medido aquí** (primera carga **sin caché**, en local, sin latencia de red real):

```text
Primera carga SIN caché, en local (sin latencia de red real):
  Modelo                              Descarga  Peticiones   Listo en
  Blazor Server (servidor)               86 KB           8     152 ms
  Blazor WebAssembly (navegador)      23052 KB         211    1844 ms
```

Y el tamaño **publicado** (Release) de TaskFlow.Web, que es lo que se despliega:

```text
$ dotnet publish -c Release -o publish   (TaskFlow.Web)
Archivos de la aplicación (sin comprimir) :   59 archivos,    9.7 MB
Los mismos con Brotli (.br)               :   55 archivos,    2.9 MB   ← lo que viaja si el servidor sirve Brotli
Los mismos con gzip (.gz)                 :   55 archivos,    3.6 MB
Los más grandes:
       2931 KB  dotnet.native.3zpaws0drs.wasm
       1742 KB  System.Private.CoreLib.6p9vom0h7c.wasm
       1081 KB  icudt_no_CJK.lfu7j35m59.dat
        934 KB  icudt_CJK.tjcz0u77k5.dat
```

**Cómo se lee:**

- Server descarga **unas decenas de KB** y está listo en **~150 ms**. WebAssembly, en compilación **Debug** con el servidor de desarrollo, **descargó 23 MB en 211 peticiones**: ese número **no es lo que verían tus usuarios**.
- **Publicada en Release**, la misma aplicación pesa **9,7 MB** sin comprimir, **2,9 MB con Brotli** (el servidor tiene que servir los `.br`), con `dotnet.native.wasm` (el runtime) y `System.Private.CoreLib` como grandes. Tras la primera visita **se guarda en caché**.
- **Compromisos:**

| | Blazor Server | Blazor WebAssembly |
|---|---|---|
| Primera carga | **Muy rápida** | Más lenta (descarga el runtime) |
| Latencia por cada interacción | Un viaje de red **por cada clic** | **Ninguna** (todo es local) |
| Servidor | Mantiene **una conexión y memoria por usuario** | Solo sirve archivos |
| Si se cae la conexión | La interfaz **deja de funcionar** (aparece el diálogo de reconexión) | Sigue funcionando; solo fallan las llamadas a la API |
| Código | **Nunca llega al navegador** (más fácil proteger lógica y secretos) | **Se descarga**: todo lo que pongas en el cliente es visible |
| Acceso directo a la base de datos | **Sí** (corre en el servidor) | **No**: debe pasar por una API |

> **La regla de seguridad que más importa en WebAssembly:** el cliente es **código público**. Nunca pongas secretos, cadenas de conexión ni reglas de autorización «reales» en él. **Esconder un botón no es seguridad**; el servidor debe validar todo (se comprueba en la lección 52).

### Código: un componente de Blazor Server (`Contador.razor`)

El «hola mundo» de Blazor, ejecutado:

```xml
@page "/contador"
@rendermode InteractiveServer

<PageTitle>Contador</PageTitle>

<h1>Contador (Blazor Server)</h1>
<p role="status">Pulsaciones: <span id="cuenta">@_cuenta</span></p>
<p>Este código C# se ejecuta <b>en el servidor</b>. Hilo: <span id="hilo">@Environment.CurrentManagedThreadId</span></p>
<button id="sumar" @onclick="Sumar">Sumar</button>

@code {
    private int _cuenta;
    private void Sumar() => _cuenta++;    // el clic viaja al servidor por SignalR (WebSocket); el servidor devuelve solo la diferencia del HTML
}
```

**Cómo se lee:**

1. **`@page "/contador"`**: la ruta. Un componente con `@page` es una **página**.
2. **`@rendermode InteractiveServer`**: este componente se ejecuta en el servidor con SignalR.
3. **`@_cuenta`**: inserta el valor de una variable de C# en el HTML. `@Environment.CurrentManagedThreadId` muestra el hilo **del servidor** que atiende al usuario.
4. **`@onclick="Sumar"`**: enlaza el evento del botón con un método de C#.
5. **`@code { … }`**: el bloque de C# del componente.
6. **Al cambiar `_cuenta` Blazor repinta el componente** y envía **solo las diferencias** al navegador.

### Los conceptos de Razor, de un vistazo

| Quieres… | Escribes… | Ejemplo |
|---|---|---|
| Mostrar un valor | `@expresión` | `<p>@_cuenta</p>` |
| Condición | `@if (…) { … }` | `@if (!_listo) { <p>Cargando…</p> }` |
| Repetir | `@foreach (var t in _tareas) { … }` | filas de la tabla |
| Reaccionar a un evento | `@onclick="Metodo"` | botones |
| Enlazar un campo en dos sentidos | `@bind="_texto"` | `<input @bind="_texto" />` |
| Enlazar mientras se escribe | `@bind:event="oninput"` | búsqueda en vivo |
| Hacer algo después de enlazar | `@bind:after="Cargar"` | recargar la lista al cambiar el filtro |
| Recibir datos del padre | `[Parameter]` | `[Parameter] public Tarea Tarea { get; set; }` |
| Avisar al padre | `EventCallback<T>` | `AlCompletar.InvokeAsync(id)` |
| Inyectar un servicio | `@inject ApiClient Api` | igual que la inyección de dependencias de la Parte III |
| Identificar elementos de una lista | `@key="tarea.Id"` | evita que Blazor «confunda» filas al reordenar |

**Ciclo de vida** (los métodos que Blazor llama por ti, en orden): `SetParametersAsync` → `OnInitialized[Async]` (**una vez**, ideal para cargar datos) → `OnParametersSet[Async]` (cada vez que el padre cambia los parámetros) → pinta → `OnAfterRender[Async]` (ya hay DOM; aquí va el JavaScript interop). Y `Dispose` al destruirse (**aquí se cancelan las suscripciones**: TaskFlow.Web lo hace con `Sesion.Cambio -= …`, y sin esa línea el componente se quedaría vivo en memoria).

### Hot Reload: editar con la aplicación funcionando

`dotnet watch` observa tus archivos y **aplica los cambios en caliente**, sin reiniciar ni perder el estado. Se probó con un navegador real:

```text
1. Tras 3 clics: cuenta = 3 | título = Contador (Blazor Server)
2. Se editó el archivo .razor en disco (cambio del título)…
3. La página mostró el cambio a los 4507 ms | título = Contador EDITADO EN CALIENTE
   ¿Se recargó la página?  No (la marca de la ventana sigue ahí)
   Cuenta tras el cambio: 3 (el estado del componente se conserva si el cambio es solo de marcado)
4. Se editó también el CÓDIGO C# (Sumar suma 10). Un clic más → cuenta = 13 (3 + 10 = 13 si el cambio se aplicó sin reiniciar)
```

**Qué se ve:**

- El cambio de **marcado** (el título) apareció **en ~4,5 s, sin recargar la página** (una marca que se puso en `window` sobrevivió) y **el contador conservó su valor (3)**.
- El cambio de **código C#** (`_cuenta++` → `_cuenta += 10`) también se aplicó **sin reiniciar**: tras un clic el valor pasó de 3 a **13**.
- **Un tropiezo real al probarlo:** mi primer intento usó `--no-launch-profile` (y el entorno quedó en *Production*) y **no funcionó**: faltaba un archivo estático y la página no se actualizaba. Con el perfil de lanzamiento normal (`--launch-profile http`, entorno *Development*) funciona. **Hot Reload es una herramienta de desarrollo**; no se despliega.
- Hay **cambios que no admite** («rude edits», como cambiar la firma de una clase); entonces `dotnet watch` ofrece **reiniciar**. La lista exacta está en la documentación de Hot Reload.
- Visual Studio y VS Code tienen su propio botón de *Recarga activa* (no probado aquí).

### Formularios y validación

`EditForm` + `DataAnnotationsValidator` + `ValidationMessage` enlazan un **modelo** con sus atributos (`[Required]`, `[StringLength]`…) y muestran los errores **antes de llamar al servidor**. `OnValidSubmit` solo se ejecuta si todo es válido. **Es comodidad para el usuario, no seguridad**: el servidor **vuelve a validar** siempre (el cliente se puede saltar).

Y **dos validaciones que no se pueden hacer en el cliente**: las reglas que dependen de datos del servidor (¿ya existe ese usuario?, ¿la fecha es pasada según el reloj del servidor?). Por eso la lección 52 muestra **los errores de la API** en la propia página.

### Errores típicos

| Síntoma | Causa | Solución |
|---|---|---|
| La pantalla no se actualiza tras un cambio | Cambio desde fuera del hilo de interfaz (un evento, un temporizador) | `InvokeAsync(StateHasChanged)` |
| Fuga de memoria o actualizaciones «fantasma» | Suscripción a un evento sin desuscribir | `IDisposable` y `-=` |
| Todo se vuelve lento en Server con muchos usuarios | Memoria y CPU **por circuito** | Escala el servidor, reduce estado, o WebAssembly |
| La app WebAssembly pesa muchos MB | Se mira la compilación Debug | Mide la publicación Release, y sirve Brotli |
| `@bind` solo actualiza al salir del campo | Es `onchange` por defecto | `@bind:event="oninput"` |
| Filas que se «mezclan» al reordenar | Falta `@key` | `@key="id"` |
| Secretos en el cliente WebAssembly | Se piensa que es privado | Nunca: todo el cliente es público |

> **Ejercicio 51.1:** convierte el contador en un componente con un parámetro `Incremento` y un `EventCallback<int> AlCambiar`, y úsalo dos veces en una página.
>
> **Ejercicio 51.2:** cambia `@rendermode` a `InteractiveWebAssembly` en una Blazor Web App y compara el tiempo de carga con el de Server.

---

## Lección 52 · TaskFlow.Web: un CRUD completo contra la API

### Qué vas a aprender

- Estructurar una aplicación Blazor en **servicios, componentes y páginas**.
- **Consumir una API con `HttpClient`**, añadir el token JWT, y **traducir errores HTTP a mensajes** para personas.
- **CORS**: por qué el navegador bloquea llamadas entre orígenes y cómo permitirlas bien.
- **Probar la interfaz de extremo a extremo** con un navegador real (Playwright).
- Tres defectos **reales** que solo aparecieron al ejecutarla.

### La idea

```text
Navegador (Blazor WebAssembly)           Servidor (API)                PostgreSQL
http://localhost:5200            ──►     http://localhost:5099   ──►   base «taskflow»
 Login.razor / Tareas.razor              Minimal API + JWT + EF
 ApiClient (HttpClient + Bearer)
```

**Dos orígenes distintos:** `localhost:5200` (la interfaz) y `localhost:5099` (la API). El navegador aplica la **política del mismo origen**: una página solo puede leer respuestas de **su propio origen**, salvo que el otro servidor lo permita con cabeceras **CORS**. Por eso en `TaskFlow.Api` se añadió, en `Program.cs`:

```csharp
string[] origenesPermitidos = builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? [];
builder.Services.AddCors(o => o.AddPolicy("Web", p => p.WithOrigins(origenesPermitidos).AllowAnyHeader().AllowAnyMethod()));
// …
app.UseCors("Web");     // antes de la autenticación: las peticiones «pre-vuelo» (OPTIONS) no llevan token
```

- Los orígenes salen de la **configuración** (`appsettings.Development.json`), **nunca** `AllowAnyOrigin()` con credenciales.
- CORS **lo aplica el navegador**: `curl` o cualquier otro servidor **no se ven afectados**. **No es una medida de seguridad contra atacantes**, es una protección para los usuarios del navegador.

### Código

`Program.cs` (la raíz de composición del cliente)

```csharp
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TaskFlow.Web;
using TaskFlow.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// La URL de la API sale de wwwroot/appsettings.json (no está escrita en el código)
string apiUrl = builder.Configuration["ApiBaseUrl"] ?? throw new InvalidOperationException("Falta ApiBaseUrl en wwwroot/appsettings.json");
builder.Services.AddScoped<Sesion>();
builder.Services.AddScoped(sp => new ApiClient(new HttpClient { BaseAddress = new Uri(apiUrl) }, sp.GetRequiredService<Sesion>()));

await builder.Build().RunAsync();
```

`Services/Modelos.cs`: los modelos del cliente (su propio contrato con la API) y los modelos de formulario

```csharp
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
```

`Services/Sesion.cs`: quién está conectado

```csharp
using System.Text;
using System.Text.Json;
using Microsoft.JSInterop;

namespace TaskFlow.Web.Services;

/// <summary>
/// Quién está conectado. Guarda el token en memoria y en sessionStorage (para sobrevivir a un F5, pero no al cerrar la pestaña).
/// Aviso de seguridad: ni sessionStorage ni localStorage son inmunes a XSS; cada opción tiene su compromiso.
/// </summary>
public sealed class Sesion(IJSRuntime js)
{
    private const string Clave = "taskflow.token";
    public string? Token { get; private set; }
    public string? Usuario { get; private set; }
    public string? Rol { get; private set; }
    public bool EstaAutenticado => Token is not null;
    public bool EsAdmin => Rol == "Admin";
    public event Action? Cambio;                       // los componentes se suscriben para repintarse

    public async Task RestaurarAsync()
    {
        string? guardado = await js.InvokeAsync<string?>("sessionStorage.getItem", Clave);
        if (guardado is not null) Aplicar(guardado);
    }

    public async Task IniciarAsync(string token) { Aplicar(token); await js.InvokeVoidAsync("sessionStorage.setItem", Clave, token); }
    public async Task CerrarAsync() { Token = Usuario = Rol = null; await js.InvokeVoidAsync("sessionStorage.removeItem", Clave); Cambio?.Invoke(); }

    private void Aplicar(string token)
    {
        // Leer el contenido del JWT en el cliente SOLO sirve para mostrar el nombre o esconder botones.
        // La seguridad real la impone el servidor: aquí cualquiera podría editar esto desde las herramientas del navegador.
        string contenido = token.Split('.')[1].Replace('-', '+').Replace('_', '/');
        contenido = contenido.PadRight(contenido.Length + (4 - contenido.Length % 4) % 4, '=');
        using var doc = JsonDocument.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(contenido)));
        Token = token;
        Usuario = doc.RootElement.GetProperty("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name").GetString();
        Rol = doc.RootElement.GetProperty("http://schemas.microsoft.com/ws/2008/06/identity/claims/role").GetString();
        Cambio?.Invoke();
    }
}
```

`Services/ApiClient.cs`: toda la comunicación con la API en un solo sitio

```csharp
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
```

`Layout/MainLayout.razor`

```xml
@inherits LayoutComponentBase
@implements IDisposable
@inject Sesion Sesion
@inject NavigationManager Navegacion

<header class="barra">
    <a class="marca" href="">✔ TaskFlow</a>
    <nav>
        @if (Sesion.EstaAutenticado)
        {
            <span class="usuario" data-testid="usuario">@Sesion.Usuario <small class="rol">@Sesion.Rol</small></span>
            <button class="boton secundario" @onclick="Salir" data-testid="salir">Salir</button>
        }
        else
        {
            <NavLink href="login" class="enlace">Entrar</NavLink>
        }
    </nav>
</header>

<main class="contenido">
    @Body
</main>

@code {
    protected override async Task OnInitializedAsync()
    {
        Sesion.Cambio += AlCambiar;                   // nos enteramos cuando alguien entra o sale
        await Sesion.RestaurarAsync();
    }

    private void AlCambiar() => InvokeAsync(StateHasChanged);

    private async Task Salir()
    {
        await Sesion.CerrarAsync();
        Navegacion.NavigateTo("login");
    }

    public void Dispose() => Sesion.Cambio -= AlCambiar;   // sin esto la suscripción mantendría el componente vivo (fuga de memoria)
}
```

`Pages/Login.razor`

```xml
@page "/login"
@inject ApiClient Api
@inject Sesion Sesion
@inject NavigationManager Navegacion

<PageTitle>Entrar · TaskFlow</PageTitle>

<section class="tarjeta estrecha">
    <h1>@(_registro ? "Crear cuenta" : "Entrar")</h1>

    <EditForm Model="_modelo" OnValidSubmit="Enviar" FormName="login">
        <DataAnnotationsValidator />

        <label>Usuario
            <InputText @bind-Value="_modelo.UserName" autocomplete="username" data-testid="usuario-campo" />
        </label>
        <div class="mensaje"><ValidationMessage For="() => _modelo.UserName" /></div>

        <label>Contraseña
            <InputText type="password" @bind-Value="_modelo.Password" autocomplete="current-password" data-testid="clave-campo" />
        </label>
        <div class="mensaje"><ValidationMessage For="() => _modelo.Password" /></div>

        @if (_error is not null)
        {
            <p class="error" role="alert" data-testid="error">@_error</p>
        }

        <button class="boton" type="submit" disabled="@_enviando" data-testid="enviar">@(_registro ? "Registrarme" : "Entrar")</button>
        <button class="boton enlace-boton" type="button" @onclick="Alternar" data-testid="alternar">
            @(_registro ? "Ya tengo cuenta" : "Crear una cuenta")
        </button>
    </EditForm>
</section>

@code {
    private readonly CredencialesModelo _modelo = new();
    private bool _registro, _enviando;
    private string? _error;

    private void Alternar() { _registro = !_registro; _error = null; }

    private async Task Enviar()
    {
        _enviando = true; _error = null;
        try
        {
            if (_registro) { await Api.RegistrarAsync(_modelo); _registro = false; }   // tras registrarse, entra directamente
            var token = await Api.EntrarAsync(_modelo);
            await Sesion.IniciarAsync(token.AccessToken);
            Navegacion.NavigateTo("");
        }
        catch (ApiException e) { _error = e.Message; }
        finally { _enviando = false; }
    }
}
```

`Components/FilaTarea.razor`: un componente hijo con parámetros y eventos

```xml
<tr class="@(Tarea.Done ? "hecha" : "")" data-testid="fila">
    <td>
        <strong>@Tarea.Title</strong>
        @if (!string.IsNullOrEmpty(Tarea.Description)) { <div class="nota">@Tarea.Description</div> }
    </td>
    <td><span class="insignia @Tarea.Priority.ToLower()">@Traducir(Tarea.Priority)</span></td>
    <td>
        @if (Tarea.DueDate is { } fecha)
        {
            <span class="@(Tarea.IsOverdue ? "vencida" : "")">@fecha.ToString("dd/MM/yyyy")@(Tarea.IsOverdue ? " ⚠" : "")</span>
        }
        else { <span class="nota">—</span> }
    </td>
    <td class="acciones">
        @if (!Tarea.Done)
        {
            <button class="boton pequeno" @onclick="() => AlCompletar.InvokeAsync(Tarea.Id)" data-testid="completar">Completar</button>
        }
        @if (PuedeBorrar)
        {
            <button class="boton pequeno peligro" @onclick="() => AlEliminar.InvokeAsync(Tarea.Id)" data-testid="eliminar">Eliminar</button>
        }
    </td>
</tr>

@code {
    // Un componente recibe datos con [Parameter] (hacia dentro) y avisa con EventCallback (hacia fuera).
    [Parameter, EditorRequired] public Tarea Tarea { get; set; } = default!;
    [Parameter] public bool PuedeBorrar { get; set; }
    [Parameter] public EventCallback<int> AlCompletar { get; set; }
    [Parameter] public EventCallback<int> AlEliminar { get; set; }

    private static string Traducir(string p) => p switch { "High" => "Alta", "Medium" => "Media", "Low" => "Baja", _ => p };
}
```

`Pages/Tareas.razor`: la página principal (el CRUD)

```xml
@page "/"
@implements IDisposable
@inject ApiClient Api
@inject Sesion Sesion
@inject NavigationManager Navegacion

<PageTitle>Tareas · TaskFlow</PageTitle>

@if (!_listo)
{
    <p>Cargando…</p>
}
else
{
    <h1>Mis tareas</h1>

    @if (_estadisticas is { } s)
    {
        <div class="resumen" data-testid="estadisticas">
            <span>Total <b>@s.Total</b></span><span>Pendientes <b>@s.Pending</b></span>
            <span>Hechas <b>@s.Done</b></span><span class="vencida">Vencidas <b>@s.Overdue</b></span>
        </div>
    }

    @if (_error is not null)
    {
        <p class="error" role="alert" data-testid="error">@_error <button class="enlace-boton" @onclick="() => _error = null">cerrar</button></p>
    }

    <section class="tarjeta">
        <h2>Nueva tarea</h2>
        <EditForm Model="_nueva" OnValidSubmit="Crear" FormName="nueva-tarea">
            <DataAnnotationsValidator />
            <label>Título <InputText @bind-Value="_nueva.Title" data-testid="titulo" /></label>
            <div class="mensaje"><ValidationMessage For="() => _nueva.Title" /></div>
            <label>Descripción <InputTextArea @bind-Value="_nueva.Description" rows="2" data-testid="descripcion" /></label>
            <div class="fila">
                <label>Prioridad
                    <InputSelect @bind-Value="_nueva.Priority" data-testid="prioridad">
                        <option value="@Prioridad.Low">Baja</option>
                        <option value="@Prioridad.Medium">Media</option>
                        <option value="@Prioridad.High">Alta</option>
                    </InputSelect>
                </label>
                <label>Fecha límite <InputDate @bind-Value="_nueva.DueDate" data-testid="fecha" /></label>
            </div>
            <button class="boton" type="submit" data-testid="crear">Añadir tarea</button>
        </EditForm>
    </section>

    <section class="tarjeta">
        <h2>Filtros</h2>
        <div class="fila">
            <label>Buscar <input @bind="_texto" @bind:event="oninput" @bind:after="Cargar" placeholder="texto…" data-testid="buscar" /></label>
            <label>Estado
                <select @bind="_estado" @bind:after="Cargar" data-testid="estado">
                    <option value="">Todas</option><option value="pend">Pendientes</option><option value="hechas">Hechas</option>
                </select>
            </label>
        </div>
    </section>

    @if (_tareas.Count == 0)
    {
        <p class="vacio" data-testid="vacio">No hay tareas que mostrar.</p>
    }
    else
    {
        <div class="tabla-envoltorio">
        <table class="tabla" data-testid="tabla">
            <thead><tr><th>Tarea</th><th>Prioridad</th><th>Límite</th><th></th></tr></thead>
            <tbody>
                @foreach (var tarea in _tareas)
                {
                    <FilaTarea @key="tarea.Id" Tarea="tarea" PuedeBorrar="Sesion.EsAdmin" AlCompletar="Completar" AlEliminar="Eliminar" />
                }
            </tbody>
        </table>
        </div>
    }
}

@code {
    private List<Tarea> _tareas = [];
    private Estadisticas? _estadisticas;
    private readonly NuevaTareaModelo _nueva = new();
    private string _texto = "", _estado = "";
    private string? _error;
    private bool _listo;

    protected override async Task OnInitializedAsync()
    {
        await Sesion.RestaurarAsync();
        if (!Sesion.EstaAutenticado) { Navegacion.NavigateTo("login"); return; }   // protección de ruta (solo comodidad: el servidor es quien protege)
        Sesion.Cambio += AlCambiarSesion;
        await Cargar();
        _listo = true;
    }

    private void AlCambiarSesion() => InvokeAsync(StateHasChanged);
    public void Dispose() => Sesion.Cambio -= AlCambiarSesion;

    private async Task Cargar()
    {
        try
        {
            bool? hechas = _estado switch { "hechas" => true, "pend" => false, _ => null };
            _tareas = await Api.ListarAsync(hechas, null, _texto);
            _estadisticas = await Api.EstadisticasAsync();
        }
        catch (ApiException e) when (e.Estado == 401) { await Sesion.CerrarAsync(); Navegacion.NavigateTo("login"); }
        catch (ApiException e) { _error = e.Message; }
    }

    private async Task Crear()
    {
        await Ejecutar(async () =>
        {
            await Api.CrearAsync(_nueva);
            _nueva.Title = ""; _nueva.Description = null; _nueva.DueDate = null; _nueva.Priority = Prioridad.Medium;
        });
    }

    private Task Completar(int id) => Ejecutar(() => Api.CompletarAsync(id));
    private Task Eliminar(int id) => Ejecutar(() => Api.EliminarAsync(id));

    // Patrón común: ejecutar una acción, mostrar el error si falla, y recargar los datos
    private async Task Ejecutar(Func<Task> accion)
    {
        _error = null;
        try { await accion(); await Cargar(); }
        catch (ApiException e) { _error = e.Message; }
    }
}
```

### Cómo se lee, paso a paso

**Las decisiones de diseño**

1. **El cliente tiene sus propios modelos** (`Tarea`, `NuevaTareaModelo`…): no referencia el `Core` del servidor. Así cliente y servidor **evolucionan y se despliegan por separado** y el cliente no arrastra código que no necesita. El precio: hay que **mantener sincronizados** los contratos (con OpenAPI se pueden generar automáticamente; no se hizo aquí).
2. **`ApiClient` concentra toda la comunicación**: añade el `Authorization: Bearer …`, serializa JSON con los enums como texto, y **traduce** `401/403/404` y los `ProblemDetails`/errores de validación a **mensajes en español** que el usuario entiende. Los componentes solo manejan `ApiException`.
3. **`Sesion`** guarda el token en memoria y en **`sessionStorage`** (sobrevive a F5, desaparece al cerrar la pestaña). **Compromiso:** cualquier script de la página (por ejemplo, uno inyectado por un fallo XSS) puede leer `sessionStorage` y `localStorage`; una cookie `HttpOnly` evita eso pero trae sus propios problemas (CSRF). **No hay una opción gratis**; para aplicaciones serias se usa el patrón BFF o un proveedor de identidad (OpenID Connect), **no cubiertos aquí**.
4. **El nombre y el rol se leen del propio token en el cliente** *solo para mostrar y para esconder botones*. Cualquiera podría editar eso en las herramientas del navegador; el servidor valida de verdad.

**El componente `Tareas`**

5. **`OnInitializedAsync`** restaura la sesión, **redirige al login** si no hay (comodidad de interfaz, no protección) y carga los datos. Mientras tanto muestra «Cargando…».
6. **Los filtros** usan `@bind:event="oninput"` + `@bind:after="Cargar"`: cada tecla recarga la lista **desde el servidor** (que filtra). *Mejora pendiente: esperar unos 300 ms antes de lanzar la consulta (debounce) para no hacer una petición por cada letra.*
7. **`Ejecutar(Func<Task>)`** concentra el patrón repetido: ejecutar la acción, mostrar el error si falla, recargar los datos.
8. **`@key="tarea.Id"`** da identidad a cada fila. **`FilaTarea`** recibe la tarea por `[Parameter]` y avisa con `EventCallback<int>`: el componente hijo **no sabe nada de la API**, y se puede reutilizar y probar aislado.
9. **`Dispose`** quita la suscripción a `Sesion.Cambio`.

### Resultado real: la aplicación usada por un navegador

`e2e.js` (Playwright) abre **Chromium real**, usa la interfaz como una persona y comprueba cada paso. **Los pasos 6 y 14 son las más reveladoras**: el 6 prueba que una regla del **dominio** (fecha en el pasado) llega hasta la pantalla, y el 14 que **esconder el botón no protege**.

```text
 1. Sin sesión, «/» redirige a /login
 2. Formulario vacío → mensajes de validación («Escribe tu usuario»)
 3. Contraseña de 3 caracteres → «Mínimo 8 caracteres» (validado en el cliente)
 4. Registro + entrada → «Mis tareas», usuario «ana Admin» (primer usuario = Admin)
 5. Título de 2 letras → mensaje de validación en el formulario
 6. Fecha pasada → la API rechaza y la página muestra: «La fecha límite no puede estar en el pasado.»
 7. Tres tareas creadas → tabla con 3 filas
 8. Completar una tarea → estadísticas: Total 3 Pendientes 2 Hechas 1 Vencidas 0
 9. Filtro «Hechas» → 1 fila
10. Búsqueda «jwt» (sin distinguir mayúsculas) → solo «Revisar JWT»
11. Recarga (F5) → sigue la sesión y los datos
12. Eliminar como Admin → 2 filas
13. Usuario «luis» (rol User): la interfaz NO muestra «Eliminar»
14. Saltándose la interfaz: DELETE directo a la API con el token de «luis» → 403 (el servidor es quien protege)
15. Contraseña incorrecta → «Credenciales incorrectas o sesión caducada.»
16. Ancho de móvil (390 px): sin desplazamiento horizontal
17. Consola del navegador: 3 respuestas 400/401/403 (las provocadas a propósito en los pasos 6, 14 y 15) y 0 errores inesperados

RESULTADO: todos los pasos pasaron
```

**Qué demuestra cada grupo de pasos**

| Pasos | Qué se verifica |
|---|---|
| 1 | La ruta protegida redirige a `/login` |
| 2, 3, 5 | Validación **en el cliente** (sin llamar al servidor) |
| 4 | Registro → entrada directa; el primer usuario es Admin |
| 6 | **Regla del dominio** (fecha pasada) rechazada por la API y **mostrada** en la página |
| 7, 8 | Crear, completar, estadísticas actualizadas |
| 9, 10 | **Filtros** que consulta el servidor |
| 11 | La sesión **sobrevive a F5** |
| 12 | Borrar como Admin |
| 13 | El rol `User` **no ve** el botón «Eliminar» |
| 14 | …pero **atacando la API directamente** con su token: **403** |
| 15 | Mensaje claro con credenciales incorrectas |
| 16 | **Móvil (390 px):** sin desplazamiento horizontal |
| 17 | Ningún error inesperado en la consola del navegador |

```text
Capturas del recorrido (en la carpeta de ejemplos): e2e-01-vacia.png · e2e-02-tareas.png · e2e-03-movil.png
```

### Los tres defectos que aparecieron al ejecutarla (y que ninguna lectura del código habría mostrado)

1. **El clic perdido.** La primera versión de la prueba **no conseguía registrar a un usuario**. Causa: al pulsar «Registrarme», el campo contraseña **pierde el foco**, se valida, el mensaje de error **desaparece** y la página **sube unos píxeles**: el botón se movió **entre que se pulsó y se soltó el ratón** y el clic se perdió. **Le puede pasar a una persona real, no solo al test.** **Arreglo:** reservar el espacio del mensaje (`.mensaje { min-height: 1.5rem }`) para que nada se desplace.
2. **Desbordamiento en móvil.** El paso 16 falló: **la tabla medía 500 px en una pantalla de 390**, y la página tenía barra horizontal. Primer intento (reducir márgenes y permitir que los botones se partan) **no fue suficiente**; el **arreglo** fue envolver la tabla en un contenedor con `overflow-x: auto`, de modo que, si no cabe, se desplaza **la tabla** y no toda la página.
3. **Procesos zombi en la prueba.** Dos servidores de desarrollo competían por el puerto 5200 y la prueba se ejecutaba contra el viejo. **No era un defecto de la aplicación, sino del entorno de pruebas**, y es la clase de problema que hace perder horas: **comprueba siempre contra qué estás probando**.

> Los tres se resolvieron **cambiando el código o la prueba, nunca ablandando una aserción**.

### Accesibilidad y diseño: lo que se hizo y lo que no

**Se hizo:** cada campo tiene su `<label>`; los errores usan `role="alert"`; el foco visible se ve (`:focus-visible`); el tema **claro/oscuro** sigue la preferencia del sistema; el diseño **se adapta al móvil**; los botones son botones (no `div` con clic).

**No se hizo (sé que falta):** prueba con **lector de pantalla** real; comprobación de **contraste** con una herramienta; navegación completa **solo con teclado** verificada por un humano; el selector de fecha muestra el formato de la configuración del navegador (`mm/dd/yyyy` en la captura, aunque el resto está en español); soporte de **Firefox/Safari** no probado. **La accesibilidad real exige probar con personas y herramientas**, no basta con etiquetas correctas.

### Errores típicos

| Síntoma | Causa | Solución |
|---|---|---|
| «CORS policy: No 'Access-Control-Allow-Origin' header» | La API no permite ese origen | `AddCors` con los orígenes concretos, **antes** de la autenticación |
| Todas las llamadas dan `401` tras recargar | El token no se restauró | `RestaurarAsync` al arrancar |
| La interfaz no refleja el cambio de otro componente | No hay notificación entre ellos | Un servicio con un evento (como `Sesion.Cambio`) + `StateHasChanged` |
| «Cargando…» para siempre | Una excepción en `OnInitializedAsync` sin capturar | `try/catch` y mostrar el error |
| Peticiones por cada tecla | `@bind:event="oninput"` sin espera | Debounce (esperar a que se deje de escribir) |
| La URL de la API escrita en el código | Comodidad | `wwwroot/appsettings.json` por entorno |
| Se confía en el rol del cliente | «Si no hay botón, no se puede» | **El servidor autoriza**; el cliente solo decide qué mostrar |

> **Ejercicio 52.1:** añade el botón «Editar» (usa `PUT /tasks/{id}`) con un formulario en la propia fila.
>
> **Ejercicio 52.2:** añade el *debounce* de 300 ms al buscador con un `CancellationTokenSource`.
>
> **Ejercicio 52.3:** añade un paso al recorrido E2E para el botón «Editar» y comprueba que **falla antes** de implementarlo.

---

## Resumen de la Parte VIII

| Si quieres… | Usa… |
|---|---|
| Una interfaz web en C# | Blazor (componentes `.razor`) |
| Arranque rápido, código en el servidor | Blazor Server (conexión permanente) |
| Funcionar en el navegador sin servidor de aplicación | Blazor WebAssembly (descarga inicial) |
| Datos en dos sentidos | `@bind` |
| Reutilizar piezas de interfaz | Componentes con `[Parameter]` y `EventCallback` |
| Validar un formulario | `EditForm` + `DataAnnotationsValidator` |
| Llamar a una API desde el navegador | `HttpClient` + CORS en la API |
| Editar con la aplicación funcionando | `dotnet watch` (Hot Reload) |
| Comprobar la interfaz de verdad | Pruebas E2E con Playwright |
| Seguridad | **Siempre en el servidor**; el cliente es público |

**Sigue con la Parte IX:** escritorio con **WPF y MVVM**.
