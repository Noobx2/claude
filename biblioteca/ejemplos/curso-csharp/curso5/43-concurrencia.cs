#:property PublishAot=false

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Channels;

Console.WriteLine($"Núcleos lógicos de esta máquina: {Environment.ProcessorCount}");

// ================= 1) Hilos (Thread) frente a tareas (Task) =================
Console.WriteLine("\n=== 1) Thread y Task ===");
var hilo = new Thread(() => Console.WriteLine($"  Thread manual: id={Environment.CurrentManagedThreadId}, ¿del pool? {Thread.CurrentThread.IsThreadPoolThread}"));
hilo.Start(); hilo.Join();                                         // Join espera a que termine
await Task.Run(() => Console.WriteLine($"  Task.Run     : id={Environment.CurrentManagedThreadId}, ¿del pool? {Thread.CurrentThread.IsThreadPoolThread}"));
Console.WriteLine("  Un Thread es caro (1 MB de pila reservado); las Task reutilizan hilos del ThreadPool.");

// ================= 2) Condición de carrera (race condition) =================
Console.WriteLine("\n=== 2) Condición de carrera ===");
const int Hilos = 8, Vueltas = 200_000;
int sinProteger = 0;
Parallel.For(0, Hilos, _ => { for (int i = 0; i < Vueltas; i++) sinProteger++; });       // ++ NO es atómico: lee, suma, escribe
Console.WriteLine($"  Esperado: {Hilos * Vueltas:N0}   Sin proteger: {sinProteger:N0}   (se pierden incrementos: {Hilos * Vueltas - sinProteger:N0})");

int conLock = 0; object candado = new();
Parallel.For(0, Hilos, _ => { for (int i = 0; i < Vueltas; i++) lock (candado) conLock++; });
Console.WriteLine($"  Con lock     : {conLock:N0}   (correcto, pero los hilos hacen cola)");

int conInterlocked = 0;
Parallel.For(0, Hilos, _ => { for (int i = 0; i < Vueltas; i++) Interlocked.Increment(ref conInterlocked); });
Console.WriteLine($"  Interlocked  : {conInterlocked:N0}   (correcto y sin bloqueo: una instrucción atómica del procesador)");

// ================= 3) Colecciones seguras para varios hilos =================
Console.WriteLine("\n=== 3) Colecciones concurrentes ===");
var palabras = new ConcurrentDictionary<string, int>();
Parallel.ForEach(Enumerable.Range(0, 10_000).Select(i => $"p{i % 5}"), p => palabras.AddOrUpdate(p, 1, (_, n) => n + 1));
Console.WriteLine("  ConcurrentDictionary: " + string.Join(", ", palabras.OrderBy(k => k.Key).Select(k => $"{k.Key}={k.Value}")));

// ================= 4) Productor / consumidor con Channel =================
Console.WriteLine("\n=== 4) Productor/consumidor con Channel<T> ===");
var canal = Channel.CreateBounded<int>(new BoundedChannelOptions(3) { FullMode = BoundedChannelFullMode.Wait });   // máx. 3 en espera
var productor = Task.Run(async () =>
{
    for (int i = 1; i <= 8; i++) { await canal.Writer.WriteAsync(i); }                  // si el canal está lleno, ESPERA (contrapresión)
    canal.Writer.Complete();                                                            // avisa de que no habrá más
});
var consumidores = Enumerable.Range(1, 2).Select(id => Task.Run(async () =>
{
    var procesados = new List<int>();
    await foreach (int n in canal.Reader.ReadAllAsync()) { await Task.Delay(5); procesados.Add(n); }
    return $"consumidor {id} procesó [{string.Join(",", procesados)}]";
})).ToArray();
await productor;
foreach (var t in await Task.WhenAll(consumidores)) Console.WriteLine("  " + t);

