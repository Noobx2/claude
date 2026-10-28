# La terminal

Todo lo que haces en una app (crear proyectos, instalar dependencias, ejecutar pruebas) empieza en la terminal. Este módulo cubre lo imprescindible.

## 1. Leer el prompt

```text
dev26@ubuntu26:~$
```

| Parte | Significado |
|---|---|
| `dev26` | Tu usuario |
| `ubuntu26` | Nombre del equipo |
| `~` | Directorio actual; `~` es tu carpeta personal (`/home/dev26`) |
| `$` | Usuario normal. Un administrador (root) vería `#` |

## 2. Moverte

| Comando | Para qué | Ejemplo |
|---|---|---|
| `pwd` | Ver dónde estás | `pwd` |
| `ls` | Listar archivos | `ls -la` (todo, con detalles y ocultos) |
| `cd` | Cambiar de carpeta | `cd ~/projects`, `cd ..` (subir), `cd -` (volver) |
| `mkdir` | Crear carpeta | `mkdir -p app/backend` (crea también las intermedias) |
| `tree` | Ver árbol (si está instalado) | `tree -L 2` |

Los archivos y carpetas que empiezan por punto (`.env`, `.git`, `.gitignore`) están **ocultos**; `ls -a` los muestra.

## 3. Archivos

| Comando | Para qué | Ejemplo |
|---|---|---|
| `touch` | Crear archivo vacío | `touch README.md` |
| `cp` | Copiar | `cp .env.example .env` |
| `mv` | Mover o renombrar | `mv old.txt new.txt` |
| `rm` | Borrar (**sin papelera**) | `rm file.txt`, `rm -r folder` |
| `cat` | Mostrar contenido | `cat package.json` |
| `less` | Leer archivos largos | `less log.txt` (sale con `q`) |
| `head` / `tail` | Primeras / últimas líneas | `tail -f server.log` (sigue el final) |

> **Peligro:** `rm -r` no tiene papelera ni confirmación. Revisa la ruta con `ls` antes de borrar.

## 4. Buscar

```bash
# Texto dentro de archivos (recursivo, con número de línea)
grep -rn "TODO" src/

# Archivos por nombre
find . -name "*.ts" -not -path "./node_modules/*"

# Dónde está un comando
which node
command -v node
```

## 5. Tuberías y redirecciones

Una terminal brilla al combinar comandos pequeños.

| Símbolo | Qué hace | Ejemplo |
|---|---|---|
| `\|` | La salida de uno es la entrada del siguiente | `ls \| grep ".md"` |
| `>` | Escribe la salida en un archivo (sobrescribe) | `ls > files.txt` |
| `>>` | Añade al final de un archivo | `echo "line" >> log.txt` |
| `2>&1` | Une errores y salida normal | `mvn --version 2>&1` |
| `&&` | Ejecuta el siguiente solo si el anterior funcionó | `mkdir app && cd app` |
| `;` | Ejecuta uno tras otro, aunque falle | `cd app ; ls` |

Eso explica la estructura de tu informe: cada `2>&1` captura mensajes de error junto con la salida para verlos todos.

## 6. Permisos

```bash
ls -l script.sh
# -rwxr-xr-- 1 dev26 dev26 120 oct 27 10:00 script.sh
#  ^^^ ^^^ ^^   dueño   grupo
```

Los tres bloques `rwx` son lectura, escritura y ejecución para dueño, grupo y resto.

```bash
chmod +x script.sh        # hacer ejecutable
./script.sh               # ejecutarlo (el ./ indica "en esta carpeta")
sudo comando              # ejecutar como administrador (úsalo solo cuando haga falta)
```

## 7. Procesos y puertos

```bash
ps aux | grep node        # procesos que contienen "node"
top                       # monitor interactivo (sale con q)
ss -ltnp                  # puertos TCP en escucha y qué proceso los usa
kill <PID>                # pedir a un proceso que termine
```

## 8. Atajos que ahorran tiempo

| Atajo | Efecto |
|---|---|
| `Tab` | Autocompleta rutas y comandos |
| `↑` / `↓` | Recorre el historial |
| `Ctrl+R` | Busca en el historial |
| `Ctrl+C` | Interrumpe el comando actual |
| `Ctrl+L` | Limpia la pantalla |
| `Ctrl+A` / `Ctrl+E` | Principio / final de la línea |

## 9. Pedir ayuda

```bash
ls --help        # resumen rápido
man ls           # manual completo (sale con q)
```

Los comandos modernos de desarrollo también suelen tener `--help`: `node --help`, `uv --help`, `cargo --help`.

> **Ejercicio:** sin copiar de aquí, crea la estructura `practice/app/backend`, `practice/app/frontend` y `practice/docs`; crea un `README.md` vacío en `practice`; lista todo con `ls -R`; y bórralo todo al final.

> **Ejercicio:** averigua qué proceso ocupa un puerto cualquiera de tu equipo usando `ss` y `grep`.
