using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Juego
{
    /// <summary>
    /// async/await en Unity. Ojo: las corrutinas y async/await no son lo mismo, ni se detienen igual (ver la tabla de la lección).
    /// ADVERTENCIA: Awaitable y destroyCancellationToken existen en versiones recientes de Unity; comprueba la tuya.
    /// </summary>
    public class CargaAsincrona : MonoBehaviour
    {
        private async void Start()                                         // «async void» aquí es aceptable: Start es un manejador de eventos del motor
        {
            // destroyCancellationToken se cancela solo cuando ESTE objeto se destruye: evita tocar objetos que ya no existen
            CancellationToken ct = destroyCancellationToken;
            try
            {
                await Awaitable.WaitForSecondsAsync(2f, ct);
                await Awaitable.NextFrameAsync(ct);                         // continuar en el siguiente fotograma, en el hilo principal
                Debug.Log("Carga terminada");
            }
            catch (System.OperationCanceledException)
            {
                // El objeto se destruyó mientras esperábamos: salir sin hacer nada
            }
        }
    }
}
