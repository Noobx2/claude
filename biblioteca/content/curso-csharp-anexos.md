# Curso de C# · Anexos: memoria, tipado y código limpio

Tres temas que casi todo temario profesional incluye y que **no caben limpiamente en una lección de las partes anteriores**: cómo gestiona .NET la **memoria** (destructores, recolector de basura, `IDisposable`), el **tipado** y los **paradigmas** (`dynamic`, `var`, estilos de programación), y el **código limpio** (nombres, responsabilidades, DRY/KISS/YAGNI, refactorizar con red de seguridad).

*Verificado: 2026-10-29 con .NET SDK 10.0.113 en Linux. Todo el código se ejecutó. Los tiempos y recuentos del recolector son de esta máquina y de esta versión del runtime: los **órdenes de magnitud** y las **conclusiones** son las que valen.*

| Anexo | Tema | Ejecutado |
|---|---|---|
| A | Memoria: montón, generaciones, finalizadores, `IDisposable`, fugas | Generaciones del GC, fuga por evento, finalizador, patrón `Dispose` |
| B | Tipado, `var`/`object`/`dynamic`, paradigmas | Errores de compilación reales, coste de `dynamic`, un problema en 4 estilos |
| C | Código limpio: nombres, refactorizar, DRY/KISS/YAGNI | Reglas del compilador, complejidad 17 → 3, 50 pruebas |

---

## Anexo A · Memoria, destructores e `IDisposable`

### Qué vas a aprender

- Dónde viven los objetos (**pila** y **montón**) y qué hace el **recolector de basura** (*garbage collector*, GC).
- Qué es un **destructor** en C# (en realidad, un **finalizador**) y **por qué no sirve para liberar a tiempo**.
- El patrón **`IDisposable`** y `using`: liberar recursos **de forma determinista**.
- Cómo se producen las **fugas de memoria en un lenguaje con recolector**.

### La idea

| Zona | Qué guarda | Quién la libera |
|---|---|---|
| **Pila** (*stack*) | Variables locales de tipos por valor, parámetros, direcciones de retorno | **Automático** al salir del método |
| **Montón** (*heap*) | Los **objetos** (`new` de clases, arreglos, `string`) | El **recolector de basura**, cuando nadie los referencia |
| **Memoria no administrada** | Lo que reserva código nativo (archivos, sockets, `Marshal.AllocHGlobal`) | **Tú** (o un finalizador como red de seguridad) |

**El recolector divide el montón en «generaciones»:** los objetos nuevos nacen en la **generación 0** (se recoge a menudo y es barata). Los que sobreviven se «promocionan» a la **1** y luego a la **2** (se recoge poco y **es cara**). Los objetos muy grandes (**≥ 85 000 bytes**) van a un área especial (*Large Object Heap*) que cuenta como generación 2.

**Un «objeto fuera de uso» no significa «objeto liberado»:** el recolector solo libera lo que **ya no es alcanzable** desde ninguna referencia viva. Un evento al que nadie se dio de baja **mantiene vivo** al suscriptor.

#### Los tres mecanismos de limpieza, y qué NO prometen

| Mecanismo | Cuándo se ejecuta | Para qué |
|---|---|---|
| **`using` / `Dispose()`** | **En el momento exacto** en que sales del bloque | Liberar **a tiempo** archivos, conexiones, memoria nativa |
| **Finalizador** (`~Clase()`) | **Cuando el GC quiera** (puede ser tarde, o **nunca** si el proceso termina) | **Red de seguridad** si alguien olvidó el `Dispose` |
| El propio GC | Cuando hace falta memoria | Solo la memoria **administrada** |

> **La regla:** si tu clase tiene un recurso no administrado, implementa `IDisposable` **y** un finalizador de respaldo. Si solo contiene otros objetos `IDisposable`, **implementa `IDisposable` y no escribas finalizador**. Casi nunca se escribe un finalizador a mano: se usan envoltorios (`SafeHandle`).

### Código (`A-memoria.cs`)

