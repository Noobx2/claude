// Lección 16: delegados, lambdas y eventos
// Un delegado es "un método guardado en una variable". Func, Action y Predicate ya vienen hechos.
Func<int, int, int> add = (a, b) => a + b;             // devuelve algo: la última pieza es el tipo de retorno
Action<string> shout = text => Console.WriteLine(text.ToUpper());   // no devuelve nada
Predicate<int> isEven = n => n % 2 == 0;                // devuelve bool
Console.WriteLine($"{add(2, 3)} {isEven(4)}");
shout("hola");

// Pasar comportamiento como argumento
Console.WriteLine(string.Join(",", Apply([1, 2, 3], x => x * 10)));
Console.WriteLine(string.Join(",", Apply([1, 2, 3], x => x * x)));

// Captura de variables (closure): la lambda "recuerda" variables de fuera
int factor = 3;
Func<int, int> multiply = x => x * factor;
factor = 10;                                            // cambia lo que la lambda ve
Console.WriteLine($"closure: {multiply(2)}   (usa el factor actual, no el de cuando se creó)");

// Combinar delegados (multicast)
Action chain = () => Console.Write("uno ");
chain += () => Console.Write("dos ");
chain += () => Console.WriteLine("tres");
chain();

// EVENTOS: una clase avisa a quien se haya suscrito
var thermostat = new Thermostat();
thermostat.TemperatureChanged += (sender, e) => Console.WriteLine($"  [pantalla] {e.OldValue} -> {e.NewValue}");
thermostat.TemperatureChanged += (sender, e) => { if (e.NewValue > 30) Console.WriteLine("  [alarma] ¡demasiado calor!"); };
thermostat.Set(20);
thermostat.Set(35);
thermostat.Set(35);   // sin cambio: no se dispara

// Quitar la suscripción (evita fugas de memoria)
EventHandler<TemperatureEventArgs> handler = (s, e) => Console.WriteLine("  [temporal]");
thermostat.TemperatureChanged += handler;
thermostat.Set(10);
thermostat.TemperatureChanged -= handler;
thermostat.Set(11);

static IEnumerable<int> Apply(IEnumerable<int> source, Func<int, int> transform)
{
    foreach (var item in source) yield return transform(item);
}

class TemperatureEventArgs(int oldValue, int newValue) : EventArgs
{
    public int OldValue { get; } = oldValue;
    public int NewValue { get; } = newValue;
}

class Thermostat
{
    private int _value;
    public event EventHandler<TemperatureEventArgs>? TemperatureChanged;   // event: solo esta clase puede dispararlo

    public void Set(int value)
    {
        if (value == _value) return;
        var old = _value;
        _value = value;
        Console.WriteLine($"Set({value})");
        TemperatureChanged?.Invoke(this, new TemperatureEventArgs(old, value));   // ?. por si nadie está suscrito
    }
}
