using Juego.Combate;

namespace Juego.Tests;

/// <summary>Un «azar» que devuelve siempre lo que la prueba decide: sin aleatoriedad, sin pruebas intermitentes.</summary>
internal sealed class AzarFijo : IAleatorio
{
    private readonly Queue<int> _valores; private readonly int _porDefecto;
    public AzarFijo(int porDefecto = 99, params int[] primeros) { _porDefecto = porDefecto; _valores = new Queue<int>(primeros); }
    public int Siguiente(int min, int max) => _valores.Count > 0 ? _valores.Dequeue() : _porDefecto;   // 99 = nunca crítico (crítico si < 10)
}

public class CombateTests
{
    private static Combatiente C(string n, int vida = 50, int atq = 10, int def = 0, int vel = 5, Habilidad? h = null) =>
        new(n, vida, atq, def, vel, h!);

    private static List<string> Registrar(SimuladorDeCombate combate)
    {
        var log = new List<string>();
        combate.Eventos.Suscribir(e => log.Add(e.Texto));
        return log;
    }

    [Theory]
    [InlineData(10, 0, 1, false, 10)]     // sin defensa: todo el ataque
    [InlineData(10, 6, 1, false, 7)]      // la defensa cuenta a la mitad: 10 - 3
    [InlineData(2, 20, 1, false, 1)]      // mínimo 1 de daño, siempre
    [InlineData(10, 0, 2, false, 20)]     // habilidad ×2
    [InlineData(10, 0, 1, true, 20)]      // crítico ×2
    [InlineData(10, 0, 2, true, 40)]      // los dos se multiplican
    public void FormulaDeDano(int ataque, int defensa, int mult, bool critico, int esperado) =>
        Assert.Equal(esperado, SimuladorDeCombate.CalcularDano(ataque, defensa, mult, critico));

    [Fact]
    public void ActuanDelMasRapidoAlMasLento()
    {
        var combate = new SimuladorDeCombate(new[] { C("Lento", vel: 1, vida: 500) }, new[] { C("Rapido", vel: 9, vida: 500), C("Medio", vel: 5, vida: 500) }, new AzarFijo());
        var log = Registrar(combate);
        combate.Ejecutar();
        var primerosTres = log.Take(3).Select(l => l.Split(' ')[0]).ToList();
        Assert.Equal(new[] { "Rapido", "Medio", "Lento" }, primerosTres);
    }

    [Fact]
    public void AtacaAlEnemigoMasDebil()
    {
        var combate = new SimuladorDeCombate(new[] { C("Heroe", atq: 5, vel: 9, vida: 999) },
                                             new[] { C("Fuerte", vida: 100, atq: 0, vel: 1), C("Debil", vida: 10, atq: 0, vel: 1) }, new AzarFijo());
        var log = Registrar(combate); combate.Ejecutar();
        Assert.StartsWith("Heroe ataca a Debil", log[0]);
    }

    [Fact]
    public void LosHeroesGananCuandoNoQuedanEnemigos()
    {
        var combate = new SimuladorDeCombate(new[] { C("Heroe", atq: 100, vel: 9) }, new[] { C("Debil", vida: 10, vel: 1) }, new AzarFijo());
        combate.Ejecutar();
        Assert.True(combate.GananLosHeroes);
        Assert.Equal(EstadoCombate.Terminado, combate.Estado);
        Assert.Equal(1, combate.Turno);
    }

    [Fact]
    public void LosEnemigosGananCuandoNoQuedanHeroes()
    {
        var combate = new SimuladorDeCombate(new[] { C("Heroe", vida: 5, vel: 1) }, new[] { C("Bestia", atq: 100, vel: 9) }, new AzarFijo());
        var log = Registrar(combate); combate.Ejecutar();
        Assert.False(combate.GananLosHeroes);
        Assert.Contains(log, l => l.StartsWith("DERROTA"));
    }

    [Fact]
    public void UnMuertoNoActuaEnELMismoTurno()
    {
        // El héroe (más rápido) mata al enemigo antes de que le toque: el enemigo no debe atacar
        var combate = new SimuladorDeCombate(new[] { C("Heroe", atq: 100, vel: 9) }, new[] { C("Debil", vida: 1, atq: 50, vel: 1) }, new AzarFijo());
        var log = Registrar(combate); combate.Ejecutar();
        Assert.DoesNotContain(log, l => l.StartsWith("Debil ataca"));
    }

