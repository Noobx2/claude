using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using TaskFlow.Domain;

namespace TaskFlow.Infrastructure;

/// <summary>Guarda en un archivo JSON después de cada cambio. Reutiliza la lógica de la versión en memoria.</summary>
public sealed class JsonTaskRepository : InMemoryTaskRepository
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Converters = { new JsonStringEnumConverter() },           // Priority se guarda como "High", no como 3
    };

    private readonly string _path;

    public JsonTaskRepository(string path)
    {
        _path = path;
        if (File.Exists(path))
        {
            var loaded = JsonSerializer.Deserialize<List<TaskItem>>(File.ReadAllText(path), Options) ?? [];
            Items.AddRange(loaded);
            SetLastId(Items.Select(t => t.Id).DefaultIfEmpty(0).Max());
        }
    }

    public override TaskItem Add(TaskItem task) { var saved = base.Add(task); Save(); return saved; }
    public override void Update(TaskItem task) { base.Update(task); Save(); }
    public override bool Remove(int id) { bool removed = base.Remove(id); if (removed) Save(); return removed; }

    private void Save()
    {
        string temp = _path + ".tmp";                                   // escritura segura: temporal + renombrar
        File.WriteAllText(temp, JsonSerializer.Serialize(Items, Options), new UTF8Encoding(false));
        File.Move(temp, _path, overwrite: true);
    }
}
