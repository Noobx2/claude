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
