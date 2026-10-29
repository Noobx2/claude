// Lección 32: patrones de diseño CREACIONALES (cómo se crean los objetos)
// Ejecutar: dotnet run 32-patrones-creacionales.cs

// ═══════════ 1. FACTORY METHOD (método de fábrica) ═══════════
// Problema: crear un objeto exige decidir QUÉ clase concreta. Se centraliza la decisión en un solo sitio.
Console.WriteLine("── Factory Method: la decisión en un único lugar ──");
foreach (string type in new[] { "pdf", "csv", "json", "xml" })
{
    try { Console.WriteLine($"   {type,-5} -> {ExporterFactory.Create(type).Export(["Ana", "Luis"])}"); }
    catch (ArgumentException e) { Console.WriteLine($"   {type,-5} -> error: {e.Message}"); }
}

// Fábrica con nombre ("constructores con nombre"): clarifica la intención frente a un constructor ambiguo
var green = Color.FromRgb(0, 255, 0);
var fromHex = Color.FromHex("#FF8800");
Console.WriteLine($"   {green} / {fromHex}   (Color.FromRgb y Color.FromHex en lugar de dos constructores confusos)");

// ═══════════ 2. ABSTRACT FACTORY (familias de objetos) ═══════════
// Problema: crear VARIOS objetos que deben ser coherentes entre sí (un tema oscuro con botones y fondos oscuros).
Console.WriteLine("\n── Abstract Factory: familias coherentes ──");
foreach (IThemeFactory theme in new IThemeFactory[] { new DarkTheme(), new LightTheme() })
{
    var button = theme.CreateButton();
    var window = theme.CreateWindow();
    Console.WriteLine($"   {theme.Name}: {window.Draw()} con {button.Draw()}");
}

// ═══════════ 3. BUILDER (constructor paso a paso) ═══════════
// Problema: un objeto con MUCHAS opciones y reglas ("telescoping constructor": new Pizza(true, false, true, null, 3...)).
Console.WriteLine("\n── Builder: construcción legible y validada ──");
var request = new HttpRequestBuilder("https://api.ejemplo.es")
    .Path("/users")
    .Query("page", "2")
    .Query("sort", "name")
    .Header("Accept", "application/json")
    .Timeout(TimeSpan.FromSeconds(10))
    .Build();                                                   // aquí se valida y se crea el objeto inmutable
Console.WriteLine($"   {request}");
try { new HttpRequestBuilder("ftp://mal").Build(); }
catch (InvalidOperationException e) { Console.WriteLine($"   Build() rechazó datos incompletos: {e.Message}"); }

// ═══════════ 4. PROTOTYPE (clonar) ═══════════
// En C#, un record ya trae el clonado con "with".
Console.WriteLine("\n── Prototype: copiar y modificar ──");
var template = new EmailTemplate("Bienvenida", "Hola {nombre}", ["info@ejemplo.es"]);
var promo = template with { Subject = "Oferta", Body = "Hola {nombre}, 20 % de descuento" };
Console.WriteLine($"   {template.Subject}: {template.Body}\n   {promo.Subject}: {promo.Body}");
Console.WriteLine($"   OJO copia superficial: las listas se COMPARTEN -> {ReferenceEquals(template.Cc, promo.Cc)}");
var deep = template with { Cc = [.. template.Cc, "ventas@ejemplo.es"] };      // para modificar una lista se crea otra
Console.WriteLine($"   copia profunda manual: {template.Cc.Length} frente a {deep.Cc.Length} destinatarios");

// ═══════════ 5. OBJECT POOL (reutilizar objetos caros) ═══════════
// Muy usado en videojuegos (balas, partículas) y servidores (conexiones, buffers).
Console.WriteLine("\n── Object Pool: reutilizar en vez de crear y destruir ──");
var pool = new Pool<Bullet>(() => new Bullet(), reset: b => b.Reset(), initial: 2);
var shots = new List<Bullet> { pool.Rent(), pool.Rent(), pool.Rent() };      // pide 3 con 2 disponibles: crea 1 más
Console.WriteLine($"   creadas: {pool.Created}, en uso: {pool.InUse}, libres: {pool.Free}");
foreach (var shot in shots) pool.Return(shot);
Console.WriteLine($"   tras devolverlas: en uso {pool.InUse}, libres {pool.Free}");
var again = pool.Rent();
Console.WriteLine($"   se reutiliza una (id {again.Id}); total creadas sigue en {pool.Created}");

