# C# sin frameworks: capturar, mostrar y gestionar datos

Todo lo de este módulo usa **solo .NET (la biblioteca base)**: sin ASP.NET, sin Entity Framework, sin paquetes salvo xUnit para las pruebas. Construimos el mismo **inventario de productos** que en Java:

1. **Captura** de datos con reintento si son inválidos.
2. **Mostrar** en tablas alineadas y resúmenes (LINQ).
3. **Gestionar**: añadir, buscar, editar, borrar, guardar en JSON y exportar a CSV.
4. **Ventana** con Windows Forms (el anfitrión Windows con Visual Studio).
5. **Pruebas** con xUnit.

> <span class="tag ok">comprobado</span> Compilado con el SDK de .NET 10 y probado: la sesión de consola con entrada simulada funciona y **13 pruebas pasan**. <span class="tag warn">limitado</span> El proyecto de Windows Forms **se compila** en Linux (con `EnableWindowsTargeting`) pero **no se puede ejecutar** aquí: ábrelo en Visual Studio en tu Windows. <span class="tag bad">en tu VM</span> `dotnet` no está instalado ahí todavía; en tu Windows, Visual Studio ya lo trae.

## 1. Estructura y por qué

```text
Inventory.slnx                         ← la solución (agrupa proyectos)
src/
├── Inventory.Core/                    ← lógica: no sabe nada de pantallas
│   ├── Product.cs
│   ├── ProductRepository.cs
│   ├── Report.cs
│   └── TablePrinter.cs
├── Inventory.Console/                 ← interfaz de consola
│   ├── ConsoleInput.cs
│   ├── App.cs
│   └── Program.cs
└── Inventory.WinForms/                ← interfaz de ventana (solo Windows)
    ├── MainForm.cs
    └── Program.cs
tests/Inventory.Tests/                 ← pruebas xUnit
```

**Un proyecto por responsabilidad**: `Core` lo comparten consola, ventana y pruebas. Si mañana quieres una web o una app móvil, reutilizas `Core`.

## 2. Crear la solución desde cero, comando a comando

```bash
mkdir inventory-cs && cd inventory-cs
dotnet new sln -n Inventory
dotnet new classlib -n Inventory.Core    -o src/Inventory.Core
dotnet new console  -n Inventory.Console -o src/Inventory.Console
dotnet new winforms -n Inventory.WinForms -o src/Inventory.WinForms
dotnet new xunit    -n Inventory.Tests   -o tests/Inventory.Tests
dotnet sln add src/Inventory.Core src/Inventory.Console src/Inventory.WinForms tests/Inventory.Tests
dotnet add src/Inventory.Console reference src/Inventory.Core
dotnet add src/Inventory.WinForms reference src/Inventory.Core
dotnet add tests/Inventory.Tests reference src/Inventory.Core src/Inventory.Console
```

| Pieza | Significado |
|---|---|
| `dotnet new sln` | Crea la solución (`.sln` o `.slnx` en .NET 10): un contenedor de proyectos |
| `-n Inventory` | *Name*: nombre del proyecto/solución (también el espacio de nombres base) |
| `dotnet new classlib` | Plantilla de **biblioteca**: código reutilizable sin `Main` |
| `dotnet new console` | Plantilla de **programa de consola** |
| `dotnet new winforms` | Plantilla de aplicación de ventanas (solo se ejecuta en Windows) |
| `dotnet new xunit` | Plantilla de proyecto de pruebas con xUnit |
| `-o src/Inventory.Core` | *Output*: carpeta donde crear el proyecto |
| `dotnet sln add ...` | Registra proyectos en la solución |
| `dotnet add A reference B` | Hace que el proyecto A use el proyecto B (referencia entre proyectos) |

## 3. El dato: `record` sellado con validación

`record` da igualdad por valores, `ToString` útil e inmutabilidad (`init`). El constructor valida: **no puede existir un `Product` inválido**.

