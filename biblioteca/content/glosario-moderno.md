# Diccionario del desarrollo de software

1783 entradas en 25 áreas, pensadas para cada perfil de un equipo. Usa la búsqueda de arriba para localizar cualquier palabra.

## Qué áreas leer según tu perfil

| Perfil | Áreas recomendadas |
|---|---|
| **Frontend** | Web y frontend; Programación: conceptos básicos; POO; Programación funcional; Concurrencia (asíncrono); Redes (HTTP); Seguridad (XSS, CORS); Pruebas; Git; Producto (UX/UI); IA (asistentes) |
| **Backend** | Backend y APIs; POO; Patrones de diseño; Concurrencia; Bases de datos; Arquitectura; Seguridad; Redes; Pruebas; Infraestructura básica; Git; IA |
| **Full-stack** | Todo lo de frontend y backend, más Bases de datos, Infraestructura y Git |
| **Móvil / escritorio / embebido** | Móvil, escritorio, embebido y videojuegos; POO; Concurrencia; Redes; Seguridad; Pruebas |
| **Arquitecto** | Arquitectura de software; Patrones; Concurrencia; Bases de datos; Infraestructura; Seguridad; Redes; Procesos; Equipo y negocio |
| **QA / Tester / SDET** | Pruebas y calidad; Procesos; Backend y Frontend (lo básico); Git; Infraestructura (CI/CD); Seguridad; IA (evals) |
| **DevOps / SRE / Infra** | Infraestructura, nube y DevOps; Redes; Sistema operativo y terminal; Seguridad; Concurrencia (procesos); Git; Procesos |
| **DBA / Datos** | Bases de datos y datos; Estructuras de datos; Seguridad; Infraestructura; Concurrencia (bloqueos y transacciones) |
| **IA / ML / Datos** | Inteligencia artificial; Bases de datos y datos; Estructuras de datos y algoritmos; Infraestructura (GPU, MLOps); Seguridad en IA |
| **Seguridad** | Seguridad; Redes; Sistema operativo; Infraestructura; Web y frontend (ataques); Backend; Pruebas |
| **Product Owner / Product Manager** | Producto, gestión y diseño; Procesos; Equipo y negocio; Frases de reunión; Siglas; Arquitectura (lo esencial) |
| **Project Manager / Scrum Master** | Procesos, metodologías y ciclo de vida; Producto, gestión y diseño; Equipo y negocio; Frases de reunión |
| **UX / UI** | Producto, gestión y diseño; Web y frontend; Móvil |
| **Tech lead / Engineering Manager** | Arquitectura; Patrones; Procesos; Equipo; Pruebas; Infraestructura; Seguridad; IA |
| **Principiante absoluto** | Programación: conceptos básicos; Sistema operativo y terminal; Control de versiones; Frases de reunión; Siglas |

## 1. Programación: conceptos básicos

