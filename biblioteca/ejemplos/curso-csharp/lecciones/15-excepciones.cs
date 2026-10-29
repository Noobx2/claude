// Lección 15: excepciones
// try / catch / finally
try
{
    int zero = 0;
    Console.WriteLine(10 / zero);
}
catch (DivideByZeroException e)
{
    Console.WriteLine($"Capturada: {e.GetType().Name} - {e.Message}");
}
finally
{
    Console.WriteLine("finally: se ejecuta SIEMPRE (con o sin error)");
}

// Varios catch: de lo más específico a lo más general
foreach (var text in new[] { "42", "abc", null, "99999999999" })
{
    try
    {
        Console.WriteLine(int.Parse(text!) * 2);
    }
    catch (FormatException) { Console.WriteLine($"'{text}' no es un número"); }
    catch (OverflowException) { Console.WriteLine($"'{text}' no cabe en un int"); }
    catch (ArgumentNullException) { Console.WriteLine("era null"); }
    catch (Exception e) { Console.WriteLine($"otro error: {e.Message}"); }
}

// Filtros con when
try { Process(-5); }
catch (ArgumentException e) when (e.ParamName == "amount") { Console.WriteLine($"filtro when: {e.Message}"); }

// Excepciones propias
try { new Wallet(10m).Pay(50m); }
catch (InsufficientFundsException e) { Console.WriteLine($"{e.Message} (faltan {e.Missing})"); }

// throw frente a throw e: throw conserva la traza original
try { Outer(); }
catch (Exception e) { Console.WriteLine($"traza conservada: {e.StackTrace!.Contains("Inner")}"); }

// Excepción interna: envolver sin perder la causa
try { Wrap(); }
catch (Exception e) { Console.WriteLine($"{e.Message} <- causa: {e.InnerException?.Message}"); }

// using: libera el recurso aunque haya error (IDisposable)
using (var resource = new Resource("A"))
{
    Console.WriteLine("trabajando con A");
}
Console.WriteLine("A ya liberado");
{
    using var resourceB = new Resource("B");           // using declaración: libera al terminar el bloque
    Console.WriteLine("trabajando con B");
}

// Cuándo NO usar excepciones: para flujo normal
Console.WriteLine(int.TryParse("12x", out var value) ? "ok" : "TryParse: sin excepción, sin coste");

// Excepciones frecuentes
Console.WriteLine(string.Join(", ", new[] { "NullReferenceException", "ArgumentException", "InvalidOperationException", "KeyNotFoundException", "IndexOutOfRangeException", "FileNotFoundException", "NotImplementedException" }));

static void Process(int amount)
{
    if (amount < 0) throw new ArgumentException("El importe no puede ser negativo", nameof(amount));
}
static void Outer() { try { Inner(); } catch { throw; } }       // "throw;" relanza sin perder la traza
static void Inner() => throw new InvalidOperationException("fallo interno");
static void Wrap()
{
    try { int.Parse("x"); }
    catch (FormatException e) { throw new InvalidOperationException("No se pudo leer la configuración", e); }
}

class InsufficientFundsException(decimal missing) : Exception($"Saldo insuficiente")
{
    public decimal Missing { get; } = missing;
}
class Wallet(decimal balance)
{
    public void Pay(decimal amount)
    {
        if (amount > balance) throw new InsufficientFundsException(amount - balance);
        balance -= amount;
    }
}
class Resource(string name) : IDisposable
{
    public void Dispose() => Console.WriteLine($"Dispose de {name}");
}
