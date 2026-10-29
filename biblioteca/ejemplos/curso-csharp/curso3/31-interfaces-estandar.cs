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
