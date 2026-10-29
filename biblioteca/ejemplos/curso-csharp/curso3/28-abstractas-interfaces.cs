// Lección 28: clases abstractas, interfaces y composición
// Ejecutar: dotnet run 28-abstractas-interfaces.cs

// ═══════════════════════ 1. CLASE ABSTRACTA ═══════════════════════
// Una clase abstracta es una plantilla INCOMPLETA: define lo común y obliga a las hijas a rellenar lo que falta.
// No se puede crear con "new" (daría error de compilación: "Cannot create an instance of the abstract type").
// var p = new PaymentMethod();   // <-- no compila

Console.WriteLine("── Clase abstracta + patrón Plantilla (Template Method) ──");
PaymentMethod[] methods = [new CreditCard("4111111111111111"), new BankTransfer("ES9121000418450200051332"), new CreditCard("1234")];
foreach (var method in methods)
{
    bool ok = method.Pay(120.50m);          // Pay() está escrito UNA vez en la clase base; cada hija aporta sus pasos
    Console.WriteLine($"   resultado: {(ok ? "COBRADO" : "RECHAZADO")}\n");
}
Console.WriteLine($"Total de pagos intentados: {PaymentMethod.Attempts}   (estado compartido en la base)\n");

// ═══════════════════════ 2. INTERFAZ ═══════════════════════
// Una interfaz es un CONTRATO: dice QUÉ se puede hacer, no CÓMO ni con qué datos. Una clase puede cumplir VARIAS.
Console.WriteLine("── Interfaces: capacidades que se combinan ──");
var robot = new RobotDuck();
foreach (object thing in new object[] { new Duck(), new Fish(), robot })
{
    if (thing is ISwimmer s) s.Swim();
    if (thing is IFlyer f) f.Fly();
    if (thing is IDescribable d) Console.WriteLine($"   {d.Describe()}");        // implementación por defecto de la interfaz
}

// ═══════════════════════ 3. ¿CUÁL USAR? ═══════════════════════
Console.WriteLine("""

    ── Abstracta o interfaz ──
    Clase abstracta : relación "ES UN" con código y datos compartidos (solo se hereda de UNA).
    Interfaz        : capacidad "PUEDE HACER" sin código compartido (se cumplen VARIAS).
    Regla práctica  : empieza por una interfaz; usa una clase abstracta solo si necesitas compartir código o estado.
    """);

// ═══════════════════════ 4. HERENCIA FRENTE A COMPOSICIÓN ═══════════════════════
// Problema de la herencia: ¿un pato de goma es un "Pato" que vuela? Se hereda lo que NO se quiere.
// Solución: COMPONER comportamientos (piezas intercambiables) en vez de heredarlos.
Console.WriteLine("── Composición: el comportamiento es una pieza que se enchufa ──");
var wild = new Animal("Pato salvaje", new FlyWithWings(), new Quack());
var rubber = new Animal("Pato de goma", new CannotFly(), new Squeak());
foreach (var a in new[] { wild, rubber }) a.Show();
rubber.SetFly(new FlyWithRocket());                 // el comportamiento se cambia EN EJECUCIÓN: con herencia es imposible
rubber.Show();

// ═══════════════════════ TIPOS ═══════════════════════

abstract class PaymentMethod
{
    private static int _attempts;
    public static int Attempts => _attempts;

    // Método plantilla: fija el ORDEN de los pasos y nadie puede alterarlo (no es virtual)
    public bool Pay(decimal amount)
    {
        _attempts++;
        Console.WriteLine($"Pago de {amount:F2} € con {Name}");
        if (!IsValid(out string reason)) { Console.WriteLine($"   ✗ {reason}"); return false; }
        Charge(amount);
        Log();
        return true;
    }

    protected abstract string Name { get; }                       // abstracto: sin cuerpo, obligatorio en las hijas
    protected abstract bool IsValid(out string reason);
    protected abstract void Charge(decimal amount);

    protected virtual void Log() => Console.WriteLine("   (registro estándar)");   // virtual: con cuerpo, las hijas PUEDEN cambiarlo
}

sealed class CreditCard(string number) : PaymentMethod            // sealed: nadie puede heredar de esta
{
    protected override string Name => $"tarjeta ****{number[^Math.Min(4, number.Length)..]}";
    protected override bool IsValid(out string reason)
    {
        reason = "El número de tarjeta debe tener 16 dígitos.";
        return number.Length == 16 && number.All(char.IsDigit);
    }
    protected override void Charge(decimal amount) => Console.WriteLine($"   ✓ cargado a la tarjeta (comisión {amount * 0.015m:F2} €)");
}

sealed class BankTransfer(string iban) : PaymentMethod
{
    protected override string Name => $"transferencia {iban[..4]}…";
    protected override bool IsValid(out string reason) { reason = "IBAN no válido."; return iban.StartsWith("ES") && iban.Length == 24; }
    protected override void Charge(decimal amount) => Console.WriteLine("   ✓ transferencia ordenada (sin comisión)");
    protected override void Log() => Console.WriteLine("   (registro especial: se avisa al banco)");        // sobrescribe lo virtual
}

interface ISwimmer { void Swim(); }
interface IFlyer { void Fly(); }
interface IDescribable
{
    string Describe() => $"{GetType().Name}: {string.Join(" + ", Abilities())}";    // implementación por defecto
    IEnumerable<string> Abilities() => [];
}
class Duck : ISwimmer, IFlyer, IDescribable                       // cumple TRES contratos
{
    public void Swim() => Console.WriteLine("   El pato nada");
    public void Fly() => Console.WriteLine("   El pato vuela");
    public string Describe() => "Pato: nada y vuela";
}
class Fish : ISwimmer, IDescribable
{
    public void Swim() => Console.WriteLine("   El pez nada");
    public IEnumerable<string> Abilities() => ["nada", "respira bajo el agua"];   // Fish NO define Describe(): usa la implementación por defecto
}
class RobotDuck : IFlyer { public void Fly() => Console.WriteLine("   El robot vuela"); }

interface IFlyBehavior { string Fly(); }
interface ISoundBehavior { string Sound(); }
class FlyWithWings : IFlyBehavior { public string Fly() => "vuela con alas"; }
class FlyWithRocket : IFlyBehavior { public string Fly() => "vuela con un cohete"; }
class CannotFly : IFlyBehavior { public string Fly() => "no vuela"; }
class Quack : ISoundBehavior { public string Sound() => "¡cuac!"; }
class Squeak : ISoundBehavior { public string Sound() => "¡pii!"; }

class Animal(string name, IFlyBehavior fly, ISoundBehavior sound)
{
    private IFlyBehavior _fly = fly;
    public void SetFly(IFlyBehavior behavior) => _fly = behavior;
    public void Show() => Console.WriteLine($"   {name}: {_fly.Fly()}, {sound.Sound()}");
}
