# Java sin frameworks: capturar, mostrar y gestionar datos

Todo lo de este módulo usa **solo la biblioteca estándar de Java** (sin Maven, sin Spring, sin librerías). Construimos un **inventario de productos** que:

1. **Captura** datos del teclado, repitiendo la pregunta si el dato es inválido.
2. **Muestra** los datos en tablas alineadas y en resúmenes.
3. **Gestiona** los datos: añadir, buscar, editar, borrar, guardar en un archivo CSV y recargarlo.
4. Tiene una **ventana gráfica** (Swing) con la misma lógica.
5. Tiene **pruebas** sin JUnit.

> <span class="tag ok">comprobado</span> Todo el código de abajo se compiló con `javac` (JDK 25) y se ejecutó: sesión de consola con entrada simulada, 6 pruebas en verde y la ventana Swing abierta bajo un servidor X virtual (captura de pantalla correcta). La ventana la ves tú en tu escritorio; en una VM sin entorno gráfico por SSH no se abrirá.

## 1. Estructura y por qué

```text
inventory-java/
├── src/inventory/
│   ├── Product.java            ← el dato (record con validación)
│   ├── Input.java              ← capturar datos del teclado
│   ├── Table.java              ← dibujar tablas alineadas
│   ├── Report.java             ← mostrar: listado y resumen
│   ├── ProductRepository.java  ← gestionar: CRUD + archivo CSV
│   ├── Main.java               ← menú de consola
│   └── InventoryWindow.java    ← ventana Swing
├── test/Tests.java             ← pruebas sin framework
└── products.csv                ← los datos (se crea solo)
```

**Regla de diseño:** la lógica (`Product`, `ProductRepository`, `Report`) **no sabe** si la interfaz es consola o ventana. Por eso la misma lógica sirve para las dos y se puede probar sin teclado.

## 2. El dato: `record` con validación

Un `record` es una clase inmutable de datos: Java genera constructor, `equals`, `hashCode`, `toString` y los métodos de acceso (`name()`, `price()`…). El **constructor compacto** (sin paréntesis) valida: si algo es inválido lanza `IllegalArgumentException` y **nunca existe un `Product` inválido**.

```java
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
```

| Pieza | Significado |
|---|---|
| `BigDecimal` | Decimal exacto. Para dinero **nunca** `double` (`0.1 + 0.2 != 0.3`) |
| `strip()` | Quita espacios al inicio y al final (versión Unicode de `trim()`) |
| `throw new IllegalArgumentException` | Dato inválido: quien llama debe corregirlo |

## 3. Capturar datos: `Scanner` y bucles de reintento

`Scanner` lee la entrada estándar. Un buen formulario de consola **no se rompe** si escribes "abc" donde va un número: repite la pregunta. Además aquí: acepta coma decimal (`12,50`), y con **Enter vacío conserva el valor actual** al editar.

