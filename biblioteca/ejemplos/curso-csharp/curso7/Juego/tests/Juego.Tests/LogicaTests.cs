using System.Collections;
using Juego.Core;

namespace Juego.Tests;

public class HealthTests
{
    [Fact]
    public void RecibirDano_BajaLaVida_YAvisaConAnteriorYNueva()
    {
        var h = new Health(100); (int, int)? aviso = null;
        h.Changed += (antes, ahora) => aviso = (antes, ahora);
        h.TakeDamage(30);
        Assert.Equal(70, h.Current);
        Assert.Equal((100, 70), aviso);
    }

    [Fact]
    public void LaVidaNuncaBajaDeCero_YMorirseAvisaUnaSolaVez()
    {
        var h = new Health(50); int muertes = 0; h.Died += () => muertes++;
        h.TakeDamage(80); h.TakeDamage(10); h.TakeDamage(5);
        Assert.Equal(0, h.Current);
        Assert.True(h.IsDead);
        Assert.Equal(1, muertes);                                  // un muerto no vuelve a morir
    }

    [Fact]
    public void CurarNoSuperaElMaximo_NiResucita()
    {
        var h = new Health(100); h.TakeDamage(10); h.Heal(500);
        Assert.Equal(100, h.Current);
        var m = new Health(10); m.TakeDamage(10); m.Heal(5);
        Assert.Equal(0, m.Current);
    }

    [Theory]
    [InlineData(0)] [InlineData(-5)]
    public void VidaMaximaInvalida_Lanza(int max) => Assert.Throws<ArgumentOutOfRangeException>(() => new Health(max));

    [Fact]
    public void DanoNegativo_Lanza() => Assert.Throws<ArgumentOutOfRangeException>(() => new Health(10).TakeDamage(-1));
}

public class CooldownTests
{
    [Fact]
    public void NoSePuedeUsarDosVecesSeguidas_PeroSiTrasLaRecarga()
    {
        var c = new Cooldown(1.0f);
        Assert.True(c.TryUsar());
        Assert.False(c.TryUsar());
        c.Tick(0.4f); Assert.False(c.Listo);
        c.Tick(0.6f); Assert.True(c.Listo);
        Assert.True(c.TryUsar());
    }
}

public class PoolTests
{
    private sealed class Bala { public bool Activa; }

    [Fact]
    public void ReutilizaObjetosEnVezDeCrearNuevos()
    {
        var pool = new Pool<Bala>(() => new Bala(), b => b.Activa = true, b => b.Activa = false);
        var a = pool.Sacar(); pool.Devolver(a);
        var b2 = pool.Sacar();
        Assert.Same(a, b2);
        Assert.Equal(1, pool.TotalCreados);                       // solo se creó UNA bala en total
    }

    [Fact]
    public void LlamaALosGanchosDeActivarYDesactivar()
    {
        var pool = new Pool<Bala>(() => new Bala(), b => b.Activa = true, b => b.Activa = false);
        var b = pool.Sacar(); Assert.True(b.Activa);
        pool.Devolver(b);     Assert.False(b.Activa);
    }

    [Fact]
    public void RespetaElMaximo_YDetectaLaDobleDevolucion()
    {
        var pool = new Pool<Bala>(() => new Bala(), maximo: 2);
        var a = pool.Sacar(); pool.Sacar();
        Assert.Throws<InvalidOperationException>(() => pool.Sacar());
        pool.Devolver(a);
        Assert.Throws<InvalidOperationException>(() => pool.Devolver(a));
    }
}

public class StateMachineTests
{
    private enum Estado { Quieto, Andando, Atacando, Muerto }

    private static StateMachine<Estado> Crear() => new StateMachine<Estado>(Estado.Quieto)
        .Permitir(Estado.Quieto, Estado.Andando, Estado.Atacando, Estado.Muerto)
        .Permitir(Estado.Andando, Estado.Quieto, Estado.Atacando, Estado.Muerto)
        .Permitir(Estado.Atacando, Estado.Quieto, Estado.Muerto);               // Muerto no tiene salidas

