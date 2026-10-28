using System.Globalization;

namespace Inventory;

/// <summary>Se lanza cuando ya no hay más datos que leer (Ctrl+D / Ctrl+Z o tubería agotada).</summary>
public sealed class EndOfInputException : Exception;

/// <summary>
/// Captura de datos por consola. Cada método REPITE la pregunta hasta recibir un dato válido,
/// así el programa nunca se rompe por lo que escriba el usuario.
/// </summary>
public sealed class ConsoleInput(TextReader reader, TextWriter writer)
{
    /// <summary>Lee una línea; si no se escribe nada y hay valor por defecto, lo usa.</summary>
    public string Text(string prompt, string? defaultValue = null)
    {
        writer.Write(defaultValue is null ? $"{prompt}: " : $"{prompt} [{defaultValue}]: ");
        string line = (reader.ReadLine() ?? throw new EndOfInputException()).Trim();
        return line.Length == 0 && defaultValue is not null ? defaultValue : line;
    }

    /// <summary>Texto obligatorio: insiste mientras esté vacío.</summary>
    public string Required(string prompt)
    {
        while (true)
        {
            string value = Text(prompt);
            if (value.Length > 0) return value;
            writer.WriteLine("  Este dato es obligatorio.");
        }
    }

    /// <summary>Entero dentro de un rango.</summary>
    public int Integer(string prompt, int min, int max, int? defaultValue = null)
    {
        while (true)
        {
            string raw = Text(prompt, defaultValue?.ToString(CultureInfo.InvariantCulture));
            // TryParse devuelve false en lugar de lanzar una excepción: ideal para datos del usuario
            if (!int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
            {
                writer.WriteLine("  Escribe un número entero (por ejemplo 12).");
            }
            else if (value < min || value > max)
            {
                writer.WriteLine($"  Debe estar entre {min} y {max}.");
            }
            else
            {
                return value;
            }
        }
    }

    /// <summary>Número decimal; acepta coma o punto como separador.</summary>
    public decimal Decimal(string prompt, decimal? defaultValue = null)
    {
        while (true)
        {
            string raw = Text(prompt, defaultValue?.ToString(CultureInfo.InvariantCulture)).Replace(',', '.');
            // InvariantCulture: el punto decimal es el mismo en cualquier ordenador, sin sorpresas de idioma
            if (!decimal.TryParse(raw, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal value))
            {
                writer.WriteLine("  Escribe un número (por ejemplo 12,50).");
            }
            else if (value < 0)
            {
                writer.WriteLine("  No puede ser negativo.");
            }
            else
            {
                return value;
            }
        }
    }

    /// <summary>Pregunta sí/no.</summary>
    public bool Confirm(string prompt)
    {
        while (true)
        {
            switch (Text($"{prompt} (s/n)").ToLowerInvariant())
            {
                case "s" or "si" or "sí": return true;
                case "n" or "no": return false;
                default: writer.WriteLine("  Responde s o n."); break;
            }
        }
    }
}
