using Tienda.Nucleo;

namespace Tienda.Pruebas;

public class CarritoTests
{
    [Fact]
    public void CarritoVacio_TieneTotalCero()
    {
        var carrito = new Carrito();
        Assert.Equal(0m, carrito.Total());
    }

    [Fact]
    public void Total_SumaPrecioPorCantidad()
    {
        var carrito = new Carrito();
        carrito.Anadir("Teclado", 25m, 2);
        carrito.Anadir("Ratón", 10m, 1);
        Assert.Equal(60m, carrito.Total());
    }

    [Theory]
    [InlineData("99.99", "99.99")]     // justo por debajo del umbral: sin descuento
    [InlineData("100", "90")]          // en el umbral: 10 % de descuento
    [InlineData("200", "180")]
    public void Total_AplicaDiezPorCientoDesdeCien(string bruto, string esperado)   // decimal no puede ir en [InlineData]: se pasa como texto
    {
        var carrito = new Carrito();
        carrito.Anadir("Producto", decimal.Parse(bruto, System.Globalization.CultureInfo.InvariantCulture), 1);
        Assert.Equal(decimal.Parse(esperado, System.Globalization.CultureInfo.InvariantCulture), carrito.Total());
    }

    [Fact]
    public void Anadir_ConCantidadNoPositiva_LanzaExcepcion()
    {
        var carrito = new Carrito();
        Assert.Throws<ArgumentOutOfRangeException>(() => carrito.Anadir("X", 5m, 0));
    }
}
