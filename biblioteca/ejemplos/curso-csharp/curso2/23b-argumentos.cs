// Lección 23 (b): datos que llegan SIN preguntar: argumentos, variables de entorno y opciones
// Ejecutar:  dotnet run 23b-argumentos.cs -- --name Ana --times 2 --shout
//            (lo que va después de "--" son argumentos de TU programa, no de dotnet)
//
// "args" existe automáticamente en programas con instrucciones de nivel superior.
Console.WriteLine($"Recibí {args.Length} argumentos: [{string.Join("] [", args)}]");

// Lectura manual de opciones: --clave valor   y   --bandera
string name = "mundo";
int times = 1;
bool shout = false;
var rest = new List<string>();

for (int i = 0; i < args.Length; i++)
{
    switch (args[i])
    {
        case "--name" or "-n":
            name = Next(ref i, "--name");               // el valor es el argumento siguiente
            break;
        case "--times" or "-t":
            if (!int.TryParse(Next(ref i, "--times"), out times) || times < 1)
            {
                Console.Error.WriteLine("error: --times necesita un entero positivo");   // Error estándar: no mezcla con los datos
                return 2;                                                                 // código de salida distinto de 0 = error
            }
            break;
        case "--shout":                                  // bandera: sin valor
            shout = true;
            break;
        case "--help" or "-h":
            Console.WriteLine("uso: programa [--name NOMBRE] [--times N] [--shout] [archivos...]");
            return 0;
        default:
            if (args[i].StartsWith('-')) { Console.Error.WriteLine($"opción desconocida: {args[i]}"); return 2; }
            rest.Add(args[i]);                           // argumento posicional
            break;
    }
}

string Next(ref int index, string option)
{
    if (index + 1 >= args.Length) throw new ArgumentException($"{option} necesita un valor");
    return args[++index];
}

// Variables de entorno: configuración que NO se escribe en el código (rutas, claves, modos)
string greeting = Environment.GetEnvironmentVariable("GREETING") ?? "Hola";      // con valor por defecto
string? home = Environment.GetEnvironmentVariable("HOME");
Console.WriteLine($"GREETING = {greeting}; HOME existe: {home is not null}");

for (int n = 1; n <= times; n++)
{
    string text = $"{greeting}, {name}";
    Console.WriteLine(shout ? text.ToUpper() + "!" : text);
}
if (rest.Count > 0) Console.WriteLine($"Archivos recibidos: {string.Join(", ", rest)}");
Console.WriteLine($"Nombre del programa: {Path.GetFileNameWithoutExtension(Environment.ProcessPath)}");
return 0;                                                // este valor es el "código de salida" del proceso
