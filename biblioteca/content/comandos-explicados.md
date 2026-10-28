# Comandos explicados flag a flag

Referencia para entender **cada pieza** de un comando: el programa, cada opción (*flag*) y cada argumento. Los comandos son de Linux (tu Ubuntu); las opciones exactas pueden variar entre versiones, así que `comando --help` y `man comando` son siempre la fuente de verdad.

*Verificado: 2026-10-27 con conocimiento estable de las herramientas; no se han ejecutado en tu máquina.*

## 1. Cómo leer un comando

```text
grep  -rn  --include="*.ts"  "TODO"  src/
 │     │          │             │      │
 │     │          │             │      └─ argumento posicional (dónde buscar)
 │     │          │             └─ argumento posicional (qué buscar)
 │     │          └─ opción larga con valor (--nombre=valor)
 │     └─ opciones cortas agrupadas (-r -n)
 └─ programa
```

| Elemento | Reglas |
|---|---|
| Opción corta | Un guion y una letra: `-l`. Se pueden agrupar: `-la` equivale a `-l -a` |
| Opción larga | Dos guiones y una palabra: `--all`. Algunas llevan valor: `--color=auto` |
| Opción con valor | `-n 5` o `-n5`; depende del programa |
| `--` | Fin de las opciones: lo que sigue son argumentos aunque empiece por `-` |
| Comillas | Protegen espacios y símbolos del shell: `"mi archivo.txt"` |

## 2. Navegar y ver

### `ls`: listar

```bash
ls -lah
```

| Pieza | Significado |
|---|---|
| `ls` | Lista el contenido de la carpeta actual |
| `-l` | Formato largo: permisos, dueño, tamaño y fecha |
| `-a` | Incluye archivos ocultos (los que empiezan por `.`) |
| `-h` | Tamaños legibles (K, M, G); solo tiene efecto con `-l` |

Otras: `-R` recursivo, `-t` ordena por fecha, `-S` por tamaño, `-r` invierte el orden, `-1` uno por línea, `-d` muestra la carpeta en sí y no su contenido.

### `cd`, `pwd`

| Comando | Significado |
|---|---|
| `cd ruta` | Entra en la carpeta |
| `cd ..` | Sube un nivel |
| `cd -` | Vuelve a la carpeta anterior |
| `cd` o `cd ~` | A tu carpeta personal |
| `pwd` | Muestra la ruta actual |

### `cat`, `less`, `head`, `tail`, `wc`

```bash
tail -n 50 -f server.log
```

| Pieza | Significado |
|---|---|
| `tail` | Muestra el final de un archivo |
| `-n 50` | Últimas 50 líneas |
| `-f` | *Follow*: sigue el archivo y muestra las líneas nuevas (se sale con Ctrl+C) |
| `server.log` | El archivo |

| Comando | Opciones |
|---|---|
| `cat -n archivo` | `-n` numera las líneas |
| `head -n 10 archivo` | Primeras 10 líneas (`-c 100` primeros 100 bytes) |
| `wc -l archivo` | `-l` líneas, `-w` palabras, `-c` bytes, `-m` caracteres |
| `less archivo` | Navegar: flechas, `/texto` busca, `n` siguiente resultado, `q` sale |

## 3. Crear, copiar, mover y borrar

```bash
mkdir -p app/backend/src
```

| Pieza | Significado |
|---|---|
| `mkdir` | Crea carpetas |
| `-p` | Crea también las intermedias y no falla si ya existen |

| Comando | Opciones importantes |
|---|---|
| `cp -r origen destino` | `-r` copia carpetas recursivamente; `-i` pregunta antes de sobrescribir; `-v` muestra lo copiado; `-a` conserva permisos y fechas |
| `mv origen destino` | Mueve o renombra; `-i` pregunta; `-n` no sobrescribe; `-v` verboso |
| `rm archivo` | `-r` borra carpetas; `-f` no pregunta ni falla si no existe; `-i` pregunta; `-v` verboso |
| `touch archivo` | Crea el archivo vacío o actualiza su fecha |
| `ln -s destino enlace` | `-s` crea un enlace simbólico (un acceso directo) |