```csharp
#:property PublishAot=false

using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// ===================== 1) El montón, las generaciones y el recolector de basura =====================
Console.WriteLine("=== 1) Generaciones del recolector de basura (GC) ===");
var viejo = new byte[100];
Console.WriteLine($"Objeto recién creado          : generación {GC.GetGeneration(viejo)}");
GC.Collect(); Console.WriteLine($"Tras sobrevivir a 1 recolección: generación {GC.GetGeneration(viejo)}   (los supervivientes se «promocionan»)");
GC.Collect(); Console.WriteLine($"Tras sobrevivir a 2 recolecciones: generación {GC.GetGeneration(viejo)}   (la 2 es la más cara de recoger)");
var grande = new byte[100_000];
Console.WriteLine($"Un arreglo de 100.000 bytes   : generación {GC.GetGeneration(grande)}   (≥ 85.000 bytes va al «montón de objetos grandes» y cuenta como gen 2)");
Console.WriteLine("Reglas: la gen 0 se recoge a menudo y es barata; la gen 2 poco y es cara. Por eso importa NO crear basura en bucles calientes.");

// ===================== 2) Memoria que NO se libera aunque «ya no se usa»: la fuga por eventos =====================
Console.WriteLine("\n=== 2) Fuga de memoria por un evento que nadie desuscribió ===");
var editor = new Editor();                                                // vive mucho tiempo (como un servicio)
WeakReference vigilante = SuscribirUnaVentana(editor, desuscribir: false);
ForzarRecoleccion();
Console.WriteLine($"Ventana cerrada SIN desuscribirse: ¿sigue viva en memoria? {vigilante.IsAlive}   ← fuga: «editor» la referencia por el evento");
WeakReference otra = SuscribirUnaVentana(editor, desuscribir: true);
ForzarRecoleccion();
Console.WriteLine($"Ventana cerrada DESUSCRIBIÉNDOSE : ¿sigue viva en memoria? {otra.IsAlive}  ← el recolector pudo liberarla");

// ===================== 3) Finalizadores: lo que NO prometen =====================
Console.WriteLine("\n=== 3) Finalizador (~Clase): el recolector lo llama... cuando quiere ===");
CrearYSoltarUnFinalizable();
Console.WriteLine("Objeto soltado. ¿Se finalizó ya? " + (Finalizable.Finalizados > 0 ? "sí" : "no  ← el finalizador NO se ejecuta al soltar el objeto"));
ForzarRecoleccion();
Console.WriteLine($"Tras GC.Collect + WaitForPendingFinalizers: finalizados = {Finalizable.Finalizados}");
Console.WriteLine("El finalizador es una RED DE SEGURIDAD para recursos no administrados, no una forma de liberarlos «a tiempo».");

// ===================== 4) IDisposable: liberar recursos no administrados de forma DETERMINISTA =====================
Console.WriteLine("\n=== 4) IDisposable y 'using' ===");
Console.WriteLine($"Bloques de memoria nativa vivos al empezar: {BufferNativo.Vivos}");
using (var b = new BufferNativo(1024)) { Console.WriteLine($"Dentro del using                           : {BufferNativo.Vivos}"); }
Console.WriteLine($"Tras salir del using (Dispose automático)  : {BufferNativo.Vivos}");
FugaNativa(5);
Console.WriteLine($"Se crearon 5 SIN Dispose → vivos            : {BufferNativo.Vivos}  ← fuga hasta que el finalizador (red de seguridad) actúe");
ForzarRecoleccion();
Console.WriteLine($"Tras GC + finalizadores                    : {BufferNativo.Vivos}  ← la red de seguridad los liberó (pero tarde y sin garantía)");

// ===================== 5) await using: liberar de forma asíncrona =====================
Console.WriteLine("\n=== 5) IAsyncDisposable ===");
await using (var conexion = new ConexionAsincrona()) { Console.WriteLine("Usando la conexión…"); }
Console.WriteLine("(La conexión se cerró con await al salir del bloque)");

// ===================== 6) Stack (pila) frente a heap (montón) =====================
Console.WriteLine("\n=== 6) Cuánto asigna cada cosa en el montón ===");
long antes = GC.GetAllocatedBytesForCurrentThread();
Span<int> enPila = stackalloc int[256];                                    // en la PILA: no cuenta como asignación del GC
for (int i = 0; i < enPila.Length; i++) enPila[i] = i;
long conPila = GC.GetAllocatedBytesForCurrentThread() - antes;
antes = GC.GetAllocatedBytesForCurrentThread();
int[] enMonton = new int[256]; for (int i = 0; i < enMonton.Length; i++) enMonton[i] = i;
long conMonton = GC.GetAllocatedBytesForCurrentThread() - antes;
Console.WriteLine($"stackalloc int[256]: {conPila} bytes en el montón · new int[256]: {conMonton:N0} bytes en el montón");
GC.KeepAlive(enMonton);

Console.WriteLine($"\nModo del GC: {(GCSettings.IsServerGC ? "servidor" : "estación de trabajo")} · colecciones gen0/1/2 hasta ahora: {GC.CollectionCount(0)}/{GC.CollectionCount(1)}/{GC.CollectionCount(2)}");

// ---------------------------------------------------------------- ayudas
[MethodImpl(MethodImplOptions.NoInlining)]                                   // evita que el compilador alargue la vida de las variables locales
static WeakReference SuscribirUnaVentana(Editor editor, bool desuscribir)
{
    var ventana = new Ventana(new byte[10_000]);
    editor.Cambio += ventana.AlCambiar;
    if (desuscribir) editor.Cambio -= ventana.AlCambiar;
    return new WeakReference(ventana);                                       // una referencia que NO mantiene vivo al objeto
}
[MethodImpl(MethodImplOptions.NoInlining)] static void CrearYSoltarUnFinalizable() { _ = new Finalizable(); }
[MethodImpl(MethodImplOptions.NoInlining)] static void FugaNativa(int n) { for (int i = 0; i < n; i++) _ = new BufferNativo(64); }
static void ForzarRecoleccion() { GC.Collect(); GC.WaitForPendingFinalizers(); GC.Collect(); }

sealed class Editor { public event Action? Cambio; public void Notificar() => Cambio?.Invoke(); }
sealed class Ventana(byte[] datos) { public void AlCambiar() => Console.WriteLine(datos.Length); }

sealed class Finalizable
{
    public static int Finalizados;
    ~Finalizable() { Interlocked.Increment(ref Finalizados); }               // «destructor» en C#: en realidad es un finalizador
}

/// <summary>El patrón completo de IDisposable con memoria no administrada, como lo recomienda Microsoft.</summary>
sealed class BufferNativo : IDisposable
{
    public static int Vivos;
    private IntPtr _memoria;                                                  // recurso NO administrado: el GC no sabe liberarlo
    private bool _liberado;

    public BufferNativo(int bytes) { _memoria = Marshal.AllocHGlobal(bytes); Interlocked.Increment(ref Vivos); }

    public void Dispose()
    {
        Liberar();
        GC.SuppressFinalize(this);                                            // ya limpié yo: el finalizador (red de seguridad) no hace falta
    }

    ~BufferNativo() => Liberar();                                             // red de seguridad si alguien olvidó el Dispose

    private void Liberar()
    {
        if (_liberado) return;                                                // idempotente: llamar dos veces no hace daño
        Marshal.FreeHGlobal(_memoria); _memoria = IntPtr.Zero; _liberado = true;
        Interlocked.Decrement(ref Vivos);
    }
}

sealed class ConexionAsincrona : IAsyncDisposable
{
    public async ValueTask DisposeAsync() { await Task.Delay(5); Console.WriteLine("Conexión cerrada (asíncrono)."); }
}
```

### Resultado real

```text
=== 1) Generaciones del recolector de basura (GC) ===
Objeto recién creado          : generación 0
Tras sobrevivir a 1 recolección: generación 1   (los supervivientes se «promocionan»)
Tras sobrevivir a 2 recolecciones: generación 2   (la 2 es la más cara de recoger)
Un arreglo de 100.000 bytes   : generación 2   (≥ 85.000 bytes va al «montón de objetos grandes» y cuenta como gen 2)
Reglas: la gen 0 se recoge a menudo y es barata; la gen 2 poco y es cara. Por eso importa NO crear basura en bucles calientes.

=== 2) Fuga de memoria por un evento que nadie desuscribió ===
Ventana cerrada SIN desuscribirse: ¿sigue viva en memoria? True   ← fuga: «editor» la referencia por el evento
Ventana cerrada DESUSCRIBIÉNDOSE : ¿sigue viva en memoria? False  ← el recolector pudo liberarla

=== 3) Finalizador (~Clase): el recolector lo llama... cuando quiere ===
Objeto soltado. ¿Se finalizó ya? no  ← el finalizador NO se ejecuta al soltar el objeto
Tras GC.Collect + WaitForPendingFinalizers: finalizados = 1
El finalizador es una RED DE SEGURIDAD para recursos no administrados, no una forma de liberarlos «a tiempo».

=== 4) IDisposable y 'using' ===
Bloques de memoria nativa vivos al empezar: 0
Dentro del using                           : 1
Tras salir del using (Dispose automático)  : 0
Se crearon 5 SIN Dispose → vivos            : 5  ← fuga hasta que el finalizador (red de seguridad) actúe
Tras GC + finalizadores                    : 0  ← la red de seguridad los liberó (pero tarde y sin garantía)

=== 5) IAsyncDisposable ===
Usando la conexión…
Conexión cerrada (asíncrono).
(La conexión se cerró con await al salir del bloque)

=== 6) Cuánto asigna cada cosa en el montón ===
stackalloc int[256]: 0 bytes en el montón · new int[256]: 1,048 bytes en el montón

Modo del GC: estación de trabajo · colecciones gen0/1/2 hasta ahora: 10/10/10
```

### Cómo se lee, paso a paso

