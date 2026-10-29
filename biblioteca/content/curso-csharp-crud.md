# Curso de C# · Parte II: capturar, mostrar y gestionar datos (CRUD)

Esta parte responde a la pregunta que de verdad importa al programar una aplicación: **¿cómo entran los datos, cómo se enseñan y cómo se gestionan?** Todo programa útil hace estas tres cosas, y casi todos se reducen a un **CRUD**:

| Letra | Operación | Qué hace | En SQL | En una API web |
|---|---|---|---|---|
| **C** | *Create* (crear) | Añadir un dato nuevo | `INSERT` | `POST` |
| **R** | *Read* (leer) | Listar, ver un detalle, buscar | `SELECT` | `GET` |
| **U** | *Update* (modificar) | Cambiar un dato existente | `UPDATE` | `PUT` / `PATCH` |
| **D** | *Delete* (borrar) | Eliminar un dato | `DELETE` | `DELETE` |

Lo que aprendes aquí **se repite igual** en una aplicación de escritorio, en una API web y en Unity: cambian los botones, no la idea.

*Verificado: 2026-10-29 con .NET SDK 10.0.113 en Linux. Todas las lecciones se ejecutaron. La entrada de teclado se simuló con archivos (`< entrada.txt`) para que la salida sea siempre la misma; en tu terminal escribes tú.*

## Cómo leer estas lecciones

Cada lección sigue siempre el mismo orden, para que sepas dónde mirar:

1. **Qué vas a aprender** y **la idea** (sin código).
2. **El código completo**, con **comentarios en verde y cursiva** y las palabras clave en azul.
3. **Cómo se lee, paso a paso:** un recorrido numerado por las partes importantes.
4. **Resultado real** de ejecutarlo.
5. **Errores típicos** y **ejercicios**.

> **Cómo se ve el código en este documento:** los **comentarios** (`// ...`) van en **verde cursiva**; los **textos** (`"..."`) en rojo; las **palabras reservadas** (`if`, `class`, `return`) en azul; los **tipos** en verde azulado. Así el comentario nunca se confunde con el código.

---

## Lección 23 · Capturar datos: todas las formas

### Qué vas a aprender

Los **once** caminos por los que un dato entra en un programa de consola, y la idea que los une: **leer, comprobar, repetir hasta que sea válido**.

### La idea

Un usuario **nunca** escribe lo que esperas. Escribe vacío, letras donde van números, fechas imposibles o cierra la entrada con Ctrl+D. Un formulario robusto sigue siempre este ciclo:

```text
        ┌──────────────────────────────────┐
        ▼                                  │
  Preguntar → Leer → ¿Es válido? ──no──→ Explicar el error
                          │
                         sí
                          ▼
                    Devolver el valor
```

Todo el código de abajo es **ese ciclo** aplicado a un tipo de dato distinto.

### Las once formas

| # | Forma | Cuándo | Pieza clave |
|---|---|---|---|
| 1 | Texto libre | Nombre, comentario | `Console.ReadLine()` + `Trim()` |
| 2 | Texto obligatorio | Un dato que no puede faltar | Bucle `while (true)` |
| 3 | Entero con rango | Edad, cantidad, opción de menú | `int.TryParse` + comprobar `min`/`max` |
| 4 | Decimal (coma o punto) | Precios | `Replace(',', '.')` + `InvariantCulture` |
| 5 | Fecha con formato | Cumpleaños, vencimientos | `DateOnly.TryParseExact` |
| 6 | Sí / No | Confirmar un borrado | `switch` sobre `"s"`, `"n"` |
| 7 | Elegir de una lista | Prioridad, categoría | Mostrar `1) 2) 3)` y devolver el valor |
| 8 | Varios datos en una línea | `Ana, 31` | `Split(',')` con `TrimEntries` |
| 9 | Varias líneas | Notas, mensajes | Leer hasta una línea vacía |
| 10 | Valor por defecto | Editar sin reescribir todo | `[valor actual]` y Enter lo conserva |
| 11 | Pregunta genérica | Cualquier tipo | `Func<string, (bool, T)>` |

**Código completo** (archivo `23-captura.cs`):

```csharp
// Lección 23: capturar datos por consola (todas las formas)
// Ejecutar interactivo:      dotnet run 23-captura.cs
// Ejecutar con entrada fija: dotnet run 23-captura.cs < 23-captura.in     (así la salida es siempre la misma)
using System.Globalization;

// ──────────────────────────────────────────────────────────────────────────────
// PIEZA BASE: leer una línea.
// Console.ReadLine() devuelve null cuando no hay más entrada (Ctrl+D / fin del archivo redirigido).
// Si la entrada viene de un archivo, el teclado no la "muestra": la repetimos nosotros para que
// el resultado de esta lección se lea como una conversación.
// ──────────────────────────────────────────────────────────────────────────────
static string? ReadLine()
{
    string? line = Console.ReadLine();
    if (Console.IsInputRedirected && line is not null) Console.WriteLine(line);   // eco solo con entrada simulada
    return line;
}

// FORMA 1: texto libre. Trim() quita espacios sobrantes; ?? "" evita trabajar con null.
Console.Write("1) Tu nombre: ");
string name = (ReadLine() ?? "").Trim();
Console.WriteLine($"   -> '{name}' ({name.Length} letras)");

// FORMA 2: texto obligatorio (repite hasta que no esté vacío)
static string AskRequired(string question)
{
    while (true)
    {
        Console.Write(question);
        string text = (ReadLine() ?? "").Trim();
        if (text.Length > 0) return text;
        Console.WriteLine("   Es obligatorio. Inténtalo otra vez.");
    }
}
string city = AskRequired("2) Ciudad (obligatoria): ");
Console.WriteLine($"   -> {city}");

// FORMA 3: número entero con rango (TryParse no lanza excepción si el texto no es un número)
static int AskInt(string question, int min, int max)
{
    while (true)
    {
        Console.Write(question);
        string? text = ReadLine();
        if (!int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
            Console.WriteLine("   Eso no es un número entero.");
        else if (value < min || value > max)
            Console.WriteLine($"   Debe estar entre {min} y {max}.");
        else
            return value;
    }
}
int age = AskInt("3) Edad (0-120): ", 0, 120);
Console.WriteLine($"   -> {age}");

// FORMA 4: decimal, aceptando coma o punto (el usuario español escribe "12,50")
static decimal AskDecimal(string question)
{
    while (true)
    {
        Console.Write(question);
        string text = (ReadLine() ?? "").Trim().Replace(',', '.');                  // normalizamos la coma
        if (decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal value) && value >= 0)
            return value;
        Console.WriteLine("   Escribe un importe válido, por ejemplo 12,50.");
    }
}
decimal price = AskDecimal("4) Precio: ");
Console.WriteLine($"   -> {price.ToString("N2", CultureInfo.GetCultureInfo("es-ES"))} €");

// FORMA 5: fecha con formato exacto
static DateOnly AskDate(string question, string format = "dd/MM/yyyy")
{
    while (true)
    {
        Console.Write(question);
        if (DateOnly.TryParseExact(ReadLine(), format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            return date;
        Console.WriteLine($"   Formato esperado: {format.ToLower()}.");
    }
}
DateOnly birthday = AskDate("5) Fecha de nacimiento (dd/mm/aaaa): ");
Console.WriteLine($"   -> {birthday.ToString("dddd d 'de' MMMM 'de' yyyy", CultureInfo.GetCultureInfo("es-ES"))}");   // la cultura pone los nombres en español

// FORMA 6: sí / no
static bool AskYesNo(string question)
{
    while (true)
    {
        Console.Write(question + " (s/n): ");
        switch ((ReadLine() ?? "").Trim().ToLowerInvariant())
        {
            case "s" or "si" or "sí" or "y" or "yes": return true;
            case "n" or "no": return false;
        }
        Console.WriteLine("   Responde s o n.");
    }
}
bool newsletter = AskYesNo("6) ¿Quieres recibir novedades?");
Console.WriteLine($"   -> {newsletter}");

// FORMA 7: elegir de una lista (el usuario escribe el número; el programa devuelve el valor)
static T AskChoice<T>(string question, IReadOnlyList<T> options)
{
    Console.WriteLine(question);
    for (int i = 0; i < options.Count; i++) Console.WriteLine($"     {i + 1}) {options[i]}");
    int choice = AskInt("   Elige una opción: ", 1, options.Count);
    return options[choice - 1];
}
Priority priority = AskChoice("7) Prioridad:", Enum.GetValues<Priority>());
Console.WriteLine($"   -> {priority} (valor {(int)priority})");

// FORMA 8: varios datos en una sola línea ("nombre, edad, ciudad")
Console.Write("8) Escribe 'nombre, edad' separados por coma: ");
string[] parts = (ReadLine() ?? "").Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
if (parts.Length == 2 && int.TryParse(parts[1], out int partAge))
    Console.WriteLine($"   -> {parts[0]} tiene {partAge} años");
else
    Console.WriteLine("   Formato incorrecto");

// FORMA 9: varias líneas hasta una línea vacía
Console.WriteLine("9) Escribe notas (una línea vacía para terminar):");
var notes = new List<string>();
while (true)
{
    string line = (ReadLine() ?? "").Trim();
    if (line.Length == 0) break;
    notes.Add(line);
}
Console.WriteLine($"   -> {notes.Count} notas: {string.Join(" | ", notes)}");

// FORMA 10: valor por defecto (Enter acepta lo que se propone entre corchetes)
static string AskWithDefault(string question, string current)
{
    Console.Write($"{question} [{current}]: ");
    string text = (ReadLine() ?? "").Trim();
    return text.Length == 0 ? current : text;
}
string email = AskWithDefault("10) Correo", "ana@example.com");
Console.WriteLine($"   -> {email}");

// FORMA 11: una pregunta genérica que sirve para CUALQUIER tipo (la herramienta que acabarás usando)
static T Ask<T>(string question, Func<string, (bool Ok, T Value)> parse, string error)
{
    while (true)
    {
        Console.Write(question);
        var (ok, value) = parse(ReadLine() ?? "");
        if (ok) return value;
        Console.WriteLine("   " + error);
    }
}
var port = Ask("11) Puerto (1-65535): ",
    text => int.TryParse(text, out int p) && p is >= 1 and <= 65535 ? (true, p) : (false, 0),   // (¿válido?, valor)
    "Puerto no válido.");
Console.WriteLine($"   -> {port}");

Console.WriteLine("\nFin. La entrada se terminó o el usuario no escribió más.");
Console.WriteLine($"¿Hay más entrada? ReadLine devuelve: {(Console.ReadLine() is null ? "null (fin de la entrada)" : "texto")}");

enum Priority { Low = 1, Medium = 2, High = 3 }
```

**Entrada que se le dio** (cada línea es lo que el usuario teclea, **incluidos errores a propósito**):

```text
  Ana García  

zz
abc
200
31
12,5
2026-02-03
03/02/1995
quizá
s
9
2
Ana, 31
primera nota
segunda nota

ana@correo.es
99999
8080
```

### Cómo se lee, paso a paso

1. **`ReadLine()` propia (líneas iniciales):** `Console.ReadLine()` devuelve `null` si no hay más entrada. Con una entrada redirigida el teclado no «escribe» en pantalla, así que **repetimos** la línea para que la salida se lea como una conversación.
2. **`AskRequired`:** un `while (true)` que solo sale con `return` cuando el texto no está vacío. Es la plantilla de **todas** las demás.
3. **`AskInt`:** `int.TryParse` **no lanza excepción** (devuelve `true`/`false`). Se usa `NumberStyles.Integer` e `InvariantCulture` para que el resultado **no dependa del idioma del sistema**.
4. **`AskDecimal`:** el usuario español escribe `12,5`. Se cambia la coma por punto **antes** de convertir, y se convierte con cultura invariante.
5. **`AskDate`:** `TryParseExact` exige el **formato exacto**. Un `2026-02-03` se rechaza porque se esperaba `dd/MM/yyyy`.
6. **`AskChoice<T>`:** una función **genérica**: sirve para elegir de cualquier lista. Enseña `1) 2) 3)` y devuelve el valor, no el número.
7. **`Ask<T>`:** la más general. Recibe **una función** que dice si el texto es válido. Con ella se escribe cualquier validación nueva en una línea.
8. **Fin de la entrada:** al final, `ReadLine()` devuelve `null`. Un programa serio **debe contemplarlo** (en la lección 27 se hace con una excepción propia).

