using UnityEngine;

namespace Juego
{
    /// <summary>
    /// ScriptableObject: un ARCHIVO de datos (.asset) que se edita en el Inspector y vive en el proyecto, no en una escena.
    /// Ideal para ajustes, estadísticas de enemigos o armas: se cambian SIN tocar código ni duplicar prefabs.
    /// </summary>
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Juego/Ajustes")]
    public class GameSettings : ScriptableObject
    {
        [Header("Jugador")]
        [Tooltip("Vida máxima del jugador")] [SerializeField] private int maxHealth = 100;
        [Range(0.1f, 5f)] [SerializeField] private float fireCooldown = 0.5f;

        [Header("Balas")]
        [SerializeField] private int bulletPoolSize = 20;

        // Solo lectura desde fuera: los datos se cambian en el Inspector, no desde el código del juego
        public int MaxHealth => maxHealth;
        public float FireCooldown => fireCooldown;
        public int BulletPoolSize => bulletPoolSize;

        /// <summary>Unity la llama en el editor cuando cambias un valor: sirve para mantener los datos coherentes.</summary>
        private void OnValidate()
        {
            maxHealth = Mathf.Max(1, maxHealth);
            bulletPoolSize = Mathf.Max(1, bulletPoolSize);
        }
    }
}
