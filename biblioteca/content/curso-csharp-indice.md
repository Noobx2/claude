# Curso de C# completo · de cero a profesional

**C# 14 · .NET 10 · ASP.NET Core · Entity Framework Core · PostgreSQL · Blazor · WPF · Unity**

Un curso práctico de **C# y de su ecosistema**, pensado para quien necesita **recuperar la autonomía** como programador: entender **qué hace cada línea y por qué**, y poder construir un proyecto completo sin copiar ciegamente. Está en **once partes y tres anexos**; **cada ejemplo se ejecutó** y la salida que ves es la real (con excepciones **señaladas** donde ocurren).

*Verificado: 2026-10-29 · .NET SDK 10.0.113 · PostgreSQL 16 · Chromium (Playwright) · Linux.*

## Cómo está organizado

| Parte | Contenido | Lecciones |
|---|---|---|
| **I. El lenguaje** | Archivos y proyectos, tipos, control de flujo, métodos, texto, colecciones, LINQ, POO, records, genéricos, null, patrones, excepciones, delegados, asincronía, archivos/JSON, xUnit, NuGet y DI, de Java a C#, de C# a Unity | 1–22 |
| **II. Captura de datos y CRUD** | Leer datos de **todas las formas** posibles, mostrarlos, un **CRUD de verdad** (memoria, archivo), y **TaskFlow** en capas con pruebas | 23–27 |
| **III. Orientación a objetos y diseño** | **Clases abstractas**, interfaces, composición, **Singleton**, **inyección de dependencias**, interfaces estándar, **patrones de diseño** (creacionales, estructurales, de comportamiento), **SOLID**, herramientas del lenguaje | 28–36 |
| **IV. .NET por dentro y herramientas** | CLR, IL, JIT, AOT, **versiones y soporte**, `global.json`, soluciones, ensamblados, **NuGet**, **Git**, depuración y trazas, estructuras de datos y rendimiento | 37–40 |
| **V. C# avanzado** | **C# 8 → C# 14**, reflexión, atributos, **generadores de código**, **concurrencia**, **asincronía** a fondo | 41–44 |
| **VI. Pruebas** | Pirámide, **TDD**, dobles, **BDD**, cobertura | 45 |
| **VII. Desarrollo web** | HTTP, middleware, MVC, arquitecturas, **API REST**, **EF Core + PostgreSQL**, migraciones, **JWT**, secretos, configuración, pruebas de integración, despliegue | 46–50 |
| **VIII. Blazor** | Componentes, Server frente a WebAssembly, **Hot Reload**, un **CRUD completo** contra la API | 51–52 |
| **IX. Escritorio** | **WPF y MVVM** | 53 |
| **X. Unity** | Lo que cambia en Unity, **límites de C# 9**, ciclo de vida, patrones | 54–55 |
| **XI. Proyectos integradores** | TaskFlow de punta a punta, **Reservas** (TDD), **Combate por turnos** | — |
| **Anexos** | **A**: memoria, destructores, `IDisposable` · **B**: tipado, `dynamic`, paradigmas · **C**: código limpio | A–C |

## Qué se ejecutó y qué NO (léelo antes de fiarte)

