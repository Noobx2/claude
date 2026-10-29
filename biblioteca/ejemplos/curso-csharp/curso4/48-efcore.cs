#:property PublishAot=false
#:package Npgsql.EntityFrameworkCore.PostgreSQL@10.0.3

using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

const string Conexion = "Host=localhost;Database=efdemo;Username=taskflow;Password=taskflow_dev";
var contador = new ContadorDeComandos();                         // cuenta cuántas consultas SQL se envían de verdad
DbContextOptions<Biblioteca> Opciones() => new DbContextOptionsBuilder<Biblioteca>().UseNpgsql(Conexion).AddInterceptors(contador).Options;

// ---- Crear el esquema desde el modelo y sembrar datos ----
await using (var db = new Biblioteca(Opciones()))
{
    await db.Database.EnsureDeletedAsync();
    await db.Database.EnsureCreatedAsync();                       // en proyectos reales: migraciones (lección 47); aquí basta para experimentar
    var tags = new[] { new Etiqueta { Nombre = "novela" }, new Etiqueta { Nombre = "clásico" }, new Etiqueta { Nombre = "ciencia" } };
    db.Autores.AddRange(
        new Autor { Nombre = "Cervantes",  Libros = { new Libro { Titulo = "El Quijote", Paginas = 1200, Etiquetas = { tags[0], tags[1] } },
                                                      new Libro { Titulo = "La Galatea", Paginas = 400, Etiquetas = { tags[0] } } } },
        new Autor { Nombre = "Borges",     Libros = { new Libro { Titulo = "Ficciones", Paginas = 200, Etiquetas = { tags[0] } },
                                                      new Libro { Titulo = "El Aleph", Paginas = 180 } } },
        new Autor { Nombre = "Sagan",      Libros = { new Libro { Titulo = "Cosmos", Paginas = 365, Etiquetas = { tags[2] } } } },
        new Autor { Nombre = "Sin libros" });
    int guardados = await db.SaveChangesAsync();
    Console.WriteLine($"Sembrado: {guardados} filas guardadas en una sola transacción");
}

// ================= 1) De LINQ a SQL =================
Console.WriteLine("\n=== 1) LINQ → SQL (ToQueryString muestra el SQL sin ejecutarlo) ===");
await using (var db = new Biblioteca(Opciones()))
{
    Mostrar("Libros de más de 300 páginas, ordenados",
        db.Libros.Where(l => l.Paginas > 300).OrderBy(l => l.Titulo));
    Mostrar("Proyección: solo título y nombre del autor",
        db.Libros.Select(l => new { l.Titulo, Autor = l.Autor.Nombre }));
    Mostrar("Agrupar: páginas totales por autor",
        db.Libros.GroupBy(l => l.Autor.Nombre).Select(g => new { Autor = g.Key, Total = g.Sum(l => l.Paginas) }));
    Mostrar("Paginación: página 2 de tamaño 2",
        db.Libros.OrderBy(l => l.Id).Skip(2).Take(2));
    Mostrar("Búsqueda sin distinguir mayúsculas",
        db.Libros.Where(l => EF.Functions.ILike(l.Titulo, "%quij%")));
}

// ================= 2) Seguimiento de cambios =================
Console.WriteLine("\n=== 2) Change tracking ===");
await using (var db = new Biblioteca(Opciones()))
{
    var libro = await db.Libros.FirstAsync(l => l.Titulo == "Cosmos");
    Console.WriteLine($"Tras cargar: estado = {db.Entry(libro).State}");
    libro.Paginas += 10;
    Console.WriteLine($"Tras cambiar Paginas: estado = {db.Entry(libro).State} (EF detectó el cambio solo)");
    contador.Reiniciar(); await db.SaveChangesAsync();
    Console.WriteLine($"SaveChanges envió {contador.Cuenta} comando(s); solo actualiza la columna modificada: {contador.Ultimo!.Replace("\n", " ")[..80]}…");
    var soloLectura = await db.Libros.AsNoTracking().FirstAsync();
    Console.WriteLine($"Con AsNoTracking: estado = {db.Entry(soloLectura).State} (más rápido para consultas de solo lectura)");
}