    [Fact]
    public void UnMuertoNoActuaAunqueElCombateSigaEnCurso()
    {
        // El orden de actuación se calcula al empezar el turno, así que «Debil1» sigue en esa lista aunque muera.
        // El combate NO termina (queda «Debil2» vivo), por eso aquí la comprobación de «¿sigue vivo?» es la que importa.
        var combate = new SimuladorDeCombate(new[] { C("Heroe", atq: 100, vel: 9, vida: 999) },
                                             new[] { C("Debil1", vida: 1, atq: 50, vel: 5), C("Debil2", vida: 999, atq: 1, vel: 1) }, new AzarFijo());
        var log = Registrar(combate); combate.Ejecutar();
        Assert.Contains(log, l => l.Contains("Debil1 cae derrotado"));
        Assert.DoesNotContain(log, l => l.StartsWith("Debil1 ataca"));
    }

    [Fact]
    public void ElCriticoDependeDelAzarInyectado()
    {
        // Primer valor del azar = 5 (< 10) → crítico; los siguientes = 99 → no crítico
        var combate = new SimuladorDeCombate(new[] { C("Heroe", atq: 10, vel: 9) }, new[] { C("Blanco", vida: 1000, atq: 0, vel: 1) }, new AzarFijo(99, 5));
        var log = Registrar(combate); combate.Ejecutar();
        Assert.Contains("¡CRÍTICO!", log[0]);
        Assert.DoesNotContain("¡CRÍTICO!", log[1]);
    }

    [Fact]
    public void LaHabilidadTieneRecargaEnTurnos()
    {
        var habilidad = new Habilidad("Golpe", multiplicador: 3, recargaEnTurnos: 2);
        var combate = new SimuladorDeCombate(new[] { C("Heroe", atq: 10, vel: 9, vida: 999, h: habilidad) },
                                             new[] { C("Blanco", vida: 1000, atq: 0, vel: 1) }, new AzarFijo());
        var eventos = new List<EventoCombate>();
        combate.Eventos.Suscribir(eventos.Add);
        combate.Ejecutar();
        var turnosConHabilidad = eventos.Where(e => e.Texto.Contains("usa Golpe")).Select(e => e.Turno).Take(3).ToList();
        Assert.Equal(new[] { 1, 3, 5 }, turnosConHabilidad);          // recarga de 2 turnos: se usa en el 1, queda lista en el 3, en el 5…
    }

    [Fact]
    public void SiNadieCaeSeTerminaPorLimiteDeTurnosEnEmpate()
    {
        var combate = new SimuladorDeCombate(new[] { C("A", vida: 100000, atq: 1) }, new[] { C("B", vida: 100000, atq: 1) }, new AzarFijo());
        var log = Registrar(combate); combate.Ejecutar();
        Assert.Equal(SimuladorDeCombate.TurnosMaximos, combate.Turno);
        Assert.Contains(log, l => l.StartsWith("EMPATE"));
    }

    [Fact]
    public void NoSePuedeEjecutarDosVeces()
    {
        var combate = new SimuladorDeCombate(new[] { C("A", atq: 100, vel: 9) }, new[] { C("B", vida: 1) }, new AzarFijo());
        combate.Ejecutar();
        Assert.Throws<InvalidOperationException>(() => combate.Ejecutar());
    }

    [Fact]
    public void ExigeAlMenosUnCombatientePorBando()
    {
        Assert.Throws<ArgumentException>(() => new SimuladorDeCombate(new List<Combatiente>(), new[] { C("B") }, new AzarFijo()));
    }

    [Fact]
    public void MismoAzar_MismoCombate()
    {
        List<string> Jugar()
        {
            var combate = new SimuladorDeCombate(new[] { C("A", atq: 12, vel: 7, vida: 80) }, new[] { C("B", atq: 11, vel: 6, vida: 80) }, new AzarFijo(40));
            var log = Registrar(combate); combate.Ejecutar(); return log;
        }
        Assert.Equal(Jugar(), Jugar());                               // reproducible: imprescindible para depurar y para repeticiones
    }
}