### Resultado real

```text
1) Tu nombre:   Ana García  
   -> 'Ana García' (10 letras)
2) Ciudad (obligatoria): 
   Es obligatorio. Inténtalo otra vez.
2) Ciudad (obligatoria): zz
   -> zz
3) Edad (0-120): abc
   Eso no es un número entero.
3) Edad (0-120): 200
   Debe estar entre 0 y 120.
3) Edad (0-120): 31
   -> 31
4) Precio: 12,5
   -> 12,50 €
5) Fecha de nacimiento (dd/mm/aaaa): 2026-02-03
   Formato esperado: dd/mm/yyyy.
5) Fecha de nacimiento (dd/mm/aaaa): 03/02/1995
   -> viernes 3 de febrero de 1995
6) ¿Quieres recibir novedades? (s/n): quizá
   Responde s o n.
6) ¿Quieres recibir novedades? (s/n): s
   -> True
7) Prioridad:
     1) Low
     2) Medium
     3) High
   Elige una opción: 9
   Debe estar entre 1 y 3.
   Elige una opción: 2
   -> Medium (valor 2)
8) Escribe 'nombre, edad' separados por coma: Ana, 31
   -> Ana tiene 31 años
9) Escribe notas (una línea vacía para terminar):
primera nota
segunda nota

   -> 2 notas: primera nota | segunda nota
10) Correo [ana@example.com]: ana@correo.es
   -> ana@correo.es
11) Puerto (1-65535): 99999
   Puerto no válido.
11) Puerto (1-65535): 8080
   -> 8080

Fin. La entrada se terminó o el usuario no escribió más.
¿Hay más entrada? ReadLine devuelve: null (fin de la entrada)
```

Fíjate en cada error: **`Es obligatorio`**, **`Eso no es un número entero`**, **`Debe estar entre 0 y 120`**, **`Formato esperado`**. El programa **explica** el fallo y **vuelve a preguntar**; nunca se cae.

### Datos que llegan sin preguntar: argumentos y variables de entorno

No todos los datos se piden en pantalla. Los programas de herramientas (como `git` o `dotnet`) reciben **argumentos** (`--name Ana --times 2`) y leen **variables de entorno**. Código (`23b-argumentos.cs`):

```csharp
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
```

**Cómo se ejecuta** (lo que va después de `--` son argumentos de **tu programa**):

```bash
dotnet run 23b-argumentos.cs -- --name Ana --times 2 --shout datos.txt notas.txt
GREETING=Buenas dotnet run 23b-argumentos.cs -- -n Luis
dotnet run 23b-argumentos.cs -- --times cero
```

**Resultados reales:**

```text
Recibí 7 argumentos: [--name] [Ana] [--times] [2] [--shout] [datos.txt] [notas.txt]
GREETING = Hola; HOME existe: True
HOLA, ANA!
HOLA, ANA!
Archivos recibidos: datos.txt, notas.txt
Nombre del programa: 23b-argumentos
```

```text
Recibí 2 argumentos: [-n] [Luis]
GREETING = Buenas; HOME existe: True
Buenas, Luis
Nombre del programa: 23b-argumentos
```

```text
código de salida: 2
```

| Pieza | Significado |
|---|---|
| `args` | Matriz con los argumentos; existe sola en programas con instrucciones de nivel superior |
| `--name Ana` | Opción **con valor**: el valor es el argumento siguiente |
| `--shout` | **Bandera** (*flag*): activa algo; no lleva valor |
| `datos.txt notas.txt` | Argumentos **posicionales** (sin guion) |
| `Console.Error.WriteLine` | Escribe en el **error estándar**: no se mezcla con los datos si rediriges la salida |
| `return 2;` | **Código de salida**: `0` = bien, distinto de `0` = error (lo usan los scripts) |
| `Environment.GetEnvironmentVariable` | Lee una variable de entorno; devuelve `null` si no existe |

### Datos que llegan por una tubería (`stdin`)

Los programas de terminal pueden **encadenarse** (`cat archivo | miprograma`). Código (`23c-tuberia.cs`):

```csharp
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
```

```bash
printf 'uno dos dos\ntres cuatro dos tres\n' | dotnet run 23c-tuberia.cs
```

```text
2 líneas, 7 palabras, 33 caracteres
  dos      3
  tres     2
  cuatro   1
```

| Pieza | Significado |
|---|---|
| `Console.IsInputRedirected` | `true` si la entrada viene de una tubería o archivo, no del teclado |
| `Console.In.ReadLine()` | Lee la siguiente línea; `null` al terminar |
| `Console.In.ReadToEnd()` | Lee **todo** de golpe (solo para entradas pequeñas) |

### Teclas sueltas y contraseñas (`ReadKey`)

Para ocultar una contraseña o esperar una sola tecla se usa `Console.ReadKey`. **Solo funciona con un teclado real**, no con entrada redirigida. Código (`23d-teclas.cs`):

```csharp
// Lección 23 (d): leer TECLAS sueltas (ReadKey): contraseñas ocultas, "pulsa una tecla", menús de una letra
// Console.ReadKey NO funciona con entrada redirigida (da error): solo con un teclado/terminal real.
if (Console.IsInputRedirected)
{
    Console.WriteLine("Esta lección necesita un teclado real (no una entrada redirigida).");
    return;
}

Console.Write("Contraseña: ");
var password = new System.Text.StringBuilder();
while (true)
{
    ConsoleKeyInfo key = Console.ReadKey(intercept: true);   // intercept: true = NO muestra la tecla en pantalla
    if (key.Key == ConsoleKey.Enter) break;
    if (key.Key == ConsoleKey.Backspace)
    {
        if (password.Length > 0)
        {
            password.Length--;                                // borrar el último carácter guardado
            Console.Write("\b \b");                           // y borrar el asterisco de la pantalla
        }
        continue;
    }
    if (!char.IsControl(key.KeyChar))                         // ignorar teclas especiales (flechas, F1...)
    {
        password.Append(key.KeyChar);
        Console.Write('*');                                   // mostramos un asterisco por cada carácter
    }
}
Console.WriteLine();
Console.WriteLine($"Longitud de la contraseña: {password.Length} (el contenido NO se muestra ni se guarda en el registro)");

Console.Write("¿Continuar? [s/n] ");
ConsoleKeyInfo answer = Console.ReadKey();                    // sin intercept: la tecla se ve
Console.WriteLine();
Console.WriteLine(char.ToLower(answer.KeyChar) == 's' ? "Continuamos." : "Cancelado.");
```

Para ejecutarlo en este entorno sin teclado, se usó una **terminal simulada** que «pulsó» `c l a v e x`, **Retroceso**, `o`, `1` y `Enter`. Resultado real:

```text
Contraseña: *******
Longitud de la contraseña: 7 (el contenido NO se muestra ni se guarda en el registro)
¿Continuar? [s/n] s
Continuamos.
```

| Pieza | Significado |
|---|---|
| `Console.ReadKey(intercept: true)` | Lee una tecla **sin mostrarla** |
| `ConsoleKeyInfo.Key` / `.KeyChar` | Qué tecla fue / qué carácter produce |
| `"\b \b"` | Retroceso, espacio, retroceso: **borra un carácter de la pantalla** |
| `char.IsControl` | Ignora teclas especiales (flechas, F1…) |

> **Una contraseña no se imprime ni se guarda en registros.** Aquí solo se muestra su longitud.

### Errores típicos

| Error | Causa | Solución |
|---|---|---|
| El programa se cae con `FormatException` | Usaste `int.Parse` con entrada del usuario | `int.TryParse` |
| Un precio `12,50` se lee como 1250 | Cultura del sistema distinta | `InvariantCulture` + normalizar la coma |
| Bucle **infinito** con la entrada terminada | `ReadLine()` devuelve `null` y vuelves a preguntar | Tratar `null` como fin y salir |
| Se salta la pregunta siguiente | Mezclaste `Read()` con `ReadLine()` | Usa solo `ReadLine()` |
| `ReadKey` lanza `InvalidOperationException` | La entrada está redirigida | Comprobar `Console.IsInputRedirected` |

> **Ejercicio 23.1:** escribe `AskEmail` que repita hasta que el texto tenga `@` y un punto después.
>
> **Ejercicio 23.2:** escribe `AskEnum<T>` que muestre las opciones de cualquier `enum`.
>
> **Ejercicio 23.3:** crea un programa que reciba `--from 1 --to 10 --step 2` y escriba los números.
>
> **Ejercicio 23.4:** lee por tubería una lista de números (uno por línea) y escribe la suma y la media.

---

## Lección 24 · Mostrar datos: tablas, formatos y vistas

### Qué vas a aprender

Los mismos datos se pueden **mostrar** de muchas maneras. Elegir bien la vista es la mitad de una buena interfaz.

### La idea

Hay **dos públicos**: **personas** (tablas alineadas, colores, resúmenes) y **programas** (JSON, CSV). Un buen programa separa **qué datos** de **cómo se presentan**. Por eso la última vista de esta lección usa una **interfaz** (`IProductView`): la misma pregunta con distintas respuestas.

**Reglas de una tabla legible:**

1. **Texto a la izquierda, números a la derecha.**
2. El **ancho** de cada columna es el de su dato **más largo**.
3. Una línea de **cabecera** y una de **separación**.
4. Siempre un caso **vacío** (`(no hay contactos)`), nunca una tabla sin filas.
5. Un **total** o recuento al final.

### Código (`24-mostrar.cs`)

