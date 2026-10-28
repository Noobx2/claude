# Endurecimiento y detección de intrusos

**Endurecer** (*hardening*) es reducir lo que un atacante puede aprovechar: menos servicios, menos puertos, menos permisos, más registros y más avisos. **Detectar** es enterarte cuando alguien lo intenta. Este módulo aplica ambas cosas a tu VM Ubuntu, usando como punto de partida **los datos reales de tu informe**.

*Verificado: 2026-10-27 en parte. El analizador de registros de SSH se ejecutó y sus pruebas pasan. Los comandos de configuración (`ufw`, `sshd`, `fail2ban`) siguen la documentación oficial pero **no se han ejecutado aquí**: pruébalos en tu VM, con una instantánea hecha antes y una segunda sesión de SSH abierta. Referencia: [Ubuntu Server: seguridad](https://documentation.ubuntu.com/server/explanation/intro-to/security/), [OpenSSH](https://www.openssh.com/manual.html).*

> **Regla:** detectas, bloqueas, registras y **denuncias**. No contraatacas ([Marco ético](#seguridad-marco)).

## 1. Principios

| Principio | Significado |
|---|---|
| **Superficie mínima** | Lo que no está instalado ni escucha no se puede atacar |
| **Mínimo privilegio** | Cada usuario y programa, solo los permisos que necesita |
| **Defensa en profundidad** | Varias capas (cortafuegos + claves SSH + actualizaciones + registros): si una falla, otra protege |
| **Actualizar** | La mayoría de las intrusiones usan fallos ya corregidos |
| **Registrar y vigilar** | Sin registros no sabes si te atacaron |
| **Copias y recuperación** | Asumir que puede ocurrir y poder volver atrás |

## 2. Punto de partida: tu informe

| Dato de tu informe | Riesgo | Qué hacer |
|---|---|---|
| SSH escuchando en `0.0.0.0:22` y `[::]:22` | Alcanzable desde cualquier interfaz | Claves en lugar de contraseñas, cortafuegos, `fail2ban`; comprueba el modo de red de la VM |
| Tu usuario en los grupos `sudo` y `docker` | Quien tome tu cuenta tiene administrador (el grupo `docker` equivale a root) | Cuenta con clave y frase de contraseña; no compartas; considera Docker *rootless* |
| No consta cortafuegos (`ufw`) | Todo puerto abierto es accesible | Activar `ufw` con política «denegar entrada» |
| Servicios activos: `avahi-daemon`, `ModemManager`, `snapd`, `cups`… | Superficie innecesaria | Desactiva los que no uses |
| `unattended-upgrades` activo | Bien: parches de seguridad automáticos | Comprueba que funciona |
| Puertos locales sin identificar (36267, 36379, 37113) | Probablemente VS Code Server | `sudo ss -ltnp` para confirmarlo |
| Git sin configurar, sin claves | — | Genera claves ed25519 ([Git](#git)) |

## 3. Auditar lo que hay

```bash
sudo ss -ltnp
systemctl list-units --type=service --state=running --no-pager
sudo ufw status verbose
id -nG
sudo -l
awk -F: '$3 == 0 {print $1}' /etc/passwd
sudo find / -xdev -perm -4000 -type f 2>/dev/null
sudo lynis audit system
```

| Comando | Significado |
|---|---|
| `sudo ss -ltnp` | Todos los puertos en escucha y su proceso |
| `systemctl list-units --type=service --state=running` | Servicios activos |
| `ufw status verbose` | Estado y reglas del cortafuegos (`inactive` = sin protección) |
| `id -nG` | Tus grupos |
| `sudo -l` | Qué puedes ejecutar con `sudo` |
| `awk -F: '$3 == 0 {print $1}' /etc/passwd` | Usuarios con UID 0 (administrador): solo debe salir `root` |
| `find / -xdev -perm -4000 -type f` | Archivos SUID (se ejecutan con permisos del dueño): conviene conocerlos; `-xdev` no sale del sistema de archivos |
| `lynis audit system` | Auditor de seguridad (paquete `lynis`): informa de debilidades de **tu** sistema con recomendaciones |

Desactivar un servicio que no necesitas:

```bash
sudo systemctl disable --now avahi-daemon
sudo apt purge ModemManager
```

`disable --now` lo detiene y evita que arranque; `purge` lo desinstala. Hazlo uno a uno y comprueba que nada deja de funcionar.

## 4. SSH: acceso solo con claves

**1. Prepara la clave** en tu PC y cópiala a la VM:

```bash
ssh-keygen -t ed25519 -C "dev26@vm" -f ~/.ssh/id_ed25519
ssh-copy-id -i ~/.ssh/id_ed25519.pub dev26@192.168.1.50
ssh dev26@192.168.1.50
```

| Pieza | Significado |
|---|---|
| `ssh-keygen -t ed25519` | Crea el par de claves (pon frase de contraseña) |
| `ssh-copy-id -i clave.pub usuario@host` | Instala tu clave **pública** en `~/.ssh/authorized_keys` del servidor |
| Última línea | Comprueba que entras **con la clave** antes de seguir |

**2. Endurece el servidor.** Crea `/etc/ssh/sshd_config.d/99-hardening.conf`:

```text
PasswordAuthentication no
KbdInteractiveAuthentication no
PermitRootLogin no
PubkeyAuthentication yes
AllowUsers dev26
MaxAuthTries 3
LoginGraceTime 30
X11Forwarding no
```

| Directiva | Efecto |
|---|---|
| `PasswordAuthentication no` | Desactiva las contraseñas: la fuerza bruta deja de ser posible |
| `KbdInteractiveAuthentication no` | Cierra la otra vía de contraseña |
| `PermitRootLogin no` | Nadie entra como `root` directamente |
| `PubkeyAuthentication yes` | Acepta claves |
| `AllowUsers dev26` | Solo ese usuario puede entrar |
| `MaxAuthTries 3` | Intentos por conexión |
| `LoginGraceTime 30` | Segundos para autenticarse |
| `X11Forwarding no` | Sin reenvío gráfico |

```bash
sudo sshd -t
sudo sshd -T | grep -iE 'passwordauthentication|permitrootlogin|allowusers'
sudo systemctl reload ssh
```

| Comando | Significado |
|---|---|
| `sshd -t` | Comprueba la sintaxis **sin aplicar nada** |
| `sshd -T` | Muestra la configuración efectiva final |
| `systemctl reload ssh` | Aplica sin cortar las sesiones abiertas (en Ubuntu el servicio se llama `ssh`) |

> **Peligro:** **no cierres tu sesión actual** hasta haber abierto otra nueva con la clave y comprobado que entra. Si te equivocas con `PasswordAuthentication no` sin tener una clave instalada, te quedas fuera. Con la VM de VirtualBox tienes la consola y las instantáneas como red de seguridad.

Cambiar el puerto 22 por otro reduce el *ruido* de los registros pero **no es seguridad**: no lo cuentes como una defensa.

## 5. Cortafuegos con `ufw`

```bash
sudo ufw default deny incoming
sudo ufw default allow outgoing
sudo ufw allow from 192.168.1.0/24 to any port 22 proto tcp
sudo ufw allow 51820/udp
sudo ufw logging medium
sudo ufw enable
sudo ufw status numbered
```

| Comando | Significado |
|---|---|
| `default deny incoming` | Bloquea todo lo entrante salvo lo permitido |
| `default allow outgoing` | Permite lo saliente |
| `allow from RED to any port 22 proto tcp` | SSH solo desde tu red local (ajusta a la tuya) |
| `allow 51820/udp` | WireGuard ([VPN](#vpn)) |
| `limit 22/tcp` | Alternativa: permite SSH pero limita ráfagas de intentos |
| `logging medium` | Registra los bloqueos |
| `enable` | Activa (¡antes asegúrate de haber permitido SSH!) |
| `status numbered` | Lista numerada; `ufw delete 3` borra la regla 3 |

> **Peligro: Docker se salta `ufw`.** Docker modifica directamente las reglas de red (`iptables`) y los puertos publicados con `-p 5432:5432` quedan abiertos **aunque `ufw` los bloquee**. Soluciones: publicar siempre en `127.0.0.1` (`-p 127.0.0.1:5432:5432`, como en todos los ejemplos de [Docker](#docker)), o gestionar la cadena `DOCKER-USER`. Revisa con `sudo ss -ltnp` lo que realmente escucha.

## 6. `fail2ban`: bloqueo automático de quien insiste

Lee los registros y bloquea durante un tiempo a los orígenes que fallan repetidamente.

```bash
sudo apt install fail2ban
```

`/etc/fail2ban/jail.local`:

```ini
[DEFAULT]
bantime  = 1h
findtime = 10m
maxretry = 5
ignoreip = 127.0.0.1/8 ::1 192.168.1.0/24

[sshd]
enabled = true
```

```bash
sudo systemctl enable --now fail2ban
sudo fail2ban-client status sshd
sudo fail2ban-client set sshd unbanip 203.0.113.5
```

| Opción | Significado |
|---|---|
| `bantime` | Cuánto dura el bloqueo |
| `findtime` + `maxretry` | Más de 5 fallos en 10 minutos → bloqueo |
| `ignoreip` | Orígenes que nunca se bloquean (**incluye tu red** para no bloquearte a ti) |
| `[sshd] enabled = true` | Activa la protección de SSH |
| `fail2ban-client status sshd` | IP bloqueadas ahora y total |
| `set sshd unbanip IP` | Desbloquea a mano |

Si solo aceptas **claves** (sección 4), la fuerza bruta de contraseñas ya no funciona; `fail2ban` sirve entonces sobre todo para reducir ruido y carga.

## 7. Actualizaciones

```bash
sudo apt update
apt list --upgradable
sudo apt upgrade
systemctl status unattended-upgrades --no-pager
cat /etc/apt/apt.conf.d/20auto-upgrades
```

Tu informe muestra `unattended-upgrades` activo: aplica los parches de seguridad automáticamente. Comprueba que `20auto-upgrades` tiene `APT::Periodic::Unattended-Upgrade "1";`. Reinicia cuando el sistema lo pida (`/var/run/reboot-required`). También actualiza las **imágenes de Docker** y las dependencias de tus proyectos (`pnpm outdated`, `mvn versions:display-dependency-updates`, `cargo update`, `uv lock --upgrade`).

## 8. Detectar: registros de acceso

```bash
sudo journalctl -u ssh --since today --no-pager
sudo journalctl -u ssh -f
last -i -n 20
sudo lastb -i -n 20
who
w
```

| Comando | Significado |
|---|---|
| `journalctl -u ssh --since today` | Lo que registró SSH hoy (`-f` en vivo) |
| `last -i -n 20` | Últimos 20 inicios de sesión **correctos**, con IP (`-i`) |
| `sudo lastb -i -n 20` | Últimos intentos **fallidos** |
| `who` / `w` | Quién está conectado ahora y qué hace |

Líneas típicas de `sshd` que debes saber leer:

```text
Failed password for invalid user admin from 203.0.113.5 port 52122 ssh2     intento con un usuario inexistente
Failed password for dev26 from 203.0.113.5 port 40000 ssh2                  usuario real, contraseña mala
Invalid user test from 203.0.113.5 port 52126                               usuario inexistente
Accepted publickey for dev26 from 192.168.1.20 port 40010 ssh2              entrada correcta con clave
```

### Un analizador propio

```python
"""Analiza registros de SSH y resume los intentos de acceso fallidos por origen.

Uso:
    sudo journalctl -u ssh --no-pager | python auth_failures.py
    python auth_failures.py < /var/log/auth.log
    python auth_failures.py --threshold 5 < auth.log
Es una herramienta de DETECCIÓN: te dice quién insiste, para que lo bloquees y lo denuncies.
"""

import argparse
import ipaddress
import re
import sys
from collections import Counter, defaultdict

FAILED = re.compile(
    r"Failed (?:password|publickey) for (?:invalid user )?(?P<user>\S+) from (?P<ip>[0-9a-fA-F.:]+) port \d+"
)
INVALID = re.compile(r"Invalid user (?P<user>\S*) from (?P<ip>[0-9a-fA-F.:]+)")
ACCEPTED = re.compile(
    r"Accepted (?P<method>\S+) for (?P<user>\S+) from (?P<ip>[0-9a-fA-F.:]+) port \d+"
)


def analyze(
    lines: list[str],
) -> tuple[Counter[str], dict[str, set[str]], list[tuple[str, str, str]]]:
    failures: Counter[str] = Counter()
    users_tried: dict[str, set[str]] = defaultdict(set)
    successes: list[tuple[str, str, str]] = []
    for line in lines:
        if match := FAILED.search(line) or INVALID.search(line):
            failures[match["ip"]] += 1
            users_tried[match["ip"]].add(match["user"])
        elif match := ACCEPTED.search(line):
            successes.append((match["user"], match["ip"], match["method"]))
    return failures, users_tried, successes


LOCAL_NETWORKS = [
    ipaddress.ip_network(net)
    for net in ("10.0.0.0/8", "172.16.0.0/12", "192.168.0.0/16", "fc00::/7")
]


def scope(ip: str) -> str:
    """«red local» para rangos privados (RFC 1918), loopback y enlace local; «pública» para el resto."""
    address = ipaddress.ip_address(ip)
    if (
        address.is_loopback
        or address.is_link_local
        or any(address in net for net in LOCAL_NETWORKS)
    ):
        return "red local"
    return "pública"


def main() -> None:
    parser = argparse.ArgumentParser(
        description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter
    )
    parser.add_argument(
        "--threshold",
        type=int,
        default=3,
        help="fallos a partir de los cuales se marca un origen",
    )
    args = parser.parse_args()

    failures, users_tried, successes = analyze(sys.stdin.read().splitlines())
    print(f"{'origen':40} {'ámbito':10} {'fallos':>7}  usuarios probados")
    for ip, count in failures.most_common(10):
        flag = (
            "  <-- BLOQUEAR"
            if count >= args.threshold and scope(ip) == "pública"
            else ""
        )
        print(
            f"{ip:40} {scope(ip):10} {count:>7}  {', '.join(sorted(users_tried[ip]))[:40]}{flag}"
        )
    print("\nAccesos correctos:")
    for user, ip, method in successes:
        print(f"  {user} desde {ip} ({method})")


if __name__ == "__main__":
    main()
```

```bash
sudo journalctl -u ssh --no-pager | uv run python src/auth_failures.py --threshold 5
```

Con registros de ejemplo (direcciones de documentación), el resultado real es:

```text
origen                                   ámbito      fallos  usuarios probados
203.0.113.5                              pública          3  admin, root, test  <-- BLOQUEAR
192.168.1.20                             red local        1  dev26

Accesos correctos:
  dev26 desde 192.168.1.20 (publickey)
```

La **señal clave**: una IP pública con muchos fallos y **muchos usuarios distintos** (`admin`, `root`, `test`…) es un *escaneo automático*. Una entrada correcta (`Accepted`) desde una IP que no reconoces es lo que de verdad hay que investigar.

### Auditoría y cambios de archivos

```bash
sudo apt install auditd
sudo auditctl -w /etc/passwd -p wa -k identity
sudo ausearch -k identity --start today
```

| Pieza | Significado |
|---|---|
| `auditd` | Sistema de auditoría del kernel |
| `-w /etc/passwd` | Vigila ese archivo |
| `-p wa` | Avisa de escrituras (`w`) y cambios de atributos (`a`) |
| `-k identity` | Etiqueta para buscar luego |
| `ausearch -k …` | Busca eventos por etiqueta |

Herramientas relacionadas: **AIDE** (comprueba la integridad de archivos importantes comparando con una huella inicial), `debsums -c` (verifica los paquetes instalados), **rkhunter** / **chkrootkit** (buscan rastros conocidos).

## 9. Qué mirar si sospechas de una intrusión

Señales: entradas `Accepted` desconocidas, un usuario nuevo, procesos extraños, puertos nuevos escuchando, uso anómalo de CPU o red, archivos modificados que no tocaste.

```bash
who -a
last -i -n 30
sudo ss -tnp
ps auxf
sudo crontab -l; ls /etc/cron.d /etc/cron.daily
systemctl list-unit-files --state=enabled | head -40
cat ~/.ssh/authorized_keys
sudo find / -xdev -mtime -2 -type f -not -path "/proc/*" 2>/dev/null | head -40
```

| Comando | Qué buscas |
|---|---|
| `who -a`, `last -i` | Sesiones que no son tuyas |
| `ss -tnp` | Conexiones salientes raras |
| `ps auxf` | Procesos que no reconoces (el árbol `f` muestra de dónde cuelgan) |
| `crontab -l`, `/etc/cron.*` | Tareas programadas añadidas por un atacante |
| `list-unit-files --state=enabled` | Servicios nuevos |
| `authorized_keys` | Claves SSH que no son tuyas |
| `find … -mtime -2` | Archivos modificados en los últimos 2 días |

### Plan de respuesta

1. **Mantén la calma y no borres nada** todavía.
2. **Aísla**: desconecta la red de la VM (VirtualBox → desmarcar el cable) o apaga el reenvío de puertos.
3. **Preserva evidencias**: copia los registros (`sudo journalctl --no-pager > evidencia.txt`), haz una **instantánea** de la VM.
4. **Identifica el vector**: ¿contraseña débil?, ¿servicio expuesto?, ¿software sin parchear?
5. **Cambia las credenciales** (claves SSH, contraseñas, tokens, secretos de `.env`) **desde un equipo limpio**, no desde la máquina comprometida.
6. **Restaura** desde una instantánea o copia **anterior** a la intrusión, o reinstala. Un sistema comprometido no se «limpia» con garantías.
7. **Corrige** lo que lo permitió y **notifica** a quien corresponda (ver abajo).

## 10. Denunciar en lugar de contraatacar

Si descubres a quien intenta entrar:

```bash
whois 203.0.113.5 | grep -iE 'abuse|netname|country|orgname'
```

1. Anota el **origen** (IP), la **hora en UTC** y las líneas de registro (cópialas tal cual).
2. `whois` te da el contacto de abusos (`abuse@…`) del proveedor que gestiona esa IP.
3. Envíale un correo con los registros: los proveedores serios actúan sobre sus clientes comprometidos o malintencionados.
4. Si hubo acceso real o daño, informa al CERT nacional (en España, INCIBE-CERT) y, si procede, denuncia a las autoridades con las evidencias.
5. Bloquéalo en tu cortafuegos: `sudo ufw deny from 203.0.113.5`.

Recuerda: la IP que ves suele ser **una víctima** cuyo equipo está siendo usado. Por eso no se contraataca.

## 11. Docker y aplicaciones

- Publica puertos en `127.0.0.1` (sección 5).
- Contenedores con usuario no root (`USER node`), sistema de archivos de solo lectura cuando se pueda (`--read-only`), sin privilegios extra (`--cap-drop ALL`, nunca `--privileged`).
- El grupo `docker` es equivalente a root: añade solo a quien deba.
- Secretos en variables de entorno o *secrets*, nunca en la imagen ([Docker](#docker)).
- Actualiza imágenes base y fija versiones (no `latest`).
- Bases de datos con autenticación y sin exposición pública ([MongoDB](#mongo), [SQL](#sql)).

## 12. Copias de seguridad

Regla **3-2-1**: 3 copias, en 2 soportes distintos, 1 fuera de tu casa. Haz **instantáneas** de la VM antes de cambios importantes, copias de los datos (`pg_dump`, `mongodump`) y **prueba la restauración**.

## 13. Lista de comprobación

- [ ] `ufw` activo, entrada denegada por defecto, solo lo imprescindible abierto.
- [ ] SSH solo con claves, sin `root`, `AllowUsers` definido; probado con una segunda sesión.
- [ ] `fail2ban` activo con tu red en `ignoreip`.
- [ ] Servicios innecesarios desactivados; `ss -ltnp` revisado y entendido.
- [ ] Actualizaciones automáticas funcionando.
- [ ] Puertos de Docker publicados en `127.0.0.1`.
- [ ] Bases de datos y paneles no expuestos a Internet.
- [ ] Registros revisados (o un aviso automático) al menos semanalmente.
- [ ] Instantánea y copia de seguridad recientes, restauración probada.
- [ ] Sin secretos en repositorios.

## 14. Errores habituales

| Síntoma | Causa | Solución |
|---|---|---|
| Te quedas fuera de SSH | Desactivaste contraseñas sin clave instalada, o `ufw enable` sin permitir SSH | Consola de VirtualBox; restaura la instantánea |
| `fail2ban` te bloquea a ti | Falta tu red en `ignoreip` | `fail2ban-client set sshd unbanip TU_IP` |
| `sshd -t` da error | Sintaxis incorrecta en el archivo | Lee la línea indicada |
| Un puerto está abierto «aunque `ufw` lo deniega» | Lo publica Docker | `127.0.0.1:` en `-p` |
| `lastb` vacío o sin permiso | Necesita `sudo` y puede no existir el archivo | `sudo lastb`; usa `journalctl` |
| Muchos intentos de fuerza bruta | Es normal en Internet para cualquier IP pública | Solo claves + `fail2ban`; no es una intrusión |
| El analizador no detecta nada | Formato de registro distinto | Ajusta las expresiones regulares a tu `journalctl` |

## 15. Ejercicios

> **Ejercicio:** haz una instantánea de la VM y aplica la configuración SSH de la sección 4 siguiendo el orden exacto. Documenta qué pasa si pruebas a entrar con contraseña.

> **Ejercicio:** activa `ufw` con la política de este módulo y comprueba con otro equipo (o tu PC) qué puertos son alcanzables antes y después.

> **Ejercicio:** ejecuta `lynis audit system`, elige las tres recomendaciones de mayor impacto y aplícalas.

> **Ejercicio:** modifica `auth_failures.py` para que genere un archivo con las IP a bloquear (`ufw deny from …`) y revísalo a mano antes de ejecutarlo.

> **Ejercicio:** simula una intrusión de forma controlada en una VM de laboratorio (añade una clave extra a `authorized_keys` y una tarea `cron`) y recórrela con la lista de la sección 9 para encontrar ambas.

## 16. Documentación oficial

- [Ubuntu Server: seguridad](https://documentation.ubuntu.com/server/explanation/intro-to/security/)
- [OpenSSH: sshd_config](https://man.openbsd.org/sshd_config)
- [UFW](https://documentation.ubuntu.com/server/how-to/security/firewalls/)
- [fail2ban](https://fail2ban.readthedocs.io/)
- [Lynis](https://cisofy.com/lynis/)
- [Docker y cortafuegos](https://docs.docker.com/engine/network/packet-filtering-firewalls/)
