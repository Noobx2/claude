// Lección 3: tipos y variables
// Ejecutar con: dotnet run 03-tipos.cs

// ---- Tipos numéricos enteros (con y sin signo) ----
int age = 31;                    // 32 bits: de -2 147 483 648 a 2 147 483 647
long population = 8_100_000_000; // 64 bits; los _ separan miles (solo para leer mejor)
byte small = 255;                // 8 bits sin signo: 0 a 255
uint positive = 4_000_000_000;   // 32 bits sin signo

// ---- Decimales: double (rápido, aproximado) y decimal (exacto, para dinero) ----
double ratio = 0.1 + 0.2;
decimal price = 0.1m + 0.2m;     // el sufijo m marca un literal decimal
float tiny = 1.5f;               // el sufijo f marca un float (32 bits)

Console.WriteLine($"double: {ratio}   decimal: {price}");
Console.WriteLine($"¿0.1+0.2 == 0.3?  double: {ratio == 0.3}   decimal: {price == 0.3m}");

// ---- Otros tipos básicos ----
bool active = true;
char letter = 'Ñ';               // un carácter: comillas simples
string name = "Ana";             // texto: comillas dobles
object anything = 42;            // object: cualquier cosa (todo hereda de object)
var inferred = 3.14;             // var: el compilador deduce el tipo (double). Sigue siendo estático.

Console.WriteLine($"{age} {population} {small} {positive} {tiny} {active} {letter} {name} {anything} {inferred}");

// ---- Tipos en el sistema: cada palabra clave es un alias de un tipo de .NET ----
Console.WriteLine($"int es System.{typeof(int).Name}, string es System.{typeof(string).Name}, decimal es System.{typeof(decimal).Name}");

// ---- Rangos y desbordamiento ----
Console.WriteLine($"int.MaxValue = {int.MaxValue}, int.MinValue = {int.MinValue}");
int max = int.MaxValue;
Console.WriteLine($"max + 1 sin comprobar = {unchecked(max + 1)}  (da la vuelta, sin avisar)");
try
{
    Console.WriteLine(checked(max + 1));   // checked: lanza excepción si desborda
}
catch (OverflowException e)
{
    Console.WriteLine($"checked avisa: {e.Message}");
}

// ---- Conversiones ----
int whole = (int)3.99;           // conversión explícita (cast): trunca, no redondea
double widened = age;            // implícita: no se pierde nada
int parsed = int.Parse("42");    // texto -> número; lanza excepción si no es válido
bool ok = int.TryParse("abc", out int result);   // TryParse no lanza: devuelve true/false
string text = 42.ToString();
Console.WriteLine($"{whole} {widened} {parsed} {ok} {result} '{text}' {Convert.ToInt32("7") + 1}");
Console.WriteLine($"{Math.Round(2.5)} {Math.Round(3.5)} {Math.Round(2.5, MidpointRounding.AwayFromZero)}  (redondeo bancario por defecto)");

// ---- Constantes y readonly ----
const double Pi = 3.14159;       // se conoce al compilar y no puede cambiar
Console.WriteLine($"Pi = {Pi}");

// ---- Valores por defecto y tipos que admiten null ----
int? maybeNumber = null;         // int? = "int o nada"
Console.WriteLine($"default(int) = {default(int)}, default(bool) = {default(bool)}, maybeNumber = {maybeNumber?.ToString() ?? "null"}");

// ---- Tipos de valor frente a tipos de referencia ----
int a = 5;
int b = a;           // se COPIA el valor
b = 99;
Console.WriteLine($"valor: a = {a}, b = {b}");

var listA = new List<int> { 1, 2 };
var listB = listA;   // se copia la REFERENCIA: las dos variables apuntan a la misma lista
listB.Add(3);
Console.WriteLine($"referencia: listA tiene {listA.Count} elementos, listB tiene {listB.Count} (son la misma lista)");
