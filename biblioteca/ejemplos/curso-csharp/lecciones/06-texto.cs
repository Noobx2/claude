// Lección 6: texto (string, formato, StringBuilder)
using System.Globalization;
using System.Text;

// Fijamos la cultura para que el resultado sea el mismo en cualquier ordenador.
// Sin esta línea, el formato de números y fechas depende de la configuración regional del sistema.
CultureInfo.CurrentCulture = new CultureInfo("es-ES");

string name = "Ana García";
Console.WriteLine($"{name.Length} caracteres; mayúsculas: {name.ToUpper()}; minúsculas: {name.ToLower()}");
Console.WriteLine($"Contiene 'Gar': {name.Contains("Gar")}; empieza por 'A': {name.StartsWith("A")}; posición de 'G': {name.IndexOf('G')}");
Console.WriteLine($"[{"  hola  ".Trim()}] [{"  hola  ".TrimStart()}] [{"  hola  ".TrimEnd()}]");
Console.WriteLine($"{name.Substring(4)} | {name[..3]} | {name[^6..]} | {name.Replace("a", "@")}");
Console.WriteLine(string.Join(" - ", "uno,dos,tres".Split(',')));
Console.WriteLine($"{string.IsNullOrEmpty("")} {string.IsNullOrWhiteSpace("   ")}");

// Los string son INMUTABLES: cada "cambio" crea uno nuevo
string s = "a";
string t = s.ToUpper();
Console.WriteLine($"s sigue siendo '{s}', t es '{t}'");

// Interpolación y formatos
double price = 1234.5678;
int units = 7;
DateTime date = new(2026, 3, 9, 14, 5, 0);
Console.WriteLine($"Precio: {price:F2} | miles: {price:N2} | moneda: {price:C} | porcentaje: {0.256:P1} | relleno: {units:D4}");
Console.WriteLine($"Alineado: [{name,-12}] [{units,6}] [{price,10:F1}]");     // ancho negativo = izquierda
Console.WriteLine($"Fecha: {date:dd/MM/yyyy HH:mm} | {date:yyyy-MM-dd} | {date:dddd d 'de' MMMM}");

// La cultura cambia el formato
var spanish = CultureInfo.GetCultureInfo("es-ES");
var american = CultureInfo.GetCultureInfo("en-US");
Console.WriteLine($"es-ES: {price.ToString("N2", spanish)}  | en-US: {price.ToString("N2", american)}");
Console.WriteLine($"InvariantCulture (para archivos y APIs): {price.ToString(CultureInfo.InvariantCulture)}");

// Comparar textos
Console.WriteLine($"== distingue mayúsculas: {"Ana" == "ana"}; sin distinguir: {string.Equals("Ana", "ana", StringComparison.OrdinalIgnoreCase)}");
Console.WriteLine($"CompareTo: {string.Compare("a", "b")} (negativo = a va antes)");

// Cadenas literales
string path = @"C:\Users\ana\datos.txt";              // @ = textual: no hace falta duplicar las barras
string multiline = """
    Primera línea
      con sangría relativa
    "comillas" sin escapar
    """;                                             // raw string: ideal para JSON y SQL
Console.WriteLine(path);
Console.WriteLine(multiline);
Console.WriteLine("Con escapes: tab[\t] salto de línea \\n y comillas \"dobles\"");

// StringBuilder: para construir texto con muchos cambios
var builder = new StringBuilder();
for (int i = 1; i <= 5; i++) builder.Append(i).Append(',');
builder.Length--;                                    // quita la última coma
builder.Insert(0, "[").Append(']');
Console.WriteLine(builder);

// char y Unicode
Console.WriteLine($"{char.IsDigit('7')} {char.IsLetter('ñ')} {char.ToUpper('ñ')} {(int)'A'} {(char)66}");
string emoji = "Hola 👋";
Console.WriteLine($"Length = {emoji.Length} (un emoji ocupa 2 unidades UTF-16)");

// Recorrer y contar
string phrase = "la casa de la pradera";
var frequency = phrase.Where(char.IsLetter).GroupBy(c => c).OrderByDescending(g => g.Count()).ThenBy(g => g.Key).Take(3);
Console.WriteLine(string.Join(", ", frequency.Select(g => $"{g.Key}:{g.Count()}")));
Console.WriteLine($"palabras: {phrase.Split(' ').Length}; invertida: {new string(phrase.Reverse().ToArray())}");
