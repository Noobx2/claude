# Curso de C# · Parte III: orientación a objetos y diseño

Esta parte es la que separa **escribir código que funciona** de **escribir código que se puede mantener**: clases abstractas, interfaces, singleton, inyección de dependencias, patrones de diseño y los principios SOLID. Es lo que suele enseñarse en las clases de programación orientada a objetos, pero **con código que se ejecuta** y con el criterio de **cuándo usar cada cosa y cuándo no**.

*Verificado: 2026-10-29 con .NET SDK 10.0.113 en Linux. Todas las lecciones se ejecutaron.*

## El temario, de un vistazo

La lista de lo que normalmente se enseña, y **dónde está** en este curso:

| Tema | Lección | Idea en una frase |
|---|---|---|
| Clases y objetos | 9 | Una clase es el molde; el objeto, la pieza creada con él |
| Encapsulamiento | 9 | Esconder los datos y exponer solo lo necesario |
| Herencia | 10 | Una clase reutiliza y amplía a otra («es un») |
| Polimorfismo | 10 | Un mismo mensaje, comportamientos distintos según el objeto |
| **Clases abstractas** | **28** | Plantilla incompleta que obliga a las hijas a rellenar huecos |
| **Interfaces** | **28 y 31** | Contrato: qué se puede hacer, no cómo |
| **Composición frente a herencia** | **28** | Armar comportamientos con piezas en vez de heredarlos |
| **Singleton** | **29** | Una sola instancia compartida, y por qué casi siempre se evita |
| **Inyección de dependencias** | **30** | Entregar a una clase lo que necesita en vez de que lo cree ella |
| Interfaces estándar de .NET | 31 | `IComparable`, `IEquatable`, `IEnumerable`, `IDisposable`… |
| **Patrones creacionales** | **32** | Factory, Abstract Factory, Builder, Prototype, Pool |
| **Patrones estructurales** | **33** | Adapter, Decorator, Facade, Proxy, Composite |
| **Patrones de comportamiento** | **34** | Strategy, Observer, Command, State, Chain of Responsibility |
| **SOLID** | **35** | Cinco reglas para que el diseño aguante los cambios |
| Operadores, indexadores, extensiones, atributos | 36 | Herramientas del lenguaje que completan todo lo anterior |

> **Regla antes de empezar:** un patrón es una **solución a un problema**. Si no tienes el problema, no uses el patrón. Aprender patrones **sin** su problema da código más complicado y no mejor.

---

## Lección 28 · Clases abstractas, interfaces y composición

### Qué vas a aprender

Tres herramientas para decir «estas cosas se parecen»: la **clase abstracta**, la **interfaz** y la **composición**. Y, sobre todo, **cuál elegir**.

### La idea

| | Clase abstracta | Interfaz | Composición |
|---|---|---|---|
| Qué expresa | «**Es un**…» con código compartido | «**Puede**…» (una capacidad) | «**Tiene un**…» (una pieza) |
| Código propio | Sí (métodos con cuerpo y datos) | No (salvo implementaciones por defecto) | Sí, en las piezas |
| Cuántas se pueden usar | **Una** (herencia simple) | **Varias** | Todas las que quieras |
| Cuándo | Hay una **base común real** y un orden de pasos fijo | Quieres **intercambiar** implementaciones | El comportamiento **cambia** o se combina |

**Una clase abstracta** (`abstract class`) es una **plantilla incompleta**: no se puede crear con `new`; define lo común y **obliga** a las clases hijas a rellenar lo que falta (`abstract`). Puede tener métodos ya hechos (`virtual` si las hijas pueden cambiarlos).

**El patrón Plantilla (Template Method):** el método de la clase base fija el **orden de los pasos** (`Pay()` valida, cobra, registra) y las hijas solo rellenan cada paso. Nadie puede alterar el orden porque el método no es `virtual`.

**Una interfaz** (`interface`) es un contrato: lista qué miembros debe tener quien lo cumpla. Una clase puede cumplir **varias**. Desde C# 8 puede traer una **implementación por defecto**.

**La composición** resuelve el problema clásico de la herencia: ¿un pato de goma **es un** pato que vuela? Con herencia hereda lo que no debe. Con composición **se le enchufa** el comportamiento que corresponda, incluso **en ejecución**.

### Código (`28-abstractas-interfaces.cs`)

```csharp
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
```

### Cómo se lee, paso a paso

1. **`abstract class PaymentMethod`**: no se puede instanciar. Tiene un método **concreto** `Pay()` y tres piezas **abstractas** (`Name`, `IsValid`, `Charge`) que **cada hija debe implementar**.
2. **`Pay()` es la plantilla**: valida → cobra → registra. Es público y **no `virtual`**, así que el orden es **inmodificable**.
3. **`protected virtual void Log()`** tiene cuerpo, pero `BankTransfer` lo **sobrescribe** (`override`) para su caso. Es la diferencia: *abstracto* = obligatorio; *virtual* = opcional.
4. **`sealed class CreditCard`**: nadie puede heredar de ella.
5. **`static int _attempts`** en la clase base: **estado compartido** por todas las hijas.
6. **`Duck : ISwimmer, IFlyer, IDescribable`**: una clase, **tres contratos**. Con herencia sería imposible (solo hay una clase base).
7. **`IDescribable.Describe()` tiene cuerpo**: es la implementación por defecto. `Fish` **no la define**, así que usa la de la interfaz. **Solo se llama a través de la interfaz** (`d.Describe()`), no desde `Fish`.
8. **Composición:** `Animal` recibe un `IFlyBehavior` y un `ISoundBehavior`. `rubber.SetFly(new FlyWithRocket())` **cambia el comportamiento** mientras el programa corre.

### Resultado real

```text
── Clase abstracta + patrón Plantilla (Template Method) ──
Pago de 120.50 € con tarjeta ****1111
   ✓ cargado a la tarjeta (comisión 1.81 €)
   (registro estándar)
   resultado: COBRADO

Pago de 120.50 € con transferencia ES91…
   ✓ transferencia ordenada (sin comisión)
   (registro especial: se avisa al banco)
   resultado: COBRADO

Pago de 120.50 € con tarjeta ****1234
   ✗ El número de tarjeta debe tener 16 dígitos.
   resultado: RECHAZADO

Total de pagos intentados: 3   (estado compartido en la base)

── Interfaces: capacidades que se combinan ──
   El pato nada
   El pato vuela
   Pato: nada y vuela
   El pez nada
   Fish: nada + respira bajo el agua
   El robot vuela

── Abstracta o interfaz ──
Clase abstracta : relación "ES UN" con código y datos compartidos (solo se hereda de UNA).
Interfaz        : capacidad "PUEDE HACER" sin código compartido (se cumplen VARIAS).
Regla práctica  : empieza por una interfaz; usa una clase abstracta solo si necesitas compartir código o estado.
── Composición: el comportamiento es una pieza que se enchufa ──
   Pato salvaje: vuela con alas, ¡cuac!
   Pato de goma: no vuela, ¡pii!
   Pato de goma: vuela con un cohete, ¡pii!
```

### Errores típicos

| Error | Causa | Solución |
|---|---|---|
| `Cannot create an instance of the abstract type` | `new` sobre una clase abstracta | Crea una hija concreta |
| `does not implement inherited abstract member` | Una hija no implementó un `abstract` | Impleméntalo con `override` |
| Stack overflow al llamar a `Describe()` | La clase redefine el método y llama a la versión por defecto de la interfaz, que es **ella misma** | No redefinas el método, o implementa el cuerpo completo |
| Jerarquías de herencia de 5 niveles | Se usó herencia para reutilizar código | Prefiere composición |

> **Ejercicio 28.1:** crea `abstract class Shape` con `Area()` abstracto y `Describe()` virtual, y tres hijas.
>
> **Ejercicio 28.2:** convierte `PaymentMethod` en una interfaz `IPayment` + una clase base `PaymentBase` con la plantilla. ¿Qué ganas y qué pierdes?
>
> **Ejercicio 28.3:** añade un comportamiento `ISwimBehavior` a `Animal` por composición.

---

## Lección 29 · El patrón Singleton

### Qué vas a aprender

Qué es un Singleton, **cinco formas de escribirlo** (de la ingenua a la correcta), **por qué es peligroso** y la **alternativa moderna**.

### La idea

Un **Singleton** garantiza que **solo exista una instancia** de una clase y que **todo el programa la comparta**. Se consigue con un **constructor privado** y una **forma estática** de pedir la instancia.

**Casos legítimos:** configuración de la aplicación, caché, registro de eventos (log), generador de identificadores.

**El problema real** no es crearlo, sino que es **estado global**: cualquiera lo modifica, nadie sabe quién, y el resultado depende de **lo que ocurrió antes**. Además, **oculta dependencias** (la clase usa `Settings.Instance` sin declararlo) e **impide las pruebas**.

| Versión | Seguro con hilos | Perezoso | Comentario |
|---|---|---|---|
| Ingenua (`_instance ??= new()`) | ✗ | Sí | **Carrera de datos**: pueden crearse varias |
| `lock` + doble comprobación | ✓ | Sí | Correcta pero larga |
| Campo estático (`static readonly`) | ✓ | No (al primer uso de la clase) | **La más sencilla**: el CLR lo garantiza |
| `Lazy<T>` | ✓ | Sí | **La recomendada** si necesitas pereza |
| **Una instancia, pero inyectada** | ✓ | — | **La moderna**: el contenedor entrega siempre la misma |

### Código (`29-singleton.cs`)

