using System.Text;

namespace Inventory;

/// <summary>Dibuja una tabla de texto con las columnas alineadas, calculando el ancho de cada una.</summary>
public static class TablePrinter
{
    public static string Format(IReadOnlyList<string> headers, IReadOnlyList<bool> rightAligned, IEnumerable<IReadOnlyList<string>> rows)
    {
        var data = rows.ToList();
        // El ancho de cada columna es el del texto más largo (cabecera incluida)
        int[] widths = headers.Select((header, c) => Math.Max(header.Length, data.Select(row => row[c].Length).DefaultIfEmpty(0).Max())).ToArray();

        var output = new StringBuilder();
        AppendRow(output, headers, widths, rightAligned);
        output.AppendLine(string.Join("-+-", widths.Select(width => new string('-', width))));
        foreach (var row in data) AppendRow(output, row, widths, rightAligned);
        return output.ToString();
    }

    private static void AppendRow(StringBuilder output, IReadOnlyList<string> cells, int[] widths, IReadOnlyList<bool> rightAligned)
    {
        var padded = cells.Select((cell, c) => rightAligned[c] ? cell.PadLeft(widths[c]) : cell.PadRight(widths[c]));
        output.AppendLine(string.Join(" | ", padded));
    }
}
