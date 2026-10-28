# Inventario del entorno

Fuentes: tu primer informe, el segundo informe de verificación y tu tabla de tecnologías. Última actualización: **2026-10-27**. No he ejecutado nada en tu máquina; todo sale de lo que pegaste.

> **Regla:** *instalado* no es *configurado* ni *funcionando*. Cada fila indica la evidencia que la sostiene.

## 1. Resumen de la verificación

- **Tu equipo es una máquina virtual VirtualBox** (servicio `virtualbox-guest-utils`, dispositivo «VirtualBox USB Tablet») con escritorio `lightdm`. Trabajas por SSH / VS Code Remote. Esto condiciona el módulo de hardware.
- **Confirmados ahora**: Git 2.53.0, Node v24.22.1, npm 11.19.1, pnpm 12.15.0, Java 25.0.4.1, Maven 3.10.0, Rust y Cargo 1.99.1, FVM 4.3.2.
- **No están en el PATH**: `tsc` y `oxlint` (normal: viven dentro de cada proyecto), `dart` y `flutter` (se llaman a través de FVM), `adb`, `dotnet`, `kotlinc`, `mongosh`, `psql`.
- **Go solo funciona dentro de Docker.** Hay un laboratorio previo `go-flutter-lab` con Go y PostgreSQL en contenedores; en el equipo, `go` sigue sin versión activa.
- **Git no está configurado**: no existe `~/.gitconfig`, así que faltan nombre y correo.
- **Tu usuario no está en el grupo `dialout`**, necesario para puertos serie.

## 2. Sistema

| Elemento | Dato | Estado |
|---|---|---|
| Sistema operativo | Ubuntu 26.04 (`resolute`), amd64, kernel 7.0.0-38 | <span class="tag ok">comprobado</span> |
| Tipo de equipo | Máquina virtual VirtualBox con escritorio | <span class="tag ok">comprobado</span> |
| Shell | `/bin/bash` | <span class="tag ok">comprobado</span> |
| Idioma / zona horaria | `es_ES`, UTC | <span class="tag ok">comprobado</span> |
| Git | 2.53.0, sin configuración global | <span class="tag ok">comprobado</span> |
| Editor | VS Code con Remote SSH | <span class="tag warn">declarado</span> |
| Equipo anfitrión | Visual Studio con C# instalado; podrías instalar Node, Vite y MongoDB en local | <span class="tag warn">declarado</span> (no verificado desde aquí) |
| Grupos del usuario | `adm cdrom sudo dip plugdev users lxd docker` | <span class="tag ok">comprobado</span> |

## 3. Lenguajes, compiladores y runtimes

| Lenguaje | Versión | Estado | Observación |
|---|---|---|---|
| Node.js | v24.22.1 | <span class="tag ok">comprobado</span> | Gestionado por NVM |
| Python | 3.14.4 | <span class="tag ok">comprobado</span> | `python` no existe; `pip` del sistema ausente; hay `uv` |
| Java | OpenJDK 25.0.4.1 LTS (Temurin) | <span class="tag ok">comprobado</span> | `javac` 25.0.4.1; SDKMAN |
| Rust | 1.99.1 (`rustc`, `cargo`) | <span class="tag ok">comprobado</span> | rustup, toolchain `stable` |
| C / C++ | gcc/g++ 15.2.0, clang 21.1.8, cmake 4.2.3, make 4.4.1 | <span class="tag ok">comprobado</span> | Del primer informe |
| TypeScript | 6.0.3 declarado | <span class="tag warn">declarado</span> | No está global. Las plantillas de Vite actuales instalan 7.0.2 (React, JS puro) o 6.0.3 (Vue); comprueba con `pnpm exec tsc -v` en cada proyecto |
| Dart | 3.13.7 declarado | <span class="tag warn">declarado</span> | No está en el PATH; vendrá con Flutter vía FVM |
| Go | — | <span class="tag bad">no disponible</span> | Shim de `mise` sin versión activa. Existe un contenedor de laboratorio con Go |
| C# / .NET | — | <span class="tag bad">no disponible</span> | `dotnet` no encontrado |
| Kotlin | — | <span class="tag bad">no disponible</span> | `kotlinc` no encontrado |

## 4. Gestores y paquetes

| Herramienta | Versión | Estado |
|---|---|---|
| NVM | 0.40.8 | <span class="tag ok">comprobado</span> (cargado desde `.bashrc`) |
| SDKMAN | — | <span class="tag ok">comprobado</span> (cargado desde `.bashrc`) |
| mise | 2026.10.19 | <span class="tag ok">comprobado</span> (activado en `.bashrc`) |
| rustup / Cargo | — / 1.99.1 | <span class="tag ok">comprobado</span> |
| uv | 0.13.5 | <span class="tag ok">comprobado</span> |
| FVM | 4.3.2 | <span class="tag ok">comprobado</span> (`~/fvm/bin`) |
| npm | 11.19.1 | <span class="tag ok">comprobado</span> |
| pnpm | 12.15.0 | <span class="tag ok">comprobado</span> |

## 5. Frontend y móvil

