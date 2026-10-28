# SQL y PostgreSQL

SQL es el lenguaje de las bases de datos **relacionales**: los datos viven en tablas con filas y columnas, enlazadas por claves. PostgreSQL es el servidor relacional de código abierto más completo y el estándar actual para aplicaciones serias.

*Verificado: 2026-10-27. Todo el SQL de este módulo se ejecutó en un servidor PostgreSQL 16 real (tu contenedor usa `postgres:18`; todo lo mostrado es SQL estándar o de uso común en ambas versiones), y el código de Node se probó con el cliente `pg`. Referencia: [postgresql.org/docs](https://www.postgresql.org/docs/current/).*

## 1. Cuándo SQL y cuándo MongoDB

Ya lo comparamos en [MongoDB](#mongo). Resumen: usa SQL cuando tus datos tienen **relaciones claras** (empleados ↔ departamentos ↔ tareas), necesitas **integridad garantizada** (que no existan tareas de un empleado inexistente) y consultas de informe (sumar, agrupar, cruzar). Usa documentos cuando los datos son flexibles y se leen siempre juntos.

## 2. PostgreSQL en tu VM con Docker

Ya tienes la imagen `postgres:18` y un contenedor detenido `go-flutter-lab-postgres-1` con su volumen. Para un entorno limpio:

```bash
docker run -d --name pg-lab \
  -e POSTGRES_PASSWORD=CHANGE_ME \
  -e POSTGRES_DB=lib_demo \
  -p 127.0.0.1:5432:5432 \
  -v pg-lab-data:/var/lib/postgresql \
  postgres:18

docker exec -it pg-lab psql -U postgres -d lib_demo
```

| Pieza | Significado |
|---|---|
| `-e POSTGRES_PASSWORD=…` | Contraseña del superusuario `postgres`. **Obligatoria**. Usa un valor propio y no lo subas a Git |
| `-e POSTGRES_DB=…` | Crea esa base de datos al primer arranque |
| `-p 127.0.0.1:5432:5432` | Publica el puerto solo en el propio equipo |
| `-v pg-lab-data:/var/lib/postgresql` | Volumen para los datos. *Comprueba en la documentación de la imagen la ruta exacta para tu versión: ha cambiado entre versiones* |
| `docker exec -it … psql -U postgres -d lib_demo` | Abre el cliente interactivo dentro del contenedor |

> **Ojo:** el volumen se inicializa **solo la primera vez**. Si cambias `POSTGRES_PASSWORD` después, no cambia la contraseña ya guardada.

## 3. El cliente `psql`

```bash
psql -h localhost -p 5432 -U postgres -d lib_demo -f schema.sql
```

| Flag | Significado |
|---|---|
| `-h` | Servidor (*host*) |
| `-p` | Puerto |
| `-U` | Usuario |
| `-d` | Base de datos |
| `-f archivo` | Ejecuta un archivo SQL |
| `-c "SQL"` | Ejecuta una sentencia y sale |
| `-v ON_ERROR_STOP=1` | Se detiene en el primer error (imprescindible en scripts) |
| `-q` | Silencioso |
| `-X` | No lee `~/.psqlrc` (reproducible) |

Dentro de `psql`: `\dt` tablas · `\d tabla` estructura · `\d+ tabla` más detalle · `\l` bases de datos · `\du` roles · `\c base` cambiar de base · `\timing` mide tiempos · `\x` salida expandida · `\i archivo` ejecutar archivo · `\q` salir.

La contraseña puede ir en la variable `PGPASSWORD` o en `~/.pgpass` (permisos `600`); nunca en la línea de comandos de un script versionado.

## 4. Crear la estructura (DDL)

```sql
-- 01_schema.sql: estructura de la base de datos (DDL)
DROP TABLE IF EXISTS tasks, employees, departments CASCADE;

CREATE TABLE departments (
    id   integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,   -- clave primaria autoincremental
    name text    NOT NULL UNIQUE                              -- obligatorio y sin repetidos
);

CREATE TABLE employees (
    id            integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name          text    NOT NULL,
    department_id integer NOT NULL REFERENCES departments (id),   -- clave foránea
    salary        integer NOT NULL CHECK (salary > 0),            -- regla de validación
    age           integer NOT NULL CHECK (age BETWEEN 16 AND 100),
    active        boolean NOT NULL DEFAULT true,
    hired_at      date    NOT NULL DEFAULT current_date
);

CREATE TABLE tasks (
    id          integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    title       text        NOT NULL CHECK (length(trim(title)) >= 2),
    done        boolean     NOT NULL DEFAULT false,
    owner_id    integer     REFERENCES employees (id) ON DELETE SET NULL,   -- si se borra el empleado, queda sin dueño
    created_at  timestamptz NOT NULL DEFAULT now()
);

CREATE INDEX idx_employees_department ON employees (department_id);   -- acelera búsquedas por departamento
CREATE INDEX idx_tasks_done ON tasks (done) WHERE done = false;       -- índice parcial: solo las pendientes
```

| Elemento | Significado |
|---|---|
| `integer`, `text`, `boolean`, `date`, `timestamptz`, `numeric(10,2)` | Tipos habituales. Usa `numeric` para dinero, nunca `float` |
| `GENERATED ALWAYS AS IDENTITY` | Clave autoincremental (sustituye a `serial`) |
| `PRIMARY KEY` | Identifica cada fila de forma única |
| `NOT NULL` | El valor es obligatorio |
| `UNIQUE` | Sin repetidos |
| `CHECK (condición)` | Regla de validación en la base de datos |
| `DEFAULT` | Valor si no se indica |
| `REFERENCES tabla (columna)` | **Clave foránea**: el valor debe existir en la otra tabla |
| `ON DELETE SET NULL` / `CASCADE` / `RESTRICT` | Qué hacer con las filas relacionadas al borrar |
| `CREATE INDEX` | Acelera búsquedas por esas columnas; `WHERE` crea un índice parcial |

## 5. Insertar datos

```sql
-- 02_seed.sql: datos de ejemplo (DML)
INSERT INTO departments (name) VALUES ('ventas'), ('tecnología'), ('recursos');

INSERT INTO employees (name, department_id, salary, age, active) VALUES
    ('Ana',   (SELECT id FROM departments WHERE name = 'ventas'),     2400, 31, true),
    ('Luis',  (SELECT id FROM departments WHERE name = 'tecnología'), 3600, 28, true),
    ('Marta', (SELECT id FROM departments WHERE name = 'tecnología'), 4100, 41, false),
    ('Pablo', (SELECT id FROM departments WHERE name = 'ventas'),     2100, 24, true),
    ('Sara',  (SELECT id FROM departments WHERE name = 'recursos'),   2800, 36, true),
    ('Iván',  (SELECT id FROM departments WHERE name = 'tecnología'), 3300, 33, true);

INSERT INTO tasks (title, done, owner_id) VALUES
    ('Preparar informe', false, 1),
    ('Revisar código',   true,  2),
    ('Entrevistar candidatos', false, 5),
    ('Actualizar servidor', false, 2),
    ('Tarea sin dueño',  false, NULL);
```

`INSERT INTO tabla (columnas) VALUES (…)`. Una subconsulta `(SELECT id FROM …)` evita escribir identificadores a mano.

## 6. Consultar (SELECT)

```sql
-- 03_queries.sql: consultas (SELECT) de menor a mayor complejidad
\echo '--- Filtrar y ordenar'
SELECT name, salary FROM employees WHERE active AND salary >= 2800 ORDER BY salary DESC;

\echo '--- Agregados'
SELECT count(*) AS total, sum(salary) AS payroll, round(avg(salary), 2) AS average,
       min(salary) AS lowest, max(salary) AS highest
FROM employees;

\echo '--- Agrupar con GROUP BY y filtrar grupos con HAVING'
SELECT d.name AS department, count(*) AS employees, sum(e.salary) AS payroll, round(avg(e.salary), 2) AS average
FROM employees e
JOIN departments d ON d.id = e.department_id
GROUP BY d.name
HAVING count(*) >= 1
ORDER BY payroll DESC;

\echo '--- LEFT JOIN: todas las tareas, tengan dueño o no'
SELECT t.title, t.done, coalesce(e.name, '(sin dueño)') AS owner
FROM tasks t
LEFT JOIN employees e ON e.id = t.owner_id
ORDER BY t.id;

\echo '--- Subconsulta: empleados que cobran más que la media'
SELECT name, salary FROM employees WHERE salary > (SELECT avg(salary) FROM employees) ORDER BY salary DESC;

\echo '--- CTE (WITH) y función de ventana: ranking de salario dentro de cada departamento'
WITH ranked AS (
    SELECT e.name, d.name AS department, e.salary,
           rank() OVER (PARTITION BY d.name ORDER BY e.salary DESC) AS position
    FROM employees e JOIN departments d ON d.id = e.department_id
)
SELECT * FROM ranked WHERE position = 1 ORDER BY department;

\echo '--- Paginación'
SELECT id, name FROM employees ORDER BY id LIMIT 2 OFFSET 2;

\echo '--- Búsqueda de texto'
SELECT title FROM tasks WHERE title ILIKE '%servidor%';

\echo '--- Tareas pendientes por empleado (con EXISTS)'
SELECT name FROM employees e
WHERE EXISTS (SELECT 1 FROM tasks t WHERE t.owner_id = e.id AND NOT t.done);

\echo '--- JSON desde SQL'
SELECT json_agg(json_build_object('id', id, 'title', title, 'done', done) ORDER BY id) AS tasks_json FROM tasks WHERE owner_id = 2;
```

Resultado de las consultas con los datos de ejemplo (las mismas cifras que en los ejercicios de [lógica y programación](#ejercicios-basicos)):

| Consulta | Resultado |
|---|---|
| Masa salarial total | `18300`; media `3050.00` |
| Por departamento | tecnología `3 / 11000 / 3666.67`, ventas `2 / 4500 / 2250.00`, recursos `1 / 2800 / 2800.00` |
| Mejor pagado de cada departamento | Sara, Marta, Ana |
| Por encima de la media | Marta (4100), Luis (3600), Iván (3300) |

### El orden en que SQL piensa

Se escribe `SELECT … FROM … WHERE … GROUP BY … HAVING … ORDER BY … LIMIT`, pero se **evalúa** en este orden:

```text
FROM / JOIN  ->  WHERE  ->  GROUP BY  ->  HAVING  ->  SELECT  ->  ORDER BY  ->  LIMIT / OFFSET
```

Por eso `WHERE` filtra filas **antes** de agrupar y `HAVING` filtra los **grupos** ya calculados.

### Tipos de JOIN

| JOIN | Resultado |
|---|---|
| `INNER JOIN` (o `JOIN`) | Solo filas con pareja en ambas tablas |
| `LEFT JOIN` | Todas las de la izquierda, con `NULL` si no hay pareja |
| `RIGHT JOIN` | Todas las de la derecha |
| `FULL JOIN` | Todas las de ambas |
| `CROSS JOIN` | Producto cartesiano (cada fila con cada fila) |

### `NULL`: ni cero ni vacío

`NULL` significa «desconocido». `x = NULL` **nunca** es verdadero: usa `x IS NULL` / `IS NOT NULL`. `coalesce(x, valor)` da un valor alternativo. Cualquier operación con `NULL` da `NULL`.

### Equivalencias con lo que ya sabes

| Idea | JavaScript / LINQ | SQL |
|---|---|---|
| Filtrar | `filter` / `Where` | `WHERE` |
| Transformar | `map` / `Select` | Lista del `SELECT` |
| Ordenar | `toSorted` / `OrderBy` | `ORDER BY` |
| Agrupar y sumar | `groupBy` + `reduce` | `GROUP BY` + `sum()` |
| Primeros N | `slice(0, n)` / `Take` | `LIMIT n` |
| Unir colecciones | `find` por id | `JOIN` |

## 7. Modificar datos, transacciones y restricciones

```sql
-- 04_changes.sql: UPDATE, DELETE y transacciones
\echo '--- UPDATE con RETURNING: subir 10 % a ventas'
UPDATE employees SET salary = salary * 11 / 10
WHERE department_id = (SELECT id FROM departments WHERE name = 'ventas')
RETURNING name, salary;

\echo '--- Transacción: todo o nada'
BEGIN;
UPDATE employees SET salary = salary - 100 WHERE name = 'Luis';
UPDATE employees SET salary = salary + 100 WHERE name = 'Iván';
COMMIT;
SELECT name, salary FROM employees WHERE name IN ('Luis', 'Iván') ORDER BY name;

\echo '--- ROLLBACK deshace los cambios'
BEGIN;
DELETE FROM tasks;
SELECT count(*) AS tasks_inside_transaction FROM tasks;
ROLLBACK;
SELECT count(*) AS tasks_after_rollback FROM tasks;

\echo '--- Restricción en acción: el CHECK impide datos inválidos'
\set ON_ERROR_STOP off
INSERT INTO employees (name, department_id, salary, age) VALUES ('Error', 1, -5, 30);
INSERT INTO tasks (title) VALUES ('x');
DELETE FROM departments WHERE name = 'ventas';
```

| Concepto | Significado |
|---|---|
| `UPDATE … SET … WHERE …` | **Siempre con `WHERE`**: sin él modifica todas las filas |
| `DELETE FROM … WHERE …` | Lo mismo para borrar |
| `RETURNING` | Devuelve las filas afectadas (propio de PostgreSQL) |
| `BEGIN` … `COMMIT` | Transacción: o se aplican todos los cambios o ninguno |
| `ROLLBACK` | Deshace la transacción |
| **ACID** | Atomicidad, Consistencia, Aislamiento y Durabilidad: las garantías de una transacción |

> **Peligro:** `DELETE FROM empleados;` y `UPDATE empleados SET salary = 0;` sin `WHERE` afectan a **toda** la tabla. Cuando trabajes con datos reales, empieza con `BEGIN;`, comprueba el resultado con `SELECT` y solo entonces `COMMIT;`.

Las restricciones (`CHECK`, `NOT NULL`, claves foráneas) hacen que la base de datos **rechace datos inválidos** aunque tu aplicación tenga un error. Es tu última línea de defensa.

## 8. Índices y rendimiento

```sql
EXPLAIN ANALYZE SELECT * FROM employees WHERE department_id = 2;
CREATE INDEX idx_employees_name ON employees (lower(name));
```

| Pieza | Significado |
|---|---|
| `EXPLAIN` | Muestra el plan de ejecución sin ejecutar |
| `EXPLAIN ANALYZE` | Lo ejecuta y muestra tiempos reales |
| `Seq Scan` | Recorre toda la tabla (lento en tablas grandes) |
| `Index Scan` | Usa un índice |

Los índices aceleran lecturas pero ralentizan escrituras y ocupan espacio. Crea índices para las columnas de `WHERE`, `JOIN` y `ORDER BY` que realmente uses, y mide.

## 9. Migraciones: cambiar el esquema con control

Una **migración** es un cambio de esquema versionado y repetible. Nunca se edita la base de producción «a mano».

```sql
-- Migración V2: añade prioridad a las tareas (un cambio de esquema versionado)
ALTER TABLE tasks ADD COLUMN priority smallint NOT NULL DEFAULT 2 CHECK (priority BETWEEN 1 AND 3);
CREATE INDEX idx_tasks_priority ON tasks (priority);
```

```text
db/migrations/
  V1__create_tables.sql
  V2__add_priority.sql
  V3__...
```

| Herramienta | Entorno |
|---|---|
| **Flyway**, **Liquibase** | Java / Spring Boot (se integran solas) |
| **node-pg-migrate**, **Prisma Migrate**, **Drizzle Kit** | Node.js |
| **Alembic** | Python (SQLAlchemy) |
| **EF Core Migrations** (`dotnet ef migrations add`) | C# |
| **golang-migrate**, **goose** | Go |

Reglas: cada migración se aplica **una sola vez** (la herramienta lo registra en una tabla), no se modifican las ya aplicadas (se crea otra), y se prueban sobre una copia de los datos antes de producción.

## 10. Usar SQL desde una aplicación

```bash
pnpm add pg
```

```js
// query.js: consultas parametrizadas con el cliente oficial de PostgreSQL para Node
import pg from 'pg';

const pool = new pg.Pool({
  connectionString: process.env.DATABASE_URL, // postgres://usuario:clave@host:5432/base
});

// Los valores van SEPARADOS de la consulta ($1, $2…): la base de datos nunca los interpreta como SQL
async function employeesBySalary(minimum) {
  const { rows } = await pool.query(
    'SELECT name, salary FROM employees WHERE salary >= $1 ORDER BY salary DESC',
    [minimum],
  );
  return rows;
}

// Una transacción con un cliente del pool: todo o nada
async function transfer(fromName, toName, amount) {
  const client = await pool.connect();
  try {
    await client.query('BEGIN');
    await client.query('UPDATE employees SET salary = salary - $1 WHERE name = $2', [amount, fromName]);
    await client.query('UPDATE employees SET salary = salary + $1 WHERE name = $2', [amount, toName]);
    await client.query('COMMIT');
  } catch (error) {
    await client.query('ROLLBACK');
    throw error;
  } finally {
    client.release(); // devuelve la conexión al pool
  }
}

console.log(await employeesBySalary(3300));

// Entrada maliciosa: con parámetros es solo un texto sin efecto
const { rows } = await pool.query('SELECT count(*) AS total FROM employees WHERE name = $1', ["'; DROP TABLE employees; --"]);
console.log(rows[0]);

await transfer('Luis', 'Iván', 50);
console.log((await pool.query("SELECT name, salary FROM employees WHERE name IN ('Luis','Iván') ORDER BY name")).rows);
await pool.end();
```

> **Peligro: inyección SQL.** Nunca construyas una consulta concatenando texto del usuario (`"… WHERE name = '" + name + "'"`). Usa **parámetros** (`$1`, `$2`); la base de datos los trata como datos, nunca como SQL. En el ejemplo, un nombre malicioso es simplemente un texto que no encuentra a nadie.

| Pieza | Significado |
|---|---|
| `new pg.Pool({ connectionString })` | Pool de conexiones: se crea **una vez** |
| `DATABASE_URL` | `postgres://usuario:clave@host:5432/base`; es un **secreto**: variable de entorno, no código |
| `pool.query(sql, [valores])` | Consulta parametrizada |
| `pool.connect()` / `client.release()` | Tomar y devolver una conexión para una transacción |
| `count(*)` devuelve texto | PostgreSQL devuelve `bigint` y `pg` lo entrega como cadena (`'0'`); conviértelo con `Number()` |

Otras tecnologías: Python (`psycopg`, SQLAlchemy), Java (JDBC, Spring Data JPA), C# (Npgsql, EF Core), Go (`pgx`), Rust (`sqlx`).

## 11. Copias de seguridad

```bash
docker exec pg-lab pg_dump -U postgres -d lib_demo -Fc > lib_demo.dump
docker exec -i pg-lab pg_restore -U postgres -d lib_demo --clean --if-exists < lib_demo.dump
```

| Flag | Significado |
|---|---|
| `pg_dump -Fc` | Copia en formato personalizado (comprimido, restaurable selectivamente) |
| `pg_restore --clean --if-exists` | Borra los objetos existentes antes de recrearlos, sin fallar si no existen |
| `docker exec -i` | Mantiene stdin abierto (necesario para el `<`) |
| `pg_dumpall` | Copia de todo el servidor (roles incluidos) |

Una copia que nunca has restaurado no es una copia: **prueba la restauración**.

## 12. Seguridad básica

- No uses el superusuario `postgres` desde la aplicación: crea un rol con permisos mínimos (`CREATE ROLE app LOGIN PASSWORD '…'; GRANT … ON … TO app;`).
- No publiques el puerto 5432 a Internet.
- Contraseñas siempre en variables de entorno o gestor de secretos.
- Las contraseñas de **tus usuarios** nunca se guardan en claro: hash lento con sal ([Criptografía](#cripto)).

## 13. Errores habituales

| Mensaje | Causa | Solución |
|---|---|---|
| `relation "x" does not exist` | Tabla inexistente o base equivocada | `\dt`, comprueba la base y el esquema |
| `column "x" does not exist` | Nombre mal escrito (PostgreSQL pasa a minúsculas lo que no lleva comillas) | `\d tabla` |
| `column "x" must appear in the GROUP BY clause` | Seleccionas una columna no agrupada | Añádela al `GROUP BY` o agrégala con `sum`, `max`… |
| `violates foreign key constraint` | Referencia a una fila inexistente o borras una referenciada | Ordena las operaciones o usa `ON DELETE` |
| `violates not-null constraint` / `violates check constraint` | Datos inválidos | Corrige el dato |
| `duplicate key value violates unique constraint` | Valor repetido | Gestiona el error (`409` en una API) |
| `password authentication failed` | Contraseña incorrecta | Revisa `POSTGRES_PASSWORD` y que el volumen no sea antiguo |
| `connection refused` | El servidor no está en marcha o el puerto es otro | `docker ps`, `ss -ltn` |
| `syntax error at or near` | Falta una coma, comilla o palabra | Lee la posición que indica el error |
| Resultados duplicados en un `JOIN` | La relación es 1:N | `DISTINCT` o agrega con `GROUP BY` |

## 14. Ejercicios

> **Ejercicio:** escribe las consultas para: los empleados de tecnología ordenados por edad; el salario medio de los activos; el departamento con la mayor masa salarial; y los empleados sin ninguna tarea.

> **Ejercicio:** añade la tabla `comments (id, task_id, text, created_at)` con clave foránea a `tasks` y `ON DELETE CASCADE`. Escribe la migración `V3`, aplícala y prueba que al borrar una tarea desaparecen sus comentarios.

> **Ejercicio:** escribe una transacción que mueva una tarea de un empleado a otro y falle (con `ROLLBACK`) si el empleado destino está inactivo.

> **Ejercicio:** compara con `EXPLAIN ANALYZE` una búsqueda sobre 100.000 filas (`generate_series`) con y sin índice.

> **Ejercicio:** conecta la API de [Node.js](#node) a PostgreSQL con un repositorio basado en `pg` que cumpla el mismo contrato que el repositorio en memoria.

## 15. Documentación oficial

- [PostgreSQL: documentación](https://www.postgresql.org/docs/current/)
- [Tutorial de SQL de PostgreSQL](https://www.postgresql.org/docs/current/tutorial.html)
- [Imagen oficial de Docker](https://hub.docker.com/_/postgres)
- [node-postgres](https://node-postgres.com/)
- [Flyway](https://documentation.red-gate.com/fd)
