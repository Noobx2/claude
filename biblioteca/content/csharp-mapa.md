# C# de una pieza: análisis, bucles y mapa

Este módulo hace tres cosas con el material de la biblioteca sobre C#:

1. **Analiza una afirmación central** en cuatro niveles (hecho → patrones → estructuras e incentivos → modelos mentales).
2. **Identifica los bucles de retroalimentación** (de refuerzo y de equilibrio) que aparecen en las fuentes.
3. **Responde a tus preguntas de síntesis sobre C#**: cómo encajan las piezas, F#, Visual Basic, la API de Windows, WinForms/WPF/MAUI, una interfaz web con Vite y Unity.

## 0. Cómo leer este módulo (importante)

**Qué cuenta como «fuentes».** Los módulos de esta biblioteca: [C# (.NET)](#csharp), [C# sin frameworks](#csharp-datos), [Java sin frameworks](#java-datos), [Spring Boot](#spring), [Node.js](#node), [Diccionario](#glosario-moderno), [Hardware](#hardware), [Ruta](#ruta), [Inventario](#inventario), [Comparativa de lenguajes](#lenguajes-comparativa) y tu propio mensaje sobre aprender escribiendo. **No son estudios independientes: son material de referencia escrito para este proyecto** (en buena parte por mí). Por eso casi nunca *explican causas*: definen cosas. Las causas y los bucles salen, sobre todo, de **conectar** definiciones.

**Lo que las fuentes NO cubren.** Buscando en toda la biblioteca: **F#, Visual Basic, P/Invoke, Win32 / «API de Windows» y Avalonia no aparecen en ningún módulo**; **MAUI**, **Blazor** y **Unity** aparecen solo como nombres en listas. Todo lo que digo de esos temas **no viene de las fuentes**.

Cada afirmación lleva una etiqueta:

| Etiqueta | Significado |
|---|---|
| <span class="tag ok">FUENTE</span> | Lo dice **directamente** un módulo de la biblioteca (se indica cuál) |
| <span class="tag warn">INFERENCIA</span> | Lo deduzco **al conectar** varias fuentes; la fuente no lo dice así |
| <span class="tag ok">EXPERIMENTO</span> | Lo comprobé **ejecutando código** para este módulo (el código está en el repositorio) |
| <span class="tag mute">GENERAL</span> | Conocimiento general mío, **fuera de las fuentes y sin comprobar aquí**: verifícalo en la documentación oficial |

> **Una discrepancia que encontré en las fuentes.** [Comparativa de lenguajes](#lenguajes-comparativa) dice que los ejemplos de C# «no se han podido ejecutar desde aquí», mientras que [C# (.NET)](#csharp) dice que **todos** se compilaron y ejecutaron con .NET 10. Ambos módulos llevan la misma fecha (2026-10-27). Lo más probable es que la comparativa se escribiera antes de tener `dotnet`, pero **no puedo afirmarlo**; trátala con más cautela.

---

# Parte 1 · Una afirmación central en cuatro niveles

## 1.1 La afirmación elegida

> «**Un proyecto por responsabilidad**: `Core` lo comparten consola, ventana y pruebas. Si mañana quieres una web o una app móvil, reutilizas `Core`.»
> — [C# sin frameworks](#csharp-datos), sección 1

La elegí porque es la que une todas tus preguntas: WinForms, WPF, MAUI, una interfaz distinta con C# de backend, F#/VB sobre la misma plataforma e incluso Unity son **la misma pregunta** («¿qué se queda y qué cambia cuando cambia la interfaz?»).

## 1.2 Nivel 1 · El hecho visible

*¿Qué se afirma y qué se ve?*

| | Contenido | Etiqueta |
|---|---|---|
| 1 | La solución tiene `Inventory.Core` (lógica) y tres clientes que lo referencian: `Inventory.Console`, `Inventory.WinForms` y `Inventory.Tests` | <span class="tag ok">FUENTE</span> [csharp-datos](#csharp-datos) §1–2 |
| 2 | El Core «no sabe nada de pantallas» y `ConsoleInput` recibe un `TextReader`/`TextWriter` para poder probarse sin teclado | <span class="tag ok">FUENTE</span> [csharp-datos](#csharp-datos) §1, §4 |
| 3 | El servicio de la API de tareas «no conoce HTTP» y «depende de la interfaz, no de la implementación» | <span class="tag ok">FUENTE</span> [csharp](#csharp) §10 |
| 4 | Las 13 pruebas pasan y WinForms **compila** en Linux pero **no se ejecuta** | <span class="tag ok">FUENTE</span> [csharp-datos](#csharp-datos) |
| 5 | **Reutilicé el mismo Core, sin tocar una línea (`diff` idéntico)**, desde: una API web con interfaz Vite, un cliente F#, un cliente Visual Basic y una ventana WPF (esta última solo compila en Linux). Las 13 pruebas originales siguen pasando | <span class="tag ok">EXPERIMENTO</span> |
| 6 | **Matiz que descubrió el experimento:** el Core **no es seguro para hilos** (usa `List<T>` sin cerrojo, porque se pensó para una ventana de un solo hilo). Una API atiende peticiones a la vez, así que hubo que envolverlo en un `lock` fuera del Core | <span class="tag ok">EXPERIMENTO</span> |

**Lectura:** la afirmación **se sostiene**, pero con una **condición que la fuente no menciona**: reutilizar el Core es gratis *si el nuevo cliente respeta los supuestos con que se escribió* (aquí, un solo hilo). <span class="tag warn">INFERENCIA</span> Un Core verdaderamente reutilizable debe **declarar sus supuestos** (hilos, cultura, codificación, ubicación de archivos).

## 1.3 Nivel 2 · Patrones que se repiten en el tiempo

*¿Dónde más aparece la misma idea y cómo ha ido cambiando de nombre?*

| Patrón | Dónde aparece | Etiqueta |
|---|---|---|
| **Misma lógica, varias envolturas.** «Esta API es intercambiable con las de Node, Python, Go, Rust y Spring Boot: mismas rutas y mismo JSON» | [csharp](#csharp) §10 | <span class="tag ok">FUENTE</span> |
| «La interfaz de React o Vue funciona con cualquiera de las dos sin cambios» | [spring](#spring), §«intercambiable» | <span class="tag ok">FUENTE</span> |
| El mismo inventario en **Java y C#**, con la misma separación (dato, repositorio, informe / menú, ventana) | [java-datos](#java-datos), [csharp-datos](#csharp-datos) | <span class="tag ok">FUENTE</span> |
| **Una idea, muchos nombres:** MVC, MVP, MVVM, arquitectura en capas, hexagonal («el núcleo no conoce bases de datos ni pantallas»), Clean/Onion («las dependencias apuntan hacia el centro»); y SOA → microservicios («antecesora de los microservicios», el ESB «hoy se considera pesado») | [Diccionario](#glosario-moderno), áreas Arquitectura y Patrones | <span class="tag ok">FUENTE</span> |
| **Lo que se repite es la separación «estable / cambiante»**: reglas del negocio por un lado, interfaz, datos y plataforma por otro | Conexión de las filas anteriores | <span class="tag warn">INFERENCIA</span> |
| Cronología aproximada de esos nombres: MVC (años 70, Smalltalk), MVVM (2005, pensado para WPF), arquitectura hexagonal (2005), Onion (2008), Clean (2012) | Fuera de las fuentes | <span class="tag mute">GENERAL</span> |
| **En .NET, cada ola de interfaz sustituyó a la anterior** (WinForms 2002 → WPF 2006 → UWP → WinUI 3 → MAUI) y las aplicaciones con la lógica metida en la ventana tuvieron que reescribirse, mientras que las que tenían un Core sobrevivieron | Fuera de las fuentes; es mi generalización | <span class="tag mute">GENERAL</span> + <span class="tag warn">INFERENCIA</span> |

**Lectura:** la fuente solo dice «reutilizas Core»; **el patrón histórico detrás** (que la interfaz es lo que más rápido caduca) es mi inferencia apoyada en nombres que sí están en el diccionario.

## 1.4 Nivel 3 · Estructuras, reglas e incentivos que producen esos patrones

*¿Qué obliga o empuja a que se repitan?*

**Reglas y estructuras (lo que el sistema impone):**

| Estructura | Cómo produce el patrón | Etiqueta |
|---|---|---|
| `ProjectReference` solo **hacia** Core; el Core no referencia ninguna interfaz | El compilador **impide** que el Core use una ventana o `HttpContext` | <span class="tag ok">FUENTE</span> [csharp-datos](#csharp-datos) §2, `.csproj` |
| `net10.0-windows`: «destino que solo existe en Windows»; `EnableWindowsTargeting` «permite compilar, no ejecutar» | Una frontera **dura** que empuja a dejar lo portable en un proyecto `net10.0` | <span class="tag ok">FUENTE</span> (el hecho) + <span class="tag warn">INFERENCIA</span> (el efecto) |
| Inyección de dependencias: «en vez de que una clase cree lo que necesita, se lo entregan desde fuera. Facilita cambiar piezas y probar» | Hace que el servicio dependa de contratos | <span class="tag ok">FUENTE</span> [Diccionario](#glosario-moderno) |
| Entorno: tu equipo es una VM sin USB y sin `dotnet`; el hardware real está en el anfitrión | **Dónde corre el código** lo decide la máquina que ve el dispositivo, no el gusto | <span class="tag ok">FUENTE</span> [hardware](#hardware), [ruta](#ruta), [inventario](#inventario) |

**Incentivos (lo que empuja las decisiones):**

| Incentivo | Etiqueta |
|---|---|
| «El mantenimiento suele costar más que construir»; la deuda técnica «se paga con intereses» → ordenar hoy compensa mañana | <span class="tag ok">FUENTE</span> [Diccionario](#glosario-moderno) |
| *Vendor lock-in*: «dependencia de un proveedor tan fuerte que cambiar de él cuesta muchísimo» | <span class="tag ok">FUENTE</span> (la definición) |
| **Probar sin interfaz es un incentivo inmediato** a separar: no se puede probar un `MessageBox` automáticamente | <span class="tag warn">INFERENCIA</span> |
| **El coste de cambiar es asimétrico:** la interfaz cambia por moda y plataforma; las reglas, por el negocio. Separarlas aísla el cambio frecuente del cambio caro | <span class="tag warn">INFERENCIA</span> |
| Los marcos de interfaz de Microsoft para escritorio (WinForms, WPF, WinUI) solo funcionan en Windows, lo que **ata** al ecosistema a quien mete la lógica dentro | <span class="tag mute">GENERAL</span> |
| Contrapeso: «**YAGNI**» y «**over-engineering**» (hacer algo más complejo de lo necesario) → separar de más también cuesta | <span class="tag ok">FUENTE</span> [Diccionario](#glosario-moderno) |

## 1.5 Nivel 4 · Modelos mentales que lo sostienen

*¿Qué creencias hacen que todo lo anterior parezca «lo normal»?*

| Modelo mental | Cómo se manifiesta | Etiqueta |
|---|---|---|
| **«Comprobado ≠ supuesto»** | Cada módulo declara qué se ejecutó y qué no; el inventario distingue «instalado» de «funcionando» | <span class="tag ok">FUENTE</span> [inicio](#inicio) («Comprobado ≠ supuesto»), [inventario](#inventario) |
| **«Entender lo que automatizan las herramientas»** | «Web antes que frameworks: para entender lo que React y Vue automatizan» | <span class="tag ok">FUENTE</span> [ruta](#ruta) |
| **«Aprender escribiendo»** | Tu mensaje: «la única forma de recordarlo es seguir escribiendo; si me pongo solo a leer se me volverá a olvidar» | <span class="tag ok">FUENTE</span> (tú) |
| **«La interfaz es un periférico, no el programa»** | Un teclado, una ventana y una web son maneras de entrar a la misma lógica | <span class="tag warn">INFERENCIA</span> |
| **«El contrato importa más que la implementación»** | Interfaces, DTO, JSON y rutas idénticas entre backends | <span class="tag warn">INFERENCIA</span> (apoyada en [csharp](#csharp) §10 y [spring](#spring)) |
| **«El coste se paga en el futuro»** | Deuda técnica, mantenimiento, *bit rot* | <span class="tag warn">INFERENCIA</span> (apoyada en el Diccionario) |
| **El modelo contrario (muy común al venir de Visual Studio o de Unity):** «el formulario o el script *es* el programa». Si lo tienes, tenderás a poner la lógica dentro del botón o del `MonoBehaviour` | <span class="tag warn">INFERENCIA</span> sobre ti: es una hipótesis, no un dato |

**Cuándo el modelo se rompe:** para un script de una tarde o un prototipo, separar en cuatro proyectos es **sobrediseño**. La regla útil: separa cuando vayas a **probar**, a **reutilizar** o a **cambiar de interfaz**; no antes.

## 1.6 Resumen de los cuatro niveles

| Nivel | En una línea | Predominio |
|---|---|---|
| 1 · Hecho | Core reutilizable por varios clientes; probado hoy con 4 más | Fuente + experimento |
| 2 · Patrones | La idea reaparece con nombres distintos (capas, MVC, hexagonal, Clean…) | Fuente + inferencia |
| 3 · Estructuras e incentivos | El compilador y el coste de cambiar empujan a separar | **Mucha inferencia** |
| 4 · Modelos mentales | «La interfaz es un periférico»; «contrato antes que implementación» | **Casi todo inferencia** |

> **Regla general:** cuanto más abajo en la pirámide, menos lo dice la fuente y más lo deduzco yo. Los niveles 3 y 4 son una lectura razonable, **no un hecho documentado**.

---

# Parte 2 · Bucles de retroalimentación

Un **bucle de refuerzo** hace que un cambio **se amplifique** (espiral: sube o baja cada vez más rápido). Un **bucle de equilibrio** hace que un cambio **active una respuesta que lo compensa** (estabiliza). Casi ninguna fuente nombra «bucles»: **definen piezas**; yo las cierro en círculo, y eso es inferencia.

## 2.1 Bucles de refuerzo (espirales)

### R1 · La espiral de la deuda técnica (vicioso)

| Paso | Qué ocurre |
|---|---|
| 1 | Hay prisa y se toma un **atajo** (por ejemplo, la lógica dentro del botón de la ventana) |
| 2 | El código queda enredado: «tirar de un hilo mueve todos» |
| 3 | **Cambiar cualquier cosa cuesta más** y se rompe más |
| 4 | Hay menos tiempo para ordenar → más prisa → **más atajos** → vuelta al paso 1 |

- <span class="tag ok">FUENTE</span> [Diccionario](#glosario-moderno): deuda técnica («atajos tomados hoy… se pagan con intereses mañana (más lentitud, más fallos)»), código espagueti, mantenimiento («suele costar más que construir»), *bit rot*.
- <span class="tag warn">INFERENCIA</span> El **cierre del círculo** (más lentitud → más prisa → más atajos) no está escrito en ninguna fuente.

### R2 · La espiral de las pruebas (virtuoso)

| Paso | Qué ocurre |
|---|---|
| 1 | El Core está **separado** de la interfaz |
| 2 | Se puede **probar sin pantalla** (`StringReader`/`StringWriter`; 13 pruebas en verde) |
| 3 | Con pruebas, **refactorizar da menos miedo** |
| 4 | Se mejora el diseño → el Core queda **aún más separable** → vuelta al paso 1 |

- <span class="tag ok">FUENTE</span> [csharp-datos](#csharp-datos) (probar sin teclear), [Diccionario](#glosario-moderno) (refactorización «sin cambiar lo que hace, para que sea más fácil de entender y modificar», TDD, mocks).
- <span class="tag warn">INFERENCIA</span> La cadena causal completa.
- **Relación con R1:** son la misma espiral con signo contrario. <span class="tag warn">INFERENCIA</span> **Separar el Core es el punto de palanca que convierte R1 en R2.**

### R3 · La espiral de aprender escribiendo (virtuoso)

| Paso | Qué ocurre |
|---|---|
| 1 | **Ejecutas** código y anotas errores y mensajes exactos |
| 2 | Con ese material **escribes** el capítulo con tus palabras |
| 3 | Al **releer y rehacer** el ejercicio, lo recuerdas |
| 4 | Recordar te hace capaz de **ejecutar cosas más difíciles** → vuelta al paso 1 |

- <span class="tag ok">FUENTE</span> Tu mensaje («la única forma de recordarlo es seguir escribiendo»), la plantilla de libro del repositorio (`libro/Plantilla_Libro_Universal.docx`: «lo fija escribirlo… y ejecutarlo», «empieza cada sesión releyendo… eso es repaso espaciado gratis»).
- <span class="tag warn">INFERENCIA</span> Que sea un bucle y no una lista de pasos: lo conecto yo.

### R4 · La espiral de dependencia de la IA (vicioso)

| Paso | Qué ocurre |
|---|---|
| 1 | Pides código a la IA y lo **aceptas sin entenderlo** |
| 2 | **Entiendes menos** tu propio proyecto |
| 3 | Para cada cambio nuevo **necesitas pedir otra vez** |
| 4 | Más dependencia → aún menos comprensión → vuelta al paso 1 |

- <span class="tag ok">FUENTE</span> [Diccionario](#glosario-moderno): *vibe coding* («rápido, pero arriesgado si no entiendes lo que sale»), «confía pero verifica»; plantilla de libro: «no pegues código de la IA que no sepas explicar».
- <span class="tag warn">INFERENCIA</span> El bucle completo. **Te afecta directamente** porque tu objetivo declarado es **recuperar autonomía**.
- **Contrapeso (R3):** escribir y ejecutar tú mismo. Qué espiral gana depende de **quién teclea primero**.

## 2.2 Bucles de equilibrio (estabilizadores)

### B1 · El pipeline que frena lo roto

| Paso | Qué ocurre |
|---|---|
| 1 | Alguien **cambia** el código |
| 2 | El pipeline lo **compila y lo prueba** automáticamente |
| 3 | Si algo falla, **el pipeline se detiene** y avisa |
| 4 | Se corrige hasta volver a verde; entonces se continúa |

- <span class="tag ok">FUENTE</span> [Diccionario](#glosario-moderno): CI («cada cambio se integra y se prueba automáticamente»), pipeline («si un paso falla, se detiene»), *quality gate*.
- <span class="tag warn">INFERENCIA</span> Presentarlo como bucle con un «objetivo» (verde).

### B2 · El presupuesto de errores

| Paso | Qué ocurre |
|---|---|
| 1 | Se mide la fiabilidad (SLO) |
| 2 | Mientras **sobra** presupuesto de fallo, el equipo **innova** |
| 3 | Al **agotarse**, se **frena** lo nuevo y se estabiliza |
| 4 | Con el servicio estable, vuelve a sobrar presupuesto |

- <span class="tag ok">FUENTE</span> [Diccionario](#glosario-moderno): «mientras sobra, se innova; si se agota, se estabiliza»; frase de reunión «hemos agotado el error budget». **Es el caso más explícito de bucle de equilibrio en las fuentes.**

### B3 · La validación con reintento

| Paso | Qué ocurre |
|---|---|
| 1 | El usuario escribe un dato |
| 2 | El Core lo **rechaza** si no cumple las reglas (`ArgumentException`) |
| 3 | La interfaz **muestra el mensaje** y vuelve a preguntar |
| 4 | Se repite hasta que el dato es válido |

- <span class="tag ok">FUENTE</span> [csharp-datos](#csharp-datos), [java-datos](#java-datos): `Input` «repite la pregunta si el dato es inválido»; `Product` valida en el constructor.
- <span class="tag ok">EXPERIMENTO</span> En la interfaz Vite, un precio negativo llega al Core de C#, que responde `400` con *«El precio no puede ser negativo»*, y ese **mismo mensaje** aparece en pantalla. **La regla vive en un único sitio; cada cliente solo la muestra.**

### B4 · El cortacircuitos

| Paso | Qué ocurre |
|---|---|
| 1 | Un servicio externo **falla** repetidamente |
| 2 | Tras un umbral, el cortacircuitos **deja de llamarlo** |
| 3 | El servicio externo, sin carga, **se recupera** |
| 4 | Se prueba con alguna llamada y, si responde, se **vuelve a llamar** con normalidad |

- <span class="tag ok">FUENTE</span> [Diccionario](#glosario-moderno): «Si un servicio falla repetidamente, se deja de llamarlo un tiempo para no empeorarlo»; reintentos con *backoff*.
- <span class="tag mute">GENERAL</span> El paso 4 («medio abierto») no está en las fuentes.

### B5 · El escalado automático

| Paso | Qué ocurre |
|---|---|
| 1 | Sube la **carga** |
| 2 | El sistema **añade servidores** |
| 3 | La carga **por servidor baja** |
| 4 | Si baja demasiado, **quita servidores** |

- <span class="tag ok">FUENTE</span> [Diccionario](#glosario-moderno): «Autoscaling: añadir o quitar servidores según la demanda».

## 2.3 Cómo se conectan los bucles

| Choque | Qué pasa |
|---|---|
| R1 (deuda) contra B1 y B2 | El pipeline y el presupuesto de errores **frenan** la espiral de la deuda, pero solo si alguien **hace caso** a los avisos |
| R1 contra R2 | Misma espiral con distinto signo; **decide el diseño inicial** (¿hay Core?) |
| R4 (IA) contra R3 (escribir) | Compiten por tu tiempo de aprendizaje |

<span class="tag warn">INFERENCIA</span> **Idea de palanca:** de todos los puntos de intervención, el más barato es **la primera decisión de diseño** (separar el Core). Después, los bucles hacen el trabajo.

---

# Parte 3 · Mapa de C# para sintetizar

## 3.1 Cómo encajan las piezas

```text
Código fuente (.cs / .fs / .vb)
        │  compilador
        ▼
Ensamblado (.dll o .exe) = código intermedio (IL) + metadatos
        │  al ejecutar: runtime (CoreCLR) con compilador JIT → código máquina
        ▼
Usa:  BCL (biblioteca base: System.*, ya incluida en el runtime)
    + frameworks (Microsoft.AspNetCore.App, Microsoft.WindowsDesktop.App…)
    + paquetes NuGet (otros .dll)

Proyecto (.csproj)  ──agrupa en──▶  Solución (.sln / .slnx)
```

| Pieza | Qué es | Dónde la ves | Etiqueta |
|---|---|---|---|
| **SDK** | Compila y trae `dotnet new/build/run/test` | `dotnet --list-sdks` | <span class="tag ok">FUENTE</span> [csharp](#csharp) §2 |
| **Runtime** | Solo ejecuta (CoreCLR, JIT, recolector de basura) | `dotnet --list-runtimes` | <span class="tag ok">FUENTE</span> (SDK frente a runtime) |
| **`TargetFramework`** | Versión y plataforma: `net10.0` (portable) o `net10.0-windows` (solo Windows) | `.csproj` | <span class="tag ok">FUENTE</span> |
| **Ensamblado** | El `.dll`/`.exe` resultante | `bin/Debug/net10.0/` | <span class="tag mute">GENERAL</span> |
| **Espacio de nombres** | Organiza tipos dentro del código | `namespace Inventory;` | <span class="tag ok">FUENTE</span> |
| **Proyecto → solución** | `.csproj` agrupado en `.sln`/`.slnx` | `dotnet sln add` | <span class="tag ok">FUENTE</span> |
| **`ProjectReference` / `PackageReference`** | Dependencia de otro proyecto / de un paquete NuGet | `.csproj` | <span class="tag ok">FUENTE</span> |
| **Framework** (`Sdk="Microsoft.NET.Sdk.Web"`, `UseWPF`, `UseWindowsForms`) | Bibliotecas grandes para web o ventanas | `.csproj` | <span class="tag ok">FUENTE</span> (el atributo) |

<span class="tag ok">EXPERIMENTO</span> En este entorno Linux, `dotnet --list-runtimes` muestra solo `Microsoft.NETCore.App` y `Microsoft.AspNetCore.App`; **no hay `Microsoft.WindowsDesktop.App`**. Eso explica, con un hecho observable, por qué WinForms y WPF compilan pero **no se ejecutan** aquí.

## 3.2 «Cómo lo uso con otros»: seis puertas

| Puerta | Cómo | Comprobado |
|---|---|---|
| **1 · Mismo lenguaje** | `ProjectReference` entre proyectos; `dotnet add package` | <span class="tag ok">EXPERIMENTO</span> (Core usado por 7 proyectos) |
| **2 · Otros lenguajes de .NET** | F# y Visual Basic referencian el mismo `.dll` (§3.3 y §3.4) | <span class="tag ok">EXPERIMENTO</span> |
| **3 · Código nativo** | P/Invoke a `user32.dll`, `libc`, etc. (§3.5) | <span class="tag ok">EXPERIMENTO</span> (Linux) |
| **4 · Otros procesos o lenguajes** | HTTP + JSON (REST), gRPC, colas | <span class="tag ok">EXPERIMENTO</span> (API + Vite, §3.7) |
| **5 · Incrustar una interfaz web en una ventana** | WebView2 dentro de WinForms/WPF, o Blazor Hybrid en MAUI | <span class="tag mute">GENERAL</span> sin probar |
| **6 · Motor de juegos** | Unity ejecuta scripts en C# con **su propio** runtime (§3.8) | <span class="tag mute">GENERAL</span> sin probar |

## 3.3 F#: qué es y para qué se usa

<span class="tag mute">GENERAL</span> **F#** es un lenguaje **funcional primero** de la misma plataforma .NET: infiere tipos casi siempre, las cosas son **inmutables por defecto**, tiene **uniones discriminadas** y *pattern matching*, y expresa los fallos como valores (`Result`) en lugar de excepciones. Se usa sobre todo en **modelado de dominio complejo, finanzas, análisis de datos, compiladores/parsers y scripting**; es un **nicho** frente a C#, con menos empleo y menos recursos, pero sigue incluido en el SDK de .NET.

Ejemplo **ejecutado** que consume tu `Inventory.Core` (escrito en C#) sin cambiarlo:

```fsharp
// F#: lenguaje "funcional primero" que compila a la MISMA plataforma .NET que C#.
// Aquí consume el Core escrito en C#, sin cambiar una sola línea de él.
open System
open Inventory

let repo = ProductRepository()

[ "Teclado mecánico", "Periféricos", 59.90m, 25
  "Ratón inalámbrico", "Periféricos", 24.50m, 60
  "Monitor 24\"", "Monitores", 139.00m, 10
  "Alfombrilla XL", "Periféricos", 14.00m, 0 ]
|> List.iter (fun (name, category, price, stock) -> repo.Add(name, category, price, stock) |> ignore)

// Unión discriminada: un tipo que es "una de varias formas"; el compilador avisa si olvidas un caso
type StockLevel =
    | OutOfStock
    | LowStock of int
    | InStock of int

let level (p: Product) =
    match p.Stock with
    | 0 -> OutOfStock
    | s when s < 10 -> LowStock s
    | s -> InStock s

// Tubería (|>): el resultado de la izquierda pasa como último argumento a la derecha
repo.FindAll()
|> Seq.filter (fun p -> p.Price > 20m)
|> Seq.sortByDescending (fun p -> p.Price)
|> Seq.iter (fun p -> printfn "%-20s %7.2f  %A" p.Name p.Price (level p))

// Result en lugar de excepciones: el fallo es un valor, no un salto
let tryProduct id name category price stock =
    try Ok(Product(id, name, category, price, stock))
    with :? ArgumentException as e -> Error e.Message

match tryProduct 99 "Cable" "Cables" -5m 3 with
| Ok p -> printfn "creado %s" p.Name
| Error message -> printfn "rechazado: %s" message

printfn "%s" (Report.Summary(repo.FindAll()))
```

Resultado:

```text
Monitor 24"           139.00  InStock 10
Teclado mecánico       59.90  InStock 25
Ratón inalámbrico      24.50  InStock 60
rechazado: El precio no puede ser negativo (Parameter 'price')
Productos: 4 | Unidades: 95 | Valor total: 4.357,50 €
  Monitores         1.390,00 €
  Periféricos       2.967,50 €
```

| Idea | C# | F# |
|---|---|---|
| Valor | `var x = 5;` (mutable) | `let x = 5` (inmutable) |
| Condicional por tipos o valores | `switch` con patrones | `match … with` (más central en el lenguaje) |
| «Una de varias formas» | jerarquía de clases / `record` + interfaz | **unión discriminada** (`type X = A | B of int`) |
| Fallo esperado | excepciones | `Result` / `Option` como valores |
| Encadenar | LINQ (`.Where().Select()`) | tubería `|>` |
| Rol típico | casi todo | dominio, datos, cálculo |

**Conclusión práctica:** no lo necesitas para tus metas (API, escritorio, Unity). Vale la pena **leer un poco de F#** porque enseña a pensar con datos inmutables, y todo eso ya existe hoy en C# (`record`, `switch` con patrones, `with`).

## 3.4 Visual Basic .NET y «.NET»: no son rivales

<span class="tag ok">EXPERIMENTO</span> **.NET es la plataforma; C#, F# y Visual Basic .NET son lenguajes sobre ella.** Los tres compilan a **el mismo tipo de ensamblado** y usan las mismas bibliotecas. Prueba: listé de qué dependen los ensamblados compilados de los tres.

```csharp
// Lista los ensamblados (.dll) de los que depende cada proyecto compilado, sea cual sea su lenguaje.
using System.Reflection;

var root = args[0];
foreach (var (label, path) in new[]
{
    ("C#  (Inventory.Core)",   "src/Inventory.Core/bin/Debug/net10.0/Inventory.Core.dll"),
    ("F#  (Inventory.FSharp)", "src/Inventory.FSharp/bin/Debug/net10.0/Inventory.FSharp.dll"),
    ("VB  (Inventory.VB)",     "src/Inventory.VB/bin/Debug/net10.0/Inventory.VB.dll"),
})
{
    var assembly = Assembly.LoadFrom(Path.Combine(root, path));
    var names = assembly.GetReferencedAssemblies().Select(a => a.Name).Order().ToList();
    Console.WriteLine($"{label}: {string.Join(", ", names)}");
}
```

```text
/tmp/claude-0/-home-user-claude/ddd43a70-8f87-5490-9d3f-755ecbba3013/scratchpad/t/synth/refs.cs(12,20): warning IL2026: Using member 'System.Reflection.Assembly.LoadFrom(String)' which has 'RequiresUnreferencedCodeAttribute' can break functionality when trimming application code. Types and members the loaded assembly depends on might be removed.
/tmp/claude-0/-home-user-claude/ddd43a70-8f87-5490-9d3f-755ecbba3013/scratchpad/t/synth/refs.cs(13,17): warning IL2026: Using member 'System.Reflection.Assembly.GetReferencedAssemblies()' which has 'RequiresUnreferencedCodeAttribute' can break functionality when trimming application code. Assembly references might be removed.
C#  (Inventory.Core): System.Collections, System.Linq, System.Runtime, System.Text.Encoding.Extensions, System.Text.Encodings.Web, System.Text.Json
F#  (Inventory.FSharp): FSharp.Core, Inventory.Core, netstandard, System.Runtime
VB  (Inventory.VB): Inventory.Core, Microsoft.VisualBasic.Core, System.Console, System.Linq, System.Runtime
```

**Cómo se lee:** los tres dependen de `System.Runtime` (la base de .NET). **F# añade `FSharp.Core`** (su biblioteca propia) y **VB añade `Microsoft.VisualBasic.Core`** (la suya). Nada más: el resto es la misma plataforma.

Visual Basic consumiendo tu Core, también **ejecutado**:

```vbnet
' Visual Basic .NET: otra sintaxis sobre la MISMA plataforma .NET.
' Consume el Core escrito en C# sin cambiarlo.
Imports Inventory

Module Program
    Sub Main()
        Dim repo As New ProductRepository()
        repo.Add("Teclado mecánico", "Periféricos", 59.9D, 25)
        repo.Add("Ratón inalámbrico", "Periféricos", 24.5D, 60)
        repo.Add("Monitor 24""", "Monitores", 139D, 10)

        ' LINQ funciona igual que en C#
        Dim expensive = repo.FindAll().Where(Function(p) p.Price > 20D).OrderByDescending(Function(p) p.Price)
        For Each p In expensive
            Console.WriteLine($"{p.Name,-20} {p.Price,7:F2}")
        Next

        Try
            Dim invalid As New Product(99, "Cable", "Cables", -5D, 3)
        Catch ex As ArgumentException
            Console.WriteLine("rechazado: " & ex.Message)
        End Try

        Console.WriteLine(Report.Summary(repo.FindAll()))
    End Sub
End Module
```

```text
Monitor 24"           139.00
Teclado mecánico       59.90
Ratón inalámbrico      24.50
rechazado: El precio no puede ser negativo (Parameter 'price')
Productos: 3 | Unidades: 95 | Valor total: 4.357,50 €
  Monitores         1.390,00 €
  Periféricos       2.967,50 €
```

| | C# | VB.NET | Visual Basic clásico (VB6) |
|---|---|---|---|
| Plataforma | .NET | .NET | **Distinta**: runtime propio y COM, sin .NET |
| Acceso a la API de Windows | P/Invoke (`DllImport`) | P/Invoke (`Declare` o `DllImport`) | `Declare` a DLL; sin BCL de .NET |
| Estado | Evoluciona cada año | <span class="tag mute">GENERAL</span> Microsoft indicó que sigue **soportado** pero **sin nuevas funciones de lenguaje**; comprueba la situación actual | <span class="tag mute">GENERAL</span> El entorno de desarrollo dejó de tener soporte en 2008; el *runtime* de VB6 ha seguido acompañando a Windows. Comprueba el estado actual |

**Respuesta a «¿qué diferencia hay en el acceso a la API de Windows?»:** <span class="tag ok">EXPERIMENTO</span> **ninguna de fondo** entre C# y VB.NET: ambos usan el mismo mecanismo (§3.5) y, como se ve arriba, el mismo ensamblado. Las diferencias son de sintaxis (por ejemplo, la palabra `Declare`) y de tradición (VB se usó mucho para automatizar Office y COM). La diferencia real es con **VB6**, que no es .NET.

## 3.5 La API de Windows desde C#: seis capas

De la más cómoda a la más directa:

| Capa | Qué es | Ejemplo | Cuándo |
|---|---|---|---|
| 1 · **Biblioteca base (BCL)** | .NET ya envuelve lo habitual | `File`, `Process`, `Environment.TickCount64` | **Siempre primero** |
| 2 · **Marcos de interfaz** | WinForms/WPF/WinUI envuelven `user32`, GDI, DirectX | `new Form()` | Ventanas |
| 3 · **APIs de Windows proyectadas** | Acceso a APIs modernas de Windows 10/11 mediante un `TargetFramework` con versión de Windows (`net10.0-windows10.0.19041.0`) | notificaciones, Bluetooth, etc. | APIs nuevas de Windows <span class="tag mute">GENERAL</span> |
| 4 · **COM interop** | Hablar con componentes COM (Office, WMI antiguo) | `Excel.Application` | Automatizar Office |
| 5 · **P/Invoke** | Llamar **directamente** a `user32.dll`, `kernel32.dll`… | `MessageBoxW`, `GetSystemMetrics` | Lo que la BCL no cubre |
| 6 · **C++/CLI** | Puente en C++ para casos extremos | — | Raro |

<span class="tag mute">GENERAL</span> Las capas 3, 4 y 6 no se probaron aquí. La **capa 5** sí, en su parte portable:

```csharp
using System.Runtime.InteropServices;

// P/Invoke (Platform Invoke): llamar a una función de una biblioteca NATIVA (no .NET) desde C#.
// Es la puerta a "la API de Windows" (Win32) y también a cualquier .so/.dylib/.dll.

Console.WriteLine($"Sistema: {RuntimeInformation.OSDescription}");

if (OperatingSystem.IsWindows())
{
    // Estas llamadas SOLO se ejecutan en Windows (user32.dll y kernel32.dll no existen en Linux)
    Console.WriteLine($"GetTickCount64 (kernel32) = {Win32.GetTickCount64()} ms");
    Console.WriteLine($"Environment.TickCount64   = {Environment.TickCount64} ms   <- la biblioteca base ya lo envuelve");
    Console.WriteLine($"Ancho de pantalla (user32 GetSystemMetrics) = {Win32.GetSystemMetrics(0)} px");
    Win32.MessageBox(IntPtr.Zero, "Hola desde la API de Windows", "P/Invoke", 0);
}
else if (OperatingSystem.IsLinux())
{
    // El MISMO mecanismo, apuntando a la biblioteca de C de Linux
    Console.WriteLine($"getpid() de libc (P/Invoke) = {Libc.getpid()}");
    Console.WriteLine($"Environment.ProcessId       = {Environment.ProcessId}   <- la biblioteca base ya lo envuelve");
    Console.WriteLine($"getpid() estilo clásico (DllImport) = {LibcClassic.getpid()}");
}

// Estilo moderno (.NET 7+): LibraryImport. Un generador de código escribe el "pegamento" al compilar (más rápido, compatible con AOT).
static partial class Win32
{
    [LibraryImport("kernel32.dll")]
    internal static partial ulong GetTickCount64();

    [LibraryImport("user32.dll")]
    internal static partial int GetSystemMetrics(int index);

    // StringMarshalling.Utf16: cómo se convierte el string de .NET al texto que espera Windows ("W" = wide, UTF-16)
    [LibraryImport("user32.dll", EntryPoint = "MessageBoxW", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
    internal static partial int MessageBox(IntPtr hWnd, string text, string caption, uint type);
}

static partial class Libc
{
    [LibraryImport("libc", EntryPoint = "getpid")]
    internal static partial int getpid();
}

// Estilo clásico (desde .NET Framework 1.0): DllImport con "extern". El "pegamento" se genera al ejecutar, por reflexión.
static class LibcClassic
{
    [DllImport("libc", EntryPoint = "getpid")]
    internal static extern int getpid();
}
```

<span class="tag ok">EXPERIMENTO</span> Ejecutado en Linux (rama de `libc`; la rama de Windows **compila pero no se ejecutó**):

```text
Sistema: Ubuntu 24.04.5 LTS
getpid() de libc (P/Invoke) = 1462
Environment.ProcessId       = 1462   <- la biblioteca base ya lo envuelve
getpid() estilo clásico (DllImport) = 1462
```

El **mismo mecanismo** que llamaría a `MessageBoxW` en Windows llama a `getpid` en Linux, y el resultado coincide con `Environment.ProcessId`: **la biblioteca base ya hace ese trabajo por ti**; P/Invoke es para lo que no hace.

| Pieza | Significado |
|---|---|
| `[LibraryImport("kernel32.dll")]` | Estilo moderno (.NET 7+): declara una función nativa; un **generador de código** escribe el «pegamento» al compilar. Exige `partial` y `AllowUnsafeBlocks` |
| `[DllImport("libc")]` + `extern` | Estilo clásico: el «pegamento» se genera al ejecutar |
| `EntryPoint = "MessageBoxW"` | Nombre real de la función en la biblioteca |
| `StringMarshalling.Utf16` | Cómo se convierten los `string` de .NET al texto que espera Windows (la «W» de `MessageBoxW` es *wide*, UTF-16) |
| `SetLastError = true` | Guarda el código de error de Windows para leerlo con `Marshal.GetLastPInvokeError()` |
| `OperatingSystem.IsWindows()` | Evita ejecutar llamadas de Windows en otro sistema |

Trampas típicas (<span class="tag mute">GENERAL</span>): tamaño de punteros (32/64 bits), cadenas ANSI frente a Unicode (`A`/`W`), disposición de estructuras (`StructLayout`), liberar recursos con `SafeHandle`. Existe un generador (**CsWin32**, paquete NuGet `Microsoft.Windows.CsWin32`) que escribe las firmas de la API de Windows por ti; **no lo probé**.

Versión de Visual Basic del mismo mecanismo, también ejecutada en Linux:

```vbnet
Imports System.Runtime.InteropServices

Module Program
    ' Visual Basic .NET: la misma P/Invoke, con la palabra "Declare" (o con <DllImport>, igual que en C#)
    Declare Auto Function MessageBox Lib "user32.dll" Alias "MessageBoxW" (hWnd As IntPtr, text As String, caption As String, type As UInteger) As Integer

    <DllImport("libc", EntryPoint:="getpid")>
    Private Function GetPid() As Integer
    End Function

    Sub Main()
        If OperatingSystem.IsWindows() Then
            MessageBox(IntPtr.Zero, "Hola desde VB.NET", "P/Invoke", 0UI)
        ElseIf OperatingSystem.IsLinux() Then
            Console.WriteLine($"getpid() desde VB.NET = {GetPid()}   (Environment.ProcessId = {Environment.ProcessId})")
        End If
    End Sub
End Module
```

```text
getpid() desde VB.NET = 1491   (Environment.ProcessId = 1491)
```

## 3.6 Interfaces de escritorio: cuál elegir para practicar

| Tecnología | Plataformas | Cómo se dibuja | «Bonita» | Etiqueta |
|---|---|---|---|---|
| **WinForms** | Solo Windows | Controles clásicos de Windows, arrastrar y soltar | Aspecto clásico; sencillo | <span class="tag ok">EXPERIMENTO</span> compila (ya en tu módulo) |
| **WPF** | Solo Windows | XAML + enlace de datos (MVVM); dibujo vectorial | **Muy flexible**: estilos, plantillas, animaciones | <span class="tag ok">EXPERIMENTO</span> compila (§siguiente) |
| **WinUI 3** | Windows 10/11 | XAML moderno (Fluent) | Aspecto actual de Windows | <span class="tag mute">GENERAL</span> sin probar |
| **.NET MAUI** | Android, iOS, macOS, Windows; <span class="tag mute">GENERAL</span> **no Linux** de forma oficial | XAML, un solo código para móvil y escritorio | Nativa en cada sistema | <span class="tag mute">GENERAL</span> sin probar (requiere cargas de trabajo y SDK móviles) |
| **Avalonia / Uno** | Windows, macOS, **Linux**, web, móvil | XAML parecido a WPF | Muy configurable | <span class="tag mute">GENERAL</span> sin probar |
| **Web con Vite (React/Vue)** | Cualquiera | HTML/CSS/JS contra una API de C# | **La que más opciones visuales ofrece** | <span class="tag ok">EXPERIMENTO</span> (§3.7) |
| **Blazor** | Cualquiera (web) | C# en lugar de JavaScript para la interfaz | Web | <span class="tag mute">GENERAL</span> sin probar |

**Ruta de práctica con tu mismo `Core`:** consola (hecho) → **WinForms** (hecho) → **WPF con MVVM** (esqueleto abajo) → **web con Vite** (comprobada) → MAUI solo si necesitas móvil.

### WPF + MVVM: lo mínimo que hay que entender

El **ViewModel** es lo que la ventana *muestra y puede hacer*, sin conocer ningún control; la **Vista** (XAML) se **enlaza** con `{Binding …}`:

```csharp
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Inventory.Wpf;

/// <summary>
/// ViewModel (patrón MVVM): lo que la ventana MUESTRA y puede hacer, sin conocer ningún control.
/// La vista (XAML) se enlaza a estas propiedades con {Binding ...}.
/// </summary>
public sealed class ProductsViewModel : INotifyPropertyChanged
{
    private readonly ProductRepository _repository = new();
    private string _query = "";

    public ProductsViewModel()
    {
        _repository.Add("Teclado mecánico", "Periféricos", 59.90m, 25);
        _repository.Add("Ratón inalámbrico", "Periféricos", 24.50m, 60);
        _repository.Add("Monitor 24\"", "Monitores", 139.00m, 10);
        Refresh();
    }

    // ObservableCollection avisa a la tabla cuando cambia: no hay que redibujar a mano
    public ObservableCollection<Product> Products { get; } = [];

    public string Query
    {
        get => _query;
        set { _query = value; OnPropertyChanged(); Refresh(); }
    }

    public string SummaryText => $"{Products.Count} productos";

    private void Refresh()
    {
        Products.Clear();
        foreach (var p in string.IsNullOrWhiteSpace(_query) ? _repository.FindAll() : _repository.Search(_query))
            Products.Add(p);
        OnPropertyChanged(nameof(SummaryText));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
```

```xml
<Window x:Class="Inventory.Wpf.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:local="clr-namespace:Inventory.Wpf"
        Title="Inventario (WPF)" Height="360" Width="620">
    <Window.DataContext>
        <local:ProductsViewModel />   <!-- la vista se conecta con su ViewModel -->
    </Window.DataContext>
    <DockPanel Margin="12">
        <StackPanel DockPanel.Dock="Top" Orientation="Horizontal" Margin="0,0,0,8">
            <TextBlock Text="Buscar:" VerticalAlignment="Center" Margin="0,0,6,0" />
            <TextBox Width="200" Text="{Binding Query, UpdateSourceTrigger=PropertyChanged}" />
            <TextBlock Margin="16,0,0,0" VerticalAlignment="Center" Text="{Binding SummaryText}" />
        </StackPanel>
        <DataGrid ItemsSource="{Binding Products}" AutoGenerateColumns="False" IsReadOnly="True">
            <DataGrid.Columns>
                <DataGridTextColumn Header="Nombre"    Binding="{Binding Name}" Width="*" />
                <DataGridTextColumn Header="Categoría" Binding="{Binding Category}" Width="130" />
                <DataGridTextColumn Header="Precio"    Binding="{Binding Price, StringFormat=N2}" Width="80" />
                <DataGridTextColumn Header="Stock"     Binding="{Binding Stock}" Width="60" />
            </DataGrid.Columns>
        </DataGrid>
    </DockPanel>
</Window>
```

<span class="tag ok">EXPERIMENTO</span> Este proyecto **compila** en Linux con `EnableWindowsTargeting`; **no se ejecutó** (WPF solo corre en Windows). Pruébalo en Visual Studio: abre `Inventory.slnx` y pon `Inventory.Wpf` como proyecto de inicio.

| Pieza | Significado |
|---|---|
| `INotifyPropertyChanged` | Avisa a la vista de que una propiedad cambió |
| `ObservableCollection<T>` | Lista que avisa cuando se añade o quita algo |
| `{Binding Query, UpdateSourceTrigger=PropertyChanged}` | Enlaza el cuadro de texto a la propiedad y actualiza al teclear |
| `DataContext` | El objeto al que apuntan los `{Binding}` de la ventana |

## 3.7 C# como backend y Vite como interfaz (comprobado)

```text
Navegador ──(HTML/JS de Vite)──▶ pide /api/products
                                       │  HTTP + JSON
                                       ▼
                         ASP.NET Core (Inventory.Api)
                                       │  referencia de proyecto
                                       ▼
                         Inventory.Core (el mismo de la consola)
```

El backend envuelve el Core (con un `lock`, porque el Core no es seguro para hilos):

```csharp
using Inventory;

var builder = WebApplication.CreateBuilder(args);

// El Core NO es seguro para hilos (usa List<T> sin cerrojo, porque se pensó para una ventana de un solo hilo).
// Una API atiende peticiones a la vez, así que lo envolvemos con un cerrojo sin tocar el Core.
builder.Services.AddSingleton<SafeInventory>();

var app = builder.Build();

app.UseDefaultFiles();   // sirve wwwroot/index.html en "/"
app.UseStaticFiles();    // sirve el resultado de "vite build" (HTML, JS, CSS)

var products = app.MapGroup("/api/products");

products.MapGet("/", (string? q, SafeInventory inventory) =>
    inventory.Read(repo => (string.IsNullOrWhiteSpace(q) ? repo.FindAll() : repo.Search(q)).Select(ProductDto.From)));

products.MapPost("/", (CreateProduct request, SafeInventory inventory) =>
{
    try
    {
        var created = inventory.Write(repo => repo.Add(request.Name, request.Category, request.Price, request.Stock));
        return Results.Created($"/api/products/{created.Id}", ProductDto.From(created));
    }
    catch (ArgumentException e)   // las reglas de validación viven en el Core (Product); aquí solo se traducen a HTTP
    {
        return Results.BadRequest(new { error = e.Message });
    }
});

products.MapDelete("/{id:int}", (int id, SafeInventory inventory) =>
    inventory.Write(repo => repo.Delete(id)) ? Results.NoContent() : Results.NotFound());

app.MapGet("/api/summary", (SafeInventory inventory) => Results.Text(inventory.Read(repo => Report.Summary(repo.FindAll())), "text/plain; charset=utf-8"));

app.Run();

public record CreateProduct(string Name, string Category, decimal Price, int Stock);

public record ProductDto(int Id, string Name, string Category, decimal Price, int Stock, decimal TotalValue)
{
    public static ProductDto From(Product p) => new(p.Id, p.Name, p.Category, p.Price, p.Stock, p.TotalValue);
}

/// <summary>Envuelve el repositorio del Core con un cerrojo (lock).</summary>
public sealed class SafeInventory
{
    private readonly ProductRepository _repo = new();
    private readonly object _gate = new();

    public SafeInventory()
    {
        _repo.Add("Teclado mecánico", "Periféricos", 59.90m, 25);
        _repo.Add("Ratón inalámbrico", "Periféricos", 24.50m, 60);
        _repo.Add("Monitor 24\"", "Monitores", 139.00m, 10);
        _repo.Add("SSD 1 TB", "Almacenamiento", 79.90m, 30);
    }

    public T Read<T>(Func<ProductRepository, T> action) { lock (_gate) return action(_repo); }
    public T Write<T>(Func<ProductRepository, T> action) { lock (_gate) return action(_repo); }
}
```

La interfaz solo habla HTTP + JSON; en desarrollo, Vite reenvía `/api` al backend, y al construir deja los archivos dentro de `wwwroot`, que ASP.NET sirve:

```javascript
import { defineConfig } from 'vite';

export default defineConfig({
  server: {
    port: 5173,
    // En desarrollo, Vite reenvía las peticiones /api al backend de C#.
    // Así el navegador cree que todo viene del mismo sitio y no hace falta CORS.
    proxy: { '/api': 'http://localhost:5080' },
  },
  build: {
    // "vite build" deja el resultado dentro de wwwroot, que ASP.NET Core sirve como archivos estáticos
    outDir: '../src/Inventory.Api/wwwroot',
    emptyOutDir: true,
  },
});
```

Dos formas de trabajar:

```bash
# Desarrollo: dos procesos, recarga instantánea de la interfaz
dotnet run --project src/Inventory.Api --urls http://localhost:5080
cd web && npx vite            # abre http://localhost:5173

# Producción: un solo proceso que sirve la API y la interfaz ya construida
cd web && npx vite build      # escribe src/Inventory.Api/wwwroot
dotnet run --project src/Inventory.Api --urls http://localhost:5080   # abre http://localhost:5080
```

| Pieza | Significado |
|---|---|
| `server.proxy: { '/api': '…' }` | Vite reenvía las peticiones `/api` al backend; el navegador cree que todo viene del mismo sitio, **no hace falta CORS** |
| `build.outDir` + `emptyOutDir` | Dónde deja `vite build` el resultado y que lo vacíe antes de escribir |
| `app.UseDefaultFiles()` / `UseStaticFiles()` | ASP.NET sirve `wwwroot/index.html` y los archivos estáticos |
| `MapGroup("/api/products")` | Agrupa rutas bajo un prefijo (Minimal API) |
| `catch (ArgumentException)` → `400` | Las **reglas viven en el Core**; la API solo las traduce a HTTP |
| `--urls` | Dirección y puerto en que escucha |

<span class="tag ok">EXPERIMENTO</span> **Prueba de extremo a extremo con un navegador real (Playwright), en los dos modos:**

| Modo | Filas iniciales | Filtro «monitor» | Tras añadir | Precio negativo |
|---|---|---|---|---|
| Producción (ASP.NET sirve el build de Vite) | 4 | 1 | 5 | `400` y mensaje del Core en pantalla |
| Desarrollo (Vite `:5173` → proxy → C# `:5080`) | 5 | 1 | 6 | `400` y mismo mensaje |

(En desarrollo había 5 filas porque ya se había añadido una en la prueba anterior: el repositorio vive en memoria mientras el servidor corre.) El único «error» de consola fue el `400` esperado.

**Esto conecta con las fuentes:** [spring](#spring) ya afirma que «la interfaz de React o Vue funciona con cualquiera de las dos [APIs] sin cambios». Aquí se comprueba con **C#** y con el **Core de consola**, y esa es la prueba más fuerte de la afirmación central: **un cuarto cliente (la web) sin modificar el Core**.

<span class="tag mute">GENERAL</span> Visual Studio ofrece integración con proyectos de JavaScript (un `.esproj` junto al proyecto de C#); no la probé.

## 3.8 Unity: lo que hay que comprobar al volver

Las fuentes solo mencionan Unity como «videojuegos» en una tabla. Todo lo siguiente es <span class="tag mute">GENERAL</span>; **compruébalo en la documentación de tu versión de Unity**:

| Tema | Qué suele ocurrir |
|---|---|
| **Lenguaje** | Los scripts son C#, pero con una **versión de lenguaje y un runtime propios de Unity** (no el .NET 10 de tu SDK); algunas novedades recientes de C# pueden no estar disponibles |
| **Runtime** | Unity usa su propio runtime (Mono o IL2CPP); ha anunciado intención de acercarse al .NET moderno. **Mira el estado en tu versión** |
| **Paquetes** | NuGet **no funciona directamente**; se usan paquetes de Unity o `.dll` incluidos a mano |
| **Estructura** | Los scripts heredan de `MonoBehaviour` (`Awake`, `Start`, `Update`…) y se **añaden a objetos** de la escena (modelo de componentes) |
| **Cómo aplicar el Core** | Pon la lógica en **clases C# normales sin `UnityEngine`**, en su propio *assembly definition* (`.asmdef`), y deja los `MonoBehaviour` **finos** (como la «ventana»). Así la lógica se puede probar sin abrir el motor |

<span class="tag warn">INFERENCIA</span> Es la afirmación central aplicada a Unity: **el `MonoBehaviour` es la interfaz; la lógica del juego es el Core**. Si metes todo en `Update`, vuelves a R1 (espiral de la deuda).

---

# Parte 4 · Síntesis

## 4.1 Tabla de una página

| Pregunta | Respuesta corta | Dónde se comprobó |
|---|---|---|
| ¿Cómo conecta cada pieza? | Código → ensamblado (IL) → runtime con JIT; proyectos en soluciones; BCL + frameworks + NuGet | §3.1, experimento de runtimes |
| ¿Cómo lo uso con otros? | Seis puertas: mismo lenguaje, otros lenguajes .NET, nativo, HTTP, interfaz web incrustada, Unity | §3.2 |
| ¿Qué es F# y para qué sirve? | Lenguaje funcional de .NET; nicho (dominio, finanzas, datos, parsers) | §3.3 (código ejecutado) |
| ¿Diferencia con .NET y Visual Basic? | .NET es la plataforma; C#, F#, VB.NET son lenguajes sobre ella; mismo acceso a Windows | §3.4 (ensamblados comparados) |
| ¿Cómo accedo a la API de Windows? | Primero la BCL; si no, P/Invoke (o COM/WinRT) | §3.5 (P/Invoke ejecutado en Linux) |
| ¿WinForms, WPF, MAUI…? | WinForms simple, WPF flexible (MVVM), MAUI móvil+escritorio sin Linux, Avalonia multiplataforma | §3.6 (WinForms y WPF compilan) |
| ¿Otra interfaz con C# de backend? | Sí: ASP.NET Core + Vite, por proxy en desarrollo o servido desde `wwwroot` | §3.7 (probado con navegador) |
| ¿Y Unity? | Mismo lenguaje, otro runtime; aplica el Core con clases sin `UnityEngine` | §3.8 (sin probar) |

## 4.2 La idea que lo une todo

> **La lógica es el activo; la interfaz es un periférico.** C#, F# y VB comparten plataforma; WinForms, WPF, MAUI, una web con Vite o Unity son *periféricos* distintos. Si el Core está separado, **cambiar de interfaz cuesta una tarde**; si no lo está, cuesta una reescritura.

## 4.3 Lo que NO se comprobó

- <span class="tag bad">no comprobado</span> Ejecución de **WinForms y WPF** (solo compilan en Linux).
- <span class="tag bad">no comprobado</span> La **rama Windows** de P/Invoke (`MessageBoxW`, `GetSystemMetrics`).
- <span class="tag bad">no comprobado</span> **MAUI, WinUI 3, Avalonia, Uno, Blazor, WebView2, CsWin32, COM, WinRT** y **Unity**.
- <span class="tag bad">no comprobado</span> Las cronologías y los estados de soporte (VB, Xamarin, versiones de Unity): son conocimiento general y cambian.
- **Limitación de las fuentes:** son material de referencia escrito para este proyecto, no estudios independientes. Los niveles 3 y 4 y los bucles son **lectura mía**.

## 4.4 Ejercicios

> **Ejercicio 1:** abre `Inventory.slnx` en Visual Studio (tu anfitrión), pon `Inventory.Wpf` como proyecto de inicio y ejecútalo. Añade un botón «Añadir» con un `ICommand` (te hará falta una clase `RelayCommand`).

> **Ejercicio 2:** añade a la API `PATCH /api/products/{id}` para cambiar el stock. Pon la validación **en el Core**, no en la API, y comprueba que la interfaz Vite muestra el mensaje sin tocar el JavaScript.

> **Ejercicio 3:** el Core no es seguro para hilos. Escribe una prueba xUnit que lance 100 `Add` en paralelo sobre `ProductRepository` y observa qué falla. Decide: ¿cerrojo en el Core o en el cliente? Justifica con R1/R2.

> **Ejercicio 4:** lleva el mismo Core a una **segunda interfaz de escritorio** que se ejecute en Linux (Avalonia) y compara el esfuerzo con WPF.

> **Ejercicio 5 (Unity):** crea una clase `Inventory` sin `UnityEngine`, en su propio `.asmdef`, con pruebas, y llámala desde un `MonoBehaviour` de una sola línea por método.

> **Ejercicio 6:** elige otra afirmación central de las fuentes (por ejemplo, «el contrato importa más que la implementación») y analízala tú en los cuatro niveles. Marca cada frase como FUENTE o INFERENCIA.
