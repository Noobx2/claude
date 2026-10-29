using Microsoft.Extensions.DependencyInjection;

// 1) Registrar: decir "cuando alguien pida ISender, entrega EmailSender"
var services = new ServiceCollection();
services.AddSingleton<IClock, SystemClock>();      // una sola instancia para toda la aplicación
services.AddScoped<IRequestLog, RequestLog>();     // una instancia por "ámbito" (en una web, por petición)
services.AddTransient<ISender, EmailSender>();     // una instancia NUEVA cada vez que se pide
services.AddTransient<OrderService>();             // las clases también se registran

// 2) Construir el contenedor
using var provider = services.BuildServiceProvider();

// 3) Pedir: el contenedor crea OrderService y le entrega sus dependencias por el constructor
using (var scope = provider.CreateScope())          // un ámbito = una petición
{
    var first = scope.ServiceProvider.GetRequiredService<OrderService>();
    var second = scope.ServiceProvider.GetRequiredService<OrderService>();
    first.PlaceOrder("Teclado");
    Console.WriteLine($"mismo ámbito, IRequestLog compartido: {ReferenceEquals(first.Log, second.Log)}");
    Console.WriteLine($"mismo ámbito, ISender distinto cada vez: {!ReferenceEquals(first.Sender, second.Sender)}");
    Console.WriteLine($"Singleton compartido: {ReferenceEquals(first.Clock, second.Clock)}");
}
using (var otherScope = provider.CreateScope())
{
    var other = otherScope.ServiceProvider.GetRequiredService<OrderService>();
    Console.WriteLine($"otro ámbito, IRequestLog nuevo: {other.Log.Id}");
}

// 4) Por qué sirve: en las pruebas se entrega un falso sin tocar OrderService
var fake = new OrderService(new SystemClock(), new RequestLog(), new FakeSender());
fake.PlaceOrder("Prueba");

interface IClock { DateTime Now { get; } }
class SystemClock : IClock { public DateTime Now => DateTime.Now; }

interface IRequestLog { int Id { get; } void Add(string line); }
class RequestLog : IRequestLog
{
    private static int _next;
    public int Id { get; } = Interlocked.Increment(ref _next);
    public void Add(string line) => Console.WriteLine($"  [log {Id}] {line}");
}

interface ISender { void Send(string message); }
class EmailSender : ISender { public void Send(string message) => Console.WriteLine($"  [email] {message}"); }
class FakeSender : ISender { public void Send(string message) => Console.WriteLine($"  [falso, no envía nada] {message}"); }

// El constructor declara lo que necesita; el contenedor lo entrega
class OrderService(IClock clock, IRequestLog log, ISender sender)
{
    public IClock Clock => clock;
    public IRequestLog Log => log;
    public ISender Sender => sender;

    public void PlaceOrder(string product)
    {
        log.Add($"pedido de {product}");
        sender.Send($"Pedido de {product} confirmado");
    }
}
