# Aplicaciones de consola: argumentos, streams y binarios

Cómo hacer un programa que reciba parámetros por la línea de comandos, lea y escriba flujos de datos, y manipule archivos binarios. Los ejemplos están en los lenguajes de tu ruta para que puedas compararlos.

*Verificado: 2026-10-27. Los comandos de ejemplo suponen Linux; en tu equipo anfitrión con Visual Studio, el programa en C# se ejecuta igual, solo cambian las rutas.*

## 1. Anatomía de un programa de consola

```text
mi-programa  --name Ana  -t 2  archivo.txt   < entrada.txt   > salida.txt   2> errores.txt
└─ comando ─┘ └─ opciones ─┘  └─ posicional ─┘   └ stdin ┘       └ stdout ┘     └ stderr ┘
```

| Concepto | Qué es |
|---|---|
| **Argumento posicional** | Valor cuya posición importa: `archivo.txt` |
| **Opción / flag** | Valor con nombre: `--name Ana`, `-t 2`; los booleanos no llevan valor: `--verbose` |
| **stdin** | Entrada estándar (teclado o tubería) |
| **stdout** | Salida estándar: los **datos** |
| **stderr** | Salida de errores: los **mensajes** |
| **Código de salida** | 0 = éxito; distinto de 0 = error |

Convenciones que casi todos los programas respetan:

| Convención | Detalle |
|---|---|
| `-h`, `--help` | Muestra el uso y sale con código 0 |
| `--version` | Muestra la versión |
| Opciones cortas y largas | `-n` y `--name` |
| `--` | Marca el fin de opciones; lo que sigue son argumentos literales |
| Códigos | `0` correcto, `1` error general, `2` uso incorrecto |
| `-` como archivo | Significa «lee de stdin» |

## 2. El mismo programa en seis lenguajes

Programa `greet`: recibe `--name` (por defecto «mundo») y `--times` (por defecto 1) e imprime un saludo tantas veces como se indique.

### Node.js

```js
// greet.mjs
import { parseArgs } from 'node:util';

const { values } = parseArgs({
  options: {
    name: { type: 'string', short: 'n', default: 'mundo' },
    times: { type: 'string', short: 't', default: '1' },
    help: { type: 'boolean', short: 'h' },
  },
});

if (values.help) {
  console.log('Uso: greet [--name NOMBRE] [--times N]');
  process.exit(0);
}

const times = Number(values.times);
if (!Number.isInteger(times) || times < 1) {
  console.error('--times debe ser un entero positivo');   // el error va por stderr
  process.exit(2);
}

for (let i = 0; i < times; i += 1) {
  console.log(`Hola ${values.name}`);
}
```

```bash
node greet.mjs --name Ana -t 2
```

`process.argv` contiene los argumentos en bruto (`process.argv.slice(2)` omite `node` y el script). `parseArgs` viene en Node y evita librerías para casos sencillos.

### Python

```python
# greet.py
import argparse


def main() -> int:
    parser = argparse.ArgumentParser(description="Saluda")
    parser.add_argument("-n", "--name", default="mundo")
    parser.add_argument("-t", "--times", type=int, default=1)
    args = parser.parse_args()          # genera --help y valida tipos solo

    for _ in range(args.times):
        print(f"Hola {args.name}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
```

```bash
python3 greet.py --name Ana -t 2
```

### C# (Visual Studio o `dotnet`)

```csharp
// Program.cs (instrucciones de nivel superior; `args` ya existe)
string name = "mundo";
int times = 1;

for (int i = 0; i < args.Length; i++)
{
    switch (args[i])
    {
        case "-n" or "--name":
            name = args[++i];
            break;
        case "-t" or "--times":
            times = int.Parse(args[++i]);
            break;
        default:
            Console.Error.WriteLine($"Argumento desconocido: {args[i]}");
            return 2;
    }
}

for (int i = 0; i < times; i++)
{
    Console.WriteLine($"Hola {name}");
}
return 0;
```

