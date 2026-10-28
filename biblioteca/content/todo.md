# Proyecto: lista de tareas de extremo a extremo

Unes todo lo aprendido: una API ([Node.js](#node)), una base de datos ([MongoDB](#mongo)), una interfaz ([React](#react) o [Vue](#vue)), contenedores ([Docker](#docker)) y control de versiones ([Git](#git)). Es el primer proyecto completo de tu ruta.

*Verificado: 2026-10-27. La API y las dos interfaces se han ejecutado y probado de forma automática en un navegador real. La parte de MongoDB y Docker no se ha podido ejecutar desde aquí: pruébala en tu VM.*

## 1. Qué vas a construir

Una aplicación para listar, crear, marcar y borrar tareas, con filtro, estados de carga y error, y datos persistentes.

| Capa | Tecnología | Módulo |
|---|---|---|
| Interfaz | React o Vue con TypeScript y Vite | [React](#react), [Vue](#vue) |
| API | Node.js + Fastify, por capas | [Node.js](#node) |
| Datos | MongoDB en Docker | [MongoDB](#mongo) |
| Ejecución | Docker Compose | [Docker](#docker) |
| Historial | Git y GitHub | [Git](#git) |

## 2. Estructura del proyecto

La estructura mínima que definiste para proyectos propios:

```text
tasks-app/
  README.md
  AGENT.md                  normas del proyecto (opcional pero útil)
  docs/                     decisiones y notas
  compose.yaml              API + MongoDB (+ interfaz en producción)
  .gitignore
  app/
    backend/                la API (proyecto del módulo Node.js)
      package.json
      src/…
      Dockerfile
    frontend/               la interfaz (proyecto Vite)
      package.json
      src/…
```

```bash
mkdir -p ~/projects/tasks-app/{docs,app} && cd ~/projects/tasks-app
git init -b main
```

| Pieza | Significado |
|---|---|
| `mkdir -p` | Crea las carpetas intermedias |
| `{docs,app}` | *Brace expansion* del shell: crea `docs` y `app` |

## 3. Hoja de ruta (sigue este orden)

Cada paso termina con algo que **funciona** y se puede probar.

| Paso | Qué haces | Cómo compruebas |
|---|---|---|
| 1 | Backend en memoria: rutas, servicio, repositorio (módulo Node.js) | `pnpm test` pasa; `curl` crea y lista tareas |
| 2 | Primer commit y `.gitignore` | `git status` limpio; `.env` no aparece |
| 3 | Frontend: lista, formulario, filtro | Se ve en el navegador contra la API |
| 4 | Estados: cargando, error con reintento, vacío, confirmación | Para la API y comprueba el error |
| 5 | MongoDB en Docker y repositorio Mongo | Reinicia la API y los datos siguen |
| 6 | Pruebas: de la API, de un componente, un recorrido completo | `pnpm test` en ambas carpetas |
| 7 | Docker Compose para todo el stack | `docker compose up -d` y la app funciona |
| 8 | README con instrucciones | Otra persona (o tú dentro de un mes) puede arrancarlo |

## 4. Variables y secretos

```bash
# app/backend/.env.example
PORT=3000
CORS_ORIGIN=http://localhost:5173
MONGODB_URI=mongodb://localhost:27017/tasks

# app/frontend/.env.example
VITE_API_URL=http://localhost:3000/api
```

Cada carpeta tiene su `.env.example` versionado y su `.env` real ignorado. Ninguna credencial real en el repositorio (ver [Git](#git)).

## 5. `compose.yaml` del conjunto

```yaml
services:
  mongo:
    image: mongo:8
    volumes:
      - mongo-data:/data/db
    ports:
      - "127.0.0.1:27017:27017"
    healthcheck:
      test: ["CMD", "mongosh", "--quiet", "--eval", "db.adminCommand('ping').ok"]
      interval: 10s
      timeout: 5s
      retries: 5

  api:
    build: ./app/backend
    environment:
      PORT: "3000"
      MONGODB_URI: mongodb://mongo:27017/tasks
      CORS_ORIGIN: http://localhost:5173
    ports:
      - "127.0.0.1:3000:3000"
    depends_on:
      mongo:
        condition: service_healthy
    restart: unless-stopped

volumes:
  mongo-data:
```

En desarrollo, el frontend se ejecuta con `pnpm dev` fuera de Docker (recarga instantánea); en producción se compila (`pnpm build`) y se sirve `dist/` con nginx.

## 6. Prueba de recorrido completo (end to end)

Una prueba E2E controla un navegador real y recorre la aplicación como una persona. Este script es el que se usó para validar las dos interfaces de este curso:

```bash
pnpm add -D playwright
pnpm exec playwright install chromium
```

```js
// e2e.cjs  →  node e2e.cjs  (con la API y la interfaz en marcha)
const { chromium } = require('playwright');

(async () => {
  const browser = await chromium.launch();
  const page = await browser.newPage();
  const errors = [];
  page.on('pageerror', (e) => errors.push(e.message));
  page.on('console', (m) => { if (m.type() === 'error') errors.push(m.text()); });

  await page.goto('http://localhost:5173');
  await page.getByText('No hay tareas').waitFor();

  await page.getByLabel('Nueva tarea').fill('x');
  await page.getByRole('button', { name: 'Añadir' }).click();
  console.log('validación:', await page.getByRole('alert').innerText());

  await page.getByLabel('Nueva tarea').fill('Estudiar React');
  await page.getByRole('button', { name: 'Añadir' }).click();
  await page.getByText('Estudiar React').waitFor();

  await page.getByRole('checkbox').first().click();
  await page.getByText('0 pendientes').waitFor();

  await page.getByRole('button', { name: 'Eliminar' }).first().click();
  await page.getByRole('button', { name: 'Sí' }).click();
  await page.getByText('No hay tareas').waitFor();

  console.log('errores de consola:', errors);
  await browser.close();
})().catch((e) => { console.error('FALLO', e.message); process.exit(1); });
```

| Pieza | Significado |
|---|---|
| `chromium.launch()` | Abre un Chromium sin ventana (modo *headless*) |
| `getByLabel`, `getByRole`, `getByText` | Localizan elementos como los ve un usuario |
| `.waitFor()` | Espera a que aparezca el elemento (evita pausas fijas) |
| `page.on('console'…)` | Recoge errores de la consola del navegador |

> **Ojo:** si un checkbox está controlado y se actualiza tras la respuesta del servidor, usa `.click()` y espera el resultado; `.check()` falla porque comprueba el estado antes de que llegue la respuesta.

**Dos fallos reales que esta prueba destapó al escribir el curso:**

1. `PATCH` y `DELETE` fallaban en el navegador pero funcionaban con `curl`: el plugin de CORS solo permite `GET`, `HEAD` y `POST` por defecto.
2. Eliminar devolvía `400`: el cliente enviaba `Content-Type: application/json` sin cuerpo y Fastify lo rechaza.

Las pruebas unitarias de la API no podían verlos; un recorrido real sí. Por eso se prueban las dos cosas.

## 7. Lista de comprobación antes de dar el proyecto por terminado

- [ ] `pnpm test` pasa en `app/backend` y `app/frontend`.
- [ ] `pnpm lint` y `pnpm build` terminan sin errores en el frontend.
- [ ] Los estados cargando, error, vacío y confirmación funcionan.
- [ ] Reiniciar la API no pierde datos (MongoDB con volumen).
- [ ] `.env` está en `.gitignore` y `.env.example` existe en ambas carpetas.
- [ ] `docker compose up -d` levanta el conjunto desde cero.
- [ ] El `README.md` explica cómo arrancarlo en menos de 10 líneas.
- [ ] Hay un historial de commits con mensajes comprensibles.

## 8. Extensiones (de menor a mayor dificultad)

1. Editar el título y una **fecha límite** con filtro «vencidas».
2. **Paginación** y búsqueda por texto en el servidor.
3. **Usuarios**: registro e inicio de sesión (hash de contraseñas con Argon2, token firmado) — se apoya en [criptografía](#cripto).
4. **Validación con esquemas** (Zod) compartida entre cliente y servidor.
5. Despliegue en tu VM con un proxy inverso (Caddy o nginx) y HTTPS.
6. Una **CLI** que use la misma API (ver [CLI y streams](#cli-args-streams)).
7. La misma app en **Flutter** consumiendo esta API ([Flutter](#flutter)).

## 9. Ejercicios

> **Ejercicio:** escribe el `README.md` del proyecto y pide a alguien (o a ti mismo en una máquina limpia) que lo arranque siguiendo solo ese archivo. Anota todo lo que tuvo que adivinar.

> **Ejercicio:** reescribe el frontend en el otro framework (de React a Vue o al revés) reutilizando `types.ts` y `tasks-api.ts` sin cambios.

> **Ejercicio:** añade una prueba de recorrido que cubra el caso «la API no responde» y verifique que aparece el botón «Reintentar».