```csharp
using System.Text.Json.Serialization;

namespace Inventory;

/// <summary>
/// Un producto del inventario. Es inmutable (solo lectura) y se valida al crearlo,
/// así es imposible tener un producto inválido en memoria.
/// </summary>
public sealed record Product
{
    public int Id { get; }
    public string Name { get; }
    public string Category { get; }
    public decimal Price { get; }
    public int Stock { get; }

    public Product(int id, string name, string category, decimal price, int stock)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("El nombre no puede estar vacío", nameof(name));
        if (string.IsNullOrWhiteSpace(category)) throw new ArgumentException("La categoría no puede estar vacía", nameof(category));
        if (price < 0) throw new ArgumentException("El precio no puede ser negativo", nameof(price));
        if (stock < 0) throw new ArgumentException("El stock no puede ser negativo", nameof(stock));

        Id = id;
        Name = name.Trim();
        Category = category.Trim();
        Price = price;
        Stock = stock;
    }

    /// <summary>Valor total de este producto en almacén. Es un cálculo: no se guarda en el archivo.</summary>
    [JsonIgnore]
    public decimal TotalValue => Price * Stock;
}
```

| Pieza | Significado |
|---|---|
| `sealed` | Nadie puede heredar de la clase: más simple y rápida |
| `decimal` | Decimal exacto, el tipo para **dinero** (nunca `double`) |
| `[JsonIgnore]` | `TotalValue` es calculado; no se guarda en el JSON |
| `ArgumentException` | Argumento inválido: quien llama debe corregirlo |

## 4. Capturar datos: `TextReader` / `TextWriter`

`ConsoleInput` no usa `Console` directamente: recibe un **lector** y un **escritor**. En el programa real son la consola; en las pruebas son cadenas en memoria. Así se puede probar sin teclear.

```csharp
using System.Globalization;

namespace Inventory;

/// <summary>Se lanza cuando ya no hay más datos que leer (Ctrl+D / Ctrl+Z o tubería agotada).</summary>
public sealed class EndOfInputException : Exception;

/// <summary>
/// Captura de datos por consola. Cada método REPITE la pregunta hasta recibir un dato válido,
/// así el programa nunca se rompe por lo que escriba el usuario.
/// </summary>
public sealed class ConsoleInput(TextReader reader, TextWriter writer)
{
    /// <summary>Lee una línea; si no se escribe nada y hay valor por defecto, lo usa.</summary>
    public string Text(string prompt, string? defaultValue = null)
    {
        writer.Write(defaultValue is null ? $"{prompt}: " : $"{prompt} [{defaultValue}]: ");
        string line = (reader.ReadLine() ?? throw new EndOfInputException()).Trim();
        return line.Length == 0 && defaultValue is not null ? defaultValue : line;
    }

    /// <summary>Texto obligatorio: insiste mientras esté vacío.</summary>
    public string Required(string prompt)
    {
        while (true)
        {
            string value = Text(prompt);
            if (value.Length > 0) return value;
            writer.WriteLine("  Este dato es obligatorio.");
        }
    }

    /// <summary>Entero dentro de un rango.</summary>
    public int Integer(string prompt, int min, int max, int? defaultValue = null)
    {
        while (true)
        {
            string raw = Text(prompt, defaultValue?.ToString(CultureInfo.InvariantCulture));
            // TryParse devuelve false en lugar de lanzar una excepción: ideal para datos del usuario
            if (!int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
            {
                writer.WriteLine("  Escribe un número entero (por ejemplo 12).");
            }
            else if (value < min || value > max)
            {
                writer.WriteLine($"  Debe estar entre {min} y {max}.");
            }
            else
            {
                return value;
            }
        }
    }

    /// <summary>Número decimal; acepta coma o punto como separador.</summary>
    public decimal Decimal(string prompt, decimal? defaultValue = null)
    {
        while (true)
        {
            string raw = Text(prompt, defaultValue?.ToString(CultureInfo.InvariantCulture)).Replace(',', '.');
            // InvariantCulture: el punto decimal es el mismo en cualquier ordenador, sin sorpresas de idioma
            if (!decimal.TryParse(raw, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal value))
            {
                writer.WriteLine("  Escribe un número (por ejemplo 12,50).");
            }
            else if (value < 0)
            {
                writer.WriteLine("  No puede ser negativo.");
            }
            else
            {
                return value;
            }
        }
    }

    /// <summary>Pregunta sí/no.</summary>
    public bool Confirm(string prompt)
    {
        while (true)
        {
            switch (Text($"{prompt} (s/n)").ToLowerInvariant())
            {
                case "s" or "si" or "sí": return true;
                case "n" or "no": return false;
                default: writer.WriteLine("  Responde s o n."); break;
            }
        }
    }
}
```

