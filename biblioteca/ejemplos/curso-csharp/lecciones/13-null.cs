// Lección 13: null y tipos que admiten null
// Con <Nullable>enable</Nullable> (activo por defecto en las plantillas), el compilador distingue:
//   string   -> NUNCA debe ser null
//   string?  -> puede ser null
string name = "Ana";
string? maybe = null;
Console.WriteLine($"{name.Length} / {maybe?.Length ?? -1}");

// ?.  acceso condicional: si es null, el resultado es null y no falla
// ??  valor por defecto si es null
// ??= asignar solo si es null
string? city = null;
Console.WriteLine(city?.ToUpper() ?? "sin ciudad");
city ??= "Madrid";
Console.WriteLine(city);

// El error clásico: NullReferenceException
try
{
    Console.WriteLine(Lookup("zzz")!.Length);   // ! = "te prometo que no es null" (el compilador deja de avisar, pero el fallo ocurre)
}
catch (NullReferenceException)
{
    Console.WriteLine("NullReferenceException: usamos algo que era null");
}

// Comprobar
string? input = Lookup("ana");
if (input is not null) Console.WriteLine(input.Length);   // dentro del if, el compilador sabe que no es null
if (input is { Length: > 2 }) Console.WriteLine("tiene más de 2 letras");   // patrón de propiedad
ArgumentNullException.ThrowIfNull(input);                  // lanza si es null (útil en parámetros)

// int? y otros tipos de valor que admiten null
int? age = null;
Console.WriteLine($"{age.HasValue} {age.GetValueOrDefault(-1)} {(age ?? 0) + 1}");
age = 30;
Console.WriteLine($"{age.Value} {age}");

// Diccionarios: TryGetValue evita null
var prices = new Dictionary<string, decimal> { ["pan"] = 1.2m };
Console.WriteLine(prices.TryGetValue("leche", out var p) ? p.ToString() : "no existe");

// Buenas prácticas
Console.WriteLine("""
    1. No devuelvas null para "no hay datos": devuelve una lista vacía.
    2. Si algo puede faltar, decláralo con ? para que el compilador te obligue a comprobarlo.
    3. Usa TryXxx(out ...) o un tipo Result en lugar de null + excepciones.
    """);
Console.WriteLine($"Lista vacía en lugar de null: {FindAll("nada").Count} elementos");

static string? Lookup(string key) => key == "ana" ? "Ana García" : null;
static List<string> FindAll(string text) => [];
