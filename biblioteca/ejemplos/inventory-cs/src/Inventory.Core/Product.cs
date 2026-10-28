using System.Text.Json.Serialization;

namespace Inventory;

/// <summary>
/// Un producto del inventario. Es inmutable (solo lectura) y se valida al crearlo,
/// así es imposible tener un producto inválido en memoria.
/// </summary>
public sealed record Product
{
    public int Id { get; }
    public string Name { get; }
    public string Category { get; }
    public decimal Price { get; }
    public int Stock { get; }

    public Product(int id, string name, string category, decimal price, int stock)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("El nombre no puede estar vacío", nameof(name));
        if (string.IsNullOrWhiteSpace(category)) throw new ArgumentException("La categoría no puede estar vacía", nameof(category));
        if (price < 0) throw new ArgumentException("El precio no puede ser negativo", nameof(price));
        if (stock < 0) throw new ArgumentException("El stock no puede ser negativo", nameof(stock));

        Id = id;
        Name = name.Trim();
        Category = category.Trim();
        Price = price;
        Stock = stock;
    }

    /// <summary>Valor total de este producto en almacén. Es un cálculo: no se guarda en el archivo.</summary>
    [JsonIgnore]
    public decimal TotalValue => Price * Stock;
}
