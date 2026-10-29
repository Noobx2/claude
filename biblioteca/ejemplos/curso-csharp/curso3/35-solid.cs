// Lección 35: los cinco principios SOLID, cada uno con el código MAL y el código BIEN
// Ejecutar: dotnet run 35-solid.cs

// ══════════════════════════════════════════════════════════════════════════════
// S — Single Responsibility (responsabilidad única): una clase, UNA razón para cambiar
// ══════════════════════════════════════════════════════════════════════════════
Console.WriteLine("S · Una clase, una razón para cambiar");
// MAL: InvoiceBad calcula, imprime Y guarda. Si cambia el formato, el cálculo o la base de datos, se toca la misma clase.
var badInvoice = new InvoiceBad(100m);
badInvoice.CalculateAndPrintAndSave();
// BIEN: cada responsabilidad en su clase; se prueban y cambian por separado.
var invoice = new Invoice(100m);
Console.WriteLine($"   {new InvoicePrinter().Print(invoice)}");
new InvoiceStore().Save(invoice);

// ══════════════════════════════════════════════════════════════════════════════
// O — Open/Closed (abierto/cerrado): abierto a EXTENSIÓN, cerrado a MODIFICACIÓN
// ══════════════════════════════════════════════════════════════════════════════
Console.WriteLine("\nO · Añadir una figura sin tocar el calculador");
// MAL: AreaCalculatorBad tiene un "if" por figura: cada figura nueva obliga a editarlo (y a re-probarlo todo).
Console.WriteLine($"   mal  : {AreaCalculatorBad.Area("circulo", 2):F2}");
// BIEN: cada figura sabe su área. Añadir Triángulo NO modifica nada de lo existente.
IShape[] shapes = [new Circle(2), new Rect(3, 4), new Triangle(3, 4)];
Console.WriteLine($"   bien : {string.Join(", ", shapes.Select(s => $"{s.GetType().Name} {s.Area():F2}"))}  (Triangle se añadió sin editar Calculator: {new Calculator().TotalArea(shapes):F2})");

// ══════════════════════════════════════════════════════════════════════════════
// L — Liskov (sustitución): una subclase debe poder usarse DONDE se espera la base, sin sorpresas
// ══════════════════════════════════════════════════════════════════════════════
Console.WriteLine("\nL · La subclase no debe romper lo que promete la base");
// MAL: un Cuadrado "es un" Rectángulo en matemáticas, pero cambiar el ancho de un Cuadrado cambia también el alto.
RectBad r = new SquareBad();
r.Width = 5; r.Height = 2;
Console.WriteLine($"   mal  : Width=5, Height=2 -> área {r.Area}   (¡debería ser 10!) El Cuadrado rompió la promesa del Rectángulo.");
// BIEN: no modelar "es un" donde no se cumple el comportamiento. Dos tipos independientes que comparten un contrato.
IShape2 rect = new RectOk(5, 2), square = new SquareOk(3);
Console.WriteLine($"   bien : {rect.Area} y {square.Area}   (ninguno promete algo que el otro no cumpla)");

// ══════════════════════════════════════════════════════════════════════════════
// I — Interface Segregation: interfaces PEQUEÑAS, que nadie implemente lo que no usa
// ══════════════════════════════════════════════════════════════════════════════
Console.WriteLine("\nI · Interfaces pequeñas");
// MAL: IWorkerBad obliga al Robot a implementar Eat() y Sleep(), que no tiene sentido (lanza excepciones).
try { new RobotBad().Eat(); } catch (NotSupportedException e) { Console.WriteLine($"   mal  : {e.Message}"); }
// BIEN: contratos separados; cada clase cumple solo los que le corresponden.
IWorkable[] workers = [new Human(), new Robot()];
foreach (var w in workers) Console.WriteLine($"   bien : {w.Work()}");
if (workers[0] is IFeedable f) Console.WriteLine($"   bien : {f.Eat()}");

