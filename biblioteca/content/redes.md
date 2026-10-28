# Redes y protocolos

Toda aplicación web, toda API y toda VPN se apoyan en lo mismo: paquetes que viajan entre equipos siguiendo reglas llamadas **protocolos**. Entender las capas te permite diagnosticar cualquier problema de conexión y, más adelante, reconocer qué es normal y qué no en tu tráfico.

*Verificado: 2026-10-27. Los comandos y programas de este módulo se ejecutaron en un entorno Linux real (servidores locales, sockets, un certificado autofirmado, una captura con `tcpdump`). Los resultados mostrados son salidas reales, recortadas. Referencia: [MDN: HTTP](https://developer.mozilla.org/es/docs/Web/HTTP) y [man-pages](https://man7.org/linux/man-pages/).*

> **Regla:** en este módulo solo observas y pruebas **tu propia máquina y tu propia red**. Ver [Marco ético](#seguridad-marco).

## 1. El modelo en capas

Cada capa usa los servicios de la inferior y ofrece los suyos a la superior.

| Capa (modelo TCP/IP) | Qué hace | Ejemplos | Herramienta para verla |
|---|---|---|---|
| **Aplicación** | El protocolo que usa tu programa | HTTP, HTTPS, DNS, SSH, SMTP | `curl`, `dig`, navegador |
| **Transporte** | Entrega entre **programas** (puertos) | TCP (fiable), UDP (rápido, sin garantías) | `ss`, `nc` |
| **Red** (Internet) | Entrega entre **equipos** (direcciones IP) y rutas | IPv4, IPv6, ICMP | `ip`, `ping`, `traceroute` |
| **Enlace** | Entrega en el medio físico local (MAC) | Ethernet, Wi-Fi | `ip link` |

Un mensaje HTTP viaja *encapsulado*: dentro de un segmento TCP, dentro de un paquete IP, dentro de una trama Ethernet.

## 2. Direcciones IP, redes y puertos

| Concepto | Significado | Ejemplo |
|---|---|---|
| **Dirección IPv4** | 32 bits, 4 números de 0–255 | `192.168.1.50` |
| **Máscara / CIDR** | Cuántos bits identifican la red | `192.168.1.0/24` = red de 256 direcciones (`.0`–`.255`) |
| **Puerta de enlace** (*gateway*) | Equipo al que se envía lo que sale de tu red | `192.168.1.1` |
| **Loopback** | Tu propio equipo | `127.0.0.1` (`localhost`) |
| **`0.0.0.0`** | «Todas las interfaces» al escuchar | un servidor en `0.0.0.0:22` acepta conexiones desde cualquier interfaz |
| **IPv6** | 128 bits | `::1` (loopback) |
| **Puerto** | Número de 0–65535 que identifica un programa dentro del equipo | `443` |

**Rangos privados** (no se enrutan por Internet): `10.0.0.0/8`, `172.16.0.0/12`, `192.168.0.0/16`. Tu router hace **NAT**: traduce las direcciones privadas a la pública.

| Puerto | Servicio |
|---|---|
| 22 | SSH |
| 53 | DNS |
| 80 / 443 | HTTP / HTTPS |
| 3000, 5173, 8080 | Habituales en desarrollo |
| 5432 / 27017 / 6379 | PostgreSQL / MongoDB / Redis (**nunca expuestos a Internet**) |
| 51820 | WireGuard (UDP) |

Los puertos inferiores a 1024 requieren privilegios para escuchar en Linux.

### Tu VM y VirtualBox

El modo de red de la VM determina quién puede alcanzarla:

| Modo | La VM sale a Internet | Tu PC alcanza la VM | Otros equipos de la LAN alcanzan la VM |
|---|---|---|---|
| **NAT** | Sí | Solo con reenvío de puertos | No |
| **NAT Network** | Sí | Con reenvío de puertos | Entre VMs |
| **Adaptador puente** (*Bridged*) | Sí | Sí | **Sí** (aparece como un equipo más de tu red) |
| **Solo anfitrión** (*Host-only*) | No | Sí | No |
| **Red interna** | No | No | Solo otras VMs en la misma red |

Tu informe muestra SSH en `0.0.0.0:22`. Si la VM está en modo **puente**, ese SSH es alcanzable desde tu red local: asegúralo ([Endurecimiento](#hardening)). Si está en **NAT** sin reenvío, no lo es.

## 3. Ver y diagnosticar tu red

```bash
ip -br a
ip r
ss -ltn
ss -tn
ping -c 4 192.168.1.1
traceroute -n 8.8.8.8
dig +short ejemplo.com
resolvectl status
```

| Comando | Significado |
|---|---|
| `ip -br a` | Direcciones por interfaz (`a` = `addr`; `-br` = breve) |
| `ip r` | Tabla de rutas: la línea `default via …` es tu puerta de enlace |
| `ip -s link` | Contadores de paquetes y errores por interfaz |
| `ss -ltn` | Puertos TCP en escucha: `-l` *listening*, `-t` TCP, `-n` números |
| `ss -tn` | Conexiones TCP establecidas ahora mismo |
| `ss -ltnp` | Añade el proceso dueño (`-p`; requiere `sudo` para los de otros usuarios) |
| `ping -c 4 host` | Envía 4 paquetes ICMP y mide la latencia |
| `traceroute -n host` | Muestra los saltos hasta el destino (`-n` sin resolver nombres) |
| `dig +short nombre` | Consulta DNS y solo muestra la respuesta; `dig MX nombre` para el correo; `dig @8.8.8.8 nombre` pregunta a un servidor concreto |
| `resolvectl status` | Servidores DNS que usa tu sistema (usa `systemd-resolved`, activo en tu equipo) |

Resultado real de un equipo de pruebas:

```text
$ ip -br a
lo               UNKNOWN        127.0.0.1/8
$ ip r
default via 192.0.2.1 dev eth0
192.0.2.0/24 dev eth0 proto kernel scope link src 192.0.2.2
```

`default via 192.0.2.1` es la puerta de enlace; `192.0.2.0/24 dev eth0` dice que esa red es directamente alcanzable por `eth0`.

### Diagnóstico por capas

Cuando «no funciona», sube capa a capa:

1. ¿Tengo dirección y ruta? `ip -br a`, `ip r`.
2. ¿Llego a la puerta de enlace? `ping -c 2 <gateway>`.
3. ¿Llego a Internet por IP? `ping -c 2 1.1.1.1`.
4. ¿Resuelve nombres? `dig +short ejemplo.com`.
5. ¿Está el puerto abierto? `nc -zv host 443`.
6. ¿Responde la aplicación? `curl -v https://host/`.

| Comando | Significado |
|---|---|
| `nc -zv host 443` | `-z` solo comprueba si el puerto acepta conexión (sin enviar datos); `-v` explica el resultado |
| `curl -I url` | Solo cabeceras (petición HEAD) |
| `curl -v url` | Muestra la conversación completa: `>` lo que envías, `<` lo que recibes, `*` información |

## 4. TCP y UDP

| | TCP | UDP |
|---|---|---|
| Conexión | Sí (se establece antes de enviar) | No |
| Entrega garantizada y en orden | **Sí** | No |
| Control de congestión | Sí | No |
| Velocidad / sobrecarga | Mayor sobrecarga | Mínima |
| Usos | Web, SSH, bases de datos, correo | DNS, vídeo en vivo, juegos, WireGuard |

El **saludo de tres vías** de TCP, capturado de verdad con `tcpdump` (ver [Monitorización](#monitoreo)):

```text
127.0.0.1.47494 > 127.0.0.1.8099: Flags [S]    seq 2111901591          (1) SYN: "quiero conectar"
127.0.0.1.8099 > 127.0.0.1.47494: Flags [S.]   seq 3847372488 ack ...  (2) SYN-ACK: "de acuerdo"
127.0.0.1.47494 > 127.0.0.1.8099: Flags [.]    ack 1                   (3) ACK: "conectados"
127.0.0.1.47494 > 127.0.0.1.8099: Flags [P.]   length 87               datos: la petición HTTP
```

### Sockets en código

```python
"""Sockets TCP y UDP con la librería estándar: servidor de eco y cliente, todo en local."""
import socket
import threading


def tcp_echo_server(server: socket.socket) -> None:
    """Acepta UNA conexión y devuelve en mayúsculas lo que recibe."""
    connection, address = server.accept()
    with connection:
        print("servidor: conexión desde", address)
        while data := connection.recv(1024):          # recv devuelve b"" cuando el otro extremo cierra
            connection.sendall(data.upper())


def main() -> None:
    # --- TCP: orientado a conexión, fiable, flujo de bytes ordenado
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as server:
        server.bind(("127.0.0.1", 0))                  # puerto 0: el sistema elige uno libre
        server.listen()
        port = server.getsockname()[1]
        threading.Thread(target=tcp_echo_server, args=(server,), daemon=True).start()

        with socket.create_connection(("127.0.0.1", port), timeout=2) as client:
            client.sendall(b"hola red")
            print("cliente recibió:", client.recv(1024))

    # --- UDP: sin conexión, sin garantías de entrega ni de orden; paquetes independientes
    with socket.socket(socket.AF_INET, socket.SOCK_DGRAM) as receiver:
        receiver.bind(("127.0.0.1", 0))
        udp_port = receiver.getsockname()[1]
        with socket.socket(socket.AF_INET, socket.SOCK_DGRAM) as sender:
            sender.sendto(b"datagrama", ("127.0.0.1", udp_port))
        data, origin = receiver.recvfrom(1024)
        print("UDP recibido:", data, "desde puerto", origin[1] != udp_port)


if __name__ == "__main__":
    main()
```

| Pieza | Significado |
|---|---|
| `AF_INET` | Familia de direcciones IPv4 |
| `SOCK_STREAM` / `SOCK_DGRAM` | TCP / UDP |
| `bind((ip, puerto))` | Asocia el socket a una dirección; puerto `0` = el sistema elige |
| `listen()` / `accept()` | El servidor espera y acepta conexiones |
| `create_connection(...)` | El cliente conecta |
| `recv(n)` | Lee hasta `n` bytes; `b""` indica que el otro extremo cerró |
| `sendall` / `sendto` | Enviar por TCP / UDP |

> **Ojo:** TCP es un **flujo** de bytes, no de mensajes: un `recv` puede devolver medio mensaje o dos pegados. Necesitas delimitadores o longitudes (como en las tramas de [Hardware](#hardware)).

## 5. HTTP, a mano

HTTP es texto. Esto es una conversación real con un servidor local, escrita con `nc`:

```bash
printf 'GET /index.html HTTP/1.1\r\nHost: localhost\r\nConnection: close\r\n\r\n' | nc -q1 127.0.0.1 8099
```

```text
HTTP/1.0 200 OK
Server: SimpleHTTP/0.6 Python/3.13
Content-type: text/html
Content-Length: 14

<h1>Hola</h1>
```

| Parte | Significado |
|---|---|
| `GET /index.html HTTP/1.1` | **Línea de petición**: método, ruta, versión |
| `Host: localhost` | Cabecera obligatoria en HTTP/1.1 (varios sitios en una IP) |
| Línea en blanco (`\r\n\r\n`) | Fin de las cabeceras |
| `HTTP/1.0 200 OK` | **Línea de estado**: versión, código, texto |
| `Content-Type`, `Content-Length` | Tipo y tamaño del cuerpo |
| `printf '…\r\n'` | HTTP usa `CRLF` (`\r\n`) como fin de línea |
| `nc -q1` | Espera 1 segundo tras el fin de la entrada antes de cerrar |

Los métodos y códigos de estado están en [Node.js](#node). Cabeceras de seguridad importantes en respuestas: `Strict-Transport-Security`, `Content-Security-Policy`, `X-Content-Type-Options: nosniff`, `Set-Cookie` con `HttpOnly; Secure; SameSite`.

## 6. DNS

El DNS traduce nombres a direcciones: `ejemplo.com` → `93.184.216.34`.

| Registro | Contenido |
|---|---|
| `A` / `AAAA` | Dirección IPv4 / IPv6 |
| `CNAME` | Alias a otro nombre |
| `MX` | Servidores de correo |
| `TXT` | Texto (verificaciones, SPF, DKIM) |
| `NS` | Servidores de nombres del dominio |

Un **resolvedor** (tu router o `systemd-resolved`) pregunta y guarda la respuesta en caché según el TTL. El DNS clásico va **sin cifrar** en el puerto 53: quien vea tu red ve los nombres que consultas. Alternativas: DNS sobre TLS (DoT) o sobre HTTPS (DoH).

## 7. TLS y HTTPS

**TLS** cifra y autentica la conexión. Proporciona:

1. **Confidencialidad**: nadie en el camino lee los datos.
2. **Integridad**: nadie los modifica sin que se note.
3. **Autenticación**: el **certificado** demuestra que hablas con el dueño del dominio, firmado por una autoridad (CA) de confianza.

Probado con un servidor HTTPS local y un certificado autofirmado creado con `openssl`:

```bash
openssl req -x509 -newkey rsa:2048 -nodes -keyout key.pem -out cert.pem -days 2 \
  -subj "/CN=localhost" -addext "subjectAltName=DNS:localhost,IP:127.0.0.1"
```

| Flag | Significado |
|---|---|
| `req -x509` | Genera directamente un certificado autofirmado |
| `-newkey rsa:2048` | Crea una clave nueva de 2048 bits |
| `-nodes` | No cifra la clave privada con contraseña (solo para pruebas) |
| `-keyout` / `-out` | Archivos de la clave privada y del certificado |
| `-days 2` | Validez |
| `-subj "/CN=localhost"` | Nombre del sujeto |
| `-addext subjectAltName=…` | Nombres válidos: los navegadores **exigen** este campo |

```python
"""Servidor HTTPS mínimo con un certificado autofirmado (solo para practicar en local)."""
import http.server
import ssl

context = ssl.SSLContext(ssl.PROTOCOL_TLS_SERVER)
context.minimum_version = ssl.TLSVersion.TLSv1_2          # rechaza versiones antiguas e inseguras
context.load_cert_chain("cert.pem", "key.pem")

server = http.server.HTTPServer(("127.0.0.1", 8443), http.server.SimpleHTTPRequestHandler)
server.socket = context.wrap_socket(server.socket, server_side=True)
print("HTTPS en https://localhost:8443", flush=True)
server.serve_forever()
```

```bash
curl https://localhost:8443/
curl --cacert cert.pem https://localhost:8443/
echo | openssl s_client -connect localhost:8443 -servername localhost -brief
```

Resultados reales:

```text
$ curl https://localhost:8443/
curl: (60) SSL certificate problem: self-signed certificate

$ curl --cacert cert.pem https://localhost:8443/        -> 200

$ openssl s_client -connect localhost:8443 -servername localhost -brief
CONNECTION ESTABLISHED
Protocol version: TLSv1.3
Ciphersuite: TLS_AES_256_GCM_SHA384
Peer certificate: CN = localhost
verify error:num=18:self-signed certificate
```

| Pieza | Significado |
|---|---|
| `curl` falla con el error 60 | El certificado no está firmado por una CA conocida: **es la protección funcionando** |
| `--cacert cert.pem` | Le dices a `curl` que confíe en ese certificado concreto |
| `openssl s_client -connect host:puerto` | Cliente TLS de diagnóstico; `-servername` envía el SNI (el nombre buscado); `-brief` resume |
| `TLSv1.3` | Versión negociada; `minimum_version = TLSv1_2` en el servidor rechaza versiones antiguas |
| `TLS_AES_256_GCM_SHA384` | Suite de cifrado: AES-256 en modo GCM |

Ver un certificado: `openssl x509 -in cert.pem -noout -subject -issuer -dates -ext subjectAltName`.

> **Peligro:** **nunca** desactives la verificación de certificados (`curl -k`, `verify=False`, `NODE_TLS_REJECT_UNAUTHORIZED=0`) en un programa real. Anula toda la protección de TLS. En desarrollo, confía en un certificado concreto como arriba.

Para HTTPS real en tu servidor: **Let's Encrypt** (certificados gratuitos y automáticos) con Caddy o `certbot`.

## 8. Cortafuegos y NAT

Un **cortafuegos** decide qué paquetes entran y salen por reglas (dirección, puerto, protocolo). En Ubuntu: `ufw` (sencillo) sobre `nftables`. Se desarrolla en [Endurecimiento](#hardening).

**NAT**: tu router comparte una IP pública entre muchos equipos. Para que algo de fuera llegue a tu VM necesita un *reenvío de puertos*. Es una barrera útil por defecto.

## 9. Errores habituales

| Síntoma | Causa probable | Cómo comprobarlo |
|---|---|---|
| `Connection refused` | Nadie escucha en ese puerto | `ss -ltn` en el destino |
| `Connection timed out` | Cortafuegos o ruta que descarta paquetes | `traceroute`, reglas del cortafuegos |
| `No route to host` | Sin ruta | `ip r` |
| `Name or service not known` | DNS | `dig +short nombre`, `resolvectl status` |
| Funciona con `localhost` pero no desde fuera | El servidor escucha en `127.0.0.1` | Escucha en `0.0.0.0` (y revisa cortafuegos / modo de red de la VM) |
| `SSL certificate problem` | CA desconocida, nombre distinto o caducado | `openssl s_client`, revisa fechas y SAN |
| Funciona por IP, no por nombre | DNS | `dig` |
| Puerto ocupado: `EADDRINUSE` | Otro proceso lo usa | `ss -ltnp \| grep puerto` |

## 10. Ejercicios

> **Ejercicio:** con `ss -ltn` y `ip -br a`, haz una tabla de todo lo que escucha en tu VM, en qué interfaz y si es alcanzable desde fuera según el modo de red de VirtualBox.

> **Ejercicio:** haz a mano con `nc` una petición `POST` con cuerpo JSON a tu API de tareas, calculando el `Content-Length`. Comprueba la respuesta.

> **Ejercicio:** monta el servidor HTTPS local de este módulo, mide con `curl -w "%{time_connect} %{time_appconnect}\n" -o /dev/null -s --cacert cert.pem https://localhost:8443/` cuánto cuesta el saludo TLS frente a TCP.

> **Ejercicio:** escribe un servidor TCP de chat en Python con sockets que acepte varios clientes (un hilo por cliente) y reenvíe los mensajes a todos.

> **Ejercicio:** calcula a mano cuántas direcciones útiles tiene `/24`, `/27` y `/16`, y comprueba con `python3 -c "import ipaddress; print(ipaddress.ip_network('192.168.1.0/27').num_addresses)"`.

## 11. Documentación oficial

- [MDN: HTTP](https://developer.mozilla.org/es/docs/Web/HTTP)
- [MDN: cómo funciona Internet](https://developer.mozilla.org/es/docs/Learn_web_development/Howto/Web_mechanics/How_does_the_Internet_work)
- [RFC 9110: semántica HTTP](https://www.rfc-editor.org/rfc/rfc9110)
- [OpenSSL: s_client](https://docs.openssl.org/master/man1/openssl-s_client/)
- [Let's Encrypt](https://letsencrypt.org/es/)