// ================= 3) El problema N+1 =================
Console.WriteLine("\n=== 3) El problema N+1 ===");
await using (var db = new Biblioteca(Opciones()))
{
    contador.Reiniciar();
    var autores = await db.Autores.ToListAsync();                       // 1 consulta
    foreach (var a in autores)
        a.Libros = await db.Libros.Where(l => l.AutorId == a.Id).ToListAsync();   // +1 consulta POR autor
    Console.WriteLine($"Cargar libros autor por autor     : {contador.Cuenta} consultas  (1 + {autores.Count})");
}
await using (var db = new Biblioteca(Opciones()))
{
    contador.Reiniciar();
    var autores = await db.Autores.Include(a => a.Libros).ToListAsync();      // 1 consulta con JOIN
    Console.WriteLine($"Con Include (un JOIN)               : {contador.Cuenta} consulta   ({autores.Sum(a => a.Libros.Count)} libros)");
}
await using (var db = new Biblioteca(Opciones()))
{
    contador.Reiniciar();
    var resumen = await db.Autores.Select(a => new { a.Nombre, Libros = a.Libros.Count, Paginas = a.Libros.Sum(l => (int?)l.Paginas) ?? 0 }).ToListAsync();
    Console.WriteLine($"Proyección (solo lo que se muestra) : {contador.Cuenta} consulta");
    foreach (var r in resumen) Console.WriteLine($"    {r.Nombre,-11} {r.Libros} libro(s), {r.Paginas} páginas");
}

// ================= 4) Muchos a muchos =================
Console.WriteLine("\n=== 4) Muchos a muchos ===");
await using (var db = new Biblioteca(Opciones()))
{
    var novelas = await db.Libros.Where(l => l.Etiquetas.Any(e => e.Nombre == "novela")).OrderBy(l => l.Titulo).Select(l => l.Titulo).ToListAsync();
    Console.WriteLine("Libros etiquetados «novela»: " + string.Join(", ", novelas));
    Console.WriteLine("Tabla intermedia creada por EF: " + string.Join(", ", db.Model.GetEntityTypes().Where(t => t.ClrType == typeof(Dictionary<string, object>)).Select(t => t.GetTableName())));
}

// ================= 5) Transacciones =================
Console.WriteLine("\n=== 5) Transacciones ===");
await using (var db = new Biblioteca(Opciones()))
{
    await using var tx = await db.Database.BeginTransactionAsync();
    db.Autores.Add(new Autor { Nombre = "Temporal" });
    await db.SaveChangesAsync();
    Console.WriteLine($"Dentro de la transacción hay {await db.Autores.CountAsync()} autores");
    await tx.RollbackAsync();                                            // deshace todo
}
await using (var db = new Biblioteca(Opciones()))
    Console.WriteLine($"Tras el rollback hay {await db.Autores.CountAsync()} autores (el «Temporal» desapareció)");

// ================= 6) Concurrencia optimista =================
Console.WriteLine("\n=== 6) Concurrencia optimista (dos usuarios editan lo mismo) ===");
await using (var db1 = new Biblioteca(Opciones()))
await using (var db2 = new Biblioteca(Opciones()))
{
    var porAna = await db1.Libros.FirstAsync(l => l.Titulo == "Ficciones");
    var porLuis = await db2.Libros.FirstAsync(l => l.Titulo == "Ficciones");
    porAna.Paginas = 210; await db1.SaveChangesAsync();                  // Ana guarda primero
    Console.WriteLine("Ana guarda: correcto");
    porLuis.Paginas = 999;
    try { await db2.SaveChangesAsync(); }
    catch (DbUpdateConcurrencyException) { Console.WriteLine("Luis guarda: DbUpdateConcurrencyException → la fila cambió desde que la leyó (no se pisan los cambios de Ana)"); }
}

