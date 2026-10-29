using System.Globalization;
using TaskFlow.Domain;

namespace TaskFlow.Console;

/// <summary>Todas las formas de capturar datos, en un solo sitio y reutilizables.</summary>
public sealed class Prompts(IConsoleIO io)
{
    // Toda lectura pasa por aquí: si no hay más entrada, se corta el formulario con una excepción propia.
    // Sin esto, un bucle "repetir hasta que sea válido" sería INFINITO con la entrada terminada.
    private string ReadLine() => io.ReadLine() ?? throw new EndOfInputException();

    /// <summary>Texto. Si "current" tiene valor y el usuario pulsa Enter, se conserva (edición).</summary>
    public string Text(string question, string? current = null, bool required = false)
    {
        while (true)
        {
            io.Write(current is null ? $"{question}: " : $"{question} [{current}]: ");
            string text = ReadLine().Trim();
            if (text.Length > 0) return text;
            if (current is not null) return current;
            if (!required) return "";
            io.WriteLine("  Es obligatorio.");
        }
    }

    public int Int(string question, int min, int max)
    {
        while (true)
        {
            io.Write($"{question}: ");
            if (!int.TryParse(ReadLine(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int value)) io.WriteLine("  Escribe un número entero.");
            else if (value < min || value > max) io.WriteLine($"  Debe estar entre {min} y {max}.");
            else return value;
        }
    }

    public bool YesNo(string question)
    {
        while (true)
        {
            io.Write($"{question} (s/n): ");
            string answer = ReadLine().Trim().ToLowerInvariant();
            if (answer is "s" or "si" or "sí") return true;
            if (answer is "n" or "no") return false;
        }
    }

    /// <summary>Fecha opcional en formato dd/MM/aaaa; vacío = sin fecha (o conservar la actual).</summary>
    public DateOnly? Date(string question, DateOnly? current = null)
    {
        while (true)
        {
            io.Write(current is null ? $"{question} (dd/mm/aaaa, vacío = ninguna): " : $"{question} [{current:dd/MM/yyyy}]: ");
            string text = ReadLine().Trim();
            if (text.Length == 0) return current;
            if (DateOnly.TryParseExact(text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)) return date;
            io.WriteLine("  Formato esperado: dd/mm/aaaa.");
        }
    }

    public Priority Priority(string question, Priority? current = null)
    {
        io.WriteLine(question + (current is null ? "" : $" (actual: {Name(current.Value)})"));
        io.WriteLine("  1) Baja   2) Media   3) Alta");
        while (true)
        {
            io.Write(current is null ? "  Elige: " : $"  Elige [Enter = {Name(current.Value)}]: ");
            string text = ReadLine().Trim();
            if (text.Length == 0 && current is not null) return current.Value;
            if (int.TryParse(text, out int n) && n is >= 1 and <= 3) return (Priority)n;
            io.WriteLine("  Escribe 1, 2 o 3.");
        }
    }

    private static string Name(Priority p) => p switch { Domain.Priority.High => "Alta", Domain.Priority.Medium => "Media", _ => "Baja" };
}
