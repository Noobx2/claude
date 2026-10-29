#:property PublishAot=false

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;

// ================= 1) Síncrono frente a asíncrono: el tiempo lo dice todo =================
Console.WriteLine("=== 1) Tres llamadas lentas (cada una simula 200 ms de red) ===");
static async Task<string> Llamada(string nombre) { await Task.Delay(200); return nombre; }
var reloj = Stopwatch.StartNew();
await Llamada("A"); await Llamada("B"); await Llamada("C");                           // una detrás de otra
Console.WriteLine($"  En serie (await uno a uno)   : {reloj.ElapsedMilliseconds,4} ms");
reloj.Restart();
await Task.WhenAll(Llamada("A"), Llamada("B"), Llamada("C"));                         // las tres a la vez
Console.WriteLine($"  En paralelo (Task.WhenAll)   : {reloj.ElapsedMilliseconds,4} ms");
reloj.Restart();
Thread.Sleep(200); Thread.Sleep(200); Thread.Sleep(200);                              // bloqueante: el hilo no hace NADA útil mientras espera
Console.WriteLine($"  Bloqueando el hilo (Sleep)   : {reloj.ElapsedMilliseconds,4} ms");
Console.WriteLine("  async NO crea hilos: libera el hilo mientras se espera (E/S), y lo recupera cuando hay respuesta.");

// ================= 2) Qué hace await por dentro =================
Console.WriteLine("\n=== 2) Dónde se ejecuta cada tramo ===");
Console.WriteLine($"  Antes del await : hilo {Environment.CurrentManagedThreadId}");
await Task.Delay(10);
Console.WriteLine($"  Después         : hilo {Environment.CurrentManagedThreadId}  (en consola puede ser otro: no hay contexto que lo devuelva al original)");

// ================= 3) Excepciones en tareas =================
Console.WriteLine("\n=== 3) Excepciones ===");
static async Task Falla(string msg) { await Task.Delay(5); throw new InvalidOperationException(msg); }
try { await Falla("un fallo"); } catch (InvalidOperationException e) { Console.WriteLine($"  await propaga la excepción original: {e.Message}"); }
var varias = Task.WhenAll(Falla("fallo 1"), Falla("fallo 2"), Llamada("bien"));
try { await varias; } catch (Exception e) { Console.WriteLine($"  WhenAll con 2 fallos: await relanza SOLO UNA («{e.Message}»); las dos están en varias.Exception.InnerExceptions, que contiene {varias.Exception!.InnerExceptions.Count}"); }
Task olvidada = Falla("nadie me espera");                                              // "fire and forget": el error se pierde en silencio
await Task.Delay(30);
Console.WriteLine($"  Tarea no esperada: IsFaulted={olvidada.IsFaulted} — nadie vio el error. Regla: SIEMPRE await (o guarda el Task).");

// ================= 4) Cancelación cooperativa =================
Console.WriteLine("\n=== 4) CancellationToken ===");
static async Task<int> TrabajoLargo(CancellationToken ct)
{
    int pasos = 0;
    for (int i = 0; i < 100; i++) { ct.ThrowIfCancellationRequested(); await Task.Delay(20, ct); pasos++; }   // el trabajo DEBE mirar el token
    return pasos;
}
using (var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100)))            // se cancela solo a los 100 ms
{
    try { await TrabajoLargo(cts.Token); }
    catch (OperationCanceledException) { Console.WriteLine("  Cancelado por tiempo límite (CancelAfter): OperationCanceledException"); }
}
using (var manual = new CancellationTokenSource())
using (var enlazado = CancellationTokenSource.CreateLinkedTokenSource(manual.Token, new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token))
{
    var tarea = TrabajoLargo(enlazado.Token);
    await Task.Delay(50); manual.Cancel();                                              // el usuario pulsa «Cancelar»
    try { await tarea; } catch (OperationCanceledException) { Console.WriteLine($"  Cancelado a mano; tarea.IsCanceled={tarea.IsCanceled}"); }
}

// ================= 5) Progreso con IProgress<T> =================
Console.WriteLine("\n=== 5) Progreso ===");
var porcentajes = new ConcurrentQueue<int>();
IProgress<int> progreso = new Progress<int>(p => porcentajes.Enqueue(p));
for (int i = 1; i <= 5; i++) { await Task.Delay(5); progreso.Report(i * 20); }
await Task.Delay(50);
Console.WriteLine($"  Recibido: {string.Join("% ", porcentajes)}%");

// ================= 6) Primera que termina, tiempo límite =================
Console.WriteLine("\n=== 6) WhenAny y tiempo límite ===");
var lenta = Task.Delay(500).ContinueWith(_ => "lenta"); var rapida = Task.Delay(50).ContinueWith(_ => "rápida");
Task<string> ganadora = await Task.WhenAny(lenta, rapida);                             // WhenAny devuelve la tarea que acabó primero…
Console.WriteLine($"  La primera en terminar: {await ganadora}");                       // …y hay que hacerle await para leer su resultado
try { await Task.Delay(2000).WaitAsync(TimeSpan.FromMilliseconds(80)); }                // .NET 6+: tiempo límite sin código extra
catch (TimeoutException) { Console.WriteLine("  WaitAsync(timeout) → TimeoutException"); }

