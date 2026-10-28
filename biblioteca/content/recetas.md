# Recetario de consulta rápida

Operaciones habituales con una plantilla fija: **problema, cuándo, requisitos, archivos, procedimiento, conceptos, verificación, errores y documentación**. Cada receta es corta y remite al módulo donde se explica a fondo.

*Verificado: 2026-10-27. Las recetas con código (paginación, resumen, validación) se ejecutaron y tienen pruebas que pasan; las de sistema usan comandos estándar de Linux. Cada comando se explica flag a flag en [Comandos explicados](#comandos-explicados).*

**Índice:** [1 Crear un proyecto](#receta-1) · [2 Dependencias](#receta-2) · [3 Entorno local](#receta-3) · [4 Endpoint + frontend](#receta-4) · [5 CRUD completo](#receta-5) · [6 JSON y colecciones](#receta-6) · [7 Validar formularios](#receta-7) · [8 Paginación y filtros](#receta-8) · [9 Base de datos](#receta-9) · [10 Migraciones y pruebas](#receta-10) · [11 Variables de entorno](#receta-11) · [12 Docker y Compose](#receta-12) · [13 Puerto ocupado](#receta-13) · [14 Procesos y servicios](#receta-14) · [15 Leer errores](#receta-15) · [16 Producción](#receta-16) · [17 Tareas programadas](#receta-17) · [18 Copias de seguridad](#receta-18)

## Receta 1: Crear un proyecto desde cero

- **Problema:** empezar un proyecto ordenado, versionado y que otra persona pueda arrancar.
- **Cuándo usarla:** al iniciar cualquier aplicación propia.
- **Requisitos previos:** Git configurado ([Git](#git)), el lenguaje que uses ([Node](#node), [Python](#python), [Java](#java), [Go](#go), [Rust](#rust), [C#](#csharp)).
- **Archivos implicados:** `README.md`, `.gitignore`, `.env.example`, el manifiesto del lenguaje.

**Procedimiento**

```bash
mkdir -p ~/projects/tasks-app/{docs,app/backend,app/frontend} && cd ~/projects/tasks-app
git init -b main
printf '# tasks-app\n\nDescribe qué hace y cómo arrancarlo.\n' > README.md
printf 'node_modules/\ntarget/\n__pycache__/\n.venv/\ndist/\nbin/\n.env\n.env.*\n!.env.example\n' > .gitignore
git add . && git commit -m "Estructura inicial"
```

| Tecnología | Crear el proyecto dentro de `app/backend` o `app/frontend` |
|---|---|
| Node | `pnpm init` |
| React / Vue | `pnpm create vite@latest frontend --template react-ts` (o `vue-ts`) |
| Python | `uv init` |
| Java | `pom.xml` a mano o [Spring Initializr](https://start.spring.io/) |
| C# | `dotnet new web` / `console` |
| Go | `go mod init ejemplo.com/tasks` |
| Rust | `cargo new tasks-rs` |
| Flutter | `fvm flutter create` |

- **Conceptos:** el manifiesto del proyecto, el `.gitignore`, la separación `backend`/`frontend` ([Normas](#normas)).
- **Verificar:** `git status` limpio; ejecuta el Hola mundo de la tecnología elegida.
- **Errores frecuentes:** olvidar el `.gitignore` y subir `node_modules/` o `.env`; crear `git init` dentro de otro repositorio.
- **Documentación:** [Git](#git), cada módulo de lenguaje.

## Receta 2: Añadir y actualizar dependencias

- **Problema:** usar una librería de terceros y mantenerla al día.
- **Cuándo usarla:** siempre que necesites algo que no quieras escribir (con criterio: cada dependencia es código ajeno que mantienes).
- **Requisitos previos:** un proyecto con su manifiesto.
- **Archivos implicados:** manifiesto (`package.json`, `pyproject.toml`, `pom.xml`, `go.mod`, `Cargo.toml`, `*.csproj`, `pubspec.yaml`) y archivo de bloqueo.

| Tecnología | Añadir | Actualizar | Bloqueo |
|---|---|---|---|
| Node (pnpm) | `pnpm add paquete` / `pnpm add -D paquete` | `pnpm update` · `pnpm outdated` | `pnpm-lock.yaml` |
| Python (uv) | `uv add paquete` / `uv add --dev paquete` | `uv lock --upgrade` · `uv sync` | `uv.lock` |
| Java (Maven) | Editar `pom.xml` (`<dependency>`) | `mvn versions:display-dependency-updates` | (ninguno) |
| Go | `go get paquete@versión` | `go get -u ./...` · `go mod tidy` | `go.sum` |
| Rust | `cargo add crate` | `cargo update` | `Cargo.lock` |
| C# | `dotnet add package X` | `dotnet list package --outdated` | (opcional) |
| Dart/Flutter | `flutter pub add paquete` | `flutter pub upgrade` · `flutter pub outdated` | `pubspec.lock` |

**Procedimiento:** añade → importa y usa → ejecuta las pruebas → `git diff` del manifiesto y del bloqueo → commit.

- **Conceptos:** versión semántica (`MAYOR.MENOR.PARCHE`), rangos (`^`, `~`), dependencias directas y transitivas, **manifiesto frente a bloqueo** ([Node](#node)).
- **Verificar:** el proyecto compila y las pruebas pasan; `pnpm audit` / `cargo audit` / `pip-audit` sin avisos graves ([Laboratorio](#lab-etico)).
- **Errores frecuentes:** mezclar gestores (npm y pnpm) en un proyecto; no versionar el bloqueo; saltar de versión mayor sin leer las notas de cambios; instalar paquetes en el Python del sistema.
- **Documentación:** la del gestor correspondiente.

## Receta 3: Configurar un entorno local

- **Problema:** que el proyecto arranque en tu máquina con el mismo resultado que en otra.
- **Cuándo usarla:** al clonar un proyecto o preparar una máquina nueva.
- **Requisitos previos:** [Entorno](#entorno-versiones): gestores de versiones instalados.
- **Archivos implicados:** `.nvmrc`, `.python-version`, `.fvmrc`, `mise.toml`, `.env.example`, `README.md`.

```bash
git clone git@github.com:USUARIO/tasks-app.git && cd tasks-app
cp app/backend/.env.example app/backend/.env       # rellena los valores propios
nvm use                                            # lee .nvmrc
pnpm install --frozen-lockfile
docker compose up -d mongo                         # dependencias en contenedores
pnpm test && pnpm dev
```

| Pieza | Significado |
|---|---|
| `nvm use` | Cambia a la versión de Node del proyecto |
| `pnpm install --frozen-lockfile` | Instala exactamente lo del bloqueo; falla si discrepa |
| `docker compose up -d mongo` | Arranca solo el servicio `mongo` en segundo plano |

- **Conceptos:** versiones fijadas por proyecto, variables de entorno ([PATH](#entorno-path)), servicios auxiliares en contenedores.
- **Verificar:** las pruebas pasan y `curl localhost:3000/api/tasks` responde.
- **Errores frecuentes:** versión de Node o Python equivocada; `.env` sin copiar; puertos ocupados (receta 13); dependencias del sistema que faltan.
- **Documentación:** [Gestores de versiones](#entorno-versiones).

## Receta 4: Crear un endpoint y consumirlo desde el frontend

- **Problema:** exponer un dato nuevo en la API y mostrarlo en la interfaz.
- **Cuándo usarla:** cualquier función que necesite datos del servidor.
- **Requisitos previos:** API de [Node.js](#node) y frontend de [React](#react) (o [Vue](#vue)).
- **Archivos implicados:** servicio, ruta, prueba de la API; cliente `fetch` y componente del frontend.

**Ejemplo:** un resumen `GET /api/tasks/stats` que devuelve `{ total, done, pending }`.

1. **Servicio** (la lógica vive aquí, no en la ruta):

```js
// src/services/task-service.js (dentro del objeto devuelto)
async stats() {
  const all = await repository.findAll();
  const done = all.filter((task) => task.done).length;
  return { total: all.length, done, pending: all.length - done };
},
```

2. **Ruta** (declárala **antes** de `/tasks/:id`, o `stats` se interpretaría como un id):

```js
app.get('/tasks/stats', async () => taskService.stats());
```

3. **Prueba:**

```js
test('resumen de tareas', async () => {
  const app = buildApp();
  await createTasks(app, ['Tarea uno', 'Tarea dos', 'Tarea tres']);
  await app.inject({ method: 'PATCH', url: '/api/tasks/1', payload: { done: true } });
  const stats = await app.inject({ method: 'GET', url: '/api/tasks/stats' });
  assert.deepEqual(stats.json(), { total: 3, done: 1, pending: 2 });
  await app.close();
});
```

4. **Cliente y componente en React:**

```ts
export interface TaskStats {
  total: number;
  done: number;
  pending: number;
}

const API_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:3000/api';

export async function fetchStats(signal?: AbortSignal): Promise<TaskStats> {
  const response = await fetch(`${API_URL}/tasks/stats`, { signal });
  if (!response.ok) throw new Error(`Error HTTP ${response.status}`);
  return (await response.json()) as TaskStats;
}
```

```tsx
import { useEffect, useState } from 'react';
import { fetchStats, type TaskStats } from '../api/stats.ts';

export function StatsBadge() {
  const [stats, setStats] = useState<TaskStats | null>(null);
  const [error, setError] = useState(false);

  useEffect(() => {
    const controller = new AbortController();
    fetchStats(controller.signal)
      .then(setStats)
      .catch(() => {
        if (!controller.signal.aborted) setError(true);
      });
    return () => controller.abort();
  }, []);

  if (error) return <p role="alert">No se pudo cargar el resumen</p>;
  if (!stats) return <p>Cargando resumen…</p>;
  return (
    <p>
      {stats.done} hechas de {stats.total} ({stats.pending} pendientes)
    </p>
  );
}
```

- **Conceptos:** capas ruta → servicio → repositorio; contrato JSON; estados cargando/error/datos; cancelación con `AbortController`.
- **Verificar:** `node --test`; `curl -s localhost:3000/api/tasks/stats`; el componente en el navegador; apaga la API y comprueba el estado de error.
- **Errores frecuentes:** orden de rutas (`/stats` después de `/:id`); CORS; olvidar el `await`; cambiar el formato del JSON sin avisar al frontend.
- **Documentación:** [Fastify routes](https://fastify.dev/docs/latest/Reference/Routes/), [MDN fetch](https://developer.mozilla.org/es/docs/Web/API/Fetch_API).

## Receta 5: Implementar un CRUD completo

- **Problema:** crear, leer, actualizar y borrar un recurso de extremo a extremo.
- **Cuándo usarla:** al incorporar cualquier entidad nueva.
- **Requisitos previos:** el proyecto base ([Proyecto de tareas](#todo)).
- **Archivos implicados:** repositorio, servicio, rutas, pruebas, cliente de API, hook/composable, componentes.

| Operación | HTTP | Éxito | Fallos típicos |
|---|---|---|---|
| Listar | `GET /api/tasks` | `200` | — |
| Consultar | `GET /api/tasks/:id` | `200` | `404` |
| Crear | `POST /api/tasks` | `201` | `400` (validación) |
| Modificar | `PATCH /api/tasks/:id` | `200` | `400`, `404` |
| Eliminar | `DELETE /api/tasks/:id` | `204` | `404` |

**Orden recomendado:** 1) prueba de la API que falla → 2) repositorio → 3) servicio con validación → 4) ruta y traducción de errores → 5) prueba en verde → 6) cliente de API → 7) estado (hook) → 8) componentes con cargando/error/vacío/confirmación.

- **Conceptos:** REST, códigos de estado, capas, validación en servidor y cliente, estado de interfaz.
- **Verificar:** `curl` de cada operación; la prueba E2E de [Proyecto de tareas](#todo).
- **Errores frecuentes:** lógica de negocio en la ruta; no manejar `404`; no cancelar peticiones al desmontar; `DELETE` con `Content-Type: application/json` sin cuerpo (rechazado por Fastify).
- **Documentación:** [Node.js](#node), [React](#react).

## Receta 6: Mapear JSON a objetos y transformar colecciones

- **Problema:** convertir lo que llega de una API en estructuras útiles y calcular resultados.
- **Cuándo usarla:** siempre que consumas JSON.
- **Requisitos previos:** [JavaScript](#js) y [TypeScript](#ts).
- **Archivos implicados:** `types.ts`, `api/*.ts`.

```ts
interface ApiTask { id: string; title: string; done: boolean; createdAt: string }
interface Task { id: string; title: string; done: boolean; createdAt: Date }

// JSON (texto) -> objetos tipados; las fechas llegan como texto y hay que convertirlas
const toTask = (raw: ApiTask): Task => ({ ...raw, createdAt: new Date(raw.createdAt) });

const text = '[{"id":"1","title":"A","done":true,"createdAt":"2026-10-27T10:00:00Z"}]';
const tasks: Task[] = (JSON.parse(text) as ApiTask[]).map(toTask);

// Transformar: agrupar y contar
const byStatus = Object.groupBy(tasks, (task) => (task.done ? 'done' : 'pending'));
const counts = Object.fromEntries(Object.entries(byStatus).map(([key, list]) => [key, list?.length ?? 0]));
```

- **Conceptos:** `JSON.parse` / `JSON.stringify`, tipos que desaparecen en ejecución, validación de datos externos, `map`/`filter`/`reduce`/`groupBy` ([Ejercicios](#ejercicios-basicos)).
- **Verificar:** una prueba con un JSON de ejemplo; `tsc --noEmit`.
- **Errores frecuentes:** asumir que `JSON.parse` devuelve el tipo declarado; fechas como texto; `undefined` que no existe en JSON.
- **Documentación:** [MDN: JSON](https://developer.mozilla.org/es/docs/Web/JavaScript/Reference/Global_Objects/JSON).

## Receta 7: Validar formularios

- **Problema:** impedir datos incorrectos con buena experiencia de usuario y sin depender del cliente.
- **Cuándo usarla:** cualquier formulario.
- **Requisitos previos:** [HTML5](#html5), [React](#react).
- **Archivos implicados:** componente del formulario; servicio de la API.

**Tres capas** (todas necesarias):

| Capa | Dónde | Para qué |
|---|---|---|
| 1. HTML nativo | `required`, `minlength`, `type="email"`, `pattern` | Validación inmediata sin JS |
| 2. JavaScript | `event.preventDefault()` + comprobaciones + mensajes con `role="alert"` | Mensajes propios, reglas complejas |
| 3. **Servidor** | `ValidationError` → `400` con `{ error: "…" }` | **La única que protege de verdad** |

El componente `TaskForm` de [React](#react) y `TaskForm.vue` de [Vue](#vue) implementan las capas 2 y 3, con prueba de que un título corto muestra el error y no llama al servidor.

- **Conceptos:** *constraint validation API*, estado de error, accesibilidad (`aria-invalid`, `aria-describedby`), mensajes del servidor.
- **Verificar:** probar vacío, muy corto, solo espacios y un envío válido; repetir con la API detenida.
- **Errores frecuentes:** validar solo en el cliente; mensajes genéricos («error»); no recortar espacios; perder lo escrito al fallar.
- **Documentación:** [MDN: validación de formularios](https://developer.mozilla.org/es/docs/Learn_web_development/Extensions/Forms/Form_validation).

## Receta 8: Implementar paginación y filtros

- **Problema:** listar muchos elementos sin cargarlos todos.
- **Cuándo usarla:** listas que pueden crecer.
- **Requisitos previos:** API de [Node.js](#node).
- **Archivos implicados:** ruta, servicio, prueba, cliente.

Ruta con validación de los parámetros de consulta y el total en una cabecera:

```js
// Capa HTTP: traduce peticiones a llamadas al servicio y errores a códigos de estado.
export default async function taskRoutes(app, { taskService }) {
  const idParams = {
    type: 'object',
    properties: { id: { type: 'string', minLength: 1 } },
    required: ['id'],
  };

  // Los parámetros de consulta se validan y convierten de texto a su tipo gracias al esquema
  const listQuery = {
    type: 'object',
    properties: {
      done: { type: 'boolean' },
      limit: { type: 'integer', minimum: 1, maximum: 100, default: 20 },
      offset: { type: 'integer', minimum: 0, default: 0 },
    },
  };

  app.get('/tasks', { schema: { querystring: listQuery } }, async (request, reply) => {
    const { items, total } = await taskService.list(request.query);
    // El total viaja en una cabecera para no cambiar el formato del cuerpo (un array)
    return reply.header('X-Total-Count', total).send(items);
  });

  app.get('/tasks/stats', async () => taskService.stats());

  app.get('/tasks/:id', { schema: { params: idParams } }, async (request) =>
    taskService.get(request.params.id),
  );

  app.post('/tasks', async (request, reply) => {
    const task = await taskService.create(request.body ?? {});
    return reply.code(201).send(task);
  });

  app.patch('/tasks/:id', { schema: { params: idParams } }, async (request) =>
    taskService.update(request.params.id, request.body ?? {}),
  );

  app.delete('/tasks/:id', { schema: { params: idParams } }, async (request, reply) => {
    await taskService.remove(request.params.id);
    return reply.code(204).send();
  });
}
```

Servicio:

```js
// Reglas de negocio. No sabe nada de HTTP ni de la base de datos.
export class ValidationError extends Error {}
export class NotFoundError extends Error {}

export function createTaskService(repository) {
  function validateTitle(title) {
    if (typeof title !== 'string' || title.trim().length < 2) {
      throw new ValidationError('title debe tener al menos 2 caracteres');
    }
    return title.trim();
  }

  return {
    // Filtro opcional por estado + paginación. Devuelve la página y el total SIN paginar
    async list({ done, limit = 20, offset = 0 } = {}) {
      const all = await repository.findAll();
      const filtered = done === undefined ? all : all.filter((task) => task.done === done);
      return { items: filtered.slice(offset, offset + limit), total: filtered.length };
    },

    // Datos agregados para un resumen: la lógica de negocio vive aquí, no en la ruta
    async stats() {
      const all = await repository.findAll();
      const done = all.filter((task) => task.done).length;
      return { total: all.length, done, pending: all.length - done };
    },

    async get(id) {
      const task = await repository.findById(id);
      if (!task) throw new NotFoundError(`No existe la tarea ${id}`);
      return task;
    },

    create({ title }) {
      return repository.create({ title: validateTitle(title), done: false });
    },

    async update(id, changes) {
      const allowed = {};
      if ('title' in changes) allowed.title = validateTitle(changes.title);
      if ('done' in changes) allowed.done = Boolean(changes.done);
      const updated = await repository.update(id, allowed);
      if (!updated) throw new NotFoundError(`No existe la tarea ${id}`);
      return updated;
    },

    async remove(id) {
      if (!(await repository.remove(id))) throw new NotFoundError(`No existe la tarea ${id}`);
    },
  };
}
```

Prueba:

```js
import { test } from 'node:test';
import assert from 'node:assert/strict';
import { buildApp } from './app.js';

async function createTasks(app, titles) {
  for (const title of titles) {
    await app.inject({ method: 'POST', url: '/api/tasks', payload: { title } });
  }
}

test('filtra por estado y pagina', async () => {
  const app = buildApp();
  await createTasks(app, ['Tarea uno', 'Tarea dos', 'Tarea tres', 'Tarea cuatro', 'Tarea cinco']);
  await app.inject({ method: 'PATCH', url: '/api/tasks/2', payload: { done: true } });
  await app.inject({ method: 'PATCH', url: '/api/tasks/4', payload: { done: true } });

  const page1 = await app.inject({ method: 'GET', url: '/api/tasks?limit=2&offset=0' });
  assert.equal(page1.json().length, 2);
  assert.equal(page1.headers['x-total-count'], '5');

  const page3 = await app.inject({ method: 'GET', url: '/api/tasks?limit=2&offset=4' });
  assert.equal(page3.json().length, 1);

  const done = await app.inject({ method: 'GET', url: '/api/tasks?done=true' });
  assert.deepEqual(done.json().map((task) => task.id), ['2', '4']);
  assert.equal(done.headers['x-total-count'], '2');

  const pending = await app.inject({ method: 'GET', url: '/api/tasks?done=false&limit=1' });
  assert.deepEqual(pending.json().map((task) => task.id), ['1']);
  assert.equal(pending.headers['x-total-count'], '3');

  const invalid = await app.inject({ method: 'GET', url: '/api/tasks?limit=1000' });
  assert.equal(invalid.statusCode, 400);

  await app.close();
});

test('resumen de tareas', async () => {
  const app = buildApp();
  await createTasks(app, ['Tarea uno', 'Tarea dos', 'Tarea tres']);
  await app.inject({ method: 'PATCH', url: '/api/tasks/1', payload: { done: true } });

  const stats = await app.inject({ method: 'GET', url: '/api/tasks/stats' });
  assert.deepEqual(stats.json(), { total: 3, done: 1, pending: 2 });
  await app.close();
});
```

```bash
curl -si 'http://localhost:3000/api/tasks?done=false&limit=2&offset=0' | grep -iE 'HTTP/|x-total-count'
```

| Pieza | Significado |
|---|---|
| `limit` / `offset` | Tamaño de página y posición de inicio |
| `schema.querystring` | Fastify valida, convierte (`"2"` → `2`) y aplica valores por defecto; un valor inválido da `400` |
| `X-Total-Count` | Total **sin paginar**, para que el cliente dibuje los botones de página |
| `exposedHeaders` en CORS | Sin esto el navegador oculta la cabecera al JavaScript |
| `slice(offset, offset + limit)` | La página; en SQL sería `LIMIT n OFFSET m`, en Mongo `.skip(m).limit(n)` |

- **Conceptos:** parámetros de consulta, límites máximos (evitan peticiones costosas), cabeceras personalizadas, `OFFSET` frente a paginación por cursor (más eficiente en tablas enormes).
- **Verificar:** `node --test`, los casos límite (página vacía, `limit` excesivo, `done` inválido).
- **Errores frecuentes:** sin tope de `limit`; olvidar `ORDER BY` (páginas inestables); filtrar en el cliente tras cargar todo.
- **Documentación:** [Fastify validation](https://fastify.dev/docs/latest/Reference/Validation-and-Serialization/).

## Receta 9: Configurar una base de datos

- **Problema:** tener una base de datos local reproducible.
- **Cuándo usarla:** al pasar de datos en memoria a persistencia.
- **Requisitos previos:** [Docker](#docker).
- **Archivos implicados:** `compose.yaml`, `.env`, repositorio.

```yaml
services:
  postgres:
    image: postgres:18
    environment:
      POSTGRES_PASSWORD: ${DB_PASSWORD:?define DB_PASSWORD en .env}
      POSTGRES_DB: app
    ports:
      - "127.0.0.1:5432:5432"
    volumes:
      - pg-data:/var/lib/postgresql
volumes:
  pg-data:
```

```bash
echo 'DB_PASSWORD=CHANGE_ME' > .env
docker compose up -d postgres
docker compose exec postgres psql -U postgres -d app -c '\dt'
```

| Pieza | Significado |
|---|---|
| `${DB_PASSWORD:?mensaje}` | Compose lee `.env` y **falla con ese mensaje** si falta |
| `127.0.0.1:` | Solo accesible desde la propia máquina |
| `volumes` | Los datos sobreviven a `docker compose down` (no a `down -v`) |

- **Conceptos:** servicios con estado y volúmenes, cadenas de conexión (`postgres://usuario:clave@host:5432/base`), secretos en `.env`.
- **Verificar:** `docker compose ps`; conectar con `psql`; que el repositorio de la aplicación guarda y lee.
- **Errores frecuentes:** `password authentication failed` (el volumen conserva la contraseña vieja); puerto ocupado; usar `localhost` desde otro contenedor (usa el nombre del servicio).
- **Documentación:** [SQL](#sql), [MongoDB](#mongo).

## Receta 10: Ejecutar migraciones y pruebas

- **Problema:** cambiar el esquema de datos de forma controlada y comprobar que nada se rompe.
- **Cuándo usarla:** antes de cada despliegue y en cada cambio de esquema.
- **Requisitos previos:** receta 9.
- **Archivos implicados:** `migrations/V1__…sql`, pruebas, CI.

```bash
psql -h localhost -U postgres -d app -v ON_ERROR_STOP=1 -f migrations/V2__add_priority.sql
pnpm test
```

| Herramienta | Comando |
|---|---|
| Flyway (Java) | `mvn flyway:migrate` o automático en Spring Boot |
| node-pg-migrate | `pnpm exec node-pg-migrate up` |
| Alembic (Python) | `uv run alembic upgrade head` |
| EF Core (C#) | `dotnet ef database update` |
| golang-migrate | `migrate -path migrations -database "$DATABASE_URL" up` |

**Pruebas por niveles:** unitarias (lógica pura) → de integración (con la base real en contenedor) → de recorrido completo ([Proyecto de tareas](#todo)). Se ejecutan en este orden de menor a mayor coste.

- **Conceptos:** migración versionada e irrepetible, `ON_ERROR_STOP`, base de pruebas aislada.
- **Verificar:** aplicar en una copia de los datos de producción; comprobar `\d tabla`.
- **Errores frecuentes:** editar una migración ya aplicada; migrar producción sin copia de seguridad; pruebas que dependen del orden o de datos previos.
- **Documentación:** [SQL](#sql).

## Receta 11: Configurar variables de entorno

- **Problema:** separar configuración y secretos del código, distinta por entorno.
- **Cuándo usarla:** siempre que haya puertos, URLs, claves.
- **Requisitos previos:** [PATH, variables y perfiles](#entorno-path).
- **Archivos implicados:** `.env`, `.env.example`, `.gitignore`, `compose.yaml`.

```bash
# .env.example  (se versiona)
PORT=3000
MONGODB_URI=mongodb://localhost:27017/tasks
JWT_SECRET=CHANGE_ME

cp .env.example .env     # .env NO se versiona
node --env-file=.env src/server.js
```

| Lenguaje | Leer | Cargar `.env` |
|---|---|---|
| Node | `process.env.PORT` | `node --env-file=.env` |
| Vite | `import.meta.env.VITE_API_URL` | Automático (`.env`, `.env.local`…) |
| Python | `os.environ["PORT"]` | `pydantic-settings` / `python-dotenv` |
| Java (Spring) | `${PORT:3000}` en `application.properties` | Variables de entorno del sistema |
| C# | `builder.Configuration["PORT"]` | `appsettings.json` + variables |
| Go | `os.Getenv("PORT")` | Exportar antes de ejecutar |
| Rust | `std::env::var("PORT")` | `dotenvy` |
| Flutter | `String.fromEnvironment("API_URL")` | `--dart-define=API_URL=…` |

- **Conceptos:** variable de shell frente a variable exportada; las variables de frontend son **públicas**; diferencia desarrollo/producción.
- **Verificar:** `printenv PORT`; arrancar con el valor cambiado; arrancar **sin** una variable obligatoria y ver que falla con un mensaje claro.
- **Errores frecuentes:** subir `.env`; esperar que cambie sin reiniciar; secretos en variables `VITE_*`; olvidarlas en producción.
- **Documentación:** [The Twelve-Factor App: configuración](https://12factor.net/es/config).

## Receta 12: Crear y ejecutar un Dockerfile y Docker Compose

- **Problema:** empaquetar la aplicación y sus servicios para ejecutarla igual en cualquier sitio.
- **Cuándo usarla:** al probar el conjunto o desplegarlo.
- **Requisitos previos:** [Docker](#docker).
- **Archivos implicados:** `Dockerfile`, `.dockerignore`, `compose.yaml`.

```bash
docker build -t tasks-api:dev app/backend
docker run --rm -p 127.0.0.1:3000:3000 --env-file app/backend/.env tasks-api:dev
docker compose up -d --build
docker compose logs -f api
docker compose down
```

- **Conceptos:** capas y caché (dependencias antes que código), `127.0.0.1:` en los puertos, volúmenes, nombres de servicio como hosts.
- **Verificar:** `docker compose ps` (sanos), `curl localhost:3000/api/tasks`.
- **Errores frecuentes:** imagen antigua (`--build`); `localhost` dentro del contenedor; perder datos sin volumen; secretos dentro de la imagen.
- **Documentación:** [Docker](#docker).

## Receta 13: Diagnosticar un puerto ocupado

- **Problema:** `EADDRINUSE` / `address already in use` al arrancar un servidor.
- **Cuándo usarla:** cuando el servidor no puede abrir su puerto.
- **Requisitos previos:** ninguno.
- **Archivos implicados:** ninguno.

```bash
sudo ss -ltnp 'sport = :3000'
sudo lsof -i :3000 -P -n
sudo fuser -v 3000/tcp
kill <PID>
```

| Comando | Significado |
|---|---|
| `ss -ltnp 'sport = :3000'` | Quién **escucha** en el puerto 3000 (`-l` listening, `-t` TCP, `-n` numérico, `-p` proceso; el filtro `sport` es el puerto de origen) |
| `lsof -i :3000 -P -n` | Archivos de red abiertos en ese puerto (`-P` puertos numéricos, `-n` sin DNS) |
| `fuser -v 3000/tcp` | PID que usa el puerto, con detalle (`-v`) |
| `kill PID` | Pide terminar con calma (SIGTERM); `kill -9` solo si no responde |

**Procedimiento:** identifica el proceso → ¿es tuyo y sobra (una ejecución anterior)? → `kill` → si es un servicio del sistema o de Docker (`docker ps`), decide si cambias **tu** puerto (`PORT=3001`) en lugar de detener el otro.

- **Conceptos:** un puerto solo lo ocupa un proceso; `0.0.0.0` frente a `127.0.0.1`; procesos huérfanos.
- **Verificar:** `ss -ltn | grep 3000` vacío tras liberarlo; el servidor arranca.
- **Errores frecuentes:** matar un proceso del sistema; no usar `sudo` y no ver al dueño; contenedor de Docker que mapea el puerto (hay que pararlo con `docker stop`).
- **Documentación:** [Comandos explicados](#comandos-explicados).

## Receta 14: Inspeccionar procesos y servicios

- **Problema:** saber qué corre, cuánto consume y cómo se llama.
- **Cuándo usarla:** al diagnosticar lentitud, consumo o servicios caídos.
- **Requisitos previos:** [Terminal](#entorno-terminal).
- **Archivos implicados:** ninguno.

```bash
ps aux --sort=-%mem | head -n 6
top -o %CPU
systemctl status docker --no-pager
journalctl -u docker -n 50 --no-pager
docker ps -a
docker stats --no-stream
df -h && free -h
```

| Comando | Significado |
|---|---|
| `ps aux --sort=-%mem` | Procesos ordenados por memoria (`-` = descendente); `head` limita |
| `top -o %CPU` | Monitor en vivo ordenado por CPU (`q` sale) |
| `systemctl status X` | Estado del servicio |
| `journalctl -u X -n 50` | Últimas 50 líneas del registro del servicio |
| `docker stats --no-stream` | Consumo de cada contenedor, una sola lectura |
| `df -h`, `free -h` | Disco y memoria |

- **Conceptos:** proceso (PID), servicio (systemd), registro (journal), recursos.
- **Verificar:** identificas la causa (¿falta de disco? ¿un proceso al 100 %? ¿un servicio parado?).
- **Errores frecuentes:** reiniciar sin mirar el registro; confundir memoria «usada» con «disponible» (usa la columna `available`).
- **Documentación:** [Comandos explicados](#comandos-explicados).

## Receta 15: Leer errores de compilación y ejecución

- **Problema:** entender un mensaje de error y encontrar la causa.
- **Cuándo usarla:** siempre que algo falla.
- **Requisitos previos:** ninguno.
- **Archivos implicados:** el que indique el error.

**Método:**

1. **Lee el mensaje completo**, empezando por la **primera** línea del error propio (no las de librerías) o, en TypeScript/Rust, por la **última**.
2. Localiza **archivo y línea** (`src/app.js:42:7`).
3. Identifica el **tipo** de error: sintaxis, tipos, referencia (`undefined`), red, permisos, configuración.
4. Reproduce con el caso más pequeño.
5. Busca el mensaje **literal** (entre comillas) junto al nombre de la tecnología.
6. Cambia **una** cosa cada vez.

| Pista | Significado |
|---|---|
| `Cannot read properties of undefined (reading 'x')` | Algo es `undefined`; mira de dónde viene |
| `ENOENT` | Archivo o carpeta inexistente |
| `EADDRINUSE` | Puerto ocupado (receta 13) |
| `ECONNREFUSED` | Nadie escucha en ese destino |
| `EACCES` | Permisos |
| `error[E0382]` (Rust) | Valor movido; el compilador explica cómo arreglarlo |
| `cannot find symbol` (Java) / `CS0103` (C#) | Nombre inexistente: ortografía, `import` o dependencia |
| `Traceback` (Python) | Lee de **abajo arriba**: la última línea es el error |

- **Conceptos:** traza de pila (*stack trace*), error de compilación frente a error de ejecución, causa raíz.
- **Verificar:** el error desaparece y añades una prueba que lo habría detectado.
- **Errores frecuentes:** arreglar el síntoma, no la causa; cambiar varias cosas a la vez; ignorar los avisos.
- **Documentación:** la sección «Errores habituales» de cada módulo.

## Receta 16: Preparar una aplicación para producción

- **Problema:** pasar de «funciona en mi máquina» a «funciona de forma estable y segura».
- **Cuándo usarla:** antes de publicar.
- **Requisitos previos:** recetas 11 y 12; [Endurecimiento](#hardening).
- **Archivos implicados:** `Dockerfile`, `compose.yaml`, configuración, proxy inverso.

**Lista de comprobación:**

- [ ] Pruebas, `lint` y `build` pasan en una máquina limpia.
- [ ] Configuración **solo** por variables de entorno; sin secretos en el repositorio o la imagen.
- [ ] `NODE_ENV=production` o equivalente; modo depuración desactivado; errores internos no se muestran al cliente.
- [ ] Dependencias fijadas (bloqueo) y sin avisos graves de seguridad.
- [ ] **HTTPS** (proxy inverso Caddy/nginx con certificado Let's Encrypt).
- [ ] CORS limitado a tu origen real, no `*`.
- [ ] Base de datos con usuario de mínimo privilegio, sin puerto público, **con copias de seguridad probadas** (receta 18).
- [ ] Registros a un destino que no se pierda; reinicio automático (`restart: unless-stopped`).
- [ ] Comprobación de salud (`/health`) y alertas.
- [ ] Cortafuegos activo, SSH solo con claves.
- [ ] Plan de **vuelta atrás** (versión anterior de la imagen, instantánea).

Proxy inverso mínimo con Caddy (HTTPS automático; sustituye el dominio):

```text
tasks.example.com {
    reverse_proxy 127.0.0.1:3000
}
```

- **Conceptos:** entorno de producción frente a desarrollo ([Node](#node)), inmutabilidad de imágenes, observabilidad.
- **Verificar:** despliegue de prueba en una VM limpia; reiniciar el servidor y comprobar que todo vuelve solo.
- **Errores frecuentes:** publicar con datos de ejemplo o credenciales por defecto; olvidar HTTPS; actualizar sin copia de seguridad.
- **Documentación:** [Docker](#docker), [Endurecimiento](#hardening).

## Receta 17: Programar una tarea con cron

- **Problema:** ejecutar algo automáticamente a ciertas horas (copias, informes, el vigilante de puertos).
- **Cuándo usarla:** tareas periódicas en un servidor.
- **Requisitos previos:** un script ejecutable ([Scripts de Bash](#terminal-scripts)).
- **Archivos implicados:** tu `crontab`, el script, un archivo de registro.

```bash
crontab -e
```

```text
# m  h  dom mes dow  comando
*/5  *  *   *   *    /home/dev26/bin/check-ports.sh >> /home/dev26/logs/ports.log 2>&1
30   2  *   *   *    /home/dev26/bin/backup.sh >> /home/dev26/logs/backup.log 2>&1
0    9  *   *   1    /home/dev26/bin/weekly-report.sh
```

| Campo | Rango | Ejemplos |
|---|---|---|
| minuto | 0–59 | `*/5` = cada 5 minutos |
| hora | 0–23 | `2` = las 2:00 |
| día del mes | 1–31 | `*` = todos |
| mes | 1–12 | |
| día de la semana | 0–7 (0 y 7 = domingo) | `1` = lunes |

| Comando | Significado |
|---|---|
| `crontab -e` | Edita tus tareas |
| `crontab -l` | Lista tus tareas |
| `>> archivo 2>&1` | Añade la salida y los errores a un registro |

- **Conceptos:** cron ejecuta con un entorno **mínimo** (sin tu `PATH` completo ni NVM): usa **rutas absolutas** y `#!/usr/bin/env bash`. Alternativa moderna: *timers* de systemd.
- **Verificar:** ejecuta primero el script a mano; mira `grep CRON /var/log/syslog` o `journalctl -u cron`.
- **Errores frecuentes:** rutas relativas; script sin permiso de ejecución (`chmod +x`); herramientas instaladas con NVM/SDKMAN que cron no encuentra; sin registro de salida.
- **Documentación:** [Scripts de Bash](#terminal-scripts).

## Receta 18: Copias de seguridad

- **Problema:** poder recuperar datos y configuración tras un error o una intrusión.
- **Cuándo usarla:** desde el primer día, no después del problema.
- **Requisitos previos:** receta 17 para automatizar.
- **Archivos implicados:** script de copia, destino externo.

```bash
#!/usr/bin/env bash
set -euo pipefail
stamp="$(date +%F)"
dest="$HOME/backups/$stamp"
mkdir -p "$dest"

docker exec pg-lab pg_dump -U postgres -d lib_demo -Fc > "$dest/postgres.dump"
docker exec tasks-mongo mongodump --archive --gzip > "$dest/mongo.archive.gz"
tar -czf "$dest/projects.tar.gz" -C "$HOME" projects --exclude='node_modules' --exclude='target' --exclude='.venv'
sha256sum "$dest"/* > "$dest/SHA256SUMS"
find "$HOME/backups" -mindepth 1 -maxdepth 1 -type d -mtime +14 -exec rm -rf {} +   # conserva 14 días
```

| Pieza | Significado |
|---|---|
| `pg_dump -Fc` / `mongodump --archive --gzip` | Copias lógicas de las bases de datos ([SQL](#sql), [MongoDB](#mongo)) |
| `tar -czf … --exclude=…` | Empaqueta y comprime sin las carpetas regenerables |
| `sha256sum > SHA256SUMS` | Huellas para verificar que las copias no se corrompen (`sha256sum -c SHA256SUMS`) |
| `find … -mtime +14 -exec rm -rf {} +` | Borra las carpetas de más de 14 días |
| `set -euo pipefail` | Detiene el script ante cualquier fallo |

**Regla 3-2-1:** 3 copias, 2 soportes, 1 fuera de casa. **Una copia no probada no existe**: restaura de verdad en una VM de pruebas al menos una vez.

- **Conceptos:** copia lógica frente a instantánea, retención, verificación, cifrado de copias con datos sensibles ([Criptografía](#cripto)).
- **Verificar:** restaurar y comprobar los datos; `sha256sum -c`.
- **Errores frecuentes:** guardar las copias en el mismo disco; no probar la restauración; incluir secretos sin cifrar; llenar el disco con copias antiguas.
- **Documentación:** [Endurecimiento](#hardening).