// ═══════════ 6. LAZY INITIALIZATION ═══════════
Console.WriteLine("\n── Inicialización perezosa: no se crea hasta que hace falta ──");
var report = new Lazy<HeavyReport>(() => { Console.WriteLine("   (creando el informe pesado...)"); return new HeavyReport(); });
Console.WriteLine($"   ¿creado todavía? {report.IsValueCreated}");
Console.WriteLine($"   primer uso: {report.Value.Title}");
Console.WriteLine($"   segundo uso: {report.Value.Title}   (no se vuelve a crear)");

// ═══════════ TIPOS ═══════════
interface IExporter { string Export(IEnumerable<string> items); }
class CsvExporter : IExporter { public string Export(IEnumerable<string> items) => string.Join(";", items); }
class JsonExporter : IExporter { public string Export(IEnumerable<string> items) => "[" + string.Join(",", items.Select(i => $"\"{i}\"")) + "]"; }
class PdfExporter : IExporter { public string Export(IEnumerable<string> items) => $"%PDF({items.Count()} líneas)"; }

static class ExporterFactory
{
    public static IExporter Create(string type) => type.ToLowerInvariant() switch
    {
        "csv" => new CsvExporter(),
        "json" => new JsonExporter(),
        "pdf" => new PdfExporter(),
        _ => throw new ArgumentException($"formato desconocido '{type}'"),
    };
}

readonly record struct Color(byte R, byte G, byte B)
{
    public static Color FromRgb(byte r, byte g, byte b) => new(r, g, b);
    public static Color FromHex(string hex) =>
        new(Convert.ToByte(hex[1..3], 16), Convert.ToByte(hex[3..5], 16), Convert.ToByte(hex[5..7], 16));
    public override string ToString() => $"rgb({R},{G},{B})";
}

interface IButton { string Draw(); }
interface IWindow { string Draw(); }
interface IThemeFactory { string Name { get; } IButton CreateButton(); IWindow CreateWindow(); }
class DarkButton : IButton { public string Draw() => "botón negro"; }
class DarkWindow : IWindow { public string Draw() => "ventana oscura"; }
class LightButton : IButton { public string Draw() => "botón blanco"; }
class LightWindow : IWindow { public string Draw() => "ventana clara"; }
class DarkTheme : IThemeFactory { public string Name => "Oscuro"; public IButton CreateButton() => new DarkButton(); public IWindow CreateWindow() => new DarkWindow(); }
class LightTheme : IThemeFactory { public string Name => "Claro"; public IButton CreateButton() => new LightButton(); public IWindow CreateWindow() => new LightWindow(); }

sealed record HttpRequest(string Url, IReadOnlyDictionary<string, string> Headers, TimeSpan Timeout)
{
    public override string ToString() => $"GET {Url} (timeout {Timeout.TotalSeconds}s, cabeceras: {string.Join(", ", Headers.Select(h => $"{h.Key}={h.Value}"))})";
}

class HttpRequestBuilder(string baseUrl)
{
    private string _path = "";
    private readonly List<string> _query = [];
    private readonly Dictionary<string, string> _headers = [];
    private TimeSpan _timeout = TimeSpan.FromSeconds(30);

    public HttpRequestBuilder Path(string path) { _path = path; return this; }                        // cada método devuelve "this": se encadenan
    public HttpRequestBuilder Query(string key, string value) { _query.Add($"{key}={Uri.EscapeDataString(value)}"); return this; }
    public HttpRequestBuilder Header(string key, string value) { _headers[key] = value; return this; }
    public HttpRequestBuilder Timeout(TimeSpan timeout) { _timeout = timeout; return this; }

    public HttpRequest Build()
    {
        if (!baseUrl.StartsWith("https://")) throw new InvalidOperationException("la URL debe empezar por https://");
        string url = baseUrl + _path + (_query.Count > 0 ? "?" + string.Join("&", _query) : "");
        return new HttpRequest(url, _headers, _timeout);
    }
}

record EmailTemplate(string Subject, string Body, string[] Cc);

class Bullet
{
    private static int _next;
    public int Id { get; } = ++_next;
    public void Reset() { /* posición a cero, daño a cero... */ }
}

class Pool<T>(Func<T> create, Action<T> reset, int initial = 0) where T : class
{
    private readonly Stack<T> _free = new(Enumerable.Range(0, initial).Select(_ => create()));
    public int Created { get; private set; } = initial;
    public int Free => _free.Count;
    public int InUse => Created - Free;
    public T Rent() { if (_free.Count > 0) return _free.Pop(); Created++; return create(); }
    public void Return(T item) { reset(item); _free.Push(item); }
}

class HeavyReport { public string Title => "Informe anual"; }
