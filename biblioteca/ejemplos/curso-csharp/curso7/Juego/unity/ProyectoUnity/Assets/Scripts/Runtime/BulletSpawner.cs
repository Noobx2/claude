using System.Collections;
using Juego.Core;
using UnityEngine;

namespace Juego
{
    public class Bullet : MonoBehaviour
    {
        public void Lanzar(Vector3 desde) { transform.position = desde; gameObject.SetActive(true); }
    }

    /// <summary>Dispara con un tiempo de recarga y reutiliza las balas con una piscina (sin Instantiate/Destroy en cada disparo).</summary>
    public class BulletSpawner : MonoBehaviour
    {
        [SerializeField] private GameSettings settings;
        [SerializeField] private Bullet bulletPrefab;

        private Pool<Bullet> _pool;
        private Coroutine _disparando;

        private void Awake()
        {
            _pool = new Pool<Bullet>(
                crear: () => { var b = Instantiate(bulletPrefab); b.gameObject.SetActive(false); return b; },
                alSacar: b => b.gameObject.SetActive(true),
                alDevolver: b => b.gameObject.SetActive(false),
                maximo: settings.BulletPoolSize);
        }

        private void OnEnable() => _disparando = StartCoroutine(Disparar());
        private void OnDisable() { if (_disparando != null) StopCoroutine(_disparando); }   // las corrutinas NO se detienen solas al desactivar un componente concreto

        private IEnumerator Disparar()
        {
            var espera = new WaitForSeconds(settings.FireCooldown);       // reutilizar la espera evita crear basura (garbage) en cada vuelta
            while (true)
            {
                Bullet bala = _pool.Sacar();
                bala.Lanzar(transform.position);
                yield return espera;
                _pool.Devolver(bala);                                      // (simplificado: en un juego real se devuelve al impactar)
            }
        }
    }
}
