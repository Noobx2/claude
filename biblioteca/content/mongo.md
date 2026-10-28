# MongoDB

MongoDB es una base de datos **de documentos**: guarda objetos parecidos a JSON (en realidad BSON) agrupados en colecciones, sin esquema fijo. Encaja muy bien con JavaScript porque los datos tienen la misma forma que tus objetos.

*Verificado: 2026-10-27. No se ha podido ejecutar un servidor MongoDB desde aquí: los comandos y el código usan la sintaxis estable de `mongosh` y del driver oficial `mongodb`; pruébalos en tu VM. Referencia: [mongodb.com/docs](https://www.mongodb.com/docs/).*

## 1. Cuándo MongoDB y cuándo SQL

| | MongoDB (documentos) | PostgreSQL (relacional) |
|---|---|---|
| Datos | Documentos anidados y flexibles | Tablas con filas y columnas |
| Esquema | Opcional (puedes validarlo) | Obligatorio |
| Relaciones | Se anidan o se referencian | `JOIN` y claves foráneas |
| Transacciones complejas | Posibles, menos habituales | Muy maduras |
| Cuándo | Prototipos rápidos, contenido variable, datos que se leen juntos | Datos muy relacionados, informes, integridad estricta |

No es «mejor» ni «peor»: es distinto. Para tu lista de tareas funciona perfectamente; para una contabilidad con muchas relaciones, mejor [SQL](#sql).

## 2. Vocabulario

| MongoDB | Equivalente SQL |
|---|---|
| Base de datos | Base de datos |
| Colección | Tabla |
| Documento | Fila |
| Campo | Columna |
| `_id` | Clave primaria (se crea sola como `ObjectId`) |
| Índice | Índice |

Un documento:

```json
{
  "_id": { "$oid": "6720f0a1c3d4e5f6a7b8c9d0" },
  "title": "Estudiar MongoDB",
  "done": false,
  "tags": ["estudio", "datos"],
  "createdAt": { "$date": "2026-10-27T10:00:00Z" }
}
```

## 3. Instalar en tu VM con Docker

No instales Mongo en el sistema: usa un contenedor (ver [Docker y Compose](#docker)).

```bash
docker run -d --name tasks-mongo -p 127.0.0.1:27017:27017 -v tasks-mongo-data:/data/db mongo:8
docker exec -it tasks-mongo mongosh
```

Dentro de `mongosh`:

```text
test> show dbs
test> use tasks
tasks> db.getName()
```

> **Peligro:** esta configuración **no tiene autenticación** y está bien solo porque el puerto se publica en `127.0.0.1`. En producción activa la autenticación (`MONGO_INITDB_ROOT_USERNAME` y `MONGO_INITDB_ROOT_PASSWORD` en la imagen) y nunca expongas el puerto 27017 a Internet.

Alternativa: instala `mongosh` en tu PC anfitrión para conectarte a la VM, o usa MongoDB Compass (interfaz gráfica).

## 4. CRUD en `mongosh`

```javascript
use tasks

// Crear
db.tasks.insertOne({ title: 'Estudiar MongoDB', done: false, priority: 2 })
db.tasks.insertMany([
  { title: 'Hacer deporte', done: false, priority: 3 },
  { title: 'Comprar pan', done: true, priority: 1 },
])

// Leer
db.tasks.find()
db.tasks.find({ done: false })
db.tasks.find({ priority: { $gte: 2 } }, { title: 1, _id: 0 })
db.tasks.find().sort({ priority: -1 }).limit(2)
db.tasks.findOne({ title: 'Comprar pan' })
db.tasks.countDocuments({ done: false })

// Actualizar
db.tasks.updateOne({ title: 'Comprar pan' }, { $set: { done: false } })
db.tasks.updateMany({ done: false }, { $inc: { priority: 1 } })

// Eliminar
db.tasks.deleteOne({ title: 'Hacer deporte' })
db.tasks.deleteMany({ done: true })
```

| Pieza | Significado |
|---|---|
| `use tasks` | Selecciona (o crea al primer uso) la base de datos |
| `db.tasks` | La colección `tasks` (se crea al insertar) |
| `insertOne` / `insertMany` | Insertan un documento / varios |
| `find(filtro, proyección)` | Filtro: qué documentos. Proyección: qué campos (`1` incluir, `0` excluir) |
| `sort({ priority: -1 })` | Ordena: `1` ascendente, `-1` descendente |
| `limit(n)` / `skip(n)` | Paginación |
| `updateOne(filtro, cambios)` | **Siempre** con un operador como `$set`: sin él se intentaría reemplazar el documento |
| `deleteOne` / `deleteMany` | Eliminan |

### Operadores más usados

| Operador | Significado | Ejemplo |
|---|---|---|
| `$eq`, `$ne` | Igual, distinto | `{ done: { $ne: true } }` |
| `$gt`, `$gte`, `$lt`, `$lte` | Comparaciones | `{ priority: { $gte: 2 } }` |
| `$in`, `$nin` | Está / no está en una lista | `{ priority: { $in: [1, 2] } }` |
| `$and`, `$or` | Lógicos | `{ $or: [{ done: true }, { priority: 3 }] }` |
| `$regex` | Texto con expresión regular | `{ title: { $regex: 'estud', $options: 'i' } }` |
| `$set`, `$unset` | Fijar / quitar un campo | `{ $set: { done: true } }` |
| `$inc` | Sumar | `{ $inc: { priority: 1 } }` |
| `$push`, `$pull` | Añadir / quitar de un array | `{ $push: { tags: 'urgente' } }` |

### Agregaciones

Equivalente a `GROUP BY`: una tubería de etapas.

```javascript
db.tasks.aggregate([
  { $match: { done: false } },
  { $group: { _id: '$priority', total: { $sum: 1 } } },
  { $sort: { _id: 1 } },
])
```

## 5. Índices

```javascript
db.tasks.createIndex({ done: 1, priority: -1 })
db.tasks.createIndex({ title: 1 }, { unique: true })
db.tasks.getIndexes()
db.tasks.find({ done: false }).explain('executionStats')
```

Un índice acelera las búsquedas por ese campo a cambio de ocupar espacio y ralentizar un poco las escrituras. `unique: true` impide duplicados (la inserción falla con el error `E11000`). `explain` muestra si una consulta usa índice (`IXSCAN`) o recorre toda la colección (`COLLSCAN`).

## 6. Usarlo desde Node

```bash
pnpm add mongodb
```

Repositorio que sustituye al de memoria del módulo de [Node.js](#node). Cumple **el mismo contrato** (métodos `async`, ids de texto), así que el servicio y las rutas no cambian:

```js
// src/repositories/mongo-task-repository.js
import { MongoClient, ObjectId } from 'mongodb';

export async function createMongoTaskRepository(uri) {
  const client = new MongoClient(uri);
  await client.connect();
  const tasks = client.db().collection('tasks'); // el nombre de la base sale de la URI

  // Traduce el documento de Mongo (_id: ObjectId) al formato de la API (id: texto)
  const toTask = (doc) => (doc ? { id: doc._id.toString(), title: doc.title, done: doc.done } : null);
  const toObjectId = (id) => (ObjectId.isValid(id) ? new ObjectId(id) : null);

  return {
    async findAll() {
      const docs = await tasks.find().sort({ _id: 1 }).toArray();
      return docs.map(toTask);
    },

    async findById(id) {
      const _id = toObjectId(id);
      return _id ? toTask(await tasks.findOne({ _id })) : null;
    },

    async create(data) {
      const doc = { ...data };
      const { insertedId } = await tasks.insertOne(doc);
      return { id: insertedId.toString(), title: data.title, done: data.done };
    },

    async update(id, changes) {
      const _id = toObjectId(id);
      if (!_id) return null;
      const doc = await tasks.findOneAndUpdate({ _id }, { $set: changes }, { returnDocument: 'after' });
      return toTask(doc);
    },

    async remove(id) {
      const _id = toObjectId(id);
      if (!_id) return false;
      const { deletedCount } = await tasks.deleteOne({ _id });
      return deletedCount === 1;
    },

    close: () => client.close(),
  };
}
```

Y en `src/app.js`, elige el repositorio según la configuración:

```js
const repository = process.env.MONGODB_URI
  ? await createMongoTaskRepository(process.env.MONGODB_URI)
  : createTaskRepository();
```

(`buildApp` pasa a ser `async` y `server.js` hace `const app = await buildApp(...)`.)

`.env`:

```bash
MONGODB_URI=mongodb://localhost:27017/tasks
```

| Pieza | Significado |
|---|---|
| `MongoClient(uri)` | Cliente con **pool de conexiones**: se crea **una vez** y se reutiliza |
| `new ObjectId(id)` | Convierte el texto de 24 caracteres hexadecimales al tipo `ObjectId` |
| `ObjectId.isValid(id)` | Evita errores con ids mal formados |
| `$set` | Imprescindible en actualizaciones |
| `returnDocument: 'after'` | Devuelve el documento ya modificado |
| `deletedCount` | Cuántos documentos se borraron |

> **Ojo:** un `id` de texto cualquiera en la URL (`/api/tasks/abc`) no es un `ObjectId` válido: por eso `toObjectId` devuelve `null` y la API responde `404` en lugar de lanzar una excepción.

### Mongoose, el ODM

`mongoose` añade esquemas, validaciones y modelos encima del driver. Aporta estructura, a cambio de otra capa que aprender. Para empezar es mejor el driver oficial: entiendes lo que ocurre; Mongoose merece la pena en proyectos grandes con muchos modelos.

## 7. Datos que persisten, copias de seguridad

- El volumen `tasks-mongo-data` guarda los datos aunque borres el contenedor.
- Copia de seguridad: `docker exec tasks-mongo mongodump --archive --gzip > backup.archive.gz`
- Restauración: `docker exec -i tasks-mongo mongorestore --archive --gzip < backup.archive.gz`

| Pieza | Significado |
|---|---|
| `mongodump` | Exporta una base de datos |
| `--archive` | Escribe todo en un único flujo (a stdout si no das nombre) |
| `--gzip` | Comprime |
| `docker exec -i` | Mantiene stdin abierto (necesario para el `<` de restaurar) |

## 8. Errores habituales

| Mensaje | Causa | Solución |
|---|---|---|
| `MongoServerSelectionError: connect ECONNREFUSED` | El servidor no está en marcha o la URI es incorrecta | `docker ps`; comprueba host y puerto |
| `E11000 duplicate key error` | Violas un índice único | Cambia el dato o gestiona el error (`409`) |
| `BSONError: input must be a 24 character hex string…` | Id con formato inválido | Valida con `ObjectId.isValid` |
| La consulta devuelve `[]` | Estás en otra base de datos o colección | `show dbs`, `use tasks`, `show collections` |
| Actualizas y el documento «desaparece» o se vacía | Falta `$set` en el reemplazo | Usa operadores de actualización |
| `Authentication failed` | Credenciales incorrectas o falta `authSource` | `mongodb://user:pass@host/db?authSource=admin` |
| Los datos se pierden al recrear el contenedor | No hay volumen | Monta `-v nombre:/data/db` |

## 9. Ejercicios

> **Ejercicio:** en `mongosh`, inserta 10 tareas con prioridades variadas y escribe las consultas para: tareas pendientes de prioridad ≥ 2, las 3 más urgentes, y cuántas hay por prioridad (con `aggregate`).

> **Ejercicio:** crea un índice único sobre `title` y provoca el error `E11000`. Haz que la API lo traduzca a una respuesta `409`.

> **Ejercicio:** cambia la API de [Node.js](#node) para usar Mongo y comprueba que las pruebas existentes siguen pasando al arrancar con y sin `MONGODB_URI`.

> **Ejercicio:** añade al documento los campos `createdAt` y `tags`, y escribe una consulta que devuelva las tareas creadas hoy con la etiqueta `urgente`.

## 10. Documentación oficial

- [Manual de MongoDB](https://www.mongodb.com/docs/manual/)
- [Driver de Node.js](https://www.mongodb.com/docs/drivers/node/current/)
- [mongosh](https://www.mongodb.com/docs/mongodb-shell/)
