# Gestores de versiones

Un gestor de versiones instala varias versiones de un lenguaje y elige cuál está activa. Evita tocar el sistema y permite que cada proyecto use la suya.

> **Regla:** un gestor por lenguaje. No mezcles, por ejemplo, Node de `apt` con Node de NVM: el `PATH` decidirá cuál ganas sin que lo notes.

## Quién gestiona qué en tu equipo

| Lenguaje | Gestor | Estado | Dónde vive |
|---|---|---|---|
| Node.js | NVM | <span class="tag ok">comprobado</span> | `~/.nvm` |
| Java, Maven | SDKMAN | <span class="tag ok">comprobado</span> | `~/.sdkman` |
| Go (y otros) | mise | <span class="tag ok">comprobado</span> (Go sin activar en el equipo; hay un contenedor de laboratorio) | shims de mise |
| Rust | rustup | <span class="tag ok">comprobado</span> | `~/.rustup`, `~/.cargo` |
| Python | uv | <span class="tag ok">comprobado</span> | gestiona intérpretes y entornos por proyecto |
| Flutter | FVM 4.3.2 | <span class="tag ok">comprobado</span> (Flutter y Dart, por verificar con `fvm list`) | `~/fvm`, por proyecto |
| JS (paquetes) | npm 11.19.1, pnpm 12.15.0 | <span class="tag ok">comprobado</span> | `node_modules` por proyecto |

Los comandos de abajo son los habituales de cada herramienta. Si alguno falla, `<herramienta> --help` es la fuente de verdad: las interfaces cambian entre versiones.

## NVM (Node.js)

```bash
nvm ls                     # versiones instaladas
nvm ls-remote --lts        # versiones LTS disponibles
nvm install --lts          # instalar la última LTS
nvm use 24                 # usar la serie 24 en esta terminal
nvm alias default 24       # versión por defecto en terminales nuevas
node -v && npm -v
```

Fijar la versión de un proyecto con un archivo `.nvmrc`:

```bash
echo "24" > .nvmrc
nvm use                    # lee .nvmrc
```

## SDKMAN (Java y Maven)

```bash
sdk current                # versiones activas
sdk list java              # versiones disponibles
sdk install java 25.0.4.1-tem   # instalar Temurin concreto
sdk use java 25.0.4.1-tem  # solo esta terminal
sdk default java 25.0.4.1-tem   # por defecto
```

Sabrás que Java es el correcto con `java -version` y `mvn --version` (esta muestra la ruta del JDK que usa).

## mise (Go y otros)

```bash
mise ls                    # lo instalado y activo
mise ls-remote go          # versiones disponibles de Go
mise use -g go@<versión>   # activar globalmente (cambia ~/.config/mise/config.toml)
mise use go@<versión>      # solo en esta carpeta (crea mise.toml)
go version
```

Para arreglar el error de tu informe (`No version is set for shim: go`), el siguiente paso será elegir una versión de `mise ls-remote go` y ejecutar `mise use -g go@<versión>`. Tu propio mensaje sugería `go@1.27.3`; compruébalo primero.

## rustup (Rust)

```bash
rustup show                # toolchain activo y objetivos
rustup update              # actualizar
rustc --version
cargo --version
rustup component add clippy rustfmt   # linter y formateador
```

## uv (Python)

```bash
uv --version
uv python list             # intérpretes disponibles
uv init my-script          # proyecto nuevo (crea pyproject.toml)
uv add requests            # añadir dependencia
uv run main.py             # ejecutar dentro del entorno del proyecto
```

`uv` crea un entorno virtual (`.venv`) por proyecto. No necesitas `pip` del sistema.

## FVM (Flutter)

```bash
fvm --version
fvm list
fvm install stable
fvm use stable             # fija la versión en el proyecto actual
fvm flutter doctor         # diagnostica la instalación
```

Dentro de un proyecto con FVM se escribe `fvm flutter ...` y `fvm dart ...` en lugar de `flutter` y `dart` a secas.

## npm y pnpm

```bash
npm --version  &&  pnpm --version
pnpm install               # instalar dependencias de package.json
pnpm add react             # añadir una dependencia
pnpm add -D typescript     # añadir una dependencia de desarrollo
pnpm run dev               # ejecutar un script de package.json
```

| | npm | pnpm |
|---|---|---|
| Lockfile | `package-lock.json` | `pnpm-lock.yaml` |
| Dónde guarda los paquetes | `node_modules` por proyecto | Almacén común con enlaces: menos disco |
| Cuándo | Viene con Node; suficiente | Más rápido y ordenado en varios proyectos |

> **Regla:** en un mismo proyecto usa **solo uno**. Mezclarlos produce dos lockfiles que discrepan.

## Cuándo usar apt y cuándo un gestor

- **apt**: compiladores y librerías del sistema (`gcc`, `cmake`, `libwebkit2gtk…`), Docker, utilidades.
- **Gestor de versiones**: lenguajes y runtimes (Node, Java, Go, Python de proyecto), porque cambian de versión con frecuencia y cada proyecto puede necesitar una distinta.

> **Ejercicio:** en una carpeta vacía, crea `.nvmrc` con otra versión de Node distinta a la tuya, entra, ejecuta `nvm use` y observa qué ocurre. Luego vuelve a la versión por defecto.
