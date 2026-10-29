// Lección 23 (c): leer datos que llegan por una TUBERÍA (stdin), como hacen grep, sort o wc
// Ejecutar:  printf 'uno dos\ntres cuatro cinco\n' | dotnet run 23c-tuberia.cs
//            dotnet run 23c-tuberia.cs < archivo.txt
//            dotnet run 23c-tuberia.cs          (sin redirección: espera a que escribas; Ctrl+D termina)
if (!Console.IsInputRedirected)
{
    Console.WriteLine("Escribe líneas y termina con Ctrl+D (Linux/macOS) o Ctrl+Z + Enter (Windows):");
}

int lines = 0, words = 0, chars = 0;
var frequency = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

// Console.In es un TextReader: se puede recorrer línea a línea hasta que devuelva null
string? line;
while ((line = Console.In.ReadLine()) is not null)
{
    lines++;
    chars += line.Length + 1;                                   // +1 por el salto de línea
    foreach (string word in line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
    {
        words++;
        frequency[word] = frequency.GetValueOrDefault(word) + 1;
    }
}

Console.WriteLine($"{lines} líneas, {words} palabras, {chars} caracteres");
foreach (var (word, count) in frequency.OrderByDescending(p => p.Value).ThenBy(p => p.Key).Take(3))
    Console.WriteLine($"  {word,-8} {count}");

// Alternativa para entradas pequeñas: leer TODO de golpe
// string everything = Console.In.ReadToEnd();
