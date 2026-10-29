# Curso de C# · Parte IV: .NET por dentro y herramientas

Hasta aquí aprendiste **el lenguaje**. Esta parte explica **lo que hay debajo y alrededor**: qué es .NET, qué hace el compilador, cómo se organiza una solución, cómo se versiona el código, cómo se depura y cómo elegir bien una estructura de datos. Es el bloque de «saber lo que haces» que separa a quien copia comandos de quien entiende el entorno.

*Verificado: 2026-10-29 con .NET SDK 10.0.113 en Linux. Los comandos y programas se ejecutaron. **No se ejecutó** nada que dependa de un IDE gráfico (Visual Studio, Rider): esos pasos se describen, y se marcan como tales.*

## Mapa de la Parte IV

| Lección | Tema | Lo verás ejecutado |
|---|---|---|
| 37 | Qué es .NET: CLR, IL, JIT, AOT, versiones y soporte | IL desensamblado, JIT medido, AOT publicado, `global.json` |
| 38 | Soluciones, proyectos, ensamblados, NuGet y Git | Solución de 2 proyectos, paquete `.nupkg` propio, conflicto de Git real |
| 39 | Depuración, trazas y registro | `Debug`/`Trace`/`ILogger`, Debug frente a Release |
| 40 | Tipos por valor/referencia, estructuras de datos y rendimiento | Mediciones: `List` frente a `HashSet`, boxing, `string` frente a `StringBuilder` |

---

## Lección 37 · Qué es .NET en 2026

### Qué vas a aprender

- La diferencia entre **lenguaje**, **plataforma** y **herramienta** (que se confunden constantemente).
- Qué pasa entre que escribes `Console.WriteLine` y que algo aparece en pantalla: **compilador → IL → CLR → JIT → procesador**.
- Qué significan *código administrado*, *compilado frente a interpretado* y *AOT*.
- Qué versiones existen, **cuál se usa y hasta cuándo tiene soporte**, y cómo fijar la versión de tu proyecto.

### La idea

#### No confundas estas tres cosas

| Es… | Se llama… | Ejemplos | Analogía |
|---|---|---|---|
| **Lenguaje** | C# | `class`, `async`, `record` | El idioma en que escribes |
| **Plataforma** | .NET | Runtime (CLR), biblioteca de clases (BCL), SDK | El país con sus leyes, carreteras y servicios |
| **Herramienta (IDE / editor)** | Visual Studio, VS Code, Rider | Resaltado, depurador, plantillas | El procesador de textos con que redactas |

Se puede escribir C# sin Visual Studio (como en todo este curso), y se pueden escribir otros lenguajes (F#, Visual Basic) que corren sobre la misma plataforma .NET.

#### Quién usa .NET y para qué