```java
package inventory;

import java.math.BigDecimal;
import java.util.Scanner;

/**
 * Captura de datos por consola. Cada método REPITE la pregunta hasta recibir un dato válido,
 * así el programa nunca se rompe por lo que escriba el usuario.
 */
public class Input {

    private final Scanner scanner;

    public Input(Scanner scanner) {
        this.scanner = scanner;
    }

    /** Lee una línea de texto; si el usuario no escribe nada y hay valor por defecto, lo usa. */
    public String text(String prompt, String defaultValue) {
        System.out.print(defaultValue == null ? prompt + ": " : prompt + " [" + defaultValue + "]: ");
        String line = readLine().strip();
        return line.isEmpty() && defaultValue != null ? defaultValue : line;
    }

    /** Texto obligatorio: insiste mientras esté vacío. */
    public String requiredText(String prompt) {
        while (true) {
            String value = text(prompt, null);
            if (!value.isEmpty()) {
                return value;
            }
            System.out.println("  Este dato es obligatorio.");
        }
    }

    /** Entero dentro de un rango. */
    public int integer(String prompt, int min, int max) {
        return integer(prompt, min, max, null);
    }

    /** Entero dentro de un rango; si el usuario pulsa Enter y hay valor por defecto, lo usa. */
    public int integer(String prompt, int min, int max, Integer defaultValue) {
        while (true) {
            String raw = text(prompt, defaultValue == null ? null : String.valueOf(defaultValue));
            try {
                int value = Integer.parseInt(raw);
                if (value >= min && value <= max) {
                    return value;
                }
                System.out.printf("  Debe estar entre %d y %d.%n", min, max);
            } catch (NumberFormatException e) {
                System.out.println("  Escribe un número entero (por ejemplo 12).");
            }
        }
    }

    /** Número decimal; acepta coma o punto como separador. */
    public BigDecimal decimal(String prompt) {
        return decimal(prompt, null);
    }

    /** Número decimal con valor por defecto (Enter lo mantiene). */
    public BigDecimal decimal(String prompt, BigDecimal defaultValue) {
        while (true) {
            String raw = text(prompt, defaultValue == null ? null : defaultValue.toPlainString()).replace(',', '.');
            try {
                BigDecimal value = new BigDecimal(raw);
                if (value.signum() >= 0) {
                    return value;
                }
                System.out.println("  No puede ser negativo.");
            } catch (NumberFormatException e) {
                System.out.println("  Escribe un número (por ejemplo 12,50).");
            }
        }
    }

    /** Pregunta sí/no. */
    public boolean confirm(String prompt) {
        while (true) {
            String answer = text(prompt + " (s/n)", null).toLowerCase();
            if (answer.equals("s") || answer.equals("si") || answer.equals("sí")) {
                return true;
            }
            if (answer.equals("n") || answer.equals("no")) {
                return false;
            }
            System.out.println("  Responde s o n.");
        }
    }

    private String readLine() {
        if (!scanner.hasNextLine()) {
            // Fin de la entrada (Ctrl+D o tubería agotada): salir limpiamente
            System.out.println();
            throw new EndOfInputException();
        }
        return scanner.nextLine();
    }

    /** Se lanza cuando ya no hay más datos que leer. */
    @SuppressWarnings("serial")
    public static class EndOfInputException extends RuntimeException {
    }
}
```

| Pieza | Significado |
|---|---|
| `scanner.nextLine()` | Lee una línea entera. **No uses `nextInt()`**: deja el salto de línea sin leer y la siguiente pregunta se salta sola |
| `Integer.parseInt(s)` | Convierte texto a `int`; lanza `NumberFormatException` si no es número |
| `new BigDecimal(s)` | Igual para decimales exactos; se usa `replace(',', '.')` antes |
| Valor por defecto | Si pulsas Enter y hay valor actual, se mantiene |
| `EndOfInputException` | Se lanza si la entrada se cierra (Ctrl+D o `<` desde archivo): evita un bucle infinito |

## 4. Mostrar datos: tablas alineadas

`String.format` con anchos fijos alinea columnas. Separamos «dibujar tabla» de «qué datos mostrar».

```java
package inventory;

import java.util.ArrayList;
import java.util.List;

/** Dibuja una tabla de texto con las columnas alineadas, calculando el ancho de cada una. */
public class Table {

    private final List<String> headers;
    private final List<Boolean> rightAligned;
    private final List<List<String>> rows = new ArrayList<>();

    public Table(List<String> headers, List<Boolean> rightAligned) {
        this.headers = headers;
        this.rightAligned = rightAligned;
    }

    public void addRow(List<String> cells) {
        rows.add(cells);
    }

    @Override
    public String toString() {
        int columns = headers.size();
        int[] widths = new int[columns];
        for (int c = 0; c < columns; c++) {
            widths[c] = headers.get(c).length();
            for (List<String> row : rows) {
                widths[c] = Math.max(widths[c], row.get(c).length());
            }
        }
        StringBuilder out = new StringBuilder();
        appendRow(out, headers, widths);
        for (int c = 0; c < columns; c++) {
            out.append(c == 0 ? "" : "-+-").append("-".repeat(widths[c]));
        }
        out.append(System.lineSeparator());
        for (List<String> row : rows) {
            appendRow(out, row, widths);
        }
        return out.toString();
    }

    private void appendRow(StringBuilder out, List<String> cells, int[] widths) {
        for (int c = 0; c < cells.size(); c++) {
            if (c > 0) {
                out.append(" | ");
            }
            // %-Ns alinea a la izquierda en N caracteres; %Ns a la derecha
            String format = rightAligned.get(c) ? "%" + widths[c] + "s" : "%-" + widths[c] + "s";
            out.append(String.format(format, cells.get(c)));
        }
        out.append(System.lineSeparator());
    }
}
```

