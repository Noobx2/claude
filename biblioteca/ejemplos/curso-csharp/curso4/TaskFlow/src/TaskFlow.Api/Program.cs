using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskFlow.Api;
using TaskFlow.Application;
using TaskFlow.Domain;
using TaskFlow.Infrastructure;
using TaskFlow.Infrastructure.Persistence;
using TaskFlow.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

// ---- Servicios (contenedor de DI) ----
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSingleton<IClock, UtcClock>();
builder.Services.AddScoped<TaskService>();
// Los enums viajan como texto ("High") y no como número: más legible para quien consume la API.
builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<DomainExceptionHandler>();
builder.Services.AddHealthChecks().AddDbContextCheck<TaskFlowDbContext>("postgres");
builder.Services.AddOpenApi();
builder.Services.AddValidation();   // .NET 10: valida los atributos [Required], [StringLength]... de los DTO y responde 400

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<JwtTokenService>();
var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? new();
if (jwt.Key.Length < 32) throw new InvalidOperationException("Jwt:Key debe tener al menos 32 caracteres (configúrala con user-secrets o variable de entorno).");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = jwt.Issuer, ValidAudience = jwt.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
        ClockSkew = TimeSpan.FromSeconds(10),
    });
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", p => p.RequireRole("Admin"));

var app = builder.Build();

// ---- Pipeline de middleware (el orden importa) ----
app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.MapHealthChecks("/health");

// ---- Autenticación ----
var auth = app.MapGroup("/auth");
auth.MapPost("/register", (RegisterRequest req, TaskFlowDbContext db) =>
{
    if (db.Users.Any(u => u.UserName == req.UserName)) return Results.Conflict(new { error = "Ese usuario ya existe." });
    // El primer usuario registrado es Admin; el resto, User.
    string role = db.Users.Any() ? "User" : "Admin";
    db.Users.Add(new UserAccount { UserName = req.UserName, PasswordHash = PasswordHasher.Hash(req.Password), Role = role });
    db.SaveChanges();
    return Results.Created($"/auth/{req.UserName}", new { req.UserName, role });
});

auth.MapPost("/login", (LoginRequest req, TaskFlowDbContext db, JwtTokenService tokens) =>
{
    var user = db.Users.FirstOrDefault(u => u.UserName == req.UserName);
    // Mismo mensaje si el usuario no existe o la contraseña falla: no revelamos cuál de los dos fue.
    if (user is null || !PasswordHasher.Verify(req.Password, user.PasswordHash)) return Results.Unauthorized();
    return Results.Ok(new TokenResponse(tokens.Create(user.Id, user.UserName, user.Role), "Bearer", jwt.ExpiresMinutes));
});

// ---- CRUD de tareas (requiere token) ----
var tasks = app.MapGroup("/tasks").RequireAuthorization();

tasks.MapGet("/", (TaskService svc, IClock clock, bool? done, Priority? priority, string? q, bool overdue = false) =>
    svc.List(new TaskFilter(done, priority, q, overdue)).Select(t => TaskResponse.From(t, clock.Today)));

tasks.MapGet("/stats", (TaskService svc) => svc.GetStats());

tasks.MapGet("/{id:int}", (int id, TaskService svc, IClock clock) => TaskResponse.From(svc.Get(id), clock.Today));

tasks.MapPost("/", (CreateTaskRequest req, TaskService svc, IClock clock) =>
{
    var created = svc.Add(new NewTask(req.Title, req.Description, req.Priority, req.DueDate));
    return Results.Created($"/tasks/{created.Id}", TaskResponse.From(created, clock.Today));
});

tasks.MapPut("/{id:int}", (int id, UpdateTaskRequest req, TaskService svc, IClock clock) =>
    TaskResponse.From(svc.Update(id, new TaskChanges(req.Title, req.Description, req.Priority, req.DueDate)), clock.Today));

tasks.MapPost("/{id:int}/done", (int id, TaskService svc, IClock clock) => TaskResponse.From(svc.SetDone(id, true), clock.Today));

// Borrar es una acción sensible: solo el rol Admin (política "AdminOnly").
tasks.MapDelete("/{id:int}", (int id, TaskService svc) => { svc.Delete(id); return Results.NoContent(); })
     .RequireAuthorization("AdminOnly");

app.Run();

// ---- Tipos auxiliares ----
public sealed class UtcClock : IClock { public DateTime Now => DateTime.UtcNow; }

/// <summary>Traduce excepciones del dominio a respuestas HTTP estándar (ProblemDetails, RFC 9457).</summary>
public sealed class DomainExceptionHandler(ILogger<DomainExceptionHandler> log, IProblemDetailsService problems) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext ctx, Exception ex, CancellationToken ct)
    {
        (int status, string title) = ex switch
        {
            ValidationException => (400, "Datos inválidos"),
            NotFoundException   => (404, "No encontrado"),
            BadHttpRequestException => (400, "Solicitud mal formada"),   // JSON roto, enum desconocido, etc.
            _                   => (500, "Error interno"),
        };
        if (status == 500) log.LogError(ex, "Excepción no controlada en {Path}", ctx.Request.Path);   // solo los 500 son "errores" del servidor
        ctx.Response.StatusCode = status;
        return await problems.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = ctx,
            ProblemDetails = { Status = status, Title = title, Detail = status == 500 ? "Ocurrió un error inesperado." : ex.Message },
        });
    }
}

public partial class Program;   // permite que las pruebas de integración hagan WebApplicationFactory<Program>
