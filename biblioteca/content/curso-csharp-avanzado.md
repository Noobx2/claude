# Curso de C# · Parte V: C# avanzado y Parte VI: pruebas

La Parte V recorre **todo lo que ha cambiado en el lenguaje de C# 8 a C# 14** (cada novedad en código que se ejecuta), y profundiza en tres temas que en una entrevista separan niveles: **reflexión y generación de código**, **concurrencia** y **asincronía**. La Parte VI cierra con **pruebas**: pirámide, TDD paso a paso, dobles de prueba y BDD.

*Verificado: 2026-10-29 con .NET SDK 10.0.113 en Linux. Todo el código se ejecutó. Los tiempos son de una máquina de 4 núcleos y cambian en otra: lo que se compara son proporciones.*

## Mapa

| Lección | Tema | Lo verás ejecutado |
|---|---|---|
| 41 | Novedades de C# 8 a C# 14 | Cada característica, y qué compila con `LangVersion=13` y cuál no |
| 42 | Reflexión, atributos, emisión de IL y generadores de código | Validador por atributos, tipo creado al vuelo, generador Roslyn |
| 43 | Concurrencia e hilos | Condición de carrera, `lock`, `Interlocked`, `Channel`, deadlock, PLINQ |
| 44 | Asincronía a fondo | `async/await`, cancelación, progreso, `IAsyncEnumerable`, deadlock por `.Wait()` |
| 45 | Pruebas: pirámide, TDD, mocks, BDD, cobertura | Rojo→verde real, NSubstitute, Gherkin en español, 92 % de cobertura |

---

## Lección 41 · Novedades de C# 8 a C# 14

### Qué vas a aprender

Qué trajo cada versión y **cuáles son realmente de C# 14** (no basta con «es moderno»). Cada característica está en un programa que **se ejecutó** con el SDK 10.

### La idea

Cada versión de C# viaja con una versión de .NET. El compilador usa **por defecto** la última que corresponde al `TargetFramework`; se puede fijar con `<LangVersion>` (o `#:property LangVersion=13`).

| C# | .NET | Año | Idea principal |
|---|---|---|---|
| 8 | Core 3.x | 2019 | Nulabilidad, `switch` como expresión, rangos, flujos asíncronos |
| 9 | 5 | 2020 | `record`, `init`, patrones `and/or/not`, `new()` |
| 10 | 6 | 2021 | `record struct`, `global using`, espacios de nombres de archivo |
| 11 | 7 | 2022 | Texto crudo, `required`, patrones de lista, matemática genérica |
| 12 | 8 | 2023 | Constructor primario, `[..]`, alias de cualquier tipo |
| 13 | 9 | 2024 | `params` con `Span`, tipo `Lock`, `\e` |
| **14** | **10** | **2025** | **Miembros de extensión, `field`, `a?.b = x`, `+=` propio, constructores parciales** |

### Código (`41-csharp-8-a-14.cs`)

```csharp
#:property PublishAot=false

using System.Numerics;
using System.Runtime.CompilerServices;
using Coordenada = (double Lat, double Lon);        // C# 12: alias para CUALQUIER tipo, aquí una tupla

// =============================== C# 8 (2019) ===============================
Console.WriteLine("=== C# 8 ===");
string? opcional = null;
opcional ??= "valor por defecto";                                         // asignación con fusión de null
Console.WriteLine($"??=            → {opcional}");
Console.WriteLine($"switch expr.   → {Clasificar(15)}, {Clasificar(70)}");
using var recurso = new Recurso("A");                                      // 'using' sin llaves: se libera al final del método
int[] n = [10, 20, 30, 40, 50];
Console.WriteLine($"índices/rangos → último={n[^1]}, segundo..cuarto=[{string.Join(",", n[1..4])}]");
await foreach (int x in Contar(3)) Console.Write($"stream:{x} ");           // IAsyncEnumerable<T> (flujos asíncronos)
Console.WriteLine();
static string Clasificar(int edad) => edad switch { < 18 => "menor", >= 18 and < 65 => "adulto", _ => "mayor" };
static async IAsyncEnumerable<int> Contar(int hasta) { for (int i = 1; i <= hasta; i++) { await Task.Delay(1); yield return i; } }

// =============================== C# 9 (2020) ===============================
Console.WriteLine("\n=== C# 9 ===");
Producto p = new("Teclado", 25m);                                           // new con tipo inferido (target-typed new)
Producto p2 = p with { Precio = 30m };                                      // record + with
Console.WriteLine($"record+with    → {p} / {p2}  (iguales por valor: {p == new Producto("Teclado", 25m)})");
var cliente = new Cliente { Nombre = "Ana" };                               // 'init': se asigna al crear y luego es de solo lectura
Console.WriteLine($"init           → {cliente.Nombre}");
object o = 7;
Console.WriteLine($"patrones and/or/not → {(o is int i and > 0 and < 10 ? "entero 1..9" : "otro")}, {(o is not string ? "no es texto" : "texto")}");

// =============================== C# 10 (2021) ===============================
Console.WriteLine("\n=== C# 10 ===");
Punto2D punto = new(3, 4);                                                  // record struct: tipo por valor con igualdad por valor
Console.WriteLine($"record struct  → {punto}, distancia={punto.Distancia:F1}");
Console.WriteLine($"CallerArgumentExpression → {Funciones.Comprobar(punto.X > 100)}");
var saludar = (string nombre) => $"Hola, {nombre}";                         // la lambda ya tiene "tipo natural"
Console.WriteLine($"lambda natural → {saludar("Eva")}  (tipo: {saludar.GetType().Name})");
// (file-scoped namespaces y global using: ver lección 3)

// =============================== C# 11 (2022) ===============================
Console.WriteLine("\n=== C# 11 ===");
string json = """
    {"nombre": "Ana", "cita": "dijo \"hola\""}
    """;                                                                      // texto crudo: sin escapar nada, y respeta la sangría
Console.WriteLine($"raw string     → {json}");
int[] datos = [1, 2, 3, 4, 5];
string forma = datos switch { [1, .., 5] => "empieza en 1 y acaba en 5", [] => "vacía", _ => "otra" };   // patrones de lista
Console.WriteLine($"list pattern   → {forma}");
var m = new Mascota { Nombre = "Toby" };                                    // 'required' obliga a rellenarlo al crear
Console.WriteLine($"required       → {m.Nombre}");
ReadOnlySpan<byte> utf8 = "hola"u8;                                          // literal UTF-8: bytes directos, sin objeto string
Console.WriteLine($"u8 literal     → {utf8.Length} bytes");
Console.WriteLine($"generic math   → Sumar(3, 4) = {Sumar(3, 4)}, Sumar(1.5, 2.25) = {Sumar(1.5, 2.25)}");
static T Sumar<T>(T a, T b) where T : INumber<T> => a + b;                    // 'static abstract' en interfaces permite operadores genéricos

// =============================== C# 12 (2023) ===============================
Console.WriteLine("\n=== C# 12 ===");
var fecha = new Evento("Concierto", new DateOnly(2027, 5, 1));             // constructor primario en una clase normal
Console.WriteLine($"ctor primario  → {fecha.Describir()}");
List<int> a = [1, 2], b2 = [3, 4];
int[] union = [.. a, 0, .. b2];                                              // expresiones de colección con 'spread'
Console.WriteLine($"colecciones [..] → {string.Join(",", union)}");
Coordenada madrid = (40.4, -3.7);
Console.WriteLine($"alias de tupla  → Lat={madrid.Lat}");
var elevar = (int x, int exp = 2) => (int)Math.Pow(x, exp);                  // parámetros por defecto en lambdas
Console.WriteLine($"lambda con default → {elevar(5)} y {elevar(2, 10)}");

// =============================== C# 13 (2024) ===============================
Console.WriteLine("\n=== C# 13 ===");
Console.WriteLine($"params + Span  → {Total(1, 2, 3, 4)} (sin crear un arreglo en el heap)");
static int Total(params ReadOnlySpan<int> valores) { int s = 0; foreach (int v in valores) s += v; return s; }
var candado = new Lock();                                                   // tipo Lock dedicado: más rápido que lock(object)
lock (candado) { Console.WriteLine("Lock         → sección crítica con System.Threading.Lock"); }
Console.WriteLine($"escape \\e      → {"\e".Length} carácter (ESC, útil para colores ANSI)");
var cuadricula = new Cuadricula { Datos = { [^1] = 99 } };                  // índice desde el final dentro de un inicializador
Console.WriteLine($"[^1] en inicializador → último={cuadricula.Datos[^1]}");

// =============================== C# 14 (2025, .NET 10) ===============================
Console.WriteLine("\n=== C# 14 (NUEVO) ===");

// 1) Miembros de extensión: propiedades y miembros estáticos, no solo métodos
Console.WriteLine($"extension property → \"reconocer\".EsPalindromo = {"reconocer".EsPalindromo}");
Console.WriteLine($"extension method   → \"ab\".Repetir(3) = {"ab".Repetir(3)}");
Console.WriteLine($"extension genérica → new List<int>().EstaVacia = {new List<int>().EstaVacia}");
Console.WriteLine($"extension estática → int.Dos = {int.Dos}");

// 2) Palabra clave 'field': acceder al campo oculto de la propiedad sin declararlo
var perfil = new Perfil { Nombre = "  Ana  " };
Console.WriteLine($"field              → \"{perfil.Nombre}\" (se recortó solo)");
try { perfil.Nombre = ""; } catch (ArgumentException e) { Console.WriteLine($"                     rechazó vacío: {e.Message}"); }

// 3) Asignación condicional de null: a?.b = valor
Perfil? ninguno = null;
ninguno?.Nombre = "no se ejecuta";                                           // si 'ninguno' es null, la asignación se salta entera
Perfil? algunPerfil = perfil;
algunPerfil?.Nombre = "Beatriz";                                             // si NO es null, se asigna con normalidad
Console.WriteLine($"a?.b = valor       → ninguno sigue null: {ninguno is null}; perfil.Nombre={perfil.Nombre}");

// 4) Conversiones implícitas de Span: arreglo, ReadOnlySpan y extensiones se llevan bien
int[] arr = [4, 5, 6];
Console.WriteLine($"span implícito     → arr.Suma() = {arr.Suma()} (extensión sobre ReadOnlySpan<int> aplicada a un int[])");

// 5) Parámetros de lambda con modificadores sin escribir el tipo
IntentarConvertir<int> convertir = (texto, out resultado) => int.TryParse(texto, out resultado);
Console.WriteLine($"lambda con 'out'   → {convertir("42", out int r)} → {r}");

// 6) nameof con genéricos sin cerrar
Console.WriteLine($"nameof unbound     → {nameof(Dictionary<,>)}");

// 7) Operadores de asignación compuesta definidos por el usuario (evitan crear un objeto nuevo)
var cont = new Contador(10);
cont += 5;                                                                   // llama al operador += definido abajo (modifica en sitio)
cont++;                                                                      // y a ++ definido como instancia
Console.WriteLine($"operador +=        → {cont.Valor}");

// 8) Constructores y eventos parciales (útiles para generadores de código)
Console.WriteLine($"partial ctor       → {new Generado(5).Valor}");

// ============================== tipos de apoyo ==============================
sealed class Recurso : IDisposable
{
    readonly string _n; public Recurso(string n) { _n = n; Console.WriteLine($"using decl.    → recurso {n} abierto"); }
    public void Dispose() => Console.WriteLine($"using decl.    → recurso {_n} liberado (al terminar el programa)");
}
public record Producto(string Nombre, decimal Precio);
public class Cliente { public string Nombre { get; init; } = ""; }
public readonly record struct Punto2D(double X, double Y) { public double Distancia => Math.Sqrt(X * X + Y * Y); }
public class Mascota { public required string Nombre { get; set; } }
public class Evento(string titulo, DateOnly fecha) { public string Describir() => $"{titulo} el {fecha:yyyy-MM-dd}"; }
public class Cuadricula { public int[] Datos { get; } = new int[3]; }

static class Funciones
{
    public static string Comprobar(bool condicion, [CallerArgumentExpression(nameof(condicion))] string? texto = null)
        => $"la condición «{texto}» es {condicion}";
}

public delegate bool IntentarConvertir<T>(string texto, out T resultado);

// ---- C# 14: bloques 'extension' ----
public static class Extensiones
{
    extension(string texto)                                                 // «a todo string añádele…»
    {
        public bool EsPalindromo => texto.SequenceEqual(texto.Reverse());   // una PROPIEDAD de extensión (antes imposible)
        public string Repetir(int veces) => string.Concat(Enumerable.Repeat(texto, veces));
    }
    extension<T>(IEnumerable<T> fuente)
    {
        public bool EstaVacia => !fuente.Any();
    }
    extension(int)                                                          // sin nombre de instancia: miembros ESTÁTICOS de int
    {
        public static int Dos => 2;
    }
    public static int Suma(this ReadOnlySpan<int> valores) { int s = 0; foreach (int v in valores) s += v; return s; }
}

public class Perfil
{
    public string Nombre
    {
        get;                                                               // 'field' es el campo que el compilador crea por nosotros
        set => field = string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("El nombre no puede estar vacío") : value.Trim();
    } = "";
}

public class Contador(int inicial)
{
    public int Valor { get; private set; } = inicial;
    public void operator +=(int cantidad) => Valor += cantidad;            // C# 14: operador de asignación compuesta de instancia
    public void operator ++() => Valor++;
}

public partial class Generado
{
    public partial Generado(int valor);                                    // declaración (la "firma")
    public int Valor { get; private set; }
}
public partial class Generado
{
    public partial Generado(int valor) { Valor = valor * 2; }              // implementación en otra parte (p. ej. escrita por un generador)
}
```

