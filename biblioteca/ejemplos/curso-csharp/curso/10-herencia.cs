// Lección 10: herencia, polimorfismo, clases abstractas e interfaces
IShape[] shapes = [new Rectangle(3, 4), new Circle(1), new Square(2)];
foreach (var shape in shapes)
    Console.WriteLine($"{shape.Describe(),-22} área {shape.Area():F2}");   // polimorfismo: cada una calcula a su manera

// Herencia con constructores y base
var savings = new SavingsAccount("Ana", 1000m, 0.02m);
savings.Deposit(500m);
savings.AddInterest();
Console.WriteLine(savings);
try { savings.Withdraw(5000m); }
catch (InvalidOperationException e) { Console.WriteLine($"No se pudo: {e.Message}"); }

// is / as: preguntar el tipo real
object value = new Circle(2);
if (value is Circle circle) Console.WriteLine($"es un círculo de radio {circle.Radius}");   // is con declaración
Shape? maybe = value as Shape;                                                               // as: null si no encaja
Console.WriteLine($"as Shape: {maybe?.Describe()}; es IShape: {value is IShape}");

// Interfaz con implementación por defecto y varias interfaces
var duck = new Duck();
duck.Fly(); duck.Swim();
ISwimmer swimmer = duck;                               // la implementación por defecto solo se ve a través de la INTERFAZ
Console.WriteLine(swimmer.Describe());                 // viene de la interfaz; con "duck.Describe()" no compilaría

// Ocultar (new) frente a sobrescribir (override): la diferencia más traicionera
Base viaBase = new Derived();
Console.WriteLine($"{viaBase.Virtual()} / {viaBase.Hidden()}   (override respeta el tipo real; new no)");
Console.WriteLine($"{((Derived)viaBase).Hidden()}");

// sealed: no se puede heredar
Console.WriteLine($"{new Final().Name}");

interface IShape
{
    double Area();
    string Describe() => $"{GetType().Name}";          // implementación por defecto (opcional)
}

abstract class Shape : IShape                          // abstract: no se puede instanciar; obliga a implementar lo abstracto
{
    public abstract double Area();                     // sin cuerpo
    public virtual string Describe() => GetType().Name;  // con cuerpo; las hijas pueden cambiarlo
}

class Rectangle(double width, double height) : Shape
{
    public override double Area() => width * height;
}

class Square(double side) : Rectangle(side, side)      // hereda de una clase concreta
{
    public double Side { get; } = side;
    public override string Describe() => $"Cuadrado de lado {Side}";
}

class Circle(double radius) : Shape
{
    public double Radius { get; } = radius;
    public override double Area() => Math.PI * Radius * Radius;
}

class Account(string owner, decimal balance)
{
    public string Owner { get; } = owner;
    public decimal Balance { get; protected set; } = balance;   // protected: la clase y sus hijas

    public void Deposit(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("El importe debe ser positivo", nameof(amount));
        Balance += amount;
    }

    public virtual void Withdraw(decimal amount)
    {
        if (amount > Balance) throw new InvalidOperationException($"Saldo insuficiente: {Balance}");
        Balance -= amount;
    }

    public override string ToString() => $"{Owner}: {Balance:F2}";
}

class SavingsAccount(string owner, decimal balance, decimal rate) : Account(owner, balance)   // llama al constructor base
{
    public void AddInterest() => Deposit(Balance * rate);
    public override void Withdraw(decimal amount)
    {
        if (amount > 1000m) throw new InvalidOperationException("Máximo 1000 por operación");
        base.Withdraw(amount);                         // base: reutiliza lo de la clase padre
    }
}

interface IFlyer { void Fly(); }
interface ISwimmer { void Swim(); string Describe() => "Sé nadar"; }

class Duck : IFlyer, ISwimmer                          // una clase hereda de UNA clase, pero implementa VARIAS interfaces
{
    public void Fly() => Console.WriteLine("Vuelo");
    public void Swim() => Console.WriteLine("Nado");
}

class Base
{
    public virtual string Virtual() => "Base.Virtual";
    public string Hidden() => "Base.Hidden";
}

class Derived : Base
{
    public override string Virtual() => "Derived.Virtual";
    public new string Hidden() => "Derived.Hidden";    // new: oculta, no sobrescribe
}

sealed class Final { public string Name => "no se puede heredar de mí"; }