```csharp
// Lección 29: el patrón Singleton (una única instancia) y por qué casi siempre se evita
// Ejecutar: dotnet run 29-singleton.cs
using System.Collections.Concurrent;

// ═══════════════════════ La idea ═══════════════════════
// Singleton = "solo puede existir UN objeto de esta clase en todo el programa y todos lo comparten".
// Ejemplos legítimos: configuración, registro (log), caché, generador de ids.
// Se consigue con: constructor PRIVADO + una forma estática de pedir la instancia única.

// ═══════════════════════ Versión 1: la ingenua (¡NO es segura con hilos!) ═══════════════════════
Console.WriteLine("── Versión 1: ingenua ──");
NaiveSingleton.Created = 0;
RunConcurrently(8, () => NaiveSingleton.Instance);
Console.WriteLine($"   Se crearon {NaiveSingleton.Created} instancias (¡debería ser 1!). Es una CARRERA DE DATOS.");

// ═══════════════════════ Versión 2: con cerrojo (lock) y doble comprobación ═══════════════════════
Console.WriteLine("── Versión 2: lock + doble comprobación ──");
LockSingleton.Created = 0;
RunConcurrently(8, () => LockSingleton.Instance);
Console.WriteLine($"   Instancias creadas: {LockSingleton.Created}");

// ═══════════════════════ Versión 3: campo estático (la más sencilla y segura) ═══════════════════════
// El CLR garantiza que un inicializador estático se ejecuta UNA sola vez y de forma segura entre hilos.
Console.WriteLine("── Versión 3: campo estático ──");
RunConcurrently(8, () => EagerSingleton.Instance);
Console.WriteLine($"   Instancias creadas: {EagerSingleton.Created}   (se crea al primer uso de la clase)");

// ═══════════════════════ Versión 4: Lazy<T> (perezosa y segura) ═══════════════════════
Console.WriteLine("── Versión 4: Lazy<T> (se crea solo cuando alguien la pide) ──");
Console.WriteLine($"   antes de pedirla: creada = {LazySingleton.IsCreated}");
RunConcurrently(8, () => LazySingleton.Instance);
Console.WriteLine($"   después: creada = {LazySingleton.IsCreated}, instancias = {LazySingleton.Created}");

// ═══════════════════════ El problema real del Singleton ═══════════════════════
Console.WriteLine("── El problema: estado global compartido ──");
var a = Settings.Instance; a.Theme = "oscuro";
Console.WriteLine($"   código A cambia el tema a: {a.Theme}");
Console.WriteLine($"   código B (otro lugar) lo ve:  {Settings.Instance.Theme}   <- B depende de lo que hizo A sin enterarse");

// Una "prueba" de dos servicios que usan el Singleton: el segundo queda contaminado por el primero
Console.WriteLine($"   ServicioX dice: {new ServiceX().Greet()}");
Settings.Instance.Theme = "claro";
Console.WriteLine($"   ServicioX dice: {new ServiceX().Greet()}   <- mismo código, otro resultado según lo que pasó antes");

// ═══════════════════════ La alternativa moderna: inyectar UNA instancia ═══════════════════════
Console.WriteLine("── Alternativa: una instancia única, pero ENTREGADA (inyectada) ──");
var shared = new AppSettings { Theme = "oscuro" };          // se crea UNA vez, en un sitio (la raíz de composición)
var y1 = new ServiceY(shared);
var y2 = new ServiceY(shared);                              // los dos comparten la misma, porque SE LA DAMOS
Console.WriteLine($"   {y1.Greet()} / {y2.Greet()}   (misma instancia: {ReferenceEquals(y1.Settings, y2.Settings)})");
var testSettings = new AppSettings { Theme = "de prueba" }; // en una prueba entregamos otra distinta: imposible con el Singleton clásico
Console.WriteLine($"   en una prueba: {new ServiceY(testSettings).Greet()}");

Console.WriteLine("""

    ── Cuándo sí y cuándo no ──
    Sí   : una instancia por aplicación, entregada por el contenedor de dependencias (AddSingleton).
    Sí   : clases estáticas sin estado (Math, Path).
    No   : Singleton clásico con "Instance" estático y estado modificable: oculta dependencias,
           impide las pruebas y es difícil de hacer seguro con hilos.
    """);

// ═══════════════════════ Utilidades y tipos ═══════════════════════

// Lanza N hilos que piden la instancia EXACTAMENTE a la vez (la Barrier los sincroniza)
static void RunConcurrently(int threads, Func<object> action)
{
    using var barrier = new Barrier(threads);
    var tasks = Enumerable.Range(0, threads).Select(_ => Task.Run(() => { barrier.SignalAndWait(); action(); })).ToArray();
    Task.WaitAll(tasks);
}

class NaiveSingleton
{
    public static int Created;
    private static NaiveSingleton? _instance;
    private NaiveSingleton() { Interlocked.Increment(ref Created); Thread.Sleep(20); }      // privado: nadie más puede hacer "new"
    public static NaiveSingleton Instance => _instance ??= new NaiveSingleton();             // ¡no es atómico!
}

class LockSingleton
{
    public static int Created;
    private static LockSingleton? _instance;
    private static readonly object Gate = new();
    private LockSingleton() { Interlocked.Increment(ref Created); Thread.Sleep(20); }
    public static LockSingleton Instance
    {
        get
        {
            if (_instance is null)                       // 1ª comprobación (rápida, sin bloquear)
                lock (Gate)
                    if (_instance is null)               // 2ª comprobación (dentro del cerrojo)
                        _instance = new LockSingleton();
            return _instance;
        }
    }
}

sealed class EagerSingleton
{
    public static int Created;
    public static readonly EagerSingleton Instance = new();        // el CLR lo inicializa una sola vez
    private EagerSingleton() { Interlocked.Increment(ref Created); }
}

sealed class LazySingleton
{
    public static int Created;
    private static readonly Lazy<LazySingleton> Holder = new(() => new LazySingleton());   // thread-safe por defecto
    public static LazySingleton Instance => Holder.Value;
    public static bool IsCreated => Holder.IsValueCreated;
    private LazySingleton() { Interlocked.Increment(ref Created); }
}

class Settings                                                      // Singleton clásico con estado modificable
{
    public static Settings Instance { get; } = new();
    private Settings() { }
    public string Theme { get; set; } = "claro";
}
class ServiceX { public string Greet() => $"hola con el tema {Settings.Instance.Theme}"; }   // dependencia OCULTA

class AppSettings { public string Theme { get; init; } = "claro"; }
class ServiceY(AppSettings settings) { public AppSettings Settings => settings; public string Greet() => $"hola con el tema {settings.Theme}"; } // dependencia VISIBLE
```

### Cómo se lee, paso a paso

1. **`private NaiveSingleton()`**: el constructor privado impide que nadie más haga `new`.
2. **`_instance ??= new ...`** parece correcto, pero **no es atómico**: dos hilos pueden ver `null` a la vez y crear dos instancias. Para **forzar** la carrera, la demostración usa una `Barrier` (todos los hilos arrancan a la vez) y un `Thread.Sleep` en el constructor.
3. **`lock` + doble comprobación:** una comprobación rápida **sin** bloquear y otra **dentro** del cerrojo.
4. **`static readonly ... = new()`:** el CLR garantiza que un inicializador estático se ejecuta **una sola vez y de forma segura**.
5. **`Lazy<T>`:** es seguro y **no crea** nada hasta que alguien accede a `.Value` (`IsValueCreated` lo demuestra).
6. **El problema:** `Settings.Instance.Theme` lo cambia **un código** y lo ve **otro** sin saberlo. `ServiceX` da resultados **distintos con el mismo código** según lo que pasó antes.
7. **La alternativa:** se crea **una** `AppSettings` en la raíz de la aplicación y se **entrega** (`new ServiceY(shared)`). Las dos instancias comparten la misma **porque se la damos**, y en una prueba se entrega **otra**.

### Resultado real

```text
── Versión 1: ingenua ──
   Se crearon 8 instancias (¡debería ser 1!). Es una CARRERA DE DATOS.
── Versión 2: lock + doble comprobación ──
   Instancias creadas: 1
── Versión 3: campo estático ──
   Instancias creadas: 1   (se crea al primer uso de la clase)
── Versión 4: Lazy<T> (se crea solo cuando alguien la pide) ──
   antes de pedirla: creada = False
   después: creada = True, instancias = 1
── El problema: estado global compartido ──
   código A cambia el tema a: oscuro
   código B (otro lugar) lo ve:  oscuro   <- B depende de lo que hizo A sin enterarse
   ServicioX dice: hola con el tema oscuro
   ServicioX dice: hola con el tema claro   <- mismo código, otro resultado según lo que pasó antes
── Alternativa: una instancia única, pero ENTREGADA (inyectada) ──
   hola con el tema oscuro / hola con el tema oscuro   (misma instancia: True)
   en una prueba: hola con el tema de prueba

── Cuándo sí y cuándo no ──
Sí   : una instancia por aplicación, entregada por el contenedor de dependencias (AddSingleton).
Sí   : clases estáticas sin estado (Math, Path).
No   : Singleton clásico con "Instance" estático y estado modificable: oculta dependencias,
       impide las pruebas y es difícil de hacer seguro con hilos.
```

> **Una nota sobre esta ejecución:** la versión ingenua creó **8** instancias porque los hilos arrancan a la vez; en una ejecución real el número varía, pero **casi nunca es 1**. Es un fallo que **aparece de vez en cuando** en producción y es dificilísimo de reproducir.

### Cuándo sí y cuándo no

| Situación | Veredicto |
|---|---|
| Una instancia por aplicación **entregada por el contenedor** (`AddSingleton`) | ✓ La forma correcta |
| `static class` sin estado (`Math`, `Path`, tus utilidades) | ✓ |
| `Instance` estático con **estado modificable** | ✗ Evítalo |
| Un Singleton dentro de un videojuego (Unity) | Muy usado, pero con cuidado (lección 52) |

> **Ejercicio 29.1:** implementa un `Logger` con `Lazy<T>` que acumule mensajes y los muestre ordenados; llámalo desde cinco hilos.
>
> **Ejercicio 29.2:** reescribe `ServiceX` para recibir `Settings` por el constructor y escribe una prueba con un valor distinto.
>
> **Ejercicio 29.3:** demuestra con 100 hilos que `lock` + doble comprobación crea **una** instancia.

---

## Lección 30 · Inyección de dependencias (DI) a fondo

### Qué vas a aprender

La DI **desde cero** (sin contenedor), el **contenedor** de .NET con sus tres duraciones, una **trampa clásica** (la dependencia cautiva), varias implementaciones, configuración tipada y el **antipatrón** del localizador de servicios.

### La idea

Una clase **depende** de otras (un servicio de pedidos necesita enviar correos y registrar mensajes). Hay **dos formas** de conseguirlas:

| ✗ La crea ella misma | ✓ Se la entregan |
|---|---|
| `var sender = new EmailSender();` dentro de la clase | `class OrderService(ISender sender)` |
| Atada para siempre a esa clase concreta | Cualquiera que cumpla `ISender` sirve |
| No se puede probar sin enviar correos reales | En una prueba se entrega un **falso** |
| La dependencia está **escondida** | La dependencia está **a la vista**, en el constructor |

**Eso es la inyección de dependencias.** Un **contenedor de DI** es solo una herramienta que **construye los objetos en el orden correcto** y los entrega.

**Tres duraciones (lifetimes):**

| Duración | Se crea… | Ejemplo típico |
|---|---|---|
| **Singleton** | **Una vez** para toda la aplicación | Configuración, caché, cliente HTTP |
| **Scoped** | **Una vez por ámbito** (en una web, por petición) | Acceso a la base de datos de una petición |
| **Transient** | **Cada vez** que se pide | Servicios ligeros sin estado |

**Regla de oro:** un servicio solo puede depender de otros con **igual o mayor** duración. Un Singleton que recibe un Scoped lo **secuestra** (*captive dependency*).

### Código (`30-inyeccion.cs`)

```csharp
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
```

### Cómo se lee, paso a paso