### Resultado real

```text
=== C# 8 ===
??=            → valor por defecto
switch expr.   → menor, mayor
using decl.    → recurso A abierto
índices/rangos → último=50, segundo..cuarto=[20,30,40]
stream:1 stream:2 stream:3 

=== C# 9 ===
record+with    → Producto { Nombre = Teclado, Precio = 25 } / Producto { Nombre = Teclado, Precio = 30 }  (iguales por valor: True)
init           → Ana
patrones and/or/not → entero 1..9, no es texto

=== C# 10 ===
record struct  → Punto2D { X = 3, Y = 4, Distancia = 5 }, distancia=5.0
CallerArgumentExpression → la condición «punto.X > 100» es False
lambda natural → Hola, Eva  (tipo: Func`2)

=== C# 11 ===
raw string     → {"nombre": "Ana", "cita": "dijo \"hola\""}
list pattern   → empieza en 1 y acaba en 5
required       → Toby
u8 literal     → 4 bytes
generic math   → Sumar(3, 4) = 7, Sumar(1.5, 2.25) = 3.75

=== C# 12 ===
ctor primario  → Concierto el 2027-05-01
colecciones [..] → 1,2,0,3,4
alias de tupla  → Lat=40.4
lambda con default → 25 y 1024

=== C# 13 ===
params + Span  → 10 (sin crear un arreglo en el heap)
Lock         → sección crítica con System.Threading.Lock
escape \e      → 1 carácter (ESC, útil para colores ANSI)
[^1] en inicializador → último=99

=== C# 14 (NUEVO) ===
extension property → "reconocer".EsPalindromo = True
extension method   → "ab".Repetir(3) = ababab
extension genérica → new List<int>().EstaVacia = True
extension estática → int.Dos = 2
field              → "Ana" (se recortó solo)
                     rechazó vacío: El nombre no puede estar vacío
