# PATH, variables y perfiles

Casi todos los errores de «el comando no se encuentra» o «usa otra versión» se explican con este módulo.

## 1. Variables de entorno

Una **variable de entorno** es un par `NOMBRE=valor` que un proceso hereda de quien lo lanzó.

```bash
# Variable de shell: solo existe en esta terminal
GREETING="hola"
echo "$GREETING"            # hola
bash -c 'echo "[$GREETING]"'  # [] -> el proceso hijo no la ve

# Variable exportada: la heredan los procesos hijos
export GREETING="hola"
bash -c 'echo "[$GREETING]"'  # [hola]
```

| Comando | Para qué |
|---|---|
| `env` o `printenv` | Listar variables exportadas |
| `printenv HOME` | Ver una variable concreta |
| `export NAME=value` | Crearla y exportarla en esta terminal |
| `unset NAME` | Eliminarla |

> **Regla:** una variable creada en una terminal **desaparece al cerrarla** y no llega a otras terminales abiertas. Para que persista hay que escribirla en un archivo de perfil (sección 3).

## 2. PATH

`PATH` es una lista de carpetas separadas por `:`. Cuando escribes `node`, el shell busca un ejecutable con ese nombre **en orden**, carpeta por carpeta, y usa el primero que encuentra.

```bash
echo "$PATH" | tr ':' '\n'      # una carpeta por línea
command -v node                  # cuál se usará
type -a python3                  # todas las coincidencias, en orden
```

El orden importa: dos versiones de la misma herramienta conviven, pero gana la que esté antes en el `PATH`. Así funcionan los gestores de versiones: ponen su carpeta **al principio**.

Añadir una carpeta al principio o al final:

```bash
export PATH="$HOME/bin:$PATH"     # gana lo de $HOME/bin
export PATH="$PATH:$HOME/bin"     # gana lo que ya existía
```

> **Ojo:** nunca escribas `export PATH="$HOME/bin"` sin `$PATH` detrás. Sustituiría la lista entera y dejarías de encontrar `ls`, `git` y todo lo demás.

## 3. Perfiles del shell

Los archivos de perfil son scripts que el shell ejecuta al arrancar.

| Archivo | Cuándo se lee | Para qué usarlo |
|---|---|---|
| `~/.bashrc` | Cada terminal interactiva de `bash` | Alias, `PATH`, activar NVM / SDKMAN / mise |
| `~/.profile` | Al iniciar sesión (login) | Variables que deben existir siempre; en Ubuntu suele cargar `.bashrc` |
| `/etc/environment` | Todo el sistema | Variables globales (no es un script; solo `NOMBRE=valor`) |

Para aplicar un cambio sin cerrar la terminal:

```bash
source ~/.bashrc
```

> **Ojo:** si una herramienta funciona por SSH pero no en otro tipo de sesión (por ejemplo, un servicio o una tarea programada), suele ser porque su configuración vive en `.bashrc`, que esa sesión no lee.

## 4. Cómo se ve un gestor de versiones en el perfil

Estos gestores añaden líneas a tu `~/.bashrc` al instalarse. Los ejemplos son representativos; las líneas exactas dependen de la versión del instalador.

```bash
# NVM: define dónde vive y carga la función `nvm`
export NVM_DIR="$HOME/.nvm"
[ -s "$NVM_DIR/nvm.sh" ] && . "$NVM_DIR/nvm.sh"

# SDKMAN: debe ir lo más al final posible del archivo
export SDKMAN_DIR="$HOME/.sdkman"
[[ -s "$HOME/.sdkman/bin/sdkman-init.sh" ]] && source "$HOME/.sdkman/bin/sdkman-init.sh"

# mise: activa sus herramientas al cambiar de carpeta
eval "$(mise activate bash)"
```

Para ver qué tienes realmente:

```bash
grep -nE 'nvm|sdkman|mise|cargo|flutter|fvm|ANDROID' ~/.bashrc ~/.profile 2>/dev/null
```

