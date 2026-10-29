using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain;

namespace TaskFlow.Infrastructure.Persistence;

/// <summary>Una cuenta de usuario. Nunca guardamos la contraseña, solo su hash.</summary>
public sealed class UserAccount
{
    public int Id { get; set; }
    public string UserName { get; set; } = "";
    public string PasswordHash { get; set; } = "";   // formato: iteraciones.sal.hash (base64)
    public string Role { get; set; } = "User";        // "User" o "Admin"
    public DateTime CreatedAt { get; set; }           // lo rellena la base de datos (ver OnModelCreating)
}

/// <summary>El DbContext es la "sesión" con la base: EF Core lo usa para consultar y guardar.</summary>
public sealed class TaskFlowDbContext(DbContextOptions<TaskFlowDbContext> options) : DbContext(options)
{
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<UserAccount> Users => Set<UserAccount>();

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<TaskItem>(e =>
        {
            e.ToTable("tasks");
            e.HasKey(t => t.Id);
            e.Property(t => t.Id).ValueGeneratedOnAdd();
            e.Property(t => t.Title).HasMaxLength(80).IsRequired();
            e.Property(t => t.Description).HasMaxLength(500);
            e.Property(t => t.Priority).HasConversion<int>();
            e.HasIndex(t => new { t.Done, t.DueDate });     // para filtrar pendientes por fecha
        });

        model.Entity<UserAccount>(e =>
        {
            e.ToTable("users");
            e.HasIndex(u => u.UserName).IsUnique();         // dos usuarios no pueden llamarse igual
            e.Property(u => u.UserName).HasMaxLength(40).IsRequired();
            e.Property(u => u.Role).HasMaxLength(20).IsRequired();
            e.Property(u => u.CreatedAt).HasDefaultValueSql("now()");   // valor por defecto calculado por PostgreSQL
        });
    }
}