```java
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
```

| Formato | Significado |
|---|---|
| `%-20s` | Texto, alineado a la **izquierda** (`-`), ancho 20 |
| `%8s` | Texto alineado a la derecha, ancho 8 |
| `"%" + ancho + "s"` | Así `Table` construye el formato según la columna: izquierda para texto, derecha para números |
| `Collectors.groupingBy(...)` | Agrupa por categoría (como `GROUP BY` de SQL) |
| `Collectors.reducing(BigDecimal.ZERO, Product::totalValue, BigDecimal::add)` | Suma los valores dentro de cada grupo |

## 5. Gestionar datos: repositorio + CSV

Un **repositorio** es la clase que guarda y recupera datos; el resto del programa no sabe si es una lista, un CSV o una base de datos. Aquí: una `List` en memoria y guardado/lectura en CSV (`id,name,category,price,stock`).

```java
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
```

| Pieza | Significado |
|---|---|
| `Files.readAllLines(path, UTF_8)` / `Files.write` | Leer/escribir texto indicando **siempre** la codificación |
| `Files.write(file, lines, UTF_8)` | Escribe todas las líneas de una vez (crea o sobrescribe el archivo) |
| `stream().filter(...)` | Buscar sin escribir bucles |
| `Optional<Product>` | «Puede que no exista»: obliga a decidir qué hacer en ese caso |
| Comillas en CSV | Si un campo lleva `,` o comillas, se entrecomilla y las comillas se duplican |

## 6. El menú de consola

```java
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
```

Lo importante: `while (true)` + `switch` es el patrón de **cualquier menú**. El `switch` con `->` (Java 14+) no necesita `break`.

## 7. Compilar y ejecutar, comando a comando

```bash
cd inventory-java
javac -Xlint:all -d out src/inventory/*.java
java -Dstdout.encoding=UTF-8 -cp out inventory.Main products.csv
```

| Pieza | Significado |
|---|---|
| `javac` | El compilador de Java: convierte `.java` en `.class` |
| `-Xlint:all` | Activa **todos** los avisos del compilador (código dudoso, conversiones inseguras…). Siempre conviene |
| `-d out` | *Destination*: carpeta donde dejar los `.class`. Sin ella los deja junto a los fuentes |
| `src/inventory/*.java` | Todos los fuentes del paquete (el shell expande el `*`) |
| `java` | Lanza la máquina virtual y ejecuta una clase |
| `-Dstdout.encoding=UTF-8` | Define una propiedad del sistema: la salida usa UTF-8. **Sin esto, en algunos terminales los acentos salen como `?`** |
| `-cp out` | *Classpath*: dónde buscar los `.class` (`-classpath` es lo mismo) |
| `inventory.Main` | Nombre **completo** de la clase con `main`: paquete + clase, **sin** `.class` |
| `products.csv` | Primer argumento (`args[0]` en `main`): el archivo de datos |

Probar sin teclear (útil para scripts y pruebas): se redirige la entrada con `<<`:

```bash
java -Dstdout.encoding=UTF-8 -cp out inventory.Main demo.csv <<'INPUT'
1
Teclado
Periféricos
19,90
12
2
0
INPUT
```