```csharp
// Lección 24: mostrar datos en consola (tablas, formatos y varias "vistas" de los mismos datos)
// Ejecutar:  dotnet run 24-mostrar.cs
#:property PublishAot=false
using System.Globalization;
using System.Text;
using System.Text.Json;

var es = CultureInfo.GetCultureInfo("es-ES");
CultureInfo.CurrentCulture = es;                         // formato español para todo el programa
Console.OutputEncoding = Encoding.UTF8;                  // en Windows: para ver tildes, € y marcos

List<Product> products =
[
    new(1, "Teclado mecánico", "Periféricos", 59.90m, 25),
    new(2, "Ratón inalámbrico", "Periféricos", 24.50m, 60),
    new(3, "Monitor 27\" 4K", "Monitores", 329.00m, 4),
    new(4, "SSD 1 TB", "Almacenamiento", 79.90m, 0),
    new(5, "Cable HDMI 2 m", "Cables", 7.50m, 80),
];

// ───────── VISTA 1: lista simple (una línea por elemento) ─────────
Console.WriteLine("── Vista 1: lista simple ──");
foreach (var p in products) Console.WriteLine($"{p.Id}. {p.Name} - {p.Price:C}");

// ───────── VISTA 2: tabla alineada con anchos calculados ─────────
// Regla: texto a la izquierda, números a la derecha. El ancho de cada columna es el de su dato más largo.
Console.WriteLine("\n── Vista 2: tabla alineada ──");
Console.WriteLine(Table(
    ["Id", "Producto", "Categoría", "Precio", "Stock"],
    [false, false, false, true, true],                   // true = alinear a la derecha
    products.Select(p => (IReadOnlyList<string>)[p.Id.ToString(), p.Name, p.Category, p.Price.ToString("N2"), p.Stock.ToString()])));

// ───────── VISTA 3: tabla con marco (caracteres de dibujo) ─────────
Console.WriteLine("\n── Vista 3: tabla con marco ──");
Console.WriteLine(BoxTable(
    ["Producto", "Stock"],
    products.Select(p => (IReadOnlyList<string>)[Truncate(p.Name, 14), p.Stock == 0 ? "AGOTADO" : p.Stock.ToString()])));

// ───────── VISTA 4: ficha de detalle (un solo elemento) ─────────
Console.WriteLine("\n── Vista 4: detalle ──");
var detail = products[2];
Console.WriteLine($"{"Id",-10}{detail.Id}");
Console.WriteLine($"{"Nombre",-10}{detail.Name}");
Console.WriteLine($"{"Precio",-10}{detail.Price:C}");
Console.WriteLine($"{"Stock",-10}{detail.Stock} unidades" + (detail.Stock < 5 ? "   <-- poco stock" : ""));
Console.WriteLine($"{"Valor",-10}{detail.Price * detail.Stock:C}");

// ───────── VISTA 5: agrupado con totales ─────────
Console.WriteLine("\n── Vista 5: agrupado ──");
foreach (var group in products.GroupBy(p => p.Category).OrderBy(g => g.Key))
{
    Console.WriteLine($"{group.Key} ({group.Count()})");
    foreach (var p in group) Console.WriteLine($"   - {p.Name,-20}{p.Price,10:N2}");
    Console.WriteLine($"   {"Valor en stock",-22}{group.Sum(p => p.Price * p.Stock),10:N2}");
}
Console.WriteLine($"{"TOTAL EN ALMACÉN",-25}{products.Sum(p => p.Price * p.Stock),10:N2}");

// ───────── VISTA 6: paginación ─────────
Console.WriteLine("\n── Vista 6: paginación (2 por página) ──");
int pageSize = 2;
int pages = (int)Math.Ceiling(products.Count / (double)pageSize);
for (int page = 1; page <= pages; page++)
{
    Console.WriteLine($"Página {page}/{pages}");
    foreach (var p in products.Skip((page - 1) * pageSize).Take(pageSize)) Console.WriteLine($"   {p.Name}");
}

// ───────── VISTA 7: gráfico de barras con texto ─────────
Console.WriteLine("\n── Vista 7: barras ──");
int maxStock = products.Max(p => p.Stock);
foreach (var p in products)
    Console.WriteLine($"{Truncate(p.Name, 16),-16} {new string('█', (int)Math.Round(20.0 * p.Stock / maxStock))} {p.Stock}");

// ───────── VISTA 8: colores (solo se ven en un terminal; con redirección no hay códigos de color) ─────────
Console.WriteLine("\n── Vista 8: colores ──");
foreach (var p in products.Where(p => p.Stock < 5))
{
    var previous = Console.ForegroundColor;
    Console.ForegroundColor = p.Stock == 0 ? ConsoleColor.Red : ConsoleColor.Yellow;
    Console.WriteLine($"{(p.Stock == 0 ? "AGOTADO" : "BAJO")}: {p.Name}");
    Console.ForegroundColor = previous;                  // SIEMPRE restaurar el color
}

// ───────── VISTA 9: los mismos datos en JSON y CSV (para otros programas, no para personas) ─────────
Console.WriteLine("\n── Vista 9: JSON ──");
Console.WriteLine(JsonSerializer.Serialize(products.Take(2), new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping }));
Console.WriteLine("── Vista 10: CSV ──");
Console.WriteLine("id;name;category;price;stock");
foreach (var p in products.Take(2))
    Console.WriteLine($"{p.Id};{p.Name};{p.Category};{p.Price.ToString(CultureInfo.InvariantCulture)};{p.Stock}");

// ───────── VISTA 11: escoger el formato en tiempo de ejecución ─────────
Console.WriteLine("\n── Vista 11: --format ──");
foreach (string format in new[] { "lista", "json" })
{
    IProductView view = format == "json" ? new JsonView() : new ListView();   // Strategy: misma pregunta, distintas respuestas
    Console.WriteLine($"[{format}]");
    Console.WriteLine(view.Render(products.Take(2).ToList()));
}

// ───────── Utilidades ─────────
static string Truncate(string text, int max) => text.Length <= max ? text : text[..(max - 1)] + "…";

static string Table(IReadOnlyList<string> headers, IReadOnlyList<bool> right, IEnumerable<IReadOnlyList<string>> rows)
{
    var all = rows.ToList();
    int[] widths = headers.Select((h, c) => Math.Max(h.Length, all.Count == 0 ? 0 : all.Max(r => r[c].Length))).ToArray();
    string Line(IReadOnlyList<string> cells) => string.Join("  ", cells.Select((cell, c) => right[c] ? cell.PadLeft(widths[c]) : cell.PadRight(widths[c])));
    var sb = new StringBuilder();
    sb.AppendLine(Line(headers));
    sb.AppendLine(string.Join("  ", widths.Select(w => new string('-', w))));
    foreach (var row in all) sb.AppendLine(Line(row));
    return sb.ToString().TrimEnd();
}

static string BoxTable(IReadOnlyList<string> headers, IEnumerable<IReadOnlyList<string>> rows)
{
    var all = rows.ToList();
    int[] w = headers.Select((h, c) => Math.Max(h.Length, all.Max(r => r[c].Length))).ToArray();
    string Border(char l, char m, char r) => l + string.Join(m, w.Select(x => new string('─', x + 2))) + r;
    string Row(IReadOnlyList<string> cells) => "│" + string.Join("│", cells.Select((c, i) => " " + c.PadRight(w[i]) + " ")) + "│";
    var lines = new List<string> { Border('┌', '┬', '┐'), Row(headers), Border('├', '┼', '┤') };
    lines.AddRange(all.Select(Row));
    lines.Add(Border('└', '┴', '┘'));
    return string.Join("\n", lines);
}

record Product(int Id, string Name, string Category, decimal Price, int Stock);

interface IProductView { string Render(IReadOnlyList<Product> products); }
class ListView : IProductView
{
    public string Render(IReadOnlyList<Product> products) => string.Join("\n", products.Select(p => $"  • {p.Name} ({p.Stock})"));
}
class JsonView : IProductView
{
    public string Render(IReadOnlyList<Product> products) => JsonSerializer.Serialize(products.Select(p => new { p.Id, p.Name }), new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
}
```

### Cómo se lee, paso a paso

1. **`CultureInfo.CurrentCulture = es`:** fija el formato español para **todo** el programa (`59,90 €`, `1.316,00`). Sin esto el resultado dependería del ordenador.
2. **`Table(...)`:** calcula el ancho de cada columna con `Max(...)` y alinea con `PadLeft` (derecha) o `PadRight` (izquierda).
3. **`BoxTable(...)`:** igual, pero dibuja el marco con caracteres de línea (`┌─┬─┐`). `Truncate` añade `…` si un texto es demasiado largo.
4. **Detalle:** `{"Id",-10}` significa «en un campo de **10**, alineado a la **izquierda**». Un número positivo alinea a la derecha.
5. **Agrupado:** `GroupBy(...)` de LINQ y una suma por grupo.
6. **Paginación:** `Skip((página - 1) * tamaño).Take(tamaño)` es **la** fórmula de la paginación; sirve igual en SQL (`OFFSET`/`LIMIT`) y en una API.
7. **Barras:** `new string('█', n)` dibuja un gráfico con texto.
8. **Colores:** se **guarda** el color anterior y se **restaura** siempre.
9. **JSON y CSV:** formatos para otros programas. El JSON necesita `UnsafeRelaxedJsonEscaping` para no escribir `á` en lugar de `á`; el CSV usa `;` porque en español la coma es el separador decimal.
10. **`IProductView`:** el patrón **Strategy**: se elige en tiempo de ejecución cómo se muestra.

### Resultado real

```text
── Vista 1: lista simple ──
1. Teclado mecánico - 59,90 €
2. Ratón inalámbrico - 24,50 €
3. Monitor 27" 4K - 329,00 €
4. SSD 1 TB - 79,90 €
5. Cable HDMI 2 m - 7,50 €

── Vista 2: tabla alineada ──
Id  Producto           Categoría       Precio  Stock
--  -----------------  --------------  ------  -----
1   Teclado mecánico   Periféricos      59,90     25
2   Ratón inalámbrico  Periféricos      24,50     60
3   Monitor 27" 4K     Monitores       329,00      4
4   SSD 1 TB           Almacenamiento   79,90      0
5   Cable HDMI 2 m     Cables            7,50     80

── Vista 3: tabla con marco ──
┌────────────────┬─────────┐
│ Producto       │ Stock   │
├────────────────┼─────────┤
│ Teclado mecán… │ 25      │
│ Ratón inalámb… │ 60      │
│ Monitor 27" 4K │ 4       │
│ SSD 1 TB       │ AGOTADO │
│ Cable HDMI 2 m │ 80      │
└────────────────┴─────────┘

── Vista 4: detalle ──
Id        3
Nombre    Monitor 27" 4K
Precio    329,00 €
Stock     4 unidades   <-- poco stock
Valor     1.316,00 €

── Vista 5: agrupado ──
Almacenamiento (1)
   - SSD 1 TB                 79,90
   Valor en stock              0,00
Cables (1)
   - Cable HDMI 2 m            7,50
   Valor en stock            600,00
Monitores (1)
   - Monitor 27" 4K          329,00
   Valor en stock          1.316,00
Periféricos (2)
   - Teclado mecánico         59,90
   - Ratón inalámbrico        24,50
   Valor en stock          2.967,50
TOTAL EN ALMACÉN           4.883,50

── Vista 6: paginación (2 por página) ──
Página 1/3
   Teclado mecánico
   Ratón inalámbrico
Página 2/3
   Monitor 27" 4K
   SSD 1 TB
Página 3/3
   Cable HDMI 2 m

── Vista 7: barras ──
Teclado mecánico ██████ 25
Ratón inalámbri… ███████████████ 60
Monitor 27" 4K   █ 4
SSD 1 TB          0
Cable HDMI 2 m   ████████████████████ 80

── Vista 8: colores ──
BAJO: Monitor 27" 4K
AGOTADO: SSD 1 TB

── Vista 9: JSON ──
[
  {
    "Id": 1,
    "Name": "Teclado mecánico",
    "Category": "Periféricos",
    "Price": 59.90,
    "Stock": 25
  },
  {
    "Id": 2,
    "Name": "Ratón inalámbrico",
    "Category": "Periféricos",
    "Price": 24.50,
    "Stock": 60
  }
]
── Vista 10: CSV ──
id;name;category;price;stock
1;Teclado mecánico;Periféricos;59.90;25
2;Ratón inalámbrico;Periféricos;24.50;60

── Vista 11: --format ──
[lista]
  • Teclado mecánico (25)
  • Ratón inalámbrico (60)
[json]
[{"Id":1,"Name":"Teclado mecánico"},{"Id":2,"Name":"Ratón inalámbrico"}]
```

### Formatos de número y fecha que debes saber

| Formato | Ejemplo | Resultado (cultura es-ES) |
|---|---|---|
| `{x:C}` | `59.9m` | `59,90 €` |
| `{x:N2}` | `1316m` | `1.316,00` |
| `{x:F1}` | `3.14159` | `3,1` |
| `{x:P1}` | `0.256` | `25,6 %` |
| `{x:D4}` | `7` | `0007` |
| `{x,10}` | `"ab"` | `        ab` (a la derecha, ancho 10) |
| `{x,-10}` | `"ab"` | `ab        ` (a la izquierda) |
| `{fecha:dd/MM/yyyy}` | | `09/03/2026` |
| `{fecha:dddd d 'de' MMMM}` | | `lunes 9 de marzo` |

### Con una biblioteca: Spectre.Console

Dibujar tablas a mano enseña cómo funcionan, pero en un proyecto real se usa una biblioteca. En .NET 10 un archivo puede declarar su paquete con `#:package`:

