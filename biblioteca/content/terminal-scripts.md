# Scripts de Bash

Un script es un archivo con comandos de terminal que se ejecutan en orden. Es la herramienta para automatizar tareas y la puerta de entrada a las aplicaciones de consola.

*Verificado: 2026-10-27. Tu shell es `/bin/bash`. Referencia: [manual de Bash](https://www.gnu.org/software/bash/manual/bash.html).*

## 1. Tu primer script

```bash
mkdir -p ~/scripts && cd ~/scripts
nano hello.sh
```

Contenido de `hello.sh`:

```bash
#!/usr/bin/env bash
# Saluda al usuario
echo "Hola, $USER"
```

```bash
chmod +x hello.sh      # permiso de ejecución
./hello.sh             # el ./ indica "en esta carpeta"
bash hello.sh          # alternativa sin permiso de ejecución
```

- La primera línea (`#!`, *shebang*) indica qué intérprete ejecuta el archivo.
- Nombres: `kebab-case.sh`, en minúsculas.

## 2. Variables y comillas

```bash
name="Ana"              # sin espacios alrededor del =
echo "Hola, $name"      # comillas dobles: expanden variables
echo 'Hola, $name'      # comillas simples: texto literal
echo "Hola, ${name}s"   # llaves para delimitar el nombre

today=$(date +%F)       # sustitución de comandos: guarda la salida
echo "Hoy es $today"

count=$((2 + 3))        # aritmética entera
```

> **Regla:** pon **siempre** comillas dobles alrededor de las variables (`"$name"`). Sin ellas, un valor con espacios se rompe en varias palabras y el script falla de formas difíciles de ver.

## 3. Argumentos: leer parámetros de la línea de comandos

```bash
#!/usr/bin/env bash
# uso: ./greet.sh NOMBRE [VECES]
name="${1:-mundo}"      # primer argumento, o "mundo" si falta
times="${2:-1}"

for ((i = 0; i < times; i++)); do
  echo "Hola, $name"
done
```

| Variable | Significado |
|---|---|
| `$0` | Nombre del script |
| `$1`, `$2`… | Argumentos posicionales |
| `$#` | Cantidad de argumentos |
| `"$@"` | Todos los argumentos, cada uno por separado (úsalo con comillas) |
| `$?` | Código de salida del último comando |
| `${1:-valor}` | Valor por defecto si falta `$1` |

```bash
./greet.sh Ana 3
```

### Opciones con nombre (`-n`, `-t`)

```bash
#!/usr/bin/env bash
name="mundo"
times=1

usage() {
  echo "Uso: $0 [-n NOMBRE] [-t VECES]" >&2
  exit 2
}

while getopts "n:t:h" opt; do
  case "$opt" in
    n) name="$OPTARG" ;;
    t) times="$OPTARG" ;;
    h|*) usage ;;
  esac
done

for ((i = 0; i < times; i++)); do
  echo "Hola, $name"
done
```

```bash
./greet.sh -n Ana -t 2
```

`getopts` solo entiende opciones cortas. Para `--name` largas, en Bash se recorre `"$@"` con un `case` y `shift`; si el script crece, es mejor pasar a un lenguaje (ver [CLI y streams](#cli-args-streams)).

## 4. Leer del teclado y de tuberías

```bash
read -r -p "¿Cómo te llamas? " answer      # -r evita interpretar barras invertidas
echo "Hola, $answer"

read -r -s -p "Contraseña: " secret        # -s no muestra lo que escribes
echo
```

Procesar líneas que llegan por una tubería o un archivo:

```bash
cat names.txt | while read -r line; do
  echo "Procesando: $line"
done

while read -r line; do
  echo "Procesando: $line"
done < names.txt
```

## 5. Condiciones

```bash
if [[ -f "$file" ]]; then
  echo "Es un archivo"
elif [[ -d "$file" ]]; then
  echo "Es una carpeta"
else
  echo "No existe"
fi
```

| Prueba | Significa |
|---|---|
| `-f ruta` / `-d ruta` / `-e ruta` | Archivo / carpeta / existe |
| `-z "$s"` / `-n "$s"` | Cadena vacía / no vacía |
| `"$a" == "$b"` | Cadenas iguales |
| `n -eq m`, `-lt`, `-gt` | Números iguales, menor, mayor |
| `&&`, `\|\|`, `!` | Y, O, negación |

Usa `[[ ... ]]` en Bash: es más seguro que `[ ... ]`.

## 6. Funciones, bucles y códigos de salida

```bash
log() {
  echo "[$(date +%T)] $*" >&2        # >&2 envía a stderr
}

for file in *.txt; do
  [[ -e "$file" ]] || continue       # si no hay coincidencias, el glob queda literal
  log "Archivo: $file"
done
```

Todo comando devuelve un código de salida: **0 = éxito**, distinto de 0 = fallo.

```bash
if grep -q "TODO" notes.txt; then echo "hay pendientes"; fi
mkdir backup || { echo "No se pudo crear la carpeta" >&2; exit 1; }
exit 0
```

## 7. Salidas: stdout, stderr y redirecciones

| Flujo | Número | Para qué |
|---|---|---|
| stdin | 0 | Entrada |
| stdout | 1 | Datos de salida |
| stderr | 2 | Mensajes de error y avisos |

```bash
./script.sh > out.txt          # solo stdout al archivo
./script.sh 2> errors.txt      # solo errores al archivo
./script.sh > all.txt 2>&1     # ambos al mismo archivo
./script.sh > /dev/null 2>&1   # silenciar todo
```

> **Regla:** los **datos** van por stdout y los **mensajes** por stderr. Así puedes encadenar el script con `|` sin mezclar mensajes con datos.

## 8. Modo estricto

```bash
set -euo pipefail
```

| Opción | Efecto |
|---|---|
| `-e` | Termina si un comando falla |
| `-u` | Error si usas una variable sin definir |
| `-o pipefail` | Una tubería falla si falla cualquiera de sus pasos |

Útil en scripts de automatización. Tiene excepciones sutiles (por ejemplo, dentro de `if` o con `||`), así que no sustituye a comprobar los resultados importantes.

## 9. Limpieza con `trap`

```bash
tmp="$(mktemp -d)"
trap 'rm -rf "$tmp"' EXIT      # se ejecuta al terminar, incluso con error o Ctrl+C
echo "Trabajando en $tmp"
```

## 10. Texto de varias líneas (*here-doc*)

```bash
cat > .env.example <<'EOF'
# Puerto del servidor
PORT=3000
JWT_SECRET=CHANGE_ME
EOF
```

Con `'EOF'` entre comillas, el contenido no expande variables.

## 11. Ejemplo útil: copia de seguridad con fecha

```bash
#!/usr/bin/env bash
set -euo pipefail

source_dir="${1:?uso: $0 CARPETA_ORIGEN [CARPETA_DESTINO]}"
dest_dir="${2:-$HOME/backups}"
mkdir -p "$dest_dir"

archive="$dest_dir/$(basename "$source_dir")-$(date +%F).tar.gz"
tar -czf "$archive" -C "$(dirname "$source_dir")" "$(basename "$source_dir")"
echo "Creado: $archive" >&2
sha256sum "$archive"
```

`${1:?mensaje}` termina el script con ese mensaje si falta el argumento.

## 12. Calidad y depuración

| Herramienta | Uso |
|---|---|
| `bash -x script.sh` | Muestra cada comando antes de ejecutarlo |
| `set -x` / `set +x` | Activa y desactiva esa traza dentro del script |
| ShellCheck | Analizador estático de scripts; detecta errores típicos (se instala con apt) |
| `bash -n script.sh` | Comprueba la sintaxis sin ejecutar |

## 13. Cuándo pasar de Bash a otro lenguaje

Bash es excelente para unir comandos. Cambia a Node, Python, Go o C# cuando:

- necesites estructuras de datos (listas, diccionarios, JSON) más allá de lo trivial;
- el script supere unas decenas de líneas con mucha lógica;
- tengas que ejecutarse también en Windows;
- necesites pruebas automáticas o librerías.

## 14. Errores habituales

| Mensaje o síntoma | Causa | Solución |
|---|---|---|
| `Permission denied` | Falta permiso de ejecución | `chmod +x script.sh` |
| `command not found` al ejecutar `script.sh` | Falta `./` | `./script.sh` |
| `bad interpreter: /bin/bash^M` | El archivo tiene finales de línea de Windows (CRLF) | Guarda con LF en el editor |
| `unbound variable` | Variable sin definir con `set -u` | Usa `${var:-}` o define el valor |
| El script se rompe con nombres con espacios | Faltan comillas en `"$var"` | Pon comillas |
| `[: too many arguments` | Variable sin comillas dentro de `[ ]` | Usa `[[ ]]` y comillas |

## 15. Ejercicios

> **Ejercicio:** escribe `count-lines.sh` que reciba una carpeta como argumento y muestre cuántas líneas tiene cada `.md`. Debe fallar con un mensaje claro y código 2 si falta el argumento.

> **Ejercicio:** crea `ask.sh` que pregunte nombre y edad con `read`, valide que la edad es un número y lo informe por stderr si no lo es.

> **Ejercicio:** convierte la copia de seguridad en una versión con `getopts` (`-s origen`, `-d destino`) y comprueba el resultado con `tar -tzf`.
