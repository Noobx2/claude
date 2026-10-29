// Lección 33: patrones de diseño ESTRUCTURALES (cómo se combinan los objetos)
// Ejecutar: dotnet run 33-patrones-estructurales.cs
using System.Diagnostics;

// ═══════════ 1. ADAPTER (adaptador) ═══════════
// Problema: tienes una clase que hace lo que necesitas pero con OTRA interfaz (código de terceros, antiguo...).
// Solución: una pieza intermedia que traduce. Como un adaptador de enchufe.
Console.WriteLine("── Adapter: que lo viejo hable el idioma nuevo ──");
ITemperatureSensor modern = new CelsiusSensor(21.5);
ITemperatureSensor legacy = new FahrenheitSensorAdapter(new OldFahrenheitSensor());     // el viejo, envuelto
foreach (var sensor in new[] { modern, legacy }) Console.WriteLine($"   {sensor.GetType().Name,-28} {sensor.ReadCelsius():F1} °C");

// ═══════════ 2. DECORATOR (decorador) ═══════════
// Problema: añadir comportamiento (registro, caché, reintentos) SIN tocar la clase ni crear mil subclases.
// Solución: envolver el objeto en otro que cumple el mismo contrato. Se pueden apilar como capas de cebolla.
Console.WriteLine("\n── Decorator: capas de comportamiento apilables ──");
IWeatherService service = new SlowWeatherService();
service = new CachedWeatherService(service);             // capa de caché
service = new LoggedWeatherService(service);             // capa de registro por fuera
foreach (string city in new[] { "Madrid", "Madrid", "Sevilla", "Madrid" })
    Console.WriteLine($"   -> {service.GetTemperature(city)} °C\n");

// ═══════════ 3. FACADE (fachada) ═══════════
// Problema: un subsistema complejo con muchos pasos. Solución: UNA puerta con un método sencillo.
Console.WriteLine("── Facade: una puerta sencilla a muchos pasos ──");
var shop = new OrderFacade(new Inventory(), new Payments(), new Shipping());
Console.WriteLine($"   {shop.Order("teclado", 1, "tarjeta")}");
Console.WriteLine($"   {shop.Order("monitor", 5, "tarjeta")}");

// ═══════════ 4. PROXY ═══════════
// Un representante que controla el acceso: carga perezosa, permisos, caché remota...
Console.WriteLine("\n── Proxy: control de acceso ──");
IDocument doc = new ProtectedDocument(new RealDocument("Plan secreto"), user: "visita");
Console.WriteLine($"   visita lee: {doc.Read()}");
IDocument admin = new ProtectedDocument(new RealDocument("Plan secreto"), user: "admin");
Console.WriteLine($"   admin lee:  {admin.Read()}");

// ═══════════ 5. COMPOSITE (composición en árbol) ═══════════
// Problema: tratar IGUAL a un elemento y a un grupo de elementos (archivos y carpetas, menús, escenas de un juego).
Console.WriteLine("\n── Composite: el árbol se trata como una sola cosa ──");
var root = new Folder("proyecto",
    new FileNode("README.md", 2),
    new Folder("src",
        new FileNode("Program.cs", 5),
        new FileNode("Service.cs", 12),
        new Folder("tests", new FileNode("Tests.cs", 8))),
    new FileNode("proyecto.csproj", 1));
root.Print(0);
Console.WriteLine($"   Tamaño total: {root.Size} KB   (la misma llamada vale para un archivo, una carpeta o todo el árbol)");

// ═══════════ TIPOS ═══════════
interface ITemperatureSensor { double ReadCelsius(); }
class CelsiusSensor(double value) : ITemperatureSensor { public double ReadCelsius() => value; }
class OldFahrenheitSensor { public double GetFahrenheitReading() => 72.5; }            // no se puede modificar (es "de terceros")
class FahrenheitSensorAdapter(OldFahrenheitSensor old) : ITemperatureSensor
{
    public double ReadCelsius() => (old.GetFahrenheitReading() - 32) * 5 / 9;           // la traducción
}

interface IWeatherService { double GetTemperature(string city); }
class SlowWeatherService : IWeatherService
{
    public double GetTemperature(string city)
    {
        Thread.Sleep(30);                                                               // simula una llamada de red lenta
        return city == "Madrid" ? 18.5 : 24.0;
    }
}
class CachedWeatherService(IWeatherService inner) : IWeatherService
{
    private readonly Dictionary<string, double> _cache = [];
    public double GetTemperature(string city)
    {
        if (_cache.TryGetValue(city, out double cached)) { Console.WriteLine($"   [caché] acierto para {city}"); return cached; }
        return _cache[city] = inner.GetTemperature(city);
    }
}
class LoggedWeatherService(IWeatherService inner) : IWeatherService
{
    public double GetTemperature(string city)
    {
        var watch = Stopwatch.StartNew();
        double value = inner.GetTemperature(city);
        Console.WriteLine($"   [log] {city} = {value} °C ({(watch.ElapsedMilliseconds < 15 ? "rápido" : "lento")})");
        return value;
    }
}

class Inventory { public bool InStock(string item, int qty) => item == "teclado" || qty <= 2; public void Reserve(string item, int qty) { } }
class Payments { public bool Charge(string method, decimal amount) => method == "tarjeta"; }
class Shipping { public string Schedule(string item) => $"envío de {item} para mañana"; }
class OrderFacade(Inventory inventory, Payments payments, Shipping shipping)           // oculta la coreografía de tres sistemas
{
    public string Order(string item, int qty, string method)
    {
        if (!inventory.InStock(item, qty)) return $"✗ No hay {qty} unidades de {item}";
        if (!payments.Charge(method, qty * 49.9m)) return "✗ Pago rechazado";
        inventory.Reserve(item, qty);
        return $"✓ {shipping.Schedule(item)}";
    }
}

interface IDocument { string Read(); }
class RealDocument(string content) : IDocument { public string Read() => content; }
class ProtectedDocument(IDocument real, string user) : IDocument
{
    public string Read() => user == "admin" ? real.Read() : "✗ acceso denegado";
}

abstract record Node(string Name)
{
    public abstract int Size { get; }
    public abstract void Print(int level);
}
record FileNode(string Name, int Kb) : Node(Name)
{
    public override int Size => Kb;
    public override void Print(int level) => Console.WriteLine($"   {new string(' ', level * 3)}- {Name} ({Kb} KB)");
}
record Folder(string Name, params Node[] Children) : Node(Name)
{
    public override int Size => Children.Sum(c => c.Size);                              // suma recursiva
    public override void Print(int level)
    {
        Console.WriteLine($"   {new string(' ', level * 3)}+ {Name}/ ({Size} KB)");
        foreach (var child in Children) child.Print(level + 1);
    }
}
