namespace Juego.Core
{
    /// <summary>Tiempo de recarga de una habilidad o disparo. Recibe el tiempo por parámetro (como Time.deltaTime), así es determinista y probable.</summary>
    public sealed class Cooldown
    {
        private float _restante;
        public float Duracion { get; }
        public bool Listo => _restante <= 0f;
        public float Restante => _restante > 0f ? _restante : 0f;

        public Cooldown(float duracion) { Duracion = duracion; }

        public void Tick(float deltaTime) { if (_restante > 0f) _restante -= deltaTime; }

        public bool TryUsar()
        {
            if (!Listo) return false;
            _restante = Duracion;
            return true;
        }
    }
}