`<<'INPUT'` es un *here-document*: lo que hay hasta la línea `INPUT` se entrega como si lo escribieras tú. Las comillas simples impiden que el shell interprete `$` dentro. Cada línea responde a una pregunta, en orden: opción 1, nombre, categoría, precio, stock, opción 2 (listar), opción 0 (salir).

## 8. Ventana gráfica con Swing

Swing viene con el JDK. Una `JTable` muestra datos a partir de un **modelo** (`AbstractTableModel`): la tabla pregunta al modelo cuántas filas hay y qué va en cada celda; no guarda los datos ella.

```java
package inventory;

import java.awt.BorderLayout;
import java.awt.Dimension;
import java.awt.FlowLayout;
import java.awt.GridBagConstraints;
import java.awt.GridBagLayout;
import java.awt.Insets;
import java.io.IOException;
import java.math.BigDecimal;
import java.nio.file.Path;
import java.util.List;
import javax.swing.JButton;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JOptionPane;
import javax.swing.JPanel;
import javax.swing.JScrollPane;
import javax.swing.JTable;
import javax.swing.JTextField;
import javax.swing.SwingConstants;
import javax.swing.SwingUtilities;
import javax.swing.table.AbstractTableModel;
import javax.swing.table.DefaultTableCellRenderer;

/**
 * Interfaz gráfica con Swing (incluido en el JDK, sin dependencias):
 * un formulario para CAPTURAR datos, una tabla para MOSTRARLOS y botones para GESTIONARLOS.
 */
// Los componentes de Swing son «serializables» por historia; no los guardamos, así que se silencia el aviso
@SuppressWarnings({"serial", "this-escape"})
public class InventoryWindow extends JFrame {

    private final ProductRepository repository;
    private final Path dataFile;
    private final ProductTableModel tableModel = new ProductTableModel();
    private final JTable table = new JTable(tableModel);

    // Campos del formulario
    private final JTextField nameField = new JTextField(16);
    private final JTextField categoryField = new JTextField(12);
    private final JTextField priceField = new JTextField(8);
    private final JTextField stockField = new JTextField(6);
    private final JLabel statusLabel = new JLabel(" ");
    private final JButton addButton = new JButton("Añadir");
    private final JButton deleteButton = new JButton("Borrar seleccionado");
    private final JButton saveButton = new JButton("Guardar");

    public InventoryWindow(ProductRepository repository, Path dataFile) {
        super("Inventario");
        this.repository = repository;
        this.dataFile = dataFile;

        setDefaultCloseOperation(EXIT_ON_CLOSE);
        setLayout(new BorderLayout(8, 8));
        add(buildForm(), BorderLayout.NORTH);
        add(new JScrollPane(buildTable()), BorderLayout.CENTER);
        add(buildStatusBar(), BorderLayout.SOUTH);

        // Los botones llaman a métodos: la lógica no está mezclada con el dibujo
        addButton.addActionListener(event -> addFromForm());
        deleteButton.addActionListener(event -> deleteSelected());
        saveButton.addActionListener(event -> save());

        refresh();
        setPreferredSize(new Dimension(760, 460));
        pack();
        setLocationRelativeTo(null);
    }

    private JPanel buildForm() {
        JPanel form = new JPanel(new GridBagLayout());
        GridBagConstraints c = new GridBagConstraints();
        c.insets = new Insets(6, 6, 6, 6);
        c.anchor = GridBagConstraints.WEST;
        String[] labels = {"Nombre", "Categoría", "Precio (€)", "Stock"};
        JTextField[] fields = {nameField, categoryField, priceField, stockField};
        for (int i = 0; i < labels.length; i++) {
            c.gridx = i;
            c.gridy = 0;
            form.add(new JLabel(labels[i]), c);
            c.gridy = 1;
            form.add(fields[i], c);
        }
        c.gridx = labels.length;
        form.add(addButton, c);
        return form;
    }

    private JTable buildTable() {
        table.setAutoCreateRowSorter(true); // clic en la cabecera para ordenar
        table.setFillsViewportHeight(true);
        DefaultTableCellRenderer right = new DefaultTableCellRenderer();
        right.setHorizontalAlignment(SwingConstants.RIGHT);
        for (int column : new int[] {0, 3, 4, 5}) {
            table.getColumnModel().getColumn(column).setCellRenderer(right);
        }
        table.getColumnModel().getColumn(0).setMaxWidth(50);
        return table;
    }

    private JPanel buildStatusBar() {
        JPanel bar = new JPanel(new FlowLayout(FlowLayout.LEFT));
        bar.add(deleteButton);
        bar.add(saveButton);
        bar.add(statusLabel);
        return bar;
    }

    // ───────────── Acciones ─────────────

    /** Lee el formulario, valida y añade. Los errores se muestran en pantalla, nunca se pierden. */
    public void addFromForm() {
        try {
            BigDecimal price = new BigDecimal(priceField.getText().strip().replace(',', '.'));
            int stock = Integer.parseInt(stockField.getText().strip());
            repository.add(nameField.getText(), categoryField.getText(), price, stock);
        } catch (NumberFormatException e) {
            showError("El precio debe ser un número (12,50) y el stock un entero.");
            return;
        } catch (IllegalArgumentException e) {
            showError(e.getMessage()); // mensajes de validación de Product
            return;
        }
        for (JTextField field : new JTextField[] {nameField, categoryField, priceField, stockField}) {
            field.setText("");
        }
        nameField.requestFocusInWindow();
        refresh();
        statusLabel.setText("Producto añadido.");
    }

    public void deleteSelected() {
        int viewRow = table.getSelectedRow();
        if (viewRow < 0) {
            showError("Selecciona primero una fila de la tabla.");
            return;
        }
        // Con el orden de la tabla alterado, la fila visible no coincide con la del modelo
        int modelRow = table.convertRowIndexToModel(viewRow);
        Product product = tableModel.productAt(modelRow);
        int answer = JOptionPane.showConfirmDialog(
                this, "¿Borrar \"" + product.name() + "\"?", "Confirmar", JOptionPane.YES_NO_OPTION);
        if (answer == JOptionPane.YES_OPTION) {
            repository.delete(product.id());
            refresh();
            statusLabel.setText("Producto borrado.");
        }
    }

    public void save() {
        try {
            repository.save(dataFile);
            statusLabel.setText("Guardado en " + dataFile);
        } catch (IOException e) {
            showError("No se pudo guardar: " + e.getMessage());
        }
    }

    private void refresh() {
        tableModel.setProducts(repository.findAll());
        statusLabel.setText(tableModel.getRowCount() + " productos");
    }

    private void showError(String message) {
        JOptionPane.showMessageDialog(this, message, "Datos no válidos", JOptionPane.WARNING_MESSAGE);
    }

    // ───────────── Acceso para pruebas ─────────────

    public void fillForm(String name, String category, String price, String stock) {
        nameField.setText(name);
        categoryField.setText(category);
        priceField.setText(price);
        stockField.setText(stock);
    }

    public int rowCount() {
        return tableModel.getRowCount();
    }

    // ───────────── Modelo de la tabla ─────────────

    /** Adapta la lista de productos a lo que necesita JTable: filas, columnas y valores. */
    @SuppressWarnings("serial")
    private static class ProductTableModel extends AbstractTableModel {

        private static final String[] COLUMNS = {"ID", "Nombre", "Categoría", "Precio", "Stock", "Valor"};
        private List<Product> products = List.of();

        void setProducts(List<Product> products) {
            this.products = products;
            fireTableDataChanged(); // avisa a la tabla de que debe redibujarse
        }

        Product productAt(int row) {
            return products.get(row);
        }

        @Override
        public int getRowCount() {
            return products.size();
        }

        @Override
        public int getColumnCount() {
            return COLUMNS.length;
        }

        @Override
        public String getColumnName(int column) {
            return COLUMNS[column];
        }

        @Override
        public Object getValueAt(int row, int column) {
            Product p = products.get(row);
            return switch (column) {
                case 0 -> p.id();
                case 1 -> p.name();
                case 2 -> p.category();
                case 3 -> Report.money(p.price());
                case 4 -> p.stock();
                case 5 -> Report.money(p.totalValue());
                default -> throw new IllegalArgumentException("Columna " + column);
            };
        }
    }

    // ───────────── Arranque ─────────────

    public static void main(String[] args) throws IOException {
        Path file = Path.of(args.length > 0 ? args[0] : "products.csv");
        ProductRepository repository = new ProductRepository();
        repository.load(file);
        // Swing se usa SIEMPRE desde su hilo propio (el hilo de eventos)
        SwingUtilities.invokeLater(() -> new InventoryWindow(repository, file).setVisible(true));
    }
}
```

