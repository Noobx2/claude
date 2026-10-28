# Comparativa de lenguajes

Las mismas ideas en ocho lenguajes: JavaScript, TypeScript, Python, Java, C#, Go, Rust y Dart. Sirve para traducir lo que sabes de uno a otro sin empezar de cero.

*Verificado: 2026-10-27. Los ejemplos de JavaScript, TypeScript, Python, Java, Go y Rust se ejecutaron; los de C# y Dart no se han podido ejecutar desde aquí, así que revisa la sintaxis con su compilador.*

## 1. Hola mundo

| Lenguaje | Archivo | Código | Ejecutar |
|---|---|---|---|
| JavaScript | `hello.mjs` | `console.log("Hola");` | `node hello.mjs` |
| TypeScript | `hello.ts` | `console.log("Hola");` | `node hello.ts` (Node reciente) o `pnpm exec tsc` + `node` |
| Python | `hello.py` | `print("Hola")` | `python3 hello.py` |
| Java | `Hello.java` | `System.out.println("Hola");` dentro de una clase con `main` | `java Hello.java` |
| C# | `Program.cs` | `Console.WriteLine("Hola");` | `dotnet run` |
| Go | `main.go` | `fmt.Println("Hola")` dentro de `package main` y `func main` | `go run .` |
| Rust | `main.rs` | `println!("Hola");` dentro de `fn main` | `cargo run` |
| Dart | `main.dart` | `print('Hola');` dentro de `void main()` | `dart run` |

## 2. Quién necesita qué

| Concepto | JS / TS | Python | Java | C# | Go | Rust | Dart |
|---|---|---|---|---|---|---|---|
| Compila antes de ejecutar | No (TS sí) | No | Sí (a bytecode) | Sí | Sí | Sí | Sí / JIT |
| Tipado | Dinámico (TS estático) | Dinámico | Estático | Estático | Estático | Estático | Estático |
| Terminar sentencias con `;` | Opcional | No | Sí | Sí | No | Sí | Sí |
| Bloques | `{ }` | Sangría | `{ }` | `{ }` | `{ }` | `{ }` | `{ }` |
| Punto de entrada | El archivo | El archivo | `main` en una clase | `Main` o instrucciones de nivel superior | `func main` | `fn main` | `void main` |
| Gestión de memoria | Recolector | Recolector | Recolector | Recolector | Recolector | Propiedad y préstamos (sin recolector) | Recolector |

## 3. Variables y constantes

| Lenguaje | Variable | Constante | Inferencia |
|---|---|---|---|
| JavaScript | `let count = 0;` | `const name = "Ana";` | Dinámica |
| TypeScript | `let count: number = 0;` | `const name: string = "Ana";` | `let count = 0;` |
| Python | `count = 0` | `NAME = "Ana"` (convención) | Dinámica |
| Java | `int count = 0;` | `final String name = "Ana";` | `var count = 0;` |
| C# | `int count = 0;` | `const string Name = "Ana";` | `var count = 0;` |
| Go | `var count int = 0` o `count := 0` | `const name = "Ana"` | `count := 0` |
| Rust | `let mut count = 0;` | `let name = "Ana";` (inmutable por defecto) | Siempre |
| Dart | `int count = 0;` | `final name = 'Ana';` o `const` | `var count = 0;` |

> **Ojo:** en Rust las variables son **inmutables** salvo que escribas `mut`; en JavaScript `const` impide reasignar pero no cambiar el contenido de un objeto.

## 4. Tipos básicos

| Idea | JS / TS | Python | Java | C# | Go | Rust | Dart |
|---|---|---|---|---|---|---|---|
| Entero | `number` | `int` | `int`, `long` | `int`, `long` | `int`, `int64` | `i32`, `i64`, `u32` | `int` |
| Decimal | `number` | `float` | `double` | `double`, `decimal` | `float64` | `f64` | `double` |
| Texto | `string` | `str` | `String` | `string` | `string` | `String`, `&str` | `String` |
| Booleano | `boolean` | `bool` | `boolean` | `bool` | `bool` | `bool` | `bool` |
| Ausencia | `null`, `undefined` | `None` | `null` | `null` | `nil` (en punteros, mapas…) | `Option<T>` (`None`) | `null` (null safety) |
| Lista | `Array` | `list` | `List<T>` | `List<T>` | slice `[]T` | `Vec<T>` | `List<T>` |
| Diccionario | `Map`, objeto | `dict` | `Map<K,V>` | `Dictionary<K,V>` | `map[K]V` | `HashMap<K,V>` | `Map<K,V>` |
| Conjunto | `Set` | `set` | `Set<T>` | `HashSet<T>` | `map[T]struct{}` | `HashSet<T>` | `Set<T>` |

