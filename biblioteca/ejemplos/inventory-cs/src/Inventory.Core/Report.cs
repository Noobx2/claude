using System.Globalization;

namespace Inventory;

/// <summary>Cálculos y presentación de datos: tablas y resúmenes.</summary>
public static class Report
{
    private static readonly CultureInfo Spanish = CultureInfo.GetCultureInfo("es-ES");

    /// <summary>Dinero con dos decimales y coma decimal (estilo español).</summary>
    public static string Money(decimal value) => value.ToString("N2", Spanish) + " €";

    public static string ProductTable(IEnumerable<Product> products) =>
        TablePrinter.Format(
            ["ID", "Nombre", "Categoría", "Precio", "Stock", "Valor"],
            [true, false, false, true, true, true],
            products.Select(p => (IReadOnlyList<string>)[p.Id.ToString(), p.Name, p.Category, Money(p.Price), p.Stock.ToString(), Money(p.TotalValue)]));

    public static string Summary(IReadOnlyList<Product> products)
    {
        var lines = new List<string>
        {
            $"Productos: {products.Count} | Unidades: {products.Sum(p => p.Stock)} | Valor total: {Money(products.Sum(p => p.TotalValue))}",
        };
        // LINQ: agrupar por categoría y sumar el valor de cada grupo
        foreach (var group in products.GroupBy(p => p.Category).OrderBy(g => g.Key))
        {
            lines.Add($"  {group.Key,-15} {Money(group.Sum(p => p.TotalValue)),12}");   // {x,-15}: alinear a la izquierda en 15; {x,12}: a la derecha en 12
        }
        return string.Join(Environment.NewLine, lines) + Environment.NewLine;
    }
}
