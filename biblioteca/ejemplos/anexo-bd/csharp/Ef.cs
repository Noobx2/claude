using Microsoft.EntityFrameworkCore;

// Entity Framework Core: ORM de .NET. Las tablas son clases (DbSet) y las consultas se escriben con LINQ.
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public List<Product> Products { get; set; } = [];             // 1 categoría -> N productos
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public int CategoryId { get; set; }                           // clave foránea por convención
    public Category Category { get; set; } = null!;
}

public class ShopContext : DbContext                              // DbContext = una sesión con la base de datos
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=shop-ef.db")             // para PostgreSQL: UseNpgsql(...); para SQL Server: UseSqlServer(...)
                  .LogTo(sql => { if (sql.Contains("Executed DbCommand")) Queries++; });

    public static int Queries;
}

public static class Ef
{
    public static void Run()
    {
        File.Delete("shop-ef.db");
        using var db = new ShopContext();
        db.Database.EnsureCreated();                              // crea las tablas (en proyectos reales: migraciones)

        db.Categories.AddRange(
            new Category { Name = "Periféricos", Products = [new() { Name = "Teclado", Price = 59.9m }, new() { Name = "Ratón", Price = 24.5m }] },
            new Category { Name = "Cables",      Products = [new() { Name = "HDMI", Price = 7.5m }] });
        db.SaveChanges();                                         // INSERT de todo en una transacción

        // LINQ se traduce a SQL
        var expensive = db.Products.Where(p => p.Price > 20).OrderByDescending(p => p.Price).Select(p => $"{p.Name} {p.Price}").ToList();
        Console.WriteLine("caros: " + string.Join(", ", expensive));

        var perCategory = db.Products.GroupBy(p => p.Category.Name).Select(g => new { Category = g.Key, Count = g.Count() }).OrderBy(x => x.Category).ToList();
        Console.WriteLine("por categoría: " + string.Join(", ", perCategory.Select(x => $"{x.Category}={x.Count}")));

        // UPDATE: se modifica el objeto y SaveChanges genera el UPDATE
        var keyboard = db.Products.Single(p => p.Name == "Teclado");
        keyboard.Price = 49.9m;
        db.SaveChanges();

        // N+1 y su solución: Include carga la relación con un JOIN
        db.ChangeTracker.Clear();
        ShopContext.Queries = 0;
        foreach (var category in db.Categories.ToList())
            _ = db.Entry(category).Collection(c => c.Products).Query().ToList();      // una consulta por categoría
        Console.WriteLine($"sin optimizar: {ShopContext.Queries} consultas");

        db.ChangeTracker.Clear();
        ShopContext.Queries = 0;
        _ = db.Categories.Include(c => c.Products).ToList();
        Console.WriteLine($"con Include: {ShopContext.Queries} consulta");

        Console.WriteLine("SQL generado: " + db.Products.Where(p => p.Price > 20).OrderBy(p => p.Name).ToQueryString().Replace("\n", " "));
    }
}
