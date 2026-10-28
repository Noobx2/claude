# Ejercicios de lógica y programación

Para aprender a programar hay que **programar**. Esta página reúne los ejercicios clásicos, ordenados de menor a mayor dificultad, para repetir en cada lenguaje de la biblioteca: funciones, condiciones, bucles, calculadoras, arrays, objetos y los métodos `map`, `filter` y `reduce`.

*Verificado: 2026-10-27. Las salidas esperadas de los ejercicios con datos se han calculado ejecutando código de referencia.*

## 1. Cómo practicar

1. **Lee el enunciado y escribe a mano** qué entra y qué sale, con dos o tres ejemplos.
2. **Descomponlo** en pasos pequeños (en papel o en comentarios).
3. **Escribe una solución simple**, aunque sea larga. Después mejórala.
4. **Pruébala** con los casos dados y con casos límite (vacío, cero, negativos, muy grandes).
5. **Compárala** con otra forma de resolverlo (bucle frente a `reduce`, por ejemplo).
6. **Repite** el ejercicio otro día, sin mirar. Si lo haces solo, lo has aprendido.

> **Regla:** no copies soluciones. Si te atascas, usa las pistas, ejecuta el programa con `console.log` en cada paso y busca el primer punto donde el valor no es el que esperabas.

## 2. Plantilla para comprobar tus soluciones

Crea `solutions.mjs` con tus funciones y `solutions.test.mjs` para las pruebas. Con Node no necesitas instalar nada.

```js
// solutions.mjs
export function isEven(n) {
  // tu código aquí
}
```

```js
// solutions.test.mjs
import { test } from 'node:test';
import assert from 'node:assert/strict';
import { isEven } from './solutions.mjs';

test('isEven', () => {
  assert.equal(isEven(4), true);
  assert.equal(isEven(7), false);
  assert.equal(isEven(0), true);
});
```

```bash
node --test
```

| Pieza | Significado |
|---|---|
| `node --test` | Busca archivos `*.test.*` y ejecuta sus pruebas |
| `assert.equal(a, b)` | Comprueba `a === b` (en modo estricto) |
| `assert.deepEqual(a, b)` | Compara arrays y objetos por contenido |

Para Python usa `python3 -m unittest` o `pytest`; en C#, `dotnet test`; en Go, `go test`; en Rust, `cargo test`.

## 3. Un ejemplo resuelto: FizzBuzz

**Enunciado:** imprime los números del 1 al 15. Si es múltiplo de 3 escribe `Fizz`; si lo es de 5, `Buzz`; si lo es de ambos, `FizzBuzz`.

**Análisis.** Hay cuatro casos y **el orden importa**: hay que comprobar «ambos» antes que los otros dos; si no, el 15 diría solo `Fizz`.

```js
for (let i = 1; i <= 15; i += 1) {
  if (i % 15 === 0) console.log('FizzBuzz');
  else if (i % 3 === 0) console.log('Fizz');
  else if (i % 5 === 0) console.log('Buzz');
  else console.log(i);
}
```

| Pieza | Significado |
|---|---|
| `%` | Resto de la división: `10 % 3` es `1`; `i % 3 === 0` significa «múltiplo de 3» |
| `i % 15` | Ser múltiplo de 3 y de 5 es serlo de 15 |
| `else if` | Solo se evalúa si lo anterior fue falso |

