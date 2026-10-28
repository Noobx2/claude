package inventory;

import java.io.IOException;
import java.nio.file.Path;
import java.util.List;
import java.util.Optional;
import java.util.Scanner;

/** Programa de consola: menú para capturar, mostrar y gestionar los datos del inventario. */
public class Main {

    private final Input input;
    private final ProductRepository repository = new ProductRepository();
    private final Path dataFile;

    public Main(Input input, Path dataFile) {
        this.input = input;
        this.dataFile = dataFile;
    }

    public static void main(String[] args) throws IOException {
        Path file = Path.of(args.length > 0 ? args[0] : "products.csv");
        Main app = new Main(new Input(new Scanner(System.in)), file);
        app.repository.load(file);
        try {
            app.run();
        } catch (Input.EndOfInputException e) {
            app.repository.save(file); // si se cierra la entrada, no se pierde lo hecho
        }
    }

    private void run() throws IOException {
        System.out.println("INVENTARIO (" + repository.findAll().size() + " productos cargados de " + dataFile + ")");
        while (true) {
            System.out.println();
            System.out.println("1) Añadir  2) Listar  3) Buscar  4) Editar  5) Borrar  6) Resumen  0) Guardar y salir");
            int option = input.integer("Opción", 0, 6);
            switch (option) {
                case 1 -> addProduct();
                case 2 -> showAll();
                case 3 -> search();
                case 4 -> edit();
                case 5 -> delete();
                case 6 -> System.out.print(Report.summary(repository.findAll()));
                case 0 -> {
                    repository.save(dataFile);
                    System.out.println("Guardado en " + dataFile + ". ¡Hasta luego!");
                    return;
                }
                default -> throw new IllegalStateException("Opción imposible: " + option);
            }
        }
    }

    private void addProduct() {
        String name = input.requiredText("Nombre");
        String category = input.requiredText("Categoría");
        var price = input.decimal("Precio (€)");
        int stock = input.integer("Stock", 0, 1_000_000);
        Product product = repository.add(name, category, price, stock);
        System.out.println("Añadido con id " + product.id());
    }

    private void showAll() {
        List<Product> all = repository.findAll();
        if (all.isEmpty()) {
            System.out.println("No hay productos. Usa la opción 1 para añadir el primero.");
            return;
        }
        System.out.print(Report.productTable(all));
    }

    private void search() {
        String text = input.requiredText("Texto a buscar (nombre o categoría)");
        List<Product> found = repository.search(text);
        System.out.println(found.isEmpty() ? "Sin resultados." : Report.productTable(found));
    }

    private void edit() {
        Optional<Product> found = repository.findById(input.integer("Id del producto", 1, Integer.MAX_VALUE));
        if (found.isEmpty()) {
            System.out.println("No existe ese producto.");
            return;
        }
        Product old = found.get();
        // Enter sin escribir nada mantiene el valor actual
        String name = input.text("Nombre", old.name());
        String category = input.text("Categoría", old.category());
        var price = input.decimal("Precio (€)", old.price());
        int stock = input.integer("Stock", 0, 1_000_000, old.stock());
        repository.update(new Product(old.id(), name, category, price, stock));
        System.out.println("Actualizado.");
    }

    private void delete() {
        int id = input.integer("Id del producto", 1, Integer.MAX_VALUE);
        Optional<Product> found = repository.findById(id);
        if (found.isEmpty()) {
            System.out.println("No existe ese producto.");
        } else if (input.confirm("¿Borrar \"" + found.get().name() + "\"?")) {
            repository.delete(id);
            System.out.println("Borrado.");
        }
    }
}
