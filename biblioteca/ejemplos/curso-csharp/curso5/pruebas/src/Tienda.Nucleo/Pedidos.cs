using System.Globalization;

namespace Tienda.Nucleo;

public interface IPasarelaPago { ResultadoPago Cobrar(string cliente, decimal importe); }
public interface INotificador { void Avisar(string cliente, string mensaje); }
public interface IRepositorioPedidos { void Guardar(Pedido pedido); }

public record ResultadoPago(bool Aceptado, string? Motivo = null);
public record Pedido(string Cliente, decimal Importe, EstadoPedido Estado);
public enum EstadoPedido { Pagado, Rechazado }

/// <summary>Orquesta: cobrar, guardar y avisar. Sus tres colaboradores llegan por el constructor, así se pueden sustituir en las pruebas.</summary>
public class ServicioPedidos(IPasarelaPago pasarela, IRepositorioPedidos repositorio, INotificador notificador)
{
    public Pedido Confirmar(string cliente, Carrito carrito)
    {
        decimal total = carrito.Total();
        ResultadoPago pago = pasarela.Cobrar(cliente, total);
        var pedido = new Pedido(cliente, total, pago.Aceptado ? EstadoPedido.Pagado : EstadoPedido.Rechazado);
        repositorio.Guardar(pedido);
        notificador.Avisar(cliente, pago.Aceptado ? $"Pago de {total.ToString("0.00", CultureInfo.InvariantCulture)} € recibido" : $"Pago rechazado: {pago.Motivo}");
        return pedido;
    }
}
