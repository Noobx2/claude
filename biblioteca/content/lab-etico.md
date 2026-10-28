# Laboratorio de hacking ético

El **hacking ético** consiste en poner a prueba la seguridad de un sistema **con permiso**, para encontrar sus fallos antes de que lo haga un atacante real y corregirlos. Aprender cómo se ataca es la mejor forma de aprender a defender, siempre que sea en un entorno que te pertenece.

*Verificado: 2026-10-27 en parte. Los programas de demostración (inyección SQL, recorrido de rutas, XSS, velocidad de hashes) se ejecutaron en un laboratorio local con datos de ejemplo. Los comandos de herramientas externas (`nmap`, contenedores de práctica) siguen su documentación oficial pero no se han ejecutado aquí. Referencia: [OWASP](https://owasp.org/), [PortSwigger Web Security Academy](https://portswigger.net/web-security).*

> **Regla de oro:** solo se prueba **lo tuyo** o aquello para lo que tienes **autorización escrita** (alcance, fechas, qué está permitido). Escanear, probar contraseñas o explotar fallos en sistemas ajenos es delito aunque «solo mirases». Si dudas de si algo es tuyo, no lo toques. Detalle en [Marco ético](#seguridad-marco).

## 1. Qué aprenderás (y qué no)

| Sí | No |
|---|---|
| Reconocer las clases de fallo más habituales y **por qué ocurren** | Explotar sistemas de terceros |
| Probar tus propias aplicaciones y tu red doméstica | Atacar a quien te escaneó o intentó entrar |
| Practicar en aplicaciones **vulnerables a propósito** | Usar credenciales filtradas o datos robados |
| Escribir informes con hallazgos y correcciones | Distribuir malware o ejecutar ataques de denegación de servicio |
| Ver tu red desde el lado del defensor (qué dejan de huella los escaneos) | Ocultar tu actividad en redes ajenas |

## 2. Montar un laboratorio aislado

```text
 PC anfitrión
 └─ VirtualBox
     ├─ VM "defensor"  (tu Ubuntu actual: servicios, registros, cortafuegos)
     ├─ VM "objetivo"  (aplicaciones vulnerables a propósito, SIN datos reales)
     └─ Red "solo anfitrión" (host-only) o "red interna": sin salida a Internet ni a tu LAN
```

1. **Red aislada**: en VirtualBox, adaptador *Solo anfitrión* o *Red interna*. Lo que ocurre ahí no puede salir.
2. **Instantáneas** antes de cada práctica: vuelves al estado limpio en segundos.
3. **Nada real**: ni contraseñas, ni claves, ni datos personales tuyos o de nadie.
4. **Objetivos vulnerables a propósito** (contenedores o VM):

| Objetivo | Para qué | Cómo (en la VM objetivo) |
|---|---|---|
| **OWASP Juice Shop** | Tienda web con cientos de fallos para practicar el Top 10 | `docker run --rm -p 127.0.0.1:3001:3000 bkimminich/juice-shop` |
| **DVWA** | Aplicación PHP con fallos clásicos y niveles de dificultad | Imagen de Docker de la comunidad (comprueba su documentación) |
| **WebGoat** | Lecciones guiadas de OWASP (Java) | Ver la guía oficial |
| **OverTheWire (Bandit)** | Retos de terminal de Linux | Se juega por SSH en su servidor, con su permiso |

5. **Plataformas legales para practicar**: PortSwigger Web Security Academy (gratuita), TryHackMe, Hack The Box, picoCTF. Están diseñadas para eso.

## 3. Metodología de una prueba

| Fase | Objetivo | Ejemplos (en tu laboratorio) |
|---|---|---|
| 1. **Alcance** | Qué se prueba, qué no, cuándo | Por escrito, incluso en tu laboratorio, como hábito |
| 2. **Reconocimiento** | Qué hay: equipos, puertos, tecnologías | `ip neigh`, `nmap`, mirar cabeceras con `curl -I` |
| 3. **Enumeración** | Versiones, rutas, usuarios, parámetros | Navegar la aplicación, DevTools |
| 4. **Análisis** | ¿Qué fallos son plausibles? | Comparar versiones con avisos conocidos |
| 5. **Prueba** | Confirmar el fallo **de forma controlada** | Una entrada de prueba, sin dañar datos |
| 6. **Informe** | Explicar, ordenar por riesgo, proponer solución | Ver la sección 8 |
| 7. **Corrección y repetición** | Arreglar y volver a probar | El paso que más importa |

## 4. Descubrir tu propia red

Sirve para saber qué dispositivos hay en **tu** red doméstica y qué exponen. `nmap` es la herramienta estándar.

```bash
sudo apt install nmap
nmap -sn 192.168.1.0/24
nmap -sV -p 1-1024 192.168.56.101
nmap -sT --top-ports 100 -oN resultado.txt 192.168.56.101
```

| Flag | Significado |
|---|---|
| `-sn` | Solo descubrimiento de equipos (*ping scan*), sin escanear puertos |
| `192.168.1.0/24` | Toda tu red local (256 direcciones). **Tu red, no otra** |
| `-p 1-1024` | Rango de puertos (`-p-` = los 65535; `--top-ports 100` = los 100 más comunes) |
| `-sV` | Intenta identificar la versión del servicio que responde |
| `-sT` | Conexión TCP completa (no requiere privilegios especiales) |
| `-oN archivo` | Guarda el resultado en texto |
| Estados de un puerto | `open` (hay un servicio), `closed` (accesible pero sin servicio), `filtered` (un cortafuegos descarta los paquetes) |

> **Peligro:** `nmap` contra una dirección que no es tuya puede ser ilegal y delata tu IP. Úsalo solo en tu LAN, tu laboratorio o con autorización. **Sé también el defensor:** mientras escaneas tu VM, observa en `tcpdump` ([Monitorización](#monitoreo)) y en los registros ([Endurecimiento](#hardening)) cómo se ve el escaneo desde el otro lado; es lo que verás cuando alguien te escanee a ti.

Qué debería sorprenderte: puertos abiertos que no conocías, versiones antiguas de servicios, dispositivos desconocidos (una cámara, una tele) y bases de datos accesibles desde la red.

## 5. Fallos clásicos y su corrección

Los fallos más habituales en aplicaciones web están recogidos en el **OWASP Top 10** (consulta la edición vigente en owasp.org). La mejor forma de entenderlos es ver el fallo y su arreglo **en código tuyo**:

```js
// vulnerabilities.mjs: tres fallos clásicos (OWASP) en una app de PRÁCTICA y su corrección.
// Se ejecuta solo contra TU base de datos de laboratorio, con datos de ejemplo.
import { mkdirSync, readFileSync, writeFileSync } from 'node:fs';
import { join, resolve, sep } from 'node:path';
import pg from 'pg';

const pool = new pg.Pool({ connectionString: process.env.DATABASE_URL });

// ───────────────────────── 1. Inyección SQL ─────────────────────────
// VULNERABLE: el texto del usuario se concatena dentro de la consulta
async function findEmployeeVulnerable(name) {
  const { rows } = await pool.query(`SELECT name, salary FROM employees WHERE name = '${name}'`);
  return rows;
}
// CORRECTO: consulta parametrizada; el valor nunca se interpreta como SQL
async function findEmployeeSafe(name) {
  const { rows } = await pool.query('SELECT name, salary FROM employees WHERE name = $1', [name]);
  return rows;
}

const attack = "' OR '1'='1";
console.log('Vulnerable con entrada normal :', (await findEmployeeVulnerable('Ana')).length, 'fila');
console.log('Vulnerable con entrada hostil :', (await findEmployeeVulnerable(attack)).length, 'filas  <-- devuelve TODA la tabla');
console.log('Corregido con entrada hostil  :', (await findEmployeeSafe(attack)).length, 'filas');

// ───────────────────────── 2. Recorrido de rutas (path traversal) ─────────────────────────
const base = resolve('public-files');
mkdirSync(base, { recursive: true });
writeFileSync(join(base, 'hello.txt'), 'archivo público');
writeFileSync(resolve('secret.txt'), 'archivo PRIVADO');

// VULNERABLE: confía en el nombre que llega
function readFileVulnerable(fileName) {
  return readFileSync(join(base, fileName), 'utf8');
}
// CORRECTO: resuelve la ruta final y comprueba que sigue DENTRO de la carpeta permitida
function readFileSafe(fileName) {
  const target = resolve(base, fileName);
  if (!target.startsWith(base + sep)) throw new Error('Ruta fuera de la carpeta permitida');
  return readFileSync(target, 'utf8');
}
console.log('Vulnerable con ../secret.txt :', readFileVulnerable('../secret.txt'));
try {
  readFileSafe('../secret.txt');
} catch (error) {
  console.log('Corregido con ../secret.txt  :', error.message);
}
console.log('Corregido con hello.txt      :', readFileSafe('hello.txt'));

// ───────────────────────── 3. XSS: escapar la salida ─────────────────────────
const comment = '<img src=x onerror="alert(1)">';
const escapeHtml = (text) =>
  text.replace(/[&<>"']/g, (c) => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' })[c]);
console.log('Sin escapar  :', `<p>${comment}</p>`);
console.log('Escapado     :', `<p>${escapeHtml(comment)}</p>`);

await pool.end();
```

Resultado real:

```text
Vulnerable con entrada normal : 1 fila
Vulnerable con entrada hostil : 6 filas  <-- devuelve TODA la tabla
Corregido con entrada hostil  : 0 filas
Vulnerable con ../secret.txt : archivo PRIVADO
Corregido con ../secret.txt  : Ruta fuera de la carpeta permitida
Corregido con hello.txt      : archivo público
Sin escapar  : <p><img src=x onerror="alert(1)"></p>
Escapado     : <p>&lt;img src=x onerror=&quot;alert(1)&quot;&gt;</p>
```

| Fallo | Qué ocurre | Por qué | Corrección |
|---|---|---|---|
| **Inyección SQL** | La entrada del usuario cambia la **estructura** de la consulta (`' OR '1'='1` hace que la condición siempre sea cierta) | Se concatenó texto en la consulta | **Consultas parametrizadas** siempre ([SQL](#sql)); mínimo privilegio en el usuario de la base |
| **Recorrido de rutas** | `../secret.txt` sale de la carpeta pública | Se confió en el nombre del archivo | Resolver la ruta final y verificar que sigue dentro de la carpeta permitida |
| **XSS** (*cross-site scripting*) | Un texto del usuario se interpreta como HTML/JS en el navegador de otro | Se insertó sin escapar | Escapar la salida; `textContent` en lugar de `innerHTML` ([JavaScript](#js)); cabecera `Content-Security-Policy` |

### Otras clases a conocer

| Clase | Idea | Defensa |
|---|---|---|
| **Control de acceso roto** (IDOR) | Cambias `/api/tasks/7` por `/api/tasks/8` y ves la tarea de otro usuario | Comprobar en el servidor que el recurso pertenece al usuario autenticado, en **cada** petición |
| **Fallos criptográficos** | Datos sensibles sin cifrar, hashes débiles, HTTP | TLS, Argon2id, AES-GCM ([Criptografía](#cripto)) |
| **Autenticación débil** | Contraseñas triviales, sin límite de intentos, sesiones que no caducan | Longitud mínima, límite de intentos, MFA, cookies `HttpOnly; Secure; SameSite` |
| **Configuración insegura** | Paneles de administración abiertos, errores con trazas, credenciales por defecto | Revisión de configuración; entornos separados ([Endurecimiento](#hardening)) |
| **Componentes vulnerables** | Dependencias con fallos conocidos | `pnpm audit`, `pip-audit`, `cargo audit`, actualizar |
| **CSRF** | Un sitio hace que tu navegador envíe una petición con tu sesión | Cookies `SameSite`, token anti-CSRF |
| **SSRF** | Haces que el servidor pida una URL interna por ti | Lista blanca de destinos, bloquear redes internas |
| **Registros insuficientes** | No te enteras de que te atacan | Registrar autenticaciones y errores; alertas ([Monitorización](#monitoreo)) |

### Por qué se guardan las contraseñas con un hash lento

Un atacante que roba una base de datos prueba contraseñas contra los hashes **sin conexión**, lo más rápido que pueda. Medido en tu propia máquina:

```python
"""Mide cuántas contraseñas por segundo puede probar un atacante contra distintos tipos de hash.

Es un ejercicio de DEFENSA: demuestra por qué las contraseñas se guardan con hashes lentos.
Todo ocurre en tu máquina con datos de ejemplo.
"""

import hashlib
import time

from argon2 import PasswordHasher


def per_second(function, seconds: float = 1.0) -> float:
    """Ejecuta `function` durante ~`seconds` segundos y devuelve cuántas veces por segundo."""
    count = 0
    start = time.perf_counter()
    while time.perf_counter() - start < seconds:
        function()
        count += 1
    return count / (time.perf_counter() - start)


def main() -> None:
    candidate = b"correcto-caballo-bateria"
    argon = PasswordHasher()
    stored = argon.hash("x")  # un hash ya calculado para medir solo la verificación

    results = {
        "SHA-256 (MAL para contraseñas)": per_second(
            lambda: hashlib.sha256(candidate).digest()
        ),
        "scrypt n=2**14": per_second(
            lambda: hashlib.scrypt(
                candidate, salt=b"0123456789abcdef", n=2**14, r=8, p=1
            ),
            2,
        ),
        "Argon2id (valores por defecto)": per_second(
            lambda: argon.verify(stored, "x") or None, 2
        ),
    }
    baseline = results["SHA-256 (MAL para contraseñas)"]
    print(f"{'algoritmo':36} {'intentos/s':>14}  {'más lento que SHA-256':>22}")
    for name, rate in results.items():
        print(f"{name:36} {rate:>14,.0f}  {baseline / rate:>21,.0f}x")


if __name__ == "__main__":
    main()
```

Resultado real en un equipo de pruebas (los números dependen de tu hardware; la proporción es lo importante):

```text
algoritmo                                intentos/s   más lento que SHA-256
SHA-256 (MAL para contraseñas)              983,491                      1x
scrypt n=2**14                                   18                 54,956x
Argon2id (valores por defecto)                   10                 94,342x
```

Contra un hash rápido, un atacante con tarjetas gráficas prueba miles de millones por segundo; contra Argon2 o scrypt, el coste lo vuelve inviable. Esta es la razón de la regla de [Criptografía](#cripto).

## 6. Auditar tus propios proyectos

| Qué | Herramienta | Comando de ejemplo |
|---|---|---|
| Dependencias vulnerables (Node) | `pnpm audit` / `npm audit` | `pnpm audit --prod` |
| Dependencias vulnerables (Python) | `pip-audit` | `uv run pip-audit` (tras `uv add --dev pip-audit`) |
| Dependencias vulnerables (Rust) | `cargo audit` | `cargo install cargo-audit && cargo audit` |
| Dependencias vulnerables (Java) | OWASP Dependency-Check | `mvn org.owasp:dependency-check-maven:check` |
| Imágenes de Docker | Trivy, Docker Scout | `trivy image tasks-api:dev` |
| Secretos en el repositorio | `gitleaks` | `gitleaks detect` |
| Análisis estático de código | Semgrep, SonarQube | `semgrep scan` |
| Aplicación web en marcha (tuya) | OWASP ZAP | `docker run -t ghcr.io/zaproxy/zaproxy:stable zap-baseline.py -t http://HOST:3000` |
| Tu servidor | Lynis | `sudo lynis audit system` |

El análisis dinámico (ZAP, Burp Suite Community) se apunta **solo a tu aplicación de prueba local**.

## 7. Cómo se ve un ataque desde la defensa

Ejercicio muy formativo: ataca tu propio laboratorio y mira qué rastro deja.

| Si haces | Dónde se ve |
|---|---|
| `nmap` contra la VM | `tcpdump` (muchos SYN a puertos distintos desde una IP), registros `UFW BLOCK`, alertas de un IDS |
| Muchos inicios de sesión SSH fallidos | `journalctl -u ssh`, `auth_failures.py` ([Endurecimiento](#hardening)), `fail2ban-client status sshd` |
| Peticiones con `' OR 1=1` a tu API | Registros de la aplicación y de la base de datos (consultas anómalas, errores `syntax error`) |
| Acceso a rutas inexistentes (`/admin`, `/.env`, `/wp-login.php`) | Registros de acceso del servidor web: muchos `404` desde una IP |

## 8. El informe

El resultado de una prueba es un **documento útil**:

1. **Resumen ejecutivo**: qué se hizo y la conclusión, en un párrafo.
2. **Alcance y fechas**.
3. **Hallazgos**, cada uno con: título, **severidad** (crítica, alta, media, baja), descripción, **evidencia** (capturas, peticiones), **impacto** y **recomendación concreta**.
4. **Correcciones aplicadas** y comprobación de que ya no ocurre.
5. Anexos: herramientas y versiones usadas.

Severidad = probabilidad × impacto. Un fallo que expone todos los datos de usuarios es crítico; un mensaje de error con la versión del servidor, bajo.

## 9. Errores habituales

| Error | Consecuencia | Correcto |
|---|---|---|
| Probar sin permiso «solo para ver» | Delito | Alcance escrito, laboratorio propio |
| Laboratorio con salida a tu LAN o a Internet | Una VM vulnerable queda expuesta | Red interna / solo anfitrión |
| Usar datos reales en el laboratorio | Fuga | Datos de ejemplo |
| Quedarse en «funciona el ataque» | No aprendes la defensa | Corrige y vuelve a probar |
| Ejecutar herramientas sin entenderlas | Daños o ruido | Lee los flags (`--help`, `man`) antes |
| No tomar instantáneas | Pierdes horas | Instantánea antes de cada práctica |
| Confiar solo en un escáner automático | Falsos positivos y negativos | Revisión manual |

## 10. Ejercicios

> **Ejercicio:** monta Juice Shop en una VM sin salida a Internet y encuentra la puntuación («Score Board») de la aplicación. Anota qué clase de fallo corresponde a cada reto y cómo se corrige.

> **Ejercicio:** programa una función con una inyección SQL a propósito en tu API de tareas, escribe una prueba que la explote y luego corrígela con parámetros hasta que la prueba pase a verde.

> **Ejercicio:** haz `nmap -sn` de tu red doméstica y crea el inventario de tus dispositivos. ¿Hay alguno que no reconozcas? ¿Qué puertos expone cada uno?

> **Ejercicio:** escanea tu VM con `nmap -sV -p-` y compara el resultado con `ss -ltnp` dentro de la VM. ¿Qué ve el exterior que tú no esperabas?

> **Ejercicio:** escribe el informe de una de las pruebas anteriores con el formato de la sección 8.

## 11. Documentación oficial

- [OWASP Top 10](https://owasp.org/Top10/)
- [OWASP Cheat Sheet Series](https://cheatsheetseries.owasp.org/)
- [PortSwigger Web Security Academy](https://portswigger.net/web-security)
- [Nmap: guía de referencia](https://nmap.org/book/man.html)
- [OWASP Juice Shop](https://owasp.org/www-project-juice-shop/)
- [OWASP ZAP](https://www.zaproxy.org/docs/)
