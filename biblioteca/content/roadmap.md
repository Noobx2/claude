# Roadmap: de programar a crear un modelo de lenguaje

Tu pregunta de fondo no es «qué lenguaje aprendo», sino **«en qué orden se construye el conocimiento, y por qué los que crearon Python, C o Apple supieron lo que sabían»**. Este documento responde a eso y cubre **tus dos metas**: (A) **entender y diseñar un modelo tipo GPT/Claude/Kimi** (sin laboratorio propio) con una idea defendible para trabajar en uno o buscar financiación, y (B) **programar herramientas de sistema y ciberseguridad de escritorio** (monitores, antivirus, acceso remoto, almacenes protegidos). Las fases 0–6 son **comunes**; después el camino se bifurca (§4).

> **Qué es cada etiqueta.** <span class="tag ok">VERIFICADO</span> lo comprobé con búsquedas web el 2026-10-29 (fuentes al final). <span class="tag mute">GENERAL</span> es conocimiento mío sin comprobar hoy; contrástalo antes de apostar dinero o años. <span class="tag warn">OPINIÓN</span> es mi recomendación, no un hecho.

## 0. Lo esencial en diez líneas

1. **El orden en que se *construyó* la informática (electrónica → hardware → ensamblador → lenguajes) no es el mejor orden para *aprenderla*.** Sirve para entender *por qué* existen las cosas.
2. <span class="tag warn">OPINIÓN</span> Aprende en **espiral**: programa pronto (motivación y resultados), y **baja** a los cimientos en fases con proyecto, no «antes de empezar».
3. Tu lista (electrónica, hardware, ensamblador, pseudocódigo, algoritmos, lógica, estructuras de datos) **es correcta como mapa de dependencias**, pero **la electrónica va casi al final y es opcional** para tu meta.
4. Los creadores de lenguajes **no empezaron «aprendiéndolo todo»**: tenían **un problema concreto** y dominaban **2–3 capas** (ver §3).
5. Para crear un LLM lo **imprescindible** es: programar bien, estructuras de datos y algoritmos, **álgebra lineal, cálculo y probabilidad**, y **aprendizaje profundo**. Ensamblador y electrónica **no** lo son.
6. Un modelo de frontera cuesta **decenas o cientos de millones**; pero **entender cómo se hace y entrenar uno pequeño de verdad** cuesta horas y decenas de dólares (§6).
7. «Diseñar el tuyo» sin laboratorio significa **una idea concreta, probada a pequeña escala, con resultados medibles**. Eso es lo que abre trabajo o financiación (§7).
8. Cada fase termina con un **proyecto y un criterio de «estoy listo»**. Sin eso, el plan es un deseo.
9. Estimación orientativa: **2 a 4 años a tiempo parcial** (8–10 h/semana) hasta nivel de «ingeniero de ML capaz de contribuir». Es mi estimación, no un dato.
10. El bucle que describes («empiezo de cero cada vez») se rompe con una regla: **revisar no es reiniciar** (§8).

## 0.1 Requisitos previos reales para empezar a programar

Pregunta directa: **¿qué necesitas saber de verdad antes de escribir tu primer programa?** Muy poco. La confusión viene de mezclar tres cosas distintas:

1. **Lo que hace posible la computación** (lógica, información, circuitos, máquinas).
2. **Lo que hace posible programar** (algoritmos, lenguajes).
3. **Lo que hace posible construir software profesional** (estructuras de datos, sistemas operativos, redes, bases de datos, pruebas, seguridad).

Entender el 1 **no es requisito** para hacer el 2 ni el 3. Es **una profundización**.

| Requisito | ¿Antes de programar? | Qué necesitas de verdad | Cuándo profundizar |
|---|---|---|---|
| Aritmética | **Sí** (nivel básico) | Operaciones, porcentajes, negativos, divisibilidad | — |
| Pensar en pasos | **Sí** | Descomponer un problema, definir entrada, proceso y salida | Siempre |
| Lógica de condiciones | **Sí, en las primeras semanas** | Verdadero/falso, `AND`, `OR`, `NOT`, comparaciones | Fase 2 (matemática discreta) |
| Álgebra | En paralelo | Variables, expresiones, ecuaciones sencillas | Fase 7 |
| Inglés técnico | **Sí** | Leer mensajes de error y documentación | Siempre |
| **Terminal** | **Desde el primer programa** | Moverte por carpetas, ejecutar, leer errores. **No** hace falta dominar Linux | Fases 0 y 4 |
| **Binario** | Introducción pronto | Qué es un bit y un byte; por qué `255` cabe en un byte | Fase 3 |
| **Hardware** | No para empezar | Saber que hay CPU, memoria y disco | Fase 3 |
| **Electrónica** | **No** (salvo microcontroladores) | Nada | Opcional; solo si vas a sistemas embebidos |
| Estructuras de datos | Después de los primeros programas | Listas, mapas, pilas, colas | Fase 2 |
| Lenguaje de programación | **Desde el principio** | Uno solo, con práctica diaria | Fase 1 |

> **Distinción clave:** algo puede ser **necesario para entender profundamente un ordenador** sin ser **requisito para programarlo**. Puedes escribir una API sin construir un transistor, igual que puedes conducir sin saber diseñar un motor.

## 0.2 ¿Quién «descubrió» la programación? De los cables a «programar este circuito»

No hubo una sola persona. Fue una cadena de ideas; cada eslabón respondió a «¿cómo le digo a una máquina qué hacer **sin tocarla**?».