// ══════════════════════════════════════════════════════════════════════════════
// D — Dependency Inversion: depende de ABSTRACCIONES, no de clases concretas
// ══════════════════════════════════════════════════════════════════════════════
Console.WriteLine("\nD · Depender de contratos");
// MAL: AlertBad hace "new SmtpSender()": atada a un envío real; no se puede probar ni cambiar a SMS.
// BIEN: AlertService recibe un IMessageSender; en producción es SMTP, en pruebas es un falso.
var fake = new FakeSender();
new AlertService(fake).Raise("Disco al 95 %");
Console.WriteLine($"   bien : con un falso se comprueba el mensaje sin enviar nada -> '{fake.Last}'");
new AlertService(new SmsSender()).Raise("Disco al 95 %");

Console.WriteLine("""

    ── Resumen ──
    S  Cada clase hace UNA cosa.                          -> clases pequeñas y fáciles de probar
    O  Se añade comportamiento nuevo con código nuevo.     -> sin romper lo que ya funciona
    L  Una subclase cumple lo que la base promete.         -> la herencia no da sorpresas
    I  Muchas interfaces pequeñas mejor que una gigante.   -> nadie implementa lo que no usa
    D  Depender de contratos, no de clases concretas.      -> piezas intercambiables y probables
    """);

// ═══════════ TIPOS ═══════════
class InvoiceBad(decimal net)
{
    public void CalculateAndPrintAndSave()
    {
        decimal total = net * 1.21m;                                   // 1) calcula
        Console.WriteLine($"   mal  : factura de {total:F2} € impresa");  // 2) imprime
        Console.WriteLine("   mal  : guardada en la base de datos");      // 3) guarda   <- tres razones para cambiar
    }
}
record Invoice(decimal Net) { public decimal Total => Net * 1.21m; }
class InvoicePrinter { public string Print(Invoice i) => $"bien : factura de {i.Total:F2} € impresa"; }
class InvoiceStore { public void Save(Invoice i) => Console.WriteLine("   bien : guardada en la base de datos"); }

static class AreaCalculatorBad
{
    public static double Area(string shape, double size) => shape switch
    {
        "circulo" => Math.PI * size * size,
        "cuadrado" => size * size,
        _ => throw new ArgumentException("figura desconocida"),    // cada figura nueva = tocar aquí
    };
}
interface IShape { double Area(); }
record Circle(double R) : IShape { public double Area() => Math.PI * R * R; }
record Rect(double W, double H) : IShape { public double Area() => W * H; }
record Triangle(double B, double H) : IShape { public double Area() => B * H / 2; }
class Calculator { public double TotalArea(IEnumerable<IShape> shapes) => shapes.Sum(s => s.Area()); }

class RectBad { public virtual int Width { get; set; } public virtual int Height { get; set; } public int Area => Width * Height; }
class SquareBad : RectBad
{
    public override int Width { get => base.Width; set { base.Width = value; base.Height = value; } }     // rompe la promesa
    public override int Height { get => base.Height; set { base.Height = value; base.Width = value; } }
}
interface IShape2 { int Area { get; } }
record RectOk(int W, int H) : IShape2 { public int Area => W * H; }
record SquareOk(int Side) : IShape2 { public int Area => Side * Side; }

interface IWorkerBad { string Work(); string Eat(); string Sleep(); }
class RobotBad : IWorkerBad
{
    public string Work() => "trabaja";
    public string Eat() => throw new NotSupportedException("un robot no come: ¡esta clase no debería tener que implementar Eat()!");
    public string Sleep() => throw new NotSupportedException("un robot no duerme");
}
interface IWorkable { string Work(); }
interface IFeedable { string Eat(); }
class Human : IWorkable, IFeedable { public string Work() => "la persona trabaja"; public string Eat() => "la persona come"; }
class Robot : IWorkable { public string Work() => "el robot trabaja 24 h"; }

interface IMessageSender { void Send(string text); }
class SmsSender : IMessageSender { public void Send(string text) => Console.WriteLine($"   bien : [SMS real] {text}"); }
class FakeSender : IMessageSender { public string? Last; public void Send(string text) => Last = text; }
class AlertService(IMessageSender sender) { public void Raise(string text) => sender.Send($"ALERTA: {text}"); }
