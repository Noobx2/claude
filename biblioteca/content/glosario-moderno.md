# Glosario moderno del desarrollo

Términos nuevos y tradicionales explicados en castellano claro. Columnas: término, qué es y «como si fuera…».

## 1. Código y sistemas «viejos» y «nuevos»

Las palabras con las que se habla de lo que ya existe y de cómo cambiarlo.

| Término | Qué es | Como si fuera… |
|---|---|---|
| **Legacy (sistema heredado)** | Software antiguo que **sigue en uso** y del que depende el negocio, aunque use tecnología anticuada y dé miedo tocarlo. No significa «malo»: significa «funciona y paga las nóminas». | Un edificio antiguo en el que aún vive gente: no se puede derribar, pero cada reforma puede romper una tubería. |
| **Monolito** | Aplicación construida como **un solo bloque**: todo el código se compila, se despliega y se escala junto. | Una casa de una sola pieza: para cambiar una ventana hay que tener en cuenta toda la casa. |
| **Deuda técnica** | Atajos tomados hoy para ir rápido que **se pagan con intereses** mañana (más lentitud, más fallos). | Dejar los platos sin fregar: ahorras tiempo hoy, mañana hay que fregar el doble. |
| **Refactorización (refactor)** | Reordenar el código **sin cambiar lo que hace**, para que sea más fácil de entender y modificar. | Reorganizar el taller: las mismas herramientas, mejor colocadas. |
| **Código espagueti** | Código enredado donde todo depende de todo y no se sabe qué afecta a qué. | Un plato de espaguetis: tirar de un hilo mueve todos. |
| **Bug** | Un fallo del programa: hace algo distinto de lo esperado. | Una errata en un libro. |
| **Hotfix** | Corrección **urgente** de un fallo en producción, fuera del calendario normal. | Un parche de emergencia en una tubería que gotea. |
| **Parche (patch)** | Pequeña modificación que corrige un fallo o un problema de seguridad. | — |
| **Workaround** | Solución temporal que **esquiva** un problema sin resolver la causa. | Poner un trapo bajo la gotera. |
| **Hack / kludge** | Solución chapucera pero que funciona. A veces es elegante, casi siempre es provisional. | — |
| **Deprecado (deprecated)** | Algo que **sigue funcionando pero no se debe usar**, porque será eliminado en el futuro. | Un letrero de «salida en desuso». |
| **Breaking change (cambio que rompe)** | Cambio en una nueva versión que **hace que lo que antes funcionaba deje de funcionar**. | Cambiar la forma del enchufe: los aparatos antiguos ya no encajan. |
| **Retrocompatibilidad** | Que una versión nueva siga funcionando con lo hecho para la versión anterior. | — |
| **EOL (End of Life) / fin de vida** | Fecha a partir de la cual un producto **deja de recibir actualizaciones** y soporte. | Un coche cuyo fabricante ya no fabrica repuestos. |
| **LTS (Long Term Support)** | Versión con **soporte prolongado** (años). Es la que se elige para producción. | La edición «de confianza» frente a la edición «de moda». |
| **Migración** | Mover datos o aplicaciones de un sistema a otro (otra base de datos, otro servidor, otra versión). | Una mudanza. |
| **Reescritura (rewrite) / Big bang** | Tirar el sistema viejo y construir uno nuevo **de golpe**. Arriesgado y muchas veces fracasa. | Derribar la casa antes de saber si la nueva tendrá tejado. |
| **Strangler fig (higuera estranguladora)** | Estrategia de migración **por trozos**: el sistema nuevo va sustituyendo partes del viejo hasta que el viejo desaparece. | Como la higuera que crece alrededor de un árbol hasta reemplazarlo. |
| **Greenfield / Brownfield** | Greenfield: proyecto desde cero. Brownfield: proyecto sobre algo que ya existe. | Solar vacío frente a reforma de una casa. |
| **Bit rot (pudrición del software)** | El software **envejece sin tocarlo**: las bibliotecas, el sistema y los servicios cambian alrededor y deja de funcionar. | Un coche parado que se estropea solo. |
| **Vendor lock-in (cautividad del proveedor)** | Dependencia de un proveedor tan fuerte que cambiar de él cuesta muchísimo. | Una impresora que solo acepta sus propios cartuchos. |
| **Boilerplate (código repetitivo)** | Código que hay que escribir una y otra vez casi igual para que lo básico funcione. | El encabezado de una carta formal. |
| **Hardcodear** | Escribir un valor directamente en el código (contraseña, dirección…) en vez de ponerlo en la configuración. | Pintar el número de teléfono en la pared. |
| **Número mágico (magic number)** | Un número en el código sin explicación de qué significa. | «Añade 86400» (¿segundos de un día?). |
| **Código muerto (dead code)** | Código que **nunca se ejecuta** pero sigue ahí. | Una puerta que no lleva a ninguna parte. |
| **Mantenimiento** | Todo el trabajo **después de la primera entrega**: corregir fallos, actualizar, adaptar. Suele costar más que construir. | — |
| **Olor a código (code smell)** | Señal en el código de que **algo probablemente está mal** (aunque funcione): funciones enormes, nombres confusos, duplicaciones. | Olor a humo: aún no hay incendio, pero conviene mirar. |
| **Documentación viva** | Documentación que **se actualiza con el código**, y se prueba o se genera de él, para que no mienta. | — |
| **Conocimiento tribal (tribal knowledge)** | Lo que solo sabe «el de siempre» y no está escrito en ningún sitio. | La receta de la abuela que nadie apuntó. |
| **Bus factor (factor autobús)** | Cuántas personas tendrían que desaparecer (por ejemplo, atropelladas por un autobús) para que el proyecto se quede sin nadie que lo entienda. Si es 1, hay riesgo. | — |

## 2. Piezas básicas del software

Los ladrillos con los que se habla de programar.