| Pieza | Significado |
|---|---|
| `SwingUtilities.invokeLater` | Toda la interfaz Swing debe crearse en el **hilo de eventos**; esto lo garantiza |
| `AbstractTableModel` | Contrato entre tus datos y la `JTable`: `getRowCount`, `getColumnCount`, `getValueAt` |
| `fireTableDataChanged()` | Avisa a la tabla de que cambiaron los datos y debe repintarse |
| `JOptionPane.showMessageDialog` | Ventana emergente para errores y avisos |
| Mismo `ProductRepository` | La ventana reutiliza la lógica de la consola sin cambios |

```bash
java -cp out inventory.InventoryWindow products.csv
```

(No lleva `-Dstdout.encoding` porque no imprime en terminal.)

## 9. Pruebas sin JUnit

JUnit no es obligatorio. Una prueba es código que llama a tu código y comprueba el resultado. Con un `main` y una función `check` basta para empezar:

```java
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
```

```bash
javac -cp out -d test-out test/Tests.java
java -Dstdout.encoding=UTF-8 -cp out:test-out Tests
```

| Pieza | Significado |
|---|---|
| `-cp out:test-out` | Dos carpetas en el classpath, separadas por `:` (en Windows se separa con `;`) |
| `Tests` | Clase sin paquete: se llama por su nombre simple |