| Término | Qué es | Como si fuera… |
|---|---|---|
| **API** | Contrato que dice cómo un programa puede pedirle cosas a otro: qué pedir, cómo y qué responde. | La carta de un restaurante: qué puedes pedir y cómo, sin entrar en la cocina. |
| **Artefacto (artifact)** | El resultado de un build: un `.jar`, un `.dll`, una imagen de contenedor… | El pan ya horneado. |
| **Asíncrono (async)** | Código que no se queda esperando: lanza una tarea lenta y sigue con otra. | Dejar la comida al fuego y poner la mesa. |
| **Biblioteca (library)** | Código de otros que tú llamas cuando lo necesitas. | Una caja de herramientas: tú decides cuándo usarla. |
| **Binario** | Archivo ejecutable ya compilado (no legible por personas). | — |
| **Build (compilación / construcción)** | Proceso que convierte el código fuente en algo ejecutable: compilar, empaquetar, probar. | Hornear la masa. |
| **Bytecode / código intermedio** | Código a medio camino entre el que escribes y el del procesador, que una máquina virtual ejecuta (Java y C# lo usan). | — |
| **Compilador** | Programa que traduce de una vez el código a lenguaje máquina o intermedio antes de ejecutarlo. | Traducir un libro entero antes de publicarlo. |
| **Concurrencia / paralelismo** | Concurrencia: hacer varias cosas a la vez en apariencia (turnándose). Paralelismo: hacerlas realmente a la vez (varios núcleos). | Un camarero atendiendo varias mesas frente a varios camareros. |
| **Condición de carrera (race condition)** | Fallo por depender del orden en que dos tareas se ejecutan. | Dos personas editando a la vez el mismo documento sin avisarse. |
| **Configuración / variables de entorno** | Valores que cambian según dónde se ejecute (claves, direcciones) y que se sacan del código. | — |
| **Cuello de botella (bottleneck)** | La parte más lenta que limita todo el sistema. | El embudo. |
| **Dependencia** | Código de otros que tu proyecto necesita para funcionar. | Los ingredientes comprados que usa tu receta. |
| **Endpoint** | Una dirección concreta de una API a la que se le hace una petición (`/api/products`). | Una ventanilla concreta de la oficina. |
| **Excepción** | Evento de error que interrumpe el flujo normal del programa. | — |
| **Framework (marco de trabajo)** | Estructura que te llama a ti: define cómo se organiza la aplicación y tú rellenas los huecos (Spring, ASP.NET, React…). | Un molde: tú pones el contenido, él decide la forma. |
| **Fuga de memoria (memory leak)** | El programa va reservando memoria sin liberarla hasta agotarla. | Una bañera sin desagüe con el grifo abierto. |
| **Gestor de paquetes** | Herramienta que instala y actualiza dependencias (npm, Maven, NuGet, pip, cargo). | Una tienda de ingredientes con su lista de la compra. |
| **Hilo (thread)** | Línea de ejecución dentro de un programa. | — |
| **Inmutabilidad** | Un dato que, una vez creado, no cambia. Evita errores. | Un contrato firmado: se hace uno nuevo en vez de tachar. |
| **Interbloqueo (deadlock)** | Dos tareas se esperan la una a la otra para siempre. | Dos coches en un puente de un solo carril, cara a cara. |
| **Intérprete** | Programa que lee y ejecuta el código línea a línea en el momento. | Un traductor simultáneo en una conferencia. |
| **JIT (compilación en tiempo de ejecución)** | La máquina virtual traduce a lenguaje máquina las partes más usadas mientras el programa corre, para que vayan rápido. | — |
| **Latencia / rendimiento (throughput)** | Latencia: cuánto tarda una petición. Rendimiento: cuántas peticiones se atienden por segundo. | Cuánto tarda un coche (latencia) frente a cuántos coches pasan por hora (rendimiento). |
| **Lenguaje de programación** | Idioma formal para dar instrucciones al ordenador (Java, C#, Python…). | — |
| **Log (registro)** | Líneas de texto que el programa va escribiendo para contar qué está haciendo. | El diario de a bordo. |
| **Máquina virtual (JVM, CLR)** | Programa que ejecuta el bytecode y hace que el mismo programa corra en distintos sistemas. JVM es la de Java; CLR la de .NET. | Un adaptador universal de enchufes. |
| **Monorepo / polyrepo** | Monorepo: muchos proyectos en un mismo repositorio. Polyrepo: un repositorio por proyecto. | — |
| **Null (nulo)** | Valor que significa «no hay nada». Intentar usarlo es uno de los fallos más famosos. | Una caja vacía que alguien intenta abrir como si tuviera algo. |
| **Recolector de basura (garbage collector)** | Mecanismo que libera automáticamente la memoria de lo que el programa ya no usa. | Un servicio de limpieza que pasa solo. |
| **Repositorio (repo)** | Carpeta con el código y todo su historial de cambios (normalmente con Git). | Un álbum que guarda todas las versiones. |
| **Runtime (entorno de ejecución)** | Lo que hace falta instalado para que un programa se ejecute (Node, JRE, .NET Runtime). | — |
| **SDK (kit de desarrollo)** | Paquete con herramientas, bibliotecas y documentación para programar para una plataforma (por ejemplo, el SDK de Android). | — |
| **SDK vs API vs librería** | API: el contrato. Librería: código que lo ofrece. SDK: caja con librerías, herramientas y documentación. | — |
| **Stack trace (traza de pila)** | Lista de llamadas que muestra por dónde iba el programa cuando falló. | El rastro de migas del error. |
| **Tipado estático / dinámico** | Estático: los tipos de los datos se comprueban antes de ejecutar (Java, C#, TypeScript). Dinámico: se comprueban al ejecutar (JavaScript, Python). | — |
| **Versionado semántico (SemVer)** | Numeración `MAYOR.MENOR.PARCHE`: sube MAYOR si rompe cosas, MENOR si añade sin romper, PARCHE si corrige. | 2.4.1 → 2.4.2 es solo un arreglo; 3.0.0 avisa de cambios grandes. |

## 2. Programación orientada a objetos (POO)

| Término | Qué es | Como si fuera… |
|---|---|---|
| **Abstracción** | Mostrar solo lo esencial y ocultar los detalles. | Conducir sin saber cómo funciona el motor. |
| **Agregación** | Composición débil: las partes pueden existir por separado. | Un equipo y sus jugadores. |
| **Anemic domain model (modelo anémico)** | Antipatrón: clases con solo datos y toda la lógica fuera. | — |
| **Asociación** | Relación general entre dos clases. | — |
| **Atributo / campo / propiedad** | Dato que guarda un objeto (nombre, precio). | La ficha de un empleado: nombre, sueldo. |
| **Atributos / anotaciones** | Etiquetas que se ponen sobre clases o métodos para dar información (`@Override`, `[Obsolete]`). | Un post-it pegado al método. |
| **Autoboxing / boxing** | Conversión automática entre un tipo simple (`int`) y su versión objeto (`Integer`). | — |
| **Bajo acoplamiento, alta cohesión** | Meta de diseño: pocas dependencias entre clases y clases enfocadas en una sola tarea. | — |
| **Clase** | Plantilla que describe cómo son y qué saben hacer los objetos de un tipo. | El molde de las galletas. |
| **Clase abstracta** | Clase que no se puede instanciar y sirve de base; puede tener métodos sin implementar. | Un plano genérico, no una casa. |
| **Clase anidada / interna** | Clase definida dentro de otra. | — |
| **Clase anónima** | Clase sin nombre definida en el mismo punto de uso. | — |
| **Clase base / superclase / padre** | La clase de la que otras heredan. | — |
| **Clase de datos (data class)** | Clase pensada solo para guardar datos (Kotlin). | — |
| **Clase derivada / subclase / hija** | La que hereda de otra. | — |
| **Clase sellada / final** | Clase de la que **no se puede heredar**. | — |
| **Clon / copia superficial / copia profunda** | Superficial: copia el objeto pero comparte lo que contiene. Profunda: copia también lo de dentro. | — |
| **Composición** | Construir un objeto **a partir de otros** («tiene un»). Suele preferirse a la herencia. | Un coche tiene motor. |
| **Constructor** | Método especial que **crea y prepara** un objeto al nacer. | El alta de un empleado nuevo. |
| **Covarianza / contravarianza** | Reglas sobre cuándo un tipo genérico puede sustituir a otro. | — |
| **Delegación** | Un objeto encarga una tarea a otro. | — |
| **Deserialización** | El camino contrario: reconstruir el objeto. | Montarlo al llegar. |
| **Destructor / finalizador** | Método que se ejecuta al destruirse el objeto (en C++; en Java/C# se evita usarlo). | La baja del empleado. |
| **Duck typing** | «Si anda como un pato y hace cuac, es un pato»: lo que importa es lo que el objeto sabe hacer, no su tipo declarado (Python, JavaScript). | — |
| **Encapsulamiento** | Esconder los datos internos y permitir tocarlos solo por métodos controlados. | Un cajero automático: no ves el dinero, solo usas los botones. |
| **Encapsular lo que varía** | Principio: aísla la parte que cambiará más. | — |
| **Enum (enumeración)** | Tipo con un conjunto fijo de valores con nombre (LUNES, MARTES…). | Los días de la semana. |
| **Genéricos (generics)** | Clases y métodos que funcionan con **cualquier tipo**, elegido al usarlos (`List<T>`). | Una caja que vale para cualquier cosa, pero etiquetada. |
| **Getter / setter** | Métodos para leer y modificar un atributo privado. | — |
| **Hash code** | Número que resume un objeto, usado por mapas y conjuntos. | — |
| **Herencia** | Una clase hereda atributos y métodos de otra y los amplía. | «Un coche es un vehículo» y añade sus propias cosas. |
| **Herencia múltiple** | Heredar de varias clases a la vez (C++, Python; Java y C# no lo permiten con clases). | — |
| **Igualdad vs identidad** | Igualdad: tienen los mismos datos. Identidad: son el **mismo objeto**. | Dos billetes de 10 € iguales frente al mismo billete. |
| **Implementar una interfaz** | Cumplir su contrato escribiendo los métodos. | — |
| **Inmutable** | Objeto que no cambia tras crearse. | — |
| **Interfaz (interface)** | Contrato que lista qué métodos debe ofrecer quien lo cumple, sin decir cómo. | El estándar de un enchufe. |
| **Interfaz fluida (fluent interface)** | Encadenar métodos para que se lea como una frase (`builder.nombre("x").edad(3).build()`). | — |
| **Ley de Demeter** | «No hables con extraños»: un objeto solo habla con sus vecinos directos. | — |
| **Método** | Función que pertenece a una clase y actúa sobre sus objetos. | Lo que sabe hacer el empleado: facturar, archivar. |
| **Miembro estático (static)** | Pertenece a la **clase**, no a cada objeto. | Un contador de empleados de la empresa. |
| **Mixin** | Trozo de comportamiento que se mezcla en una clase sin ser su padre. | — |
| **Modelo de dominio** | Clases que representan los conceptos del negocio. | — |
| **Modificadores de acceso** | Reglas de visibilidad: `public` (todos), `private` (solo la clase), `protected` (la clase y sus hijas), `internal`/paquete. | — |
| **Objeto / instancia** | Cada ejemplar concreto creado a partir de una clase. | Cada galleta hecha con el molde. |
| **Objeto nulo (null object)** | Objeto «que no hace nada» que sustituye al `null` para evitar comprobaciones. | — |
| **Polimorfismo** | Un mismo mensaje produce comportamientos distintos según el objeto que lo recibe. | «Hacer ruido»: el perro ladra, el gato maúlla. |
| **POO (programación orientada a objetos)** | Estilo de programar en el que el programa se organiza en **objetos** que juntan datos y las operaciones que los manejan. | Una oficina con empleados: cada uno guarda su información y sabe hacer su trabajo. |
| **Principio de sustitución de Liskov** | Una subclase debe poder usarse donde se espera la base sin romper nada. | — |
| **Programación orientada a aspectos (AOP)** | Separar tareas transversales (registro, seguridad) para no repetirlas por todo el código. | — |
| **Programación orientada a componentes** | Construir con piezas reutilizables con interfaces claras. | — |
| **Propiedad (en C#)** | Forma de C# de exponer un campo con lectura/escritura controlada. | — |
| **Record** | Clase pensada para **guardar datos inmutables** con poco código (Java, C#). | Una ficha impresa. |
| **Reflexión (reflection)** | Capacidad de un programa de **examinarse a sí mismo** en ejecución (qué clases y métodos tiene). | — |
| **Serialización** | Convertir un objeto en un formato (JSON, binario) para guardarlo o enviarlo. | Desmontar un mueble para mandarlo en caja. |
| **Singleton** | Patrón: **una sola instancia** en todo el programa. | — |
| **Sobrecarga (overload)** | Varios métodos con el **mismo nombre y distintos parámetros**. | «Calcular(a)» y «Calcular(a, b)». |
| **Sobrescritura / anulación (override)** | Una subclase **redefine** un método heredado. | — |
| **Tell, don't ask** | Dile al objeto qué hacer; no le pidas sus datos para decidir tú. | — |
| **this / self** | Referencia al propio objeto dentro de sus métodos. | «Yo mismo». |
| **Tipo parametrizado** | `List<String>`: lista de un tipo concreto. | — |
| **Tipo valor / tipo referencia** | Valor: la variable contiene el dato. Referencia: contiene la dirección de dónde está. | Una nota con el dato frente a una nota con la dirección del armario. |
| **Trait** | Conjunto de métodos reutilizable que una clase puede incorporar (Rust, PHP, Scala). | — |

## 3. Programación funcional y otros paradigmas

| Término | Qué es | Como si fuera… |
|---|---|---|
| **Aplicación parcial** | Fijar algunos parámetros de una función y obtener otra. | — |
| **Async/await** | Sintaxis para escribir código asíncrono como si fuera secuencial. | — |
| **Callback** | Función que se pasa a otra para que la llame **cuando termine** algo. | «Llámame cuando esté listo». |
| **Callback hell** | Funciones anidadas dentro de funciones que dejan el código ilegible. | — |
| **Caso base** | Condición que detiene la recursión. | — |
| **Closure (clausura)** | Función que **recuerda las variables** del lugar donde se creó. | Una nota que lleva anotados los datos del sitio donde se escribió. |
| **Coincidencia de patrones (pattern matching)** | Comprobar la forma de un dato y extraer sus partes en un solo paso. | — |
| **Composición de funciones** | Encadenar funciones: la salida de una es la entrada de la siguiente. | — |
| **Corrutina (coroutine)** | Función que puede **pausarse y reanudarse**. | — |
| **Currying** | Convertir una función de varios parámetros en una cadena de funciones de uno. | — |
| **Desestructuración** | Sacar partes de una lista u objeto en variables sueltas. | — |
| **Efecto secundario (side effect)** | Cualquier cosa que una función hace además de devolver un valor (escribir un archivo, cambiar una variable). | — |
| **Evaluación ansiosa (eager)** | Se calcula de inmediato. | — |
| **Evaluación perezosa (lazy)** | No se calcula un valor hasta que hace falta. | — |
| **Función de orden superior** | Función que recibe o devuelve otras funciones (`map`, `filter`). | — |
| **Función pura** | Siempre devuelve lo mismo para los mismos datos y no tiene efectos secundarios. | Una calculadora. |
| **Generador (generator) / iterador** | Objeto que **produce valores uno a uno** bajo demanda. | — |
| **Lambda / función anónima** | Función breve sin nombre, escrita donde se usa. | — |
| **Macro** | Código que se expande en otro código antes de compilar. | — |
| **Map / filter / reduce** | `map`: transforma cada elemento. `filter`: se queda con los que cumplen una condición. `reduce`: combina todos en un solo valor. | Máquina de zumo: lavar (map), descartar fruta mala (filter), exprimir todo en un vaso (reduce). |
| **Memoización** | Guardar el resultado de una función para no recalcularlo con los mismos datos. | — |
| **Metaprogramación** | Programas que **generan o modifican código**. | — |
| **Mónada** | Patrón para encadenar operaciones que pueden fallar o tener efectos (`Option`, `Result`, `Promise`). | — |
| **Observer (observador)** | Patrón: los interesados se suscriben y reciben aviso de cambios. | — |
| **Option / Maybe** | Tipo que representa «hay un valor» o «no hay nada». | — |
| **Paradigma** | Manera de enfocar y organizar la programación (orientada a objetos, funcional, imperativa, declarativa…). | Estilos de cocina. |
| **Programación basada en eventos** | El flujo lo determinan eventos (clic, mensaje). | — |
| **Programación declarativa** | Se describe **qué** se quiere y no cómo (SQL, HTML). | Pedir en un restaurante sin dar la receta. |
| **Programación estructurada** | Uso de secuencias, condicionales y bucles sin saltos arbitrarios (`goto`). | — |
| **Programación funcional** | Construir el programa con **funciones puras** que transforman datos, sin cambiar estado. | Una cadena de montaje: entra pieza, sale pieza. |
| **Programación imperativa** | Se escribe **paso a paso cómo** hacer las cosas. | Una receta con pasos. |
| **Programación lógica** | Se declaran hechos y reglas y el sistema deduce (Prolog). | — |
| **Programación procedural** | Organizar el código en procedimientos o funciones. | — |
| **Programación reactiva** | Programar con **flujos de datos** que reaccionan a cambios. | Una hoja de cálculo: cambias una celda y se recalculan las demás. |
| **Promesa (Promise) / Future** | Objeto que representa un resultado **que llegará más tarde**. | El resguardo de la tintorería. |
| **Recursión** | Una función que **se llama a sí misma** para resolver un problema más pequeño. | Muñecas rusas. |
| **Recursión de cola (tail recursion)** | La llamada recursiva es lo último que se hace; permite optimizar el uso de pila. | — |
| **Result / Either** | Tipo que representa éxito o error, sin lanzar excepciones. | — |
| **Tipo algebraico (ADT)** | Tipos construidos combinando otros: «uno u otro» (suma) y «todos juntos» (producto). | — |
| **Transparencia referencial** | Se puede sustituir una expresión por su valor sin cambiar el programa. | — |

## 4. Estructuras de datos y algoritmos

| Término | Qué es | Como si fuera… |
|---|---|---|
| **Aleatoriedad / semilla (seed)** | Números «al azar» generados por un algoritmo; la semilla fija la secuencia. | — |
| **Algoritmo** | Secuencia finita de pasos para resolver un problema. | Una receta. |
| **Algoritmo de Dijkstra** | Calcula el camino más corto en un grafo con pesos. | El GPS. |
| **Algoritmo voraz (greedy)** | Elige siempre la mejor opción inmediata. | — |
| **Árbol** | Estructura jerárquica de nodos con un padre y varios hijos. | Un organigrama. |
| **Árbol balanceado (AVL, rojo-negro)** | Árbol que se reorganiza para no deformarse. | — |
| **Árbol binario** | Cada nodo tiene como mucho dos hijos. | — |
| **Árbol binario de búsqueda (BST)** | Árbol ordenado que permite buscar rápido. | — |
| **Array (vector, arreglo)** | Lista de elementos contiguos con acceso por posición (índice). | Una fila de casillas numeradas. |
| **AST (árbol de sintaxis abstracta)** | Representación en árbol del código tras analizarlo. | — |
| **Backtracking (vuelta atrás)** | Probar caminos y retroceder si no sirven. | Salir de un laberinto. |
| **Binario / hexadecimal / octal** | Sistemas numéricos de base 2, 16 y 8. | — |
| **Bit / byte** | Bit: 0 o 1. Byte: 8 bits. | — |
| **Búsqueda en anchura / en profundidad (BFS / DFS)** | Recorrer un grafo por niveles / por ramas hasta el fondo. | — |
| **Búsqueda lineal / binaria** | Lineal: mirar uno a uno. Binaria: partir por la mitad en una lista ordenada. | Buscar una palabra en un diccionario abriéndolo por el medio. |
| **Cola (queue)** | Entra por un extremo y sale por el otro: **FIFO** (primero en entrar, primero en salir). | La cola del supermercado. |
| **Cola de prioridad** | Sale primero el elemento más prioritario. | Urgencias de un hospital. |
| **Cola doble (deque)** | Cola que permite añadir y sacar por ambos extremos. | — |
| **Colisión (hash)** | Dos datos distintos dan el mismo hash. | — |
| **Complejidad temporal / espacial** | Cuánto tarda / cuánta memoria usa. | — |
| **Conjunto (set)** | Colección **sin repetidos**. | — |
| **Divide y vencerás** | Dividir el problema en partes pequeñas, resolverlas y combinar. | — |
| **Endianness (orden de bytes)** | Orden en que se guardan los bytes de un número (little-endian, big-endian). | — |
| **Estructura de datos** | Forma de organizar datos en memoria para usarlos con eficiencia. | Estantería, cajón o archivador. |
| **Expresión regular (regex)** | Patrón para buscar y validar texto. | Un comodín muy potente. |
| **Filtro de Bloom** | Estructura que dice «seguro que no» o «quizá sí» ocupando muy poca memoria. | — |
| **Fuerza bruta** | Probar todas las posibilidades. | — |
| **Grafo** | Nodos (vértices) unidos por aristas. | Un mapa de metro. |
| **Hash (función hash)** | Convierte datos en un número de tamaño fijo. | — |
| **Heurística** | Regla práctica que da buenas soluciones sin garantía de ser la mejor. | — |
| **Lista enlazada** | Elementos unidos por referencias: cada uno apunta al siguiente. | Una cadena de eslabones. |
| **Mapa / diccionario / tabla hash** | Pares clave → valor con búsqueda rápida. | Una agenda de contactos. |
| **Montículo (heap)** | Árbol que mantiene siempre el mayor (o menor) en la cima. | — |
| **Nodo / vértice / arista** | Elementos de un grafo o árbol: los puntos y las conexiones. | — |
| **Notación O grande (Big O)** | Manera de expresar **cómo crece el tiempo o la memoria** al crecer los datos. O(1) constante, O(log n) logarítmica, O(n) lineal, O(n²) cuadrática. | Cuánto tarda en buscar a alguien: en una fila entera (n) o abriendo un diccionario por la mitad (log n). |
| **Ordenación (sorting)** | Colocar elementos en orden (burbuja, inserción, mezcla/merge, rápida/quick). | — |
| **Parsear (parseo)** | Analizar un texto y convertirlo en datos estructurados. | — |
| **Pila (stack)** | Se añade y se saca por el mismo extremo: **LIFO** (último en entrar, primero en salir). | Una pila de platos. |
| **Programación dinámica** | Resolver subproblemas una vez y reutilizar el resultado. | — |
| **Punto flotante** | Forma de guardar decimales (`double`); puede dar pequeños errores (0,1+0,2). | — |
| **Tokenizar / lexer** | Dividir texto en piezas con significado (palabras, números, símbolos). | — |
| **Trie** | Árbol de prefijos para buscar palabras (autocompletado). | — |
| **Unicode / UTF-8 / ASCII** | Estándares para representar texto; UTF-8 es el más usado. | — |

## 5. Concurrencia, procesos y paralelismo

| Término | Qué es | Como si fuera… |
|---|---|---|
| **Barrera de memoria / modelo de memoria** | Reglas sobre cuándo los cambios de un hilo son visibles para otros. | — |
| **Cambio de contexto (context switch)** | Cuando el sistema deja de ejecutar una tarea y pasa a otra; tiene coste. | — |
| **Canal (channel)** | Conducto para pasar datos entre tareas (Go, Rust). | — |
| **Cola de tareas / work queue** | Lista de trabajos pendientes que van cogiendo los trabajadores. | — |
| **Compare-and-swap (CAS)** | Instrucción atómica: «si vale X, cámbialo por Y». | — |
| **CompletableFuture** | Tipo de Java para encadenar resultados asíncronos. | — |
| **Concurrencia** | Gestionar varias tareas **que avanzan solapadas** (aunque no a la vez). | Un camarero que atiende varias mesas. |
| **Contenedor de procesos (cgroups, namespaces)** | Funciones del sistema que aíslan y limitan procesos; base de los contenedores. | — |
| **Cron / tarea programada** | Ejecución de tareas a horas o intervalos fijos. | — |
| **Deadlock (interbloqueo)** | Dos o más hilos se esperan mutuamente para siempre. | Dos personas que se ceden el paso eternamente en una puerta. |
| **Demonio (daemon) / servicio** | Proceso que corre en segundo plano sin interfaz. | — |
| **E/S bloqueante / no bloqueante** | Bloqueante: el hilo espera. No bloqueante: sigue y se le avisa. | — |
| **Escalado de CPU (CPU-bound) / de E/S (I/O-bound)** | Tareas limitadas por cálculo / por esperas de disco o red. | — |
| **Estado compartido mutable** | Origen de casi todos los errores de concurrencia. | — |
| **Event loop (bucle de eventos)** | Bucle que recoge eventos y ejecuta sus manejadores uno a uno. | — |
| **Exclusión mutua (mutex / lock)** | Cerrojo: solo un hilo entra a la vez en la sección crítica. | La llave del baño. |
| **Fork** | En sistemas Unix, crear un proceso copia de otro. | — |
| **Fork/Join** | Dividir una tarea en subtareas paralelas y juntar los resultados. | — |
| **GIL (Global Interpreter Lock)** | Cerrojo de Python que impide que dos hilos ejecuten código Python a la vez. | — |
| **Goroutine** | Tarea ligera de Go. | — |
| **Hilo de interfaz (UI thread)** | El hilo que dibuja la pantalla; nunca debe bloquearse. | — |
| **Hilo demonio (daemon thread)** | Hilo secundario que no impide que el programa termine. | — |
| **Hilos virtuales (virtual threads)** | Hilos muy ligeros gestionados por el runtime (Java 21+). | — |
| **IPC (comunicación entre procesos)** | Mecanismos para que procesos se hablen: tuberías, sockets, memoria compartida. | — |
| **Join** | Esperar a que un hilo termine. | — |
| **Ley de Amdahl** | Límite de cuánto se acelera algo al paralelizarlo: lo que no se puede paralelizar frena el conjunto. | — |
| **Livelock** | Los hilos no se bloquean pero cambian de estado sin avanzar. | Dos personas que esquivan y se vuelven a cruzar. |
| **Lock-free / wait-free** | Algoritmos concurrentes sin cerrojos. | — |
| **MapReduce** | Modelo: repartir trabajo (map) y combinar resultados (reduce) en muchas máquinas. | — |
| **Memoria compartida** | Zona de memoria que usan varios hilos o procesos. | — |
| **Modelo de actores** | Cada actor tiene su estado y se comunica solo por mensajes (Akka, Erlang). | — |
| **Monitor** | Mecanismo que agrupa datos, cerrojo y condiciones de espera. | — |
| **Monohilo (single-thread)** | Un único hilo de ejecución (JavaScript en el navegador, Node). | — |
| **Multihilo (multithreading)** | Usar varios hilos dentro de un proceso. | — |
| **Multiproceso (multiprocessing)** | Usar **varios procesos** a la vez para trabajar en paralelo; no comparten memoria, se comunican por canales. | Varios restaurantes que se pasan notas. |
| **Núcleo (core) / CPU** | Unidad que ejecuta instrucciones; un procesador moderno tiene varios. | — |
| **Operación atómica** | Operación **indivisible**: nadie puede ver un estado a medias. | — |
| **Paralelismo** | Ejecutar tareas **realmente al mismo tiempo** en varios núcleos. | Varios camareros. |
| **Paso de mensajes** | Comunicarse enviando mensajes en vez de compartir memoria. | — |
| **PID** | Número que identifica a un proceso. | — |
| **Planificador (scheduler)** | Parte del sistema que decide qué tarea se ejecuta y cuándo. | — |
| **Pool de hilos (thread pool)** | Conjunto de hilos reutilizables que atienden tareas. | Una plantilla fija de empleados. |
| **Preemptivo / cooperativo** | Preemptivo: el sistema interrumpe las tareas. Cooperativo: cada tarea cede el control por voluntad propia. | — |
| **Prioridad (nice)** | Nivel de preferencia de un proceso ante el planificador. | — |
| **Proceso** | Programa **en ejecución**, con su propia memoria y recursos aislados del resto. | Un restaurante entero con su cocina y su personal. |
| **Proceso huérfano** | Proceso cuyo padre murió. | — |
| **Proceso zombi** | Proceso terminado cuyo padre aún no ha recogido su estado. | — |
| **Productor / consumidor** | Un lado genera tareas, otro las consume, con una cola entre medias. | — |
| **Reactor / Proactor** | Patrones para gestionar E/S con bucle de eventos. | — |
| **Sandbox** | Entorno restringido donde un proceso no puede dañar al resto. | — |
| **Sección crítica** | Trozo de código que **no puede ejecutarse a la vez** por dos hilos. | Un baño con una sola llave. |
| **Semáforo** | Contador que limita cuántos hilos pueden acceder a un recurso a la vez. | Las plazas de un aparcamiento. |
| **Señal (signal)** | Aviso que el sistema envía a un proceso (SIGTERM: «termina», SIGKILL: «mátalo»). | — |
| **Socket** | Extremo de una conexión de red o entre procesos. | — |
| **Spinlock** | Cerrojo en el que el hilo espera dando vueltas sin dormir. | — |
| **Starvation (inanición)** | Un hilo nunca consigue los recursos porque otros se los llevan. | — |
| **Task / Task Parallel Library (TPL)** | Abstracción de tarea asíncrona en .NET. | — |
| **Thread-safe (seguro para hilos)** | Código que funciona bien aunque lo usen varios hilos a la vez. | — |
| **Tiempo compartido (time slicing)** | Cada tarea usa la CPU un rato breve por turnos. | — |
| **Trabajo en segundo plano (background job)** | Tarea que se ejecuta fuera de la petición del usuario. | — |
| **Tubería (pipe)** | Canal que lleva la salida de un proceso a la entrada de otro. | — |
| **Variable de condición** | Permite a un hilo **esperar** hasta que se cumpla algo. | — |
| **Variable volátil (volatile)** | Marca que obliga a leer siempre el valor real de memoria. | — |

## 6. Sistema operativo, terminal y línea de comandos

| Término | Qué es | Como si fuera… |
|---|---|---|
| **Alias** | Nombre corto para un comando largo. | — |
| **Argumento (parámetro posicional)** | Dato que se pasa a un comando: `cp **origen destino**`. | — |
| **Autocompletado (Tab)** | Completar nombres de comandos y rutas con la tecla Tab. | — |
| **Bash / zsh / PowerShell** | Intérpretes de comandos: los dos primeros en Linux/macOS, PowerShell en Windows. | — |
| **`chmod` / `chown`** | Cambiar permisos / cambiar propietario. | — |
| **Clave SSH (pública/privada)** | Par de claves para entrar sin contraseña: la pública se deja en el servidor y la privada **nunca se comparte**. | — |
| **CLI (interfaz de línea de comandos)** | Programa que se maneja escribiendo comandos. | — |
| **Código de salida (exit code)** | Número con el que termina un programa: 0 = bien; otro = error. | — |
| **Comando** | Orden que se escribe en la terminal. | — |
| **Comodín (glob)** | `*` y `?` para designar varios archivos (`*.txt`). | — |
| **Controlador (driver)** | Programa que enseña al sistema a usar un dispositivo. | — |
| **Cron** | Programador de tareas periódicas. | — |
| **Ctrl+C / Ctrl+Z / Ctrl+D** | Interrumpir / suspender / fin de entrada. | — |
| **`curl` / `wget`** | Descargar o hacer peticiones HTTP desde la terminal. | — |
| **`df` / `du`** | Espacio libre del disco / espacio ocupado por carpetas. | — |
| **Directorio de trabajo (cwd)** | Carpeta en la que estás ahora. | — |
| **Directorio home** | La carpeta personal del usuario (`~`). | — |
| **`--` (doble guion solo)** | Separador: lo que va después no se interpreta como opciones. | — |
| **Dotfiles** | Archivos de configuración ocultos (`.bashrc`). | — |
| **Dry run (`--dry-run`)** | Simula lo que haría sin hacerlo. | — |
| **Emulador de terminal** | Programa gráfico que simula una terminal. | — |
| **Enlace simbólico (symlink)** | Acceso directo a otro archivo o carpeta. | — |
| **Entrada estándar (stdin)** | Por donde un programa lee datos (el teclado por defecto). | — |
| **`env` / `export`** | Ver variables de entorno / definirlas para los procesos hijos. | — |
| **Error estándar (stderr)** | Canal aparte para mensajes de error. | — |
| **Espacio de usuario / espacio de kernel** | Donde corren los programas normales / donde corre el núcleo. | — |
| **Expansión de llaves** | `a{1,2}` se convierte en `a1 a2`. | — |
| **Feature flag** | Interruptor en la aplicación para activar o desactivar una función. | — |
| **`find` / `xargs`** | Buscar archivos / construir comandos a partir de una lista. | — |
| ****Flag / opción / switch**** | Modificador de un comando que cambia su comportamiento: `ls -l` (la `-l` es el flag). Los cortos llevan un guion (`-l`), los largos dos (`--long`). | Los botones de una lavadora: un programa, varias opciones. |
| **Flag booleano** | Se pone o no se pone: activa algo (`--force`). | — |
| **Flag corto / largo** | Corto: una letra (`-v`). Largo: palabra (`--verbose`). | — |
| **Flag de bits (bit flag)** | En programación, un número cuyos bits individuales son interruptores de opciones. | — |
| **Gestor de paquetes del sistema** | `apt`, `dnf`, `brew`, `winget`: instala programas desde repositorios. | — |
| **`grep` / `sed` / `awk` / `cut` / `sort` / `uniq` / `wc`** | Herramientas clásicas para buscar y transformar texto. | — |
| **GUI (interfaz gráfica)** | Programa que se maneja con ventanas y ratón. | — |
| **`--help` / `-h`** | Muestra la ayuda de un comando. | — |
| **Here-document** | Bloque de texto que se entrega como entrada de un comando. | — |
| **Historial** | Lista de comandos escritos antes (`history`, flecha arriba). | — |
| **`journalctl` / logs del sistema** | Visor de registros del sistema. | — |
| **`jq`** | Herramienta para filtrar y transformar JSON desde la terminal. | — |
| **Kernel (núcleo)** | La parte central del sistema operativo, con acceso total al hardware. | — |
| **`kill`** | Enviar una señal a un proceso. | — |
| **Llamada al sistema (syscall)** | Petición de un programa al núcleo (abrir un archivo). | — |
| **`localhost` / 127.0.0.1** | Tu propio equipo. | — |
| **`man`** | Manual del sistema: `man ls`. | — |
| **Máscara de bits (bitmask)** | Valor para activar, apagar o consultar bits concretos. | — |
| **Opción con valor** | Flag que recibe un dato (`--output archivo.txt`, `-p 8080`). | — |
| **Operadores de bits** | `&` (y), `\|` (o), `^` (xor), `~` (no), `<<` y `>>` (desplazamiento). | — |
| **Partición** | Trozo de un disco con su propio sistema de archivos. | — |
| **PATH** | Lista de carpetas donde el sistema busca los comandos. | — |
| **Permisos (rwx)** | Lectura, escritura y ejecución para propietario, grupo y otros. | — |
| **`ping` / `traceroute` / `dig`** | Probar conectividad / ver la ruta / consultar DNS. | — |
| **Proceso en primer plano / segundo plano** | Foreground: ocupa la terminal. Background (`&`): corre sin bloquearla. | — |
| **Propietario / grupo** | Usuario dueño de un archivo y grupo con ciertos permisos. | — |
| **Puerto** | Número que identifica un servicio en una máquina (80 web, 22 SSH). | — |
| **Punto de montaje** | Carpeta donde se «engancha» un disco o partición. | — |
| **Redirección (`>`, `>>`, `<`)** | Enviar la salida a un archivo / añadir / leer desde un archivo. | — |
| **Repositorio de paquetes** | Servidor con programas listos para instalar. | — |
| **Ruta absoluta / relativa** | Absoluta: desde la raíz (`/home/ana/x`). Relativa: desde donde estás (`../x`). | — |
| **Salida estándar (stdout)** | Por donde escribe resultados (la pantalla). | — |
| **SCP / SFTP / rsync** | Formas de copiar archivos entre máquinas. | — |
| **Script** | Archivo con una lista de comandos que se ejecutan en secuencia. | — |
| **Servicio / systemd** | Programa de fondo; `systemd` es el que los arranca y supervisa en Linux. | — |
| **Shebang (`#!`)** | Primera línea de un script que indica con qué programa se ejecuta. | — |
| **Shell** | Programa que interpreta lo que escribes en la terminal (bash, zsh, PowerShell). | — |
| **Sistema de archivos** | Forma en que se organizan los datos en un disco (ext4, NTFS). | — |
| **Sistema operativo (SO)** | Software que gestiona el equipo y deja que los programas usen CPU, memoria y dispositivos (Windows, Linux, macOS). | El director del edificio. |
| **`ss` / `netstat`** | Ver conexiones y puertos de red. | — |
| **SSH** | Conexión **segura** a otra máquina para ejecutar comandos. | — |
| **Subcomando** | Comando dentro de un comando (`git commit`, `dotnet run`). | — |
| **Subshell** | Shell hija que ejecuta un grupo de comandos aparte. | — |
| **`sudo` / root** | Ejecutar como administrador / el superusuario. | — |
| **`tar` / `gzip` / `zip`** | Empaquetar y comprimir archivos. | — |
| **Terminal / consola** | Ventana para escribir comandos. | — |
| **`tmux` / `screen`** | Multiplexores: varias sesiones de terminal que sobreviven a la desconexión. | — |
| **`top` / `htop` / `ps`** | Ver los procesos y su uso de CPU y memoria. | — |
| **Tubería (`\|`, pipe)** | Conecta la salida de un comando con la entrada del siguiente. | Una cinta transportadora entre dos máquinas. |
| **TUI (interfaz de texto)** | Interfaz con ventanas dibujadas con caracteres en la terminal. | — |
| **Variable `$?`** | En bash, el código de salida del último comando. | — |
| **Variable de entorno** | Pareja nombre-valor disponible para los programas (`PATH`, `HOME`). | — |
| **Verbose (`-v`) / quiet (`-q`)** | Más información / menos información. | — |
| **WSL** | Subsistema de Windows para ejecutar Linux. | — |

## 7. Código heredado, mantenimiento y calidad del código

| Término | Qué es | Como si fuera… |
|---|---|---|
| **Bit rot (pudrición del software)** | El software envejece sin tocarlo: las bibliotecas, el sistema y los servicios cambian alrededor y deja de funcionar. | Un coche parado que se estropea solo. |
| **Boilerplate (código repetitivo)** | Código que hay que escribir una y otra vez casi igual para que lo básico funcione. | El encabezado de una carta formal. |
| **Breaking change (cambio que rompe)** | Cambio en una nueva versión que hace que lo que antes funcionaba deje de funcionar. | Cambiar la forma del enchufe: los aparatos antiguos ya no encajan. |
| **Bug** | Un fallo del programa: hace algo distinto de lo esperado. | Una errata en un libro. |
| **Bus factor (factor autobús)** | Cuántas personas tendrían que desaparecer (por ejemplo, atropelladas por un autobús) para que el proyecto se quede sin nadie que lo entienda. Si es 1, hay riesgo. | — |
| **Código espagueti** | Código enredado donde todo depende de todo y no se sabe qué afecta a qué. | Un plato de espaguetis: tirar de un hilo mueve todos. |
| **Código muerto (dead code)** | Código que nunca se ejecuta pero sigue ahí. | Una puerta que no lleva a ninguna parte. |
| **Conocimiento tribal (tribal knowledge)** | Lo que solo sabe «el de siempre» y no está escrito en ningún sitio. | La receta de la abuela que nadie apuntó. |
| **Deprecado (deprecated)** | Algo que sigue funcionando pero no se debe usar, porque será eliminado en el futuro. | Un letrero de «salida en desuso». |
| **Deuda técnica** | Atajos tomados hoy para ir rápido que se pagan con intereses mañana (más lentitud, más fallos). | Dejar los platos sin fregar: ahorras tiempo hoy, mañana hay que fregar el doble. |
| **Documentación viva** | Documentación que se actualiza con el código, y se prueba o se genera de él, para que no mienta. | — |
| **EOL (End of Life) / fin de vida** | Fecha a partir de la cual un producto deja de recibir actualizaciones y soporte. | Un coche cuyo fabricante ya no fabrica repuestos. |
| **Greenfield / Brownfield** | Greenfield: proyecto desde cero. Brownfield: proyecto sobre algo que ya existe. | Solar vacío frente a reforma de una casa. |
| **Hack / kludge** | Solución chapucera pero que funciona. A veces es elegante, casi siempre es provisional. | — |
| **Hardcodear** | Escribir un valor directamente en el código (contraseña, dirección…) en vez de ponerlo en la configuración. | Pintar el número de teléfono en la pared. |
| **Hotfix** | Corrección urgente de un fallo en producción, fuera del calendario normal. | Un parche de emergencia en una tubería que gotea. |
| **Legacy (sistema heredado)** | Software antiguo que sigue en uso y del que depende el negocio, aunque use tecnología anticuada y dé miedo tocarlo. No significa «malo»: significa «funciona y paga las nóminas». | Un edificio antiguo en el que aún vive gente: no se puede derribar, pero cada reforma puede romper una tubería. |
| **LTS (Long Term Support)** | Versión con soporte prolongado (años). Es la que se elige para producción. | La edición «de confianza» frente a la edición «de moda». |
| **Mantenimiento** | Todo el trabajo después de la primera entrega: corregir fallos, actualizar, adaptar. Suele costar más que construir. | — |
| **Migración** | Mover datos o aplicaciones de un sistema a otro (otra base de datos, otro servidor, otra versión). | Una mudanza. |
| **Monolito** | Aplicación construida como un solo bloque: todo el código se compila, se despliega y se escala junto. | Una casa de una sola pieza: para cambiar una ventana hay que tener en cuenta toda la casa. |
| **Número mágico (magic number)** | Un número en el código sin explicación de qué significa. | «Añade 86400» (¿segundos de un día?). |
| **Olor a código (code smell)** | Señal en el código de que algo probablemente está mal (aunque funcione): funciones enormes, nombres confusos, duplicaciones. | Olor a humo: aún no hay incendio, pero conviene mirar. |
| **Parche (patch)** | Pequeña modificación que corrige un fallo o un problema de seguridad. | — |
| **Reescritura (rewrite) / Big bang** | Tirar el sistema viejo y construir uno nuevo de golpe. Arriesgado y muchas veces fracasa. | Derribar la casa antes de saber si la nueva tendrá tejado. |
| **Refactorización (refactor)** | Reordenar el código sin cambiar lo que hace, para que sea más fácil de entender y modificar. | Reorganizar el taller: las mismas herramientas, mejor colocadas. |
| **Retrocompatibilidad** | Que una versión nueva siga funcionando con lo hecho para la versión anterior. | — |
| **Strangler fig (higuera estranguladora)** | Estrategia de migración por trozos: el sistema nuevo va sustituyendo partes del viejo hasta que el viejo desaparece. | Como la higuera que crece alrededor de un árbol hasta reemplazarlo. |
| **Vendor lock-in (cautividad del proveedor)** | Dependencia de un proveedor tan fuerte que cambiar de él cuesta muchísimo. | Una impresora que solo acepta sus propios cartuchos. |
| **Workaround** | Solución temporal que esquiva un problema sin resolver la causa. | Poner un trapo bajo la gotera. |

## 8. Arquitectura de software

| Término | Qué es | Como si fuera… |
|---|---|---|
| **ACID / BASE** | ACID: transacciones estrictas y fiables (bases relacionales). BASE: más flexible y disponible, consistencia eventual (muchas NoSQL). | — |
| **Acoplamiento / cohesión** | Acoplamiento: cuánto depende una pieza de otras (poco es bueno). Cohesión: cuánto encaja lo que hay dentro de una pieza (mucho es bueno). | — |
| **Activo-activo / activo-pasivo** | Todas las réplicas atienden / una espera como reserva. | — |
| **Adaptador (adapter)** | Convierte una interfaz en otra. | — |
| **Agregado / entidad / objeto de valor** | Entidad: tiene identidad (un cliente). Objeto de valor: se define por sus datos (una dirección, un importe). Agregado: grupo que se trata como una unidad. | — |
| **Aislamiento de tenants** | Separación de datos y recursos: base por tenant, esquema por tenant o columna `tenant_id`. | — |
| **Almacenamiento de objetos** | Guardar archivos como objetos con identificador (S3). | — |
| **Almacenamiento en bloque / en archivos** | Como un disco / como carpetas compartidas. | — |
| **Alta disponibilidad (HA)** | Que el sistema siga funcionando aunque algo falle. | — |
| **Antipatrón** | Solución que parece buena y causa problemas habitualmente (objeto «dios», copiar-pegar). | — |
| **API Gateway** | Puerta de entrada única a muchos servicios: autentica, reparte y limita. | La recepción de un edificio. |
| **Arquitectura basada en espacio (space-based)** | Datos y proceso replicados en memoria para aguantar picos enormes de carga. | — |
| **Arquitectura basada en servicios** | Pocos servicios grandes (más que un monolito, menos que microservicios). | — |
| **Arquitectura cliente-servidor** | Unos programas piden (clientes) y otros responden (servidores). | Un cliente y un camarero. |
| **Arquitectura de microkernel / plug-in** | Un núcleo pequeño y extensiones que se enchufan (editores de código, navegadores). | — |
| **Arquitectura de n capas (n-tier)** | Variante con más capas. | — |
| **Arquitectura de pizarra (blackboard)** | Varios componentes leen y escriben en un espacio común hasta resolver un problema. | — |
| **Arquitectura de referencia** | Modelo base de una organización para nuevos sistemas. | — |
| **Arquitectura de software** | Las decisiones grandes sobre cómo se divide y se comunica un sistema, difíciles de cambiar luego. | El plano de un edificio. |
| **Arquitectura de tres capas (3-tier)** | Presentación, lógica de negocio y datos, separadas. | — |
| **Arquitectura del sistema vs. del software** | Del sistema: hardware, red, nube. Del software: código y componentes. | — |
| **Arquitectura empresarial (enterprise architecture)** | Cómo encajan procesos, sistemas y tecnología de toda una empresa (TOGAF, Zachman). | — |
| **Arquitectura en capas (layered)** | El código se divide en capas (presentación, lógica, datos) y cada una solo habla con la de al lado. | Un pastel de pisos. |
| **Arquitectura en cebolla (onion)** | Capas concéntricas con el dominio en el centro. | — |
| **Arquitectura evolutiva** | Diseñar para poder cambiar con facilidad. | — |
| **Arquitectura hexagonal** | Puertos y adaptadores alrededor del núcleo de negocio. | — |
| **Arquitectura hexagonal (puertos y adaptadores)** | El núcleo del negocio no conoce bases de datos ni pantallas; habla con el exterior solo a través de «puertos» y «adaptadores». | Un aparato con tomas estándar: cambias el cable sin abrir el aparato. |
| **Arquitectura limpia (clean)** | Dependencias hacia el centro; el negocio no conoce la tecnología. | — |
| **Arquitectura monolítica** | Todo en un único programa desplegable. | — |
| **Arquitectura orientada a eventos (EDA)** | Las piezas se avisan por eventos («pedido creado») sin conocerse entre sí. | Un tablón de anuncios. |
| **Arquitectura orientada a recursos (ROA)** | Diseño basado en recursos con direcciones propias (la base de REST). | — |
| **Atributos de calidad (requisitos no funcionales)** | Cómo debe comportarse el sistema: rendimiento, seguridad, disponibilidad, mantenibilidad… | — |
| **Autoscaling (escalado automático)** | Añadir o quitar servidores según la demanda. | — |
| **Backpressure (contrapresión)** | El que recibe avisa al emisor de que vaya más despacio. | — |
| **Backup / restore** | Copia de seguridad / recuperación. | — |
| **Balanceador de carga (load balancer)** | Reparte las peticiones entre varios servidores. | El que indica en el supermercado qué caja queda libre. |
| **BFF (Backend For Frontend)** | Un backend hecho a medida para cada tipo de cliente (web, móvil). | — |
| **Big ball of mud (gran bola de barro)** | Sistema sin estructura reconocible. | — |
| **Bounded context (contexto delimitado)** | Zona donde un término tiene un significado concreto (un «cliente» en facturación no es igual que en soporte). | — |
| **Caché** | Copia temporal de algo para no recalcularlo o pedirlo otra vez. | Apuntar en un papel lo que consultas cada día. |
| **Cache aside / write-through / write-back** | Estrategias para mantener la caché sincronizada con la base. | — |
| **CDN** | Red de servidores repartidos por el mundo que sirven copias de tus archivos desde el más cercano al usuario. | Quioscos de prensa por todo el país en vez de una sola imprenta. |
| **Circuit breaker (cortacircuitos)** | Si un servicio falla repetidamente, se deja de llamarlo un tiempo para no empeorarlo. | Los fusibles de la casa. |
| **Clean Architecture / Onion** | Parecida a la hexagonal: las dependencias apuntan hacia el centro (lo importante del negocio), nunca hacia fuera. | Capas de cebolla; el centro no sabe que existe la piel. |
| **Cola de mensajes / message broker** | Intermediario que guarda y reparte mensajes entre sistemas (RabbitMQ, Kafka). | Un buzón entre dos oficinas. |
| **Composable architecture** | Montar el sistema con piezas independientes intercambiables. | — |
| **Consistencia eventual** | Los datos acaban coincidiendo en todas partes, pero no instantáneamente. | Una noticia que tarda en llegar a todos los pueblos. |
| **Consistent hashing** | Reparto de claves que cambia poco al añadir o quitar servidores. | — |
| **CQRS** | Separar cómo se escribe de cómo se lee un dato, con modelos distintos. | Una caja para cobrar y un mostrador aparte para consultar. |
| **DDD (diseño guiado por el dominio)** | Diseñar el software a partir del lenguaje y las reglas del negocio. | — |
| **Dead letter queue (DLQ)** | Cola donde van los mensajes que no se pudieron procesar. | — |
| **Deuda de arquitectura** | Decisiones que hoy impiden evolucionar con comodidad. | — |
| **Diagrama de clases / de secuencia / de componentes / de despliegue** | Estructura / orden de mensajes en el tiempo / piezas del sistema / dónde corre cada pieza. | — |
| **Diagrama de contexto** | Muestra el sistema como una caja y con quién se relaciona. | — |
| **Disaster recovery (DR)** | Plan para recuperar el sistema tras un desastre. | — |
| **Dominio / subdominio** | Área del negocio / parte de ella (central, de soporte, genérica). | — |
| **DRY / KISS / YAGNI** | DRY: no repitas. KISS: hazlo simple. YAGNI: no construyas lo que todavía no necesitas. | — |
| **DTO** | Objeto sencillo para transportar datos entre capas o por la red. | Un sobre. |
| **Elasticidad** | Crecer y reducirse automáticamente según la carga. | — |
| **ESB (bus de servicios empresarial)** | Pieza central de las arquitecturas SOA por la que pasaban todos los mensajes. Hoy se considera pesada. | — |
| **Escalabilidad** | Capacidad de crecer para atender más carga. | — |
| **Escalado vertical / horizontal** | Vertical: máquina más potente. Horizontal: más máquinas. | Un camión más grande frente a más camiones. |
| **Event loop** | Mecanismo (Node, navegadores) que va atendiendo tareas una a una sin bloquearse. | — |
| **Event sourcing** | En vez de guardar el estado actual, se guarda la lista de todo lo ocurrido y el estado se calcula. | El extracto bancario frente al saldo: el saldo sale de sumar movimientos. |
| **Event storming** | Taller para descubrir el negocio colocando notas con eventos. | — |
| **Evento / mensaje** | Un aviso de que algo ocurrió (evento) o una orden para hacer algo (comando). Se manda por una cola o un tema. | — |
| **Exactly-once / at-least-once / at-most-once** | Garantías de entrega de mensajes: una vez exacta / al menos una (puede repetirse) / como mucho una (puede perderse). | — |
| **FaaS (función como servicio)** | Forma concreta de serverless: una función que se ejecuta cuando ocurre un evento (AWS Lambda, Azure Functions). | — |
| **Fachada (facade)** | Interfaz simple que oculta un subsistema complejo. | — |
| **Failover / failback** | Pasar a la reserva al fallar / volver al principal. | — |
| **Fitness function** | Prueba automática que comprueba que la arquitectura sigue cumpliendo sus objetivos. | — |
| **Gateway / puerta de enlace** | Punto de entrada o de conexión con otros sistemas. | — |
| **God object (objeto dios)** | Clase que lo sabe y lo hace todo. | — |
| **GraphQL** | Lenguaje de consulta: el cliente pide exactamente los campos que quiere en una sola petición. | — |
| **gRPC** | Forma rápida de comunicarse entre servicios con contratos definidos y formato binario. | — |
| **Headless** | Sistema sin interfaz propia, accesible por API. | — |
| **Hot spot (punto caliente)** | Parte del sistema que recibe mucho más tráfico que el resto. | — |
| **Idempotencia** | Repetir una operación da el mismo resultado que hacerla una vez. Clave para reintentos seguros. | Pulsar el botón de «apagar» dos veces. |
| **Inyección de dependencias (DI) / IoC** | En vez de que una clase cree lo que necesita, se lo entregan desde fuera. Facilita cambiar piezas y probar. | El cocinero no compra los ingredientes: se los traen. |
| **Kafka** | Plataforma que guarda un registro ordenado e histórico de eventos que muchos sistemas pueden leer. | — |
| **Lenguaje ubicuo** | Que desarrolladores y expertos del negocio usen las mismas palabras y el código también. | — |
| **Líder / seguidor** | Un nodo coordina; los demás copian. | — |
| **Mapa de contextos (context map)** | Cómo se relacionan los contextos de un sistema. | — |
| **Mediador / bus de eventos** | Intermediario que desacopla componentes. | — |
| **Microservicios** | Aplicación dividida en muchos servicios pequeños que se despliegan y escalan por separado y hablan por red. | Un centro comercial de tiendas independientes en vez de un único gran almacén. |
| **Modelo C4** | Forma de dibujar la arquitectura en cuatro niveles: contexto, contenedores, componentes y código. | Mapas con distinto zoom. |
| **Monolito modular** | Un solo despliegue, pero dividido por dentro en módulos bien separados. Muchas veces es mejor punto de partida que los microservicios. | Un edificio con habitaciones independientes. |
| **Multi-AZ** | Replicado en varios centros de datos de una región. | — |
| **Multi-cloud** | Usar varios proveedores de nube. | — |
| **Multi-región** | Sistema repartido por varias regiones geográficas. | — |
| **Multi-tenant (multiinquilino)** | Una misma aplicación sirve a varios clientes manteniendo sus datos aislados. | Un edificio de apartamentos. |
| **MVC (Modelo-Vista-Controlador)** | Separa los datos (modelo), lo que se ve (vista) y quien coordina (controlador). | Cocina (modelo), sala (vista), camarero (controlador). |
| **MVVM / MVP** | Variantes de MVC muy usadas en interfaces y móviles; cambian cómo la vista se conecta con la lógica. | — |
| **Noisy neighbor (vecino ruidoso)** | Un cliente que consume tantos recursos que perjudica a los demás. | — |
| **OpenAPI (Swagger)** | Archivo estándar que describe una API REST: rutas, datos y respuestas. | — |
| **ORM** | Herramienta que traduce entre objetos del programa y tablas de la base de datos (JPA/Hibernate, Entity Framework). | Un intérprete entre dos idiomas. |
| **Orquestación / coreografía** | Orquestación: un coordinador manda. Coreografía: cada servicio reacciona a eventos. | Un director de orquesta frente a un baile. |
| **Outbox pattern** | Guardar el evento junto al dato en la misma transacción para enviarlo después con seguridad. | — |
| **Particionamiento por rango / por hash** | Dividir datos según un intervalo o el hash de la clave. | — |
| **Patrón de diseño** | Solución probada a un problema habitual (Singleton, Factory, Observer, Strategy…). | Una receta clásica. |
| **Peer-to-peer (P2P)** | Todos los nodos son a la vez clientes y servidores (BitTorrent). | — |
| **Pipes y filtros (pipe and filter)** | Una cadena de pasos donde cada uno transforma los datos y los pasa al siguiente. | La cadena de montaje. |
| **Plugin** | Extensión que añade funciones sin tocar el núcleo. | — |
| **Principios SOLID** | Cinco reglas de diseño orientado a objetos: responsabilidad única, abierto/cerrado, sustitución de Liskov, segregación de interfaces e inversión de dependencias. | — |
| **Proxy / proxy inverso (reverse proxy)** | Proxy: intermediario del cliente. Proxy inverso: intermediario delante de tus servidores (Nginx). | Un portero. |
| **Pub/Sub (publicación/suscripción)** | Quien publica no sabe quién escucha; los suscritos reciben lo que les interesa. | Una revista y sus suscriptores. |
| **Quórum / consenso (Raft, Paxos)** | Acuerdo entre varias máquinas sobre un valor aunque alguna falle. | — |
| **Rate limiting / throttling** | Limitar cuántas peticiones puede hacer alguien en un tiempo. | — |
| **Reintentos con backoff** | Volver a intentar tras un fallo esperando cada vez más. | — |
| **Replicación** | Mantener copias de los datos en varias máquinas. | — |
| **Repositorio (patrón)** | Clase que oculta cómo se guardan los datos y ofrece métodos como `findById`. | — |
| **Requisito funcional** | Qué debe **hacer** el sistema. | — |
| **Requisito no funcional (RNF)** | Qué tan bien lo debe hacer (rapidez, fiabilidad). | — |
| **Resiliencia / tolerancia a fallos** | Capacidad de aguantar y recuperarse de los fallos. | — |
| **REST** | Estilo de API sobre HTTP con recursos (`/products/3`) y verbos (GET, POST, PUT, DELETE). | — |
| **Saga** | Forma de hacer una operación larga entre varios servicios con pasos y pasos de deshacer si algo falla. | — |
| **Serverless (sin servidor)** | Tú subes funciones y el proveedor se encarga de ejecutarlas y escalarlas; pagas por uso. Hay servidores, pero no los gestionas. | Un taxi: no tienes coche, pagas por trayecto. |
| **Service mesh (malla de servicios)** | Capa que gestiona la comunicación entre microservicios (seguridad, reintentos, métricas) sin tocar su código. | — |
| **Sesión pegajosa (sticky session)** | El balanceador envía a un usuario siempre al mismo servidor. | — |
| **Sharding (particionado)** | Repartir los datos de una base entre varias máquinas según un criterio. | Dividir un archivo en cajones por letra inicial. |
| **Sidecar** | Proceso auxiliar que corre pegado a otro y le añade funciones (logs, seguridad). | El sidecar de una moto. |
| **Single-tenant** | Cada cliente tiene su propia instalación. | Una casa por cliente. |
| **SOA (arquitectura orientada a servicios)** | Antecesora de los microservicios: servicios grandes y centralizados (con ESB y SOAP). | — |
| **Split brain** | Dos partes del clúster creen ser el líder a la vez. | — |
| **SPOF (punto único de fallo)** | Pieza cuya caída tumba todo el sistema. | El único puente de un pueblo. |
| **Stateless / stateful** | Sin estado (cada petición es independiente) / con estado (recuerda). | — |
| **Strangler / anticorruption layer** | Capa que traduce entre el sistema nuevo y el viejo para que no se contaminen. | — |
| **Tenant (inquilino)** | Cada cliente u organización dentro de un sistema multi-tenant. | — |
| **Teorema CAP** | En un sistema distribuido, ante una partición de red hay que elegir entre consistencia y disponibilidad. | — |
| **TOGAF** | Marco de arquitectura empresarial muy conocido. | — |
| **Trade-off (compromiso)** | Ganar algo a cambio de perder otra cosa; toda decisión de arquitectura tiene alguno. | — |
| **TTL (time to live)** | Cuánto vive un dato en caché antes de caducar. | — |
| **Twelve-factor app (aplicación de 12 factores)** | Conjunto de buenas prácticas para aplicaciones en la nube (configuración en el entorno, logs como flujo, etc.). | — |
| **Última responsabilidad / «el momento más tardío responsable»** | Retrasar decisiones difíciles de revertir hasta tener información. | — |
| **UML** | Lenguaje estándar para dibujar diagramas de software (clases, secuencia…). | — |
| **Webhook** | El sistema A te llama (envía una petición) cuando ocurre algo, en vez de que tú le preguntes continuamente. | Que te avisen por teléfono en vez de llamar tú cada hora. |
| **WebSocket / SSE** | Conexiones que se mantienen abiertas para que el servidor envíe datos en tiempo real (chats, cotizaciones). | — |

## 9. Patrones de diseño y principios

| Término | Qué es | Como si fuera… |
|---|---|---|
| **Active Record / Data Mapper** | Dos formas de mapear objetos a base de datos: el objeto se guarda a sí mismo / un mapeador lo hace. | — |
| **Builder** | Construye objetos complejos paso a paso. | — |
| **Cargo cult programming** | Copiar código o técnicas sin entender por qué funcionan. | — |
| **Chain of responsibility** | La petición pasa por una cadena hasta que alguien la atiende. | — |
| **Command** | Convierte una acción en un objeto (permite deshacer y encolar). | — |
| **Composición sobre herencia** | Preferir «tiene un» a «es un». | — |
| **Composite** | Trata igual a un objeto y a un grupo de ellos. | — |
| **Convención sobre configuración** | Valores por defecto razonables; configuras solo lo que cambia. | — |
| **Decorator** | Añade comportamiento a un objeto envolviéndolo. | — |
| **Dependency Injection (DI)** | Entregar desde fuera lo que una clase necesita. | — |
| **Diseño por contrato** | Precondiciones, postcondiciones e invariantes explícitas. | — |
| **Factory (fábrica)** | Objeto o método que crea otros objetos. | — |
| **Fail fast** | Fallar pronto y con claridad al detectar un problema. | — |
| **Gold plating** | Añadir extras que nadie pidió. | — |
| **Iterator** | Recorre una colección sin conocer su estructura. | — |
| **Optimización prematura** | Optimizar antes de saber que hace falta. | — |
| **Over-engineering** | Hacer algo más complejo de lo que el problema exige. | — |
| **Patrones creacionales** | Cómo crear objetos: Singleton, Factory, Abstract Factory, Builder, Prototype. | — |
| **Patrones de comportamiento** | Cómo colaboran: Observer, Strategy, Command, State, Template Method, Iterator, Visitor, Mediator, Memento, Chain of Responsibility. | — |
| **Patrones estructurales** | Cómo combinar objetos: Adapter, Decorator, Facade, Proxy, Composite, Bridge, Flyweight. | — |
| **Patrones GoF** | Los 23 patrones clásicos del libro de la «banda de los cuatro». | — |
| **Principio abierto/cerrado (OCP)** | Extender sin modificar. | — |
| **Principio de inversión de dependencias (DIP)** | Lo importante no depende de los detalles. | — |
| **Principio de menor sorpresa** | El código debe comportarse como se espera. | — |
| **Principio de responsabilidad única (SRP)** | Una clase, una razón para cambiar. | — |
| **Principio de segregación de interfaces (ISP)** | Muchas interfaces pequeñas mejor que una gigante. | — |
| **Programación defensiva** | Comprobar entradas y casos raros. | — |
| **Proxy** | Representante que controla el acceso a otro objeto. | — |
| **Regla de tres** | Repite dos veces; a la tercera, abstrae. | — |
| **Separation of Concerns** | Cada parte se ocupa de una sola cosa. | — |
| **Service locator** | Pedir las dependencias a un registro central (suele considerarse antipatrón). | — |
| **SOLID** | S: una responsabilidad. O: abierto a extensión y cerrado a modificación. L: sustitución de Liskov. I: interfaces pequeñas. D: depender de abstracciones. | — |
| **Specification** | Regla de negocio encapsulada y combinable. | — |
| **State** | Un objeto cambia de comportamiento según su estado. | — |
| **Strategy** | Intercambia algoritmos en ejecución. | — |
| **Template Method** | Define los pasos de un algoritmo y deja que las subclases rellenen algunos. | — |
| **Unit of Work** | Agrupa cambios y los guarda de una vez. | — |

## 10. Backend y APIs

| Término | Qué es | Como si fuera… |
|---|---|---|
| **ASP.NET Core / Minimal APIs / Razor / Blazor** | Framework web de .NET / APIs con muy poco código / plantillas de página / interfaz con C#. | — |
| **Auditoría (audit log)** | Registro de quién hizo qué y cuándo. | — |
| **Backend** | La parte del sistema que **no se ve**: lógica, datos, seguridad, integraciones. | La cocina y la oficina del restaurante. |
| **Bulkhead (mamparo)** | Aislar recursos para que un fallo no hunda todo. | — |
| **Cabeceras (headers)** | Metadatos de una petición o respuesta (`Content-Type`, `Authorization`). | — |
| **Circuit breaker** | Dejar de llamar a un servicio que falla. | — |
| **Classpath** | Dónde busca Java sus clases. | — |
| **Código de estado HTTP** | 200 OK, 201 creado, 204 sin contenido, 301/302 redirección, 400 petición mala, 401 no autenticado, 403 prohibido, 404 no encontrado, 409 conflicto, 422 no procesable, 429 demasiadas peticiones, 500 error del servidor, 502/503/504 puerta de enlace / no disponible / tiempo agotado. | — |
| **Compensación** | Acción que deshace un paso anterior. | — |
| **Configuración centralizada** | Un lugar único para configurar muchos servicios. | — |
| **Configuración externalizada** | Sacar configuraciones del código. | — |
| **Connection pool** | Conjunto de conexiones a base de datos reutilizables. | — |
| **Contenedor de servlets** | Parte de un servidor Java que ejecuta servlets. | — |
| **Content negotiation** | El cliente indica qué formato prefiere. | — |
| **Contract testing (Pact)** | Pruebas que verifican que el cliente y el servidor cumplen el mismo contrato. | — |
| **Contrato de API (contract first / code first)** | Escribir primero la descripción o primero el código. | — |
| **Controlador (controller)** | Clase que recibe las peticiones. | — |
| **Cron job / scheduler / batch job** | Tarea programada / planificador / tarea por lotes. | — |
| **Cuerpo (body)** | Los datos que se envían. | — |
| **Cursor / offset** | Dos formas de paginar: por marcador / por posición. | — |
| **Dead letter / poison message** | Mensaje que no se procesa y bloquea. | — |
| **DTO / ViewModel** | Objeto que transporta datos hacia fuera o dentro de la API. | — |
| **Entity Framework (EF) / Dapper** | ORM de .NET / micro-ORM más ligero. | — |
| **ETL job** | Tarea que mueve y transforma datos. | — |
| **Feature toggle (backend)** | Activar funciones por configuración. | — |
| **Filtrado / ordenación** | Parámetros para pedir solo algunos datos y en cierto orden. | — |
| **Filtro / interceptor** | Parecido a middleware en Java/Spring. | — |
| **Framework web** | Estructura para construir aplicaciones web (Spring Boot, ASP.NET Core, Express, Django, FastAPI, Rails, Laravel). | — |
| **Garbage collection pause** | Pausa del recolector de basura. | — |
| **Graceful shutdown** | Apagar atendiendo antes las peticiones en curso. | — |
| **GraphQL / query / mutation / resolver** | Lenguaje de consulta; lectura; escritura; función que obtiene un campo. | — |
| **gRPC / Protocol Buffers** | RPC moderno, rápido y con contratos. | — |
| **HATEOAS** | REST con enlaces a las acciones posibles dentro de las respuestas. | — |
| **Health check / readiness / liveness** | «¿Estás vivo?» / «¿Estás listo para recibir tráfico?». | — |
| **Heap / stack** | Zonas de memoria: montón (objetos) / pila (llamadas y variables locales). | — |
| **Hot path** | Trozo de código que se ejecuta muchísimo. | — |
| **Hot reload / live reload** | Recargar al guardar cambios sin reiniciar. | — |
| **Idempotency key** | Clave que evita que una operación repetida se ejecute dos veces (pagos). | — |
| **Idempotent consumer** | Receptor que tolera recibir un mensaje repetido. | — |
| **JAR / WAR / DLL** | Paquetes compilados de Java / aplicación web Java / biblioteca de .NET. | — |
| **JSON** | Formato de texto para datos (`{"nombre":"Ana"}`). | — |
| **Lazy loading / eager loading** | Cargar datos relacionados bajo demanda / de golpe. | — |
| **LINQ** | Consultas integradas en C#. | — |
| **Localización de servicios (service discovery)** | Cómo se encuentran los servicios entre sí. | — |
| **Mapeo (mapper)** | Convertir entre entidades y DTO. | — |
| **Maven / Gradle** | Gestores de proyectos y dependencias de Java. | — |
| **Método HTTP** | GET (leer), POST (crear), PUT (reemplazar), PATCH (modificar en parte), DELETE (borrar). | — |
| **Middleware** | Código que se ejecuta **antes o después** de cada petición (autenticar, registrar). | Un control de seguridad en el pasillo. |
| **Mock server** | Servidor falso que responde como la API real. | — |
| **Multitenancy en backend** | Resolver el tenant de cada petición y aislar datos. | — |
| **N+1 queries** | Fallo típico: una consulta inicial y luego una más por cada fila. | — |
| **Nivel de madurez de Richardson** | Escala de 0 a 3 sobre cuánto se parece una API a REST de verdad. | — |
| **NuGet** | Gestor de paquetes de .NET. | — |
| **OpenAPI / Swagger / Postman** | Descripción estándar de APIs / interfaz para verlas / herramienta para probarlas. | — |
| **Out of memory (OOM)** | Sin memoria. | — |
| **Paginación** | Devolver los datos por trozos (página 2 de 10). | — |
| **Perfil (profile)** | Conjunto de configuraciones para un entorno (dev, prod). | — |
| **Petición (request) / respuesta (response)** | Lo que envía el cliente / lo que devuelve el servidor. | — |
| **Pipeline de petición** | Cadena de middlewares por la que pasa cada petición. | — |
| **Polling / long polling** | Preguntar cada cierto tiempo / mantener la pregunta abierta hasta que haya novedad. | — |
| **Profiler / profiling** | Herramienta que mide dónde gasta tiempo y memoria un programa. | — |
| **Protobuf** | Formato binario compacto de Google. | — |
| **Query string / parámetros de ruta** | `?q=ana` / `/users/7`. | — |
| **Rate limiting / cuota / throttling** | Límite de peticiones. | — |
| **Repositorio (repository)** | Clase que accede a los datos. | — |
| **Retry con backoff exponencial** | Reintentar esperando cada vez más. | — |
| **RPC** | Llamar a una función remota como si fuera local. | — |
| **Ruta (route)** | Asociación entre una dirección y el código que la atiende. | — |
| **Secrets manager** | Servicio para guardar claves de forma segura. | — |
| **Serialización / deserialización** | Convertir objetos a JSON y al revés. | — |
| **Server-Sent Events (SSE)** | El servidor envía eventos continuos al navegador. | — |
| **Servicio (service)** | Clase con la lógica de negocio. | — |
| **Servidor de aplicaciones** | Entorno que ejecuta aplicaciones de servidor (Tomcat, Jetty, WildFly). | — |
| **Servidor web** | Programa que atiende peticiones HTTP (Nginx, Apache, Kestrel, Tomcat). | — |
| **SOAP / WSDL** | Protocolo antiguo basado en XML y su descripción. | — |
| **Soft delete / hard delete** | Marcar como borrado / borrar de verdad. | — |
| **Spring Boot** | Framework Java que arranca aplicaciones con poca configuración. | — |
| **Spring Data / JPA / Hibernate** | Acceso a datos en Java; estándar de mapeo; la implementación más conocida. | — |
| **Stack overflow** | Pila agotada por demasiada recursión. | — |
| **Thread dump / heap dump** | Foto de los hilos / de la memoria para diagnosticar. | — |
| **Timeout** | Tiempo máximo de espera. | — |
| **Transacción distribuida / 2PC** | Confirmar a la vez en varios sistemas (costoso). | — |
| **tRPC** | Llamadas tipadas entre cliente y servidor TypeScript. | — |
| **Validación** | Comprobar que los datos recibidos son correctos. | — |
| **Versionado de API** | Mantener varias versiones a la vez (`/v1`, `/v2`). | — |
| **WebSocket** | Conexión bidireccional continua. | — |
| **XML** | Formato de datos con etiquetas. | — |
| **YAML / TOML** | Formatos legibles para configuración. | — |

## 11. Web y frontend

| Término | Qué es | Como si fuera… |
|---|---|---|
| **Accesibilidad (a11y)** | Que cualquier persona, también con discapacidades, pueda usarlo. | — |
| **Accesibilidad (a11y) / ARIA / WCAG / lector de pantalla** | Uso para todos / atributos de accesibilidad / pautas de referencia / software que lee la pantalla. | — |
| **Angular / Svelte / Solid / Lit** | Otros frameworks y librerías de interfaz. | — |
| **Atomic design** | Organizar componentes en átomos, moléculas, organismos, plantillas y páginas. | — |
| **Binding (enlace de datos)** | Conectar datos con la interfaz (unidireccional o bidireccional). | — |
| **BOM / window** | Objetos del navegador (ventana, historial, ubicación). | — |
| **Breakpoint** | Ancho a partir del cual cambia el diseño. | — |
| **Bundle / chunk** | Paquete final de JavaScript / trozo del paquete. | — |
| **Bundler / transpilador** | Bundler: junta y optimiza archivos (Vite). Transpilador: traduce código moderno a otro compatible. | — |
| **CDN / caché del navegador / Service Worker** | Red de distribución / almacenamiento local / script que permite uso sin conexión. | — |
| **Componente** | Pieza reutilizable de interfaz (botón, tarjeta). | Una pieza de Lego. |
| **Composition API / Options API** | Dos formas de escribir componentes en Vue. | — |
| **Context** | Forma de compartir datos sin pasarlos de padre a hijo. | — |
| **Contraste / foco / navegación por teclado** | Legibilidad de colores / elemento activo / usar sin ratón. | — |
| **Cookie (HttpOnly, Secure, SameSite)** | Pequeño dato del navegador y sus protecciones. | — |
| **Cookie / localStorage** | Pequeños datos guardados en el navegador. | — |
| **Core Web Vitals** | Métricas de Google sobre carga, interactividad y estabilidad visual. | — |
| **Core Web Vitals (LCP, INP, CLS)** | Métricas de carga, respuesta y estabilidad visual. | — |
| **CORS** | Regla del navegador que limita qué sitios pueden pedir datos a tu API. | — |
| **CORS / preflight** | Regla de acceso entre sitios y su consulta previa. | — |
| **CSP (Content Security Policy)** | Reglas que limitan qué código puede ejecutar la página. | — |
| **Dark mode / tema** | Modo oscuro / conjunto de estilos. | — |
| **Debounce / throttle** | Esperar a que termine una ráfaga de eventos / limitar la frecuencia. | — |
| **Design system (sistema de diseño)** | Biblioteca de componentes, colores y reglas comunes para toda la marca. | — |
| **Design tokens** | Valores de diseño con nombre (colores, espacios). | — |
| **Directivas** | Atributos especiales en plantillas (`v-if`, `*ngIf`). | — |
| **DOM** | Representación en árbol de la página que el navegador manipula. | — |
| **Edge rendering** | Generar páginas en servidores cercanos al usuario. | — |
| **Efecto (effect) / ciclo de vida** | Código que reacciona a cambios / fases de un componente (montar, actualizar, desmontar). | — |
| **ESLint / Prettier / Biome / Oxlint** | Linter y formateadores de JavaScript. | — |
| **Estado (state) / props** | Estado: datos que cambian dentro de un componente. Props: datos que recibe de fuera. | — |
| **Estado global / store** | Datos compartidos por toda la aplicación (Redux, Pinia, Zustand, Context). | — |
| **Estado vacío / de error / de carga** | Pantallas que deben diseñarse además del caso feliz. | — |
| **Evento / event bubbling / delegación** | Aviso de una acción / el evento sube por los padres / escuchar en un padre para todos los hijos. | — |
| **Flash of unstyled content (FOUC)** | Parpadeo de la página sin estilos al cargar. | — |
| **Flexbox / Grid** | Sistemas de maquetación de CSS. | — |
| **Flux / Redux** | Patrón de flujo de datos en un solo sentido con acciones y reductores. | — |
| **Formulario controlado** | Formulario cuyos valores gobierna el estado de la aplicación. | — |
| **Framework vs. librería de UI** | React es librería; Angular, Next o Nuxt son frameworks. | — |
| **Frontend / backend / full-stack** | Frontend: lo que ve el usuario. Backend: lo que hay detrás. Full-stack: ambos. | — |
| **Headless CMS** | Gestor de contenidos sin la parte visual; entrega los datos por API. | — |
| **Headless UI** | Componentes con comportamiento pero sin estilo. | — |
| **Hidratación (hydration)** | Hacer interactiva en el navegador una página que llegó ya dibujada del servidor. | — |
| **Hidratación parcial / islands** | Hacer interactivas solo algunas zonas de la página. | — |
| **HMR (hot module replacement)** | Actualizar el módulo cambiado sin recargar la página. | — |
| **Hooks** | Funciones de React (`useState`, `useEffect`) para añadir estado y efectos a componentes. | — |
| **i18n / l10n** | Internacionalización (preparar para varios idiomas) y localización (adaptar a uno). | — |
| **i18n / l10n / RTL** | Internacionalización / localización / escritura de derecha a izquierda. | — |
| **Infinite scroll / paginación** | Cargar más al bajar / páginas numeradas. | — |
| **ISR (regeneración estática incremental)** | Páginas estáticas que se regeneran en segundo plano. | — |
| **Jamstack / sitio estático** | Sitios de archivos prefabricados servidos desde una CDN, con funciones para lo dinámico. | — |
| **JSX / TSX / plantilla (template)** | Sintaxis que mezcla HTML y código / su versión con tipos / el HTML de un componente. | — |
| **Keys (claves)** | Identificadores para que el framework distinga elementos de una lista. | — |
| **Layout shift** | Saltos visuales mientras carga. | — |
| **Lazy loading / code splitting** | Cargar partes de la aplicación solo cuando se necesitan. | — |
| **Lighthouse** | Herramienta que audita rendimiento y accesibilidad. | — |
| **localStorage / sessionStorage / IndexedDB** | Almacenamiento del navegador. | — |
| **Media query / container query** | Estilos según la pantalla / el contenedor. | — |
| **Memoización (memo, useMemo)** | Evitar recalcular o redibujar si nada cambió. | — |
| **Micro-frontends** | Dividir la interfaz en partes que equipos distintos construyen y despliegan por separado. | — |
| **Micro-frontends / Module Federation** | Interfaz dividida en partes independientes. | — |
| **Minificación** | Reducir el tamaño del código quitando espacios y acortando nombres. | — |
| **Mobile first** | Diseñar primero para móvil. | — |
| **Módulos ES (`import`/`export`) / CommonJS (`require`)** | Dos sistemas de módulos de JavaScript. | — |
| **Monorepo (Nx, Turborepo)** | Varios proyectos en un repositorio con herramientas de construcción. | — |
| **Next.js / Nuxt / Remix / SvelteKit / Astro** | Frameworks de aplicaciones web de nueva generación. | — |
| **npm / pnpm / yarn / Bun / Deno** | Gestores de paquetes y entornos de ejecución JavaScript. | — |
| **Open Graph** | Datos que se muestran al compartir un enlace. | — |
| **Optimistic UI** | Mostrar el resultado antes de que el servidor confirme. | — |
| **Polyfill** | Código que añade una función moderna a navegadores antiguos. | — |
| **Prefetch / preload** | Cargar de antemano lo que se prevé necesitar. | — |
| **Prop drilling** | Pasar datos por muchos componentes intermedios. | — |
| **PWA** | Sitio web que se comporta como aplicación instalable y funciona sin conexión. | — |
| **PWA / manifest / push notification** | Aplicación web instalable / su descripción / notificación enviada al usuario. | — |
| **Reducer / action** | Función que calcula el nuevo estado / orden que describe un cambio. | — |
| **Renderizado / repaint / reflow** | Dibujar la página / redibujar / recalcular posiciones. | — |
| **Responsive / mobile first** | Responsive: se adapta a cualquier pantalla. Mobile first: se diseña primero para móvil. | — |
| **Rutas / router / enrutamiento** | Relacionar direcciones con pantallas. | — |
| **Semántica HTML** | Usar las etiquetas por su significado (`<nav>`, `<main>`, `<button>`). | — |
| **SEO** | Optimización para aparecer en buscadores. | — |
| **SEO técnico / meta tags / sitemap / canonical** | Cómo ayudar a los buscadores. | — |
| **Server Components** | Componentes que se ejecutan solo en el servidor (React). | — |
| **SFC (Single File Component)** | Componente en un solo archivo (Vue). | — |
| **Shadow DOM** | DOM encapsulado dentro de un componente. | — |
| **Signals** | Valores reactivos que avisan a quien los usa cuando cambian. | — |
| **Skeleton / spinner** | Esquema de carga / indicador de espera. | — |
| **Source map** | Archivo que relaciona el código final con el original para depurar. | — |
| **SPA / MPA** | SPA: una sola página que cambia sin recargar (React, Vue). MPA: varias páginas tradicionales. | — |
| **SSR / CSR / SSG** | SSR: la página se genera en el servidor en cada visita. CSR: la genera el navegador. SSG: se genera de antemano al construir el sitio. | — |
| **Storybook** | Herramienta para ver y probar componentes aislados. | — |
| **Streaming SSR** | El servidor envía la página por trozos. | — |
| **Tailwind / CSS Modules / CSS-in-JS / BEM / Sass** | Formas de organizar y escribir estilos. | — |
| **Toast / modal / dropdown / tooltip** | Mensaje breve / ventana encima / lista desplegable / ayuda emergente. | — |
| **Transpilar (Babel, SWC)** | Traducir código moderno a otro soportado. | — |
| **Tree shaking** | Eliminar del paquete final el código que no se usa. | — |
| **TTFB / FCP / TTI** | Tiempo hasta el primer byte / primer contenido / interactividad. | — |
| **TypeScript / tipos / interfaz / genéricos / `any` / `unknown`** | JavaScript con tipos; descripciones de forma; tipos parametrizados; «cualquier cosa» (evitar) / «desconocido» (seguro). | — |
| **Unidades relativas (rem, em, vw, %)** | Tamaños que se adaptan. | — |
| **Validación en cliente / en servidor** | Siempre se necesita la del servidor; la del cliente mejora la experiencia. | — |
| **Variables CSS (custom properties)** | Valores reutilizables en CSS. | — |
| **Viewport** | Área visible del navegador. | — |
| **Virtual DOM / reactividad** | Técnicas para actualizar solo lo que cambió en la pantalla. | — |
| **Vite / Webpack / esbuild / Rollup / Turbopack** | Herramientas de empaquetado y desarrollo. | — |
| **Web Components** | Estándar para crear componentes propios en el navegador. | — |
| **WebAssembly (WASM)** | Formato para ejecutar en el navegador código compilado casi a velocidad nativa. | — |
| **WebAssembly / WebGL / WebGPU / Canvas / SVG** | Código casi nativo / gráficos 3D / gráficos con GPU / dibujo / gráficos vectoriales. | — |
| **WebRTC / MediaStream** | Comunicación en tiempo real (vídeo, audio). | — |
| **XSS / CSRF / clickjacking** | Ataques web frecuentes. | — |
| **Z-index / stacking context** | Orden de apilado de elementos. | — |

## 12. Móvil, escritorio, embebido y videojuegos

| Término | Qué es | Como si fuera… |
|---|---|---|
| **Activity / Fragment / Intent / Service / BroadcastReceiver** | Piezas de Android: pantalla / parte de pantalla / petición entre componentes / tarea en segundo plano / receptor de eventos del sistema. | — |
| **ADB** | Herramienta para comunicarse con dispositivos Android. | — |
| **Android / iOS** | Los dos sistemas móviles principales. | — |
| **APK / AAB / IPA** | Paquetes de instalación de Android / de Google Play / de iOS. | — |
| **App nativa / híbrida / multiplataforma / web app** | Hecha para una plataforma / mezcla web y nativo / un código para varias / en el navegador. | — |
| **Bootloader** | Programa que arranca el dispositivo. | — |
| **Build de juego / parche / DLC** | Versión / actualización / contenido adicional. | — |
| **Ciclo de vida de la app** | Estados: iniciar, pausar, reanudar, detener, destruir. | — |
| **Colisión / física / hitbox** | Detección de choques / simulación / zona de impacto. | — |
| **Cross-compilation** | Compilar en un equipo para otro distinto. | — |
| **Deep link / universal link** | Enlace que abre una pantalla concreta de la app. | — |
| **Driver / libusb** | Controlador / biblioteca de acceso USB. | — |
| **Electron / Tauri** | Aplicaciones de escritorio con tecnología web / versión ligera con Rust. | — |
| **Emulador / simulador / dispositivo real** | Móvil simulado / en el ordenador / físico. | — |
| **Entity-Component-System (ECS)** | Arquitectura de juegos por entidades, componentes y sistemas. | — |
| **Escritorio (desktop)** | Aplicaciones para ordenador: WinForms, WPF, WinUI, Qt, GTK, Swing, JavaFX, Electron, Tauri. | — |
| **Event loop de interfaz / hilo de UI** | Bucle que atiende la interfaz. | — |
| **Firma de la app / keystore** | Certificado que identifica al autor. | — |
| **Firmware** | Software grabado en un dispositivo. | — |
| **Flutter / Dart** | Framework multiplataforma de Google y su lenguaje. | — |
| **Frame / FPS / delta time** | Imagen / fotogramas por segundo / tiempo entre fotogramas. | — |
| **Game loop** | Bucle que actualiza y dibuja el juego muchas veces por segundo. | — |
| **Gateway IoT / edge** | Pasarela entre sensores y la nube / procesamiento local. | — |
| **Google Play / App Store / TestFlight** | Tiendas y distribución de pruebas. | — |
| **GPIO** | Pines de entrada y salida de propósito general. | — |
| **Gradle (Android)** | Sistema de construcción. | — |
| **I2C / SPI / UART / CAN** | Protocolos de comunicación entre chips. | — |
| **Instalador (MSI, MSIX, DEB, DMG)** | Paquete de instalación. | — |
| **Interrupción (interrupt)** | Aviso del hardware que detiene lo que se hace. | — |
| **IoT** | Internet de las cosas: dispositivos conectados. | — |
| **Jetpack Compose / SwiftUI** | Interfaces declarativas modernas de Android e iOS. | — |
| **JTAG / SWD / depurador** | Interfaces para depurar hardware. | — |
| **Kotlin / Java / Swift / Objective-C** | Lenguajes nativos de Android e iOS. | — |
| **Manifest (AndroidManifest / Info.plist)** | Archivo que declara permisos y componentes. | — |
| **Microcontrolador (MCU)** | Chip con procesador y memoria (Arduino, ESP32, STM32). | — |
| **Netcode / lag / rollback** | Red de un juego / retardo / corrección de predicciones. | — |
| **Notificación push / FCM / APNs** | Mensaje enviado al dispositivo. | — |
| **Offline first** | Funciona sin conexión y sincroniza después. | — |
| **OTA (over the air)** | Actualización inalámbrica. | — |
| **Permiso (runtime)** | Autorización que el usuario concede (cámara, ubicación). | — |
| **Pipeline de assets** | Cómo se preparan imágenes y modelos. | — |
| **Placa de desarrollo (Raspberry Pi, Arduino)** | Placas para prototipos. | — |
| **Platform channel / puente (bridge)** | Comunicación entre Flutter o React Native y el código nativo. | — |
| **ProGuard / R8** | Reducen y ofuscan el código de Android. | — |
| **Puerto serie / USB / HID / Bluetooth LE** | Comunicación con periféricos. | — |
| **PWM / ADC / DAC** | Señal modulada / conversión analógico-digital / digital-analógico. | — |
| **React Native / Kotlin Multiplatform / .NET MAUI / Xamarin / Ionic / Capacitor** | Otras tecnologías multiplataforma. | — |
| **Responsive / adaptativo** | Se ajusta a distintos tamaños de pantalla. | — |
| **RTOS** | Sistema operativo de tiempo real. | — |
| **Sensores (acelerómetro, giroscopio, GPS)** | Hardware del dispositivo accesible por API. | — |
| **Sincronización / resolución de conflictos** | Igualar datos entre dispositivo y servidor. | — |
| **Sistema embebido** | Ordenador pequeño dentro de otro aparato. | — |
| **Sprite / tilemap / mesh / shader / textura** | Imagen 2D / mapa de baldosas / malla 3D / programa gráfico / imagen de superficie. | — |
| **SQLite / Room / Core Data / Realm** | Bases locales móviles. | — |
| **StatelessWidget / StatefulWidget** | Sin estado / con estado. | — |
| **Tiempo real duro / blando** | Plazo imprescindible / deseable. | — |
| **Videojuegos: motor (engine)** | Unity, Unreal, Godot: base para crear juegos. | — |
| **ViewModel / LiveData / Flow** | Piezas de arquitectura de Android. | — |
| **Widget** | Pieza de interfaz (en Flutter, todo es un widget). | — |
| **WPF / XAML / MVVM** | Interfaces de Windows / lenguaje de marcado / patrón de enlace de datos. | — |
| **XML layout / Storyboard** | Formas clásicas de diseñar pantallas. | — |

## 13. Bases de datos y datos

| Término | Qué es | Como si fuera… |
|---|---|---|
| **A/B testing / significancia estadística** | Experimento entre dos variantes / confianza en que la diferencia no es azar. | — |
| **ACID** | Atomicidad, consistencia, aislamiento y durabilidad. | — |
| **Agregación (pipeline de agregación)** | Cadena de etapas para transformar documentos. | — |
| **Backup completo / incremental / diferencial / PITR** | Copias y recuperación a un instante exacto. | — |
| **Base de datos en memoria** | Datos en RAM para máxima velocidad. | — |
| **Base de datos relacional (SQL)** | Datos en tablas relacionadas entre sí, consultadas con SQL (PostgreSQL, MySQL, SQL Server, Oracle). | Hojas de cálculo conectadas. |
| **Base de datos vectorial** | Guarda embeddings para buscar por parecido de significado. | — |
| **Base de documentos** | Guarda documentos tipo JSON (MongoDB). | — |
| **Base de grafos** | Optimizada para relaciones (Neo4j). | Un mapa de conexiones. |
| **Base de series temporales** | Optimizada para datos ordenados por tiempo (métricas). | — |
| **Base de series temporales (TSDB)** | Optimizada para datos con marca de tiempo. | — |
| **Base vectorial (pgvector, Pinecone, Qdrant, Chroma)** | Para embeddings. | — |
| **Batch / streaming** | Batch: procesar por lotes (cada noche). Streaming: procesar al instante según llegan. | — |
| **Batch / streaming / micro-batch** | Por lotes / continuo / lotes muy pequeños. | — |
| **BI (inteligencia de negocio) / dashboard / KPI** | Informes y paneles / indicadores. | — |
| **Big data** | Volúmenes de datos tan grandes que no caben en herramientas corrientes. | — |
| **Bloqueo (lock) / deadlock / MVCC** | Reserva de datos / interbloqueo / versiones para leer sin bloquear. | — |
| **BSON** | Formato binario de documentos de MongoDB. | — |
| **Búsqueda de texto completo** | Buscar palabras dentro de textos largos. | — |
| **Calidad de datos / linaje / catálogo / gobierno** | Fiabilidad / de dónde viene / inventario / reglas de uso. | — |
| **CDC** | Captura de cambios. | — |
| **CDC (captura de cambios)** | Detectar y enviar cada cambio que ocurre en una base de datos. | — |
| **Clave primaria (PK) / clave foránea (FK)** | Identifica una fila / apunta a otra tabla. | — |
| **Clave-valor / caché en memoria** | Guarda pares clave → valor muy rápido (Redis). | Un diccionario. |
| **Copia de seguridad (backup) / RPO / RTO** | RPO: cuántos datos puedes permitirte perder. RTO: cuánto puedes tardar en recuperar. | — |
| **Correlación / causalidad** | Que dos cosas varíen juntas no significa que una cause la otra. | — |
| **Data contract** | Acuerdo sobre la forma de los datos entre productor y consumidor. | — |
| **Data engineer / data analyst / data scientist** | Construye flujos de datos / analiza e informa / modela y predice. | — |
| **Data lake / lakehouse / Delta / Iceberg** | Datos en bruto / mezcla / formatos de tablas abiertas. | — |
| **Data mart** | Subconjunto del almacén para un área. | — |
| **Data mesh / data product** | Datos gestionados como producto por cada equipo. | — |
| **Data warehouse (BigQuery, Snowflake, Redshift)** | Almacén para análisis. | — |
| **Data warehouse / data lake / lakehouse** | Warehouse: almacén de datos ordenados para análisis. Lake: «lago» con datos en bruto. Lakehouse: mezcla de ambos. | — |
| **Dato personal / RGPD** | Cualquier dato que identifique a una persona. El RGPD es la ley europea que regula su tratamiento. | — |
| **Datos faltantes / imputación** | Valores vacíos / rellenarlos. | — |
| **Datos sintéticos** | Datos generados artificialmente. | — |
| **DBA (administrador de bases de datos)** | Persona que cuida el rendimiento, copias y seguridad de las bases. | — |
| **Dimensión / hecho** | Contexto (cliente, fecha) / medidas (importe). | — |
| **Esquema / migración / seed** | Estructura / cambio versionado / datos iniciales. | — |
| **Esquema / migración de base de datos** | Esquema: estructura de las tablas. Migración: cambio versionado de ese esquema. | — |
| **Esquema flexible (schema-less)** | Documentos sin estructura fija obligatoria. | — |
| **Estadística: media, mediana, moda, desviación** | Medidas básicas de un conjunto de datos. | — |
| **ETL / ELT** | Extraer, transformar y cargar datos hacia un almacén. ELT carga primero y transforma después. | — |
| **ETL / ELT / reverse ETL** | Extraer-transformar-cargar / cargar y luego transformar / devolver datos a las aplicaciones. | — |
| **Flyway / Liquibase / EF Migrations** | Herramientas de migraciones. | — |
| **Full table scan** | Leer toda la tabla. | — |
| **GROUP BY / HAVING / ORDER BY / LIMIT** | Agrupar / filtrar grupos / ordenar / limitar. | — |
| **Hoja de cálculo / CSV / Parquet / Avro / JSON Lines** | Formatos y herramientas de datos. | — |
| **Índice** | Estructura que acelera las búsquedas. | El índice alfabético de un libro. |
| **Índice (B-tree, hash, GIN, compuesto)** | Acelera búsquedas; tipos. | — |
| **Ingeniería de características (feature engineering)** | Preparar las variables que usa un modelo. | — |
| **JOIN (INNER, LEFT, RIGHT, FULL, CROSS)** | Unir tablas. | — |
| **Kafka / Flink / Spark / Beam** | Plataformas de eventos y procesamiento. | — |
| **Master data / MDM** | Datos maestros únicos de la empresa. | — |
| **Modelo dimensional / estrella / copo de nieve** | Diseño para análisis: hechos y dimensiones. | — |
| **MongoDB / Redis / Cassandra / DynamoDB / Neo4j / Elasticsearch** | Ejemplos de cada familia. | — |
| **Muestreo / sesgo de muestra** | Elegir una parte / muestra no representativa. | — |
| **NewSQL** | Bases relacionales distribuidas. | — |
| **Nivel de aislamiento** | Cuánto se ven entre sí las transacciones (read committed, repeatable read, serializable). | — |
| **Normalización** | Organizar las tablas para no repetir datos. | — |
| **Normalización (1FN, 2FN, 3FN) / desnormalización** | Evitar repetición / repetir a propósito por rendimiento. | — |
| **Normalización / estandarización** | Poner los valores en la misma escala. | — |
| **NoSQL** | Familia de bases no basadas en tablas: documentos, clave-valor, grafos, columnas. | — |
| **NoSQL / documental / clave-valor / columnar / grafos** | Familias de bases no relacionales. | — |
| **OLTP / OLAP** | OLTP: operaciones del día a día (ventas). OLAP: análisis de grandes volúmenes (informes). | — |
| **OLTP / OLAP / cubo** | Transaccional / analítico / datos multidimensionales. | — |
| **ORM / consulta nativa / query builder** | Mapeo objetos-tablas / SQL directo / construir consultas con código. | — |
| **Pandas / NumPy / Polars / Jupyter** | Bibliotecas y cuadernos de análisis en Python. | — |
| **Particionado / sharding** | Dividir tablas / dividir entre servidores. | — |
| **Pipeline de datos** | Cadena automática que mueve y transforma datos de un lugar a otro. | — |
| **Pipeline de datos / orquestador (Airflow, Dagster)** | Cadena de procesamiento / programa que la coordina. | — |
| **Plan de ejecución (EXPLAIN)** | Cómo ejecutará la base una consulta. | — |
| **Pool de conexiones** | Conexiones reutilizables. | — |
| **Power BI / Tableau / Looker / Metabase / Grafana** | Herramientas de visualización. | — |
| **Privacidad diferencial** | Añadir ruido para proteger a las personas de un dataset. | — |
| **Procedimiento almacenado / función / trigger** | Código dentro de la base / función propia / se dispara con un cambio. | — |
| **Relación 1:1, 1:N, N:M** | Uno a uno, uno a muchos, muchos a muchos (tabla intermedia). | — |
| **Replicación (primaria/réplica) / lectura en réplica** | Copias de la base. | — |
| **Restricción (constraint)** | Regla: NOT NULL, UNIQUE, CHECK, FK. | — |
| **SELECT / INSERT / UPDATE / DELETE** | Leer / insertar / modificar / borrar. | — |
| **SGBD (DBMS)** | Programa que gestiona una base de datos (PostgreSQL, MySQL, SQL Server, Oracle, SQLite, MongoDB). | — |
| **SQL (DDL, DML, DCL, TCL)** | Definición / manipulación / permisos / transacciones. | — |
| **Subconsulta / CTE / ventana (window function)** | Consulta dentro de otra / consulta con nombre / cálculo sobre un grupo de filas. | — |
| **Tabla / fila / columna / registro / campo** | Estructura de datos relacional. | — |
| **Teorema CAP / consistencia eventual** | Compromisos en sistemas distribuidos. | — |
| **Transacción** | Conjunto de operaciones que se hacen todas o ninguna. | Una transferencia: se resta de una cuenta y se suma a la otra, o no ocurre nada. |
| **Transacción / COMMIT / ROLLBACK / savepoint** | Operaciones todo o nada / confirmar / deshacer. | — |
| **Vacuum / mantenimiento** | Limpieza interna (PostgreSQL). | — |
| **Valor atípico (outlier)** | Dato muy distinto del resto. | — |
| **Vista / vista materializada** | Consulta guardada / con resultados guardados. | — |

## 14. Inteligencia artificial y aprendizaje automático

| Término | Qué es | Como si fuera… |
|---|---|---|
| **Agente (agent)** | IA que planifica y ejecuta acciones usando herramientas, no solo responde. | — |
| **Agente de programación** | Agente que lee el proyecto, edita archivos, ejecuta pruebas y propone cambios. | — |
| **Agente de uso de ordenador (computer use)** | Agente que maneja un entorno de escritorio como una persona. | — |
| **Alineamiento (alignment)** | Que el modelo haga lo que queremos y respete valores humanos. | — |
| **Alucinación** | La IA inventa algo falso con apariencia convincente (una función que no existe). | — |
| **API de un modelo / SDK** | Interfaz para usar el modelo desde código. | — |
| **Aprendizaje automático (machine learning, ML)** | Sistemas que **aprenden patrones a partir de datos** en vez de seguir reglas escritas a mano. | Aprender a reconocer perros viendo miles de fotos, no leyendo una definición. |
| **Aprendizaje profundo (deep learning)** | ML con redes neuronales de muchas capas. | — |
| **Aprendizaje supervisado / no supervisado / por refuerzo** | Con respuestas correctas / buscando estructura sin respuestas / por prueba y error con recompensas. | — |
| **Asistente de código / copiloto** | Herramienta de IA que sugiere o escribe código (GitHub Copilot, Claude Code, Cursor…). | — |
| **Atención (attention)** | Mecanismo con el que el modelo decide a qué partes del texto «mirar». | — |
| **Autocompletado con IA** | Sugerencias mientras escribes. | — |
| **Automatización robótica (RPA)** | Robots de software que repiten tareas de interfaz; anterior a los agentes. | — |
| **Autonomía / permisos del agente** | Qué puede hacer sin preguntar. | — |
| **Autosupervisado (self-supervised)** | El modelo genera sus propias etiquetas (predecir la siguiente palabra). | — |
| **Base de datos vectorial / búsqueda semántica** | Almacén de embeddings / buscar por significado, no por palabras exactas. | — |
| **Benchmark** | Conjunto de pruebas estándar para comparar modelos. | — |
| **Bucle del agente** | Pensar → actuar → observar el resultado → seguir. | — |
| **Caché de prompt (prompt caching)** | Reutilizar la parte repetida de un prompt para ahorrar coste y tiempo. | — |
| **Caja negra** | Sistema cuyo funcionamiento interno no se entiende. | — |
| **Chain of thought (cadena de pensamiento)** | Pedir al modelo que razone paso a paso. | — |
| **Chat de código / edición en línea** | Conversar sobre el código / pedir un cambio dentro del editor. | — |
| **Chatbot / asistente virtual** | Programa conversacional. | — |
| **Chunking (troceado)** | Dividir documentos en fragmentos para indexarlos. | — |
| **Clasificación / regresión / agrupamiento (clustering)** | Predecir una categoría / un número / agrupar por parecido. | — |
| **Compactación** | Resumir la conversación para liberar contexto. | — |
| **Context engineering** | Seleccionar y organizar la información que se da al modelo. | — |
| **Context rot / ventana saturada** | Pérdida de calidad cuando el contexto está lleno. | — |
| **Contexto del proyecto (CLAUDE.md, AGENTS.md, reglas)** | Archivo con normas y datos del proyecto que el agente lee siempre. | — |
| **Copiloto (copilot) / pair programming con IA** | Asistente que sugiere código mientras escribes. | — |
| **Cuantización** | Reducir la precisión de los pesos para que el modelo ocupe y consuma menos. | — |
| **CUDA** | Plataforma de NVIDIA para programar GPU. | — |
| **Cuota / límite de uso (rate limit)** | Máximo de peticiones o tokens por periodo. | — |
| **Dataset (conjunto de datos)** | Colección de ejemplos para entrenar o evaluar. | — |
| **Deepfake** | Contenido falso hiperrealista generado por IA. | — |
| **Delimitadores** | Marcas (comillas, etiquetas) que separan instrucciones de datos. | — |
| **Deriva de datos / de concepto (drift)** | Los datos reales cambian con el tiempo y el modelo pierde precisión. | — |
| **Descenso del gradiente / retropropagación** | Método para ajustar los pesos reduciendo el error. | — |
| **Despliegue de modelo / endpoint de inferencia** | Poner el modelo a disposición como servicio. | — |
| **Destilación (distillation)** | Entrenar un modelo pequeño para imitar a uno grande. | — |
| **Detección de anomalías** | Encontrar lo que se sale de lo normal. | — |
| **Embedding** | Representación numérica del significado de un texto para comparar parecidos. | — |
| **Entrenamiento (training)** | Proceso de ajustar los pesos con datos para que el modelo acierte. | — |
| **Entrenamiento / validación / prueba (train / validation / test)** | Tres partes del dataset: aprender / ajustar / medir el resultado final. | — |
| **Época / lote (batch)** | Una pasada completa por los datos / grupo de ejemplos procesados a la vez. | — |
| **Equidad (fairness) / explicabilidad / interpretabilidad** | Que el modelo no discrimine / poder justificar sus decisiones / entender qué hace por dentro. | — |
| **Etiqueta (label) / anotación** | La respuesta correcta de un ejemplo. | — |
| **Evals (evaluaciones)** | Pruebas que miden qué tan bien responde un sistema de IA. | — |
| **Experimento / seguimiento (MLflow, Weights & Biases)** | Registro de ejecuciones de entrenamiento. | — |
| **Falso positivo / falso negativo** | Dice que sí cuando era no / dice que no cuando era sí. | — |
| **Feature store** | Almacén de variables reutilizables para entrenar modelos. | — |
| **Fine-tuning (ajuste fino)** | Entrenar un poco más un modelo con tus propios datos. | — |
| **Fuga de datos / privacidad en IA** | Riesgo de que el modelo revele información sensible. | — |
| **Función de pérdida (loss)** | Número que mide cuánto se equivoca el modelo. | — |
| **Generalización** | Que acierte con datos que no ha visto. | — |
| **Gobernanza de IA** | Reglas y controles para usar IA con responsabilidad. | — |
| **Golden set** | Conjunto de ejemplos de referencia con la respuesta ideal. | — |
| **GPU / TPU / VRAM** | Procesadores y memoria que aceleran la IA. | — |
| **Grounding (anclaje)** | Basar las respuestas en fuentes verificables. | — |
| **Guardrails (barreras)** | Reglas que limitan lo que la IA puede hacer o decir. | — |
| **Hiperparámetro** | Ajuste que eliges tú antes de entrenar (tasa de aprendizaje, tamaño). | — |
| **Hugging Face** | Plataforma con modelos y datasets abiertos. | — |
| **Human in the loop** | Una persona revisa o aprueba lo que decide o genera la IA. | — |
| **Human in the loop (humano en el circuito)** | Una persona revisa o aprueba las acciones importantes. | — |
| **IA / aprendizaje automático (ML) / aprendizaje profundo** | IA: máquinas que hacen tareas que requieren inteligencia. ML: aprenden de datos. Profundo: lo hace con redes neuronales grandes. | — |
| **IA agéntica (agentic)** | Enfoque con agentes que actúan de forma más autónoma. | — |
| **IA estrecha / IA general (AGI)** | Estrecha: hace bien **una** tarea. General: capaz de aprender cualquier tarea intelectual humana (todavía no existe). | — |
| **IA generativa** | Modelos que **crean** contenido nuevo: texto, imágenes, audio, código, vídeo. | — |
| **Inferencia** | El momento de usar un modelo ya entrenado. | — |
| **Ingeniería de prompts** | Escribir instrucciones claras y con contexto para obtener mejores respuestas. | — |
| **Ingeniería de prompts (prompt engineering)** | Diseñar instrucciones claras para obtener buenos resultados. | — |
| **Inteligencia artificial (IA)** | Campo que construye sistemas capaces de hacer tareas que normalmente requieren inteligencia humana (entender lenguaje, reconocer imágenes, decidir). | — |
| **Inyección de prompt (prompt injection)** | Ataque que esconde órdenes en un texto para que la IA las obedezca. | — |
| **Jailbreak** | Intentar saltarse las restricciones de un modelo. | — |
| **Latencia / tokens por segundo / coste por token** | Tiempo de respuesta / velocidad de generación / precio de uso. | — |
| **Ley de IA (EU AI Act)** | Reglamento europeo que clasifica sistemas de IA por riesgo. | — |
| **Llamada a funciones / uso de herramientas (tool use)** | El modelo decide llamar a una función o servicio externo y recibe el resultado. | — |
| **LLM (modelo de lenguaje grande)** | Modelo entrenado con enormes cantidades de texto que predice y genera lenguaje (los que están detrás de los asistentes de IA). | — |
| **LLM como juez** | Usar un modelo para valorar respuestas de otro. | — |
| **LLMOps** | MLOps para aplicaciones con LLM. | — |
| **Local / on-device** | Ejecutar el modelo en tu equipo. | — |
| **LoRA / PEFT** | Técnicas para ajustar un modelo cambiando solo una pequeña parte. | — |
| **Marca de agua / procedencia (C2PA)** | Señales para identificar contenido generado. | — |
| **Matriz de confusión** | Tabla de aciertos y fallos por clase. | — |
| **Máximo de tokens (max tokens)** | Tope de longitud de la respuesta. | — |
| **MCP (Model Context Protocol)** | Estándar abierto para conectar modelos con herramientas y fuentes de datos. | — |
| **Memoria del agente** | Información que el agente conserva entre pasos o sesiones. | — |
| **MLOps** | Prácticas DevOps aplicadas a modelos (entrenar, desplegar, vigilar). | — |
| **Modelo abierto / de pesos abiertos** | Modelo cuyos pesos se pueden descargar y ejecutar (Llama, Mistral, Qwen…). | — |
| **Modelo de difusión** | Modelos que generan imágenes eliminando ruido paso a paso. | — |
| **Modelo de lenguaje (LM)** | Modelo que predice la siguiente palabra (o trozo de palabra). | — |
| **Modelo fundacional (foundation model)** | Modelo general muy grande que se adapta a muchas tareas. | — |
| **Modelo propietario / cerrado** | Solo se usa por API. | — |
| **Multiagente** | Varios agentes que colaboran con roles distintos. | — |
| **Multimodal** | IA que maneja varios tipos de dato: texto, imagen, audio. | — |
| **Neurona / capa / pesos / parámetros** | Unidad básica / grupo de neuronas / valores que el modelo ajusta al aprender / el conjunto de todos esos valores (los modelos grandes tienen miles de millones). | — |
| **Observabilidad de IA** | Registrar prompts, respuestas, costes y errores. | — |
| **OCR** | Reconocer texto en imágenes. | — |
| **Ollama / llama.cpp / vLLM** | Herramientas para ejecutar modelos propios. | — |
| **Orquestador / subagente** | Agente que reparte trabajo / el que lo recibe. | — |
| **Pipeline de ML** | Cadena automática de datos → entrenamiento → evaluación → despliegue. | — |
| **Planificación** | El agente descompone un objetivo en pasos. | — |
| **Plugin / extensión de IA** | Complemento que da nuevas capacidades al asistente. | — |
| **Pre-entrenamiento / post-entrenamiento** | Fase de aprender del texto bruto / fase de pulir el comportamiento. | — |
| **Precisión / exhaustividad (recall) / F1** | Cuántas de las respuestas positivas eran correctas / cuántos casos reales se encontraron / medida combinada. | — |
| **Procesamiento del lenguaje natural (PLN / NLP)** | IA que trabaja con texto y lenguaje. | — |
| **Prompt** | La instrucción que le das a la IA. | — |
| **Prompt de usuario / de asistente** | Mensaje del usuario / respuesta del modelo en una conversación. | — |
| **Prompt del día a día** | «Resume este texto», «explícame este error», «escribe una prueba para esta función». | — |
| **Prompt reutilizable / plantilla de prompt** | Instrucción con huecos para rellenar. | — |
| **Pruebas generadas con IA** | Casos de prueba creados por un modelo; deben revisarse. | — |
| **RAG** | Dar a la IA documentos de consulta antes de responder para que se base en ellos. | Un examen con apuntes. |
| **RAG (generación aumentada por recuperación)** | Buscar documentos relevantes y dárselos al modelo antes de responder. | — |
| **Razonamiento (reasoning models)** | Modelos que «piensan» más antes de responder. | — |
| **Reconocimiento de voz / síntesis de voz (ASR / TTS)** | Voz a texto / texto a voz. | — |
| **Red neuronal** | Modelo formado por capas de «neuronas» artificiales que transforman números. | — |
| **Red teaming (en IA)** | Probar el sistema intentando que falle de formas dañinas. | — |
| **Regularización / dropout** | Técnicas para evitar el sobreajuste. | — |
| **Reranking** | Reordenar los resultados de una búsqueda para quedarse con los mejores. | — |
| **Revisión de código con IA** | Un modelo señala errores y mejoras en un cambio. | — |
| **RLHF** | Entrenamiento con valoraciones humanas para que el modelo responda de forma útil y segura. | — |
| **Roles en el prompt** | Pedir al modelo que actúe como un perfil (revisor, tutor). | — |
| **Salida estructurada (structured output)** | Obligar al modelo a responder en un formato (JSON con un esquema). | — |
| **Sesgo (bias)** | Tendencias injustas del modelo heredadas de los datos. También: parámetro de una neurona. | — |
| **Similitud coseno** | Medida de parecido entre dos embeddings. | — |
| **Sistema de recomendación** | Sugiere productos o contenidos. | — |
| **Skill (habilidad)** | Paquete de instrucciones y recursos que un agente carga para una tarea concreta. | — |
| **SLM (modelo de lenguaje pequeño)** | Modelo más pequeño, rápido y barato; puede correr en un portátil o móvil. | — |
| **Sobreajuste (overfitting)** | El modelo memoriza los ejemplos y falla con datos nuevos. | Un alumno que memoriza el examen. |
| **Spec-driven development** | Escribir primero una especificación clara y dejar que la IA la implemente. | — |
| **Stop sequence** | Texto que, al generarse, detiene la respuesta. | — |
| **Streaming (de respuesta)** | Recibir la respuesta según se genera. | — |
| **Subagente / worktree** | Agente secundario / copia aislada del repositorio donde trabaja. | — |
| **Subajuste (underfitting)** | El modelo es demasiado simple y no aprende lo suficiente. | — |
| **System prompt (instrucciones del sistema)** | Instrucciones de fondo que fijan el papel y las reglas del asistente. | — |
| **Tasa de aprendizaje (learning rate)** | Cuánto se corrigen los pesos en cada paso. | — |
| **Temperatura** | Ajuste de aleatoriedad: baja = respuestas más estables; alta = más creativas. | — |
| **Texto a imagen / imagen a imagen / inpainting** | Generar una imagen desde texto / transformar una / rellenar una zona. | — |
| **Token / ventana de contexto** | Token: trozo de texto que el modelo procesa. Ventana de contexto: cuánto texto puede tener presente a la vez. | — |
| **Token de acceso (API key)** | Clave para usar un servicio de IA; **no se publica ni se sube al repositorio**. | — |
| **Tokenización / token** | Dividir el texto en trozos (tokens) que el modelo entiende; una palabra suele ser 1–3 tokens. | — |
| **Top-p / top-k** | Otros ajustes que limitan qué palabras puede elegir el modelo. | — |
| **Transformer** | Arquitectura de red neuronal en la que se basan los LLM actuales. | — |
| **Uso de herramientas / MCP** | Que la IA llame a programas externos. MCP es un estándar para conectar la IA con herramientas y datos. | — |
| **Ventana de contexto** | Cantidad máxima de tokens que el modelo puede manejar a la vez (entrada + salida). | La memoria de trabajo del modelo. |
| **Verificación / «confía pero verifica»** | Comprobar siempre lo que genera la IA ejecutándolo y contrastándolo. | — |
| **Vibe coding** | Programar describiendo lo que quieres y aceptando lo que genera la IA, sin revisar el código a fondo. Rápido, pero arriesgado si no entiendes lo que sale. | — |
| **Visión por computador** | IA que interpreta imágenes y vídeo. | — |
| **Visión-lenguaje (VLM)** | Modelos que entienden imagen y texto a la vez. | — |
| **Zero-shot / one-shot / few-shot** | Sin ejemplos / con uno / con varios ejemplos en el prompt. | — |

## 15. Pruebas y calidad (QA)

| Término | Qué es | Como si fuera… |
|---|---|---|
| **Adivinanza de errores (error guessing)** | Probar donde la experiencia dice que suele fallar. | — |
| **Aislamiento de pruebas** | Que una prueba no dependa de otra. | — |
| **Análisis dinámico (DAST)** | Revisar la aplicación en marcha. | — |
| **Análisis estático (SAST) / linter** | Revisar el código sin ejecutarlo. | — |
| **Análisis estático / linter / formateador** | Analizan el código sin ejecutarlo: linter busca problemas, formateador unifica el estilo. | — |
| **BDD** | Describir el comportamiento en lenguaje natural («Dado… cuando… entonces…») y convertirlo en pruebas. | — |
| **Caja negra / caja blanca / caja gris** | Probar sin ver el código / viéndolo / parcialmente. | — |
| **Caso de prueba (test case)** | Pasos, datos y resultado esperado de una comprobación. | — |
| **Ciclo de vida del bug** | Nuevo → asignado → corregido → verificado → cerrado (o reabierto). | — |
| **Cobertura (coverage)** | Porcentaje de código ejecutado por las pruebas. Alta cobertura no garantiza buenas pruebas. | — |
| **Cobertura de código (líneas, ramas, condiciones)** | Porcentaje de código ejecutado por las pruebas. | — |
| **Cobertura de requisitos / trazabilidad** | Que cada requisito tenga pruebas asociadas. | — |
| **Complejidad ciclomática** | Medida de cuántos caminos distintos tiene una función. | — |
| **Criterios de entrada / de salida** | Condiciones para empezar / dar por terminada una fase de pruebas. | — |
| **Datos de prueba (test data)** | Información usada en las pruebas. | — |
| **Datos sintéticos / anonimizados** | Datos inventados / datos reales sin identidad. | — |
| **Defecto / incidencia / bug** | Fallo detectado. | — |
| **Definición de hecho (DoD) / criterios de aceptación** | Lo que debe cumplirse para dar algo por terminado. | — |
| **Doble de prueba (test double)** | Término general para mock, stub, fake… | — |
| **Entorno de pruebas / staging / sandbox** | Copia donde probar sin tocar producción. | — |
| **Entorno de pruebas aislado (sandbox)** | Lugar seguro donde probar sin afectar a nada real. | — |
| **Error budget** | Fallo permitido antes de frenar los cambios. | — |
| **Escenario de prueba** | Situación a probar, que agrupa varios casos. | — |
| **Espera explícita / implícita (wait)** | Esperar a que algo aparezca antes de actuar. | — |
| **Estrategia de pruebas** | Enfoque general: qué se prueba, con qué nivel y herramientas. | — |
| **Evals de LLM** | Conjuntos de pruebas para medir respuestas de modelos. | — |
| **Fixture** | Datos o estado preparado antes de una prueba. | — |
| **Fixture / setup / teardown** | Preparación / antes / después de cada prueba. | — |
| **Fuzzing** | Alimentar el programa con datos aleatorios para encontrar fallos. | — |
| **Gestor de pruebas (TestRail, Zephyr, Xray)** | Herramienta para organizar casos y ejecuciones. | — |
| **Gherkin / Cucumber / Given-When-Then** | Lenguaje y herramienta para escribir escenarios: «Dado… cuando… entonces…». | — |
| **Go / no-go** | Decisión de publicar o no. | — |
| **Happy path / caso límite (edge case)** | Happy path: el camino sin problemas. Caso límite: situación extrema (lista vacía, texto larguísimo). | — |
| **Heurística de pruebas** | Regla práctica para decidir qué probar. | — |
| **Ingeniería del caos (chaos engineering)** | Provocar fallos a propósito en producción controlada para comprobar la resiliencia. | — |
| **Locator / selector** | Cómo se identifica un elemento en pantalla. | — |
| **Métrica de defectos (densidad, fuga)** | Defectos por tamaño / defectos que escapan a producción. | — |
| **Mock / stub / fake** | Piezas simuladas que sustituyen a las reales durante las pruebas. | Un muñeco de pruebas de choque. |
| **Mock / stub / spy / fake / dummy** | Simulaciones de dependencias: responde lo programado / respuesta fija / registra llamadas / implementación simple / relleno. | — |
| **MTTD / MTTR** | Tiempo medio de detección / de recuperación. | — |
| **Observabilidad para pruebas** | Poder ver qué ocurre dentro durante las pruebas. | — |
| **Page Object** | Patrón que encapsula una pantalla para las pruebas de interfaz. | — |
| **Pairwise (combinatoria por pares)** | Reducir combinaciones probando todos los pares de valores. | — |
| **Pirámide / trofeo / panal de pruebas** | Modelos de cómo repartir los tipos de pruebas. | — |
| **Pirámide de pruebas** | Muchas unitarias, algunas de integración y pocas E2E: más rápidas y baratas abajo. | — |
| **Plan de pruebas** | Documento con alcance, enfoque, recursos y calendario. | — |
| **Property-based testing** | Se definen propiedades y la herramienta genera casos aleatorios. | — |
| **Prueba de carga / de estrés** | Carga: cuánto aguanta con el tráfico esperado. Estrés: qué pasa al límite. | — |
| **Prueba de integración** | Comprueba que varias piezas funcionan juntas (código + base de datos). | Probar que el tornillo encaja en la pieza. |
| **Prueba de regresión** | Comprueba que lo que antes funcionaba sigue funcionando. | — |
| **Prueba end-to-end (E2E)** | Simula a un usuario real recorriendo la aplicación completa. | — |
| **Prueba flaky (inestable)** | Prueba que a veces pasa y a veces falla sin cambios en el código. | — |
| **Prueba unitaria** | Prueba de una pieza pequeña (una función) de forma aislada. | Probar un tornillo. |
| **Pruebas A/B** | Comparar dos versiones con usuarios reales. | — |
| **Pruebas alfa / beta** | Con usuarios internos / con usuarios externos reales. | — |
| **Pruebas con IA** | Generar casos, datos y scripts con modelos; también probar aplicaciones de IA con evals. | — |
| **Pruebas continuas** | Ejecutar pruebas automáticamente en cada cambio. | — |
| **Pruebas de accesibilidad** | ¿Pueden usarlo personas con discapacidad? | — |
| **Pruebas de aceptación (UAT)** | El cliente valida que cumple lo pedido. | — |
| **Pruebas de API** | Se prueban las direcciones y respuestas sin interfaz. | — |
| **Pruebas de carga / estrés / pico / resistencia (soak)** | Tráfico esperado / más allá del límite / subida brusca / carga larga. | — |
| **Pruebas de compatibilidad / multinavegador / multidispositivo** | En distintos navegadores y dispositivos. | — |
| **Pruebas de componente** | Una pieza completa con sus dependencias simuladas. | — |
| **Pruebas de contrato** | Cliente y servidor cumplen el mismo acuerdo. | — |
| **Pruebas de escalabilidad** | Cómo responde al crecer. | — |
| **Pruebas de humo (smoke) / de cordura (sanity)** | Lo esencial arranca / un cambio concreto funciona. | — |
| **Pruebas de instalación / actualización / migración** | Se instala, actualiza y migra sin perder datos. | — |
| **Pruebas de integración / de sistema / end-to-end** | Piezas juntas / todo el sistema / flujo completo de usuario. | — |
| **Pruebas de interfaz (UI)** | Se manejan pantallas (Selenium, Playwright, Cypress). | — |
| **Pruebas de localización** | Idiomas, formatos, monedas. | — |
| **Pruebas de mutación** | Se estropea el código a propósito para ver si las pruebas lo detectan. | — |
| **Pruebas de recuperación / caos** | Cómo se recupera ante fallos. | — |
| **Pruebas de regresión** | Lo anterior sigue funcionando. | — |
| **Pruebas de rendimiento** | Miden velocidad y consumo. | — |
| **Pruebas de seguridad / de penetración** | Buscan fallos de seguridad. | — |
| **Pruebas de usabilidad** | ¿Se entiende y se usa fácilmente? | — |
| **Pruebas en producción / canary / shadow** | Probar con tráfico real controlado. | — |
| **Pruebas exploratorias** | Probar investigando sin guion fijo. | — |
| **Pruebas funcionales / no funcionales** | Qué hace / cómo lo hace (rendimiento, seguridad, usabilidad). | — |
| **Pruebas visuales / de capturas (snapshot)** | Comparar imágenes o salidas con una referencia. | — |
| **QA (Quality Assurance, aseguramiento de calidad)** | Prevenir defectos mejorando el **proceso**. | — |
| **QC (Quality Control, control de calidad)** | Detectar defectos en el **producto**. | — |
| **Reproducible / pasos para reproducir** | Poder repetir un fallo / descripción de cómo. | — |
| **Resultado esperado / obtenido** | Lo que debería pasar / lo que pasó. | — |
| **Revisión por pares / inspección / walkthrough** | Revisar el trabajo entre compañeros. | — |
| **Self-healing tests** | Pruebas que se reparan solas cuando cambia un selector (con IA). | — |
| **Severidad / prioridad** | Gravedad técnica del fallo / urgencia de arreglarlo. | — |
| **Shift-left** | Mover las pruebas y la seguridad al principio del proceso, no al final. | — |
| **Shift-left / shift-right** | Probar antes en el proceso / vigilar y probar también en producción. | — |
| **Smoke test (prueba de humo)** | Comprobación rápida de que lo esencial arranca. | Encender el aparato y ver que no sale humo. |
| **Suite de pruebas** | Conjunto de pruebas que se ejecutan juntas. | — |
| **Tabla de decisión / transición de estados** | Técnicas para probar combinaciones y cambios de estado. | — |
| **TDD (desarrollo guiado por pruebas)** | Escribir primero la prueba (que falla), luego el código mínimo que la pasa, luego mejorarlo. Ciclo rojo-verde-refactor. | — |
| **TDD / BDD / ATDD** | Pruebas primero / comportamiento en lenguaje natural / pruebas de aceptación primero. | — |
| **Test flaky (inestable)** | Prueba que falla o pasa sin cambios en el código. | — |
| **Testabilidad** | Lo fácil que es probar un sistema. | — |
| **Tester / QA engineer / SDET** | Persona que prueba / ingeniero de calidad / desarrollador especializado en automatización de pruebas. | — |
| **Triage** | Clasificar los defectos por gravedad y prioridad. | — |
| **Valor límite / partición de equivalencia** | Técnicas de diseño de casos: probar los bordes / un caso por cada grupo parecido. | — |
| **Verificación / validación** | ¿Lo hemos construido bien? / ¿Hemos construido lo correcto? | — |

## 16. Infraestructura, nube y DevOps

| Término | Qué es | Como si fuera… |
|---|---|---|
| **Alert fatigue** | Demasiadas alertas: se ignoran. | — |
| **Alerta / umbral / pager** | Aviso al superar un límite / límite / sistema de guardia. | — |
| **Ansible / Chef / Puppet** | Gestión de configuración de máquinas. | — |
| **APM / OpenTelemetry** | APM: herramientas de rendimiento de aplicaciones. OpenTelemetry: estándar abierto para emitir logs, métricas y trazas. | — |
| **Backstage** | Plataforma de portal de desarrolladores. | — |
| **Balanceador L4 / L7** | Reparte por conexión / por contenido de la petición. | — |
| **Bastión (jump host)** | Máquina de entrada segura para acceder a las internas. | — |
| **Caché de dependencias** | Guardar lo descargado para acelerar el pipeline. | — |
| **Capacity planning** | Prever los recursos necesarios. | — |
| **Certificado TLS / Let's Encrypt** | Prueba de identidad y cifrado de un sitio / entidad gratuita que los emite. | — |
| **Chaos engineering** | Provocar fallos para comprobar la resiliencia. | — |
| **Cold start (arranque en frío)** | Retraso de la primera ejecución de una función o contenedor que estaba «dormido». | — |
| **Contenedor** | Paquete ligero con tu aplicación y todo lo que necesita, que se ejecuta igual en cualquier sitio. Comparte el núcleo del sistema. | Un contenedor de transporte: da igual el barco o el camión. |
| **Contenedor / imagen / capa** | Paquete ejecutable / plantilla / cada paso acumulado de una imagen. | — |
| **Dashboard (panel)** | Pantalla con gráficos del estado del sistema. | — |
| **Despliegue (deploy) / release / promoción** | Poner una versión en un entorno / publicarla / pasarla de un entorno al siguiente. | — |
| **Dev container / entorno en la nube** | Entorno de desarrollo reproducible. | — |
| **DevOps engineer / SRE / platform engineer / cloud engineer** | Perfiles que automatizan despliegues, fiabilidad y plataformas. | — |
| **Disaster recovery / RPO / RTO** | Recuperación ante desastres / pérdida máxima de datos / tiempo máximo de recuperación. | — |
| **DNS (registro A, CNAME, MX, TXT)** | Traduce nombres a direcciones; tipos de registros. | — |
| **Docker** | Herramienta más popular para crear y ejecutar contenedores. | — |
| **Docker Compose** | Define y levanta varios contenedores con un archivo. | — |
| **Dockerfile / Docker Compose** | Dockerfile: instrucciones para construir una imagen. Compose: describe y levanta varios contenedores juntos. | — |
| **Dockerfile / multi-stage build** | Receta para construir una imagen / construir en varias fases para que la final sea pequeña. | — |
| **Drift (deriva)** | La realidad difiere de lo declarado en el código. | — |
| **Edge computing (computación en el borde)** | Ejecutar código cerca del usuario o del dispositivo, no en un centro de datos lejano. | — |
| **ELK / OpenSearch / Splunk / Datadog / New Relic** | Pila y plataformas de logs y monitorización. | — |
| **Entorno / environment** | Dev, test, staging, producción. | — |
| **Estado de Terraform (state)** | Registro de lo que Terraform ha creado. | — |
| **Estrategias de despliegue** | Recreate, rolling, blue-green, canary, shadow. | — |
| **Feature flags / dark launch** | Activar funciones sin redesplegar / lanzar oculto. | — |
| **FinOps** | Gestión del coste de la nube. | — |
| **FinOps / etiquetado de costes (tags)** | Gestión del gasto en nube / marcar recursos para saber quién gasta. | — |
| **GitHub Actions / GitLab CI / Jenkins / Azure DevOps / CircleCI** | Plataformas de CI/CD. | — |
| **GitOps (Argo CD, Flux)** | El repositorio es la fuente de verdad de la infraestructura. | — |
| **Golden path** | Camino recomendado y ya preparado para hacer algo. | — |
| **Golden signals** | Latencia, tráfico, errores y saturación. | — |
| **Grupo de seguridad / firewall** | Reglas de qué tráfico entra y sale. | — |
| **HA (alta disponibilidad) / 99,9 % («tres nueves»)** | Que no se caiga / unos 8,8 horas de caída al año. | — |
| **Health check** | Pregunta periódica al servicio: «¿estás bien?». | — |
| **Helm / Kustomize / operador** | Paquetes / personalización de manifiestos / automatiza software complejo en K8s. | — |
| **Helm / manifiesto** | Helm: gestor de paquetes de Kubernetes. Manifiesto: archivo YAML que describe lo que se quiere desplegar. | — |
| **Hipervisor** | Programa que crea y gestiona máquinas virtuales (VirtualBox, KVM, VMware, Hyper-V). | — |
| **HPA / autoescalado** | Escala el número de pods según la carga. | — |
| **IaaS / PaaS / SaaS** | IaaS: alquilas máquinas. PaaS: alquilas una plataforma donde subes tu código. SaaS: usas el programa ya hecho (correo, CRM). | Alquilar el local, la cocina o comer en el restaurante. |
| **IaC (infraestructura como código)** | Describir la infraestructura en archivos que se versionan y se aplican automáticamente (Terraform, Ansible). | Una receta escrita en lugar de cocinar de memoria. |
| **IaC / Terraform / Pulumi / CloudFormation / Bicep** | Infraestructura descrita con código. | — |
| **IAM** | Gestión de identidades y permisos en la nube. | — |
| **Imagen (image)** | Plantilla de solo lectura de la que se crean contenedores. | Una receta (imagen) y el plato servido (contenedor). |
| **Imagen de máquina (AMI) / plantilla** | Copia base para crear instancias. | — |
| **Incident commander** | Persona que coordina la respuesta a un incidente. | — |
| **Incidente / severidad (SEV1…)** | Interrupción del servicio / su gravedad. | — |
| **Infraestructura** | Servidores, redes, almacenamiento y servicios sobre los que corre el software. | — |
| **Inmutable / mutable** | No se modifica una máquina, se reemplaza / se modifica. | — |
| **Instancia** | Una máquina virtual o servicio en ejecución en la nube. | — |
| **Instancia reservada / spot** | Descuento por compromiso / máquina barata que pueden retirar. | — |
| **Kubernetes (K8s)** | Sistema que orquesta contenedores: los reparte, los reinicia si caen y los escala. | Un director de orquesta. |
| **Logs / métricas / trazas** | Logs: eventos escritos. Métricas: números en el tiempo (CPU, peticiones/s). Trazas: recorrido de una petición por varios servicios. | — |
| **Máquina virtual (VM)** | Ordenador simulado dentro de otro, con su propio sistema operativo. | Un piso dentro de un edificio. |
| **Matriz de pruebas** | Ejecutar lo mismo en varias versiones o sistemas. | — |
| **Métricas RED / USE** | Rate-Errors-Duration (servicios) / Utilization-Saturation-Errors (recursos). | — |
| **Mínimo privilegio** | Solo los permisos necesarios. | — |
| **Monitorización y alertas** | Vigilar el sistema y avisar cuando algo se sale de lo normal. | — |
| **Multi-cuenta / landing zone** | Organización de cuentas cloud con reglas comunes. | — |
| **Nodo / plano de control / kubelet** | Máquina del clúster / cerebro / agente en cada nodo. | — |
| **Nube (cloud)** | Usar recursos informáticos de un proveedor por internet, pagando por uso (AWS, Azure, Google Cloud). | Alquilar en vez de comprar. |
| **Nube híbrida / multinube** | Híbrida: parte en local y parte en la nube. Multinube: varios proveedores a la vez. | — |
| **Observabilidad** | Poder entender qué ocurre dentro del sistema a partir de lo que emite: logs, métricas y trazas. | — |
| **Observabilidad (logs, métricas, trazas)** | Entender el sistema desde sus datos. | — |
| **On-call / guardia / rotación** | Turno de atención a incidentes. | — |
| **On-premise (en local)** | Infraestructura en tus propias instalaciones en lugar de en la nube. | — |
| **OpenTelemetry (OTel)** | Estándar para emitir datos de observabilidad. | — |
| **Orquestación** | Coordinar automáticamente muchos componentes (contenedores, servidores). | — |
| **Paridad de entornos** | Que dev y producción se parezcan lo más posible. | — |
| **Pets vs cattle** | Servidores únicos y mimados / piezas intercambiables. | — |
| **Pipeline CI/CD / etapa / trabajo / artefacto** | Cadena automática / fase / tarea / resultado. | — |
| **Plataforma interna de desarrolladores (IDP)** | Portal con herramientas para desplegar sin ser experto. | — |
| **Pod / nodo / clúster** | Pod: unidad mínima en Kubernetes (uno o más contenedores). Nodo: máquina. Clúster: conjunto de nodos. | — |
| **Pod / ReplicaSet / Deployment / StatefulSet / DaemonSet** | Unidad mínima / réplicas / despliegue gestionado / para apps con estado / uno por nodo. | — |
| **Podman / containerd / OCI** | Alternativas a Docker / motor de contenedores / estándar de formato. | — |
| **Postmortem sin culpa (blameless)** | Análisis para aprender, no para culpar. | — |
| **Prometheus / Grafana / Loki / Tempo** | Métricas / paneles / logs / trazas. | — |
| **Proxy inverso / ingress** | Puerta de entrada del tráfico a los servicios. | — |
| **Quality gate** | Condición que debe cumplirse para continuar el pipeline. | — |
| **Red de contenedores / puerto publicado** | Conexión entre contenedores / puerto accesible desde fuera. | — |
| **Región / zona de disponibilidad** | Región: área geográfica del proveedor. Zona: centro de datos independiente dentro de ella. | — |
| **Registro (registry) / etiqueta (tag) / digest** | Almacén de imágenes / nombre de versión / huella única de contenido. | — |
| **Registro de contenedores (registry)** | Almacén de imágenes (Docker Hub, GitHub Container Registry). | — |
| **Reproducible build** | Mismo código → mismo artefacto. | — |
| **Rol / política / principal** | Conjunto de permisos / reglas de permiso / quién actúa. | — |
| **Rollback / roll forward** | Volver atrás / corregir hacia delante. | — |
| **Runbook / playbook** | Guía operativa paso a paso. | — |
| **Runner / agente** | Máquina que ejecuta los trabajos. | — |
| **Serverless / FaaS / BaaS** | Sin gestionar servidores / funciones / backend como servicio. | — |
| **Service / Ingress / ConfigMap / Secret / Namespace** | Acceso estable a pods / entrada HTTP / configuración / datos sensibles / espacio de nombres. | — |
| **Service mesh (Istio, Linkerd)** | Gestiona la comunicación entre servicios. | — |
| **Servidor** | Ordenador (físico o virtual) que ofrece un servicio a otros. | — |
| **Servidor bare metal** | Máquina física sin virtualizar. | — |
| **SLI / SLO / SLA / error budget** | Indicador / objetivo / acuerdo con el cliente / margen de fallo. | — |
| **SLSA / firma de artefactos** | Marco de seguridad de la cadena de suministro / firmar lo construido. | — |
| **Sysadmin / administrador de sistemas** | Persona que mantiene servidores y servicios. | — |
| **Terraform / Ansible** | Terraform: crea infraestructura. Ansible: configura máquinas. | — |
| **Toil** | Trabajo manual repetitivo que conviene automatizar. | — |
| **Trace / span / correlation ID** | Recorrido de una petición / cada paso dentro / identificador que la sigue. | — |
| **Volumen / bind mount** | Almacenamiento persistente de un contenedor. | — |
| **VPC / subred / gateway NAT / ruta** | Red privada virtual en la nube / división / salida a internet / camino del tráfico. | — |
| **VPN** | Túnel cifrado entre redes o equipos. | — |
| **War room / bridge** | Sala o llamada de coordinación durante un incidente. | — |
| **Zona / región de disponibilidad** | Centro de datos / área geográfica. | — |

## 17. Redes y protocolos

| Término | Qué es | Como si fuera… |
|---|---|---|
| **AMQP** | Protocolo de colas (RabbitMQ). | — |
| **CDN / anycast** | Red de distribución / una misma dirección en muchos lugares. | — |
| **DHCP** | Asigna direcciones IP automáticamente. | — |
| **Dirección IP (IPv4 / IPv6)** | Identificador de un equipo en la red. | — |
| **Dirección MAC** | Identificador físico de una tarjeta de red. | — |
| **DMZ** | Zona intermedia para servicios expuestos. | — |
| **DNS / DoH / DoT** | Resolución de nombres y sus variantes cifradas. | — |
| **Firewall / ACL** | Filtro de tráfico / lista de reglas. | — |
| **FTP / SFTP** | Transferencia de archivos. | — |
| **Handshake** | Intercambio inicial para establecer una conexión. | — |
| **HTTP / HTTPS / HTTP/2 / HTTP/3 (QUIC)** | Protocolo web y sus versiones. | — |
| **ICMP / ping / traceroute** | Mensajes de control de red / prueba de alcance / ruta. | — |
| **IDS / IPS** | Detección / prevención de intrusiones. | — |
| **IP pública / privada** | Visible en internet / solo en una red interna (192.168.x.x, 10.x.x.x). | — |
| **LAN / WAN / WLAN** | Red local / extensa / inalámbrica. | — |
| **Latencia / ancho de banda / jitter / pérdida de paquetes** | Retardo / capacidad / variación del retardo / paquetes perdidos. | — |
| **Malla (mesh)** | Red donde los nodos se interconectan. | — |
| **Máscara / CIDR (`/24`)** | Define qué parte de la IP es la red. | — |
| **Modelo OSI / TCP/IP** | Los dos modelos de capas de red (física, enlace, red, transporte, aplicación…). | — |
| **MQTT** | Protocolo ligero de mensajes para IoT. | — |
| **MTU** | Tamaño máximo de un paquete. | — |
| **NAT** | Traduce direcciones privadas a públicas. | — |
| **Proxy / proxy inverso / balanceador** | Intermediarios. | — |
| **Puerta de enlace (gateway)** | Salida hacia otra red. | — |
| **Puerto / socket** | Número de servicio en un equipo / extremo de conexión. | — |
| **RDP / VNC** | Escritorio remoto. | — |
| **Red** | Equipos conectados que intercambian datos. | — |
| **Segmentación de red** | Dividir en zonas para limitar daños. | — |
| **SMTP / IMAP / POP3** | Correo: enviar / leer. | — |
| **Switch / router / punto de acceso / módem** | Equipos de red. | — |
| **TCP / UDP** | Transporte fiable con conexión / rápido sin garantías. | — |
| **TLS / SSL** | Cifrado de las comunicaciones. | — |
| **VLAN** | Red virtual separada dentro de una red física. | — |
| **WebSocket / gRPC / GraphQL** | Protocolos de aplicación modernos. | — |
| **WireGuard / OpenVPN / IPsec** | Tecnologías de VPN. | — |
| **Wireshark / tcpdump** | Herramientas para capturar y analizar tráfico. | — |
| **Zero trust network access (ZTNA)** | Acceso por identidad, no por estar dentro de la red. | — |

## 18. Seguridad

| Término | Qué es | Como si fuera… |
|---|---|---|
| **Activo / amenaza / vulnerabilidad / riesgo / control** | Lo que se protege / lo que puede dañarlo / debilidad / probabilidad × impacto / medida de protección. | — |
| **AES / RSA / ECC / ChaCha20** | Algoritmos de cifrado. | — |
| **Autenticación / autorización** | Autenticación: quién eres. Autorización: qué puedes hacer. | El DNI frente a la llave de cada puerta. |
| **Autenticación / autorización / contabilidad (AAA)** | Quién eres / qué puedes / registro. | — |
| **Autenticación rota / gestión de sesiones** | Fallos en el inicio de sesión. | — |
| **bcrypt / scrypt / Argon2** | Algoritmos recomendados para guardar contraseñas. | — |
| **Botnet / DDoS** | Red de equipos infectados / ataque de saturación. | — |
| **Bug bounty** | Recompensas por reportar fallos. | — |
| **Cadena de custodia** | Registro de quién manejó las evidencias. | — |
| **Cadena de suministro (supply chain) de software** | Riesgo de que lo que usas de terceros (bibliotecas, herramientas) esté comprometido. | — |
| **Cadena de suministro / typosquatting / dependency confusion** | Riesgo por terceros / paquetes con nombre parecido / paquete falso que sustituye al interno. | — |
| **Ciberseguridad** | Proteger sistemas, redes y datos. | — |
| **Cifrado de extremo a extremo (E2EE)** | Solo emisor y receptor pueden leer. | — |
| **Cifrado en tránsito / en reposo** | En tránsito: mientras viaja (HTTPS). En reposo: mientras está guardado. | — |
| **Cifrado simétrico / asimétrico** | Una clave / par público-privada. | — |
| **Compliance / auditoría / control** | Cumplimiento / revisión independiente / medida. | — |
| **Consentimiento / base legal / minimización** | Permiso / razón para tratar datos / recoger solo lo necesario. | — |
| **Copia de seguridad 3-2-1** | 3 copias, 2 soportes, 1 fuera. | — |
| **CVE / CVSS** | CVE: identificador público de una vulnerabilidad. CVSS: puntuación de gravedad (0–10). | — |
| **CVE / CWE / CVSS / KEV** | Identificador / tipo de debilidad / gravedad / catálogo de explotadas. | — |
| **Data poisoning** | Contaminar los datos de entrenamiento. | — |
| **Dato personal / dato sensible** | Identifica a una persona / datos especialmente protegidos. | — |
| **DDoS** | Ataque que satura un servicio con tráfico desde muchos orígenes. | — |
| **Defensa en profundidad** | Varias capas de protección. | — |
| **Derecho al olvido / portabilidad / acceso** | Derechos del interesado. | — |
| **Deserialización insegura** | Reconstruir objetos desde datos no fiables. | — |
| **Divulgación responsable** | Avisar del fallo al responsable antes de publicarlo. | — |
| **DPO / DPIA** | Delegado de protección de datos / evaluación de impacto. | — |
| **EDR / XDR** | Detección y respuesta en equipos / ampliada. | — |
| **Escaneo de contenedores / imágenes** | Buscar vulnerabilidades en imágenes. | — |
| **Forense digital** | Investigación de evidencias tras un incidente. | — |
| **Fuerza bruta / credential stuffing / password spraying** | Probar contraseñas / reutilizar filtradas / una contraseña común en muchas cuentas. | — |
| **Gestor de contraseñas** | Programa que guarda contraseñas seguras. | — |
| **Hardening (bastionado)** | Reducir la superficie de ataque configurando con seguridad. | — |
| **Hash / sal (salt) / pimienta (pepper)** | Huella / valor aleatorio por contraseña / secreto común. | — |
| **Hash / salt** | Hash: huella irreversible de un dato. Salt: valor aleatorio que se añade para que dos contraseñas iguales den hashes distintos. | — |
| **HMAC / firma digital** | Verificar integridad y origen. | — |
| **Honeypot / honeynet** | Señuelo para detectar atacantes. | — |
| **Ingeniería social** | Manipular a las personas. | — |
| **Ingeniería social / phishing** | Engañar a personas (no a máquinas) para obtener datos o acceso. | — |
| **Inmutabilidad de copias** | Copias que no se pueden modificar. | — |
| **Inyección (SQL, comandos, LDAP)** | Datos que se interpretan como órdenes. | — |
| **Inyección SQL / XSS / CSRF** | Tres ataques clásicos: meter órdenes en consultas; meter scripts en páginas; hacer que tu navegador envíe peticiones sin que lo sepas. | — |
| **ISO 27001 / SOC 2 / PCI DSS / ENS / HIPAA** | Normas y marcos de seguridad y cumplimiento. | — |
| **JWT** | Token firmado con datos del usuario que el cliente envía en cada petición. | — |
| **JWT / refresh token / access token** | Token firmado / para renovar / de acceso. | — |
| **KMS / HSM / bóveda de secretos** | Gestores de claves. | — |
| **LOPDGDD** | Ley española de protección de datos. | — |
| **Malware / virus / gusano / troyano / ransomware / spyware** | Software malicioso y sus tipos. | — |
| **Man-in-the-middle (MITM)** | Atacante entre dos partes. | — |
| **MFA / 2FA** | Verificar la identidad con dos o más factores (contraseña + código del móvil). | — |
| **MFA / 2FA / FIDO2 / passkeys / TOTP** | Varios factores / estándar sin contraseña / claves de acceso / códigos temporales. | — |
| **Mínimo privilegio / separación de funciones** | Solo lo necesario / nadie hace todo. | — |
| **MITRE ATT&CK** | Catálogo de tácticas y técnicas de atacantes. | — |
| **Model extraction / inversion** | Robar o reconstruir un modelo o sus datos. | — |
| **Modelado de amenazas (threat modeling)** | Pensar qué podría salir mal y cómo atacarían antes de construir. | — |
| **Modelado de amenazas / STRIDE** | Analizar cómo atacarían / categorías de amenazas (suplantación, manipulación…). | — |
| **OAuth 2 / OpenID Connect** | OAuth 2: permitir que una app acceda a tus datos sin darle la contraseña. OIDC: añade «quién eres» a OAuth. | Una llave de valet para el aparcacoches. |
| **OWASP / Top 10 / ASVS** | Proyecto de seguridad web / lista de riesgos / estándar de verificación. | — |
| **OWASP Top 10** | Lista de los diez riesgos más comunes en aplicaciones web. | — |
| **Parche / gestión de parches** | Corrección / proceso de aplicarlos. | — |
| **Pentest (prueba de intrusión)** | Ataque autorizado para encontrar fallos antes que los malos. | — |
| **Pentest / red team / blue team / purple team** | Pruebas de intrusión / atacantes / defensores / ambos juntos. | — |
| **Phishing / spear phishing / smishing / vishing** | Engaño por correo / dirigido / SMS / voz. | — |
| **PKI / certificado / CA** | Infraestructura de claves públicas. | — |
| **Principio de defensa en profundidad** | Varias capas de protección, no una sola. | — |
| **RBAC** | Permisos por roles (administrador, lector). | — |
| **RBAC / ABAC / ACL** | Control por rol / por atributos / lista de permisos. | — |
| **Red team / blue team** | Red: atacantes autorizados. Blue: defensores. | — |
| **Respuesta a incidentes (IR)** | Detectar, contener, erradicar y recuperar. | — |
| **RGPD / GDPR** | Reglamento europeo de protección de datos. | — |
| **Rotación de secretos** | Cambiar claves periódicamente. | — |
| **SAST / DAST / IAST / SCA** | Análisis de código / de la aplicación en marcha / interactivo / de dependencias. | — |
| **SAST / DAST / SCA** | SAST: analiza el código. DAST: ataca la aplicación en marcha. SCA: revisa las dependencias. | — |
| **SBOM** | Lista de todos los componentes de un software, como la lista de ingredientes. | — |
| **Secreto / vault (bóveda)** | Secreto: contraseña o clave. Vault: sistema para guardarlos de forma segura. | — |
| **Secreto filtrado (leaked)** | Clave publicada por error; hay que **revocarla**. | — |
| **Seguridad en IA** | Proteger modelos y datos: inyección de prompt, fuga de datos, envenenamiento de datos. | — |
| **Seguridad por diseño / por defecto** | Pensada desde el inicio / configuración segura de fábrica. | — |
| **Seudonimización / anonimización** | Datos que ya no apuntan directamente a una persona / imposibles de reidentificar. | — |
| **Shadow IT / shadow AI** | Herramientas usadas sin autorización de la empresa. | — |
| **Shift-left security / DevSecOps** | Seguridad desde el principio y dentro del pipeline. | — |
| **SIEM / SOAR / SOC** | Análisis de eventos / respuesta automatizada / centro de operaciones de seguridad. | — |
| **SSO (inicio de sesión único)** | Entrar una vez y acceder a varios sistemas. | — |
| **SSO / SAML / OIDC / OAuth 2** | Inicio único y protocolos de federación. | — |
| **Superficie de ataque** | Todos los puntos por los que se podría atacar. | — |
| **Threat intelligence (inteligencia de amenazas) / IOC** | Información sobre atacantes / indicadores de compromiso. | — |
| **TLS / mTLS** | Cifrado de transporte / ambos lados se autentican. | — |
| **Token** | Pieza de datos que acredita algo (sesión, permiso). También: fragmento de texto en IA. | — |
| **Tríada CIA** | Confidencialidad, integridad y disponibilidad. | — |
| **Vector de ataque** | Camino que usa el atacante. | — |
| **Vulnerabilidad / exploit / parche** | Vulnerabilidad: fallo aprovechable. Exploit: técnica que lo aprovecha. Parche: arreglo. | — |
| **WAF / cortafuegos** | Filtros que bloquean tráfico malicioso (el WAF, el de aplicaciones web). | — |
| **WAF / RASP** | Cortafuegos de aplicaciones / protección desde dentro. | — |
| **XSS / CSRF / SSRF / XXE / IDOR** | Ataques web: script en página / petición falsificada / servidor engañado / entidades XML / acceso directo a objetos ajenos. | — |
| **Zero trust (confianza cero)** | No fiarse de nadie por estar «dentro» de la red: todo se verifica siempre. | — |
| **Zero-day** | Vulnerabilidad sin parche conocida. | — |

## 19. Control de versiones y colaboración

| Término | Qué es | Como si fuera… |
|---|---|---|
| **Add / commit / push / pull / fetch / clone** | Preparar / guardar / subir / traer y unir / traer sin unir / copiar. | — |
| **Blame / bisect / log / diff** | Quién cambió cada línea / buscar el commit que rompió algo / historial / diferencias. | — |
| **Branch protection / CODEOWNERS** | Reglas sobre ramas / dueños de rutas. | — |
| **Checkout / switch / restore** | Cambiar de rama o restaurar archivos. | — |
| **Cherry-pick** | Copiar un commit a otra rama. | — |
| **CLA / DCO** | Acuerdos de contribución. | — |
| **Commit** | Instantánea de cambios con mensaje. | — |
| **Conflicto de merge** | Dos cambios chocan; hay que resolverlo a mano. | — |
| **Control de versiones (VCS)** | Sistema que guarda el historial de cambios de los archivos. | — |
| **Copyleft / permisiva** | Obliga a mantener abierta la licencia / permite casi todo. | — |
| **Dependabot / Renovate** | Bots que actualizan dependencias. | — |
| **Fork / pull request / merge request** | Copia propia / solicitud de incorporación. | — |
| **Git** | El sistema de control de versiones más usado; distribuido. | — |
| **Git LFS** | Gestión de archivos grandes. | — |
| **GitHub / GitLab / Bitbucket / Azure Repos** | Plataformas que alojan repositorios. | — |
| **.gitignore** | Lista de archivos que Git no sigue. | — |
| **Hook de Git / pre-commit** | Script que se ejecuta antes o después de una acción. | — |
| **Issue / label / milestone / project board** | Tarea o incidencia / etiqueta / hito / tablero. | — |
| **Licencia (MIT, Apache 2.0, GPL, BSD)** | Condiciones de uso del código. | — |
| **Mensaje de commit / Conventional Commits** | Descripción / convención (`feat:`, `fix:`, `docs:`). | — |
| **Merge / fast-forward / merge commit** | Unir ramas. | — |
| **Open source / inner source** | Código abierto / abierto dentro de la empresa. | — |
| **Rama (branch) / rama principal (main) / HEAD** | Línea de desarrollo / principal / apuntador a donde estás. | — |
| **Rebase / rebase interactivo / squash** | Reescribir historial sobre otra base / reordenar / fusionar commits. | — |
| **Repositorio / clon / remoto (origin)** | Proyecto con historial / copia local / copia en el servidor. | — |
| **Reset / revert / reflog** | Mover la rama / deshacer con un commit nuevo / historial de movimientos de HEAD. | — |
| **Revisión (review) / aprobación / comentario** | Proceso de revisar un PR. | — |
| **Semantic Versioning / release notes** | Numeración de versiones / notas de la versión. | — |
| **Squash and merge / rebase and merge** | Formas de integrar un PR. | — |
| **Staging area (índice)** | Zona donde se preparan cambios antes del commit. | — |
| **Stash** | Guardar cambios sin commit temporalmente. | — |
| **Submódulo / subtree** | Repositorio dentro de otro. | — |
| **Tag / release** | Etiqueta de versión. | — |
| **Trunk-based / GitFlow / GitHub Flow** | Estrategias de ramas. | — |
| **Wiki / README / CONTRIBUTING / LICENSE / CHANGELOG** | Documentos habituales de un repositorio. | — |
| **Working tree / directorio de trabajo** | Archivos tal como los ves. | — |

## 20. Procesos, metodologías y ciclo de vida

| Término | Qué es | Como si fuera… |
|---|---|---|
| **ADR (registro de decisión de arquitectura)** | Nota corta: qué decisión técnica se tomó, por qué y qué alternativas había. | — |
| **Agile (ágil)** | Filosofía de entregar poco y a menudo, recibir opiniones y adaptarse, en vez de planificarlo todo al inicio. Nace del Manifiesto Ágil (2001). | — |
| **Backlog** | Lista priorizada de todo lo pendiente por hacer. | La lista de la compra priorizada. |
| **Blue-green** | Dos entornos idénticos: el azul sirve, el verde recibe la nueva versión; se cambia el tráfico de uno a otro. | — |
| **Cascada (waterfall)** | Fases secuenciales (requisitos → diseño → código → pruebas) sin volver atrás. Es el modelo clásico. | Construir un puente: no puedes cambiar los cimientos al final. |
| **CD (entrega / despliegue continuo)** | Entrega continua: lo que pasa CI queda listo para desplegar. Despliegue continuo: se despliega solo a producción. | — |
| **CI (integración continua)** | Cada cambio se integra y se prueba automáticamente varias veces al día. | — |
| **Code review (revisión de código)** | Otra persona lee tus cambios antes de aceptarlos. | — |
| **Criterios de aceptación / Definition of Done** | Condiciones que deben cumplirse para considerar algo terminado. | — |
| **Daily (stand-up)** | Reunión corta diaria: qué hice, qué haré, qué me bloquea. | — |
| **Despliegue canary** | Se publica la versión nueva solo a un pequeño porcentaje de usuarios para vigilarla. | El canario de la mina: avisa antes que nadie. |
| **Despliegue progresivo (rolling)** | La nueva versión va reemplazando servidores poco a poco. | — |
| **DevOps** | Cultura y prácticas para que desarrollo y operaciones trabajen juntos y se pueda entregar con rapidez y fiabilidad. | Dejar de pasarse el trabajo «por encima de la valla». |
| **DevSecOps** | DevOps con la seguridad integrada desde el principio. | — |
| **Entorno (dev, test, staging, producción)** | Copias del sistema en distintas fases: dev (desarrollo), test (pruebas), staging (ensayo general idéntico a producción), prod (usuarios reales). | — |
| **Épica** | Historia grande que se divide en otras menores. | — |
| **Error budget (presupuesto de errores)** | Cuánto fallo se puede permitir sin incumplir el objetivo; mientras sobra, se innova; si se agota, se estabiliza. | — |
| **Estimación** | Cálculo aproximado de tiempo o esfuerzo. Casi siempre optimista. | — |
| **Feature flag (interruptor de función)** | Se publica código apagado y se activa más tarde, o solo para algunos usuarios. | El interruptor de una luz ya instalada. |
| **GitFlow** | Modelo de ramas con `develop`, `feature`, `release` y `hotfix`. Más pesado. | — |
| **GitOps** | El estado deseado de la infraestructura vive en Git, y una herramienta lo aplica automáticamente. | — |
| **Historia de usuario (user story)** | Requisito escrito desde el usuario: «Como cliente, quiero buscar productos, para encontrarlos rápido». | — |
| **Hito (milestone) / Gantt** | Hito: punto de control del proyecto. Gantt: diagrama de barras con tareas y plazos. | — |
| **Incidente / on-call / guardia** | Incidente: fallo que afecta al servicio. On-call: persona de guardia que atiende las alarmas. | — |
| **Kanban** | Método visual con columnas (por hacer, en curso, hecho) y límite de tareas en curso. | Un tablero de post-its. |
| **Lean** | Eliminar desperdicio y aprender rápido; inspirado en la fabricación de Toyota. | — |
| **LGTM / nit / WIP** | LGTM: «looks good to me» (me parece bien). Nit: detalle menor. WIP: trabajo en curso. | — |
| **Merge / rebase / cherry-pick** | Merge: unir ramas. Rebase: reescribir tus cambios sobre otra base. Cherry-pick: copiar un cambio concreto a otra rama. | — |
| **MTTR / MTBF** | MTTR: tiempo medio de recuperación. MTBF: tiempo medio entre fallos. | — |
| **MVP (producto mínimo viable)** | La versión más pequeña que ya aporta valor y permite aprender. | Un primer prototipo de la tarta, no la boda entera. |
| **Pipeline (tubería)** | Cadena automática de pasos (compilar → probar → analizar → empaquetar → desplegar). Si un paso falla, se detiene. | Una cinta de montaje de fábrica. |
| **Platform engineering / plataforma interna** | Equipo que construye herramientas internas (golden paths) para que los demás desplieguen sin ser expertos en infraestructura. | — |
| **PoC (prueba de concepto)** | Experimento para ver si algo es posible; se tira después. | — |
| **Postmortem (análisis posincidente)** | Informe sin culpables de qué pasó en un incidente, por qué y cómo evitarlo. | — |
| **Product Owner / Scrum Master** | Product Owner: decide qué se construye y su prioridad. Scrum Master: ayuda al equipo a seguir el método y quita obstáculos. | — |
| **Prototipo / spike** | Prototipo: maqueta que se enseña. Spike: breve investigación técnica para resolver una duda. | — |
| **Pull request / merge request (PR)** | Solicitud para incorporar tus cambios a la rama principal, con revisión previa. | — |
| **QA / UAT** | QA: control de calidad. UAT: pruebas de aceptación hechas por el usuario o el cliente. | — |
| **Release (lanzamiento)** | Versión que se entrega a los usuarios. | — |
| **Release candidate (RC)** | Versión casi final que se prueba antes de publicar la definitiva. | — |
| **Retrospectiva** | Reunión al final del sprint para mejorar el equipo: qué fue bien, qué no. | — |
| **RFC / design doc** | Documento donde se propone y discute un cambio técnico antes de hacerlo. | — |
| **Roadmap (hoja de ruta)** | Plan a alto nivel de qué se hará y en qué orden. | — |
| **Rollback** | Volver a la versión anterior si la nueva falla. | Deshacer. |
| **Runbook** | Guía paso a paso para resolver un problema operativo conocido. | El manual de emergencias. |
| **Scrum** | Marco ágil con iteraciones cortas (sprints) y roles definidos. | — |
| **SDLC (ciclo de vida del desarrollo)** | Todas las fases por las que pasa un software: idea, análisis, diseño, desarrollo, pruebas, despliegue y mantenimiento. | — |
| **SLI / SLO / SLA** | SLI: medida (p. ej. % de respuestas correctas). SLO: objetivo interno (99,9 %). SLA: compromiso contractual con el cliente. | — |
| **Sprint** | Periodo fijo (normalmente 1–4 semanas) en el que se completa un conjunto de trabajo. | — |
| **SRE (Site Reliability Engineering)** | Disciplina (nacida en Google) que trata la operación como un problema de ingeniería: objetivos de fiabilidad y automatización. | — |
| **Stage / job / step** | Etapas del pipeline: stage (fase), job (tarea dentro de ella), step (paso individual). | — |
| **Story points / velocidad** | Puntos: medida relativa de esfuerzo. Velocidad: cuántos puntos completa el equipo por sprint. | — |
| **Trunk-based development** | Todos integran en una rama principal con cambios pequeños y frecuentes. | — |
| **Ventana de mantenimiento** | Franja horaria acordada para hacer cambios o paradas. | — |
| **XP (Extreme Programming)** | Conjunto de prácticas técnicas: pruebas primero, programación en pareja, integración continua. | — |

## 21. Producto, gestión y diseño

| Término | Qué es | Como si fuera… |
|---|---|---|
| **Accesibilidad / inclusión** | Diseñar para todas las personas. | — |
| **Alcance (scope) / scope creep** | Lo incluido / crecimiento sin control. | — |
| **Analítica de producto** | Medir cómo usan el producto (Amplitude, Mixpanel, GA). | — |
| **Arquitectura de la información / navegación** | Cómo se organiza el contenido. | — |
| **Backlog / refinamiento (grooming)** | Lista de pendientes / aclararlos y estimarlos. | — |
| **Cambio de alcance / control de cambios** | Gestionar modificaciones pedidas. | — |
| **Camino crítico** | Tareas que determinan la fecha final. | — |
| **Capacidad / compromiso / forecast** | Esfuerzo disponible / lo prometido / lo previsto. | — |
| **Churn / retención / conversión / ARPU / LTV / CAC** | Abandono / permanencia / paso a cliente / ingreso por usuario / valor de vida / coste de captación. | — |
| **Contrato precio cerrado / tiempo y materiales** | Importe fijo / se paga por horas. | — |
| **Criterios de aceptación / DoR / DoD** | Cuándo está bien / listo para empezar / terminado. | — |
| **CTO / VP Engineering / CPO** | Máximo responsable técnico / de ingeniería / de producto. | — |
| **Dark pattern** | Diseño que engaña al usuario. | — |
| **Definition of Ready** | Condiciones para entrar en un sprint. | — |
| **Delivery manager / release manager** | Asegura que lo planificado se entrega / gestiona los lanzamientos. | — |
| **Deprecación / sunset** | Retirada planificada. | — |
| **Design system / componentes / tokens** | Sistema de diseño común. | — |
| **Discovery / delivery** | Descubrir qué construir / construirlo. | — |
| **Documentación de producto: PRD / one-pager / spec** | Documento de requisitos / resumen de una página / especificación. | — |
| **Engineering manager (EM)** | Dirige personas de un equipo de ingeniería. | — |
| **Entrevista de usuario / persona / customer journey** | Conversación / usuario tipo / recorrido del cliente. | — |
| **Épica / historia / tarea / subtarea / spike** | Niveles de trabajo; spike: investigación acotada. | — |
| **Equipo multifuncional (cross-functional)** | Con todos los perfiles necesarios. | — |
| **Escalado ágil (SAFe, LeSS, Nexus)** | Marcos para muchos equipos. | — |
| **Feature flag / lanzamiento gradual** | Activar por fases. | — |
| **Figma / Sketch / Adobe XD** | Herramientas de diseño. | — |
| **Funnel (embudo) / cohorte** | Pasos hasta convertir / grupo de usuarios con algo en común. | — |
| **Gantt / PERT / EDT (WBS)** | Diagrama de barras / de redes / descomposición del trabajo. | — |
| **Gestión del cambio** | Ayudar a las personas a adoptar algo nuevo. | — |
| **Go-to-market** | Plan para llevarlo al mercado. | — |
| **Gobierno de TI / ITIL / COBIT** | Marcos de gestión de servicios y control. | — |
| **Handoff** | Entrega del diseño a desarrollo. | — |
| **Heurísticas de Nielsen** | Diez reglas de usabilidad. | — |
| **Historia de usuario (INVEST)** | Independiente, negociable, valiosa, estimable, pequeña, comprobable. | — |
| **Hito / entregable** | Punto de control / resultado entregable. | — |
| **Impediment / blocker** | Lo que impide avanzar. | — |
| **Incidencia / problema / cambio / petición (ITIL)** | Tipos de gestión de servicio. | — |
| **Increment (incremento)** | Resultado utilizable de un sprint. | — |
| **Jobs to be done (JTBD)** | Qué «trabajo» contrata el usuario al usar el producto. | — |
| **Madurez / estándar / CMMI** | Niveles de madurez de procesos. | — |
| **Mesa de ayuda / ticket** | Sistema de atención y su solicitud. | — |
| **Microcopy** | Textos pequeños de la interfaz. | — |
| **MRR / ARR** | Ingresos recurrentes mensuales / anuales. | — |
| **MVP / MLP / prototipo / PoC** | Mínimo viable / mínimo atractivo / maqueta / prueba de concepto. | — |
| **OKR / KPI / North Star Metric** | Objetivos y resultados / indicadores / métrica que resume el éxito. | — |
| **Onboarding / empty state / error state** | Primera experiencia / pantalla sin datos / pantalla de fallo. | — |
| **Pliego / RFP / propuesta / SOW** | Petición de ofertas / propuesta / descripción del trabajo contratado. | — |
| **Presupuesto / ROI / TCO / CAPEX / OPEX** | Plan económico / retorno / coste total / inversión / gasto operativo. | — |
| **Priorización: MoSCoW, RICE, WSJF, Kano** | Técnicas para ordenar: debe/debería/podría/no, alcance-impacto-confianza-esfuerzo, coste del retraso, satisfacción. | — |
| **Problema / solución / hipótesis** | Se valida el problema antes de la solución. | — |
| **Product Manager (PM)** | Define la estrategia y la visión del producto; une negocio, usuarios y tecnología. | — |
| **Product Owner (PO)** | Responsable del valor del producto: decide **qué** y en qué orden se construye. | — |
| **Product-market fit / pivotar** | Encaje con el mercado / cambiar de rumbo. | — |
| **Producto vs proyecto** | Producto: algo que evoluciona con el tiempo. Proyecto: esfuerzo con principio y fin. | — |
| **Program manager** | Coordina varios proyectos relacionados. | — |
| **Project Manager (PM, jefe de proyecto)** | Gestiona plazos, presupuesto, riesgos y recursos de un proyecto. | — |
| **RACI** | Responsable, aprobador, consultado, informado. | — |
| **Riesgo / mitigación / plan de contingencia** | Qué puede ir mal / cómo reducirlo / plan B. | — |
| **Scrum Master / agile coach** | Facilita el método ágil y quita obstáculos / acompaña la transformación. | — |
| **Service design / blueprint** | Diseñar todo el servicio, incluidos procesos internos. | — |
| **Service desk / soporte N1, N2, N3** | Atención al usuario por niveles. | — |
| **SLA / OLA** | Acuerdo con el cliente / acuerdo interno. | — |
| **Sprint planning / daily / review / retro** | Ceremonias de Scrum. | — |
| **Squad / tribe / chapter / guild** | Estructura de equipos de Spotify. | — |
| **Stakeholder / sponsor** | Interesado / quien patrocina o financia. | — |
| **Story points / T-shirt sizing / planning poker** | Estimación relativa / por tallas / con cartas. | — |
| **Tech lead** | Guía técnica de un equipo. | — |
| **Test de usuario / card sorting / tree testing** | Probar con personas / agrupar contenidos / probar la navegación. | — |
| **Time-to-market** | Tiempo hasta lanzar. | — |
| **Trabajo asíncrono / remoto** | Sin coincidir en tiempo y lugar. | — |
| **Trazabilidad** | Poder seguir un requisito hasta su prueba y su código. | — |
| **Triple restricción** | Alcance, tiempo y coste. | — |
| **UI (interfaz de usuario)** | Lo visual: pantallas, colores, componentes. | — |
| **UX (experiencia de usuario)** | Cómo se siente al usar el producto. | — |
| **UX researcher / UX writer / product designer** | Investiga usuarios / escribe textos de interfaz / diseña el producto. | — |
| **Velocidad / burndown / burnup / cycle time / lead time** | Medidas del ritmo del equipo y del flujo. | — |
| **Visión / misión / estrategia / roadmap** | Hacia dónde / para qué / cómo / en qué orden. | — |
| **WIP limit** | Límite de trabajo en curso. | — |
| **Wireframe / mockup / prototipo** | Boceto / diseño visual / simulación navegable. | — |

## 22. Equipo, negocio y cultura

| Término | Qué es | Como si fuera… |
|---|---|---|
| **Bikeshedding** | Discutir sin fin sobre algo trivial mientras lo importante se ignora. | — |
| **Bleeding edge / cutting edge** | Lo más nuevo y arriesgado / lo más avanzado. | — |
| **Build vs buy** | Decidir entre construir algo o comprarlo. | — |
| **Código abierto (open source) / licencias (MIT, Apache, GPL)** | Código con licencia que permite verlo, usarlo y a veces modificarlo; cada licencia pone condiciones. | — |
| **Dogfooding** | Usar el equipo su propio producto. | — |
| **Inner source** | Aplicar prácticas de código abierto dentro de una empresa. | — |
| **Junior / mid / senior** | Niveles de experiencia. | — |
| **NIH (Not Invented Here)** | Rechazar soluciones externas solo por no haberlas inventado uno mismo. | — |
| **OKR / KPI** | Objetivos y resultados clave / indicadores de rendimiento. | — |
| **Onboarding** | Proceso de incorporar a una persona nueva al equipo. | — |
| **Pair programming / mob programming** | Dos personas (o todo el equipo) programando juntas en un mismo ordenador. | — |
| **PM / Product Manager / Project Manager** | Producto: decide qué construir y por qué. Proyecto: gestiona plazos y recursos. | — |
| **Remoto / híbrido / asíncrono** | Trabajo desde casa, mezcla con oficina, o sin necesidad de coincidir en el tiempo. | — |
| **Rubber duck debugging** | Explicar el problema en voz alta a un patito de goma para encontrar el fallo. | — |
| **SaaS / B2B / B2C** | Software como servicio; empresas que venden a empresas / a consumidores. | — |
| **Silver bullet (bala de plata)** | La idea de una solución mágica para todo. No existe. | — |
| **Stakeholder (parte interesada)** | Cualquiera a quien le afecta o le importa el proyecto. | — |
| **Sunset (retirada)** | Retirar un producto de forma planificada. | — |
| **TCO / ROI** | TCO: coste total de tener algo. ROI: retorno de la inversión. | — |
| **Tech lead / arquitecto / staff engineer** | Perfiles técnicos senior que guían decisiones. | — |
| **Technical writer** | Persona que escribe la documentación técnica. | — |
| **Works on my machine («en mi máquina funciona»)** | Frase clásica ante un fallo que solo ocurre en otro entorno. Origen de los contenedores. | — |
| **Yak shaving** | Una cadena de tareas secundarias hasta olvidar la tarea original. | — |

## 23. Perfiles de un equipo de software

| Perfil | Qué hace |
|---|---|
| **Desarrollador / programador (developer)** | Escribe, prueba y mantiene código. Sinónimos: ingeniero de software, software engineer. |
| **Desarrollador frontend** | Construye lo que ve y toca el usuario en el navegador: HTML, CSS, JavaScript/TypeScript, React, Vue, Angular; cuida accesibilidad y rendimiento. |
| **Desarrollador backend** | Construye la lógica del servidor, las APIs, la seguridad y el acceso a datos (Java, C#, Python, Go, Node…). |
| **Desarrollador full-stack** | Trabaja en frontend y backend (y a menudo base de datos y despliegue). |
| **Desarrollador móvil** | Crea aplicaciones para Android e iOS (nativas o multiplataforma). |
| **Desarrollador de escritorio** | Crea aplicaciones para Windows, Linux o macOS. |
| **Desarrollador embebido / firmware** | Programa dispositivos y microcontroladores. |
| **Desarrollador de videojuegos** | Programa juegos y herramientas de juego. |
| **Ingeniero de datos (data engineer)** | Construye los flujos que mueven y preparan datos. |
| **Analista de datos** | Consulta y analiza datos para responder preguntas de negocio. |
| **Científico de datos (data scientist)** | Modela y predice con estadística y aprendizaje automático. |
| **Ingeniero de ML / de IA (ML/AI engineer)** | Lleva modelos a producción y construye aplicaciones con IA. |
| **Ingeniero de prompts / de contexto** | Diseña instrucciones y contexto para sistemas de IA (rol emergente). |
| **DBA** | Administra bases de datos. |
| **Arquitecto de software** | Toma las decisiones grandes de estructura y tecnología de un sistema. |
| **Arquitecto de soluciones** | Diseña soluciones que encajan necesidades de negocio con tecnología (a menudo en cloud). |
| **Arquitecto de la empresa** | Alinea sistemas y negocio en toda la organización. |
| **Tech lead** | Guía técnica del equipo; sigue programando. |
| **Staff / principal engineer** | Ingeniero senior de impacto en varios equipos. |
| **Engineering manager** | Gestiona personas, carreras y entrega del equipo. |
| **CTO / VP Engineering** | Dirección técnica de la empresa. |
| **Tester manual** | Prueba el software a mano y reporta defectos. |
| **QA engineer / ingeniero de calidad** | Garantiza calidad en todo el proceso. |
| **SDET / QA automation** | Programa las pruebas automáticas. |
| **Performance engineer** | Especialista en rendimiento y pruebas de carga. |
| **DevOps engineer** | Automatiza construcción, despliegue y operación. |
| **SRE** | Mantiene la fiabilidad con ingeniería. |
| **Platform engineer** | Construye la plataforma interna que usan los desarrolladores. |
| **Cloud engineer / cloud architect** | Diseña y opera infraestructura en la nube. |
| **Administrador de sistemas / de redes** | Mantiene servidores y redes. |
| **Ingeniero de seguridad / AppSec** | Protege aplicaciones e infraestructura. |
| **Pentester / analista SOC** | Ataca de forma autorizada / vigila alertas. |
| **Product Owner** | Gestiona el backlog y maximiza el valor. |
| **Product Manager** | Define estrategia y visión del producto. |
| **Project Manager / delivery manager** | Planifica y controla plazos, coste y riesgos. |
| **Scrum Master / agile coach** | Facilita la forma de trabajar. |
| **Business analyst (analista funcional)** | Traduce necesidades del negocio en requisitos. |
| **Analista técnico** | Detalla requisitos técnicos y de integración. |
| **UX / UI designer / product designer** | Diseña la experiencia y la interfaz. |
| **UX researcher** | Investiga a los usuarios. |
| **Technical writer** | Escribe la documentación. |
| **Developer relations (DevRel)** | Conecta la empresa con la comunidad de desarrolladores. |
| **Soporte técnico / customer success** | Atiende a los clientes. |
| **Data steward / DPO** | Responsable de la calidad de los datos / de la protección de datos. |
| **Becario / junior / mid / senior / lead** | Niveles de experiencia. |
| **T-shaped / generalista / especialista** | Profundo en una cosa y amplio en otras / de todo un poco / experto en una. |

## 24. Frases de reunión y jerga por perfil

| Frase | Qué significa | Quién la dice |
|---|---|---|
| **«Esto es legacy»** | Es un sistema antiguo del que depende el negocio y que da miedo tocar. | Desarrollo, arquitectura |
| **«Hay mucha deuda técnica»** | Se tomaron atajos que ahora frenan el trabajo. | Desarrollo, arquitectura, PM |
| **«Hay que refactorizar»** | Reordenar el código sin cambiar lo que hace. | Desarrollo |
| **«No escala»** | Cuando crezca la carga, dejará de funcionar bien. | Arquitectura, backend, infra |
| **«Es un cuello de botella»** | La parte más lenta que limita a todo el sistema. | Backend, infra |
| **«Rompe la retrocompatibilidad»** | Lo que antes funcionaba dejará de hacerlo. | Backend, producto |
| **«Es un breaking change»** | Un cambio que obliga a los demás a adaptarse. | Backend, frontend |
| **«Mergeo y lo despliego»** | Uno mi cambio a la rama principal y lo pongo en un entorno. | Desarrollo |
| **«La PR está lista para review»** | Mi cambio está listo para que lo revisen. | Desarrollo |
| **«LGTM» / «nit»** | Me parece bien / detalle menor. | Desarrollo |
| **«Se ha roto el pipeline»** | La cadena automática de construcción y pruebas falla. | Desarrollo, DevOps |
| **«Está en verde»** | Todas las pruebas y comprobaciones pasaron. | Todos |
| **«El build está en rojo»** | La construcción falla; nadie debería integrar hasta arreglarlo. | Desarrollo |
| **«En mi máquina funciona»** | El fallo depende del entorno, no de mi código (o eso creo). | Desarrollo, QA |
| **«No reproduce»** | No consigo hacer que falle de nuevo. | QA, desarrollo |
| **«Es flaky»** | Una prueba que falla a veces sin motivo. | QA |
| **«Cobertura al 80 %»** | Las pruebas ejecutan el 80 % del código. | QA, desarrollo |
| **«Falta un caso de borde»** | No se contempla una situación extrema. | QA, desarrollo |
| **«¿Hay pasos para reproducirlo?»** | Dime cómo provocar el fallo. | QA |
| **«Es un bloqueante»** | Impide continuar o publicar. | Todos |
| **«Lo bajamos a P2»** | Reducimos su prioridad. | PM, QA |
| **«Está en staging»** | Ya está en el entorno de ensayo, aún no en producción. | Todos |
| **«Pasa a producción esta noche»** | Se publica fuera de horas de más uso. | Infra, PM |
| **«Hacemos un rollback»** | Volvemos a la versión anterior. | Infra, desarrollo |
| **«Hot fix a producción»** | Corrección urgente fuera del calendario. | Todos |
| **«Despliegue canario»** | Se publica a unos pocos usuarios primero. | Infra |
| **«Detrás de un feature flag»** | Está escondido tras un interruptor y se activa cuando se decida. | Desarrollo, producto |
| **«¿Cuál es el SLA?»** | ¿Qué nivel de servicio nos hemos comprometido a dar? | PM, infra |
| **«Hemos agotado el error budget»** | Ya hemos fallado lo permitido; toca estabilizar antes de añadir funciones. | SRE |
| **«¿Qué dice la observabilidad?»** | ¿Qué muestran los logs, métricas y trazas? | Infra, backend |
| **«Hay un incidente SEV1»** | El servicio está caído o muy afectado. | Infra, soporte |
| **«Haremos un postmortem»** | Analizaremos qué pasó sin buscar culpables. | Infra, PM |
| **«Estoy de guardia (on-call)»** | Me avisan si falla algo. | Infra, desarrollo |
| **«Es un single point of failure»** | Si cae esa pieza, cae todo. | Arquitectura, infra |
| **«Necesitamos alta disponibilidad»** | Que el sistema siga funcionando aunque algo falle. | Arquitectura, infra |
| **«Pasémoslo a microservicios»** | Dividirlo en servicios pequeños e independientes. | Arquitectura |
| **«¿Monolito modular o microservicios?»** | ¿Un solo bloque ordenado por dentro o muchos servicios separados? | Arquitectura |
| **«Hace falta un ADR»** | Dejemos escrita la decisión técnica y sus motivos. | Arquitectura |
| **«Es un trade-off»** | Ganamos algo a cambio de perder otra cosa. | Arquitectura |
| **«Cumple los NFR»** | Cumple los requisitos de rendimiento, seguridad y similares. | Arquitectura |
| **«El dominio dice…»** | Lo establece el negocio, no la tecnología. | Arquitectura, producto |
| **«Es multi-tenant»** | Una instalación sirve a varios clientes con datos separados. | Arquitectura, backend |
| **«Idempotente»** | Si se repite, el resultado es el mismo. | Backend |
| **«Eventualmente consistente»** | Los datos acaban coincidiendo, pero no al instante. | Backend, datos |
| **«Es asíncrono»** | No espera; responde más tarde. | Backend, frontend |
| **«Se queda en la cola»** | El mensaje espera su turno para procesarse. | Backend |
| **«Hay una condición de carrera»** | El resultado depende del orden impredecible de dos tareas. | Backend |
| **«Se me bloquea el hilo principal»** | Una tarea larga congela la interfaz. | Frontend, móvil |
| **«Rehidrata en cliente»** | La página llegó del servidor y el navegador la hace interactiva. | Frontend |
| **«Hay que mirar el LCP»** | Medir cuánto tarda en verse el contenido principal. | Frontend |
| **«Es responsive / mobile first»** | Se adapta a cualquier pantalla, empezando por el móvil. | Frontend, UX |
| **«Necesitamos el handoff»** | Que diseño entregue las especificaciones a desarrollo. | UX, frontend |
| **«No cumple accesibilidad»** | Hay personas que no podrán usarlo. | Frontend, UX, QA |
| **«Cuéntamelo como user story»** | Escríbelo como «Como…, quiero…, para…». | Producto |
| **«¿Qué aporta valor?»** | ¿Qué consigue el usuario o el negocio? | PO, PM |
| **«Entra en el sprint»** | Se compromete para este periodo de trabajo. | Scrum |
| **«Lo metemos en el backlog»** | Queda anotado, pero sin fecha. | PO, PM |
| **«Priorizamos por impacto»** | Primero lo que más efecto tenga. | PO, PM |
| **«Hay scope creep»** | El alcance crece sin control. | PM |
| **«Es camino crítico»** | Si se retrasa, se retrasa todo el proyecto. | PM |
| **«Mitigamos el riesgo»** | Reducimos su probabilidad o su efecto. | PM |
| **«Alineemos expectativas»** | Asegurémonos de que todos esperan lo mismo. | PM, PO |
| **«Lo vemos en la daily / en la retro»** | En la reunión diaria / de mejora del equipo. | Scrum |
| **«Un MVP primero»** | Lo mínimo útil para aprender antes de invertir más. | Producto |
| **«Hay que pivotar»** | Cambiar de rumbo según lo aprendido. | Producto |
| **«Nos falta product-market fit»** | Aún no encaja con lo que el mercado quiere. | Producto |
| **«El stakeholder no está alineado»** | Alguien interesado no comparte el plan. | PM, PO |
| **«Hay que hacer un spike»** | Investigar un tiempo corto para resolver una duda técnica. | Desarrollo, PO |
| **«Eso es un nice to have»** | Estaría bien, pero no es imprescindible. | Todos |
| **«Hay que hacer shift-left»** | Probar y revisar la seguridad antes en el proceso. | QA, seguridad |
| **«Es un falso positivo»** | Una alarma que no es un problema real. | Seguridad, QA |
| **«Hay que rotar las credenciales»** | Cambiar claves y contraseñas. | Seguridad, infra |
| **«Mínimo privilegio»** | Dar solo los permisos necesarios. | Seguridad, infra |
| **«Está en la lista de CVE»** | Tiene una vulnerabilidad pública conocida. | Seguridad |
| **«Evalúalo con evals»** | Mide con pruebas sistemáticas qué tan bien responde el sistema de IA. | IA |
| **«Es una alucinación»** | El modelo se inventó el dato. | IA, desarrollo |
| **«Necesita más contexto»** | Hay que darle al modelo más información en el prompt. | IA, desarrollo |
| **«Pásale la salida por un parser»** | Convierte la respuesta del modelo en datos estructurados. | IA |
| **«Usa RAG»** | Busca documentos y dáselos al modelo antes de responder. | IA |
| **«El agente llama a una herramienta»** | El modelo decide usar una función o servicio externo. | IA |
| **«¿Cuánto cuesta por token?»** | ¿Cuánto cobra el proveedor por cada trozo de texto procesado? | IA, PM |
| **«Se queda sin ventana de contexto»** | Ya no cabe más texto en la memoria del modelo. | IA |
| **«Hay que revisar lo que genera la IA»** | Nunca se acepta sin leer y ejecutar. | Todos |
| **«Es vibe coding»** | Programar aceptando lo que sugiere la IA sin entenderlo del todo. | Desarrollo |
| **«Human in the loop»** | Una persona revisa o aprueba antes de actuar. | IA, producto |
| **«Hay drift»** | Los datos o el comportamiento cambiaron con el tiempo y el modelo empeora. | IA, datos |
| **«Hay que alinear el modelo»** | Ajustarlo para que se comporte como queremos. | IA |
| **«Pásalo a un contenedor»** | Empaquétalo con todo lo que necesita. | DevOps, desarrollo |
| **«Está dockerizado»** | Ya se ejecuta en contenedor. | DevOps |
| **«Aplica el manifiesto»** | Despliega lo que describe el archivo de configuración. | DevOps |
| **«Lo tenemos como IaC»** | La infraestructura está descrita en código. | DevOps |
| **«Hay drift de configuración»** | Lo real ya no coincide con lo declarado. | DevOps |
| **«Sube la réplica»** | Añade una copia más del servicio. | Infra |
| **«Escala horizontalmente»** | Añade más máquinas en vez de una más grande. | Infra, arquitectura |
| **«Hay latencia alta»** | Las respuestas tardan demasiado. | Infra, backend |
| **«Cache miss»** | El dato no estaba en caché y hubo que pedirlo. | Backend, infra |
| **«Hay un memory leak»** | El programa consume cada vez más memoria. | Backend, desarrollo |
| **«Es CPU-bound / I/O-bound»** | Limitado por cálculo / por esperas de disco o red. | Backend |
| **«N+1»** | Una consulta por cada fila: lentísimo. | Backend |
| **«Falta un índice»** | La base de datos busca sin atajos. | Backend, DBA |
| **«Haz la migración»** | Aplica el cambio de estructura de la base. | Backend, DBA |
| **«Hay un deadlock en la base»** | Dos transacciones se esperan para siempre. | DBA, backend |
| **«Tenemos un lag de replicación»** | La copia va retrasada respecto a la principal. | DBA, infra |
| **«Eso es un DTO»** | Un objeto solo para transportar datos. | Backend |
| **«Inyecta la dependencia»** | Dale desde fuera lo que necesita. | Desarrollo |
| **«Es un singleton»** | Existe una sola instancia en todo el programa. | Desarrollo |
| **«No es thread-safe»** | Falla si lo usan varios hilos a la vez. | Desarrollo |
| **«Hay que parametrizar el flag»** | Que la opción se pueda elegir al ejecutar (`--opción`). | Desarrollo |
| **«Dockeriza y haz el compose»** | Contenedor y orquestación de varios servicios. | DevOps |
| **«Esto es un hack»** | Es una solución chapucera y provisional. | Todos |
| **«Yak shaving»** | Estamos resolviendo problemas laterales sin avanzar el principal. | Todos |
| **«Bikeshedding»** | Discusión larga sobre un detalle trivial. | Todos |
| **«Eso no es un bug, es un feature»** | Se comporta así a propósito (o se justifica así). | Todos |
| **«Lo hablamos offline»** | Lo hablamos aparte de esta reunión. | Todos |
| **«Hagamos un quick win»** | Una mejora rápida y de bajo coste. | PM, PO |
| **«No reinventemos la rueda»** | Usemos algo que ya existe. | Arquitectura, desarrollo |
| **«Sigue el golden path»** | Usa el camino recomendado y ya preparado. | Plataforma |
| **«Se necesita un PoC»** | Una prueba de concepto antes de decidir. | Arquitectura, producto |
| **«Bus factor 1»** | Solo una persona entiende esta parte. | PM, desarrollo |
| **«Pair programming»** | Dos personas programan juntas. | Desarrollo |
| **«Hay que documentar»** | Dejarlo por escrito para el futuro. | Todos |

## 25. Siglas y abreviaturas habituales

| Término | Qué es | Como si fuera… |
|---|---|---|
| **CI / CD / CI/CD** | Integración continua / entrega o despliegue continuo. | — |
| **CLI** | Interfaz de línea de comandos. | — |
| **CPU / GPU / TPU / RAM / SSD** | Componentes de hardware. | — |
| **CRUD** | Crear, leer (read), actualizar y borrar (delete): las cuatro operaciones básicas sobre datos. | — |
| **CVE / CVSS / OWASP** | Vulnerabilidades, gravedad y organización de seguridad web. | — |
| **DevOps / DevSecOps / GitOps / MLOps** | Prácticas de operación integradas. | — |
| **DNS** | Sistema de nombres de dominio. | — |
| **GDPR / RGPD / LOPDGDD** | Normativa de protección de datos. | — |
| **GUI** | Interfaz gráfica de usuario. | — |
| **HA / DR / BCP** | Alta disponibilidad / recuperación ante desastres / continuidad de negocio. | — |
| **HTML / CSS / JS / TS** | Lenguajes de la web. | — |
| **HTTP / HTTPS / TLS / SSL** | Protocolo web y su cifrado. | — |
| **I/O (E/S)** | Entrada y salida. | — |
| **IaaS / PaaS / SaaS / FaaS / BaaS** | Modelos de servicio en la nube. | — |
| **IaC** | Infraestructura como código. | — |
| **IDE** | Entorno de desarrollo integrado (editor, compilador y depurador en uno). | — |
| **JDK / JRE / SDK .NET** | Kit y entorno de ejecución de Java; kit de .NET. | — |
| **JSON / XML / YAML / CSV** | Formatos de datos. | — |
| **JVM / CLR / JIT / GC** | Máquinas virtuales de Java y .NET, compilación al vuelo, recolector de basura. | — |
| **JWT / OAuth / OIDC / SAML** | Tokens y protocolos de identidad. | — |
| **KPI / OKR / ROI / TCO** | Indicadores y economía. | — |
| **LLM / SLM / RAG / MCP / NLP / ML / DL / AGI** | Siglas de IA. | — |
| **LTS / EOL / GA / RC / beta / alpha** | Soporte largo / fin de vida / disponibilidad general / candidata / versiones de prueba. | — |
| **MFA / 2FA / SSO** | Autenticación multifactor / de dos factores / inicio único. | — |
| **MTTR / MTBF / MTTD** | Tiempo medio de reparación / entre fallos / de detección. | — |
| **MVP / PoC / MLP** | Producto mínimo viable / prueba de concepto / mínimo atractivo. | — |
| **OOP / POO / FP** | Programación orientada a objetos / funcional. | — |
| **OS / SO** | Sistema operativo. | — |
| **PR / MR / LGTM / WIP / NIT** | Solicitud de integración / aprobación / en curso / detalle menor. | — |
| **PRD / BRD / SRS** | Documentos de requisitos de producto / de negocio / de software. | — |
| **RBAC / ABAC / IAM** | Control de acceso por roles / atributos / gestión de identidades. | — |
| **RFC / ADR / RFP** | Propuesta técnica / decisión de arquitectura / petición de ofertas. | — |
| **RPO / RTO** | Pérdida / tiempo de recuperación máximos admisibles. | — |
| **RTFM / FOSS / FAQ** | «Lee el manual» / software libre y de código abierto / preguntas frecuentes. | — |
| **SAST / DAST / SCA / SBOM** | Análisis de seguridad y lista de componentes. | — |
| **SDK** | Kit de desarrollo de software. | — |
| **SIEM / SOC / EDR / WAF / IDS / IPS** | Herramientas y centros de seguridad. | — |
| **SLA / SLO / SLI** | Acuerdo / objetivo / indicador de nivel de servicio. | — |
| **SOLID / DRY / KISS / YAGNI** | Principios de diseño. | — |
| **SPA / MPA / SSR / SSG / CSR** | Tipos de aplicación y renderizado web. | — |
| **SQL / NoSQL** | Lenguaje de consulta / bases no relacionales. | — |
| **SSH / SFTP / FTP** | Acceso y transferencia segura/clásica. | — |
| **TCP / UDP / IP** | Protocolos de red. | — |
| **TDD / BDD / DDD / CQRS** | Enfoques de desarrollo y diseño. | — |
| **TL;DR / FYI / ASAP / ETA / EOD / OOO / IMO / AFAIK** | «Demasiado largo, no lo leí» / para tu información / cuanto antes / hora estimada / fin del día / fuera de la oficina / en mi opinión / que yo sepa. | — |
| **UAT / QA / QC** | Pruebas de aceptación / aseguramiento / control de calidad. | — |
| **UI / UX** | Interfaz de usuario / experiencia de usuario. | — |
| **VM / VPS** | Máquina virtual / servidor virtual privado. | — |
| **VPN / VPC / LAN / WAN** | Redes privadas virtuales y locales/extensas. | — |
| **XSS / CSRF / SQLi / RCE / SSRF** | Ataques frecuentes. | — |

