// Lección 34: patrones de diseño de COMPORTAMIENTO (cómo colaboran los objetos)
// Ejecutar: dotnet run 34-patrones-comportamiento.cs

// ═══════════ 1. STRATEGY (estrategia) ═══════════
// Problema: un algoritmo con VARIAS variantes (descuentos, ordenaciones, rutas). Evita el "if/else" gigante.
// Solución: cada variante es una pieza intercambiable que cumple el mismo contrato.
Console.WriteLine("── Strategy: algoritmos intercambiables ──");
var cart = new Cart([("Teclado", 60m), ("Ratón", 25m), ("Monitor", 140m)]);
foreach (IDiscount discount in new IDiscount[] { new NoDiscount(), new PercentDiscount(10), new BulkDiscount(200, 30) })
    Console.WriteLine($"   {discount.Name,-26} total = {cart.Total(discount),7:F2} €");
// En C# moderno una estrategia pequeña puede ser SOLO una función (delegado):
Func<decimal, decimal> blackFriday = total => total * 0.7m;
Console.WriteLine($"   función lambda (viernes negro) total = {blackFriday(cart.Total(new NoDiscount())),7:F2} €");

// ═══════════ 2. OBSERVER (observador) ═══════════
// Problema: cuando algo cambia, OTROS deben enterarse, sin que el que cambia los conozca.
// En C# esto son los EVENTOS (lección 16). Aquí con una tienda de acciones.
Console.WriteLine("\n── Observer: avisar a los interesados ──");
var stock = new StockTicker("ACME", 100m);
stock.PriceChanged += (symbol, old, current) => Console.WriteLine($"   [pantalla] {symbol}: {old} -> {current}");
stock.PriceChanged += (symbol, old, current) => { if (current < old * 0.9m) Console.WriteLine($"   [alerta] ¡{symbol} cae más de un 10 %!"); };
stock.Update(105m);
stock.Update(90m);
// Con IObservable<T> (estándar para flujos) funciona igual; el suscriptor puede cancelar la suscripción.

// ═══════════ 3. COMMAND (comando) con DESHACER ═══════════
// Problema: convertir una ACCIÓN en un objeto para guardarla, deshacerla o ponerla en cola.
Console.WriteLine("\n── Command: acciones que se pueden deshacer ──");
var editor = new TextEditor();
var history = new CommandHistory();
history.Run(new AppendCommand(editor, "Hola"));
history.Run(new AppendCommand(editor, " mundo"));
history.Run(new UppercaseCommand(editor));
Console.WriteLine($"   texto: \"{editor.Text}\"");
history.Undo(); Console.WriteLine($"   deshacer -> \"{editor.Text}\"");
history.Undo(); Console.WriteLine($"   deshacer -> \"{editor.Text}\"");
history.Redo(); Console.WriteLine($"   rehacer  -> \"{editor.Text}\"");

// ═══════════ 4. STATE (estado) ═══════════
// Problema: un objeto cambia de comportamiento según su estado (pedido: nuevo, pagado, enviado, entregado).
Console.WriteLine("\n── State: el comportamiento depende del estado ──");
var order = new Order();
foreach (string action in new[] { "ship", "pay", "pay", "ship", "deliver", "cancel" })
{
    try { order.Handle(action); Console.WriteLine($"   {action,-8} -> estado: {order.StateName}"); }
    catch (InvalidOperationException e) { Console.WriteLine($"   {action,-8} -> ✗ {e.Message}"); }
}

// ═══════════ 5. CHAIN OF RESPONSIBILITY (cadena de responsabilidad) ═══════════
// Problema: una petición debe pasar por VARIOS pasos y cualquiera puede cortarla. Es el "middleware" de ASP.NET.
Console.WriteLine("\n── Chain of Responsibility: una tubería de pasos ──");
var pipeline = new PipelineBuilder()
    .Use(next => req => { Console.WriteLine($"   [1 registro] entra {req.Path}"); var r = next(req); Console.WriteLine($"   [1 registro] sale {r}"); return r; })
    .Use(next => req => req.User is null ? "401 No autenticado" : next(req))                  // puede cortar la cadena
    .Use(next => req => req.Path.StartsWith("/admin") && req.User != "admin" ? "403 Prohibido" : next(req))
    .Build(req => $"200 OK ({req.Path} para {req.User})");