1. **`#:package ...`:** las cuatro primeras líneas declaran los paquetes NuGet del archivo (modo archivo único de .NET 10).
2. **DI manual (apartado 1):** `new OrderService(sender, logger)`. **Esto es toda la técnica**: pasar lo que la clase necesita por el constructor. El resto es comodidad.
3. **Tres formas de inyectar:** **constructor** (obligatoria, la normal), **propiedad** (opcional) y **método** (solo para una operación).
4. **`services.AddSingleton<ILogger, ConsoleLogger>()`:** «cuando alguien pida `ILogger`, entrega un `ConsoleLogger`». `OrderService` se registra por su propio tipo.
5. **`ValidateScopes` y `ValidateOnBuild`:** el contenedor **comprueba al arrancar** que las duraciones son coherentes y que no falta ninguna dependencia, en lugar de fallar en producción.
6. **Dependencia cautiva (apartado 4):** `CachedReport` (Singleton) pide un `IUnitOfWork` (Scoped). El contenedor lo **detecta al construirse** y explica el error.
7. **Varias implementaciones (apartado 5):** `IEnumerable<INotifier>` recibe **todas**; pedir un solo `INotifier` entrega **la última registrada**; `AddKeyedSingleton` permite pedirlas **por nombre**.
8. **Registro con fábrica:** `AddSingleton<INotifier>(sp => ...)` decide **al crear** (en la vida real, según la configuración).
9. **Patrón opciones:** `IOptions<MailOptions>` recibe la configuración **ya tipada**; `ValidateOnStart` la **valida al arrancar**.
10. **Prueba:** con un `RecordingSender` se comprueba el mensaje **sin enviar nada real**.

### Resultado real

```text
/tmp/claude-0/-home-user-claude/ddd43a70-8f87-5490-9d3f-755ecbba3013/scratchpad/curso3/30-inyeccion.cs(99,1): info SP0001: Diagnostic 'IL2026: Using member 'Microsoft.Extensions.DependencyInjection.OptionsBuilderConfigurationExtensions.Bind<TOptions>(OptionsBuilder<TOptions>, IConfiguration)' which has 'RequiresUnreferencedCodeAttribute' can break functionality when trimming application code. TOptions's dependent types may have their members trimmed. Ensure all required members are preserved.' was programmatically suppressed by a DiagnosticSuppressor with suppression ID 'SYSLIBSUPPRESS0002' and justification 'The target method has been intercepted by a generated static variant.'
/tmp/claude-0/-home-user-claude/ddd43a70-8f87-5490-9d3f-755ecbba3013/scratchpad/curso3/30-inyeccion.cs(99,1): info SP0001: Diagnostic 'IL3050: Using member 'Microsoft.Extensions.DependencyInjection.OptionsBuilderConfigurationExtensions.Bind<TOptions>(OptionsBuilder<TOptions>, IConfiguration)' which has 'RequiresDynamicCodeAttribute' can break functionality when AOT compiling. Binding strongly typed objects to configuration values may require generating dynamic code at runtime.' was programmatically suppressed by a DiagnosticSuppressor with suppression ID 'SYSLIBSUPPRESS0003' and justification 'The target method has been intercepted by a generated static variant.'
── 1. DI manual: componer a mano ──
   [log] pedido de Teclado
   [log] enviando correo: Pedido de Teclado confirmado

── 2. Constructor / propiedad / método ──
Constructor : la dependencia es OBLIGATORIA (lo normal). new OrderService(sender, logger)
Propiedad   : OPCIONAL, se puede cambiar después. service.Logger = otro   (poco usada)
Método      : solo para UNA operación. service.Export(IExporter exporter)
── 3. Contenedor: registrar y resolver ──
   Transient distinto cada vez: True
   Scoped igual dentro del ámbito: True
   Singleton igual siempre: True

── 4. Dependencia cautiva (captive dependency) ──
   El contenedor lo detectó al construirse:
   Error while validating the service descriptor 'ServiceType: CachedReport Lifetime: Singleton ImplementationType: CachedReport': Cannot consume scoped service 'IUnitOfWork' from singleton 'CachedReport'.

── 5. Varias implementaciones de una interfaz ──
   [email] Servidor caído
   [sms] Servidor caído
   La última registrada gana si se pide una sola: SmsNotifier
   Por clave 'urgente': SmsNotifier

── 6. Registro con fábrica (decide al crear) ──
   Se eligió: SmsNotifier

── 7. Opciones: configuración tipada e inyectada ──
   Servidor smtp.ejemplo.es:587, remitente no-reply@ejemplo.es

── 8. Antipatrón: pedir las cosas al contenedor desde dentro ──
   class Malo(IServiceProvider sp) { void Hacer() { var x = sp.GetRequiredService<IAlgo>(); } }
   Esconde qué necesita la clase (no se ve en el constructor) y falla en ejecución, no al compilar.
   Úsalo solo en la raíz de la aplicación, nunca dentro de la lógica.
── 9. En una prueba se entrega un falso ──
   [log] pedido de Ratón
   El falso recibió: [Pedido de Ratón confirmado]  (no se envió ningún correo real)
```

### Errores típicos

| Mensaje / problema | Causa | Solución |
|---|---|---|
| `Unable to resolve service for type 'X'` | Falta registrar `X` | `services.AddXxx<X>()` |
| `Cannot consume scoped service ... from singleton ...` | Dependencia cautiva | Cambia las duraciones o crea un ámbito dentro del Singleton |
| `A circular dependency was detected` | A necesita B y B necesita A | Rediseña: extrae una tercera pieza |
| El constructor tiene **8** parámetros | La clase hace demasiado | Divídela (principio S de SOLID) |
| `sp.GetRequiredService<>()` dentro de la lógica | Localizador de servicios | Declara la dependencia en el constructor |

> **Ejercicio 30.1:** crea `IClock` con dos implementaciones (real y falsa) y úsala en un servicio que diga «buenos días/tardes».
>
> **Ejercicio 30.2:** registra dos `INotifier` con clave y crea un servicio que reciba una **función** que los resuelva por nombre.
>
> **Ejercicio 30.3:** provoca a propósito una dependencia cautiva y lee el mensaje de error.

---

## Lección 31 · Las interfaces estándar de .NET

### Qué vas a aprender

Las interfaces que **la propia plataforma** espera que implementes para que tus tipos funcionen con `Sort`, `HashSet`, `foreach`, `using`, `{valor:formato}` y `T.Parse`.

### La idea

Cada interfaz es una **promesa** que un tipo hace al resto del framework:

| Interfaz | Promesa | Lo usa… |
|---|---|---|
| `IComparable<T>` | «Sé ordenarme respecto a otro» | `Sort`, `OrderBy`, `Max` |
| `IComparer<T>` | «Sé ordenar dos cosas de ese tipo» (orden alternativo) | `Sort(comparador)` |
| `IEquatable<T>` | «Sé compararme por valor» | `HashSet`, `Dictionary`, `Contains` |
| `IEnumerable<T>` | «Puedes recorrerme» | `foreach`, LINQ |
| `IDisposable` | «Libera mis recursos cuando acabes» | `using` |
| `IFormattable` | «Sé mostrarme con distintos formatos» | `{valor:formato}` |
| `IParsable<T>` | «Sé crearme desde un texto» | `T.Parse`, `T.TryParse` genéricos |

**Regla de oro de `Equals`:** si redefines `Equals`, **redefine siempre `GetHashCode`** con los mismos campos. Si no, `HashSet` y `Dictionary` **no te encuentran**.

### Código (`31-interfaces-estandar.cs`)

