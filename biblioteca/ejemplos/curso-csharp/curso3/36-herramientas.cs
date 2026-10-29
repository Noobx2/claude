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