1. **Generaciones:** un `byte[100]` recién creado es **gen 0**; tras `GC.Collect()` sobrevive y pasa a **gen 1**; tras otra, a **gen 2**. El arreglo de 100 000 bytes **nace en gen 2** (montón de objetos grandes).
2. **La fuga por evento:** `editor` vive mucho tiempo y tiene un evento `Cambio`. Una ventana se suscribe (`editor.Cambio += ventana.AlCambiar`) y luego «se cierra» (se suelta la variable). **El editor sigue apuntando a ella** a través del evento, así que el GC **no puede** liberarla: `IsAlive = True`. Al **desuscribirse**, `IsAlive = False`. Es **la fuga más frecuente en .NET**. `WeakReference` es una referencia que **no impide** la recolección: sirve para comprobarlo.
3. **`[MethodImpl(NoInlining)]`** en las ayudas evita que el compilador alargue la vida de las variables locales: **lo que ves es el comportamiento en Release**; en Debug, las variables viven hasta el final del método y el resultado cambiaría (otra razón para medir en Release).
4. **El finalizador no se ejecuta al soltar el objeto** («¿se finalizó ya? no»). Solo tras `GC.Collect()` + `WaitForPendingFinalizers()` aparece `finalizados = 1`. **No lo uses para liberar a tiempo.**
5. **`BufferNativo` es el patrón completo:** `Dispose()` libera y llama a **`GC.SuppressFinalize(this)`** (así el finalizador no hace falta, y el objeto no se retiene un ciclo extra de recolección); el finalizador `~BufferNativo()` es la red de seguridad; `Liberar()` es **idempotente** (llamarla dos veces no rompe nada).
6. **El `using` demostrado:** vivos 0 → **1** dentro del `using` → **0** al salir. Al crear 5 **sin** `Dispose`, quedan **5 vivos** hasta que el GC y los finalizadores actúan (y **esa llamada no es determinista**).
7. **`await using` / `IAsyncDisposable`** para liberar con `await` (cerrar una conexión de red).
8. **`stackalloc int[256]`** asigna **0 bytes** del montón; `new int[256]` asigna **1 048** (1 024 de datos + la cabecera del objeto). En bucles calientes, `stackalloc` + `Span<T>` evitan basura.

### Errores típicos

| Síntoma | Causa | Solución |
|---|---|---|
| La memoria crece sin parar | Suscripciones a eventos sin baja; cachés estáticas sin límite | `-=` en `Dispose`/`OnDisable`; límites en cachés; `WeakReference` |
| «Archivo en uso por otro proceso» | Se olvidó el `Dispose` | `using` siempre que el tipo sea `IDisposable` |
| Un finalizador que «no se ejecuta» | Se confía en él para liberar a tiempo | `IDisposable` |
| Finalizadores en todas las clases | Se cree que «es el destructor» | Solo para recursos **no administrados**; mejor `SafeHandle` |
| `GC.Collect()` por todas partes | Se cree que «ayuda» | **Casi nunca** conviene; confía en el GC. Aquí se usa **solo para demostrar** |
| Excepciones en `Dispose` | Liberar lanzando errores | `Dispose` **no debe lanzar** |

> **Ejercicio A.1:** modifica `Editor` para que `Ventana` se desuscriba al «cerrarse» implementando `IDisposable`, y vuelve a comprobar con `WeakReference`.
>
> **Ejercicio A.2:** convierte `BufferNativo` para que use `SafeHandle` en lugar de `IntPtr` y finalizador propio.
>
> **Ejercicio A.3:** crea un bucle que genere 1 millón de objetos pequeños y mide `GC.CollectionCount(0)` antes y después; luego hazlo con un `Span<T>`.

---

## Anexo B · Tipado, `dynamic` y paradigmas

### Qué vas a aprender

- **Tipado estático frente a dinámico, y fuerte frente a débil**: dónde se sitúa C#.
- La diferencia real entre **`var`**, **`object`** y **`dynamic`**, y **cuánto cuesta** este último.
- Qué es un **paradigma** y por qué C# es **multiparadigma**: el mismo problema en cuatro estilos.

### La idea

| Pregunta | Estático | Dinámico |
|---|---|---|
| ¿Cuándo se comprueban los tipos? | **Al compilar** | **Al ejecutar** |
| ¿Qué pasa con un error de tipo? | **No compila** | Falla **cuando se ejecuta esa línea** (puede ser en producción) |
| Ejemplos | **C#**, Java, Rust, Go | Python, JavaScript, Ruby |

| Pregunta | Fuerte | Débil |
|---|---|---|
| ¿Convierte tipos «a escondidas»? | **Casi nunca**: pides la conversión | Mucho (`"5" * 2`, `[] + {}`) |
| Ejemplos | **C#**, Python, Java | JavaScript, PHP, C |

**C# es estático y fuerte**, pero con **una puerta dinámica opcional**: `dynamic`. Y **una conversión generosa**: cualquier cosa `+` un `string` se convierte en texto.

#### `var`, `object` y `dynamic`

| | ¿Quién conoce el tipo? | ¿Cuándo se detectan los errores? | Para qué |
|---|---|---|---|
| **`var x = 5;`** | El **compilador** (lo deduce y queda **fijado**) | Al compilar | Ahorrar escribir el tipo. **No es «sin tipo»** |
| **`object x = …;`** | Solo se sabe «es un objeto»; hace falta **convertir** (*cast*) | **Mal cast = excepción al ejecutar** | Colecciones heterogéneas antiguas |
| **`dynamic x = …;`** | **Nadie hasta ejecutar** | **Todo** al ejecutar: miembros inexistentes, tipos incorrectos | Interoperabilidad (COM/Office, lenguajes dinámicos), objetos que se construyen sobre la marcha |

> **`dynamic` apaga la red de seguridad del compilador** y es **entre decenas y cientos de veces más lento** que una llamada tipada (se mide abajo). **Úsalo solo cuando no haya alternativa.** Para JSON sin clases hay `JsonNode`.

### Código (`B-tipado-paradigmas.cs`)