```csharp
// Lección 31: las interfaces estándar de .NET que debes conocer
// Ejecutar: dotnet run 31-interfaces-estandar.cs
using System.Collections;
using System.Globalization;

// ═══════════════ IComparable<T>: "sé ordenarme" ═══════════════
Console.WriteLine("── IComparable<T> e IComparer<T>: ordenar ──");
var people = new List<Person> { new("Luis", 28), new("Ana", 31), new("Marta", 25), new("Ana", 22) };
people.Sort();                                                    // usa Person.CompareTo (orden natural)
Console.WriteLine("natural (por nombre y edad): " + string.Join(", ", people));
people.Sort(new ByAgeDescending());                               // orden alternativo con un IComparer<T>
Console.WriteLine("por edad desc.:              " + string.Join(", ", people));
Console.WriteLine("con lambda:                   " + string.Join(", ", people.OrderBy(p => p.Age)));

// ═══════════════ IEquatable<T>: "sé compararme" ═══════════════
Console.WriteLine("\n── IEquatable<T>, Equals y GetHashCode ──");
var p1 = new Point(1, 2); var p2 = new Point(1, 2);
Console.WriteLine($"Point (clase propia)   == : {p1 == p2}; Equals: {p1.Equals(p2)}");
var set = new HashSet<Point> { p1 };
Console.WriteLine($"HashSet contiene un punto igual: {set.Contains(p2)}   (necesita GetHashCode coherente con Equals)");
var s1 = new BadPoint(1, 2); var s2 = new BadPoint(1, 2);
Console.WriteLine($"BadPoint sin GetHashCode: HashSet lo encuentra = {new HashSet<BadPoint> { s1 }.Contains(s2)}   <- ¡error clásico!");

// ═══════════════ IEnumerable<T>: "puedes recorrerme con foreach" ═══════════════
Console.WriteLine("\n── IEnumerable<T>: tu propia colección ──");
var playlist = new Playlist { "Intro", "Tema 1", "Tema 2" };      // inicializador: usa Add
foreach (string song in playlist) Console.Write($"[{song}] ");
Console.WriteLine($"\nLINQ funciona sobre ella: {string.Join(",", playlist.Where(s => s.StartsWith("Tema")))}");
Console.WriteLine($"Con indexador: playlist[1] = {playlist[1]}; playlist[^1] = {playlist[^1]}");

// ═══════════════ IDisposable: "libera tus recursos" ═══════════════
Console.WriteLine("\n── IDisposable: using libera siempre ──");
using (var connection = new FakeConnection("base-1"))
{
    connection.Query("SELECT 1");
}                                                                 // Dispose() se llama aquí, haya o no excepción
try
{
    using var failing = new FakeConnection("base-2");
    failing.Query("SELECT mal");
}
catch (InvalidOperationException e) { Console.WriteLine($"   error: {e.Message}  (y aun así se liberó)"); }

// ═══════════════ IFormattable: formatos propios ═══════════════
Console.WriteLine("\n── IFormattable: {valor:formato} ──");
var money = new Money(1234.5m, "EUR");
Console.WriteLine($"{money} | {money:short} | {money:long}");

// ═══════════════ IParsable<T>: "puedo crearme desde un texto" ═══════════════
Console.WriteLine("\n── IParsable<T>: TryParse genérico ──");
Console.WriteLine($"Parse<int>(\"42\")    = {ParseOr<int>("42", -1)}");
Console.WriteLine($"Parse<double>(\"4.5\") = {ParseOr<double>("4.5", -1)}");
Console.WriteLine($"Parse<Guid>(\"mal\")   = {ParseOr<Guid>("mal", Guid.Empty)}");
Console.WriteLine($"Parse<Money>(\"9 USD\") = {ParseOr(  "9 USD", new Money(0, "?"))}");

// ═══════════════ Covarianza ═══════════════
Console.WriteLine("\n── Covarianza: IEnumerable<out T> ──");
IEnumerable<string> strings = ["a", "b"];
IEnumerable<object> objects = strings;                            // válido: un recorrido de strings ES un recorrido de objects
Console.WriteLine($"IEnumerable<string> -> IEnumerable<object>: {string.Join(",", objects)}");
// List<object> no = new List<string>();                          // NO compila: List<T> es invariante (se puede escribir)

// ═══════════════ Implementación explícita ═══════════════
Console.WriteLine("\n── Implementación explícita (resolver choques de nombres) ──");
var both = new Both();
((IEnglish)both).Greet();
((ISpanish)both).Greet();

// ═══════════════ Segregación de interfaces ═══════════════
Console.WriteLine("\n── Interfaces pequeñas y específicas ──");
var repo = new MemoryRepo();
IReadOnlyRepo<string> readOnly = repo;                            // el que SOLO lee no recibe métodos de escritura
Console.WriteLine($"   solo lectura ve: {string.Join(",", readOnly.All())}");
IRepo<string> full = repo; full.Add("nuevo");                     // el mismo objeto, visto con otro contrato
Console.WriteLine($"   tras escribir: {string.Join(",", readOnly.All())}");

static T ParseOr<T>(string text, T fallback) where T : IParsable<T> =>
    T.TryParse(text, CultureInfo.InvariantCulture, out var value) ? value : fallback;

// ═══════════════ TIPOS ═══════════════
record Person(string Name, int Age) : IComparable<Person>
{
    public int CompareTo(Person? other)                          // negativo: yo voy antes; 0: iguales; positivo: yo voy después
    {
        if (other is null) return 1;
        int byName = string.Compare(Name, other.Name, StringComparison.CurrentCulture);
        return byName != 0 ? byName : Age.CompareTo(other.Age);
    }
    public override string ToString() => $"{Name}({Age})";
}
class ByAgeDescending : IComparer<Person>
{
    public int Compare(Person? x, Person? y) => (y?.Age ?? 0).CompareTo(x?.Age ?? 0);
}

sealed class Point(int x, int y) : IEquatable<Point>
{
    public int X { get; } = x;                                          // los parámetros del constructor principal NO son miembros: se copian
    public int Y { get; } = y;
    public bool Equals(Point? other) => other is not null && X == other.X && Y == other.Y;
    public override bool Equals(object? obj) => Equals(obj as Point);
    public override int GetHashCode() => HashCode.Combine(X, Y);        // SIEMPRE junto con Equals
    public static bool operator ==(Point? a, Point? b) => a?.Equals(b) ?? b is null;
    public static bool operator !=(Point? a, Point? b) => !(a == b);
}
#pragma warning disable CS0659   // (el aviso del compilador ya nos dice el error: lo silenciamos a propósito para la demostración)
sealed class BadPoint(int x, int y)
{
    public int X { get; } = x;
    public int Y { get; } = y;
    public override bool Equals(object? obj) => obj is BadPoint o && o.X == X && o.Y == Y;   // Equals sí, GetHashCode no
}
#pragma warning restore CS0659

class Playlist : IEnumerable<string>
{
    private readonly List<string> _songs = [];
    public void Add(string song) => _songs.Add(song);
    public string this[Index i] => _songs[i];                          // indexador: playlist[1], playlist[^1]
    public IEnumerator<string> GetEnumerator() => _songs.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

sealed class FakeConnection : IDisposable
{
    private readonly string _name;
    public FakeConnection(string name) { _name = name; Console.WriteLine($"   abrir {name}"); }
    public void Query(string sql)
    {
        if (sql.Contains("mal")) throw new InvalidOperationException("SQL incorrecto");
        Console.WriteLine($"   ejecutar {sql}");
    }
    public void Dispose() => Console.WriteLine($"   cerrar {_name}");
}

readonly record struct Money(decimal Amount, string Currency) : IFormattable, IParsable<Money>
{
    public override string ToString() => ToString("short", CultureInfo.InvariantCulture);
    public string ToString(string? format, IFormatProvider? provider) => format switch
    {
        "long" => $"{Amount.ToString("N2", CultureInfo.GetCultureInfo("es-ES"))} {Currency} (importe en {Currency})",
        _ => $"{Amount.ToString("F2", CultureInfo.InvariantCulture)} {Currency}",
    };
    public static Money Parse(string s, IFormatProvider? provider) => TryParse(s, provider, out var m) ? m : throw new FormatException();
    public static bool TryParse(string? s, IFormatProvider? provider, out Money result)
    {
        var parts = s?.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts is { Length: 2 } && decimal.TryParse(parts[0], NumberStyles.Number, CultureInfo.InvariantCulture, out var amount)) { result = new(amount, parts[1]); return true; }
        result = default; return false;
    }
}

interface IEnglish { void Greet(); }
interface ISpanish { void Greet(); }
class Both : IEnglish, ISpanish                                    // dos interfaces con el MISMO nombre de método
{
    void IEnglish.Greet() => Console.WriteLine("   Hello");        // explícita: solo se ve a través de la interfaz
    void ISpanish.Greet() => Console.WriteLine("   Hola");
}

interface IReadOnlyRepo<T> { IEnumerable<T> All(); }
interface IRepo<T> : IReadOnlyRepo<T> { void Add(T item); }
class MemoryRepo : IRepo<string>
{
    private readonly List<string> _items = ["uno", "dos"];
    public IEnumerable<string> All() => _items;
    public void Add(string item) => _items.Add(item);
}
```

### Cómo se lee, paso a paso

1. **`CompareTo`** devuelve un número **negativo** (yo voy antes), **cero** (iguales) o **positivo** (yo voy después). Con él `people.Sort()` ordena por nombre y después por edad.
2. **`IComparer<Person>`** es un **orden alternativo** (por edad descendente) sin tocar la clase.
3. **`Equals` + `GetHashCode`:** `Point` los redefine bien y el `HashSet` encuentra el punto. **`BadPoint`** redefine solo `Equals` y el `HashSet` **no lo encuentra** (`False`): es el error clásico.
4. **`IEnumerable<string>` en `Playlist`:** el compilador convierte `foreach` en llamadas a `GetEnumerator()`. Con ello **LINQ funciona gratis** sobre tu clase.
5. **Indexador `this[Index i]`:** permite `playlist[1]` y `playlist[^1]` (el último).
6. **`IDisposable` + `using`:** se llama a `Dispose()` **siempre**, incluso con una excepción (la salida muestra `cerrar base-2` tras el error).
7. **`IFormattable`:** `{money:long}` llama a `ToString("long", ...)`.
8. **`IParsable<T>`:** una sola función `ParseOr<T>` sirve para `int`, `double`, `Guid` y tu propio `Money`.
9. **Covarianza:** un `IEnumerable<string>` **se puede usar** como `IEnumerable<object>` (solo se **lee**); un `List<string>` no como `List<object>` (se podría **escribir** algo incorrecto).
10. **Implementación explícita:** `void IEnglish.Greet()` resuelve dos interfaces con el mismo nombre de método.
11. **Segregación:** `IReadOnlyRepo<T>` (leer) e `IRepo<T>` (leer y escribir). El objeto es el mismo; el contrato, distinto.

### Resultado real

```text
── IComparable<T> e IComparer<T>: ordenar ──
natural (por nombre y edad): Ana(22), Ana(31), Luis(28), Marta(25)
por edad desc.:              Ana(31), Luis(28), Marta(25), Ana(22)
con lambda:                   Ana(22), Marta(25), Luis(28), Ana(31)

── IEquatable<T>, Equals y GetHashCode ──
Point (clase propia)   == : True; Equals: True
HashSet contiene un punto igual: True   (necesita GetHashCode coherente con Equals)
BadPoint sin GetHashCode: HashSet lo encuentra = False   <- ¡error clásico!

── IEnumerable<T>: tu propia colección ──
[Intro] [Tema 1] [Tema 2] 
LINQ funciona sobre ella: Tema 1,Tema 2
Con indexador: playlist[1] = Tema 1; playlist[^1] = Tema 2

── IDisposable: using libera siempre ──
   abrir base-1
   ejecutar SELECT 1
   cerrar base-1
   abrir base-2
   cerrar base-2
   error: SQL incorrecto  (y aun así se liberó)

── IFormattable: {valor:formato} ──
1234.50 EUR | 1234.50 EUR | 1.234,50 EUR (importe en EUR)

── IParsable<T>: TryParse genérico ──
Parse<int>("42")    = 42
Parse<double>("4.5") = 4.5
Parse<Guid>("mal")   = 00000000-0000-0000-0000-000000000000
Parse<Money>("9 USD") = 9.00 USD

── Covarianza: IEnumerable<out T> ──
IEnumerable<string> -> IEnumerable<object>: a,b

── Implementación explícita (resolver choques de nombres) ──
   Hello
   Hola

── Interfaces pequeñas y específicas ──
   solo lectura ve: uno,dos
   tras escribir: uno,dos,nuevo
```

> **Ejercicio 31.1:** haz que `Person` implemente `IEquatable<Person>` y compruébalo en un `HashSet`.
>
> **Ejercicio 31.2:** escribe una `Matrix` con `IEnumerable<double>` que recorra por filas.
>
> **Ejercicio 31.3:** implementa `IDisposable` en una clase que abra un archivo y compruébalo con `using`.

---

## Lección 32 · Patrones creacionales

### Qué vas a aprender

Cinco formas de **crear** objetos cuando `new` ya no basta.

| Patrón | Problema que resuelve | Idea |
|---|---|---|
| **Factory Method** | Decidir **qué clase** crear en un único lugar | `ExporterFactory.Create("csv")` |
| **Abstract Factory** | Crear **familias coherentes** de objetos | Tema oscuro = botón oscuro + ventana oscura |
| **Builder** | Un objeto con **muchas opciones** | `.Path().Query().Header().Build()` |
| **Prototype** | **Copiar** un objeto y cambiar algo | `record` + `with` |
| **Object Pool** | Reutilizar objetos **caros** | Balas en un juego, conexiones |
| *Lazy* | No crear hasta que haga falta | `Lazy<T>` |

### Código (`32-patrones-creacionales.cs`)