// ================= 7) Operaciones masivas =================
Console.WriteLine("\n=== 7) ExecuteUpdate / ExecuteDelete (sin cargar entidades) ===");
await using (var db = new Biblioteca(Opciones()))
{
    contador.Reiniciar();
    int cambiados = await db.Libros.Where(l => l.Paginas < 300).ExecuteUpdateAsync(s => s.SetProperty(l => l.Paginas, l => l.Paginas + 1));
    Console.WriteLine($"ExecuteUpdate: {cambiados} filas en {contador.Cuenta} comando · SQL: {contador.Ultimo!.Replace("\n", " ")}");
    int borrados = await db.Autores.Where(a => !a.Libros.Any()).ExecuteDeleteAsync();
    Console.WriteLine($"ExecuteDelete: {borrados} autor(es) sin libros eliminados");
}

// ================= 8) SQL propio y la inyección SQL =================
Console.WriteLine("\n=== 8) SQL propio e inyección SQL ===");
await using (var db = new Biblioteca(Opciones()))
{
    string entrada = "x' OR '1'='1";                                      // lo que escribiría un atacante
    var mal = await db.Libros.FromSqlRaw("SELECT *, xmin FROM \"Libros\" WHERE \"Titulo\" = '" + entrada + "'").ToListAsync();   // MAL: concatenar. (Se añade xmin porque FromSql debe devolver TODAS las columnas del modelo, incluida la de concurrencia)
    Console.WriteLine($"CONCATENANDO el texto del usuario : {mal.Count} filas devueltas  ← ¡devolvió TODA la tabla!");
    var bien = await db.Libros.FromSql($"SELECT *, xmin FROM \"Libros\" WHERE \"Titulo\" = {entrada}").ToListAsync();   // BIEN: parámetro
    Console.WriteLine($"Con FromSql (interpolación segura): {bien.Count} filas devueltas  ← el texto viaja como parámetro, no como SQL");
}

static void Mostrar<T>(string titulo, IQueryable<T> consulta)
    => Console.WriteLine($"\n-- {titulo}\n{consulta.ToQueryString().Replace("\r", "")}");

// ---------------------------------------------------------------- modelo
public class Autor { public int Id { get; set; } public string Nombre { get; set; } = ""; public List<Libro> Libros { get; set; } = []; }
public class Libro
{
    public int Id { get; set; } public string Titulo { get; set; } = ""; public int Paginas { get; set; }
    public int AutorId { get; set; } public Autor Autor { get; set; } = null!;
    public List<Etiqueta> Etiquetas { get; set; } = [];                 // muchos a muchos: EF crea la tabla intermedia sola
}
public class Etiqueta { public int Id { get; set; } public string Nombre { get; set; } = ""; public List<Libro> Libros { get; set; } = []; }

public class Biblioteca(DbContextOptions<Biblioteca> opciones) : DbContext(opciones)
{
    public DbSet<Autor> Autores => Set<Autor>();
    public DbSet<Libro> Libros => Set<Libro>();
    protected override void OnModelCreating(ModelBuilder m)
    {
        m.Entity<Libro>().Property<uint>("xmin").HasColumnType("xid").ValueGeneratedOnAddOrUpdate().IsConcurrencyToken();   // token de concurrencia de PostgreSQL
        m.Entity<Autor>().HasMany(a => a.Libros).WithOne(l => l.Autor).HasForeignKey(l => l.AutorId).OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>Interceptor: se entera de cada comando SQL que EF envía a la base. Muy útil para detectar N+1.</summary>
public class ContadorDeComandos : DbCommandInterceptor
{
    public int Cuenta { get; private set; } public string? Ultimo { get; private set; }
    public void Reiniciar() { Cuenta = 0; Ultimo = null; }
    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand c, CommandEventData e, InterceptionResult<DbDataReader> r, CancellationToken ct = default)
    { Cuenta++; Ultimo = c.CommandText; return base.ReaderExecutingAsync(c, e, r, ct); }
    public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand c, CommandEventData e, InterceptionResult<int> r, CancellationToken ct = default)
    { Cuenta++; Ultimo = c.CommandText; return base.NonQueryExecutingAsync(c, e, r, ct); }
}