    [Fact]
    public void SoloPermiteLasTransicionesDeclaradas()
    {
        var m = Crear();
        Assert.True(m.TryIr(Estado.Andando));
        Assert.False(m.TryIr(Estado.Andando));                    // ya está ahí y no declaramos Andando→Andando
        Assert.Equal(Estado.Andando, m.Actual);
    }

    [Fact]
    public void DeMuertoNoSeSale_YLaTransicionAvisa()
    {
        var m = Crear(); var vistas = new List<(Estado, Estado)>();
        m.Cambió += (d, h) => vistas.Add((d, h));
        m.TryIr(Estado.Muerto);
        Assert.False(m.TryIr(Estado.Atacando));                   // el jefe muerto no ataca
        Assert.Equal(new[] { (Estado.Quieto, Estado.Muerto) }, vistas);
    }
}

public class EventChannelTests
{
    [Fact]
    public void LosOyentesRecibenElValor_YDarseDeBajaFunciona()
    {
        var canal = new EventChannel<int>(); var recibidos = new List<int>();
        var baja = canal.Suscribir(recibidos.Add);
        canal.Emitir(1); baja.Dispose(); canal.Emitir(2);
        Assert.Equal(new[] { 1 }, recibidos);
        Assert.Equal(0, canal.NumeroDeOyentes);
    }

    [Fact]
    public void UnOyentePuedeDarseDeBajaMientrasSeEmite()
    {
        var canal = new EventChannel<string>(); IDisposable? baja = null; int llamadas = 0;
        baja = canal.Suscribir(_ => { llamadas++; baja!.Dispose(); });
        canal.Suscribir(_ => llamadas++);
        canal.Emitir("x");                                         // sin la copia interna esto lanzaría InvalidOperationException
        Assert.Equal(2, llamadas);
    }
}

public class CorrutinaTests
{
    [Fact]
    public void EsperarSegundos_ReanudaCuandoSeAcumulaElTiempo()
    {
        var pasos = new List<string>(); var runner = new CorrutinaRunner();
        IEnumerator C() { pasos.Add("a"); yield return new EsperarSegundos(0.1f); pasos.Add("b"); }
        runner.Empezar(C());
        Assert.Equal(new[] { "a" }, pasos);                         // el primer tramo corre AL INSTANTE, como en StartCoroutine
        runner.Tick(0.04f); runner.Tick(0.04f);
        Assert.Equal(new[] { "a" }, pasos);                         // 0,08 s: aún no
        runner.Tick(0.04f);
        Assert.Equal(new[] { "a", "b" }, pasos);                    // 0,12 s: reanuda
        Assert.Equal(0, runner.EnCurso);
    }

    [Fact]
    public void YieldReturnNull_EsperaExactamenteUnFotograma()
    {
        var pasos = new List<int>(); var runner = new CorrutinaRunner();
        IEnumerator C() { pasos.Add(1); yield return null; pasos.Add(2); yield return null; pasos.Add(3); }
        runner.Empezar(C());
        Assert.Equal(new[] { 1 }, pasos);                           // hasta el primer yield, de inmediato
        runner.Tick(0.016f); Assert.Equal(new[] { 1, 2 }, pasos);   // un yield return null = un fotograma
        runner.Tick(0.016f); Assert.Equal(new[] { 1, 2, 3 }, pasos);
    }

    [Fact]
    public void EsperarHasta_EsperaALaCondicion_YVariasCorrutinasSeAlternan()
    {
        bool puerta = false; var log = new List<string>(); var runner = new CorrutinaRunner();
        IEnumerator A() { yield return new EsperarHasta(() => puerta); log.Add("A pasó"); }
        IEnumerator B() { log.Add("B1"); yield return null; log.Add("B2"); }
        runner.Empezar(A()); runner.Empezar(B());
        Assert.Equal(new[] { "B1" }, log);
        runner.Tick(0.01f); runner.Tick(0.01f);
        Assert.Equal(new[] { "B1", "B2" }, log);                    // A sigue esperando; B ya acabó
        puerta = true; runner.Tick(0.01f);
        Assert.Equal(new[] { "B1", "B2", "A pasó" }, log);
    }
}
