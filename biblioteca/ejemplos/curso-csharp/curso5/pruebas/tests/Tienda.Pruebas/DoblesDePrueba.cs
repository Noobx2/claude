using NSubstitute;
using Tienda.Nucleo;

namespace Tienda.Pruebas;

// ---- Dobles escritos a mano: así se ve qué es cada cosa ----
file sealed class PasarelaStub(bool acepta) : IPasarelaPago          // STUB: devuelve respuestas preparadas
{
    public ResultadoPago Cobrar(string cliente, decimal importe) => new(acepta, acepta ? null : "Fondos insuficientes");
}
file sealed class NotificadorEspia : INotificador                    // SPY: recuerda cómo lo llamaron para comprobarlo después
{
    public List<(string Cliente, string Mensaje)> Enviados { get; } = [];
    public void Avisar(string cliente, string mensaje) => Enviados.Add((cliente, mensaje));
}
file sealed class RepositorioFalso : IRepositorioPedidos             // FAKE: implementación real pero simple (en memoria)
{
    public List<Pedido> Guardados { get; } = [];
    public void Guardar(Pedido pedido) => Guardados.Add(pedido);
}

public class ServicioPedidosTests
{
    private static Carrito CarritoDe(decimal precio) { var c = new Carrito(); c.Anadir("Cosa", precio, 1); return c; }

    [Fact]
    public void ConDoblesManuales_PagoAceptado_GuardaPagadoYAvisa()
    {
        var repo = new RepositorioFalso(); var aviso = new NotificadorEspia();
        var servicio = new ServicioPedidos(new PasarelaStub(acepta: true), repo, aviso);

        var pedido = servicio.Confirmar("ana", CarritoDe(200m));

        Assert.Equal(EstadoPedido.Pagado, pedido.Estado);
        Assert.Equal(180m, pedido.Importe);                                  // 200 € con 10 % de descuento
        Assert.Single(repo.Guardados);
        Assert.Equal("Pago de 180.00 € recibido", aviso.Enviados.Single().Mensaje);
    }

    [Fact]
    public void ConNSubstitute_PagoRechazado_NoDiceQueSePago()
    {
        // NSubstitute crea los dobles en una línea y permite configurar y verificar
        var pasarela = Substitute.For<IPasarelaPago>();
        pasarela.Cobrar("luis", Arg.Any<decimal>()).Returns(new ResultadoPago(false, "Tarjeta caducada"));
        var repo = Substitute.For<IRepositorioPedidos>();
        var aviso = Substitute.For<INotificador>();
        var servicio = new ServicioPedidos(pasarela, repo, aviso);

        var pedido = servicio.Confirmar("luis", CarritoDe(50m));

        Assert.Equal(EstadoPedido.Rechazado, pedido.Estado);
        repo.Received(1).Guardar(Arg.Is<Pedido>(p => p.Estado == EstadoPedido.Rechazado));          // se guardó una vez, como rechazado
        aviso.Received(1).Avisar("luis", Arg.Is<string>(m => m.Contains("Tarjeta caducada")));
        aviso.DidNotReceive().Avisar(Arg.Any<string>(), Arg.Is<string>(m => m.Contains("recibido")));   // y NO se avisó de un pago
    }

    [Fact]
    public void Cobrar_RecibeElTotalConDescuento()
    {
        var pasarela = Substitute.For<IPasarelaPago>();
        pasarela.Cobrar(default!, default).ReturnsForAnyArgs(new ResultadoPago(true));
        var servicio = new ServicioPedidos(pasarela, Substitute.For<IRepositorioPedidos>(), Substitute.For<INotificador>());

        servicio.Confirmar("eva", CarritoDe(100m));

        pasarela.Received().Cobrar("eva", 90m);                               // lo que realmente se le pidió a la pasarela
    }
}