```csharp
// Lección 32: patrones de diseño CREACIONALES (cómo se crean los objetos)
// Ejecutar: dotnet run 32-patrones-creacionales.cs

// ═══════════ 1. FACTORY METHOD (método de fábrica) ═══════════
// Problema: crear un objeto exige decidir QUÉ clase concreta. Se centraliza la decisión en un solo sitio.
Console.WriteLine("── Factory Method: la decisión en un único lugar ──");
foreach (string type in new[] { "pdf", "csv", "json", "xml" })
{
    try { Console.WriteLine($"   {type,-5} -> {ExporterFactory.Create(type).Export(["Ana", "Luis"])}"); }
    catch (ArgumentException e) { Console.WriteLine($"   {type,-5} -> error: {e.Message}"); }
}

// Fábrica con nombre ("constructores con nombre"): clarifica la intención frente a un constructor ambiguo
var green = Color.FromRgb(0, 255, 0);
var fromHex = Color.FromHex("#FF8800");
Console.WriteLine($"   {green} / {fromHex}   (Color.FromRgb y Color.FromHex en lugar de dos constructores confusos)");

// ═══════════ 2. ABSTRACT FACTORY (familias de objetos) ═══════════
// Problema: crear VARIOS objetos que deben ser coherentes entre sí (un tema oscuro con botones y fondos oscuros).
Console.WriteLine("\n── Abstract Factory: familias coherentes ──");
foreach (IThemeFactory theme in new IThemeFactory[] { new DarkTheme(), new LightTheme() })
{
    var button = theme.CreateButton();
    var window = theme.CreateWindow();
    Console.WriteLine($"   {theme.Name}: {window.Draw()} con {button.Draw()}");
}

// ═══════════ 3. BUILDER (constructor paso a paso) ═══════════
// Problema: un objeto con MUCHAS opciones y reglas ("telescoping constructor": new Pizza(true, false, true, null, 3...)).
Console.WriteLine("\n── Builder: construcción legible y validada ──");
var request = new HttpRequestBuilder("https://api.ejemplo.es")
    .Path("/users")
    .Query("page", "2")
    .Query("sort", "name")
    .Header("Accept", "application/json")
    .Timeout(TimeSpan.FromSeconds(10))
    .Build();                                                   // aquí se valida y se crea el objeto inmutable
Console.WriteLine($"   {request}");
try { new HttpRequestBuilder("ftp://mal").Build(); }
catch (InvalidOperationException e) { Console.WriteLine($"   Build() rechazó datos incompletos: {e.Message}"); }

// ═══════════ 4. PROTOTYPE (clonar) ═══════════
// En C#, un record ya trae el clonado con "with".
Console.WriteLine("\n── Prototype: copiar y modificar ──");
var template = new EmailTemplate("Bienvenida", "Hola {nombre}", ["info@ejemplo.es"]);
var promo = template with { Subject = "Oferta", Body = "Hola {nombre}, 20 % de descuento" };
Console.WriteLine($"   {template.Subject}: {template.Body}\n   {promo.Subject}: {promo.Body}");
Console.WriteLine($"   OJO copia superficial: las listas se COMPARTEN -> {ReferenceEquals(template.Cc, promo.Cc)}");
var deep = template with { Cc = [.. template.Cc, "ventas@ejemplo.es"] };      // para modificar una lista se crea otra
Console.WriteLine($"   copia profunda manual: {template.Cc.Length} frente a {deep.Cc.Length} destinatarios");

// ═══════════ 5. OBJECT POOL (reutilizar objetos caros) ═══════════
// Muy usado en videojuegos (balas, partículas) y servidores (conexiones, buffers).
Console.WriteLine("\n── Object Pool: reutilizar en vez de crear y destruir ──");
var pool = new Pool<Bullet>(() => new Bullet(), reset: b => b.Reset(), initial: 2);
var shots = new List<Bullet> { pool.Rent(), pool.Rent(), pool.Rent() };      // pide 3 con 2 disponibles: crea 1 más
Console.WriteLine($"   creadas: {pool.Created}, en uso: {pool.InUse}, libres: {pool.Free}");
foreach (var shot in shots) pool.Return(shot);
Console.WriteLine($"   tras devolverlas: en uso {pool.InUse}, libres {pool.Free}");
var again = pool.Rent();
Console.WriteLine($"   se reutiliza una (id {again.Id}); total creadas sigue en {pool.Created}");

// ═══════════ 6. LAZY INITIALIZATION ═══════════
Console.WriteLine("\n── Inicialización perezosa: no se crea hasta que hace falta ──");
var report = new Lazy<HeavyReport>(() => { Console.WriteLine("   (creando el informe pesado...)"); return new HeavyReport(); });
Console.WriteLine($"   ¿creado todavía? {report.IsValueCreated}");
Console.WriteLine($"   primer uso: {report.Value.Title}");
Console.WriteLine($"   segundo uso: {report.Value.Title}   (no se vuelve a crear)");

// ═══════════ TIPOS ═══════════
interface IExporter { string Export(IEnumerable<string> items); }
class CsvExporter : IExporter { public string Export(IEnumerable<string> items) => string.Join(";", items); }
class JsonExporter : IExporter { public string Export(IEnumerable<string> items) => "[" + string.Join(",", items.Select(i => $"\"{i}\"")) + "]"; }
class PdfExporter : IExporter { public string Export(IEnumerable<string> items) => $"%PDF({items.Count()} líneas)"; }

static class ExporterFactory
{
    public static IExporter Create(string type) => type.ToLowerInvariant() switch
    {
        "csv" => new CsvExporter(),
        "json" => new JsonExporter(),
        "pdf" => new PdfExporter(),
        _ => throw new ArgumentException($"formato desconocido '{type}'"),
    };
}

readonly record struct Color(byte R, byte G, byte B)
{
    public static Color FromRgb(byte r, byte g, byte b) => new(r, g, b);
    public static Color FromHex(string hex) =>
        new(Convert.ToByte(hex[1..3], 16), Convert.ToByte(hex[3..5], 16), Convert.ToByte(hex[5..7], 16));
    public override string ToString() => $"rgb({R},{G},{B})";
}

interface IButton { string Draw(); }
interface IWindow { string Draw(); }
interface IThemeFactory { string Name { get; } IButton CreateButton(); IWindow CreateWindow(); }
class DarkButton : IButton { public string Draw() => "botón negro"; }
class DarkWindow : IWindow { public string Draw() => "ventana oscura"; }
class LightButton : IButton { public string Draw() => "botón blanco"; }
class LightWindow : IWindow { public string Draw() => "ventana clara"; }
class DarkTheme : IThemeFactory { public string Name => "Oscuro"; public IButton CreateButton() => new DarkButton(); public IWindow CreateWindow() => new DarkWindow(); }
class LightTheme : IThemeFactory { public string Name => "Claro"; public IButton CreateButton() => new LightButton(); public IWindow CreateWindow() => new LightWindow(); }

sealed record HttpRequest(string Url, IReadOnlyDictionary<string, string> Headers, TimeSpan Timeout)
{
    public override string ToString() => $"GET {Url} (timeout {Timeout.TotalSeconds}s, cabeceras: {string.Join(", ", Headers.Select(h => $"{h.Key}={h.Value}"))})";
}

class HttpRequestBuilder(string baseUrl)
{
    private string _path = "";
    private readonly List<string> _query = [];
    private readonly Dictionary<string, string> _headers = [];
    private TimeSpan _timeout = TimeSpan.FromSeconds(30);

    public HttpRequestBuilder Path(string path) { _path = path; return this; }                        // cada método devuelve "this": se encadenan
    public HttpRequestBuilder Query(string key, string value) { _query.Add($"{key}={Uri.EscapeDataString(value)}"); return this; }
    public HttpRequestBuilder Header(string key, string value) { _headers[key] = value; return this; }
    public HttpRequestBuilder Timeout(TimeSpan timeout) { _timeout = timeout; return this; }

    public HttpRequest Build()
    {
        if (!baseUrl.StartsWith("https://")) throw new InvalidOperationException("la URL debe empezar por https://");
        string url = baseUrl + _path + (_query.Count > 0 ? "?" + string.Join("&", _query) : "");
        return new HttpRequest(url, _headers, _timeout);
    }
}

record EmailTemplate(string Subject, string Body, string[] Cc);

class Bullet
{
    private static int _next;
    public int Id { get; } = ++_next;
    public void Reset() { /* posición a cero, daño a cero... */ }
}

class Pool<T>(Func<T> create, Action<T> reset, int initial = 0) where T : class
{
    private readonly Stack<T> _free = new(Enumerable.Range(0, initial).Select(_ => create()));
    public int Created { get; private set; } = initial;
    public int Free => _free.Count;
    public int InUse => Created - Free;
    public T Rent() { if (_free.Count > 0) return _free.Pop(); Created++; return create(); }
    public void Return(T item) { reset(item); _free.Push(item); }
}

class HeavyReport { public string Title => "Informe anual"; }
```

### Cómo se lee, paso a paso

1. **Factory Method:** `ExporterFactory.Create(type)` es **la única** parte que conoce las clases concretas. Añadir `xml` se hace en un lugar. Las **fábricas con nombre** (`Color.FromRgb`, `Color.FromHex`) aclaran la intención mejor que dos constructores.
2. **Abstract Factory:** cada tema (`DarkTheme`, `LightTheme`) crea **su** botón y **su** ventana, así que **nunca se mezclan** piezas de familias distintas.
3. **Builder:** cada método devuelve `this` (se **encadenan**); `Build()` **valida** y crea el objeto **inmutable**. Evita el «constructor con 9 parámetros».
4. **Prototype:** `with` crea una **copia**. **Cuidado:** la copia es **superficial**: los arrays y listas se **comparten** (`True` en la salida). Para cambiar una lista hay que **crear otra**.
5. **Object Pool:** `Rent()` toma uno libre o crea uno nuevo; `Return()` lo **reinicia** y lo devuelve al montón. Las «creadas» **no suben** cuando se reutiliza.
6. **`Lazy<T>`:** el informe pesado **no existe** hasta el primer `.Value`, y solo se crea **una vez**.

### Resultado real

```text
── Factory Method: la decisión en un único lugar ──
   pdf   -> %PDF(2 líneas)
   csv   -> Ana;Luis
   json  -> ["Ana","Luis"]
   xml   -> error: formato desconocido 'xml'
   rgb(0,255,0) / rgb(255,136,0)   (Color.FromRgb y Color.FromHex en lugar de dos constructores confusos)

── Abstract Factory: familias coherentes ──
   Oscuro: ventana oscura con botón negro
   Claro: ventana clara con botón blanco

── Builder: construcción legible y validada ──
   GET https://api.ejemplo.es/users?page=2&sort=name (timeout 10s, cabeceras: Accept=application/json)
   Build() rechazó datos incompletos: la URL debe empezar por https://

── Prototype: copiar y modificar ──
   Bienvenida: Hola {nombre}
   Oferta: Hola {nombre}, 20 % de descuento
   OJO copia superficial: las listas se COMPARTEN -> True
   copia profunda manual: 1 frente a 2 destinatarios

── Object Pool: reutilizar en vez de crear y destruir ──
   creadas: 3, en uso: 3, libres: 0
   tras devolverlas: en uso 0, libres 3
   se reutiliza una (id 3); total creadas sigue en 3

── Inicialización perezosa: no se crea hasta que hace falta ──
   ¿creado todavía? False
   (creando el informe pesado...)
   primer uso: Informe anual
   segundo uso: Informe anual   (no se vuelve a crear)
```

> **Ejercicio 32.1:** añade `xml` a `ExporterFactory` y comprueba que el resto del código **no cambia**.
>
> **Ejercicio 32.2:** crea un `PizzaBuilder` con tamaño, ingredientes y masa, que rechace una pizza sin masa.
>
> **Ejercicio 32.3:** usa el `Pool<T>` para reutilizar 1000 `StringBuilder` y compara el tiempo con crear nuevos.

