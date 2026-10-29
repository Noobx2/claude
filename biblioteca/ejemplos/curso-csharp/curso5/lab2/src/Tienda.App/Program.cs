using Tienda.Nucleo;
Console.WriteLine($"100 € netos = {Precio.ConIva(100m)} € con IVA");
Console.WriteLine($"La clase Precio vive en el ensamblado: {typeof(Precio).Assembly.GetName().Name}.dll");
