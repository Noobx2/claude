using Juego.Combate;

// El «adaptador» entre el azar real de .NET y la interfaz IAleatorio que usa el simulador.
// Con una SEMILLA (seed) la secuencia es siempre la misma: mismos números → mismo combate.
int semilla = args.Length > 0 && int.TryParse(args[0], out int s) ? s : 42;
var azar = new AzarReal(semilla);

var heroes = new[]
{
    new Combatiente("Aria",  vida: 60, ataque: 14, defensa: 6, velocidad: 12, new Habilidad("Tajo doble", multiplicador: 2, recargaEnTurnos: 3)),
    new Combatiente("Bruno", vida: 80, ataque: 10, defensa: 10, velocidad: 6),
};
var enemigos = new[]
{
    new Combatiente("Goblin",  vida: 35, ataque: 9, defensa: 3, velocidad: 9),
    new Combatiente("Orco",    vida: 70, ataque: 13, defensa: 8, velocidad: 4, new Habilidad("Golpe brutal", 2, 4)),
};

var combate = new SimuladorDeCombate(heroes, enemigos, azar);
using var suscripcion = combate.Eventos.Suscribir(e => Console.WriteLine(e));    // la interfaz solo ESCUCHA los eventos

Console.WriteLine($"Semilla: {semilla}\n");
combate.Ejecutar();
Console.WriteLine($"\nEstado final: {combate.Estado}, turnos: {combate.Turno}");
foreach (var c in heroes.Concat(enemigos)) Console.WriteLine($"  {c.Nombre,-7} {c.Vida.Current,3}/{c.Vida.Max}");

sealed class AzarReal : IAleatorio
{
    private readonly Random _r;
    public AzarReal(int semilla) { _r = new Random(semilla); }
    public int Siguiente(int min, int max) => _r.Next(min, max);
}
