import inventory.Input;
import inventory.Product;
import inventory.ProductRepository;
import inventory.Report;
import java.math.BigDecimal;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.Scanner;

/**
 * Pruebas sin ningún framework: un método por prueba y una función `check` que falla con un mensaje claro.
 * Ejecutar:  java -cp out:test-out Tests
 */
public class Tests {

    private static int passed = 0;
    private static int failed = 0;

    public static void main(String[] args) throws Exception {
        run("producto válido calcula su valor total", Tests::validProduct);
        run("producto inválido lanza excepción", Tests::invalidProduct);
        run("el repositorio añade, busca, edita y borra", Tests::repositoryCrud);
        run("el CSV conserva comas y comillas", Tests::csvRoundTrip);
        run("la entrada repite hasta recibir un dato válido", Tests::inputRetries);
        run("el resumen agrupa por categoría", Tests::summaryGroups);

        System.out.printf("%n%d correctas, %d fallidas%n", passed, failed);
        if (failed > 0) {
            System.exit(1);
        }
    }

    // ───────────── pruebas ─────────────

    static void validProduct() {
        Product p = new Product(1, "  Teclado ", "Periféricos", new BigDecimal("25.50"), 10);
        check("Teclado".equals(p.name()), "el nombre debe recortarse");
        check(new BigDecimal("255.00").compareTo(p.totalValue()) == 0, "valor total 255.00, fue " + p.totalValue());
    }

    static void invalidProduct() {
        expectFailure(() -> new Product(1, " ", "x", BigDecimal.ONE, 1), "nombre vacío");
        expectFailure(() -> new Product(1, "a", "x", new BigDecimal("-1"), 1), "precio negativo");
        expectFailure(() -> new Product(1, "a", "x", BigDecimal.ONE, -1), "stock negativo");
    }

    static void repositoryCrud() {
        ProductRepository repo = new ProductRepository();
        Product a = repo.add("Teclado", "Periféricos", new BigDecimal("25"), 3);
        repo.add("Monitor", "Pantallas", new BigDecimal("100"), 2);
        check(repo.findAll().size() == 2, "debe haber 2 productos");
        check(repo.search("tecla").size() == 1, "la búsqueda no distingue mayúsculas");
        check(repo.update(new Product(a.id(), "Teclado RGB", a.category(), a.price(), 5)), "update debe devolver true");
        check(repo.findById(a.id()).orElseThrow().stock() == 5, "el stock debe ser 5");
        check(repo.delete(a.id()) && !repo.delete(a.id()), "borrar dos veces: true y luego false");
    }

    static void csvRoundTrip() throws Exception {
        Path file = Files.createTempFile("products", ".csv");
        try {
            ProductRepository repo = new ProductRepository();
            repo.add("Cable \"premium\", 2 m", "Accesorios, varios", new BigDecimal("3.5"), 100);
            repo.save(file);

            ProductRepository loaded = new ProductRepository();
            loaded.load(file);
            Product p = loaded.findAll().get(0);
            check(p.name().equals("Cable \"premium\", 2 m"), "nombre con comillas y coma: " + p.name());
            check(p.category().equals("Accesorios, varios"), "categoría con coma: " + p.category());
            check(loaded.add("Nuevo", "x", BigDecimal.ONE, 1).id() == 2, "el siguiente id debe continuar (2)");
        } finally {
            Files.deleteIfExists(file);
        }
    }

    static void inputRetries() {
        Input input = new Input(new Scanner("abc\n-5\n99\n7\n"));
        check(input.integer("n", 1, 10) == 7, "debe ignorar 'abc', -5 y 99 y devolver 7");
        Input decimals = new Input(new Scanner("x\n12,50\n"));
        check(new BigDecimal("12.50").compareTo(decimals.decimal("p")) == 0, "debe aceptar la coma decimal");
    }

    static void summaryGroups() {
        ProductRepository repo = new ProductRepository();
        repo.add("A", "Cat1", new BigDecimal("10"), 2);
        repo.add("B", "Cat1", new BigDecimal("5"), 2);
        repo.add("C", "Cat2", new BigDecimal("1"), 1);
        String text = Report.summary(repo.findAll());
        check(text.contains("Productos: 3"), text);
        check(text.contains("Cat1") && text.contains("30,00") && text.contains("Cat2"), text);
    }

    // ───────────── mini marco de pruebas ─────────────

    interface TestBody {
        void run() throws Exception;
    }

    static void run(String name, TestBody body) {
        try {
            body.run();
            passed++;
            System.out.println("  OK   " + name);
        } catch (Throwable e) {
            failed++;
            System.out.println("  FALLA " + name + " -> " + e.getMessage());
        }
    }

    static void check(boolean condition, String message) {
        if (!condition) {
            throw new AssertionError(message);
        }
    }

    static void expectFailure(Runnable code, String what) {
        try {
            code.run();
        } catch (IllegalArgumentException expected) {
            return;
        }
        throw new AssertionError("debía fallar: " + what);
    }
}
