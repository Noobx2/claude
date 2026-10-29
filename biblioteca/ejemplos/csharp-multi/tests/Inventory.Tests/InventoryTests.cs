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