// ================= 7) Flujos asíncronos con cancelación =================
Console.WriteLine("\n=== 7) IAsyncEnumerable<T> ===");
static async IAsyncEnumerable<int> Lecturas([EnumeratorCancellation] CancellationToken ct = default)
{
    for (int i = 1; ; i++) { await Task.Delay(10, ct); yield return i * i; }          // flujo infinito
}
using var corte = new CancellationTokenSource();
var recibidas = new List<int>();
try { await foreach (int v in Lecturas(corte.Token)) { recibidas.Add(v); if (recibidas.Count == 5) corte.Cancel(); } }
catch (OperationCanceledException) { }
Console.WriteLine($"  Recibidos antes de cancelar: {string.Join(", ", recibidas)}");

// ================= 8) ValueTask y PeriodicTimer =================
Console.WriteLine("\n=== 8) ValueTask / PeriodicTimer ===");
var cache = new Dictionary<int, string>();
async ValueTask<string> Buscar(int id)                                                  // ValueTask: si el dato ya está, NO reserva un Task en el heap
{
    if (cache.TryGetValue(id, out var v)) return v;
    await Task.Delay(10); return cache[id] = $"dato-{id}";
}
Console.WriteLine($"  1.ª vez (asíncrona): {await Buscar(1)}  · 2.ª vez (síncrona, sin asignar Task): {await Buscar(1)}");
using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(30));
int ticks = 0; while (await timer.WaitForNextTickAsync() && ++ticks < 3) Console.WriteLine($"  tic {ticks}");

// ================= 9) El clásico DEADLOCK «sync over async» =================
Console.WriteLine("\n=== 9) Deadlock por bloquear con .Result (reproducido con un contexto tipo interfaz gráfica) ===");
// Las aplicaciones de ventanas (WPF, WinForms) tienen UN hilo de interfaz y un SynchronizationContext que devuelve ahí el código tras cada await.
// Simulamos uno: un solo hilo que procesa una cola de trabajos.
using var contexto = new ContextoUnHilo();
bool terminoSinBloqueo = contexto.EjecutarYEsperar(() =>
{
    // Este código corre «en el hilo de interfaz». Bloqueamos con .Result la tarea asíncrona:
    var tarea = Task.Delay(10).ContinueWith(_ => 0);
    Func<Task<int>> asincrona = async () => { await Task.Delay(10); return 42; };       // al terminar necesita volver al hilo de interfaz…
    return asincrona().Wait(500);                                                       // …que está bloqueado esperando aquí → nunca vuelve
}, tiempoMs: 1500);
Console.WriteLine($"  Con .Wait() dentro del contexto: terminó = {terminoSinBloqueo}  (false = DEADLOCK; se abandonó a los 500 ms)");
bool conAwait = await contexto.EjecutarAsync(async () => { await Task.Delay(10); return true; });
Console.WriteLine($"  Con await en vez de .Wait()    : terminó = {conAwait}");
Console.WriteLine("  Reglas: 1) no bloquees con .Result/.Wait()/GetAwaiter().GetResult()  2) async hasta arriba  3) en bibliotecas, ConfigureAwait(false).");

/// <summary>Un SynchronizationContext mínimo con UN hilo, como el hilo de interfaz de una aplicación de escritorio.</summary>
sealed class ContextoUnHilo : SynchronizationContext, IDisposable
{
    private readonly BlockingCollection<(SendOrPostCallback cb, object? estado)> _cola = new();
    private readonly Thread _hilo;
    public ContextoUnHilo() { _hilo = new Thread(Bucle) { IsBackground = true }; _hilo.Start(); }
    private void Bucle() { SetSynchronizationContext(this); foreach (var (cb, estado) in _cola.GetConsumingEnumerable()) cb(estado); }
    public override void Post(SendOrPostCallback d, object? state) => _cola.Add((d, state));
    public bool EjecutarYEsperar(Func<bool> trabajo, int tiempoMs)
    {
        var fin = new TaskCompletionSource<bool>();
        Post(_ => fin.SetResult(trabajo()), null);
        // Como el hilo está bloqueado dentro de trabajo(), dejamos de esperar pasado un rato.
        return fin.Task.Wait(tiempoMs) && fin.Task.Result;
    }
    public Task<bool> EjecutarAsync(Func<Task<bool>> trabajo)
    {
        var fin = new TaskCompletionSource<bool>();
        Post(async _ => fin.SetResult(await trabajo()), null);
        return fin.Task;
    }
    public void Dispose() => _cola.CompleteAdding();
}