```bash
dotnet run -- --name Ana -t 2        # el -- separa los argumentos de dotnet de los del programa
```

En Visual Studio, los argumentos de depuración se escriben en *Propiedades del proyecto → Depurar*. Para programas grandes existe la librería `System.CommandLine`, que genera la ayuda y valida.

### Go

```go
// main.go
package main

import (
	"flag"
	"fmt"
)

func main() {
	name := flag.String("name", "mundo", "nombre a saludar")
	times := flag.Int("times", 1, "número de saludos")
	flag.Parse() // genera -h automáticamente

	for i := 0; i < *times; i++ {
		fmt.Printf("Hola %s\n", *name)
	}
}
```

```bash
go run . -name Ana -times 2
```

### Rust

```rust
// src/main.rs
use std::env;

fn main() {
    let mut name = String::from("mundo");
    let mut times: u32 = 1;

    let mut args = env::args().skip(1); // omite el nombre del programa
    while let Some(arg) = args.next() {
        match arg.as_str() {
            "-n" | "--name" => name = args.next().expect("falta el valor de --name"),
            "-t" | "--times" => {
                times = args
                    .next()
                    .expect("falta el valor de --times")
                    .parse()
                    .expect("--times debe ser un número");
            }
            _ => eprintln!("argumento desconocido: {arg}"),
        }
    }

    for _ in 0..times {
        println!("Hola {name}");
    }
}
```

```bash
cargo run -- --name Ana -t 2
```

Para programas reales, la librería `clap` es el estándar: genera la ayuda, valida y completa.

### Java

```java
// Greet.java
public class Greet {
    public static void main(String[] args) {
        String name = "mundo";
        int times = 1;

        for (int i = 0; i < args.length; i++) {
            switch (args[i]) {
                case "-n", "--name" -> name = args[++i];
                case "-t", "--times" -> times = Integer.parseInt(args[++i]);
                default -> System.err.println("Argumento desconocido: " + args[i]);
            }
        }

        for (int i = 0; i < times; i++) {
            System.out.println("Hola " + name);
        }
    }
}
```

```bash
java Greet.java --name Ana -t 2        # ejecuta el archivo fuente directamente
```

### Comparativa

| | Argumentos en bruto | Análisis incluido | Librería habitual |
|---|---|---|---|
| Node | `process.argv` | `util.parseArgs` | commander, yargs |
| Python | `sys.argv` | `argparse` | click, typer |
| C# | `args` | no | System.CommandLine |
| Go | `os.Args` | `flag` | cobra |
| Rust | `std::env::args` | no | clap |
| Java | `args` de `main` | no | picocli |

> **Ojo:** los argumentos llegan siempre como **texto**. Conviértelos y valida su rango antes de usarlos.

## 3. Entrada interactiva y entrada por tubería

Un buen programa de consola funciona en los dos modos: pregunta si no recibe datos, y lee de la tubería si se los dan.

### Leer todas las líneas de stdin

```js
// count-lines.mjs: cuenta líneas no vacías
import { createInterface } from 'node:readline';

const lines = createInterface({ input: process.stdin });
let count = 0;
for await (const line of lines) {
  if (line.trim()) count += 1;
}
console.log(count);
```

```python
# count_lines.py
import sys

count = sum(1 for line in sys.stdin if line.strip())
print(count)
```

```bash
cat notes.txt | node count-lines.mjs
python3 count_lines.py < notes.txt
```

### Preguntar en interactivo

```js
import { createInterface } from 'node:readline/promises';

const rl = createInterface({ input: process.stdin, output: process.stdout });
const name = await rl.question('¿Cómo te llamas? ');
rl.close();
console.log(`Hola ${name}`);
```

```python
name = input("¿Cómo te llamas? ")
print(f"Hola {name}")
```