> **Peligro:** `rm -rf` no pregunta y no tiene papelera. Antes de borrar, ejecuta el mismo comando con `ls` en lugar de `rm` para ver qué afectaría.

## 4. Buscar

### `grep`: buscar texto

```bash
grep -rniE --include="*.js" "todo|fixme" src/
```

| Pieza | Significado |
|---|---|
| `-r` | Recursivo: entra en las subcarpetas |
| `-n` | Muestra el número de línea |
| `-i` | Ignora mayúsculas y minúsculas |
| `-E` | Expresiones regulares extendidas (permite `\|` como «o») |
| `--include="*.js"` | Solo archivos que coincidan con el patrón |
| `"todo\|fixme"` | Patrón: «todo» o «fixme» |
| `src/` | Dónde buscar |

Más opciones: `-v` invierte (líneas que NO coinciden), `-w` palabra completa, `-c` cuenta las líneas, `-l` solo nombres de archivos, `-q` silencioso (solo el código de salida), `-F` texto literal (sin regex), `-A 2` / `-B 2` / `-C 2` muestran 2 líneas después / antes / alrededor.

### `find`: buscar archivos

```bash
find . -maxdepth 3 -type f -name "*.log" -mtime -1 -size +1M
```

| Pieza | Significado |
|---|---|
| `.` | Empieza en la carpeta actual |
| `-maxdepth 3` | No baja más de 3 niveles |
| `-type f` | Solo archivos (`d` carpetas, `l` enlaces) |
| `-name "*.log"` | Nombre que encaje con el patrón (distingue mayúsculas; `-iname` no) |
| `-mtime -1` | Modificados hace menos de 1 día |
| `-size +1M` | Mayores de 1 MiB |

Acciones:

```bash
find . -name "*.tmp" -delete                      # borra lo encontrado
find . -name "*.js" -exec wc -l {} +              # ejecuta wc con los archivos: {} es cada archivo, + los agrupa
find . -not -path "./node_modules/*" -name "*.ts" # excluye una ruta
```

## 5. Procesar texto con tuberías

```bash
sort data.txt | uniq -c | sort -rn | head -n 5
```

| Pieza | Significado |
|---|---|
| `sort data.txt` | Ordena las líneas |
| `\| uniq -c` | Colapsa repetidas y **cuenta** cuántas hay (exige entrada ordenada) |
| `\| sort -rn` | Ordena numéricamente (`-n`) de mayor a menor (`-r`) |
| `\| head -n 5` | Se queda con las 5 primeras |

| Comando | Opciones |
|---|---|
| `sort` | `-n` numérico, `-r` inverso, `-u` sin repetidos, `-k2` por la 2.ª columna, `-t,` separador coma, `-h` tamaños legibles |
| `uniq` | `-c` cuenta, `-d` solo las repetidas, `-u` solo las únicas |
| `cut -d',' -f1,3 archivo` | `-d` separador, `-f` columnas a conservar; `-c1-5` por caracteres |
| `tr 'a-z' 'A-Z'` | Sustituye caracteres; `-d 'x'` borra; `-s ' '` comprime repeticiones |
| `tee archivo` | Escribe a pantalla **y** a un archivo; `-a` añade al final |
| `xargs` | Convierte líneas de entrada en argumentos |

```bash
find . -name "*.md" -print0 | xargs -0 -n 1 wc -l
```

| Pieza | Significado |
|---|---|
| `-print0` | Separa los nombres con un byte nulo (seguro con espacios) |
| `xargs -0` | Espera esa separación |
| `-n 1` | Un argumento por ejecución de `wc` |

### `sed` y `awk`

```bash
sed -i.bak 's/localhost/db.example.com/g' config.txt
```

| Pieza | Significado |
|---|---|
| `-i.bak` | Modifica el archivo **en su sitio** y guarda una copia con extensión `.bak` |
| `s/…/…/g` | Sustituye (*substitute*): `s/buscar/reemplazo/`; `g` = todas las apariciones de cada línea |
| `-n '5,10p'` | `-n` no imprime por defecto; `5,10p` imprime las líneas 5 a 10 |
| `-E` | Regex extendidas |

```bash
awk -F',' '{ print $1, $3 }' data.csv
```

