# Honeypots: señuelos para detectar intrusos

Un **honeypot** es un servicio señuelo que no ofrece nada real y que **nadie legítimo tiene motivo para usar**. Cualquier conexión que reciba es sospechosa por definición. Es la versión defensiva de «hacerles caer»: no entras en ningún sistema ajeno; **observas y registras a quien llama a tu puerta**.

*Verificado: 2026-10-27 en parte. El señuelo en Python y su informe se ejecutaron en local y tienen pruebas que pasan. Los comandos para software de terceros (Cowrie, Canarytokens) siguen su documentación y no se han ejecutado aquí. Referencia: [Cowrie](https://docs.cowrie.org/), [Canarytokens](https://canarytokens.org/).*

> **Esto es lo que sustituye al contraataque.** Un honeypot no ataca, no rastrea, no entra en el equipo del visitante ni le envía nada dañino. Solo te dice **qué intentan contra ti** (para bloquear y denunciar) y, dentro de tu red, **si algo está comprometido** (porque se pone a escanear). Ver [Marco ético](#seguridad-marco).

## 1. Dos usos distintos

| Uso | Dónde | Qué te dice |
|---|---|---|
| **Señuelo expuesto a Internet** | Una VM aislada con un puerto abierto hacia fuera | Qué escaneos y ataques automáticos recibe una IP pública; qué credenciales y órdenes prueban. Sirve para **aprender** y para alimentar tu lista de bloqueo |
| **Alarma interna** (*tripwire*) | En tu red local, un servicio señuelo que nadie debería tocar | Si un dispositivo de tu LAN empieza a escanear o conectar con él, **está comprometido**. Muy pocas falsas alarmas, muy alto valor |

El segundo uso es el más práctico para un entorno doméstico.

## 2. Tipos

| Tipo | Qué hace | Riesgo | Ejemplos |
|---|---|---|---|
| **Baja interacción** | Simula banners y responde poco | Mínimo | El script de este módulo, OpenCanary |
| **Media interacción** | Emula un servicio con suficiente detalle para recibir órdenes | Bajo | Cowrie (SSH/Telnet), Dionaea |
| **Alta interacción** | Un sistema real sin protección para ser atacado | **Alto**: puede usarse como trampolín | Solo expertos y entornos muy controlados. Evítalo |
| **Fichas trampa** (*canary tokens*) | Un archivo, enlace o credencial falsos que avisan al abrirse | Mínimo | Canarytokens |

## 3. Reglas de seguridad (léelas antes de desplegar nada)

1. **Aislamiento total.** El señuelo va en una VM con red **separada**: sin acceso a tu LAN ni a tus otras máquinas. Si lo comprometen, no debe haber a dónde saltar.
2. **Sin salida a Internet.** Bloquea el tráfico saliente de esa VM (`ufw default deny outgoing`), salvo lo imprescindible. Un señuelo comprometido no debe poder atacar a terceros.
3. **Nada real dentro**: ni claves, ni datos, ni credenciales que uses en otro sitio.
4. **Mínimo privilegio**: el proceso corre con un usuario sin permisos; puertos altos (>1024); en un contenedor si es posible.
5. **Instantánea** de la VM limpia para restaurar.
6. **Sin gancho.** Un honeypot no «invita» ni anuncia nada: simplemente existe.
7. **Responsabilidad.** Un honeypot expuesto atrae tráfico hostil hacia tu conexión: avisa a quien comparta tu red y revisa las condiciones de tu proveedor de Internet.
8. **Datos personales.** Una dirección IP es un dato personal en la Unión Europea. Guarda solo lo necesario, para un fin concreto (seguridad), durante un tiempo limitado, y no publiques listados con IP.

## 4. Un señuelo propio

Un servicio TCP que envía un banner de SSH falso y **registra** cada visita en un archivo JSON (una línea por visita). No ejecuta nada de lo que recibe.

```python
"""Señuelo TCP de baja interacción: escucha en un puerto, envía un banner falso y REGISTRA quién conecta.

No ejecuta nada de lo que recibe, no responde más allá del banner y cierra pronto la conexión.
Pensado para una VM AISLADA de laboratorio, sin datos reales, con una red que no alcance al resto de tus equipos.

Uso:
    python honeypot.py --port 2222 --log honeypot.jsonl
"""

import argparse
import asyncio
import json
import time
from pathlib import Path

BANNER = b"SSH-2.0-OpenSSH_9.6p1 Ubuntu-3ubuntu13\r\n"
MAX_BYTES = 512  # lo máximo que se guarda de lo que envía el visitante
READ_TIMEOUT = 5.0  # segundos que se espera a que el visitante envíe algo
MAX_CONNECTIONS = 50  # evita que el señuelo se convierta en un problema de recursos


def make_record(peer: tuple[str, int], port: int, data: bytes, seconds: float) -> dict:
    """Crea el registro de una visita. Se guarda hexadecimal y texto seguro, nunca bytes sin tratar."""
    return {
        "time": time.strftime("%Y-%m-%dT%H:%M:%SZ", time.gmtime()),
        "source_ip": peer[0],
        "source_port": peer[1],
        "destination_port": port,
        "bytes_received": len(data),
        "first_bytes_hex": data[:64].hex(),
        "first_bytes_text": data[:64]
        .decode("ascii", errors="replace")
        .replace("\r", "\\r")
        .replace("\n", "\\n"),
        "duration_seconds": round(seconds, 3),
    }


async def handle(
    reader: asyncio.StreamReader,
    writer: asyncio.StreamWriter,
    port: int,
    log: Path,
    limit: asyncio.Semaphore,
) -> None:
    peer = writer.get_extra_info("peername")[:2]
    started = time.monotonic()
    data = b""
    async with limit:
        try:
            writer.write(BANNER)
            await writer.drain()
            data = await asyncio.wait_for(reader.read(MAX_BYTES), timeout=READ_TIMEOUT)
        except (TimeoutError, ConnectionError):
            pass
        finally:
            writer.close()
            with log.open("a", encoding="utf-8") as file:
                file.write(
                    json.dumps(
                        make_record(peer, port, data, time.monotonic() - started)
                    )
                    + "\n"
                )


async def main() -> None:
    parser = argparse.ArgumentParser(
        description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter
    )
    parser.add_argument("--host", default="0.0.0.0")
    parser.add_argument(
        "--port",
        type=int,
        default=2222,
        help="puerto alto (>1024) para no necesitar privilegios",
    )
    parser.add_argument("--log", type=Path, default=Path("honeypot.jsonl"))
    args = parser.parse_args()

    limit = asyncio.Semaphore(MAX_CONNECTIONS)
    server = await asyncio.start_server(
        lambda r, w: handle(r, w, args.port, args.log, limit), args.host, args.port
    )
    print(
        f"Señuelo escuchando en {args.host}:{args.port}; registro en {args.log}",
        flush=True,
    )
    async with server:
        await server.serve_forever()


if __name__ == "__main__":
    asyncio.run(main())
```

```bash
cd ~/projects && uv init hp-py && cd hp-py
uv run python src/honeypot.py --port 2222 --log honeypot.jsonl
```

| Parte | Significado |
|---|---|
| `asyncio.start_server` | Servidor TCP asíncrono: atiende muchas conexiones a la vez |
| `BANNER` | La primera línea que enviaría un SSH real; lo que ven los escáneres |
| `reader.read(MAX_BYTES)` + `wait_for(timeout)` | Lee un máximo de bytes y con límite de tiempo: no se puede colgar ni inundar |
| `Semaphore(MAX_CONNECTIONS)` | Limita las conexiones simultáneas |
| `make_record` | Guarda hexadecimal y texto con escapes: **nunca** se guarda ni se interpreta el contenido tal cual |
| `--host 127.0.0.1` | Para probar en local; `0.0.0.0` lo expone en todas las interfaces |
| Archivo `.jsonl` | Un objeto JSON por línea: fácil de procesar con cualquier herramienta |

Probarlo en local con `nc`:

```bash
printf 'GET / HTTP/1.1\r\n\r\n' | nc -q1 127.0.0.1 2299
```

```text
SSH-2.0-OpenSSH_9.6p1 Ubuntu-3ubuntu13
```

Registro real de una visita:

```json
{"time": "2026-10-27T16:57:51Z", "source_ip": "127.0.0.1", "source_port": 40964, "destination_port": 2299, "bytes_received": 18, "first_bytes_hex": "474554202f20485454502f312e310d0a0d0a", "first_bytes_text": "GET / HTTP/1.1\\r\\n\\r\\n", "duration_seconds": 0.001}
```

### El informe

```python
"""Resume el registro del señuelo: quién visita, cuánto y qué envía.

Uso:  python honeypot_report.py honeypot.jsonl
"""

import json
import sys
from collections import Counter
from pathlib import Path


def load(path: Path) -> list[dict]:
    return [
        json.loads(line)
        for line in path.read_text(encoding="utf-8").splitlines()
        if line.strip()
    ]


def summarize(records: list[dict]) -> str:
    by_ip = Counter(record["source_ip"] for record in records)
    payloads = Counter(
        record["first_bytes_text"] for record in records if record["bytes_received"]
    )
    lines = [
        f"Visitas totales: {len(records)}  |  IP distintas: {len(by_ip)}",
        "",
        "Orígenes más activos:",
    ]
    lines += [f"  {ip:40} {count:>4} visitas" for ip, count in by_ip.most_common(5)]
    lines += ["", "Lo que envían (primeros bytes):"]
    lines += [
        f"  {count:>4}x  {text!r}" for text, count in payloads.most_common(5)
    ] or ["  (nada)"]
    return "\n".join(lines)


if __name__ == "__main__":
    print(summarize(load(Path(sys.argv[1]))))
```

```bash
uv run python src/honeypot_report.py honeypot.jsonl
```

```text
Visitas totales: 5  |  IP distintas: 1

Orígenes más activos:
  127.0.0.1                                   5 visitas

Lo que envían (primeros bytes):
     3x  'USER root\r\n'
     1x  'GET / HTTP/1.1\r\n\r\n'
```

### Pruebas

```python
import json

from honeypot import make_record
from honeypot_report import summarize


def test_record_is_safe_and_complete() -> None:
    record = make_record(
        ("203.0.113.9", 4444), 2222, b"SSH-2.0-libssh\r\n\x00\xff", 0.25
    )
    assert record["source_ip"] == "203.0.113.9"
    assert record["bytes_received"] == 18
    assert "\\r\\n" in record["first_bytes_text"]
    json.dumps(record)  # se puede serializar tal cual


def test_summary_counts_visits() -> None:
    records = [
        make_record(("203.0.113.9", 1), 2222, b"GET / HTTP/1.1", 0.1),
        make_record(("203.0.113.9", 2), 2222, b"", 5.0),
        make_record(("198.51.100.7", 3), 2222, b"GET / HTTP/1.1", 0.1),
    ]
    text = summarize(records)
    assert "Visitas totales: 3" in text
    assert "IP distintas: 2" in text
```

## 5. Alertas

Un registro que nadie mira no sirve. Para la **alarma interna**, cualquier línea nueva es una alerta:

```bash
tail -F honeypot.jsonl | while read -r line; do
  echo "ALERTA señuelo: $line" | logger -t honeypot
done
```

| Pieza | Significado |
|---|---|
| `tail -F archivo` | Sigue el archivo aunque se rote (`-F`, a diferencia de `-f`, lo reabre) |
| `while read -r line` | Procesa cada línea nueva |
| `logger -t honeypot` | Escribe en el registro del sistema con la etiqueta `honeypot` (`journalctl -t honeypot`) |

Desde ahí puedes enviar un correo, una notificación al móvil (ntfy, Telegram) o un aviso en un canal. Si recibes alertas sin parar, es que el señuelo está expuesto a Internet; si es interno, **investiga cada una**.

Como servicio, para que arranque solo (`/etc/systemd/system/honeypot.service`):

```ini
[Unit]
Description=Señuelo TCP
After=network.target

[Service]
User=honeypot
WorkingDirectory=/opt/honeypot
ExecStart=/opt/honeypot/.venv/bin/python src/honeypot.py --port 2222 --log /var/log/honeypot/honeypot.jsonl
Restart=on-failure
NoNewPrivileges=true
ProtectSystem=strict
ReadWritePaths=/var/log/honeypot

[Install]
WantedBy=multi-user.target
```

```bash
sudo systemctl daemon-reload && sudo systemctl enable --now honeypot
```

Las líneas `NoNewPrivileges` y `ProtectSystem` limitan lo que puede hacer el proceso aunque lo comprometan.

## 6. Herramientas listas

| Herramienta | Qué ofrece | Notas |
|---|---|---|
| **Cowrie** | SSH y Telnet de media interacción: registra intentos de acceso, contraseñas probadas, órdenes ejecutadas en un sistema de archivos falso | `docker run -p 2222:2222 cowrie/cowrie` (comprueba la guía oficial); registros en JSON |
| **OpenCanary** | Muchos servicios señuelo (SSH, HTTP, FTP, bases de datos…) con alertas | Pensado para alarmas internas |
| **Canarytokens** | Archivos, enlaces, claves falsas que te avisan por correo cuando alguien las usa | Sin instalar nada; ideal para colocar un «secreto falso» en un repositorio o un `.env` señuelo |
| **T-Pot** | Plataforma completa con muchos señuelos y paneles | Pesada: necesita recursos y un aislamiento muy cuidadoso |

Un **archivo señuelo** (por ejemplo `passwords.xlsx` en una carpeta compartida) con un *canary token* avisa si alguien lo abre: es una alarma de bajo coste.

## 7. Qué aprender de los registros

| Observación | Lectura |
|---|---|
| Muchas conexiones de muchos orígenes distintos | Ruido de fondo de Internet: escaneos automáticos continuos |
| Las mismas pocas contraseñas de siempre (`admin`, `123456`, `root`) | Diccionarios de fuerza bruta: refuerza las tuyas |
| Orígenes en una red de proveedores de alojamiento | Máquinas alquiladas o comprometidas |
| Órdenes tras entrar en un señuelo de media interacción | Qué descargan o instalan (para reconocer tendencias, **sin ejecutarlo**) |
| Un equipo de **tu** red conectando al señuelo interno | Posible compromiso: aíslalo e investiga ([Endurecimiento](#hardening)) |

**Qué haces con ello:**

1. Añade las IP persistentes a tu lista de bloqueo (`ufw deny from …`, `fail2ban`).
2. Refuerza lo que ves que se intenta (contraseñas, versiones antiguas, puertos).
3. Si detectas origen malicioso persistente, **denuncia** al proveedor con los registros ([Endurecimiento](#hardening)).
4. Nunca ejecutes, abras ni «analices» en tu máquina real los archivos que el atacante descargue: usa una VM de laboratorio sin red.

## 8. Errores habituales

| Error | Consecuencia | Correcto |
|---|---|---|
| Poner el señuelo en la misma red que tus equipos | Si lo comprometen, llegan a todo | Red separada, sin rutas hacia tu LAN |
| Dejar la salida a Internet libre | Puede atacar a terceros desde tu IP | `default deny outgoing` |
| Reutilizar contraseñas reales en el señuelo | Las regalas | Credenciales de usar y tirar |
| Ejecutar como `root` | Un fallo da control total | Usuario sin privilegios, `NoNewPrivileges` |
| Guardar el contenido sin tratar | Inyección en tus herramientas de análisis | Hex y escapes, como `make_record` |
| No mirar nunca los registros | No sirve de nada | Alertas automáticas |
| Pretender «rastrear» al visitante | Ilegal y poco fiable | Registrar, bloquear y denunciar |
| Publicar listas con IP | Posible problema de privacidad y difamación | Uso interno, retención corta |

## 9. Ejercicios

> **Ejercicio:** arranca el señuelo en una VM interna, conéctate desde otra VM con `nc` y comprueba que el informe lo refleja. Después lanza un `nmap -sT` contra él y observa cómo aparece en el registro.

> **Ejercicio:** añade al señuelo un segundo puerto con el banner de una base de datos falsa (por ejemplo MySQL o Redis) y registra el servicio al que llama cada visitante.

> **Ejercicio:** escribe una alerta que envíe una notificación (a un archivo, un correo o un servicio de avisos) cuando una IP de tu red privada conecte con el señuelo.

> **Ejercicio:** crea un *canary token* de tipo URL o de documento, ponlo en un lugar donde solo tú irías, ábrelo y comprueba el aviso.

> **Ejercicio:** redacta una política de uso de tu honeypot: qué recoge, para qué, cuánto tiempo se guarda y quién puede verlo.

## 10. Documentación oficial

- [Cowrie](https://docs.cowrie.org/en/latest/)
- [OpenCanary](https://opencanary.readthedocs.io/)
- [Canarytokens](https://docs.canarytokens.org/guide/)
- [The Honeynet Project](https://www.honeynet.org/)
- [INCIBE-CERT (España)](https://www.incibe.es/incibe-cert)
