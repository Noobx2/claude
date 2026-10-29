using System;
using System.Collections.Generic;

namespace Juego.Core
{
    /// <summary>
    /// Piscina de objetos: en vez de crear y destruir (Instantiate/Destroy) cientos de balas o enemigos,
    /// se REUTILIZAN. En Unity, crear y destruir objetos genera basura (garbage) y provoca tirones.
    /// </summary>
    public sealed class Pool<T> where T : class
    {
        private readonly Stack<T> _libres = new Stack<T>();
        private readonly Func<T> _crear;
        private readonly Action<T> _alSacar;
        private readonly Action<T> _alDevolver;
        private readonly int _maximo;

        public int TotalCreados { get; private set; }
        public int EnUso { get; private set; }
        public int Libres => _libres.Count;

        public Pool(Func<T> crear, Action<T> alSacar = null, Action<T> alDevolver = null, int maximo = 100)
        {
            _crear = crear ?? throw new ArgumentNullException(nameof(crear));
            _alSacar = alSacar; _alDevolver = alDevolver; _maximo = maximo;
        }

        public T Sacar()
        {
            T item;
            if (_libres.Count > 0) item = _libres.Pop();
            else
            {
                if (EnUso >= _maximo) throw new InvalidOperationException($"La piscina alcanzó su máximo ({_maximo}).");
                item = _crear(); TotalCreados++;
            }
            EnUso++;
            _alSacar?.Invoke(item);
            return item;
        }

        public void Devolver(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (_libres.Contains(item)) throw new InvalidOperationException("Ese objeto ya estaba devuelto (doble devolución).");
            _alDevolver?.Invoke(item);
            EnUso--;
            _libres.Push(item);
        }
    }
}