| Pieza | Significado |
|---|---|
| `-F','` | Separador de campos |
| `'{ print $1, $3 }'` | Para cada línea, imprime el campo 1 y el 3 |
| `$0` | La línea completa; `NF` número de campos; `NR` número de línea |

## 6. Permisos, usuarios y grupos

```bash
chmod 755 script.sh
```

| Dígito | Quién | Valor |
|---|---|---|
| 7 | Dueño | `rwx` = 4 (leer) + 2 (escribir) + 1 (ejecutar) |
| 5 | Grupo | `r-x` = 4 + 1 |
| 5 | Resto | `r-x` = 4 + 1 |

| Comando | Significado |
|---|---|
| `chmod +x archivo` | Añade permiso de ejecución |
| `chmod 600 archivo` | Solo el dueño lee y escribe (típico de claves privadas y `.env`) |
| `chmod -R 755 carpeta` | `-R` recursivo |
| `chown usuario:grupo archivo` | Cambia dueño y grupo (`-R` recursivo) |
| `id -nG` | `-n` muestra nombres en lugar de números; `-G` todos los grupos |
| `groups` | Grupos del usuario actual |
| `sudo comando` | Ejecuta como administrador; `-u usuario` como otro usuario; `-i` shell completa de root |

```bash
sudo usermod -aG dialout dev26
```

| Pieza | Significado |
|---|---|
| `usermod` | Modifica un usuario |
| `-G dialout` | Grupos adicionales |
| `-a` | **Añadir**: sin `-a`, `-G` reemplaza **todos** los grupos y puedes perder acceso |
| `dev26` | El usuario |

El cambio solo se aplica al **iniciar sesión de nuevo**.

## 7. Procesos y sistema

```bash
ps aux | grep node
```

| Pieza | Significado |
|---|---|
| `ps` | Lista procesos |
| `a` | De todos los usuarios con terminal |
| `u` | Formato orientado al usuario (CPU, memoria) |
| `x` | También los que no tienen terminal |
| `grep node` | Filtra las líneas que contienen «node» |

| Comando | Significado |
|---|---|
| `kill 1234` | Envía SIGTERM (terminar con calma) al proceso 1234 |
| `kill -9 1234` | SIGKILL: terminación inmediata, sin limpieza (último recurso) |
| `pkill -f patrón` | Mata procesos por nombre; `-f` busca en la línea de comando completa |
| `top` / `htop` | Monitor interactivo (`q` sale) |
| `du -sh carpeta` | `-s` resumen total, `-h` legible |
| `du -h -d 1` | `-d 1` muestra un nivel de profundidad |
| `df -h` | Espacio en discos; `-T` muestra el tipo de sistema de archivos |
| `free -h` | Memoria libre y usada |

### `ss`: puertos y conexiones

```bash
ss -ltnp
```

| Pieza | Significado |
|---|---|
| `ss` | Estadísticas de sockets (sustituye a `netstat`) |
| `-l` | Solo los que **escuchan** |
| `-t` | TCP (`-u` sería UDP) |
| `-n` | Números en lugar de nombres (puertos y direcciones) |
| `-p` | Proceso propietario (necesita `sudo` para procesos de otros usuarios) |

Más: `-a` todos los estados, `-4` / `-6` solo IPv4 / IPv6. Para saber quién usa el puerto 3000: `sudo ss -ltnp 'sport = :3000'`.

### `systemctl` y `journalctl`

```bash
systemctl list-units --type=service --state=running --no-pager
```

| Pieza | Significado |
|---|---|
| `systemctl` | Controla los servicios de systemd |
| `list-units` | Lista unidades cargadas |
| `--type=service` | Solo servicios |
| `--state=running` | Solo los que están corriendo |
| `--no-pager` | Imprime todo sin abrir `less` |

| Comando | Significado |
|---|---|
| `systemctl status docker` | Estado de un servicio |
| `sudo systemctl restart docker` | Reinicia |
| `sudo systemctl enable --now docker` | `enable` lo arranca al iniciar el sistema; `--now` además lo arranca ya |
| `journalctl -u ssh -n 50 --no-pager` | `-u` unidad, `-n 50` últimas 50 líneas |
| `journalctl -u ssh -f` | `-f` sigue los mensajes nuevos |
| `journalctl -p err -b` | `-p err` nivel error o peor; `-b` desde el último arranque |
| `journalctl --since "1 hour ago"` | Desde una fecha |

