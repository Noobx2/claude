using System;

namespace Juego.Core
{
    /// <summary>La vida de cualquier cosa con puntos de vida. C# puro: se prueba sin Unity.</summary>
    public sealed class Health
    {
        public int Max { get; }
        public int Current { get; private set; }
        public bool IsDead => Current <= 0;

        public event Action<int, int> Changed;     // (anterior, nuevo)
        public event Action Died;

        public Health(int max)
        {
            if (max <= 0) throw new ArgumentOutOfRangeException(nameof(max), "La vida máxima debe ser positiva.");
            Max = max; Current = max;
        }

        public void TakeDamage(int amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
            if (IsDead || amount == 0) return;                       // un muerto no recibe más daño
            int anterior = Current;
            Current = Math.Max(0, Current - amount);
            Changed?.Invoke(anterior, Current);
            if (IsDead) Died?.Invoke();
        }

        public void Heal(int amount)
        {
            if (amount < 0) throw new ArgumentOutOfRangeException(nameof(amount));
            if (IsDead) return;                                       // no se resucita curando
            int anterior = Current;
            Current = Math.Min(Max, Current + amount);
            if (Current != anterior) Changed?.Invoke(anterior, Current);
        }
    }
}
