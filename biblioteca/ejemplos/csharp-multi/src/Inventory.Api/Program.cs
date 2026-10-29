using Inventory;

var builder = WebApplication.CreateBuilder(args);

// El Core NO es seguro para hilos (usa List<T> sin cerrojo, porque se pensó para una ventana de un solo hilo).
// Una API atiende peticiones a la vez, así que lo envolvemos con un cerrojo sin tocar el Core.
builder.Services.AddSingleton<SafeInventory>();

var app = builder.Build();

app.UseDefaultFiles();   // sirve wwwroot/index.html en "/"
app.UseStaticFiles();    // sirve el resultado de "vite build" (HTML, JS, CSS)

var products = app.MapGroup("/api/products");

products.MapGet("/", (string? q, SafeInventory inventory) =>
    inventory.Read(repo => (string.IsNullOrWhiteSpace(q) ? repo.FindAll() : repo.Search(q)).Select(ProductDto.From)));

products.MapPost("/", (CreateProduct request, SafeInventory inventory) =>
{
    try
    {
        var created = inventory.Write(repo => repo.Add(request.Name, request.Category, request.Price, request.Stock));
        return Results.Created($"/api/products/{created.Id}", ProductDto.From(created));
    }
    catch (ArgumentException e)   // las reglas de validación viven en el Core (Product); aquí solo se traducen a HTTP
    {
        return Results.BadRequest(new { error = e.Message });
    }
});

products.MapDelete("/{id:int}", (int id, SafeInventory inventory) =>
    inventory.Write(repo => repo.Delete(id)) ? Results.NoContent() : Results.NotFound());

app.MapGet("/api/summary", (SafeInventory inventory) => Results.Text(inventory.Read(repo => Report.Summary(repo.FindAll())), "text/plain; charset=utf-8"));

app.Run();

public record CreateProduct(string Name, string Category, decimal Price, int Stock);

public record ProductDto(int Id, string Name, string Category, decimal Price, int Stock, decimal TotalValue)
{
    public static ProductDto From(Product p) => new(p.Id, p.Name, p.Category, p.Price, p.Stock, p.TotalValue);
}

/// <summary>Envuelve el repositorio del Core con un cerrojo (lock).</summary>
public sealed class SafeInventory
{
    private readonly ProductRepository _repo = new();
    private readonly object _gate = new();

    public SafeInventory()
    {
        _repo.Add("Teclado mecánico", "Periféricos", 59.90m, 25);
        _repo.Add("Ratón inalámbrico", "Periféricos", 24.50m, 60);
        _repo.Add("Monitor 24\"", "Monitores", 139.00m, 10);
        _repo.Add("SSD 1 TB", "Almacenamiento", 79.90m, 30);
    }

    public T Read<T>(Func<ProductRepository, T> action) { lock (_gate) return action(_repo); }
    public T Write<T>(Func<ProductRepository, T> action) { lock (_gate) return action(_repo); }
}
