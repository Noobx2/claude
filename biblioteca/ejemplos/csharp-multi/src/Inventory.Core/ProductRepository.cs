using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Inventory;

/// <summary>
/// Gestiona los datos: los guarda en memoria y los persiste en JSON (y los exporta a CSV).
/// Solo usa la biblioteca base de .NET: sin NuGet, sin frameworks.
/// </summary>
public sealed class ProductRepository
{
    private readonly List<Product> _products = [];
    private int _lastId;

    // WriteIndented: JSON legible. UnsafeRelaxed: conserva tildes y € tal cual en lugar de \u00E9
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    // UTF-8 SIN marca de orden de bytes (BOM): lo habitual en archivos JSON
    private static readonly UTF8Encoding Utf8NoBom = new(encoderShouldEmitUTF8Identifier: false);

    // ───────────── Consultar ─────────────

    /// <summary>Devuelve una COPIA de solo lectura: quien la reciba no puede estropear la lista interna.</summary>
    public IReadOnlyList<Product> FindAll() => _products.ToList();

    public Product? FindById(int id) => _products.FirstOrDefault(p => p.Id == id);

    public IReadOnlyList<Product> Search(string text) =>
        _products
            .Where(p => p.Name.Contains(text, StringComparison.OrdinalIgnoreCase)
                     || p.Category.Contains(text, StringComparison.OrdinalIgnoreCase))
            .OrderBy(p => p.Name, StringComparer.CurrentCultureIgnoreCase)
            .ToList();

    // ───────────── Crear, modificar, borrar ─────────────

    public Product Add(string name, string category, decimal price, int stock)
    {
        var product = new Product(++_lastId, name, category, price, stock);
        _products.Add(product);
        return product;
    }

    public bool Update(Product updated)
    {
        int index = _products.FindIndex(p => p.Id == updated.Id);
        if (index < 0) return false;
        _products[index] = updated;
        return true;
    }

    public bool Delete(int id) => _products.RemoveAll(p => p.Id == id) > 0;

    // ───────────── Guardar y cargar (JSON) ─────────────

    public void Save(string path) => File.WriteAllText(path, JsonSerializer.Serialize(_products, JsonOptions), Utf8NoBom);

    public void Load(string path)
    {
        _products.Clear();
        _lastId = 0;
        if (!File.Exists(path)) return;   // primera ejecución: aún no hay archivo

        var loaded = JsonSerializer.Deserialize<List<Product>>(File.ReadAllText(path, Encoding.UTF8)) ?? [];
        _products.AddRange(loaded);
        _lastId = _products.Select(p => p.Id).DefaultIfEmpty(0).Max();
    }

    // ───────────── Exportar a CSV (para abrir en Excel) ─────────────

    public void ExportCsv(string path)
    {
        var lines = new List<string> { "id;name;category;price;stock" };   // «;» porque en español Excel usa la coma decimal
        lines.AddRange(_products.Select(p => string.Join(';',
            p.Id, Quote(p.Name), Quote(p.Category),
            p.Price.ToString(System.Globalization.CultureInfo.InvariantCulture), p.Stock)));
        File.WriteAllLines(path, lines, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));   // BOM: Excel reconoce UTF-8
    }

    internal static string Quote(string value) =>
        value.Contains(';') || value.Contains('"') || value.Contains('\n')
            ? $"\"{value.Replace("\"", "\"\"")}\""
            : value;
}
