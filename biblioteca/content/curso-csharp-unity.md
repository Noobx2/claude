# Curso de C# · Parte X: C# en Unity (el complemento)

Unity usa C# como lenguaje de scripting, así que **todo lo aprendido sirve**, pero con **tres peculiaridades** que descolocan a quien viene de .NET «normal»: Unity **tiene su propia versión de C#** (más antigua), **su propio ciclo de vida** (los `MonoBehaviour`) y **sus propias reglas de memoria y de hilos**. Esta parte enseña a trabajar con ellas **y a aislar la lógica del juego para poder probarla sin abrir Unity**.

> ## Aviso: lo que se pudo verificar aquí y lo que NO
>
> **Unity no está instalado en este entorno** (no hay editor, ni licencia, ni pantalla). Por tanto, **ningún script se ha ejecutado dentro de Unity**. Esto es lo que sí se hizo, y lo que no:
>
> | Qué | Cómo se comprobó | Fiabilidad |
> |---|---|---|
> | **Lógica del juego** (vida, recarga, piscina de objetos, máquina de estados, eventos, corrutinas simuladas) | Escrita en **C# puro** con los límites de Unity (`netstandard2.1` + C# 9), **17 pruebas** ejecutadas y **4 mutaciones** detectadas | **Alta**: es C# normal |
> | Qué características de C# **no** se pueden usar en Unity | **Compilado** con `LangVersion 9` y `netstandard2.1`, característica por característica | **Alta** |
> | **Scripts que usan la API de Unity** (`MonoBehaviour`, `ScriptableObject`, `Instantiate`…) | **Solo compilados contra un «molde» (stub) escrito por mí** | **Baja**: confirma sintaxis y tipos de **mi** código; **no** confirma que la API real se llame así ni cómo se comporte |
> | Orden de los mensajes (`Awake`, `Start`, `Update`…) | **Simulación** que reproduce el orden **documentado** | Sirve para **entender**, no para verificar Unity |
> | Datos de versiones y hoja de ruta de Unity | Búsqueda web, octubre 2026 (fuentes al final) | Cambian rápido: **consulta la documentación oficial** |
>
> **Antes de fiarte de cualquiera de los scripts de `Assets/`, ábrelo en Unity.** Si algo no coincide con lo que aquí se dice, **manda la documentación de Unity de tu versión**.

## Mapa de la Parte X

| Lección | Tema |
|---|---|
| 54 | El modelo de Unity, la versión de C# que puedes usar, el ciclo de vida y cómo organizar el proyecto |
| 55 | Patrones: ScriptableObject, eventos, piscina de objetos, corrutinas frente a `async`, singleton y sus trampas, pruebas |

---

## Lección 54 · Unity para quien ya sabe C#

### Qué vas a aprender

- Cómo se organiza un juego en Unity: **escenas, GameObjects y componentes**.
- **Qué versión de C# puedes usar** (la respuesta importa mucho tras estudiar C# 14).
- El **ciclo de vida** de un `MonoBehaviour`.
- Cómo estructurar el proyecto: **carpetas, `asmdef`, `.gitignore`**.
- El principio que ordena todo lo demás: **lógica en C# puro, `MonoBehaviour` fino**.

### La idea

#### El modelo: GameObject + componentes

En Unity **no se hereda** para construir cosas: **se compone**. (Es la «composición frente a herencia» de la Parte III, convertida en motor.)

```text
Escena
 └── GameObject «Jugador»
      ├── Transform        (posición, rotación, escala; todo GameObject lo tiene)
      ├── Rigidbody        (física)
      ├── PlayerHealth     ← tu script (un MonoBehaviour)
      └── BulletSpawner    ← otro script
```

| Concepto | Qué es |
|---|---|
| **Escena** | Un nivel o pantalla: un conjunto de GameObjects |
| **GameObject** | Un «contenedor» vacío con nombre y `Transform` |
| **Componente** | Una pieza con comportamiento o datos que se **añade** a un GameObject. Tus scripts son componentes |
| **`MonoBehaviour`** | La clase base de los scripts que se pegan a un GameObject y reciben mensajes del motor |
| **Prefab** | Un GameObject guardado como plantilla para crear muchas copias (`Instantiate`) |
| **`ScriptableObject`** | Un **archivo de datos** (`.asset`) con tu propia clase, que no vive en una escena |
| **Inspector** | El panel donde editas los campos **serializados** de un componente |

#### ¡Qué versión de C# puedes usar en Unity!

> **Esta es la tabla que más sorprende:** tras estudiar C# 14, **casi todo eso no está disponible en Unity**.

Según la actualización oficial de Unity de **octubre de 2026**, **Unity sigue limitado a C# 9 y a la superficie de API de .NET Standard 2.1**, incluso en la versión experimental con CoreCLR. Unity ha anunciado que **C# 14 y .NET 10 llegarán después** de migrar del runtime Mono a **CoreCLR** (el mismo que usa .NET moderno), pero **a fecha de este curso no están disponibles**, y las fechas se han movido varias veces (lo que se planeó como Unity 6.8 ahora se llama Unity 7.0, en alfa).