```csharp
Console.Write("¿Cómo te llamas? ");
string? name = Console.ReadLine();
Console.WriteLine($"Hola {name}");
```

Para saber si la entrada viene de una tubería o del teclado: `process.stdin.isTTY` (Node), `sys.stdin.isatty()` (Python), `Console.IsInputRedirected` (C#).

## 4. Streams: procesar datos sin cargarlos enteros

Leer un archivo completo en memoria (`readFileSync`, `File.ReadAllBytes`) falla o se ralentiza con archivos grandes. Un **stream** lo procesa **por trozos** (*chunks*).

```text
archivo grande  →  [trozo] [trozo] [trozo] …  →  tu código  →  destino
                    memoria usada: un trozo
```

### Calcular el SHA-256 de un archivo grande

```js
// hash-file.mjs
import { createReadStream } from 'node:fs';
import { createHash } from 'node:crypto';

const hash = createHash('sha256');
for await (const chunk of createReadStream(process.argv[2])) {
  hash.update(chunk);
}
console.log(hash.digest('hex'));
```

```python
# hash_file.py
import hashlib
import sys

digest = hashlib.sha256()
with open(sys.argv[1], "rb") as source:
    while chunk := source.read(1024 * 1024):     # trozos de 1 MiB
        digest.update(chunk)
print(digest.hexdigest())
```

```bash
node hash-file.mjs sample.bin
sha256sum sample.bin          # debe dar el mismo resultado
```

### Encadenar con `pipeline` (Node)

```js
import { createReadStream, createWriteStream } from 'node:fs';
import { pipeline } from 'node:stream/promises';
import { createGzip } from 'node:zlib';

await pipeline(
  createReadStream('big.log'),
  createGzip(),
  createWriteStream('big.log.gz'),
);
```

`pipeline` gestiona los errores y la presión de retroceso (*backpressure*: no leer más rápido de lo que se puede escribir). Es preferible a unir los streams a mano.

| Lenguaje | Stream de archivo | Copiar de uno a otro |
|---|---|---|
| Node | `fs.createReadStream` | `stream.pipeline` |
| Python | `open(path, "rb")` y `read(n)` | `shutil.copyfileobj` |
| C# | `FileStream` | `stream.CopyToAsync` |
| Go | `os.Open` (un `io.Reader`) | `io.Copy` |
| Rust | `File` + `BufReader` | `std::io::copy` |
| Java | `FileInputStream` / `Files.newInputStream` | `InputStream.transferTo` |

## 5. Archivos binarios

Un archivo de **texto** es una secuencia de bytes interpretada con una codificación (UTF-8). Un archivo **binario** es una secuencia de bytes con un formato propio: cabeceras, números, datos comprimidos. No se puede abrir como texto.

### Ver los bytes

```bash
head -c 10M /dev/urandom > sample.bin    # archivo de prueba de 10 MiB con bytes aleatorios
xxd sample.bin | head                     # volcado hexadecimal
hexdump -C sample.bin | head              # otro formato
file image.png                            # detecta el tipo por su cabecera
```

Una imagen PNG empieza **siempre** por los bytes `89 50 4E 47 0D 0A 1A 0A` (el «número mágico»). Leerlo en código:

```python
with open("image.png", "rb") as source:
    header = source.read(8)
print(header.hex(" "))                    # 89 50 4e 47 0d 0a 1a 0a
print(header[1:4])                        # b'PNG'
```

### Números en binario: tamaño y orden de bytes

Un entero de 32 bits ocupa 4 bytes. El **orden de bytes** (*endianness*) decide cuál va primero:

| Orden | Bytes de 258 (`0x00000102`) | Quién lo usa |
|---|---|---|
| Little-endian | `02 01 00 00` | x86, ARM habituales, la mayoría de formatos de archivo |
| Big-endian | `00 00 01 02` | Protocolos de red («orden de red») |

```python
import struct

data = bytes([0x02, 0x01, 0x00, 0x00])
print(struct.unpack("<I", data)[0])       # 258  (< little-endian, I entero sin signo de 4 bytes)
print(struct.pack(">I", 258).hex(" "))    # 00 00 01 02
```

```js
const buffer = Buffer.from([0x02, 0x01, 0x00, 0x00]);
console.log(buffer.readUInt32LE(0));      // 258
```

```csharp
byte[] data = { 0x02, 0x01, 0x00, 0x00 };
Console.WriteLine(BitConverter.ToUInt32(data, 0));   // 258 en una máquina little-endian
```

### Partir y unir archivos con herramientas del sistema

```bash
head -c 10M /dev/urandom > sample.bin
split -b 3M -d sample.bin sample.part.       # sample.part.00, .01, .02, .03
cat sample.part.* > rebuilt.bin               # unirlas en orden
sha256sum sample.bin rebuilt.bin              # hashes iguales = archivos idénticos
cmp sample.bin rebuilt.bin && echo "idénticos"
```

| Comando | Para qué |
|---|---|
| `head -c N` / `tail -c N` | Primeros / últimos N bytes |
| `dd if=ENTRADA of=SALIDA bs=1M count=3 skip=2` | Copia por bloques; permite saltar y limitar |
| `split -b TAMAÑO` | Parte en trozos de tamaño fijo |
| `cat parte* > todo` | Une trozos |
| `xxd`, `hexdump -C`, `od -An -tx1` | Volcado hexadecimal |
| `xxd -r` | Convierte un volcado hexadecimal a binario |
| `cmp a b` | Compara byte a byte |
| `truncate -s 1M archivo` | Crea o ajusta el tamaño de un archivo |

> **Peligro:** `dd` escribe sin preguntar. Con `of=/dev/...` puede **destruir un disco**. Comprueba siempre `if` y `of` antes de pulsar Enter.

### Partir un archivo con código

```js
// split-file.mjs  →  node split-file.mjs archivo 1048576
import { open } from 'node:fs/promises';

const [inputPath, chunkSizeArg] = process.argv.slice(2);
const chunkSize = Number(chunkSizeArg);
if (!inputPath || !Number.isInteger(chunkSize) || chunkSize < 1) {
  console.error('Uso: split-file ARCHIVO TAMAÑO_EN_BYTES');
  process.exit(2);
}

const input = await open(inputPath, 'r');
const buffer = Buffer.alloc(chunkSize);
let part = 0;

while (true) {
  const { bytesRead } = await input.read(buffer, 0, chunkSize, null);   // null = posición actual
  if (bytesRead === 0) break;

  const output = await open(`${inputPath}.part${String(part).padStart(3, '0')}`, 'w');
  await output.write(buffer, 0, bytesRead);
  await output.close();
  part += 1;
}

await input.close();
console.log(`${part} partes`);
```

```python
# split_file.py  →  python3 split_file.py archivo 1048576
import sys
from pathlib import Path


def split_file(path: Path, chunk_size: int) -> int:
    count = 0
    with path.open("rb") as source:
        while chunk := source.read(chunk_size):
            part = path.with_name(f"{path.name}.part{count:03d}")
            part.write_bytes(chunk)
            count += 1
    return count


if __name__ == "__main__":
    total = split_file(Path(sys.argv[1]), int(sys.argv[2]))
    print(f"{total} partes")
```

```csharp
// SplitFile: dotnet run -- archivo 1048576
string inputPath = args[0];
int chunkSize = int.Parse(args[1]);

using FileStream input = File.OpenRead(inputPath);
byte[] buffer = new byte[chunkSize];
int part = 0;
int bytesRead;

while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
{
    string partPath = $"{inputPath}.part{part:D3}";
    using FileStream output = File.Create(partPath);
    output.Write(buffer, 0, bytesRead);
    part++;
}

Console.WriteLine($"{part} partes");
```

Compruébalo siempre: junta las partes con `cat` y compara con `sha256sum` el original y el resultado.

## 6. ¿Línea de comandos, interfaz de texto o gráfica?

| Tipo | Qué es | Cuándo | Ejemplos de tecnología |
|---|---|---|---|
| **CLI** | Comandos con argumentos | Automatización, herramientas, encadenar con `\|` | Todo lo anterior |
| **Interactivo** | Pregunta paso a paso | Asistentes, configuración | `readline`, `input()`, `Console.ReadLine` |
| **TUI** | Interfaz de texto con pantallas, menús | Herramientas en servidores sin escritorio | Bibliotecas de terminal (Ink, Textual, Ratatui) |
| **GUI** | Ventanas | Usuarios no técnicos, uso en escritorio | WinForms / WPF (C#), Tkinter (Python), JavaFX, Flutter de escritorio, interfaz web local |

Empezar por una CLI bien hecha es lo más rentable: es fácil de probar y de automatizar, y puedes poner después una GUI encima que llame a la misma lógica. Eso exige separar la lógica del programa (funciones) de su entrada y salida.

## 7. Estructura recomendable

```text
greet/
  src/
    core         lógica pura (sin leer argumentos ni imprimir)
    cli          lee argumentos, llama al core, imprime, devuelve el código de salida
  tests/         pruebas del core
```

Si la lógica está separada, la misma función sirve para una CLI, una API web y una GUI.

## 8. Errores habituales

| Síntoma | Causa | Solución |
|---|---|---|
| El programa recibe los argumentos de `dotnet` / `cargo` | Falta `--` | `dotnet run -- --name Ana` |
| Un número llega como texto | Los argumentos son siempre `string` | Convierte y valida |
| Los mensajes salen mezclados con los datos al usar `\|` | Escribes los errores por stdout | Usa stderr |
| El programa se queda esperando | Lee de stdin y nadie le da datos | Pasa un archivo con `<` o `\|`, o pulsa Ctrl+D |
| Memoria agotada con archivos grandes | Lees el archivo entero | Usa streams o lee por trozos |
| El archivo reconstruido no coincide | Unión en el orden incorrecto o trozo perdido | Compara con `sha256sum` y revisa el orden de los trozos |
| Caracteres raros en la salida | Codificación distinta de UTF-8 | Fuerza UTF-8 en la lectura y la escritura |

## 9. Ejercicios

> **Ejercicio:** escribe `wordcount` en el lenguaje que prefieras: debe aceptar un archivo como argumento o leer de stdin si no se le da ninguno, y soportar `--lines`, `--words` y `--help`. Comprueba que `cat file | wordcount` y `wordcount file` dan lo mismo.

> **Ejercicio:** escribe `join-parts` que reciba una lista de partes y reconstruya el archivo original, y verifica el resultado con `sha256sum` contra el original.

> **Ejercicio:** escribe un programa que lea los primeros 16 bytes de un archivo y diga si es PNG, JPEG (`FF D8 FF`), ZIP (`50 4B 03 04`) o desconocido. Pruébalo con archivos reales cuyo nombre hayas cambiado.

> **Ejercicio:** ejecuta tu programa de C# en Visual Studio con argumentos de depuración, y luego desde la terminal con `dotnet run --`. Anota qué diferencias encuentras.

## 10. Documentación oficial

- [Node: `util.parseArgs`](https://nodejs.org/api/util.html#utilparseargsconfig)
- [Node: streams](https://nodejs.org/api/stream.html)
- [Python: `argparse`](https://docs.python.org/3/library/argparse.html)
- [Python: `struct`](https://docs.python.org/3/library/struct.html)
- [.NET: argumentos de línea de comandos](https://learn.microsoft.com/es-es/dotnet/csharp/fundamentals/program-structure/main-command-line)
- [Go: paquete `flag`](https://pkg.go.dev/flag)
- [Rust: `clap`](https://docs.rs/clap)