| Tecnología | Estado | Observación |
|---|---|---|
| React 19.3.1, Vite 8.3.5, Oxlint | <span class="tag warn">declarado</span> | Hay un proyecto en `~/projects/frontend-react`; sus versiones están en su `package.json` |
| Flutter 3.47.8 | <span class="tag warn">declarado</span> | `flutter` no está en el PATH; verifica con `fvm list` |
| Android SDK 36 | <span class="tag warn">declarado</span> | `adb` no está en el PATH, pero el puerto 5037 en escucha es el del servidor de `adb` por defecto (hipótesis) |
| Vue | <span class="tag mute">sin instalar</span> | Se crea por proyecto con `npm create vue@latest` |
| Backends (Spring, Express, FastAPI…) | <span class="tag mute">sin datos</span> | Solo hay lenguajes y el laboratorio de Docker |
| Electron, Tauri, Gradle | <span class="tag bad">no disponibles</span> | Fuera de prioridad |

## 6. Docker y datos

| Elemento | Dato | Estado |
|---|---|---|
| Docker Engine / Compose | 29.9.1 / v5.6.0; servicio `docker` activo | <span class="tag ok">funcionando</span> |
| Contenedores | `go-flutter-lab-postgres-1` (`postgres:18`) y `go-flutter-lab-backend-1`, ambos **detenidos** | <span class="tag ok">comprobado</span> |
| Imágenes | `go-flutter-lab-backend` (1.24 GB), `postgres:18`, `hello-world` | <span class="tag ok">comprobado</span> |
| Volúmenes | `go-flutter-lab_go-build-cache`, `go-flutter-lab_go-mod-cache`, `go-flutter-lab_postgres-data` | <span class="tag ok">comprobado</span> |
| `psql`, `mongosh` | No encontrados en el PATH | <span class="tag bad">no disponible</span> |
| Redis | No instalado | <span class="tag bad">no disponible</span> |

> **Ojo:** pertenecer al grupo `docker` equivale en la práctica a tener permisos de administrador. Es habitual en desarrollo, pero no lo repliques en un servidor compartido.

## 7. Servicios y puertos

| Puerto | Dirección | Probable origen |
|---|---|---|
| 22 | Todas las interfaces | SSH (`ssh.service`) |
| 53 | Solo local | Resolución de nombres (`systemd-resolved`) |
| 5037 | Solo local | Servidor de `adb` (puerto por defecto) |
| 36267, 36379, 37113 | Solo local | Sin identificar (probablemente herramientas de VS Code); haría falta `sudo ss -ltnp` |

Servicios relevantes en ejecución: `docker`, `containerd`, `ssh`, `lightdm`, `snapd`, `cron`, `udisks2`, `ModemManager`, `virtualbox-guest-utils`. **No hay servidor web, base de datos ni servicio de aplicación escuchando**, y ningún puerto de desarrollo (3000, 5173, 5432…) está ocupado.

## 8. Hardware

| Elemento | Dato | Estado |
|---|---|---|
| Dispositivos USB visibles | Solo concentradores raíz y «VirtualBox USB Tablet» | <span class="tag ok">comprobado</span> |
| Puertos serie USB | Ninguno | <span class="tag ok">comprobado</span> |
| `python3-serial` (pyserial) | 3.5 | <span class="tag ok">comprobado</span> |
| Grupo `dialout` | No pertenece | <span class="tag bad">falta</span> |
| `ModemManager` activo | Puede intentar abrir dispositivos serie recién conectados | <span class="tag warn">a tener en cuenta</span> |

Al ser una máquina virtual, los dispositivos físicos del ordenador anfitrión **no son visibles** salvo que los reenvíes a la VM desde VirtualBox (USB passthrough). El módulo de hardware parte de esto.

## 9. Avisos y discrepancias

- **`PATH` con duplicados.** `~/.local/bin` aparece dos veces. Es inofensivo, pero indica que dos archivos de perfil lo añaden.
- **`~/.cargo/env` se carga dos veces** (`.bashrc` línea 130 y `.profile` línea 30). También inofensivo.
- **Go.** `mise use -g go@<versión>` lo activaría en el sistema. Alternativa ya preparada: el contenedor `go-flutter-lab-backend`.
- **Git sin configurar.** Antes de hacer tu primer `commit` hay que fijar `user.name` y `user.email` (se explicará en el módulo de Git).
- **`python3` con dos versiones de paquete.** El paquete `python3` (3.14.3) es un metapaquete; el intérprete que ejecutas es 3.14.4.

## 10. Lo que falta o conviene instalar (no se ha instalado nada)

| Para qué | Qué falta | Cuándo |
|---|---|---|
| Go en el equipo | Activar una versión con `mise` | Antes de M16 |
| C# | SDK de .NET | Antes de M15 |
| MongoDB | Una imagen de Docker (no hace falta nada en el sistema) | M8–M9 |
| Android nativo | Kotlin y herramientas de Android en el PATH | Antes de M19 |
| Hardware | Añadir tu usuario a `dialout`; `usbutils` ya muestra `lsusb`, así que está disponible | Antes de M20 |
