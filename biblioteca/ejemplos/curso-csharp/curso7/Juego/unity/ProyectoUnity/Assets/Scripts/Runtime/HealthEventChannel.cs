using Juego.Core;
using UnityEngine;

namespace Juego
{
    /// <summary>
    /// Canal de eventos como ScriptableObject: el jugador EMITE y la interfaz ESCUCHA sin conocerse.
    /// Se arrastra el mismo asset a los dos objetos desde el Inspector (nada de FindObjectOfType ni singletons).
    /// </summary>
    [CreateAssetMenu(fileName = "HealthChanged", menuName = "Juego/Canales/Vida cambiada")]
    public class HealthEventChannel : ScriptableObject
    {
        private readonly EventChannel<(int antes, int ahora)> _canal = new EventChannel<(int, int)>();

        public System.IDisposable Suscribir(System.Action<(int antes, int ahora)> oyente) => _canal.Suscribir(oyente);
        public void Emitir(int antes, int ahora) => _canal.Emitir((antes, ahora));
    }
}
