package inventory;

import java.math.BigDecimal;
import java.util.List;
import java.util.Map;
import java.util.TreeMap;
import java.util.stream.Collectors;

/** Cálculos y presentación de datos: tablas y resúmenes. */
public class Report {

    private Report() {
    }

    public static String productTable(List<Product> products) {
        Table table = new Table(
                List.of("ID", "Nombre", "Categoría", "Precio", "Stock", "Valor"),
                List.of(true, false, false, true, true, true));
        for (Product p : products) {
            table.addRow(List.of(
                    String.valueOf(p.id()), p.name(), p.category(), money(p.price()), String.valueOf(p.stock()), money(p.totalValue())));
        }
        return table.toString();
    }

    public static String summary(List<Product> products) {
        BigDecimal total = products.stream().map(Product::totalValue).reduce(BigDecimal.ZERO, BigDecimal::add);
        int units = products.stream().mapToInt(Product::stock).sum();

        // Agrupar por categoría: valor total de cada una (TreeMap = ordenadas alfabéticamente)
        Map<String, BigDecimal> byCategory = products.stream().collect(Collectors.groupingBy(
                Product::category, TreeMap::new,
                Collectors.reducing(BigDecimal.ZERO, Product::totalValue, BigDecimal::add)));

        StringBuilder out = new StringBuilder();
        out.append(String.format("Productos: %d | Unidades: %d | Valor total: %s%n", products.size(), units, money(total)));
        byCategory.forEach((category, value) -> out.append(String.format("  %-15s %12s%n", category, money(value))));
        return out.toString();
    }

    /** Formato de dinero con dos decimales y coma decimal (estilo español). */
    public static String money(BigDecimal value) {
        return String.format(java.util.Locale.forLanguageTag("es-ES"), "%,.2f €", value);
    }
}
