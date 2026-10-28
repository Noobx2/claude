# Ruta de aprendizaje

> **Estado:** todos los módulos están escritos. Esta página queda como mapa y orden recomendado.

Orden pensado para tus objetivos: apps rápidas con JS/TS, varios lenguajes de propósito general, y acceso a las APIs del dispositivo y del hardware. Cada fila es un módulo; el orden respeta las dependencias.

## Principios del orden

1. **Terminal y entorno primero.** Todo lo demás depende de ellos.
2. **Web antes que frameworks.** HTML, CSS y JS sin librerías, para entender lo que React y Vue automatizan.
3. **TypeScript justo después de JS.** El stack de apps rápidas lo usa en todas partes.
4. **Docker antes de las bases de datos.** MongoDB y PostgreSQL se ejecutan en contenedor.
5. **Un lenguaje a la vez**, con la misma plantilla de ficha para poder comparar.

## Mapa de módulos

| Código | Módulo | Meta concreta | Estado |
|---|---|---|---|
| M0 | Inventario, ruta y normas | Saber qué hay y qué falta | <span class="tag ok">listo</span> |
| M1 | Entorno, terminal, comandos flag a flag, Bash, CLI y binarios | Moverte por la terminal, entender PATH y gestores de versiones | <span class="tag ok">listo</span> |
| M1+ | Scripts de Bash; CLI, streams y binarios | Programas de consola con argumentos, tuberías y archivos binarios | <span class="tag ok">listo</span> |
| EJ | Ejercicios de lógica y comparativa de lenguajes | 80 ejercicios clásicos y equivalencias entre 8 lenguajes | <span class="tag ok">listo</span> |
| M2 | HTML5 moderno | Página semántica con formulario validado | <span class="tag ok">listo</span> |
| M3 | CSS moderno | Maquetación adaptable en modo claro y oscuro | <span class="tag ok">listo</span> |
| M4 | JavaScript moderno | Lista de tareas con JS puro | <span class="tag ok">listo</span> |
| M5 | TypeScript | La misma app con tipos, usando Vite | <span class="tag ok">listo</span> |
| M6 | Git y GitHub | Historial, ramas, `.gitignore` | <span class="tag ok">listo</span> |
| M7 | Node.js | API HTTP mínima | <span class="tag ok">listo</span> |
| M8 | Docker y Compose | Levantar servicios con un comando | <span class="tag ok">listo</span> |
| M9 | MongoDB | CRUD contra una base real | <span class="tag ok">listo</span> |
| M10 | React + Vite | Interfaz que consume la API | <span class="tag ok">listo</span> |
| M11 | Vue + Vite | La misma interfaz en Vue | <span class="tag ok">listo</span> |
| M12 | Proyecto lista de tareas | Extremo a extremo: Node + Mongo + React/Vue | <span class="tag ok">listo</span> |
| M13 | Python (uv) | Script con dependencias y pruebas | <span class="tag ok">listo</span> |
| M14 | Java (Maven) | Proyecto Maven con pruebas | <span class="tag ok">listo</span> |
| M15 | C# (.NET) | App de consola con pruebas | <span class="tag ok">listo</span> |
| M16 | Go | Programa y servidor HTTP mínimo | <span class="tag ok">listo</span> |
| M17 | Rust (Cargo) | Binario con propiedad y errores bien usados | <span class="tag ok">listo</span> |
| M18 | Dart y Flutter | App móvil con una API del dispositivo | <span class="tag ok">listo</span> |
| M19 | Android nativo | App Kotlin con permisos y sensores | <span class="tag ok">listo</span> |
| M20 | Hardware del PC | Hablar con un dispositivo por puerto serie o USB | <span class="tag ok">listo</span> |
| M21 | SQL y PostgreSQL | Consultas, joins, migraciones | <span class="tag ok">listo</span> |
| M22 | Spring Boot | API REST por capas | <span class="tag ok">listo</span> |
| M23 | Recetario | 16 recetas con plantilla fija | <span class="tag ok">listo</span> |
| M24 | Manifiestos y configuración | Una ficha por archivo | <span class="tag ok">listo</span> |
| M25 | Glosario y referencias | Equivalencias y enlaces oficiales | <span class="tag ok">listo</span> |
| S0 | Seguridad: marco ético y plan | Reglas, límites y ruta de seguridad | <span class="tag ok">listo</span> |
| S1 | Redes y protocolos | Leer y diagnosticar tu tráfico | <span class="tag ok">listo</span> |
| S2 | Criptografía aplicada | Hash, HMAC, AES-GCM, firmas, contraseñas | <span class="tag ok">listo</span> |
| S3 | Monitorización de red | `tcpdump`, líneas base y vigilancia | <span class="tag ok">listo</span> |
| S4 | VPN propia con WireGuard | Túnel entre tu PC y tu VM | <span class="tag ok">listo</span> |
| S5 | Endurecimiento y detección | SSH con claves, cortafuegos, `fail2ban`, registros | <span class="tag ok">listo</span> |
| S6 | Laboratorio de hacking ético | Fallos clásicos y su corrección en un entorno aislado | <span class="tag ok">listo</span> |
| S7 | Honeypots | Señuelos para detectar intrusos | <span class="tag ok">listo</span> |

