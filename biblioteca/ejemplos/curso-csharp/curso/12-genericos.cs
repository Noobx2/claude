// Lección 12: genéricos, IEnumerable<T> y yield
// ---- Clase genérica: T es un "hueco" que se rellena al usarla ----
var numbers = new Stack<int>();
numbers.Push(1); numbers.Push(2);
var texts = new Stack<string>();
texts.Push("a"); texts.Push("b");
Console.WriteLine($"{numbers.Pop()} {texts.Pop()}   (la misma clase para dos tipos, con comprobación de tipos)");

// ---- Método genérico ----
Console.WriteLine(Largest(3, 9, 4));
Console.WriteLine(Largest("pera", "manzana", "uva"));
Console.WriteLine(string.Join(",", Swap(1, 2)));

// ---- Restricciones: qué debe cumplir T ----
Console.WriteLine(CreateDefault<List<int>>().Count);
var pair = new Pair<string, int>("edad", 31);
Console.WriteLine($"{pair.Key}={pair.Value}");

// ---- Result<T>: éxito o error sin excepciones ----
var ok = Result<int>.Success(42);
var fail = Result<int>.Failure("no es un número");
Console.WriteLine($"{ok} | {fail}");

// ---- yield return: generar una secuencia perezosa ----
foreach (var n in Evens(10)) Console.Write($"{n} ");
Console.WriteLine();
Console.WriteLine(string.Join(",", Fibonacci().Take(10)));   // la secuencia es infinita; Take(10) la corta

// ---- IEnumerable<T>: lo único que necesita foreach y LINQ ----
var countdown = new Countdown(3);
foreach (var n in countdown) Console.Write($"{n}... ");
Console.WriteLine("¡ya!");
Console.WriteLine(countdown.Sum());                           // LINQ funciona sobre cualquier IEnumerable<T>

static T Largest<T>(params T[] values) where T : IComparable<T>      // where: restricción
{
    T best = values[0];
    foreach (var v in values) if (v.CompareTo(best) > 0) best = v;
    return best;
}

static (T, T) Swap<T>(T a, T b) => (b, a);

static T CreateDefault<T>() where T : new() => new T();             // T debe tener constructor sin parámetros

static IEnumerable<int> Evens(int limit)
{
    for (int i = 0; i <= limit; i += 2) yield return i;             // devuelve uno, se queda "pausado", y sigue
}

static IEnumerable<long> Fibonacci()
{
    long a = 0, b = 1;
    while (true) { yield return a; (a, b) = (b, a + b); }
}

record Pair<TKey, TValue>(TKey Key, TValue Value);

readonly struct Result<T>
{
    public T? Value { get; }
    public string? Error { get; }
    public bool IsSuccess => Error is null;
    private Result(T? value, string? error) { Value = value; Error = error; }
    public static Result<T> Success(T value) => new(value, null);
    public static Result<T> Failure(string error) => new(default, error);
    public override string ToString() => IsSuccess ? $"Éxito({Value})" : $"Error({Error})";
}

class Countdown(int from) : IEnumerable<int>
{
    public IEnumerator<int> GetEnumerator() { for (int i = from; i > 0; i--) yield return i; }
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
}
