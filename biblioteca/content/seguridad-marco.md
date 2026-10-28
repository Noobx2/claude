# Seguridad: marco ético y plan de estudio

Quieres aprender cifrado, redes y protocolos, monitorizar el tráfico, montar tu propia VPN, practicar hacking ético para defenderte y detectar intrusos. Todo eso encaja en una ruta de **seguridad defensiva con práctica en laboratorio propio**. Este módulo fija las reglas y ordena los temas.

*Verificado: 2026-10-27. Esto no es asesoramiento legal; la normativa varía según el país.*

## 1. Las reglas

> **Regla:** solo se prueba, escanea o ataca lo que **es tuyo** o para lo que tienes **autorización escrita** de su dueño. Esto vale también para una IP «sospechosa» o para quien te atacó primero.

| Permitido | No permitido |
|---|---|
| Atacar tus propias máquinas virtuales aisladas | Escanear o probar redes, servidores o equipos ajenos |
| Escanear tu red doméstica y tus dispositivos | Entrar en el equipo de quien te escaneó o intentó entrar |
| Practicar en plataformas diseñadas para ello (CTF, laboratorios) | Usar credenciales filtradas, aunque estén públicas |
| Registrar y bloquear conexiones hacia tus sistemas | Instalar software en equipos ajenos, aunque sea para «rastrear» |
| Denunciar a los proveedores y a las autoridades | Tomar represalias de cualquier tipo |

## 2. Por qué «contraatacar» no es una opción

Entrar en el sistema de quien te atacó es, en la mayoría de los países, **un delito en sí mismo**, aunque él haya empezado. Además es poco útil:

- Los atacantes casi nunca usan su propio equipo: utilizan **máquinas ajenas ya comprometidas**, servidores alquilados con identidad falsa o redes como Tor. Quien aparece en tus registros suele ser otra víctima.
- Un contraataque puede dañar a un tercero inocente, destruir pruebas y dejarte expuesto legalmente.
- Delata lo que sabes y te puede convertir en objetivo.

### Lo que sí puedes hacer, y es más eficaz

| Objetivo | Cómo | Módulo |
|---|---|---|
| **Detectar** quién intenta entrar | Registros de acceso, `journalctl`, `last`, `lastb`, alertas de SSH | S5 |
| **Bloquear** | Cortafuegos, `fail2ban`, solo claves SSH, cerrar puertos | S5 |
| **Observar sin tocar** | Capturar tu propio tráfico, ver intentos contra tu red | S3 |
| **Atraer y estudiar** | Un *honeypot*: un señuelo aislado que registra lo que intentan contra él, sin atacar a nadie | S7 |
| **Identificar el origen aparente** | IP, país, proveedor (ASN), huellas de herramientas en tus registros | S5 |
| **Denunciar** | Aviso de abuso al proveedor de esa IP, al CERT nacional y, si procede, a las fuerzas de seguridad, con tus registros como prueba | S5 |

La estrategia profesional es **detectar, contener, registrar y notificar**. Un honeypot es la versión legítima de «hacerles caer»: ellos eligen entrar en un sistema falso tuyo; tú no entras en ningún sitio.

## 3. Ruta de estudio

| Código | Módulo | Qué aprenderás | Práctica |
|---|---|---|---|
| S0 | Marco ético | Esta página | — |
| S1 | Redes y protocolos | Modelo TCP/IP, direcciones, puertos, DNS, HTTP, TLS; leer una captura | Observar tu propio tráfico |
| S2 | Criptografía aplicada | Hash, HMAC, cifrado simétrico y asimétrico, firmas, contraseñas, gestión de claves | Cifrar y descifrar archivos con librerías estándar |
| S3 | Monitorización de red | Capturas con `tcpdump` y Wireshark, `ss`, registros del cortafuegos | Capturar y filtrar tráfico de tu VM |
| S4 | VPN propia | Cómo funciona WireGuard, claves, rutas, acceso a tu VM desde fuera | Túnel entre tu PC y tu VM |
| S5 | Endurecimiento y detección | SSH solo con claves, cortafuegos, `fail2ban`, auditoría, comprobación de integridad | Reducir la superficie de ataque de tu VM |
| S6 | Laboratorio de hacking ético | Reconocimiento, vulnerabilidades web básicas, contraseñas débiles | Aplicaciones diseñadas para ser atacadas, en red aislada |
| S7 | Honeypots | Desplegar un señuelo aislado y leer lo que registra | Un señuelo SSH en una VM separada |

Dos reglas sobre criptografía que se repetirán:

> **Regla:** nunca inventes tu propio algoritmo de cifrado ni lo programes desde cero. Usa las librerías estándar del lenguaje (`node:crypto`, `cryptography` en Python, `System.Security.Cryptography` en C#, `crypto/*` en Go) y modos autenticados como AES-GCM.

> **Regla:** las contraseñas nunca se guardan cifradas, sino con un **hash lento y con sal** (Argon2, scrypt o bcrypt), jamás con SHA-256 a secas.

## 4. Tu punto de partida real

Los datos de tu [inventario](#inventario) ofrecen ejercicios de defensa concretos, sin tocar nada ajeno:

| Dato | Por qué importa | Pregunta para el módulo S5 |
|---|---|---|
| SSH escucha en `0.0.0.0:22` y `[::]:22` | Acepta conexiones por todas las interfaces | ¿Es alcanzable desde fuera de tu red? Depende del modo de red de la VM (NAT, puente, interna) |
| Tu usuario está en `sudo` y `docker` | `docker` equivale en la práctica a tener root | ¿Qué riesgo supone y cómo se acota? |
| `ModemManager`, `avahi-daemon`, `snapd` activos | Servicios que quizá no necesitas | ¿Cuáles puedes desactivar? |
| Máquina virtual de VirtualBox | Perfecta como laboratorio con instantáneas | ¿Cómo aislar una VM de práctica con red «solo anfitrión»? |
| Sin cortafuegos conocido | No consta en el informe | Comprueba si `ufw` o `nftables` están activos |

## 5. Laboratorio seguro

1. **Máquinas virtuales de práctica** aparte de la que usas para desarrollar.
2. **Red aislada** (solo anfitrión o red interna): lo que ocurre ahí no sale a Internet ni a tu red doméstica.
3. **Instantáneas** antes de cada experimento para poder deshacerlo.
4. Aplicaciones **hechas para ser atacadas** (como OWASP Juice Shop o DVWA) y plataformas de práctica con permiso (CTF, laboratorios formativos).
5. Nada de credenciales reales ni datos personales dentro del laboratorio.

> **Ejercicio:** escribe tres frases sobre lo que **sí** vas a hacer con la seguridad y tres sobre lo que **no**. Guárdalas: serán tu criterio cuando dudes.

> **Ejercicio:** desde tu VM, lista los puertos que escuchan con `ss -ltn` y escribe para cada uno qué servicio es y si necesitas que esté abierto.

## 6. Documentación oficial

- [OWASP: recursos de seguridad web](https://owasp.org/)
- [WireGuard](https://www.wireguard.com/)
- [Manual de Wireshark](https://www.wireshark.org/docs/)
- [Guía de endurecimiento de Ubuntu](https://ubuntu.com/security)