| Pieza | Significado |
|---|---|
| `reader.ReadLine()` | Lee una línea; devuelve `null` si se cierra la entrada (Ctrl+D / Ctrl+Z) |
| `int.TryParse(s, out var n)` | Convierte **sin lanzar excepción**: devuelve `true/false`. Mejor que `int.Parse` para entradas de usuario |
| `NumberStyles` + `CultureInfo.InvariantCulture` | Fija cómo se interpreta el número. Sin cultura explícita, `12.5` o `12,5` depende del idioma del sistema |
| `s.Replace(',', '.')` | Acepta coma decimal del usuario |
| `EndOfInputException` | Propia: evita bucle infinito si no hay más entrada |

## 5. Mostrar: tablas y resumen con LINQ

```csharp
using System.Text;

namespace Inventory;

/// <summary>Dibuja una tabla de texto con las columnas alineadas, calculando el ancho de cada una.</summary>
public static class TablePrinter
{
    public static string Format(IReadOnlyList<string> headers, IReadOnlyList<bool> rightAligned, IEnumerable<IReadOnlyList<string>> rows)
    {
        var data = rows.ToList();
        // El ancho de cada columna es el del texto más largo (cabecera incluida)
        int[] widths = headers.Select((header, c) => Math.Max(header.Length, data.Select(row => row[c].Length).DefaultIfEmpty(0).Max())).ToArray();

        var output = new StringBuilder();
        AppendRow(output, headers, widths, rightAligned);
        output.AppendLine(string.Join("-+-", widths.Select(width => new string('-', width))));
        foreach (var row in data) AppendRow(output, row, widths, rightAligned);
        return output.ToString();
    }

    private static void AppendRow(StringBuilder output, IReadOnlyList<string> cells, int[] widths, IReadOnlyList<bool> rightAligned)
    {
        var padded = cells.Select((cell, c) => rightAligned[c] ? cell.PadLeft(widths[c]) : cell.PadRight(widths[c]));
        output.AppendLine(string.Join(" | ", padded));
    }
}
```

```csharp
using System.Globalization;

namespace Inventory;

/// <summary>Cálculos y presentación de datos: tablas y resúmenes.</summary>
public static class Report
{
    private static readonly CultureInfo Spanish = CultureInfo.GetCultureInfo("es-ES");

    /// <summary>Dinero con dos decimales y coma decimal (estilo español).</summary>
    public static string Money(decimal value) => value.ToString("N2", Spanish) + " €";

    public static string ProductTable(IEnumerable<Product> products) =>
        TablePrinter.Format(
            ["ID", "Nombre", "Categoría", "Precio", "Stock", "Valor"],
            [true, false, false, true, true, true],
            products.Select(p => (IReadOnlyList<string>)[p.Id.ToString(), p.Name, p.Category, Money(p.Price), p.Stock.ToString(), Money(p.TotalValue)]));

    public static string Summary(IReadOnlyList<Product> products)
    {
        var lines = new List<string>
        {
            $"Productos: {products.Count} | Unidades: {products.Sum(p => p.Stock)} | Valor total: {Money(products.Sum(p => p.TotalValue))}",
        };
        // LINQ: agrupar por categoría y sumar el valor de cada grupo
        foreach (var group in products.GroupBy(p => p.Category).OrderBy(g => g.Key))
        {
            lines.Add($"  {group.Key,-15} {Money(group.Sum(p => p.TotalValue)),12}");   // {x,-15}: alinear a la izquierda en 15; {x,12}: a la derecha en 12
        }
        return string.Join(Environment.NewLine, lines) + Environment.NewLine;
    }
}
```

| Pieza | Significado |
|---|---|
| `PadRight(n)` / `PadLeft(n)` | Rellena con espacios a izquierda/derecha hasta `n` caracteres: alinea columnas |
| `GroupBy(p => p.Category)` | Agrupa (como `GROUP BY`) |
| `Sum`, `OrderBy` | LINQ: consultas sobre colecciones sin bucles |
| `{x,-15}` / `{x,12}` | En cadenas interpoladas: alinear a la izquierda en 15 / a la derecha en 12 |
| `CultureInfo.GetCultureInfo("es-ES")` | Formatea dinero como `1.234,50 €`, independiente del sistema |

