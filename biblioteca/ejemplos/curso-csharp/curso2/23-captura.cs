// Lección 23: capturar datos por consola (todas las formas)
// Ejecutar interactivo:      dotnet run 23-captura.cs
// Ejecutar con entrada fija: dotnet run 23-captura.cs < 23-captura.in     (así la salida es siempre la misma)
using System.Globalization;

// ──────────────────────────────────────────────────────────────────────────────
// PIEZA BASE: leer una línea.
// Console.ReadLine() devuelve null cuando no hay más entrada (Ctrl+D / fin del archivo redirigido).
// Si la entrada viene de un archivo, el teclado no la "muestra": la repetimos nosotros para que
// el resultado de esta lección se lea como una conversación.
// ──────────────────────────────────────────────────────────────────────────────
static string? ReadLine()
{
    string? line = Console.ReadLine();
    if (Console.IsInputRedirected && line is not null) Console.WriteLine(line);   // eco solo con entrada simulada
    return line;
}

// FORMA 1: texto libre. Trim() quita espacios sobrantes; ?? "" evita trabajar con null.
Console.Write("1) Tu nombre: ");
string name = (ReadLine() ?? "").Trim();
Console.WriteLine($"   -> '{name}' ({name.Length} letras)");

// FORMA 2: texto obligatorio (repite hasta que no esté vacío)
static string AskRequired(string question)
{
    while (true)
    {
        Console.Write(question);
        string text = (ReadLine() ?? "").Trim();
        if (text.Length > 0) return text;
        Console.WriteLine("   Es obligatorio. Inténtalo otra vez.");
    }
}
string city = AskRequired("2) Ciudad (obligatoria): ");
Console.WriteLine($"   -> {city}");

// FORMA 3: número entero con rango (TryParse no lanza excepción si el texto no es un número)
static int AskInt(string question, int min, int max)
{
    while (true)
    {
        Console.Write(question);
        string? text = ReadLine();
        if (!int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
            Console.WriteLine("   Eso no es un número entero.");
        else if (value < min || value > max)
            Console.WriteLine($"   Debe estar entre {min} y {max}.");
        else
            return value;
    }
}
int age = AskInt("3) Edad (0-120): ", 0, 120);
Console.WriteLine($"   -> {age}");

// FORMA 4: decimal, aceptando coma o punto (el usuario español escribe "12,50")
static decimal AskDecimal(string question)
{
    while (true)
    {
        Console.Write(question);
        string text = (ReadLine() ?? "").Trim().Replace(',', '.');                  // normalizamos la coma
        if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal value) && value >= 0)
            return value;
        Console.WriteLine("   Escribe un importe válido, por ejemplo 12,50.");
    }
}
decimal price = AskDecimal("4) Precio: ");
Console.WriteLine($"   -> {price.ToString("N2", CultureInfo.GetCultureInfo("es-ES"))} €");

// FORMA 5: fecha con formato exacto
static DateOnly AskDate(string question, string format = "dd/MM/yyyy")
{
    while (true)
    {
        Console.Write(question);
        if (DateOnly.TryParseExact(ReadLine(), format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            return date;
        Console.WriteLine($"   Formato esperado: {format.ToLower()}.");
    }
}
DateOnly birthday = AskDate("5) Fecha de nacimiento (dd/mm/aaaa): ");
Console.WriteLine($"   -> {birthday.ToString("dddd d 'de' MMMM 'de' yyyy", CultureInfo.GetCultureInfo("es-ES"))}");   // la cultura pone los nombres en español

// FORMA 6: sí / no
static bool AskYesNo(string question)
{
    while (true)
    {
        Console.Write(question + " (s/n): ");
        switch ((ReadLine() ?? "").Trim().ToLowerInvariant())
        {
            case "s" or "si" or "sí" or "y" or "yes": return true;
            case "n" or "no": return false;
        }
        Console.WriteLine("   Responde s o n.");
    }
}
bool newsletter = AskYesNo("6) ¿Quieres recibir novedades?");
Console.WriteLine($"   -> {newsletter}");

// FORMA 7: elegir de una lista (el usuario escribe el número; el programa devuelve el valor)
static T AskChoice<T>(string question, IReadOnlyList<T> options)
{
    Console.WriteLine(question);
    for (int i = 0; i < options.Count; i++) Console.WriteLine($"     {i + 1}) {options[i]}");
    int choice = AskInt("   Elige una opción: ", 1, options.Count);
    return options[choice - 1];
}
Priority priority = AskChoice("7) Prioridad:", Enum.GetValues<Priority>());
Console.WriteLine($"   -> {priority} (valor {(int)priority})");

// FORMA 8: varios datos en una sola línea ("nombre, edad, ciudad")
Console.Write("8) Escribe 'nombre, edad' separados por coma: ");
string[] parts = (ReadLine() ?? "").Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
if (parts.Length == 2 && int.TryParse(parts[1], out int partAge))
    Console.WriteLine($"   -> {parts[0]} tiene {partAge} años");
else
    Console.WriteLine("   Formato incorrecto");

// FORMA 9: varias líneas hasta una línea vacía
Console.WriteLine("9) Escribe notas (una línea vacía para terminar):");
var notes = new List<string>();
while (true)
{
    string line = (ReadLine() ?? "").Trim();
    if (line.Length == 0) break;
    notes.Add(line);
}
Console.WriteLine($"   -> {notes.Count} notas: {string.Join(" | ", notes)}");

// FORMA 10: valor por defecto (Enter acepta lo que se propone entre corchetes)
static string AskWithDefault(string question, string current)
{
    Console.Write($"{question} [{current}]: ");
    string text = (ReadLine() ?? "").Trim();
    return text.Length == 0 ? current : text;
}
string email = AskWithDefault("10) Correo", "ana@example.com");
Console.WriteLine($"   -> {email}");

// FORMA 11: una pregunta genérica que sirve para CUALQUIER tipo (la herramienta que acabarás usando)
static T Ask<T>(string question, Func<string, (bool Ok, T Value)> parse, string error)
{
    while (true)
    {
        Console.Write(question);
        var (ok, value) = parse(ReadLine() ?? "");
        if (ok) return value;
        Console.WriteLine("   " + error);
    }
}
var port = Ask("11) Puerto (1-65535): ",
    text => int.TryParse(text, out int p) && p is >= 1 and <= 65535 ? (true, p) : (false, 0),   // (¿válido?, valor)
    "Puerto no válido.");
Console.WriteLine($"   -> {port}");

Console.WriteLine("\nFin. La entrada se terminó o el usuario no escribió más.");
Console.WriteLine($"¿Hay más entrada? ReadLine devuelve: {(Console.ReadLine() is null ? "null (fin de la entrada)" : "texto")}");

enum Priority { Low = 1, Medium = 2, High = 3 }
