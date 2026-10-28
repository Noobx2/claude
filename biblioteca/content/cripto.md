# Criptografía aplicada

La criptografía protege datos con matemáticas: que nadie los lea (**confidencialidad**), que nadie los altere sin que se note (**integridad**) y que se sepa quién los creó (**autenticidad**). Este módulo enseña a **usar** bien las herramientas, no a inventarlas.

*Verificado: 2026-10-27. Todo el código se ejecutó (Python 3.13 con `cryptography` 50 y `argon2-cffi`, Node 22 con `node:crypto`, OpenSSL 3.0) y las pruebas pasan. Referencia: [Cryptography (Python)](https://cryptography.io/), [Node: crypto](https://nodejs.org/api/crypto.html), [OWASP: almacenamiento de contraseñas](https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html).*

> **Regla de oro:** **nunca** inventes tu propio algoritmo ni programes primitivas criptográficas desde cero. Usa las librerías estándar del lenguaje, con modos *autenticados* (AES-GCM, ChaCha20-Poly1305) y valores por defecto seguros. La mayoría de los fallos reales vienen de **usar mal** un algoritmo correcto (nonce repetido, clave débil, sin autenticación), no de que el algoritmo se rompa.

## 1. El mapa de herramientas

| Necesidad | Herramienta | Qué no usar |
|---|---|---|
| Huella de un dato (integridad) | **Hash**: SHA-256, SHA-3, BLAKE2 | MD5 y SHA-1 (rotos) |
| Integridad + autenticidad con secreto compartido | **HMAC** | Hash a secas |
| Guardar contraseñas | **Argon2id**, scrypt, bcrypt | SHA-256 a secas, MD5, cifrado reversible |
| Cifrar datos con una clave compartida | **AES-256-GCM**, ChaCha20-Poly1305 (cifrado *autenticado*) | AES-ECB, CBC sin autenticación, DES |
| Acordar claves / cifrar sin compartir secreto | **Criptografía asimétrica**: X25519, RSA-OAEP | RSA < 2048 bits, RSA sin relleno |
| Firmar | **Ed25519**, ECDSA, RSA-PSS | Firmas sin verificar |
| Aleatoriedad para seguridad | `secrets` (Python), `crypto.randomBytes` (Node) | `random`, `Math.random()` |
| Canal seguro entre equipos | **TLS** ([Redes](#redes)), SSH, WireGuard | Protocolos propios |

## 2. Hash

Una función hash convierte cualquier dato en una huella de tamaño fijo. Es **unidireccional** (no se puede volver al original) y cualquier cambio mínimo altera la huella por completo.

```bash
echo -n "hola" | sha256sum
echo -n "hola" | openssl dgst -sha256
sha256sum archivo.iso
sha256sum -c SHA256SUMS
```

| Pieza | Significado |
|---|---|
| `echo -n` | Sin salto de línea final (el `\n` cambiaría el hash) |
| `sha256sum archivo` | Huella de un archivo |
| `sha256sum -c lista` | Verifica los archivos contra una lista de huellas (así se comprueba una descarga) |
| `openssl dgst -sha256` | Lo mismo con OpenSSL; `-hmac clave` calcula un HMAC |

Usos: verificar descargas, detectar cambios en archivos, identificar contenido. **No sirve** por sí solo para autenticar (cualquiera puede recalcularlo) ni para contraseñas.

## 3. Todo en código

```python
"""Criptografía práctica con librerías estándar y bien revisadas. NO inventes tus propios algoritmos."""

import hashlib
import hmac
import secrets

from argon2 import PasswordHasher
from argon2.exceptions import VerifyMismatchError
from cryptography.exceptions import InvalidSignature, InvalidTag
from cryptography.hazmat.primitives.asymmetric.ed25519 import Ed25519PrivateKey
from cryptography.hazmat.primitives.ciphers.aead import AESGCM

# 1. Hash: huella de tamaño fijo. Mismo dato -> mismo hash; un cambio mínimo -> hash totalmente distinto
print(hashlib.sha256(b"hola").hexdigest())
print(hashlib.sha256(b"Hola").hexdigest())

# 2. HMAC: hash con clave secreta. Demuestra integridad Y que lo creó quien conoce la clave
key = secrets.token_bytes(32)  # secrets: aleatoriedad criptográfica (NO uses random)
tag = hmac.new(key, b"pedido=42", hashlib.sha256).digest()
valid = hmac.compare_digest(
    tag, hmac.new(key, b"pedido=42", hashlib.sha256).digest()
)  # comparación en tiempo constante
tampered = hmac.compare_digest(
    tag, hmac.new(key, b"pedido=43", hashlib.sha256).digest()
)
print(valid, tampered)

# 3. Contraseñas: hash LENTO y con sal (Argon2). Jamás SHA-256 a secas ni cifrado reversible
hasher = PasswordHasher()
stored = hasher.hash(
    "correcto-caballo-bateria"
)  # incluye sal y parámetros dentro del texto
print(stored.split("$")[1:3])  # algoritmo y versión
print(hasher.verify(stored, "correcto-caballo-bateria"))
try:
    hasher.verify(stored, "otra-contraseña")
except VerifyMismatchError:
    print("contraseña incorrecta")

# 4. Cifrado simétrico autenticado (AES-256-GCM): confidencialidad + detección de manipulación
aes_key = AESGCM.generate_key(bit_length=256)
aesgcm = AESGCM(aes_key)
nonce = secrets.token_bytes(12)  # único por mensaje con la misma clave; nunca repetirlo
ciphertext = aesgcm.encrypt(nonce, b"mensaje secreto", b"cabecera-visible")
print(aesgcm.decrypt(nonce, ciphertext, b"cabecera-visible"))
corrupted = bytearray(ciphertext)
corrupted[0] ^= 1  # un solo bit alterado
try:
    aesgcm.decrypt(nonce, bytes(corrupted), b"cabecera-visible")
except InvalidTag:
    print("manipulación detectada")

# 5. Firma digital (Ed25519): la clave privada firma, la pública verifica
private_key = Ed25519PrivateKey.generate()
public_key = private_key.public_key()
signature = private_key.sign(b"documento")
public_key.verify(signature, b"documento")  # no lanza excepción: es válida
try:
    public_key.verify(signature, b"documento alterado")
except InvalidSignature:
    print("firma inválida")
```

Y lo mismo con `node:crypto`, sin instalar nada:

```js
// crypto-demo.mjs: criptografía con node:crypto (sin dependencias)
import {
  createCipheriv, createDecipheriv, createHash, createHmac, generateKeyPairSync,
  randomBytes, scrypt, sign, timingSafeEqual, verify,
} from 'node:crypto';
import { promisify } from 'node:util';

const scryptAsync = promisify(scrypt);

// 1. Hash
console.log(createHash('sha256').update('hola').digest('hex'));

// 2. HMAC con comparación en tiempo constante
const key = randomBytes(32);
const tag = createHmac('sha256', key).update('pedido=42').digest();
const check = createHmac('sha256', key).update('pedido=42').digest();
console.log(timingSafeEqual(tag, check));

// 3. Contraseñas con scrypt (incluido en Node): hash lento, con sal aleatoria
async function hashPassword(password) {
  const salt = randomBytes(16);
  const derived = await scryptAsync(password, salt, 64);
  return `${salt.toString('hex')}:${derived.toString('hex')}`;
}
async function verifyPassword(password, stored) {
  const [saltHex, hashHex] = stored.split(':');
  const derived = await scryptAsync(password, Buffer.from(saltHex, 'hex'), 64);
  return timingSafeEqual(derived, Buffer.from(hashHex, 'hex'));
}
const stored = await hashPassword('correcto-caballo-bateria');
console.log(await verifyPassword('correcto-caballo-bateria', stored), await verifyPassword('otra', stored));

// 4. AES-256-GCM: cifrado autenticado
const aesKey = randomBytes(32);
const iv = randomBytes(12); // nunca repetir con la misma clave
const cipher = createCipheriv('aes-256-gcm', aesKey, iv);
const encrypted = Buffer.concat([cipher.update('mensaje secreto', 'utf8'), cipher.final()]);
const authTag = cipher.getAuthTag();

const decipher = createDecipheriv('aes-256-gcm', aesKey, iv);
decipher.setAuthTag(authTag);
console.log(Buffer.concat([decipher.update(encrypted), decipher.final()]).toString());

encrypted[0] ^= 1; // manipulación
const tampered = createDecipheriv('aes-256-gcm', aesKey, iv);
tampered.setAuthTag(authTag);
try {
  Buffer.concat([tampered.update(encrypted), tampered.final()]);
} catch {
  console.log('manipulación detectada');
}

// 5. Firma Ed25519
const { publicKey, privateKey } = generateKeyPairSync('ed25519');
const signature = sign(null, Buffer.from('documento'), privateKey);
console.log(verify(null, Buffer.from('documento'), publicKey, signature), verify(null, Buffer.from('alterado'), publicKey, signature));
```

### Qué ocurre en cada bloque

| Bloque | Idea clave |
|---|---|
| **Hash** | `sha256("hola")` y `sha256("Hola")` no se parecen en nada |
| **HMAC** | Solo quien tiene la clave puede generar una etiqueta válida. Se compara con `hmac.compare_digest` / `timingSafeEqual` para evitar *ataques de temporización* |
| **Contraseñas** | Hash **lento** y con **sal** única: Argon2id o scrypt. La sal evita que dos usuarios con la misma contraseña tengan el mismo hash y arruina las tablas precalculadas |
| **AES-GCM** | Cifra **y** autentica. El *nonce* (12 bytes) debe ser **único por mensaje con la misma clave**: repetirlo destruye la seguridad. Un bit alterado hace fallar el descifrado |
| **Ed25519** | La clave **privada** firma; la **pública** verifica. La pública se puede compartir |

## 4. Contraseñas de usuarios

```text
registro:   contraseña --(Argon2id + sal aleatoria)--> hash  --> se guarda el hash (con sal y parámetros)
inicio:     contraseña --(mismo cálculo con la sal guardada)--> ¿coincide con el hash guardado?
```

Reglas:

1. **Nunca** guardes la contraseña ni una versión cifrada: solo su hash lento.
2. Usa la librería de tu plataforma: `argon2-cffi` (Python), `scrypt` incorporado en Node, `Isopoh`/`Konscious` o `Rfc2898DeriveBytes` en .NET, `BCryptPasswordEncoder`/`Argon2PasswordEncoder` en Spring, `golang.org/x/crypto/argon2` en Go.
3. Límite de intentos y bloqueo temporal en el inicio de sesión.
4. Exige longitud (≥ 12 caracteres) antes que reglas rebuscadas; permite pegar contraseñas (gestores).
5. Ofrece segundo factor (TOTP, llaves FIDO2).

## 5. Cifrar un archivo con contraseña

Un programa completo y pequeño. Muestra cómo se combinan los ingredientes:

```python
"""Cifra y descifra archivos pequeños con una contraseña.

Formato del archivo cifrado:
    MAGIC (4 B) | sal (16 B) | nonce (12 B) | datos cifrados + etiqueta de autenticación
La clave se deriva de la contraseña con scrypt (lento a propósito) y los datos se cifran con AES-256-GCM.
Uso:
    python file_crypt.py encrypt secreto.txt        -> crea secreto.txt.enc
    python file_crypt.py decrypt secreto.txt.enc    -> crea secreto.txt.dec
"""

import argparse
import getpass
import secrets
from pathlib import Path

from cryptography.exceptions import InvalidTag
from cryptography.hazmat.primitives.ciphers.aead import AESGCM
from cryptography.hazmat.primitives.kdf.scrypt import Scrypt

MAGIC = b"FCR1"
SALT_SIZE = 16
NONCE_SIZE = 12


def derive_key(password: str, salt: bytes) -> bytes:
    """Convierte una contraseña en una clave de 256 bits. n=2**15 hace el cálculo costoso para un atacante."""
    return Scrypt(salt=salt, length=32, n=2**15, r=8, p=1).derive(
        password.encode("utf-8")
    )


def encrypt_bytes(data: bytes, password: str) -> bytes:
    salt = secrets.token_bytes(SALT_SIZE)
    nonce = secrets.token_bytes(NONCE_SIZE)
    ciphertext = AESGCM(derive_key(password, salt)).encrypt(nonce, data, MAGIC)
    return MAGIC + salt + nonce + ciphertext


def decrypt_bytes(blob: bytes, password: str) -> bytes:
    if not blob.startswith(MAGIC):
        raise ValueError("No es un archivo cifrado por este programa")
    salt = blob[4 : 4 + SALT_SIZE]
    nonce = blob[4 + SALT_SIZE : 4 + SALT_SIZE + NONCE_SIZE]
    ciphertext = blob[4 + SALT_SIZE + NONCE_SIZE :]
    try:
        return AESGCM(derive_key(password, salt)).decrypt(nonce, ciphertext, MAGIC)
    except InvalidTag as error:
        # Contraseña incorrecta O archivo manipulado: GCM no distingue los dos casos
        raise ValueError("Contraseña incorrecta o archivo dañado") from error


def main() -> None:
    parser = argparse.ArgumentParser(description="Cifrado de archivos con contraseña")
    parser.add_argument("action", choices=["encrypt", "decrypt"])
    parser.add_argument("path", type=Path)
    args = parser.parse_args()

    password = getpass.getpass(
        "Contraseña: "
    )  # no se muestra ni se guarda en el historial
    if args.action == "encrypt":
        args.path.with_name(args.path.name + ".enc").write_bytes(
            encrypt_bytes(args.path.read_bytes(), password)
        )
    else:
        args.path.with_name(args.path.name + ".dec").write_bytes(
            decrypt_bytes(args.path.read_bytes(), password)
        )


if __name__ == "__main__":
    main()
```

```python
import pytest

from file_crypt import decrypt_bytes, encrypt_bytes


def test_roundtrip() -> None:
    blob = encrypt_bytes(b"datos importantes", "clave-larga-1")
    assert decrypt_bytes(blob, "clave-larga-1") == b"datos importantes"


def test_same_data_gives_different_ciphertext() -> None:
    assert encrypt_bytes(b"x", "clave") != encrypt_bytes(
        b"x", "clave"
    )  # sal y nonce aleatorios


def test_wrong_password() -> None:
    blob = encrypt_bytes(b"datos", "correcta")
    with pytest.raises(ValueError):
        decrypt_bytes(blob, "incorrecta")


def test_tampering_is_detected() -> None:
    blob = bytearray(encrypt_bytes(b"datos", "clave"))
    blob[-1] ^= 1
    with pytest.raises(ValueError):
        decrypt_bytes(bytes(blob), "clave")
```

| Pieza | Significado |
|---|---|
| `Scrypt(…, n=2**15, …)` | Función de derivación de clave: transforma una **contraseña** (débil, predecible) en una **clave** de 256 bits y es lenta a propósito |
| Sal de 16 bytes | Aleatoria y guardada junto al archivo; no es secreta |
| Nonce de 12 bytes | Aleatorio por archivo; se guarda al lado |
| `MAGIC` como *datos asociados* | Se autentica pero no se cifra: detecta que alguien cambió la cabecera |
| `getpass.getpass` | Pide la contraseña sin mostrarla ni dejarla en el historial de la shell |
| Un solo error para «clave mala» o «archivo dañado» | GCM no distingue ambos casos, y no conviene dar pistas |

> **Ojo:** este programa carga el archivo entero en memoria y es una demostración. Para archivos grandes se cifra por bloques (cada uno con su nonce); para uso real considera herramientas auditadas como **age** o **GnuPG**.

Con la herramienta del sistema (OpenSSL), cifrado simétrico por línea de comandos:

```bash
openssl enc -aes-256-cbc -pbkdf2 -iter 600000 -salt -in secreto.txt -out secreto.txt.enc
openssl enc -d -aes-256-cbc -pbkdf2 -iter 600000 -in secreto.txt.enc
```

| Flag | Significado |
|---|---|
| `enc -aes-256-cbc` | Cifrado AES de 256 bits en modo CBC |
| `-pbkdf2 -iter 600000` | Deriva la clave de la contraseña con PBKDF2 y ese número de iteraciones |
| `-salt` | Sal aleatoria |
| `-d` | Descifrar |
| `-in` / `-out` | Archivos de entrada y salida |

> **Ojo:** `openssl enc` con CBC **no autentica**: si alguien altera el archivo, puede descifrar basura sin avisar. Para datos importantes prefiere un modo autenticado (como el programa de arriba) o `age`/`gpg`.

## 6. Criptografía asimétrica

Dos claves relacionadas matemáticamente:

| | Clave pública | Clave privada |
|---|---|---|
| Se comparte | Sí, con todos | **Nunca** |
| Cifrar | Cualquiera cifra *para ti* | Solo tú descifras |
| Firmar | Cualquiera verifica tu firma | Solo tú firmas |

Es la base de TLS, SSH, las firmas de código y las claves de WireGuard. Se usa para **acordar una clave simétrica** (más rápida) y para **firmar**, no para cifrar grandes volúmenes.

Gestión de claves SSH (también cubierta en [Git](#git)):

```bash
ssh-keygen -t ed25519 -C "tu-correo@example.com" -f ~/.ssh/id_ed25519
ssh-keygen -lf ~/.ssh/id_ed25519.pub
chmod 600 ~/.ssh/id_ed25519
```

| Pieza | Significado |
|---|---|
| `-t ed25519` | Algoritmo moderno y rápido |
| `-l -f archivo.pub` | Muestra la huella (*fingerprint*) de una clave |
| `chmod 600` | Solo tu usuario puede leer la clave privada (SSH la rechaza si es más abierta) |
| Frase de contraseña | Cifra la clave privada en disco: **ponla siempre** |

## 7. Gestión de secretos y claves

- Las claves y contraseñas **no van en el código ni en Git** ([Git](#git) y [PATH, variables y perfiles](#entorno-path)).
- Producción: variables de entorno inyectadas, gestor de secretos (Vault, el de tu nube) o archivos montados con permisos `600`.
- **Rotación**: cambia las claves periódicamente y siempre que sospeches una fuga.
- **Separa** las claves por entorno (desarrollo ≠ producción).
- Si se filtra una clave, **revócala y crea otra**; borrarla del repositorio no basta ([Git](#git)).

## 8. Tokens de sesión

- Un **JWT** es un token firmado (HMAC o firma asimétrica) con datos (`sub`, `exp`). Cualquiera puede *leerlo*; solo se puede *firmar* con la clave. Valida siempre la firma, la caducidad (`exp`) y el emisor (`iss`), y nunca aceptes el algoritmo `none`.
- Una **cookie de sesión** `HttpOnly; Secure; SameSite=Lax` con un identificador aleatorio guardado en el servidor es más sencilla y más fácil de revocar.
- Tiempo de vida corto + renovación; no guardes tokens en `localStorage` si puedes evitarlo (un XSS los roba).

## 9. Errores habituales

| Error | Consecuencia | Correcto |
|---|---|---|
| `random.random()` / `Math.random()` para claves, tokens o nonces | Valores predecibles | `secrets.token_bytes`, `crypto.randomBytes` |
| Guardar contraseñas con SHA-256 | Se descifran por fuerza bruta en horas | Argon2id / scrypt / bcrypt |
| Reutilizar el nonce de AES-GCM con la misma clave | Se pierde confidencialidad e integridad | Nonce aleatorio por mensaje |
| Cifrar sin autenticar (CBC suelto, ECB) | Manipulación silenciosa | AES-GCM o ChaCha20-Poly1305 |
| Comparar etiquetas con `==` | Ataques de temporización | `compare_digest` / `timingSafeEqual` |
| Secretos en el repositorio | Fuga permanente | Variables de entorno; revocar si ya se subió |
| Desactivar la verificación TLS | Cualquiera intercepta | Confiar en un certificado concreto |
| Claves de 1024 bits RSA, MD5, SHA-1, DES | Rotos | RSA ≥ 2048 (mejor Ed25519), SHA-256 o superior |
| Rodar tu propia criptografía | Casi siempre vulnerable | Librerías estándar |

## 10. Ejercicios

> **Ejercicio:** escribe un programa que calcule el SHA-256 de un archivo grande por trozos (ver [CLI, streams y binarios](#cli-args-streams)) y compruébalo contra `sha256sum`.

> **Ejercicio:** implementa el registro e inicio de sesión de usuarios en tu API de tareas con Argon2id o scrypt, con límite de intentos fallidos.

> **Ejercicio:** firma un archivo con Ed25519 y verifica la firma con la clave pública en otra máquina o en otro lenguaje (Python → Node).

> **Ejercicio:** modifica `file_crypt.py` para cifrar archivos grandes por bloques de 1 MiB, con un nonce distinto para cada bloque. ¿Cómo evitas que alguien reordene los bloques?

> **Ejercicio:** mide cuánto tarda Argon2 / scrypt con distintos parámetros y razona qué coste es razonable para tu servidor.

## 11. Documentación oficial

- [Cryptography (Python)](https://cryptography.io/en/latest/)
- [Node.js: módulo crypto](https://nodejs.org/api/crypto.html)
- [OWASP: Password Storage](https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html)
- [OWASP: Cryptographic Storage](https://cheatsheetseries.owasp.org/cheatsheets/Cryptographic_Storage_Cheat_Sheet.html)
- [age: cifrado de archivos](https://age-encryption.org/)