---

## Lección 33 · Patrones estructurales

### Qué vas a aprender

Cinco formas de **combinar** objetos para que encajen y para **añadirles** comportamiento sin tocarlos.

| Patrón | Problema | Idea | Ejemplo cotidiano |
|---|---|---|---|
| **Adapter** | Una clase útil con **otra interfaz** | Una pieza que traduce | Adaptador de enchufe |
| **Decorator** | Añadir comportamiento **sin subclases** | Envolver con el mismo contrato | Capas de cebolla: caché, registro |
| **Facade** | Un subsistema **complejo** | Una puerta con un método sencillo | Recepción de un hotel |
| **Proxy** | Controlar el **acceso** | Un representante | Portero |
| **Composite** | Tratar **igual** a un elemento y a un grupo | Árbol recursivo | Archivos y carpetas |

### Código (`33-patrones-estructurales.cs`)

```csharp
// Lección 33: patrones de diseño ESTRUCTURALES (cómo se combinan los objetos)
// Ejecutar: dotnet run 33-patrones-estructurales.cs
using System.Diagnostics;

// ═══════════ 1. ADAPTER (adaptador) ═══════════
// Problema: tienes una clase que hace lo que necesitas pero con OTRA interfaz (código de terceros, antiguo...).
// Solución: una pieza intermedia que traduce. Como un adaptador de enchufe.
Console.WriteLine("── Adapter: que lo viejo hable el idioma nuevo ──");
ITemperatureSensor modern = new CelsiusSensor(21.5);
ITemperatureSensor legacy = new FahrenheitSensorAdapter(new OldFahrenheitSensor());     // el viejo, envuelto
foreach (var sensor in new[] { modern, legacy }) Console.WriteLine($"   {sensor.GetType().Name,-28} {sensor.ReadCelsius():F1} °C");

// ═══════════ 2. DECORATOR (decorador) ═══════════
// Problema: añadir comportamiento (registro, caché, reintentos) SIN tocar la clase ni crear mil subclases.
// Solución: envolver el objeto en otro que cumple el mismo contrato. Se pueden apilar como capas de cebolla.
Console.WriteLine("\n── Decorator: capas de comportamiento apilables ──");
IWeatherService service = new SlowWeatherService();
service = new CachedWeatherService(service);             // capa de caché
service = new LoggedWeatherService(service);             // capa de registro por fuera
foreach (string city in new[] { "Madrid", "Madrid", "Sevilla", "Madrid" })
    Console.WriteLine($"   -> {service.GetTemperature(city)} °C\n");

// ═══════════ 3. FACADE (fachada) ═══════════
// Problema: un subsistema complejo con muchos pasos. Solución: UNA puerta con un método sencillo.
Console.WriteLine("── Facade: una puerta sencilla a muchos pasos ──");
var shop = new OrderFacade(new Inventory(), new Payments(), new Shipping());
Console.WriteLine($"   {shop.Order("teclado", 1, "tarjeta")}");
Console.WriteLine($"   {shop.Order("monitor", 5, "tarjeta")}");

// ═══════════ 4. PROXY ═══════════
// Un representante que controla el acceso: carga perezosa, permisos, caché remota...
Console.WriteLine("\n── Proxy: control de acceso ──");
IDocument doc = new ProtectedDocument(new RealDocument("Plan secreto"), user: "visita");
Console.WriteLine($"   visita lee: {doc.Read()}");
IDocument admin = new ProtectedDocument(new RealDocument("Plan secreto"), user: "admin");
Console.WriteLine($"   admin lee:  {admin.Read()}");

// ═══════════ 5. COMPOSITE (composición en árbol) ═══════════
// Problema: tratar IGUAL a un elemento y a un grupo de elementos (archivos y carpetas, menús, escenas de un juego).
Console.WriteLine("\n── Composite: el árbol se trata como una sola cosa ──");
var root = new Folder("proyecto",
    new FileNode("README.md", 2),
    new Folder("src",
        new FileNode("Program.cs", 5),
        new FileNode("Service.cs", 12),
        new Folder("tests", new FileNode("Tests.cs", 8))),
    new FileNode("proyecto.csproj", 1));
root.Print(0);
Console.WriteLine($"   Tamaño total: {root.Size} KB   (la misma llamada vale para un archivo, una carpeta o todo el árbol)");

// ═══════════ TIPOS ═══════════
interface ITemperatureSensor { double ReadCelsius(); }
class CelsiusSensor(double value) : ITemperatureSensor { public double ReadCelsius() => value; }
class OldFahrenheitSensor { public double GetFahrenheitReading() => 72.5; }            // no se puede modificar (es "de terceros")
class FahrenheitSensorAdapter(OldFahrenheitSensor old) : ITemperatureSensor
{
    public double ReadCelsius() => (old.GetFahrenheitReading() - 32) * 5 / 9;           // la traducción
}

interface IWeatherService { double GetTemperature(string city); }
class SlowWeatherService : IWeatherService
{
    public double GetTemperature(string city)
    {
        Thread.Sleep(30);                                                               // simula una llamada de red lenta
        return city == "Madrid" ? 18.5 : 24.0;
    }
}
class CachedWeatherService(IWeatherService inner) : IWeatherService
{
    private readonly Dictionary<string, double> _cache = [];
    public double GetTemperature(string city)
    {
        if (_cache.TryGetValue(city, out double cached)) { Console.WriteLine($"   [caché] acierto para {city}"); return cached; }
        return _cache[city] = inner.GetTemperature(city);
    }
}
class LoggedWeatherService(IWeatherService inner) : IWeatherService
{
    public double GetTemperature(string city)
    {
        var watch = Stopwatch.StartNew();
        double value = inner.GetTemperature(city);
        Console.WriteLine($"   [log] {city} = {value} °C ({(watch.ElapsedMilliseconds < 15 ? "rápido" : "lento")})");
        return value;
    }
}

class Inventory { public bool InStock(string item, int qty) => item == "teclado" || qty <= 2; public void Reserve(string item, int qty) { } }
class Payments { public bool Charge(string method, decimal amount) => method == "tarjeta"; }
class Shipping { public string Schedule(string item) => $"envío de {item} para mañana"; }
class OrderFacade(Inventory inventory, Payments payments, Shipping shipping)           // oculta la coreografía de tres sistemas
{
    public string Order(string item, int qty, string method)
    {
        if (!inventory.InStock(item, qty)) return $"✗ No hay {qty} unidades de {item}";
        if (!payments.Charge(method, qty * 49.9m)) return "✗ Pago rechazado";
        inventory.Reserve(item, qty);
        return $"✓ {shipping.Schedule(item)}";
    }
}

interface IDocument { string Read(); }
class RealDocument(string content) : IDocument { public string Read() => content; }
class ProtectedDocument(IDocument real, string user) : IDocument
{
    public string Read() => user == "admin" ? real.Read() : "✗ acceso denegado";
}

abstract record Node(string Name)
{
    public abstract int Size { get; }
    public abstract void Print(int level);
}
record FileNode(string Name, int Kb) : Node(Name)
{
    public override int Size => Kb;
    public override void Print(int level) => Console.WriteLine($"   {new string(' ', level * 3)}- {Name} ({Kb} KB)");
}
record Folder(string Name, params Node[] Children) : Node(Name)
{
    public override int Size => Children.Sum(c => c.Size);                              // suma recursiva
    public override void Print(int level)
    {
        Console.WriteLine($"   {new string(' ', level * 3)}+ {Name}/ ({Size} KB)");
        foreach (var child in Children) child.Print(level + 1);
    }
}
```

### Cómo se lee, paso a paso

1. **Adapter:** `FahrenheitSensorAdapter` implementa `ITemperatureSensor` y **por dentro** llama al sensor antiguo convirtiendo los grados. El resto del programa **no sabe** que hay un sensor viejo.
2. **Decorator:** `CachedWeatherService` y `LoggedWeatherService` implementan `IWeatherService` **y reciben otro `IWeatherService`**. Se **apilan**: `Logged(Cached(Slow))`. En la salida, la 2.ª consulta de Madrid es `[caché] acierto` y `rápido`.
3. **Facade:** `OrderFacade.Order(...)` coordina tres sistemas. El cliente llama a **un solo método**.
4. **Proxy:** `ProtectedDocument` tiene **la misma interfaz** que `RealDocument` pero comprueba permisos antes.
5. **Composite:** `Folder.Size` suma recursivamente a sus hijos; `FileNode.Size` devuelve su tamaño. **La misma llamada** sirve para un archivo, una carpeta o todo el árbol.

**Decorator y Proxy se parecen** (los dos envuelven y cumplen el mismo contrato). Se distinguen por la **intención**: el decorador **añade** comportamiento; el proxy **controla el acceso**.

### Resultado real

```text
── Adapter: que lo viejo hable el idioma nuevo ──
   CelsiusSensor                21.5 °C
   FahrenheitSensorAdapter      22.5 °C

── Decorator: capas de comportamiento apilables ──
   [log] Madrid = 18.5 °C (lento)
   -> 18.5 °C

   [caché] acierto para Madrid
   [log] Madrid = 18.5 °C (rápido)
   -> 18.5 °C

   [log] Sevilla = 24 °C (lento)
   -> 24 °C

   [caché] acierto para Madrid
   [log] Madrid = 18.5 °C (rápido)
   -> 18.5 °C

── Facade: una puerta sencilla a muchos pasos ──
   ✓ envío de teclado para mañana
   ✗ No hay 5 unidades de monitor

── Proxy: control de acceso ──
   visita lee: ✗ acceso denegado
   admin lee:  Plan secreto

── Composite: el árbol se trata como una sola cosa ──
   + proyecto/ (28 KB)
      - README.md (2 KB)
      + src/ (25 KB)
         - Program.cs (5 KB)
         - Service.cs (12 KB)
         + tests/ (8 KB)
            - Tests.cs (8 KB)
      - proyecto.csproj (1 KB)
   Tamaño total: 28 KB   (la misma llamada vale para un archivo, una carpeta o todo el árbol)
```

> **Ejercicio 33.1:** añade un decorador `RetryWeatherService` que reintente 3 veces si falla.
>
> **Ejercicio 33.2:** añade a `Folder` una operación `Find(name)` que busque recursivamente.
>
> **Ejercicio 33.3:** escribe un adaptador entre `System.IO.Stream` y una interfaz propia `IReader`.

---

## Lección 34 · Patrones de comportamiento

### Qué vas a aprender

Cinco formas de organizar **quién hace qué y cuándo**.

| Patrón | Problema | Idea |
|---|---|---|
| **Strategy** | Un algoritmo con **variantes** | Cada variante es una pieza intercambiable |
| **Observer** | **Avisar** a otros cuando algo cambia | Eventos (`event`) |
| **Command** | Convertir una **acción en objeto** | Guardar, deshacer, rehacer, encolar |
| **State** | El comportamiento **depende del estado** | Un objeto por estado |
| **Chain of Responsibility** | Pasos que pueden **cortar** la petición | Tubería (*middleware*) |
| *Template Method* | Pasos **fijos** con huecos | Lección 28 |
| *Iterator* | Recorrer sin conocer la estructura | `yield return` (lección 12) |

