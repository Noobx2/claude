using System;
using System.Collections.Generic;
using System.Linq;
using Juego.Core;

namespace Juego.Combate
{
    /// <summary>El azar como DEPENDENCIA: en el juego es aleatorio de verdad; en las pruebas, una secuencia fija.</summary>
    public interface IAleatorio
    {
        int Siguiente(int minInclusivo, int maxExclusivo);
    }

    public sealed class Habilidad
    {
        public string Nombre { get; }
        public int Multiplicador { get; }                // 2 = hace el doble de daño
        public Cooldown Recarga { get; }
        public Habilidad(string nombre, int multiplicador, float recargaEnTurnos)
        {
            Nombre = nombre; Multiplicador = multiplicador; Recarga = new Cooldown(recargaEnTurnos);
        }
    }

    public sealed class Combatiente
    {
        public string Nombre { get; }
        public Health Vida { get; }
        public int Ataque { get; }
        public int Defensa { get; }
        public int Velocidad { get; }
        public Habilidad Habilidad { get; }
        public bool EstaVivo => !Vida.IsDead;

        public Combatiente(string nombre, int vida, int ataque, int defensa, int velocidad, Habilidad habilidad = null)
        {
            Nombre = nombre; Vida = new Health(vida); Ataque = ataque; Defensa = defensa; Velocidad = velocidad; Habilidad = habilidad;
        }
    }

    public enum EstadoCombate { Preparando, EnCurso, Terminado }

    /// <summary>Lo que ocurre en el combate, como datos: la interfaz (consola, Unity…) decide cómo mostrarlo.</summary>
    public sealed class EventoCombate
    {
        public int Turno { get; }
        public string Texto { get; }
        public EventoCombate(int turno, string texto) { Turno = turno; Texto = texto; }
        public override string ToString() => $"[T{Turno}] {Texto}";
    }

    /// <summary>Un combate por turnos entre dos bandos. Sin consola, sin Unity, sin azar real: todo inyectado.</summary>
    public sealed class SimuladorDeCombate
    {
        public const int ProbabilidadCriticoPorCien = 10;
        public const int TurnosMaximos = 100;               // protección contra combates que no acaban nunca

        private readonly List<Combatiente> _heroes, _enemigos;
        private readonly IAleatorio _azar;
        private readonly StateMachine<EstadoCombate> _estado;

        public EventChannel<EventoCombate> Eventos { get; } = new EventChannel<EventoCombate>();
        public int Turno { get; private set; }
        public EstadoCombate Estado => _estado.Actual;
        public IReadOnlyList<Combatiente> Heroes => _heroes;
        public IReadOnlyList<Combatiente> Enemigos => _enemigos;
        public bool GananLosHeroes => _enemigos.All(e => !e.EstaVivo) && _heroes.Any(h => h.EstaVivo);

        public SimuladorDeCombate(IEnumerable<Combatiente> heroes, IEnumerable<Combatiente> enemigos, IAleatorio azar)
        {
            _heroes = heroes.ToList(); _enemigos = enemigos.ToList(); _azar = azar;
            if (_heroes.Count == 0 || _enemigos.Count == 0) throw new ArgumentException("Cada bando necesita al menos un combatiente.");
            _estado = new StateMachine<EstadoCombate>(EstadoCombate.Preparando)
                .Permitir(EstadoCombate.Preparando, EstadoCombate.EnCurso)
                .Permitir(EstadoCombate.EnCurso, EstadoCombate.Terminado);
        }

        /// <summary>Calcula el daño de un ataque. Público para poder probar la fórmula sola.</summary>
        public static int CalcularDano(int ataque, int defensa, int multiplicador, bool critico)
        {
            int baseDano = Math.Max(1, ataque - defensa / 2);          // siempre se hace al menos 1 de daño
            int dano = baseDano * multiplicador;
            return critico ? dano * 2 : dano;
        }

        public void Ejecutar()
        {
            if (!_estado.TryIr(EstadoCombate.EnCurso)) throw new InvalidOperationException("El combate ya empezó o terminó.");
            while (Turno < TurnosMaximos && !HayGanador())
            {
                Turno++;
                // Orden de actuación: del más rápido al más lento (desempate por nombre para que sea determinista)
                var orden = _heroes.Concat(_enemigos).Where(c => c.EstaVivo)
                                   .OrderByDescending(c => c.Velocidad).ThenBy(c => c.Nombre).ToList();
                foreach (var actor in orden)
                {
                    if (!actor.EstaVivo) continue;                      // pudo morir antes de que le tocara
                    if (HayGanador()) break;
                    Actuar(actor);
                }
                foreach (var c in orden) c.Habilidad?.Recarga.Tick(1f); // la recarga se mide en turnos
            }
            _estado.TryIr(EstadoCombate.Terminado);
            Eventos.Emitir(new EventoCombate(Turno, HayGanador()
                ? (GananLosHeroes ? "VICTORIA de los héroes" : "DERROTA: ganan los enemigos")
                : $"EMPATE: se alcanzó el máximo de {TurnosMaximos} turnos"));
        }

        private bool HayGanador() => _heroes.All(h => !h.EstaVivo) || _enemigos.All(e => !e.EstaVivo);

        private void Actuar(Combatiente actor)
        {
            var bandoContrario = _heroes.Contains(actor) ? _enemigos : _heroes;
            var objetivo = bandoContrario.Where(c => c.EstaVivo).OrderBy(c => c.Vida.Current).ThenBy(c => c.Nombre).First();   // ataca al más débil

            bool usaHabilidad = actor.Habilidad != null && actor.Habilidad.Recarga.TryUsar();
            int multiplicador = usaHabilidad ? actor.Habilidad.Multiplicador : 1;
            bool critico = _azar.Siguiente(0, 100) < ProbabilidadCriticoPorCien;
            int dano = CalcularDano(actor.Ataque, objetivo.Defensa, multiplicador, critico);

            string accion = usaHabilidad ? $"usa {actor.Habilidad.Nombre} sobre" : "ataca a";
            objetivo.Vida.TakeDamage(dano);
            Eventos.Emitir(new EventoCombate(Turno,
                $"{actor.Nombre} {accion} {objetivo.Nombre}: {dano} de daño{(critico ? " ¡CRÍTICO!" : "")} ({objetivo.Vida.Current}/{objetivo.Vida.Max})"));
            if (!objetivo.EstaVivo) Eventos.Emitir(new EventoCombate(Turno, $"{objetivo.Nombre} cae derrotado"));
        }
    }
}
