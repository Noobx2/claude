using NUnit.Framework;
using UnityEngine;
using Juego;

namespace Juego.Tests
{
    // Pruebas de modo EDICIÓN: se ejecutan en el editor, sin darle al Play. Rápidas. Para lógica y ScriptableObjects.
    public class GameSettingsTests
    {
        [Test]
        public void MaxHealth_PorDefecto_EsPositivo()
        {
            var ajustes = ScriptableObject.CreateInstance<GameSettings>();
            Assert.That(ajustes.MaxHealth, Is.GreaterThan(0));
        }
    }
}