| Año | Quién / qué | Idea clave | Etiqueta |
|---|---|---|---|
| 1804 | Telar de Jacquard | Tarjetas perforadas que **controlan** un telar: un patrón guardado en un soporte | <span class="tag mute">GENERAL</span> |
| 1830s–40s | **Charles Babbage**, Máquina Analítica | Diseño de una máquina de **propósito general** con memoria y unidad de cálculo | <span class="tag mute">GENERAL</span> |
| 1843 | **Ada Lovelace** | Notas con un **procedimiento paso a paso** para calcular números de Bernoulli y la idea de que una máquina puede manipular **símbolos**, no solo números | <span class="tag mute">GENERAL</span> |
| 1854 | George Boole | Álgebra de la **lógica** (verdadero/falso) | <span class="tag mute">GENERAL</span> |
| 1936 | **Alan Turing** | Modelo matemático de qué es **computable** y qué no | <span class="tag mute">GENERAL</span> |
| 1937 | Claude Shannon | Usó el álgebra de Boole para **diseñar circuitos de conmutación**: la lógica se convierte en electricidad | <span class="tag mute">GENERAL</span> |
| 1945–46 | **ENIAC** | **Programar era recablear:** seis mujeres (Betty Snyder Holberton, Jean Jennings Bartik, Marlyn Wescoff Meltzer, Ruth Lichterman Teitelbaum, Frances Bilas Spence y Kay McNulty) programaron la máquina **con cables e interruptores**, a partir de planos y sin manuales; un cálculo podía exigir miles de interruptores y cientos de cables | <span class="tag ok">VERIFICADO</span> |
| 1948–49 | **Programa almacenado** (Manchester «Baby» 1948, EDSAC 1949) | <span class="tag mute">GENERAL</span> El programa se guarda **en la memoria, como datos**: cambiar de programa **ya no exige recablear**. <span class="tag ok">VERIFICADO</span> Cuál fue «la primera» depende de cómo se defina; las fuentes no coinciden en la fecha |
| 1952 | **Grace Hopper**, primer compilador (A-0) | <span class="tag mute">GENERAL</span> Escribir en algo parecido al lenguaje humano y que una máquina lo **traduzca** |
| 1957–58 | FORTRAN, LISP | <span class="tag mute">GENERAL</span> Primeros lenguajes de alto nivel |

**Qué responde esto a «vamos a programar este circuito para automatizarlo»:**

1. Al principio, **programar era recablear** el circuito (ENIAC).
2. La gran idea fue guardar las **instrucciones como datos en memoria** (el «programa almacenado»): el mismo circuito sirve para **cualquier** programa.
3. Después llegó el **ensamblador** (nombres en vez de números), el **compilador** (lenguajes humanos) y, por último, lo que usas hoy.

**Ejemplo con un circuito:** una luz con sensor de presencia.

| Capa | Qué ocurre |
|---|---|
| Física | El sensor detecta movimiento |
| Electrónica | Genera una señal eléctrica |
| Representación digital | El microcontrolador la lee como 0 o 1 |
| Lógica | «Si hay presencia, luz encendida» |
| Algoritmo | Encender; reiniciar un temporizador; apagar si pasa el tiempo |
| Programación | Escribir esas reglas en C o en el lenguaje de la placa |
| Pruebas | Comprobar con distintas entradas y tiempos |

```text
INICIO
    LEER sensor
    SI hay presencia
        ENCENDER luz
        REINICIAR temporizador
    SI NO
        SI el temporizador ha vencido
            APAGAR luz
FIN
```

Ese pseudocódigo **no depende de ningún lenguaje**. Un algoritmo es una parte del sistema real, pero **no lo sustituye**: hay que decidir cómo se mide el tiempo, cómo se conecta la luz y qué medidas eléctricas y de seguridad requiere el circuito.

## 0.3 ¿Entonces el mejor lenguaje es siempre C o C++?

**No «siempre»**; depende de qué quieras comprender primero:

| Objetivo | Lenguaje | Por qué |
|---|---|---|
| Lógica y algoritmos sin mucha sintaxis | Python o pseudocódigo | Pocas ceremonias |
| Memoria, punteros y representación de datos | **C** | Modelo pequeño y casi sin abstracciones |
| Sistemas y rendimiento con abstracciones | **C++** (o **Rust**) | Control más herramientas modernas |
| Aplicaciones, APIs, Unity, escritorio de Windows | **C#** | Productivo; muy cercano a Java |
| Microcontroladores | **C** o C++ | Lo que soportan las placas |

**Un matiz importante:** C **no te enseña cómo funciona el hardware** por sí solo. `int x = 5;` se traduce a instrucciones de una arquitectura concreta, y C no te muestra registros ni circuitos. Para eso hay que **complementar** con arquitectura y ensamblador (Fase 3).

<span class="tag warn">OPINIÓN</span> **Dos escuelas válidas:** *de abajo arriba* (C primero, como hacen algunos cursos universitarios) o *de arriba abajo* (un lenguaje cómodo y bajar después). Para ti, con herramientas de sistema como meta B, propongo una **combinación**:

- **C# como lenguaje principal** para aplicaciones, interfaces, servicios y Unity (ya tienes el material).
- **C como segundo lenguaje, desde la Fase 3**, para memoria, punteros, ensamblador y cualquier cosa de bajo nivel.
- Más adelante, **C++ o Rust** solo si tu proyecto lo exige (motores de antivirus, controladores).

Así evitas el bucle de «volver a empezar con otro lenguaje».

## 1. Tu orden frente al que recomiendo

Tu orden: *electrónica → hardware → ensamblador → programación → pseudocódigo → algoritmos → lógica matemática → estructuras de datos.*

Es el orden **de dependencias físicas**. Como orden de estudio tiene tres problemas: tardas **años** en escribir algo útil, la motivación cae, y la mayoría de ese conocimiento **no se usa a diario** para tu meta.

