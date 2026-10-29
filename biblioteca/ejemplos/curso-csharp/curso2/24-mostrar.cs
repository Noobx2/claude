// Lección 24: mostrar datos en consola (tablas, formatos y varias "vistas" de los mismos datos)
// Ejecutar:  dotnet run 24-mostrar.cs
#:property PublishAot=false
using System.Globalization;
using System.Text;
using System.Text.Json;

var es = CultureInfo.GetCultureInfo("es-ES");
CultureInfo.CurrentCulture = es;                         // formato español para todo el programa
Console.OutputEncoding = Encoding.UTF8;                  // en Windows: para ver tildes, € y marcos

List<Product> products =
[
    new(1, "Teclado mecánico", "Periféricos", 59.90m, 25),
    new(2, "Ratón inalámbrico", "Periféricos", 24.50m, 60),
    new(3, "Monitor 27\" 4K", "Monitores", 329.00m, 4),
    new(4, "SSD 1 TB", "Almacenamiento", 79.90m, 0),
    new(5, "Cable HDMI 2 m", "Cables", 7.50m, 80),
];

// ───────── VISTA 1: lista simple (una línea por elemento) ─────────
Console.WriteLine("── Vista 1: lista simple ──");
foreach (var p in products) Console.WriteLine($"{p.Id}. {p.Name} - {p.Price:C}");

// ───────── VISTA 2: tabla alineada con anchos calculados ─────────
// Regla: texto a la izquierda, números a la derecha. El ancho de cada columna es el de su dato más largo.
Console.WriteLine("\n── Vista 2: tabla alineada ──");
Console.WriteLine(Table(
    ["Id", "Producto", "Categoría", "Precio", "Stock"],
    [false, false, false, true, true],                   // true = alinear a la derecha
    products.Select(p => (IReadOnlyList<string>)[p.Id.ToString(), p.Name, p.Category, p.Price.ToString("N2"), p.Stock.ToString()])));

// ───────── VISTA 3: tabla con marco (caracteres de dibujo) ─────────
Console.WriteLine("\n── Vista 3: tabla con marco ──");
Console.WriteLine(BoxTable(
    ["Producto", "Stock"],
    products.Select(p => (IReadOnlyList<string>)[Truncate(p.Name, 14), p.Stock == 0 ? "AGOTADO" : p.Stock.ToString()])));

// ───────── VISTA 4: ficha de detalle (un solo elemento) ─────────
Console.WriteLine("\n── Vista 4: detalle ──");
var detail = products[2];
Console.WriteLine($"{"Id",-10}{detail.Id}");
Console.WriteLine($"{"Nombre",-10}{detail.Name}");
Console.WriteLine($"{"Precio",-10}{detail.Price:C}");
Console.WriteLine($"{"Stock",-10}{detail.Stock} unidades" + (detail.Stock < 5 ? "   <-- poco stock" : ""));
Console.WriteLine($"{"Valor",-10}{detail.Price * detail.Stock:C}");

// ───────── VISTA 5: agrupado con totales ─────────
Console.WriteLine("\n── Vista 5: agrupado ──");
foreach (var group in products.GroupBy(p => p.Category).OrderBy(g => g.Key))
{
    Console.WriteLine($"{group.Key} ({group.Count()})");
    foreach (var p in group) Console.WriteLine($"   - {p.Name,-20}{p.Price,10:N2}");
    Console.WriteLine($"   {"Valor en stock",-22}{group.Sum(p => p.Price * p.Stock),10:N2}");
}
Console.WriteLine($"{"TOTAL EN ALMACÉN",-25}{products.Sum(p => p.Price * p.Stock),10:N2}");

// ───────── VISTA 6: paginación ─────────
Console.WriteLine("\n── Vista 6: paginación (2 por página) ──");
int pageSize = 2;
int pages = (int)Math.Ceiling(products.Count / (double)pageSize);
for (int page = 1; page <= pages; page++)
{
    Console.WriteLine($"Página {page}/{pages}");
    foreach (var p in products.Skip((page - 1) * pageSize).Take(pageSize)) Console.WriteLine($"   {p.Name}");
}

