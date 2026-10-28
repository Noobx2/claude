# Glosario y referencias

Terminología técnica en orden alfabético, las **diferencias de vocabulario entre lenguajes** y los enlaces a la documentación oficial. Los términos que ya se explican a fondo en un módulo remiten a él.

*Verificado: 2026-10-27.*

## 1. Mismo concepto, distinto nombre

Cuando cambias de lenguaje, lo difícil no son las ideas sino los nombres.

| Concepto | JavaScript / TS | Python | Java | C# | Go | Rust | Dart |
|---|---|---|---|---|---|---|---|
| Función de una clase | método | método | método | método | método (receptor) | método (`impl`) | método |
| Función suelta | función | función | método `static` | método `static` / local | función | función | función |
| Lista dinámica | `Array` | `list` | `ArrayList` / `List` | `List<T>` | slice (`[]T`) | `Vec<T>` | `List<T>` |
| Diccionario | `Map` / objeto | `dict` | `HashMap` / `Map` | `Dictionary` | `map` | `HashMap` | `Map` |
| Conjunto | `Set` | `set` | `HashSet` / `Set` | `HashSet` | `map[T]struct{}` | `HashSet` | `Set` |
| Ausencia de valor | `null`, `undefined` | `None` | `null` | `null` | `nil` | `Option::None` | `null` |
| Contrato de comportamiento | `interface` (TS) | `Protocol` / clase abstracta | `interface` | `interface` | `interface` (implícita) | `trait` | `abstract class` / `interface` |
| Agrupar código | módulo (archivo) | módulo / paquete | paquete | espacio de nombres (`namespace`) | paquete | crate / módulo | librería / paquete |
| Gestor de paquetes | npm, pnpm | uv, pip | Maven, Gradle | NuGet (`dotnet`) | `go mod` | Cargo | pub |
| Manifiesto | `package.json` | `pyproject.toml` | `pom.xml` | `.csproj` | `go.mod` | `Cargo.toml` | `pubspec.yaml` |
| Repositorio de paquetes | npm registry | PyPI | Maven Central | nuget.org | proxy de módulos (pkg.go.dev) | crates.io | pub.dev |
| Fallo | excepción | excepción | excepción | excepción | **valor** `error` | **valor** `Result` | excepción |
| Concurrencia ligera | `Promise` / `async` | `asyncio` | hilos virtuales / `CompletableFuture` | `Task` / `async` | goroutine | `async` / hilos | `Future` / `Isolate` |
| Genéricos | `<T>` (TS) | tipado opcional | `<T>` | `<T>` | `[T any]` | `<T>` | `<T>` |
| Construcción inmutable | `const`, `readonly` | `dataclass(frozen=True)` | `record`, `final` | `record`, `readonly` | (por convención) | por defecto | `final`, `const` |

## 2. Glosario

### A

