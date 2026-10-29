using System;
using System.Collections.Generic;

namespace Juego.Core
{
    /// <summary>
    /// Canal de eventos: quien emite no conoce a quien escucha (desacoplamiento).
    /// En Unity suele guardarse en un ScriptableObject para conectar objetos de escenas distintas sin referencias directas.
    /// </summary>
    public sealed class EventChannel<T>
    {
        private readonly List<Action<T>> _oyentes = new List<Action<T>>();

        public IDisposable Suscribir(Action<T> oyente)
        {
            _oyentes.Add(oyente);
            return new Baja(() => _oyentes.Remove(oyente));         // devolver un IDisposable facilita no olvidar darse de baja
        }

        public void Emitir(T valor)
        {
            // Recorremos una COPIA: un oyente puede darse de baja mientras se emite, y modificar la lista que se recorre lanza excepción.
            foreach (var oyente in _oyentes.ToArray()) oyente(valor);
        }

        public int NumeroDeOyentes => _oyentes.Count;

        private sealed class Baja : IDisposable
        {
            private Action _accion;
            public Baja(Action accion) { _accion = accion; }
            public void Dispose() { _accion?.Invoke(); _accion = null; }
        }
    }
}