```csharp
#:property PublishAot=false

using System.Diagnostics;
using System.Dynamic;
using System.Text.Json.Nodes;
using Microsoft.CSharp.RuntimeBinder;

// ============ 1) Tipado estático: el compilador conoce el tipo de TODO antes de ejecutar ============
Console.WriteLine("=== 1) var NO es «sin tipo»: el compilador lo deduce y queda fijado ===");
var numero = 42;                                   // el compilador deduce int; desde aquí es int para siempre
var texto = "hola";
Console.WriteLine($"numero es {numero.GetType().Name}, texto es {texto.GetType().Name}");
// numero = "hola";                                // ERROR de compilación (CS0029): no se puede asignar string a int
Console.WriteLine("(La línea comentada no compila: «Cannot implicitly convert type 'string' to 'int'»; se muestra su error real más abajo.)");

// ============ 2) object: cualquier cosa, pero hay que volver a convertir ============
Console.WriteLine("\n=== 2) object: acepta todo, pero el compilador ya no sabe qué es ===");
object caja = "texto dentro de object";
// Console.WriteLine(caja.Length);                 // ERROR de compilación: object no tiene Length
Console.WriteLine($"Con conversión (cast): {((string)caja).Length}");
try { int mal = (int)caja; }                        // el compilador lo deja pasar, pero FALLA al ejecutar
catch (InvalidCastException e) { Console.WriteLine($"Cast equivocado → error en EJECUCIÓN: {e.GetType().Name}"); }

// ============ 3) dynamic: el compilador DEJA DE COMPROBAR; los errores aparecen al ejecutar ============
Console.WriteLine("\n=== 3) dynamic: la comprobación se aplaza hasta la ejecución ===");
dynamic cualquiera = "texto";
Console.WriteLine($"cualquiera.Length (resuelto al ejecutar) = {cualquiera.Length}");
cualquiera = 10;                                    // la misma variable ahora guarda un int
Console.WriteLine($"ahora cualquiera + 5 = {cualquiera + 5}");
try { Console.WriteLine(cualquiera.NoExiste); }     // el compilador NO se queja: el error llega al ejecutar
catch (RuntimeBinderException e) { Console.WriteLine($"Miembro inexistente → error en EJECUCIÓN: {e.GetType().Name}"); }

dynamic expando = new ExpandoObject();              // un objeto al que se le añaden miembros sobre la marcha
expando.Nombre = "Ana"; expando.Edad = 31;
expando.Saludar = (Func<string>)(() => $"Hola, soy {expando.Nombre}");
Console.WriteLine($"ExpandoObject: {expando.Saludar()} · edad {expando.Edad}");
Console.WriteLine($"Miembros de un Expando: {string.Join(", ", (IDictionary<string, object?>)expando)}");

// ============ 4) Precio de dynamic ============
Console.WriteLine("\n=== 4) El coste de dynamic (1.000.000 de llamadas, Release) ===");
object fija = "abc"; dynamic dinamica = "abc"; string tipada = "abc";
Medir("string tipada        .Length", () => { int s = 0; for (int i = 0; i < 1_000_000; i++) s += tipada.Length; return s; });
Medir("dynamic              .Length", () => { int s = 0; for (int i = 0; i < 1_000_000; i++) s += dinamica.Length; return s; });
Medir("reflexión            .Length", () => { int s = 0; var p = typeof(string).GetProperty("Length")!; for (int i = 0; i < 1_000_000; i++) s += (int)p.GetValue(fija)!; return s; });

// ============ 5) La alternativa moderna a dynamic para JSON ============
Console.WriteLine("\n=== 5) JSON sin clases y SIN dynamic: JsonNode (se comprueba en ejecución, pero con una API tipada) ===");
JsonNode json = JsonNode.Parse("""{ "usuario": { "nombre": "Ana", "roles": ["admin", "dev"] } }""")!;
Console.WriteLine($"nombre = {json["usuario"]!["nombre"]}, primer rol = {json["usuario"]!["roles"]![0]}, ¿existe 'x'? {json["x"] is not null}");

// ============ 6) Fuerte frente a débil ============
Console.WriteLine("\n=== 6) Tipado fuerte: C# convierte solo lo que es seguro ===");
int pequeño = 5; long grande = pequeño;             // int → long: seguro, implícito
double d = pequeño;                                 // int → double: implícito
Console.WriteLine($"int→long y int→double: implícitos ({grande}, {d})");
// int vuelta = grande;                             // ERROR de compilación: long → int puede perder datos; hay que pedirlo explícitamente
int vuelta = (int)grande; Console.WriteLine($"long→int: hace falta un cast explícito ({vuelta})");
Console.WriteLine($"\"1\" + 1 = {"1" + 1}   ← la ÚNICA conversión «generosa»: cualquier cosa + string se convierte en texto");
checked { try { int enorme = int.MaxValue; enorme++; } catch (OverflowException) { Console.WriteLine("checked: el desbordamiento lanza OverflowException (por defecto C# lo ignora y da la vuelta)"); } }

// ============ 7) Cuatro paradigmas, el mismo problema ============
Console.WriteLine("\n=== 7) Mismo problema, cuatro estilos: «suma de los cuadrados de los números pares» ===");
int[] datos = [1, 2, 3, 4, 5, 6];

// PROCEDURAL: una secuencia de pasos que modifica variables
int acumulado = 0;
for (int i = 0; i < datos.Length; i++) { if (datos[i] % 2 == 0) acumulado += datos[i] * datos[i]; }
Console.WriteLine($"Procedural           : {acumulado}");

// ORIENTADO A OBJETOS: el comportamiento vive en un objeto con estado
var calc = new SumadorDeCuadrados(esValido: n => n % 2 == 0);
foreach (int n in datos) calc.Añadir(n);
Console.WriteLine($"Orientado a objetos  : {calc.Total}");

// FUNCIONAL: transformar datos con funciones, sin variables que cambian
Console.WriteLine($"Funcional (LINQ)     : {datos.Where(n => n % 2 == 0).Select(n => n * n).Sum()}");

// DECLARATIVO: describes QUÉ quieres, no CÓMO (aquí con una consulta LINQ en sintaxis de consulta, parecida a SQL)
var declarativo = (from n in datos where n % 2 == 0 select n * n).Sum();
Console.WriteLine($"Declarativo (query)  : {declarativo}");
Console.WriteLine("C# es MULTIPARADIGMA: no obliga a uno. Lo normal es mezclar objetos para estructurar y estilo funcional para transformar datos.");

static void Medir<T>(string nombre, Func<T> f)
{
    f(); var t0 = Stopwatch.GetTimestamp(); f();
    Console.WriteLine($"{nombre} {Stopwatch.GetElapsedTime(t0).TotalMilliseconds,8:F2} ms");
}

sealed class SumadorDeCuadrados(Func<int, bool> esValido)
{
    public int Total { get; private set; }
    public void Añadir(int n) { if (esValido(n)) Total += n * n; }
}
```

### Resultado real

```text
=== 1) var NO es «sin tipo»: el compilador lo deduce y queda fijado ===
numero es Int32, texto es String
(La línea comentada no compila: «Cannot implicitly convert type 'string' to 'int'»; se muestra su error real más abajo.)

=== 2) object: acepta todo, pero el compilador ya no sabe qué es ===
Con conversión (cast): 22
Cast equivocado → error en EJECUCIÓN: InvalidCastException

=== 3) dynamic: la comprobación se aplaza hasta la ejecución ===
cualquiera.Length (resuelto al ejecutar) = 5
ahora cualquiera + 5 = 15
Miembro inexistente → error en EJECUCIÓN: RuntimeBinderException
ExpandoObject: Hola, soy Ana · edad 31
Miembros de un Expando: [Nombre, Ana], [Edad, 31], [Saludar, System.Func`1[System.String]]

=== 4) El coste de dynamic (1.000.000 de llamadas, Release) ===
string tipada        .Length     0.43 ms
dynamic              .Length    41.78 ms
reflexión            .Length    88.53 ms

=== 5) JSON sin clases y SIN dynamic: JsonNode (se comprueba en ejecución, pero con una API tipada) ===
nombre = Ana, primer rol = admin, ¿existe 'x'? False