### `apt`

| Comando | Significado |
|---|---|
| `sudo apt update` | Actualiza el **índice** de paquetes (no instala nada) |
| `sudo apt upgrade` | Actualiza los paquetes instalados |
| `sudo apt install -y paquete` | Instala; `-y` responde «sí» automáticamente |
| `apt search texto` / `apt show paquete` | Buscar / ver detalles |
| `sudo apt remove paquete` | Desinstala (conserva configuración); `purge` también la borra |
| `sudo apt autoremove` | Quita dependencias que ya nadie usa |

## 8. Archivos comprimidos y comparación

```bash
tar -czvf backup.tar.gz -C ~/projects my-app
```

| Pieza | Significado |
|---|---|
| `-c` | **C**rear un archivo |
| `-z` | Comprimir con gzip (`-j` bzip2, `-J` xz) |
| `-v` | Mostrar los archivos procesados |
| `-f backup.tar.gz` | Nombre del archivo (**debe ir justo antes** del nombre) |
| `-C ~/projects` | Cambia a esa carpeta antes de empaquetar |
| `my-app` | Lo que se empaqueta |

Extraer: `tar -xzvf backup.tar.gz -C destino/` (`-x` extraer). Listar sin extraer: `tar -tzf backup.tar.gz` (`-t`).

| Comando | Significado |
|---|---|
| `diff -u a.txt b.txt` | `-u` formato unificado (el de Git); `-r` carpetas; `-q` solo dice si difieren |
| `cmp a b` | Compara byte a byte; `-s` silencioso; `-l` lista todas las diferencias |
| `sha256sum archivo` | Huella del archivo; `-c lista.sha256` verifica contra una lista |
| `file archivo` | Detecta el tipo por el contenido; `-i` muestra el tipo MIME |
| `stat archivo` | Tamaño, permisos, fechas |

## 9. Bytes y binarios

```bash
xxd -l 32 -c 16 -g 1 imagen.png
```

| Pieza | Significado |
|---|---|
| `xxd` | Volcado hexadecimal |
| `-l 32` | Solo los primeros 32 bytes |
| `-c 16` | 16 bytes por línea |
| `-g 1` | Agrupa de 1 en 1 byte |

Otras: `-s 100` empieza en el byte 100, `-p` formato plano, `-r` revierte un volcado a binario.

```bash
dd if=sample.bin of=part.bin bs=1M count=3 skip=2 status=progress
```

| Pieza | Significado |
|---|---|
| `if=sample.bin` | *Input file*: de dónde lee |
| `of=part.bin` | *Output file*: dónde escribe |
| `bs=1M` | Tamaño de bloque (1 MiB) |
| `count=3` | Copia 3 bloques |
| `skip=2` | Se salta 2 bloques de la entrada |
| `status=progress` | Muestra el avance |

Más: `seek=N` salta bloques en la salida, `conv=notrunc` no recorta el destino.

> **Peligro:** `dd` no pregunta. Con `of=/dev/sdX` destruye un disco. Revisa `if` y `of` dos veces.

| Comando | Significado |
|---|---|
| `split -b 3M -d -a 3 archivo prefijo.` | `-b` trozos de 3 MiB; `-d` sufijos numéricos; `-a 3` tres dígitos de sufijo |
| `split -l 1000 archivo` | `-l` trozos de 1000 líneas |
| `head -c 10M /dev/urandom > f.bin` | `-c` bytes en lugar de líneas |
| `truncate -s 1M archivo` | Fija el tamaño del archivo |
| `hexdump -C -n 64 archivo` | `-C` formato canónico (hex + ASCII); `-n` limita bytes |
| `od -An -tx1 -N 16 archivo` | `-An` sin direcciones; `-tx1` hex de 1 byte; `-N 16` limita a 16 bytes |

## 10. Red

| Comando | Significado |
|---|---|
| `ip -br a` | `a` direcciones; `-br` formato breve |
| `ip r` | Tabla de rutas |
| `ping -c 4 host` | `-c 4` envía 4 paquetes y termina |
| `dig +short ejemplo.com` | Consulta DNS; `+short` solo la respuesta; `MX` / `AAAA` tipos; `@8.8.8.8` servidor concreto |
| `lsusb -t` | Dispositivos USB en árbol (`-v` detallado) |