- **ACID**: Atomicidad, Consistencia, Aislamiento y Durabilidad; garantías de una transacción ([SQL](#sql)).
- **AES-GCM**: cifrado simétrico autenticado ([Criptografía](#cripto)).
- **API**: interfaz por la que un programa ofrece funciones a otro. **API REST**: API sobre HTTP con recursos y verbos ([Node.js](#node)).
- **Argon2**: función de hash lenta para contraseñas ([Criptografía](#cripto)).
- **Async/await**: sintaxis para escribir código asíncrono como si fuera secuencial ([JavaScript](#js)).
- **Autenticación**: comprobar *quién eres*. **Autorización**: decidir *qué puedes hacer*.

### B

- **Backend / frontend**: la parte del servidor / la parte que ve el usuario.
- **Baudios**: velocidad de un puerto serie ([Hardware](#hardware)).
- **Bean**: objeto gestionado por el contenedor de Spring ([Spring Boot](#spring)).
- **Binario (archivo)**: datos en bytes con un formato propio, no texto ([CLI y binarios](#cli-args-streams)).
- **Bytecode**: código intermedio que ejecuta una máquina virtual (JVM, .NET).

### C

- **Cache**: copia temporal para acelerar accesos.
- **CIDR**: notación de redes (`192.168.1.0/24`) ([Redes](#redes)).
- **CLI**: interfaz de línea de comandos.
- **Clausura (*closure*)**: función que recuerda las variables de donde nació ([JavaScript](#js)).
- **Commit**: foto del proyecto en Git ([Git](#git)).
- **Componente**: pieza reutilizable de interfaz ([React](#react), [Vue](#vue)).
- **Compose (Docker)**: archivo que define varios servicios ([Docker](#docker)).
- **Contenedor**: proceso aislado con todo lo que necesita ([Docker](#docker)).
- **Controlador (*controller*)**: capa que recibe peticiones HTTP y delega.
- **CORS**: mecanismo por el que el navegador limita peticiones entre orígenes ([Node.js](#node)).
- **CRUD**: Create, Read, Update, Delete ([Receta 5](#recetas)).
- **CSRF / XSS / SSRF**: clases de ataque web ([Laboratorio](#lab-etico)).

### D

- **Daemon / demonio**: proceso en segundo plano (servicio).
- **DDL / DML**: lenguaje de definición / de manipulación de datos en SQL.
- **Dependencia**: librería de la que depende tu proyecto; **transitiva** si la arrastra otra ([Configuración](#config)).
- **DNS**: traducción de nombres a direcciones IP ([Redes](#redes)).
- **Docker**: ver *contenedor*.
- **DTO**: objeto que describe los datos que viajan por la API ([Spring Boot](#spring)).

### E

- **Endpoint**: ruta concreta de una API (`GET /api/tasks`).
- **Entidad**: objeto del dominio con identidad (`Task`).
- **Entorno virtual**: carpeta con un intérprete y paquetes propios del proyecto ([Python](#python)).
- **Evento**: algo que ocurre y se escucha (un clic, una petición).
- **Excepción**: mecanismo de error que «salta» hasta un `catch`.

### F

- **Firma digital**: prueba criptográfica de autoría e integridad ([Criptografía](#cripto)).
- **Framework**: estructura que impone cómo organizar tu código (React, Spring…); una **librería** la llamas tú, un framework te llama a ti.
- **Futuro / promesa (*Future*, *Promise*)**: valor que llegará más tarde.

### G

- **Genéricos**: tipos parametrizados (`List<T>`).
- **Gestor de versiones**: instala varias versiones de un lenguaje (NVM, SDKMAN, mise, FVM) ([Gestores de versiones](#entorno-versiones)).
- **Git**: sistema de control de versiones ([Git](#git)).
- **Goroutine**: hilo ligero de Go ([Go](#go)).

### H

- **Hash**: huella de tamaño fijo de un dato ([Criptografía](#cripto)).
- **HMAC**: hash con clave secreta.
- **Honeypot**: servicio señuelo que registra visitantes ([Honeypots](#honeypots)).
- **Hook**: función especial de React (`useState`) o gancho en general.
- **HTTP / HTTPS**: protocolo de la web / con cifrado TLS ([Redes](#redes)).

### I

- **IDOR**: acceder a recursos de otros cambiando un identificador ([Laboratorio](#lab-etico)).
- **Idempotente**: repetir la operación da el mismo resultado (`PUT`, `DELETE`).
- **Imagen (Docker)**: plantilla de solo lectura de un contenedor.
- **Índice (base de datos)**: estructura que acelera búsquedas ([SQL](#sql)).
- **Inyección de dependencias**: recibir lo que necesitas en lugar de crearlo ([Spring Boot](#spring)).
- **Inyección SQL**: fallo por concatenar entrada en una consulta ([Laboratorio](#lab-etico)).
- **IP**: dirección de un equipo en la red.

### J

- **JDK / JRE / JVM**: kit de desarrollo de Java / entorno de ejecución / máquina virtual ([Java](#java)).
- **JSON**: formato de texto para datos estructurados ([JavaScript](#js)).
- **JWT**: token firmado con datos ([Criptografía](#cripto)).

### L

- **Linter**: analizador estático que detecta errores y malas prácticas (Oxlint, Ruff, Clippy).
- **Lockfile**: archivo de bloqueo con versiones exactas ([Configuración](#config)).
- **Log**: registro de eventos.

### M

- **Manifiesto**: archivo que declara el proyecto y sus dependencias ([Configuración](#config)).
- **Middleware**: función que se intercala en el procesamiento de una petición.
- **Migración**: cambio versionado del esquema de datos ([SQL](#sql)).
- **Módulo**: unidad de código importable.
- **Monorepo**: un repositorio con varios proyectos.

### N

- **NAT**: traducción de direcciones en un router ([Redes](#redes)).
- **Nonce**: valor que no debe repetirse con la misma clave ([Criptografía](#cripto)).
- **Null safety**: el sistema de tipos distingue valores que pueden ser nulos ([Dart](#flutter), [Kotlin](#android)).

### O

- **ORM / ODM**: mapeo entre objetos y tablas / documentos.
- **OWASP**: organización de referencia en seguridad de aplicaciones ([Laboratorio](#lab-etico)).

### P

- **Paginación**: dividir resultados en páginas ([Receta 8](#recetas)).
- **PATH**: lista de carpetas donde se buscan ejecutables ([PATH](#entorno-path)).
- **Petición / respuesta**: mensajes de HTTP ([Node.js](#node)).
- **pty**: pseudo-terminal, un «cable serie» virtual ([Hardware](#hardware)).
- **Pull Request**: propuesta de fusionar una rama ([Git](#git)).
- **Puerto**: número que identifica un servicio dentro de un equipo ([Redes](#redes)).

### R

- **React / Vue**: bibliotecas de interfaces con componentes y estado ([React](#react), [Vue](#vue)).
- **Reactividad**: la interfaz se actualiza sola al cambiar los datos.
- **Rebase / merge**: dos formas de integrar ramas en Git.
- **Repositorio (patrón)**: capa que accede a los datos. **Repositorio (Git)**: carpeta con historial.
- **REST**: estilo de API con recursos y verbos HTTP.
- **Runtime**: entorno que ejecuta un programa (Node, JVM, .NET).

### S

- **Sal (*salt*)**: valor aleatorio que se mezcla con una contraseña antes del hash.
- **SDK**: kit de desarrollo (compilador, bibliotecas, herramientas).
- **Servicio (capa)**: reglas de negocio. **Servicio (sistema)**: proceso gestionado por systemd.
- **Shim**: ejecutable falso que redirige a la versión real (mise) ([PATH](#entorno-path)).
- **Socket**: extremo de una comunicación de red.
- **SQL**: lenguaje de bases relacionales ([SQL](#sql)).
- **Stack trace**: traza de llamadas al producirse un error.
- **Stream**: flujo de datos procesado por trozos ([CLI y streams](#cli-args-streams)).

### T

- **TCP / UDP**: protocolos de transporte ([Redes](#redes)).
- **Tipado estático / dinámico**: tipos comprobados al compilar / al ejecutar.
- **TLS**: cifrado y autenticación de conexiones ([Redes](#redes)).
- **Token**: cadena que representa una identidad o permiso.
- **Transacción**: grupo de operaciones «todo o nada» ([SQL](#sql)).
- **Trait**: contrato de comportamiento en Rust ([Rust](#rust)).

### U–Z

- **udev**: gestor de dispositivos de Linux ([Hardware](#hardware)).
- **Variable de entorno**: pareja `NOMBRE=valor` heredada por los procesos ([PATH](#entorno-path)).
- **Vite**: herramienta de desarrollo y empaquetado del frontend ([React](#react)).
- **Volumen (Docker)**: almacenamiento que sobrevive al contenedor ([Docker](#docker)).
- **VPN**: túnel cifrado entre dispositivos ([VPN](#vpn)).
- **Widget**: elemento de interfaz en Flutter ([Flutter](#flutter)).
- **YAML / TOML**: formatos de configuración ([Configuración](#config)).

## 3. Referencias oficiales

### Lenguajes

| Tecnología | Documentación |
|---|---|
| JavaScript | [MDN: JavaScript](https://developer.mozilla.org/es/docs/Web/JavaScript) |
| TypeScript | [typescriptlang.org/docs](https://www.typescriptlang.org/docs/) |
| Python | [docs.python.org](https://docs.python.org/es/3/) |
| Java | [dev.java](https://dev.java/learn/) |
| C# | [learn.microsoft.com/dotnet/csharp](https://learn.microsoft.com/es-es/dotnet/csharp/) |
| Go | [go.dev/doc](https://go.dev/doc/) |
| Rust | [doc.rust-lang.org/book](https://doc.rust-lang.org/book/) |
| Dart | [dart.dev/language](https://dart.dev/language) |
| Kotlin | [kotlinlang.org/docs](https://kotlinlang.org/docs/home.html) |
| SQL (PostgreSQL) | [postgresql.org/docs](https://www.postgresql.org/docs/current/) |

### Web y frontend

| Tema | Documentación |
|---|---|
| HTML, CSS, APIs del navegador | [MDN Web Docs](https://developer.mozilla.org/es/) |
| Compatibilidad | [Baseline en web.dev](https://web.dev/baseline) |
| Accesibilidad | [WCAG](https://www.w3.org/WAI/standards-guidelines/wcag/) |
| React | [react.dev](https://react.dev/) |
| Vue | [vuejs.org](https://vuejs.org/) |
| Vite | [vite.dev](https://vite.dev/) |
| Flutter | [docs.flutter.dev](https://docs.flutter.dev/) |
| Android | [developer.android.com](https://developer.android.com/) |

### Backend, datos y operaciones

| Tema | Documentación |
|---|---|
| Node.js | [nodejs.org/docs](https://nodejs.org/docs/latest/api/) |
| Fastify | [fastify.dev](https://fastify.dev/docs/latest/) |
| Spring Boot | [docs.spring.io/spring-boot](https://docs.spring.io/spring-boot/reference/) |
| ASP.NET Core | [learn.microsoft.com/aspnet/core](https://learn.microsoft.com/es-es/aspnet/core/) |
| FastAPI | [fastapi.tiangolo.com](https://fastapi.tiangolo.com/) |
| MongoDB | [mongodb.com/docs](https://www.mongodb.com/docs/) |
| Docker | [docs.docker.com](https://docs.docker.com/) |
| Git | [git-scm.com/book](https://git-scm.com/book/es/v2) |
| Bash | [gnu.org/software/bash/manual](https://www.gnu.org/software/bash/manual/bash.html) |
| Linux (comandos) | [man7.org/linux/man-pages](https://man7.org/linux/man-pages/) |

### Seguridad y redes

| Tema | Documentación |
|---|---|
| Seguridad web | [OWASP](https://owasp.org/) · [Cheat Sheets](https://cheatsheetseries.owasp.org/) |
| Práctica legal | [PortSwigger Web Security Academy](https://portswigger.net/web-security) |
| WireGuard | [wireguard.com](https://www.wireguard.com/) |
| Wireshark / tcpdump | [wireshark.org/docs](https://www.wireshark.org/docs/) · [tcpdump.org](https://www.tcpdump.org/) |
| Cryptography (Python) | [cryptography.io](https://cryptography.io/) |
| Ubuntu Server | [documentation.ubuntu.com/server](https://documentation.ubuntu.com/server/) |
| CERT en España | [INCIBE-CERT](https://www.incibe.es/incibe-cert) |

### Versiones de tu stack (comprobar de vez en cuando)

| Herramienta | Dónde ver la versión actual |
|---|---|
| Node.js (LTS) | [nodejs.org/en/about/previous-releases](https://nodejs.org/en/about/previous-releases) |
| Java | [adoptium.net](https://adoptium.net/) |
| .NET | [dotnet.microsoft.com/download](https://dotnet.microsoft.com/download) |
| Go | [go.dev/dl](https://go.dev/dl/) |
| Rust | [rust-lang.org/tools/install](https://www.rust-lang.org/tools/install) |
| Flutter / Dart | [docs.flutter.dev/release](https://docs.flutter.dev/release/archive) |
| TypeScript | [github.com/microsoft/TypeScript/releases](https://github.com/microsoft/TypeScript/releases) |

> **Regla:** cada vez que un comando o una versión de esta biblioteca no coincida con lo que ves, **gana la documentación oficial**. Anota la discrepancia: la biblioteca debe actualizarse.
