package inventory;

import java.io.IOException;
import java.math.BigDecimal;
import java.nio.charset.StandardCharsets;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.ArrayList;
import java.util.Comparator;
import java.util.List;
import java.util.Optional;

/**
 * Gestiona los datos: los guarda en memoria y los persiste en un archivo CSV.
 * Solo usa la librería estándar de Java (sin frameworks ni dependencias).
 */
public class ProductRepository {

    private final List<Product> products = new ArrayList<>();
    private int lastId = 0;

    // ───────────── Consultar ─────────────

    /** Devuelve una COPIA: quien la reciba no puede estropear nuestra lista interna. */
    public List<Product> findAll() {
        return List.copyOf(products);
    }

    public Optional<Product> findById(int id) {
        return products.stream().filter(product -> product.id() == id).findFirst();
    }

    public List<Product> search(String text) {
        String needle = text.toLowerCase();
        return products.stream()
                .filter(p -> p.name().toLowerCase().contains(needle) || p.category().toLowerCase().contains(needle))
                .sorted(Comparator.comparing(Product::name, String.CASE_INSENSITIVE_ORDER))
                .toList();
    }

    // ───────────── Crear, modificar, borrar ─────────────

    public Product add(String name, String category, BigDecimal price, int stock) {
        Product product = new Product(++lastId, name, category, price, stock);
        products.add(product);
        return product;
    }

    public boolean update(Product updated) {
        for (int i = 0; i < products.size(); i++) {
            if (products.get(i).id() == updated.id()) {
                products.set(i, updated);
                return true;
            }
        }
        return false;
    }

    public boolean delete(int id) {
        return products.removeIf(product -> product.id() == id);
    }

    // ───────────── Guardar y cargar (CSV) ─────────────

    public void save(Path file) throws IOException {
        List<String> lines = new ArrayList<>();
        lines.add("id,name,category,price,stock");
        for (Product p : products) {
            lines.add(String.join(",",
                    String.valueOf(p.id()), quote(p.name()), quote(p.category()), p.price().toPlainString(), String.valueOf(p.stock())));
        }
        Files.write(file, lines, StandardCharsets.UTF_8);
    }

    public void load(Path file) throws IOException {
        products.clear();
        lastId = 0;
        if (!Files.exists(file)) {
            return; // primera ejecución: aún no hay archivo
        }
        List<String> lines = Files.readAllLines(file, StandardCharsets.UTF_8);
        for (String line : lines.stream().skip(1).filter(l -> !l.isBlank()).toList()) {
            List<String> cells = parseCsvLine(line);
            Product product = new Product(
                    Integer.parseInt(cells.get(0)), cells.get(1), cells.get(2), new BigDecimal(cells.get(3)), Integer.parseInt(cells.get(4)));
            products.add(product);
            lastId = Math.max(lastId, product.id());
        }
    }

    /** Si el texto lleva comas, comillas o saltos de línea, se rodea de comillas y se duplican las internas. */
    static String quote(String value) {
        if (value.contains(",") || value.contains("\"") || value.contains("\n")) {
            return "\"" + value.replace("\"", "\"\"") + "\"";
        }
        return value;
    }

    /** Divide una línea CSV respetando los campos entre comillas. */
    static List<String> parseCsvLine(String line) {
        List<String> cells = new ArrayList<>();
        StringBuilder current = new StringBuilder();
        boolean inQuotes = false;
        for (int i = 0; i < line.length(); i++) {
            char c = line.charAt(i);
            if (inQuotes) {
                if (c == '"' && i + 1 < line.length() && line.charAt(i + 1) == '"') {
                    current.append('"'); // comilla escapada
                    i++;
                } else if (c == '"') {
                    inQuotes = false;
                } else {
                    current.append(c);
                }
            } else if (c == '"') {
                inQuotes = true;
            } else if (c == ',') {
                cells.add(current.toString());
                current.setLength(0);
            } else {
                current.append(c);
            }
        }
        cells.add(current.toString());
        return cells;
    }
}
