# TypeScript

TypeScript es JavaScript con **tipos**. Los tipos se comprueban mientras escribes y al compilar, y **desaparecen** al ejecutar: el navegador y Node ejecutan JavaScript normal. Por eso TypeScript detecta errores antes, pero no valida los datos que llegan en tiempo de ejecución (una API, un formulario).

*Verificado: 2026-10-27. Las plantillas oficiales de Vite instalan hoy **TypeScript 7.0.2** (React y JavaScript puro) o **6.0.3** (Vue): lo comprobé generando los tres proyectos y ejecutando `tsc --version`, `tsc -b` y los builds. Las notas de cambios de las versiones 6.0 y 7.0 salen de cobertura secundaria; contrástalas en [typescriptlang.org](https://www.typescriptlang.org/). Tu tabla declara la 6.0.3: es la que fija la plantilla de Vue.*

## 1. Archivos y nombres

| Extensión | Contenido |
|---|---|
| `.ts` | TypeScript |
| `.tsx` | TypeScript con JSX (componentes React) |
| `.d.ts` | Solo declaraciones de tipos |
| `.mts` / `.cts` | TypeScript como módulo ES / CommonJS |

Los nombres siguen las mismas convenciones que JavaScript: `kebab-case.ts`, `camelCase` para valores, `PascalCase` para tipos, interfaces y clases.

## 2. Instalación: por proyecto, no global

En tu equipo `tsc` no está en el PATH, y es lo correcto: cada proyecto fija su versión.

```bash
pnpm add -D typescript          # en la carpeta del proyecto
pnpm exec tsc --version         # usa el del proyecto
```

Los proyectos de Vite con plantilla TypeScript ya lo incluyen. Para ver cuál tiene el tuyo:

```bash
cd ~/projects/frontend-react
pnpm exec tsc --version
```

## 3. Cuatro formas de ejecutar TypeScript

| Forma | Comprueba tipos | Para qué |
|---|---|---|
| **Vite** (`pnpm dev`, `pnpm build`) | No | Quita los tipos y sirve el JS. Rápido, pero **no avisa de errores de tipo** |
| **`tsc --noEmit`** | Sí | Verificación de tipos; es lo que debe ejecutar tu CI y tu script `typecheck` |
| **Node 24 directamente** (`node archivo.ts`) | No | Elimina tipos y ejecuta; solo admite sintaxis «borrable» (sin `enum` ni `namespace`). Compruébalo con tu versión |
| **tsx** u otros ejecutores | No | Scripts rápidos en desarrollo |

> **Regla:** el editor muestra los errores mientras escribes, pero el proyecto debe tener un script que los compruebe: `"typecheck": "tsc --noEmit"`. Un build con Vite puede pasar con errores de tipo.

## 4. Hola mundo

```ts
// hello.ts
const language: string = 'TypeScript';
console.log(`Hola ${language}`);
```

```bash
pnpm exec tsc --noEmit hello.ts   # solo comprueba
node hello.ts                      # ejecuta (Node 24 elimina los tipos)
```

## 5. Tipos básicos

```ts
const name: string = 'Ana';
const age: number = 30;
const active: boolean = true;
const ids: number[] = [1, 2, 3];
const point: [number, number] = [10, 20];          // tupla
let value: string | number = 'texto';              // unión
const direction: 'left' | 'right' = 'left';        // literal
```

La **inferencia** hace innecesario anotar casi todo:

```ts
const count = 3;                // number
const names = ['a', 'b'];       // string[]
```

> **Regla:** anota los **parámetros y retornos de funciones públicas** y deja que TypeScript infiera las variables locales.

### `any`, `unknown` y `never`

| Tipo | Significado | Cuándo |
|---|---|---|
| `any` | Desactiva la comprobación | Evítalo; contagia |
| `unknown` | «No sé qué es»: debes comprobarlo antes de usarlo | Datos externos (JSON, `catch`) |
| `never` | Nunca ocurre | Comprobaciones de exhaustividad |

```ts
function parse(text: string): unknown {
  return JSON.parse(text);
}
const data = parse('{"a":1}');
if (typeof data === 'object' && data !== null && 'a' in data) {
  console.log(data.a);        // ahora es seguro
}
```

## 6. Objetos: `interface` y `type`

```ts
interface Task {
  id: number;
  title: string;
  done: boolean;
  dueDate?: string;           // opcional
  readonly createdAt: string; // no se puede reasignar
}

type TaskStatus = 'todo' | 'doing' | 'done';
type TaskDraft = Pick<Task, 'title'> & { status: TaskStatus };

const task: Task = { id: 1, title: 'Estudiar TS', done: false, createdAt: '2026-10-27' };
```

| | `interface` | `type` |
|---|---|---|
| Describir objetos | Sí | Sí |
| Extender | `extends` | `&` (intersección) |
| Uniones y literales | No | Sí |
| Cuándo | Contratos de objetos públicos | Uniones, alias y tipos calculados |

Ambos son válidos; elige uno por proyecto y sé constante. Diccionarios:

```ts
const priorities: Record<string, number> = { high: 3, low: 1 };
```

## 7. Funciones y genéricos

```ts
function add(a: number, b: number): number {
  return a + b;
}

const greet = (name: string, greeting = 'Hola'): string => `${greeting} ${name}`;

// Genérico: el tipo se decide al usarlo
function first<T>(items: T[]): T | undefined {
  return items[0];
}
first([1, 2, 3]);      // number | undefined
first(['a', 'b']);     // string | undefined
```

## 8. Estrechamiento de tipos

TypeScript afina el tipo según tus comprobaciones.

```ts
function format(value: string | number): string {
  if (typeof value === 'string') return value.trim();
  return value.toFixed(2);
}
```

### Uniones discriminadas: el patrón más útil

```ts
type LoadState =
  | { status: 'loading' }
  | { status: 'error'; message: string }
  | { status: 'success'; tasks: Task[] };

function describe(state: LoadState): string {
  switch (state.status) {
    case 'loading': return 'Cargando…';
    case 'error':   return `Error: ${state.message}`;
    case 'success': return `${state.tasks.length} tareas`;
    default: {
      const unreachable: never = state;   // si añades un estado y olvidas tratarlo, falla aquí
      return unreachable;
    }
  }
}
```

Este patrón es la forma natural de modelar los estados de carga, error y éxito de una pantalla.

## 9. Tipos de utilidad

| Tipo | Efecto | Ejemplo |
|---|---|---|
| `Partial<T>` | Todo opcional | `Partial<Task>` para una edición parcial |
| `Required<T>` | Todo obligatorio | |
| `Pick<T, K>` | Solo ciertas propiedades | `Pick<Task, 'id' \| 'title'>` |
| `Omit<T, K>` | Todas menos algunas | `Omit<Task, 'id'>` para crear |
| `Readonly<T>` | Todo de solo lectura | |
| `ReturnType<typeof f>` | Tipo devuelto por una función | |
| `Awaited<T>` | Tipo resuelto de una promesa | |

```ts
type NewTask = Omit<Task, 'id' | 'createdAt'>;
type TaskPatch = Partial<NewTask>;
```

## 10. `satisfies`, aserciones y el operador `!`

```ts
// satisfies: comprueba el tipo sin ensanchar el valor
const routes = {
  home: '/',
  tasks: '/tasks',
} satisfies Record<string, string>;

// as: tú sabes más que el compilador. Úsalo con cautela
const input = document.querySelector('#title') as HTMLInputElement;

// ! : afirma que no es null. Evítalo; comprueba mejor
const el = document.querySelector('#title');
if (el instanceof HTMLInputElement) {
  console.log(el.value);
}
```

## 11. Módulos y tipos importados

```ts
import type { Task } from './types';          // solo tipo: desaparece al compilar
import { fetchTasks } from './api';

export type { Task };
```

Con `verbatimModuleSyntax: true` el compilador te obliga a escribir `import type` cuando importas solo tipos. Es lo que hacen las plantillas actuales de Vite.

## 12. Tipar datos de una API

Un `Task[]` declarado **no garantiza** que la API devuelva eso.

```ts
async function fetchTasks(): Promise<Task[]> {
  const response = await fetch('/api/tasks');
  if (!response.ok) throw new Error(`HTTP ${response.status}`);
  const data: unknown = await response.json();
  if (!Array.isArray(data)) throw new Error('Respuesta inesperada');
  return data as Task[];     // asunción: no hay validación de cada elemento
}
```

Para validar de verdad en ejecución se usan librerías de esquemas (Zod, Valibot), que definen la forma una vez y derivan el tipo. Se verán en el módulo de Node.

## 13. TypeScript con el DOM

```ts
const form = document.querySelector<HTMLFormElement>('#task-form');
const input = document.querySelector<HTMLInputElement>('#task-input');
if (!form || !input) throw new Error('Falta el formulario');

form.addEventListener('submit', (event: SubmitEvent) => {
  event.preventDefault();
  console.log(input.value);
});
```

`querySelector` devuelve `Element | null`; el genérico le dice qué elemento esperas y la comprobación de `null` lo hace seguro.

## 14. `tsconfig.json`

Es el archivo que dice al compilador y al editor cómo interpretar el proyecto.

```json
{
  "compilerOptions": {
    "target": "ES2022",
    "lib": ["ES2023", "DOM", "DOM.Iterable"],
    "module": "ESNext",
    "moduleResolution": "bundler",
    "strict": true,
    "noEmit": true,
    "verbatimModuleSyntax": true,
    "isolatedModules": true,
    "skipLibCheck": true,
    "types": ["vite/client"]
  },
  "include": ["src"]
}
```

| Campo | Qué hace |
|---|---|
| `target` | Versión de JavaScript que produce la salida |
| `lib` | APIs que el compilador considera disponibles (JS y DOM) |
| `module` / `moduleResolution` | Cómo se interpretan los `import`; `bundler` es lo habitual con Vite |
| `strict` | Activa el conjunto de comprobaciones estrictas. **Déjalo en `true`** |
| `noEmit` | No genera archivos: solo comprueba (Vite se encarga del JS) |
| `verbatimModuleSyntax` | Obliga a distinguir `import type` |
| `skipLibCheck` | No revisa los `.d.ts` de las librerías: más rápido |
| `types` | Qué paquetes `@types` se cargan automáticamente |
| `include` | Qué carpetas se comprueban |

### TypeScript 6.0 y 7.0: qué cambió

**TypeScript 7.0** reescribe el compilador en Go (compilador *nativo*): el comando sigue siendo `tsc` y el lenguaje es el mismo, pero los builds son mucho más rápidos. **6.0** fue la última versión sobre el compilador en JavaScript y sirve de puente: deja en desuso lo que 7.0 elimina. Según la cobertura de su lanzamiento, y a confirmar en las notas oficiales:

- `strict` pasa a valer `true` por defecto.
- `module` pasa a `esnext` por defecto y `target` sigue el último estándar ECMAScript.
- `types` pasa a ser una lista vacía por defecto: si usas `process` u otros globales de Node, necesitas `"types": ["node"]` (y `pnpm add -D @types/node`).
- Opciones antiguas como `target: es5`, `moduleResolution: node` y `baseUrl` están en desuso; `"ignoreDeprecations": "6.0"` las silencia de forma temporal.
- **7.0 aún no ofrece una API programática estable** (se espera en 7.1 o posterior). Por eso herramientas que usan la API del compilador, como `vue-tsc`, siguen en 6.0: **es la razón de que la plantilla de Vue fije `typescript ~6.0.3`** mientras las de React y JavaScript puro usan `~7.0.2`.

> **Regla:** mira la versión que fija cada proyecto en su `package.json` y no la cambies sin comprobar que las herramientas que dependen de ella (`vue-tsc`, `typescript-eslint`) la soportan.

La configuración real de la plantilla de React (con `erasableSyntaxOnly`, `noUnusedLocals`, `target: es2023`…) está comentada campo a campo en [Manifiestos y configuración](#config).

Las plantillas de Vite suelen dividir la configuración en `tsconfig.json`, `tsconfig.app.json` y `tsconfig.node.json` (código de la app y código de herramientas). Es normal; abre `tsconfig.app.json` para ver las reglas que afectan a `src/`.

## 15. JS puro con TypeScript y Vite

Un proyecto sin framework, solo con TypeScript:

```bash
cd ~/projects
pnpm create vite@latest my-ts-app --template vanilla-ts
cd my-ts-app
pnpm install
pnpm dev                 # servidor de desarrollo
```

Estructura que se genera (puede variar entre versiones de Vite):

```text
my-ts-app/
  index.html          punto de entrada; enlaza src/main.ts
  package.json        scripts y dependencias
  tsconfig.json       reglas de TypeScript
  public/             archivos estáticos servidos tal cual
  src/
    main.ts           código de la aplicación
    style.css         estilos
```

Añade el script de comprobación a `package.json`:

```json
{
  "scripts": {
    "dev": "vite",
    "build": "tsc && vite build",
    "typecheck": "tsc --noEmit",
    "preview": "vite preview"
  }
}
```

| Comando | Para qué |
|---|---|
| `pnpm dev` | Servidor con recarga al guardar |
| `pnpm typecheck` | Comprueba los tipos |
| `pnpm build` | Genera la versión de producción en `dist/` |
| `pnpm preview` | Sirve `dist/` localmente para probar el build |

Para los tipos de las variables de entorno de Vite (`import.meta.env.VITE_*`), el archivo `src/vite-env.d.ts` o `types: ["vite/client"]` aportan la declaración.

## 16. Errores habituales

Lee el error **de abajo hacia arriba**: la última línea suele decir la causa concreta.

| Mensaje | Causa | Solución |
|---|---|---|
| `Property 'x' does not exist on type 'Y'` | El tipo no tiene esa propiedad | Corrige el nombre o amplía el tipo |
| `'x' is possibly 'null'` / `'undefined'` | Puede no existir | Comprueba antes (`if`, `?.`, `??`) |
| `Type 'X' is not assignable to type 'Y'` | Formas incompatibles | Compara ambas formas; revisa opcionales y uniones |
| `Cannot find name 'process'` | Faltan los tipos de Node | `pnpm add -D @types/node` y `"types": ["node"]` |
| `Cannot find module './x'` | Ruta o resolución incorrecta | Revisa la ruta, `moduleResolution` y la extensión |
| `Parameter 'x' implicitly has an 'any' type` | Falta anotar un parámetro | Escribe su tipo |
| `'X' is a type and must be imported using a type-only import` | `verbatimModuleSyntax` activo | Usa `import type` |
| El editor marca error y `pnpm dev` funciona | Vite no comprueba tipos | Ejecuta `pnpm typecheck` |

## 17. Ejercicios

> **Ejercicio:** convierte la lista de tareas de JavaScript (módulo anterior) a TypeScript con Vite. Define `Task` y `TaskDraft`, y haz que `pnpm typecheck` pase sin errores y sin `any`.

> **Ejercicio:** modela con una unión discriminada los estados de la lista (`loading`, `error`, `empty`, `success`) y escribe `render(state)` con un `switch` exhaustivo.

> **Ejercicio:** escribe a mano un *type guard* `isTask(value: unknown): value is Task` y úsalo para validar la respuesta de una API. Después piensa qué ventaja tendría una librería de esquemas.

## 18. Documentación oficial

- [Manual de TypeScript](https://www.typescriptlang.org/docs/handbook/intro.html)
- [Referencia de tsconfig](https://www.typescriptlang.org/tsconfig/)
- [Guía de Vite](https://vite.dev/guide/)
