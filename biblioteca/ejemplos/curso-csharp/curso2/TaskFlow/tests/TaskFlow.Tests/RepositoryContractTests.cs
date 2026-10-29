using TaskFlow.Application;
using TaskFlow.Domain;
using TaskFlow.Infrastructure;

namespace TaskFlow.Tests;

/// <summary>
/// "Prueba de contrato": la MISMA batería de pruebas se ejecuta contra todas las implementaciones.
/// Si cumplen el contrato, son intercambiables.
/// </summary>
public class RepositoryContractTests : IDisposable
{
    private readonly string _folder = Path.Combine(Path.GetTempPath(), "taskflow-tests-" + Guid.NewGuid());

    public RepositoryContractTests() => Directory.CreateDirectory(_folder);
    public void Dispose() => Directory.Delete(_folder, recursive: true);

    public static TheoryData<string> Kinds => new() { "memory", "json" };

    private ITaskRepository Create(string kind) =>
        kind == "memory" ? new InMemoryTaskRepository() : new JsonTaskRepository(Path.Combine(_folder, "tasks.json"));

    private static TaskItem Sample(string title) => new(0, title, null, Priority.Medium, null, false, new DateTime(2026, 3, 10));

    [Theory, MemberData(nameof(Kinds))]
    public void Add_AssignsConsecutiveIds(string kind)
    {
        var repo = Create(kind);
        Assert.Equal(1, repo.Add(Sample("a")).Id);
        Assert.Equal(2, repo.Add(Sample("b")).Id);
        Assert.Equal(2, repo.GetAll().Count);
    }

    [Theory, MemberData(nameof(Kinds))]
    public void Remove_ReturnsFalseWhenMissing(string kind)
    {
        var repo = Create(kind);
        var saved = repo.Add(Sample("a"));
        Assert.True(repo.Remove(saved.Id));
        Assert.False(repo.Remove(saved.Id));
        Assert.Null(repo.Find(saved.Id));
    }

    [Fact]
    public void Json_PersistsBetweenInstances()
    {
        string path = Path.Combine(_folder, "persist.json");
        var first = new JsonTaskRepository(path);
        first.Add(Sample("Guardada"));
        first.Add(Sample("Segunda"));

        var second = new JsonTaskRepository(path);                                // "otra ejecución del programa"
        Assert.Equal(["Guardada", "Segunda"], second.GetAll().Select(t => t.Title));
        Assert.Equal(3, second.Add(Sample("Tercera")).Id);                        // el contador de Id continúa
    }

    [Fact]
    public void Json_StoresPriorityAsText()
    {
        string path = Path.Combine(_folder, "text.json");
        new JsonTaskRepository(path).Add(Sample("x") with { Priority = Priority.High });
        Assert.Contains("\"Priority\": \"High\"", File.ReadAllText(path));
    }
}
