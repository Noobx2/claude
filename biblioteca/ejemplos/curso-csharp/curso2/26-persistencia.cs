// Lección 26: CRUD que GUARDA sus datos (JSON y CSV) y se maneja por argumentos
// Cada ejecución es un comando: los datos sobreviven entre ejecuciones porque están en un archivo.
//   dotnet run 26-persistencia.cs -- add "Ana Ruiz" ana@correo.es --city Madrid
//   dotnet run 26-persistencia.cs -- list
//   dotnet run 26-persistencia.cs -- update 1 --city Bilbao
//   dotnet run 26-persistencia.cs -- delete 1
//   dotnet run 26-persistencia.cs -- export contactos.csv        /   import contactos.csv
// El archivo de datos se elige con la variable CONTACTS_FILE (por defecto, contactos.json).
#:property PublishAot=false
using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

string file = Environment.GetEnvironmentVariable("CONTACTS_FILE") ?? "contactos.json";
IContactStore store = Path.GetExtension(file).Equals(".csv", StringComparison.OrdinalIgnoreCase) ? new CsvContactStore(file) : new JsonContactStore(file);
var contacts = store.Load();                                   // 1) al empezar: cargar lo guardado

if (args.Length == 0) { Console.WriteLine("comandos: add, list, update, delete, export, import"); return 1; }

switch (args[0])
{
    case "add":
    {
        string name = args.ElementAtOrDefault(1) ?? "", email = args.ElementAtOrDefault(2) ?? "";
        if (name.Length < 2 || !email.Contains('@')) { Console.Error.WriteLine("uso: add NOMBRE CORREO [--city CIUDAD]"); return 2; }
        var contact = new Contact(contacts.Select(c => c.Id).DefaultIfEmpty(0).Max() + 1, name, email, Option("--city"));
        contacts.Add(contact);
        store.Save(contacts);                                  // 2) tras cada cambio: guardar
        Console.WriteLine($"creado {contact.Id}: {contact.Name}");
        break;
    }
    case "list":
        foreach (var c in contacts.OrderBy(c => c.Id)) Console.WriteLine($"{c.Id,3} {c.Name,-14} {c.Email,-16} {c.City}");
        Console.WriteLine($"{contacts.Count} contacto(s) en {file}");
        break;
    case "update":
    {
        int id = int.Parse(args[1]);
        var old = contacts.FirstOrDefault(c => c.Id == id);
        if (old is null) { Console.Error.WriteLine($"no existe {id}"); return 3; }
        contacts[contacts.IndexOf(old)] = old with { City = Option("--city") ?? old.City, Name = Option("--name") ?? old.Name };
        store.Save(contacts);
        Console.WriteLine($"actualizado {id}");
        break;
    }
    case "delete":
    {
        int id = int.Parse(args[1]);
        Console.WriteLine(contacts.RemoveAll(c => c.Id == id) > 0 ? $"borrado {id}" : $"no existe {id}");
        store.Save(contacts);
        break;
    }
    case "export":                                              // de JSON a CSV (o al revés, según la extensión)
        new CsvContactStore(args[1]).Save(contacts);
        Console.WriteLine($"exportados {contacts.Count} a {args[1]}");
        break;
    case "import":
    {
        var imported = new CsvContactStore(args[1]).Load();
        int next = contacts.Select(c => c.Id).DefaultIfEmpty(0).Max();
        foreach (var c in imported) contacts.Add(c with { Id = ++next });   // reasignamos Id para no chocar
        store.Save(contacts);
        Console.WriteLine($"importados {imported.Count}");
        break;
    }
    default:
        Console.Error.WriteLine($"comando desconocido: {args[0]}");
        return 2;
}
return 0;

string? Option(string name)                                     // valor de una opción "--city Madrid"
{
    int i = Array.IndexOf(args, name);
    return i >= 0 && i + 1 < args.Length ? args[i + 1] : null;
}

// ───────────────────────── CONTRATO: la lógica no sabe CÓMO se guarda ─────────────────────────
interface IContactStore
{
    List<Contact> Load();
    void Save(IEnumerable<Contact> contacts);
}

record Contact(int Id, string Name, string Email, string? City);

// ───────────────────────── Guardar en JSON ─────────────────────────
class JsonContactStore(string path) : IContactStore
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

    public List<Contact> Load() =>
        File.Exists(path) ? JsonSerializer.Deserialize<List<Contact>>(File.ReadAllText(path)) ?? [] : [];   // sin archivo = lista vacía

    public void Save(IEnumerable<Contact> contacts)
    {
        // ESCRITURA SEGURA: primero a un archivo temporal y luego se renombra.
        // Si el programa se corta a mitad, el archivo original NO queda a medias.
        string temp = path + ".tmp";
        File.WriteAllText(temp, JsonSerializer.Serialize(contacts, Options), new UTF8Encoding(false));
        File.Move(temp, path, overwrite: true);
    }
}

// ───────────────────────── Guardar en CSV (se abre en Excel) ─────────────────────────
class CsvContactStore(string path) : IContactStore
{
    public List<Contact> Load()
    {
        if (!File.Exists(path)) return [];
        return File.ReadAllLines(path, Encoding.UTF8).Skip(1).Where(l => l.Trim().Length > 0)   // Skip(1): saltar la cabecera
            .Select(ParseLine)
            .Select(f => new Contact(int.Parse(f[0], CultureInfo.InvariantCulture), f[1], f[2], f[3].Length == 0 ? null : f[3]))
            .ToList();
    }

    public void Save(IEnumerable<Contact> contacts)
    {
        var lines = new List<string> { "id;name;email;city" };
        lines.AddRange(contacts.Select(c => string.Join(';', c.Id, Quote(c.Name), Quote(c.Email), Quote(c.City ?? ""))));
        File.WriteAllLines(path, lines, new UTF8Encoding(true));        // con BOM: Excel reconoce las tildes
    }

    // Un campo con ; o " o salto de línea va entre comillas y las comillas se duplican
    static string Quote(string v) => v.Contains(';') || v.Contains('"') || v.Contains('\n') ? $"\"{v.Replace("\"", "\"\"")}\"" : v;

    // Lector de una línea CSV respetando las comillas (no basta con Split(';'))
    static List<string> ParseLine(string line)
    {
        var fields = new List<string>(); var sb = new StringBuilder(); bool inQuotes = false;
        for (int i = 0; i < line.Length; i++)
        {
            char ch = line[i];
            if (inQuotes) { if (ch == '"' && i + 1 < line.Length && line[i + 1] == '"') { sb.Append('"'); i++; } else if (ch == '"') inQuotes = false; else sb.Append(ch); }
            else if (ch == '"') inQuotes = true;
            else if (ch == ';') { fields.Add(sb.ToString()); sb.Clear(); }
            else sb.Append(ch);
        }
        fields.Add(sb.ToString());
        return fields;
    }
}