```csharp
// Lección 24 (b): tablas bonitas con una biblioteca (Spectre.Console)
// En .NET 10, un archivo único puede declarar paquetes NuGet con la directiva #:package
#:package Spectre.Console@0.57.2
using Spectre.Console;

// Sin terminal real (salida redirigida) Spectre no conoce el ancho: lo fijamos a mano
if (Console.IsOutputRedirected) AnsiConsole.Profile.Width = 80;

var table = new Table().RoundedBorder().Title("Inventario");
table.AddColumn("Id");
table.AddColumn("Producto");
table.AddColumn(new TableColumn("Precio").RightAligned());
table.AddColumn(new TableColumn("Stock").RightAligned());
table.AddRow("1", "Teclado mecánico", "59,90", "25");
table.AddRow("2", "Ratón inalámbrico", "24,50", "60");
table.AddRow("[red]3[/]", "[red]SSD 1 TB[/]", "79,90", "[red]AGOTADO[/]");   // [red]...[/] = marcado de color
AnsiConsole.Write(table);

AnsiConsole.MarkupLine("[bold green]Listo.[/] Con la salida redirigida los colores se omiten solos.");

// Preguntas con validación incluida (en un terminal real son interactivas)
// var name = AnsiConsole.Prompt(new TextPrompt<string>("Nombre?").Validate(n => n.Length >= 2 ? ValidationResult.Success() : ValidationResult.Error("Mínimo 2 letras")));
// var option = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Elige").AddChoices("Crear", "Listar", "Salir"));
```

```text
                 Inventario                  
╭────┬───────────────────┬────────┬─────────╮
│ Id │ Producto          │ Precio │   Stock │
├────┼───────────────────┼────────┼─────────┤
│ 1  │ Teclado mecánico  │  59,90 │      25 │
│ 2  │ Ratón inalámbrico │  24,50 │      60 │
│ 3  │ SSD 1 TB          │  79,90 │ AGOTADO │
╰────┴───────────────────┴────────┴─────────╯
Listo. Con la salida redirigida los colores se omiten solos.
```

> **Un detalle que cuesta una hora si no se sabe:** con la salida **redirigida** (sin terminal real) Spectre no conoce el ancho de pantalla y pinta solo `…`. Se arregla con `AnsiConsole.Profile.Width = 80;`. Con terminal real no hace falta.

### Errores típicos

| Error | Causa | Solución |
|---|---|---|
| Las columnas se descuadran | Un texto con tildes o `€` más largo de lo previsto, o anchos fijos | Calcular el ancho con `Max(...)` |
| El `€` y las tildes salen como `?` | Codificación de la consola | `Console.OutputEncoding = Encoding.UTF8` |
| Los números salen con punto en vez de coma | Cultura invariante | `CultureInfo.CurrentCulture = ...` |
| El color se queda «pegado» | No se restauró | Guardar y restaurar `ForegroundColor` |
| Una lista enorme inunda la pantalla | Sin paginación | `Skip`/`Take` |

> **Ejercicio 24.1:** añade a `Table` una columna de **totales** al pie.
>
> **Ejercicio 24.2:** escribe `CsvView` y úsalo en el `--format` de la vista 11.
>
> **Ejercicio 24.3:** muestra los productos con paginación **interactiva** (`n` siguiente, `p` anterior, `q` salir).
>
> **Ejercicio 24.4:** pinta en rojo las filas con stock 0 y en amarillo las de stock menor que 5.

---

## Lección 25 · CRUD completo en consola (datos en memoria)

### Qué vas a aprender

A construir **de principio a fin** una aplicación con menú que **crea, lista, busca, edita y borra** contactos, con validación y confirmaciones. Es el esqueleto de **cualquier** aplicación de gestión.

### La idea

Una aplicación de gestión tiene **cuatro piezas**. Aprenderlas separadas es lo que te permitirá, después, cambiarlas una a una:

| Pieza | Qué es | Aquí |
|---|---|---|
| **Modelo** | La forma de los datos | `record Contact(...)` |
| **Almacén** | Dónde viven los datos | `List<Contact>` (en memoria) |
| **Operaciones** | Crear, leer, modificar, borrar | `Create()`, `List()`, `Edit()`, `Delete()` |
| **Interfaz** | Cómo se pide y se muestra | Menú y formularios de la consola |

**Reglas de un buen CRUD:**

- Cada contacto tiene un **identificador único** (`Id`) que nunca cambia ni se reutiliza.
- **Validar siempre** antes de guardar, y explicar el error.
- **Confirmar** antes de borrar.
- Al **editar**, mostrar el valor actual y dejar que `Enter` lo conserve.
- Tratar el caso **sin datos** y el caso **«no existe ese Id»**.

### Código (`25-crud-memoria.cs`)

```csharp
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
```

### Cómo se lee, paso a paso

1. **Modelo:** `record Contact(int Id, string Name, ...)`. Un `record` es **inmutable**: para cambiar algo se crea una **copia** (`with`).
2. **Datos:** `var contacts = new List<Contact>()` y `int nextId = 1`. `nextId++` entrega un Id nuevo cada vez.
3. **Bucle del menú:** `while (true)` + `switch`. Solo se sale con `return` en la opción `0`.
4. **Create:** pide los datos **con validación** (`Validate.Name`, `Validate.Email`…), crea el `Contact` y lo añade a la lista.
5. **Read:** `List(...)` imprime la tabla (o `(no hay contactos)`); `Detail()` muestra uno por su Id.
6. **Update:** `old with { Name = name, ... }` crea la copia modificada; `contacts[contacts.IndexOf(old)] = updated` sustituye la antigua.
7. **Delete:** primero `AskYesNo` y solo si responde `s` se llama a `contacts.Remove`.
8. **Search:** `Where(...)` con `Contains(..., StringComparison.CurrentCultureIgnoreCase)`: ignora mayúsculas.
9. **Formularios reutilizables:** `AskInt`, `AskText`, `AskYesNo` hacen **todo** el trabajo de pedir y validar; las operaciones solo los **usan**.
10. **`Validate`:** las reglas viven en **un solo sitio**. Para cambiar una regla se toca una línea.

### Resultado real (con la entrada simulada)

La conversación completa: crear con un correo inválido (se repite), un teléfono opcional vacío, ver un Id inexistente, editar conservando valores con `Enter`, buscar, cancelar un borrado, borrar y salir.

```text

=== AGENDA ===
1) Crear   2) Listar   3) Ver detalle   4) Editar   5) Borrar   6) Buscar   0) Salir
Opción: 2
(no hay contactos)

=== AGENDA ===
1) Crear   2) Listar   3) Ver detalle   4) Editar   5) Borrar   6) Buscar   0) Salir
Opción: 1
-- Nuevo contacto --
Nombre: Ana Ruiz
Correo: ana-correo
  Correo no válido (ejemplo: ana@correo.es).
Correo: ana@correo.es
Teléfono (opcional): 600 111 222
Ciudad (opcional): Madrid
Contacto creado con id 1.

=== AGENDA ===
1) Crear   2) Listar   3) Ver detalle   4) Editar   5) Borrar   6) Buscar   0) Salir
Opción: 1
-- Nuevo contacto --
Nombre: Luis Pérez
Correo: luis@correo.es
Teléfono (opcional): 
Ciudad (opcional): 
Contacto creado con id 2.

=== AGENDA ===
1) Crear   2) Listar   3) Ver detalle   4) Editar   5) Borrar   6) Buscar   0) Salir
Opción: 1
-- Nuevo contacto --
Nombre: Marta Gil
Correo: marta@correo.es
Teléfono (opcional): +34 611 000 111
Ciudad (opcional): Madrid
Contacto creado con id 3.

=== AGENDA ===
1) Crear   2) Listar   3) Ver detalle   4) Editar   5) Borrar   6) Buscar   0) Salir
Opción: 2
 Id  Nombre             Correo                 Teléfono         Ciudad
  1  Ana Ruiz           ana@correo.es          600 111 222      Madrid
  2  Luis Pérez         luis@correo.es                          
  3  Marta Gil          marta@correo.es        +34 611 000 111  Madrid
3 contacto(s)

=== AGENDA ===
1) Crear   2) Listar   3) Ver detalle   4) Editar   5) Borrar   6) Buscar   0) Salir
Opción: 3
Id del contacto: 2
Id: 2
Nombre: Luis Pérez
Correo: luis@correo.es
Teléfono: 
Ciudad: 

=== AGENDA ===
1) Crear   2) Listar   3) Ver detalle   4) Editar   5) Borrar   6) Buscar   0) Salir
Opción: 3
Id del contacto: 99
No existe el contacto 99.

=== AGENDA ===
1) Crear   2) Listar   3) Ver detalle   4) Editar   5) Borrar   6) Buscar   0) Salir
Opción: 4
Id del contacto a editar: 2
(pulsa Enter para dejar el valor actual)
Nombre [Luis Pérez]: Luis P. Díaz
Correo [luis@correo.es]: 
Teléfono []: abc
  Solo dígitos, espacios y +.
Teléfono []: 600 222 333
Ciudad []: Sevilla
Contacto actualizado.

=== AGENDA ===
1) Crear   2) Listar   3) Ver detalle   4) Editar   5) Borrar   6) Buscar   0) Salir
Opción: 6
Buscar en nombre, correo o ciudad: mad
 Id  Nombre             Correo                 Teléfono         Ciudad
  1  Ana Ruiz           ana@correo.es          600 111 222      Madrid
  3  Marta Gil          marta@correo.es        +34 611 000 111  Madrid
2 contacto(s)

=== AGENDA ===
1) Crear   2) Listar   3) Ver detalle   4) Editar   5) Borrar   6) Buscar   0) Salir
Opción: 5
Id del contacto a borrar: 3
¿Borrar a Marta Gil? (s/n): n
Cancelado.

=== AGENDA ===
1) Crear   2) Listar   3) Ver detalle   4) Editar   5) Borrar   6) Buscar   0) Salir
Opción: 5
Id del contacto a borrar: 3
¿Borrar a Marta Gil? (s/n): s
Borrado.

=== AGENDA ===
1) Crear   2) Listar   3) Ver detalle   4) Editar   5) Borrar   6) Buscar   0) Salir
Opción: 2
 Id  Nombre             Correo                 Teléfono         Ciudad
  1  Ana Ruiz           ana@correo.es          600 111 222      Madrid
  2  Luis P. Díaz       luis@correo.es         600 222 333      Sevilla
2 contacto(s)

=== AGENDA ===
1) Crear   2) Listar   3) Ver detalle   4) Editar   5) Borrar   6) Buscar   0) Salir
Opción: 7
Entre 0 y 6.
Opción: 0
Hasta luego.
```

### Errores típicos

| Error | Causa | Solución |
|---|---|---|
| Dos contactos con el mismo Id | Calcular el Id con `Count + 1` | Un contador (`nextId`) que **nunca baja** |
| «Se perdió todo al cerrar» | Datos solo en memoria | Lección 26 |
| El editor pisa lo que no querías | No se muestra el valor actual | `Nombre [valor actual]:` y Enter lo conserva |
| Se borra sin querer | Sin confirmación | `AskYesNo` antes de `Remove` |
| El programa se cae con un Id inexistente | `First` en lugar de `FirstOrDefault` | `FirstOrDefault` y comprobar `null` |

> **Ejercicio 25.1:** añade la opción **7) Ordenar** por nombre, por ciudad o por Id.
>
> **Ejercicio 25.2:** impide **correos duplicados** (la regla debe avisar antes de crear).
>
> **Ejercicio 25.3:** añade un campo **fecha de nacimiento** con `AskDate` y muestra la edad calculada.
>
> **Ejercicio 25.4:** añade **deshacer** del último borrado.

---

## Lección 26 · CRUD que guarda sus datos (JSON y CSV)

### Qué vas a aprender

Que los datos **sobrevivan** al cierre del programa, y a **cambiar el almacenamiento** sin tocar la lógica. Además, el CRUD se maneja por **argumentos** (`add`, `list`, `update`, `delete`), como las herramientas de terminal.

### La idea

La lógica **no debe saber cómo se guardan los datos**. Se define un **contrato** (`IContactStore`) con dos operaciones y se escriben **dos implementaciones** (JSON y CSV). Cambiar de una a otra es cambiar **una línea**.

| Formato | Ventaja | Cuidado |
|---|---|---|
| **JSON** | Estructurado, lo entiende cualquier lenguaje | Más verboso |
| **CSV** | Se abre en Excel | Comillas, separadores, codificación |

**Escritura segura:** se escribe primero en un archivo **temporal** y después se **renombra**. Si el programa muere a mitad, el archivo original **no queda a medias**.