## 5. Condicionales

```js
// JavaScript / TypeScript
if (score >= 90) { grade = 'A'; } else if (score >= 70) { grade = 'B'; } else { grade = 'C'; }
```

```python
# Python
if score >= 90:
    grade = "A"
elif score >= 70:
    grade = "B"
else:
    grade = "C"
```

```java
// Java (y C# casi idéntico)
if (score >= 90) { grade = "A"; } else if (score >= 70) { grade = "B"; } else { grade = "C"; }
```

```go
// Go: sin paréntesis alrededor de la condición
if score >= 90 {
	grade = "A"
} else if score >= 70 {
	grade = "B"
} else {
	grade = "C"
}
```

```rust
// Rust: if es una expresión, devuelve valor
let grade = if score >= 90 { "A" } else if score >= 70 { "B" } else { "C" };
```

```dart
// Dart
if (score >= 90) { grade = 'A'; } else if (score >= 70) { grade = 'B'; } else { grade = 'C'; }
```

## 6. Bucles

Contar del 1 al 5:

| Lenguaje | Código |
|---|---|
| JavaScript / TypeScript | `for (let i = 1; i <= 5; i += 1) { console.log(i); }` |
| Python | `for i in range(1, 6): print(i)` |
| Java | `for (int i = 1; i <= 5; i++) { System.out.println(i); }` |
| C# | `for (int i = 1; i <= 5; i++) { Console.WriteLine(i); }` |
| Go | `for i := 1; i <= 5; i++ { fmt.Println(i) }` |
| Rust | `for i in 1..=5 { println!("{i}"); }` |
| Dart | `for (var i = 1; i <= 5; i++) { print(i); }` |

Recorrer los elementos de una colección:

| Lenguaje | Código |
|---|---|
| JavaScript / TypeScript | `for (const item of items) { … }` |
| Python | `for item in items: …` |
| Java | `for (String item : items) { … }` |
| C# | `foreach (var item in items) { … }` |
| Go | `for _, item := range items { … }` (el `_` descarta el índice) |
| Rust | `for item in &items { … }` |
| Dart | `for (final item in items) { … }` |

`while`: igual en todos con `while (condición) { … }` (Python: `while condición:`; Go: `for condición { … }`; Rust: `while condición { … }`).

## 7. Funciones

| Lenguaje | Definición |
|---|---|
| JavaScript | `function add(a, b) { return a + b; }` y `const add = (a, b) => a + b;` |
| TypeScript | `function add(a: number, b: number): number { return a + b; }` |
| Python | `def add(a: int, b: int) -> int: return a + b` |
| Java | `static int add(int a, int b) { return a + b; }` |
| C# | `static int Add(int a, int b) => a + b;` |
| Go | `func add(a int, b int) int { return a + b }` |
| Rust | `fn add(a: i32, b: i32) -> i32 { a + b }` (sin `;` final: es el valor devuelto) |
| Dart | `int add(int a, int b) => a + b;` |

Funciones anónimas (lambdas) para pasar a `map` o `filter`:

| Lenguaje | Cuadrado de x |
|---|---|
| JS / TS | `x => x * x` |
| Python | `lambda x: x * x` |
| Java | `x -> x * x` |
| C# | `x => x * x` |
| Go | `func(x int) int { return x * x }` |
| Rust | `\|x\| x * x` |
| Dart | `(x) => x * x` |

## 8. Colecciones: transformar, filtrar, sumar

Dado `[1, 2, 3, 4, 5]`: dobla los números, quédate con los pares y suma todo.

```js
// JavaScript
const numbers = [1, 2, 3, 4, 5];
const doubled = numbers.map((n) => n * 2);
const evens = numbers.filter((n) => n % 2 === 0);
const total = numbers.reduce((sum, n) => sum + n, 0);
```

```python
# Python
numbers = [1, 2, 3, 4, 5]
doubled = [n * 2 for n in numbers]          # comprensión de lista
evens = [n for n in numbers if n % 2 == 0]
total = sum(numbers)
```

```java
// Java (streams)
import java.util.*;
import java.util.stream.*;

List<Integer> numbers = List.of(1, 2, 3, 4, 5);
List<Integer> doubled = numbers.stream().map(n -> n * 2).toList();
List<Integer> evens = numbers.stream().filter(n -> n % 2 == 0).toList();
int total = numbers.stream().mapToInt(Integer::intValue).sum();
```

```csharp
// C# (LINQ)
using System.Linq;

var numbers = new List<int> { 1, 2, 3, 4, 5 };
var doubled = numbers.Select(n => n * 2).ToList();
var evens = numbers.Where(n => n % 2 == 0).ToList();
int total = numbers.Sum();
```