foreach (var req in new Request[] { new("/inicio", "ana"), new("/inicio", null), new("/admin/panel", "ana"), new("/admin/panel", "admin") })
    Console.WriteLine($"   => {pipeline(req)}\n");

// ═══════════ 6. TEMPLATE METHOD ya visto en la lección 28 (clase abstracta con pasos fijos) ═══════════
// ═══════════ 7. ITERATOR ya visto: yield return / IEnumerable<T> (lección 12) ═══════════

// ═══════════ TIPOS ═══════════
interface IDiscount { string Name { get; } decimal Apply(decimal total); }
class NoDiscount : IDiscount { public string Name => "sin descuento"; public decimal Apply(decimal total) => total; }
class PercentDiscount(int percent) : IDiscount { public string Name => $"{percent} % de descuento"; public decimal Apply(decimal total) => total * (100 - percent) / 100; }
class BulkDiscount(decimal threshold, decimal off) : IDiscount
{
    public string Name => $"-{off} € si pasa de {threshold}";
    public decimal Apply(decimal total) => total >= threshold ? total - off : total;
}
class Cart(IEnumerable<(string Name, decimal Price)> items)
{
    public decimal Total(IDiscount discount) => discount.Apply(items.Sum(i => i.Price));    // no sabe QUÉ descuento es
}

delegate void PriceChangedHandler(string symbol, decimal oldPrice, decimal newPrice);
class StockTicker(string symbol, decimal price)
{
    public event PriceChangedHandler? PriceChanged;
    public void Update(decimal newPrice)
    {
        decimal old = price; price = newPrice;
        PriceChanged?.Invoke(symbol, old, newPrice);          // avisa a TODOS los suscritos
    }
}

class TextEditor { public string Text { get; set; } = ""; }
interface ICommand { void Execute(); void Undo(); }
class AppendCommand(TextEditor editor, string text) : ICommand
{
    public void Execute() => editor.Text += text;
    public void Undo() => editor.Text = editor.Text[..^text.Length];
}
class UppercaseCommand(TextEditor editor) : ICommand
{
    private string _before = "";
    public void Execute() { _before = editor.Text; editor.Text = editor.Text.ToUpper(); }
    public void Undo() => editor.Text = _before;                     // guarda lo necesario para volver atrás
}
class CommandHistory
{
    private readonly Stack<ICommand> _done = new(), _undone = new();
    public void Run(ICommand c) { c.Execute(); _done.Push(c); _undone.Clear(); }
    public void Undo() { if (_done.TryPop(out var c)) { c.Undo(); _undone.Push(c); } }
    public void Redo() { if (_undone.TryPop(out var c)) { c.Execute(); _done.Push(c); } }
}

class Order
{
    private IOrderState _state = new NewState();
    public string StateName => _state.GetType().Name.Replace("State", "");
    public void Handle(string action) => _state = _state.Handle(action);
}
interface IOrderState { IOrderState Handle(string action); }
class NewState : IOrderState
{
    public IOrderState Handle(string action) => action switch
    {
        "pay" => new PaidState(),
        "cancel" => new CancelledState(),
        _ => throw new InvalidOperationException($"un pedido nuevo no admite '{action}'"),
    };
}
class PaidState : IOrderState
{
    public IOrderState Handle(string action) => action switch
    {
        "ship" => new ShippedState(),
        "cancel" => new CancelledState(),
        _ => throw new InvalidOperationException($"un pedido pagado no admite '{action}'"),
    };
}
class ShippedState : IOrderState
{
    public IOrderState Handle(string action) => action == "deliver" ? new DeliveredState()
        : throw new InvalidOperationException($"un pedido enviado no admite '{action}'");
}
class DeliveredState : IOrderState { public IOrderState Handle(string action) => throw new InvalidOperationException("el pedido ya terminó"); }
class CancelledState : IOrderState { public IOrderState Handle(string action) => throw new InvalidOperationException("el pedido está cancelado"); }

record Request(string Path, string? User);
delegate string Handler(Request request);
class PipelineBuilder
{
    private readonly List<Func<Handler, Handler>> _steps = [];
    public PipelineBuilder Use(Func<Handler, Handler> step) { _steps.Add(step); return this; }
    public Handler Build(Handler last)
    {
        Handler current = last;
        foreach (var step in Enumerable.Reverse(_steps)) current = step(current);          // se encadenan de atrás hacia delante
        return current;
    }
}
