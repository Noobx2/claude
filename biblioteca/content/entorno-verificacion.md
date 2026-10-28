# Script de verificación

El [inventario](#inventario) se apoya en tu primer informe. Este script cubre lo que quedó sin comprobar: versiones declaradas, servicios, puertos, contenedores y el `PATH`.

> **Regla:** el script **solo lee**. No instala, no escribe archivos ni cambia configuración. Aun así, léelo antes de ejecutarlo; es un buen hábito con cualquier script.

## Qué hace

- Para cada herramienta comprueba si existe en el `PATH` y muestra la primera línea de su versión.
- Lista servicios en ejecución y puertos en escucha.
- Lista contenedores, imágenes y volúmenes de Docker.
- Muestra el `PATH`, una carpeta por línea.
- Busca en tus perfiles las líneas de NVM, SDKMAN, mise, Flutter y Android.
- Revisa si hay dispositivos USB o serie y a qué grupos perteneces.

## Cómo usarlo

1. Copia el bloque en tu terminal y pulsa Enter.
2. Pega la salida completa en la conversación.
3. Antes de pegar, **revisa que no aparezca ningún dato sensible** (Git puede mostrar tu correo; los perfiles, rutas privadas).

```bash
echo "===== HERRAMIENTAS ====="
check() {
  if command -v "$1" >/dev/null 2>&1; then
    printf '%-10s %s\n' "$1" "$("$@" 2>&1 | head -n 1)"
  else
    printf '%-10s NO ENCONTRADO\n' "$1"
  fi
}
check git --version
check node --version
check npm --version
check pnpm --version
check tsc --version
check oxlint --version
check python3 --version
check uv --version
check java -version
check javac -version
check mvn --version
check go version
check rustc --version
check cargo --version
check dart --version
check flutter --version
check fvm --version
check dotnet --version
check kotlinc -version
check adb --version
check docker --version
check mongosh --version
check psql --version
check mise --version

echo "===== SHELL ====="
echo "SHELL=$SHELL"

echo "===== PATH (una carpeta por linea) ====="
echo "$PATH" | tr ':' '\n'

echo "===== LINEAS DE PERFIL RELEVANTES ====="
grep -nE 'nvm|sdkman|mise|cargo|flutter|fvm|ANDROID' ~/.bashrc ~/.profile ~/.bash_profile 2>/dev/null || echo "(ninguna)"

echo "===== SERVICIOS EN EJECUCION ====="
systemctl list-units --type=service --state=running --no-pager 2>&1 | head -n 40

echo "===== PUERTOS EN ESCUCHA ====="
ss -ltn 2>&1

echo "===== DOCKER ====="
docker ps -a 2>&1
docker images 2>&1
docker volume ls 2>&1

echo "===== GIT (configuracion global; revisa antes de pegar) ====="
git config --global --list 2>&1

echo "===== HARDWARE ====="
command -v lsusb >/dev/null 2>&1 && lsusb || echo "lsusb no instalado"
ls /dev/ttyUSB* /dev/ttyACM* 2>/dev/null || echo "sin puertos serie USB conectados"
echo "grupos: $(id -nG)"

echo "===== FIN ====="
```

## Cómo leer la salida

| Sección | Qué buscar |
|---|---|
| Herramientas | Las que digan `NO ENCONTRADO` no están en el `PATH`; las declaradas pasan a comprobadas |
| PATH | Que aparezcan las carpetas de NVM, SDKMAN, mise y Cargo, y en qué orden |
| Perfiles | Qué gestores están activados en `.bashrc` |
| Servicios y puertos | Qué está escuchando ya, para no chocar al arrancar tus apps |
| Docker | Si hay contenedores o volúmenes antiguos |
| Hardware | Si tu usuario está en `dialout` (necesario para puertos serie) y si hay dispositivos conectados |

> **Ojo:** `ss -ltn` no muestra el nombre del proceso sin permisos de administrador. Si necesitas saber qué proceso ocupa un puerto, la receta correspondiente explicará cómo hacerlo con `sudo`.

> **Ejercicio:** antes de ejecutar el script, predice qué herramientas dirán `NO ENCONTRADO`. Compara tu predicción con la salida.

## Resultado de la primera ejecución (2026-10-27)

La salida ya está incorporada al [inventario](#inventario). Lo más relevante:

- Pasan a comprobadas las versiones de Git, Node, npm, pnpm, Java, Rust, Cargo y FVM.
- `tsc` y `oxlint` no están en el PATH porque son dependencias de cada proyecto, no herramientas globales.
- Tu equipo es una máquina virtual y tu usuario no está en el grupo `dialout`.
- Git no tiene configuración global.

Puedes volver a ejecutar el script cuando instales algo; la comparación entre ambas salidas muestra qué cambió.
