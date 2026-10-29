using System;
using System.Collections.Generic;

namespace Juego.Core
{
    /// <summary>Máquina de estados: de un estado solo se puede pasar a los permitidos. Evita «el jefe está muerto y atacando».</summary>
    public sealed class StateMachine<TState> where TState : struct, Enum
    {
        private readonly Dictionary<TState, HashSet<TState>> _permitidas = new Dictionary<TState, HashSet<TState>>();
        public TState Actual { get; private set; }
        public event Action<TState, TState> Cambió;                 // (desde, hasta)

        public StateMachine(TState inicial) { Actual = inicial; }

        public StateMachine<TState> Permitir(TState desde, params TState[] hacia)
        {
            if (!_permitidas.TryGetValue(desde, out var conjunto)) _permitidas[desde] = conjunto = new HashSet<TState>();
            foreach (var h in hacia) conjunto.Add(h);
            return this;                                           // interfaz fluida: encadenar llamadas
        }

        public bool PuedeIr(TState hacia) => _permitidas.TryGetValue(Actual, out var c) && c.Contains(hacia);

        public bool TryIr(TState hacia)
        {
            if (!PuedeIr(hacia)) return false;
            var anterior = Actual; Actual = hacia;
            Cambió?.Invoke(anterior, hacia);
            return true;
        }
    }
}
