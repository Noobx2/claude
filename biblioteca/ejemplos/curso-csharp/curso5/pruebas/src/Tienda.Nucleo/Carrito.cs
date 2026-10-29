namespace Tienda.Nucleo;

public class Carrito
{
    private const decimal UmbralDescuento = 100m;
    private const decimal PorcentajeDescuento = 0.10m;

    private readonly List<(string Producto, decimal Precio, int Cantidad)> _lineas = [];

    public void Anadir(string producto, decimal precio, int cantidad)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(cantidad);
        ArgumentOutOfRangeException.ThrowIfNegative(precio);
        _lineas.Add((producto, precio, cantidad));
    }

    public decimal Bruto() => _lineas.Sum(l => l.Precio * l.Cantidad);

    public decimal Total()                       // REFACTOR: el descuento tiene su propio nombre en vez de estar mezclado
    {
        decimal bruto = Bruto();
        return bruto >= UmbralDescuento ? Math.Round(bruto * (1 - PorcentajeDescuento), 2) : bruto;
    }
}