| Qué | Estado |
|---|---|
| Programas de consola, lógica, colecciones, LINQ, async, concurrencia | **Ejecutados**, con su salida real |
| API web, PostgreSQL, migraciones, JWT, EF Core | **Ejecutados** (API real, base real, `curl`, 40 pruebas) |
| Blazor WebAssembly | **Ejecutado en Chromium** (17 pasos de usuario, vista móvil) |
| Rendimiento, mediciones | **Medidos** en una máquina de 4 núcleos; **valen las proporciones**, no los milisegundos |
| **Docker, `docker compose`, GitHub Actions** | **Escritos y con sintaxis validada; NO ejecutados** (sin demonio Docker) |
| **WPF** | La lógica (**ViewModels**) se **ejecutó y probó**; el XAML solo se **compiló**, **no se ejecutó** (WPF solo corre en Windows) |
| **Unity** | La lógica pura (C# 9, `netstandard2.1`) se **ejecutó y probó**; los scripts de Unity **solo se compilaron contra un «molde»** escrito aquí; **ninguno se ejecutó en Unity** |
| **Visual Studio / Rider** | **No usados**. Los pasos del depurador y los atajos se **describen**, no se ejecutaron |
| **HTTP/3, Aspire, .NET MAUI** | **No ejecutados** (se explican o se mencionan) |
| Otros sistemas (Windows, macOS), otros navegadores | **No probados** |

Cada lección vuelve a indicar sus propias salvedades.

## Cómo estudiar este curso

1. **Escribe cada ejemplo a mano** y ejecútalo. Cambia algo y **predice** qué saldrá antes de ejecutar.
2. **Haz los ejercicios** (no tienen solución a propósito; los proyectos de la Parte XI sí).
3. **Cuando un resultado te sorprenda**, no sigas: es **la** lección.
4. **Rompe el código a propósito** y mira si las pruebas se quejan. Una prueba que nunca ha fallado no ha demostrado nada.
5. Orden recomendado: **I → II → III** forman el núcleo; después, **según tu objetivo**: web (**IV, VII, VIII**), escritorio (**IX**), juegos (**X**), calidad (**VI**, Anexo C).

**Convenciones:** los **identificadores** del código van **en inglés** (en las primeras partes) o **en español** donde el dominio lo pide (Reservas, Combate); los **comentarios**, en español; **cada comando y cada opción se explican**.

## Si tu pregunta es «¿hay un gestor de versiones?»

Son **tres cosas distintas** (se explican en la lección 37):

| Quiero controlar… | Herramienta | Lección |
|---|---|---|
| **El historial de mi código** | **Git** (y GitHub/GitLab para compartirlo) | 38 |
| **Qué versión de .NET usa mi proyecto** | `global.json` + `<TargetFramework>` | 37 |
| **Qué versión de cada biblioteca uso** | **NuGet** (`PackageReference`) | 38, 20 |

## Cobertura del temario comercial «C# 14 con .NET 10 y Visual Studio 2026»

Esta tabla compara el curso con el **temario de un curso presencial comercial de 5 días**. Para cada punto se indica **dónde se cubre** y **con qué profundidad**. Los puntos que dependen de **un instructor** (evaluación inicial, quiz final, certificado, ejercicios en grupo) **no son cuestión de contenido** y no se comparan.

**Leyenda:** **Completo** = explicado y ejecutado · **Parcial** = tratado pero con menos profundidad o sin ejecutar parte · **Solo texto** = explicado, **no** ejecutado · **Solo mención** = nombrado, sin desarrollar.

### Introducción: ¿qué es .NET en 2026?

| Punto del temario | Dónde | Estado |
|---|---|---|
| ¿Para quién es .NET? Evolución del framework | L37 | Completo |
| El futuro de .NET (ciclo anual, LTS/STS, hoja de ruta) | L37; Unity y CoreCLR en L54 | **Parcial**: ciclo y soporte al día; sin la hoja de ruta de versiones futuras |
| Arquitectura, código administrado, IL / CLR / JIT | L37 | Completo (IL desensamblado, JIT medido) |
| Lenguaje interpretado frente a compilado | L37 | Completo |
| Visual Studio frente a VS Code frente a Rider | L38 | **Solo texto** (no se usó ningún IDE) |
| No confundir IDE, lenguaje y framework | L37 | Completo |
| Evolución y novedades de C# | L41 | Completo |
| Ciclo LTS/Current y soporte | L37 | Completo (fechas de octubre de 2026) |
| .NET 10: **Aspire** | L37 | **Solo mención** |
| .NET 10: mejoras, **Native AOT**, **AOT** | L37 | Completo (AOT publicado y medido) |
| .NET 10: **globalización** | L6, L23, L37 | **Parcial** (cultura e invariante) |
| .NET 10: **contenedores** | L37, L50 | **Solo texto** (Dockerfile escrito, no ejecutado) |
| .NET 10: **NuGet** | L20, L38 | Completo (paquete propio creado y consumido) |

### Ecosistema de .NET

| Punto | Dónde | Estado |
|---|---|---|
| Core frente a «Mainstream» frente a Standard | L37 | Completo |
| Tabla de versiones de C# por versión de .NET | L37, L41 | Completo |
| WPF | L53 | **Parcial** (ViewModel ejecutado; XAML solo compilado) |
| Entity Framework | L48 | Completo |
| Documentación (MSDN/Learn), GitHub, Git | Abajo; L38 | **Parcial** (Git ejecutado; GitHub solo descrito) |

### Visual Studio: lo básico

| Punto | Dónde | Estado |
|---|---|---|
| Solución, proyecto, **ensamblado**, DLL, paquete NuGet | L2, L38 | Completo |
| Añadir un proyecto a una solución, **referenciar** un proyecto, referenciar bibliotecas externas | L38, L20 | Completo (con `dotnet`; **no** con el IDE) |
| *Shared Project* | L38 | Solo texto |
| Espacios de nombres | L1, L38 | Completo |
| Atajos de teclado | L38 | **Solo texto** |

### C#: lo básico

| Punto | Dónde | Estado |
|---|---|---|
| Paradigmas y tipado | **Anexo B** | Completo |
| Tipos primitivos | L3 | Completo |
| Miembros de clase, encapsulación y niveles de visibilidad | L9, L10 | Completo |
| Estructuras frente a clases frente a records | L11, L40 | Completo |
| Tuplas | L5, L7, L11, L14 | Completo |
| `using` | L1 | Completo |
| **Constructores y destructores** | L9; **Anexo A** | Completo |
| Genéricos | L12 | Completo |
| Operadores y expresiones | L4, L36 | Completo |
| Mundo **estático** frente a **instancia** | L5, L9, L28 | Completo |
| Mundo **administrado** frente a **no administrado** | L37, **Anexo A** | Completo |

### Depuración

| Punto | Dónde | Estado |
|---|---|---|
| Depurar, *breakpoints*, condiciones, modificar variables en vivo | L39 | **Solo texto** (procedimiento del IDE; no se ejecutó un depurador) |
| Registro, instrumentación, `Trace`, *listeners* | L39 | Completo |

### Programación orientada a objetos y buenas prácticas

| Punto | Dónde | Estado |
|---|---|---|
| Procedural frente a POO; encapsulación, herencia, polimorfismo | L9, L10, L28; Anexo B | Completo |
| Convenciones de nombres, responsabilidad, refactorización | **Anexo C** | Completo (medido por el compilador) |
| **SOLID** | L35 | Completo |
| **DRY / YAGNI / KISS / WET** | **Anexo C** | Completo |
| Intención y nombres | **Anexo C** | Completo |
| Ciclo de validación | L45 (TDD), L50 (CI) | Parcial |

### Codificación básica

| Punto | Dónde | Estado |
|---|---|---|
| Variables locales, `static`/`const`, declaraciones, expresiones, booleanos | L3, L4, L5 | Completo |
| Comentarios | En todo el código | Completo |
| **Directivas de preprocesador** y símbolos de compilación (`#if`, `#define`, `#region`, `#pragma`) | L39; Anexo C | Completo |
| Tipos por valor y por referencia | L40 | Completo |

### Estructuras de datos y código optimizado

| Punto | Dónde | Estado |
|---|---|---|
| LIFO, FIFO; arreglos, listas, pilas, colas, diccionarios, *hashset*, listas genéricas | L7, L40 | Completo |
| Elegir la estructura para el rendimiento | L40 (**medido**) | Completo |
| **Boxing**/unboxing; concatenar cadenas; excepciones; `while/for/foreach` | L40, L4, L15 | Completo (medido) |
| Delegados | L16 | Completo |

### Clásicos de C#

| Punto | Dónde | Estado |
|---|---|---|
| Clases **abstractas, virtuales y parciales** | L28, L36 | Completo |
| **Interfaces**, y **abstracta frente a interfaz** | L28, L31 | Completo |
| Mutable frente a inmutable | L11 | Completo |
| Genéricos; tipos anulables | L12, L13 | Completo |
| **Tipos dinámicos** | **Anexo B** | Completo (con su coste medido) |
| Sobrecarga de operadores; igualdad | L31, L36 | Completo |
| `try/catch/finally` | L15 | Completo |
| Serialización | L18 | Completo (JSON y binario) |

### Patrones de diseño, pruebas y novedades

| Punto | Dónde | Estado |
|---|---|---|
| GoF: creacionales, estructurales, de comportamiento | L32, L33, L34 | Completo |
| **Factory**, **Decorator**, **Observer** | L32, L33, L34 | Completo |
| Tipos de pruebas, **pirámide**, **cuadrantes**, mocks, **TDD / BDD** | L45, L19 | Completo |
| Novedades de **C# 8 a C# 14** y **C# 14** | L41 | Completo (comprobado con `LangVersion`) |

### Concurrencia, asincronía y reflexión

| Punto | Dónde | Estado |
|---|---|---|
| Concurrencia, hilos, *thread pool*, sincronización, **interbloqueos** | L43 | Completo (deadlock reproducido) |
| Llamadas síncronas frente a asíncronas, `async/await`, progreso, **`CancellationToken`** | L44 | Completo |
| Reflexión, atributos, parámetros de atributos | L42 | Completo |
| **Generación de código** («emitters») | L42 | Completo (IL emitido + generador Roslyn real) |

### Proyecto .NET y web

| Punto | Dónde | Estado |
|---|---|---|
| `Program`/*Startup*, **IoC**, **middleware**, variables de entorno y secciones, **secretos** | L30, L46, L47, L49, L50 | Completo |
| Cómo funciona la web, **HTTP/2** y **HTTP/3**, cookies, MVC, validación del modelo | L46 | **Parcial**: HTTP/2 ejecutado; **HTTP/3 no** |
| Arquitecturas: **N capas**, **Onion/Clean/Hexagonal**, **Vertical Slice** | L46, L47, L50 | Completo (con prueba de arquitectura) |

### Blazor, API y persistencia

| Punto | Dónde | Estado |
|---|---|---|
| Blazor Server frente a WebAssembly | L51 | Completo (medido) |
| Barra de navegación HTML/CSS, modelo, página, **Hot Reload**, **CRUD** | L51, L52 | Completo |
| **Minimal API**, **controladores**, consumir la API desde un cliente | L46, L47, L52 | Completo |
| `DbContext`, **Code First**, CRUD, LINQ, **migraciones** | L48, L8 | Completo |

### Lo que ese temario NO tiene y este curso sí (y viceversa)

**Este curso añade:** captura de datos y un CRUD de verdad (Parte II), seguridad (hash, JWT, roles), pruebas de integración y de arquitectura, **mutación manual** para validar las pruebas, escritorio con MVVM, **Unity con los límites reales de C# 9**, proyectos integradores, y **la medición honesta** de cada afirmación de rendimiento.

**Ese curso presencial aporta, y este no sustituye:** un **instructor** que resuelve dudas, **Visual Studio 2026 usado en directo**, trabajo en grupo, evaluación y certificación.

> **Sobre la contradicción del propio temario:** su título anuncia **C# 14** pero su descripción habla de «la versión estable de **C# 13**». En este curso **todo lo de C# 14 está comprobado con el compilador** (lección 41), no tomado de un folleto.

## Dónde leer la documentación oficial

| Para… | Dónde |
|---|---|
| Referencia de .NET y C# | **learn.microsoft.com/dotnet** y **learn.microsoft.com/dotnet/csharp** |
| Novedades de cada versión de C# | La sección **«What's new in C#»** de esa misma documentación |
| **Fechas de soporte** de cada versión de .NET | La página **«.NET Support Policy»** de Microsoft (cambia cada año) |
| Buscar un paquete | **nuget.org** |
| Código fuente de .NET | **github.com/dotnet** (`runtime`, `aspnetcore`, `efcore`) |

**Cómo se lee una página de la documentación:** (1) la **descripción** de arriba, (2) la tabla de **sobrecargas** (un mismo método con distintos parámetros), (3) **Remarks** (matices que no caben arriba), (4) los **ejemplos**, (5) **«Applies to»** (¿existe en mi versión?). **Si algo de aquí contradice la documentación de tu versión, manda la documentación.**

## Lecturas que recomienda el temario comercial

Los blogs de **Thomas Levesque** (trucos y buenas prácticas de C#) y de **Steve Smith «Ardalis»** (prácticas de desarrollo y tecnologías Microsoft), el canal de YouTube de **Microsoft Developer** y los foros de desarrolladores. *No se han revisado para este curso.*