Salida esperada: seis líneas «OK» y `6 pruebas, 0 fallos`.

## 10. Errores típicos

| Síntoma | Causa | Solución |
|---|---|---|
| Acentos como `?` | Codificación del terminal | `-Dstdout.encoding=UTF-8` |
| La pregunta siguiente se salta sola | Mezclar `nextInt()` con `nextLine()` | Usar solo `nextLine()` y convertir |
| `NoSuchElementException` | Entrada cerrada (Ctrl+D) | Capturarlo (ver `EndOfInputException`) |
| `Could not find or load main class` | Nombre sin paquete o `-cp` mal | `inventory.Main` y `-cp out` |
| Decimal que no se parsea | Coma en vez de punto | Normalizar con `replace(',', '.')` |
| `HeadlessException` | Sin escritorio gráfico (SSH) | Ejecutar la ventana en un entorno con pantalla |

## 11. Ejercicios

1. Añade un campo `supplier` (proveedor) al `record` y al CSV. ¿Qué pasa con un CSV antiguo sin esa columna? Hazlo compatible.
2. Añade la opción **«Ordenar por»** precio, nombre o stock (`Comparator.comparing`).
3. Añade un **informe de bajo stock**: productos con stock menor que un umbral que pide el usuario.
4. Cambia el repositorio para guardar en **JSON** a mano (sin librerías): escribe y lee el formato tú.
5. Añade **deshacer** del último borrado (guarda el producto borrado en una variable).
6. En la ventana Swing, añade un campo de **búsqueda** que filtre la tabla al escribir.
7. Escribe tres pruebas más: precio negativo, nombre vacío, id inexistente al editar.
8. Reescribe `Main` para aceptar **argumentos**: `list`, `add "Nombre" "Cat" 9.90 5`, sin menú interactivo.
