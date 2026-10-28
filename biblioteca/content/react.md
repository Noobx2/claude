# React + Vite

React es una biblioteca para construir interfaces con **componentes**: funciones que reciben datos (*props*) y devuelven lo que debe verse. Cuando el **estado** cambia, React vuelve a ejecutar el componente y actualiza solo lo que cambió en la pantalla. Vite es la herramienta que sirve el proyecto en desarrollo (con recarga instantánea) y lo empaqueta para producción.

*Verificado: 2026-10-27. Todo el código de este módulo se ha compilado con `tsc`, pasado por Oxlint, probado con Vitest y ejecutado en un navegador real (Chromium) contra la API del módulo de [Node.js](#node). Versiones usadas: React 19.3.1, Vite 8.3.5, TypeScript 7.0.2, react-router 8.5, Vitest 5.0. Referencia: [react.dev](https://react.dev/) y [vite.dev](https://vite.dev/).*

## 1. Cuándo React y cuándo no

| Enfoque | Cuándo |
|---|---|
| HTML + formularios | Sitios de contenido, formularios simples ([HTML5](#html5)) |
| JS puro con `fetch` | Una o dos zonas interactivas ([JavaScript](#js)) |
| **React / Vue** | Muchas vistas, mucho estado, componentes reutilizables, equipos |

React no es obligatorio. El patrón «estado → `render()`» de la lista de tareas en JS puro es exactamente lo que React automatiza.

## 2. Crear el proyecto

```bash
cd ~/projects
pnpm create vite@latest tasks-web --template react-ts
cd tasks-web
pnpm install
pnpm dev
```

| Pieza | Significado |
|---|---|
| `pnpm create vite@latest` | Ejecuta el generador oficial de Vite en su última versión |
| `tasks-web` | Nombre de la carpeta del proyecto |
| `--template react-ts` | Plantilla React con TypeScript (otras: `react`, `vue-ts`, `vanilla-ts`) |
| `pnpm install` | Instala las dependencias de `package.json` |
| `pnpm dev` | Arranca el servidor de desarrollo, normalmente en `http://localhost:5173` |

Desde una VM, usa `pnpm dev --host` para que escuche en todas las interfaces, o accede mediante el reenvío de puertos de VS Code Remote. `--host` expone el servidor a tu red: úsalo solo en redes de confianza.

## 3. Qué genera la plantilla

```text
tasks-web/
  index.html            página única; contiene <div id="root"> y carga src/main.tsx
  package.json          dependencias y scripts
  vite.config.ts        configuración de Vite
  tsconfig.json         raíz de TypeScript: solo referencia a los otros dos
  tsconfig.app.json     reglas para el código de la app (src/)
  tsconfig.node.json    reglas para el código de herramientas (vite.config.ts)
  .oxlintrc.json        reglas de Oxlint
  .gitignore
  public/               archivos estáticos que se sirven tal cual
  src/
    main.tsx            punto de entrada: monta React en #root
    App.tsx             componente raíz
    index.css, App.css  estilos
    assets/             imágenes importables
```

```json
{
  "name": "app-react-ts",
  "private": true,
  "version": "0.0.0",
  "type": "module",
  "scripts": {
    "dev": "vite",
    "build": "tsc -b && vite build",
    "lint": "oxlint",
    "preview": "vite preview"
  },
  "dependencies": {
    "react": "^19.3.1",
    "react-dom": "^19.3.1"
  },
  "devDependencies": {
    "@types/node": "^24.19.2",
    "@types/react": "^19.3.0",
    "@types/react-dom": "^19.3.0",
    "@vitejs/plugin-react": "^6.1.2",
    "oxlint": "^1.89.0",
    "typescript": "~7.0.2",
    "vite": "^8.3.5"
  }
}
```

| Parte de `package.json` | Significado |
|---|---|
| `"type": "module"` | El proyecto usa módulos ES |
| `scripts.dev` | `vite`: servidor de desarrollo |
| `scripts.build` | `tsc -b && vite build`: **primero** comprueba los tipos y después empaqueta |
| `scripts.lint` | `oxlint`: análisis estático |
| `scripts.preview` | `vite preview`: sirve la carpeta `dist/` para probar el build |
| `dependencies` | `react` y `react-dom` (el que dibuja en el navegador) |
| `devDependencies` | Herramientas: TypeScript, Vite, el plugin de React, Oxlint, tipos |

`vite.config.ts` de la plantilla:

```ts
import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
})
```

`tsconfig.app.json` (los campos importantes: `jsx: "react-jsx"` permite escribir JSX sin importar React, `moduleResolution: "bundler"` y `noEmit` porque Vite genera el JS, y las comprobaciones `noUnused*`):

```json
{
  "compilerOptions": {
    "tsBuildInfoFile": "./node_modules/.tmp/tsconfig.app.tsbuildinfo",
    "target": "es2023",
    "lib": ["ES2023", "DOM"],
    "module": "esnext",
    "types": ["vite/client"],
    "allowArbitraryExtensions": true,
    "skipLibCheck": true,

    /* Bundler mode */
    "moduleResolution": "bundler",
    "allowImportingTsExtensions": true,
    "verbatimModuleSyntax": true,
    "moduleDetection": "force",
    "noEmit": true,
    "jsx": "react-jsx",

    /* Linting */
    "noUnusedLocals": true,
    "noUnusedParameters": true,
    "erasableSyntaxOnly": true,
    "noFallthroughCasesInSwitch": true
  },
  "include": ["src"]
}
```

## 4. Qué es JSX

JSX es sintaxis para describir interfaz dentro de JavaScript. No es HTML: se convierte en llamadas a funciones.

```tsx
const name = 'Ana';
const element = (
  <h1 className="title">
    Hola {name}, tienes {2 + 3} tareas
  </h1>
);
```

| Diferencia con HTML | Motivo |
|---|---|
| `className` en lugar de `class` | `class` es palabra reservada de JS |
| `htmlFor` en lugar de `for` | Ídem |
| `{ ... }` inserta cualquier **expresión** JS | No sentencias (`if`, `for`) |
| Estilos en objeto: `style={{ color: 'red' }}` | Las propiedades van en `camelCase` |
| Toda etiqueta se cierra: `<input />` | Es más estricto que HTML |
| Un solo elemento raíz; `<>…</>` (fragmento) agrupa sin añadir nodos | |
| Eventos en `camelCase`: `onClick`, `onChange`, `onSubmit` | |

## 5. Componentes, props y estado

Un componente es una función cuyo nombre empieza por **mayúscula** y que devuelve JSX.

```tsx
interface GreetingProps {
  name: string;
  times?: number; // opcional
}

function Greeting({ name, times = 1 }: GreetingProps) {
  return <p>{'¡Hola, ' + name + '! '.repeat(times)}</p>;
}

// Uso: <Greeting name="Ana" times={2} />
```

- **Props**: datos que **recibe** el componente. Son de solo lectura.
- **Estado**: datos que el componente **posee** y que, al cambiar, provocan un nuevo renderizado.

```tsx
import { useState } from 'react';

function Counter() {
  const [count, setCount] = useState(0); // [valor actual, función para cambiarlo]
  return <button onClick={() => setCount(count + 1)}>Clics: {count}</button>;
}
```

| Pieza | Significado |
|---|---|
| `useState(0)` | Crea una variable de estado con valor inicial 0 |
| `count` | El valor en este renderizado |
| `setCount(n)` | Pide a React cambiar el valor y volver a dibujar |
| `setCount((c) => c + 1)` | Forma funcional: calcula desde el valor más reciente |

> **Regla:** **nunca modifiques el estado directamente** (`tasks.push(x)`). Crea un valor nuevo: `setTasks([...tasks, x])`. React detecta el cambio comparando referencias.

### Cómo se actualiza la pantalla

```text
Evento (clic) ─▶ setState ─▶ React vuelve a ejecutar el componente ─▶ compara con lo anterior ─▶ cambia solo lo necesario del DOM
```

Un renderizado es **solo calcular** qué debe verse; no debe tener efectos secundarios (peticiones, `console.log` importantes, cambios fuera del componente).

## 6. La aplicación de tareas, paso a paso

Estructura de `src/`:

```text
src/
  main.tsx                    arranque
  App.tsx                     pantalla: filtro + formulario + lista
  types.ts                    tipos compartidos (Task, Filter)
  api/tasks-api.ts            comunicación con el servidor
  hooks/use-tasks.ts          estado de las tareas y sus acciones
  components/
    TaskForm.tsx              formulario con validación
    TaskList.tsx              lista o mensaje de «vacío»
    TaskItem.tsx              una tarea con confirmación de borrado
```

**Responsabilidades.** Los componentes solo muestran y recogen eventos; el *hook* `useTasks` gestiona los datos; `tasks-api.ts` conoce la URL y el formato de la API. Si cambias el servidor, solo tocas `tasks-api.ts`.

### Tipos

```ts
export interface Task {
  id: string;
  title: string;
  done: boolean;
}

export type Filter = 'all' | 'pending' | 'done';
```

### Capa de red: peticiones HTTP y JSON

```ts
import type { Task } from '../types.ts';

// La URL base sale de una variable de Vite (solo las que empiezan por VITE_ llegan al navegador)
const API_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:3000/api';

async function request<T>(path: string, options: RequestInit = {}): Promise<T> {
  // Content-Type solo si hay cuerpo: Fastify rechaza un DELETE sin cuerpo que declare JSON
  const headers = options.body ? { 'Content-Type': 'application/json' } : undefined;
  const response = await fetch(`${API_URL}${path}`, { ...options, headers });
  if (!response.ok) {
    const body = (await response.json().catch(() => null)) as { error?: string } | null;
    throw new Error(body?.error ?? `Error HTTP ${response.status}`);
  }
  return response.status === 204 ? (undefined as T) : ((await response.json()) as T);
}

export const tasksApi = {
  list: (signal?: AbortSignal) => request<Task[]>('/tasks', { signal }),
  create: (title: string) =>
    request<Task>('/tasks', { method: 'POST', body: JSON.stringify({ title }) }),
  update: (id: string, changes: Partial<Pick<Task, 'title' | 'done'>>) =>
    request<Task>(`/tasks/${id}`, { method: 'PATCH', body: JSON.stringify(changes) }),
  remove: (id: string) => request<void>(`/tasks/${id}`, { method: 'DELETE' }),
};
```

| Pieza | Significado |
|---|---|
| `import.meta.env.VITE_API_URL` | Variable de entorno de Vite. Solo las que empiezan por `VITE_` llegan al navegador, y **son públicas** |
| `request<T>` | Función genérica: `T` es el tipo que se espera recibir |
| `response.ok` | `fetch` no lanza error con 404 o 500: hay que comprobarlo |
| `Content-Type` solo con cuerpo | Fastify rechaza un `DELETE` sin cuerpo que declare JSON |
| `204` | Sin contenido: no hay JSON que leer |
| `as Task[]` | Afirmación de tipo: el JSON no se valida en ejecución (ver [TypeScript](#ts)) |

### El hook de datos

```ts
import { useEffect, useState } from 'react';
import { tasksApi } from '../api/tasks-api.ts';
import type { Task } from '../types.ts';

export function useTasks() {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Carga inicial. Los setState ocurren dentro de las promesas (no de forma síncrona en el efecto)
  useEffect(() => {
    const controller = new AbortController();
    tasksApi
      .list(controller.signal)
      .then((data) => setTasks(data))
      .catch((caught: unknown) => {
        if (controller.signal.aborted) return; // el componente se desmontó: no es un error
        setError(caught instanceof Error ? caught.message : 'Error desconocido');
      })
      .finally(() => {
        if (!controller.signal.aborted) setLoading(false);
      });
    return () => controller.abort(); // limpieza al desmontar
  }, []);

  // Reintento manual tras un error
  async function reload() {
    setLoading(true);
    setError(null);
    try {
      setTasks(await tasksApi.list());
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'Error desconocido');
    } finally {
      setLoading(false);
    }
  }

  // Cada acción actualiza el servidor y después el estado local con la respuesta
  async function addTask(title: string) {
    const created = await tasksApi.create(title);
    setTasks((current) => [...current, created]);
  }

  async function toggleTask(task: Task) {
    const updated = await tasksApi.update(task.id, { done: !task.done });
    setTasks((current) => current.map((item) => (item.id === task.id ? updated : item)));
  }

  async function removeTask(id: string) {
    await tasksApi.remove(id);
    setTasks((current) => current.filter((item) => item.id !== id));
  }

  return { tasks, loading, error, reload, addTask, toggleTask, removeTask };
}
```

- `useEffect(() => {…}, [])` ejecuta la carga **una vez**, al montar. El `return` es la **limpieza**: se ejecuta al desmontar y aquí cancela la petición.
- `loading` y `error` son estados explícitos: la interfaz siempre sabe en qué situación está.
- Las acciones actualizan el servidor y después el estado con **la respuesta** del servidor, para no mostrar algo que el servidor rechazó.

### Formulario con validación

```tsx
import { useState, type FormEvent } from 'react';

interface TaskFormProps {
  onAdd: (title: string) => Promise<void>;
}

export function TaskForm({ onAdd }: TaskFormProps) {
  const [title, setTitle] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const trimmed = title.trim();
    if (trimmed.length < 2) {
      setError('Escribe al menos 2 caracteres');
      return;
    }
    setSaving(true);
    setError(null);
    try {
      await onAdd(trimmed);
      setTitle('');
    } catch (caught) {
      setError(caught instanceof Error ? caught.message : 'No se pudo guardar');
    } finally {
      setSaving(false);
    }
  }

  return (
    <form onSubmit={handleSubmit} noValidate>
      <label htmlFor="task-title">Nueva tarea</label>
      <input
        id="task-title"
        value={title}
        onChange={(event) => setTitle(event.target.value)}
        aria-invalid={error !== null}
        aria-describedby={error ? 'task-title-error' : undefined}
      />
      <button type="submit" disabled={saving}>
        {saving ? 'Guardando…' : 'Añadir'}
      </button>
      {error && (
        <p id="task-title-error" role="alert">
          {error}
        </p>
      )}
    </form>
  );
}
```

| Pieza | Significado |
|---|---|
| `value` + `onChange` | **Componente controlado**: React es la fuente de verdad del valor del campo |
| `event.preventDefault()` | Evita la recarga de página que haría el navegador |
| `noValidate` | Desactiva los mensajes nativos para usar los propios (puedes mantenerlos si prefieres) |
| `aria-invalid`, `aria-describedby`, `role="alert"` | Accesibilidad: los lectores de pantalla anuncian el error |
| `try / catch / finally` | Si el servidor falla, se muestra el error; el botón se reactiva siempre |
| `FormEvent<HTMLFormElement>` | Tipo del evento de envío |

> **Ojo:** valida siempre **también en el servidor**. La validación del cliente es comodidad, no seguridad.

### Elemento de lista con confirmación

```tsx
import { useState } from 'react';
import type { Task } from '../types.ts';

interface TaskItemProps {
  task: Task;
  onToggle: (task: Task) => Promise<void>;
  onRemove: (id: string) => Promise<void>;
}

export function TaskItem({ task, onToggle, onRemove }: TaskItemProps) {
  const [confirming, setConfirming] = useState(false);

  return (
    <li>
      <label>
        <input type="checkbox" checked={task.done} onChange={() => void onToggle(task)} />
        <span className={task.done ? 'done' : undefined}>{task.title}</span>
      </label>
      {confirming ? (
        <span>
          ¿Eliminar?{' '}
          <button type="button" onClick={() => void onRemove(task.id)}>
            Sí
          </button>{' '}
          <button type="button" onClick={() => setConfirming(false)}>
            No
          </button>
        </span>
      ) : (
        <button type="button" onClick={() => setConfirming(true)}>
          Eliminar
        </button>
      )}
    </li>
  );
}
```

El estado `confirming` es **local** del componente: nadie más necesita saberlo. Cada elemento tiene el suyo.

### Lista y estado vacío

```tsx
import type { Task } from '../types.ts';
import { TaskItem } from './TaskItem.tsx';

interface TaskListProps {
  tasks: Task[];
  onToggle: (task: Task) => Promise<void>;
  onRemove: (id: string) => Promise<void>;
}

export function TaskList({ tasks, onToggle, onRemove }: TaskListProps) {
  if (tasks.length === 0) {
    return <p>No hay tareas. ¡Añade la primera!</p>;
  }
  return (
    <ul>
      {tasks.map((task) => (
        <TaskItem key={task.id} task={task} onToggle={onToggle} onRemove={onRemove} />
      ))}
    </ul>
  );
}
```

`key={task.id}` ayuda a React a identificar qué elemento es cuál al reordenar o borrar. Debe ser **estable y único**; no uses el índice del array si la lista cambia.

### Pantalla principal

```tsx
import { useMemo, useState } from 'react';
import { TaskForm } from './components/TaskForm.tsx';
import { TaskList } from './components/TaskList.tsx';
import { useTasks } from './hooks/use-tasks.ts';
import type { Filter } from './types.ts';

const FILTERS: { value: Filter; label: string }[] = [
  { value: 'all', label: 'Todas' },
  { value: 'pending', label: 'Pendientes' },
  { value: 'done', label: 'Hechas' },
];

export default function App() {
  const { tasks, loading, error, reload, addTask, toggleTask, removeTask } = useTasks();
  const [filter, setFilter] = useState<Filter>('all');

  // Valor derivado: se calcula a partir del estado, no se guarda aparte
  const visibleTasks = useMemo(
    () =>
      tasks.filter((task) =>
        filter === 'all' ? true : filter === 'done' ? task.done : !task.done,
      ),
    [tasks, filter],
  );
  const pendingCount = tasks.filter((task) => !task.done).length;

  return (
    <main>
      <h1>Tareas</h1>
      <TaskForm onAdd={addTask} />

      <nav aria-label="Filtro">
        {FILTERS.map(({ value, label }) => (
          <button
            key={value}
            type="button"
            aria-pressed={filter === value}
            onClick={() => setFilter(value)}
          >
            {label}
          </button>
        ))}
      </nav>

      {loading && <p>Cargando…</p>}
      {error && (
        <p role="alert">
          {error} <button onClick={() => void reload()}>Reintentar</button>
        </p>
      )}
      {!loading && !error && (
        <>
          <p>{pendingCount} pendientes</p>
          <TaskList tasks={visibleTasks} onToggle={toggleTask} onRemove={removeTask} />
        </>
      )}
    </main>
  );
}
```

| Concepto | Dónde se ve |
|---|---|
| Estado de interfaz (`filter`) | `useState` en `App` |
| Datos del servidor (`tasks`) | Hook `useTasks` |
| **Valor derivado** | `visibleTasks` y `pendingCount` se calculan, no se guardan |
| Renderizado condicional | `{loading && …}`, `{error && …}`, `{!loading && !error && …}` |
| Los cuatro estados de una pantalla | Cargando, error con reintento, vacío (en `TaskList`) y datos |

```tsx
import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import App from './App.tsx';
import './index.css';

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <App />
  </StrictMode>,
);
```

`StrictMode` ejecuta algunos pasos dos veces **solo en desarrollo** para destapar errores; no afecta a producción.

## 7. Ejecutar y comprobar

Dos terminales:

```bash
# Terminal 1: la API (módulo Node.js)
cd ~/projects/tasks-api && PORT=3000 node --watch src/server.js

# Terminal 2: la interfaz
cd ~/projects/tasks-web && cp .env.example .env && pnpm dev
```

Abre `http://localhost:5173`. Pruebas manuales: añadir con menos de 2 caracteres (error), añadir, marcar, filtrar, eliminar con confirmación, **parar la API** y recargar (estado de error con «Reintentar»).

## 8. Navegación y rutas

Con React Router, la URL decide qué componente se dibuja.

```bash
pnpm add react-router
```

```tsx
import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { createBrowserRouter, RouterProvider } from 'react-router';
import App from './App.tsx';
import { Layout } from './Layout.tsx';
import { AboutPage } from './pages/AboutPage.tsx';
import { TaskDetailPage } from './pages/TaskDetailPage.tsx';
import './index.css';

const router = createBrowserRouter([
  {
    path: '/',
    element: <Layout />,
    children: [
      { index: true, element: <App /> },
      { path: 'tasks/:id', element: <TaskDetailPage /> },
      { path: 'about', element: <AboutPage /> },
      { path: '*', element: <p>Página no encontrada</p> },
    ],
  },
]);

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <RouterProvider router={router} />
  </StrictMode>,
);
```

```tsx
import { NavLink, Outlet } from 'react-router';

// Marco común de todas las páginas; <Outlet /> es el hueco donde se dibuja la ruta activa
export function Layout() {
  return (
    <>
      <header>
        <nav aria-label="Principal">
          <NavLink to="/">Tareas</NavLink> <NavLink to="/about">Acerca de</NavLink>
        </nav>
      </header>
      <Outlet />
    </>
  );
}
```

```tsx
import { Link, useParams } from 'react-router';

export function TaskDetailPage() {
  const { id } = useParams(); // lee el parámetro :id de la URL
  return (
    <section>
      <h2>Tarea {id}</h2>
      <Link to="/">Volver a la lista</Link>
    </section>
  );
}
```

| Pieza | Significado |
|---|---|
| `createBrowserRouter` | Define la tabla de rutas |
| `children` + `<Outlet />` | Rutas anidadas: el layout se mantiene y cambia el hueco |
| `index: true` | Ruta por defecto del padre (`/`) |
| `path: 'tasks/:id'` | `:id` es un parámetro; `useParams()` lo lee |
| `path: '*'` | Cualquier otra URL: página 404 |
| `NavLink` / `Link` | Enlaces **sin recargar la página** (usa `<a href>` solo para sitios externos) |

> **Ojo:** al servir el build con otro servidor, las rutas del navegador (`/about`) deben redirigirse a `index.html`; `vite preview` y `pnpm dev` ya lo hacen.

## 9. Componentes reutilizables y composición

- Un componente hace **una cosa**. Si tiene más de ~100 líneas o mezcla datos con presentación, divídelo.
- **`children`**: un componente puede envolver contenido ajeno.

```tsx
import type { ReactNode } from 'react';

function Card({ title, children }: { title: string; children: ReactNode }) {
  return (
    <section className="card">
      <h2>{title}</h2>
      {children}
    </section>
  );
}

// <Card title="Resumen"><p>3 tareas</p></Card>
```

- Los hooks personalizados (`useTasks`) reutilizan **lógica con estado**; los componentes reutilizan **interfaz**.

## 10. Comunicación entre componentes y gestión de estado

| Necesidad | Solución |
|---|---|
| Padre → hijo | Props |
| Hijo → padre | Funciones pasadas por props (`onAdd`, `onToggle`) |
| Dos hermanos comparten estado | **Subir el estado** al padre común |
| Muchos niveles de componentes | Context (`createContext` / `useContext`) |
| Datos del servidor con caché y reintentos | TanStack Query (librería de consulta de datos) |
| Estado global complejo | Zustand, Redux Toolkit |

> **Regla:** empieza con `useState` y subir el estado. Añade librerías solo cuando notes el dolor concreto.

## 11. Estilos, diseño adaptable y accesibilidad

Estilos de la aplicación, con las técnicas de [CSS moderno](#css):

```css
:root {
  color-scheme: light dark;
  font-family: system-ui, sans-serif;
}
main {
  max-width: 32rem;
  margin: 2rem auto;
  padding-inline: 1rem;
}
.done {
  text-decoration: line-through;
  opacity: 0.6;
}
button[aria-pressed='true'] {
  font-weight: 700;
}
```

- Un archivo CSS global importado desde `main.tsx` basta para empezar. Alternativas: CSS Modules (`TaskItem.module.css`, nombres de clase locales) o Tailwind.
- Diseño adaptable: `max-width`, `padding-inline`, `clamp()`, grid/flex; prueba el ancho móvil en las DevTools.
- Accesibilidad: elementos nativos (`button`, `label`, `nav`), `aria-pressed` en botones de filtro, mensajes de error con `role="alert"`, foco visible y navegación por teclado.

## 12. Pruebas

Vitest ejecuta las pruebas con el mismo Vite; Testing Library prueba lo que ve el usuario.

```bash
pnpm add -D vitest jsdom @testing-library/react @testing-library/user-event @testing-library/jest-dom
```

```ts
/// <reference types="vitest/config" />
import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  test: {
    environment: 'jsdom',
    globals: true,
    setupFiles: './src/test-setup.ts',
  },
})
```

```ts
import '@testing-library/jest-dom/vitest';
```

```tsx
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { describe, expect, it, vi } from 'vitest';
import { TaskForm } from './TaskForm.tsx';

describe('TaskForm', () => {
  it('muestra un error si el título es demasiado corto', async () => {
    const onAdd = vi.fn().mockResolvedValue(undefined);
    render(<TaskForm onAdd={onAdd} />);

    await userEvent.type(screen.getByLabelText('Nueva tarea'), 'x');
    await userEvent.click(screen.getByRole('button', { name: 'Añadir' }));

    expect(screen.getByRole('alert')).toHaveTextContent('al menos 2 caracteres');
    expect(onAdd).not.toHaveBeenCalled();
  });

  it('llama a onAdd con el título sin espacios sobrantes y vacía el campo', async () => {
    const onAdd = vi.fn().mockResolvedValue(undefined);
    render(<TaskForm onAdd={onAdd} />);

    await userEvent.type(screen.getByLabelText('Nueva tarea'), '  Estudiar React  ');
    await userEvent.click(screen.getByRole('button', { name: 'Añadir' }));

    expect(onAdd).toHaveBeenCalledWith('Estudiar React');
    expect(screen.getByLabelText('Nueva tarea')).toHaveValue('');
  });
});
```

Añade `"test": "vitest run"` a los scripts y ejecuta:

```bash
pnpm test
```

| Pieza | Significado |
|---|---|
| `environment: 'jsdom'` | Simula un navegador dentro de Node |
| `render(<Comp />)` | Dibuja el componente |
| `screen.getByLabelText / getByRole` | Busca como lo haría un usuario (por etiqueta y rol, no por clase CSS) |
| `userEvent.type / click` | Simula interacción real |
| `vi.fn()` | Función espía: permite comprobar con qué se llamó |
| `vitest run` | Ejecuta una vez (sin modo vigilancia) |

## 13. Depuración

- **React Developer Tools** (extensión del navegador): árbol de componentes, props y estado.
- Pestaña *Red* de las DevTools: ¿se envió la petición?, ¿qué código y qué cuerpo devolvió?
- `console.log` dentro del componente y dentro de los efectos.
- Puntos de interrupción en la pestaña *Fuentes* (Vite genera mapas de código fuente).

## 14. Variables de entorno y producción

```bash
# .env.example
VITE_API_URL=http://localhost:3000/api
```

| Archivo | Cuándo se usa |
|---|---|
| `.env` | Siempre |
| `.env.development` | Con `pnpm dev` |
| `.env.production` | Con `pnpm build` |
| `.env.local` | Tu máquina; ignorado por Git |

Las variables se **incrustan en el código en el momento de compilar**: cambiar `.env` exige volver a compilar.

```bash
pnpm lint
pnpm build
pnpm preview
```

`pnpm build` comprueba los tipos y genera `dist/` (HTML, JS y CSS con nombres con *hash*, minificados). `dist/` son archivos estáticos: se sirve con nginx, Caddy, un CDN o desde la propia API. No incluyas secretos.

Contenedor para producción (sirve `dist/` con nginx):

```dockerfile
FROM node:24-slim AS build
WORKDIR /app
RUN npm install -g pnpm
COPY package.json pnpm-lock.yaml ./
RUN pnpm install --frozen-lockfile
COPY . .
ARG VITE_API_URL
ENV VITE_API_URL=$VITE_API_URL
RUN pnpm build

FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
EXPOSE 80
```

`ARG` recibe el valor al construir (`docker build --build-arg VITE_API_URL=https://api.example.com/api .`), porque Vite lo incrusta al compilar.

## 15. Los hooks esenciales

| Hook | Para qué | Regla |
|---|---|---|
| `useState` | Estado local | No mutar; usar la función `set` |
| `useEffect` | Sincronizar con algo **externo** (red, suscripciones, temporizadores) | Dependencias completas; devolver limpieza |
| `useMemo` | Cachear un cálculo costoso | Solo si mides un problema |
| `useCallback` | Cachear una función | Idem |
| `useRef` | Referencia a un elemento del DOM o valor que no provoca renderizado | `ref.current` |
| `useContext` | Leer un contexto | |

**Reglas de los hooks**: se llaman solo en el nivel superior de un componente o de otro hook (nunca dentro de `if`, bucles o funciones anidadas), y su nombre empieza por `use`. Oxlint lo comprueba (`react/rules-of-hooks`).

## 16. Errores habituales

| Síntoma | Causa | Solución |
|---|---|---|
| La pantalla no se actualiza | Mutaste el estado | Crea una copia nueva |
| Bucle infinito de renderizados | `setState` durante el renderizado o efecto con dependencias mal puestas | Mueve el `setState` a un evento o ajusta las dependencias |
| `Each child in a list should have a unique "key"` | Falta `key` | `key={item.id}` |
| `Cannot read properties of undefined` | Usas datos antes de que lleguen | Estados `loading` o valores iniciales |
| Los datos se piden dos veces en desarrollo | `StrictMode` | Es normal solo en desarrollo; usa limpieza en el efecto |
| `CORS policy` en la consola | La API no permite el origen o el método (`PATCH`, `DELETE`) | Configura CORS en el servidor |
| `import.meta.env.X` es `undefined` | Falta el prefijo `VITE_` o reiniciar `pnpm dev` | Renombra la variable y reinicia |
| 404 al recargar `/about` en producción | El servidor no redirige a `index.html` | Configura el *fallback* del servidor |
| `Warning: A component is changing an uncontrolled input to be controlled` | `value` empieza `undefined` | Inicializa el estado con `''` |

## 17. Ejercicios

> **Ejercicio:** añade **edición** del título de una tarea con doble clic. Piensa dónde vive el estado de «estoy editando» y cómo se envía el `PATCH`.

> **Ejercicio:** guarda el filtro en la URL (`?filter=done`) con `useSearchParams` de React Router, de modo que se conserve al recargar.

> **Ejercicio:** añade paginación en el cliente (5 tareas por página) y después hazla en el servidor con `?limit` y `?offset`. ¿Qué cambia en `tasks-api.ts`?

> **Ejercicio:** escribe una prueba para `TaskItem` que compruebe que «Eliminar» pide confirmación y que «No» la cancela.

> **Ejercicio:** convierte el estado de las tareas a un contexto (`TasksProvider`) y lee desde `TaskDetailPage` el título de la tarea abierta.

## 18. Documentación oficial

- [React: aprender](https://react.dev/learn)
- [Referencia de hooks](https://react.dev/reference/react/hooks)
- [Vite: guía](https://vite.dev/guide/)
- [React Router](https://reactrouter.com/)
- [Vitest](https://vitest.dev/) y [Testing Library](https://testing-library.com/docs/react-testing-library/intro/)