### `curl`

```bash
curl -sS -X POST -H "Content-Type: application/json" -d '{"title":"Hola"}' -o out.json -w "%{http_code}\n" http://localhost:3000/api/tasks
```

| Pieza | Significado |
|---|---|
| `-s` | Silencioso (sin barra de progreso) |
| `-S` | Con `-s`, aun así muestra los errores |
| `-X POST` | Método HTTP |
| `-H "…"` | Cabecera de la petición |
| `-d '{…}'` | Cuerpo de la petición |
| `-o out.json` | Guarda la respuesta en un archivo |
| `-w "%{http_code}\n"` | Al final imprime el código de estado |

Otras: `-L` sigue redirecciones, `-I` solo cabeceras (petición HEAD), `-i` incluye las cabeceras en la salida, `-f` falla con código distinto de 0 si el HTTP es 4xx/5xx, `-v` muestra el diálogo completo, `-O` guarda con el nombre remoto.

## 11. Variables, expansión y utilidades del shell

| Comando | Significado |
|---|---|
| `echo -n "texto"` | `-n` sin salto de línea final; `-e` interpreta `\n`, `\t` |
| `printf '%-10s %s\n' "$a" "$b"` | `%-10s` texto alineado a la izquierda en 10 caracteres; `%s` texto; `\n` salto |
| `date +%F` | Fecha `2026-10-27`; `+%T` hora; `+%s` segundos desde 1970 |
| `command -v git` | Dónde está el comando (o nada si no existe) |
| `type -a python3` | Todas las coincidencias |
| `export NAME=valor` | Variable para los procesos hijos |
| `source ~/.bashrc` | Ejecuta el archivo en la shell actual |
| `env` | Muestra las variables exportadas |
| `history`, `!!`, `Ctrl+R` | Historial, repetir último comando, buscar |

## 12. El script de verificación, pieza a pieza

```bash
check() {
  if command -v "$1" >/dev/null 2>&1; then
    printf '%-10s %s\n' "$1" "$("$@" 2>&1 | head -n 1)"
  else
    printf '%-10s NO ENCONTRADO\n' "$1"
  fi
}
```

| Pieza | Significado |
|---|---|
| `check() { … }` | Define una función llamada `check` |
| `"$1"` | Primer argumento: el nombre del programa |
| `command -v "$1"` | ¿Existe en el PATH? |
| `>/dev/null` | Descarta la salida normal |
| `2>&1` | Redirige los errores a donde va la salida normal (aquí, a la nada) |
| `if … then … else … fi` | Condición según el código de salida de `command -v` |
| `"$@"` | Todos los argumentos tal cual: ejecuta, por ejemplo, `node --version` |
| `$( … )` | Sustituye por la salida del comando |
| `\| head -n 1` | Se queda con la primera línea |
| `printf '%-10s %s\n'` | Imprime dos columnas alineadas |

```bash
grep -nE 'nvm|sdkman|mise|cargo|flutter|fvm|ANDROID' ~/.bashrc ~/.profile ~/.bash_profile 2>/dev/null
```

| Pieza | Significado |
|---|---|
| `-n` | Número de línea |
| `-E` | Regex extendida: `a\|b\|c` es «a o b o c» |
| Tres archivos | Los busca en todos |
| `2>/dev/null` | Oculta el error de los archivos que no existan |

```bash
ss -ltn
```

Igual que `ss -ltnp` sin `-p`: no necesita permisos de administrador.

## 13. Ejercicios

> **Ejercicio:** toma tres comandos de las secciones anteriores y, **sin mirar**, escribe el significado de cada flag. Después compruébalo con `man`.

> **Ejercicio:** encuentra los 5 archivos más grandes de tu carpeta personal usando solo `find`, `sort`, `head` y `du`/`stat`. Escribe el desglose de cada tubería.

> **Ejercicio:** cuenta cuántas veces aparece cada palabra de un archivo de texto y muestra las 10 más frecuentes, usando `tr`, `sort`, `uniq` y `head`.
