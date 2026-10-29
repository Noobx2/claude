using System;
using System.Collections;
using System.Collections.Generic;

namespace Juego.Core
{
    /// <summary>Lo que una corrutina «cede» (yield) para decir cuánto quiere esperar.</summary>
    public abstract class Espera { public abstract bool Terminada(float deltaTime); }

    public sealed class EsperarSegundos : Espera
    {
        private float _restante;
        public EsperarSegundos(float segundos) { _restante = segundos; }
        public override bool Terminada(float deltaTime) { _restante -= deltaTime; return _restante <= 0f; }
    }

    public sealed class EsperarHasta : Espera
    {
        private readonly Func<bool> _condicion;
        public EsperarHasta(Func<bool> condicion) { _condicion = condicion; }
        public override bool Terminada(float deltaTime) => _condicion();
    }

    /// <summary>
    /// Una versión mínima de lo que hace Unity con StartCoroutine: una corrutina es SOLO un iterador (IEnumerator, con 'yield return')
    /// que alguien avanza un paso por fotograma. No hay hilos: todo ocurre en el hilo principal, de forma cooperativa.
    /// </summary>
    public sealed class CorrutinaRunner
    {
        private sealed class Activa { public IEnumerator Iterador; public Espera Espera; }
        private readonly List<Activa> _activas = new List<Activa>();
        public int EnCurso => _activas.Count;

        /// <summary>
        /// Como StartCoroutine de Unity: el código se ejecuta AL INSTANTE hasta el primer 'yield'.
        /// Solo lo que viene después de ese primer yield depende de los fotogramas siguientes.
        /// </summary>
        public void Empezar(IEnumerator corrutina)
        {
            var activa = new Activa { Iterador = corrutina };
            if (!corrutina.MoveNext()) return;                                       // terminó sin cederle nunca el control
            activa.Espera = corrutina.Current as Espera;
            _activas.Add(activa);
        }

        /// <summary>Se llama una vez por fotograma (Unity lo hace después de Update).</summary>
        public void Tick(float deltaTime)
        {
            foreach (var c in _activas.ToArray())                                    // copia: una corrutina puede iniciar otras mientras se recorre
            {
                if (c.Espera != null && !c.Espera.Terminada(deltaTime)) continue;   // sigue esperando
                c.Espera = null;
                if (!c.Iterador.MoveNext()) { _activas.Remove(c); continue; }        // terminó
                c.Espera = c.Iterador.Current as Espera;                             // 'yield return null' = esperar al siguiente fotograma
            }
        }
    }
}
