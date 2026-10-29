// Lección 25: un CRUD completo en consola (datos en memoria)
// CRUD = Create (crear), Read (leer/listar/buscar), Update (modificar), Delete (borrar)
// Ejecutar interactivo:      dotnet run 25-crud-memoria.cs
// Ejecutar con entrada fija: dotnet run 25-crud-memoria.cs < 25-crud-memoria.in
using System.Globalization;

// ───────────────────────── 1. DATOS (el "modelo") ─────────────────────────
var contacts = new List<Contact>();      // aquí viven los datos; al cerrar el programa se pierden (lo arregla la lección 26)
int nextId = 1;                          // contador para dar un Id único a cada contacto

// ───────────────────────── 2. MENÚ PRINCIPAL ─────────────────────────
while (true)
{
    Console.WriteLine();
    Console.WriteLine("=== AGENDA ===");
    Console.WriteLine("1) Crear   2) Listar   3) Ver detalle   4) Editar   5) Borrar   6) Buscar   0) Salir");
    int option = AskInt("Opción: ", 0, 6);

    switch (option)
    {
        case 1: Create(); break;
        case 2: List(contacts); break;
        case 3: Detail(); break;
        case 4: Edit(); break;
        case 5: Delete(); break;
        case 6: Search(); break;
        case 0:
            Console.WriteLine("Hasta luego.");
            return;
    }
}

// ───────────────────────── 3. LAS CUATRO OPERACIONES ─────────────────────────

// C = CREATE: pedir datos, validarlos, crear el objeto y añadirlo a la lista
void Create()
{
    Console.WriteLine("-- Nuevo contacto --");
    string name = AskText("Nombre: ", Validate.Name);
    string email = AskText("Correo: ", Validate.Email);
    string phone = AskText("Teléfono (opcional): ", Validate.Phone, optional: true);
    string city = AskText("Ciudad (opcional): ", _ => null, optional: true);

    var contact = new Contact(nextId++, name, email, phone, city);
    contacts.Add(contact);
    Console.WriteLine($"Contacto creado con id {contact.Id}.");
}

// R = READ (1): listar todos
static void List(IReadOnlyList<Contact> list)
{
    if (list.Count == 0)
    {
        Console.WriteLine("(no hay contactos)");               // SIEMPRE tratar el caso "vacío"
        return;
    }
    Console.WriteLine($"{"Id",3}  {"Nombre",-18} {"Correo",-22} {"Teléfono",-16} Ciudad");
    foreach (var c in list.OrderBy(c => c.Name, StringComparer.CurrentCultureIgnoreCase))
        Console.WriteLine($"{c.Id,3}  {c.Name,-18} {c.Email,-22} {c.Phone,-16} {c.City}");
    Console.WriteLine($"{list.Count} contacto(s)");
}

// R = READ (2): un solo contacto por su Id
void Detail()
{
    Contact? contact = AskContact("Id del contacto: ");
    if (contact is null) return;
    Console.WriteLine($"Id: {contact.Id}\nNombre: {contact.Name}\nCorreo: {contact.Email}\nTeléfono: {contact.Phone}\nCiudad: {contact.City}");
}

// U = UPDATE: mostrar el valor actual entre corchetes; Enter lo conserva
void Edit()
{
    Contact? old = AskContact("Id del contacto a editar: ");
    if (old is null) return;

    Console.WriteLine("(pulsa Enter para dejar el valor actual)");
    string name = AskText($"Nombre [{old.Name}]: ", Validate.Name, keep: old.Name);
    string email = AskText($"Correo [{old.Email}]: ", Validate.Email, keep: old.Email);
    string phone = AskText($"Teléfono [{old.Phone}]: ", Validate.Phone, keep: old.Phone);
    string city = AskText($"Ciudad [{old.City}]: ", _ => null, keep: old.City);

    // Los records son inmutables: "with" crea una COPIA con los cambios y sustituimos la antigua
    var updated = old with { Name = name, Email = email, Phone = phone, City = city };
    contacts[contacts.IndexOf(old)] = updated;
    Console.WriteLine("Contacto actualizado.");
}

// D = DELETE: pedir CONFIRMACIÓN antes de destruir datos
void Delete()
{
    Contact? contact = AskContact("Id del contacto a borrar: ");
    if (contact is null) return;
    if (AskYesNo($"¿Borrar a {contact.Name}?"))
    {
        contacts.Remove(contact);
        Console.WriteLine("Borrado.");
    }
    else Console.WriteLine("Cancelado.");
}

// Búsqueda: filtrar la lista por un texto
void Search()
{
    string text = AskText("Buscar en nombre, correo o ciudad: ", _ => null);
    var found = contacts.Where(c => new[] { c.Name, c.Email, c.City }.Any(field => field.Contains(text, StringComparison.CurrentCultureIgnoreCase))).ToList();
    List(found);
}

// ───────────────────────── 4. FORMULARIOS REUTILIZABLES (captura) ─────────────────────────

Contact? AskContact(string question)
{
    int id = AskInt(question, 1, int.MaxValue);
    Contact? contact = contacts.FirstOrDefault(c => c.Id == id);
    if (contact is null) Console.WriteLine($"No existe el contacto {id}.");
    return contact;
}

static string? ReadLine()
{
    string? line = Console.ReadLine();
    if (Console.IsInputRedirected && line is not null) Console.WriteLine(line);   // eco para la demostración
    return line;
}

static int AskInt(string question, int min, int max)
{
    while (true)
    {
        Console.Write(question);
        if (!int.TryParse(ReadLine(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int value)) Console.WriteLine("Escribe un número.");
        else if (value < min || value > max) Console.WriteLine($"Entre {min} y {max}.");
        else return value;
    }
}

// validate devuelve null si el texto es válido, o el mensaje de error si no lo es
static string AskText(string question, Func<string, string?> validate, bool optional = false, string? keep = null)
{
    while (true)
    {
        Console.Write(question);
        string text = (ReadLine() ?? "").Trim();
        if (text.Length == 0)
        {
            if (keep is not null) return keep;           // edición: Enter conserva
            if (optional) return "";                      // campo opcional: vacío vale
        }
        string? error = text.Length == 0 ? "Este dato es obligatorio." : validate(text);
        if (error is null) return text;
        Console.WriteLine("  " + error);
    }
}

static bool AskYesNo(string question)
{
    while (true)
    {
        Console.Write(question + " (s/n): ");
        string answer = (ReadLine() ?? "").Trim().ToLowerInvariant();
        if (answer is "s" or "si" or "sí") return true;
        if (answer is "n" or "no") return false;
    }
}

// ───────────────────────── 5. TIPOS ─────────────────────────
record Contact(int Id, string Name, string Email, string Phone, string City);

static class Validate                                     // reglas de validación en un solo sitio
{
    public static string? Name(string text) => text.Length < 2 ? "El nombre necesita al menos 2 letras." : null;
    public static string? Email(string text) => text.Contains('@') && text.Split('@')[1].Contains('.') ? null : "Correo no válido (ejemplo: ana@correo.es).";
    public static string? Phone(string text) => text.All(ch => char.IsDigit(ch) || ch is ' ' or '+') ? null : "Solo dígitos, espacios y +.";
}
