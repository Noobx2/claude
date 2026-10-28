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
