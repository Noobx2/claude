// Lección 11: record, struct, enum y flags
// ---- record: clase inmutable con igualdad POR VALOR ----
var p1 = new Point(1, 2);
var p2 = new Point(1, 2);
Console.WriteLine($"{p1} | iguales: {p1 == p2} | mismo objeto: {ReferenceEquals(p1, p2)}");
var moved = p1 with { X = 10 };                        // with: copia cambiando algo (el original no cambia)
Console.WriteLine($"{moved} | original: {p1}");
var (x, y) = p1;                                       // desconstrucción
Console.WriteLine($"x={x}, y={y}");

// ---- struct: tipo de VALOR (se copia) ----
var s1 = new Vec { X = 1, Y = 1 };
var s2 = s1;
s2.X = 99;
Console.WriteLine($"struct: s1.X = {s1.X}, s2.X = {s2.X} (se copió)");
var c1 = new Box { Value = 1 };
var c2 = c1;
c2.Value = 99;
Console.WriteLine($"class : c1.Value = {c1.Value}, c2.Value = {c2.Value} (misma referencia)");

// ---- enum ----
Color color = Color.Green;
Console.WriteLine($"{color} = {(int)color}; desde texto: {Enum.Parse<Color>("Blue")}; todos: {string.Join(",", Enum.GetNames<Color>())}");
Console.WriteLine($"TryParse 'Pink': {Enum.TryParse<Color>("Pink", out _)}");

// ---- enum de banderas ([Flags]): varios valores a la vez con operaciones de bits ----
var permissions = Permissions.Read | Permissions.Write;          // | une
Console.WriteLine($"permisos: {permissions} ({(int)permissions})");
Console.WriteLine($"¿puede escribir? {permissions.HasFlag(Permissions.Write)}; ¿puede ejecutar? {(permissions & Permissions.Execute) != 0}");
permissions |= Permissions.Execute;                              // activar un bit
permissions &= ~Permissions.Write;                               // desactivar un bit (& con el complemento)
permissions ^= Permissions.Read;                                 // alternar un bit
Console.WriteLine($"ahora: {permissions}; todos: {Permissions.All}; ninguno: {Permissions.None}");
Console.WriteLine($"en binario: {Convert.ToString((int)Permissions.All, 2).PadLeft(4, '0')}");

// ---- tuplas con nombre ----
(string Name, int Age) person = ("Ana", 31);
Console.WriteLine($"{person.Name} {person.Age}");

// ---- record struct y record con propiedades ----
var money = new Money(9.99m, "EUR");
Console.WriteLine($"{money} | {money with { Amount = 5m }}");
var customer = new Customer("Ana", new Address("Madrid", "28001"));
Console.WriteLine(customer);          // los records imprimen sus propiedades

record Point(int X, int Y);

struct Vec { public int X; public int Y; }
class Box { public int Value; }

enum Color { Red, Green, Blue }                       // valores 0, 1, 2

[Flags]                                               // hace que ToString() muestre "Read, Write"
enum Permissions
{
    None = 0,
    Read = 1,             // 0001
    Write = 2,            // 0010
    Execute = 4,          // 0100
    All = Read | Write | Execute,
}

readonly record struct Money(decimal Amount, string Currency);
record Address(string City, string Zip);
record Customer(string Name, Address Address);
