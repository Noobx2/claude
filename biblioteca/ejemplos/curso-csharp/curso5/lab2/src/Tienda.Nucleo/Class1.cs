namespace Tienda.Nucleo;
public static class Precio { public static decimal ConIva(decimal neto) => Math.Round(neto * 1.21m, 2); }