## 6. Gestionar: repositorio, JSON y CSV

```csharp
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Inventory;

/// <summary>
/// Gestiona los datos: los guarda en memoria y los persiste en JSON (y los exporta a CSV).
/// Solo usa la biblioteca base de .NET: sin NuGet, sin frameworks.
/// </summary>
public sealed class ProductRepository
{
    private readonly List<Product> _products = [];
    private int _lastId;

    // WriteIndented: JSON legible. UnsafeRelaxed: conserva tildes y € tal cual en lugar de \u00E9
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    // UTF-8 SIN marca de orden de bytes (BOM): lo habitual en archivos JSON
    private static readonly UTF8Encoding Utf8NoBom = new(encoderShouldEmitUTF8Identifier: false);

    // ───────────── Consultar ─────────────

    /// <summary>Devuelve una COPIA de solo lectura: quien la reciba no puede estropear la lista interna.</summary>
    public IReadOnlyList<Product> FindAll() => _products.ToList();

    public Product? FindById(int id) => _products.FirstOrDefault(p => p.Id == id);

    public IReadOnlyList<Product> Search(string text) =>
        _products
            .Where(p => p.Name.Contains(text, StringComparison.OrdinalIgnoreCase)
                     || p.Category.Contains(text, StringComparison.OrdinalIgnoreCase))
            .OrderBy(p => p.Name, StringComparer.CurrentCultureIgnoreCase)
            .ToList();

    // ───────────── Crear, modificar, borrar ─────────────

    public Product Add(string name, string category, decimal price, int stock)
    {
        var product = new Product(++_lastId, name, category, price, stock);
        _products.Add(product);
        return product;
    }

    public bool Update(Product updated)
    {
        int index = _products.FindIndex(p => p.Id == updated.Id);
        if (index < 0) return false;
        _products[index] = updated;
        return true;
    }

    public bool Delete(int id) => _products.RemoveAll(p => p.Id == id) > 0;

    // ───────────── Guardar y cargar (JSON) ─────────────

    public void Save(string path) => File.WriteAllText(path, JsonSerializer.Serialize(_products, JsonOptions), Utf8NoBom);

    public void Load(string path)
    {
        _products.Clear();
        _lastId = 0;
        if (!File.Exists(path)) return;   // primera ejecución: aún no hay archivo

        var loaded = JsonSerializer.Deserialize<List<Product>>(File.ReadAllText(path, Encoding.UTF8)) ?? [];
        _products.AddRange(loaded);
        _lastId = _products.Select(p => p.Id).DefaultIfEmpty(0).Max();
    }

    // ───────────── Exportar a CSV (para abrir en Excel) ─────────────

    public void ExportCsv(string path)
    {
        var lines = new List<string> { "id;name;category;price;stock" };   // «;» porque en español Excel usa la coma decimal
        lines.AddRange(_products.Select(p => string.Join(';',
            p.Id, Quote(p.Name), Quote(p.Category),
            p.Price.ToString(System.Globalization.CultureInfo.InvariantCulture), p.Stock)));
        File.WriteAllLines(path, lines, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));   // BOM: Excel reconoce UTF-8
    }

    internal static string Quote(string value) =>
        value.Contains(';') || value.Contains('"') || value.Contains('\n')
            ? $"\"{value.Replace("\"", "\"\"")}\""
            : value;
}
```

| Pieza | Significado |
|---|---|
| `JsonSerializer.Serialize / Deserialize` | `System.Text.Json`: objeto ↔ texto JSON |
| `WriteIndented = true` | JSON con sangrías (legible) |
| `JavaScriptEncoder.UnsafeRelaxedJsonEscaping` | No convierte `á` en `á`: el archivo queda legible. Es seguro para archivos locales |
| `new UTF8Encoding(false)` | UTF-8 **sin BOM** (el JSON no debe llevarlo) |
| CSV con `;` y BOM | Excel en español usa `;` y necesita el BOM para ver bien los acentos |
| `Path.ChangeExtension` | Cambia `products.json` por `products.csv` |

## 7. El menú de consola

