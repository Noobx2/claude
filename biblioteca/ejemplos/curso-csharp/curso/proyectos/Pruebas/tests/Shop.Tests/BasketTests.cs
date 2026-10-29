using Shop.Core;

namespace Shop.Tests;

public class BasketTests
{
    // El patrón de toda prueba: Arrange (preparar) - Act (actuar) - Assert (comprobar)
    [Fact]
    public void Total_SumsPriceTimesQuantity()
    {
        var basket = new Basket();                       // Arrange
        basket.Add("Teclado", 59.90m);
        basket.Add("Cable", 5m, quantity: 3);

        var total = basket.Total;                        // Act

        Assert.Equal(74.90m, total);                     // Assert (el valor ESPERADO va primero)
        Assert.Equal(4, basket.Count);
    }

    [Theory]                                             // la misma prueba con varios datos
    [InlineData(49.99, 49.99)]                           // sin descuento
    [InlineData(50, 47.50)]                              // 5 %
    [InlineData(99.99, 94.99)]
    [InlineData(100, 90)]                                // 10 %
    [InlineData(200, 180)]
    public void Discount_AppliesByTier(double amount, double expected)
    {
        var basket = new Basket();
        basket.Add("Producto", (decimal)amount);

        Assert.Equal((decimal)expected, basket.TotalWithDiscount());
    }

    [Theory]
    [InlineData("", 1, 1)]
    [InlineData("   ", 1, 1)]
    [InlineData("X", -1, 1)]
    [InlineData("X", 1, 0)]
    public void Add_RejectsInvalidData(string name, decimal price, int quantity)
    {
        var basket = new Basket();
        Assert.ThrowsAny<ArgumentException>(() => basket.Add(name, price, quantity));   // se espera una excepción
        Assert.Equal(0, basket.Count);                                                   // y que no se haya añadido nada
    }

    [Fact]
    public void EmptyBasket_HasZeroTotal()
    {
        var basket = new Basket();
        Assert.Equal(0m, basket.Total);
        Assert.Empty(Enumerable.Empty<int>());
    }

    [Fact(Skip = "ejemplo de prueba pendiente: se omite pero queda visible")]
    public void PendingTest() { }
}