=== 6) Tipado fuerte: C# convierte solo lo que es seguro ===
int→long y int→double: implícitos (5, 5)
long→int: hace falta un cast explícito (5)
"1" + 1 = 11   ← la ÚNICA conversión «generosa»: cualquier cosa + string se convierte en texto
checked: el desbordamiento lanza OverflowException (por defecto C# lo ignora y da la vuelta)

=== 7) Mismo problema, cuatro estilos: «suma de los cuadrados de los números pares» ===
Procedural           : 56
Orientado a objetos  : 56
Funcional (LINQ)     : 56
Declarativo (query)  : 56
C# es MULTIPARADIGMA: no obliga a uno. Lo normal es mezclar objetos para estructurar y estilo funcional para transformar datos.
```

### Los errores que el compilador rechaza (comprobados)

El código de abajo **no compila**, y eso es **lo bueno** del tipado estático: el error aparece **antes de ejecutar**.

```csharp
int numero = 42;
numero = "hola";
object caja = "x";
System.Console.WriteLine(caja.Length);
long grande = 5;
int vuelta = grande;
```

```text
$ dotnet build   (el compilador rechaza los tres errores ANTES de ejecutar)
The build failed. Fix the build errors and run again.
e1.cs(2,10): error CS0029: Cannot implicitly convert type 'string' to 'int'
e1.cs(4,31): error CS1061: 'object' does not contain a definition for 'Length' and no accessible extension method 'Length' accepting a first argument of type 'object' could be found (are you missing a using directive or an assembly reference?)
e1.cs(6,14): error CS0266: Cannot implicitly convert type 'long' to 'int'. An explicit conversion exists (are you missing a cast?)
```

### Cómo se lee, paso a paso

1. **`var numero = 42`** → el compilador deduce `int`; **`numero = "hola"`** no compila (`CS0029`).
2. **`object caja = "…"`** compila, pero `caja.Length` **no** (`CS1061`: `object` no tiene `Length`). Hay que convertir: `((string)caja).Length`. **Un mal *cast* compila y falla al ejecutar** (`InvalidCastException`).
3. **`dynamic`** deja pasar **todo** al compilar: `cualquiera.NoExiste` **no da error de compilación**; da `RuntimeBinderException` al ejecutar. **La misma variable puede guardar un `string` y luego un `int`.**
4. **`ExpandoObject`:** un objeto al que se añaden miembros en ejecución; por dentro es un diccionario.
5. **Coste medido (1 000 000 de llamadas, Release):**

| Forma | Tiempo | Comparado con la tipada |
|---|---|---|
| `string.Length` tipado | 0,43 ms | 1× |
| `dynamic` | 41,8 ms | **~100×** más lento |
| Reflexión (`PropertyInfo.GetValue`) | 88,5 ms | **~200×** |

6. **`JsonNode`:** para leer JSON sin crear clases ni usar `dynamic`: `json["usuario"]["nombre"]`. Sigue comprobando **en ejecución**, pero con una API tipada y sin el coste de `dynamic`.
7. **Fuerte:** `int → long` e `int → double` son **implícitas** (seguras); `long → int` **exige** un *cast* explícito porque puede **perder datos**. Y `"1" + 1` da **`"11"`**: la conversión «generosa». `checked` convierte un desbordamiento en excepción; **por defecto C# lo ignora y «da la vuelta»**.
8. **Cuatro paradigmas, el mismo problema** (la suma de los cuadrados de los pares = **56** en los cuatro):

| Paradigma | Idea | En el código |
|---|---|---|
| **Procedural** | Una secuencia de pasos que **modifica variables** | `for` con un acumulador |
| **Orientado a objetos** | El comportamiento vive en **objetos con estado** | `SumadorDeCuadrados` |
| **Funcional** | **Transformar datos con funciones**, sin variables que cambian | `Where().Select().Sum()` |
| **Declarativo** | Describes **qué** quieres, no **cómo** | La consulta `from … where … select` |

**C# es multiparadigma:** no te obliga a uno. Lo habitual es **objetos para estructurar** y **estilo funcional para transformar datos**.

### Errores típicos

| Síntoma | Causa | Solución |
|---|---|---|
| `RuntimeBinderException` | Un miembro de un `dynamic` no existe | Evita `dynamic`; tipa |
| `InvalidCastException` | *Cast* a un tipo equivocado | `is`/`as` o *pattern matching* (lección 14) |
| «`var` hace el código menos legible» | Se usa con tipos poco evidentes | `var` cuando el tipo es obvio por la derecha (`new`, literal); si no, el tipo explícito |
| Desbordamiento silencioso | `int` se «da la vuelta» | `checked` donde importe |
| `dynamic` en código caliente | Cree que «es cómodo» | Medir: ~100× más lento |

> **Ejercicio B.1:** reescribe un fragmento de tu proyecto que use `object` con *casts* usando genéricos.
>
> **Ejercicio B.2:** resuelve «agrupar palabras por longitud» en los cuatro estilos y comprueba que dan lo mismo.
>
> **Ejercicio B.3:** usa `JsonNode` para leer una respuesta JSON real de una API (por ejemplo, la de TaskFlow) y extraer un campo anidado.

---

## Anexo C · Código limpio

### Qué vas a aprender

- **Nombres**, **responsabilidad** y **refactorización**, con **medidas objetivas**, no opiniones.
- **DRY, KISS, YAGNI, WET**: qué son y cuándo se contradicen.
- Cómo **refactorizar código heredado sin pruebas** (pruebas de caracterización).
- Cómo hacer que **el compilador haga cumplir** las convenciones (`.editorconfig`).

### La idea

#### Las siglas

| Sigla | Significa | Idea | Cuidado |
|---|---|---|---|
| **DRY** | *Don't Repeat Yourself* | Cada **conocimiento** debe estar en **un solo sitio** | **No** es «cero líneas parecidas»: dos fragmentos iguales **por casualidad** (que cambiarán por motivos distintos) **no** son duplicación real |
| **WET** | *Write Everything Twice* | La broma opuesta: duplicar | Tres repeticiones suelen ser el momento de **extraer** |
| **KISS** | *Keep It Simple* | La solución más simple que funcione | Simple ≠ corto: una línea críptica **no** es simple |
| **YAGNI** | *You Aren't Gonna Need It* | **No construyas lo que no necesitas todavía** | **No** es «no pensar en el futuro»: es **no pagar hoy** por algo que quizá nunca llegue |

**DRY frente a KISS/YAGNI:** una abstracción prematura para «evitar duplicación» **viola** YAGNI y complica. La regla práctica: **«la regla de tres»**: la primera vez, escríbelo; la segunda, duplícalo y anótalo; **a la tercera, extrae**.

#### Nombres: que digan **la intención**

| Mal | Bien | Por qué |
|---|---|---|
| `calc`, `d`, `s`, `t`, `p` | `CalcularTotal`, `descuento`, `subtotal`, `cliente`, `destino` | El nombre **es** la documentación |
| `data`, `info`, `manager`, `helper` | Lo que **realmente** es | Son «cajones de sastre» |
| `flag`, `e = true` | `esExpress` | Un booleano debe leerse como una pregunta |
| `x1`, `x2`, `x3` | Una colección con nombre | |
| Un comentario que explica una variable | **Renombra la variable** | El comentario se queda desfasado; el nombre no |

**Convenciones de C#** (las de Microsoft, y las que impone el `.editorconfig` de abajo):

| Elemento | Estilo | Ejemplo |
|---|---|---|
| Clases, métodos, propiedades, enums, espacios de nombres | `PascalCase` | `CalculadoraDePedidos`, `Total` |
| Interfaces | `I` + `PascalCase` | `IReloj` |
| Parámetros y variables locales | `camelCase` | `precioUnitario` |
| Campos privados | `_camelCase` | `_reservas` |
| Constantes | `PascalCase` | `MaximoNoches` |
| Booleanos | Empiezan por `es`/`tiene`/`puede` | `esExpress` |

#### Responsabilidad y funciones pequeñas

Un método debería **hacer una cosa** y su nombre debería **decirla**. Señales de que hace demasiado: **más de un nivel de `if` anidado**, parámetros booleanos que cambian su comportamiento, **comentarios que separan «secciones»** dentro del método, o **«y» en el nombre** que lo describe.

### Código: antes y después

**El código heredado** (con todos los vicios típicos: nombres de una letra, números mágicos, `if` anidados, país repetido en dos sitios, un booleano que cambia el comportamiento, una `Exception` genérica):

```csharp
namespace Limpio.Antes;

