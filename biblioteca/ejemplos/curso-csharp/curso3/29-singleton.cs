// Lección 29: el patrón Singleton (una única instancia) y por qué casi siempre se evita
// Ejecutar: dotnet run 29-singleton.cs
using System.Collections.Concurrent;

// ═══════════════════════ La idea ═══════════════════════
// Singleton = "solo puede existir UN objeto de esta clase en todo el programa y todos lo comparten".
// Ejemplos legítimos: configuración, registro (log), caché, generador de ids.
// Se consigue con: constructor PRIVADO + una forma estática de pedir la instancia única.

// ═══════════════════════ Versión 1: la ingenua (¡NO es segura con hilos!) ═══════════════════════
Console.WriteLine("── Versión 1: ingenua ──");
NaiveSingleton.Created = 0;
RunConcurrently(8, () => NaiveSingleton.Instance);
Console.WriteLine($"   Se crearon {NaiveSingleton.Created} instancias (¡debería ser 1!). Es una CARRERA DE DATOS.");

// ═══════════════════════ Versión 2: con cerrojo (lock) y doble comprobación ═══════════════════════
Console.WriteLine("── Versión 2: lock + doble comprobación ──");
LockSingleton.Created = 0;
RunConcurrently(8, () => LockSingleton.Instance);
Console.WriteLine($"   Instancias creadas: {LockSingleton.Created}");

// ═══════════════════════ Versión 3: campo estático (la más sencilla y segura) ═══════════════════════
// El CLR garantiza que un inicializador estático se ejecuta UNA sola vez y de forma segura entre hilos.
Console.WriteLine("── Versión 3: campo estático ──");
RunConcurrently(8, () => EagerSingleton.Instance);
Console.WriteLine($"   Instancias creadas: {EagerSingleton.Created}   (se crea al primer uso de la clase)");

// ═══════════════════════ Versión 4: Lazy<T> (perezosa y segura) ═══════════════════════
Console.WriteLine("── Versión 4: Lazy<T> (se crea solo cuando alguien la pide) ──");
Console.WriteLine($"   antes de pedirla: creada = {LazySingleton.IsCreated}");
RunConcurrently(8, () => LazySingleton.Instance);
Console.WriteLine($"   después: creada = {LazySingleton.IsCreated}, instancias = {LazySingleton.Created}");

// ═══════════════════════ El problema real del Singleton ═══════════════════════
Console.WriteLine("── El problema: estado global compartido ──");
var a = Settings.Instance; a.Theme = "oscuro";
Console.WriteLine($"   código A cambia el tema a: {a.Theme}");
Console.WriteLine($"   código B (otro lugar) lo ve:  {Settings.Instance.Theme}   <- B depende de lo que hizo A sin enterarse");

// Una "prueba" de dos servicios que usan el Singleton: el segundo queda contaminado por el primero
Console.WriteLine($"   ServicioX dice: {new ServiceX().Greet()}");
Settings.Instance.Theme = "claro";
Console.WriteLine($"   ServicioX dice: {new ServiceX().Greet()}   <- mismo código, otro resultado según lo que pasó antes");

// ═══════════════════════ La alternativa moderna: inyectar UNA instancia ═══════════════════════
Console.WriteLine("── Alternativa: una instancia única, pero ENTREGADA (inyectada) ──");
var shared = new AppSettings { Theme = "oscuro" };          // se crea UNA vez, en un sitio (la raíz de composición)
var y1 = new ServiceY(shared);
var y2 = new ServiceY(shared);                              // los dos comparten la misma, porque SE LA DAMOS
Console.WriteLine($"   {y1.Greet()} / {y2.Greet()}   (misma instancia: {ReferenceEquals(y1.Settings, y2.Settings)})");
var testSettings = new AppSettings { Theme = "de prueba" }; // en una prueba entregamos otra distinta: imposible con el Singleton clásico
Console.WriteLine($"   en una prueba: {new ServiceY(testSettings).Greet()}");

Console.WriteLine("""

    ── Cuándo sí y cuándo no ──
    Sí   : una instancia por aplicación, entregada por el contenedor de dependencias (AddSingleton).
    Sí   : clases estáticas sin estado (Math, Path).
    No   : Singleton clásico con "Instance" estático y estado modificable: oculta dependencias,
           impide las pruebas y es difícil de hacer seguro con hilos.
    """);

// ═══════════════════════ Utilidades y tipos ═══════════════════════

// Lanza N hilos que piden la instancia EXACTAMENTE a la vez (la Barrier los sincroniza)
static void RunConcurrently(int threads, Func<object> action)
{
    using var barrier = new Barrier(threads);
    var tasks = Enumerable.Range(0, threads).Select(_ => Task.Run(() => { barrier.SignalAndWait(); action(); })).ToArray();
    Task.WaitAll(tasks);
}

class NaiveSingleton
{
    public static int Created;
    private static NaiveSingleton? _instance;
    private NaiveSingleton() { Interlocked.Increment(ref Created); Thread.Sleep(20); }      // privado: nadie más puede hacer "new"
    public static NaiveSingleton Instance => _instance ??= new NaiveSingleton();             // ¡no es atómico!
}

class LockSingleton
{
    public static int Created;
    private static LockSingleton? _instance;
    private static readonly object Gate = new();
    private LockSingleton() { Interlocked.Increment(ref Created); Thread.Sleep(20); }
    public static LockSingleton Instance
    {
        get
        {
            if (_instance is null)                       // 1ª comprobación (rápida, sin bloquear)
                lock (Gate)
                    if (_instance is null)               // 2ª comprobación (dentro del cerrojo)
                        _instance = new LockSingleton();
            return _instance;
        }
    }
}

sealed class EagerSingleton
{
    public static int Created;
    public static readonly EagerSingleton Instance = new();        // el CLR lo inicializa una sola vez
    private EagerSingleton() { Interlocked.Increment(ref Created); }
}

sealed class LazySingleton
{
    public static int Created;
    private static readonly Lazy<LazySingleton> Holder = new(() => new LazySingleton());   // thread-safe por defecto
    public static LazySingleton Instance => Holder.Value;
    public static bool IsCreated => Holder.IsValueCreated;
    private LazySingleton() { Interlocked.Increment(ref Created); }
}

class Settings                                                      // Singleton clásico con estado modificable
{
    public static Settings Instance { get; } = new();
    private Settings() { }
    public string Theme { get; set; } = "claro";
}
class ServiceX { public string Greet() => $"hola con el tema {Settings.Instance.Theme}"; }   // dependencia OCULTA

class AppSettings { public string Theme { get; init; } = "claro"; }
class ServiceY(AppSettings settings) { public AppSettings Settings => settings; public string Greet() => $"hola con el tema {settings.Theme}"; } // dependencia VISIBLE