### Código (`34-patrones-comportamiento.cs`)

```csharp
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
```

### Cómo se lee, paso a paso

1. **Strategy:** `Cart.Total(IDiscount)` **no sabe** qué descuento es. Para un descuento nuevo se escribe **una clase** y no se toca el carrito. Una estrategia sencilla puede ser **solo una función** (`Func<decimal, decimal>`).
2. **Observer:** `StockTicker` expone un `event`; cada interesado se **suscribe** y el emisor **no los conoce**. `?.Invoke(...)` evita fallar si nadie escucha.
3. **Command + deshacer:** cada acción es un objeto con `Execute()` y `Undo()`. Un **historial** (dos pilas) permite deshacer y rehacer. `UppercaseCommand` **guarda** el texto anterior para poder volver.
4. **State:** cada estado es una clase que sabe **qué transiciones admite**; las ilegales lanzan una excepción. Se acabaron los `if (estado == ...)` repartidos por todo el código.
5. **Chain of Responsibility:** cada paso decide **llamar al siguiente** (`next(req)`) o **cortar** devolviendo `401` o `403`. **Es exactamente cómo funciona el *middleware* de ASP.NET** (lección 39).

### Resultado real

```text
── Strategy: algoritmos intercambiables ──
   sin descuento              total =  225.00 €
   10 % de descuento          total =  202.50 €
   -30 € si pasa de 200       total =  195.00 €
   función lambda (viernes negro) total =  157.50 €

── Observer: avisar a los interesados ──
   [pantalla] ACME: 100 -> 105
   [pantalla] ACME: 105 -> 90
   [alerta] ¡ACME cae más de un 10 %!

── Command: acciones que se pueden deshacer ──
   texto: "HOLA MUNDO"
   deshacer -> "Hola mundo"
   deshacer -> "Hola"
   rehacer  -> "Hola mundo"

── State: el comportamiento depende del estado ──
   ship     -> ✗ un pedido nuevo no admite 'ship'
   pay      -> estado: Paid
   pay      -> ✗ un pedido pagado no admite 'pay'
   ship     -> estado: Shipped
   deliver  -> estado: Delivered
   cancel   -> ✗ el pedido ya terminó

── Chain of Responsibility: una tubería de pasos ──
   [1 registro] entra /inicio
   [1 registro] sale 200 OK (/inicio para ana)
   => 200 OK (/inicio para ana)

   [1 registro] entra /inicio
   [1 registro] sale 401 No autenticado
   => 401 No autenticado

   [1 registro] entra /admin/panel
   [1 registro] sale 403 Prohibido
   => 403 Prohibido

   [1 registro] entra /admin/panel
   [1 registro] sale 200 OK (/admin/panel para admin)
   => 200 OK (/admin/panel para admin)
```

> **Ejercicio 34.1:** añade `BuyOneGetOne` (2×1) a las estrategias.
>
> **Ejercicio 34.2:** añade un `ClearCommand` con deshacer a `TextEditor`.
>
> **Ejercicio 34.3:** añade un estado `Returned` al pedido, permitido solo desde `Delivered`.

---

## Lección 35 · SOLID

### Qué vas a aprender

Cinco principios que **resumen** todo el diseño orientado a objetos. Cada uno se enseña con **el código mal** y **el código bien**, para que veas el problema real.

| Letra | Principio | En una frase | Síntoma de que lo incumples |
|---|---|---|---|
| **S** | Responsabilidad única | Una clase, **una razón** para cambiar | Una clase que calcula, imprime **y** guarda |
| **O** | Abierto / cerrado | **Extender** sin **modificar** | Un `switch` por tipo que crece con cada tipo nuevo |
| **L** | Sustitución de Liskov | La hija **cumple lo que promete** la base | `NotSupportedException` o resultados sorprendentes en una hija |
| **I** | Segregación de interfaces | Interfaces **pequeñas** | Clases que implementan métodos «para nada» |
| **D** | Inversión de dependencias | Depender de **contratos**, no de clases | `new` de clases concretas dentro de la lógica |

### Código (`35-solid.cs`)

```csharp
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
```

### Cómo se lee, paso a paso

1. **S:** `InvoiceBad` calcula, imprime y guarda: **tres razones** para cambiar. `Invoice`, `InvoicePrinter` e `InvoiceStore` se **separan**.
2. **O:** `AreaCalculatorBad` tiene un `switch` por figura. En el diseño bueno **cada figura sabe su área**, y `Triangle` se añadió **sin editar** `Calculator`.
3. **L:** `SquareBad` hereda de `RectBad` pero al fijar `Height = 2` cambia también `Width`: el área sale **4** en vez de **10**. La hija **rompió la promesa**. La solución: **no modelar «es un» donde el comportamiento no se cumple**.
4. **I:** `RobotBad` está **obligado** a implementar `Eat()` y lanza una excepción. Con `IWorkable` e `IFeedable` separadas, cada clase cumple **solo lo suyo**.
5. **D:** `AlertService` recibe un `IMessageSender`. En producción es SMS; en una prueba, un `FakeSender` que **guarda** el mensaje para comprobarlo.

### Resultado real

```text
S · Una clase, una razón para cambiar
   mal  : factura de 121.00 € impresa
   mal  : guardada en la base de datos
   bien : factura de 121.00 € impresa
   bien : guardada en la base de datos

O · Añadir una figura sin tocar el calculador
   mal  : 12.57
   bien : Circle 12.57, Rect 12.00, Triangle 6.00  (Triangle se añadió sin editar Calculator: 30.57)

L · La subclase no debe romper lo que promete la base
   mal  : Width=5, Height=2 -> área 4   (¡debería ser 10!) El Cuadrado rompió la promesa del Rectángulo.
   bien : 10 y 9   (ninguno promete algo que el otro no cumpla)

I · Interfaces pequeñas
   mal  : un robot no come: ¡esta clase no debería tener que implementar Eat()!
   bien : la persona trabaja
   bien : el robot trabaja 24 h
   bien : la persona come

D · Depender de contratos
   bien : con un falso se comprueba el mensaje sin enviar nada -> 'ALERTA: Disco al 95 %'
   bien : [SMS real] ALERTA: Disco al 95 %

── Resumen ──
S  Cada clase hace UNA cosa.                          -> clases pequeñas y fáciles de probar
O  Se añade comportamiento nuevo con código nuevo.     -> sin romper lo que ya funciona
L  Una subclase cumple lo que la base promete.         -> la herencia no da sorpresas
I  Muchas interfaces pequeñas mejor que una gigante.   -> nadie implementa lo que no usa
D  Depender de contratos, no de clases concretas.      -> piezas intercambiables y probables
```

### SOLID, con sentido común

| Aviso | Por qué |
|---|---|
| SOLID son **guías**, no leyes | Un script de 30 líneas **no necesita** cuatro interfaces |
| No apliques un principio **antes** de tener el problema | Es el camino al sobrediseño (*over-engineering*) |
| Se aprende **notando el dolor**: «cambié X y se rompió Y» | Y entonces se aplica el principio que corresponde |

> **Ejercicio 35.1:** busca en TaskFlow (lección 27) un ejemplo de **cada** principio.
>
> **Ejercicio 35.2:** escribe una clase `ReportGenerator` que viole **S** y refactorízala en tres.
>
> **Ejercicio 35.3:** añade un `Pentagon` a las figuras sin tocar `Calculator`.

---

## Lección 36 · Herramientas del lenguaje que completan la orientación a objetos

### Qué vas a aprender

Los mecanismos de C# que hacen que tus clases se **sientan** como parte del lenguaje.

| Herramienta | Para qué | Ejemplo |
|---|---|---|
| **Método de extensión** | Añadir métodos a tipos que **no controlas** | `"hola".Capitalize()` |
| **Operador propio** | Que `a + b` funcione con tus tipos | `Vector2D + Vector2D` |
| **Conversión implícita** | Pasar de un tipo a otro **sin escribirlo** | `(5.0, 6.0)` → `Vector2D` |
| **Indexador** | `objeto[clave]` | `matrix[0,1]`, `phone["Ana"]` |
| **`partial`** | Repartir una clase en varios archivos | Código generado + código tuyo |
| **Atributo + reflexión** | Marcar el código y que el programa **se lea a sí mismo** | `[MinLength(3)]` |
| **Atributos del llamador** | El compilador rellena el nombre y la línea | `[CallerMemberName]` |
| **Matemáticas genéricas** | Una función para `int`, `double`, `decimal` | `where T : INumber<T>` |
| **Miembros estáticos en interfaces** | Contratos sobre el **tipo**, no la instancia | `static abstract` |

### Código (`36-herramientas.cs`)