```csharp
namespace Inventory;

/// <summary>Menú de consola: capturar, mostrar y gestionar los datos del inventario.</summary>
public sealed class App(ConsoleInput input, ProductRepository repository, string dataFile, TextWriter output)
{
    public void Run()
    {
        repository.Load(dataFile);
        output.WriteLine($"INVENTARIO ({repository.FindAll().Count} productos cargados de {dataFile})");
        try
        {
            while (true)
            {
                output.WriteLine();
                output.WriteLine("1) Añadir  2) Listar  3) Buscar  4) Editar  5) Borrar  6) Resumen  7) Exportar CSV  0) Guardar y salir");
                switch (input.Integer("Opción", 0, 7))
                {
                    case 1: AddProduct(); break;
                    case 2: ShowAll(); break;
                    case 3: Search(); break;
                    case 4: Edit(); break;
                    case 5: Delete(); break;
                    case 6: output.Write(Report.Summary(repository.FindAll())); break;
                    case 7: repository.ExportCsv(Path.ChangeExtension(dataFile, ".csv")); output.WriteLine("Exportado a " + Path.ChangeExtension(dataFile, ".csv")); break;
                    case 0:
                        repository.Save(dataFile);
                        output.WriteLine($"Guardado en {dataFile}. ¡Hasta luego!");
                        return;
                }
            }
        }
        catch (EndOfInputException)
        {
            repository.Save(dataFile);   // si se cierra la entrada, no se pierde lo hecho
        }
    }

    private void AddProduct()
    {
        string name = input.Required("Nombre");
        string category = input.Required("Categoría");
        decimal price = input.Decimal("Precio (€)");
        int stock = input.Integer("Stock", 0, 1_000_000);
        var product = repository.Add(name, category, price, stock);
        output.WriteLine($"Añadido con id {product.Id}");
    }

    private void ShowAll()
    {
        var all = repository.FindAll();
        output.Write(all.Count == 0 ? "No hay productos. Usa la opción 1 para añadir el primero." + Environment.NewLine : Report.ProductTable(all));
    }

    private void Search()
    {
        var found = repository.Search(input.Required("Texto a buscar (nombre o categoría)"));
        output.Write(found.Count == 0 ? "Sin resultados." + Environment.NewLine : Report.ProductTable(found));
    }

    private void Edit()
    {
        var old = repository.FindById(input.Integer("Id del producto", 1, int.MaxValue));
        if (old is null)
        {
            output.WriteLine("No existe ese producto.");
            return;
        }
        // Enter sin escribir nada mantiene el valor actual
        string name = input.Text("Nombre", old.Name);
        string category = input.Text("Categoría", old.Category);
        decimal price = input.Decimal("Precio (€)", old.Price);
        int stock = input.Integer("Stock", 0, 1_000_000, old.Stock);
        repository.Update(new Product(old.Id, name, category, price, stock));
        output.WriteLine("Actualizado.");
    }

    private void Delete()
    {
        var product = repository.FindById(input.Integer("Id del producto", 1, int.MaxValue));
        if (product is null) output.WriteLine("No existe ese producto.");
        else if (input.Confirm($"¿Borrar \"{product.Name}\"?"))
        {
            repository.Delete(product.Id);
            output.WriteLine("Borrado.");
        }
    }
}
```

```csharp
using System.Text;
using Inventory;

// Con UTF-8 se ven bien las tildes y el símbolo € en cualquier consola
Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

string file = args.Length > 0 ? args[0] : "products.json";
var app = new App(new ConsoleInput(Console.In, Console.Out), new ProductRepository(), file, Console.Out);
app.Run();
```

