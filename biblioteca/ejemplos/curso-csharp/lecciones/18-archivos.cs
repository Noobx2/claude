// Lección 18: archivos, carpetas, JSON y datos binarios
#:property PublishAot=false
using System.Text;
using System.Text.Json;

// Trabajamos en una carpeta temporal propia
string folder = Path.Combine(Path.GetTempPath(), "curso-csharp-" + Environment.ProcessId);
Directory.CreateDirectory(folder);                    // crea la carpeta (si existe, no pasa nada)

// ---- Rutas: nunca concatenes con "/" o "\" a mano ----
string file = Path.Combine(folder, "notas.txt");
Console.WriteLine($"{Path.GetFileName(file)} | {Path.GetExtension(file)} | {Path.GetFileNameWithoutExtension(file)}");

// ---- Texto: leer y escribir ----
File.WriteAllText(file, "primera línea\nsegunda línea\n", Encoding.UTF8);
File.AppendAllText(file, "tercera línea\n");
Console.WriteLine($"leído entero: {File.ReadAllText(file).Length} caracteres");
string[] lines = File.ReadAllLines(file);
Console.WriteLine($"{lines.Length} líneas; la segunda es '{lines[1]}'; existe: {File.Exists(file)}");

// ---- Flujos (streams): para archivos grandes, línea a línea ----
using (var writer = new StreamWriter(Path.Combine(folder, "log.txt")))
{
    for (int i = 1; i <= 3; i++) writer.WriteLine($"evento {i}");
}                                                      // al cerrar el using se vuelcan los datos
using (var reader = new StreamReader(Path.Combine(folder, "log.txt")))
{
    string? line;
    while ((line = reader.ReadLine()) != null) Console.Write($"[{line}] ");
    Console.WriteLine();
}
foreach (var l in File.ReadLines(Path.Combine(folder, "log.txt")).Where(l => l.EndsWith('2'))) Console.WriteLine($"ReadLines (perezoso): {l}");

// ---- JSON ----
var tasks = new List<TaskItem> { new(1, "Estudiar C#", false), new(2, "Hacer la compra", true) };
var options = new JsonSerializerOptions { WriteIndented = true };    // reutiliza las opciones: crearlas cuesta
string json = JsonSerializer.Serialize(tasks, options);
Console.WriteLine(json);
File.WriteAllText(Path.Combine(folder, "tasks.json"), json);
var back = JsonSerializer.Deserialize<List<TaskItem>>(File.ReadAllText(Path.Combine(folder, "tasks.json")))!;
Console.WriteLine($"leídas {back.Count} tareas; la primera: {back[0]}");

// JSON sin clase: JsonDocument
using (var doc = JsonDocument.Parse("""{"user":{"name":"Ana","tags":["a","b"]},"active":true}"""))
{
    var root = doc.RootElement;
    Console.WriteLine($"{root.GetProperty("user").GetProperty("name").GetString()} {root.GetProperty("user").GetProperty("tags").GetArrayLength()} {root.GetProperty("active").GetBoolean()}");
}

// ---- Binario: bytes ----
string binPath = Path.Combine(folder, "datos.bin");
using (var stream = new FileStream(binPath, FileMode.Create))
using (var bw = new BinaryWriter(stream))
{
    bw.Write(42);              // 4 bytes
    bw.Write(3.14);            // 8 bytes
    bw.Write("hola");          // longitud + bytes
    bw.Write(true);            // 1 byte
}
Console.WriteLine($"tamaño: {new FileInfo(binPath).Length} bytes; hexadecimal: {Convert.ToHexString(File.ReadAllBytes(binPath))}");
using (var br = new BinaryReader(File.OpenRead(binPath)))
{
    Console.WriteLine($"leídos en el mismo orden: {br.ReadInt32()} {br.ReadDouble()} {br.ReadString()} {br.ReadBoolean()}");
}

// ---- Carpetas ----
Directory.CreateDirectory(Path.Combine(folder, "sub", "otra"));
File.WriteAllText(Path.Combine(folder, "sub", "a.txt"), "a");
Console.WriteLine("archivos .txt (recursivo): " + string.Join(", ", Directory.GetFiles(folder, "*.txt", SearchOption.AllDirectories).Select(Path.GetFileName).Order()));
Console.WriteLine("subcarpetas: " + string.Join(", ", Directory.GetDirectories(folder).Select(Path.GetFileName).Order()));
File.Move(file, Path.Combine(folder, "renombrado.txt"));
File.Copy(Path.Combine(folder, "renombrado.txt"), Path.Combine(folder, "copia.txt"));
File.Delete(Path.Combine(folder, "copia.txt"));

// ---- Manejo de errores típicos ----
try { File.ReadAllText(Path.Combine(folder, "no-existe.txt")); }
catch (FileNotFoundException e) { Console.WriteLine($"FileNotFoundException: {Path.GetFileName(e.FileName)}"); }

// ---- Limpieza ----
Directory.Delete(folder, recursive: true);            // recursive: borra también el contenido
Console.WriteLine($"carpeta borrada: {!Directory.Exists(folder)}");

record TaskItem(int Id, string Title, bool Done);