| Término | Qué es | Como si fuera… |
|---|---|---|
| **Lenguaje de programación** | Idioma formal para dar instrucciones al ordenador (Java, C#, Python…). | — |
| **Compilador** | Programa que **traduce de una vez** el código a lenguaje máquina o intermedio antes de ejecutarlo. | Traducir un libro entero antes de publicarlo. |
| **Intérprete** | Programa que **lee y ejecuta el código línea a línea** en el momento. | Un traductor simultáneo en una conferencia. |
| **Bytecode / código intermedio** | Código a medio camino entre el que escribes y el del procesador, que una máquina virtual ejecuta (Java y C# lo usan). | — |
| **Máquina virtual (JVM, CLR)** | Programa que ejecuta el bytecode y hace que el mismo programa corra en distintos sistemas. JVM es la de Java; CLR la de .NET. | Un adaptador universal de enchufes. |
| **JIT (compilación en tiempo de ejecución)** | La máquina virtual **traduce a lenguaje máquina las partes más usadas mientras el programa corre**, para que vayan rápido. | — |
| **Recolector de basura (garbage collector)** | Mecanismo que **libera automáticamente la memoria** de lo que el programa ya no usa. | Un servicio de limpieza que pasa solo. |
| **Runtime (entorno de ejecución)** | Lo que hace falta instalado para **que un programa se ejecute** (Node, JRE, .NET Runtime). | — |
| **Biblioteca (library)** | Código de otros que **tú llamas** cuando lo necesitas. | Una caja de herramientas: tú decides cuándo usarla. |
| **Framework (marco de trabajo)** | Estructura que **te llama a ti**: define cómo se organiza la aplicación y tú rellenas los huecos (Spring, ASP.NET, React…). | Un molde: tú pones el contenido, él decide la forma. |
| **SDK (kit de desarrollo)** | Paquete con herramientas, bibliotecas y documentación para programar para una plataforma (por ejemplo, el SDK de Android). | — |
| **API** | **Contrato** que dice cómo un programa puede pedirle cosas a otro: qué pedir, cómo y qué responde. | La carta de un restaurante: qué puedes pedir y cómo, sin entrar en la cocina. |
| **Endpoint** | Una dirección concreta de una API a la que se le hace una petición (`/api/products`). | Una ventanilla concreta de la oficina. |
| **SDK vs API vs librería** | API: el contrato. Librería: código que lo ofrece. SDK: caja con librerías, herramientas y documentación. | — |
| **Dependencia** | Código de otros que tu proyecto **necesita para funcionar**. | Los ingredientes comprados que usa tu receta. |
| **Gestor de paquetes** | Herramienta que instala y actualiza dependencias (npm, Maven, NuGet, pip, cargo). | Una tienda de ingredientes con su lista de la compra. |
| **Build (compilación / construcción)** | Proceso que **convierte el código fuente en algo ejecutable**: compilar, empaquetar, probar. | Hornear la masa. |
| **Artefacto (artifact)** | El **resultado de un build**: un `.jar`, un `.dll`, una imagen de contenedor… | El pan ya horneado. |
| **Binario** | Archivo ejecutable ya compilado (no legible por personas). | — |
| **Repositorio (repo)** | Carpeta con el código y **todo su historial de cambios** (normalmente con Git). | Un álbum que guarda todas las versiones. |
| **Monorepo / polyrepo** | Monorepo: muchos proyectos en un mismo repositorio. Polyrepo: un repositorio por proyecto. | — |
| **Versionado semántico (SemVer)** | Numeración `MAYOR.MENOR.PARCHE`: sube MAYOR si rompe cosas, MENOR si añade sin romper, PARCHE si corrige. | 2.4.1 → 2.4.2 es solo un arreglo; 3.0.0 avisa de cambios grandes. |
| **Configuración / variables de entorno** | Valores que **cambian según dónde se ejecute** (claves, direcciones) y que se sacan del código. | — |
| **Log (registro)** | Líneas de texto que el programa va escribiendo para contar **qué está haciendo**. | El diario de a bordo. |
| **Stack trace (traza de pila)** | Lista de llamadas que muestra **por dónde iba el programa** cuando falló. | El rastro de migas del error. |
| **Excepción** | Evento de error que interrumpe el flujo normal del programa. | — |
| **Null (nulo)** | Valor que significa «**no hay nada**». Intentar usarlo es uno de los fallos más famosos. | Una caja vacía que alguien intenta abrir como si tuviera algo. |
| **Tipado estático / dinámico** | Estático: los tipos de los datos se comprueban **antes** de ejecutar (Java, C#, TypeScript). Dinámico: se comprueban **al ejecutar** (JavaScript, Python). | — |
| **Inmutabilidad** | Un dato que, una vez creado, **no cambia**. Evita errores. | Un contrato firmado: se hace uno nuevo en vez de tachar. |
| **Concurrencia / paralelismo** | Concurrencia: hacer varias cosas **a la vez en apariencia** (turnándose). Paralelismo: hacerlas **realmente a la vez** (varios núcleos). | Un camarero atendiendo varias mesas frente a varios camareros. |
| **Asíncrono (async)** | Código que **no se queda esperando**: lanza una tarea lenta y sigue con otra. | Dejar la comida al fuego y poner la mesa. |
| **Hilo (thread)** | Línea de ejecución dentro de un programa. | — |
| **Condición de carrera (race condition)** | Fallo por **depender del orden** en que dos tareas se ejecutan. | Dos personas editando a la vez el mismo documento sin avisarse. |
| **Interbloqueo (deadlock)** | Dos tareas se esperan la una a la otra **para siempre**. | Dos coches en un puente de un solo carril, cara a cara. |
| **Fuga de memoria (memory leak)** | El programa **va reservando memoria sin liberarla** hasta agotarla. | Una bañera sin desagüe con el grifo abierto. |
| **Cuello de botella (bottleneck)** | La parte más lenta que limita todo el sistema. | El embudo. |
| **Latencia / rendimiento (throughput)** | Latencia: cuánto tarda **una** petición. Rendimiento: cuántas peticiones se atienden **por segundo**. | Cuánto tarda un coche (latencia) frente a cuántos coches pasan por hora (rendimiento). |

## 3. Arquitectura: cómo se organiza un sistema

Nombres de estilos y piezas. Una arquitectura es el plano; ningún estilo es «el bueno» siempre.

| Término | Qué es | Como si fuera… |
|---|---|---|
| **Arquitectura de software** | Las **decisiones grandes** sobre cómo se divide y se comunica un sistema, difíciles de cambiar luego. | El plano de un edificio. |
| **Arquitectura en capas (layered)** | El código se divide en capas (presentación, lógica, datos) y cada una solo habla con la de al lado. | Un pastel de pisos. |
| **MVC (Modelo-Vista-Controlador)** | Separa los datos (modelo), lo que se ve (vista) y quien coordina (controlador). | Cocina (modelo), sala (vista), camarero (controlador). |
| **MVVM / MVP** | Variantes de MVC muy usadas en interfaces y móviles; cambian cómo la vista se conecta con la lógica. | — |
| **Arquitectura hexagonal (puertos y adaptadores)** | El núcleo del negocio **no conoce** bases de datos ni pantallas; habla con el exterior solo a través de «puertos» y «adaptadores». | Un aparato con tomas estándar: cambias el cable sin abrir el aparato. |
| **Clean Architecture / Onion** | Parecida a la hexagonal: las dependencias apuntan **hacia el centro** (lo importante del negocio), nunca hacia fuera. | Capas de cebolla; el centro no sabe que existe la piel. |
| **Monolito modular** | Un solo despliegue, pero **dividido por dentro en módulos bien separados**. Muchas veces es mejor punto de partida que los microservicios. | Un edificio con habitaciones independientes. |
| **Microservicios** | Aplicación dividida en **muchos servicios pequeños** que se despliegan y escalan por separado y hablan por red. | Un centro comercial de tiendas independientes en vez de un único gran almacén. |
| **SOA (arquitectura orientada a servicios)** | Antecesora de los microservicios: servicios grandes y centralizados (con ESB y SOAP). | — |
| **ESB (bus de servicios empresarial)** | Pieza central de las arquitecturas SOA por la que pasaban todos los mensajes. Hoy se considera pesada. | — |
| **Serverless (sin servidor)** | Tú subes **funciones** y el proveedor se encarga de ejecutarlas y escalarlas; pagas por uso. Hay servidores, pero no los gestionas. | Un taxi: no tienes coche, pagas por trayecto. |
| **FaaS (función como servicio)** | Forma concreta de serverless: **una función** que se ejecuta cuando ocurre un evento (AWS Lambda, Azure Functions). | — |
| **Arquitectura orientada a eventos (EDA)** | Las piezas **se avisan por eventos** («pedido creado») sin conocerse entre sí. | Un tablón de anuncios. |
| **Evento / mensaje** | Un aviso de que **algo ocurrió** (evento) o una orden para hacer algo (comando). Se manda por una cola o un tema. | — |
| **Cola de mensajes / message broker** | Intermediario que **guarda y reparte mensajes** entre sistemas (RabbitMQ, Kafka). | Un buzón entre dos oficinas. |
| **Pub/Sub (publicación/suscripción)** | Quien publica no sabe quién escucha; los suscritos reciben lo que les interesa. | Una revista y sus suscriptores. |
| **Kafka** | Plataforma que guarda un **registro ordenado e histórico de eventos** que muchos sistemas pueden leer. | — |
| **CQRS** | Separar **cómo se escribe** de **cómo se lee** un dato, con modelos distintos. | Una caja para cobrar y un mostrador aparte para consultar. |
| **Event sourcing** | En vez de guardar el estado actual, se guarda **la lista de todo lo ocurrido** y el estado se calcula. | El extracto bancario frente al saldo: el saldo sale de sumar movimientos. |
| **DDD (diseño guiado por el dominio)** | Diseñar el software **a partir del lenguaje y las reglas del negocio**. | — |
| **Lenguaje ubicuo** | Que desarrolladores y expertos del negocio **usen las mismas palabras** y el código también. | — |
| **Bounded context (contexto delimitado)** | Zona donde un término tiene **un significado concreto** (un «cliente» en facturación no es igual que en soporte). | — |
| **Agregado / entidad / objeto de valor** | Entidad: tiene identidad (un cliente). Objeto de valor: se define por sus datos (una dirección, un importe). Agregado: grupo que se trata como una unidad. | — |
| **API Gateway** | Puerta de entrada única a muchos servicios: autentica, reparte y limita. | La recepción de un edificio. |
| **BFF (Backend For Frontend)** | Un backend hecho a medida para **cada tipo de cliente** (web, móvil). | — |
| **Service mesh (malla de servicios)** | Capa que gestiona la **comunicación entre microservicios** (seguridad, reintentos, métricas) sin tocar su código. | — |
| **Sidecar** | Proceso auxiliar que corre **pegado a otro** y le añade funciones (logs, seguridad). | El sidecar de una moto. |
| **Balanceador de carga (load balancer)** | Reparte las peticiones entre varios servidores. | El que indica en el supermercado qué caja queda libre. |
| **Proxy / proxy inverso (reverse proxy)** | Proxy: intermediario del cliente. Proxy inverso: intermediario **delante de tus servidores** (Nginx). | Un portero. |
| **CDN** | Red de servidores repartidos por el mundo que **sirven copias** de tus archivos desde el más cercano al usuario. | Quioscos de prensa por todo el país en vez de una sola imprenta. |
| **Caché** | Copia temporal de algo **para no recalcularlo o pedirlo otra vez**. | Apuntar en un papel lo que consultas cada día. |
| **Escalado vertical / horizontal** | Vertical: máquina más potente. Horizontal: **más máquinas**. | Un camión más grande frente a más camiones. |
| **Alta disponibilidad (HA)** | Que el sistema **siga funcionando** aunque algo falle. | — |
| **SPOF (punto único de fallo)** | Pieza cuya caída tumba **todo** el sistema. | El único puente de un pueblo. |
| **Resiliencia / tolerancia a fallos** | Capacidad de **aguantar y recuperarse** de los fallos. | — |
| **Circuit breaker (cortacircuitos)** | Si un servicio falla repetidamente, **se deja de llamarlo** un tiempo para no empeorarlo. | Los fusibles de la casa. |
| **Reintentos con backoff** | Volver a intentar tras un fallo **esperando cada vez más**. | — |
| **Rate limiting / throttling** | Limitar cuántas peticiones puede hacer alguien en un tiempo. | — |
| **Idempotencia** | Repetir una operación **da el mismo resultado** que hacerla una vez. Clave para reintentos seguros. | Pulsar el botón de «apagar» dos veces. |
| **Consistencia eventual** | Los datos **acaban** coincidiendo en todas partes, pero no instantáneamente. | Una noticia que tarda en llegar a todos los pueblos. |
| **Teorema CAP** | En un sistema distribuido, ante una partición de red hay que elegir entre **consistencia** y **disponibilidad**. | — |
| **ACID / BASE** | ACID: transacciones estrictas y fiables (bases relacionales). BASE: más flexible y disponible, consistencia eventual (muchas NoSQL). | — |
| **Saga** | Forma de hacer una operación larga entre varios servicios con **pasos y pasos de deshacer** si algo falla. | — |
| **Sharding (particionado)** | Repartir los datos de una base **entre varias máquinas** según un criterio. | Dividir un archivo en cajones por letra inicial. |
| **Replicación** | Mantener **copias** de los datos en varias máquinas. | — |
| **Multi-tenant (multiinquilino)** | Una misma aplicación sirve a **varios clientes** manteniendo sus datos aislados. | Un edificio de apartamentos. |
| **Twelve-factor app (aplicación de 12 factores)** | Conjunto de buenas prácticas para aplicaciones en la nube (configuración en el entorno, logs como flujo, etc.). | — |
| **REST** | Estilo de API sobre HTTP con **recursos** (`/products/3`) y verbos (GET, POST, PUT, DELETE). | — |
| **GraphQL** | Lenguaje de consulta: el cliente **pide exactamente los campos que quiere** en una sola petición. | — |
| **gRPC** | Forma rápida de comunicarse entre servicios con **contratos definidos** y formato binario. | — |
| **WebSocket / SSE** | Conexiones que **se mantienen abiertas** para que el servidor envíe datos en tiempo real (chats, cotizaciones). | — |
| **Webhook** | El sistema A **te llama** (envía una petición) cuando ocurre algo, en vez de que tú le preguntes continuamente. | Que te avisen por teléfono en vez de llamar tú cada hora. |
| **OpenAPI (Swagger)** | Archivo estándar que **describe una API REST**: rutas, datos y respuestas. | — |
| **ORM** | Herramienta que **traduce entre objetos del programa y tablas** de la base de datos (JPA/Hibernate, Entity Framework). | Un intérprete entre dos idiomas. |
| **DTO** | Objeto sencillo para **transportar datos** entre capas o por la red. | Un sobre. |
| **Repositorio (patrón)** | Clase que **oculta cómo se guardan los datos** y ofrece métodos como `findById`. | — |
| **Inyección de dependencias (DI) / IoC** | En vez de que una clase **cree** lo que necesita, se lo **entregan desde fuera**. Facilita cambiar piezas y probar. | El cocinero no compra los ingredientes: se los traen. |
| **Principios SOLID** | Cinco reglas de diseño orientado a objetos: responsabilidad única, abierto/cerrado, sustitución de Liskov, segregación de interfaces e inversión de dependencias. | — |
| **DRY / KISS / YAGNI** | DRY: no repitas. KISS: hazlo simple. YAGNI: no construyas lo que **todavía no necesitas**. | — |
| **Acoplamiento / cohesión** | Acoplamiento: cuánto depende una pieza de otras (**poco** es bueno). Cohesión: cuánto encaja lo que hay dentro de una pieza (**mucho** es bueno). | — |
| **Patrón de diseño** | Solución probada a un problema habitual (Singleton, Factory, Observer, Strategy…). | Una receta clásica. |
| **Antipatrón** | Solución que parece buena y **causa problemas** habitualmente (objeto «dios», copiar-pegar). | — |
| **God object (objeto dios)** | Clase que lo sabe y lo hace todo. | — |
| **Event loop** | Mecanismo (Node, navegadores) que va atendiendo tareas **una a una** sin bloquearse. | — |

## 4. Procesos, metodologías y ciclo de vida

Cómo se organiza el trabajo de crear y mantener software.

| Término | Qué es | Como si fuera… |
|---|---|---|
| **SDLC (ciclo de vida del desarrollo)** | Todas las fases por las que pasa un software: idea, análisis, diseño, desarrollo, pruebas, despliegue y mantenimiento. | — |
| **Cascada (waterfall)** | Fases **secuenciales** (requisitos → diseño → código → pruebas) sin volver atrás. Es el modelo clásico. | Construir un puente: no puedes cambiar los cimientos al final. |
| **Agile (ágil)** | Filosofía de **entregar poco y a menudo**, recibir opiniones y adaptarse, en vez de planificarlo todo al inicio. Nace del Manifiesto Ágil (2001). | — |
| **Scrum** | Marco ágil con **iteraciones cortas** (sprints) y roles definidos. | — |
| **Sprint** | Periodo fijo (normalmente 1–4 semanas) en el que se completa un conjunto de trabajo. | — |
| **Backlog** | Lista priorizada de **todo lo pendiente** por hacer. | La lista de la compra priorizada. |
| **Product Owner / Scrum Master** | Product Owner: decide **qué** se construye y su prioridad. Scrum Master: ayuda al equipo a **seguir el método** y quita obstáculos. | — |
| **Daily (stand-up)** | Reunión corta diaria: qué hice, qué haré, qué me bloquea. | — |
| **Retrospectiva** | Reunión al final del sprint para **mejorar el equipo**: qué fue bien, qué no. | — |
| **Kanban** | Método visual con **columnas** (por hacer, en curso, hecho) y límite de tareas en curso. | Un tablero de post-its. |
| **Lean** | Eliminar desperdicio y **aprender rápido**; inspirado en la fabricación de Toyota. | — |
| **XP (Extreme Programming)** | Conjunto de **prácticas técnicas**: pruebas primero, programación en pareja, integración continua. | — |
| **DevOps** | Cultura y prácticas para que **desarrollo y operaciones** trabajen juntos y se pueda entregar con rapidez y fiabilidad. | Dejar de pasarse el trabajo «por encima de la valla». |
| **DevSecOps** | DevOps con la **seguridad integrada** desde el principio. | — |
| **SRE (Site Reliability Engineering)** | Disciplina (nacida en Google) que **trata la operación como un problema de ingeniería**: objetivos de fiabilidad y automatización. | — |
| **Platform engineering / plataforma interna** | Equipo que construye **herramientas internas** (golden paths) para que los demás desplieguen sin ser expertos en infraestructura. | — |
| **GitOps** | El estado deseado de la infraestructura **vive en Git**, y una herramienta lo aplica automáticamente. | — |
| **CI (integración continua)** | Cada cambio se **integra y se prueba automáticamente** varias veces al día. | — |
| **CD (entrega / despliegue continuo)** | Entrega continua: lo que pasa CI queda **listo** para desplegar. Despliegue continuo: se despliega **solo** a producción. | — |
| **Pipeline (tubería)** | **Cadena automática de pasos** (compilar → probar → analizar → empaquetar → desplegar). Si un paso falla, se detiene. | Una cinta de montaje de fábrica. |
| **Stage / job / step** | Etapas del pipeline: **stage** (fase), **job** (tarea dentro de ella), **step** (paso individual). | — |
| **Runner / agente** | Máquina que **ejecuta** los trabajos del pipeline. | — |
| **Release (lanzamiento)** | Versión que se entrega a los usuarios. | — |
| **Release candidate (RC)** | Versión casi final que se prueba **antes** de publicar la definitiva. | — |
| **Rollback** | **Volver a la versión anterior** si la nueva falla. | Deshacer. |
| **Despliegue canary** | Se publica la versión nueva **solo a un pequeño porcentaje** de usuarios para vigilarla. | El canario de la mina: avisa antes que nadie. |
| **Blue-green** | Dos entornos idénticos: el azul sirve, el verde recibe la nueva versión; se **cambia el tráfico** de uno a otro. | — |
| **Despliegue progresivo (rolling)** | La nueva versión **va reemplazando** servidores poco a poco. | — |
| **Feature flag (interruptor de función)** | Se publica código **apagado** y se activa más tarde, o solo para algunos usuarios. | El interruptor de una luz ya instalada. |
| **Trunk-based development** | Todos integran en una rama principal con cambios **pequeños y frecuentes**. | — |
| **GitFlow** | Modelo de ramas con `develop`, `feature`, `release` y `hotfix`. Más pesado. | — |
| **Pull request / merge request (PR)** | Solicitud para **incorporar tus cambios** a la rama principal, con revisión previa. | — |
| **Code review (revisión de código)** | Otra persona **lee tus cambios** antes de aceptarlos. | — |
| **Merge / rebase / cherry-pick** | Merge: unir ramas. Rebase: reescribir tus cambios sobre otra base. Cherry-pick: copiar **un** cambio concreto a otra rama. | — |
| **LGTM / nit / WIP** | LGTM: «looks good to me» (me parece bien). Nit: detalle menor. WIP: trabajo en curso. | — |
| **Entorno (dev, test, staging, producción)** | Copias del sistema en distintas fases: **dev** (desarrollo), **test** (pruebas), **staging** (ensayo general idéntico a producción), **prod** (usuarios reales). | — |
| **QA / UAT** | QA: control de calidad. UAT: pruebas de aceptación **hechas por el usuario o el cliente**. | — |
| **MVP (producto mínimo viable)** | La versión **más pequeña** que ya aporta valor y permite aprender. | Un primer prototipo de la tarta, no la boda entera. |
| **PoC (prueba de concepto)** | Experimento para ver **si algo es posible**; se tira después. | — |
| **Prototipo / spike** | Prototipo: maqueta que se enseña. Spike: breve investigación técnica para **resolver una duda**. | — |
| **Historia de usuario (user story)** | Requisito escrito desde el usuario: «**Como** cliente, **quiero** buscar productos, **para** encontrarlos rápido». | — |
| **Épica** | Historia **grande** que se divide en otras menores. | — |
| **Criterios de aceptación / Definition of Done** | Condiciones que deben cumplirse para considerar algo **terminado**. | — |
| **Story points / velocidad** | Puntos: medida relativa de esfuerzo. Velocidad: cuántos puntos completa el equipo por sprint. | — |
| **Roadmap (hoja de ruta)** | Plan a **alto nivel** de qué se hará y en qué orden. | — |
| **RFC / design doc** | Documento donde se **propone y discute** un cambio técnico antes de hacerlo. | — |
| **ADR (registro de decisión de arquitectura)** | Nota corta: qué decisión técnica se tomó, por qué y qué alternativas había. | — |
| **Postmortem (análisis posincidente)** | Informe **sin culpables** de qué pasó en un incidente, por qué y cómo evitarlo. | — |
| **Incidente / on-call / guardia** | Incidente: fallo que afecta al servicio. On-call: persona de guardia que atiende las alarmas. | — |
| **Runbook** | Guía paso a paso para **resolver un problema operativo** conocido. | El manual de emergencias. |
| **SLI / SLO / SLA** | SLI: medida (p. ej. % de respuestas correctas). SLO: objetivo interno (99,9 %). SLA: **compromiso contractual** con el cliente. | — |
| **MTTR / MTBF** | MTTR: tiempo medio de recuperación. MTBF: tiempo medio entre fallos. | — |
| **Error budget (presupuesto de errores)** | Cuánto fallo se **puede permitir** sin incumplir el objetivo; mientras sobra, se innova; si se agota, se estabiliza. | — |
| **Toil** | Trabajo operativo manual, repetitivo y automatizable. | — |
| **Ventana de mantenimiento** | Franja horaria acordada para hacer cambios o paradas. | — |
| **Hito (milestone) / Gantt** | Hito: punto de control del proyecto. Gantt: diagrama de barras con tareas y plazos. | — |
| **Estimación** | Cálculo aproximado de tiempo o esfuerzo. Casi siempre **optimista**. | — |

## 5. Infraestructura y nube

Dónde y cómo corre el software.

| Término | Qué es | Como si fuera… |
|---|---|---|
| **Servidor** | Ordenador (físico o virtual) que **ofrece un servicio** a otros. | — |
| **On-premise (en local)** | Infraestructura en **tus propias instalaciones** en lugar de en la nube. | — |
| **Nube (cloud)** | Usar **recursos informáticos de un proveedor por internet**, pagando por uso (AWS, Azure, Google Cloud). | Alquilar en vez de comprar. |
| **IaaS / PaaS / SaaS** | IaaS: alquilas máquinas. PaaS: alquilas una plataforma donde subes tu código. SaaS: usas el programa ya hecho (correo, CRM). | Alquilar el local, la cocina o comer en el restaurante. |
| **Región / zona de disponibilidad** | Región: área geográfica del proveedor. Zona: centro de datos independiente dentro de ella. | — |
| **Nube híbrida / multinube** | Híbrida: parte en local y parte en la nube. Multinube: **varios proveedores** a la vez. | — |
| **Máquina virtual (VM)** | Ordenador simulado dentro de otro, con su propio sistema operativo. | Un piso dentro de un edificio. |
| **Contenedor** | Paquete ligero con **tu aplicación y todo lo que necesita**, que se ejecuta igual en cualquier sitio. Comparte el núcleo del sistema. | Un contenedor de transporte: da igual el barco o el camión. |
| **Docker** | Herramienta más popular para crear y ejecutar contenedores. | — |
| **Imagen (image)** | Plantilla de solo lectura de la que se **crean** contenedores. | Una receta (imagen) y el plato servido (contenedor). |
| **Registro de contenedores (registry)** | Almacén de imágenes (Docker Hub, GitHub Container Registry). | — |
| **Dockerfile / Docker Compose** | Dockerfile: instrucciones para construir una imagen. Compose: describe y levanta **varios contenedores** juntos. | — |
| **Kubernetes (K8s)** | Sistema que **orquesta contenedores**: los reparte, los reinicia si caen y los escala. | Un director de orquesta. |
| **Pod / nodo / clúster** | Pod: unidad mínima en Kubernetes (uno o más contenedores). Nodo: máquina. Clúster: conjunto de nodos. | — |
| **Helm / manifiesto** | Helm: gestor de paquetes de Kubernetes. Manifiesto: archivo YAML que describe lo que se quiere desplegar. | — |
| **Orquestación** | Coordinar automáticamente muchos componentes (contenedores, servidores). | — |
| **IaC (infraestructura como código)** | Describir la infraestructura **en archivos** que se versionan y se aplican automáticamente (Terraform, Ansible). | Una receta escrita en lugar de cocinar de memoria. |
| **Terraform / Ansible** | Terraform: crea infraestructura. Ansible: configura máquinas. | — |
| **Edge computing (computación en el borde)** | Ejecutar código **cerca del usuario** o del dispositivo, no en un centro de datos lejano. | — |
| **Observabilidad** | Poder entender **qué ocurre dentro** del sistema a partir de lo que emite: logs, métricas y trazas. | — |
| **Logs / métricas / trazas** | Logs: eventos escritos. Métricas: números en el tiempo (CPU, peticiones/s). Trazas: recorrido de **una petición** por varios servicios. | — |
| **Monitorización y alertas** | Vigilar el sistema y avisar cuando algo se sale de lo normal. | — |
| **APM / OpenTelemetry** | APM: herramientas de rendimiento de aplicaciones. OpenTelemetry: estándar abierto para emitir logs, métricas y trazas. | — |
| **Dashboard (panel)** | Pantalla con gráficos del estado del sistema. | — |
| **FinOps** | Gestión del **coste de la nube**. | — |
| **Cold start (arranque en frío)** | Retraso de la primera ejecución de una función o contenedor que estaba «dormido». | — |
| **Health check** | Pregunta periódica al servicio: «¿estás bien?». | — |

## 6. Pruebas y calidad

Cómo se comprueba que el software hace lo que debe.

| Término | Qué es | Como si fuera… |
|---|---|---|
| **Prueba unitaria** | Prueba de **una pieza pequeña** (una función) de forma aislada. | Probar un tornillo. |
| **Prueba de integración** | Comprueba que **varias piezas funcionan juntas** (código + base de datos). | Probar que el tornillo encaja en la pieza. |
| **Prueba end-to-end (E2E)** | Simula **a un usuario real** recorriendo la aplicación completa. | — |
| **Pirámide de pruebas** | Muchas unitarias, algunas de integración y pocas E2E: más rápidas y baratas abajo. | — |
| **TDD (desarrollo guiado por pruebas)** | Escribir **primero la prueba** (que falla), luego el código mínimo que la pasa, luego mejorarlo. Ciclo rojo-verde-refactor. | — |
| **BDD** | Describir el comportamiento en **lenguaje natural** («Dado… cuando… entonces…») y convertirlo en pruebas. | — |
| **Mock / stub / fake** | Piezas **simuladas** que sustituyen a las reales durante las pruebas. | Un muñeco de pruebas de choque. |
| **Fixture** | Datos o estado preparado antes de una prueba. | — |
| **Cobertura (coverage)** | Porcentaje de código **ejecutado** por las pruebas. Alta cobertura no garantiza buenas pruebas. | — |
| **Prueba de regresión** | Comprueba que lo que antes funcionaba **sigue funcionando**. | — |
| **Smoke test (prueba de humo)** | Comprobación rápida de que **lo esencial arranca**. | Encender el aparato y ver que no sale humo. |
| **Prueba de carga / de estrés** | Carga: cuánto aguanta con el tráfico esperado. Estrés: qué pasa **al límite**. | — |
| **Prueba flaky (inestable)** | Prueba que a veces pasa y a veces falla **sin cambios** en el código. | — |
| **Pruebas de mutación** | Se **estropea el código a propósito** para ver si las pruebas lo detectan. | — |
| **Análisis estático / linter / formateador** | Analizan el código **sin ejecutarlo**: linter busca problemas, formateador unifica el estilo. | — |
| **Complejidad ciclomática** | Medida de cuántos caminos distintos tiene una función. | — |
| **Fuzzing** | Alimentar el programa con **datos aleatorios** para encontrar fallos. | — |
| **Ingeniería del caos (chaos engineering)** | Provocar fallos a propósito en producción controlada para comprobar la resiliencia. | — |
| **Shift-left** | Mover las **pruebas y la seguridad al principio** del proceso, no al final. | — |
| **Happy path / caso límite (edge case)** | Happy path: el camino sin problemas. Caso límite: situación extrema (lista vacía, texto larguísimo). | — |
| **Entorno de pruebas aislado (sandbox)** | Lugar seguro donde probar sin afectar a nada real. | — |

## 7. Datos

Cómo se guarda y se mueve la información.

| Término | Qué es | Como si fuera… |
|---|---|---|
| **Base de datos relacional (SQL)** | Datos en **tablas** relacionadas entre sí, consultadas con SQL (PostgreSQL, MySQL, SQL Server, Oracle). | Hojas de cálculo conectadas. |
| **NoSQL** | Familia de bases **no basadas en tablas**: documentos, clave-valor, grafos, columnas. | — |
| **Base de documentos** | Guarda **documentos** tipo JSON (MongoDB). | — |
| **Clave-valor / caché en memoria** | Guarda pares clave → valor muy rápido (Redis). | Un diccionario. |
| **Base de grafos** | Optimizada para **relaciones** (Neo4j). | Un mapa de conexiones. |
| **Base de series temporales** | Optimizada para datos **ordenados por tiempo** (métricas). | — |
| **Índice** | Estructura que **acelera las búsquedas**. | El índice alfabético de un libro. |
| **Transacción** | Conjunto de operaciones que se hacen **todas o ninguna**. | Una transferencia: se resta de una cuenta y se suma a la otra, o no ocurre nada. |
| **Esquema / migración de base de datos** | Esquema: estructura de las tablas. Migración: cambio **versionado** de ese esquema. | — |
| **Normalización** | Organizar las tablas para **no repetir datos**. | — |
| **ETL / ELT** | Extraer, transformar y cargar datos hacia un almacén. ELT carga primero y transforma después. | — |
| **Data warehouse / data lake / lakehouse** | Warehouse: almacén de datos ordenados para análisis. Lake: «lago» con datos en bruto. Lakehouse: mezcla de ambos. | — |
| **Pipeline de datos** | Cadena automática que **mueve y transforma** datos de un lugar a otro. | — |
| **Batch / streaming** | Batch: procesar **por lotes** (cada noche). Streaming: procesar **al instante** según llegan. | — |
| **OLTP / OLAP** | OLTP: operaciones del día a día (ventas). OLAP: análisis de grandes volúmenes (informes). | — |
| **CDC (captura de cambios)** | Detectar y enviar **cada cambio** que ocurre en una base de datos. | — |
| **Big data** | Volúmenes de datos tan grandes que no caben en herramientas corrientes. | — |
| **Copia de seguridad (backup) / RPO / RTO** | RPO: cuántos datos puedes permitirte perder. RTO: cuánto puedes tardar en recuperar. | — |
| **Base de datos vectorial** | Guarda **embeddings** para buscar por parecido de significado. | — |
| **Dato personal / RGPD** | Cualquier dato que identifique a una persona. El RGPD es la ley europea que regula su tratamiento. | — |

## 8. Seguridad

Vocabulario defensivo.

| Término | Qué es | Como si fuera… |
|---|---|---|
| **Autenticación / autorización** | Autenticación: **quién eres**. Autorización: **qué puedes hacer**. | El DNI frente a la llave de cada puerta. |
| **MFA / 2FA** | Verificar la identidad con **dos o más factores** (contraseña + código del móvil). | — |
| **SSO (inicio de sesión único)** | Entrar una vez y **acceder a varios sistemas**. | — |
| **OAuth 2 / OpenID Connect** | OAuth 2: permitir que una app acceda a tus datos **sin darle la contraseña**. OIDC: añade «quién eres» a OAuth. | Una llave de valet para el aparcacoches. |
| **JWT** | Token firmado con datos del usuario que el cliente envía en cada petición. | — |
| **Token** | Pieza de datos que **acredita** algo (sesión, permiso). También: fragmento de texto en IA. | — |
| **RBAC** | Permisos por **roles** (administrador, lector). | — |
| **Mínimo privilegio** | Dar a cada cual **solo los permisos que necesita**. | — |
| **Zero trust (confianza cero)** | No fiarse de nadie por estar «dentro» de la red: **todo se verifica siempre**. | — |
| **Cifrado en tránsito / en reposo** | En tránsito: mientras viaja (HTTPS). En reposo: mientras está guardado. | — |
| **Hash / salt** | Hash: huella irreversible de un dato. Salt: valor aleatorio que se añade para que dos contraseñas iguales den hashes distintos. | — |
| **Secreto / vault (bóveda)** | Secreto: contraseña o clave. Vault: sistema para guardarlos de forma segura. | — |
| **CVE / CVSS** | CVE: identificador público de una vulnerabilidad. CVSS: puntuación de gravedad (0–10). | — |
| **Vulnerabilidad / exploit / parche** | Vulnerabilidad: fallo aprovechable. Exploit: técnica que lo aprovecha. Parche: arreglo. | — |
| **SBOM** | Lista de **todos los componentes** de un software, como la lista de ingredientes. | — |
| **Cadena de suministro (supply chain) de software** | Riesgo de que lo que **usas de terceros** (bibliotecas, herramientas) esté comprometido. | — |
| **SAST / DAST / SCA** | SAST: analiza el código. DAST: ataca la aplicación en marcha. SCA: revisa las dependencias. | — |
| **OWASP Top 10** | Lista de los **diez riesgos más comunes** en aplicaciones web. | — |
| **Inyección SQL / XSS / CSRF** | Tres ataques clásicos: meter órdenes en consultas; meter scripts en páginas; hacer que tu navegador envíe peticiones sin que lo sepas. | — |
| **WAF / cortafuegos** | Filtros que bloquean tráfico malicioso (el WAF, el de aplicaciones web). | — |
| **Pentest (prueba de intrusión)** | Ataque **autorizado** para encontrar fallos antes que los malos. | — |
| **Red team / blue team** | Red: atacantes autorizados. Blue: defensores. | — |
| **Modelado de amenazas (threat modeling)** | Pensar **qué podría salir mal** y cómo atacarían antes de construir. | — |
| **Ingeniería social / phishing** | Engañar a personas (no a máquinas) para obtener datos o acceso. | — |
| **DDoS** | Ataque que **satura** un servicio con tráfico desde muchos orígenes. | — |
| **Principio de defensa en profundidad** | Varias capas de protección, no una sola. | — |

## 9. Web y frontend

Todo lo que se ve y se toca.

| Término | Qué es | Como si fuera… |
|---|---|---|
| **SPA / MPA** | SPA: una sola página que cambia sin recargar (React, Vue). MPA: varias páginas tradicionales. | — |
| **SSR / CSR / SSG** | SSR: la página se genera en el servidor en cada visita. CSR: la genera el navegador. SSG: se genera **de antemano** al construir el sitio. | — |
| **Hidratación (hydration)** | Hacer interactiva en el navegador una página que llegó ya dibujada del servidor. | — |
| **PWA** | Sitio web que se comporta como **aplicación instalable** y funciona sin conexión. | — |
| **Jamstack / sitio estático** | Sitios de archivos prefabricados servidos desde una CDN, con funciones para lo dinámico. | — |
| **Headless CMS** | Gestor de contenidos **sin la parte visual**; entrega los datos por API. | — |
| **Componente** | Pieza reutilizable de interfaz (botón, tarjeta). | Una pieza de Lego. |
| **Estado (state) / props** | Estado: datos que cambian dentro de un componente. Props: datos que recibe de fuera. | — |
| **Virtual DOM / reactividad** | Técnicas para **actualizar solo lo que cambió** en la pantalla. | — |
| **Hooks** | Funciones de React (`useState`, `useEffect`) para añadir estado y efectos a componentes. | — |
| **Bundler / transpilador** | Bundler: junta y optimiza archivos (Vite). Transpilador: traduce código moderno a otro compatible. | — |
| **Tree shaking** | Eliminar del paquete final el **código que no se usa**. | — |
| **Polyfill** | Código que añade una función moderna a navegadores antiguos. | — |
| **Web Components** | Estándar para crear componentes propios en el navegador. | — |
| **Micro-frontends** | Dividir la interfaz en partes que **equipos distintos** construyen y despliegan por separado. | — |
| **Design system (sistema de diseño)** | Biblioteca de **componentes, colores y reglas** comunes para toda la marca. | — |
| **Responsive / mobile first** | Responsive: se adapta a cualquier pantalla. Mobile first: se diseña primero para móvil. | — |
| **Accesibilidad (a11y)** | Que cualquier persona, también con discapacidades, **pueda usarlo**. | — |
| **i18n / l10n** | Internacionalización (preparar para varios idiomas) y localización (adaptar a uno). | — |
| **Core Web Vitals** | Métricas de Google sobre carga, interactividad y estabilidad visual. | — |
| **CORS** | Regla del navegador que **limita** qué sitios pueden pedir datos a tu API. | — |
| **Cookie / localStorage** | Pequeños datos guardados en el navegador. | — |
| **WebAssembly (WASM)** | Formato para ejecutar en el navegador **código compilado** casi a velocidad nativa. | — |
| **SEO** | Optimización para aparecer en buscadores. | — |
| **Frontend / backend / full-stack** | Frontend: lo que ve el usuario. Backend: lo que hay detrás. Full-stack: ambos. | — |

## 10. Inteligencia artificial en el desarrollo

Vocabulario de la IA que ya se usa para programar.

| Término | Qué es | Como si fuera… |
|---|---|---|
| **IA / aprendizaje automático (ML) / aprendizaje profundo** | IA: máquinas que hacen tareas que requieren inteligencia. ML: aprenden de **datos**. Profundo: lo hace con redes neuronales grandes. | — |
| **LLM (modelo de lenguaje grande)** | Modelo entrenado con enormes cantidades de texto que **predice y genera lenguaje** (los que están detrás de los asistentes de IA). | — |
| **Prompt** | La **instrucción** que le das a la IA. | — |
| **Ingeniería de prompts** | Escribir instrucciones claras y con contexto para obtener mejores respuestas. | — |
| **Token / ventana de contexto** | Token: trozo de texto que el modelo procesa. Ventana de contexto: **cuánto texto puede tener presente** a la vez. | — |
| **Alucinación** | La IA **inventa** algo falso con apariencia convincente (una función que no existe). | — |
| **RAG** | Dar a la IA **documentos de consulta** antes de responder para que se base en ellos. | Un examen con apuntes. |
| **Embedding** | Representación numérica del significado de un texto para **comparar parecidos**. | — |
| **Fine-tuning (ajuste fino)** | Entrenar un poco más un modelo con tus propios datos. | — |
| **Agente (agent)** | IA que **planifica y ejecuta acciones** usando herramientas, no solo responde. | — |
| **Uso de herramientas / MCP** | Que la IA llame a programas externos. MCP es un estándar para conectar la IA con herramientas y datos. | — |
| **Copiloto (copilot) / pair programming con IA** | Asistente que sugiere código mientras escribes. | — |
| **Vibe coding** | Programar **describiendo lo que quieres** y aceptando lo que genera la IA, sin revisar el código a fondo. Rápido, pero arriesgado si no entiendes lo que sale. | — |
| **Evals (evaluaciones)** | Pruebas que miden **qué tan bien** responde un sistema de IA. | — |
| **Inyección de prompt (prompt injection)** | Ataque que **esconde órdenes** en un texto para que la IA las obedezca. | — |
| **Inferencia** | El momento de **usar** un modelo ya entrenado. | — |
| **Guardrails (barreras)** | Reglas que limitan lo que la IA puede hacer o decir. | — |
| **Multimodal** | IA que maneja varios tipos de dato: texto, imagen, audio. | — |
| **Human in the loop** | Una persona **revisa o aprueba** lo que decide o genera la IA. | — |

## 11. Equipo, negocio y cultura

Roles y expresiones del día a día en una empresa de software.

| Término | Qué es | Como si fuera… |
|---|---|---|
| **Stakeholder (parte interesada)** | Cualquiera a quien **le afecta o le importa** el proyecto. | — |
| **PM / Product Manager / Project Manager** | Producto: decide qué construir y por qué. Proyecto: gestiona plazos y recursos. | — |
| **Tech lead / arquitecto / staff engineer** | Perfiles técnicos senior que guían decisiones. | — |
| **Junior / mid / senior** | Niveles de experiencia. | — |
| **Pair programming / mob programming** | Dos personas (o todo el equipo) programando **juntas** en un mismo ordenador. | — |
| **Rubber duck debugging** | Explicar el problema en voz alta a un patito de goma para encontrar el fallo. | — |
| **Onboarding** | Proceso de **incorporar** a una persona nueva al equipo. | — |
| **Dogfooding** | Usar el equipo **su propio producto**. | — |
| **Bikeshedding** | Discutir sin fin sobre algo trivial mientras lo importante se ignora. | — |
| **Yak shaving** | Una cadena de tareas secundarias hasta olvidar la tarea original. | — |
| **Remoto / híbrido / asíncrono** | Trabajo desde casa, mezcla con oficina, o sin necesidad de coincidir en el tiempo. | — |
| **OKR / KPI** | Objetivos y resultados clave / indicadores de rendimiento. | — |
| **Build vs buy** | Decidir entre **construir** algo o **comprarlo**. | — |
| **NIH (Not Invented Here)** | Rechazar soluciones externas solo por no haberlas inventado uno mismo. | — |
| **Código abierto (open source) / licencias (MIT, Apache, GPL)** | Código con licencia que permite verlo, usarlo y a veces modificarlo; cada licencia pone condiciones. | — |
| **Inner source** | Aplicar prácticas de código abierto **dentro** de una empresa. | — |
| **SaaS / B2B / B2C** | Software como servicio; empresas que venden a empresas / a consumidores. | — |
| **TCO / ROI** | TCO: coste total de tener algo. ROI: retorno de la inversión. | — |
| **Product-market fit / pivotar** | Que el producto encaje con el mercado / cambiar de rumbo. | — |
| **Sunset (retirada)** | Retirar un producto de forma planificada. | — |
| **Bleeding edge / cutting edge** | Lo más nuevo y arriesgado / lo más avanzado. | — |
| **Silver bullet (bala de plata)** | La idea de una solución mágica para todo. No existe. | — |
| **Works on my machine («en mi máquina funciona»)** | Frase clásica ante un fallo que solo ocurre en otro entorno. Origen de los contenedores. | — |
| **Technical writer** | Persona que escribe la documentación técnica. | — |