```go
// Go: no hay map/filter genéricos en la librería base; se usan bucles
numbers := []int{1, 2, 3, 4, 5}
var evens []int
total := 0
for _, n := range numbers {
	if n%2 == 0 {
		evens = append(evens, n)
	}
	total += n
}
```

```rust
// Rust (iteradores)
let numbers = vec![1, 2, 3, 4, 5];
let doubled: Vec<i32> = numbers.iter().map(|n| n * 2).collect();
let evens: Vec<i32> = numbers.iter().filter(|n| *n % 2 == 0).cloned().collect();
let total: i32 = numbers.iter().sum();
```

```dart
// Dart
final numbers = [1, 2, 3, 4, 5];
final doubled = numbers.map((n) => n * 2).toList();
final evens = numbers.where((n) => n % 2 == 0).toList();
final total = numbers.reduce((a, b) => a + b);
```

| Operación | JS | Python | Java | C# | Rust | Dart |
|---|---|---|---|---|---|---|
| Transformar | `map` | comprensión / `map` | `stream().map` | `Select` | `iter().map` | `map` |
| Filtrar | `filter` | comprensión con `if` | `filter` | `Where` | `filter` | `where` |
| Acumular | `reduce` | `sum`, `functools.reduce` | `reduce`, `sum` | `Aggregate`, `Sum` | `fold`, `sum` | `reduce`, `fold` |
| Ordenar | `toSorted` | `sorted` | `sorted` | `OrderBy` | `sort` / `sort_by` | `sort` |
| Alguno / todos | `some` / `every` | `any` / `all` | `anyMatch` / `allMatch` | `Any` / `All` | `any` / `all` | `any` / `every` |
| Primero que cumple | `find` | `next(… for … if …)` | `findFirst` | `FirstOrDefault` | `find` | `firstWhere` |
| Agrupar | `Object.groupBy` | `itertools.groupby`, `defaultdict` | `Collectors.groupingBy` | `GroupBy` | (a mano o crate) | (a mano o paquete) |

## 9. Diccionarios y recorrido

```js
// JavaScript
const ages = new Map([['Ana', 31], ['Luis', 28]]);
for (const [name, age] of ages) console.log(name, age);
```

```python
# Python
ages = {"Ana": 31, "Luis": 28}
for name, age in ages.items():
    print(name, age)
```

```java
// Java
Map<String, Integer> ages = Map.of("Ana", 31, "Luis", 28);
for (var entry : ages.entrySet()) System.out.println(entry.getKey() + " " + entry.getValue());
```

```csharp
// C#
var ages = new Dictionary<string, int> { ["Ana"] = 31, ["Luis"] = 28 };
foreach (var (name, age) in ages) Console.WriteLine($"{name} {age}");
```

```go
// Go (el orden de recorrido de un mapa no está garantizado)
ages := map[string]int{"Ana": 31, "Luis": 28}
for name, age := range ages {
	fmt.Println(name, age)
}
```

```rust
// Rust
use std::collections::HashMap;
let ages = HashMap::from([("Ana", 31), ("Luis", 28)]);
for (name, age) in &ages {
    println!("{name} {age}");
}
```

```dart
// Dart
final ages = {'Ana': 31, 'Luis': 28};
ages.forEach((name, age) => print('$name $age'));
```

## 10. Texto

| Operación | JS / TS | Python | Java | C# | Go | Rust | Dart |
|---|---|---|---|---|---|---|---|
| Longitud | `s.length` | `len(s)` | `s.length()` | `s.Length` | `len(s)` (bytes) | `s.len()` (bytes) | `s.length` |
| Mayúsculas | `s.toUpperCase()` | `s.upper()` | `s.toUpperCase()` | `s.ToUpper()` | `strings.ToUpper(s)` | `s.to_uppercase()` | `s.toUpperCase()` |
| Contiene | `s.includes(x)` | `x in s` | `s.contains(x)` | `s.Contains(x)` | `strings.Contains(s, x)` | `s.contains(x)` | `s.contains(x)` |
| Partir | `s.split(",")` | `s.split(",")` | `s.split(",")` | `s.Split(',')` | `strings.Split(s, ",")` | `s.split(',')` | `s.split(',')` |
| Unir | `a.join(", ")` | `", ".join(a)` | `String.join(", ", a)` | `string.Join(", ", a)` | `strings.Join(a, ", ")` | `a.join(", ")` | `a.join(', ')` |
| Interpolar | `` `Hola ${n}` `` | `f"Hola {n}"` | `"Hola " + n` | `$"Hola {n}"` | `fmt.Sprintf("Hola %s", n)` | `format!("Hola {n}")` | `'Hola $n'` |

> **Ojo:** en Go y Rust la longitud de un texto cuenta **bytes**, no caracteres. Con tildes o emojis, `len("año")` no es 3.

