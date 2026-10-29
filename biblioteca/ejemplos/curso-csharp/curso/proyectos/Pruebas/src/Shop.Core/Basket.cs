namespace Shop.Core;

/// <summary>Cesta de la compra: la lógica que queremos probar.</summary>
public class Basket
{
    private readonly List<(string Name, decimal Price, int Quantity)> _lines = [];

    public int Count => _lines.Sum(l => l.Quantity);
    public decimal Total => _lines.Sum(l => l.Price * l.Quantity);

    public void Add(string name, decimal price, int quantity = 1)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("El nombre es obligatorio", nameof(name));
        if (price < 0) throw new ArgumentOutOfRangeException(nameof(price), "El precio no puede ser negativo");
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity), "La cantidad debe ser positiva");
        _lines.Add((name.Trim(), price, quantity));
    }

    /// <summary>Descuento por tramos: 5 % desde 50 €, 10 % desde 100 €.</summary>
    public decimal TotalWithDiscount() => Total switch
    {
        >= 100m => Math.Round(Total * 0.90m, 2),
        >= 50m => Math.Round(Total * 0.95m, 2),
        _ => Total,
    };
}