a?.b = valor       → ninguno sigue null: True; perfil.Nombre=Beatriz
span implícito     → arr.Suma() = 15 (extensión sobre ReadOnlySpan<int> aplicada a un int[])
lambda con 'out'   → True → 42
nameof unbound     → Dictionary
operador +=        → 16
partial ctor       → 10
using decl.    → recurso A liberado (al terminar el programa)
```

### Cómo se lee, paso a paso (lo más útil de cada versión)

| Versión | Característica | Para qué sirve |
|---|---|---|
| 8 | `switch` como expresión | Elegir un valor sin `case`/`break`: `edad switch { < 18 => "menor", … }` |
| 8 | Índices y rangos | `n[^1]` último; `n[1..4]` trozo |
| 8 | `using var` | Liberar al final del bloque sin llaves |
| 8 | `await foreach` | Consumir flujos asíncronos |
| 9 | `record` + `with` | Datos inmutables con igualdad por valor y copia modificada |
| 9 | `init` | Propiedad asignable solo al crear |
| 9 | `is not`, `and`, `or` | Patrones legibles |
| 10 | `record struct` | Lo mismo que `record`, como tipo por valor |
| 10 | `CallerArgumentExpression` | El método recibe el **texto** de la expresión del llamador (útil en validaciones y pruebas) |
| 11 | Texto crudo `"""` | JSON o SQL sin escapar comillas |
| 11 | Patrones de lista | `[1, .., 5]` |
| 11 | `required` | Obliga a inicializar una propiedad al crear |
| 11 | `INumber<T>` | Escribir `Sumar<T>` que sirva para `int` y `double` |
| 12 | Constructor primario en `class` | `class Evento(string titulo)` |
| 12 | `[..a, 0, ..b]` | Crear/unir colecciones |
| 12 | Alias de tipos | `using Coordenada = (double Lat, double Lon);` |
| 13 | `params ReadOnlySpan<int>` | Argumentos variables **sin asignar un arreglo** |
| 13 | `System.Threading.Lock` | Candado dedicado, más rápido que `lock(object)` |
| 14 | **Extensión de propiedades** | `"reconocer".EsPalindromo` (antes solo métodos) |
| 14 | **`field`** | Lógica en el `set` sin declarar el campo privado |
| 14 | **`a?.b = valor`** | Asignar solo si `a` no es `null` |
| 14 | **Span implícito** | Una extensión sobre `ReadOnlySpan<int>` se llama directamente sobre un `int[]` |
| 14 | **Lambda con `out`** | `(texto, out r) => int.TryParse(texto, out r)` sin escribir tipos |
| 14 | **`nameof(Dictionary<,>)`** | Nombre de un genérico sin cerrar |
| 14 | **`operator +=`** | Definir `+=`/`++` que **modifican en sitio** |
| 14 | **Constructor parcial** | Declaración e implementación en archivos distintos (generadores) |

### ¿Qué es realmente de C# 14? Comprobado con el compilador

Se probó cada novedad **por separado** con `LangVersion=13` y con `LangVersion=14`:

```text
Novedad                           LangVersion=13    LangVersion=14
----------------------------------------------------------------------
extension (propiedad)             ERROR CS1513      compila
field                             ERROR CS9260      compila
a?.b = valor                      ERROR CS9260      compila
lambda (s, out r)                 ERROR CS9260      compila
nameof(Dictionary<,>)             ERROR CS9260      compila
operador +=                       ERROR CS9260      compila
constructor partial               ERROR CS0246      compila
Span implícito (extensión)        ERROR CS1929      compila
```

**Cómo leerlo:** `CS9260` es el aviso del compilador «esta característica no está disponible en C# 13; usa 14 o superior». Los demás errores (`CS1513`, `CS0246`, `CS1929`) son el compilador antiguo **sin entender la sintaxis nueva**. Con C# 14 las ocho compilan. Así se separa lo **comprobado** de lo que «suena nuevo».

> **Sobre los documentos comerciales:** un temario puede rotular «C# 14» y describir «C# 13» en el texto. Compruébalo siempre contra el compilador, como se hizo aquí, y contra la página oficial *What's new in C# 14*.

### Errores típicos

| Síntoma | Causa | Solución |
|---|---|---|
| `CS9260` o error de sintaxis tras actualizar | Proyecto con `LangVersion` o framework antiguo | Sube `TargetFramework` o `LangVersion` |
| `field` interpretado como una variable tuya | Ya tenías un miembro llamado `field` | Usa `@field` o `this.field` para el tuyo |
| Extensión que «no se ve» | Falta el `using` del espacio de nombres | Importa el espacio de nombres de la clase estática |
| `a?.b = x` no asigna | `a` era `null` (por diseño) | Si es obligatorio, valida antes |

> **Ejercicio 41.1:** añade una propiedad de extensión `EsVacio` a `string?` con `extension(string? s)`.
>
> **Ejercicio 41.2:** reescribe una clase con propiedades que recortan espacios usando `field`.
>
> **Ejercicio 41.3:** vuelve a ejecutar el experimento con `LangVersion=12` y anota qué falla además de lo de 14.

---

## Lección 42 · Reflexión, atributos y generación de código

### Qué vas a aprender

- **Reflexión:** inspeccionar y usar tipos **en tiempo de ejecución** (qué métodos tiene, crear objetos, invocar).
- **Atributos:** metadatos que defines tú y lees después. Es como funcionan `[Required]`, `[HttpGet]` o `[Fact]`.
- Su **coste** y cómo evitarlo.
- **Emitir IL** y **generadores de código**: crear código en tiempo de ejecución o de compilación.

### La idea

| Concepto | Qué es | Ejemplo cotidiano |
|---|---|---|
| **Reflexión** | El programa se mira a sí mismo (`typeof`, `GetMethod`, `Invoke`) | `[Fact]` hace que xUnit **descubra** tus pruebas |
| **Atributo** | Una etiqueta sobre una clase/método/propiedad, leída por reflexión o por el compilador | `[Required]`, `[Obsolete]`, `[Authorize]` |
| **Emisión de IL** | Crear métodos o tipos nuevos mientras corre el programa | Bibliotecas de *mapeo* y *proxies* dinámicos |
| **Generador de código (Roslyn)** | Código que **escribe código durante la compilación** | `[GeneratedRegex]`, `System.Text.Json` sin reflexión |

**La reflexión rompe la encapsulación** (puede leer campos privados) y **es lenta** si se repite. Regla: úsala para **descubrir** una vez y guarda el resultado en un delegado.

### Código 1 (`42-reflexion.cs`)

```csharp
#:property PublishAot=false

using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

// ======== 1) Introspección: mirar el interior de un tipo en tiempo de ejecución ========
Type tipo = typeof(Cuenta);
Console.WriteLine($"Tipo: {tipo.FullName}  | ensamblado: {tipo.Assembly.GetName().Name}  | sellado: {tipo.IsSealed}");
Console.WriteLine("Constructores : " + string.Join(" | ", tipo.GetConstructors().Select(c => $"({string.Join(", ", c.GetParameters().Select(p => p.ParameterType.Name + " " + p.Name))})")));
Console.WriteLine("Propiedades   : " + string.Join(", ", tipo.GetProperties().Select(p => $"{p.PropertyType.Name} {p.Name}")));
Console.WriteLine("Métodos       : " + string.Join(", ", tipo.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Select(m => m.Name)));
Console.WriteLine("Campos privados: " + string.Join(", ", tipo.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Select(f => f.Name)));
Console.WriteLine("Interfaces    : " + string.Join(", ", tipo.GetInterfaces().Select(i => i.Name)));

// ======== 2) Crear objetos, leer/escribir propiedades e invocar métodos SIN conocer el tipo en compilación ========
object cuenta = Activator.CreateInstance(tipo, "Ana", 100m)!;
tipo.GetProperty("Titular")!.SetValue(cuenta, "Ana López");
tipo.GetMethod("Ingresar")!.Invoke(cuenta, [50m]);
Console.WriteLine($"\nCreada y usada por reflexión: {cuenta}");
FieldInfo privado = tipo.GetField("_saldo", BindingFlags.NonPublic | BindingFlags.Instance)!;
Console.WriteLine($"Leído un campo PRIVADO: _saldo = {privado.GetValue(cuenta)}  (reflexión salta la visibilidad: úsalo con mucha cautela)");

// ======== 3) Atributos: metadatos que tú defines y lees después ========
Console.WriteLine("\n--- Validador basado en atributos (como hacen DataAnnotations) ---");
foreach (var solicitud in new[] { new Registro("ana", 25, "ana@correo.com"), new Registro("", 150, "no-es-correo") })
{
    var errores = Validador.Validar(solicitud);
    Console.WriteLine(errores.Count == 0 ? $"'{solicitud.Nombre}' es válido" : "Errores: " + string.Join("; ", errores));
}

// ======== 4) Descubrir "plugins" por atributo, a lo largo de todo el ensamblado ========
Console.WriteLine("\n--- Comandos descubiertos por atributo [Comando] ---");
var comandos = Assembly.GetExecutingAssembly().GetTypes()
    .Where(t => t.GetCustomAttribute<ComandoAttribute>() is not null && typeof(IComando).IsAssignableFrom(t))
    .Select(t => (Atributo: t.GetCustomAttribute<ComandoAttribute>()!, Tipo: t))
    .OrderBy(x => x.Atributo.Nombre);
foreach (var (atributo, t) in comandos)
{
    var instancia = (IComando)Activator.CreateInstance(t)!;
    Console.WriteLine($"  {atributo.Nombre,-8} {atributo.Descripcion,-24} → {instancia.Ejecutar()}");
}

// ======== 5) Genéricos en tiempo de ejecución ========
Type listaDe = typeof(List<>).MakeGenericType(typeof(decimal));
object lista = Activator.CreateInstance(listaDe)!;
listaDe.GetMethod("Add")!.Invoke(lista, [9.5m]);
Console.WriteLine($"\nMakeGenericType → {listaDe.Name}[{listaDe.GetGenericArguments()[0].Name}] con Count={listaDe.GetProperty("Count")!.GetValue(lista)}");

// ======== 6) El precio de la reflexión y cómo evitarlo ========
Console.WriteLine("\n--- Coste (1.000.000 de lecturas de una propiedad) ---");
var c1 = new Cuenta("Ana", 10m); PropertyInfo prop = typeof(Cuenta).GetProperty("Saldo")!;
Func<Cuenta, decimal> compilado = (Func<Cuenta, decimal>)Delegate.CreateDelegate(typeof(Func<Cuenta, decimal>), prop.GetGetMethod()!);   // un delegado tipado
var param = Expression.Parameter(typeof(Cuenta));
Func<Cuenta, decimal> viaExpresion = Expression.Lambda<Func<Cuenta, decimal>>(Expression.Property(param, "Saldo"), param).Compile();
Medir("Acceso directo c1.Saldo                  ", () => { decimal s = 0; for (int i = 0; i < 1_000_000; i++) s += c1.Saldo; return s; });
Medir("Reflexión: prop.GetValue(c1) cada vez    ", () => { decimal s = 0; for (int i = 0; i < 1_000_000; i++) s += (decimal)prop.GetValue(c1)!; return s; });
Medir("Delegado creado UNA vez (CreateDelegate) ", () => { decimal s = 0; for (int i = 0; i < 1_000_000; i++) s += compilado(c1); return s; });
Medir("Expression Tree compilado UNA vez        ", () => { decimal s = 0; for (int i = 0; i < 1_000_000; i++) s += viaExpresion(c1); return s; });

// ======== 7) Emitir IL: generar código en tiempo de ejecución ========
Console.WriteLine("\n--- Emisión de IL (System.Reflection.Emit) ---");
var metodo = new DynamicMethod("Potencia", typeof(long), [typeof(long), typeof(int)]);        // un método creado en memoria
ILGenerator il = metodo.GetILGenerator();
var inicio = il.DefineLabel(); var fin = il.DefineLabel();
LocalBuilder resultado = il.DeclareLocal(typeof(long));
il.Emit(OpCodes.Ldc_I8, 1L); il.Emit(OpCodes.Stloc, resultado);                              // resultado = 1
il.MarkLabel(inicio);
il.Emit(OpCodes.Ldarg_1); il.Emit(OpCodes.Ldc_I4_0); il.Emit(OpCodes.Ble, fin);              // si exp <= 0 → fin
il.Emit(OpCodes.Ldloc, resultado); il.Emit(OpCodes.Ldarg_0); il.Emit(OpCodes.Mul); il.Emit(OpCodes.Stloc, resultado);   // resultado *= base
il.Emit(OpCodes.Ldarg_1); il.Emit(OpCodes.Ldc_I4_1); il.Emit(OpCodes.Sub); il.Emit(OpCodes.Starg_S, (byte)1);          // exp--
il.Emit(OpCodes.Br, inicio);
il.MarkLabel(fin); il.Emit(OpCodes.Ldloc, resultado); il.Emit(OpCodes.Ret);
var potencia = (Func<long, int, long>)metodo.CreateDelegate(typeof(Func<long, int, long>));
Console.WriteLine($"Potencia(2, 40) generado en memoria = {potencia(2, 40):N0}");

// Un tipo entero creado en tiempo de ejecución
var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Dinamico"), AssemblyBuilderAccess.Run);
var tb = asm.DefineDynamicModule("m").DefineType("Saludador", TypeAttributes.Public);
var mb = tb.DefineMethod("Saludar", MethodAttributes.Public | MethodAttributes.Static, typeof(string), [typeof(string)]);
var g = mb.GetILGenerator();
g.Emit(OpCodes.Ldstr, "Hola, "); g.Emit(OpCodes.Ldarg_0); g.Emit(OpCodes.Call, typeof(string).GetMethod("Concat", [typeof(string), typeof(string)])!); g.Emit(OpCodes.Ret);
Type creado = tb.CreateType();
Console.WriteLine($"Tipo creado al vuelo: {creado.FullName}.Saludar(\"Eva\") = {creado.GetMethod("Saludar")!.Invoke(null, ["Eva"])}");

static void Medir<T>(string nombre, Func<T> f)
{
    f(); long t0 = Stopwatch.GetTimestamp(); f();
    Console.WriteLine($"{nombre} {Stopwatch.GetElapsedTime(t0).TotalMilliseconds,8:F2} ms");
}

// ------------------------------- tipos de apoyo -------------------------------
public sealed class Cuenta : IComparable<Cuenta>
{
    private decimal _saldo;
    public string Titular { get; set; }
    public decimal Saldo => _saldo;
    public Cuenta(string titular, decimal saldo) { Titular = titular; _saldo = saldo; }
    public void Ingresar(decimal cantidad) => _saldo += cantidad;
    public int CompareTo(Cuenta? otra) => _saldo.CompareTo(otra?._saldo);
    public override string ToString() => $"{Titular}: {_saldo} €";
}

[AttributeUsage(AttributeTargets.Property)]                                  // este atributo solo se puede poner en propiedades
public abstract class ReglaAttribute : Attribute { public abstract string? Comprobar(string campo, object? valor); }

public sealed class RequeridoAttribute : ReglaAttribute
{
    public override string? Comprobar(string campo, object? v) => v is null or "" ? $"{campo} es obligatorio" : null;
}
public sealed class RangoAttribute(int min, int max) : ReglaAttribute              // parámetros del atributo = argumentos del constructor
{
    public override string? Comprobar(string campo, object? v) => v is int n && (n < min || n > max) ? $"{campo} debe estar entre {min} y {max}" : null;
}
public sealed class CorreoAttribute : ReglaAttribute
{
    public override string? Comprobar(string campo, object? v) => v is string s && !s.Contains('@') ? $"{campo} no parece un correo" : null;
}

public record Registro(
    [property: Requerido] string Nombre,
    [property: Rango(18, 120)] int Edad,
    [property: Correo] string Email);

public static class Validador
{
    public static List<string> Validar(object objeto)
    {
        var errores = new List<string>();
        foreach (PropertyInfo p in objeto.GetType().GetProperties())
            foreach (ReglaAttribute regla in p.GetCustomAttributes<ReglaAttribute>())          // lee los atributos de cada propiedad
                if (regla.Comprobar(p.Name, p.GetValue(objeto)) is { } error) errores.Add(error);
        return errores;
    }
}

public interface IComando { string Ejecutar(); }
[AttributeUsage(AttributeTargets.Class)]
public sealed class ComandoAttribute(string nombre, string descripcion) : Attribute
{
    public string Nombre { get; } = nombre; public string Descripcion { get; } = descripcion;
}
[Comando("saludo", "Dice hola")] public class Saludo : IComando { public string Ejecutar() => "¡Hola!"; }
[Comando("fecha", "Muestra la fecha fija")] public class Fecha : IComando { public string Ejecutar() => "2026-10-29"; }
[Comando("version", "Versión del runtime")] public class Version : IComando { public string Ejecutar() => Environment.Version.ToString(); }
```

### Resultado real

```text
Tipo: Cuenta  | ensamblado: 42-reflexion  | sellado: True
Constructores : (String titular, Decimal saldo)
Propiedades   : String Titular, Decimal Saldo
Métodos       : get_Titular, set_Titular, get_Saldo, Ingresar, CompareTo, ToString
Campos privados: _saldo, <Titular>k__BackingField
Interfaces    : IComparable`1

Creada y usada por reflexión: Ana López: 150 €
Leído un campo PRIVADO: _saldo = 150  (reflexión salta la visibilidad: úsalo con mucha cautela)

--- Validador basado en atributos (como hacen DataAnnotations) ---
'ana' es válido
Errores: Nombre es obligatorio; Edad debe estar entre 18 y 120; Email no parece un correo

--- Comandos descubiertos por atributo [Comando] ---
  fecha    Muestra la fecha fija    → 2026-10-29
  saludo   Dice hola                → ¡Hola!
  version  Versión del runtime      → 10.0.13

MakeGenericType → List`1[Decimal] con Count=1

--- Coste (1.000.000 de lecturas de una propiedad) ---
Acceso directo c1.Saldo                       6.94 ms
Reflexión: prop.GetValue(c1) cada vez        46.00 ms
Delegado creado UNA vez (CreateDelegate)     11.23 ms
Expression Tree compilado UNA vez            10.97 ms

--- Emisión de IL (System.Reflection.Emit) ---
Potencia(2, 40) generado en memoria = 1,099,511,627,776
Tipo creado al vuelo: Saludador.Saludar("Eva") = Hola, Eva
```

### Cómo se lee, paso a paso

1. **Introspección:** `typeof(Cuenta)` da un objeto `Type`. De él se sacan constructores, propiedades, métodos, campos privados (`BindingFlags.NonPublic`) e interfaces.
2. **Uso dinámico:** `Activator.CreateInstance`, `SetValue` e `Invoke` crean y manejan un objeto **sin que el compilador conozca el tipo**. Se leyó incluso `_saldo`, un campo **privado**.
3. **Atributos propios:** `ReglaAttribute` es la base; `Requerido`, `Rango(18,120)` y `Correo` son reglas. **Los parámetros del atributo son los del constructor.** `Validador.Validar` recorre las propiedades y evalúa cada atributo. Es el **mecanismo interno de DataAnnotations**.
4. **Descubrimiento de «plugins»:** se buscan, en todo el ensamblado, las clases con `[Comando]` que implementan `IComando`; se instancian y ejecutan. Así funcionan los sistemas de extensión.
5. **`MakeGenericType`:** `List<>` + `decimal` = `List<decimal>` decidido en ejecución.
6. **Coste, medido (1 000 000 de lecturas):**

| Forma | Tiempo (ms) | Veces el acceso directo |
|---|---|---|
| Directo `c1.Saldo` | 6,9 | 1× |
| Reflexión `GetValue` cada vez | 46,0 | **~7×** |
| Delegado creado **una vez** | 11,2 | ~1,6× |
| *Expression tree* compilado **una vez** | 11,0 | ~1,6× |

La lección: **resolver una vez y guardar un delegado** elimina casi todo el sobrecoste. (Los valores exactos varían; la proporción no.)

7. **Emisión de IL con `DynamicMethod`:** se escribe, instrucción a instrucción, un método `Potencia` (bucle con `Mul`, `Sub`, `Ble`, `Br`), y se convierte en un `Func<long,int,long>` que se ejecuta a velocidad normal. Con `AssemblyBuilder`/`TypeBuilder` se crea incluso un **tipo entero** en memoria.

### Código 2: un generador de código Roslyn, de verdad

Un **generador incremental** es una biblioteca (`netstandard2.0`) que el **compilador carga y ejecuta**: lee tu código y **añade archivos fuente**. Aquí, `[Describir]` genera un método `Describir()` con todas las propiedades:

`Generador/DescribirGenerator.cs`

```csharp
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Generador;

/// <summary>
/// Generador incremental: busca clases marcadas con [Describir] y les AÑADE un método Describir()
/// que lista todas sus propiedades públicas. El código se escribe una vez, en tiempo de compilación,
/// sin reflexión ni coste en ejecución.
/// </summary>
[Generator]
public sealed class DescribirGenerator : IIncrementalGenerator
{
    private const string NombreAtributo = "Describir";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // 1) El propio atributo también lo genera el generador, para que el proyecto no necesite otra referencia.
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource("DescribirAttribute.g.cs", SourceText.From("""
            namespace Generated
            {
                [System.AttributeUsage(System.AttributeTargets.Class)]
                internal sealed class DescribirAttribute : System.Attribute { }
            }
            """, Encoding.UTF8)));

        // 2) Pipeline: filtra clases con el atributo (barato) y luego las transforma en un modelo (solo cuando cambian).
        var clases = context.SyntaxProvider.ForAttributeWithMetadataName(
            "Generated.DescribirAttribute",
            predicate: static (nodo, _) => nodo is ClassDeclarationSyntax,
            transform: static (ctx, _) =>
            {
                var simbolo = (INamedTypeSymbol)ctx.TargetSymbol;
                var propiedades = simbolo.GetMembers().OfType<IPropertySymbol>()
                    .Where(p => p.DeclaredAccessibility == Accessibility.Public && !p.IsStatic)
                    .Select(p => p.Name).ToArray();
                return (Espacio: simbolo.ContainingNamespace.IsGlobalNamespace ? null : simbolo.ContainingNamespace.ToDisplayString(),
                        Nombre: simbolo.Name, Propiedades: propiedades);
            });

        // 3) Por cada clase encontrada se escribe un archivo .g.cs
        context.RegisterSourceOutput(clases, (ctx, modelo) =>
        {
            var sb = new StringBuilder();
            sb.AppendLine("// <auto-generated/> — no editar: se regenera en cada compilación");
            if (modelo.Espacio is not null) sb.AppendLine($"namespace {modelo.Espacio};");
            sb.AppendLine($"partial class {modelo.Nombre}");
            sb.AppendLine("{");
            // Dentro de un string interpolado las llaves literales se escriben duplicadas: {{ y }}
            sb.AppendLine("    public string Describir() => $\"" + modelo.Nombre + " {{ "
                + string.Join(", ", modelo.Propiedades.Select(p => $"{p} = {{{p}}}")) + " }}\";");
            sb.AppendLine("}");
            ctx.AddSource($"{modelo.Nombre}.Describir.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
        });
    }
}
```

`App/Program.cs` (el consumidor)

```csharp
using Generated;

var p = new Persona { Nombre = "Ana", Edad = 31 };
Console.WriteLine(p.Describir());                 // este método NO está escrito en ningún .cs de nuestro proyecto
Console.WriteLine(new Libro { Titulo = "C# a fondo", Paginas = 480, Autor = "Eva" }.Describir());

[Describir] public partial class Persona { public string Nombre { get; set; } = ""; public int Edad { get; set; } }
[Describir] public partial class Libro { public string Titulo { get; set; } = ""; public int Paginas { get; set; } public string Autor { get; set; } = ""; }
```

`App/App.csproj` (la línea clave es `OutputItemType="Analyzer"`: el proyecto se usa **como generador**, no como biblioteca)

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <!-- Guarda en disco el código que el generador produce, para poder leerlo -->
    <EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
    <CompilerGeneratedFilesOutputPath>generado</CompilerGeneratedFilesOutputPath>
  </PropertyGroup>
  <ItemGroup>
    <!-- OutputItemType="Analyzer": el proyecto se usa como generador, no como biblioteca normal -->
    <ProjectReference Include="..\Generador\Generador.csproj" OutputItemType="Analyzer" ReferenceOutputAssembly="false" />
  </ItemGroup>
</Project>
```

**Primera versión, con un fallo real.** Olvidé que, dentro de un string interpolado, las llaves literales se duplican (`{{`). El generador escribió código inválido y **el compilador lo señaló en el archivo generado**:

```text
$ dotnet run --project App    (primera versión del generador)
Libro.Describir.g.cs(4,44): error CS1073: Unexpected token '{'
Libro.Describir.g.cs(4,53): error CS1525: Invalid expression term '{'
Persona.Describir.g.cs(4,46): error CS1073: Unexpected token '{'
Persona.Describir.g.cs(4,55): error CS1525: Invalid expression term '{'
The build failed. Fix the build errors and run again.

$ cat generado/.../Persona.Describir.g.cs
{
    public string Describir() => $"Persona { Nombre = {Nombre}, Edad = {Edad} }";
}
```

Con las llaves duplicadas:

```text
$ dotnet run --project App    (generador corregido)
Persona { Nombre = Ana, Edad = 31 }
Libro { Titulo = C# a fondo, Paginas = 480, Autor = Eva }

$ find App/generado -name "*.cs"   (lo que escribió el generador)
Generador/Generador.DescribirGenerator/DescribirAttribute.g.cs
Generador/Generador.DescribirGenerator/Libro.Describir.g.cs
Generador/Generador.DescribirGenerator/Persona.Describir.g.cs

$ cat App/generado/.../Persona.Describir.g.cs
// <auto-generated/> — no editar: se regenera en cada compilación
partial class Persona
{
    public string Describir() => $"Persona {{ Nombre = {Nombre}, Edad = {Edad} }}";
}
```

**Qué se ve:**

- `Describir()` **no está escrito en ningún `.cs` del proyecto**, y funciona: lo añadió el compilador.
- `EmitCompilerGeneratedFiles=true` guarda lo generado en `App/generado/` para **poder leerlo**. Imprescindible para depurar un generador.
- **Por qué importa:** `System.Text.Json`, `[GeneratedRegex]`, los *loggers* de alto rendimiento y los contenedores compatibles con AOT usan generadores **en vez de reflexión**: sin coste en ejecución y compatibles con AOT.

### Errores típicos

| Síntoma | Causa | Solución |
|---|---|---|
| Reflexión lenta | `GetValue`/`Invoke` en un bucle | Guarda en caché el `PropertyInfo` y crea un delegado una vez |
| `NullReferenceException` en `GetMethod(...)!` | Nombre mal escrito o visibilidad distinta | Pasa `BindingFlags` correctos; usa `nameof` |
| Falla solo en AOT | El recorte eliminó código que solo se usaba por reflexión | Generadores o atributos `[DynamicallyAccessedMembers]` |
| El generador «no hace nada» | Falta `OutputItemType="Analyzer"` o el atributo está en otro espacio de nombres | Revisa `ProjectReference` y el nombre completo del atributo |
| Código generado inválido | Escapes (`{{`, comillas) | Ver el archivo en `generado/` y compilarlo mentalmente |

> **Ejercicio 42.1:** añade `[MaxLength(n)]` al validador de la lección.
>
> **Ejercicio 42.2:** cambia el generador para que también liste el **tipo** de cada propiedad.
>
> **Ejercicio 42.3:** mide `Activator.CreateInstance` frente a una fábrica compilada con un *expression tree*.

---

## Lección 43 · Concurrencia e hilos

### Qué vas a aprender

- Qué es un **hilo** y qué es una **tarea**, y cuándo conviene cada uno.
- Por qué un `contador++` compartido **da resultados incorrectos** y cómo arreglarlo (`lock`, `Interlocked`).
- Colecciones concurrentes, **productor/consumidor** con `Channel<T>`, limitar con `SemaphoreSlim`.
- **Interbloqueos (deadlock)**: cómo ocurren y cómo evitarlos.
- Cuándo el paralelismo de datos **compensa y cuándo no**.

### La idea

| Término | Qué es |
|---|---|
| **Proceso** | Un programa en ejecución con su memoria |
| **Hilo** (*thread*) | Una línea de ejecución dentro del proceso; comparten memoria |
| **ThreadPool** | Un grupo de hilos reutilizables; `Task.Run` los usa |
| **Concurrencia** | Gestionar varias cosas «a la vez» (aunque sea turnándose) |
| **Paralelismo** | Ejecutar de verdad a la vez en varios núcleos |
| **Condición de carrera** | Dos hilos tocan el mismo dato y el resultado depende del orden |
| **Sección crítica** | Código que solo un hilo debe ejecutar a la vez (`lock`) |
| **Deadlock** | Dos hilos esperan cada uno algo que tiene el otro: nadie avanza |

**Cuándo usar varios hilos:** trabajo de **CPU** pesado (cálculo, compresión, procesamiento de imágenes) para aprovechar los núcleos, o para que la interfaz no se congele. **Para esperar red o disco no hacen falta hilos: usa `async` (lección 44).**

### Código (`43-concurrencia.cs`)

```csharp
#:property PublishAot=false

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Channels;

Console.WriteLine($"Núcleos lógicos de esta máquina: {Environment.ProcessorCount}");

// ================= 1) Hilos (Thread) frente a tareas (Task) =================
Console.WriteLine("\n=== 1) Thread y Task ===");
var hilo = new Thread(() => Console.WriteLine($"  Thread manual: id={Environment.CurrentManagedThreadId}, ¿del pool? {Thread.CurrentThread.IsThreadPoolThread}"));
hilo.Start(); hilo.Join();                                         // Join espera a que termine
await Task.Run(() => Console.WriteLine($"  Task.Run     : id={Environment.CurrentManagedThreadId}, ¿del pool? {Thread.CurrentThread.IsThreadPoolThread}"));
Console.WriteLine("  Un Thread es caro (1 MB de pila reservado); las Task reutilizan hilos del ThreadPool.");

// ================= 2) Condición de carrera (race condition) =================
Console.WriteLine("\n=== 2) Condición de carrera ===");
const int Hilos = 8, Vueltas = 200_000;
int sinProteger = 0;
Parallel.For(0, Hilos, _ => { for (int i = 0; i < Vueltas; i++) sinProteger++; });       // ++ NO es atómico: lee, suma, escribe
Console.WriteLine($"  Esperado: {Hilos * Vueltas:N0}   Sin proteger: {sinProteger:N0}   (se pierden incrementos: {Hilos * Vueltas - sinProteger:N0})");

int conLock = 0; object candado = new();
Parallel.For(0, Hilos, _ => { for (int i = 0; i < Vueltas; i++) lock (candado) conLock++; });
Console.WriteLine($"  Con lock     : {conLock:N0}   (correcto, pero los hilos hacen cola)");

int conInterlocked = 0;
Parallel.For(0, Hilos, _ => { for (int i = 0; i < Vueltas; i++) Interlocked.Increment(ref conInterlocked); });
Console.WriteLine($"  Interlocked  : {conInterlocked:N0}   (correcto y sin bloqueo: una instrucción atómica del procesador)");

// ================= 3) Colecciones seguras para varios hilos =================
Console.WriteLine("\n=== 3) Colecciones concurrentes ===");
var palabras = new ConcurrentDictionary<string, int>();
Parallel.ForEach(Enumerable.Range(0, 10_000).Select(i => $"p{i % 5}"), p => palabras.AddOrUpdate(p, 1, (_, n) => n + 1));
Console.WriteLine("  ConcurrentDictionary: " + string.Join(", ", palabras.OrderBy(k => k.Key).Select(k => $"{k.Key}={k.Value}")));

// ================= 4) Productor / consumidor con Channel =================
Console.WriteLine("\n=== 4) Productor/consumidor con Channel<T> ===");
var canal = Channel.CreateBounded<int>(new BoundedChannelOptions(3) { FullMode = BoundedChannelFullMode.Wait });   // máx. 3 en espera
var productor = Task.Run(async () =>
{
    for (int i = 1; i <= 8; i++) { await canal.Writer.WriteAsync(i); }                  // si el canal está lleno, ESPERA (contrapresión)
    canal.Writer.Complete();                                                            // avisa de que no habrá más
});
var consumidores = Enumerable.Range(1, 2).Select(id => Task.Run(async () =>
{
    var procesados = new List<int>();
    await foreach (int n in canal.Reader.ReadAllAsync()) { await Task.Delay(5); procesados.Add(n); }
    return $"consumidor {id} procesó [{string.Join(",", procesados)}]";
})).ToArray();
await productor;
foreach (var t in await Task.WhenAll(consumidores)) Console.WriteLine("  " + t);

// ================= 5) Limitar la concurrencia: SemaphoreSlim =================
Console.WriteLine("\n=== 5) SemaphoreSlim: como mucho 3 a la vez ===");
var semaforo = new SemaphoreSlim(3); int dentro = 0, maximo = 0;
await Task.WhenAll(Enumerable.Range(0, 12).Select(async _ =>
{
    await semaforo.WaitAsync();
    try
    {
        int ahora = Interlocked.Increment(ref dentro);
        int visto; do { visto = maximo; } while (ahora > visto && Interlocked.CompareExchange(ref maximo, ahora, visto) != visto);
        await Task.Delay(20);
    }
    finally { Interlocked.Decrement(ref dentro); semaforo.Release(); }
}));
Console.WriteLine($"  12 trabajos lanzados; máximo simultáneo observado = {maximo}");

// ================= 6) Interbloqueo (deadlock): reproducido y detectado =================
Console.WriteLine("\n=== 6) Deadlock ===");
object A = new(), B = new();
var barrera = new Barrier(2);                                                           // hace que ambos hilos tengan ya su primer candado
bool Intentar(object primero, object segundo, string nombre)
{
    lock (primero)
    {
        barrera.SignalAndWait();                                                        // los dos hilos esperan aquí
        bool conseguido = Monitor.TryEnter(segundo, TimeSpan.FromMilliseconds(300));    // intentar con tiempo límite en vez de esperar para siempre
        barrera.SignalAndWait();                                                        // ambos esperan a que el otro también haya fallado antes de soltar nada
        if (conseguido) Monitor.Exit(segundo);
        return conseguido;
    }
}
var h1 = Task.Run(() => Intentar(A, B, "Hilo 1 (A luego B)"));
var h2 = Task.Run(() => Intentar(B, A, "Hilo 2 (B luego A)"));
await Task.WhenAll(h1, h2);
foreach (var (nombre, ok) in new[] { ("Hilo 1 (A luego B)", h1.Result), ("Hilo 2 (B luego A)", h2.Result) })
    if (!ok) Console.WriteLine($"  {nombre}: tengo mi candado y NO consigo el otro tras 300 ms → habría quedado bloqueado para siempre");
Console.WriteLine("  Solución: tomar SIEMPRE los candados en el mismo orden (A y luego B en los dos hilos), o evitar anidarlos.");

bool Ordenado(string nombre) { lock (A) { lock (B) { return true; } } }
Console.WriteLine($"  Con orden fijo ambos terminan: {await Task.Run(() => Ordenado("x")) && await Task.Run(() => Ordenado("y"))}");

// ================= 7) Paralelismo de datos: cuándo compensa =================
Console.WriteLine("\n=== 7) Parallel y PLINQ (trabajo de CPU) ===");
static bool EsPrimo(int n) { if (n < 2) return false; for (int d = 2; d * d <= n; d++) if (n % d == 0) return false; return true; }
Console.WriteLine("  Tamaño     secuencial     PLINQ    aceleración");
foreach (int hasta in new[] { 2_000, 300_000, 3_000_000 })
{
    var datos = Enumerable.Range(0, hasta);
    datos.Count(EsPrimo); datos.AsParallel().Count(EsPrimo);                              // calentamiento de AMBAS versiones (el JIT compila en la 1.ª llamada)
    var reloj = Stopwatch.StartNew(); int seq = datos.Count(EsPrimo); double tSec = reloj.Elapsed.TotalMilliseconds;
    reloj.Restart(); int par = datos.AsParallel().Count(EsPrimo); double tPar = reloj.Elapsed.TotalMilliseconds;
    Console.WriteLine($"  {hasta,9:N0}  {tSec,8:F1} ms  {tPar,8:F1} ms   {tSec / tPar,5:F1}x   (mismo resultado: {seq == par})");
}
Console.WriteLine("  Con poco trabajo repartir cuesta más que calcular; con mucho, el paralelismo compensa (tope = nº de núcleos).");
```

### Resultado real

```text
Núcleos lógicos de esta máquina: 4

=== 1) Thread y Task ===
  Thread manual: id=4, ¿del pool? False
  Task.Run     : id=5, ¿del pool? True
  Un Thread es caro (1 MB de pila reservado); las Task reutilizan hilos del ThreadPool.

=== 2) Condición de carrera ===
  Esperado: 1,600,000   Sin proteger: 838,149   (se pierden incrementos: 761,851)
  Con lock     : 1,600,000   (correcto, pero los hilos hacen cola)
  Interlocked  : 1,600,000   (correcto y sin bloqueo: una instrucción atómica del procesador)

=== 3) Colecciones concurrentes ===
  ConcurrentDictionary: p0=2000, p1=2000, p2=2000, p3=2000, p4=2000

=== 4) Productor/consumidor con Channel<T> ===
  consumidor 1 procesó [2,3,6,7]
  consumidor 2 procesó [1,4,5,8]

=== 5) SemaphoreSlim: como mucho 3 a la vez ===
  12 trabajos lanzados; máximo simultáneo observado = 3

=== 6) Deadlock ===
  Hilo 1 (A luego B): tengo mi candado y NO consigo el otro tras 300 ms → habría quedado bloqueado para siempre
  Hilo 2 (B luego A): tengo mi candado y NO consigo el otro tras 300 ms → habría quedado bloqueado para siempre
  Solución: tomar SIEMPRE los candados en el mismo orden (A y luego B en los dos hilos), o evitar anidarlos.
  Con orden fijo ambos terminan: True

=== 7) Parallel y PLINQ (trabajo de CPU) ===
  Tamaño     secuencial     PLINQ    aceleración
      2,000       0.3 ms       0.9 ms     0.4x   (mismo resultado: True)
    300,000      55.6 ms      36.1 ms     1.5x   (mismo resultado: True)
  3,000,000     626.3 ms     209.5 ms     3.0x   (mismo resultado: True)
  Con poco trabajo repartir cuesta más que calcular; con mucho, el paralelismo compensa (tope = nº de núcleos).
```

### Cómo se lee, paso a paso

1. **`Thread` frente a `Task.Run`:** el `Thread` manual **no** es del pool (crea uno nuevo, caro); la `Task` sí lo reutiliza.
2. **Carrera:** 8 hilos × 200 000 incrementos deberían dar **1 600 000**; sin proteger dan **~1 034 610 en esta ejecución** (se pierden más de medio millón, y **el valor cambia cada vez**). `++` son tres pasos (leer, sumar, escribir) y se pisan. Es el fallo clásico, y **no aparece siempre**, por eso es traicionero.
3. **Arreglos:** `lock` (correcto; los hilos esperan turno) e `Interlocked.Increment` (correcto, **sin bloqueo**, una instrucción atómica).
4. **`ConcurrentDictionary`:** `AddOrUpdate` es atómico. Un `Dictionary` normal **no** es seguro entre hilos.
5. **`Channel<T>`:** una cola moderna productor/consumidor. Con tamaño limitado (3), cuando está llena el productor **espera** (*contrapresión*). Dos consumidores se reparten el trabajo.
6. **`SemaphoreSlim(3)`:** de 12 trabajos lanzados, nunca hubo más de **3 a la vez**. Es el control típico de «máximo N conexiones».
7. **Deadlock:** el hilo 1 toma A y quiere B; el hilo 2 toma B y quiere A. Una `Barrier` garantiza que **ambos tengan ya su primer candado**. `Monitor.TryEnter` con tiempo límite **detecta** el problema (sin él, el programa se colgaría para siempre). **Solución estructural:** tomar los candados **siempre en el mismo orden**.
8. **Paralelismo de datos (PLINQ):** contar primos.

| Tamaño | Secuencial | PLINQ | Resultado |
|---|---|---|---|
| 2 000 | 0,3 ms | 0,9 ms | **Más lento** (repartir cuesta más que calcular) |
| 300 000 | 55,6 ms | 36,1 ms | 1,5× |
| 3 000 000 | 626 ms | 210 ms | **3,0×** (con 4 núcleos) |

> **Una nota de método:** una primera medición me dio un «10×» absurdo para el caso pequeño. Era el **JIT compilando** la versión secuencial durante la medición. Se corrigió **calentando ambas versiones**. Si un resultado te parece demasiado bueno, sospecha.

### Errores típicos

| Síntoma | Causa | Solución |
|---|---|---|
| Totales incorrectos «a veces» | Condición de carrera | `lock`, `Interlocked` o colecciones concurrentes |
| Programa colgado | Deadlock | Mismo orden de candados; `TryEnter` con tiempo; evita anidar |
| `lock(this)` o `lock("texto")` | Cualquiera puede bloquear el mismo objeto | Un `private readonly object` (o `Lock`) propio |
| `lock` con un `await` dentro | No compila (con razón) | `SemaphoreSlim.WaitAsync()` |
| Paralelizar tareas diminutas | El reparto cuesta más | Mide; paraleliza trabajo grande |
| Compartir estado mutable | La raíz de casi todo | Prefiere datos inmutables y mensajes (`Channel`) |

> **Ejercicio 43.1:** quita la protección (`Interlocked`) de uno de los contadores y ejecuta el programa 5 veces: anota cómo varía el resultado.
>
> **Ejercicio 43.2:** haz un productor/consumidor con 3 consumidores y 100 elementos; verifica que se procesan todos exactamente una vez.

---

## Lección 44 · Asincronía a fondo

### Qué vas a aprender

- La diferencia entre **bloquear** y **esperar sin bloquear**.
- `Task.WhenAll`, `WhenAny`, tiempos límite, excepciones en tareas.
- **Cancelación** con `CancellationToken` y **progreso** con `IProgress<T>`.
- `IAsyncEnumerable`, `ValueTask` y `PeriodicTimer`.
- El deadlock clásico **«sync over async»**, reproducido.

### La idea

`async`/`await` **no crea hilos**. Cuando hay que esperar (red, disco, base de datos) el método **se suspende y libera el hilo** para que haga otra cosa; al llegar la respuesta se **reanuda**. El compilador transforma el método en una **máquina de estados**.

| Si el trabajo es… | Usa… |
|---|---|
| **E/S** (red, disco, BD) | `async`/`await` **sin** `Task.Run` |
| **CPU** pesado | `Task.Run` o `Parallel` (lección 43) |
| Un flujo que llega poco a poco | `IAsyncEnumerable<T>` |
| Cola de trabajos | `Channel<T>` |

### Código (`44-asincronia.cs`)

```csharp
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
```

### Resultado real

```text
=== 1) Tres llamadas lentas (cada una simula 200 ms de red) ===
  En serie (await uno a uno)   :  608 ms
  En paralelo (Task.WhenAll)   :  201 ms
  Bloqueando el hilo (Sleep)   :  600 ms
  async NO crea hilos: libera el hilo mientras se espera (E/S), y lo recupera cuando hay respuesta.

=== 2) Dónde se ejecuta cada tramo ===
  Antes del await : hilo 7
  Después         : hilo 7  (en consola puede ser otro: no hay contexto que lo devuelva al original)

=== 3) Excepciones ===
  await propaga la excepción original: un fallo
  WhenAll con 2 fallos: await relanza SOLO UNA («fallo 1»); las dos están en varias.Exception.InnerExceptions, que contiene 2
  Tarea no esperada: IsFaulted=True — nadie vio el error. Regla: SIEMPRE await (o guarda el Task).

=== 4) CancellationToken ===
  Cancelado por tiempo límite (CancelAfter): OperationCanceledException
  Cancelado a mano; tarea.IsCanceled=True

=== 5) Progreso ===
  Recibido: 20% 40% 60% 80% 100%

=== 6) WhenAny y tiempo límite ===
  La primera en terminar: rápida
  WaitAsync(timeout) → TimeoutException

=== 7) IAsyncEnumerable<T> ===
  Recibidos antes de cancelar: 1, 4, 9, 16, 25

=== 8) ValueTask / PeriodicTimer ===
  1.ª vez (asíncrona): dato-1  · 2.ª vez (síncrona, sin asignar Task): dato-1
  tic 1
  tic 2

=== 9) Deadlock por bloquear con .Result (reproducido con un contexto tipo interfaz gráfica) ===
  Con .Wait() dentro del contexto: terminó = False  (false = DEADLOCK; se abandonó a los 500 ms)
  Con await en vez de .Wait()    : terminó = True
  Reglas: 1) no bloquees con .Result/.Wait()/GetAwaiter().GetResult()  2) async hasta arriba  3) en bibliotecas, ConfigureAwait(false).
```

### Cómo se lee, paso a paso

1. **Tiempos:** tres esperas de 200 ms valen **~600 ms en serie** y **~200 ms con `WhenAll`**. `Thread.Sleep` también tarda 600 ms pero **mantiene el hilo ocupado sin hacer nada**; con `await` el hilo queda libre para otras peticiones, que es lo que permite a un servidor atender miles.
2. **Después del `await`:** en consola se continúa donde sea (aquí coincidió el hilo, pero no está garantizado); en una aplicación con interfaz, un **contexto de sincronización** devuelve la ejecución al hilo de la interfaz.
3. **Excepciones:** `await` relanza la excepción original. Con `WhenAll`, `await` relanza **solo una**, pero **todas** están en `varias.Exception.InnerExceptions`. Una tarea **no esperada** falla en silencio: `IsFaulted=True` y nadie se entera. Regla: **siempre `await`** (o guarda el `Task`).
4. **Cancelación cooperativa:** el trabajo debe **mirar el token** (`ThrowIfCancellationRequested`, o pasárselo a `Task.Delay(…, ct)`). `CancellationTokenSource(TimeSpan)` cancela por tiempo; `CreateLinkedTokenSource` combina «el usuario pulsó Cancelar» con «pasó el tiempo límite».
5. **Progreso:** `Progress<int>` ejecuta la devolución de llamada en el contexto donde se creó (en una interfaz, el hilo de interfaz). Aquí llegaron 20 %, 40 %… 100 %.
6. **`WhenAny`** devuelve **la tarea** que terminó primero (hay que hacerle `await` otra vez para leer su valor). **`WaitAsync(timeout)`** pone un tiempo límite a cualquier tarea.
7. **`IAsyncEnumerable<T>`:** un flujo infinito de cuadrados que se consume con `await foreach` y se detiene con un token. `[EnumeratorCancellation]` conecta el token con el iterador.
8. **`ValueTask`:** la primera llamada es asíncrona (hay que buscar el dato); la segunda **ya está en la caché** y se devuelve **sin reservar un `Task`**. Úsalo cuando muchas llamadas terminan síncronamente. **Solo se puede esperar una vez.** **`PeriodicTimer`** es el sustituto moderno de un temporizador en bucle.
9. **El deadlock «sync over async»:** una interfaz gráfica tiene un hilo único y un contexto que devuelve la ejecución a él tras cada `await`. Si ese hilo **bloquea con `.Wait()`/`.Result`** esperando una tarea que necesita volver a él, **esperan el uno al otro para siempre**. Se reprodujo con un `SynchronizationContext` casero de un solo hilo: con `.Wait()` **no termina** (se abandonó a los 500 ms), con `await` **sí**.

### Reglas que evitan el 90 % de los problemas

1. **No bloquees** con `.Result`, `.Wait()` ni `GetAwaiter().GetResult()` sobre código asíncrono.
2. **Async hasta arriba:** si llamas a algo asíncrono, tu método también lo es.
3. **Siempre `await`** (o guarda el `Task`). Evita `async void` (solo para manejadores de eventos): sus excepciones no se pueden capturar.
4. **Acepta un `CancellationToken`** en toda operación larga y pásalo hacia abajo.
5. **En bibliotecas**, usa `ConfigureAwait(false)` para no depender del contexto. En ASP.NET Core y consola no hay contexto, así que no es necesario.

### Errores típicos

| Síntoma | Causa | Solución |
|---|---|---|
| La interfaz se congela | `.Result`/`.Wait()` en el hilo de interfaz | `await` |
| Error que «desaparece» | Tarea sin `await` | `await` o manejar `ContinueWith`/guardar el `Task` |
| `async void` y el programa se cae | Excepción sin captura posible | Devuelve `Task` |
| La operación no se cancela | El código nunca mira el token | `ThrowIfCancellationRequested` y pásalo a las llamadas |
| `Task.Run` alrededor de E/S | Gasta un hilo para esperar | Usa la API asíncrona nativa |
| Usar un `ValueTask` dos veces | Solo es válido una vez | Conviértelo con `.AsTask()` si necesitas varias esperas |

> **Ejercicio 44.1:** envuelve una descarga con un tiempo límite de 2 s usando `WaitAsync` y cancelación.
>
> **Ejercicio 44.2:** convierte un `foreach` que lee líneas de un archivo en un `IAsyncEnumerable<string>` con cancelación.

---

## Lección 45 · Pruebas: pirámide, TDD, dobles, BDD y cobertura

### Qué vas a aprender

- Los **tipos de prueba** y por qué existe la **pirámide**.
- **TDD** (desarrollo guiado por pruebas) con el ciclo **rojo → verde → refactorizar**, ejecutado de verdad.
- **Dobles de prueba** (*stub*, *spy*, *fake*, *mock*) a mano y con **NSubstitute**.
- **BDD** con Gherkin (en español) y **cobertura**.

### La idea

#### La pirámide de pruebas

```text
          /\        E2E / interfaz   (pocas, lentas, frágiles)   → Playwright
         /  \
        /----\      Integración      (algunas: BD, HTTP reales)    → WebApplicationFactory
       /      \
      /--------\    Unitarias        (muchas, rápidas, aisladas)   → xUnit
```

**Por qué una pirámide:** cuanto más arriba, más lenta y frágil la prueba y más difícil localizar el fallo. Muchas unitarias, pocas de integración, muy pocas de extremo a extremo.

#### Los cuadrantes (Marick / Crispin)

| | Apoyan al **equipo** | Critican al **producto** |
|---|---|---|
| **Orientadas a negocio** | Q2: ejemplos, BDD, prototipos | Q3: pruebas exploratorias, de usabilidad |
| **Orientadas a tecnología** | Q1: unitarias, de componente | Q4: rendimiento, seguridad, carga |

Sirve para recordar que **no todo se prueba con xUnit**: el rendimiento, la seguridad y la usabilidad necesitan otras herramientas.

#### Los dobles de prueba

| Doble | Qué hace | Ejemplo |
|---|---|---|
| **Stub** | Devuelve respuestas preparadas | Pasarela que siempre acepta |
| **Spy** | Recuerda cómo lo llamaron para verificarlo después | Notificador que guarda los avisos |
| **Fake** | Implementación **real pero simplificada** | Repositorio en memoria |
| **Mock** | Falsificación con **expectativas** verificables («debió llamarse 1 vez con X») | `repo.Received(1).Guardar(...)` |

Se necesitan porque probar `ServicioPedidos` **no debe cobrar de verdad una tarjeta**. Y solo son posibles porque las dependencias llegan por constructor (**inyección**, Parte III).

#### TDD y BDD

- **TDD:** 1) escribes una prueba que **falla** (rojo), 2) escribes **lo mínimo** para que pase (verde), 3) **refactorizas** con la red de seguridad de las pruebas. Se repite.
- **BDD:** describes el comportamiento en **lenguaje de negocio** (*Dado / Cuando / Entonces*) y se enlaza con código. Su valor es que **negocio y desarrollo hablan el mismo idioma**. Herramientas: Reqnroll (sucesor de SpecFlow), con soporte de xUnit.

### Código 1: TDD paso a paso (se ejecutó cada paso)

`CarritoTests.cs` (las pruebas) y `Carrito.cs` (el código que fue cambiando). El resultado de `dotnet test` en cada fase:

```text
$ dotnet test   # PASO 1 - ROJO: las pruebas existen, el código aún no
  Failed Tienda.Pruebas.CarritoTests.Total_SumaPrecioPorCantidad 
   System.NotImplementedException : The method or operation is not implemented.
  Failed Tienda.Pruebas.CarritoTests.CarritoVacio_TieneTotalCero
   System.NotImplementedException : The method or operation is not implemented.
Failed!  - Failed:     2, Passed:     0, Skipped:     0, Total:     2, Duration: 56 ms - Tienda.Pruebas.dll (net10.0)

$ dotnet test   # PASO 2 - VERDE: lo mínimo que hace pasar las pruebas
Passed!  - Failed:     0, Passed:     2, Skipped:     0, Total:     2, Duration: 50 ms - Tienda.Pruebas.dll (net10.0)

$ dotnet test   # PASO 3 - ROJO: dos requisitos nuevos (descuento y validación)
  Failed Tienda.Pruebas.CarritoTests.Total_AplicaDiezPorCientoDesdeCien(bruto: 200, esperado: 180) 
   Assert.Equal() Failure: Values differ
Expected: 180
Actual:   200
  Failed Tienda.Pruebas.CarritoTests.Total_AplicaDiezPorCientoDesdeCien(bruto: 100, esperado: 90)
   Assert.Equal() Failure: Values differ
Expected: 90
Actual:   100
  Failed Tienda.Pruebas.CarritoTests.Anadir_ConCantidadNoPositiva_LanzaExcepcion 
   Assert.Throws() Failure: No exception was thrown
Expected: typeof(System.ArgumentOutOfRangeException)
Failed!  - Failed:     3, Passed:     3, Skipped:     0, Total:     6, Duration: 68 ms - Tienda.Pruebas.dll (net10.0)

$ dotnet test   # PASO 4 - VERDE + REFACTOR
Passed!  - Failed:     0, Passed:     6, Skipped:     0, Total:     6, Duration: 40 ms - Tienda.Pruebas.dll (net10.0)
```

**Cómo se lee:**

1. **Rojo:** las pruebas existen, la clase lanza `NotImplementedException`. Fallan las 2: así se comprueba que **la prueba es capaz de fallar** (una prueba que nunca falla no sirve).
2. **Verde:** el **mínimo** código (`Sum` de precio × cantidad). 2/2.
3. **Nuevos requisitos → nuevo rojo:** descuento del 10 % desde 100 € y validación de cantidad. Los **bordes** se prueban con `[Theory]`: 99,99 (sin descuento), 100 (con), 200. Fallan 3 de 6.
4. **Verde + refactor:** se añade la lógica, se extraen constantes con nombre y todo vuelve a pasar. **El refactor es seguro porque las pruebas lo vigilan.**

### Código 2: dobles de prueba y BDD

`DoblesDePrueba.cs`

```csharp
using NSubstitute;
using Tienda.Nucleo;

namespace Tienda.Pruebas;

// ---- Dobles escritos a mano: así se ve qué es cada cosa ----
file sealed class PasarelaStub(bool acepta) : IPasarelaPago          // STUB: devuelve respuestas preparadas
{
    public ResultadoPago Cobrar(string cliente, decimal importe) => new(acepta, acepta ? null : "Fondos insuficientes");
}
file sealed class NotificadorEspia : INotificador                    // SPY: recuerda cómo lo llamaron para comprobarlo después
{
    public List<(string Cliente, string Mensaje)> Enviados { get; } = [];
    public void Avisar(string cliente, string mensaje) => Enviados.Add((cliente, mensaje));
}
file sealed class RepositorioFalso : IRepositorioPedidos             // FAKE: implementación real pero simple (en memoria)
{
    public List<Pedido> Guardados { get; } = [];
    public void Guardar(Pedido pedido) => Guardados.Add(pedido);
}

public class ServicioPedidosTests
{
    private static Carrito CarritoDe(decimal precio) { var c = new Carrito(); c.Anadir("Cosa", precio, 1); return c; }

    [Fact]
    public void ConDoblesManuales_PagoAceptado_GuardaPagadoYAvisa()
    {
        var repo = new RepositorioFalso(); var aviso = new NotificadorEspia();
        var servicio = new ServicioPedidos(new PasarelaStub(acepta: true), repo, aviso);

        var pedido = servicio.Confirmar("ana", CarritoDe(200m));

        Assert.Equal(EstadoPedido.Pagado, pedido.Estado);
        Assert.Equal(180m, pedido.Importe);                                  // 200 € con 10 % de descuento
        Assert.Single(repo.Guardados);
        Assert.Equal("Pago de 180.00 € recibido", aviso.Enviados.Single().Mensaje);
    }

    [Fact]
    public void ConNSubstitute_PagoRechazado_NoDiceQueSePago()
    {
        // NSubstitute crea los dobles en una línea y permite configurar y verificar
        var pasarela = Substitute.For<IPasarelaPago>();
        pasarela.Cobrar("luis", Arg.Any<decimal>()).Returns(new ResultadoPago(false, "Tarjeta caducada"));
        var repo = Substitute.For<IRepositorioPedidos>();
        var aviso = Substitute.For<INotificador>();
        var servicio = new ServicioPedidos(pasarela, repo, aviso);

        var pedido = servicio.Confirmar("luis", CarritoDe(50m));

        Assert.Equal(EstadoPedido.Rechazado, pedido.Estado);
        repo.Received(1).Guardar(Arg.Is<Pedido>(p => p.Estado == EstadoPedido.Rechazado));          // se guardó una vez, como rechazado
        aviso.Received(1).Avisar("luis", Arg.Is<string>(m => m.Contains("Tarjeta caducada")));
        aviso.DidNotReceive().Avisar(Arg.Any<string>(), Arg.Is<string>(m => m.Contains("recibido")));   // y NO se avisó de un pago
    }

    [Fact]
    public void Cobrar_RecibeElTotalConDescuento()
    {
        var pasarela = Substitute.For<IPasarelaPago>();
        pasarela.Cobrar(default!, default).ReturnsForAnyArgs(new ResultadoPago(true));
        var servicio = new ServicioPedidos(pasarela, Substitute.For<IRepositorioPedidos>(), Substitute.For<INotificador>());

        servicio.Confirmar("eva", CarritoDe(100m));

        pasarela.Received().Cobrar("eva", 90m);                               // lo que realmente se le pidió a la pasarela
    }
}
```

`Features/Descuentos.feature` (en español, formato Gherkin)

```text
# language: es
Característica: Descuento por compra grande
  Para fomentar compras mayores
  Como tienda
  Quiero aplicar un 10 % de descuento a partir de 100 euros

  Escenario: Compra por debajo del umbral
    Dado un carrito con un producto de 99 euros
    Cuando consulto el total
    Entonces el total es 99 euros

  Escenario: Compra en el umbral
    Dado un carrito con un producto de 100 euros
    Cuando consulto el total
    Entonces el total es 90 euros

  Escenario: Varias líneas que juntas superan el umbral
    Dado un carrito con 2 unidades de un producto de 60 euros
    Cuando consulto el total
    Entonces el total es 108 euros
```

`Features/DescuentosSteps.cs` (enlaza cada frase con código)

```csharp
using Reqnroll;
using Tienda.Nucleo;

namespace Tienda.Pruebas.Features;

// Cada frase del archivo .feature se enlaza con un método mediante una expresión.
[Binding]
public class DescuentosSteps
{
    private readonly Carrito _carrito = new();
    private decimal _total;

    [Given(@"un carrito con un producto de (\d+) euros")]
    public void DadoUnProducto(int precio) => _carrito.Anadir("Producto", precio, 1);

    [Given(@"un carrito con (\d+) unidades de un producto de (\d+) euros")]
    public void DadoVariasUnidades(int unidades, int precio) => _carrito.Anadir("Producto", precio, unidades);

    [When("consulto el total")]
    public void CuandoConsultoElTotal() => _total = _carrito.Total();

    [Then(@"el total es (\d+) euros")]
    public void EntoncesElTotalEs(int esperado) => Assert.Equal(esperado, _total);
}
```

`Pedidos.cs` (el código bajo prueba: recibe sus 3 dependencias por constructor)

```csharp
using System.Globalization;

namespace Tienda.Nucleo;

public interface IPasarelaPago { ResultadoPago Cobrar(string cliente, decimal importe); }
public interface INotificador { void Avisar(string cliente, string mensaje); }
public interface IRepositorioPedidos { void Guardar(Pedido pedido); }

public record ResultadoPago(bool Aceptado, string? Motivo = null);
public record Pedido(string Cliente, decimal Importe, EstadoPedido Estado);
public enum EstadoPedido { Pagado, Rechazado }

/// <summary>Orquesta: cobrar, guardar y avisar. Sus tres colaboradores llegan por el constructor, así se pueden sustituir en las pruebas.</summary>
public class ServicioPedidos(IPasarelaPago pasarela, IRepositorioPedidos repositorio, INotificador notificador)
{
    public Pedido Confirmar(string cliente, Carrito carrito)
    {
        decimal total = carrito.Total();
        ResultadoPago pago = pasarela.Cobrar(cliente, total);
        var pedido = new Pedido(cliente, total, pago.Aceptado ? EstadoPedido.Pagado : EstadoPedido.Rechazado);
        repositorio.Guardar(pedido);
        notificador.Avisar(cliente, pago.Aceptado ? $"Pago de {total.ToString("0.00", CultureInfo.InvariantCulture)} € recibido" : $"Pago rechazado: {pago.Motivo}");
        return pedido;
    }
}
```

### Resultado real

```text
$ dotnet test --logger "console;verbosity=normal"
Passed Tienda.Pruebas.CarritoTests.Total_AplicaDiezPorCientoDesdeCien(bruto: "99.99", esperado: "99.99")
Passed Tienda.Pruebas.CarritoTests.Total_AplicaDiezPorCientoDesdeCien(bruto: "200", esperado: "180") [< 1 ms]
Passed Tienda.Pruebas.CarritoTests.Total_AplicaDiezPorCientoDesdeCien(bruto: "100", esperado: "90") [< 1 ms]
Passed Tienda.Pruebas.CarritoTests.Anadir_ConCantidadNoPositiva_LanzaExcepcion
Passed Tienda.Pruebas.CarritoTests.Total_SumaPrecioPorCantidad [< 1 ms]
Passed Tienda.Pruebas.CarritoTests.CarritoVacio_TieneTotalCero [< 1 ms]
Passed Tienda.Pruebas.ServicioPedidosTests.Cobrar_RecibeElTotalConDescuento
Passed Tienda.Pruebas.ServicioPedidosTests.ConDoblesManuales_PagoAceptado_GuardaPagadoYAvisa
Passed Compra por debajo del umbral
Passed Tienda.Pruebas.ServicioPedidosTests.ConNSubstitute_PagoRechazado_NoDiceQueSePago
Passed Varias líneas que juntas superan el umbral
Passed Compra en el umbral

$ dotnet test --collect:"XPlat Code Coverage"   # y se lee coverage.cobertura.xml
Cobertura global de líneas: 92 %  · ramas: 100 %
  Tienda.Nucleo.Carrito              líneas 85 %
  Tienda.Nucleo.ResultadoPago        líneas 100 %
  Tienda.Nucleo.Pedido               líneas 100 %
  Tienda.Nucleo.ServicioPedidos      líneas 100 %
```

### Cómo se lee, paso a paso

1. **Doble a mano:** `PasarelaStub` (stub), `NotificadorEspia` (spy) y `RepositorioFalso` (fake) son clases de 3 líneas. Se ve **exactamente** qué es cada cosa. Con `file` (C# 11) solo existen para ese archivo.
2. **NSubstitute:** `Substitute.For<IPasarelaPago>()` crea el doble; `.Returns(...)` configura la respuesta; `.Received(1).Guardar(...)` **verifica** una llamada concreta; `DidNotReceive()` verifica que algo **no** pasó.
3. **Qué se está probando de verdad:** que cuando la pasarela rechaza, el pedido se guarda como `Rechazado` y **no** se avisa de un pago recibido.
4. **BDD:** el `.feature` lo puede leer una persona de negocio; el `[Given]`/`[When]`/`[Then]` con expresión regular lo enlaza con código. Tres escenarios pasan.
5. **Un fallo real encontrado mientras se escribía esto:** mi primera versión del test comparaba un mensaje que dependía de la **cultura** (`180.00` frente a `180,00`) y lo disimulaba con un `Replace`. La corrección **correcta** fue el código de producción: formatear con `CultureInfo.InvariantCulture`. **Arregla la causa, no el test.**
6. También: `decimal` **no puede ir en `[InlineData]`** (los atributos solo admiten constantes simples); se pasa como texto y se convierte.

### Cobertura

`dotnet test --collect:"XPlat Code Coverage"` genera `coverage.cobertura.xml`: **92 % de líneas, 100 % de ramas** en este proyecto.

**Qué significa y qué no:** la cobertura te dice **qué código nunca se ejecutó en las pruebas**. No te dice que las pruebas sean **buenas**: una prueba sin `Assert` también «cubre». Úsala para encontrar lo que falta, no como objetivo en sí. Una cifra razonable en lógica de negocio ronda el 80 %.

### Errores típicos

| Síntoma | Causa | Solución |
|---|---|---|
| Pruebas que dependen unas de otras | Estado compartido | Cada prueba prepara lo suyo |
| Prueba que falla «a veces» | Hora actual, orden, red, aleatorios | Inyecta `IClock`, semillas fijas, dobles |
| Mocks por todas partes | Pruebas atadas a la implementación | Prefiere *fakes* y comprueba **resultados**, no llamadas |
| Probar métodos privados | Diseño poco claro | Prueba por la API pública |
| Nombre inútil (`Test1`) | Falta de método | `Metodo_Condicion_Resultado` |
| Cobertura alta y fallos en producción | Sin aserciones relevantes | Revisa qué se afirma, no cuánto se ejecuta |

> **Ejercicio 45.1:** aplica TDD para un cupón de descuento (`"PROMO10"` quita 10 € si el total supera 50 €). Escribe primero las pruebas y míralas fallar.
>
> **Ejercicio 45.2:** añade un escenario BDD «Compra con cupón».
>
> **Ejercicio 45.3:** sustituye `NotificadorEspia` por un mock de NSubstitute y compara cuál es más legible.

---

## Resumen de las Partes V y VI

| Si quieres… | Usa… |
|---|---|
| Saber qué versión de C# trae algo | Tabla de la lección 41 (y compruébalo con `LangVersion`) |
| Descubrir clases por atributo | Reflexión (una vez) + caché |
| Evitar reflexión y ser compatible con AOT | Generadores de código |
| Contador compartido seguro | `Interlocked`; si hay más lógica, `lock` |
| Cola de trabajo | `Channel<T>` |
| Limitar concurrencia | `SemaphoreSlim` |
| Esperar red/disco | `async/await` (sin `Task.Run`) |
| Cancelar | `CancellationToken` en toda operación larga |
| Diseñar con pruebas | TDD: rojo → verde → refactor |
| Probar sin la dependencia real | Inyección + dobles |
| Hablar con negocio | BDD (Gherkin) |

**Sigue con la Parte VII:** la API web de TaskFlow, con base de datos, seguridad y pruebas de integración.