public class item { public string n; public decimal p; public int q; }      // nombres que no dicen nada

public class pedidoHelper
{
    // Calcula el total de un pedido. (¿Qué significan t, p, c y e? Hay que leer TODO el método para saberlo.)
    public decimal calc(List<item> l, string t, string p, string c, bool e)
    {
        decimal s = 0;
        foreach (var i in l) { s = s + i.p * i.q; }
        decimal d = 0;
        if (t == "VIP") { d = s * 0.15m; }
        else if (t == "EMPLEADO") { d = s * 0.25m; }
        else { if (s > 100) { d = s * 0.05m; } }
        if (c == "PROMO10") { if (s > 50) { d = d + 10; } }
        decimal env = 0;
        if (p == "ES") { if (s >= 50) { env = 0; } else { env = 4.95m; } }
        else if (p == "FR") { if (s >= 100) { env = 0; } else { env = 9.9m; } }
        else if (p == "US") { if (s >= 200) { env = 0; } else { env = 24.9m; } }
        else { throw new Exception("pais no valido"); }
        if (e == true) { env = env * 1.5m; }
        decimal iva = 0;
        if (p == "ES") { iva = (s - d) * 0.21m; }
        else if (p == "FR") { iva = (s - d) * 0.20m; }
        else if (p == "US") { iva = 0; }
        decimal r = s - d + env + iva;
        return Math.Round(r, 2);
    }
}
```

**El refactorizado** (nombres con intención, constantes con nombre, **las reglas por país como datos en una tabla**, funciones cortas, tipos que impiden valores inválidos):

```csharp
namespace Limpio.Despues;

public enum TipoCliente { Normal, Vip, Empleado }
public enum Pais { España, Francia, EstadosUnidos }

public sealed record LineaDePedido(string Producto, decimal PrecioUnitario, int Cantidad)
{
    public decimal Importe => PrecioUnitario * Cantidad;
}

public sealed record Pedido(IReadOnlyList<LineaDePedido> Lineas, TipoCliente Cliente, Pais Destino, string? Cupon = null, bool EsExpress = false);

/// <summary>Las condiciones de envío e impuestos de un país: son DATOS, no código repetido en un «if» por país.</summary>
public sealed record ReglasDePais(decimal CosteEnvio, decimal EnvioGratisDesde, decimal TipoDeIva);

public static class Tarifas
{
    private static readonly IReadOnlyDictionary<Pais, ReglasDePais> PorPais = new Dictionary<Pais, ReglasDePais>
    {
        [Pais.España]        = new(CosteEnvio: 4.95m,  EnvioGratisDesde: 50m,  TipoDeIva: 0.21m),
        [Pais.Francia]       = new(CosteEnvio: 9.90m,  EnvioGratisDesde: 100m, TipoDeIva: 0.20m),
        [Pais.EstadosUnidos] = new(CosteEnvio: 24.90m, EnvioGratisDesde: 200m, TipoDeIva: 0m),
    };

    public static ReglasDePais Para(Pais pais) =>
        PorPais.TryGetValue(pais, out var reglas) ? reglas : throw new ArgumentOutOfRangeException(nameof(pais), pais, "País no soportado.");
}

public static class CalculadoraDePedidos
{
    private const decimal DescuentoVip = 0.15m;
    private const decimal DescuentoEmpleado = 0.25m;
    private const decimal DescuentoPorCompraGrande = 0.05m;
    private const decimal UmbralCompraGrande = 100m;
    private const string CuponDiezEuros = "PROMO10";
    private const decimal ValorDelCupon = 10m;
    private const decimal GastoMinimoParaCupon = 50m;
    private const decimal RecargoExpress = 1.5m;

    /// <summary>El total que paga el cliente. Cada paso es una función corta con nombre que dice QUÉ hace.</summary>
    public static decimal Total(Pedido pedido)
    {
        decimal subtotal = Subtotal(pedido);
        decimal descuento = Descuento(pedido, subtotal);
        decimal baseImponible = subtotal - descuento;
        var reglas = Tarifas.Para(pedido.Destino);

        decimal envio = CosteDeEnvio(subtotal, reglas, pedido.EsExpress);
        decimal iva = baseImponible * reglas.TipoDeIva;               // el envío no lleva IVA (así lo hacía el código original)

        return Math.Round(baseImponible + envio + iva, 2);
    }

    private static decimal Subtotal(Pedido pedido) => pedido.Lineas.Sum(l => l.Importe);

    private static decimal Descuento(Pedido pedido, decimal subtotal) =>
        DescuentoPorCliente(pedido.Cliente, subtotal) + DescuentoPorCupon(pedido.Cupon, subtotal);

    private static decimal DescuentoPorCliente(TipoCliente cliente, decimal subtotal) => cliente switch
    {
        TipoCliente.Vip => subtotal * DescuentoVip,
        TipoCliente.Empleado => subtotal * DescuentoEmpleado,
        _ => subtotal > UmbralCompraGrande ? subtotal * DescuentoPorCompraGrande : 0m,
    };

    private static decimal DescuentoPorCupon(string? cupon, decimal subtotal) =>
        cupon == CuponDiezEuros && subtotal > GastoMinimoParaCupon ? ValorDelCupon : 0m;

    private static decimal CosteDeEnvio(decimal subtotal, ReglasDePais reglas, bool esExpress)
    {
        decimal coste = subtotal >= reglas.EnvioGratisDesde ? 0m : reglas.CosteEnvio;
        return esExpress ? coste * RecargoExpress : coste;
    }
}
```

### La red de seguridad: pruebas de caracterización

El código heredado **no tenía pruebas**. Antes de tocarlo se hizo lo que se hace en la realidad:

1. **Ejecutarlo tal cual** con 24 entradas elegidas en las **fronteras** de cada regla (49,99 € frente a 50 €, 99,99 frente a 100…).
2. **Anotar** lo que devuelve y **convertirlo en pruebas** (*golden master*).
3. **Refactorizar**, ejecutando las **mismas** pruebas contra el código nuevo.

> **Estas pruebas NO dicen que el resultado sea «correcto»**, dicen que **no ha cambiado**. Si el original tenía un error, las pruebas lo **preservan**. Es lo que se quiere al refactorizar: **cambiar la estructura sin cambiar el comportamiento**. Los errores se arreglan **después**, con otra prueba.

```csharp
namespace Limpio.Pruebas;