### Código (`26-persistencia.cs`)

```csharp
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
```

### Cómo se lee, paso a paso

1. **Qué almacén se usa:** se decide **solo por la extensión** del archivo (`.csv` → CSV, otro → JSON). La variable `CONTACTS_FILE` elige el archivo.
2. **Al empezar:** `store.Load()`. Si el archivo no existe, devuelve una lista vacía (**primera ejecución**).
3. **`add`:** crea el contacto con el Id siguiente (`Max() + 1`) y **guarda tras cada cambio**.
4. **`update 2 --city Sevilla`:** `old with { City = ... }` cambia solo lo indicado.
5. **`export` e `import`:** pasar de un formato a otro. Al importar se **reasignan los Id** para no chocar con los existentes.
6. **`JsonContactStore.Save`:** archivo temporal + `File.Move(..., overwrite: true)`.
7. **`CsvContactStore`:** un campo con `;` o `"` va **entre comillas** y las comillas se **duplican** (`"Luis ""El Jefe"" Pérez"`). Leerlo exige un analizador propio: **`Split(';')` no basta**.
8. **BOM:** el CSV se guarda con *marca de orden de bytes* para que Excel reconozca las tildes. El JSON, **sin** BOM.

### Resultado real (varias ejecuciones seguidas)

Cada línea que empieza por `$` es **una ejecución distinta** del programa. Los datos **persisten** entre ellas.

```text
$ dotnet run 26-persistencia.cs -- list
0 contacto(s) en contactos.json
$ dotnet run 26-persistencia.cs -- add Ana Ruiz ana@correo.es --city Madrid
creado 1: Ana Ruiz
$ dotnet run 26-persistencia.cs -- add Luis "El Jefe" Pérez luis@correo.es
creado 2: Luis "El Jefe" Pérez
$ dotnet run 26-persistencia.cs -- add Marta; Gil marta@correo.es --city Bilbao
creado 3: Marta; Gil
$ dotnet run 26-persistencia.cs -- list
  1 Ana Ruiz       ana@correo.es    Madrid
  2 Luis "El Jefe" Pérez luis@correo.es   
  3 Marta; Gil     marta@correo.es  Bilbao
3 contacto(s) en contactos.json
$ dotnet run 26-persistencia.cs -- update 2 --city Sevilla
actualizado 2
$ dotnet run 26-persistencia.cs -- delete 1
borrado 1
$ dotnet run 26-persistencia.cs -- list
  2 Luis "El Jefe" Pérez luis@correo.es   Sevilla
  3 Marta; Gil     marta@correo.es  Bilbao
2 contacto(s) en contactos.json

$ cat contactos.json
[
  {
    "Id": 2,
    "Name": "Luis \"El Jefe\" Pérez",
    "Email": "luis@correo.es",
    "City": "Sevilla"
  },
  {
    "Id": 3,
    "Name": "Marta; Gil",
    "Email": "marta@correo.es",
    "City": "Bilbao"
  }
]
$ dotnet run 26-persistencia.cs -- export contactos.csv
exportados 2 a contactos.csv
$ cat contactos.csv
﻿id;name;email;city
2;"Luis ""El Jefe"" Pérez";luis@correo.es;Sevilla
3;"Marta; Gil";marta@correo.es;Bilbao
$ dotnet run 26-persistencia.cs -- import contactos.csv
importados 2
$ dotnet run 26-persistencia.cs -- list
  2 Luis "El Jefe" Pérez luis@correo.es   Sevilla
  3 Marta; Gil     marta@correo.es  Bilbao
  4 Luis "El Jefe" Pérez luis@correo.es   Sevilla
  5 Marta; Gil     marta@correo.es  Bilbao
4 contacto(s) en contactos.json
$ CONTACTS_FILE=otro.csv dotnet run 26-persistencia.cs -- add Pedro pedro@correo.es --city Cádiz
creado 1: Pedro
$ cat otro.csv
﻿id;name;email;city
1;Pedro;pedro@correo.es;Cádiz
$ dotnet run 26-persistencia.cs -- update 99 --city X
no existe 99
código de salida: 3
```

Fíjate en el CSV: `"Marta; Gil"` va entre comillas porque contiene un `;`, y `"Luis ""El Jefe"" Pérez"` tiene las comillas duplicadas. El primer carácter invisible del archivo es el **BOM**.

### Errores típicos

| Error | Causa | Solución |
|---|---|---|
| El CSV pierde las tildes en Excel | Sin BOM | `new UTF8Encoding(true)` |
| Una coma en un nombre rompe el CSV | Separar con `Split` sin respetar comillas | Analizador que entienda comillas |
| Archivo corrupto tras un fallo | Se escribió directo sobre el original | Temporal + renombrar |
| Los Id se repiten tras importar | Se copiaron tal cual | Reasignar al importar |
| JSON con `á` | Escapado por defecto | `UnsafeRelaxedJsonEscaping` |

> **Ejercicio 26.1:** añade `XmlContactStore` y úsalo cuando el archivo termine en `.xml`.
>
> **Ejercicio 26.2:** añade una copia de seguridad automática (`contactos.json.bak`) antes de cada guardado.
>
> **Ejercicio 26.3:** haz que `import` **omita** los contactos cuyo correo ya existe e indique cuántos omitió.

---

## Lección 27 · CRUD profesional en capas, con pruebas

### Qué vas a aprender

A pasar del programa «de un solo archivo» a una **solución profesional**: capas separadas, inyección de dependencias, errores del negocio y **22 pruebas automáticas**. Es el **Proyecto 1** del curso: **TaskFlow** (gestor de tareas).

### La idea

En las lecciones 25 y 26 la validación, el menú, el guardado y la lógica estaban **mezclados**. Eso funciona, pero **no se puede probar sin teclear** ni cambiar una pieza sin tocar las demás. La solución es dividir en **capas** con una **regla de dependencias**: *las flechas solo apuntan hacia dentro*.

```text
┌──────────────────────────────────────────────────────────────┐
│  TaskFlow.Console   (menús, formularios: ConsoleApp, Prompts)│  ← lo que ve el usuario
└──────────────┬───────────────────────────────┬───────────────┘
               │ usa                           │ se conecta (DI)
               ▼                               ▼
┌──────────────────────────────┐   ┌───────────────────────────┐
│  TaskFlow.Core               │◀──│  TaskFlow.Infrastructure  │
│  Domain: TaskItem, errores   │   │  JsonTaskRepository       │
│  Application: TaskService,   │   │  InMemoryTaskRepository   │
│  ITaskRepository, IClock     │   └───────────────────────────┘
└──────────────────────────────┘
        ▲ no depende de nadie
```

| Capa | Contiene | Sabe de… | **No** sabe de… |
|---|---|---|---|
| **Core / Domain** | `TaskItem`, `Priority`, excepciones | Nada | Consola, archivos, bases de datos |
| **Core / Application** | `TaskService`, `ITaskRepository`, `IClock` | Domain | Cómo se guarda; cómo se muestra |
| **Infrastructure** | `JsonTaskRepository`, `InMemoryTaskRepository` | Core | La consola |
| **Console** | `ConsoleApp`, `Prompts`, `IConsoleIO` | Core | Cómo se guarda |
| **Tests** | Pruebas de todo lo anterior | Todo | — |

**Tres decisiones que lo hacen probable:**

1. **`IConsoleIO`**: la consola es una **dependencia**; en las pruebas se sustituye por una falsa con la entrada escrita de antemano.
2. **`IClock`**: el **reloj** también es una dependencia; así se puede probar «una tarea que vence mañana» sin esperar un día.
3. **`ITaskRepository`**: el servicio no sabe si guarda en memoria o en JSON.

### La solución, archivo a archivo

**Crear la estructura:**

```bash
dotnet new sln -n TaskFlow
dotnet new classlib -n TaskFlow.Core           -o src/TaskFlow.Core
dotnet new classlib -n TaskFlow.Infrastructure -o src/TaskFlow.Infrastructure
dotnet new console  -n TaskFlow.Console        -o src/TaskFlow.Console
dotnet new xunit    -n TaskFlow.Tests          -o tests/TaskFlow.Tests
dotnet sln add src/TaskFlow.Core src/TaskFlow.Infrastructure src/TaskFlow.Console tests/TaskFlow.Tests
dotnet add src/TaskFlow.Infrastructure reference src/TaskFlow.Core
dotnet add src/TaskFlow.Console reference src/TaskFlow.Core src/TaskFlow.Infrastructure
dotnet add tests/TaskFlow.Tests reference src/TaskFlow.Core src/TaskFlow.Infrastructure src/TaskFlow.Console
dotnet add src/TaskFlow.Console package Microsoft.Extensions.DependencyInjection
```

Observa quién referencia a quién: **`Core` no referencia a nadie**.

**1. Dominio** (`Core/Domain/TaskItem.cs`):

```csharp
namespace TaskFlow.Domain;

public enum Priority { Low = 1, Medium = 2, High = 3 }

/// <summary>
/// Una tarea. Es un record: inmutable (para cambiarla se crea una copia con "with").
/// El dominio no sabe nada de consola, archivos ni bases de datos.
/// </summary>
public sealed record TaskItem(
    int Id,
    string Title,
    string? Description,
    Priority Priority,
    DateOnly? DueDate,
    bool Done,
    DateTime CreatedAt)
{
    /// <summary>Una tarea está vencida si tiene fecha, no está hecha y esa fecha ya pasó.</summary>
    public bool IsOverdue(DateOnly today) => !Done && DueDate is { } due && due < today;
}
```

**Errores del negocio** (`Core/Domain/Errors.cs`):

```csharp
namespace TaskFlow.Domain;

/// <summary>Los datos no cumplen las reglas del negocio (se muestra al usuario tal cual).</summary>
public sealed class ValidationException(string message) : Exception(message);

/// <summary>Se pidió algo que no existe.</summary>
public sealed class NotFoundException(string message) : Exception(message);
```

**2. Contratos** (`Core/Application/Ports.cs`): lo que el servicio **necesita** del exterior.

```csharp
using TaskFlow.Domain;

namespace TaskFlow.Application;

/// <summary>
/// "Puerto": el contrato que el servicio NECESITA. La implementación (memoria, JSON, base de datos)
/// vive fuera, así que se puede cambiar sin tocar la lógica.
/// </summary>
public interface ITaskRepository
{
    IReadOnlyList<TaskItem> GetAll();
    TaskItem? Find(int id);
    TaskItem Add(TaskItem task);     // asigna el Id
    void Update(TaskItem task);
    bool Remove(int id);
}

/// <summary>La hora actual, como dependencia: así las pruebas controlan el tiempo.</summary>
public interface IClock
{
    DateTime Now { get; }
    DateOnly Today => DateOnly.FromDateTime(Now);
}
```

**3. Las reglas y el CRUD** (`Core/Application/TaskService.cs`):