Aplicaciones web y APIs (ASP.NET Core), servicios en la nube, escritorio (WPF, WinForms), móviles (.NET MAUI), videojuegos (Unity usa C#), herramientas de consola, microservicios, IoT. Es multiplataforma: **Windows, Linux y macOS**.

#### Las «familias» de .NET (el origen de «Core frente a Standard»)

| Nombre | Qué fue | Estado |
|---|---|---|
| **.NET Framework** (2002) | La plataforma original, **solo Windows** | Congelada en la 4.8. Se mantiene, no evoluciona |
| **.NET Core** (2016) | Reescritura abierta y multiplataforma | Evolucionó hacia el «.NET» actual |
| **.NET Standard** | Un **contrato** de APIs para que una biblioteca funcionara en Framework y en Core | Solo importa si mantienes bibliotecas que deben llegar al Framework antiguo (`netstandard2.0`) |
| **.NET 5, 6, 7… 10** (2020 →) | La unificación: **una sola plataforma**, sin la palabra «Core» | **Lo que usas hoy** |
| **Mono / Xamarin** | Runtimes para móviles y navegador | Absorbidos en .NET 5+ y .NET MAUI |

Regla práctica: **proyecto nuevo = .NET moderno** (`net10.0`). Solo tocas `netstandard2.0` o `net48` por compatibilidad con código antiguo.

#### Versiones y soporte (datos de Microsoft, consultados el 2026-10-29)

| Versión | Tipo | Salida | Fin de soporte | C# por defecto |
|---|---|---|---|---|
| .NET 8 | **LTS** | nov 2023 | **10 nov 2026** | C# 12 |
| .NET 9 | STS | nov 2024 | **10 nov 2026** | C# 13 |
| **.NET 10** | **LTS** | **11 nov 2025** | **14 nov 2028** | **C# 14** |

- **LTS** (*Long Term Support*): 3 años de parches. Es la elección para producción que no quieres migrar cada año.
- **STS** (*Standard Term Support*): desde .NET 9 tiene **24 meses** (antes eran 18).
- Se publica una versión **cada noviembre**: las pares son LTS (8, 10, 12…) y las impares STS.
- **Importante hoy:** .NET 8 y .NET 9 **dejan de recibir parches de seguridad el 10 de noviembre de 2026**, es decir, **días después de la fecha de este curso**. Si mantienes algo en esas versiones, migrar a .NET 10 es tarea pendiente.
- Fuente oficial, que cambia con el tiempo: la página *.NET Support Policy* de Microsoft. Consúltala siempre antes de decidir.

#### El viaje del código: de `.cs` a la pantalla

```text
 Program.cs ──► [Compilador Roslyn] ──► Program.dll ──► [CLR] ──► [JIT] ──► código de máquina ──► CPU
  (tu C#)                            (IL + metadatos)   (runtime)  (compila al vuelo)
```

1. **Roslyn** (el compilador de C#) **no genera código de máquina**. Genera **IL** (*Intermediate Language*): instrucciones de una máquina virtual imaginaria, guardadas en un `.dll` o `.exe` junto con **metadatos** (qué tipos y métodos existen).
2. Al ejecutar, el **CLR** (*Common Language Runtime*) carga el ensamblado, verifica y gestiona la memoria.
3. El **JIT** (*Just-In-Time*) compila el IL a **código de máquina real** la **primera vez que se llama** a cada método.
4. Con la **compilación por niveles** (*tiered compilation*) el JIT primero compila rápido y sin optimizar, y **recompila optimizado** los métodos que se llaman mucho.

**¿Interpretado o compilado?** Ni lo uno ni lo otro puros. Un lenguaje *interpretado* ejecuta el texto o un código intermedio instrucción a instrucción (Python clásico). Uno *compilado* a nativo traduce todo antes de ejecutar (C, Rust). C# es **compilado a IL y luego compilado a nativo (JIT)**; con **AOT** (*Ahead-Of-Time*) se hace la segunda parte **antes de publicar**.

**Código administrado (*managed*) frente a no administrado (*unmanaged*):** el administrado corre bajo el CLR, que le da **recolector de basura** (libera la memoria que ya nadie usa), comprobación de límites en arreglos y seguridad de tipos. El **no administrado** (C/C++, bibliotecas del sistema) maneja su propia memoria. Se comunican mediante **P/Invoke** (`DllImport`), que verás abajo.

#### Qué trae .NET 10 que conviene conocer

| Novedad | Qué es | ¿Probado aquí? |
|---|---|---|
| **C# 14** | Miembros de extensión, `field`, `a?.b = x`… (lección 41) | **Sí** |
| **Aplicaciones de un solo archivo** | `dotnet run archivo.cs`, con `#:package` y `#:property` (todo este curso) | **Sí** |
| **Native AOT** | Publicar un ejecutable nativo sin runtime | **Sí**, más abajo |
| **Aspire** | Herramienta para orquestar aplicaciones distribuidas en desarrollo. Desde la versión 13 se llama solo «Aspire» (antes «.NET Aspire») | No (requiere contenedores) |
| **Contenedores** | `dotnet publish` puede crear la imagen Docker directamente | No (sin demonio Docker aquí) |
| **NuGet** | Auditoría de seguridad de paquetes (`NuGetAudit`) activada por defecto | Parcial |
| **Globalización** | Modo invariante, ICU: cómo se ordenan y comparan textos según la cultura | Parcial (lección 5) |

> **Honestidad:** estas novedades se listan para que sepas que existen. Solo las marcadas «Sí» se ejecutaron en este curso.

#### «¿Hay un gestor de versiones o algo así?» — son **tres cosas distintas**

La pregunta mezcla herramientas que se parecen en el nombre y no en el trabajo:

| Quiero controlar… | Herramienta | Qué hace | Dónde se ve |
|---|---|---|---|
| **El historial de mi código** (qué cambié, cuándo, deshacer, trabajar en equipo) | **Git** (con GitHub, GitLab…) | Guarda «fotos» (commits) de tu proyecto | Lección 38 |
| **Qué versión de .NET usa mi proyecto** | `global.json` y `<TargetFramework>` | Fija el SDK y el framework de destino | Aquí, en el código |
| **Qué versión de cada biblioteca uso** | **NuGet** (`PackageReference`) | Descarga paquetes y sus versiones | Lección 38 |

Si lo que buscas es lo que en otros lenguajes hacen *nvm*, *pyenv* o *sdkman* (cambiar entre versiones del lenguaje instaladas), en .NET lo cubre `global.json`: varios SDK pueden convivir instalados, y **el archivo decide cuál se usa en esa carpeta**.

### Código 1 (`37-clr-il.cs`): mirar el runtime por dentro

```csharp
#:property PublishAot=false

using System.Reflection;
using System.Reflection.Emit;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// 1) ¿Sobre qué runtime estoy corriendo?
Console.WriteLine($"Runtime ......: {RuntimeInformation.FrameworkDescription}");
Console.WriteLine($"SO / arquitec.: {RuntimeInformation.OSDescription} / {RuntimeInformation.ProcessArchitecture}");
Console.WriteLine($"Versión CLR ..: {Environment.Version}");
Console.WriteLine($"Ensamblado ...: {typeof(Calculadora).Assembly.GetName().Name} (ejecutable en memoria)");
Console.WriteLine($"Ensamblado de List<T>: {typeof(List<int>).Assembly.GetName().Name}");

// 2) El compilador NO genera código de máquina: genera IL (lenguaje intermedio).
//    Aquí lo leemos del propio programa y lo "desensamblamos" a mano.
Console.WriteLine("\n--- IL del método Calculadora.Sumar(int a, int b) ---");
MethodInfo sumar = typeof(Calculadora).GetMethod(nameof(Calculadora.Sumar))!;
byte[] il = sumar.GetMethodBody()!.GetILAsByteArray()!;
Console.WriteLine("Bytes: " + Convert.ToHexString(il));
Desensamblar(il);

Console.WriteLine("\n--- IL de un método con un bucle: Calculadora.SumaHasta(int n) ---");
Desensamblar(typeof(Calculadora).GetMethod(nameof(Calculadora.SumaHasta))!.GetMethodBody()!.GetILAsByteArray()!);

// 3) El JIT compila el IL a código de máquina la PRIMERA vez que se llama a un método.
Console.WriteLine("\n--- JIT: primera llamada frente a las siguientes ---");
long compilacionesAntes = JitInfo.GetCompiledMethodCount();
var t0 = System.Diagnostics.Stopwatch.GetTimestamp();
Calculadora.SumaHasta(10);                                     // aquí el JIT compila SumaHasta
double primera = System.Diagnostics.Stopwatch.GetElapsedTime(t0).TotalMicroseconds;
t0 = System.Diagnostics.Stopwatch.GetTimestamp();
Calculadora.SumaHasta(10);                                     // ya está compilado: solo se ejecuta
double segunda = System.Diagnostics.Stopwatch.GetElapsedTime(t0).TotalMicroseconds;
Console.WriteLine($"Métodos compilados por el JIT en este tramo: {JitInfo.GetCompiledMethodCount() - compilacionesAntes}");
Console.WriteLine($"Primera llamada (compila + ejecuta): {(primera > segunda ? "más lenta" : "similar")}");
Console.WriteLine($"Segunda llamada es {primera / Math.Max(segunda, 0.001):F0}x más rápida que la primera (los valores exactos cambian en cada máquina)");

// 4) Compilación por niveles (tiered compilation): primero rápido y sin optimizar, luego optimizado si el método es "caliente".
bool tieredActiva = !(AppContext.TryGetSwitch("System.Runtime.TieredCompilation", out bool valor) && !valor);
Console.WriteLine($"\nTiered compilation activa: {(tieredActiva ? "sí" : "no")}");

// 5) Código administrado (managed) frente a no administrado (unmanaged).
Console.WriteLine("\n--- Managed vs unmanaged ---");
int[] administrado = new int[1000];                             // lo gestiona el recolector de basura (GC)
Console.WriteLine($"Array administrado: generación del GC = {GC.GetGeneration(administrado)}");
IntPtr bloque = Marshal.AllocHGlobal(1024);                     // memoria NO administrada: el GC no la ve, hay que liberarla
Console.WriteLine($"Bloque no administrado en una dirección de memoria: {(bloque != IntPtr.Zero ? "reservado" : "falló")}");
Marshal.FreeHGlobal(bloque);                                    // si olvidas esta línea hay una fuga de memoria
Console.WriteLine($"PID desde una función C nativa (P/Invoke a libc): {NativeLibc.getpid() == Environment.ProcessId}");
Console.WriteLine($"Modo del GC: {(GCSettings.IsServerGC ? "servidor" : "estación de trabajo")}, latencia: {GCSettings.LatencyMode}");

// --- desensamblador mínimo: traduce bytes de IL a nombres de instrucciones ---
static void Desensamblar(byte[] il)
{
    // Tabla opcode → nombre, obtenida por reflexión de System.Reflection.Emit.OpCodes
    // Las instrucciones de 1 byte se identifican por ese byte; las de 2 bytes empiezan por 0xFE.
    var tabla = typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static)
        .Select(f => (OpCode)f.GetValue(null)!)
        .ToDictionary(o => (ushort)o.Value, o => o);
    for (int i = 0; i < il.Length;)
    {
        int inicio = i;
        OpCode op = il[i] == 0xFE ? tabla[(ushort)(il[i] << 8 | il[i + 1])] : tabla[il[i]];
        i += op.Size;
        string operando = "";
        switch (op.OperandType)
        {
            case OperandType.InlineNone: break;
            case OperandType.ShortInlineBrTarget: operando = $"IL_{i + 1 + (sbyte)il[i]:X4}"; i += 1; break;
            case OperandType.ShortInlineI: case OperandType.ShortInlineVar: operando = il[i].ToString(); i += 1; break;
            case OperandType.InlineI: operando = BitConverter.ToInt32(il, i).ToString(); i += 4; break;
            default: operando = "(…)"; i += 4; break;
        }
        Console.WriteLine($"  IL_{inicio:X4}: {op.Name,-10} {operando}");
    }
}

public static class Calculadora
{
    public static int Sumar(int a, int b) => a + b;

    public static int SumaHasta(int n)
    {
        int total = 0;
        for (int i = 1; i <= n; i++) total += i;
        return total;
    }
}

public static partial class NativeLibc
{
    [DllImport("libc")] public static extern int getpid();      // P/Invoke: llamar a código nativo del sistema
}
```

### Cómo se lee, paso a paso

1. **`RuntimeInformation` y `Environment.Version`** dicen sobre qué runtime corres. Sirve para diagnosticar «en mi máquina funciona».
2. **`typeof(List<int>).Assembly`** es `System.Private.CoreLib`: la biblioteca base vive en un **ensamblado**, igual que tu código.
3. **`GetMethodBody().GetILAsByteArray()`** devuelve los **bytes de IL** del método. `Desensamblar` los traduce a nombres de instrucción usando la tabla `OpCodes`. Es lo que hace una herramienta como `ildasm`, en 25 líneas.
4. **Cómo leer el IL de `Sumar`:** `ldarg.0` (apila el argumento 0), `ldarg.1`, `add` (suma los dos de la pila), `ret` (devuelve). **El IL es una máquina de pila.**
5. **Cómo leer el bucle:** `br.s` salta a la condición; `cgt` + `ldc.i4.0` + `ceq` implementan «`i <= n`»; `brtrue.s` vuelve al cuerpo. Hay un `nop` inicial porque se compiló en **Debug** (en Release desaparece).
6. **`JitInfo.GetCompiledMethodCount()`** cuenta métodos compilados por el JIT. La primera llamada a `SumaHasta` **paga la compilación**; la segunda no.
7. **`Marshal.AllocHGlobal` / `FreeHGlobal`:** memoria **no administrada**. El recolector de basura no la ve: si olvidas liberarla, hay una **fuga**.
8. **`[DllImport("libc")] getpid()`** llama a una función C del sistema (P/Invoke).

### Resultado real

```text
Runtime ......: .NET 10.0.13
SO / arquitec.: Ubuntu 24.04.5 LTS / X64
Versión CLR ..: 10.0.13
Ensamblado ...: 37-clr-il (ejecutable en memoria)
Ensamblado de List<T>: System.Private.CoreLib

--- IL del método Calculadora.Sumar(int a, int b) ---
Bytes: 0203582A
  IL_0000: ldarg.0    
  IL_0001: ldarg.1    
  IL_0002: add        
  IL_0003: ret        

--- IL de un método con un bucle: Calculadora.SumaHasta(int n) ---
  IL_0000: nop        
  IL_0001: ldc.i4.0   
  IL_0002: stloc.0    
  IL_0003: ldc.i4.1   
  IL_0004: stloc.1    
  IL_0005: br.s       IL_000F
  IL_0007: ldloc.0    
  IL_0008: ldloc.1    
  IL_0009: add        
  IL_000A: stloc.0    
  IL_000B: ldloc.1    
  IL_000C: ldc.i4.1   
  IL_000D: add        
  IL_000E: stloc.1    
  IL_000F: ldloc.1    
  IL_0010: ldarg.0    
  IL_0011: cgt        
  IL_0013: ldc.i4.0   
  IL_0014: ceq        
  IL_0016: stloc.2    
  IL_0017: ldloc.2    
  IL_0018: brtrue.s   IL_0007
  IL_001A: ldloc.0    
  IL_001B: stloc.3    
  IL_001C: br.s       IL_001E
  IL_001E: ldloc.3    
  IL_001F: ret        

--- JIT: primera llamada frente a las siguientes ---
Métodos compilados por el JIT en este tramo: 1
Primera llamada (compila + ejecuta): más lenta
Segunda llamada es 142800x más rápida que la primera (los valores exactos cambian en cada máquina)

Tiered compilation activa: sí

--- Managed vs unmanaged ---
Array administrado: generación del GC = 0
Bloque no administrado en una dirección de memoria: reservado
PID desde una función C nativa (P/Invoke a libc): True
Modo del GC: estación de trabajo, latencia: Interactive
```

> **Lee los números con criterio:** «1980x más rápida» es un efecto real (compilar + ejecutar frente a solo ejecutar), pero **el valor exacto cambia en cada ejecución**. La conclusión que importa es cualitativa: **la primera llamada a algo es más cara**.

### Código 2: versiones del SDK, `global.json` y `net10.0`

Estos son **comandos de terminal** (cada uno explicado):

| Comando | Qué hace |
|---|---|
| `dotnet --list-sdks` | Lista los SDK instalados (el SDK contiene compilador y herramientas) |
| `dotnet --list-runtimes` | Lista los runtimes instalados (lo que ejecuta programas ya compilados) |
| `dotnet new globaljson --sdk-version 10.0.113 --roll-forward latestFeature` | Crea `global.json` fijando la versión; `rollForward` define cuánto se permite «subir» a una versión posterior |
| `dotnet --version` | Muestra el SDK que se usaría **en esta carpeta** (respeta `global.json`) |
| `dotnet build -c Release` | Compila en configuración Release |

```text
$ dotnet --list-sdks
10.0.113 [/usr/lib/dotnet/sdk]

$ dotnet --list-runtimes
Microsoft.AspNetCore.App 10.0.13 [/usr/lib/dotnet/shared/Microsoft.AspNetCore.App]
Microsoft.NETCore.App 10.0.13 [/usr/lib/dotnet/shared/Microsoft.NETCore.App]

$ dotnet new globaljson --sdk-version 10.0.113 --roll-forward latestFeature


$ cat global.json
{
  "sdk": {
    "rollForward": "latestFeature",
    "version": "10.0.113"
  }
}

$ dotnet --version   (con global.json presente)
10.0.113

--- pedimos un SDK que NO está instalado, con rollForward desactivado ---
$ dotnet --version
The command could not be loaded, possibly because:
  * You intended to execute a .NET application:
      The application '--version' does not exist or is not a managed .dll or .exe.
  * You intended to execute a .NET SDK command:

$ dotnet new console -n Hola -o Hola



$ cat Hola/Hola.csproj
﻿<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>

$ dotnet build -c Release Hola | tail -3
    0 Error(s)

Time Elapsed 00:00:01.65

$ ls Hola/bin/Release/net10.0
Hola
Hola.deps.json
Hola.dll
Hola.pdb
Hola.runtimeconfig.json

$ dotnet Hola/bin/Release/net10.0/Hola.dll   (un .dll ejecutable con el host "dotnet")
Hello, World!
```

**Qué observar:**

- Con `global.json` pidiendo un SDK **9.0.100** y `rollForward: disable`, el sistema responde **«A compatible .NET SDK was not found»** y lista lo instalado: el archivo **manda** sobre lo instalado. Es exactamente lo que quieres en un equipo: que todos compilen con la misma versión.
- Valores de `rollForward`: `disable` (exactamente esa), `patch`, `feature`, `latestPatch`, `latestFeature`, `latestMajor`… de lo más estricto a lo más flexible.
- El `.csproj` de un proyecto nuevo de .NET 10 tiene **muy pocas líneas** (se ve arriba). `TargetFramework` = `net10.0` es el **moniker** (TFM): qué plataforma usa **este proyecto**. No es lo mismo que el SDK: **un SDK 10 puede compilar proyectos `net8.0`**, si el runtime de destino está disponible.
- Un `.dll` compilado se ejecuta con `dotnet archivo.dll` (el «host» busca el runtime). Junto a él, `Hola.runtimeconfig.json` dice qué runtime necesita, y `Hola.deps.json` sus dependencias.

### Código 3: Native AOT, medido

`PublishAot=true` compila a **código nativo antes de publicar**:

| Comando | Qué hace |
|---|---|
| `dotnet publish -c Release -r linux-x64 -p:PublishAot=true -o aot` | `-c Release` optimizado; `-r linux-x64` plataforma de destino; `-p:PublishAot=true` activa AOT; `-o aot` carpeta de salida |
| `file aot/Hola` | Muestra qué tipo de archivo es |
| `ldd aot/Hola` | Lista de qué bibliotecas del sistema necesita |

```text
$ dotnet publish -c Release -r linux-x64 -p:PublishAot=true -o aot     (compilación AOT: IL → código nativo ANTES de ejecutar)
$ ls -la aot/Hola
1270784 aot/Hola

$ file aot/Hola
aot/Hola: ELF 64-bit LSB pie executable, x86-64, version 1 (SYSV), dynamically linked, interpreter /lib64/ld-l

$ ldd aot/Hola | head -3   (no depende del runtime de .NET)
	linux-vdso.so.1 (0x00007fafa2af1000)
	libm.so.6 => /lib/x86_64-linux-gnu/libm.so.6 (0x00007fafa2896000)
	libc.so.6 => /lib/x86_64-linux-gnu/libc.so.6 (0x00007fafa2600000)

$ dotnet publish -c Release -o normal   (publicación normal, necesita runtime instalado)
  Hola -> /tmp/claude-0/-home-user-claude/ddd43a70-8f87-5490-9d3f-755ecbba3013/scratchpad/curso5/lab/normal/

$ du -sh aot normal
3.6M	aot
112K	normal

$ tiempo de arranque (media de 20 ejecuciones)
  AOT nativo ............:    2.8 ms
  IL + JIT (dotnet .dll) :   34.4 ms
```

**Cómo se interpreta:**

- El resultado es un **ejecutable ELF nativo** que **no necesita instalar .NET** (`ldd` solo muestra `libc` y `libm` del sistema).
- **Arranque:** 2,8 ms frente a 34,4 ms. Es la gran ventaja de AOT (útil en *serverless*, herramientas de consola).
- **Tamaño:** 3,6 MB frente a 112 KB: el AOT **incluye el runtime recortado dentro**. El «normal» pesa poco **porque depende del runtime instalado**.
- **Precio a pagar:** AOT limita la **reflexión**, la carga dinámica de código y algunas bibliotecas. Por eso `#:property PublishAot=false` aparece en los archivos de este curso (los archivos sueltos se publican con AOT por defecto en .NET 10 y entonces la reflexión daría problemas).
- **ReadyToRun** (`PublishReadyToRun`) es un término medio: precompila parte del código pero mantiene el JIT. No se probó aquí.

### Errores típicos

| Síntoma | Causa | Solución |
|---|---|---|
| «A compatible .NET SDK was not found» | `global.json` pide una versión que no tienes | Instala esa versión o ajusta `rollForward` |
| «You must install or update .NET to run this application» | Falta el **runtime** que exige `runtimeconfig.json` | Instala el runtime, o publica *self-contained* |
| La app funciona en tu máquina y no en el servidor | Versión del runtime distinta | Fija `TargetFramework` y `global.json`; pruébalo en contenedor |
| AOT falla o se comporta distinto | Reflexión o código dinámico | Usa generadores de código (lección 42) y revisa los avisos de recorte (*trimming*) |

> **Ejercicio 37.1:** ejecuta `dotnet --list-sdks` en tu equipo y crea un `global.json` que fije tu versión con `rollForward: latestPatch`.
>
> **Ejercicio 37.2:** desensambla con `Desensamblar` un método con un `if/else` y localiza la instrucción de salto.
>
> **Ejercicio 37.3:** publica tu propio programa con y sin AOT y compara tamaño y arranque.

---

## Lección 38 · Soluciones, proyectos, ensamblados, NuGet y Git

### Qué vas a aprender

- La jerarquía **solución → proyecto → ensamblado → espacio de nombres**.
- Crear una solución con varios proyectos, **referenciarlos** entre sí y ver qué se copia al compilar.
- **Empaquetar tu propia biblioteca como paquete NuGet** y consumirla.
- **Git** lo bastante para no perder trabajo: commit, rama, fusión y **resolver un conflicto**.

### La idea

| Concepto | Qué es | Archivo | Analogía |
|---|---|---|---|
| **Solución** (*solution*) | Una carpeta lógica que agrupa varios proyectos | `.sln` o `.slnx` | La carpeta de una obra completa |
| **Proyecto** | Una unidad que se compila en **un** ensamblado | `.csproj` | Un plano concreto (cocina, fontanería…) |
| **Ensamblado** (*assembly*) | El resultado compilado: **`.dll`** (biblioteca) o `.exe`/`.dll` ejecutable | `Tienda.Nucleo.dll` | La pieza ya fabricada |
| **Espacio de nombres** (*namespace*) | Organiza los **nombres** dentro del código | `namespace Tienda.Nucleo` | Los apellidos de una clase |
| **Referencia de proyecto** | «Mi proyecto usa a este otro» | `<ProjectReference>` | Un cable entre dos piezas |
| **Paquete NuGet** | Un `.dll` + metadatos, **versionado** y compartible | `.nupkg` (un zip) | Una pieza con ficha y número de lote |

Dos aclaraciones que confunden:

- **Proyecto compartido (*Shared Project*):** su código **se copia dentro de cada proyecto que lo usa** y no produce un `.dll` propio. Es un recurso antiguo (Xamarin/Windows). Para compartir código hoy se usa una **biblioteca de clases** (`classlib`).
- **Un espacio de nombres no es un ensamblado.** Un ensamblado puede contener muchos espacios de nombres, y un mismo espacio de nombres puede repartirse en varios ensamblados.

### Código 1: una solución con dos proyectos y un paquete NuGet

| Comando | Qué hace |
|---|---|
| `dotnet new sln -n Tienda` | Crea la solución vacía |
| `dotnet new classlib -n Tienda.Nucleo -o src/Tienda.Nucleo` | Crea una **biblioteca** (genera un `.dll`) |
| `dotnet new console -n Tienda.App -o src/Tienda.App` | Crea una aplicación de consola |
| `dotnet sln add …` | Añade proyectos a la solución |
| `dotnet add A reference B` | Hace que el proyecto A use al B (`ProjectReference`) |
| `dotnet pack … -p:Version=1.2.0` | Empaqueta una biblioteca como `.nupkg` con esa versión |
| `dotnet add Consumidor package X --version 1.2.0 --source ./feed` | Añade un paquete desde una carpeta local (un *feed*) |
| `dotnet list … package --outdated` | Informa de paquetes con versión más nueva disponible |

```text
$ dotnet new sln -n Tienda

$ dotnet new classlib -n Tienda.Nucleo -o src/Tienda.Nucleo

$ dotnet new console -n Tienda.App -o src/Tienda.App

$ dotnet sln add src/Tienda.Nucleo src/Tienda.App
Project `src/Tienda.Nucleo/Tienda.Nucleo.csproj` added to the solution.
Project `src/Tienda.App/Tienda.App.csproj` added to the solution.
$ dotnet add src/Tienda.App reference src/Tienda.Nucleo   (referencia de proyecto)
Reference `..\Tienda.Nucleo\Tienda.Nucleo.csproj` added to the project.

$ cat src/Tienda.App/Tienda.App.csproj | grep -i reference
    <ProjectReference Include="..\Tienda.Nucleo\Tienda.Nucleo.csproj" />

$ dotnet sln list
Project(s)
----------
src/Tienda.App/Tienda.App.csproj
src/Tienda.Nucleo/Tienda.Nucleo.csproj

$ dotnet run --project src/Tienda.App
100 € netos = 121.00 € con IVA
La clase Precio vive en el ensamblado: Tienda.Nucleo.dll

$ ls src/Tienda.App/bin/Debug/net10.0/*.dll   (el .dll de la biblioteca se copia junto al ejecutable)
Tienda.App.dll
Tienda.Nucleo.dll

--- NuGet: empaquetar la biblioteca y consumirla desde un "feed" local ---
$ dotnet pack src/Tienda.Nucleo -c Release -o feed -p:Version=1.2.0
  Successfully created package '/tmp/claude-0/-home-user-claude/ddd43a70-8f87-5490-9d3f-755ecbba3013/scratchpad/curso5/lab2/feed/Tienda.Nucleo.1.2.0.nupkg'.
$ ls feed
Tienda.Nucleo.1.2.0.nupkg
$ unzip -l feed/Tienda.Nucleo.1.2.0.nupkg   (un .nupkg es un .zip)
_rels/.rels
Tienda.Nucleo.nuspec
lib/net10.0/Tienda.Nucleo.dll
[Content_Types].xml
package/services/metadata/core-properties/2830531eaaf24609bcb1b3bced9ba28d.psmdcp

$ dotnet add Consumidor package Tienda.Nucleo --version 1.2.0 --source ./feed
info : Adding PackageReference for package 'Tienda.Nucleo' into project '/tmp/claude-0/-home-user-claude/ddd43a70-8f87-5490-9d3f-755ecbba3013/scratchpad/curso5/lab2/Consumidor/Consumidor.csproj'.
info : PackageReference for package 'Tienda.Nucleo' version '1.2.0' added to file '/tmp/claude-0/-home-user-claude/ddd43a70-8f87-5490-9d3f-755ecbba3013/scratchpad/curso5/lab2/Consumidor/Consumidor.csproj'.
$ grep PackageReference Consumidor/Consumidor.csproj
    <PackageReference Include="Tienda.Nucleo" Version="1.2.0" />

$ dotnet list Consumidor package   (qué paquetes usa un proyecto)
   Top-level Package      Requested   Resolved
   > Tienda.Nucleo        1.2.0       1.2.0   


$ dotnet list Consumidor package --outdated  (¿hay versiones nuevas? necesita internet)
   Top-level Package      Requested   Resolved   Latest                  
   > Newtonsoft.Json      12.0.1      12.0.1     13.0.4                  
   > Tienda.Nucleo        1.2.0       1.2.0      Not found at the sources
```

**Qué observar:**

- La referencia de proyecto es una sola línea en el `.csproj`. Al compilar, **`Tienda.Nucleo.dll` se copia junto al ejecutable**.
- Un `.nupkg` es un **zip**: contiene el `.dll` en `lib/net10.0/` y un `.nuspec` con los metadatos. Así se distribuye **todo** en NuGet.
- **Versionado semántico** (`MAYOR.MENOR.PARCHE`, como `1.2.0`): *parche* corrige errores sin cambiar nada visible; *menor* añade funciones compatibles; *mayor* rompe compatibilidad. En NuGet, `Version="1.2.0"` significa «**1.2.0 como mínimo**» (se usa la más baja que cumpla todos los requisitos); para fijar exactamente una versión se escribe `[1.2.0]`.
- `--outdated` muestra la versión instalada (`12.0.1`) y la última (`13.0.4`). Ojo: actualizar a una versión **mayor** puede romper tu código; actualiza de una en una y ejecuta las pruebas.
- Un paquete **referenciado desde tu propio feed** muestra «Not found at the sources» como última versión porque los orígenes públicos no lo conocen: es lo esperado.

### Código 2: Git en diez minutos

**Git no es GitHub.** Git es el programa que lleva el historial **en tu equipo**. GitHub (y GitLab, Azure DevOps…) son **servicios** que alojan copias remotas para colaborar.

| Comando | Qué hace |
|---|---|
| `git init -b main` | Crea el repositorio con la rama inicial `main` |
| `git add .` / `git commit -m "…"` | Prepara los cambios / guarda una «foto» con mensaje |
| `git status --short` | Qué cambió (`??` = no seguido, `UU` = conflicto) |
| `git switch -c rama` | Crea y cambia a otra rama (línea de trabajo paralela) |
| `git diff --stat` | Resumen de cambios |
| `git log --oneline --graph --all` | Historial dibujado |
| `git merge rama` | Une la rama indicada a la actual |
| `git tag v1.0.0` | Marca una versión importante |
| `git restore archivo` | Descarta cambios no guardados de un archivo |

Y lo imprescindible: un **`.gitignore`** que deje fuera `bin/`, `obj/`, `.vs/` y archivos con secretos (`appsettings.Local.json`). **Nunca subas contraseñas, tokens o cadenas de conexión.**

```text
$ git init -q -b main proyecto

$ printf 'bin/\nobj/\n.vs/\n*.user\nappsettings.Local.json\n' > .gitignore; cat .gitignore
bin/
obj/
.vs/
*.user
appsettings.Local.json

$ dotnet new console -n App -o . >/dev/null; git status --short
?? .gitignore
?? App.csproj
?? Program.cs

$ git add . && git commit -qm 'Proyecto inicial' && git log --oneline
a7aecde Proyecto inicial

$ git switch -c ampliar-saludo
Switched to a new branch 'ampliar-saludo'

$ echo 'Console.WriteLine("Adiós");' >> Program.cs; git diff --stat
 Program.cs | 1 +
 1 file changed, 1 insertion(+)

$ git commit -qam 'Añade despedida' && git log --oneline --graph --all
* 84ce63a Añade despedida
* a7aecde Proyecto inicial

$ git switch -q main; echo 'Console.WriteLine("Hola desde main");' >> Program.cs; git commit -qam 'Cambia main'; git merge ampliar-saludo 2>&1 | tail -4
Auto-merging Program.cs
CONFLICT (content): Merge conflict in Program.cs
Automatic merge failed; fix conflicts and then commit the result.

$ git status --short; git diff --name-only --diff-filter=U
UU Program.cs
Program.cs

$ cat Program.cs
﻿// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
<<<<<<< HEAD
Console.WriteLine("Hola desde main");
=======
Console.WriteLine("Adiós");
>>>>>>> ampliar-saludo

$ printf '// Saludo y despedida\nConsole.WriteLine("Hola desde main");\nConsole.WriteLine("Adiós");\n' > Program.cs; cat Program.cs
// Saludo y despedida
Console.WriteLine("Hola desde main");
Console.WriteLine("Adiós");

$ git add Program.cs && git commit -qm 'Resuelve el conflicto' && git log --oneline --graph --all
*   8d8a5af Resuelve el conflicto
|\  
| * 84ce63a Añade despedida
* | cda6d16 Cambia main
|/  
* a7aecde Proyecto inicial

$ git tag v1.0.0 && git tag
v1.0.0

$ git switch -c error-mio; git switch -q main; git branch -d error-mio
Switched to a new branch 'error-mio'
Deleted branch error-mio (was 8d8a5af).

$ git restore --source=HEAD~1 Program.cs && git diff --stat && git restore Program.cs   # deshacer cambios de un archivo
 Program.cs | 4 ++--
 1 file changed, 2 insertions(+), 2 deletions(-)
```

**Qué observar (es un conflicto real, no inventado):**

1. Dos ramas modificaron **la misma zona** de `Program.cs`; Git no puede decidir y marca `UU` y el archivo con `<<<<<<<`, `=======`, `>>>>>>>`.
2. **Resolver** es editar el archivo dejando el resultado correcto, **borrar los marcadores**, hacer `git add` y `git commit`.
3. `git log --graph` muestra la bifurcación y la unión: dos líneas que se juntan en el commit de resolución.
4. **Un conflicto no es un error:** es Git pidiéndote una decisión humana.

> **No se ejecutó aquí:** `git push`/`git pull` contra un servidor remoto en este ejemplo (usa la misma idea: `git remote add origin URL`, `git push -u origin main`). Y los **flujos de trabajo** (*Pull Requests*, revisión de código) son cuestión de equipo, no de comandos.

### Visual Studio, VS Code y Rider (no ejecutados aquí)

| Herramienta | Tipo | Fortaleza |
|---|---|---|
| **Visual Studio 2026** (v. 18) | IDE completo, Windows | Depurador, diseñadores, perfiladores, pruebas integradas; Community es gratuita para uso personal y equipos pequeños |
| **Visual Studio Code** + *C# Dev Kit* | Editor ligero, multiplataforma | Rápido y gratuito; suficiente para casi todo lo de este curso |
| **JetBrains Rider** | IDE multiplataforma | Refactorizaciones potentes; licencia de pago (con uso gratuito no comercial) |

Atajos de **Visual Studio** que conviene conocer (valores por defecto del perfil C#; **verifícalos en tu instalación**, porque se pueden cambiar):

| Atajo | Acción |
|---|---|
| `F5` / `Ctrl+F5` | Ejecutar con / sin depurador |
| `Ctrl+Shift+B` | Compilar la solución |
| `F9` | Poner o quitar un *breakpoint* |
| `F10` / `F11` / `Shift+F11` | Paso por encima / entrar / salir |
| `F12` | Ir a la definición |
| `Shift+F12` | Buscar todas las referencias |
| `Ctrl+.` | Acciones rápidas y arreglos |
| `Ctrl+R, Ctrl+R` | Renombrar con refactorización |
| `Ctrl+K, Ctrl+D` | Dar formato al documento |
| `Ctrl+K, Ctrl+C` / `Ctrl+K, Ctrl+U` | Comentar / descomentar selección |
| `Ctrl+T` | Ir a cualquier archivo, tipo o miembro |

### Errores típicos

| Síntoma | Causa | Solución |
|---|---|---|
| «The type or namespace name could not be found» | Falta la **referencia** o el `using` | `dotnet add reference` / `dotnet add package`, y revisa el `using` |
| Referencia circular | A usa a B y B usa a A | Extrae lo común a un tercer proyecto; revisa la arquitectura |
| `bin/` y `obj/` en el repositorio | Falta `.gitignore` | Añádelo **antes** del primer commit (si ya están: `git rm -r --cached bin obj`) |
| «Conflicto» y pánico | No leer los marcadores | Léelos: arriba lo tuyo, abajo lo de la otra rama |
| Contraseña subida a Git | Secreto en `appsettings.json` | Rótala **ya** (cámbiala): borrar el commit no basta, ya se copió |

> **Ejercicio 38.1:** crea una solución con una biblioteca y una consola, empaqueta la biblioteca como `1.0.0`, súbela a `1.1.0` y comprueba que el consumidor ve las dos.
>
> **Ejercicio 38.2:** provoca y resuelve un conflicto de Git en tu propio repositorio de prueba.

---

## Lección 39 · Depuración, trazas y registro

### Qué vas a aprender

- La diferencia entre **depurar** (parar y mirar), **trazar** (dejar rastro) y **registrar** (*logging* con niveles).
- `Debug` frente a `Trace`, `#if DEBUG`, símbolos de compilación y `[Conditional]`.
- Registro profesional con **`ILogger`**: niveles, plantillas y ámbitos.
- Cómo se usa el depurador de un IDE (procedimiento, **no ejecutado aquí**).

### La idea

| Técnica | Cuándo | Costo | Está en producción |
|---|---|---|---|
| **Depurador** (*breakpoints*) | Entender un fallo que puedes reproducir | Detiene el programa | No |
| **`Debug.*`** | Mensajes solo para el desarrollador | Cero en Release | **No** (desaparece al compilar en Release) |
| **`Trace.*`** | Rastro que quieres conservar en producción | Pequeño | Sí |
| **`ILogger`** | Registro de aplicación con niveles | Configurable | Sí |

**Niveles de `ILogger`** (de menos a más grave): `Trace`, `Debug`, `Information`, `Warning`, `Error`, `Critical`. Se configura un **nivel mínimo**: en desarrollo `Debug`, en producción `Information` o `Warning`.

**Regla de oro del registro:** usa **plantillas** (`"Pedido {Numero} de {Cliente}"`), **no** interpolación (`$"Pedido {n}"`): la plantilla guarda los datos como campos que se pueden **buscar y filtrar**, y evita construir el texto si el nivel está desactivado.

**Nunca registres contraseñas, tokens ni datos personales sensibles.**

### Código (`39-depuracion.cs`)

```csharp
#:property PublishAot=false
#:package Microsoft.Extensions.Logging.Console@10.0.0

#define TRAZA_EXTRA            // símbolo definido en el propio archivo (debe ir antes de cualquier código)

using System.Diagnostics;
using Microsoft.Extensions.Logging;

// ---- 1) Símbolos de compilación y directivas #if ----
// DEBUG existe solo en la configuración Debug; en Release este bloque ni siquiera se compila.
#if DEBUG
Console.WriteLine("Configuración: DEBUG (hay información para el depurador, sin optimizar)");
#else
Console.WriteLine("Configuración: RELEASE (optimizada)");
#endif
#if TRAZA_EXTRA
Console.WriteLine("Símbolo TRAZA_EXTRA definido con #define");
#endif

// ---- 2) Debug.* solo se ejecuta en Debug; Trace.* también en Release ----
Debug.WriteLine("Esto solo aparece en el depurador (Debug)");
Debug.Assert(1 + 1 == 2, "Esta aserción se cumple");
Console.WriteLine($"Debugger.IsAttached = {Debugger.IsAttached}  (true solo si lo ejecutas con F5 en el IDE)");

// ---- 3) Trace con "listeners": ¿a dónde van los mensajes? ----
string archivoTraza = Path.Combine(Path.GetTempPath(), "traza-demo.log");
File.Delete(archivoTraza);
Trace.Listeners.Clear();
Trace.Listeners.Add(new ConsoleTraceListener());                                         // a la consola
var enArchivo = new TextWriterTraceListener(archivoTraza);                               // y a un archivo
Trace.Listeners.Add(enArchivo);
Trace.AutoFlush = true;
Trace.TraceInformation("Inicio del proceso");
Trace.TraceWarning("Queda poco espacio en disco");
Trace.TraceError("No se pudo abrir la conexión");
Trace.WriteLineIf(Environment.ProcessorCount > 0, "Mensaje condicional (la condición era cierta)");
Console.WriteLine($"El archivo de traza tiene {File.ReadAllLines(archivoTraza).Length} líneas");

// ---- 4) TraceSwitch: subir o bajar el nivel de detalle sin recompilar ----
var nivel = new TraceSwitch("General", "Nivel de detalle") { Level = TraceLevel.Warning };
if (nivel.TraceInfo) Trace.WriteLine("(esto NO sale: el nivel es Warning)");
if (nivel.TraceWarning) Trace.WriteLine("(esto SÍ sale: Warning está activado)");

// ---- 5) Atributos para el depurador ----
var pedido = new Pedido(1042, "Ana", 3, 19.99m);
Console.WriteLine($"DebuggerDisplay mostraría en la ventana Locales: {pedido.GetType().GetCustomAttributes(typeof(DebuggerDisplayAttribute), false).Cast<DebuggerDisplayAttribute>().First().Value}");
Registrar("Mensaje de diagnóstico que solo se compila en Debug");   // [Conditional("DEBUG")]: la LLAMADA desaparece en Release

// ---- 6) Registro profesional: ILogger con niveles, plantillas y ámbitos ----
using ILoggerFactory fabrica = LoggerFactory.Create(b => b
    .SetMinimumLevel(LogLevel.Debug)
    .AddSimpleConsole(o => { o.SingleLine = true; o.IncludeScopes = true; o.TimestampFormat = null; }));
ILogger log = fabrica.CreateLogger("Tienda.Pedidos");

log.LogDebug("Detalle fino para depurar");
log.LogInformation("Pedido {Numero} de {Cliente} por {Total:C}", pedido.Numero, pedido.Cliente, pedido.Total);
using (log.BeginScope("PedidoId={Id}", pedido.Numero))                        // todo lo que se escriba aquí lleva ese dato
{
    log.LogWarning("Stock bajo para {Producto}", "Teclado");
    try { throw new InvalidOperationException("Pago rechazado"); }
    catch (Exception ex) { log.LogError(ex, "Falló el cobro"); }               // pasa la excepción entera, no solo el texto
}

// ---- 7) Medir con Stopwatch: el depurador no sirve para medir tiempos ----
var reloj = Stopwatch.StartNew();
Thread.Sleep(50);
Console.WriteLine($"Tiempo medido: {reloj.ElapsedMilliseconds >= 50}");
enArchivo.Dispose();

static void Registrar(string texto) => Prueba.Escribir(texto);

[DebuggerDisplay("Pedido {Numero} — {Cliente} — {Total} €")]   // lo que ve el programador al pasar el ratón sobre la variable
public record Pedido(int Numero, string Cliente, int Cantidad, decimal Precio)
{
    public decimal Total => Cantidad * Precio;
}

public static class Prueba
{
    [Conditional("DEBUG")]                                      // en Release el compilador elimina TODAS las llamadas
    public static void Escribir(string texto) => Console.WriteLine($"[solo-debug] {texto}");
}
```

### Cómo se lee, paso a paso

1. **`#define TRAZA_EXTRA`** define un símbolo de compilación **dentro del archivo**. `#if DEBUG … #else … #endif` incluye un bloque u otro **en tiempo de compilación** (no es un `if` normal: el bloque descartado ni se compila). `DEBUG` lo define automáticamente la configuración Debug.
2. **`Debug.WriteLine` / `Debug.Assert`** solo existen en Debug. **`Trace.*`** existen siempre.
3. **Los *listeners*** deciden **a dónde** van los mensajes de `Trace`: aquí consola y archivo a la vez.
4. **`TraceSwitch`** deja subir o bajar el detalle **sin recompilar** (normalmente leyendo la configuración).
5. **`[DebuggerDisplay("Pedido {Numero} — …")]`**: personaliza cómo se ve el objeto en las ventanas del depurador. Verás ese texto al pasar el ratón sobre la variable.
6. **`[Conditional("DEBUG")]`**: las **llamadas** al método **desaparecen** en Release (el compilador las elimina).
7. **`ILogger`** (paquete `Microsoft.Extensions.Logging`): `LogInformation` con plantilla; **`BeginScope`** añade datos a todo lo que se escriba dentro del bloque; `LogError(ex, …)` incluye la excepción completa con su pila.
8. **`Stopwatch`** mide tiempos; el depurador **no sirve** para medir rendimiento porque detiene y ralentiza el programa.

### Resultado real: en Debug

```text
$ dotnet run 39-depuracion.cs        (configuración Debug, la predeterminada)
Configuración: DEBUG (hay información para el depurador, sin optimizar)
Símbolo TRAZA_EXTRA definido con #define
Debugger.IsAttached = False  (true solo si lo ejecutas con F5 en el IDE)
39-depuracion Information: 0 : Inicio del proceso
39-depuracion Warning: 0 : Queda poco espacio en disco
39-depuracion Error: 0 : No se pudo abrir la conexión
Mensaje condicional (la condición era cierta)
El archivo de traza tiene 4 líneas
(esto SÍ sale: Warning está activado)
DebuggerDisplay mostraría en la ventana Locales: Pedido {Numero} — {Cliente} — {Total} €
[solo-debug] Mensaje de diagnóstico que solo se compila en Debug
dbug: Tienda.Pedidos[0] Detalle fino para depurar
info: Tienda.Pedidos[0] Pedido 1042 de Ana por ¤59.97
warn: Tienda.Pedidos[0] => PedidoId=1042 Stock bajo para Teclado
fail: Tienda.Pedidos[0] => PedidoId=1042 Falló el cobro System.InvalidOperationException: Pago rechazado    at Program.<Main>$(String[] args) in /tmp/claude-0/-home-user-claude/ddd43a70-8f87-5490-9d3f-755ecbba3013/scratchpad/curso5/39-depuracion.cs:line 60
Tiempo medido: True
```

### Resultado real: en Release

```text
$ dotnet run -c Release 39-depuracion.cs      (configuración Release)
Configuración: RELEASE (optimizada)
Símbolo TRAZA_EXTRA definido con #define
Debugger.IsAttached = False  (true solo si lo ejecutas con F5 en el IDE)
39-depuracion Information: 0 : Inicio del proceso
39-depuracion Warning: 0 : Queda poco espacio en disco
39-depuracion Error: 0 : No se pudo abrir la conexión
Mensaje condicional (la condición era cierta)
El archivo de traza tiene 4 líneas
(esto SÍ sale: Warning está activado)
DebuggerDisplay mostraría en la ventana Locales: Pedido {Numero} — {Cliente} — {Total} €
dbug: Tienda.Pedidos[0] Detalle fino para depurar
info: Tienda.Pedidos[0] Pedido 1042 de Ana por ¤59.97
warn: Tienda.Pedidos[0] => PedidoId=1042 Stock bajo para Teclado
fail: Tienda.Pedidos[0] => PedidoId=1042 Falló el cobro System.InvalidOperationException: Pago rechazado    at Program.<Main>$(String[] args) in /tmp
Tiempo medido: True
```

**La diferencia está en dos líneas:**

- **Debug:** «Configuración: DEBUG…» y la línea `[solo-debug]`.
- **Release:** «Configuración: RELEASE (optimizada)» y **la línea `[solo-debug]` ya no aparece**.

Todo lo demás (`Trace`, `ILogger`) se comporta igual. Dos anotaciones sobre la salida: `¤59.97` aparece porque el formato `:C` usa la cultura invariante del contenedor (que no tiene símbolo de moneda), y la pila de la excepción muestra la **línea exacta** del `throw`: por eso Debug conserva la información de depuración.

### El depurador de Visual Studio / VS Code (procedimiento, no ejecutado)

Este equipo no tiene IDE gráfico, así que **no se ejecutó**. Es el procedimiento estándar:

1. **Breakpoint:** clic en el margen izquierdo de la línea (o `F9`). El programa se detiene **antes** de ejecutarla.
2. **Ejecutar con depurador:** `F5`. Para avanzar: `F10` (siguiente línea sin entrar), `F11` (entrar al método), `Shift+F11` (salir).
3. **Mirar valores:** ventanas **Locales**, **Automático**, **Inspección** (*Watch*: escribes una expresión y la ve cambiar) y la **Ventana Inmediato** (ejecutas código sobre la marcha).
4. **Breakpoint condicional:** clic derecho en el punto → *Condiciones* → `pedido.Cantidad > 100`. Solo se detiene cuando se cumple. También *Recuento de pasos* (la n-ésima vez) y **Acciones** (*tracepoint*: escribe un mensaje **sin detenerse**).
5. **Modificar una variable en vivo:** en *Locales* o *Inspección*, doble clic en el valor y escribe otro; continúas con ese valor.
6. **Pila de llamadas** (*Call Stack*): quién llamó a quién hasta llegar aquí.
7. **Excepciones:** *Configuración de excepciones* para detenerse **al lanzarse**, no solo cuando nadie las captura.
8. **Editar y continuar / Hot Reload:** cambias código mientras la aplicación está parada o en marcha y se aplica sin reiniciar (con límites).
9. **Depurar procesos ya en marcha:** *Depurar → Asociar al proceso*.

En **VS Code** con *C# Dev Kit*: se genera un `launch.json`, y los atajos `F5/F9/F10/F11` son los mismos.

### Errores típicos

| Síntoma | Causa | Solución |
|---|---|---|
| «Funciona en Debug y falla en Release» | Optimizaciones o código solo-Debug (`Debug.Assert` con efectos secundarios) | No pongas lógica dentro de `Debug.Assert`; prueba en Release |
| El log no sale | Nivel mínimo más alto que el del mensaje | Ajusta `SetMinimumLevel` o la sección `Logging` |
| Logs con datos sensibles | Registrar objetos enteros | Registra solo lo necesario; enmascara |
| Rendimiento pésimo con logs | Interpolación en bucles | Plantillas y `if (log.IsEnabled(LogLevel.Debug))` |
| Medir con el depurador conectado | Distorsiona los tiempos | Mide en Release, sin depurador |

> **Ejercicio 39.1:** añade un `TextWriterTraceListener` que escriba a un archivo y comprueba su contenido tras ejecutar.
>
> **Ejercicio 39.2:** cambia el nivel mínimo a `Warning` y observa qué mensajes desaparecen.

---

## Lección 40 · Tipos por valor y referencia, estructuras de datos y rendimiento

### Qué vas a aprender

- Qué cambia entre **tipo por valor** (`struct`) y **por referencia** (`class`) en la práctica.
- **Cuándo usar** `List`, `Dictionary`, `HashSet`, `Stack`, `Queue`, `LinkedList`, `PriorityQueue`.
- Qué es el **boxing**, por qué las cadenas se concatenan con `StringBuilder` y por qué `TryParse` gana a `try/catch`.
- Medir en lugar de adivinar.

### La idea

| Pregunta | Valor (`struct`, `int`, `bool`, `DateOnly`…) | Referencia (`class`, `string`, `record`, arreglos…) |
|---|---|---|
| Qué guarda la variable | **El dato** | **Una dirección** al objeto |
| Al asignar `b = a` | Se **copia** el contenido | Se copia la **dirección** (los dos apuntan al mismo objeto) |
| Dónde vive | Pila o dentro de otro objeto | **Montón** (*heap*), lo libera el recolector |
| Puede ser `null` | No (salvo `int?`) | Sí |

**LIFO / FIFO:** una **pila** (*Stack*) saca **lo último que entró** (deshacer). Una **cola** (*Queue*) saca **lo primero que entró** (turnos).

| Estructura | Úsala cuando… | Buscar por valor | Insertar |
|---|---|---|---|
| `T[]` (arreglo) | Tamaño fijo, máxima velocidad | O(n) | no se puede |
| `List<T>` | Secuencia ordenada que crece; el 90 % de los casos | O(n) | al final O(1) |
| `Dictionary<K,V>` | Buscar por **clave** | **O(1)** medio | O(1) medio |
| `HashSet<T>` | «¿Está o no está?», sin duplicados | **O(1)** medio | O(1) medio |
| `SortedDictionary` | Recorrer **ordenado** por clave | O(log n) | O(log n) |
| `Stack<T>` / `Queue<T>` | LIFO / FIFO | — | O(1) |
| `LinkedList<T>` | Insertar/quitar en medio **con el nodo en la mano** | O(n) | O(1) |
| `PriorityQueue<T,P>` | Sacar siempre el más urgente | — | O(log n) |

*(O(1) = el tiempo no depende del tamaño; O(n) = crece con él; O(log n) = crece muy despacio.)*

**Boxing** es convertir un tipo por valor en `object`: se **reserva una caja en el montón** y se copia el valor. Las colecciones no genéricas antiguas (`ArrayList`) guardan `object`, así que cada `int` se encaja. Las genéricas (`List<int>`) **no**.

**Un `string` es inmutable.** `s += "x"` crea una cadena nueva cada vez: 20 000 concatenaciones son 20 000 copias crecientes.

### Código (`40-estructuras.cs`)

```csharp
#:property PublishAot=false

using System.Collections;
using System.Diagnostics;
using System.Text;

// ================= 1) Tipos por valor frente a tipos por referencia =================
Console.WriteLine("=== Valor vs referencia ===");
Punto a = new(1, 2); Punto b = a; b.X = 99;                       // struct: b es una COPIA de a
Console.WriteLine($"struct: a.X={a.X}, b.X={b.X}   → cada variable tiene su propia copia");
Persona p1 = new() { Nombre = "Ana" }; Persona p2 = p1; p2.Nombre = "Luis";   // class: p2 apunta al MISMO objeto
Console.WriteLine($"class : p1={p1.Nombre}, p2={p2.Nombre}   → las dos variables apuntan al mismo objeto");
Cambiar(a, p1);
Console.WriteLine($"tras Cambiar(): a.X={a.X} (el struct no cambió), p1.Nombre={p1.Nombre} (la clase sí)");
static void Cambiar(Punto punto, Persona persona) { punto.X = -1; persona.Nombre = "Cambiado"; }

// ================= 2) Las colecciones y para qué sirve cada una =================
Console.WriteLine("\n=== Colecciones ===");
var pila = new Stack<string>();                                   // LIFO: último en entrar, primero en salir
foreach (var paso in new[] { "abrir", "editar", "guardar" }) pila.Push(paso);
Console.Write("Stack (deshacer): "); while (pila.Count > 0) Console.Write(pila.Pop() + " "); Console.WriteLine();

var cola = new Queue<string>();                                   // FIFO: primero en entrar, primero en salir
foreach (var cliente in new[] { "Ana", "Luis", "Eva" }) cola.Enqueue(cliente);
Console.Write("Queue (turnos)  : "); while (cola.Count > 0) Console.Write(cola.Dequeue() + " "); Console.WriteLine();

var edades = new Dictionary<string, int> { ["Ana"] = 31, ["Luis"] = 27 };   // clave → valor, búsqueda ~O(1)
edades["Eva"] = 45;
Console.WriteLine($"Dictionary: Ana={edades["Ana"]}, ¿existe Pedro? {edades.TryGetValue("Pedro", out _)}");

var etiquetas = new HashSet<string> { "c#", "net", "c#", "web" };          // sin duplicados, pertenencia ~O(1)
Console.WriteLine($"HashSet: {etiquetas.Count} elementos (el duplicado se ignoró); contiene 'net': {etiquetas.Contains("net")}");

var ordenado = new SortedDictionary<string, int> { ["pera"] = 3, ["manzana"] = 1, ["kiwi"] = 2 };
Console.WriteLine("SortedDictionary recorre ordenado por clave: " + string.Join(", ", ordenado.Keys));

var lista = new LinkedList<int>([1, 2, 3]); lista.AddFirst(0);
Console.WriteLine("LinkedList (insertar al inicio sin desplazar): " + string.Join(",", lista));

var prioridad = new PriorityQueue<string, int>();                 // saca primero el de MENOR prioridad numérica
prioridad.Enqueue("baja", 5); prioridad.Enqueue("urgente", 1); prioridad.Enqueue("media", 3);
Console.Write("PriorityQueue: "); while (prioridad.TryDequeue(out var t, out _)) Console.Write(t + " "); Console.WriteLine();

// ================= 3) Elegir bien importa: mediciones =================
Console.WriteLine("\n=== Mediciones (Release; los tiempos cambian en cada máquina, lo que importa es la proporción) ===");
const int N = 20_000;
var lista2 = Enumerable.Range(0, N).ToList();
var conjunto = lista2.ToHashSet();
int[] busquedas = Enumerable.Range(0, 2000).Select(i => N - 1 - i).ToArray();   // elementos del final de la lista

Medir("Buscar 2000 valores en List<int> (Contains) ", () => { int c = 0; foreach (int x in busquedas) if (lista2.Contains(x)) c++; return c; });
Medir("Buscar 2000 valores en HashSet<int>         ", () => { int c = 0; foreach (int x in busquedas) if (conjunto.Contains(x)) c++; return c; });

// ---- boxing: convertir un tipo por valor en object copia el valor al montón (heap) ----
var arrayList = new ArrayList();                                                  // guarda 'object'
Medir("Sumar 1.000.000 int en ArrayList (boxing)   ", () => { arrayList.Clear(); for (int i = 0; i < 1_000_000; i++) arrayList.Add(i); long s = 0; foreach (int x in arrayList) s += x; return s; });
var generica = new List<int>();
Medir("Sumar 1.000.000 int en List<int> (sin box)  ", () => { generica.Clear(); for (int i = 0; i < 1_000_000; i++) generica.Add(i); long s = 0; foreach (int x in generica) s += x; return s; });

// Ojo: .NET 10 es capaz de ELIMINAR un boxing cuando el objeto no "escapa" del método (el JIT lo detecta).
// Para ver el coste real hay que hacer que la caja se guarde en algún sitio, como aquí en un arreglo.
object[] guardadas = new object[1000];
long antes = GC.GetAllocatedBytesForCurrentThread();
for (int i = 0; i < guardadas.Length; i++) guardadas[i] = i;                      // boxing: cada int se copia a una caja en el heap
long bytes = GC.GetAllocatedBytesForCurrentThread() - antes;
Console.WriteLine($"1000 boxings guardados reservaron {bytes:N0} bytes en el heap (~{bytes / 1000} bytes por int de 4 bytes)");

// ---- strings: son inmutables, concatenar en bucle crea un string nuevo cada vez ----
Medir("Concatenar 20.000 veces con +=              ", () => { string s = ""; for (int i = 0; i < 20_000; i++) s += "x"; return s.Length; });
Medir("Concatenar 20.000 veces con StringBuilder   ", () => { var sb = new StringBuilder(); for (int i = 0; i < 20_000; i++) sb.Append('x'); return sb.Length; });

// ---- excepciones: sirven para lo EXCEPCIONAL, no para el flujo normal ----
string[] textos = Enumerable.Range(0, 5_000).Select(i => i % 2 == 0 ? "123" : "abc").ToArray();
Medir("Validar 5000 textos con try/catch           ", () => { int ok = 0; foreach (var t in textos) { try { ok += int.Parse(t) > 0 ? 1 : 0; } catch (FormatException) { } } return ok; });
Medir("Validar 5000 textos con int.TryParse        ", () => { int ok = 0; foreach (var t in textos) if (int.TryParse(t, out int v) && v > 0) ok++; return ok; });

// ---- bucles: for sobre array/List frente a foreach sobre IEnumerable y LINQ ----
int[] datos = Enumerable.Range(1, 5_000_000).ToArray();
Medir("Suma con for sobre array                    ", () => { long s = 0; for (int i = 0; i < datos.Length; i++) s += datos[i]; return s; });
Medir("Suma con foreach sobre array                ", () => { long s = 0; foreach (int x in datos) s += x; return s; });
Medir("Suma con LINQ (Sum de long)                 ", () => datos.Sum(x => (long)x));
Medir("Suma con Span<int> (sin copiar)             ", () => { long s = 0; foreach (int x in datos.AsSpan()) s += x; return s; });

// Medir: calienta una vez (para que el JIT compile) y toma la mejor de 5 pasadas.
static void Medir<T>(string nombre, Func<T> accion)
{
    accion();                                   // calentamiento: el JIT compila y las cachés se llenan
    double mejor = double.MaxValue;
    for (int i = 0; i < 5; i++)
    {
        long t0 = Stopwatch.GetTimestamp();
        accion();
        mejor = Math.Min(mejor, Stopwatch.GetElapsedTime(t0).TotalMilliseconds);
    }
    Console.WriteLine($"{nombre} {mejor,9:F3} ms");
}

public struct Punto(int x, int y) { public int X { get; set; } = x; public int Y { get; set; } = y; }
public class Persona { public string Nombre { get; set; } = ""; }
```

### Cómo se lee, paso a paso

1. **`Punto b = a; b.X = 99;`**: `Punto` es `struct`, `b` es una **copia**; `a.X` sigue en 1. Con la `class Persona`, `p2 = p1` apunta al mismo objeto: cambiar `p2` cambia `p1`.
2. **Pasar a una función:** un `struct` se copia (la función modifica su copia); una `class` se pasa la dirección (la función modifica el original).
3. **Una colección por frase:** `Stack` (deshacer), `Queue` (turnos), `Dictionary` (buscar por clave), `HashSet` (sin duplicados), `PriorityQueue` (lo urgente primero).
4. **`Medir`**: ejecuta una vez para **calentar** (el JIT compila), y toma la **mejor de 5** pasadas. Todo en **Release**.
5. **Boxing medido:** 1000 `int` guardados como `object` reservaron **24 000 bytes (24 por entero)**, frente a los 4 de un `int` suelto: 16 bytes de cabecera del objeto + 4 de dato + relleno.
6. **Sobre `Span<int>`:** una vista sobre memoria existente, sin copiar nada (más en la lección 44).

### Resultado real

```text
=== Valor vs referencia ===
struct: a.X=1, b.X=99   → cada variable tiene su propia copia
class : p1=Luis, p2=Luis   → las dos variables apuntan al mismo objeto
tras Cambiar(): a.X=1 (el struct no cambió), p1.Nombre=Cambiado (la clase sí)

=== Colecciones ===
Stack (deshacer): guardar editar abrir 
Queue (turnos)  : Ana Luis Eva 
Dictionary: Ana=31, ¿existe Pedro? False
HashSet: 3 elementos (el duplicado se ignoró); contiene 'net': True
SortedDictionary recorre ordenado por clave: kiwi, manzana, pera
LinkedList (insertar al inicio sin desplazar): 0,1,2,3
PriorityQueue: urgente media baja 

=== Mediciones (Release; los tiempos cambian en cada máquina, lo que importa es la proporción) ===
Buscar 2000 valores en List<int> (Contains)      7.131 ms
Buscar 2000 valores en HashSet<int>              0.076 ms
Sumar 1.000.000 int en ArrayList (boxing)       33.208 ms
Sumar 1.000.000 int en List<int> (sin box)       1.674 ms
1000 boxings guardados reservaron 24,000 bytes en el heap (~24 bytes por int de 4 bytes)
Concatenar 20.000 veces con +=                  51.256 ms
Concatenar 20.000 veces con StringBuilder        0.033 ms
Validar 5000 textos con try/catch                5.811 ms
Validar 5000 textos con int.TryParse             0.070 ms
Suma con for sobre array                         6.495 ms
Suma con foreach sobre array                     2.837 ms
Suma con LINQ (Sum de long)                     11.069 ms
Suma con Span<int> (sin copiar)                  2.741 ms
```

**Qué dicen los números** (en esta máquina; lo que importa son las proporciones):

| Comparación | Resultado |
|---|---|
| Buscar en `HashSet` frente a `List` | **~70–90 veces más rápido** (con 20 000 elementos) |
| `List<int>` frente a `ArrayList` (boxing) | **~20 veces más rápido** |
| `StringBuilder` frente a `+=` | **más de 1000 veces más rápido** (20 000 anexos) |
| `TryParse` frente a `try/catch` | **~80 veces más rápido** con entradas inválidas |
| `for`, `foreach`, `Span` | **Equivalentes** dentro del ruido |
| LINQ `Sum` | Más lento por su abstracción (aceptable casi siempre) |

> **Dos advertencias sobre estas mediciones:**
>
> 1. **Un microbenchmark casero no es una prueba científica.** Para medir en serio se usa **BenchmarkDotNet**, que controla calentamiento, estadística y ruido. Aquí solo sirven para ver **órdenes de magnitud**.
> 2. **Una primera versión mía fue engañosa y la descarté:** el boxing «no reservaba nada» porque el JIT de .NET 10 **elimina las cajas que no escapan** del método, y una comparación entre llamada directa y por delegado salió al revés por ruido. Medir bien es difícil; **desconfía de un resultado que contradice la teoría** antes de publicarlo.

### Errores típicos

| Síntoma | Causa | Solución |
|---|---|---|
| Cambias una copia y el original no cambia (o al revés) | Confundir `struct` con `class` | Piensa «¿copia o misma cosa?»; los `struct` grandes y mutables son mala idea |
| `List.Contains` en un bucle grande | Búsqueda lineal repetida | Usa `HashSet` o `Dictionary` |
| `+=` con `string` en un bucle | Copias repetidas | `StringBuilder` o `string.Join` |
| `try/catch` para validar entradas | Excepciones lentas y ruidosas | `TryParse` |
| Optimizar «a ojo» | Sin medir | Mide primero; optimiza el cuello de botella real |
| Modificar una colección mientras la recorres | `InvalidOperationException` | Recorre una copia (`ToList()`) o usa `RemoveAll` |

> **Ejercicio 40.1:** mide `Dictionary` frente a `List` para buscar por clave con 100 000 elementos.
>
> **Ejercicio 40.2:** mide cuántos bytes ocupa una lista de 1 000 000 `int` frente a un `ArrayList`.

---

## Resumen de la Parte IV

| Si quieres… | Mira… |
|---|---|
| Saber qué versión usar y cuándo caduca | Tabla de soporte (lección 37) |
| Fijar el SDK de un equipo | `global.json` |
| Arranque rapidísimo / sin runtime | Native AOT |
| Compartir tu código como paquete | `dotnet pack` + NuGet |
| Historial y trabajo en equipo | Git: commit, rama, merge |
| Diagnosticar | Depurador, `Trace`, `ILogger` |
| Elegir bien una colección | Tabla de estructuras (lección 40) |

**Sigue con la Parte V:** las novedades de C# 8 a C# 14, reflexión y generadores de código, concurrencia y asincronía a fondo.
