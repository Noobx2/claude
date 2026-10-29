// Lección 4: operadores y control de flujo
int a = 17, b = 5;
Console.WriteLine($"{a} + {b} = {a + b}; {a} - {b} = {a - b}; {a} * {b} = {a * b}");
Console.WriteLine($"{a} / {b} = {a / b} (entero); {a} % {b} = {a % b} (resto); {(double)a / b:F2} (decimal)");

// Incremento y compuestos
int n = 10;
n += 5; n -= 3; n *= 2; n /= 4;   // n = ((10+5)-3)*2/4
Console.WriteLine($"n = {n}; n++ devuelve {n++} y luego n vale {n}; ++n devuelve {++n}");

// Comparación y lógica
Console.WriteLine($"{a > b} {a == b} {a != b} | && {a > 0 && b > 0} || {a < 0 || b > 0} ! {!(a > b)}");
// && y || cortocircuitan: si ya se sabe el resultado, no evalúan el resto
string? text = null;
Console.WriteLine($"cortocircuito: {text != null && text.Length > 3}");   // no falla aunque text sea null

// if / else if / else
int score = 72;
if (score >= 90) Console.WriteLine("A");
else if (score >= 70) Console.WriteLine("B");
else Console.WriteLine("C");

// Operador condicional (ternario)
Console.WriteLine(score >= 50 ? "aprobado" : "suspenso");

// switch clásico (instrucción)
int day = 3;
switch (day)
{
    case 1:
    case 7:
        Console.WriteLine("fin de semana");
        break;                           // sin break no compila: C# no "cae" al siguiente caso
    case >= 2 and <= 6:
        Console.WriteLine("entre semana");
        break;
    default:
        Console.WriteLine("día inválido");
        break;
}

// switch como expresión: devuelve un valor
string DayName(int d) => d switch
{
    1 => "lunes",
    2 => "martes",
    3 => "miércoles",
    >= 4 and <= 5 => "jueves o viernes",
    6 or 7 => "fin de semana",
    _ => "inválido",                      // _ = cualquier otro valor
};
Console.WriteLine($"{DayName(3)} / {DayName(6)} / {DayName(9)}");

// Bucles
for (int i = 1; i <= 3; i++) Console.Write($"for {i}; ");
Console.WriteLine();

int countdown = 3;
while (countdown > 0) Console.Write($"while {countdown--}; ");
Console.WriteLine();

int tries = 0;
do { tries++; } while (tries < 3);        // do-while: se ejecuta al menos una vez
Console.WriteLine($"do-while: {tries} vueltas");

foreach (char c in "abc") Console.Write($"[{c}]");
Console.WriteLine();

// break y continue
for (int i = 1; i <= 10; i++)
{
    if (i % 2 == 0) continue;             // salta a la siguiente vuelta
    if (i > 7) break;                     // sale del bucle
    Console.Write($"{i} ");
}
Console.WriteLine();

// Bucles anidados con etiqueta (goto es rarísimo; mejor extraer un método)
for (int row = 1; row <= 3; row++)
{
    for (int col = 1; col <= 3; col++) Console.Write($"{row * col,3}");
    Console.WriteLine();
}