```csharp
using TaskFlow.Domain;

namespace TaskFlow.Application;

public sealed record NewTask(string Title, string? Description, Priority Priority, DateOnly? DueDate);
public sealed record TaskChanges(string? Title = null, string? Description = null, Priority? Priority = null, DateOnly? DueDate = null);
public sealed record TaskFilter(bool? Done = null, Priority? Priority = null, string? Text = null, bool OnlyOverdue = false);
public sealed record TaskStats(int Total, int Done, int Pending, int Overdue);

/// <summary>
/// Las reglas del negocio y las operaciones CRUD. Recibe sus dependencias por el constructor
/// (inyección de dependencias) y no conoce ni la consola ni cómo se guardan los datos.
/// </summary>
public sealed class TaskService(ITaskRepository repository, IClock clock)
{
    public TaskItem Add(NewTask input)
    {
        string title = ValidateTitle(input.Title);
        if (input.DueDate is { } due && due < clock.Today)
            throw new ValidationException("La fecha límite no puede estar en el pasado.");

        return repository.Add(new TaskItem(0, title, NullIfEmpty(input.Description), input.Priority, input.DueDate, false, clock.Now));
    }

    public TaskItem Get(int id) =>
        repository.Find(id) ?? throw new NotFoundException($"No existe la tarea {id}.");

    public IReadOnlyList<TaskItem> List(TaskFilter? filter = null)
    {
        filter ??= new TaskFilter();
        IEnumerable<TaskItem> query = repository.GetAll();
        if (filter.Done is { } done) query = query.Where(t => t.Done == done);
        if (filter.Priority is { } priority) query = query.Where(t => t.Priority == priority);
        if (!string.IsNullOrWhiteSpace(filter.Text))
            query = query.Where(t => t.Title.Contains(filter.Text, StringComparison.CurrentCultureIgnoreCase)
                                  || (t.Description?.Contains(filter.Text, StringComparison.CurrentCultureIgnoreCase) ?? false));
        if (filter.OnlyOverdue) query = query.Where(t => t.IsOverdue(clock.Today));

        // Primero las pendientes, luego por prioridad (alta antes) y por fecha límite
        return query.OrderBy(t => t.Done)
                    .ThenByDescending(t => t.Priority)
                    .ThenBy(t => t.DueDate ?? DateOnly.MaxValue)
                    .ThenBy(t => t.Id)
                    .ToList();
    }

    public TaskItem Update(int id, TaskChanges changes)
    {
        TaskItem current = Get(id);
        TaskItem updated = current with
        {
            Title = changes.Title is null ? current.Title : ValidateTitle(changes.Title),
            Description = changes.Description is null ? current.Description : NullIfEmpty(changes.Description),
            Priority = changes.Priority ?? current.Priority,
            DueDate = changes.DueDate ?? current.DueDate,
        };
        repository.Update(updated);
        return updated;
    }

    public TaskItem SetDone(int id, bool done)
    {
        TaskItem updated = Get(id) with { Done = done };
        repository.Update(updated);
        return updated;
    }

    public void Delete(int id)
    {
        if (!repository.Remove(id)) throw new NotFoundException($"No existe la tarea {id}.");
    }

    public TaskStats GetStats()
    {
        var all = repository.GetAll();
        int done = all.Count(t => t.Done);
        return new TaskStats(all.Count, done, all.Count - done, all.Count(t => t.IsOverdue(clock.Today)));
    }

    private static string ValidateTitle(string? title)
    {
        string trimmed = title?.Trim() ?? "";
        if (trimmed.Length < 3) throw new ValidationException("El título necesita al menos 3 caracteres.");
        if (trimmed.Length > 80) throw new ValidationException("El título no puede superar 80 caracteres.");
        return trimmed;
    }

    private static string? NullIfEmpty(string? text) => string.IsNullOrWhiteSpace(text) ? null : text.Trim();
}
```

**4. Almacenes** (`Infrastructure`):

```csharp
using TaskFlow.Application;
using TaskFlow.Domain;

namespace TaskFlow.Infrastructure;

/// <summary>Implementación en memoria: ideal para pruebas.</summary>
public class InMemoryTaskRepository : ITaskRepository
{
    protected readonly List<TaskItem> Items = [];
    private int _lastId;

    public IReadOnlyList<TaskItem> GetAll() => Items.ToList();          // copia: nadie modifica la lista interna

    public TaskItem? Find(int id) => Items.FirstOrDefault(t => t.Id == id);

    public virtual TaskItem Add(TaskItem task)
    {
        var saved = task with { Id = ++_lastId };
        Items.Add(saved);
        return saved;
    }

    public virtual void Update(TaskItem task)
    {
        int index = Items.FindIndex(t => t.Id == task.Id);
        if (index < 0) throw new NotFoundException($"No existe la tarea {task.Id}.");
        Items[index] = task;
    }

    public virtual bool Remove(int id) => Items.RemoveAll(t => t.Id == id) > 0;

    protected void SetLastId(int id) => _lastId = id;
}
```

```csharp
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
```

**5. La consola como dependencia** (`Console/IConsoleIO.cs`):

```csharp
namespace TaskFlow.Console;

/// <summary>
/// La consola como dependencia. En producción se usa la real; en las pruebas, una falsa con
/// la entrada escrita de antemano y la salida guardada en una lista.
/// </summary>
/// <summary>Se acabó la entrada (Ctrl+D, archivo terminado): hay que salir, no seguir preguntando.</summary>
public sealed class EndOfInputException : Exception;

public interface IConsoleIO
{
    string? ReadLine();
    void Write(string text);
    void WriteLine(string text = "");
}

public sealed class SystemConsoleIO : IConsoleIO
{
    public string? ReadLine()
    {
        string? line = System.Console.ReadLine();
        if (System.Console.IsInputRedirected && line is not null) System.Console.WriteLine(line);   // eco (solo con entrada simulada)
        return line;
    }
    public void Write(string text) => System.Console.Write(text);
    public void WriteLine(string text = "") => System.Console.WriteLine(text);
}
```

**6. Los formularios** (`Console/Prompts.cs`): todas las formas de capturar de la lección 23, reunidas.

```csharp
using System.Globalization;
using TaskFlow.Domain;

namespace TaskFlow.Console;

/// <summary>Todas las formas de capturar datos, en un solo sitio y reutilizables.</summary>
public sealed class Prompts(IConsoleIO io)
{
    // Toda lectura pasa por aquí: si no hay más entrada, se corta el formulario con una excepción propia.
    // Sin esto, un bucle "repetir hasta que sea válido" sería INFINITO con la entrada terminada.
    private string ReadLine() => io.ReadLine() ?? throw new EndOfInputException();

    /// <summary>Texto. Si "current" tiene valor y el usuario pulsa Enter, se conserva (edición).</summary>
    public string Text(string question, string? current = null, bool required = false)
    {
        while (true)
        {
            io.Write(current is null ? $"{question}: " : $"{question} [{current}]: ");
            string text = ReadLine().Trim();
            if (text.Length > 0) return text;
            if (current is not null) return current;
            if (!required) return "";
            io.WriteLine("  Es obligatorio.");
        }
    }

    public int Int(string question, int min, int max)
    {
        while (true)
        {
            io.Write($"{question}: ");
            if (!int.TryParse(ReadLine(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int value)) io.WriteLine("  Escribe un número entero.");
            else if (value < min || value > max) io.WriteLine($"  Debe estar entre {min} y {max}.");
            else return value;
        }
    }

    public bool YesNo(string question)
    {
        while (true)
        {
            io.Write($"{question} (s/n): ");
            string answer = ReadLine().Trim().ToLowerInvariant();
            if (answer is "s" or "si" or "sí") return true;
            if (answer is "n" or "no") return false;
        }
    }

    /// <summary>Fecha opcional en formato dd/MM/aaaa; vacío = sin fecha (o conservar la actual).</summary>
    public DateOnly? Date(string question, DateOnly? current = null)
    {
        while (true)
        {
            io.Write(current is null ? $"{question} (dd/mm/aaaa, vacío = ninguna): " : $"{question} [{current:dd/MM/yyyy}]: ");
            string text = ReadLine().Trim();
            if (text.Length == 0) return current;
            if (DateOnly.TryParseExact(text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)) return date;
            io.WriteLine("  Formato esperado: dd/mm/aaaa.");
        }
    }

    public Priority Priority(string question, Priority? current = null)
    {
        io.WriteLine(question + (current is null ? "" : $" (actual: {Name(current.Value)})"));
        io.WriteLine("  1) Baja   2) Media   3) Alta");
        while (true)
        {
            io.Write(current is null ? "  Elige: " : $"  Elige [Enter = {Name(current.Value)}]: ");
            string text = ReadLine().Trim();
            if (text.Length == 0 && current is not null) return current.Value;
            if (int.TryParse(text, out int n) && n is >= 1 and <= 3) return (Priority)n;
            io.WriteLine("  Escribe 1, 2 o 3.");
        }
    }

    private static string Name(Priority p) => p switch { Domain.Priority.High => "Alta", Domain.Priority.Medium => "Media", _ => "Baja" };
}
```

**7. La interfaz** (`Console/ConsoleApp.cs`): pregunta, llama al servicio, muestra. **No contiene reglas del negocio.**

```csharp
using TaskFlow.Application;
using TaskFlow.Domain;

namespace TaskFlow.Console;

/// <summary>La interfaz de consola: pregunta, llama al servicio y muestra el resultado. No contiene reglas del negocio.</summary>
public sealed class ConsoleApp(TaskService service, IConsoleIO io, IClock clock)
{
    private readonly Prompts _ask = new(io);

    public void Run()
    {
        try { Loop(); }
        catch (EndOfInputException) { io.WriteLine(); io.WriteLine("(fin de la entrada)"); }   // Ctrl+D o archivo terminado: salir sin error
    }

    private void Loop()
    {
        while (true)
        {
            io.WriteLine();
            io.WriteLine("=== TASKFLOW ===");
            io.WriteLine("1) Nueva  2) Listar  3) Detalle  4) Editar  5) Completar  6) Borrar  7) Buscar  8) Resumen  0) Salir");
            int option = _ask.Int("Opción", 0, 8);
            if (option == 0) { io.WriteLine("Hasta luego."); return; }

            try
            {
                switch (option)
                {
                    case 1: Create(); break;
                    case 2: Show(service.List()); break;
                    case 3: Detail(); break;
                    case 4: Edit(); break;
                    case 5: Complete(); break;
                    case 6: Delete(); break;
                    case 7: Show(service.List(new TaskFilter(Text: _ask.Text("Texto a buscar", required: true)))); break;
                    case 8: Summary(); break;
                }
            }
            catch (ValidationException e) { io.WriteLine($"  ✗ {e.Message}"); }     // errores del negocio: se explican al usuario
            catch (NotFoundException e) { io.WriteLine($"  ✗ {e.Message}"); }
        }
    }

    private void Create()
    {
        var input = new NewTask(
            _ask.Text("Título", required: true),
            _ask.Text("Descripción (opcional)"),
            _ask.Priority("Prioridad"),
            _ask.Date("Fecha límite"));
        TaskItem task = service.Add(input);                   // si algo no cumple las reglas, lanza ValidationException
        io.WriteLine($"  ✓ Tarea {task.Id} creada.");
    }

    private void Detail()
    {
        TaskItem t = service.Get(_ask.Int("Id", 1, int.MaxValue));
        io.WriteLine($"  Id:          {t.Id}");
        io.WriteLine($"  Título:      {t.Title}");
        io.WriteLine($"  Descripción: {t.Description ?? "(sin descripción)"}");
        io.WriteLine($"  Prioridad:   {Label(t.Priority)}");
        io.WriteLine($"  Límite:      {(t.DueDate is { } d ? d.ToString("dd/MM/yyyy") : "(sin fecha)")}{(t.IsOverdue(clock.Today) ? "  ¡VENCIDA!" : "")}");
        io.WriteLine($"  Estado:      {(t.Done ? "hecha" : "pendiente")}");
        io.WriteLine($"  Creada:      {t.CreatedAt:dd/MM/yyyy HH:mm}");
    }

    private void Edit()
    {
        TaskItem current = service.Get(_ask.Int("Id a editar", 1, int.MaxValue));
        io.WriteLine("  (Enter conserva el valor actual)");
        var changes = new TaskChanges(
            _ask.Text("Título", current.Title),
            _ask.Text("Descripción", current.Description ?? ""),
            _ask.Priority("Prioridad", current.Priority),
            _ask.Date("Fecha límite", current.DueDate));
        service.Update(current.Id, changes);
        io.WriteLine("  ✓ Tarea actualizada.");
    }

    private void Complete()
    {
        TaskItem t = service.SetDone(_ask.Int("Id a completar", 1, int.MaxValue), true);
        io.WriteLine($"  ✓ «{t.Title}» marcada como hecha.");
    }

    private void Delete()
    {
        TaskItem t = service.Get(_ask.Int("Id a borrar", 1, int.MaxValue));
        if (_ask.YesNo($"¿Borrar «{t.Title}»?")) { service.Delete(t.Id); io.WriteLine("  ✓ Borrada."); }
        else io.WriteLine("  Cancelado.");
    }

    private void Summary()
    {
        TaskStats s = service.GetStats();
        io.WriteLine($"  Total {s.Total} · hechas {s.Done} · pendientes {s.Pending} · vencidas {s.Overdue}");
    }

    private void Show(IReadOnlyList<TaskItem> tasks)
    {
        if (tasks.Count == 0) { io.WriteLine("  (sin resultados)"); return; }
        io.WriteLine($"  {"Id",3}  {"",1} {"Título",-26} {"Prio",-6} {"Límite",-10}");
        foreach (var t in tasks)
            io.WriteLine($"  {t.Id,3}  {(t.Done ? "✓" : " ")} {Truncate(t.Title, 26),-26} {Label(t.Priority),-6} {(t.DueDate is { } d ? d.ToString("dd/MM/yyyy") : ""),-10}{(t.IsOverdue(clock.Today) ? " ¡vencida!" : "")}");
        io.WriteLine($"  {tasks.Count} tarea(s)");
    }

    private static string Label(Priority p) => p switch { Priority.High => "Alta", Priority.Medium => "Media", _ => "Baja" };
    private static string Truncate(string text, int max) => text.Length <= max ? text : text[..(max - 1)] + "…";
}
```

