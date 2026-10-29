using Reqnroll;
using Tienda.Nucleo;

namespace Tienda.Pruebas.Features;

// Cada frase del archivo .feature se enlaza con un método mediante una expresión.
[Binding]
public class DescuentosSteps
{
    private readonly Carrito _carrito = new();
    private decimal _total;

    [Given(@"un carrito con un producto de (\d+) euros")]
    public void DadoUnProducto(int precio) => _carrito.Anadir("Producto", precio, 1);

    [Given(@"un carrito con (\d+) unidades de un producto de (\d+) euros")]
    public void DadoVariasUnidades(int unidades, int precio) => _carrito.Anadir("Producto", precio, unidades);

    [When("consulto el total")]
    public void CuandoConsultoElTotal() => _total = _carrito.Total();

    [Then(@"el total es (\d+) euros")]
    public void EntoncesElTotalEs(int esperado) => Assert.Equal(esperado, _total);
}