Esto se **comprobó compilando** código con los mismos límites (`netstandard2.1` + `LangVersion 9`):

```text
Compilado con <TargetFramework>netstandard2.1</TargetFramework> y <LangVersion>9.0</LangVersion> (los límites de Unity):

Característica                Resultado
--------------------------------------------
record (C# 9)                 ERROR CS0518
init (C# 9)                   ERROR CS0518
pattern and/or/not (C# 9)     compila
new() inferido (C# 9)         compila
namespace de archivo (C# 10)  ERROR CS8773
record struct (C# 10)         ERROR CS8773
required (C# 11)              ERROR CS8773
patrón de lista (C# 11)       ERROR CS8773
texto crudo """ (C# 11)       ERROR CS8773
constructor primario (C# 12)  ERROR CS8773
colección [..] (C# 12)        ERROR CS8773
params Span (C# 13)           ERROR CS8773
field (C# 14)                 ERROR CS8773
extension(...) (C# 14)        ERROR CS1513

Con este «polyfill» añadido a CUALQUIER archivo del proyecto, record e init sí compilan:

    namespace System.Runtime.CompilerServices { internal static class IsExternalInit { } }
```

**Cómo se lee:**

- Lo de **C# 9** funciona: patrones `and/or/not`, `new()` inferido.
- **`record` e `init` fallan con `CS0518`** aunque son de C# 9: el compilador busca un tipo (`IsExternalInit`) que **no existe en .NET Standard 2.1**. **Solución habitual:** declararlo uno mismo (se verificó que funciona), o usar clases normales.
- **De C# 10 en adelante**: error `CS8773`, «esta característica no está disponible en C# 9». Eso incluye **espacios de nombres de archivo, `record struct`, `required`, patrones de lista, texto crudo, constructores primarios, `[..]`, `params Span`, `field` y las extensiones de C# 14** (esta última falla con otro error porque ni siquiera entiende la sintaxis).
- **Qué sí tienes en Unity:** `async/await` (con matices, lección 55), LINQ, genéricos, `yield`, tuplas, `?.`/`??`, `switch` como expresión (C# 8), interpolación de cadenas, `Span<T>` (según el backend y la versión).

> **Consecuencia práctica:** si escribes la lógica de tu juego en una biblioteca con `netstandard2.1` y `LangVersion 9` (como se hace aquí), **te asegura** que cuando la copies a Unity **compile**.

#### Versiones de Unity (consultadas el 2026-10-29)

| Línea | Estado según las fuentes consultadas | Qué significa |
|---|---|---|
| **Unity 6.0 LTS** (6000.0) | Soporte hasta **octubre de 2026** | A punto de acabar |
| **Unity 6.3 LTS** (6000.3) | Soporte hasta **diciembre de 2027** | La línea de producción actual |
| Unity 6.7 LTS | En beta *(fuente secundaria, sin confirmar con Unity)* | Se acerca |
| **Unity 7.0** | En alfa; primer editor con CoreCLR | **Todavía C# 9** en la alfa |

- **LTS** (*Long Term Support*): versiones para producción, con correcciones durante años. Las otras («Update/Supported») traen novedades y duran menos.
- **Mono frente a IL2CPP:** Unity compila tu C# de dos maneras: **Mono** (más rápido de compilar, para el editor y pruebas) o **IL2CPP** (convierte el IL a C++ y lo compila nativo, para iOS, consolas y la mayoría de lanzamientos). **Unity ha anunciado que Mono dejará de estar disponible** con la llegada de CoreCLR; **IL2CPP se mantiene**. Comprueba el estado exacto en el momento en que empieces tu proyecto.

### El ciclo de vida de un MonoBehaviour

Unity **llama a tus métodos** por su nombre (los «mensajes de evento»). No hace falta heredar ni implementar nada: basta con **escribirlos con el nombre exacto**.

| Mensaje | Cuándo se llama | Para qué sirve |
|---|---|---|
| `Awake` | Al crearse el objeto (incluso **desactivado**), **una vez** | Prepararse a sí mismo; coger los componentes propios |
| `OnEnable` | Cada vez que el componente **se activa** | Suscribirse a eventos |
| `Start` | **Una vez**, justo antes del **primer** `Update` | Cosas que dependan de que **otros objetos ya hayan hecho su `Awake`** |
| `FixedUpdate` | A intervalos **fijos** (por defecto 50 veces/s) | La **física** |
| `Update` | **Una vez por fotograma** | Entrada del jugador, lógica que cambia cada fotograma |
| `LateUpdate` | Tras **todos** los `Update` | Cámaras que siguen a un objeto |
| `OnDisable` | Al desactivarse | **Darse de baja** de eventos |
| `OnDestroy` | Al destruirse | Liberar recursos |

**Regla práctica:** `Awake` para **lo mío**, `Start` para **lo de los demás**; `OnEnable` y `OnDisable` **siempre en pareja** (suscribir/desuscribir).

**El orden entre scripts distintos no está garantizado** (el `Awake` de A y el de B pueden ir en cualquier orden): **no te apoyes en él**; si necesitas orden, usa `Start`, o el ajuste de orden de ejecución de scripts de Unity, o mejor una referencia explícita.

#### Una simulación para ver el orden

> **Esto NO es Unity.** Es una simulación escrita para este curso que reproduce el orden **documentado**. Sirve para ver la secuencia; no prueba el comportamiento real del motor.

```text
Orden de los mensajes (simulación del orden documentado por Unity):

— Fotograma 0: se crean los objetos —
Jugador.Awake
Jugador.OnEnable
Enemigo.Awake
— Fotograma 1 —
Jugador.Start
Jugador.FixedUpdate
Jugador.Update
Jugador.LateUpdate
— Fotograma 2 —
(el script activa al Enemigo)
Enemigo.OnEnable
Enemigo.Start
Jugador.FixedUpdate
Enemigo.FixedUpdate
Jugador.FixedUpdate
Enemigo.FixedUpdate
Jugador.Update
Enemigo.Update
Jugador.LateUpdate
Enemigo.LateUpdate
— Fotograma 3 —
(el script destruye al Jugador)
Jugador.OnDisable
Jugador.OnDestroy
Enemigo.FixedUpdate
Enemigo.Update
Enemigo.LateUpdate

--- Corrutinas: un iterador que se avanza un paso por fotograma ---

  fotograma 0: empieza el parpadeo
  fotograma 2: parpadea 1
  fotograma 5: parpadea 2
  fotograma 6: fin
  (no hay hilos: es una máquina de estados que el motor avanza en el hilo principal)
```

**Qué se ve:**

- El `Enemigo` se crea **desactivado**: su `Awake` se llama, **su `OnEnable`/`Start`/`Update` no** hasta que se activa en el fotograma 2.
- En el fotograma 2, `Start` del enemigo ocurre **antes** de su primer `Update`. Y ese fotograma tiene **dos `FixedUpdate`** (el 1 tuvo uno): el motor acumula el tiempo (30 ms por fotograma) y ejecuta un paso de física por cada 20 ms acumulados. **La física y el dibujado van a ritmos distintos**: un fotograma puede tener cero, uno o varios `FixedUpdate`.
- Al destruir al jugador: `OnDisable` y luego `OnDestroy`.

### Serialización: qué ve el Inspector

Unity guarda los campos de tus componentes en los archivos de escena y de prefab, y los muestra en el **Inspector**. Un campo se **serializa** si:

- es `public`, **o** es `private` con **`[SerializeField]`**, y
- es de un tipo que Unity sabe guardar (números, `string`, `bool`, `Vector3`, enums, referencias a objetos de Unity, arreglos y `List<T>` de eso, y clases propias con `[System.Serializable]`).

**Regla de estilo:** **campos privados con `[SerializeField]`** y propiedades de solo lectura públicas (así no se «modifica desde fuera sin querer»). Unity **no serializa** propiedades, campos `static`, `readonly`, ni **diccionarios**.

**Cuidado al renombrar:** si cambias el nombre de un campo serializado, **pierdes el valor** que tenía en las escenas. `[FormerlySerializedAs("nombreAntiguo")]` lo evita.

### Organizar el proyecto

```text
MiJuego/
├── Assets/                  ← TODO tu contenido (escenas, scripts, arte, prefabs)
│   ├── Scripts/
│   │   ├── Runtime/          ← lo que va dentro del juego   (asmdef: Juego.Runtime)
│   │   └── Tests/EditMode/   ← pruebas en el editor        (asmdef: Juego.Tests.EditMode)
│   └── ...
├── Packages/manifest.json   ← los paquetes (como NuGet) — SE SUBE A GIT
├── ProjectSettings/         ← ajustes del proyecto          — SE SUBE A GIT
├── Library/                 ← caché generada — NO SE SUBE
├── Temp/ Logs/ Obj/ Build/  ← generados — NO SE SUBEN
```

Los scripts de este curso están en esta forma (el árbol real que se creó):

```text
$ find ProyectoUnity -type f | sort
ProyectoUnity/Assets/Scripts/Runtime/BulletSpawner.cs
ProyectoUnity/Assets/Scripts/Runtime/CargaAsincrona.cs
ProyectoUnity/Assets/Scripts/Runtime/GameManager.cs
ProyectoUnity/Assets/Scripts/Runtime/GameSettings.cs
ProyectoUnity/Assets/Scripts/Runtime/HealthEventChannel.cs
ProyectoUnity/Assets/Scripts/Runtime/Juego.Runtime.asmdef
ProyectoUnity/Assets/Scripts/Runtime/PlayerHealth.cs
ProyectoUnity/Assets/Scripts/Tests/EditMode/GameSettingsTests.cs
ProyectoUnity/Assets/Scripts/Tests/EditMode/Juego.Tests.EditMode.asmdef
```

**`asmdef` (Assembly Definition):** por defecto, Unity compila **todos** tus scripts juntos en un solo ensamblado; cada cambio recompila **todo**. Un archivo `.asmdef` crea **un ensamblado propio** para esa carpeta: **recompila solo lo que cambió** y **controla las dependencias** (es la `ProjectReference` de la Parte IV). También es el modo de separar el código de **pruebas** del código del juego. Ambos `.asmdef` de este curso son JSON **válidos** (comprobado) pero **no se abrieron en Unity**.

**`.gitignore` para Unity:** imprescindible, o subirás gigas de caché. Contiene como mínimo `/[Ll]ibrary/`, `/[Tt]emp/`, `/[Oo]bj/`, `/[Bb]uild/`, `/[Ll]ogs/`, `/[Uu]ser[Ss]ettings/`, y los `.csproj`/`.sln` que Unity genera. **Y los archivos `.meta` SÍ se suben**: guardan los identificadores con los que las escenas enlazan los recursos.

### El principio que ordena todo: lógica pura, MonoBehaviour fino

El problema: un `MonoBehaviour` **solo funciona dentro de Unity**: no se puede crear con `new`, ni probar con xUnit, ni ejecutar en un servidor. Y los juegos crecen hasta ser **código enredado imposible de probar**.

**La solución** (la misma de TaskFlow: `Core` aislado): **las reglas del juego viven en C# puro**, y el `MonoBehaviour` es **una cola fina** que traduce entre Unity y la lógica.

```text
 Unity (MonoBehaviour)          C# puro (Juego.Core)
 ┌──────────────────┐          ┌──────────────────┐
 │ PlayerHealth     │ ───────► │ Health           │  ← se prueba con xUnit, sin Unity
 │ (Awake/OnEnable) │          │ (reglas)         │
 └──────────────────┘          └──────────────────┘
```

El proyecto `Juego.Core` **se compila con los límites de Unity** (`netstandard2.1` + C# 9) y no referencia nada de `UnityEngine`:

```csharp
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <!-- Los MISMOS límites que Unity: .NET Standard 2.1 y C# 9. Si compila aquí, compila en Unity. -->
    <TargetFramework>netstandard2.1</TargetFramework>
    <LangVersion>9.0</LangVersion>
    <Nullable>disable</Nullable>
    <RootNamespace>Juego.Core</RootNamespace>
  </PropertyGroup>
</Project>
```

En Unity se puede **imponer** esa separación: en el `asmdef` de la lógica, la opción **`noEngineReferences`** hace que **no pueda** usar nada de UnityEngine (el compilador lo impide). *(No se probó en Unity.)*

### Errores típicos

| Síntoma | Causa | Solución |
|---|---|---|
| «Feature is not available in C# 9» | Se usa sintaxis moderna | No usarla; o esperar CoreCLR en Unity |
| `CS0518 IsExternalInit` con `record`/`init` | No existe en .NET Standard 2.1 | Declarar el tipo uno mismo o usar clases |
| `NullReferenceException` en `Start` | Campo `[SerializeField]` sin asignar en el Inspector | Asignarlo, o comprobar y avisar con `Debug.LogError` |
| El valor del Inspector «desapareció» | Se renombró el campo | `[FormerlySerializedAs]` |
| `Awake` de A depende de B | El orden no está garantizado | `Start`, o una referencia explícita |
| El proyecto pesa gigas en Git | Falta `.gitignore` | Ignorar `Library/`, `Temp/`, etc. |
| Todo recompila a cada cambio | Sin `asmdef` | Dividir en ensamblados |
| Se prueba solo «dándole al Play» | Toda la lógica en `MonoBehaviour` | Lógica pura + `MonoBehaviour` fino |

> **Ejercicio 54.1:** en una carpeta nueva crea un proyecto `netstandard2.1` con `LangVersion 9`, mueve ahí una regla de tu juego y escribe sus pruebas.
>
> **Ejercicio 54.2 (necesita Unity):** crea una escena con un GameObject, añade un script con `Awake/OnEnable/Start/Update/OnDisable/OnDestroy` que escriban con `Debug.Log`, y **compara el orden con la simulación**. Anota lo que **no** coincida.

---

## Lección 55 · Patrones de Unity: datos, eventos, piscinas, corrutinas y pruebas

### Qué vas a aprender

- **`ScriptableObject`** para datos y para eventos.
- La **piscina de objetos** (*object pool*) y por qué existe (memoria y recolector de basura).
- **Corrutinas frente a `async/await`**, y qué hace una corrutina por dentro.
- **El singleton en Unity**: sus trampas reales y alternativas.
- **Probar** la lógica del juego, y qué ofrece el **Unity Test Framework**.

### La idea

#### Corrutinas: un iterador que el motor avanza

Una **corrutina** es un método que devuelve `IEnumerator` y usa `yield return`. Es **exactamente el iterador de la lección 31**: no hay hilos; **Unity avanza el iterador un paso en cada fotograma** (o cuando se cumple lo que se «cede»).

| `yield return …` | Espera |
|---|---|
| `null` | Hasta el **siguiente fotograma** |
| `new WaitForSeconds(2f)` | 2 segundos de **tiempo de juego** |
| `new WaitForEndOfFrame()` | Al final del fotograma actual |
| otra corrutina | A que esa termine |

El código de la lección implementa el mismo mecanismo en C# puro (`CorrutinaRunner`), **para entender qué hace Unity** (no es el código de Unity):

```csharp
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
```

**Un detalle que corrigió una prueba:** mi primera versión **esperaba al primer fotograma para ejecutar el primer tramo**. En Unity, `StartCoroutine` ejecuta el código **de inmediato hasta el primer `yield`**. Una prueba (`EsperarSegundos_…`) falló, y **se arregló el código**, no la prueba. Lo ves en `Empezar`.

Ejecutada (30 ms por fotograma en la primera parte, 40 ms en la segunda):

```text
Orden de los mensajes (simulación del orden documentado por Unity):

— Fotograma 0: se crean los objetos —
Jugador.Awake
Jugador.OnEnable
Enemigo.Awake
— Fotograma 1 —
Jugador.Start
Jugador.FixedUpdate
Jugador.Update
Jugador.LateUpdate
— Fotograma 2 —
(el script activa al Enemigo)
Enemigo.OnEnable
Enemigo.Start
Jugador.FixedUpdate
Enemigo.FixedUpdate
Jugador.FixedUpdate
Enemigo.FixedUpdate
Jugador.Update
Enemigo.Update
Jugador.LateUpdate
Enemigo.LateUpdate
— Fotograma 3 —
(el script destruye al Jugador)
Jugador.OnDisable
Jugador.OnDestroy
Enemigo.FixedUpdate
Enemigo.Update
Enemigo.LateUpdate

--- Corrutinas: un iterador que se avanza un paso por fotograma ---

  fotograma 0: empieza el parpadeo
  fotograma 2: parpadea 1
  fotograma 5: parpadea 2
  fotograma 6: fin
  (no hay hilos: es una máquina de estados que el motor avanza en el hilo principal)
```

**Qué se ve:** el parpadeo empieza en el fotograma 0, el primero espera 0,1 s (entonces ocurre en el fotograma 2, con 40 ms por fotograma), el segundo en el 5, y termina en el 6 tras un `yield return null`. **Siempre en el hilo principal.**

#### Corrutinas frente a `async/await`

| | Corrutina | `async/await` (`Awaitable`, `Task`) |
|---|---|---|
| Qué devuelve | `IEnumerator` | `Awaitable` / `Task` / `void` |
| Se detiene sola al **desactivar el GameObject** | **Sí** | **No**: sigue ejecutándose |
| Al **destruir** el objeto | Se detiene | **Sigue**, y puede tocar objetos destruidos: usa `destroyCancellationToken` |
| Devuelve un valor | No directamente | **Sí** |
| Excepciones | Detienen esa corrutina y se registran | Con `async void`, no se pueden capturar fuera |
| Hilo | Principal | Principal, **salvo `Task.Run`** |
| Coste en basura | Reutiliza `WaitForSeconds` | `Awaitable` está pensado para evitarlo |

**Reglas:** (1) **solo el hilo principal** puede tocar la API de Unity (`transform`, `GameObject`…); (2) en `async`, **pasa siempre un token de cancelación**; (3) **desactivar un componente concreto (`enabled = false`) no detiene sus corrutinas**: desactivar el **GameObject** sí. *(Esto último lo documenta Unity; no se pudo comprobar aquí.)*

#### El singleton en Unity: las trampas

El singleton (lección 29) en Unity tiene dos problemas **adicionales**:

1. **Dominio no recargado.** Si en el editor activas *Enter Play Mode Options* (para entrar al Play más rápido), **los campos estáticos conservan su valor** entre ejecuciones. Hay que reiniciarlos con `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]`.
2. **Duplicados entre escenas.** Si cargas una escena que también contiene un `GameManager`, habrá **dos**; hay que destruir el duplicado en `Awake`.

Y los problemas de siempre: acoplamiento global, difícil de probar. **Alternativas, de más simple a más compleja:**

| Alternativa | Idea |
|---|---|
| **Referencias en el Inspector** | Arrastras el objeto que necesitas. La más simple y la más recomendable |
| **`ScriptableObject` compartido** | Un `.asset` con datos o eventos, referenciado desde quien lo necesite |
| **Canales de eventos (SO)** | Quien emite no conoce a quien escucha |
| **Contenedores de inyección** (VContainer, Zenject/Extenject) | La inyección de dependencias de la Parte III en Unity. *Existen; no se probaron aquí* |

### Código: los scripts de Unity (compilados contra un molde, NO ejecutados en Unity)

`GameSettings.cs`: un `ScriptableObject` con datos editables

```csharp
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
```

`HealthEventChannel.cs`: un canal de eventos como `ScriptableObject`

```csharp
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
```

`PlayerHealth.cs`: el `MonoBehaviour` **fino** que pega la lógica pura al motor

```csharp
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
```

`BulletSpawner.cs`: piscina + corrutina

```csharp
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
```

`GameManager.cs`: el singleton con sus dos trampas resueltas

```csharp
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
```

`CargaAsincrona.cs`: `async` con cancelación

```csharp
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
```

Y todos compilan —**contra un molde, recuérdalo**:

```text
$ dotnet build Comprobar.csproj    (netstandard2.1 + C# 9, contra el STUB; NO es Unity)
    0 Error(s)
    0 Warning(s)
```

> **Qué significa ese `0 Error(s)` y qué no:** significa que **mi código es coherente consigo mismo y con mi molde**. **No** significa que `Awaitable.WaitForSecondsAsync`, `destroyCancellationToken` o `FindFirstObjectByType` existan con esa firma en **tu** versión de Unity. **Abre el proyecto en Unity y mira la consola.**

### La lógica del juego, en C# puro y probada

Estas son las clases que **sí** se ejecutaron: `Health`, `Cooldown`, `Pool<T>`, `StateMachine<T>`, `EventChannel<T>` y el planificador de corrutinas.

`Health.cs`

```csharp
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
```

`Pool.cs`

```csharp
using System;
using System.Collections.Generic;

namespace Juego.Core
{
    /// <summary>
    /// Piscina de objetos: en vez de crear y destruir (Instantiate/Destroy) cientos de balas o enemigos,
    /// se REUTILIZAN. En Unity, crear y destruir objetos genera basura (garbage) y provoca tirones.
    /// </summary>
    public sealed class Pool<T> where T : class
    {
        private readonly Stack<T> _libres = new Stack<T>();
        private readonly Func<T> _crear;
        private readonly Action<T> _alSacar;
        private readonly Action<T> _alDevolver;
        private readonly int _maximo;

        public int TotalCreados { get; private set; }
        public int EnUso { get; private set; }
        public int Libres => _libres.Count;

        public Pool(Func<T> crear, Action<T> alSacar = null, Action<T> alDevolver = null, int maximo = 100)
        {
            _crear = crear ?? throw new ArgumentNullException(nameof(crear));
            _alSacar = alSacar; _alDevolver = alDevolver; _maximo = maximo;
        }

        public T Sacar()
        {
            T item;
            if (_libres.Count > 0) item = _libres.Pop();
            else
            {
                if (EnUso >= _maximo) throw new InvalidOperationException($"La piscina alcanzó su máximo ({_maximo}).");
                item = _crear(); TotalCreados++;
            }
            EnUso++;
            _alSacar?.Invoke(item);
            return item;
        }

        public void Devolver(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (_libres.Contains(item)) throw new InvalidOperationException("Ese objeto ya estaba devuelto (doble devolución).");
            _alDevolver?.Invoke(item);
            EnUso--;
            _libres.Push(item);
        }
    }
}
```

`StateMachine.cs`

```csharp
using System;
using System.Collections.Generic;

namespace Juego.Core
{
    /// <summary>Máquina de estados: de un estado solo se puede pasar a los permitidos. Evita «el jefe está muerto y atacando».</summary>
    public sealed class StateMachine<TState> where TState : struct, Enum
    {
        private readonly Dictionary<TState, HashSet<TState>> _permitidas = new Dictionary<TState, HashSet<TState>>();
        public TState Actual { get; private set; }
        public event Action<TState, TState> Cambió;                 // (desde, hasta)

        public StateMachine(TState inicial) { Actual = inicial; }

        public StateMachine<TState> Permitir(TState desde, params TState[] hacia)
        {
            if (!_permitidas.TryGetValue(desde, out var conjunto)) _permitidas[desde] = conjunto = new HashSet<TState>();
            foreach (var h in hacia) conjunto.Add(h);
            return this;                                           // interfaz fluida: encadenar llamadas
        }

        public bool PuedeIr(TState hacia) => _permitidas.TryGetValue(Actual, out var c) && c.Contains(hacia);

        public bool TryIr(TState hacia)
        {
            if (!PuedeIr(hacia)) return false;
            var anterior = Actual; Actual = hacia;
            Cambió?.Invoke(anterior, hacia);
            return true;
        }
    }
}
```

`EventChannel.cs`

```csharp
using System;
using System.Collections.Generic;

namespace Juego.Core
{
    /// <summary>
    /// Canal de eventos: quien emite no conoce a quien escucha (desacoplamiento).
    /// En Unity suele guardarse en un ScriptableObject para conectar objetos de escenas distintas sin referencias directas.
    /// </summary>
    public sealed class EventChannel<T>
    {
        private readonly List<Action<T>> _oyentes = new List<Action<T>>();

        public IDisposable Suscribir(Action<T> oyente)
        {
            _oyentes.Add(oyente);
            return new Baja(() => _oyentes.Remove(oyente));         // devolver un IDisposable facilita no olvidar darse de baja
        }

        public void Emitir(T valor)
        {
            // Recorremos una COPIA: un oyente puede darse de baja mientras se emite, y modificar la lista que se recorre lanza excepción.
            foreach (var oyente in _oyentes.ToArray()) oyente(valor);
        }

        public int NumeroDeOyentes => _oyentes.Count;

        private sealed class Baja : IDisposable
        {
            private Action _accion;
            public Baja(Action accion) { _accion = accion; }
            public void Dispose() { _accion?.Invoke(); _accion = null; }
        }
    }
}
```

**Por qué una piscina:** crear y destruir objetos (`Instantiate`/`Destroy`) asigna memoria; el recolector de basura la liberará más tarde y **provoca un tirón** (un fotograma lento) en el peor momento. Con una piscina se **crean una vez y se reutilizan**. La prueba lo demuestra: tras sacar, devolver y volver a sacar, `TotalCreados` sigue en 1.

**Por qué `EventChannel` recorre una copia:** un oyente puede darse de baja **mientras se emite** (por ejemplo, un enemigo que muere al recibir el evento). Modificar la lista que se recorre lanza `InvalidOperationException`. La prueba `UnOyentePuedeDarseDeBajaMientrasSeEmite` lo cubre.

### Resultado real: las pruebas

```text
Passed Juego.Tests.CooldownTests.NoSePuedeUsarDosVecesSeguidas_PeroSiTrasLaRecarga
Passed Juego.Tests.StateMachineTests.SoloPermiteLasTransicionesDeclaradas
Passed Juego.Tests.HealthTests.LaVidaNuncaBajaDeCero_YMorirseAvisaUnaSolaVez
Passed Juego.Tests.HealthTests.DanoNegativo_Lanza [< 1 ms]
Passed Juego.Tests.HealthTests.CurarNoSuperaElMaximo_NiResucita [< 1 ms]
Passed Juego.Tests.HealthTests.VidaMaximaInvalida_Lanza(max: 0) [< 1 ms]
Passed Juego.Tests.HealthTests.VidaMaximaInvalida_Lanza(max: -5) [< 1 ms]
Passed Juego.Tests.HealthTests.RecibirDano_BajaLaVida_YAvisaConAnteriorYNueva
Passed Juego.Tests.StateMachineTests.DeMuertoNoSeSale_YLaTransicionAvisa
Passed Juego.Tests.CorrutinaTests.YieldReturnNull_EsperaExactamenteUnFotograma
Passed Juego.Tests.EventChannelTests.LosOyentesRecibenElValor_YDarseDeBajaFunciona
Passed Juego.Tests.EventChannelTests.UnOyentePuedeDarseDeBajaMientrasSeEmite
Passed Juego.Tests.CorrutinaTests.EsperarSegundos_ReanudaCuandoSeAcumulaElTiempo
Passed Juego.Tests.PoolTests.ReutilizaObjetosEnVezDeCrearNuevos
Passed Juego.Tests.CorrutinaTests.EsperarHasta_EsperaALaCondicion_YVariasCorrutinasSeAlternan
Passed Juego.Tests.PoolTests.LlamaALosGanchosDeActivarYDesactivar [< 1 ms]
Passed Juego.Tests.PoolTests.RespetaElMaximo_YDetectaLaDobleDevolucion [< 1 ms]
```

### ¿Detectan algo estas pruebas? Mutación manual

```text
$ MUTACIÓN: (original)
Passed!  - Failed:     0, Passed:    17, Skipped:     0, Total:    17, Duration: 105 ms

$ MUTACIÓN: un muerto sigue recibiendo daño (Died se dispararía varias veces)
Failed Juego.Tests.HealthTests.LaVidaNuncaBajaDeCero_YMorirseAvisaUnaSolaVez
Failed!  - Failed:     1, Passed:    16, Skipped:     0, Total:    17, Duration: 99 ms

$ MUTACIÓN: EventChannel recorre la lista original en vez de una copia
Failed Juego.Tests.EventChannelTests.UnOyentePuedeDarseDeBajaMientrasSeEmite
Failed!  - Failed:     1, Passed:    16, Skipped:     0, Total:    17, Duration: 103 ms

$ MUTACIÓN: la piscina deja de detectar la doble devolución
Failed Juego.Tests.PoolTests.RespetaElMaximo_YDetectaLaDobleDevolucion
Failed!  - Failed:     1, Passed:    16, Skipped:     0, Total:    17, Duration: 114 ms

$ MUTACIÓN: StartCoroutine ya no ejecuta el primer tramo al instante
Failed Juego.Tests.CorrutinaTests.YieldReturnNull_EsperaExactamenteUnFotograma
Failed Juego.Tests.CorrutinaTests.EsperarSegundos_ReanudaCuandoSeAcumulaElTiempo
Failed Juego.Tests.CorrutinaTests.EsperarHasta_EsperaALaCondicion_YVariasCorrutinasSeAlternan
Failed!  - Failed:     3, Passed:    14, Skipped:     0, Total:    17, Duration: 128 ms
```

Cada error introducido a propósito lo detectó **una prueba concreta**, la que correspondía. Eso es lo que da confianza: **una prueba que nunca ha fallado no ha demostrado nada**.

### Unity Test Framework (no ejecutado)

Unity trae su propio marco de pruebas, basado en **NUnit**:

| Modo | Dónde corre | Para qué | Ejemplo |
|---|---|---|---|
| **Edit Mode** | En el editor, **sin dar al Play** | Lógica, `ScriptableObject`s, utilidades. Rápido | `[Test]` (ver `GameSettingsTests.cs` más arriba) |
| **Play Mode** | Dentro del juego en ejecución | Comportamiento con escenas, física, corrutinas | `[UnityTest] public IEnumerator …` (la prueba **es** una corrutina) |

`GameSettingsTests.cs` (compilado contra el molde y NUnit; **no se ejecutó**):

```csharp
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
```

> **Pero la mayor parte de tus pruebas no deberían necesitar Unity:** con la lógica en C# puro, se prueban con xUnit en milisegundos. El Test Framework queda para **lo que de verdad depende del motor**.

### Rendimiento: lo que de verdad importa en un juego

| Problema | Por qué | Qué hacer |
|---|---|---|
| **Basura (GC) en `Update`** | Crear objetos, cadenas, listas, `foreach` sobre ciertos tipos, lambdas que capturan variables… cada fotograma → tirones | Reutilizar (`WaitForSeconds` en un campo, listas reutilizables, piscinas); medir con el **Profiler** de Unity |
| `GetComponent`/`Find` en `Update` | Búsqueda cara repetida | Guardarlo en `Awake` |
| `Update` vacío o innecesario | Unity lo llama aunque no haga nada | Quitarlo; usar eventos |
| `Debug.Log` en el bucle | Muy lento, y sigue ahí en la versión final | Un envoltorio con `[Conditional]` (lección 39) |
| Cadenas concatenadas con `+` | Basura (lección 40) | `StringBuilder`, o no actualizar texto cada fotograma |

### Lo que NO cubre esta parte (y que un curso de Unity completo sí)

Física, animación, **UI** (uGUI/UI Toolkit), **Input System**, iluminación y renderizado (URP/HDRP), audio, **shaders**, **DOTS/ECS**, multijugador, construir para plataformas. El objetivo aquí es C#: **cómo escribirlo y organizarlo** para Unity, no el motor.

### Errores típicos

| Síntoma | Causa | Solución |
|---|---|---|
| Tirones periódicos | Basura generada cada fotograma | Piscinas, reutilizar objetos; Profiler |
| `MissingReferenceException` | Se usa un objeto de Unity ya destruido | Comprobar antes; cancelar `async` con `destroyCancellationToken` |
| La corrutina «sigue» tras desactivar el componente | `enabled = false` no la detiene | Detenerla en `OnDisable`, o desactivar el GameObject |
| Eventos que siguen llamando a un objeto destruido | No se dio de baja | `OnDisable`: siempre en pareja con `OnEnable` |
| «Solo se puede llamar desde el hilo principal» | API de Unity usada desde otro hilo | Volver al hilo principal antes de usarla |
| Dos `GameManager` | Singleton sin protección | Destruir el duplicado en `Awake` |
| `Instance` de una ejecución anterior | Dominio no recargado | `[RuntimeInitializeOnLoadMethod]` para reiniciar |
| Pruebas lentas y frágiles | Toda la lógica pegada a Unity | Lógica pura + `MonoBehaviour` fino |

> **Ejercicio 55.1:** añade a `Health` un escudo que absorba daño antes que la vida. **Primero** las pruebas.
>
> **Ejercicio 55.2:** usa `StateMachine<T>` para un enemigo con estados `Patrullando`, `Persiguiendo`, `Atacando` y `Muerto`, y prueba que de `Muerto` no se sale.
>
> **Ejercicio 55.3 (necesita Unity):** crea un `ScriptableObject` `EnemyStats`, haz 3 assets con valores distintos y un script que cree enemigos desde ellos.
>
> **Ejercicio 55.4 (necesita Unity):** compara con el Profiler el coste de `Instantiate/Destroy` 1000 veces frente a una piscina.

---

## Resumen de la Parte X

| Si quieres… | Haz… |
|---|---|
| Saber qué C# puedes usar | **C# 9** y .NET Standard 2.1 (hoy) |
| Usar `record`/`init` | Declarar `IsExternalInit` o usar clases |
| Entender el orden de los scripts | `Awake` → `OnEnable` → `Start` → `FixedUpdate` → `Update` → `LateUpdate` |
| Guardar datos de diseño | `ScriptableObject` |
| Conectar objetos sin acoplarlos | Canales de eventos |
| Evitar tirones por basura | Piscinas, reutilizar, medir con el Profiler |
| Esperar tiempo sin hilos | Corrutinas |
| Código asíncrono seguro | `async` + `destroyCancellationToken` |
| Singleton | Mejor: referencias en el Inspector o SO; si no, con las dos trampas resueltas |
| Probar de verdad | **Lógica en C# puro** + xUnit; Test Framework para lo del motor |
| No inflar el repositorio | `.gitignore` de Unity; sube los `.meta` |

### Fuentes de los datos de Unity (consultadas el 2026-10-29)

- Unity Discussions, *CoreCLR, Scripting, and .NET Update, October 2026* (C# 9 y .NET Standard 2.1 vigentes; C# 14 y .NET 10 tras la migración).
- Unity Discussions, *Path to CoreCLR, 2026: Upgrade Guide*.
- Unity, *Unity 6.3 LTS is now available* (soporte hasta diciembre de 2027; 6.0 LTS hasta octubre de 2026).
- Los datos de 6.7 LTS y 7.0 proceden de **fuentes secundarias**; verifícalos en la web oficial de Unity.

**Sigue con la Parte XI:** tres **proyectos integradores** que juntan todo.