/// <summary>
/// TESTS DE CARACTERIZACIÓN («golden master»): antes de tocar código heredado que NO tiene pruebas, se ejecuta tal como está,
/// se anotan sus resultados y se convierten en pruebas. No dicen que el resultado sea «correcto»: dicen que NO HA CAMBIADO.
/// Con esa red, se puede refactorizar sin miedo. Los mismos casos se ejecutan contra el código de ANTES y el de DESPUÉS.
/// </summary>
public abstract class CaracterizacionBase
{
    // (precio, cantidad, cliente, país, cupón, ¿exprés?, resultado que dio el código original)
    public static TheoryData<decimal, int, string, string, string, bool, decimal> Casos => new()
    {
    { 10m, 1, "NORMAL", "ES", "", false, 17.05m },
    { 60m, 1, "NORMAL", "ES", "", false, 72.60m },
    { 49.99m, 1, "NORMAL", "ES", "", false, 65.44m },
    { 50m, 1, "NORMAL", "ES", "", false, 60.50m },
    { 150m, 1, "NORMAL", "ES", "", false, 172.42m },
    { 100m, 1, "NORMAL", "ES", "", false, 121.00m },
    { 100.01m, 1, "NORMAL", "ES", "", false, 114.96m },
    { 200m, 1, "VIP", "FR", "", false, 204.00m },
    { 200m, 1, "EMPLEADO", "FR", "", false, 180.00m },
    { 99.99m, 1, "NORMAL", "FR", "", false, 129.89m },
    { 100m, 1, "NORMAL", "FR", "", false, 120.00m },
    { 80m, 1, "NORMAL", "US", "", false, 104.9m },
    { 200m, 1, "NORMAL", "US", "", false, 190.00m },
    { 199.99m, 1, "NORMAL", "US", "", false, 214.89m },
    { 60m, 1, "NORMAL", "ES", "PROMO10", false, 60.50m },
    { 50m, 1, "NORMAL", "ES", "PROMO10", false, 60.50m },
    { 51m, 1, "NORMAL", "ES", "PROMO10", false, 49.61m },
    { 150m, 1, "VIP", "ES", "PROMO10", false, 142.18m },
    { 20m, 1, "NORMAL", "ES", "", true, 31.62m },
    { 20m, 1, "NORMAL", "FR", "", true, 38.85m },
    { 20m, 1, "NORMAL", "US", "", true, 57.35m },
    { 200m, 1, "NORMAL", "US", "", true, 190.00m },
    { 9.99m, 3, "NORMAL", "ES", "", false, 41.21m },
    { 0m, 1, "NORMAL", "ES", "", false, 4.95m }
    };

    protected abstract decimal Calcular(decimal precio, int cantidad, string cliente, string pais, string cupon, bool expres);

    [Theory, MemberData(nameof(Casos))]
    public void MismoResultadoQueElCodigoOriginal(decimal precio, int cantidad, string cliente, string pais, string cupon, bool expres, decimal esperado) =>
        Assert.Equal(esperado, Calcular(precio, cantidad, cliente, pais, cupon, expres));

    [Fact]
    public void ConPaisNoSoportado_Falla() =>
        Assert.ThrowsAny<Exception>(() => Calcular(10m, 1, "NORMAL", "XX", "", false));
}

public class CaracterizacionAntes : CaracterizacionBase
{
    protected override decimal Calcular(decimal precio, int cantidad, string cliente, string pais, string cupon, bool expres) =>
        new Limpio.Antes.pedidoHelper().calc(
            new List<Limpio.Antes.item> { new() { n = "x", p = precio, q = cantidad } }, cliente, pais, cupon, expres);
}

public class CaracterizacionDespues : CaracterizacionBase
{
    protected override decimal Calcular(decimal precio, int cantidad, string cliente, string pais, string cupon, bool expres)
    {
        var tipo = cliente switch { "VIP" => Limpio.Despues.TipoCliente.Vip, "EMPLEADO" => Limpio.Despues.TipoCliente.Empleado, _ => Limpio.Despues.TipoCliente.Normal };
        var destino = pais switch { "ES" => Limpio.Despues.Pais.España, "FR" => Limpio.Despues.Pais.Francia, "US" => Limpio.Despues.Pais.EstadosUnidos, _ => (Limpio.Despues.Pais)99 };
        var pedido = new Limpio.Despues.Pedido(new[] { new Limpio.Despues.LineaDePedido("x", precio, cantidad) }, tipo, destino, cupon, expres);
        return Limpio.Despues.CalculadoraDePedidos.Total(pedido);
    }
}
```

Resultado:

```text
$ dotnet test
Passed!  - Failed: 0, Passed: 50, Skipped: 0, Total: 50
```

**50 = 24 casos × 2 implementaciones + 2 pruebas de «país no soportado».** Los **mismos** 24 casos dan **el mismo resultado** en el código viejo y en el nuevo.

### Medir en lugar de opinar: el compilador como revisor

Se pidió al compilador que **impusiera** las convenciones (`.editorconfig`) y que **midiera la complejidad ciclomática** (el número de caminos independientes por el código: cuantos más, más difícil de entender y de probar).

`.editorconfig` (las reglas de nombres y el umbral de complejidad):

```text
root = true

[*.cs]
# ---- Convenciones de nombres, impuestas por el compilador (severidad error) ----
# Tipos, métodos, propiedades y campos públicos: PascalCase
dotnet_naming_rule.publicos_en_pascal.severity = error
dotnet_naming_rule.publicos_en_pascal.symbols = publicos
dotnet_naming_rule.publicos_en_pascal.style = pascal
dotnet_naming_symbols.publicos.applicable_kinds = class, struct, enum, method, property, field, event
dotnet_naming_symbols.publicos.applicable_accessibilities = public, protected, internal
dotnet_naming_style.pascal.capitalization = pascal_case

# Interfaces: empiezan por I
dotnet_naming_rule.interfaces_con_i.severity = error
dotnet_naming_rule.interfaces_con_i.symbols = interfaces
dotnet_naming_rule.interfaces_con_i.style = con_i
dotnet_naming_symbols.interfaces.applicable_kinds = interface
dotnet_naming_style.con_i.required_prefix = I
dotnet_naming_style.con_i.capitalization = pascal_case

# Constantes y campos estáticos (aunque sean privados): PascalCase.
# IMPORTANTE: las reglas se evalúan EN ORDEN y gana la primera que coincide, así que la más específica va ANTES.
dotnet_naming_rule.constantes_en_pascal.severity = error
dotnet_naming_rule.constantes_en_pascal.symbols = constantes
dotnet_naming_rule.constantes_en_pascal.style = pascal
dotnet_naming_symbols.constantes.applicable_kinds = field
dotnet_naming_symbols.constantes.required_modifiers = static     # incluye las const (que son estáticas) y los static readonly