Las recetas, las fichas de configuración y el glosario **crecen con cada módulo**: cuando un lenguaje introduce `Cargo.toml` o `pom.xml`, su ficha se escribe entonces; M23–M25 los consolidan.

## Decisiones tomadas

| Tema | Decisión |
|---|---|
| Dónde se trabaja | Anfitrión (Windows con Visual Studio y C#; Node, Vite y MongoDB posibles en local) y VM Ubuntu como servidor y base de datos |
| Seguridad | Defensiva, con laboratorio aislado propio; sin contraataques (ver S0) |
| Formato | Una única página de consulta (esta), sin repositorio de archivos |
| Stack de apps rápidas | JS puro + TS, Vite con React y con Vue, Node, MongoDB |
| Lenguajes | JS, TS, Python, Java, C#, Go, Rust, Dart |
| Java | Maven y Spring Boot, de básico a avanzado |
| Fuera de prioridad | Electron, Tauri. Gradle solo lo necesario para leer proyectos Android |
| Dispositivos | Flutter/Dart y Android nativo para las APIs del móvil; hardware del propio ordenador en M20 (tu equipo es una VM de VirtualBox: ver inventario) |

## Cómo se decide entre Flutter y Android nativo

| | Flutter / Dart | Android nativo (Kotlin) |
|---|---|---|
| Una base de código para varias plataformas | Sí | No |
| Acceso a APIs del dispositivo | A través de plugins y *platform channels* | Directo, sin intermediarios |
| Cuándo | Aplicaciones normales con cámara, sensores, ubicación | Cuando un plugin no existe o falla, o necesitas una API muy reciente o específica de Android |
| Ya lo tienes | Flutter y FVM declarados | Android SDK declarado; Kotlin sin datos |

Se empieza por Flutter (M18). Android nativo (M19) se estudia con el mismo ejemplo para comparar.

## Tres vías para el hardware (M20)

- **Desde el sistema operativo** (Python, Rust, Go, C): archivos de dispositivo en `/dev`, permisos por grupos, bibliotecas de puerto serie y USB.
- **Desde el navegador** (Web Serial, WebUSB, WebHID): útil para apps rápidas, limitado a navegadores basados en Chromium.
- **Microcontroladores** (Arduino, ESP32): fuera del alcance por ahora; el PC se comunica con ellos por puerto serie.

Tu equipo Linux es una **máquina virtual**: el hardware físico del PC no es visible para ella salvo que lo reenvíes (USB passthrough) o programes en el anfitrión (por ejemplo, C# con Visual Studio). [Hardware](#hardware) explica las opciones.
