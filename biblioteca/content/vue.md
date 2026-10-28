# Vue + Vite

Vue es un framework para interfaces con **componentes de un solo archivo** (`.vue`): plantilla, lógica y estilos juntos. Su sistema de **reactividad** detecta solo qué datos cambian y actualiza lo necesario, sin que tengas que decírselo. Es la alternativa a React más cercana al HTML de siempre.

*Verificado: 2026-10-27. El código de este módulo se ha compilado con `vue-tsc`, probado con Vitest y ejecutado en un navegador real (Chromium) contra la API del módulo de [Node.js](#node). Versiones usadas: Vue 3.5, Vite 8.3.5, vue-router 5.4, Vitest 5.0. Referencia: [vuejs.org](https://vuejs.org/).*

## 1. React frente a Vue

La misma aplicación (la lista de tareas) está en [React](#react) y aquí. Comparar es la mejor forma de entender ambos.

| Concepto | React | Vue |
|---|---|---|
| Archivo de componente | `.tsx` (JS que devuelve JSX) | `.vue` (`<script>`, `<template>`, `<style>`) |
| Estado | `useState` | `ref()` / `reactive()` |
| Valor derivado | `useMemo` o calcular en el render | `computed()` |
| Cambiar el estado | `setX(nuevo)` (inmutable) | `x.value = nuevo` (Vue detecta la mutación) |
| Props | Argumento de la función | `defineProps<…>()` |
| Eventos hacia el padre | Funciones por props | `defineEmits<…>()` y `emit(…)` |
| Bucles | `array.map(…)` con `key` | `v-for` con `:key` |
| Condicionales | `&&` / ternario | `v-if` / `v-else` |
| Formularios | `value` + `onChange` | `v-model` |
| Lógica reutilizable | Hooks (`useX`) | Composables (`useX`) |
| Rutas | react-router | vue-router |
| Reacciona a cambios | Re-ejecuta el componente | Rastrea dependencias y actualiza |

## 2. Crear el proyecto

```bash
cd ~/projects
pnpm create vite@latest tasks-web-vue --template vue-ts
cd tasks-web-vue
pnpm install
pnpm dev
```

Equivale a `pnpm create vue@latest` (el generador oficial de Vue, con asistente que permite añadir router, pruebas y linter).

Estructura generada:

```text
tasks-web-vue/
  index.html            contiene <div id="app"> y carga src/main.ts
  package.json
  vite.config.ts        plugins: [vue()]
  tsconfig.json         referencias a tsconfig.app.json y tsconfig.node.json
  src/
    main.ts             crea la aplicación y la monta en #app
    App.vue             componente raíz
    components/         componentes
    style.css
```

```json
{
  "name": "app-vue-ts",
  "private": true,
  "version": "0.0.0",
  "type": "module",
  "scripts": {
    "dev": "vite",
    "build": "vue-tsc -b && vite build",
    "preview": "vite preview"
  },
  "dependencies": {
    "vue": "^3.5.44"
  },
  "devDependencies": {
    "@types/node": "^24.19.2",
    "@vitejs/plugin-vue": "^6.0.10",
    "@vue/tsconfig": "^0.9.1",
    "typescript": "~6.0.3",
    "vite": "^8.3.5",
    "vue-tsc": "^3.3.13"
  }
}
```

| Pieza | Significado |
|---|---|
| `scripts.build` | `vue-tsc -b && vite build`: `vue-tsc` comprueba tipos **también dentro de los `.vue`** |
| `@vitejs/plugin-vue` | Permite a Vite entender los archivos `.vue` |
| `vue-tsc` | `tsc` para Vue |
| `typescript` | La plantilla de Vue fija TypeScript 6.0 porque `vue-tsc` aún no soporta la línea 7 (comprueba si cambió) |

## 3. Anatomía de un componente `.vue`

```html
<script setup lang="ts">
import { ref } from 'vue';

const count = ref(0); // estado reactivo: se lee y escribe con .value en el script
</script>

<template>
  <button @click="count++">Clics: {{ count }}</button>
</template>

<style scoped>
button { padding: .5rem 1rem; }
</style>
```

| Parte | Significado |
|---|---|
| `<script setup lang="ts">` | Lógica del componente en TypeScript; todo lo que declares está disponible en la plantilla |
| `ref(0)` | Valor reactivo. En el script: `count.value`; en la plantilla: `count` (se desenvuelve solo) |
| `<template>` | HTML con directivas de Vue |
| `<style scoped>` | Los estilos solo afectan a este componente |
| `{{ expr }}` | Interpolación de una expresión |

### Directivas

| Directiva | Atajo | Para qué |
|---|---|---|
| `v-bind:prop="x"` | `:prop="x"` | Enlazar un atributo o prop a una expresión |
| `v-on:evento="f"` | `@evento="f"` | Escuchar eventos (`@click`, `@submit.prevent`) |
| `v-model="x"` | | Enlace bidireccional en formularios |
| `v-if` / `v-else-if` / `v-else` | | Crear o destruir el elemento |
| `v-show` | | Solo oculta (`display: none`) |
| `v-for="item in items" :key="item.id"` | | Repetir |

Modificadores de eventos: `@submit.prevent` (preventDefault), `@click.stop` (stopPropagation), `@keyup.enter`.

## 4. La aplicación de tareas

Reutiliza **sin cambios** los archivos de capa de red y tipos de [React](#react):

```text
src/
  main.ts
  App.vue
  types.ts                    igual que en React
  api/tasks-api.ts            igual que en React
  composables/use-tasks.ts    equivale al hook useTasks
  components/
    TaskForm.vue
    TaskList.vue
    TaskItem.vue
```

Que `tasks-api.ts` sea idéntico en ambos frameworks demuestra la ventaja de separar la comunicación con el servidor de la interfaz.

### Composable de datos

```ts
import { onMounted, onUnmounted, ref } from 'vue';
import { tasksApi } from '../api/tasks-api.ts';
import type { Task } from '../types.ts';

// "Composable": una función que agrupa estado reactivo y la lógica que lo modifica
export function useTasks() {
  const tasks = ref<Task[]>([]);
  const loading = ref(true);
  const error = ref<string | null>(null);
  let controller: AbortController | null = null;

  async function load() {
    controller?.abort(); // cancela una carga anterior si seguía en curso
    controller = new AbortController();
    const { signal } = controller;
    loading.value = true;
    error.value = null;
    try {
      tasks.value = await tasksApi.list(signal);
    } catch (caught) {
      if (signal.aborted) return;
      error.value = caught instanceof Error ? caught.message : 'Error desconocido';
    } finally {
      if (!signal.aborted) loading.value = false;
    }
  }

  onMounted(load);
  onUnmounted(() => controller?.abort());

  async function addTask(title: string) {
    tasks.value.push(await tasksApi.create(title));
  }

  async function toggleTask(task: Task) {
    const updated = await tasksApi.update(task.id, { done: !task.done });
    tasks.value = tasks.value.map((item) => (item.id === task.id ? updated : item));
  }

  async function removeTask(id: string) {
    await tasksApi.remove(id);
    tasks.value = tasks.value.filter((item) => item.id !== id);
  }

  return { tasks, loading, error, reload: load, addTask, toggleTask, removeTask };
}
```

| Pieza | Significado |
|---|---|
| `ref<Task[]>([])` | Lista reactiva: al asignar `tasks.value`, la pantalla se actualiza |
| `onMounted(load)` | Gancho de ciclo de vida: ejecuta la carga al montar el componente |
| `onUnmounted` | Limpieza: cancela la petición |
| `tasks.value = tasks.value.map(…)` | Aquí se reasigna; Vue también detecta `push` y mutaciones, pero reasignar es más claro |

### Formulario

```html
<script setup lang="ts">
import { ref } from 'vue';

const props = defineProps<{ onAdd: (title: string) => Promise<void> }>();

const title = ref('');
const error = ref<string | null>(null);
const saving = ref(false);

async function handleSubmit() {
  const trimmed = title.value.trim();
  if (trimmed.length < 2) {
    error.value = 'Escribe al menos 2 caracteres';
    return;
  }
  saving.value = true;
  error.value = null;
  try {
    await props.onAdd(trimmed);
    title.value = '';
  } catch (caught) {
    error.value = caught instanceof Error ? caught.message : 'No se pudo guardar';
  } finally {
    saving.value = false;
  }
}
</script>

<template>
  <!-- .prevent equivale a event.preventDefault() -->
  <form novalidate @submit.prevent="handleSubmit">
    <label for="task-title">Nueva tarea</label>
    <input
      id="task-title"
      v-model="title"
      :aria-invalid="error !== null"
      :aria-describedby="error ? 'task-title-error' : undefined"
    />
    <button type="submit" :disabled="saving">{{ saving ? 'Guardando…' : 'Añadir' }}</button>
    <p v-if="error" id="task-title-error" role="alert">{{ error }}</p>
  </form>
</template>
```

| Pieza | Significado |
|---|---|
| `defineProps<{ onAdd: … }>()` | Declara las props con tipos |
| `v-model="title"` | Mantiene `title` sincronizado con el campo |
| `@submit.prevent` | Evita la recarga y llama a `handleSubmit` |
| `:aria-invalid`, `:aria-describedby` | El `:` enlaza el atributo a una expresión |
| `v-if="error"` | Solo se dibuja si hay error |

> **Ojo:** dentro de `<script setup>` el estado es `title.value`; dentro de `<template>` es `title`.

### Elemento de lista

```html
<script setup lang="ts">
import { ref } from 'vue';
import type { Task } from '../types.ts';

defineProps<{ task: Task }>();
// Eventos que el componente emite hacia su padre
const emit = defineEmits<{ toggle: [task: Task]; remove: [id: string] }>();

const confirming = ref(false);
</script>

<template>
  <li>
    <label>
      <input type="checkbox" :checked="task.done" @change="emit('toggle', task)" />
      <span :class="{ done: task.done }">{{ task.title }}</span>
    </label>
    <span v-if="confirming">
      ¿Eliminar?
      <button type="button" @click="emit('remove', task.id)">Sí</button>
      <button type="button" @click="confirming = false">No</button>
    </span>
    <button v-else type="button" @click="confirming = true">Eliminar</button>
  </li>
</template>
```

El componente **no modifica datos ajenos**: avisa al padre con eventos (`emit('toggle', task)`) y el padre decide. Es la regla «props hacia abajo, eventos hacia arriba».

### Lista y estado vacío

```html
<script setup lang="ts">
import type { Task } from '../types.ts';
import TaskItem from './TaskItem.vue';

defineProps<{ tasks: Task[] }>();
defineEmits<{ toggle: [task: Task]; remove: [id: string] }>();
</script>

<template>
  <p v-if="tasks.length === 0">No hay tareas. ¡Añade la primera!</p>
  <ul v-else>
    <TaskItem
      v-for="task in tasks"
      :key="task.id"
      :task="task"
      @toggle="$emit('toggle', $event)"
      @remove="$emit('remove', $event)"
    />
  </ul>
</template>
```

`@toggle="$emit('toggle', $event)"` reenvía el evento del hijo al abuelo.

### Pantalla principal

```html
<script setup lang="ts">
import { computed, ref } from 'vue';
import TaskForm from './components/TaskForm.vue';
import TaskList from './components/TaskList.vue';
import { useTasks } from './composables/use-tasks.ts';
import type { Filter } from './types.ts';

const FILTERS: { value: Filter; label: string }[] = [
  { value: 'all', label: 'Todas' },
  { value: 'pending', label: 'Pendientes' },
  { value: 'done', label: 'Hechas' },
];

const { tasks, loading, error, reload, addTask, toggleTask, removeTask } = useTasks();
const filter = ref<Filter>('all');

// computed: valor derivado que Vue recalcula solo cuando cambian sus dependencias
const visibleTasks = computed(() =>
  tasks.value.filter((task) =>
    filter.value === 'all' ? true : filter.value === 'done' ? task.done : !task.done,
  ),
);
const pendingCount = computed(() => tasks.value.filter((task) => !task.done).length);
</script>

<template>
  <main>
    <h1>Tareas</h1>
    <TaskForm :on-add="addTask" />

    <nav aria-label="Filtro">
      <button
        v-for="item in FILTERS"
        :key="item.value"
        type="button"
        :aria-pressed="filter === item.value"
        @click="filter = item.value"
      >
        {{ item.label }}
      </button>
    </nav>

    <p v-if="loading">Cargando…</p>
    <p v-if="error" role="alert">
      {{ error }} <button type="button" @click="reload()">Reintentar</button>
    </p>
    <template v-if="!loading && !error">
      <p>{{ pendingCount }} pendientes</p>
      <TaskList :tasks="visibleTasks" @toggle="toggleTask" @remove="removeTask" />
    </template>
  </main>
</template>
```

```ts
import { createApp } from 'vue';
import App from './App.vue';
import './style.css';

createApp(App).mount('#app');
```

## 5. Ejecutar y comprobar

Igual que en React: API en un terminal (`PORT=3000 node --watch src/server.js`) y `pnpm dev` en otro. Pruebas manuales: validar, añadir, marcar, filtrar, eliminar con confirmación y detener la API para ver el estado de error con «Reintentar».

## 6. Rutas

```bash
pnpm add vue-router
```

```ts
import { createRouter, createWebHistory } from 'vue-router';
import TasksPage from './App.vue';
import AboutPage from './pages/AboutPage.vue';
import TaskDetailPage from './pages/TaskDetailPage.vue';

export const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', component: TasksPage },
    { path: '/tasks/:id', component: TaskDetailPage },
    { path: '/about', component: AboutPage },
    { path: '/:pathMatch(.*)*', component: { template: '<p>Página no encontrada</p>' } },
  ],
});
```

```html
<template>
  <header>
    <nav aria-label="Principal">
      <RouterLink to="/">Tareas</RouterLink> <RouterLink to="/about">Acerca de</RouterLink>
    </nav>
  </header>
  <RouterView />
</template>
```

```html
<script setup lang="ts">
import { useRoute } from 'vue-router';

const route = useRoute(); // la ruta activa; route.params.id es el parámetro de la URL
</script>

<template>
  <section>
    <h2>Tarea {{ route.params.id }}</h2>
    <RouterLink to="/">Volver a la lista</RouterLink>
  </section>
</template>
```

```ts
import { createApp } from 'vue';
import Root from './Root.vue';
import { router } from './router.ts';
import './style.css';

createApp(Root).use(router).mount('#app');
```

| Pieza | Significado |
|---|---|
| `createRouter` + `createWebHistory` | Tabla de rutas con URLs limpias (sin `#`) |
| `<RouterLink to="…">` | Enlace sin recarga |
| `<RouterView />` | Hueco donde se dibuja la página de la ruta activa |
| `/tasks/:id` | Ruta con parámetro; `useRoute().params.id` lo lee |
| `/:pathMatch(.*)*` | Ruta comodín: página 404 |
| `app.use(router)` | Instala el plugin |

## 7. Estado compartido

| Necesidad | Solución en Vue |
|---|---|
| Padre ↔ hijo | Props y eventos; `v-model` en componentes propios |
| Lógica con estado reutilizable | Composables (`useTasks`) |
| Estado global | **Pinia** (el almacén oficial) |
| Datos inyectados a descendientes | `provide` / `inject` |
| Datos del servidor con caché | TanStack Query para Vue |

Un composable que se llama una vez y se comparte (por ejemplo, definiendo el estado **fuera** de la función) actúa como almacén sencillo; Pinia lo estructura cuando crece.

## 8. Reactividad: lo que conviene entender

```ts
import { computed, reactive, ref, watch } from 'vue';

const count = ref(1);
const double = computed(() => count.value * 2); // se recalcula solo si count cambia

const user = reactive({ name: 'Ana', age: 31 });  // objeto reactivo: sin .value
user.age += 1;

watch(count, (now, before) => {                   // efecto secundario ante un cambio
  console.log(`de ${before} a ${now}`);
});
```

| API | Cuándo |
|---|---|
| `ref` | Valores simples y también objetos o arrays (se acceden con `.value`) |
| `reactive` | Objetos con muchas propiedades (no se puede reasignar entero) |
| `computed` | Valores derivados (sin efectos secundarios) |
| `watch` / `watchEffect` | Reaccionar a cambios con un efecto (llamadas, almacenamiento) |

> **Ojo:** desestructurar un objeto `reactive` (`const { age } = user`) **pierde la reactividad**. Usa `toRefs(user)` o accede a `user.age`.

## 9. Estilos y accesibilidad

Todo lo de [CSS moderno](#css) aplica. Añade `<style scoped>` a cada componente cuando los estilos sean propios. Mantén el HTML semántico (`button`, `label`, `nav`, `main`), `role="alert"` en errores y foco visible, igual que en React.

## 10. Pruebas

```bash
pnpm add -D vitest jsdom @vue/test-utils
```

```ts
/// <reference types="vitest/config" />
import vue from '@vitejs/plugin-vue'
import { defineConfig } from 'vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [vue()],
  test: {
    environment: 'jsdom',
  },
})
```

```ts
import { mount } from '@vue/test-utils';
import { describe, expect, it, vi } from 'vitest';
import TaskForm from './TaskForm.vue';

describe('TaskForm', () => {
  it('muestra un error si el título es demasiado corto', async () => {
    const onAdd = vi.fn().mockResolvedValue(undefined);
    const wrapper = mount(TaskForm, { props: { onAdd } });

    await wrapper.get('input').setValue('x');
    await wrapper.get('form').trigger('submit');

    expect(wrapper.get('[role="alert"]').text()).toContain('al menos 2 caracteres');
    expect(onAdd).not.toHaveBeenCalled();
  });

  it('llama a onAdd con el título sin espacios y vacía el campo', async () => {
    const onAdd = vi.fn().mockResolvedValue(undefined);
    const wrapper = mount(TaskForm, { props: { onAdd } });

    await wrapper.get('input').setValue('  Estudiar Vue  ');
    await wrapper.get('form').trigger('submit');
    await vi.waitFor(() => expect(onAdd).toHaveBeenCalledWith('Estudiar Vue'));

    expect((wrapper.get('input').element as HTMLInputElement).value).toBe('');
  });
});
```

Añade `"test": "vitest run"` a los scripts y ejecuta `pnpm test`.

| Pieza | Significado |
|---|---|
| `mount(Componente, { props })` | Monta el componente real |
| `wrapper.get('input').setValue(…)` | Escribe en un campo |
| `wrapper.get('form').trigger('submit')` | Dispara el evento |
| `await …` | Vue actualiza el DOM de forma asíncrona: espera tras cada acción |
| `vi.waitFor(() => expect(…))` | Reintenta hasta que la aserción pasa (para lógica asíncrona) |

## 11. Variables de entorno y producción

Idéntico a [React](#react): variables `VITE_*` incrustadas al compilar, `pnpm build` → `dist/`, `pnpm preview` para probar. La imagen Docker de producción es la misma, ajustando solo `COPY` si cambias de carpeta.

## 12. Errores habituales

| Síntoma | Causa | Solución |
|---|---|---|
| `Property 'value' does not exist on type …` | Usas `.value` donde ya está desenvuelto | En la plantilla no se usa `.value` |
| La interfaz no se actualiza | Reasignaste un `reactive` entero o desestructuraste | Usa `ref` o `toRefs` |
| `[Vue warn]: Failed to resolve component` | Falta importar el componente | `import X from './X.vue'` |
| `Component name should always be multi-word` | Un componente se llama `Item`, `Form`… | Nombres de varias palabras (`TaskItem`) |
| Hijo que modifica una prop | Las props son de solo lectura | Emite un evento al padre |
| `v-for` sin `:key` | Aviso/errores al reordenar | `:key="item.id"` |
| `CORS policy` | La API no permite el origen o el método | Configura CORS en el servidor |
| `vue-tsc` falla y `pnpm dev` no | `dev` no comprueba tipos | Ejecuta `pnpm build` o `vue-tsc -b` |

## 13. Ejercicios

> **Ejercicio:** añade edición del título con doble clic usando `v-if`/`v-else` para alternar entre texto y campo.

> **Ejercicio:** pasa el filtro a la URL (`?filter=done`) leyendo y escribiendo `route.query`.

> **Ejercicio:** mueve el estado de las tareas a un almacén de Pinia y compara el resultado con el composable.

> **Ejercicio:** reimplementa `TaskForm` emitiendo un evento `add` en lugar de recibir una función `onAdd`. ¿Cómo muestras entonces el error del servidor en el formulario?

## 14. Documentación oficial

- [Vue: introducción](https://vuejs.org/guide/introduction.html)
- [Vue Router](https://router.vuejs.org/)
- [Pinia](https://pinia.vuejs.org/)
- [Vue Test Utils](https://test-utils.vuejs.org/)
