package shop;

import jakarta.persistence.Entity;
import jakarta.persistence.FetchType;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.Id;
import jakarta.persistence.ManyToOne;
import java.math.BigDecimal;

@Entity
public class Product {
    @Id @GeneratedValue
    Long id;
    String name;
    BigDecimal price;

    @ManyToOne(fetch = FetchType.LAZY)       // LAZY: la categoría se carga solo cuando se usa
    Category category;

    protected Product() { }
    Product(String name, String price) { this.name = name; this.price = new BigDecimal(price); }
}
