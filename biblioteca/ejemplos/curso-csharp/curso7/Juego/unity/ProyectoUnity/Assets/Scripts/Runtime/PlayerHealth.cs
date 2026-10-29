using Juego.Core;
using UnityEngine;

namespace Juego
{
    /// <summary>
    /// MonoBehaviour = la «cola» que pega la lógica pura (Health) al motor. Fina y sin reglas del juego:
    /// las reglas viven en Juego.Core, que se prueba SIN Unity.
    /// </summary>
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private GameSettings settings;                 // se asigna arrastrando el asset en el Inspector
        [SerializeField] private HealthEventChannel healthChanged;

        private Health _health;
        private System.IDisposable _suscripcion;

        public Health Health => _health;

        private void Awake()
        {
            // Awake: preparar el propio objeto. NO asumir que otros objetos ya están listos (aún no han hecho su Awake).
            _health = new Health(settings.MaxHealth);
        }

        private void OnEnable()
        {
            _health.Changed += AlCambiarVida;                          // suscribirse en OnEnable…
            _health.Died += AlMorir;
        }

        private void OnDisable()
        {
            _health.Changed -= AlCambiarVida;                          // …y SIEMPRE darse de baja en OnDisable (si no: fugas y llamadas a objetos destruidos)
            _health.Died -= AlMorir;
            _suscripcion?.Dispose();
        }

        public void TakeDamage(int cantidad) => _health.TakeDamage(cantidad);

        private void AlCambiarVida(int antes, int ahora)
        {
            if (healthChanged != null) healthChanged.Emitir(antes, ahora);
        }

        private void AlMorir()
        {
            Debug.Log($"{name} ha muerto");
            gameObject.SetActive(false);
        }
    }
}