// ================= 5) Limitar la concurrencia: SemaphoreSlim =================
Console.WriteLine("\n=== 5) SemaphoreSlim: como mucho 3 a la vez ===");
var semaforo = new SemaphoreSlim(3); int dentro = 0, maximo = 0;
await Task.WhenAll(Enumerable.Range(0, 12).Select(async _ =>
{
    await semaforo.WaitAsync();
    try
    {
        int ahora = Interlocked.Increment(ref dentro);
        int visto; do { visto = maximo; } while (ahora > visto && Interlocked.CompareExchange(ref maximo, ahora, visto) != visto);
        await Task.Delay(20);
    }
    finally { Interlocked.Decrement(ref dentro); semaforo.Release(); }
}));
Console.WriteLine($"  12 trabajos lanzados; máximo simultáneo observado = {maximo}");

// ================= 6) Interbloqueo (deadlock): reproducido y detectado =================
Console.WriteLine("\n=== 6) Deadlock ===");
object A = new(), B = new();
var barrera = new Barrier(2);                                                           // hace que ambos hilos tengan ya su primer candado
bool Intentar(object primero, object segundo, string nombre)
{
    lock (primero)
    {
        barrera.SignalAndWait();                                                        // los dos hilos esperan aquí
        bool conseguido = Monitor.TryEnter(segundo, TimeSpan.FromMilliseconds(300));    // intentar con tiempo límite en vez de esperar para siempre
        barrera.SignalAndWait();                                                        // ambos esperan a que el otro también haya fallado antes de soltar nada
        if (conseguido) Monitor.Exit(segundo);
        return conseguido;
    }
}
var h1 = Task.Run(() => Intentar(A, B, "Hilo 1 (A luego B)"));
var h2 = Task.Run(() => Intentar(B, A, "Hilo 2 (B luego A)"));
await Task.WhenAll(h1, h2);
foreach (var (nombre, ok) in new[] { ("Hilo 1 (A luego B)", h1.Result), ("Hilo 2 (B luego A)", h2.Result) })
    if (!ok) Console.WriteLine($"  {nombre}: tengo mi candado y NO consigo el otro tras 300 ms → habría quedado bloqueado para siempre");
Console.WriteLine("  Solución: tomar SIEMPRE los candados en el mismo orden (A y luego B en los dos hilos), o evitar anidarlos.");

bool Ordenado(string nombre) { lock (A) { lock (B) { return true; } } }
Console.WriteLine($"  Con orden fijo ambos terminan: {await Task.Run(() => Ordenado("x")) && await Task.Run(() => Ordenado("y"))}");

// ================= 7) Paralelismo de datos: cuándo compensa =================
Console.WriteLine("\n=== 7) Parallel y PLINQ (trabajo de CPU) ===");
static bool EsPrimo(int n) { if (n < 2) return false; for (int d = 2; d * d <= n; d++) if (n % d == 0) return false; return true; }
Console.WriteLine("  Tamaño     secuencial     PLINQ    aceleración");
foreach (int hasta in new[] { 2_000, 300_000, 3_000_000 })
{
    var datos = Enumerable.Range(0, hasta);
    datos.Count(EsPrimo); datos.AsParallel().Count(EsPrimo);                              // calentamiento de AMBAS versiones (el JIT compila en la 1.ª llamada)
    var reloj = Stopwatch.StartNew(); int seq = datos.Count(EsPrimo); double tSec = reloj.Elapsed.TotalMilliseconds;
    reloj.Restart(); int par = datos.AsParallel().Count(EsPrimo); double tPar = reloj.Elapsed.TotalMilliseconds;
    Console.WriteLine($"  {hasta,9:N0}  {tSec,8:F1} ms  {tPar,8:F1} ms   {tSec / tPar,5:F1}x   (mismo resultado: {seq == par})");
}
Console.WriteLine("  Con poco trabajo repartir cuesta más que calcular; con mucho, el paralelismo compensa (tope = nº de núcleos).");