## 11. Errores

| Lenguaje | Modelo | Ejemplo |
|---|---|---|
| JS / TS | Excepciones | `try { … } catch (error) { … }` |
| Python | Excepciones | `try: … except ValueError as e: …` |
| Java | Excepciones (algunas comprobadas) | `try { … } catch (IOException e) { … }` |
| C# | Excepciones | `try { … } catch (FormatException e) { … }` |
| Go | **Valores** de error devueltos | `value, err := f(); if err != nil { return err }` |
| Rust | **Valores** `Result<T, E>` | `let n: i32 = text.parse()?;` o `match` |
| Dart | Excepciones | `try { … } on FormatException catch (e) { … }` |

Go y Rust obligan a decidir qué hacer con cada fallo en el lugar donde ocurre, a diferencia de las excepciones, que «saltan» hasta un `catch`.

## 12. Módulos y archivos

| Lenguaje | Unidad de código | Importar |
|---|---|---|
| JS / TS | Archivo (módulo ES) | `import { x } from "./x.js"` |
| Python | Archivo = módulo; carpeta con `__init__.py` = paquete | `from x import y` |
| Java | Clase en un paquete | `import com.app.Task;` |
| C# | Espacio de nombres (`namespace`) | `using App.Models;` |
| Go | Paquete (una carpeta) | `import "app/internal/tasks"` |
| Rust | Crate y módulos (`mod`) | `use crate::tasks::Task;` |
| Dart | Archivo / librería | `import 'package:app/task.dart';` |

## 13. FizzBuzz en todos

**JavaScript**

```js
for (let i = 1; i <= 15; i += 1) {
  if (i % 15 === 0) console.log('FizzBuzz');
  else if (i % 3 === 0) console.log('Fizz');
  else if (i % 5 === 0) console.log('Buzz');
  else console.log(i);
}
```

**TypeScript**

```ts
for (let i: number = 1; i <= 15; i += 1) {
  const output: string = i % 15 === 0 ? 'FizzBuzz' : i % 3 === 0 ? 'Fizz' : i % 5 === 0 ? 'Buzz' : String(i);
  console.log(output);
}
```

**Python**

```python
for i in range(1, 16):
    if i % 15 == 0:
        print("FizzBuzz")
    elif i % 3 == 0:
        print("Fizz")
    elif i % 5 == 0:
        print("Buzz")
    else:
        print(i)
```

**Java**

```java
public class Fizz {
    public static void main(String[] args) {
        for (int i = 1; i <= 15; i++) {
            if (i % 15 == 0) System.out.println("FizzBuzz");
            else if (i % 3 == 0) System.out.println("Fizz");
            else if (i % 5 == 0) System.out.println("Buzz");
            else System.out.println(i);
        }
    }
}
```

**C#**

```csharp
for (int i = 1; i <= 15; i++)
{
    if (i % 15 == 0) Console.WriteLine("FizzBuzz");
    else if (i % 3 == 0) Console.WriteLine("Fizz");
    else if (i % 5 == 0) Console.WriteLine("Buzz");
    else Console.WriteLine(i);
}
```

**Go**

```go
package main

import "fmt"

func main() {
	for i := 1; i <= 15; i++ {
		switch {
		case i%15 == 0:
			fmt.Println("FizzBuzz")
		case i%3 == 0:
			fmt.Println("Fizz")
		case i%5 == 0:
			fmt.Println("Buzz")
		default:
			fmt.Println(i)
		}
	}
}
```

**Rust**

```rust
fn main() {
    for i in 1..=15 {
        if i % 15 == 0 {
            println!("FizzBuzz");
        } else if i % 3 == 0 {
            println!("Fizz");
        } else if i % 5 == 0 {
            println!("Buzz");
        } else {
            println!("{i}");
        }
    }
}
```

**Dart**

```dart
void main() {
  for (var i = 1; i <= 15; i++) {
    if (i % 15 == 0) {
      print('FizzBuzz');
    } else if (i % 3 == 0) {
      print('Fizz');
    } else if (i % 5 == 0) {
      print('Buzz');
    } else {
      print(i);
    }
  }
}
```

## 14. Ejercicios

> **Ejercicio:** implementa `isPrime` y `wordFrequency` (de [Ejercicios](#ejercicios-basicos)) en tres lenguajes distintos. Anota qué te costó más de cada uno: ¿tipos, sintaxis o librería?

> **Ejercicio:** repite el ejercicio de empleados (filtrar activos, sumar salarios, agrupar por departamento) en C# con LINQ y en Python con comprensiones. ¿Cuál lee mejor?

> **Ejercicio:** elige un lenguaje con valores de error (Go o Rust) y reescribe una función que lanzaba excepciones en JavaScript.
