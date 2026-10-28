package inventory;

import java.math.BigDecimal;

/**
 * Un producto del inventario. Es un "record": una clase inmutable pensada para guardar datos.
 * El constructor compacto valida SIEMPRE, así es imposible crear un producto inválido.
 */
public record Product(int id, String name, String category, BigDecimal price, int stock) {

    public Product {
        if (name == null || name.isBlank()) {
            throw new IllegalArgumentException("El nombre no puede estar vacío");
        }
        if (category == null || category.isBlank()) {
            throw new IllegalArgumentException("La categoría no puede estar vacía");
        }
        if (price == null || price.signum() < 0) {
            throw new IllegalArgumentException("El precio no puede ser negativo");
        }
        if (stock < 0) {
            throw new IllegalArgumentException("El stock no puede ser negativo");
        }
        name = name.strip();
        category = category.strip();
    }

    /** Valor total de este producto en almacén (precio por unidades). */
    public BigDecimal totalValue() {
        return price.multiply(BigDecimal.valueOf(stock));
    }
}