`public sealed class App(ConsoleInput input, ...)` es un **constructor primario** (C# 12): los parámetros están disponibles en toda la clase.

## 8. Ejecutar, comando a comando

```bash
dotnet build
dotnet run --project src/Inventory.Console -- products.json
```

| Pieza | Significado |
|---|---|
| `dotnet build` | Compila todos los proyectos de la solución |
| `dotnet run` | Compila (si hace falta) y ejecuta |
| `--project src/Inventory.Console` | Qué proyecto ejecutar (hay varios en la solución) |
| `--` | **Separador**: lo que va después son argumentos de **tu programa**, no de `dotnet` |
| `products.json` | Primer argumento (`args[0]`): el archivo de datos |

Con entrada simulada, útil para probar en scripts:

```bash
dotnet run --project src/Inventory.Console -- demo.json <<'INPUT'
1
Teclado
Periféricos
19,90
12
2
0
INPUT
```

## 9. Ventana con Windows Forms

Windows Forms es el sistema de ventanas clásico de .NET. Tres piezas clave: `DataGridView` (tabla), `BindingList<T>` (lista que avisa a la tabla cuando cambia) y `ErrorProvider` (icono rojo junto a un campo inválido).

```csharp
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
```

```csharp
using Inventory;

// Windows Forms necesita ejecutarse en un hilo «STA» y con esta inicialización
ApplicationConfiguration.Initialize();

string file = args.Length > 0 ? args[0] : "products.json";
var repository = new ProductRepository();
repository.Load(file);
Application.Run(new MainForm(repository, file));
```

El archivo de proyecto es lo que lo hace «solo Windows»:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <!-- "-windows": solo compila y se ejecuta en Windows -->
    <TargetFramework>net10.0-windows</TargetFramework>
    <UseWindowsForms>true</UseWindowsForms>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <RootNamespace>Inventory</RootNamespace>
    <!-- Permite COMPILAR este proyecto en Linux o macOS (no ejecutarlo) -->
    <EnableWindowsTargeting>true</EnableWindowsTargeting>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\Inventory.Core\Inventory.Core.csproj" />
  </ItemGroup>

</Project>
```

| Pieza | Significado |
|---|---|
| `net10.0-windows` | Destino que solo existe en Windows |
| `UseWindowsForms` | Activa la biblioteca de ventanas |
| `EnableWindowsTargeting` | Permite **compilar** (no ejecutar) el proyecto en Linux/macOS. Útil en integración continua |
| `ApplicationConfiguration.Initialize()` | Configura DPI, fuente y estilos visuales por defecto |
| `MessageBox.Show(..., YesNo)` | Ventana de confirmación; devuelve `DialogResult.Yes/No` |

En tu Windows: abre `Inventory.slnx` en Visual Studio, pon `Inventory.WinForms` como proyecto de inicio y pulsa F5. Desde Linux solo para comprobar que compila:

```bash
dotnet build src/Inventory.WinForms -p:EnableWindowsTargeting=true
```

(`-p:Nombre=valor` define una propiedad de MSBuild para esa compilación.)

## 10. Pruebas con xUnit

```csharp
using Inventory;

namespace Inventory.Tests;

public class ProductTests
{
    [Fact]
    public void ValidProductCalculatesTotalValue()
    {
        var product = new Product(1, "  Teclado ", "Periféricos", 25.50m, 10);
        Assert.Equal("Teclado", product.Name);          // se recorta
        Assert.Equal(255.00m, product.TotalValue);
    }

    [Theory]                                             // la misma prueba con varios datos
    [InlineData("", "x", 1, 1)]
    [InlineData("a", " ", 1, 1)]
    [InlineData("a", "x", -1, 1)]
    [InlineData("a", "x", 1, -1)]
    public void InvalidProductThrows(string name, string category, double price, int stock)
        => Assert.Throws<ArgumentException>(() => new Product(1, name, category, (decimal)price, stock));
}

public class ProductRepositoryTests
{
    [Fact]
    public void AddSearchUpdateDelete()
    {
        var repo = new ProductRepository();
        var keyboard = repo.Add("Teclado", "Periféricos", 25m, 3);
        repo.Add("Monitor", "Pantallas", 100m, 2);

        Assert.Equal(2, repo.FindAll().Count);
        Assert.Single(repo.Search("tecla"));              // no distingue mayúsculas
        Assert.True(repo.Update(new Product(keyboard.Id, "Teclado RGB", keyboard.Category, keyboard.Price, 5)));
        Assert.Equal(5, repo.FindById(keyboard.Id)!.Stock);
        Assert.True(repo.Delete(keyboard.Id));
        Assert.False(repo.Delete(keyboard.Id));           // la segunda vez ya no existe
    }

    [Fact]
    public void JsonRoundTripKeepsDataAndIds()
    {
        string path = Path.GetTempFileName();
        try
        {
            var repo = new ProductRepository();
            repo.Add("Cable \"premium\", 2 m", "Accesorios; varios", 3.5m, 100);
            repo.Save(path);

            string text = File.ReadAllText(path);
            Assert.DoesNotContain("TotalValue", text);    // los cálculos no se guardan
            Assert.Contains("Accesorios; varios", text);

            var loaded = new ProductRepository();
            loaded.Load(path);
            var product = Assert.Single(loaded.FindAll());
            Assert.Equal("Cable \"premium\", 2 m", product.Name);
            Assert.Equal(2, loaded.Add("Nuevo", "x", 1m, 1).Id);   // el siguiente id continúa
        }
        finally
        {
            File.Delete(path);
        }
    }
}

public class ConsoleInputTests
{
    private static ConsoleInput Input(string text) => new(new StringReader(text), TextWriter.Null);

    [Fact]
    public void IntegerRepeatsUntilValid()
        => Assert.Equal(7, Input("abc\n-5\n99\n7\n").Integer("n", 1, 10));

    [Fact]
    public void DecimalAcceptsCommaAndPoint()
    {
        Assert.Equal(12.50m, Input("x\n12,50\n").Decimal("p"));
        Assert.Equal(3.25m, Input("3.25\n").Decimal("p"));
    }

    [Fact]
    public void EnterKeepsDefaultValue()
        => Assert.Equal("Teclado", Input("\n").Text("Nombre", "Teclado"));

    [Fact]
    public void EndOfInputThrows()
        => Assert.Throws<EndOfInputException>(() => Input("").Required("Nombre"));
}

public class ReportTests
{
    [Fact]
    public void SummaryGroupsByCategory()
    {
        var repo = new ProductRepository();
        repo.Add("A", "Cat1", 10m, 2);
        repo.Add("B", "Cat1", 5m, 2);
        repo.Add("C", "Cat2", 1m, 1);

        string text = Report.Summary(repo.FindAll());
        Assert.Contains("Productos: 3", text);
        Assert.Contains("Cat1", text);
        Assert.Contains("30,00 €", text);
    }

    [Fact]
    public void TableAlignsColumns()
    {
        var lines = Report.ProductTable([new Product(1, "A", "C", 1m, 1)]).TrimEnd().Split(Environment.NewLine);
        Assert.Equal(3, lines.Length);                       // cabecera, separador, una fila
        Assert.Equal(lines[0].Length, lines[2].Length);      // columnas alineadas: misma longitud
    }
}
```

```bash
dotnet test
```

| Pieza | Significado |
|---|---|
| `[Fact]` | Marca un método como una prueba |
| `Assert.Equal(esperado, real)` | Falla si no coinciden (**primero el esperado**) |
| `Assert.Throws<T>(() => ...)` | Comprueba que se lanza esa excepción |
| `StringReader` / `StringWriter` | Simulan teclado y pantalla en memoria |
| `dotnet test` | Compila y ejecuta todas las pruebas; muestra `Passed: 13` |

## 11. Errores típicos

| Síntoma | Causa | Solución |
|---|---|---|
| Acentos como `?` en Windows | Codificación de la consola | `Console.OutputEncoding = Encoding.UTF8` al inicio |
| `12.5` se lee como 125 | Cultura del sistema | `CultureInfo.InvariantCulture` explícito |
| `NETSDK1100` al compilar WinForms en Linux | Falta la propiedad | `-p:EnableWindowsTargeting=true` |
| Excel muestra `Ã¡` en el CSV | Falta el BOM | `new UTF8Encoding(true)` |
| `dotnet run` pide elegir proyecto | Hay varios `Exe` | `--project ruta` |
| JSON con `á` | Escapado por defecto | `UnsafeRelaxedJsonEscaping` |

## 12. Ejercicios

1. Añade el campo `Supplier` al `record` y al JSON. ¿Qué pasa al cargar un JSON antiguo?
2. Añade **ordenar por** precio, nombre o stock (`OrderBy` / `ThenBy`).
3. Añade un informe de **bajo stock** con umbral pedido al usuario.
4. Guarda también en **CSV leyendo de vuelta** (importar), con validación fila a fila e informe de filas rechazadas.
5. En WinForms, añade un `TextBox` de **búsqueda** que filtre al escribir.
6. En WinForms, pide confirmación antes de borrar (`MessageBox.Show` con `MessageBoxButtons.YesNo`).
7. Escribe tres pruebas más: precio negativo, id inexistente, JSON corrupto.
8. Haz que la consola acepte argumentos: `list`, `add "Nombre" "Cat" 9.90 5`.
