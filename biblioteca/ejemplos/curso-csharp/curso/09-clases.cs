// Lección 9: clases y objetos
var ana = new Person("Ana", 31);                       // new + constructor
ana.Birthday();
Console.WriteLine(ana);                                // llama a ToString()
Console.WriteLine($"{ana.Name} {ana.Age} {ana.IsAdult}");

var initial = new Product { Name = "Teclado", Price = 59.9m };   // inicializador de objeto
Console.WriteLine($"{initial.Name} cuesta {initial.Price}");

// Propiedad con validación: set rechaza valores inválidos
try { initial.Price = -1; }
catch (ArgumentOutOfRangeException e) { Console.WriteLine($"rechazado: {e.ParamName}"); }

// static: pertenece a la CLASE, no a cada objeto
var a = new Counter(); var b = new Counter(); var c = new Counter();
Console.WriteLine($"contadores creados: {Counter.Created}; ids: {a.Id}, {b.Id}, {c.Id}");
Console.WriteLine($"{MathHelper.Double(21)} (método estático sin crear objeto)");

// Igualdad: dos objetos distintos con los mismos datos NO son iguales (salvo record)
var p1 = new Person("Luis", 28); var p2 = new Person("Luis", 28);
Console.WriteLine($"mismo objeto: {ReferenceEquals(p1, p2)}; == {p1 == p2}; Equals {p1.Equals(p2)}");

// Constructor principal (C# 12): los parámetros se usan en toda la clase
var account = new Account("Ana", 100m);
account.Deposit(50m);
Console.WriteLine($"{account.Owner}: {account.Balance:F2}");

// Propiedades init y required
var config = new Settings { Host = "localhost", Port = 5432 };
Console.WriteLine($"{config.Host}:{config.Port}");
// config.Port = 1;   // ERROR de compilación: init solo permite asignar al crear

// Miembros y modificadores de acceso
Console.WriteLine($"{nameof(Person)} tiene campos privados, propiedades públicas y métodos");

class Person
{
    private int _age;                                  // campo privado (convención: _camelCase)

    public string Name { get; }                        // propiedad de solo lectura: solo se asigna en el constructor
    public int Age                                     // propiedad con lógica
    {
        get => _age;
        private set => _age = value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(value));
    }
    public bool IsAdult => Age >= 18;                  // propiedad calculada: sin campo detrás

    public Person(string name, int age)                // constructor: mismo nombre que la clase, sin tipo de retorno
    {
        Name = name;
        Age = age;
    }

    public void Birthday() => Age++;
    public override string ToString() => $"{Name} ({Age})";
}

class Product
{
    private decimal _price;
    public string Name { get; set; } = "";             // propiedad automática con valor inicial
    public decimal Price
    {
        get => _price;
        set
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "El precio no puede ser negativo");
            _price = value;
        }
    }
}

class Counter
{
    private static int _created;                       // un único valor compartido por todos los objetos
    public static int Created => _created;
    public int Id { get; } = ++_created;               // cada objeto toma el siguiente número
}

static class MathHelper                                // clase estática: solo miembros estáticos, no se instancia
{
    public static int Double(int x) => x * 2;
}

class Account(string owner, decimal balance)           // constructor principal
{
    public string Owner { get; } = owner;
    public decimal Balance { get; private set; } = balance;
    public void Deposit(decimal amount) => Balance += amount;
}

class Settings
{
    public required string Host { get; init; }         // required: hay que darlo al crear; init: no se cambia después
    public int Port { get; init; } = 80;
}
