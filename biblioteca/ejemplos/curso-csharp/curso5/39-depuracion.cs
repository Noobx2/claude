#:property PublishAot=false
#:package Microsoft.Extensions.Logging.Console@10.0.0

#define TRAZA_EXTRA            // símbolo definido en el propio archivo (debe ir antes de cualquier código)

using System.Diagnostics;
using Microsoft.Extensions.Logging;

// ---- 1) Símbolos de compilación y directivas #if ----
// DEBUG existe solo en la configuración Debug; en Release este bloque ni siquiera se compila.
#if DEBUG
Console.WriteLine("Configuración: DEBUG (hay información para el depurador, sin optimizar)");
#else
Console.WriteLine("Configuración: RELEASE (optimizada)");
#endif
#if TRAZA_EXTRA
Console.WriteLine("Símbolo TRAZA_EXTRA definido con #define");
#endif

// ---- 2) Debug.* solo se ejecuta en Debug; Trace.* también en Release ----
Debug.WriteLine("Esto solo aparece en el depurador (Debug)");
Debug.Assert(1 + 1 == 2, "Esta aserción se cumple");
Console.WriteLine($"Debugger.IsAttached = {Debugger.IsAttached}  (true solo si lo ejecutas con F5 en el IDE)");

// ---- 3) Trace con "listeners": ¿a dónde van los mensajes? ----
string archivoTraza = Path.Combine(Path.GetTempPath(), "traza-demo.log");
File.Delete(archivoTraza);
Trace.Listeners.Clear();
Trace.Listeners.Add(new ConsoleTraceListener());                                         // a la consola
var enArchivo = new TextWriterTraceListener(archivoTraza);                               // y a un archivo
Trace.Listeners.Add(enArchivo);
Trace.AutoFlush = true;
Trace.TraceInformation("Inicio del proceso");
Trace.TraceWarning("Queda poco espacio en disco");
Trace.TraceError("No se pudo abrir la conexión");
Trace.WriteLineIf(Environment.ProcessorCount > 0, "Mensaje condicional (la condición era cierta)");
Console.WriteLine($"El archivo de traza tiene {File.ReadAllLines(archivoTraza).Length} líneas");

// ---- 4) TraceSwitch: subir o bajar el nivel de detalle sin recompilar ----
var nivel = new TraceSwitch("General", "Nivel de detalle") { Level = TraceLevel.Warning };
if (nivel.TraceInfo) Trace.WriteLine("(esto NO sale: el nivel es Warning)");
if (nivel.TraceWarning) Trace.WriteLine("(esto SÍ sale: Warning está activado)");

// ---- 5) Atributos para el depurador ----
var pedido = new Pedido(1042, "Ana", 3, 19.99m);
Console.WriteLine($"DebuggerDisplay mostraría en la ventana Locales: {pedido.GetType().GetCustomAttributes(typeof(DebuggerDisplayAttribute), false).Cast<DebuggerDisplayAttribute>().First().Value}");
Registrar("Mensaje de diagnóstico que solo se compila en Debug");   // [Conditional("DEBUG")]: la LLAMADA desaparece en Release

// ---- 6) Registro profesional: ILogger con niveles, plantillas y ámbitos ----
using ILoggerFactory fabrica = LoggerFactory.Create(b => b
    .SetMinimumLevel(LogLevel.Debug)
    .AddSimpleConsole(o => { o.SingleLine = true; o.IncludeScopes = true; o.TimestampFormat = null; }));
ILogger log = fabrica.CreateLogger("Tienda.Pedidos");

log.LogDebug("Detalle fino para depurar");
log.LogInformation("Pedido {Numero} de {Cliente} por {Total:C}", pedido.Numero, pedido.Cliente, pedido.Total);
using (log.BeginScope("PedidoId={Id}", pedido.Numero))                        // todo lo que se escriba aquí lleva ese dato
{
    log.LogWarning("Stock bajo para {Producto}", "Teclado");
    try { throw new InvalidOperationException("Pago rechazado"); }
    catch (Exception ex) { log.LogError(ex, "Falló el cobro"); }               // pasa la excepción entera, no solo el texto
}

// ---- 7) Medir con Stopwatch: el depurador no sirve para medir tiempos ----
var reloj = Stopwatch.StartNew();
Thread.Sleep(50);
Console.WriteLine($"Tiempo medido: {reloj.ElapsedMilliseconds >= 50}");
enArchivo.Dispose();

static void Registrar(string texto) => Prueba.Escribir(texto);

[DebuggerDisplay("Pedido {Numero} — {Cliente} — {Total} €")]   // lo que ve el programador al pasar el ratón sobre la variable
public record Pedido(int Numero, string Cliente, int Cantidad, decimal Precio)
{
    public decimal Total => Cantidad * Precio;
}

public static class Prueba
{
    [Conditional("DEBUG")]                                      // en Release el compilador elimina TODAS las llamadas
    public static void Escribir(string texto) => Console.WriteLine($"[solo-debug] {texto}");
}