```csharp
// Lección 36: herramientas del lenguaje que completan la programación orientada a objetos
// Ejecutar: dotnet run 36-herramientas.cs
using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;

// ═══════════ 1. MÉTODOS DE EXTENSIÓN: añadir métodos a tipos que NO controlas ═══════════
Console.WriteLine("── Métodos de extensión ──");
Console.WriteLine($"   \"hola mundo\".Capitalize()  -> {"hola mundo".Capitalize()}");
Console.WriteLine($"   \"ana@correo.es\".IsEmail()  -> {"ana@correo.es".IsEmail()}");
Console.WriteLine($"   [1,2,3,4,5,6].Chunked(4)   -> {string.Join(" | ", new[] { 1, 2, 3, 4, 5, 6 }.Chunked(4).Select(c => string.Join(",", c)))}");
Console.WriteLine($"   LINQ entero está hecho así: Where, Select, OrderBy son métodos de extensión de IEnumerable<T>");

// ═══════════ 2. SOBRECARGA DE OPERADORES ═══════════
Console.WriteLine("\n── Operadores propios ──");
var a = new Vector2D(1, 2); var b = new Vector2D(3, 4);
Console.WriteLine($"   {a} + {b} = {a + b};  {a} * 3 = {a * 3};  -{a} = {-a};  |{b}| = {b.Length}");
Console.WriteLine($"   {a} == (1,2): {a == new Vector2D(1, 2)};  conversión implícita desde tupla: {(Vector2D)(5.0, 6.0)}");

// ═══════════ 3. INDEXADORES ═══════════
Console.WriteLine("\n── Indexadores: objeto[clave] ──");
var matrix = new Matrix(2, 3);
matrix[0, 1] = 5; matrix[1, 2] = 9;
Console.WriteLine($"   matrix[0,1] = {matrix[0, 1]}, matrix[1,2] = {matrix[1, 2]}");
var phone = new PhoneBook { ["Ana"] = "600111222" };
Console.WriteLine($"   phone[\"Ana\"] = {phone["Ana"]}; phone[\"Zoe\"] = {phone["Zoe"] ?? "(no existe)"}");

// ═══════════ 4. CLASES PARCIALES ═══════════
Console.WriteLine("\n── partial: una clase repartida en varios archivos ──");
Console.WriteLine($"   {new Customer { Name = "Ana", Surname = "Ruiz" }.FullName}   (el código generado y el escrito a mano conviven sin pisarse)");

// ═══════════ 5. ATRIBUTOS Y REFLEXIÓN ═══════════
Console.WriteLine("\n── Atributos + reflexión: el programa se lee a sí mismo ──");
var user = new SignUpForm { Name = "A", Email = "no-es-correo", Age = 17 };
foreach (var error in Validator.Validate(user)) Console.WriteLine($"   ✗ {error}");
Console.WriteLine("   (así funciona la validación automática de ASP.NET y de muchos frameworks)");

// ═══════════ 6. ATRIBUTOS DEL COMPILADOR ═══════════
Console.WriteLine("\n── Información del llamador ──");
new Worker().Run();
Console.WriteLine($"   nameof(Vector2D.Length) = {nameof(Vector2D.Length)}   (no se rompe al renombrar, a diferencia de un texto)");

// ═══════════ 7. MATEMÁTICAS GENÉRICAS (.NET 7+) ═══════════
Console.WriteLine("\n── Matemáticas genéricas: INumber<T> ──");
Console.WriteLine($"   Sum<int>    = {Sum([1, 2, 3])}");
Console.WriteLine($"   Sum<double> = {Sum([1.5, 2.5])}");
Console.WriteLine($"   Sum<decimal>= {Sum([0.1m, 0.2m])}");

// ═══════════ 8. MIEMBROS ESTÁTICOS EN INTERFACES ═══════════
Console.WriteLine("\n── Interfaces con miembros estáticos ──");
Console.WriteLine($"   {Describe<Dog>()} / {Describe<Cat>()}");

static T Sum<T>(IEnumerable<T> values) where T : INumber<T>                  // sirve para int, double, decimal...
{
    T total = T.Zero;
    foreach (T v in values) total += v;
    return total;
}

static string Describe<T>() where T : IAnimal => $"{T.Species} hace {T.Sound}";    // se llama sin crear ningún objeto

// ═══════════ TIPOS ═══════════
static class Extensions
{
    public static string Capitalize(this string text) =>                    // "this" en el primer parámetro = método de extensión
        text.Length == 0 ? text : char.ToUpper(text[0]) + text[1..];
    public static bool IsEmail(this string text) => text.Contains('@') && text.Split('@') is [{ Length: > 0 }, var domain] && domain.Contains('.');
    public static IEnumerable<T[]> Chunked<T>(this IEnumerable<T> source, int size) => source.Chunk(size);
}

readonly record struct Vector2D(double X, double Y)
{
    public static Vector2D operator +(Vector2D a, Vector2D b) => new(a.X + b.X, a.Y + b.Y);
    public static Vector2D operator *(Vector2D a, double k) => new(a.X * k, a.Y * k);
    public static Vector2D operator -(Vector2D a) => new(-a.X, -a.Y);
    public static implicit operator Vector2D((double X, double Y) t) => new(t.X, t.Y);     // conversión automática desde una tupla
    public double Length => Math.Sqrt(X * X + Y * Y);
    public override string ToString() => $"({X},{Y})";
}

class Matrix(int rows, int cols)
{
    private readonly double[,] _data = new double[rows, cols];
    public double this[int row, int col] { get => _data[row, col]; set => _data[row, col] = value; }    // indexador de dos índices
}
class PhoneBook
{
    private readonly Dictionary<string, string> _numbers = [];
    public string? this[string name] { get => _numbers.GetValueOrDefault(name); set => _numbers[name] = value!; }
}

partial class Customer                                                      // parte escrita a mano
{
    public string FullName => $"{Name} {Surname}";
}
partial class Customer                                                      // parte "generada" (en la vida real, en otro archivo)
{
    public string Name { get; set; } = "";
    public string Surname { get; set; } = "";
}

[AttributeUsage(AttributeTargets.Property)]
class MinLengthAttribute(int length) : Attribute { public int Length { get; } = length; }
[AttributeUsage(AttributeTargets.Property)]
class EmailAttribute : Attribute;
[AttributeUsage(AttributeTargets.Property)]
class RangeAttribute(int min, int max) : Attribute { public int Min { get; } = min; public int Max { get; } = max; }

class SignUpForm
{
    [MinLength(3)] public string Name { get; set; } = "";
    [Email] public string Email { get; set; } = "";
    [Range(18, 120)] public int Age { get; set; }
}

static class Validator
{
    public static IEnumerable<string> Validate(object target)
    {
        foreach (PropertyInfo property in target.GetType().GetProperties())     // reflexión: pregunta qué propiedades tiene
        {
            object? value = property.GetValue(target);
            if (property.GetCustomAttribute<MinLengthAttribute>() is { } min && (value as string ?? "").Length < min.Length)
                yield return $"{property.Name} necesita al menos {min.Length} caracteres";
            if (property.GetCustomAttribute<EmailAttribute>() is not null && !((value as string) ?? "").IsEmail())
                yield return $"{property.Name} no es un correo válido";
            if (property.GetCustomAttribute<RangeAttribute>() is { } range && value is int n && (n < range.Min || n > range.Max))
                yield return $"{property.Name} debe estar entre {range.Min} y {range.Max}";
        }
    }
}


interface IAnimal { static abstract string Species { get; } static abstract string Sound { get; } }
class Dog : IAnimal { public static string Species => "perro"; public static string Sound => "guau"; }
class Cat : IAnimal { public static string Species => "gato"; public static string Sound => "miau"; }

class Worker { public void Run() => Tracer.Trace("algo ocurrió"); }          // el llamador es "Run"
static class Tracer
{
    public static void Trace(string message, [CallerMemberName] string? caller = null, [CallerLineNumber] int line = 0)
        => Console.WriteLine($"   [{caller}:{line}] {message}");              // el compilador rellena caller y line SOLO
}
```

### Cómo se lee, paso a paso

1. **Extensión:** `this string text` en el primer parámetro de un método **estático** de una clase **estática**. **LINQ entero** (`Where`, `Select`…) está hecho así.
2. **Operadores:** `public static Vector2D operator +(...)`. Con `==` hay que definir también `!=` (aquí lo hace el `record struct`).
3. **Conversión implícita:** `implicit operator` permite `(Vector2D)(5.0, 6.0)` o asignar una tupla directamente.
4. **Indexador:** `public double this[int row, int col]` con `get` y `set`. Admite **cualquier tipo de clave**.
5. **`partial class`:** el compilador **junta** las partes. Es lo que usan los diseñadores gráficos y los generadores de código.
6. **Atributo + reflexión:** `Validator` **pregunta por las propiedades** del objeto (`GetProperties`), lee sus atributos y valida **sin conocer la clase**. Así funcionan `[Required]` de ASP.NET o `[Fact]` de xUnit.
7. **`[CallerMemberName]`:** el compilador **inserta** el nombre del método llamador y el número de línea. Nunca se desactualizan al renombrar.
8. **`INumber<T>`:** `Sum<T>` sirve para **todos** los tipos numéricos sin duplicar código.
9. **`static abstract`:** `Describe<Dog>()` accede a `T.Species` **sin crear un objeto**.

### Resultado real

```text
/tmp/claude-0/-home-user-claude/ddd43a70-8f87-5490-9d3f-755ecbba3013/scratchpad/curso3/36-herramientas.cs(120,43): warning IL2075: 'this' argument does not satisfy 'DynamicallyAccessedMemberTypes.PublicProperties' in call to 'System.Type.GetProperties()'. The return value of method 'System.Object.GetType()' does not have matching annotations. The source value must declare at least the same requirements as those declared on the target location it is assigned to.
── Métodos de extensión ──
   "hola mundo".Capitalize()  -> Hola mundo
   "ana@correo.es".IsEmail()  -> True
   [1,2,3,4,5,6].Chunked(4)   -> 1,2,3,4 | 5,6
   LINQ entero está hecho así: Where, Select, OrderBy son métodos de extensión de IEnumerable<T>

── Operadores propios ──
   (1,2) + (3,4) = (4,6);  (1,2) * 3 = (3,6);  -(1,2) = (-1,-2);  |(3,4)| = 5
   (1,2) == (1,2): True;  conversión implícita desde tupla: (5,6)

── Indexadores: objeto[clave] ──
   matrix[0,1] = 5, matrix[1,2] = 9
   phone["Ana"] = 600111222; phone["Zoe"] = (no existe)

── partial: una clase repartida en varios archivos ──
   Ana Ruiz   (el código generado y el escrito a mano conviven sin pisarse)

── Atributos + reflexión: el programa se lee a sí mismo ──
   ✗ Name necesita al menos 3 caracteres
   ✗ Email no es un correo válido
   ✗ Age debe estar entre 18 y 120
   (así funciona la validación automática de ASP.NET y de muchos frameworks)

── Información del llamador ──
   [Run:138] algo ocurrió
   nameof(Vector2D.Length) = Length   (no se rompe al renombrar, a diferencia de un texto)

── Matemáticas genéricas: INumber<T> ──
   Sum<int>    = 6
   Sum<double> = 4
   Sum<decimal>= 0.3

── Interfaces con miembros estáticos ──
   perro hace guau / gato hace miau
```

### Errores típicos

| Error | Causa | Solución |
|---|---|---|
| El método de extensión «no existe» | Falta el `using` del espacio de nombres | Añade el `using` |
| `==` definido pero no `!=` | Los dos son obligatorios | Defínelos juntos (o usa `record`) |
| Reflexión lenta o frágil | Se usa en un bucle caliente o sin `Cache` | Resuelve una vez y guarda; considera generadores de código |
| El `partial` no compila | Una parte está en **otro proyecto** | Deben estar en el **mismo** proyecto |

> **Ejercicio 36.1:** escribe `ToSlug()` (extensión de `string`) que convierta «Hola Mundo!» en `hola-mundo`.
>
> **Ejercicio 36.2:** añade un atributo `[Required]` al validador de la lección.
>
> **Ejercicio 36.3:** añade un operador `-` y `/` a `Vector2D`.

---

## Resumen de la Parte III

| Si quieres… | Usa… |
|---|---|
| Compartir código entre clases parecidas | **Clase abstracta** (con plantilla) |
| Intercambiar implementaciones | **Interfaz** |
| Combinar comportamientos | **Composición** |
| Una instancia compartida | **Inyección** (`AddSingleton`), casi nunca `Instance` estático |
| Probar sin dependencias reales | **DI + falsos** |
| Decidir qué crear | **Factory** |
| Un objeto con muchas opciones | **Builder** |
| Añadir comportamiento a algo existente | **Decorator** |
| Intercambiar un algoritmo | **Strategy** |
| Avisar a quien le interese | **Observer** (eventos) |
| Acciones que se deshacen | **Command** |
| Mantener el diseño sano | **SOLID** y sentido común |

**Sigue con la Parte IV:** convertir TaskFlow en una **API web** con base de datos, seguridad y pruebas.
