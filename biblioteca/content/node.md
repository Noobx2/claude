# Node.js

Node.js ejecuta JavaScript fuera del navegador. Se usa para servidores y APIs, herramientas de línea de comandos y todo el ecosistema de construcción del frontend (Vite, TypeScript, pruebas). Es la base del backend de tu stack de apps rápidas.

*Verificado: 2026-10-27 con Node 22 (ejemplos ejecutados) y Fastify 5. Tu Node es 24.22.1. Referencia: [nodejs.org/docs](https://nodejs.org/docs/latest/api/) y [fastify.dev](https://fastify.dev/docs/latest/).*

## 1. Instalación y versiones

Ya lo tienes con NVM. Comprueba:

```bash
node --version
npm --version
pnpm --version
command -v node
```

| Pieza | Significado |
|---|---|
| `--version` | Imprime la versión y termina (`-v` es el atajo en `node`) |
| `command -v node` | Muestra de qué carpeta sale el ejecutable (debe estar en `~/.nvm/…`) |

Usa siempre una versión **LTS** (soporte largo) para proyectos reales. Los números pares (22, 24) son las líneas LTS. Detalles en [Gestores de versiones](#entorno-versiones).

## 2. Archivos y nombres

| Elemento | Convención |
|---|---|
| Archivos | `kebab-case.js` o `.mjs` |
| Variables y funciones | `camelCase` |
| Clases | `PascalCase` |
| Módulos | ES modules: `import` / `export`, con `"type": "module"` en `package.json` |
| Punto de entrada | `src/server.js` (servidor) y `src/app.js` (construcción de la app) |

## 3. Hola mundo y REPL

```bash
node -e "console.log('Hola desde Node')"
node
```

| Pieza | Significado |
|---|---|
| `node archivo.js` | Ejecuta un archivo |
| `node -e "código"` | `-e` evalúa un fragmento (*evaluate*) |
| `node` sin argumentos | Abre el REPL (consola interactiva); se sale con Ctrl+D |
| `node --watch archivo.js` | Reinicia el programa al guardar |
| `node --env-file=.env archivo.js` | Carga las variables del archivo `.env` en `process.env` |
| `node --test` | Ejecuta las pruebas integradas |
| `node --inspect archivo.js` | Abre el depurador (se conecta desde Chrome o VS Code) |

## 4. Un proyecto desde cero

```bash
mkdir -p ~/projects/tasks-api && cd ~/projects/tasks-api
git init -b main
pnpm init
```

`pnpm init` crea el `package.json`. Edítalo así:

```json
{
  "name": "tasks-api",
  "version": "0.1.0",
  "private": true,
  "type": "module",
  "scripts": {
    "dev": "node --watch --env-file=.env src/server.js",
    "start": "node src/server.js",
    "test": "node --test"
  },
  "dependencies": {
    "@fastify/cors": "^11.3.1",
    "fastify": "^5.12.7"
  }
}
```

| Campo | Para qué sirve |
|---|---|
| `name`, `version` | Identidad del paquete |
| `private: true` | Impide publicarlo por error en npm |
| `type: "module"` | Los `.js` se tratan como módulos ES (`import`) |
| `scripts` | Atajos que se ejecutan con `pnpm run <nombre>` |
| `dependencies` | Paquetes necesarios en producción (se añaden solos) |
| `devDependencies` | Paquetes solo de desarrollo (pruebas, linter, TypeScript) |

Añade dependencias:

```bash
pnpm add fastify @fastify/cors
pnpm add -D typescript
pnpm remove nombre-del-paquete
pnpm update
```

| Comando | Significado |
|---|---|
| `pnpm add paquete` | Instala y apunta en `dependencies` |
| `pnpm add -D paquete` | `-D` (`--save-dev`): en `devDependencies` |
| `pnpm install` | Instala todo lo del `package.json` según el lockfile |
| `pnpm install --frozen-lockfile` | Falla si el lockfile no coincide; es lo que se usa en CI y despliegues |
| `pnpm update` | Sube versiones dentro de lo permitido por los rangos |
| `pnpm outdated` | Muestra paquetes con versiones nuevas |
| `pnpm run dev` | Ejecuta el script `dev` (también `pnpm dev`) |
| `pnpm exec comando` | Ejecuta un binario instalado en el proyecto |
| `pnpm dlx paquete` | Ejecuta un paquete sin instalarlo (equivale a `npx`) |

### Versiones y archivos de bloqueo

| Símbolo en `package.json` | Significado |
|---|---|
| `"5.1.0"` | Exactamente esa versión |
| `"^5.1.0"` | Compatible: acepta `5.x.x` desde 5.1.0, pero no 6 |
| `"~5.1.0"` | Solo parches: `5.1.x` |

El archivo `pnpm-lock.yaml` (en npm, `package-lock.json`) fija la versión **exacta** de cada paquete instalado. **Se versiona en Git**; `node_modules/` no. No se edita a mano. Cada proyecto usa un solo gestor.

## 5. Un servidor HTTP con lo que trae Node

Sin librerías, para entender qué hay debajo:

```js
// server-basic.mjs
import { createServer } from 'node:http';

const server = createServer((request, response) => {
  if (request.method === 'GET' && request.url === '/health') {
    response.writeHead(200, { 'Content-Type': 'application/json' });
    response.end(JSON.stringify({ status: 'ok' }));
    return;
  }
  response.writeHead(404, { 'Content-Type': 'application/json' });
  response.end(JSON.stringify({ error: 'No encontrado' }));
});

server.listen(3000, () => console.log('Escuchando en http://localhost:3000'));
```

```bash
node server-basic.mjs
curl -i http://localhost:3000/health
```

| Pieza de `curl -i` | Significado |
|---|---|
| `-i` | Incluye las cabeceras de la respuesta en la salida |

Una **petición HTTP** lleva método (`GET`, `POST`…), ruta, cabeceras y a veces cuerpo; la **respuesta** lleva código de estado, cabeceras y cuerpo. Hacerlo a mano exige leer el cuerpo, interpretar JSON, validar, enrutar y manejar errores: por eso se usa un framework.

## 6. Métodos HTTP y códigos de estado

| Método | Significado | Ejemplo | Éxito |
|---|---|---|---|
| `GET` | Leer | `GET /api/tasks` | `200` |
| `POST` | Crear | `POST /api/tasks` | `201` |
| `PUT` | Reemplazar entero | `PUT /api/tasks/3` | `200` |
| `PATCH` | Modificar parte | `PATCH /api/tasks/3` | `200` |
| `DELETE` | Eliminar | `DELETE /api/tasks/3` | `204` (sin cuerpo) |

| Código | Significado |
|---|---|
| `200` OK · `201` Creado · `204` Sin contenido | Éxito |
| `400` Petición inválida | El cliente envió datos incorrectos |
| `401` No autenticado · `403` Sin permiso | Identidad / autorización |
| `404` No encontrado | No existe el recurso |
| `409` Conflicto | Por ejemplo, un duplicado |
| `500` Error interno | Fallo del servidor |

## 7. Una API REST por capas con Fastify

**Arquitectura.** Cada capa tiene una sola responsabilidad:

```text
Petición HTTP
   │
   ▼
routes/         Capa HTTP: lee la petición, llama al servicio, devuelve código y JSON
   │
   ▼
services/       Reglas de negocio y validación. No conoce HTTP ni la base de datos
   │
   ▼
repositories/   Acceso a datos. Es lo único que sabe dónde se guardan las tareas
```

> **Regla:** no pongas la lógica de negocio en las rutas. Si mañana cambias HTTP por una CLI o la base de datos por MongoDB, solo cambia la capa que corresponde.

Estructura del proyecto:

```text
tasks-api/
  package.json
  .env.example
  .gitignore
  src/
    server.js                     arranca el servidor (puerto, host)
    app.js                        construye la aplicación (plugins, rutas, errores)
    routes/tasks.js               endpoints
    services/task-service.js      reglas de negocio
    repositories/task-repository.js   datos en memoria (después: MongoDB)
    app.test.js                   pruebas
```

### Repositorio

```js
// Repositorio en memoria: guarda las tareas. Más adelante se sustituye por MongoDB
// sin tocar el servicio ni las rutas: ambos usan el mismo contrato (métodos async, ids de texto).
export function createTaskRepository() {
  const tasks = new Map();
  let nextId = 1;

  return {
    async findAll() {
      return [...tasks.values()];
    },
    async findById(id) {
      return tasks.get(id) ?? null;
    },
    async create(data) {
      const task = { id: String(nextId), ...data };
      tasks.set(task.id, task);
      nextId += 1;
      return task;
    },
    async update(id, changes) {
      const current = tasks.get(id);
      if (!current) return null;
      const updated = { ...current, ...changes };
      tasks.set(id, updated);
      return updated;
    },
    async remove(id) {
      return tasks.delete(id);
    },
  };
}
```

### Servicio

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
    list: () => repository.findAll(),

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

### Rutas

```js
// Capa HTTP: traduce peticiones a llamadas al servicio y errores a códigos de estado.
export default async function taskRoutes(app, { taskService }) {
  const idParams = {
    type: 'object',
    properties: { id: { type: 'string', minLength: 1 } },
    required: ['id'],
  };

  app.get('/tasks', async () => taskService.list());

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

### Aplicación

```js
import Fastify from 'fastify';
import cors from '@fastify/cors';
import taskRoutes from './routes/tasks.js';
import { createTaskRepository } from './repositories/task-repository.js';
import { createTaskService, NotFoundError, ValidationError } from './services/task-service.js';

export function buildApp({ logger = false } = {}) {
  const app = Fastify({ logger });
  const taskService = createTaskService(createTaskRepository());

  // Por defecto el plugin solo permite GET, HEAD y POST: PATCH y DELETE hay que habilitarlos
  app.register(cors, {
    origin: process.env.CORS_ORIGIN ?? 'http://localhost:5173',
    methods: ['GET', 'POST', 'PATCH', 'DELETE'],
  });
  app.register(taskRoutes, { prefix: '/api', taskService });

  // Un único sitio donde los errores se convierten en respuestas
  app.setErrorHandler((error, request, reply) => {
    if (error instanceof ValidationError) return reply.code(400).send({ error: error.message });
    if (error instanceof NotFoundError) return reply.code(404).send({ error: error.message });
    if (error.validation) return reply.code(400).send({ error: error.message });
    request.log.error(error);
    return reply.code(500).send({ error: 'Error interno' });
  });

  return app;
}
```

### Servidor

```js
import { buildApp } from './app.js';

const port = Number(process.env.PORT ?? 3000);
const app = buildApp({ logger: true });

try {
  await app.listen({ port, host: '0.0.0.0' });
} catch (error) {
  app.log.error(error);
  process.exit(1);
}
```

| Pieza | Significado |
|---|---|
| `Fastify({ logger })` | Crea el servidor; el *logger* registra cada petición |
| `app.register(plugin, opciones)` | Añade funcionalidad modular (CORS, rutas con prefijo `/api`) |
| `schema.params` | Validación automática de parámetros de ruta con JSON Schema |
| `reply.code(201).send(x)` | Fija el código de estado y envía el cuerpo (Fastify serializa el objeto a JSON) |
| `setErrorHandler` | Un único lugar donde los errores se transforman en respuestas |
| `host: '0.0.0.0'` | Escucha en todas las interfaces (necesario en contenedores y para acceder desde fuera de la VM) |
| `process.env.PORT ?? 3000` | Configuración por variable de entorno con valor por defecto |

### Pruebas

```js
import { test } from 'node:test';
import assert from 'node:assert/strict';
import { buildApp } from './app.js';

test('CRUD de tareas', async () => {
  const app = buildApp();

  const created = await app.inject({ method: 'POST', url: '/api/tasks', payload: { title: 'Estudiar Node' } });
  assert.equal(created.statusCode, 201);
  assert.equal(created.json().done, false);

  const list = await app.inject({ method: 'GET', url: '/api/tasks' });
  assert.equal(list.json().length, 1);

  const patched = await app.inject({ method: 'PATCH', url: '/api/tasks/1', payload: { done: true } });
  assert.equal(patched.json().done, true);

  const invalid = await app.inject({ method: 'POST', url: '/api/tasks', payload: { title: 'x' } });
  assert.equal(invalid.statusCode, 400);

  const removed = await app.inject({ method: 'DELETE', url: '/api/tasks/1' });
  assert.equal(removed.statusCode, 204);

  const missing = await app.inject({ method: 'GET', url: '/api/tasks/1' });
  assert.equal(missing.statusCode, 404);

  await app.close();
});
```

```bash
pnpm test
```

`app.inject()` simula peticiones **sin abrir un puerto**: rápido y estable.

### Ejecutar y probar a mano

```bash
pnpm dev                        # servidor con recarga
curl -s -X POST -H 'Content-Type: application/json' -d '{"title":"Estudiar Node"}' http://localhost:3000/api/tasks
curl -s http://localhost:3000/api/tasks
curl -s -X PATCH -H 'Content-Type: application/json' -d '{"done":true}' http://localhost:3000/api/tasks/1
curl -s -o /dev/null -w "%{http_code}\n" -X DELETE http://localhost:3000/api/tasks/1
```

| Flag de `curl` | Significado |
|---|---|
| `-s` | Silencioso |
| `-X MÉTODO` | Método HTTP |
| `-H 'Cabecera: valor'` | Añade una cabecera; aquí indica que el cuerpo es JSON |
| `-d '{…}'` | Cuerpo de la petición |
| `-o /dev/null` | Descarta el cuerpo de la respuesta |
| `-w "%{http_code}\n"` | Imprime el código de estado al final |

## 8. Configuración, secretos y entornos

```bash
# .env.example  (se versiona; valores ficticios)
PORT=3000
CORS_ORIGIN=http://localhost:5173
# MONGODB_URI=mongodb://localhost:27017/tasks
```

```bash
cp .env.example .env       # .env real: NO se sube a Git
```

- Node lee `.env` con `node --env-file=.env …` (sin librerías).
- En producción las variables las define el servidor o el contenedor, no un archivo.
- Valida las variables al arrancar: si falta una obligatoria, falla con un mensaje claro.

## 9. CORS y autenticación

**CORS** (*Cross-Origin Resource Sharing*): el navegador impide que una página de `http://localhost:5173` lea respuestas de `http://localhost:3000` salvo que el servidor lo permita con cabeceras (`Access-Control-Allow-Origin`). En el ejemplo lo hace `@fastify/cors`, limitado a un origen. **No pongas `*` con datos privados.**

**Autenticación** (¿quién eres?) y **autorización** (¿qué puedes hacer?): el esquema habitual es iniciar sesión, recibir un token firmado (JWT) o una cookie de sesión, y comprobarlo en cada petición. Se estudia más adelante junto a [criptografía aplicada](#cripto); no guardes nunca contraseñas en claro.

## 10. Logging y errores

- Registra con el logger del framework (`request.log.info(...)`), no con `console.log` suelto.
- Niveles: `error`, `warn`, `info`, `debug`.
- Al cliente nunca le envíes la traza de un fallo interno; regístrala y devuelve un mensaje genérico (`500`).
- No registres contraseñas, tokens ni datos personales.

## 11. Desarrollo frente a producción

| | Desarrollo | Producción |
|---|---|---|
| Arranque | `node --watch --env-file=.env src/server.js` | `node src/server.js` bajo un gestor (Docker, systemd) |
| Dependencias | Todas | `pnpm install --prod --frozen-lockfile` |
| `NODE_ENV` | `development` | `production` |
| Logs | Legibles en consola | JSON a un agregador |
| Reinicio ante caída | No | Sí (contenedor con `restart`, systemd) |
| HTTPS | No | Detrás de un proxy inverso (nginx, Caddy) |

## 12. Errores habituales

| Mensaje | Causa | Solución |
|---|---|---|
| `Cannot use import statement outside a module` | Falta `"type": "module"` | Añádelo a `package.json` |
| `ERR_MODULE_NOT_FOUND` | Ruta de import sin extensión o con nombre erróneo | Usa `./archivo.js` completo |
| `EADDRINUSE: address already in use :::3000` | Otro proceso usa el puerto | `ss -ltnp \| grep 3000` y detén ese proceso, o cambia `PORT` |
| `Cannot find module 'x'` | Falta instalar | `pnpm install` |
| `EACCES` al instalar global | Intentas instalar con permisos de sistema | No uses `sudo` con npm; con NVM no hace falta |
| La petición desde el navegador falla pero con `curl` funciona | CORS | Revisa `CORS_ORIGIN` y la pestaña *Red* |
| La petición `PATCH` o `DELETE` falla en el navegador por CORS pero con `curl` funciona | `@fastify/cors` solo permite `GET`, `HEAD` y `POST` por defecto | Declara `methods: ['GET','POST','PATCH','DELETE']` en el registro del plugin |
| `FST_ERR_CTP_EMPTY_JSON_BODY` (`Body cannot be empty when content-type is set to 'application/json'`) | Un `DELETE` o `POST` sin cuerpo declara `Content-Type: application/json` | Envía esa cabecera solo cuando haya cuerpo |
| Cambios que no se ven | El servidor no se reinició | Usa `--watch` |

## 13. Ejercicios

> **Ejercicio:** añade al servicio la regla «no puede haber dos tareas con el mismo título» y haz que la API responda `409`. Escribe primero la prueba.

> **Ejercicio:** añade `GET /api/tasks?done=true` para filtrar, y `?limit=2&offset=0` para paginar. Comprueba los casos con una lista vacía y con valores inválidos.

> **Ejercicio:** escribe un middleware (hook `onRequest`) que registre el método, la ruta y el tiempo de respuesta de cada petición.

> **Ejercicio:** sustituye `node:test` por pruebas con el servidor real levantado en un puerto aleatorio (`port: 0`) usando `fetch`.

## 14. Documentación oficial

- [Node.js: API](https://nodejs.org/docs/latest/api/)
- [Fastify: documentación](https://fastify.dev/docs/latest/)
- [pnpm: documentación](https://pnpm.io/es/)
- [MDN: HTTP](https://developer.mozilla.org/es/docs/Web/HTTP)
