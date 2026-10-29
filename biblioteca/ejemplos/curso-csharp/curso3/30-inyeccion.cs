// Lección 30: inyección de dependencias (DI) a fondo
// Ejecutar: dotnet run 30-inyeccion.cs
#:package Microsoft.Extensions.DependencyInjection@10.0.0
#:package Microsoft.Extensions.Options@10.0.0
#:package Microsoft.Extensions.Configuration@10.0.0
#:package Microsoft.Extensions.Options.ConfigurationExtensions@10.0.0
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

// ═══════════════════════ 1. EL PROBLEMA: dependencias escondidas ═══════════════════════
// MAL: la clase crea lo que necesita con "new". Queda atada a EmailSender para siempre.
//   class OrderServiceBad { void Place() { new EmailSender().Send(...); } }      // no se puede probar sin enviar correos reales
//
// BIEN: la clase DECLARA lo que necesita (constructor) y se lo ENTREGAN desde fuera.

// ═══════════════════════ 2. DI MANUAL ("pobre") ═══════════════════════
// Antes de usar un contenedor conviene entender que NO es magia: es construir los objetos en el orden correcto.
Console.WriteLine("── 1. DI manual: componer a mano ──");
var logger = new ConsoleLogger();
var sender = new EmailSender(logger);
var service = new OrderService(sender, logger);      // las dependencias se entregan POR EL CONSTRUCTOR
service.Place("Teclado");

// ═══════════════════════ 3. TRES FORMAS DE INYECTAR ═══════════════════════
Console.WriteLine("\n── 2. Constructor / propiedad / método ──");
Console.WriteLine("""
    Constructor : la dependencia es OBLIGATORIA (lo normal). new OrderService(sender, logger)
    Propiedad   : OPCIONAL, se puede cambiar después. service.Logger = otro   (poco usada)
    Método      : solo para UNA operación. service.Export(IExporter exporter)
    """);

// ═══════════════════════ 4. EL CONTENEDOR ═══════════════════════
Console.WriteLine("── 3. Contenedor: registrar y resolver ──");
var services = new ServiceCollection();
services.AddSingleton<ILogger, ConsoleLogger>();          // 1 instancia para toda la aplicación
services.AddScoped<IUnitOfWork, UnitOfWork>();            // 1 instancia POR ÁMBITO (en una web: por petición)
services.AddTransient<ISender, EmailSender>();            // 1 NUEVA cada vez que se pide
services.AddTransient<OrderService>();
// La clase concreta se pide por su tipo; el contenedor crea sus dependencias SOLO, en cascada.

using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
using (var scope = provider.CreateScope())
{
    var o1 = scope.ServiceProvider.GetRequiredService<OrderService>();
    var o2 = scope.ServiceProvider.GetRequiredService<OrderService>();
    Console.WriteLine($"   Transient distinto cada vez: {!ReferenceEquals(o1, o2)}");
    Console.WriteLine($"   Scoped igual dentro del ámbito: {ReferenceEquals(o1.Unit, o2.Unit)}");
    Console.WriteLine($"   Singleton igual siempre: {ReferenceEquals(provider.GetRequiredService<ILogger>(), scope.ServiceProvider.GetRequiredService<ILogger>())}");
}

// ═══════════════════════ 5. TRAMPA CLÁSICA: dependencia "cautiva" ═══════════════════════
// Un Singleton que recibe un Scoped lo "secuestra": el Scoped viviría para siempre.
Console.WriteLine("\n── 4. Dependencia cautiva (captive dependency) ──");
var bad = new ServiceCollection();
bad.AddScoped<IUnitOfWork, UnitOfWork>();
bad.AddSingleton<CachedReport>();                         // Singleton que depende de un Scoped
try
{
    using var badProvider = bad.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
}
catch (AggregateException e)
{
    Console.WriteLine("   El contenedor lo detectó al construirse:");
    Console.WriteLine("   " + e.InnerExceptions[0].Message.Split('(')[0].Trim());
}
catch (InvalidOperationException e) { Console.WriteLine("   " + e.Message); }

// ═══════════════════════ 6. VARIAS IMPLEMENTACIONES ═══════════════════════
Console.WriteLine("\n── 5. Varias implementaciones de una interfaz ──");
var multi = new ServiceCollection();
multi.AddSingleton<INotifier, EmailNotifier>();
multi.AddSingleton<INotifier, SmsNotifier>();
multi.AddKeyedSingleton<INotifier, EmailNotifier>("principal");        // con nombre (clave)
multi.AddKeyedSingleton<INotifier, SmsNotifier>("urgente");
multi.AddSingleton<AlertService>();
using var multiProvider = multi.BuildServiceProvider();
var alert = multiProvider.GetRequiredService<AlertService>();
alert.NotifyAll("Servidor caído");                                    // recibe IEnumerable<INotifier>: TODAS
Console.WriteLine($"   La última registrada gana si se pide una sola: {multiProvider.GetRequiredService<INotifier>().GetType().Name}");
Console.WriteLine($"   Por clave 'urgente': {multiProvider.GetRequiredKeyedService<INotifier>("urgente").GetType().Name}");

