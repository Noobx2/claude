// Lección 17: asincronía y concurrencia
using System.Diagnostics;

// ---- async/await: esperar SIN bloquear el hilo ----
var watch = Stopwatch.StartNew();
string one = await DownloadAsync("a", 300);           // await: "pausa aquí y vuelve cuando termine"
string two = await DownloadAsync("b", 300);
Console.WriteLine($"En SECUENCIA: {one}, {two} -> ~{Round(watch.ElapsedMilliseconds)} ms");

watch.Restart();
Task<string> ta = DownloadAsync("a", 300);            // se lanzan las dos SIN esperar
Task<string> tb = DownloadAsync("b", 300);
string[] both = await Task.WhenAll(ta, tb);           // y se espera a ambas a la vez
Console.WriteLine($"En PARALELO : {string.Join(", ", both)} -> ~{Round(watch.ElapsedMilliseconds)} ms");

// WhenAny: la primera que termine
var fastest = await Task.WhenAny(DownloadAsync("lenta", 400), DownloadAsync("rápida", 100));
Console.WriteLine($"WhenAny: {await fastest}");

// ---- Cancelación ----
using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(150));    // se cancela a los 150 ms
try
{
    await LongWorkAsync(cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Cancelado a tiempo");
}

// ---- Excepciones en tareas: se lanzan al hacer await ----
try { await FailAsync(); }
catch (InvalidOperationException e) { Console.WriteLine($"Capturada tras await: {e.Message}"); }

// ---- Hilos y datos compartidos ----
int unsafeCounter = 0, safeCounter = 0;
var gate = new object();
Parallel.For(0, 100_000, _ =>
{
    unsafeCounter++;                                  // carrera de datos: se pierden incrementos
    lock (gate) { safeCounter++; }                    // lock: solo un hilo a la vez
});
Console.WriteLine($"sin lock: {unsafeCounter} (casi seguro < 100000) | con lock: {safeCounter}");

int atomic = 0;
Parallel.For(0, 100_000, _ => Interlocked.Increment(ref atomic));    // operación atómica, sin lock
Console.WriteLine($"Interlocked: {atomic}");

// ---- Canal productor/consumidor ----
var channel = System.Threading.Channels.Channel.CreateBounded<int>(2);   // cola con capacidad: da contrapresión
var producer = Task.Run(async () =>
{
    for (int i = 1; i <= 5; i++) await channel.Writer.WriteAsync(i);
    channel.Writer.Complete();
});
var consumed = new List<int>();
await foreach (var item in channel.Reader.ReadAllAsync()) consumed.Add(item);
await producer;
Console.WriteLine($"consumidos: {string.Join(",", consumed)}");

// ---- Reglas ----
Console.WriteLine("""
    1. async/await se propaga hacia arriba: un método que espera suele ser async Task.
    2. NUNCA .Result ni .Wait() sobre una tarea: pueden bloquear la aplicación (sobre todo en interfaces).
    3. async void solo para manejadores de eventos; en lo demás, async Task.
    4. Pasa siempre un CancellationToken a las operaciones largas.
    """);

static async Task<string> DownloadAsync(string name, int milliseconds)
{
    await Task.Delay(milliseconds);                   // simula una espera de red (no bloquea el hilo)
    return $"{name} listo";
}

static async Task LongWorkAsync(CancellationToken token)
{
    for (int i = 0; i < 10; i++)
    {
        token.ThrowIfCancellationRequested();         // comprobar si nos han pedido parar
        await Task.Delay(100, token);
    }
}

static async Task FailAsync()
{
    await Task.Delay(10);
    throw new InvalidOperationException("falló la tarea");
}

static long Round(long ms) => (ms + 50) / 100 * 100;  // redondea a centenas para que la salida sea estable