## 5. Leer los mensajes de tu informe

| Salida de tu informe | Qué significa | Qué mirar |
|---|---|---|
| `Command 'python' not found, did you mean: command 'python3'` | Ubuntu no crea `python` por defecto; solo `python3` | Usa `python3`. Existe un paquete `python-is-python3` que lo crea, pero no es necesario |
| `/usr/bin/python3: No module named pip` | El `python3` del sistema no trae `pip` | En proyectos usa `uv`; evita instalar paquetes en el Python del sistema |
| `mise ERROR No version is set for shim: go` | El shim de `go` existe, pero ninguna versión está activada | `mise ls-remote go`, luego `mise use -g go@<versión>` |
| `Command 'gradle' not found, but can be installed with: sudo apt install gradle` | No está instalado | No hace falta ahora |

Un **shim** es un ejecutable falso que vive en el `PATH` y, al ejecutarse, decide qué versión real lanzar. Por eso `go` «existe» y falla: el shim está, pero no tiene a qué redirigir.

## 6. Variables del sistema frente a variables de una aplicación

No son lo mismo, aunque se lean igual.

| | Variables del sistema / shell | Variables de la aplicación |
|---|---|---|
| Ejemplos | `PATH`, `HOME`, `JAVA_HOME` | `DATABASE_URL`, `PORT`, `API_KEY` |
| Para quién | El sistema y las herramientas | Tu programa |
| Dónde se definen | Perfiles del shell, `/etc/environment` | Archivo `.env`, variables del contenedor, panel del servidor |
| Quién las lee | El shell y los procesos | Tu código o un cargador (`dotenv`, Vite, Spring…) |

Cómo las lee cada lenguaje:

| Lenguaje | Lectura |
|---|---|
| JavaScript (Node) | `process.env.PORT` |
| Vite (frontend) | `import.meta.env.VITE_API_URL` (solo las que empiezan por `VITE_`) |
| Python | `os.environ.get("PORT")` |
| Java | `System.getenv("PORT")` |
| C# | `Environment.GetEnvironmentVariable("PORT")` |
| Go | `os.Getenv("PORT")` |
| Rust | `std::env::var("PORT")` |
| Dart | `Platform.environment['PORT']` |

> **Peligro:** todo lo que va al frontend (por ejemplo `VITE_*`) acaba **visible para cualquiera** en el navegador. Jamás pongas un secreto ahí.

### Desarrollo y producción

| | Desarrollo | Producción |
|---|---|---|
| Dónde viven los valores | `.env` local (ignorado por Git) | Variables del servidor o del contenedor, gestor de secretos |
| Se versiona | `.env.example` (con valores falsos) | Nunca el real |
| Cambia | A mano, a menudo | Con un despliegue controlado |

Ejemplo de `.env.example` (valores ficticios):

```bash
# Puerto del servidor
PORT=3000
# Cadena de conexión: sustituye por la tuya en .env
DATABASE_URL=mongodb://localhost:27017/app
# Clave secreta de ejemplo; genera una real y no la subas a Git
JWT_SECRET=CHANGE_ME
```

## 7. Diagnóstico rápido

1. `command -v <herramienta>`: ¿existe en el `PATH`?
2. `type -a <herramienta>`: ¿hay varias? ¿cuál gana?
3. `echo "$PATH" | tr ':' '\n'`: ¿está la carpeta esperada?
4. ¿Reiniciaste la terminal o hiciste `source ~/.bashrc` tras editar el perfil?
5. ¿Estás en la terminal correcta (otra sesión SSH, otra ventana de VS Code)?

> **Ejercicio:** crea el archivo `~/bin/hello` con un script de una línea que haga `echo "hello"`, hazlo ejecutable, añade `~/bin` al `PATH` en tu perfil y comprueba que `hello` funciona en una terminal nueva.

> **Ejercicio:** define una variable `APP_MODE=dev` sin `export`, y pruébala con `bash -c`. Repite con `export`. Explica la diferencia con tus palabras.