// ───────── VISTA 7: gráfico de barras con texto ─────────
Console.WriteLine("\n── Vista 7: barras ──");
int maxStock = products.Max(p => p.Stock);
foreach (var p in products)
    Console.WriteLine($"{Truncate(p.Name, 16),-16} {new string('█', (int)Math.Round(20.0 * p.Stock / maxStock))} {p.Stock}");

// ───────── VISTA 8: colores (solo se ven en un terminal; con redirección no hay códigos de color) ─────────
Console.WriteLine("\n── Vista 8: colores ──");
foreach (var p in products.Where(p => p.Stock < 5))
{
    var previous = Console.ForegroundColor;
    Console.ForegroundColor = p.Stock == 0 ? ConsoleColor.Red : ConsoleColor.Yellow;
    Console.WriteLine($"{(p.Stock == 0 ? "AGOTADO" : "BAJO")}: {p.Name}");
    Console.ForegroundColor = previous;                  // SIEMPRE restaurar el color
}

// ───────── VISTA 9: los mismos datos en JSON y CSV (para otros programas, no para personas) ─────────
Console.WriteLine("\n── Vista 9: JSON ──");
Console.WriteLine(JsonSerializer.Serialize(products.Take(2), new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping }));
Console.WriteLine("── Vista 10: CSV ──");
Console.WriteLine("id;name;category;price;stock");
foreach (var p in products.Take(2))
    Console.WriteLine($"{p.Id};{p.Name};{p.Category};{p.Price.ToString(CultureInfo.InvariantCulture)};{p.Stock}");

// ───────── VISTA 11: escoger el formato en tiempo de ejecución ─────────
Console.WriteLine("\n── Vista 11: --format ──");
foreach (string format in new[] { "lista", "json" })
{
    IProductView view = format == "json" ? new JsonView() : new ListView();   // Strategy: misma pregunta, distintas respuestas
    Console.WriteLine($"[{format}]");
    Console.WriteLine(view.Render(products.Take(2).ToList()));
}

// ───────── Utilidades ─────────
static string Truncate(string text, int max) => text.Length <= max ? text : text[..(max - 1)] + "…";

static string Table(IReadOnlyList<string> headers, IReadOnlyList<bool> right, IEnumerable<IReadOnlyList<string>> rows)
{
    var all = rows.ToList();
    int[] widths = headers.Select((h, c) => Math.Max(h.Length, all.Count == 0 ? 0 : all.Max(r => r[c].Length))).ToArray();
    string Line(IReadOnlyList<string> cells) => string.Join("  ", cells.Select((cell, c) => right[c] ? cell.PadLeft(widths[c]) : cell.PadRight(widths[c])));
    var sb = new StringBuilder();
    sb.AppendLine(Line(headers));
    sb.AppendLine(string.Join("  ", widths.Select(w => new string('-', w))));
    foreach (var row in all) sb.AppendLine(Line(row));
    return sb.ToString().TrimEnd();
}

static string BoxTable(IReadOnlyList<string> headers, IEnumerable<IReadOnlyList<string>> rows)
{
    var all = rows.ToList();
    int[] w = headers.Select((h, c) => Math.Max(h.Length, all.Max(r => r[c].Length))).ToArray();
    string Border(char l, char m, char r) => l + string.Join(m, w.Select(x => new string('─', x + 2))) + r;
    string Row(IReadOnlyList<string> cells) => "│" + string.Join("│", cells.Select((c, i) => " " + c.PadRight(w[i]) + " ")) + "│";
    var lines = new List<string> { Border('┌', '┬', '┐'), Row(headers), Border('├', '┼', '┤') };
    lines.AddRange(all.Select(Row));
    lines.Add(Border('└', '┴', '┘'));
    return string.Join("\n", lines);
}

record Product(int Id, string Name, string Category, decimal Price, int Stock);

interface IProductView { string Render(IReadOnlyList<Product> products); }
class ListView : IProductView
{
    public string Render(IReadOnlyList<Product> products) => string.Join("\n", products.Select(p => $"  • {p.Name} ({p.Stock})"));
}
class JsonView : IProductView
{
    public string Render(IReadOnlyList<Product> products) => JsonSerializer.Serialize(products.Select(p => new { p.Id, p.Name }), new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
}