# Campos privados (de instancia): _camelCase
dotnet_naming_rule.privados_con_guion.severity = error
dotnet_naming_rule.privados_con_guion.symbols = privados
dotnet_naming_rule.privados_con_guion.style = guion_camel
dotnet_naming_symbols.privados.applicable_kinds = field
dotnet_naming_symbols.privados.applicable_accessibilities = private
dotnet_naming_style.guion_camel.required_prefix = _
dotnet_naming_style.guion_camel.capitalization = camel_case

# ---- Complejidad ciclomática: aviso si un método supera el umbral de CodeMetricsConfig.txt ----
dotnet_diagnostic.CA1502.severity = warning
dotnet_diagnostic.IDE1006.severity = error
```

Y el resultado de **compilar cada versión**:

```text
$ dotnet build MedirAntes --no-incremental   (nombres: error · complejidad máx. 8: aviso)
Pedidos.cs(3,14): error IDE1006: Naming rule violation: These words must begin with upper case characters: item
Pedidos.cs(3,35): error IDE1006: Naming rule violation: These words must begin with upper case characters: n
Pedidos.cs(3,53): error IDE1006: Naming rule violation: These words must begin with upper case characters: p
Pedidos.cs(3,67): error IDE1006: Naming rule violation: These words must begin with upper case characters: q
Pedidos.cs(5,14): error IDE1006: Naming rule violation: These words must begin with upper case characters: pedidoHelper
Pedidos.cs(8,20): error IDE1006: Naming rule violation: These words must begin with upper case characters: calc
Pedidos.cs(8,20): warning CA1502: 'calc' has a cyclomatic complexity of '17'. Rewrite or refactor the code to decrease its complexity below '9'.
1 Warning(s)
6 Error(s)

$ dotnet build MedirDespues --no-incremental   (nombres: error · complejidad máx. 8: aviso)
0 Warning(s)
0 Error(s)
```

Y la complejidad de **cada método**:

```text
$ complejidad ciclomática de cada método (umbral 1 para que el analizador los liste todos)
-- Antes
   calc → 17
-- Despues
   Para → 2
   DescuentoPorCupon → 3
   DescuentoPorCliente → 2
   CosteDeEnvio → 3
```

**Cómo se lee:**

| | Antes | Después |
|---|---|---|
| Errores de nombres | **6** | **0** |
| Complejidad del método principal | **17** (`calc`) | **≤ 3** en todos |
| Un método que hace de todo | 1 de ~22 líneas | **7 métodos** cortos, cada uno con su nombre |
| Para añadir un país | Tocar **2 `if/else if`** repartidos | **Una línea** en una tabla |
| Pruebas de caracterización | Mismos 24 casos | Mismos 24 casos |

> **Dos descubrimientos al montar la medición** (los cuento porque son lo que más tiempo hace perder):
>
> 1. **Las reglas de nombres no se aplicaban.** `EnforceCodeStyleInBuild` **no bastaba**: hacía falta además **`dotnet_diagnostic.IDE1006.severity = error`**. Y, al enlazar el archivo fuente desde otra carpeta, el `.editorconfig` **tampoco se aplicaba**, porque se asocia por la **ruta real del archivo**.
> 2. **Mi regla se pasó de estricta.** Exigí `_camelCase` a «todos los campos privados» y el compilador me marcó como error las **constantes** (`private const …`), que se escriben en `PascalCase`. **Las reglas de nombres se evalúan en orden y gana la primera que coincide**: la más específica (constantes y estáticos) debe ir **antes**.

### `#region` y `#pragma`

| Directiva | Qué hace | Cuándo |
|---|---|---|
| `#region Nombre` … `#endregion` | Un **pliegue** visual en el IDE. **No cambia nada** al ejecutar | Con **mucha moderación**: si necesitas regiones, probablemente la clase **es demasiado grande** |
| `#pragma warning disable CS0219` … `#pragma warning restore CS0219` | Silencia un aviso **solo en ese fragmento** | Cuando **sabes** que un aviso es un falso positivo; **siempre con `restore`** y un comentario del porqué |
| `#nullable enable/disable` | Activa o desactiva el análisis de nulos en un fragmento | Al migrar código antiguo, poco a poco |

```text
$ dotnet run sin-pragma.cs
sin-pragma.cs(1,5): warning CS0219: The variable 'sinUsar' is assigned but its value is never used
hola

$ dotnet run con-pragma.cs
hola
(sin avisos: el #pragma los silenció; #region no altera el resultado)
```

### Errores típicos

| Síntoma | Causa | Solución |
|---|---|---|
| «Refactoricé y se rompió algo» | Sin pruebas | **Primero** pruebas de caracterización |
| Abstracciones para «por si acaso» | Se confunde DRY con YAGNI | Regla de tres |
| Un método de 200 líneas | Crecimiento sin control | Extraer funciones con nombre |
| Comentarios que mienten | El código cambió, el texto no | Renombrar; comentar el **porqué**, no el **qué** |
| `Exception("error")` | Atajo | Excepciones **propias** con significado (como `ReglaDeNegocioException` de Reservas) |
| Revisiones de código sobre estilo | Sin automatizar | `.editorconfig` + analizadores en la compilación |
| Parámetros booleanos (`Calcular(x, true)`) | Cambian el comportamiento | Dos métodos, o un `enum` |
| Silenciar avisos en masa | Comodidad | `#pragma` solo local y comentado |

> **Ejercicio C.1:** añade el país «Portugal» (envío 6,90 €, gratis desde 75 €, IVA 23 %) al código **nuevo**. Cuenta cuántas líneas tocas, y cuántas habría tocado en el viejo.
>
> **Ejercicio C.2:** encuentra en tu código un método con complejidad > 8 (usa `CA1502` con el umbral) y refactorízalo con **pruebas de caracterización primero**.
>
> **Ejercicio C.3:** el original calculaba el IVA **sin incluir el envío**. ¿Es correcto? Escribe **una prueba** que fije lo que **debería** pasar y observa qué ocurre con los 24 casos de caracterización. (Pista: es la diferencia entre **preservar** comportamiento y **decidir** cuál es el correcto.)

---

## Resumen de los anexos

| Si quieres… | Haz… |
|---|---|
| Liberar un recurso a tiempo | `IDisposable` + `using` |
| Un respaldo si se olvidó el `Dispose` | Finalizador (o mejor, `SafeHandle`) |
| Evitar fugas por eventos | Desuscribirse (`-=`) en `Dispose`/`OnDisable` |
| Evitar basura en bucles calientes | `stackalloc`/`Span<T>`, reutilizar objetos |
| Que un error de tipo se vea al compilar | Tipado estático: evita `object`/`dynamic` |
| Leer JSON sin clases | `JsonNode` (no `dynamic`) |
| Refactorizar con seguridad | Pruebas de caracterización primero |
| Que las convenciones no se discutan | `.editorconfig` + `EnforceCodeStyleInBuild` + severidad explícita |
| Saber si un método es demasiado complejo | CA1502 (complejidad ciclomática) |