// ═══════════════════════ 7. FÁBRICAS Y REGISTRO CON LÓGICA ═══════════════════════
Console.WriteLine("\n── 6. Registro con fábrica (decide al crear) ──");
var factory = new ServiceCollection();
factory.AddSingleton<ILogger, ConsoleLogger>();
bool useSms = true;                                                    // (en la vida real vendría de la configuración)
factory.AddSingleton<INotifier>(sp => useSms ? new SmsNotifier() : new EmailNotifier());
Console.WriteLine($"   Se eligió: {factory.BuildServiceProvider().GetRequiredService<INotifier>().GetType().Name}");

// ═══════════════════════ 8. PATRÓN OPCIONES (configuración tipada) ═══════════════════════
Console.WriteLine("\n── 7. Opciones: configuración tipada e inyectada ──");
var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>   // en la vida real: appsettings.json
{
    ["Mail:Host"] = "smtp.ejemplo.es", ["Mail:Port"] = "587", ["Mail:From"] = "no-reply@ejemplo.es",
}).Build();

var opts = new ServiceCollection();
opts.AddOptions<MailOptions>().Bind(config.GetSection("Mail"))
    .Validate(o => o.Port is > 0 and < 65536, "Puerto no válido")        // la validación sucede al ARRANCAR, no al fallar en producción
    .ValidateOnStart();
opts.AddSingleton<MailClient>();
using var optProvider = opts.BuildServiceProvider();
Console.WriteLine("   " + optProvider.GetRequiredService<MailClient>().Describe());

// ═══════════════════════ 9. ANTIPATRÓN: Service Locator ═══════════════════════
Console.WriteLine("\n── 8. Antipatrón: pedir las cosas al contenedor desde dentro ──");
Console.WriteLine("""
       class Malo(IServiceProvider sp) { void Hacer() { var x = sp.GetRequiredService<IAlgo>(); } }
       Esconde qué necesita la clase (no se ve en el constructor) y falla en ejecución, no al compilar.
       Úsalo solo en la raíz de la aplicación, nunca dentro de la lógica.
    """);

// ═══════════════════════ 10. PRUEBAS: aquí brilla la DI ═══════════════════════
Console.WriteLine("── 9. En una prueba se entrega un falso ──");
var fakeSender = new RecordingSender();
new OrderService(fakeSender, new ConsoleLogger()).Place("Ratón");
Console.WriteLine($"   El falso recibió: [{string.Join(" | ", fakeSender.Messages)}]  (no se envió ningún correo real)");

// ═══════════════════════ TIPOS ═══════════════════════
interface ILogger { void Log(string message); }
class ConsoleLogger : ILogger { public void Log(string message) => Console.WriteLine($"   [log] {message}"); }

interface ISender { void Send(string message); }
class EmailSender(ILogger logger) : ISender
{
    public void Send(string message) { logger.Log($"enviando correo: {message}"); }
}
class RecordingSender : ISender
{
    public List<string> Messages { get; } = [];
    public void Send(string message) => Messages.Add(message);
}

interface IUnitOfWork { Guid Id { get; } }
class UnitOfWork : IUnitOfWork { public Guid Id { get; } = Guid.NewGuid(); }

class OrderService(ISender sender, ILogger logger, IUnitOfWork? unit = null)   // "= null": dependencia OPCIONAL
{
    public IUnitOfWork? Unit => unit;
    public void Place(string product) { logger.Log($"pedido de {product}"); sender.Send($"Pedido de {product} confirmado"); }
}

class CachedReport(IUnitOfWork unit) { public Guid Id => unit.Id; }

interface INotifier { void Notify(string message); }
class EmailNotifier : INotifier { public void Notify(string message) => Console.WriteLine($"   [email] {message}"); }
class SmsNotifier : INotifier { public void Notify(string message) => Console.WriteLine($"   [sms] {message}"); }
class AlertService(IEnumerable<INotifier> notifiers)
{
    public void NotifyAll(string message) { foreach (var n in notifiers) n.Notify(message); }
}

class MailOptions { public string Host { get; set; } = ""; public int Port { get; set; } public string From { get; set; } = ""; }
class MailClient(IOptions<MailOptions> options)
{
    public string Describe() => $"Servidor {options.Value.Host}:{options.Value.Port}, remitente {options.Value.From}";
}
