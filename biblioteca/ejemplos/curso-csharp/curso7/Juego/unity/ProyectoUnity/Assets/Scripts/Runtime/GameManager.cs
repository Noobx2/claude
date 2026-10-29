using UnityEngine;

namespace Juego
{
    /// <summary>
    /// El singleton «a la manera de Unity» y sus trampas. Mejor alternativa: ScriptableObject + referencias en el Inspector.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public int Puntos { get; private set; }

        // TRAMPA 1: con «Enter Play Mode Options» (dominio no recargado) los campos ESTÁTICOS conservan su valor entre
        // ejecuciones del editor. Hay que reiniciarlos a mano en un método que Unity llama al arrancar.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ReiniciarEstaticos() => Instance = null;

        private void Awake()
        {
            // TRAMPA 2: si cargas una escena que también contiene un GameManager, habrá DOS. Hay que destruir el duplicado.
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);                                 // sobrevive al cambio de escena
        }

        private void OnDestroy() { if (Instance == this) Instance = null; }

        public void SumarPuntos(int n) => Puntos += n;
    }
}