**8. La raíz de composición** (`Console/Program.cs`): **el único sitio** que conoce todas las piezas.

```csharp
using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Application;
using TaskFlow.Console;
using TaskFlow.Infrastructure;

// "Raíz de composición": el ÚNICO sitio que conoce todas las piezas y las conecta.
// Para cambiar el almacenamiento (JSON -> base de datos) se cambia solo una línea de aquí.
string dataFile = Environment.GetEnvironmentVariable("TASKFLOW_FILE") ?? "tasks.json";

var services = new ServiceCollection();
services.AddSingleton<IClock, SystemClock>();
services.AddSingleton<ITaskRepository>(_ => new JsonTaskRepository(dataFile));
services.AddSingleton<IConsoleIO, SystemConsoleIO>();
services.AddSingleton<TaskService>();
services.AddSingleton<ConsoleApp>();

using var provider = services.BuildServiceProvider();
provider.GetRequiredService<ConsoleApp>().Run();

file sealed class SystemClock : IClock                     // "file": visible solo en este archivo
{
    public DateTime Now => DateTime.Now;
}
```

### Cómo se lee, paso a paso

1. **`TaskItem` es un `record`**: inmutable. `IsOverdue(today)` recibe «hoy» como **parámetro** en vez de leer el reloj: así es predecible y probable.
2. **`TaskService`** recibe `ITaskRepository` e `IClock` **por el constructor** (inyección de dependencias). Todas las reglas están aquí: título de 3 a 80 caracteres, fecha límite no pasada, orden de la lista.
3. **Dos excepciones del negocio** (`ValidationException`, `NotFoundException`): son **errores esperados** y la interfaz los **traduce a un mensaje**; no son fallos del programa.
4. **`JsonTaskRepository` hereda de `InMemoryTaskRepository`** y solo añade «guardar tras cada cambio». La lógica de la lista no se repite.
5. **`Prompts`** recibe un `IConsoleIO` y **no usa `Console` directamente**. Toda lectura pasa por un único método que lanza `EndOfInputException` si no hay más entrada: así **ningún bucle de «repetir hasta que sea válido» puede ser infinito**.
6. **`ConsoleApp.Run`** llama a `Loop()` dentro de un `try`: si la entrada termina, escribe `(fin de la entrada)` y sale limpiamente.
7. **`Program.cs`** registra las piezas (`AddSingleton<...>`) y arranca. Cambiar de JSON a una base de datos sería **cambiar la línea** del repositorio.

### Resultado real: dos ejecuciones distintas

**Primera ejecución** (crea dos tareas, una con una fecha imposible que se rechaza; completa una, busca y pide el resumen):

```text
$ TASKFLOW_FILE=/tmp/tf.json dotnet run --project src/TaskFlow.Console < sesion1.txt

=== TASKFLOW ===
1) Nueva  2) Listar  3) Detalle  4) Editar  5) Completar  6) Borrar  7) Buscar  8) Resumen  0) Salir
Opción: 1
Título: Preparar el curso de C#
Descripción (opcional): Lecciones 1 a 5
Prioridad
  1) Baja   2) Media   3) Alta
  Elige: 3
Fecha límite (dd/mm/aaaa, vacío = ninguna): 31/12/2030
  ✓ Tarea 1 creada.

=== TASKFLOW ===
1) Nueva  2) Listar  3) Detalle  4) Editar  5) Completar  6) Borrar  7) Buscar  8) Resumen  0) Salir
Opción: 1
Título: Comprar cable HDMI
Descripción (opcional): 
Prioridad
  1) Baja   2) Media   3) Alta
  Elige: 1
Fecha límite (dd/mm/aaaa, vacío = ninguna): 31/02/2030
  Formato esperado: dd/mm/aaaa.
Fecha límite (dd/mm/aaaa, vacío = ninguna): 15/11/2030
  ✓ Tarea 2 creada.

=== TASKFLOW ===
1) Nueva  2) Listar  3) Detalle  4) Editar  5) Completar  6) Borrar  7) Buscar  8) Resumen  0) Salir
Opción: 2
   Id    Título                     Prio   Límite    
    1    Preparar el curso de C#    Alta   31/12/2030
    2    Comprar cable HDMI         Baja   15/11/2030
  2 tarea(s)

=== TASKFLOW ===
1) Nueva  2) Listar  3) Detalle  4) Editar  5) Completar  6) Borrar  7) Buscar  8) Resumen  0) Salir
Opción: 5
Id a completar: 1
  ✓ «Preparar el curso de C#» marcada como hecha.

=== TASKFLOW ===
1) Nueva  2) Listar  3) Detalle  4) Editar  5) Completar  6) Borrar  7) Buscar  8) Resumen  0) Salir
Opción: 7
Texto a buscar: cable
   Id    Título                     Prio   Límite    
    2    Comprar cable HDMI         Baja   15/11/2030
  1 tarea(s)

=== TASKFLOW ===
1) Nueva  2) Listar  3) Detalle  4) Editar  5) Completar  6) Borrar  7) Buscar  8) Resumen  0) Salir
Opción: 8
  Total 2 · hechas 1 · pendientes 1 · vencidas 0

=== TASKFLOW ===
1) Nueva  2) Listar  3) Detalle  4) Editar  5) Completar  6) Borrar  7) Buscar  8) Resumen  0) Salir
Opción: 0
Hasta luego.
```

**Segunda ejecución** (otro proceso: los datos **siguen ahí**; lista, edita conservando valores, cancela un borrado, borra y pide el resumen). Al final, el archivo:

```text
$ TASKFLOW_FILE=/tmp/tf.json dotnet run --project src/TaskFlow.Console < sesion2.txt     # OTRA ejecución: los datos siguen ahí

=== TASKFLOW ===
1) Nueva  2) Listar  3) Detalle  4) Editar  5) Completar  6) Borrar  7) Buscar  8) Resumen  0) Salir
Opción: 2
   Id    Título                     Prio   Límite    
    2    Comprar cable HDMI         Baja   15/11/2030
    1  ✓ Preparar el curso de C#    Alta   31/12/2030
  2 tarea(s)

=== TASKFLOW ===
1) Nueva  2) Listar  3) Detalle  4) Editar  5) Completar  6) Borrar  7) Buscar  8) Resumen  0) Salir
Opción: 3
Id: 2
  Id:          2
  Título:      Comprar cable HDMI
  Descripción: (sin descripción)
  Prioridad:   Baja
  Límite:      15/11/2030
  Estado:      pendiente
  Creada:      29/10/2026 17:39

=== TASKFLOW ===
1) Nueva  2) Listar  3) Detalle  4) Editar  5) Completar  6) Borrar  7) Buscar  8) Resumen  0) Salir
Opción: 4
Id a editar: 2
  (Enter conserva el valor actual)
Título [Comprar cable HDMI]: Comprar cable HDMI 2 m
Descripción []: 
Prioridad (actual: Baja)
  1) Baja   2) Media   3) Alta
  Elige [Enter = Baja]: 
Fecha límite [15/11/2030]: 
  ✓ Tarea actualizada.

=== TASKFLOW ===
1) Nueva  2) Listar  3) Detalle  4) Editar  5) Completar  6) Borrar  7) Buscar  8) Resumen  0) Salir
Opción: 6
Id a borrar: 1
¿Borrar «Preparar el curso de C#»? (s/n): n
  Cancelado.

=== TASKFLOW ===
1) Nueva  2) Listar  3) Detalle  4) Editar  5) Completar  6) Borrar  7) Buscar  8) Resumen  0) Salir
Opción: 6
Id a borrar: 1
¿Borrar «Preparar el curso de C#»? (s/n): s
  ✓ Borrada.

=== TASKFLOW ===
1) Nueva  2) Listar  3) Detalle  4) Editar  5) Completar  6) Borrar  7) Buscar  8) Resumen  0) Salir
Opción: 2
   Id    Título                     Prio   Límite    
    2    Comprar cable HDMI 2 m     Baja   15/11/2030
  1 tarea(s)

=== TASKFLOW ===
1) Nueva  2) Listar  3) Detalle  4) Editar  5) Completar  6) Borrar  7) Buscar  8) Resumen  0) Salir
Opción: 8
  Total 1 · hechas 0 · pendientes 1 · vencidas 0

=== TASKFLOW ===
1) Nueva  2) Listar  3) Detalle  4) Editar  5) Completar  6) Borrar  7) Buscar  8) Resumen  0) Salir
Opción: 0
Hasta luego.

$ cat /tmp/tf.json
[
  {
    "Id": 2,
    "Title": "Comprar cable HDMI 2 m",
    "Description": null,
    "Priority": "Low",
    "DueDate": "2030-11-15",
    "Done": false,
    "CreatedAt": "2026-10-29T17:39:42.7525931+00:00"
  }
]
```

### Las pruebas

Cuatro tipos de prueba, cada una con un objetivo:

| Archivo | Qué prueba | Truco que usa |
|---|---|---|
| `TaskServiceTests` | Las **reglas del negocio** | `FakeClock` controla «hoy» |
| `RepositoryContractTests` | Que **todos los almacenes** cumplen el mismo contrato | `[Theory]` + `[MemberData]`: la misma prueba contra memoria y JSON |
| `ConsoleAppTests` | **Conversaciones enteras** con la consola | `FakeConsole`: entrada escrita, salida acumulada |
| (dentro de las anteriores) | Errores, límites y entrada terminada | `Assert.Throws<T>` |

Los dobles de prueba:

```csharp
using TaskFlow.Application;
using TaskFlow.Console;
using TaskFlow.Infrastructure;

namespace TaskFlow.Tests;

/// <summary>Reloj controlable: el "hoy" de las pruebas es siempre el mismo.</summary>
public sealed class FakeClock(DateTime now) : IClock
{
    public DateTime Now { get; set; } = now;
}

/// <summary>Consola falsa: la entrada está escrita de antemano y la salida se acumula para poder comprobarla.</summary>
public sealed class FakeConsole(params string[] input) : IConsoleIO
{
    private readonly Queue<string> _input = new(input);
    private readonly List<string> _lines = [];
    private string _pending = "";

    public string? ReadLine() => _input.Count > 0 ? _input.Dequeue() : null;
    public void Write(string text) => _pending += text;
    public void WriteLine(string text = "") { _lines.Add(_pending + text); _pending = ""; }

    public string Output => string.Join("\n", _lines) + _pending;
}

public static class TestFactory
{
    public static readonly DateTime Today = new(2026, 3, 10, 9, 0, 0);

    public static TaskService NewService(out FakeClock clock, ITaskRepository? repository = null)
    {
        clock = new FakeClock(Today);
        return new TaskService(repository ?? new InMemoryTaskRepository(), clock);
    }
}
```

Pruebas de las reglas:

```csharp
using TaskFlow.Application;
using TaskFlow.Domain;

namespace TaskFlow.Tests;

public class TaskServiceTests
{
    [Fact]
    public void Add_AssignsIdTrimsTitleAndStartsPending()
    {
        var service = TestFactory.NewService(out _);

        var task = service.Add(new NewTask("  Comprar pan  ", "  ", Priority.Medium, null));

        Assert.Equal(1, task.Id);
        Assert.Equal("Comprar pan", task.Title);
        Assert.Null(task.Description);                    // una descripción en blanco se guarda como "sin descripción"
        Assert.False(task.Done);
        Assert.Equal(TestFactory.Today, task.CreatedAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("ab")]
    public void Add_RejectsShortTitles(string title)
    {
        var service = TestFactory.NewService(out _);
        var error = Assert.Throws<ValidationException>(() => service.Add(new NewTask(title, null, Priority.Low, null)));
        Assert.Contains("3 caracteres", error.Message);
    }

    [Fact]
    public void Add_RejectsDueDateInThePast()
    {
        var service = TestFactory.NewService(out _);
        Assert.Throws<ValidationException>(() => service.Add(new NewTask("Tarea", null, Priority.Low, new DateOnly(2026, 3, 9))));
        service.Add(new NewTask("Tarea", null, Priority.Low, new DateOnly(2026, 3, 10)));    // hoy sí vale
    }

    [Fact]
    public void List_OrdersPendingFirstThenByPriority()
    {
        var service = TestFactory.NewService(out _);
        service.Add(new NewTask("Baja", null, Priority.Low, null));
        service.Add(new NewTask("Alta", null, Priority.High, null));
        var hecha = service.Add(new NewTask("Hecha alta", null, Priority.High, null));
        service.SetDone(hecha.Id, true);

        var titles = service.List().Select(t => t.Title);

        Assert.Equal(["Alta", "Baja", "Hecha alta"], titles);
    }

    [Fact]
    public void List_FiltersByTextAndOverdue()
    {
        var service = TestFactory.NewService(out var clock);
        service.Add(new NewTask("Pagar luz", null, Priority.Low, new DateOnly(2026, 3, 11)));
        service.Add(new NewTask("Llamar al banco", "por la luz", Priority.Low, null));
        Assert.Equal(2, service.List(new TaskFilter(Text: "LUZ")).Count);        // mayúsculas y descripción incluidas

        clock.Now = clock.Now.AddDays(5);                                        // el tiempo pasa
        var overdue = service.List(new TaskFilter(OnlyOverdue: true));
        Assert.Single(overdue);
        Assert.Equal("Pagar luz", overdue[0].Title);
    }

    [Fact]
    public void Update_ChangesOnlyWhatIsGiven()
    {
        var service = TestFactory.NewService(out _);
        var task = service.Add(new NewTask("Original", "nota", Priority.Low, null));

        var updated = service.Update(task.Id, new TaskChanges(Priority: Priority.High));

        Assert.Equal(Priority.High, updated.Priority);
        Assert.Equal("Original", updated.Title);
        Assert.Equal("nota", updated.Description);
    }

    [Fact]
    public void MissingTask_ThrowsNotFound()
    {
        var service = TestFactory.NewService(out _);
        Assert.Throws<NotFoundException>(() => service.Get(99));
        Assert.Throws<NotFoundException>(() => service.Delete(99));
        Assert.Throws<NotFoundException>(() => service.SetDone(99, true));
    }

    [Fact]
    public void Stats_CountsDoneAndOverdue()
    {
        var service = TestFactory.NewService(out var clock);
        var a = service.Add(new NewTask("Primera", null, Priority.Low, new DateOnly(2026, 3, 11)));
        service.Add(new NewTask("Segunda", null, Priority.Low, null));
        service.SetDone(a.Id, true);
        clock.Now = clock.Now.AddDays(30);

        Assert.Equal(new TaskStats(2, 1, 1, 0), service.GetStats());           // la vencida está hecha: no cuenta
    }
}
```

Prueba de contrato (**la misma batería contra varias implementaciones**):

```csharp
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
```

Pruebas de la interfaz (**se teclea una conversación y se comprueba lo que sale**):

```csharp
using TaskFlow.Console;
using TaskFlow.Infrastructure;

namespace TaskFlow.Tests;

/// <summary>Pruebas de la interfaz: se "teclea" una conversación entera y se comprueba lo que se muestra.</summary>
public class ConsoleAppTests
{
    private static (string Output, FakeClock Clock, InMemoryTaskRepository Repo) Run(params string[] input)
    {
        var repo = new InMemoryTaskRepository();
        var service = TestFactory.NewService(out var clock, repo);
        var io = new FakeConsole(input);
        new ConsoleApp(service, io, clock).Run();
        return (io.Output, clock, repo);
    }

    [Fact]
    public void CreateAndList_ShowsTheTask()
    {
        var (output, _, repo) = Run("1", "Estudiar C#", "", "3", "", "2", "0");   // nueva: título, sin descripción, prioridad alta, sin fecha; listar; salir

        Assert.Single(repo.GetAll());
        Assert.Contains("✓ Tarea 1 creada", output);
        Assert.Contains("Estudiar C#", output);
        Assert.Contains("Alta", output);
    }

    [Fact]
    public void InvalidInput_IsRetriedWithoutCrashing()
    {
        var (output, _, repo) = Run("abc", "9", "1", "", "Título válido", "", "7", "2", "", "0");
        // "abc" y "9" no son opciones; "1" crea; título vacío se rechaza; descripción vacía;
        // prioridad "7" no válida y "2" sí; fecha vacía; "0" sale

        Assert.Contains("Escribe un número entero", output);
        Assert.Contains("Debe estar entre 0 y 8", output);
        Assert.Contains("Es obligatorio", output);
        Assert.Contains("Escribe 1, 2 o 3", output);
        Assert.Single(repo.GetAll());
    }

    [Fact]
    public void EndOfInput_ExitsCleanlyInsteadOfLooping()
    {
        var (output, _, _) = Run("1", "Título");                 // la entrada se acaba a mitad de un formulario

        Assert.Contains("(fin de la entrada)", output);          // sin esta protección el programa preguntaría eternamente
    }

    [Fact]
    public void BusinessErrors_AreShownNotThrown()
    {
        var (output, _, repo) = Run("1", "ab", "", "1", "01/01/2020", "0");        // título de 2 letras: la regla del negocio lo rechaza

        Assert.Contains("✗ El título necesita al menos 3 caracteres.", output);
        Assert.Empty(repo.GetAll());
        Assert.Contains("Hasta luego", output);                                       // y el programa sigue funcionando
    }

    [Fact]
    public void Delete_AsksForConfirmation()
    {
        var (output, _, repo) = Run(
            "1", "Borrar esto", "", "1", "",     // crear
            "6", "1", "n",                        // borrar y decir que no
            "6", "1", "s",                        // borrar y decir que sí
            "0");

        Assert.Contains("Cancelado", output);
        Assert.Contains("✓ Borrada", output);
        Assert.Empty(repo.GetAll());
    }

    [Fact]
    public void Edit_KeepsValuesOnEnter()
    {
        var (output, _, repo) = Run(
            "1", "Original", "nota", "1", "",     // crear
            "4", "1", "", "", "3", "",            // editar: Enter, Enter, prioridad 3, Enter
            "0");

        var task = repo.GetAll().Single();
        Assert.Equal("Original", task.Title);
        Assert.Equal("nota", task.Description);
        Assert.Equal(TaskFlow.Domain.Priority.High, task.Priority);
    }
}
```

```bash
dotnet test
```

**Resultado real:** `Passed! - Failed: 0, Passed: 22, Skipped: 0, Total: 22`. Los nombres de las pruebas:

```text
TaskFlow.Tests.ConsoleAppTests.BusinessErrors_AreShownNotThrown
TaskFlow.Tests.ConsoleAppTests.CreateAndList_ShowsTheTask
TaskFlow.Tests.ConsoleAppTests.Delete_AsksForConfirmation
TaskFlow.Tests.ConsoleAppTests.Edit_KeepsValuesOnEnter
TaskFlow.Tests.ConsoleAppTests.EndOfInput_ExitsCleanlyInsteadOfLooping
TaskFlow.Tests.ConsoleAppTests.InvalidInput_IsRetriedWithoutCrashing
TaskFlow.Tests.RepositoryContractTests.Add_AssignsConsecutiveIds(kind: "json")
TaskFlow.Tests.RepositoryContractTests.Add_AssignsConsecutiveIds(kind: "memory")
TaskFlow.Tests.RepositoryContractTests.Json_PersistsBetweenInstances
TaskFlow.Tests.RepositoryContractTests.Json_StoresPriorityAsText
TaskFlow.Tests.RepositoryContractTests.Remove_ReturnsFalseWhenMissing(kind: "json")
TaskFlow.Tests.RepositoryContractTests.Remove_ReturnsFalseWhenMissing(kind: "memory")
TaskFlow.Tests.TaskServiceTests.Add_AssignsIdTrimsTitleAndStartsPending
TaskFlow.Tests.TaskServiceTests.Add_RejectsDueDateInThePast
TaskFlow.Tests.TaskServiceTests.Add_RejectsShortTitles(title: "  ")
TaskFlow.Tests.TaskServiceTests.Add_RejectsShortTitles(title: "")
TaskFlow.Tests.TaskServiceTests.Add_RejectsShortTitles(title: "ab")
TaskFlow.Tests.TaskServiceTests.List_FiltersByTextAndOverdue
TaskFlow.Tests.TaskServiceTests.List_OrdersPendingFirstThenByPriority
TaskFlow.Tests.TaskServiceTests.MissingTask_ThrowsNotFound
TaskFlow.Tests.TaskServiceTests.Stats_CountsDoneAndOverdue
TaskFlow.Tests.TaskServiceTests.Update_ChangesOnlyWhatIsGiven
```

> **Un fallo real que se encontró al escribir las pruebas:** una prueba se quedó **en bucle infinito**. La causa: al acabarse la entrada simulada, `ReadLine()` devolvía `null` y el formulario seguía preguntando eternamente. La solución (`EndOfInputException`) es exactamente la protección contra Ctrl+D. **Un bucle «repetir hasta que sea válido» siempre necesita una salida cuando no hay más entrada.**

### Errores típicos

| Error | Causa | Solución |
|---|---|---|
| El servicio imprime por pantalla | Mezcla de capas | El servicio devuelve datos o lanza excepciones; la interfaz imprime |
| No se puede probar sin teclear | `Console` usada directamente | Una interfaz `IConsoleIO` |
| La prueba de «vencida» falla según el día | Usa `DateTime.Now` | Una dependencia `IClock` |
| Una referencia circular entre proyectos | `Core` referencia a `Infrastructure` | Las dependencias apuntan **hacia el Core** |
| `Unable to resolve service for type...` | Una dependencia sin registrar | `services.AddSingleton<...>()` |

> **Ejercicio 27.1:** añade el comando **«Reabrir tarea»** (marcarla de nuevo como pendiente), con su prueba en `TaskServiceTests` **antes** de escribir el código (TDD).
>
> **Ejercicio 27.2:** añade **etiquetas** a las tareas (`List<string>`) y un filtro por etiqueta.
>
> **Ejercicio 27.3:** escribe `SqliteTaskRepository` y comprueba que **pasa la misma prueba de contrato**.
>
> **Ejercicio 27.4:** haz que `ConsoleApp` acepte un **comando directo** (`taskflow add "Título"`) además del menú.

---

## Lo que has aprendido en esta parte

| Capacidad | Dónde |
|---|---|
| Capturar datos de **once** formas y validarlos | Lección 23 |
| Recibir argumentos, variables de entorno y tuberías | Lección 23 |
| Mostrar datos como tabla, detalle, resumen, barras, JSON y CSV | Lección 24 |
| Construir un CRUD completo con menú | Lección 25 |
| Guardar en JSON y CSV sin tocar la lógica | Lección 26 |
| Separar en capas, inyectar dependencias y **probar** | Lección 27 |

**Sigue con la Parte III** (clases abstractas, interfaces, singleton, patrones de diseño y SOLID) y con la **Parte IV** (el mismo TaskFlow convertido en API web con base de datos y seguridad).