Este ejercicio en ocho lenguajes está en [Comparativa de lenguajes](#lenguajes-comparativa).

## 4. Nivel 1: funciones y condiciones

Escribe cada una como **función** que recibe datos y **devuelve** el resultado (no imprime dentro).

| # | Ejercicio | Ejemplos | Pista |
|---|---|---|---|
| 1 | `isEven(n)`: ¿es par? | `4 → true`, `7 → false` | Operador `%` |
| 2 | `max3(a, b, c)`: el mayor de tres sin `Math.max` | `(3, 9, 5) → 9` | Compara y guarda el mayor |
| 3 | `sign(n)`: devuelve `"positivo"`, `"negativo"` o `"cero"` | `-4 → "negativo"` | `if / else if / else` |
| 4 | `isLeapYear(year)`: año bisiesto | `2000 → true`, `1900 → false`, `2024 → true` | Divisible por 4, salvo múltiplos de 100 que no lo sean de 400 |
| 5 | `celsiusToFahrenheit(c)` | `100 → 212`, `0 → 32` | `c * 9 / 5 + 32` |
| 6 | `bmi(weightKg, heightM)` y su categoría | `(70, 1.75) → 22.9` | Peso / altura²; redondea con `toFixed(1)` |
| 7 | `gradeLabel(score)`: nota a texto | `95 → "A"`, `72 → "C"`, `40 → "F"` | Cadena de `if` de mayor a menor |
| 8 | `factorial(n)` con bucle | `5 → 120`, `0 → 1` | Acumulador que multiplica |
| 9 | `factorialRecursive(n)` | `10 → 3628800` | Caso base `n <= 1` y llamada a sí misma |
| 10 | `sumTo(n)`: suma de 1 a n | `100 → 5050` | Bucle `for` |
| 11 | `multiplicationTable(n)`: tabla del n | `3 → ["3 x 1 = 3", …, "3 x 10 = 30"]` | Devuelve un array de textos |
| 12 | `isPrime(n)` | `2 → true`, `15 → false`, `1 → false` | Prueba divisores hasta la raíz de n |
| 13 | `primesUpTo(n)` | `29 → [2,3,5,7,11,13,17,19,23,29]` | Reutiliza `isPrime` |
| 14 | `fibonacci(n)`: el n-ésimo | `0..9 → 0,1,1,2,3,5,8,13,21,34` | Dos variables que se desplazan |
| 15 | `gcd(a, b)`: máximo común divisor | `(12, 18) → 6` | Algoritmo de Euclides: `gcd(b, a % b)` |

## 5. Nivel 2: cadenas

| # | Ejercicio | Ejemplos | Pista |
|---|---|---|---|
| 16 | `reverseString(s)` | `"hola" → "aloh"` | `split`, `reverse`, `join`, o un bucle |
| 17 | `isPalindrome(s)` ignorando espacios y mayúsculas | `"Anita lava la tina" → true` | Normaliza y compara con el reverso |
| 18 | `countVowels(s)` | `"programación" → 5` | Un conjunto de vocales y un contador |
| 19 | `capitalize(s)` y `titleCase(s)` | `"hola mundo" → "Hola Mundo"` | `split(' ')`, `map`, `join(' ')` |
| 20 | `countWords(s)` | `"uno dos  tres" → 3` | `split(/\s+/)` y filtrar vacíos |
| 21 | `isAnagram(a, b)` | `"roma", "amor" → true` | Ordena las letras de ambas |
| 22 | `caesar(text, shift)` | `("abc", 3) → "def"` | Código de carácter + desplazamiento, con vuelta al final del alfabeto |
| 23 | `compress(s)`: contar repeticiones seguidas | `"aaabbc" → "a3b2c1"` | Recorre guardando la letra actual y su contador |

## 6. Nivel 3: calculadoras y programas pequeños

### Calculadora de funciones

Escribe `calculate(a, operator, b)` que devuelva el resultado para `+`, `-`, `*`, `/`, `%` y `**`.

| Entrada | Salida esperada |
|---|---|
| `calculate(8, '+', 2)` | `10` |
| `calculate(8, '/', 2)` | `4` |
| `calculate(8, '/', 0)` | Lanza un `Error` con el mensaje «No se puede dividir entre cero» |
| `calculate(2, '**', 10)` | `1024` |
| `calculate(8, '?', 2)` | Lanza un `Error` con «Operador no válido» |

Pistas: `switch` sobre el operador; valida antes de dividir.

### Calculadora por consola

Un programa que lea de la línea de comandos: `node calc.mjs 8 + 2` imprime `10`.

- Argumentos en `process.argv` (ver [CLI y streams](#cli-args-streams)).
- Recuerda que llegan como **texto**: conviértelos con `Number()`.
- Si faltan argumentos o no son números, escribe el uso por stderr y sal con código 2.

### Calculadora interactiva con menú

Un bucle que pregunta una operación y dos números, muestra el resultado y vuelve a preguntar hasta que el usuario escriba `salir`.

### Otros programas pequeños

| # | Programa | Qué practica |
|---|---|---|
| 24 | **Adivina el número**: el programa piensa un número del 1 al 100; tú introduces intentos y dice «mayor» o «menor» | Bucle, condiciones, entrada del usuario, aleatorios |
| 25 | **Piedra, papel o tijera**: `winner(a, b)` devuelve quién gana | Lógica y tablas de decisión |
| 26 | **Conversor de unidades** con menú | Funciones, `switch` |
| 27 | **Contador de palabras** de un archivo | Archivos, `split`, mapas |
| 28 | **Tres en raya**: comprobar si un tablero (matriz 3×3) tiene ganador | Matrices y bucles anidados |
| 29 | **Cajero**: saldo, ingresar, retirar, con errores si no hay fondos | Objetos, funciones y excepciones |
| 30 | **Cuenta atrás** con `setInterval` | Asincronía básica |

## 7. Nivel 4: arrays

Usa primero **bucles** y después repite con los métodos de array.

| # | Ejercicio | Ejemplos | Pista |
|---|---|---|---|
| 31 | `sum(numbers)` | `[1,2,3,4] → 10` | Acumulador, luego `reduce` |
| 32 | `average(numbers)` | `[3,1,4,1,5,9,2,6,5,3,5] → 4` | Suma / longitud; ¿qué pasa con `[]`? |
| 33 | `maxOf(numbers)` y `minOf(numbers)` sin `Math.max` | `→ 9` y `1` | Guarda el mejor visto hasta ahora |
| 34 | `unique(numbers)` | `[3,1,4,1,5,9,2,6,5,3,5] → [3,1,4,5,9,2,6]` | `Set`, o `includes` |
| 35 | `countOccurrences(numbers)` | `→ { '1': 2, '2': 1, '3': 2, '4': 1, '5': 3, '6': 1, '9': 1 }` | Objeto como contador |
| 36 | `secondLargest(numbers)` | `→ 6` | Ordena sin duplicados |
| 37 | `rotate(numbers, k)` | `([1,2,3,4,5], 2) → [4,5,1,2,3]` | `slice` y concatenar |
| 38 | `flatten(matrix)` | `[[1,2],[3],[4,5]] → [1,2,3,4,5]` | `flat()` o bucle doble |
| 39 | `intersection(a, b)` | `([1,2,3],[2,3,4]) → [2,3]` | `filter` + `includes` o `Set` |
| 40 | `bubbleSort(numbers)` **sin** `sort` | `[5,2,9,1] → [1,2,5,9]` | Dos bucles; intercambia vecinos desordenados |
| 41 | `binarySearch(sorted, target)` | `([1,3,5,7,9], 7) → 3` | Mira el medio y descarta la mitad |
| 42 | `twoSum(numbers, target)`: índices de dos números que suman el objetivo | `([2,7,11,15], 9) → [0,1]` | Un `Map` de valor → índice |
| 43 | `chunk(array, size)` | `([1,2,3,4,5], 2) → [[1,2],[3,4],[5]]` | Bucle con paso `size` y `slice` |
| 44 | `transpose(matrix)` | `[[1,2,3],[4,5,6]] → [[1,4],[2,5],[3,6]]` | Bucles anidados |

## 8. Nivel 5: objetos

| # | Ejercicio | Ejemplo | Pista |
|---|---|---|---|
| 45 | `pick(object, keys)` | `({a:1,b:2,c:3}, ['a','c']) → {a:1,c:3}` | `Object.fromEntries` o bucle |
| 46 | `omit(object, keys)` | `({a:1,b:2,c:3}, ['b']) → {a:1,c:3}` | Lo contrario |
| 47 | `invert(object)` | `({a:'x',b:'y'}) → {x:'a',y:'b'}` | `Object.entries` y `fromEntries` |
| 48 | `merge(a, b)`: b manda sobre a | `({a:1,b:2},{b:3,c:4}) → {a:1,b:3,c:4}` | `{...a, ...b}` |
| 49 | `deepEqual(a, b)` | `({x:{y:1}},{x:{y:1}}) → true` | Recursión sobre claves |
| 50 | `wordFrequency(text)` | `"a b a c b a" → {a:3,b:2,c:1}` | `split` y contador |
| 51 | `groupBy(array, keyFn)` propio | agrupar empleados por departamento | `reduce`; compáralo con `Object.groupBy` |
| 52 | `countBy(array, keyFn)` | `([1,2,3,4,5], n => n % 2 ? 'odd' : 'even') → {odd:3,even:2}` | `reduce` con contador |
| 53 | Recorrer un objeto anidado y sumar todos los números | `{a:1,b:{c:2,d:{e:3}}} → 6` | Recursión |

## 9. Nivel 6: `map`, `filter`, `reduce` y compañía

Todos los ejercicios usan este conjunto de datos. Cópialo en un archivo y trabaja con él.

```js
const employees = [
  { id: 1, name: 'Ana',   department: 'ventas',     salary: 2400, age: 31, active: true  },
  { id: 2, name: 'Luis',  department: 'tecnología', salary: 3600, age: 28, active: true  },
  { id: 3, name: 'Marta', department: 'tecnología', salary: 4100, age: 41, active: false },
  { id: 4, name: 'Pablo', department: 'ventas',     salary: 2100, age: 24, active: true  },
  { id: 5, name: 'Sara',  department: 'recursos',   salary: 2800, age: 36, active: true  },
  { id: 6, name: 'Iván',  department: 'tecnología', salary: 3300, age: 33, active: true  },
];
```

Resuélvelos **sin bucles `for`**, con los métodos de array.

| # | Ejercicio | Salida esperada | Método |
|---|---|---|---|
| 54 | Nombres de los empleados activos | `['Ana','Luis','Pablo','Sara','Iván']` | `filter` + `map` |
| 55 | Suma de todos los salarios | `18300` | `reduce` |
| 56 | Salario medio | `3050` | `reduce` y división |
| 57 | Los tres mejor pagados (solo nombres) | `['Marta','Luis','Iván']` | `toSorted` + `slice` + `map` |
| 58 | Primer empleado mayor de 40 | `'Marta'` | `find` |
| 59 | ¿Todos cobran más de 2000? ¿Hay alguno inactivo? | `true`, `true` | `every`, `some` |
| 60 | Nombres en una sola cadena | `'Ana, Luis, Marta, Pablo, Sara, Iván'` | `map` + `join` |
| 61 | El empleado de más edad | `'Marta'` | `reduce` |
| 62 | Masa salarial por departamento | `{ ventas: 4500, tecnología: 11000, recursos: 2800 }` | `reduce` con objeto |
| 63 | Salario medio por departamento | `ventas 2250`, `tecnología 3666.67`, `recursos 2800` | `Object.groupBy` + `map` + `reduce` |
| 64 | Cuántos empleados hay en tecnología | `3` | `filter` + `length` |
| 65 | Subir el 10 % a los de ventas **sin modificar** el array original | nuevo array con salarios `2640` y `2310` para ventas | `map` + copia con `...` |
| 66 | Lista de departamentos sin repetir | `['ventas','tecnología','recursos']` | `map` + `Set` |
| 67 | Texto «Ana (31), Luis (28), …» | `'Ana (31), Luis (28), …'` | `map` + plantilla + `join` |
| 68 | Ordenar por departamento y, dentro de cada uno, por salario descendente | a tu criterio, comprobando a ojo | `toSorted` con comparador doble |
| 69 | Convertir el array en un objeto indexado por `id` | `{1:{…},2:{…},…}` | `Object.fromEntries` o `reduce` |
| 70 | Implementar **tu propio** `map`, `filter` y `reduce` con bucles | mismo resultado que los nativos | Funciones que reciben un callback |

> **Ojo:** `sort`, `reverse` y `splice` **modifican** el array. Para no tocar el original usa `toSorted`, `toReversed`, `toSpliced` o copia primero con `[...array]`.

## 10. Nivel 7: problemas de entrevista

| # | Ejercicio | Ejemplo |
|---|---|---|
| 71 | `isBalanced(s)`: paréntesis, corchetes y llaves bien cerrados | `"{[()]}" → true`, `"([)]" → false` (pista: una pila) |
| 72 | `toRoman(n)` y `fromRoman(s)` | `1994 → "MCMXCIV"` |
| 73 | `groupAnagrams(words)` | `["eat","tea","tan","ate","nat","bat"] → [["eat","tea","ate"],["tan","nat"],["bat"]]` |
| 74 | `longestCommonPrefix(words)` | `["flower","flow","flight"] → "fl"` |
| 75 | `mostFrequentWord(text)` | `"a b a c b a" → "a"` |
| 76 | `rotateMatrix(matrix)` 90° | `[[1,2],[3,4]] → [[3,1],[4,2]]` |
| 77 | `debounce(fn, ms)` | solo ejecuta `fn` tras `ms` sin nuevas llamadas |
| 78 | `memoize(fn)`: guarda resultados por argumento | `fibonacci` memoizado |
| 79 | `flattenDeep(array)` | `[1,[2,[3,[4]]]] → [1,2,3,4]` |
| 80 | `sleep(ms)` con promesas y úsala con `await` | espera sin bloquear |

## 11. Nivel 8: mini proyectos

Cada uno se escribe primero como **consola** y después, si quieres, con interfaz.

| Proyecto | Requisitos mínimos |
|---|---|
| **Lista de tareas** | Añadir, listar, completar, borrar, guardar en un archivo JSON |
| **Agenda de contactos** | Alta, búsqueda por nombre, edición, ordenación |
| **Cuenta bancaria** | Clase `Account` con `deposit`, `withdraw`, historial y errores |
| **Inventario** | Productos con stock; informe de los que están bajo mínimos |
| **Cuestionario** | Preguntas en un JSON, puntuación, repetir fallos |
| **Gestor de gastos** | Categorías, total por categoría y por mes con `reduce` |

## 12. El mismo ejercicio en otros lenguajes

Cuando termines un ejercicio, resuélvelo **otra vez** en otro lenguaje. Así separas el pensamiento algorítmico de la sintaxis. La tabla de equivalencias está en [Comparativa de lenguajes](#lenguajes-comparativa).

| Ejercicio | Qué te enseña del segundo lenguaje |
|---|---|
| `isPrime` | Tipos numéricos y bucles |
| `wordFrequency` | Diccionarios (`dict`, `Dictionary`, `map`, `HashMap`) |
| Employees: agrupar y sumar | LINQ en C#, streams en Java, comprensiones en Python, iteradores en Rust |
| Calculadora por consola | Argumentos y errores |

## 13. Cuando te atasques

1. Ejecuta con valores pequeños y escribe a mano qué debería pasar en cada vuelta.
2. Imprime los valores intermedios (`console.log`).
3. Reduce el problema: ¿funciona con `[]`? ¿con un solo elemento?
4. Cambia de estrategia: bucle ↔ recursión ↔ métodos de array.
5. Explícaselo en voz alta a alguien (o a un patito de goma): el error suele aparecer al contarlo.
