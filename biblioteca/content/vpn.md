# VPN propia con WireGuard

Una **VPN** (red privada virtual) crea un túnel cifrado entre tus dispositivos, como si estuvieran en la misma red local aunque estén en sitios distintos. Con **WireGuard** puedes montar la tuya: un servidor (por ejemplo, tu VM) y tus clientes (tu PC, tu móvil).

*Verificado: 2026-10-27 en parte. El generador de configuración se ejecutó y sus archivos se validaron con `wg-quick strip`. **No se pudo levantar un túnel real**: el entorno de pruebas no tiene el módulo de WireGuard en el kernel. Los pasos de activación siguen la documentación oficial y debes probarlos en tu VM. Referencia: [wireguard.com](https://www.wireguard.com/quickstart/).*

## 1. Para qué sirve una VPN propia

| Objetivo | Cómo |
|---|---|
| Acceder a tu VM y a sus servicios (SSH, API, bases de datos) **sin publicarlos en Internet** | Los servicios escuchan solo en la IP de la VPN |
| Proteger tu tráfico en una Wi-Fi pública | Envías todo por el túnel a tu servidor (*full tunnel*) |
| Unir equipos en distintas redes en una sola red privada | Cada uno es un *peer* |
| Entender cómo funcionan los túneles y las claves | Es el mejor ejercicio práctico |

> **Ojo:** una VPN **no** te hace anónimo. Mueve tu punto de salida a tu servidor, que sí conoce tu tráfico. Cifra el tramo entre tu dispositivo y el servidor, no más allá.

## 2. Cómo funciona WireGuard

```text
   Tu PC (cliente)                                    Tu VM (servidor)
   IP VPN: 10.8.0.2        túnel UDP cifrado          IP VPN: 10.8.0.1
   clave privada A  <────────────────────────────>    clave privada B
   clave pública de B       puerto 51820/UDP          clave pública de A
```

- Cada extremo tiene un **par de claves** (pública y privada). Solo se intercambian las **públicas**.
- Cada *peer* sabe a qué direcciones puede hablar el otro (**AllowedIPs**), y WireGuard descarta cualquier paquete que no encaje.
- Va por **UDP**, es minimalista (pocas líneas de código) y muy rápido. No guarda estado de «sesión»: si no hay tráfico, el túnel está en silencio.
- Es el protocolo de bajo nivel; hay herramientas encima (Tailscale, Netbird) que automatizan el reparto de claves.

## 3. Requisitos

| Requisito | Detalle |
|---|---|
| Un servidor **alcanzable** | Necesita IP pública (o nombre DDNS) y un **puerto UDP abierto** (51820 por defecto) |
| Tu VM en VirtualBox | Con adaptador **puente** tiene IP en tu LAN; con **NAT** necesitas reenvío de puertos de VirtualBox, además del de tu router |
| Router doméstico | Reenvío de `UDP 51820` hacia la IP de la VM |
| IP dinámica | Un nombre DDNS (DuckDNS, No-IP…) para el `Endpoint` |
| Ubuntu | `sudo apt install wireguard` (el soporte está en el kernel de Linux) |

Si ya quieres acceder a la VM **desde tu propio PC en la misma máquina física**, no necesitas VPN: la red *solo anfitrión* o el reenvío de puertos de VirtualBox bastan. La VPN aporta cuando hay redes distintas de por medio.

## 4. Generar claves y configuración

Un script que usa el comando `wg` y escribe los archivos con permisos `600`, sin imprimir nunca las claves privadas:

```python
"""Genera las claves y los archivos de configuración de una VPN WireGuard con un servidor y un cliente.

No imprime NUNCA las claves privadas: las escribe en los archivos de configuración con permisos 600.
Uso:
    python3 wg_config.py --endpoint vpn.example.com:51820 --out ./wg
Requiere el paquete wireguard-tools (comando `wg`).
"""
import argparse
import ipaddress
import os
import subprocess
from pathlib import Path


def run(command: list[str], stdin: str | None = None) -> str:
    return subprocess.run(command, input=stdin, capture_output=True, text=True, check=True).stdout.strip()


def new_keypair() -> tuple[str, str]:
    """Devuelve (clave_privada, clave_pública)."""
    private = run(["wg", "genkey"])
    public = run(["wg", "pubkey"], stdin=private)
    return private, public


def write_private(path: Path, text: str) -> None:
    """Crea el archivo ya con permisos 600 (nunca existe con permisos más abiertos)."""
    descriptor = os.open(path, os.O_WRONLY | os.O_CREAT | os.O_TRUNC, 0o600)
    with os.fdopen(descriptor, "w") as file:
        file.write(text)


def main() -> None:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--endpoint", required=True, help="dirección pública y puerto del servidor, ej. vpn.example.com:51820")
    parser.add_argument("--network", default="10.8.0.0/24", help="red interna de la VPN")
    parser.add_argument("--out", type=Path, default=Path("wg"))
    args = parser.parse_args()

    network = ipaddress.ip_network(args.network)
    server_ip, client_ip = list(network.hosts())[:2]
    port = args.endpoint.rsplit(":", 1)[1]
    args.out.mkdir(parents=True, exist_ok=True)

    server_private, server_public = new_keypair()
    client_private, client_public = new_keypair()
    preshared = run(["wg", "genpsk"])  # secreto simétrico adicional (defensa extra)

    server_conf = f"""[Interface]
Address = {server_ip}/{network.prefixlen}
ListenPort = {port}
PrivateKey = {server_private}

[Peer]
# cliente: mi PC
PublicKey = {client_public}
PresharedKey = {preshared}
AllowedIPs = {client_ip}/32
"""
    client_conf = f"""[Interface]
Address = {client_ip}/{network.prefixlen}
PrivateKey = {client_private}

[Peer]
# servidor: mi VM
PublicKey = {server_public}
PresharedKey = {preshared}
Endpoint = {args.endpoint}
AllowedIPs = {network}
PersistentKeepalive = 25
"""
    write_private(args.out / "wg0-server.conf", server_conf)
    write_private(args.out / "wg0-client.conf", client_conf)
    print(f"Escrito en {args.out}/: wg0-server.conf (para la VM) y wg0-client.conf (para tu PC)")
    print(f"Clave pública del servidor: {server_public}")
    print(f"Clave pública del cliente:  {client_public}")


if __name__ == "__main__":
    main()
```

```bash
sudo apt install wireguard
python3 wg_config.py --endpoint vpn.example.com:51820 --out ./wg
```

| Comando de `wg` | Significado |
|---|---|
| `wg genkey` | Genera una clave privada |
| `wg pubkey` | Deriva la pública a partir de la privada (lee de stdin) |
| `wg genpsk` | Genera una clave precompartida opcional (defensa adicional frente a futuros ordenadores cuánticos) |

Resultado real (las claves privadas están ocultas aquí; las **públicas** de ejemplo son de usar y tirar):

```ini
# wg0-server.conf  (va en la VM)
[Interface]
Address = 10.8.0.1/24
ListenPort = 51820
PrivateKey = <CLAVE_PRIVADA_DEL_SERVIDOR>

[Peer]
# cliente: mi PC
PublicKey = 0EMWEBF0txz4rbHIhc+hqQvOoekkIWioPkNfkGUheSU=
PresharedKey = <CLAVE_PRECOMPARTIDA>
AllowedIPs = 10.8.0.2/32
```

```ini
# wg0-client.conf  (va en tu PC o móvil)
[Interface]
Address = 10.8.0.2/24
PrivateKey = <CLAVE_PRIVADA_DEL_CLIENTE>

[Peer]
# servidor: mi VM
PublicKey = VTYnI3+iI/LfgZXKjtTojRl4n4fDl/PCALAdAdqIWCM=
PresharedKey = <CLAVE_PRECOMPARTIDA>
Endpoint = vpn.example.com:51820
AllowedIPs = 10.8.0.0/24
PersistentKeepalive = 25
```

| Campo | Significado |
|---|---|
| `[Interface]` | La configuración de **este** extremo |
| `Address` | IP de este extremo dentro de la VPN |
| `ListenPort` | Puerto UDP en que escucha (solo necesario en quien recibe conexiones) |
| `PrivateKey` | Clave privada de este extremo. **Secreta: nunca sale del dispositivo ni se sube a Git** |
| `[Peer]` | Un interlocutor |
| `PublicKey` | Clave pública del interlocutor |
| `AllowedIPs` | Qué IP puede **usar** el peer como origen y a qué IP se le **enruta** tráfico |
| `Endpoint` | Dirección y puerto donde encontrarlo (el servidor lo conoce el cliente; el servidor aprende el del cliente cuando llega tráfico) |
| `PersistentKeepalive = 25` | Envía un paquete cada 25 s: mantiene abierto el NAT del cliente |
| `PresharedKey` | Secreto simétrico opcional extra |

### `AllowedIPs` del cliente: dos modos

| Valor | Efecto |
|---|---|
| `10.8.0.0/24` | **Túnel dividido** (*split tunnel*): solo el tráfico hacia la VPN va por el túnel; el resto sale normal |
| `0.0.0.0/0, ::/0` | **Túnel completo** (*full tunnel*): todo tu tráfico sale por el servidor (necesita reenvío y NAT en el servidor) |

## 5. Activarla en el servidor (VM)

```bash
sudo install -m 600 wg0-server.conf /etc/wireguard/wg0.conf
sudo ufw allow 51820/udp
sudo wg-quick up wg0
sudo wg show
sudo systemctl enable --now wg-quick@wg0
```

| Comando | Significado |
|---|---|
| `install -m 600 origen destino` | Copia con permisos `600` (solo el dueño lee) |
| `ufw allow 51820/udp` | Abre el puerto UDP en el cortafuegos |
| `wg-quick up wg0` | Crea la interfaz `wg0`, le pone IP y rutas |
| `wg-quick down wg0` | La desactiva |
| `wg show` | Estado: peers, último saludo (*handshake*), bytes |
| `systemctl enable --now wg-quick@wg0` | Arranca ahora y en cada inicio |

**Para túnel completo** el servidor debe reenviar paquetes y hacer NAT:

```bash
echo "net.ipv4.ip_forward=1" | sudo tee /etc/sysctl.d/99-wireguard.conf
sudo sysctl --system
```

y añadir a `[Interface]` del servidor (cambia `eth0` por tu interfaz real de `ip -br a`):

```ini
PostUp = iptables -t nat -A POSTROUTING -s 10.8.0.0/24 -o eth0 -j MASQUERADE
PostDown = iptables -t nat -D POSTROUTING -s 10.8.0.0/24 -o eth0 -j MASQUERADE
```

## 6. Activarla en el cliente

- **Linux**: igual (`wg-quick up`).
- **Windows / macOS / Android / iOS**: instala la aplicación oficial de WireGuard y **importa** el archivo `wg0-client.conf` (en el móvil, escaneando un código QR: `qrencode -t ansiutf8 < wg0-client.conf`, y luego borra el archivo de ese dispositivo si no lo necesitas).

Comprobar:

```bash
ping -c 3 10.8.0.1
ssh dev26@10.8.0.1
sudo wg show
```

En `wg show` debe aparecer un `latest handshake` de hace unos segundos. Sin él, el túnel no se ha establecido.

## 7. Que los servicios solo escuchen en la VPN

Ahora puedes hacer que SSH, tu API y tus bases de datos **no estén en Internet**: configúralos para escuchar en `10.8.0.1` en lugar de `0.0.0.0`, o ciérrales el puerto en el cortafuegos salvo desde `wg0`:

```bash
sudo ufw allow in on wg0 to any port 22 proto tcp
sudo ufw deny 22/tcp
```

Así, para llegar a SSH hay que estar dentro de la VPN. Mira [Endurecimiento y detección](#hardening) para el resto de la configuración del servidor.

## 8. Alternativa rápida: túnel SSH

Para un solo servicio no necesitas una VPN:

```bash
ssh -N -L 3000:localhost:3000 dev26@vm
ssh -N -D 1080 dev26@vm
```

| Flag | Significado |
|---|---|
| `-N` | No ejecuta comandos remotos: solo mantiene el túnel |
| `-L 3000:localhost:3000` | *Local forward*: tu `localhost:3000` llega al `localhost:3000` **de la VM** (así accedes a una API que solo escucha localmente) |
| `-D 1080` | Proxy SOCKS5 dinámico en tu puerto 1080 |
| `-J salto` | Salta a través de un servidor intermedio |

VS Code Remote SSH hace esto automáticamente al reenviar puertos.

## 9. Seguridad

- **Una pareja de claves por dispositivo.** Si pierdes el móvil, borra su `[Peer]` del servidor y listo.
- La clave privada **nunca** viaja por correo, chat ni repositorio. Si debes pasarla, hazlo por un canal cifrado y bórrala después.
- Archivos de configuración con permisos `600`.
- Mantén `wireguard` y el sistema actualizados.
- Menos es más: `AllowedIPs` ajustados al mínimo necesario por peer.
- Un servidor VPN es un servicio expuesto a Internet: aplica el [endurecimiento](#hardening) a la VM.

## 10. Diagnóstico

| Síntoma | Causa probable | Qué comprobar |
|---|---|---|
| `latest handshake` ausente | El paquete UDP no llega o las claves no coinciden | Puerto UDP abierto en router, VirtualBox y `ufw`; `Endpoint` correcto; claves públicas cruzadas |
| Handshake bien pero `ping` falla | `AllowedIPs` o rutas | `ip route`, `AllowedIPs` de ambos lados |
| Funciona un rato y se corta | NAT cierra el mapeo | `PersistentKeepalive = 25` en el cliente |
| Sin Internet con túnel completo | Falta reenvío o NAT | `sysctl net.ipv4.ip_forward`, regla `MASQUERADE` |
| `RTNETLINK answers: Operation not supported` | El kernel no tiene WireGuard | Kernel reciente (Ubuntu lo trae) |
| `Warning: ... world accessible` | Permisos del archivo | `chmod 600` |
| Páginas se cuelgan con túnel completo | MTU | Prueba `MTU = 1380` en `[Interface]` |
| Ver si llegan los paquetes | | `sudo tcpdump -ni eth0 udp port 51820` ([Monitorización](#monitoreo)) |

## 11. Ejercicios

> **Ejercicio:** monta un túnel entre dos VMs de VirtualBox en una red interna (así practicas sin tocar el router) y comprueba el `handshake` con `wg show`.

> **Ejercicio:** haz que tu API de tareas solo escuche en la IP de la VPN y compruébalo desde dentro y desde fuera del túnel.

> **Ejercicio:** captura con `tcpdump` el tráfico UDP del puerto 51820 mientras haces `ping` por el túnel y verifica que no se ve ningún contenido legible.

> **Ejercicio:** añade un segundo cliente con su propio par de claves, y después revoca el primero. ¿Qué cambia en el servidor?

> **Ejercicio:** compara un túnel SSH (`-L`) con la VPN para acceder a PostgreSQL de tu VM. ¿Cuándo prefieres cada uno?

## 12. Documentación oficial

- [WireGuard: inicio rápido](https://www.wireguard.com/quickstart/)
- [wg-quick(8)](https://man7.org/linux/man-pages/man8/wg-quick.8.html)
- [Ubuntu: WireGuard](https://documentation.ubuntu.com/server/how-to/wireguard-vpn/)
- [Tailscale](https://tailscale.com/kb/)
