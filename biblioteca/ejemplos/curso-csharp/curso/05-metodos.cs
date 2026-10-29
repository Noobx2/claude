// Lección 5: métodos
// Los métodos de nivel superior (sin clase) se escriben detrás del código en archivos de un solo fichero.

Console.WriteLine(Calculator.Add(2, 3));
Console.WriteLine(Calculator.Add(2.5, 1.5));            // sobrecarga: mismo nombre, otros tipos (solo en métodos de una clase)
Console.WriteLine(Greet("Ana"));                        // parámetro opcional con valor por defecto
Console.WriteLine(Greet("Ana", "Buenas"));
Console.WriteLine(Greet(greeting: "Hey", name: "Luis")); // argumentos con nombre: el orden no importa
Console.WriteLine(Sum(1, 2, 3, 4));                     // params: número variable de argumentos

// out: el método devuelve varios valores
if (TryDivide(10, 3, out int quotient, out int remainder))
    Console.WriteLine($"10 / 3 = {quotient} resto {remainder}");

// Tuplas: otra forma de devolver varios valores
var (min, max) = MinMax([4, 9, 1, 7]);
Console.WriteLine($"min {min}, max {max}");

// ref: pasa la VARIABLE (no una copia) y el método puede cambiarla
int counter = 0;
Increment(ref counter);
Increment(ref counter);
Console.WriteLine($"counter = {counter}");

// Sin ref, los enteros se pasan por valor: el original no cambia
int original = 5;
TryChange(original);
Console.WriteLine($"original sigue valiendo {original}");

// Recursión
Console.WriteLine($"5! = {Factorial(5)}, fib(10) = {Fibonacci(10)}");

// Función local: un método dentro de otro, solo visible ahí
int Square(int x) => x * x;
Console.WriteLine($"3² = {Square(3)}");

// Métodos como valores (delegados)
Func<int, int, int> operation = Calculator.Add;         // Func<entrada1, entrada2, salida>
Console.WriteLine($"por delegado: {operation(10, 20)}");

// Métodos de la clase base más útiles
Console.WriteLine($"{Math.Max(3, 8)} {Math.Pow(2, 10)} {Math.Sqrt(81)} {Math.Abs(-4)} {Math.Floor(2.7)} {Math.Ceiling(2.1)}");

static string Greet(string name, string greeting = "Hola") => $"{greeting}, {name}";

static int Sum(params int[] numbers) => numbers.Sum();

static bool TryDivide(int a, int b, out int quotient, out int remainder)
{
    if (b == 0) { quotient = 0; remainder = 0; return false; }   // out: hay que asignarlo siempre
    quotient = a / b;
    remainder = a % b;
    return true;
}

static (int Min, int Max) MinMax(int[] values) => (values.Min(), values.Max());

static void Increment(ref int value) => value++;

static void TryChange(int value) => value = 999;         // cambia la copia, no el original

static long Factorial(int n) => n <= 1 ? 1 : n * Factorial(n - 1);

static long Fibonacci(int n) => n < 2 ? n : Fibonacci(n - 1) + Fibonacci(n - 2);

// Los métodos escritos entre las instrucciones de nivel superior son "funciones locales" y NO admiten sobrecarga.
// Para sobrecargar hay que ponerlos en una clase:
static class Calculator
{
    public static int Add(int a, int b) => a + b;        // cuerpo de expresión: => en lugar de { return ...; }
    public static double Add(double a, double b) => a + b;
}
