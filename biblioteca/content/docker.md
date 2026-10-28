# Docker y Compose

Docker ejecuta aplicaciones en **contenedores**: procesos aislados que llevan todo lo que necesitan (sistema base, runtime, librerías). Lo que funciona en un contenedor funciona igual en tu VM, en el servidor o en la máquina de otra persona. Compose describe varios contenedores (tu API, la base de datos) en un solo archivo.

*Verificado: 2026-10-27. Tu equipo: Docker 29.9.1, Compose v5.6.0, servicio activo. Los comandos no se han podido ejecutar desde aquí (sin daemon de Docker); la sintaxis es la estable de Docker. Referencia: [docs.docker.com](https://docs.docker.com/).*

## 1. Ideas clave

| Concepto | Qué es | Analogía |
|---|---|---|
| **Imagen** | Plantilla de solo lectura, en capas | La receta / la clase |
| **Contenedor** | Instancia en ejecución (o detenida) de una imagen | El plato / el objeto |
| **Volumen** | Almacenamiento que **sobrevive** al contenedor | Un disco externo |
| **Red** | Red virtual que conecta contenedores por nombre | Un conmutador |
| **Registro** | Almacén de imágenes (Docker Hub) | Un repositorio de recetas |
| **Dockerfile** | Archivo con las instrucciones para construir una imagen | La receta escrita |
| **Compose** | Archivo `compose.yaml` que define varios servicios | El menú completo |

> **Regla:** un contenedor es **desechable**. Todo lo que deba perdurar (datos de la base de datos) va en un volumen o en una carpeta montada.

## 2. Tu situación

El informe muestra un laboratorio anterior, `go-flutter-lab`, detenido:

| Recurso | Nombre |
|---|---|
| Contenedores | `go-flutter-lab-postgres-1`, `go-flutter-lab-backend-1` |
| Imágenes | `go-flutter-lab-backend`, `postgres:18`, `hello-world` |
| Volúmenes | `go-flutter-lab_postgres-data`, `go-flutter-lab_go-mod-cache`, `go-flutter-lab_go-build-cache` |

El sufijo `-1` y el prefijo `go-flutter-lab_` indican que se crearon con **Compose** desde una carpeta llamada `go-flutter-lab` (su nombre de proyecto). Para volver a levantarlo, entra en esa carpeta y ejecuta `docker compose up -d`.

## 3. Primeros comandos

```bash
docker run --rm hello-world
docker ps
docker ps -a
docker images
docker volume ls
docker system df
```

| Comando | Significado |
|---|---|
| `docker run imagen` | Crea e inicia un contenedor |
| `--rm` | Lo borra al terminar |
| `docker ps` | Contenedores en ejecución; `-a` incluye los detenidos; `-q` solo los ids |
| `docker images` | Imágenes locales |
| `docker volume ls` | Volúmenes |
| `docker system df` | Cuánto espacio usan imágenes, contenedores y volúmenes |

## 4. `docker run`, flag a flag

```bash
docker run -d --name my-mongo -p 127.0.0.1:27017:27017 -v mongo-data:/data/db -e TZ=UTC --restart unless-stopped mongo:8
```

| Pieza | Significado |
|---|---|
| `-d` | *Detached*: en segundo plano |
| `--name my-mongo` | Nombre del contenedor (si no, Docker inventa uno) |
| `-p 127.0.0.1:27017:27017` | Publica un puerto: `IP_anfitrión:puerto_anfitrión:puerto_contenedor`. Con `127.0.0.1` solo es accesible desde la propia máquina |
| `-v mongo-data:/data/db` | Monta el volumen `mongo-data` en la carpeta `/data/db` del contenedor (si la parte izquierda es una ruta, como `./datos`, es un montaje de carpeta) |
| `-e TZ=UTC` | Variable de entorno dentro del contenedor |
| `--restart unless-stopped` | Se reinicia solo salvo que lo pares tú |
| `mongo:8` | Imagen y etiqueta (versión). Sin etiqueta sería `latest`, que cambia sin avisar |

Otras opciones frecuentes:

| Flag | Significado |
|---|---|
| `-it` | `-i` mantiene la entrada abierta y `-t` asigna terminal: para sesiones interactivas |
| `--network nombre` | Conecta a una red concreta |
| `-w /app` | Directorio de trabajo dentro del contenedor |
| `-u 1000:1000` | Ejecuta con ese usuario y grupo |
| `--env-file .env` | Carga variables desde un archivo |

> **Peligro:** `-p 27017:27017` (sin IP) publica el puerto en **todas** las interfaces: cualquiera que alcance tu máquina puede conectarse a la base de datos. Para desarrollo usa siempre `127.0.0.1:` delante.

## 5. Gestionar contenedores

```bash
docker logs -f --tail 100 my-mongo
docker exec -it my-mongo mongosh
docker stop my-mongo
docker start my-mongo
docker rm my-mongo
docker rm -f my-mongo
```

| Comando | Significado |
|---|---|
| `docker logs my-mongo` | Salida del contenedor; `-f` la sigue; `--tail 100` solo las últimas 100 líneas |
| `docker exec -it c comando` | Ejecuta un comando **dentro** de un contenedor en marcha (`-it` para interactivo) |
| `docker stop` / `start` / `restart` | Parar, iniciar, reiniciar |
| `docker rm c` | Borra un contenedor detenido; `-f` lo fuerza aunque esté corriendo; `-v` borra también sus volúmenes anónimos |
| `docker inspect c` | Toda la configuración en JSON |
| `docker stats` | Uso de CPU y memoria en vivo |
| `docker cp c:/ruta ./destino` | Copia archivos entre contenedor y anfitrión |

## 6. Imágenes y limpieza

```bash
docker pull node:24-slim
docker rmi node:24-slim
docker image prune
docker system prune
```

| Comando | Significado |
|---|---|
| `docker pull` | Descarga una imagen |
| `docker rmi` | Borra una imagen |
| `docker image prune` | Borra imágenes sin etiqueta («colgantes»); `-a` borra todas las que no use ningún contenedor |
| `docker system prune` | Borra contenedores parados, redes y caché sin uso; `--volumes` añade volúmenes sin uso (**¡pierde datos!**) |

> **Peligro:** `docker system prune -a --volumes` puede borrar los datos de tus bases de datos. Antes ejecuta `docker volume ls` y `docker system df`.

## 7. Dockerfile

Imagen para la API de [Node.js](#node):

```dockerfile
# syntax=docker/dockerfile:1
FROM node:24-slim

WORKDIR /app
RUN npm install -g pnpm

# Primero solo los manifiestos: así la instalación se guarda en caché si no cambian
COPY package.json pnpm-lock.yaml ./
RUN pnpm install --prod --frozen-lockfile

COPY src ./src

ENV NODE_ENV=production
EXPOSE 3000
USER node
CMD ["node", "src/server.js"]
```

| Instrucción | Significado |
|---|---|
| `FROM imagen` | Imagen base. `-slim` es una variante pequeña |
| `WORKDIR /app` | Carpeta de trabajo; se crea si no existe |
| `RUN comando` | Ejecuta un comando **durante la construcción** |
| `COPY origen destino` | Copia archivos del proyecto a la imagen |
| `ENV` | Variable de entorno por defecto |
| `EXPOSE 3000` | **Documenta** el puerto; no lo publica (eso lo hace `-p` o Compose) |
| `USER node` | No ejecutar como root |
| `CMD [...]` | Comando por defecto al iniciar el contenedor (forma de lista, sin shell) |

Cada instrucción crea una **capa**. El orden importa: pon lo que cambia poco arriba (dependencias) y lo que cambia mucho abajo (tu código).

`.dockerignore` (misma idea que `.gitignore`; evita copiar basura a la imagen):

```text
node_modules
.git
.env
*.log
dist
```

Construir y ejecutar:

```bash
docker build -t tasks-api:dev .
docker run --rm -p 127.0.0.1:3000:3000 --env-file .env tasks-api:dev
```

| Pieza | Significado |
|---|---|
| `docker build` | Construye una imagen |
| `-t tasks-api:dev` | Le pone nombre y etiqueta |
| `.` | Contexto de construcción: la carpeta actual |
| `-f otro.Dockerfile` | Usa otro archivo en lugar de `Dockerfile` |
| `--no-cache` | Ignora la caché de capas |

## 8. Docker Compose

`compose.yaml` en la raíz del proyecto: la API y MongoDB juntas.

```yaml
services:
  api:
    build: .
    ports:
      - "127.0.0.1:3000:3000"
    environment:
      PORT: "3000"
      MONGODB_URI: mongodb://mongo:27017/tasks
      CORS_ORIGIN: http://localhost:5173
    depends_on:
      mongo:
        condition: service_healthy
    restart: unless-stopped

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

volumes:
  mongo-data:
```

| Clave | Significado |
|---|---|
| `services` | Cada entrada es un contenedor |
| `build: .` | Construye con el `Dockerfile` de esta carpeta |
| `image` | Usa una imagen existente |
| `ports` | Igual que `-p` |
| `environment` | Variables de entorno |
| `volumes` | Montajes; el bloque `volumes:` final **declara** los volúmenes con nombre |
| `depends_on` + `condition: service_healthy` | La API espera a que Mongo responda al *healthcheck* |
| `healthcheck` | Comando que decide si el servicio está sano |
| `restart` | Política de reinicio |

Dentro de la red de Compose, los servicios se encuentran **por nombre**: la API usa `mongo` como host (`mongodb://mongo:27017`), no `localhost`.

```bash
docker compose up -d
docker compose ps
docker compose logs -f api
docker compose exec mongo mongosh
docker compose down
```

| Comando | Significado |
|---|---|
| `docker compose up` | Crea y arranca los servicios; `-d` en segundo plano; `--build` reconstruye imágenes |
| `docker compose ps` | Estado de los servicios |
| `docker compose logs -f servicio` | Registros en vivo |
| `docker compose exec servicio cmd` | Comando dentro de un servicio |
| `docker compose run --rm servicio cmd` | Contenedor temporal para una tarea |
| `docker compose stop` / `start` | Parar / iniciar sin borrar |
| `docker compose down` | Para y **borra** contenedores y redes; **no** los volúmenes |
| `docker compose down -v` | Además borra los volúmenes (**pierdes los datos**) |
| `docker compose config` | Muestra el archivo ya interpretado: ideal para depurar |

> **Ojo:** el nombre `compose.yaml` es el actual; `docker-compose.yml` sigue funcionando. La clave `version:` de ejemplos antiguos ya no hace falta.

## 9. Docker en tu VM

- Tu usuario está en el grupo `docker`. Eso equivale a tener permisos de administrador: quien controle tu cuenta controla la máquina.
- El servicio arranca solo con el sistema (`docker.service` activo).
- Puertos: publícalos con `127.0.0.1:` y accede desde tu PC con un túnel SSH (`ssh -L 3000:localhost:3000 usuario@vm`) o con VS Code Remote, que reenvía los puertos.

## 10. Errores habituales

| Mensaje | Causa | Solución |
|---|---|---|
| `permission denied … /var/run/docker.sock` | Tu usuario no está en el grupo `docker` (o no reiniciaste sesión) | `id -nG`; cierra sesión y vuelve a entrar |
| `port is already allocated` / `address already in use` | El puerto del anfitrión está ocupado | `ss -ltnp \| grep 27017` y cambia el puerto o para el otro proceso |
| `No such container` | Nombre erróneo | `docker ps -a` |
| `pull access denied` / `manifest unknown` | Imagen o etiqueta inexistente | Revisa el nombre y la etiqueta en Docker Hub |
| La API no conecta con Mongo | Usas `localhost` dentro del contenedor | Usa el nombre del servicio (`mongo`) |
| Pierdes los datos al recrear | No había volumen | Declara un volumen con nombre |
| `no space left on device` | Imágenes y caché acumuladas | `docker system df` y limpia con cuidado |
| Cambios de código que no se ven | Imagen antigua | `docker compose up -d --build` |
| `exec format error` | Imagen para otra arquitectura (ARM frente a amd64) | Usa una imagen para tu arquitectura |

## 11. Ejercicios

> **Ejercicio:** arranca PostgreSQL con `docker run`, conéctate con `docker exec -it … psql`, crea una tabla, detén y borra el contenedor y comprueba que los datos siguen si usaste un volumen.

> **Ejercicio:** escribe un `Dockerfile` para el servidor `server-basic.mjs` de [Node.js](#node), constrúyelo y ejecútalo publicando el puerto solo en `127.0.0.1`.

> **Ejercicio:** localiza la carpeta del laboratorio `go-flutter-lab`, ejecuta `docker compose config` y escribe qué hace cada servicio y cada volumen.

> **Ejercicio:** rompe a propósito el orden de las instrucciones del Dockerfile (copiar el código antes de instalar dependencias) y mide la diferencia de tiempo al reconstruir tras un cambio mínimo.

## 12. Documentación oficial

- [Docker: primeros pasos](https://docs.docker.com/get-started/)
- [Referencia del Dockerfile](https://docs.docker.com/reference/dockerfile/)
- [Referencia de Compose](https://docs.docker.com/reference/compose-file/)
- [Docker Hub](https://hub.docker.com/)