| Paso | Tu orden | Orden recomendado <span class="tag warn">OPINIÓN</span> | Motivo |
|---|---|---|---|
| 1 | Electrónica | **Lógica y pseudocódigo** (pensar en pasos, condiciones, bucles) | Es lo que más se usa y se aprende en semanas |
| 2 | Hardware | **Programación real** en un lenguaje (C# o Python) | Resultados pronto; ya tienes bases |
| 3 | Ensamblador | **Estructuras de datos y algoritmos** + complejidad (Big O) | Es el 80 % de «pensar como programador» |
| 4 | Programación | **Cómo funciona un ordenador** (hardware lógico y ensamblador) | Ahora lo entiendes porque ya programas: explica *por qué* |
| 5 | Pseudocódigo | **Sistemas operativos, redes y bases de datos** | Todo programa real vive aquí |
| 6 | Algoritmos | **Lenguajes y compiladores** (hacer un intérprete) | Responde a «¿cómo se creó Python?» |
| 7 | Lógica matemática | **Matemáticas para ML** (álgebra lineal, cálculo, probabilidad) | Imprescindible para el LLM; se aprende en paralelo |
| 8 | Estructuras de datos | **Aprendizaje profundo y transformers** | Tu meta |
| 9 | — | **LLM en serio, investigación o producto** | Tu meta |
| — | — | Electrónica / física de circuitos | **Opcional**: solo si te apasiona; no cambia lo que puedes construir con LLM |

**Regla de oro:** estudia lo que *explica lo que ya usas*. La electrónica explica algo que usas, pero **a un nivel que no necesitas** para programar ni para IA.

## 2. El mapa de capas

Cada capa se apoya en la anterior. Para cada **meta**, los símbolos indican cuánto importa: **●** imprescindible, **◐** útil, **○** opcional.

| Capa | Qué es | Programar bien | Crear un lenguaje | Crear un LLM |
|---|---|---|---|---|
| 1 · Física y electrónica | Transistores, voltajes | ○ | ○ | ○ |
| 2 · Lógica digital | Puertas AND/OR/NOT, álgebra de Boole, memoria, ALU | ◐ | ◐ | ○ |
| 3 · Arquitectura del procesador | CPU, registros, caché, memoria, instrucciones | ◐ | ● | ◐ (para optimizar GPU) |
| 4 · Ensamblador y código máquina | Instrucciones del procesador | ◐ | ● | ○ |
| 5 · Sistema operativo | Procesos, memoria, archivos, hilos | ● | ● | ◐ |
| 6 · Lenguajes y compiladores | Cómo se traduce y ejecuta el código | ● | ● | ◐ |
| 7 · Estructuras de datos y algoritmos | Listas, árboles, grafos, ordenación, Big O | ● | ● | ● |
| 8 · Ingeniería de software | Pruebas, Git, diseño, bases de datos, redes | ● | ◐ | ● |
| 9 · Sistemas distribuidos | Muchas máquinas trabajando juntas | ◐ | ○ | ● (entrenar a escala) |
| 10 · Matemáticas para ML | Álgebra lineal, cálculo, probabilidad, información | ○ | ○ | ● |
| 11 · Aprendizaje automático y profundo | Redes neuronales, optimización | ○ | ○ | ● |
| 12 · Transformers y LLM | Atención, preentrenamiento, ajuste, evaluación | ○ | ○ | ● |
| 13 · Computación en GPU | CUDA, paralelismo, memoria de GPU | ○ | ○ | ◐–● |

<span class="tag warn">OPINIÓN</span> Fíjate en la columna de LLM: **las capas 1 y 4 son opcionales**. Eso es lo que más cambia tu plan.

## 3. Por qué crearon lo que crearon, y qué sabían

Patrón general: **cada creación resolvió un dolor concreto del creador**, y **se apoyó en algo anterior**.

| Creador | Qué creó | Problema que resolvía | Qué sabía antes | Lección |
|---|---|---|---|---|
| **Steve Wozniak** («Woz») | Apple I y II (1976–77); el BASIC del Apple II | Hacer un ordenador barato y usable para aficionados | Electrónica autodidacta desde joven, diseño con chips; <span class="tag ok">VERIFICADO</span> escribió el **Integer BASIC en ensamblador del 6502 y lo convirtió a código máquina a mano**, sin ensamblador (decía que no podía pagar uno de ~20 $) | Dominaba **hardware y software a la vez**; pero eso fue en 1976, con ordenadores de miles de transistores, no millones de millones |
| **Dennis Ritchie** | C (≈1972–73), con Ken Thompson para reescribir Unix | Escribir un sistema operativo sin depender del ensamblador de cada máquina | <span class="tag mute">GENERAL</span> Física y matemática aplicada en Harvard; trabajo previo con el lenguaje B (de Thompson) y BCPL | Un lenguaje **de sistemas**: portátil pero cerca del hardware |
| **Guido van Rossum** | Python (empezó en dic. 1989; primera versión pública feb. 1991) | <span class="tag ok">VERIFICADO</span> Quería un lenguaje que combinara la sencillez del *shell* de Unix, la productividad de **ABC** (donde había trabajado en el CWI), la potencia de C y la modularidad de Modula-2; lo empezó como **proyecto de ocio en Navidad** | Trabajó en **ABC** (un lenguaje para enseñar) y en sistemas distribuidos; <span class="tag mute">GENERAL</span> estudió matemáticas e informática | Un lenguaje nuevo **mezcla ideas de otros** y nace de **usar** lenguajes durante años |
| **Anders Hejlsberg** | Turbo Pascal (1983), Delphi, C# (2000–02), TypeScript | Compiladores rápidos y entornos productivos | <span class="tag ok">VERIFICADO</span> Estudió ingeniería eléctrica en la Universidad Técnica de Dinamarca (no he podido confirmar si terminó); <span class="tag mute">GENERAL</span> escribió compiladores desde muy joven | **Compiladores** como oficio: hizo lo mismo durante 40 años |
| **James Gosling** | Java (1991–95) | <span class="tag mute">GENERAL</span> Programar dispositivos con distintos procesadores («escribe una vez, ejecuta en cualquier sitio») | <span class="tag mute">GENERAL</span> Doctorado en informática; había escrito editores y sistemas de ventanas | La **máquina virtual** como respuesta a la portabilidad |
| **Brendan Eich** | JavaScript (1995, en pocos días) | <span class="tag mute">GENERAL</span> Un lenguaje sencillo para páginas web en Netscape | <span class="tag mute">GENERAL</span> Sistemas, redes y conocimiento de Scheme | Lo que importa **no es el tiempo de diseño**, sino **qué ecosistema lo adopta** |
| **Bjarne Stroustrup** | C++ (desde 1979, «C con clases») | <span class="tag mute">GENERAL</span> Quería la abstracción de Simula con la velocidad de C | <span class="tag mute">GENERAL</span> Simula (clases) y doctorado en Cambridge | Combinar **dos ideas buenas** de lenguajes existentes |
| **Linus Torvalds** | Linux (1991), Git (2005) | <span class="tag mute">GENERAL</span> Tener un sistema tipo Unix en su PC y, después, control de versiones adecuado para el kernel | <span class="tag mute">GENERAL</span> Estudiaba en Helsinki; aprendió de Minix y de C/ensamblador x86 | De **rascarse la propia picazón** salen los proyectos que duran |
| **Grace Hopper / John Backus / John McCarthy** | Primeros compiladores / FORTRAN (1957) / LISP (1958) | <span class="tag mute">GENERAL</span> Dejar de programar en código máquina; expresar matemáticas y símbolos | <span class="tag mute">GENERAL</span> Matemáticas | **Abstracción**: cada generación cubre la anterior |

**Lo que se repite** <span class="tag warn">OPINIÓN</span>:

1. Casi todos tenían **un problema real** y **una herramienta anterior que les quedaba corta**.
2. Dominaban **dos o tres capas contiguas** (hardware + ensamblador; lenguajes + compiladores; sistemas + lenguajes), **no todas**.
3. Casi todos **usaron mucho** el sistema anterior antes de crear el siguiente.
4. Muchos empezaron como **proyecto personal** (Python en Navidad; Linux como afición).
5. El éxito depende tanto de **los usuarios y del ecosistema** como del diseño técnico.

**Para ti:** si quieres «entender cómo se creó Python», **escribe tu propio intérprete** (Fase 5). Es la forma más honesta de hacerlo.

## 4. Roadmap por fases

**Fases 0–6: comunes. Fases 7–10: rama A (IA y LLM). Rama B (seguridad y sistemas): ver §4B.**

Horas = estimación orientativa de trabajo real (<span class="tag warn">OPINIÓN</span>). A 8–10 h/semana: **120 h ≈ 3 meses**. Cada fase tiene **proyecto** y **criterio de salida**: no pases a la siguiente sin cumplirlo.

### Fase 0 · Herramientas y hábitos (≈25 h)
- **Qué:** terminal, editor, Git, escribir y ejecutar un programa, leer errores.
- **Proyecto:** un repositorio con README y tres scripts pequeños.
- **Listo cuando:** puedes crear un proyecto desde cero, ejecutarlo y subirlo a Git **sin mirar apuntes**.
- **Tu biblioteca:** módulos de terminal, Git y PATH.

### Fase 1 · Programar de verdad (≈120 h)
- **Qué:** un lenguaje principal (**C#** es buena elección para ti: sirve para Unity, APIs y escritorio). Variables, control de flujo, funciones, clases, colecciones, archivos, errores, pruebas.
- **Proyecto:** el **inventario** (consola + archivo + ventana) y, después, una **API**.
- **Listo cuando:** escribes un programa de 300 líneas con pruebas **sin copiar**.
- **Tu biblioteca:** el curso de C# y los módulos de C#/Java sin frameworks.

### Fase 2 · Pensar con algoritmos y datos (≈120 h)
- **Qué:** pseudocódigo, lógica, **Big O**, listas, pilas, colas, mapas hash, árboles, grafos, recursión, ordenación y búsqueda, programación dinámica básica.
- **Proyecto:** **implementa tú** lista enlazada, tabla hash, árbol binario de búsqueda y Dijkstra. Resuelve ~100 problemas.
- **Listo cuando:** puedes explicar por qué un `Dictionary` es O(1) y cuándo no, y resolver un problema medio en 45 min.
- **Libros:** *Introduction to Algorithms* (CLRS), *Algorithms* (Sedgewick), *The Algorithm Design Manual* (Skiena). <span class="tag mute">GENERAL</span>

### Fase 3 · Cómo funciona un ordenador (≈140 h)
- **Qué:** puertas lógicas → memoria → **CPU** → **ensamblador** → máquina virtual → compilador → sistema operativo, **construyéndolos**.
- **Proyecto:** **Nand2Tetris** (construyes un ordenador desde puertas NAND hasta el Tetris). <span class="tag mute">GENERAL</span>
- **Complementos:** *Code* (Petzold) para la historia; *Computer Systems: A Programmer's Perspective* para ensamblador x86-64 y memoria.
- **Listo cuando:** puedes explicar qué pasa, paso a paso, desde `a = b + c` hasta las instrucciones del procesador.
- **Aquí cubres** lo de «electrónica, hardware y ensamblador» **sin años de física**.

### Fase 4 · Sistemas operativos, redes y datos (≈120 h)
- **Qué:** procesos, hilos, memoria virtual, archivos; TCP/IP, HTTP, DNS; SQL y cómo funciona una base de datos por dentro.
- **Proyecto:** un **servidor HTTP** pequeño, un **shell** sencillo y un **almacén clave-valor** persistente.
- **Libro:** *Operating Systems: Three Easy Pieces* (gratuito en línea). <span class="tag mute">GENERAL</span>
- **Tu biblioteca:** módulos de Redes, SQL y el Anexo de bases de datos.

### Fase 5 · Lenguajes y compiladores (≈80 h)
- **Qué:** léxico, gramática, árbol sintáctico, intérprete, máquina virtual.
- **Proyecto:** **tu propio lenguaje** (un intérprete con variables, funciones y bucles). Libro: *Crafting Interpreters* (gratuito en línea). <span class="tag mute">GENERAL</span>
- **Listo cuando:** tu lenguaje ejecuta una función recursiva y un bucle.
- **Responde a:** «¿cómo hizo Guido van Rossum Python?».

### Fase 6 · Ingeniería de software (≈80 h)
- **Qué:** pruebas, arquitectura (capas, hexagonal), despliegue, observabilidad, seguridad básica.
- **Proyecto:** una aplicación con API, base de datos, pruebas, Docker y CI.
- **Tu biblioteca:** Docker, Spring/ASP.NET, Hardening, CI/CD en el diccionario.

### Fase 7 · Matemáticas para ML (≈100 h, **en paralelo** desde la Fase 2)
- Ver §5. No esperes a «saber todo»: aprende cada tema **cuando lo vayas a usar**.

### Fase 8 · Aprendizaje automático y profundo (≈160 h)
- **Qué:** regresión, clasificación, sobreajuste, validación, redes neuronales, **retropropagación**, optimizadores (SGD, Adam), regularización.
- **Proyecto:** la serie de Karpathy: **micrograd** (retropropagación desde cero), **makemore** (modelo de lenguaje de caracteres) y **nanoGPT**. <span class="tag ok">VERIFICADO</span> Es una serie gratuita de vídeos en la que se programa y entrena redes neuronales desde cero hasta llegar a un GPT.
- **Complementos:** curso de ML de Andrew Ng, fast.ai, *Dive into Deep Learning*. <span class="tag mute">GENERAL</span>
- **Listo cuando:** implementas retropropagación **a mano** y entrenas un modelo de caracteres que genera texto.

### Fase 9 · Modelos de lenguaje en serio (≈200 h)
- **Qué:** el artículo del **Transformer** (2017), **tokenización** (BPE), preentrenamiento, leyes de escala, ajuste fino (SFT, LoRA), alineamiento (RLHF/DPO), evaluación, inferencia (caché KV, cuantización), GPU y entrenamiento distribuido.
- **Proyecto:** **reproduce un GPT-2 pequeño** y **ajusta un modelo abierto** con tus datos (en español, por ejemplo).
- **Listo cuando:** puedes explicar y medir cada pieza de un LLM y has **entrenado uno pequeño de principio a fin**.

### Fase 10 · Contribución original (abierta)
- **Qué:** una **idea concreta** probada con experimentos: un conjunto de datos, un método de evaluación, un ajuste para una lengua, una arquitectura pequeña, una mejora de eficiencia.
- **Entregable:** repositorio reproducible + informe corto con resultados. **Esto es lo que se enseña a quien contrata o financia** (§7).

## 4B. Rama B: sistemas y ciberseguridad de escritorio

Esta rama sigue a las fases 0–6 y **sustituye** a las 8–10 si tu meta es construir herramientas como **un monitor de archivos, un antivirus, un acceso remoto tipo TeamViewer o un almacén protegido con alertas**. Aquí las fases 3, 4 y 5 **dejan de ser opcionales**.

### Lo que queda por construir (y en qué orden)

| Paso | Qué aprendes | Proyecto de salida |
|---|---|---|
| **B1 · C y memoria** | Punteros, memoria, compilación, depuración (`gdb`) | Un `ls` y un `cat` propios; una pequeña tabla hash en C |
| **B2 · Arquitectura y ensamblador** | CPU, registros, pila, llamadas, x86-64 | Leer el ensamblador de tus programas en C y explicarlo |
| **B3 · Internos del sistema operativo** | Procesos, hilos, permisos, descriptores/*handles*, servicios; en Windows: tokens, ACL, API Win32 | Listar procesos con su usuario y ejecutable en Linux y Windows |
| **B4 · Criptografía aplicada** | Cifrado autenticado (AES-GCM), derivación de claves, gestión de claves | Un contenedor cifrado de archivos con **bibliotecas existentes** (nunca tu propio algoritmo) |
| **B5 · Monitorización** | Eventos de archivos y procesos, auditoría, registros | Un monitor con alertas (ver el prototipo de abajo) |
| **B6 · Aislamiento** | Máquinas virtuales, contenedores, mecanismos de aislamiento por virtualización, controladores de archivos | Un almacén dentro de una **VM** con el monitor fuera |
| **B7 · Proyecto integrador** | Interfaz, servicio, almacén y monitor juntos | Tu panel con alertas |

### Primero, define contra quién proteges (modelo de amenazas)

«Que ni Windows pueda verlo» puede significar cuatro cosas **muy distintas**. Elige una antes de diseñar nada.

| Nivel | ¿Contra quién? | Mecanismos típicos | ¿Realista en solitario? |
|---|---|---|---|
| **1** | Otros programas y usuarios sin privilegios (incluida una IA local que corra como programa normal) | Cifrar los datos; permisos del sistema; la clave **no** guardada en disco | **Sí** |
| **2** | Un **administrador** de Windows | Aislar en una **máquina virtual** o sistema aparte; claves fuera del sistema (llave o token) | **Sí**, con esfuerzo |
| **3** | Un Windows **comprometido a nivel de kernel** | Aislamiento respaldado por **virtualización o hardware**; otro dispositivo | **Se apoya en funciones de la plataforma**; no se programa desde cero |
| **4** | Alguien con **acceso físico** al equipo apagado | Cifrado de disco completo, TPM con PIN, arranque seguro | Se configura; no se reinventa |

<span class="tag mute">GENERAL</span> Una **carpeta normal** protegida con permisos **no** es una barrera frente a un administrador o un kernel comprometido. Cifrar protege el **contenido**, pero no oculta necesariamente que **existe** ni los metadatos.

<span class="tag warn">OPINIÓN</span> **Sobre «que la IA local no pueda acceder»:** una IA local es **un proceso más**. Si el contenido está **cifrado** y la clave **no está** en el equipo (o solo se carga mientras tú lo desbloqueas), no puede leerlo. Si lo desbloqueas **mientras la IA corre bajo tu mismo usuario**, sí podría leer la vista abierta: por eso el diseño robusto separa **usuario o máquina**, no solo carpeta.

### Prototipo ejecutado: ¿qué proceso toca mi carpeta? (Linux, `fanotify`)

Pides «que el monitor me avise de qué aplicación intenta ver la carpeta». En **Linux** se puede hacer sin escribir un controlador con `fanotify`. Este programa en C **vigila una carpeta y muestra el proceso y el ejecutable** que abre, lee o escribe sus archivos:

```c
// Monitor de acceso a una carpeta con fanotify (Linux).
// Muestra QUÉ proceso (pid y ejecutable) abre, lee o modifica archivos de la carpeta vigilada.
// Compilar:  gcc -Wall -Wextra -O2 -o watch watch.c
// Ejecutar:  sudo ./watch /ruta/a/la/carpeta      (fanotify exige privilegios de administrador)
#define _GNU_SOURCE
#include <errno.h>
#include <fcntl.h>
#include <limits.h>
#include <signal.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/fanotify.h>
#include <time.h>
#include <unistd.h>

static volatile sig_atomic_t running = 1;
static void stop(int sig) { (void)sig; running = 0; }

// Convierte la máscara de eventos en texto legible. El kernel puede FUSIONAR varios eventos
// (mismo proceso y mismo archivo) en uno solo, por eso la máscara puede traer varios bits a la vez.
static void describe(unsigned long long mask, char *out, size_t size) {
    out[0] = '\0';
    if (mask & FAN_OPEN)        strncat(out, "ABRIR ", size - strlen(out) - 1);
    if (mask & FAN_ACCESS)      strncat(out, "LEER ", size - strlen(out) - 1);
    if (mask & FAN_MODIFY)      strncat(out, "ESCRIBIR ", size - strlen(out) - 1);
    if (mask & FAN_CLOSE_WRITE) strncat(out, "CERRAR ", size - strlen(out) - 1);
}

int main(int argc, char **argv) {
    if (argc != 2) { fprintf(stderr, "uso: %s <carpeta>\n", argv[0]); return 2; }
    signal(SIGINT, stop);
    signal(SIGTERM, stop);

    // 1) Crear el "grupo" de notificaciones. CLASS_NOTIF = solo observar (no bloquear)
    int fan = fanotify_init(FAN_CLOEXEC | FAN_CLASS_NOTIF, O_RDONLY | O_LARGEFILE);
    if (fan < 0) { perror("fanotify_init (¿eres root?)"); return 1; }

    // 2) Marcar la carpeta: eventos de ella y de sus hijos
    unsigned long long mask = FAN_OPEN | FAN_ACCESS | FAN_MODIFY | FAN_CLOSE_WRITE | FAN_EVENT_ON_CHILD;
    if (fanotify_mark(fan, FAN_MARK_ADD, mask, AT_FDCWD, argv[1]) < 0) { perror("fanotify_mark"); return 1; }
    fprintf(stderr, "vigilando %s (Ctrl+C para salir)\n", argv[1]);

    char buffer[8192];
    while (running) {
        ssize_t length = read(fan, buffer, sizeof buffer);
        if (length < 0) { if (errno == EINTR) continue; perror("read"); break; }

        // 3) Recorrer los eventos recibidos
        for (struct fanotify_event_metadata *ev = (struct fanotify_event_metadata *)buffer;
             FAN_EVENT_OK(ev, length); ev = FAN_EVENT_NEXT(ev, length)) {
            if (ev->vers != FANOTIFY_METADATA_VERSION) { fprintf(stderr, "versión de fanotify inesperada\n"); return 1; }
            if (ev->fd < 0) continue;                                  // desbordamiento de cola

            char fd_path[64], file[PATH_MAX], exe_path[64], exe[PATH_MAX];
            // El evento trae un descriptor ya abierto: /proc/self/fd/N nos dice qué archivo es
            snprintf(fd_path, sizeof fd_path, "/proc/self/fd/%d", ev->fd);
            ssize_t n = readlink(fd_path, file, sizeof file - 1);
            file[n > 0 ? n : 0] = '\0';
            // Y /proc/<pid>/exe nos dice qué programa lo hizo
            snprintf(exe_path, sizeof exe_path, "/proc/%d/exe", ev->pid);
            n = readlink(exe_path, exe, sizeof exe - 1);
            if (n > 0) exe[n] = '\0'; else strcpy(exe, "(ya terminó)");

            char what[64];
            describe(ev->mask, what, sizeof what);
            time_t now = time(NULL);
            char when[32];
            strftime(when, sizeof when, "%H:%M:%S", localtime(&now));
            printf("%s  pid=%-6d %-22s %s  <-  %s\n", when, ev->pid, what, strrchr(file, '/') ? strrchr(file, '/') + 1 : file, exe);
            fflush(stdout);
            close(ev->fd);                                             // IMPORTANTE: cada evento abre un descriptor
        }
    }
    close(fan);
    return 0;
}
```

Compilación y uso, con cada pieza explicada:

```bash
gcc -Wall -Wextra -O2 -o watch watch.c
sudo ./watch /ruta/a/la/carpeta
```

| Pieza | Significado |
|---|---|
| `gcc` | Compilador de C |
| `-Wall -Wextra` | Activa casi todos los avisos del compilador |
| `-O2` | Optimización de nivel 2 |
| `-o watch` | Nombre del ejecutable resultante |
| `sudo` | `fanotify` exige privilegios de administrador |

<span class="tag ok">EXPERIMENTO</span> Prueba real: con la carpeta `secreto/` vigilada, ejecuté `cat`, una línea de Python, un `echo >>` y un `sha256sum` sobre `nota.txt`. Resultado:

```text
11:43:00  pid=1967   ABRIR LEER             nota.txt  <-  (ya terminó)
11:43:00  pid=1968   ABRIR                  nota.txt  <-  /usr/bin/python3.13
11:43:00  pid=1968   LEER                   nota.txt  <-  /usr/bin/python3.13
11:43:00  pid=1956   ABRIR ESCRIBIR CERRAR  nota.txt  <-  /usr/bin/bash
11:43:00  pid=1970   ABRIR LEER             nota.txt  <-  (ya terminó)
```

**Cómo se lee:** cada fila es un acceso, con su hora, `pid`, la acción y el programa. Se distinguen el `python3` que abrió y leyó, y el `bash` que escribió con `echo >>`.

**Los límites que muestra el propio experimento** (lo más importante del prototipo):

1. **`(ya terminó)`:** `cat` y `sha256sum` acabaron **antes** de que el monitor pudiera consultar qué ejecutable eran. Es una **condición de carrera**: el proceso desaparece entre el aviso y la consulta. Un monitor serio lo resuelve con modos que **retienen** el acceso hasta decidir (permisos de `fanotify`), con auditoría del sistema (`auditd`) o con **eBPF**.
2. **Eventos fusionados:** el kernel agrupa eventos del mismo proceso y archivo en uno (`ABRIR LEER`).
3. **Es observar, no bloquear.** Impedir el acceso exige otro modo de `fanotify` o permisos del sistema.
4. **Solo ve lo que ocurre en esa carpeta y en ese sistema.** No detecta a un atacante con control del kernel.

### El equivalente en Windows

<span class="tag ok">VERIFICADO</span> (fuentes de terceros; contrasta con la documentación de Microsoft) En Windows hay una vía **sin escribir controladores**: **auditoría del sistema de archivos**. Se necesitan **tres cosas** para que aparezca el evento **4663** («se intentó acceder a un objeto») en el registro de seguridad:

1. Activar la directiva **Auditar sistema de archivos** (en una consola de administrador: `auditpol /set /subcategory:"File System" /success:enable /failure:enable`).
2. Añadir a la carpeta una **SACL** (lista de auditoría) en *Seguridad → Opciones avanzadas → Auditoría*.
3. Que el tipo de acceso coincida con lo auditado (lectura, escritura…).

Después, tu aplicación C# **lee el registro de eventos** y muestra el panel. <span class="tag mute">GENERAL</span> Para **bloquear** accesos en tiempo real los antivirus usan **controladores de filtro de sistema de archivos (*minifilters*)**: código en **modo kernel**, con requisitos de firma y riesgo de pantallazos azules. **No es el primer paso.**

### Dos advertencias de oficio

- **Herramientas defensivas, en sistemas tuyos o autorizados.** Los programas de **acceso remoto** (como TeamViewer) y los **monitores** son de **doble uso**: construye lo que te proteja a ti. No añadas funciones de ocultación o persistencia sin consentimiento.
- **No inventes criptografía.** Usa bibliotecas probadas. Los fallos casi nunca están en el algoritmo, sino en cómo se gestionan las claves.

### Camino corto si solo quieres *usar* LLM en productos
<span class="tag warn">OPINIÓN</span> Fases 0–2, 4 (parcial), 6 y una versión corta de la 8–9 (APIs, RAG, agentes, evaluaciones). **Te ahorras las fases 3, 5 y gran parte de las matemáticas.** Esto es empleable en 12–18 meses; **no** te lleva a diseñar modelos.

## 5. Matemáticas y lógica: qué y para qué

No hace falta una carrera de matemáticas, pero **sí** estos bloques, y **conviene aprenderlos con código**:

| Bloque | Qué incluye | Dónde se usa en un LLM | Cuándo estudiarlo |
|---|---|---|---|
| **Lógica y matemática discreta** | Proposiciones, conjuntos, combinatoria, grafos | Algoritmos, estructuras de datos | Fase 2 |
| **Álgebra lineal** | Vectores, matrices, producto matricial, valores propios | **Todo**: cada capa es una multiplicación de matrices; los *embeddings* son vectores | Fase 7 (antes de la 8) |
| **Cálculo** | Derivadas, regla de la cadena, gradiente | **Retropropagación y entrenamiento** | Fase 7 |
| **Probabilidad** | Distribuciones, esperanza, Bayes, muestreo | El modelo **predice una distribución** sobre el siguiente *token* | Fase 7 |
| **Estadística** | Estimación, sobreajuste, validación, significación | **Evaluar** si un resultado es real | Fase 7 |
| **Teoría de la información** | Entropía, entropía cruzada, divergencia KL | La **función de pérdida** del entrenamiento es la entropía cruzada | Fase 8 |
| **Optimización** | Descenso del gradiente, SGD, Adam, tasas de aprendizaje | **Cómo aprende** el modelo | Fase 8 |

**Recursos** <span class="tag mute">GENERAL</span>: *Mathematics for Machine Learning* (Deisenroth et al., gratuito), los vídeos de 3Blue1Brown sobre álgebra lineal, cálculo y redes neuronales, Khan Academy.

**Criterio de «lo sé»:** puedes derivar a mano el gradiente de una neurona con activación y pérdida, y comprobarlo con código.

## 6. Anatomía de un LLM y qué puedes hacer tú

| Pieza | Qué es | Cómo se hace en grande | Qué puedes hacer tú |
|---|---|---|---|
| **Datos** | Billones de *tokens* de texto, filtrados y sin duplicados | Rastreo web, filtrado de calidad, mezcla de fuentes | Conjuntos abiertos (p. ej. FineWeb); tus propios datos limpios y de calidad |
| **Tokenizador** | Trocea el texto en *tokens* (BPE) | Se entrena sobre el corpus | Entrenar uno propio: un buen ejercicio de Fase 9 |
| **Arquitectura** | *Embeddings* + bloques Transformer (atención + MLP + normalización + residuales) | Cientos de capas, miles de millones de parámetros | Implementar uno pequeño (nanoGPT) |
| **Preentrenamiento** | Predecir el siguiente *token* (entropía cruzada) | Miles de GPU durante semanas | Entrenar un modelo pequeño en una o pocas GPU |
| **Posentrenamiento** | SFT, RLHF/DPO para que obedezca y sea seguro | Datos humanos y de preferencias a gran escala | **Ajuste fino con LoRA** de un modelo abierto: accesible |
| **Evaluación** | Pruebas y *benchmarks* | Cientos de evaluaciones | **Diseñar buenas evaluaciones es un hueco real**; accesible y valioso |
| **Servicio (inferencia)** | Responder rápido y barato | Lotes, caché KV, cuantización, GPU | Ejecutar modelos locales (llama.cpp, vLLM) |
| **Seguridad y alineamiento** | Que el modelo no cause daño | Equipos enteros, *red teaming* | Investigación abierta, evaluaciones, interpretabilidad |

**Costes reales:**

| Qué | Orden de magnitud |
|---|---|
| Reproducir **GPT-2 (124 M parámetros)** con llm.c de Karpathy | <span class="tag ok">VERIFICADO</span> ≈ 90 minutos en 8 GPU A100 y unos **20 $** en la nube (2024) |
| Una sola GPU | <span class="tag ok">VERIFICADO</span> entre 4 y 24 horas según la GPU |
| GPT-2 de 1,5 B parámetros | <span class="tag ok">VERIFICADO (fuente secundaria)</span> ≈ 24 h en un nodo de 8 H100, ≈ 670 $ |
| Modelos de **frontera** | <span class="tag mute">GENERAL</span> decenas o cientos de millones de dólares, según declaraciones públicas: no es tu camino |

<span class="tag warn">OPINIÓN</span> **No intentes igualar a un laboratorio. Intenta entender cada pieza y aportar algo en una de ellas.**

## 7. Trabajo y financiación: lo realista

### Roles que existen
<span class="tag mute">GENERAL</span> (los títulos varían por empresa)

| Rol | Qué hace | Qué suelen pedir |
|---|---|---|
| **Research Scientist** | Propone y prueba ideas nuevas, publica | Doctorado frecuente o publicaciones equivalentes |
| **Research Engineer** | Implementa y escala experimentos; une investigación e ingeniería | **Ingeniería muy sólida** + entender el aprendizaje profundo; a menudo **sin doctorado** |
| **ML / Applied Engineer** | Construye productos con modelos | Software + ML aplicado |
| **MLOps / Infra** | Entrenamiento y servicio a escala | Sistemas distribuidos, GPU, Kubernetes |
| **Evaluación / seguridad** | Mide y reduce riesgos | Estadística, ingeniería, criterio |

### Qué enseñar (portafolio)
<span class="tag warn">OPINIÓN</span>

1. **Reproducciones**: reproducir un resultado conocido y comparar cifras honestamente.
2. **Un experimento propio con resultados** (aunque sea pequeño y negativo): conjunto de datos, evaluación o ablación.
3. **Código limpio, abierto y reproducible** (README, semillas, tablas).
4. **Un informe corto** que cualquiera pueda leer.
5. **Contribuciones a proyectos abiertos** (Hugging Face, llama.cpp, PyTorch…).

### Fuentes de financiación y apoyo
<span class="tag mute">GENERAL</span> **Verifica convocatorias y condiciones vigentes**, cambian cada año.

- **Ayudas públicas** de innovación y empresas emergentes en tu país y a nivel europeo.
- **Aceleradoras** de empresas tecnológicas.
- **Créditos de cómputo** de programas de investigación o de startups de algunos proveedores.
- **Patrocinio de proyectos abiertos.**
- <span class="tag ok">VERIFICADO</span> **Ecosistema español:** **ALIA**, infraestructura pública, abierta y multilingüe de IA coordinada por el **Barcelona Supercomputing Center**, entrenada en MareNostrum 5 en español y las cuatro lenguas cooficiales; en diciembre de 2025 se publicó el modelo ALIA 40B Instruido. Es un ecosistema donde **hay espacio para quien sepa de datos, evaluación y ajuste en español**.

### Qué hace defendible «tu idea»
<span class="tag warn">OPINIÓN</span>

| Débil | Fuerte |
|---|---|
| «Un GPT mejor» | «Un modelo de **X** (lengua, dominio) con evaluación **Y** que supera a **Z** en **N** puntos» |
| «Entrenar un modelo enorme» | «**Probé a pequeña escala** que el método funciona y **estimo** cómo escala» |
| Usar una API con otro nombre | **Dato, método o evaluación propios** que otros no tienen |
| Promesas | **Resultados reproducibles** y una demostración |

## 8. Cómo evitar el bucle de olvidar y empezar de cero

Tu bucle (Java → olvido → C# → olvido → Unity → ...) es una espiral de refuerzo: **cambiar de lenguaje antes de consolidar** → **no consolidar** → **sentir que no sabes nada** → **cambiar**. Rómpelo así:

1. **Un lenguaje principal durante 12 meses** (<span class="tag warn">OPINIÓN</span> C#). **No «vuelvas a Java»**: si lo necesitas, lees la tabla Java → C# (Java y C# comparten más del 80 % de las ideas).
2. **Revisar no es reiniciar.** Vuelve a un capítulo, rehaz **un ejercicio** y sigue. No repitas todo.
3. **Todo lo que estudies, lo escribes** (tu propia observación: «solo se me queda si lo escribo»). Una página por tema, con **código que has ejecutado**.
4. **Un proyecto por fase** (§4) y un criterio de salida.
5. **Máximo dos frentes a la vez**: un lenguaje y un tema de base (por ejemplo, C# + Nand2Tetris).
6. **Repaso espaciado:** empieza cada sesión rehaciendo el último ejercicio.
7. **Unity ahora no es una desviación:** es un **proyecto de la Fase 1** para practicar C#. Hazlo, pero con **lógica separada en clases normales** (ver el módulo «C#: análisis, bucles y mapa»).
8. **Sesión mínima:** 30 minutos al día valen más que 5 horas el domingo.

## 9. Recursos por fase

<span class="tag mute">GENERAL</span> Comprueba disponibilidad y ediciones; los enlaces cambian.

| Fase | Recursos |
|---|---|
| 0–1 | Esta biblioteca; documentación de Microsoft Learn (C#) |
| 2 | CLRS, Sedgewick, Skiena; plataformas de problemas (LeetCode, Exercism, Advent of Code) |
| 3 | **Nand2Tetris** (libro y curso), *Code* (Petzold), *CS:APP* |
| 4 | *Operating Systems: Three Easy Pieces*; *Computer Networking: A Top-Down Approach* |
| 5 | *Crafting Interpreters* (Nystrom) |
| 6 | *A Philosophy of Software Design* (Ousterhout), *Designing Data-Intensive Applications* (Kleppmann) |
| 7 | *Mathematics for Machine Learning*; 3Blue1Brown; Khan Academy |
| 8 | Andrew Ng (ML), fast.ai, **Karpathy: Neural Networks: Zero to Hero**, *Dive into Deep Learning* |
| 9 | Artículo «Attention Is All You Need» (2017); *Build a Large Language Model (From Scratch)* (Raschka); nanoGPT y llm.c de Karpathy; Hugging Face |
| GPU | *Programming Massively Parallel Processors*; documentación de CUDA |

## 10. Lo que no sé o no comprobé

- Las **estimaciones de horas y años** son mías, no datos.
- Los **requisitos de empleo** y las **convocatorias de financiación** cambian; no los comprobé.
- Los datos de creadores marcados **GENERAL** vienen de mi conocimiento, no de una búsqueda de hoy.
- No sé si Hejlsberg terminó la carrera: **solo** se confirma que estudió ingeniería eléctrica en la Universidad Técnica de Dinamarca.
- Los costes de entrenamiento de modelos de frontera son **órdenes de magnitud**.
- Los mecanismos de aislamiento de Windows (virtualización, enclaves, controladores) están descritos aquí a nivel general; **no los probé**.
- El prototipo de `fanotify` **solo se ejecutó en Linux**; la rama de Windows (auditoría y eventos) no se probó.
- Nada de esto garantiza un empleo: **prueba, mide y ajusta** el plan cada trimestre.

### Fuentes consultadas (2026-10-29)

- Wozniak e *Integer BASIC*: [Wikipedia: Integer BASIC](https://en.wikipedia.org/wiki/Integer_BASIC), [Gizmodo](https://gizmodo.com/how-steve-wozniak-wrote-basic-for-the-original-apple-fr-1570573636). Los detalles proceden de declaraciones del propio Wozniak citadas en fuentes secundarias.
- Python: [Computer History Museum](https://www.computerhistory.org/collections/catalog/102738719), [History of Python](https://www.geeksforgeeks.org/?p=299587).
- Karpathy: [Zero to Hero](https://karpathy.ai/zero-to-hero.html), [llm.c: reproducir GPT-2](https://simonwillison.net/2024/May/28/reproducing-gpt-2).
- ALIA: [BSC](https://bsc.es/news/bsc-news/alia-europes-first-public-open-and-multilingual-ai-infrastructure), [Gobierno de España](https://digital.gob.es/eu/comunicacion/notas-prensa/secretaria-digitalizacion-e-inteligencia-artificial/2025/12/17-12-2025).
- Hejlsberg: [Wikipedia](https://en.wikipedia.org/wiki/Anders_Hejlsberg).
- ENIAC y sus programadoras: [Penn Almanac](https://almanac.upenn.edu/articles/75th-anniversary-of-the-electronic-numerical-integrator-and-computer-eniac), [All About Circuits](https://www.allaboutcircuits.com/news/six-female-programmers-behind-eniac/).
- Auditoría de archivos en Windows (evento 4663, SACL): [Morgan Tech Space](https://morgantechspace.com/2014/11/event-4663-an-attempt-was-made-to-access-an-object.html) y otras guías de terceros; **no pude verificar la página oficial de Microsoft**.

## 11. Primeros pasos esta semana

> **Ejercicio 1:** escribe, en una página, **tu meta** («entender y construir un LLM pequeño») y **tu primera fase** (la 1, con C#). Fecha de revisión: dentro de un mes.

> **Ejercicio 2:** elige **un** lenguaje principal y **un** tema de base (C# y Nand2Tetris parte 1). Escribe cuántas horas por semana.

> **Ejercicio 3:** haz el ejercicio de la Fase 2: implementa una **tabla hash** en C# **sin usar `Dictionary`** y compárala con `Dictionary` en velocidad.

> **Ejercicio 4:** mira el primer vídeo de «Zero to Hero» (micrograd) y **escribe con tus palabras** qué es una derivada y por qué sirve para entrenar.
