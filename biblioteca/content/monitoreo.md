# Monitorización de red

Monitorizar es **observar** qué pasa en tu red para saber qué es normal y notar lo que no lo es. Se hace con capturas de paquetes, listas de conexiones y registros. Saber leer tráfico también te sirve para depurar tus propias APIs.

*Verificado: 2026-10-27. Los comandos y scripts se ejecutaron en un entorno Linux real capturando tráfico **local** (loopback). Las salidas son reales, recortadas. Referencia: [tcpdump](https://www.tcpdump.org/manpages/tcpdump.1.html), [Wireshark](https://www.wireshark.org/docs/).*

> **Regla:** captura **solo el tráfico de tu propia máquina y de tu propia red**. Capturar el tráfico de redes ajenas (Wi-Fi de una cafetería, un compañero, una empresa) sin autorización es ilegal en la mayoría de los países. Ver [Marco ético](#seguridad-marco).

## 1. Qué puedes ver y qué no

| Puedes ver | No puedes ver |
|---|---|
| Quién habla con quién: direcciones y puertos | El contenido de conexiones cifradas con TLS/SSH/WireGuard |
| Cuándo y cuántos datos (tamaño, frecuencia) | Páginas HTTPS concretas, solo el servidor (SNI) y el volumen |
| El contenido de protocolos **sin cifrar** (HTTP, DNS clásico, Telnet, FTP) | El tráfico de otros equipos en una red conmutada (solo el tuyo y el *broadcast*) |
| Fallos: reintentos, conexiones rechazadas, retrasos | |

Que casi todo vaya cifrado es **bueno**: protege a los usuarios. Para ti significa que la monitorización se apoya mucho en **metadatos** (quién, cuándo, cuánto).

## 2. Instantánea: conexiones y procesos

Antes de capturar paquetes, lo más barato es mirar el estado actual:

```bash
ss -ltnp
ss -tnp
sudo lsof -i -P -n
ip neigh
```

| Comando | Significado |
|---|---|
| `ss -ltnp` | Qué **escucha** y qué proceso (`-l` *listening*, `-t` TCP, `-n` numérico, `-p` proceso; `sudo` para ver todos) |
| `ss -tnp` | Conexiones **establecidas** ahora mismo con el proceso dueño |
| `ss -s` | Resumen de totales por estado |
| `sudo lsof -i -P -n` | Archivos de red abiertos: `-i` solo red, `-P` puertos numéricos, `-n` sin resolver nombres |
| `ip neigh` | Vecinos de la red local (tabla ARP): los equipos con los que has hablado recientemente |

**Qué buscar**: puertos escuchando que no reconoces, procesos inesperados con conexiones salientes, conexiones a direcciones raras, puertos de bases de datos en `0.0.0.0`.

### Vigilar cambios en lo que escucha

Un script que guarda una línea base y avisa de lo nuevo:

```python
"""Compara los puertos en escucha con una línea base y avisa de lo nuevo.

Primera ejecución:  guarda la línea base (lo que consideras normal).
Siguientes:         avisa de puertos nuevos y de los que han desaparecido.
Uso:  python listening_watch.py [--baseline baseline.json]
"""

import argparse
import json
from pathlib import Path

import psutil


def listening_ports() -> dict[str, str]:
    """Devuelve {'tcp:22': 'sshd', ...} de todo lo que escucha ahora mismo."""
    result: dict[str, str] = {}
    for connection in psutil.net_connections(kind="inet"):
        if connection.status != psutil.CONN_LISTEN or not connection.laddr:
            continue
        try:
            name = psutil.Process(connection.pid).name() if connection.pid else "?"
        except psutil.Error:
            name = "?"
        result[f"tcp:{connection.laddr.port}"] = name
    return dict(sorted(result.items()))


def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--baseline", type=Path, default=Path("baseline.json"))
    args = parser.parse_args()

    current = listening_ports()
    if not args.baseline.exists():
        args.baseline.write_text(json.dumps(current, indent=2))
        print(f"Línea base guardada con {len(current)} puertos en {args.baseline}")
        return

    known = json.loads(args.baseline.read_text())
    for port in sorted(current.keys() - known.keys()):
        print(f"NUEVO     {port} ({current[port]})")
    for port in sorted(known.keys() - current.keys()):
        print(f"DESAPARECE {port} ({known[port]})")
    if current.keys() == known.keys():
        print("Sin cambios respecto a la línea base")


if __name__ == "__main__":
    main()
```

```bash
cd ~/projects/mon-py && uv add psutil
uv run python src/listening_watch.py            # primera vez: guarda la línea base
uv run python src/listening_watch.py            # luego: avisa de cambios
```

Resultado real tras arrancar un servidor nuevo:

```text
NUEVO     tcp:8123 (python3)
```

Se programa con `cron` (ver [Recetario](#recetas)) y el aviso puede ir a un correo o a un mensaje. Es una forma sencilla de detectar «¿quién ha abierto un puerto?».

## 3. Capturar paquetes con `tcpdump`

```bash
sudo tcpdump -D
sudo tcpdump -i lo -nn -c 12 -w captura.pcap 'tcp port 8099'
tcpdump -nn -r captura.pcap
```

| Pieza | Significado |
|---|---|
| `sudo` | Capturar requiere privilegios (o permisos especiales del binario) |
| `-D` | Lista las interfaces disponibles |
| `-i lo` | Interfaz a escuchar (`any` = todas; `eth0`, `lo`…) |
| `-nn` | No resuelve nombres de host **ni** de puertos (más rápido y claro) |
| `-c 12` | Se detiene tras 12 paquetes |
| `-w captura.pcap` | Guarda los paquetes en un archivo (ábrelo después con Wireshark) |
| `-r captura.pcap` | Lee de un archivo en lugar de la red |
| `'tcp port 8099'` | **Filtro BPF**: solo ese tráfico |

Otras opciones útiles: `-A` muestra el contenido como texto ASCII, `-X` en hexadecimal y ASCII, `-s 0` captura el paquete entero, `-v`/`-vv` más detalle, `-l` sin buffer para encadenar con `|`.

### Filtros BPF más usados

| Filtro | Selecciona |
|---|---|
| `host 192.168.1.50` | Tráfico desde o hacia esa IP (`src host` / `dst host`) |
| `port 443` | Ese puerto en origen o destino (`src port`, `dst port`) |
| `tcp`, `udp`, `icmp` | Por protocolo |
| `net 192.168.1.0/24` | Una red |
| `tcp port 22 and not host 192.168.1.10` | Combinaciones con `and`, `or`, `not` |
| `tcp[tcpflags] & tcp-syn != 0` | Paquetes con el indicador SYN activo |

### Leer una captura real

Resumen de una petición HTTP local:

```text
127.0.0.1.47494 > 127.0.0.1.8099: Flags [S],  seq 2111901591, length 0       SYN
127.0.0.1.8099 > 127.0.0.1.47494: Flags [S.], seq 3847372488, ack ..., length 0   SYN-ACK
127.0.0.1.47494 > 127.0.0.1.8099: Flags [.],  ack 1, length 0                ACK
127.0.0.1.47494 > 127.0.0.1.8099: Flags [P.], seq 1:88, length 87            petición HTTP
127.0.0.1.8099 > 127.0.0.1.47494: Flags [P.], seq 1:187, length 186          cabeceras de respuesta
127.0.0.1.8099 > 127.0.0.1.47494: Flags [P.], seq 187:201, length 14         cuerpo: <h1>Hola</h1>
```

| Indicador (`Flags`) | Significado |
|---|---|
| `S` | SYN: iniciar conexión |
| `S.` | SYN + ACK |
| `.` | ACK |
| `P.` | PSH + ACK: hay datos |
| `F.` | FIN: cierre ordenado |
| `R` | RST: conexión rechazada o abortada |

Y con `-A` se ve el contenido de un protocolo sin cifrar:

```text
GET /index.html HTTP/1.1
Host: 127.0.0.1:8099
HTTP/1.0 200 OK
<h1>Hola</h1>
```

Por eso HTTP sin cifrar es inseguro: **cualquiera en el camino lo lee**. Mismo tráfico con TLS no deja ver nada de eso.

### Resumir una captura

```python
"""Resume una captura de tcpdump: quién habla con quién y cuántos paquetes/bytes.

Uso:
    tcpdump -nn -r captura.pcap | python top_talkers.py
    sudo tcpdump -nn -i eth0 -c 500 | python top_talkers.py
"""

import re
import sys
from collections import Counter

# Ejemplo de línea:  16:48:38.5 IP 127.0.0.1.47494 > 127.0.0.1.8099: Flags [P.], seq 1:88, ..., length 87
LINE = re.compile(
    r"IP6? (?P<src>[\w.:-]+?)(?:\.(?P<sport>\d+))? > (?P<dst>[\w.:-]+?)(?:\.(?P<dport>\d+))?: .*?length (?P<length>\d+)"
)


def summarize(lines: list[str]) -> tuple[Counter[str], Counter[str]]:
    packets: Counter[str] = Counter()
    volume: Counter[str] = Counter()
    for line in lines:
        match = LINE.search(line)
        if not match:
            continue
        flow = f"{match['src']} -> {match['dst']}:{match['dport'] or '?'}"
        packets[flow] += 1
        volume[flow] += int(match["length"])
    return packets, volume


def main() -> None:
    packets, volume = summarize(sys.stdin.read().splitlines())
    print(f"{'flujo':45} {'paquetes':>9} {'bytes':>8}")
    for flow, count in packets.most_common(10):
        print(f"{flow:45} {count:>9} {volume[flow]:>8}")


if __name__ == "__main__":
    main()
```

```bash
tcpdump -nn -r captura.pcap | uv run python src/top_talkers.py
```

```text
flujo                                          paquetes    bytes
127.0.0.1 -> 127.0.0.1:8099                           6       87
127.0.0.1 -> 127.0.0.1:47494                          6      200
```

Con un resumen así detectas volúmenes anómalos, destinos inesperados o un equipo que habla demasiado.

## 4. Wireshark y `tshark`

**Wireshark** es la interfaz gráfica para analizar capturas con comodidad; `tshark` es su versión de terminal.

```bash
sudo apt install wireshark tshark
tshark -r captura.pcap -Y 'http'
tshark -i eth0 -f 'tcp port 443' -a duration:10
```

| Pieza | Significado |
|---|---|
| `-r archivo` | Leer una captura |
| `-Y 'filtro'` | **Filtro de visualización** (sintaxis de Wireshark, distinta de BPF) |
| `-i` / `-f` | Interfaz / filtro de **captura** (BPF) |
| `-a duration:10` | Parar tras 10 segundos |

Filtros de visualización útiles: `http`, `dns`, `tcp.port == 8099`, `ip.addr == 192.168.1.50`, `tcp.flags.syn == 1 && tcp.flags.ack == 0`, `tls.handshake.type == 1` (saludos TLS) y `tcp.analysis.retransmission` (reintentos).

La instalación de Wireshark pregunta si permitir captura a usuarios no administradores: eso los añade al grupo `wireshark`. Valora si lo quieres; capturar como administrador es lo más seguro por defecto.

## 5. Ancho de banda y volumen

| Herramienta | Para qué |
|---|---|
| `iftop -n` | Conexiones con más tráfico en vivo, por equipo |
| `nethogs` | Tráfico por **proceso** |
| `vnstat` | Histórico de consumo por interfaz |
| `nload` | Gráfico simple en terminal |
| `ip -s link` | Contadores de bytes y errores por interfaz |

## 6. Registros del cortafuegos

Con `ufw` activo puedes registrar lo bloqueado:

```bash
sudo ufw logging medium
sudo journalctl -k --since "1 hour ago" --no-pager | grep "UFW BLOCK" | head
```

Cada línea `UFW BLOCK` indica origen (`SRC=`), destino, puerto (`DPT=`) y protocolo. Un mismo origen probando muchos puertos distintos es el patrón de un **escaneo de puertos**; muchos intentos al mismo puerto (22) es **fuerza bruta**. Ver [Endurecimiento y detección](#hardening).

## 7. Detección de intrusiones (IDS)

Un **IDS** analiza el tráfico automáticamente con reglas y firmas:

| Herramienta | Qué hace |
|---|---|
| **Suricata**, **Snort** | Reglas contra patrones de ataque conocidos; alertas |
| **Zeek** | Genera registros detallados de cada conexión, DNS, HTTP, TLS para analizar |
| **fail2ban** | Lee registros y bloquea orígenes que fallan repetidamente |

Se verán con la práctica en un laboratorio ([Laboratorio](#lab-etico)). Un IDS genera **falsos positivos**: afina las reglas con tu tráfico normal.

## 8. Método

1. **Línea base**: cuando todo está bien, guarda qué escucha, con quién habla y cuánto tráfico hay.
2. **Compara**: ante algo raro, ¿es nuevo respecto a la línea base?
3. **Captura acotada**: filtra por host y puerto; capturas cortas y a archivo.
4. **Cifrado**: si el tráfico va cifrado, mira metadatos y registros de la aplicación.
5. **Registra** lo que veas con fecha: te servirá para denunciar o para entender.

## 9. Errores habituales

| Síntoma | Causa | Solución |
|---|---|---|
| `You don't have permission to capture on that device` | Sin privilegios | `sudo`, o grupo `wireshark` |
| No se ve el tráfico de otros equipos | Red conmutada: solo ves el tuyo | Es lo normal; captura en el equipo que te interesa |
| Capturas enormes | Sin filtro ni límite | `-c`, `-G`/`-W` para rotar, filtro BPF |
| Paquetes truncados | `-s` pequeño por defecto en versiones antiguas | `-s 0` |
| Nombres que tardan en salir | Resolución DNS | `-nn` |
| `tcpdump: no suitable device found` | Interfaz incorrecta | `tcpdump -D` |
| Solo ves tráfico de `lo` | Tráfico local (VM ↔ servicios locales) | `-i any` o la interfaz de red |

## 10. Ejercicios

> **Ejercicio:** arranca tu API de tareas, captura con `tcpdump -i lo -nn -A 'tcp port 3000'` una petición `POST` y localiza en la salida el cuerpo JSON enviado y el código de estado devuelto.

> **Ejercicio:** repite con un servidor HTTPS local ([Redes](#redes)) y comprueba qué ves ahora del contenido. ¿Qué metadatos siguen visibles?

> **Ejercicio:** crea la línea base de puertos con `listening_watch.py`, arranca un servicio nuevo y comprueba el aviso. Prográmalo cada 5 minutos con `cron`.

> **Ejercicio:** abre una captura en Wireshark y aplica los filtros `tcp.flags.syn == 1 && tcp.flags.ack == 0` y `http`. ¿Cuántas conexiones nuevas hay?

> **Ejercicio:** con `ip neigh` y `ip -br a`, haz un inventario de los equipos de tu red local que conoces y compáralo en una semana.

## 11. Documentación oficial

- [tcpdump: página de manual](https://www.tcpdump.org/manpages/tcpdump.1.html)
- [Filtros BPF (pcap-filter)](https://www.tcpdump.org/manpages/pcap-filter.7.html)
- [Wireshark: guía del usuario](https://www.wireshark.org/docs/wsug_html_chunked/)
- [Suricata](https://docs.suricata.io/)
- [Zeek](https://docs.zeek.org/)
