using UsingsDemo.Models;       // los using también pueden ir aquí: cada archivo declara los suyos

namespace UsingsDemo.Services  // namespace "de bloque" (estilo antiguo): hay que abrir y cerrar llaves
{
    public class PriceService
    {
        public decimal WithVat(Product product, decimal vat = 0.21m) => Math.Round(product.Price * (1 + vat), 2);
    }
}
