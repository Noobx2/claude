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
