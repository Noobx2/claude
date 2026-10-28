# JavaScript moderno

JavaScript es el lenguaje de la web y, con Node.js, también del servidor. Este módulo cubre el lenguaje actual (ES2015 en adelante) y su uso en el navegador. El estándar se llama **ECMAScript (ES)** y recibe una edición cada año.

*Verificado: 2026-10-27. Referencia: [MDN](https://developer.mozilla.org/es/docs/Web/JavaScript). Tu Node es 24.22.1: soporta lo que sigue, pero compruébalo con `node -e "..."` cuando dudes.*

## 1. Archivos y nombres

| Elemento | Convención |
|---|---|
| Archivos | `kebab-case.js` (`task-list.js`); `.mjs` fuerza módulo en Node |
| Variables y funciones | `camelCase` |
| Clases | `PascalCase` |
| Constantes globales | `UPPER_SNAKE_CASE` |
| Módulos | Sí: `import` / `export` (no uses `require` en código nuevo) |
| Comentarios | `// una línea` y `/* varias */` |

## 2. Hola mundo

En Node:

```bash
echo 'console.log("Hola mundo");' > hello.js
node hello.js
```

En el navegador, en `index.html`:

```html
<script type="module" src="main.js"></script>
```

```js
// main.js
console.log('Hola mundo');
document.body.append('Hola mundo');
```

Los scripts de tipo `module` se ejecutan en modo estricto y diferidos (después de leer el HTML).

## 3. Variables, tipos y conversiones

```js
const language = 'JavaScript';   // no se puede reasignar
let counter = 0;                 // se puede reasignar
counter += 1;
// var: no lo uses en código nuevo
```

> **Regla:** usa `const` siempre; cambia a `let` solo si necesitas reasignar. `const` no hace inmutable el objeto, solo impide reasignar la variable.

| Tipo | Ejemplo | Notas |
|---|---|---|
| `string` | `'texto'`, `` `plantilla ${x}` `` | Comillas simples por convención |
| `number` | `42`, `3.14` | Coma flotante: `0.1 + 0.2 !== 0.3` |
| `bigint` | `9007199254740993n` | Enteros enormes |
| `boolean` | `true`, `false` | |
| `undefined` | | Valor sin asignar |
| `null` | | Ausencia intencionada |
| `symbol` | `Symbol('id')` | Avanzado |
| `object` | `{}`, `[]`, `new Date()` | Incluye arrays y funciones |

```js
typeof 42;              // 'number'
typeof null;            // 'object'  (error histórico del lenguaje)
Array.isArray([]);      // true

Number('42');           // 42
Number('abc');          // NaN
parseInt('42px', 10);   // 42
String(42);             // '42'
Boolean('');            // false
```

**Falsy** (se comportan como `false`): `false`, `0`, `''`, `null`, `undefined`, `NaN`. Todo lo demás es *truthy*, incluidos `'0'` y `[]`.

> **Ojo:** compara siempre con `===` y `!==`. `==` convierte tipos y produce sorpresas (`0 == ''` es `true`).

## 4. Operadores y condiciones

```js
const name = user?.profile?.name ?? 'Anónimo';
// ?.  → si user o profile son null/undefined, devuelve undefined (sin error)
// ??  → usa el valor de la derecha solo si la izquierda es null o undefined
// ||  → usa el de la derecha si la izquierda es falsy (¡también con 0 y '')
```

```js
if (score >= 90) {
  grade = 'A';
} else if (score >= 70) {
  grade = 'B';
} else {
  grade = 'C';
}

const label = isDone ? 'Hecha' : 'Pendiente';

switch (status) {
  case 'open':
    break;
  default:
    break;
}
```

### Bucles

```js
for (let i = 0; i < 3; i += 1) { /* ... */ }

for (const item of items) { /* recorre valores de un array, Set, Map, string */ }

for (const [key, value] of Object.entries(obj)) { /* claves y valores de un objeto */ }

let n = 3;
while (n > 0) { n -= 1; }
```

No uses `for...in` con arrays; recorre propiedades, no valores.

## 5. Funciones

```js
function add(a, b) { return a + b; }          // declaración (se "eleva")

const multiply = (a, b) => a * b;              // flecha

const greet = (name = 'amigo') => `Hola ${name}`;   // parámetro por defecto

const sum = (...numbers) => numbers.reduce((acc, n) => acc + n, 0);   // rest
sum(1, 2, 3);        // 6
```

Una **clausura** es una función que recuerda las variables de donde se creó:

```js
function makeCounter() {
  let count = 0;
  return () => { count += 1; return count; };
}
const next = makeCounter();
next(); next();   // 1, 2
```

Una función puede recibir otra como argumento (*callback*): es la base de `map`, `filter` y los eventos.

## 6. Objetos y desestructuración

```js
const task = { id: 1, title: 'Estudiar JS', done: false };

const { title, done } = task;                  // extraer
const { id, ...rest } = task;                  // separar
const updated = { ...task, done: true };       // copia con cambio (no muta)
const key = 'priority';
const withKey = { ...task, [key]: 'alta' };    // clave calculada

Object.keys(task);      // ['id', 'title', 'done']
Object.values(task);
Object.entries(task);

const [first, second] = ['a', 'b', 'c'];       // desestructurar array
```

La copia con `...` es **superficial**. Para una copia profunda:

```js
const copy = structuredClone(task);
```

## 7. Colecciones: arrays, Map y Set

### Arrays: recorrer, filtrar, transformar, ordenar y agrupar

```js
const tasks = [
  { id: 1, title: 'Comprar pan', done: true,  priority: 2 },
  { id: 2, title: 'Estudiar JS', done: false, priority: 1 },
  { id: 3, title: 'Hacer deporte', done: false, priority: 3 },
];

tasks.forEach((t) => console.log(t.title));                 // recorrer
const pending = tasks.filter((t) => !t.done);                // filtrar
const titles = tasks.map((t) => t.title);                    // transformar
const first = tasks.find((t) => t.id === 2);                 // primer elemento que cumple
const hasDone = tasks.some((t) => t.done);                   // ¿alguno?
const allDone = tasks.every((t) => t.done);                  // ¿todos?
const total = tasks.reduce((sum, t) => sum + t.priority, 0); // acumular
const last = tasks.at(-1);                                   // último

// Ordenar sin mutar el original (toSorted), con criterio numérico
const byPriority = tasks.toSorted((a, b) => a.priority - b.priority);
// Texto con idioma: localeCompare
const byTitle = tasks.toSorted((a, b) => a.title.localeCompare(b.title, 'es'));

// Agrupar
const grouped = Object.groupBy(tasks, (t) => (t.done ? 'done' : 'pending'));
// { done: [...], pending: [...] }
```

> **Ojo:** `sort()` y `reverse()` **modifican** el array original. Prefiere `toSorted()` y `toReversed()`.

| Operación | Método | Devuelve |
|---|---|---|
| Recorrer | `forEach`, `for...of` | nada |
| Filtrar | `filter` | array |
| Transformar | `map`, `flatMap` | array |
| Buscar | `find`, `findLast`, `findIndex`, `includes` | elemento, índice o booleano |
| Comprobar | `some`, `every` | booleano |
| Acumular | `reduce` | cualquier valor |
| Ordenar | `toSorted` | array nuevo |
| Cambiar un elemento sin mutar | `with(index, value)` | array nuevo |
| Agrupar | `Object.groupBy`, `Map.groupBy` | objeto o `Map` |

### Map y Set

```js
const byId = new Map(tasks.map((t) => [t.id, t]));
byId.get(2);             // la tarea 2
byId.has(9);             // false

const tags = new Set(['js', 'html', 'js']);   // sin duplicados
tags.size;                                    // 2

const a = new Set([1, 2, 3]);
const b = new Set([2, 3, 4]);
a.union(b);              // {1, 2, 3, 4}
a.intersection(b);       // {2, 3}
a.difference(b);         // {1}
```

| Usa | Cuándo |
|---|---|
| Array | Lista ordenada |
| Objeto | Registro con claves conocidas (texto) |
| `Map` | Diccionario con claves cualesquiera, mantiene el orden, buen rendimiento |
| `Set` | Valores únicos |

## 8. JSON

```js
const text = JSON.stringify(task, null, 2);   // objeto → texto (2 espacios de sangría)
const data = JSON.parse(text);                // texto → objeto
```

- `JSON.parse` **lanza un error** si el texto es inválido: envuélvelo en `try/catch`.
- Las fechas se serializan como texto: vuelve a crearlas con `new Date(data.createdAt)`.
- `undefined`, funciones y `Symbol` no se serializan.
- JSON exige comillas dobles en claves y textos.

```js
function parseJson(text, fallback) {
  try {
    return JSON.parse(text);
  } catch {
    return fallback;
  }
}
```

## 9. Errores

```js
try {
  const data = JSON.parse(input);
} catch (error) {
  console.error('JSON inválido:', error.message);
} finally {
  // se ejecuta siempre
}

function divide(a, b) {
  if (b === 0) throw new RangeError('No se puede dividir entre cero');
  return a / b;
}

class ValidationError extends Error {
  constructor(message, field) {
    super(message);
    this.name = 'ValidationError';
    this.field = field;
  }
}

try {
  riskyCall();
} catch (cause) {
  throw new Error('No se pudo guardar', { cause });   // conserva el error original
}
```

## 10. Módulos

```js
// math.js
export const PI = 3.14159;
export function add(a, b) { return a + b; }
export default function subtract(a, b) { return a - b; }

// main.js
import subtract, { PI, add } from './math.js';   // ruta relativa con extensión
```

- Un `export default` por archivo como máximo; los `export` con nombre pueden ser varios.
- En el navegador, las rutas llevan extensión (`./math.js`). Vite y otros empaquetadores te permiten omitirla.
- `import()` dinámico carga módulos bajo demanda: `const m = await import('./chart.js');`.
- En Node, para usar módulos pon `"type": "module"` en `package.json` (o usa la extensión `.mjs`).

## 11. Programación orientada a objetos

```js
class Task {
  #createdAt = new Date();           // campo privado
  static count = 0;                  // estático

  constructor(title) {
    this.title = title;
    this.done = false;
    Task.count += 1;
  }

  toggle() {
    this.done = !this.done;
  }

  get age() {
    return Date.now() - this.#createdAt.getTime();
  }
}

class Subtask extends Task {
  constructor(title, parent) {
    super(title);
    this.parent = parent;
  }
}
```

En JavaScript es habitual combinar **funciones y objetos planos** (como arriba, con `map` y desestructuración) y reservar las clases para entidades con comportamiento. La clase no es obligatoria.

> **Ojo:** `this` depende de **cómo se llama** a la función. Si pasas `task.toggle` como callback pierde el `this`. Las flechas heredan el `this` de donde se escriben.

## 12. Asincronía

JavaScript ejecuta tu código en un solo hilo. Las operaciones lentas (red, temporizadores, archivos) se piden al sistema y su resultado vuelve más tarde como una **Promesa**.

```js
const wait = (ms) => new Promise((resolve) => setTimeout(resolve, ms));

async function main() {
  console.log('inicio');
  await wait(500);            // pausa esta función sin bloquear el programa
  console.log('medio segundo después');
}
main();
```

```js
// En paralelo, si no dependen entre sí
const [users, tasks] = await Promise.all([fetchUsers(), fetchTasks()]);

// Esperar todas aunque alguna falle
const results = await Promise.allSettled([a(), b()]);
```

`await` solo puede usarse dentro de funciones `async` o en el nivel superior de un módulo.

> **Ojo:** olvidar `await` devuelve una promesa pendiente en lugar del dato. Si ves `Promise { <pending> }`, falta un `await`.

## 13. El navegador: DOM y eventos

El **DOM** es el árbol de objetos que representa tu HTML. JavaScript lo lee y lo modifica.

```js
const title = document.querySelector('h1');          // uno
const items = document.querySelectorAll('.task');    // todos (NodeList)

title.textContent = 'Nuevo título';                  // texto seguro
title.classList.add('highlight');
title.dataset.taskId = '7';                          // data-task-id="7"

const li = document.createElement('li');
li.textContent = 'Nueva tarea';
list.append(li);
```

> **Peligro:** `innerHTML` con texto del usuario permite inyectar scripts (XSS). Usa `textContent` o `createElement`. Si necesitas HTML, sanéalo.

### Eventos

```js
button.addEventListener('click', (event) => {
  console.log(event.target);
});

form.addEventListener('submit', (event) => {
  event.preventDefault();                 // evita la recarga
  const data = Object.fromEntries(new FormData(form));
});

// Delegación: un solo oyente para muchos elementos
list.addEventListener('click', (event) => {
  const button = event.target.closest('button[data-action]');
  if (!button) return;
  console.log(button.dataset.action);
});
```

### Ejemplo completo mínimo: lista de tareas

`index.html`:

```html
<!doctype html>
<html lang="es">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <title>Tareas</title>
  <script type="module" src="main.js"></script>
</head>
<body>
  <h1>Tareas</h1>
  <form id="task-form">
    <label for="task-input">Nueva tarea</label>
    <input id="task-input" required minlength="2">
    <button type="submit">Añadir</button>
  </form>
  <ul id="task-list"></ul>
</body>
</html>
```

`main.js`:

```js
const form = document.querySelector('#task-form');
const input = document.querySelector('#task-input');
const list = document.querySelector('#task-list');

// Estado: la fuente de la verdad; la pantalla se dibuja a partir de él
let tasks = JSON.parse(localStorage.getItem('tasks') ?? '[]');

function save() {
  localStorage.setItem('tasks', JSON.stringify(tasks));
}

function render() {
  list.replaceChildren(
    ...tasks.map((task) => {
      const item = document.createElement('li');
      const label = document.createElement('label');
      const checkbox = document.createElement('input');
      checkbox.type = 'checkbox';
      checkbox.checked = task.done;
      checkbox.dataset.id = task.id;
      label.append(checkbox, ' ', task.title);   // el texto se inserta de forma segura
      item.append(label);
      return item;
    }),
  );
}

form.addEventListener('submit', (event) => {
  event.preventDefault();
  const title = input.value.trim();
  if (!title) return;
  tasks = [...tasks, { id: Date.now(), title, done: false }];
  input.value = '';
  save();
  render();
});

list.addEventListener('change', (event) => {
  const id = Number(event.target.dataset.id);
  tasks = tasks.map((t) => (t.id === id ? { ...t, done: event.target.checked } : t));
  save();
  render();
});

render();
```

Fíjate en el patrón **estado → `render()`**: cada cambio actualiza el estado y vuelve a dibujar. React y Vue automatizan exactamente esto.

## 14. Peticiones HTTP con `fetch`

```js
async function request(url, options) {
  const response = await fetch(url, options);
  if (!response.ok) {                    // fetch NO lanza error con 404 o 500
    throw new Error(`HTTP ${response.status}`);
  }
  return response.status === 204 ? null : response.json();
}

const tasks = await request('/api/tasks');                            // listar

await request('/api/tasks', {                                         // crear
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ title: 'Nueva', done: false }),
});

await request('/api/tasks/3', {                                       // editar
  method: 'PATCH',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ done: true }),
});

await request('/api/tasks/3', { method: 'DELETE' });                  // eliminar
```

Cancelar o limitar el tiempo:

```js
const controller = new AbortController();
const timer = setTimeout(() => controller.abort(), 5000);
try {
  const data = await request('/api/tasks', { signal: controller.signal });
} finally {
  clearTimeout(timer);
}
```

> **Ojo:** si el navegador bloquea la petición con un error de **CORS**, no es un fallo de tu JavaScript: el servidor debe permitir tu origen. Se explica en el módulo de Node.

## 15. Pruebas, depuración y calidad

| Herramienta | Uso |
|---|---|
| `console.log`, `console.table`, `console.error` | Inspección rápida |
| DevTools: *Fuentes* | Puntos de interrupción y ejecución paso a paso |
| `debugger;` | Pausa el código si las DevTools están abiertas |
| `node --watch app.js` | Reinicia al guardar |
| `node --test` | Ejecutor de pruebas integrado en Node |
| Oxlint / ESLint | Análisis estático: detecta errores y malas prácticas |
| Prettier | Formato automático |

Prueba mínima con el ejecutor de Node:

```js
// math.test.js
import { test } from 'node:test';
import assert from 'node:assert/strict';
import { add } from './math.js';

test('add suma dos números', () => {
  assert.equal(add(2, 3), 5);
});
```

```bash
node --test
```

## 16. Novedades recientes que merece la pena conocer

| Función | Qué aporta | Edición |
|---|---|---|
| `Array.prototype.toSorted`, `toReversed`, `with` | Operaciones sin mutar | ES2023 |
| `Object.groupBy`, `Map.groupBy` | Agrupar colecciones | ES2024 |
| `Promise.withResolvers` | Crear una promesa y sus controles | ES2024 |
| Métodos de `Set` (`union`, `intersection`…) | Álgebra de conjuntos | ES2025 |
| Iterator helpers | `map`, `filter`, `take` sobre iteradores perezosos | ES2025 |
| `Promise.try` | Ejecutar una función como promesa | ES2025 |
| `structuredClone` | Copia profunda | Web/Node |

Comprueba la compatibilidad del navegador en MDN antes de usarlas en código que se ejecute en el cliente.

## 17. Errores habituales

| Mensaje | Causa | Solución |
|---|---|---|
| `Cannot use import statement outside a module` | Node o el navegador ejecuta el archivo como script clásico | `"type": "module"` en `package.json` o `<script type="module">` |
| `require is not defined` | Mezclas CommonJS con módulos | Usa `import` |
| `Cannot read properties of undefined (reading 'x')` | Accedes a una propiedad de `undefined` | Revisa el origen con `console.log`; usa `?.` |
| `x is not a function` | Nombre mal escrito o `x` no es lo que crees | `console.log(typeof x)` |
| `Promise { <pending> }` | Falta `await` | Añade `await` |
| `Unexpected token < in JSON` | Esperabas JSON y llegó HTML (error del servidor o URL incorrecta) | Mira la respuesta en la pestaña *Red* |
| `Failed to fetch` | CORS, servidor caído o URL errónea | Pestaña *Red* y *Consola* |
| `Uncaught TypeError: Assignment to constant variable` | Reasignas un `const` | Usa `let` o no reasignes |
| El array original cambió | `sort`, `reverse`, `push` mutan | `toSorted`, `toReversed`, `[...a, x]` |

## 18. Ejercicios

> **Ejercicio:** amplía la lista de tareas para que se puedan **eliminar** y **editar** el título, con delegación de eventos.

> **Ejercicio:** añade un filtro (Todas / Pendientes / Hechas) y un contador de pendientes. El filtro es parte del estado.

> **Ejercicio:** escribe una función `groupBy` propia con `reduce` y compárala con `Object.groupBy`. ¿Qué devuelve cada una cuando el array está vacío?

> **Ejercicio:** consume una API pública de tu elección con `fetch`, maneja los estados de carga, error y lista vacía, y pinta el resultado.

## 19. Documentación oficial

- [JavaScript en MDN](https://developer.mozilla.org/es/docs/Web/JavaScript)
- [Guía de JavaScript moderno (javascript.info)](https://es.javascript.info/)
- [Node.js: documentación](https://nodejs.org/docs/latest/api/)
- [Especificación ECMAScript](https://tc39.es/ecma262/)
