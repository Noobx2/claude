using System.ComponentModel;
using System.Globalization;

namespace Inventory;

/// <summary>
/// Interfaz gráfica con Windows Forms: un formulario para CAPTURAR datos,
/// una tabla (DataGridView) para MOSTRARLOS y botones para GESTIONARLOS.
/// </summary>
public sealed class MainForm : Form
{
    private readonly ProductRepository _repository;
    private readonly string _dataFile;

    // BindingList avisa a la tabla cada vez que cambia: no hay que redibujar a mano
    private readonly BindingList<ProductRow> _rows = [];
    private readonly DataGridView _grid = new() { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
    private readonly TextBox _name = new() { Width = 150 };
    private readonly TextBox _category = new() { Width = 110 };
    private readonly TextBox _price = new() { Width = 70 };
    private readonly NumericUpDown _stock = new() { Width = 70, Maximum = 1_000_000 };
    private readonly ErrorProvider _errors = new();
    private readonly Label _status = new() { AutoSize = true, Padding = new Padding(8, 6, 0, 0) };

    public MainForm(ProductRepository repository, string dataFile)
    {
        _repository = repository;
        _dataFile = dataFile;

        Text = "Inventario";
        ClientSize = new Size(760, 440);
        StartPosition = FormStartPosition.CenterScreen;

        var add = new Button { Text = "Añadir", AutoSize = true };
        var delete = new Button { Text = "Borrar seleccionado", AutoSize = true };
        var save = new Button { Text = "Guardar", AutoSize = true };
        add.Click += (_, _) => AddFromForm();
        delete.Click += (_, _) => DeleteSelected();
        save.Click += (_, _) => Save();

        var form = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(8) };
        form.Controls.AddRange([Labeled("Nombre", _name), Labeled("Categoría", _category), Labeled("Precio (€)", _price), Labeled("Stock", _stock), add]);

        var bar = new FlowLayoutPanel { Dock = DockStyle.Bottom, AutoSize = true, Padding = new Padding(8) };
        bar.Controls.AddRange([delete, save, _status]);

        _grid.DataSource = _rows;
        Controls.Add(_grid);       // el orden importa: lo que se añade antes queda debajo
        Controls.Add(bar);
        Controls.Add(form);
        AcceptButton = add;        // Enter en el formulario = pulsar «Añadir»

        Refresh(_repository.FindAll());
    }

    private static Control Labeled(string text, Control control)
    {
        var panel = new FlowLayoutPanel { FlowDirection = FlowDirection.TopDown, AutoSize = true };
        panel.Controls.Add(new Label { Text = text, AutoSize = true });
        panel.Controls.Add(control);
        return panel;
    }

    // ───────────── Acciones ─────────────

    private void AddFromForm()
    {
        _errors.Clear();
        string priceText = _price.Text.Trim().Replace(',', '.');
        if (!decimal.TryParse(priceText, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal price))
        {
            _errors.SetError(_price, "Escribe un número, por ejemplo 12,50");   // icono rojo junto al campo
            return;
        }
        try
        {
            _repository.Add(_name.Text, _category.Text, price, (int)_stock.Value);
        }
        catch (ArgumentException e)
        {
            MessageBox.Show(this, e.Message, "Datos no válidos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        _name.Clear(); _category.Clear(); _price.Clear(); _stock.Value = 0;
        _name.Focus();
        Refresh(_repository.FindAll());
        _status.Text = "Producto añadido.";
    }

    private void DeleteSelected()
    {
        if (_grid.CurrentRow?.DataBoundItem is not ProductRow row)
        {
            MessageBox.Show(this, "Selecciona primero una fila de la tabla.", "Borrar", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (MessageBox.Show(this, $"¿Borrar \"{row.Nombre}\"?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            _repository.Delete(row.Id);
            Refresh(_repository.FindAll());
            _status.Text = "Producto borrado.";
        }
    }

    private void Save()
    {
        try
        {
            _repository.Save(_dataFile);
            _status.Text = $"Guardado en {_dataFile}";
        }
        catch (IOException e)
        {
            MessageBox.Show(this, $"No se pudo guardar: {e.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void Refresh(IReadOnlyList<Product> products)
    {
        _rows.Clear();
        foreach (var p in products)
        {
            _rows.Add(new ProductRow(p.Id, p.Name, p.Category, Report.Money(p.Price), p.Stock, Report.Money(p.TotalValue)));
        }
        _status.Text = $"{_rows.Count} productos";
    }

    /// <summary>Lo que muestra la tabla: las propiedades públicas se convierten en columnas.</summary>
    private sealed record ProductRow(int Id, string Nombre, string Categoria, string Precio, int Stock, string Valor);
}
