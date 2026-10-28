package shop;

import jakarta.persistence.CascadeType;
import jakarta.persistence.Entity;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.Id;
import jakarta.persistence.OneToMany;
import java.util.ArrayList;
import java.util.List;

@Entity   // esta clase es una tabla
public class Category {
    @Id @GeneratedValue
    Long id;
    String name;

    @OneToMany(mappedBy = "category", cascade = CascadeType.ALL)   // 1 categoría -> N productos
    List<Product> products = new ArrayList<>();

    protected Category() { }                                       // Hibernate necesita un constructor sin argumentos
    Category(String name) { this.name = name; }

    void add(Product product) { products.add(product); product.category = this; }
}
