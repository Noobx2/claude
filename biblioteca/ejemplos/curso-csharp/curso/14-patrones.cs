// Lección 14: pattern matching (coincidencia de patrones)
object[] values = [42, "hola", 3.14, null!, new int[] { 1, 2, 3 }, new Circle(2), new Square(3), -7, "", 150];
foreach (var v in values) Console.WriteLine($"{v ?? "null",-22} -> {Classify(v)}");

// Patrones de propiedades y relacionales con objetos
var order = new Order("paid", 120m, new Customer("Ana", true));
Console.WriteLine(Discount(order));
Console.WriteLine(Discount(new Order("pending", 500m, new Customer("Luis", false))));
Console.WriteLine(Discount(new Order("paid", 40m, new Customer("Sara", false))));

// Patrones de lista
int[] data = [1, 2, 3, 4, 5];
Console.WriteLine(Describe(data) + " | " + Describe([]) + " | " + Describe([9]) + " | " + Describe([1, 2]));

// Operadores de patrón: and, or, not
int temperature = 22;
string feeling = temperature switch
{
    < 0 => "helada",
    >= 0 and < 15 => "frío",
    >= 15 and < 25 => "agradable",
    >= 25 and not > 40 => "calor",
    _ => "extremo",
};
Console.WriteLine(feeling);

// Tuplas en el switch: varias condiciones a la vez
static string Fizz(int n) => (n % 3, n % 5) switch
{
    (0, 0) => "FizzBuzz",
    (0, _) => "Fizz",
    (_, 0) => "Buzz",
    _ => n.ToString(),
};
Console.WriteLine(string.Join(" ", Enumerable.Range(1, 15).Select(Fizz)));

static string Classify(object? value) => value switch
{
    null => "null",
    int n when n < 0 => $"entero negativo {n}",         // when: condición extra
    int n and > 100 => $"entero grande {n}",
    int n => $"entero {n}",
    string { Length: 0 } => "texto vacío",
    string s => $"texto de {s.Length} letras",
    double d => $"decimal {d:F1}",
    int[] { Length: > 0 } list => $"array de {list.Length}, primero {list[0]}",
    Circle { Radius: > 1 } big => $"círculo grande (r={big.Radius})",
    Square or Circle => "figura",
    _ => "otra cosa",
};

static decimal Discount(Order order) => order switch
{
    { Status: "paid", Total: >= 100, Customer.Vip: true } => order.Total * 0.15m,
    { Status: "paid", Total: >= 100 } => order.Total * 0.05m,
    { Status: "paid" } => 0m,
    _ => 0m,
};

static string Describe(int[] items) => items switch
{
    [] => "vacío",
    [var only] => $"uno ({only})",
    [var first, .., var last] => $"de {first} a {last}",
};

record Circle(double Radius);
record Square(double Side);
record Customer(string Name, bool Vip);
record Order(string Status, decimal Total, Customer Customer);
