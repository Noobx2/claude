# Controlar hardware del ordenador

Este módulo explica cómo un programa **lee y controla dispositivos físicos**: información del propio equipo (CPU, temperatura, discos), puertos serie, USB y dispositivos conectados a tu PC. Se basa en una idea: el sistema operativo ya tiene los **controladores** (*drivers*); tu programa habla con ellos mediante archivos de dispositivo y bibliotecas.

*Verificado: 2026-10-27. Los programas de Python y Node se ejecutaron contra un dispositivo serie **simulado** (sin hardware real); el de C# se compiló. Nada se ha probado contra un dispositivo físico: el entorno de pruebas no tiene puertos ni USB. Referencia: [pyserial](https://pyserial.readthedocs.io/), [kernel.org: dispositivos](https://www.kernel.org/doc/html/latest/admin-guide/devices.html).*

## 1. Tu situación: una máquina virtual

Tu informe muestra `virtualbox-guest-utils` y un «VirtualBox USB Tablet» como único dispositivo USB. Eso implica:

| Qué quieres controlar | Dónde está el hardware | Qué hacer |
|---|---|---|
| Información de la propia máquina (CPU, RAM, discos) | La VM la ve (virtualizada) | Funciona en la VM; los sensores de temperatura y batería **no existen** |
| Un dispositivo USB o serie conectado a tu PC | El PC físico (anfitrión) | **Opción A:** reenviarlo a la VM con USB passthrough. **Opción B:** programar en el anfitrión (por ejemplo, C# con Visual Studio) |
| Los sensores reales del PC (temperatura, ventiladores) | El anfitrión | Programar en el anfitrión |

> **Regla:** para aprender, empieza con el **dispositivo simulado** de este módulo (no necesitas hardware) y después pasa a un dispositivo real, preferiblemente un adaptador USB-serie barato o una placa tipo Arduino (comunican por puerto serie).

### USB passthrough en VirtualBox

Resumen (confirma los nombres de los menús en la documentación de VirtualBox de tu versión):

1. En el anfitrión, instala el *Extension Pack* de VirtualBox para USB 2.0/3.0.
2. En un anfitrión Linux, añade tu usuario al grupo `vboxusers`.
3. Con la VM encendida: *Dispositivos → USB →* elige el dispositivo, o crea un **filtro USB** en la configuración de la VM para que se conecte siempre.
4. Mientras está conectado a la VM, el anfitrión **no** lo ve, y al revés.
5. En la VM comprueba con `lsusb` y `ls /dev/ttyUSB* /dev/ttyACM*`.

## 2. Leer información del equipo

Sin hardware extra, desde la terminal de Linux:

```bash
lscpu
lsblk -f
free -h
df -h
lsusb -t
ip -br link
sudo dmidecode -t system
journalctl -k -n 30 --no-pager
```

| Comando | Significado |
|---|---|
| `lscpu` | CPU: modelo, núcleos, frecuencias, virtualización |
| `lsblk -f` | Discos y particiones; `-f` añade sistema de archivos, etiqueta y UUID |
| `free -h` | Memoria; `-h` legible |
| `df -h` | Espacio por sistema de archivos |
| `lsusb -t` | Dispositivos USB en árbol (`-v` muy detallado; `-d vid:pid` filtra) |
| `ip -br link` | Interfaces de red; `-br` breve |
| `sudo dmidecode -t system` | Fabricante y modelo desde la BIOS (necesita administrador) |
| `journalctl -k` | Mensajes del núcleo (`-k` = *kernel*): aquí aparece cuando enchufas un dispositivo |
| `lspci` | Dispositivos PCI (tarjeta gráfica, red); viene en el paquete `pciutils` |
| `sensors` | Temperaturas y ventiladores (paquete `lm-sensors`; no funciona en máquinas virtuales) |

Archivos del núcleo que exponen datos como texto:

| Ruta | Contenido |
|---|---|
| `/proc/cpuinfo`, `/proc/meminfo` | CPU y memoria |
| `/sys/class/thermal/thermal_zone*/temp` | Temperatura en milésimas de grado |
| `/sys/class/power_supply/*/capacity` | Carga de batería |
| `/sys/class/net/*/` | Estado de las interfaces de red |
| `/sys/bus/usb/devices/` | Dispositivos USB |

Con Python, la librería `psutil` unifica esto en Linux, Windows y macOS:

```python
"""Lee información del hardware sin necesitar ningún dispositivo externo."""

import platform
from pathlib import Path

import psutil


def read_text(path: str) -> str | None:
    """Lee un archivo de /sys o /proc; devuelve None si no existe (depende del equipo)."""
    file = Path(path)
    return file.read_text().strip() if file.exists() else None


def main() -> None:
    print(f"Sistema: {platform.system()} {platform.release()} ({platform.machine()})")
    print(
        f"CPU: {psutil.cpu_count(logical=False)} núcleos físicos, {psutil.cpu_count()} lógicos"
    )
    print(f"Uso de CPU (1 s): {psutil.cpu_percent(interval=1)} %")

    memory = psutil.virtual_memory()
    print(f"RAM: {memory.total / 1024**3:.1f} GiB totales, {memory.percent} % en uso")

    for partition in psutil.disk_partitions(all=False)[:3]:
        usage = psutil.disk_usage(partition.mountpoint)
        print(
            f"Disco {partition.mountpoint}: {usage.percent} % de {usage.total / 1024**3:.0f} GiB"
        )

    # Los sensores solo existen si el hardware y el sistema los exponen
    temperatures = (
        psutil.sensors_temperatures() if hasattr(psutil, "sensors_temperatures") else {}
    )
    print(
        "Temperaturas:",
        temperatures or "no disponibles (normal en una máquina virtual)",
    )
    battery = psutil.sensors_battery() if hasattr(psutil, "sensors_battery") else None
    print("Batería:", f"{battery.percent:.0f} %" if battery else "no hay")

    print(
        "Modelo de CPU:",
        read_text("/sys/devices/virtual/dmi/id/product_name") or "no disponible",
    )


if __name__ == "__main__":
    main()
```

```bash
cd ~/projects && uv init hw-py && cd hw-py
uv add psutil pyserial
uv run python src/system_info.py
```

Otros lenguajes: `os` y `os.cpus()` en Node; `System.Management` (WMI) y `Environment` en C# sobre Windows; `gopsutil` en Go; el crate `sysinfo` en Rust.

> **Ojo:** muchos sensores solo existen en hardware real y con permisos. Tu programa debe tratar «no disponible» como un resultado normal, no como un error.

## 3. Cómo ve Linux los dispositivos

| Concepto | Significado |
|---|---|
| **Archivo de dispositivo** | Entrada en `/dev` que representa un dispositivo: `/dev/ttyUSB0`, `/dev/ttyACM0`, `/dev/sda` |
| `ttyUSB*` | Adaptadores USB-serie (chips FTDI, CH340, CP210x) |
| `ttyACM*` | Dispositivos USB de clase módem (muchos Arduino, placas modernas) |
| `ttyS*` | Puertos serie «de verdad» de la placa (en tu VM aparece `/dev/ttyS0`) |
| **Permisos** | El archivo pertenece al grupo `dialout`; para usarlo sin `sudo` tu usuario debe estar en él |
| **udev** | El gestor de dispositivos: crea los archivos de `/dev` al enchufar y aplica reglas |

Tu usuario **no** está en `dialout` (tu informe lo muestra). Para añadirlo:

```bash
sudo usermod -aG dialout dev26
id -nG
```

| Pieza | Significado |
|---|---|
| `usermod` | Modifica un usuario |
| `-a` | **Añadir** a grupos (sin esto, `-G` reemplaza los grupos y podrías perder `sudo`) |
| `-G dialout` | Grupo a añadir |
| `dev26` | Tu usuario |

Cierra la sesión y vuelve a entrar para que se aplique.

Seguir lo que ocurre al enchufar un dispositivo:

```bash
sudo udevadm monitor --udev
journalctl -k -f
ls -l /dev/ttyUSB* /dev/ttyACM*
udevadm info -a -n /dev/ttyUSB0
```

| Comando | Significado |
|---|---|
| `udevadm monitor --udev` | Muestra eventos de dispositivos en vivo |
| `journalctl -k -f` | Mensajes del núcleo en vivo (`-f` sigue) |
| `ls -l /dev/ttyUSB*` | Dueño, grupo y permisos del puerto |
| `udevadm info -a -n RUTA` | Atributos del dispositivo (fabricante, producto): sirven para escribir reglas |

> **Ojo:** el servicio **`ModemManager`** (activo en tu equipo) puede abrir los dispositivos serie recién conectados para comprobar si son un módem, y bloquear o confundir al tuyo. Si ves que tu puerto «se ocupa solo», deshabilítalo (`sudo systemctl disable --now ModemManager`) o marca el dispositivo para que lo ignore con una regla udev. Valora si lo necesitas antes de desactivarlo.

Regla udev de ejemplo (`/etc/udev/rules.d/99-mi-dispositivo.rules`; `idVendor` e `idProduct` salen de `lsusb`):

```text
# Un dispositivo concreto: accesible al grupo plugdev y ModemManager lo ignora
SUBSYSTEM=="tty", ATTRS{idVendor}=="1a86", ATTRS{idProduct}=="7523", MODE="0660", GROUP="plugdev", ENV{ID_MM_DEVICE_IGNORE}="1"
```

```bash
sudo udevadm control --reload-rules && sudo udevadm trigger
```

## 4. Puerto serie: conceptos

Un puerto serie envía **bytes uno detrás de otro** por un par de hilos. Ambos extremos deben acordar los parámetros:

| Parámetro | Significado | Habitual |
|---|---|---|
| **Baudios** (*baud rate*) | Velocidad en símbolos por segundo | 9600, 115200 |
| **Bits de datos** | Bits por byte | 8 |
| **Paridad** | Comprobación de errores | Ninguna (`N`) |
| **Bits de parada** | Marca el fin del byte | 1 |
| **Control de flujo** | Evitar desbordamientos | Ninguno |

«8N1» = 8 bits de datos, sin paridad, 1 bit de parada. Si los parámetros no coinciden, se recibe basura.

Probar a mano desde la terminal:

```bash
stty -F /dev/ttyUSB0 9600 cs8 -cstopb -parenb raw -echo
cat /dev/ttyUSB0
echo "PING" > /dev/ttyUSB0
picocom -b 9600 /dev/ttyUSB0
screen /dev/ttyUSB0 9600
```

| Pieza | Significado |
|---|---|
| `stty -F ruta` | Configura el puerto (`-F` = archivo) |
| `9600` | Baudios |
| `cs8` | 8 bits de datos |
| `-cstopb` | 1 bit de parada (`cstopb` serían 2) |
| `-parenb` | Sin paridad |
| `raw` | Modo «crudo»: sin interpretar caracteres de control |
| `-echo` | Sin eco local |
| `picocom -b 9600 ruta` | Terminal serie interactiva (salir: `Ctrl+A` y luego `Ctrl+X`) |
| `screen ruta 9600` | Alternativa (salir: `Ctrl+A` y luego `K`) |

## 5. Diseñar un protocolo

Una línea de bytes no dice dónde empieza ni termina un mensaje: **tú defines el protocolo**.

**Protocolo de texto** (fácil de depurar con un terminal): una orden por línea, terminada en `\n`.

```text
PING     -> PONG
TEMP?    -> TEMP 23.5
LED ON   -> OK
LED OFF  -> OK
otra     -> ERR
```

**Protocolo binario** (compacto y rápido): tramas con cabecera, longitud y comprobación de errores.

```text
+------+----------+-----------------+----------+
| 0xAA | longitud |  carga útil     | checksum |
| 1 B  |   1 B    | `longitud` bytes|   1 B    |
+------+----------+-----------------+----------+
```

```python
"""Tramas binarias: cabecera + longitud + carga útil + suma de comprobación.

    +------+----------+-----------------+----------+
    | 0xAA | longitud |  carga útil     | checksum |
    | 1 B  |   1 B    | `longitud` bytes|   1 B    |
    +------+----------+-----------------+----------+
checksum = XOR de todos los bytes de la carga útil.
"""

import struct

START = 0xAA


class FrameError(ValueError):
    """La trama recibida es inválida."""


def checksum(payload: bytes) -> int:
    result = 0
    for byte in payload:
        result ^= byte
    return result


def encode(payload: bytes) -> bytes:
    if len(payload) > 255:
        raise FrameError("La carga útil no puede superar 255 bytes")
    return bytes([START, len(payload)]) + payload + bytes([checksum(payload)])


def decode(frame: bytes) -> bytes:
    if len(frame) < 3 or frame[0] != START:
        raise FrameError("Cabecera inválida")
    length = frame[1]
    if len(frame) != length + 3:
        raise FrameError("Longitud incorrecta")
    payload = frame[2 : 2 + length]
    if frame[-1] != checksum(payload):
        raise FrameError("Suma de comprobación incorrecta")
    return payload


def encode_reading(sensor_id: int, millicelsius: int) -> bytes:
    """Ejemplo: lectura de un sensor = id (1 byte) + temperatura (entero de 4 bytes con signo, little-endian)."""
    return encode(struct.pack("<Bi", sensor_id, millicelsius))


def decode_reading(frame: bytes) -> tuple[int, float]:
    sensor_id, millicelsius = struct.unpack("<Bi", decode(frame))
    return sensor_id, millicelsius / 1000
```

```python
import pytest

from frames import FrameError, decode, decode_reading, encode, encode_reading


def test_roundtrip() -> None:
    assert decode(encode(b"hola")) == b"hola"


def test_known_frame() -> None:
    assert encode(b"\x01\x02").hex(" ") == "aa 02 01 02 03"


def test_reading() -> None:
    frame = encode_reading(7, 23_500)
    assert decode_reading(frame) == (7, 23.5)


def test_corrupted_frame_is_rejected() -> None:
    frame = bytearray(encode(b"hola"))
    frame[3] ^= 0xFF  # un bit cambiado en el camino
    with pytest.raises(FrameError):
        decode(bytes(frame))
```

| Concepto | Para qué |
|---|---|
| Cabecera fija (`0xAA`) | Resincronizar si se pierde un byte |
| Longitud | Saber cuántos bytes leer |
| *Checksum* (XOR, suma, CRC) | Detectar corrupción en el camino |
| `struct.pack("<Bi", …)` | Empaquetar números: `<` little-endian, `B` byte sin signo, `i` entero de 4 bytes con signo (ver [CLI, streams y binarios](#cli-args-streams)) |

> **Regla:** trata todo lo que llegue del dispositivo como **entrada no confiable**: valida longitud, rango y checksum, y limita el tiempo de espera.

## 6. Un dispositivo simulado

Para practicar sin hardware, un **pseudo-terminal (pty)** actúa como un cable serie virtual: lo que escribes en un extremo sale por el otro.

```python
"""Demostración sin hardware: un dispositivo SIMULADO en un pseudo-terminal (pty) y el cliente real.

Un pty es un par de extremos conectados: lo que se escribe en uno sale por el otro,
igual que un cable serie. El dispositivo simulado usa un extremo; pyserial abre el otro.
"""

import os
import threading

from serial_client import Device


def simulate_device(master_fd: int) -> None:
    """Responde a los comandos del protocolo, como lo haría el firmware de un microcontrolador."""
    buffer = b""
    while True:
        try:
            chunk = os.read(master_fd, 64)
        except OSError:
            return
        buffer += chunk
        while b"\n" in buffer:
            line, buffer = buffer.split(b"\n", 1)
            command = line.decode("ascii").strip()
            if command == "PING":
                reply = "PONG"
            elif command == "TEMP?":
                reply = "TEMP 23.5"
            elif command in ("LED ON", "LED OFF"):
                reply = "OK"
            else:
                reply = "ERR"
            os.write(master_fd, f"{reply}\n".encode("ascii"))


def main() -> None:
    master_fd, slave_fd = os.openpty()
    port = os.ttyname(slave_fd)  # algo como /dev/pts/3
    threading.Thread(target=simulate_device, args=(master_fd,), daemon=True).start()

    with Device(port) as device:
        print("puerto simulado:", port)
        print(device.command("PING"))
        print(device.command("TEMP?"))
        print(device.command("LED ON"))
        print(device.command("BORRAR"))


if __name__ == "__main__":
    main()
```

Y una versión que se queda en marcha para conectarle clientes escritos en cualquier lenguaje:

```python
"""Dispositivo serie SIMULADO que se queda en marcha: imprime la ruta del puerto y responde a comandos.

Úsalo para probar clientes (Python, Node, C#...) sin hardware real:
    uv run python src/device_sim.py
"""

import os
import threading
import time

from serial_demo import simulate_device


def main() -> None:
    master_fd, slave_fd = os.openpty()
    print("puerto simulado:", os.ttyname(slave_fd), flush=True)
    threading.Thread(target=simulate_device, args=(master_fd,), daemon=True).start()
    while True:
        time.sleep(1)


if __name__ == "__main__":
    main()
```

```bash
cd src
uv run python device_sim.py
# imprime: puerto simulado: /dev/pts/0
```

## 7. Clientes en varios lenguajes

### Python (pyserial)

```python
"""Cliente de un dispositivo por puerto serie con un protocolo de líneas de texto.

Protocolo (cada mensaje termina en \\n):
    PING    -> PONG
    TEMP?   -> TEMP 23.5
    LED ON  -> OK       LED OFF -> OK
    otra cosa -> ERR
"""

import argparse
from typing import Self

import serial  # paquete pyserial


class Device:
    def __init__(self, port: str, baudrate: int = 9600, timeout: float = 1.0) -> None:
        # timeout: segundos máximos de espera al leer; sin él, readline() podría bloquearse siempre
        self._serial = serial.Serial(port, baudrate=baudrate, timeout=timeout)

    def command(self, text: str) -> str:
        self._serial.write(
            f"{text}\n".encode("ascii")
        )  # los puertos serie envían BYTES, no texto
        line = self._serial.readline()  # lee hasta \n o hasta que venza el timeout
        if not line:
            raise TimeoutError(f"El dispositivo no respondió a {text!r}")
        return line.decode("ascii").strip()

    def close(self) -> None:
        self._serial.close()

    def __enter__(self) -> Self:
        return self

    def __exit__(self, *_: object) -> None:
        self.close()


def main() -> None:
    parser = argparse.ArgumentParser(description="Cliente serie de ejemplo")
    parser.add_argument("--port", required=True, help="por ejemplo /dev/ttyUSB0 o COM3")
    parser.add_argument("--baud", type=int, default=9600)
    args = parser.parse_args()

    with Device(args.port, args.baud) as device:
        print(device.command("PING"))
        print(device.command("TEMP?"))
        print(device.command("LED ON"))


if __name__ == "__main__":
    main()
```

```bash
uv run python src/serial_client.py --port /dev/pts/0
uv run python -m serial.tools.list_ports -v
```

| Pieza | Significado |
|---|---|
| `serial.Serial(port, baudrate, timeout)` | Abre el puerto; **siempre** con `timeout` |
| `.write(bytes)` | Envía bytes (texto → `.encode("ascii")`) |
| `.readline()` | Lee hasta `\n` o hasta que venza el `timeout` |
| `.read(n)` | Lee hasta `n` bytes |
| `.in_waiting` | Bytes pendientes de leer |
| `list_ports` | Lista los puertos detectados |
| `with … as device` | Cierra el puerto aunque haya un error |

### Node.js (serialport)

```bash
pnpm add serialport
```

```js
// client.mjs: node client.mjs /dev/ttyUSB0 9600
import { ReadlineParser, SerialPort } from 'serialport';

const [path, baudRate = '9600'] = process.argv.slice(2);
if (!path) {
  console.error('Uso: node client.mjs RUTA_DEL_PUERTO [BAUDIOS]');
  process.exit(2);
}

const port = new SerialPort({ path, baudRate: Number(baudRate) });
const parser = port.pipe(new ReadlineParser({ delimiter: '\n' })); // convierte bytes en líneas

port.on('error', (error) => {
  console.error('Error del puerto:', error.message);
  process.exit(1);
});

parser.on('data', (line) => {
  console.log('<-', line.trim());   // cada línea que llega del dispositivo
});

port.on('open', () => {
  for (const command of ['PING', 'TEMP?', 'LED ON']) {
    port.write(`${command}\n`);    // los comandos terminan en \n
  }
  setTimeout(() => port.close(), 500);
});
```

### C# (System.IO.Ports)

```bash
dotnet add package System.IO.Ports
```

```csharp
using System.IO.Ports;

// Uso: dotnet run -- /dev/ttyUSB0 9600   (en Windows: COM3)
string portName = args.Length > 0 ? args[0] : SerialPort.GetPortNames().FirstOrDefault() ?? "";
int baudRate = args.Length > 1 ? int.Parse(args[1]) : 9600;

if (portName == "")
{
    Console.Error.WriteLine("No hay puertos serie. Puertos detectados: " + string.Join(", ", SerialPort.GetPortNames()));
    return 1;
}

// using: cierra el puerto al terminar, incluso si hay una excepción
using var port = new SerialPort(portName, baudRate)
{
    ReadTimeout = 1000,     // ms máximos de espera al leer
    WriteTimeout = 1000,
    NewLine = "\n",         // terminador de línea del protocolo
};

try
{
    port.Open();
    port.WriteLine("PING");                  // envía "PING\n"
    Console.WriteLine(port.ReadLine());      // espera una línea completa
    return 0;
}
catch (UnauthorizedAccessException)
{
    Console.Error.WriteLine("Sin permiso: en Linux añade tu usuario al grupo dialout; en Windows cierra otros programas que usen el puerto.");
    return 2;
}
catch (TimeoutException)
{
    Console.Error.WriteLine("El dispositivo no respondió a tiempo.");
    return 3;
}
```

En Windows los puertos se llaman `COM3`; en Linux, `/dev/ttyUSB0`. `SerialPort.GetPortNames()` los lista. Visual Studio es cómodo para depurar: puntos de interrupción mientras el dispositivo responde.

### Go y Rust

Go: `go.bug.st/serial`. Rust: el crate `serialport`. Ambos siguen el mismo patrón: abrir con velocidad y *timeout*, escribir bytes, leer hasta un delimitador o una longitud.

## 8. USB y dispositivos HID

Más allá del puerto serie, un dispositivo USB puede hablar con protocolos propios:

| Tecnología | Cuándo | Biblioteca |
|---|---|---|
| USB-serie (CDC, FTDI, CH340) | Arduino, adaptadores | pyserial, serialport |
| **HID** (teclados, ratones, mandos, lectores, lectores de códigos) | Dispositivos de interfaz humana y muchos lectores | `hidapi` (`hid`), `node-hid` |
| USB genérico (libusb) | Protocolos propios del fabricante | `pyusb`, `libusb`, el crate `rusb` |
| Impresoras, escáneres | Servicios del sistema | CUPS / SANE |

Para acceder sin `sudo` hace falta una **regla udev** con el identificador del dispositivo (`idVendor`, `idProduct`) como la de arriba. Averigua los identificadores con `lsusb`.

## 9. Desde el navegador (Web Serial)

Los navegadores basados en Chromium pueden hablar con un puerto serie **conectado al equipo donde se ejecuta el navegador**, con permiso explícito del usuario y solo en páginas servidas por HTTPS o `localhost`.

```html
<button id="connect" type="button">Conectar</button>
<pre id="log"></pre>
<script type="module">
  const log = document.querySelector('#log');

  document.querySelector('#connect').addEventListener('click', async () => {
    // Debe ejecutarse desde un gesto del usuario (clic); abre el selector del navegador
    const port = await navigator.serial.requestPort();
    await port.open({ baudRate: 9600 });

    const writer = port.writable.getWriter();
    await writer.write(new TextEncoder().encode('PING\n'));
    writer.releaseLock();

    const reader = port.readable.pipeThrough(new TextDecoderStream()).getReader();
    const { value } = await reader.read();
    log.textContent += value;
    reader.releaseLock();
  });
</script>
```

*Este ejemplo no se pudo probar sin hardware.* Comprueba la compatibilidad en [MDN](https://developer.mozilla.org/es/docs/Web/API/Web_Serial_API): no existe en todos los navegadores. Si la página la abres desde tu PC pero el dispositivo está en la VM, no lo verá: el navegador solo accede a los dispositivos de su propio equipo.

## 10. Seguridad y buenas prácticas

- **Nunca** escribas en archivos de bloque (`/dev/sda`, `/dev/nvme0n1`) sin estar seguro: `dd of=/dev/sdX` o `cat > /dev/sdX` **destruye un disco**.
- No ejecutes tus programas como administrador por comodidad: da permisos al grupo correcto.
- Un dispositivo conectado es una entrada externa: valida, limita tamaños y tiempos.
- Cuida el hardware: voltajes, polaridades y corrientes de los dispositivos reales (una placa de 3,3 V no admite 5 V).
- Un programa que controla algo físico (motores, relés, calor) necesita un **estado seguro por defecto** y tiempo máximo de actuación.
- Abre el puerto con *timeout*, ciérralo siempre (`with`, `using`, `finally`) y gestiona la desconexión del dispositivo.

## 11. Errores habituales

| Mensaje o síntoma | Causa | Solución |
|---|---|---|
| `PermissionError: [Errno 13] Permission denied: '/dev/ttyUSB0'` | No estás en `dialout` | `sudo usermod -aG dialout $USER` y reiniciar sesión |
| `No such file or directory: '/dev/ttyUSB0'` | El dispositivo no está (o llegó como `ttyACM0`) | `ls /dev/tty*`, `journalctl -k`, `lsusb` |
| `device or resource busy` | Otro programa tiene el puerto (o `ModemManager`) | `sudo fuser /dev/ttyUSB0`; cierra el otro programa |
| Se recibe basura | Baudios o parámetros distintos | Alinea velocidad y 8N1 en los dos extremos |
| `readline()` devuelve vacío | El dispositivo no responde o el `timeout` venció | Comprueba el cable, el terminador (`\n` o `\r\n`) y el protocolo |
| El dispositivo no aparece en la VM | Sin passthrough | Dispositivos → USB en VirtualBox; Extension Pack |
| `UnauthorizedAccessException` en C# | Sin permisos o puerto ocupado | Grupo `dialout` / cierra otros programas |
| Los datos llegan partidos | Un `read` no entrega mensajes completos | Acumula en un búfer y busca el delimitador |
| `SerialPort.GetPortNames()` vacío | No hay puertos visibles | Revisa el controlador (Windows) o `udev` (Linux) |

## 12. Ejercicios

> **Ejercicio:** ejecuta `system_info.py` en tu VM y en tu anfitrión (si puedes). ¿Qué sensores aparecen en cada uno y por qué?

> **Ejercicio:** arranca `device_sim.py` y conecta un cliente en Node y otro en Python al mismo tiempo. ¿Qué ocurre? ¿Por qué solo uno debe abrir un puerto a la vez?

> **Ejercicio:** amplía el protocolo del simulador con `LED?` (devuelve `LED ON` o `LED OFF` según el estado) y añade su prueba.

> **Ejercicio:** envía tramas binarias de `frames.py` por el simulador y comprueba que una trama corrupta se rechaza.

> **Ejercicio:** consigue un adaptador USB-serie o una placa Arduino, conéctalo a la VM con USB passthrough, añádete a `dialout` y haz que tu cliente lea una línea real.

## 13. Documentación oficial

- [pyserial](https://pyserial.readthedocs.io/)
- [psutil](https://psutil.readthedocs.io/)
- [serialport (Node)](https://serialport.io/)
- [System.IO.Ports](https://learn.microsoft.com/es-es/dotnet/api/system.io.ports.serialport)
- [Web Serial API (MDN)](https://developer.mozilla.org/es/docs/Web/API/Web_Serial_API)
- [VirtualBox: USB](https://www.virtualbox.org/manual/ch03.html#usb-support)
- [udev](https://wiki.archlinux.org/title/Udev)
