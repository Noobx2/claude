# Python (con uv)

Python es un lenguaje de sintaxis clara, muy usado en automatización, ciencia de datos, scripts, APIs web y control de hardware. Se ejecuta con un intérprete (no hay compilación previa) y la **sangría** define los bloques.

*Verificado: 2026-10-27. Todos los ejemplos se han ejecutado con Python 3.13 y uv 0.11 (tu equipo: Python 3.14.4 y uv 0.13.5; la sintaxis es la misma). Referencia: [docs.python.org](https://docs.python.org/3/) y [docs.astral.sh/uv](https://docs.astral.sh/uv/).*

## 1. Para qué sirve

| Uso | Ejemplo |
|---|---|
| Scripts y automatización | Renombrar archivos, procesar CSV, copias de seguridad |
| APIs web | FastAPI, Flask, Django |
| Datos y ciencia | pandas, NumPy, Jupyter |
| Hardware y redes | `pyserial`, `scapy`, sockets (ver [Hardware](#hardware) y [Redes](#redes)) |
| Aprendizaje | Es un lenguaje excelente para empezar |

## 2. Instalación, versiones y ejecución

Tu equipo ya tiene `python3` (3.14.4) y `uv`. No existe el comando `python` ni `pip` del sistema, y **no hace falta** instalarlos: `uv` crea un entorno aislado por proyecto.

```bash
python3 --version
uv --version
python3 hello.py
uv run hello.py
```

| Comando | Significado |
|---|---|
| `python3 archivo.py` | Ejecuta con el Python del sistema |
| `python3` | Abre el REPL interactivo (se sale con Ctrl+D) |
| `python3 -m modulo` | Ejecuta un módulo como programa (`-m` = *module*) |
| `python3 -c "print(1+1)"` | Ejecuta un fragmento (`-c` = *command*) |
| `uv run archivo.py` | Ejecuta con el entorno del proyecto, creándolo si hace falta |
| `uv python list` | Intérpretes disponibles |

> **Regla:** no instales paquetes en el Python del sistema (Ubuntu lo protege). Crea siempre un proyecto con `uv`.

## 3. Archivos y convenciones de nombres

| Elemento | Convención |
|---|---|
| Archivos y módulos | `snake_case.py` (sin guiones: no se podrían importar) |
| Variables y funciones | `snake_case` |
| Clases | `PascalCase` |
| Constantes | `UPPER_SNAKE_CASE` |
| «Privado» | Prefijo `_nombre` (convención, no se impone) |
| Carpetas con `__init__.py` | Paquetes |
| Sangría | 4 espacios (nunca mezclar con tabuladores) |
| Docstrings | `"""Texto"""` justo bajo la definición |

## 4. Hola mundo

```python
print("Hola mundo")
```

## 5. Variables, tipos y conversiones

```python
# basics.py: variables, tipos, condiciones, bucles y funciones
name = "Ana"            # str
age = 31                # int
height = 1.68           # float
is_student = False      # bool
nothing = None          # ausencia de valor

MAX_TRIES = 3           # constante (solo por convención: Python no impide cambiarla)

print(type(age), type(height), type(name), type(nothing))
print(int("42") + 1, float("3.5"), str(7) + "!", bool(""), bool("0"))

# f-strings: texto con valores incrustados
print(f"{name} tiene {age} años y mide {height:.1f} m")

# Condiciones: la sangría (4 espacios) define el bloque
score = 72
if score >= 90:
    grade = "A"
elif score >= 70:
    grade = "B"
else:
    grade = "C"
print(grade)

label = "hecha" if is_student else "pendiente"   # operador condicional
print(label)

# Bucles
for i in range(1, 4):                 # 1, 2, 3 (el final no se incluye)
    print("vuelta", i)

for index, letter in enumerate("abc"):
    print(index, letter)

count = 3
while count > 0:
    count -= 1
print(count)


def add(a: int, b: int = 0) -> int:
    """Suma dos números (el segundo es opcional)."""
    return a + b


def stats(*numbers: float) -> dict[str, float]:
    """*numbers recoge cualquier cantidad de argumentos posicionales."""
    return {"min": min(numbers), "max": max(numbers), "avg": sum(numbers) / len(numbers)}


print(add(2, 3), add(5), add(b=1, a=2))   # argumentos por posición y por nombre
print(stats(3, 1, 4, 1, 5))

square = lambda x: x * x                     # función anónima
print(list(map(square, [1, 2, 3])))
```

| Tipo | Ejemplo | Notas |
|---|---|---|
| `int` | `42` | Tamaño ilimitado |
| `float` | `3.14` | Coma flotante: `0.1 + 0.2 != 0.3` |
| `str` | `"texto"` | Inmutable; comillas simples o dobles |
| `bool` | `True`, `False` | Con mayúscula |
| `None` | `None` | Ausencia de valor |
| `list` | `[1, 2, 3]` | Mutable y ordenada |
| `tuple` | `(1, 2)` | Inmutable |
| `dict` | `{"a": 1}` | Clave → valor, mantiene el orden de inserción |
| `set` | `{1, 2}` | Sin repetidos |

**Falsy** (cuenta como falso): `False`, `None`, `0`, `""`, `[]`, `{}`, `set()`. Todo lo demás es verdadero (incluido `"0"`).

Operadores: `//` división entera, `%` resto, `**` potencia, `and` / `or` / `not` (no `&&`), `is` identidad (`x is None`), `==` igualdad, `in` pertenencia.

## 6. Colecciones: recorrer, filtrar, transformar, ordenar y agrupar

```python
# collections_demo.py: listas, diccionarios, conjuntos y su transformación
from collections import Counter, defaultdict
from functools import reduce

employees = [
    {"id": 1, "name": "Ana",   "department": "ventas",     "salary": 2400, "age": 31, "active": True},
    {"id": 2, "name": "Luis",  "department": "tecnología", "salary": 3600, "age": 28, "active": True},
    {"id": 3, "name": "Marta", "department": "tecnología", "salary": 4100, "age": 41, "active": False},
    {"id": 4, "name": "Pablo", "department": "ventas",     "salary": 2100, "age": 24, "active": True},
    {"id": 5, "name": "Sara",  "department": "recursos",   "salary": 2800, "age": 36, "active": True},
    {"id": 6, "name": "Iván",  "department": "tecnología", "salary": 3300, "age": 33, "active": True},
]

# Comprensiones de lista: [expresión for elemento in colección if condición]
active_names = [e["name"] for e in employees if e["active"]]
print(active_names)

total = sum(e["salary"] for e in employees)          # expresión generadora: no crea lista
print(total, total / len(employees))

# Ordenar sin modificar el original: sorted devuelve una lista nueva
top3 = sorted(employees, key=lambda e: e["salary"], reverse=True)[:3]
print([e["name"] for e in top3])

# Primer elemento que cumple (o None)
first_over_40 = next((e["name"] for e in employees if e["age"] > 40), None)
print(first_over_40, all(e["salary"] > 2000 for e in employees), any(not e["active"] for e in employees))

print(", ".join(e["name"] for e in employees))

# Agrupar con defaultdict
by_department = defaultdict(list)
for e in employees:
    by_department[e["department"]].append(e)
payroll = {dept: sum(e["salary"] for e in group) for dept, group in by_department.items()}
print(payroll)

# Contar con Counter
numbers = [3, 1, 4, 1, 5, 9, 2, 6, 5, 3, 5]
print(Counter(numbers).most_common(2), sorted(set(numbers)))

# Diccionarios: recorrido y acceso seguro
ages = {"Ana": 31, "Luis": 28}
ages["Sara"] = 36
for person, years in ages.items():
    print(person, years)
print(ages.get("Pedro", "desconocido"), "Ana" in ages)

# reduce existe, pero en Python suele preferirse sum/max/min o un bucle
longest = reduce(lambda a, b: a if len(a) >= len(b) else b, ["uno", "tres", "cuatro"])
print(longest)

# Desempaquetado
first, *rest = numbers
print(first, rest[:3])
matrix = [[1, 2, 3], [4, 5, 6]]
print([list(row) for row in zip(*matrix)])   # traspuesta
```

| Operación | Python |
|---|---|
| Transformar | `[f(x) for x in xs]` |
| Filtrar | `[x for x in xs if cond(x)]` |
| Sumar / mínimo / máximo | `sum(xs)`, `min(xs)`, `max(xs)` |
| Alguno / todos | `any(…)`, `all(…)` |
| Ordenar | `sorted(xs, key=…, reverse=True)` (no modifica el original; `xs.sort()` sí) |
| Primero que cumple | `next((x for x in xs if cond(x)), None)` |
| Agrupar | `defaultdict(list)` o `itertools.groupby` (sobre datos ordenados) |
| Contar | `collections.Counter` |
| Quitar repetidos | `set(xs)` o `dict.fromkeys(xs)` (conserva el orden) |

> **Ojo:** una función con valor por defecto mutable (`def f(items=[])`) comparte esa lista entre llamadas. Usa `None` y crea la lista dentro.

## 7. JSON, archivos y errores

```python
# json_files.py: JSON, archivos y errores
import json
from pathlib import Path

task = {"id": 1, "title": "Estudiar Python", "done": False, "tags": ["estudio", "código"]}

text = json.dumps(task, ensure_ascii=False, indent=2)   # dict -> texto JSON
print(text)
data = json.loads(text)                                   # texto JSON -> dict
print(data["tags"][1])

path = Path("tasks.json")
path.write_text(json.dumps([task], ensure_ascii=False, indent=2), encoding="utf-8")

# "with" cierra el archivo aunque haya un error
with path.open(encoding="utf-8") as file:
    loaded = json.load(file)
print(len(loaded), path.exists(), path.suffix)

path.unlink()                                             # borra el archivo


def read_tasks(file: Path) -> list[dict]:
    try:
        return json.loads(file.read_text(encoding="utf-8"))
    except FileNotFoundError:
        return []                                         # sin archivo: lista vacía
    except json.JSONDecodeError as error:
        raise ValueError(f"JSON inválido en {file}: {error.msg}") from error
    finally:
        pass                                              # se ejecuta siempre


print(read_tasks(Path("no-existe.json")))
try:
    int("abc")
except ValueError as error:
    print("Error:", error)
```

| Función | Para qué |
|---|---|
| `json.dumps` / `json.loads` | Objeto ⇄ **texto** JSON |
| `json.dump` / `json.load` | Objeto ⇄ **archivo** JSON |
| `ensure_ascii=False` | Conserva tildes y ñ en la salida |
| `indent=2` | Salida legible |
| `Path.read_text` / `write_text` | Leer y escribir un archivo de texto entero |
| `with open(…) as f` | Gestor de contexto: cierra el archivo siempre |

Jerarquía básica de excepciones: `Exception` → `ValueError`, `TypeError`, `KeyError`, `FileNotFoundError`, … Captura la más **específica** posible y nunca un `except:` vacío.

## 8. Clases y programación orientada a objetos

```python
# classes_demo.py: clases, dataclasses y tipado
from dataclasses import dataclass, field


class InsufficientFundsError(Exception):
    """Excepción propia: el saldo no alcanza."""


class Account:
    def __init__(self, owner: str, balance: float = 0) -> None:
        self.owner = owner
        self._balance = balance           # el guion bajo indica "uso interno" (convención)

    @property
    def balance(self) -> float:
        return self._balance

    def deposit(self, amount: float) -> None:
        if amount <= 0:
            raise ValueError("El importe debe ser positivo")
        self._balance += amount

    def withdraw(self, amount: float) -> None:
        if amount > self._balance:
            raise InsufficientFundsError(f"Saldo insuficiente: {self._balance}")
        self._balance -= amount

    def __str__(self) -> str:
        return f"{self.owner}: {self._balance:.2f} €"


class SavingsAccount(Account):             # herencia
    def __init__(self, owner: str, balance: float = 0, rate: float = 0.02) -> None:
        super().__init__(owner, balance)
        self.rate = rate

    def add_interest(self) -> None:
        self.deposit(self._balance * self.rate)


account = SavingsAccount("Ana", 1000)
account.deposit(500)
account.add_interest()
print(account)
try:
    account.withdraw(5000)
except InsufficientFundsError as error:
    print("No se pudo:", error)


# dataclass: clase para guardar datos, con __init__, __repr__ e __eq__ generados
@dataclass
class Task:
    title: str
    done: bool = False
    tags: list[str] = field(default_factory=list)   # nunca uses [] como valor por defecto


t = Task("Estudiar")
t.tags.append("python")
print(t, t == Task("Estudiar", False, ["python"]))
```

- `self` es el objeto actual y va como primer parámetro de cada método.
- `__init__` es el constructor; `__str__` define cómo se imprime.
- `@property` expone un método como si fuera un atributo.
- `@dataclass` evita escribir el constructor en clases que solo guardan datos.
- Python no tiene `private` real: `_nombre` es una convención.

## 9. Módulos, paquetes e imports

```python
import json                              # el módulo entero: json.dumps(...)
from pathlib import Path                 # un nombre concreto
from app.models import Task as TaskDTO  # con alias
```

```text
mi_proyecto/
  app/
    __init__.py       (puede estar vacío: marca la carpeta como paquete)
    models.py
    service.py
  tests/
```

El código que solo debe ejecutarse al lanzar el archivo (no al importarlo) va bajo:

```python
if __name__ == "__main__":
    main()
```

## 10. Asincronía y concurrencia

```python
# async_demo.py: asincronía con asyncio
import asyncio
import time


async def fetch(name: str, seconds: float) -> str:
    await asyncio.sleep(seconds)          # cede el control mientras espera
    return f"{name} listo"


async def main() -> None:
    start = time.perf_counter()
    results = await asyncio.gather(fetch("a", 0.2), fetch("b", 0.2), fetch("c", 0.2))
    print(results, f"{time.perf_counter() - start:.1f}s")   # ~0.2 s, no 0.6 s


asyncio.run(main())
```

| Modelo | Cuándo |
|---|---|
| `asyncio` (`async`/`await`) | Muchas esperas de red o disco (APIs, sockets) |
| `threading` | Esperas de E/S con código bloqueante; el GIL impide paralelismo real de cálculo |
| `multiprocessing` | Cálculo intensivo repartido en varios procesos |

## 11. Entrada y salida por consola

```python
name = input("¿Cómo te llamas? ")
print(f"Hola {name}")

import sys
for line in sys.stdin:           # lee líneas de una tubería
    print(line.strip().upper())
print("error", file=sys.stderr)  # a stderr
```

Argumentos de línea de comandos con `argparse`: ver [CLI, streams y binarios](#cli-args-streams).

## 12. Gestor de dependencias: `uv` y `pyproject.toml`

```bash
cd ~/projects
uv init tasks-py
cd tasks-py
uv add fastapi uvicorn
uv add --dev pytest httpx ruff
uv run pytest
uv sync
uv lock
```

| Comando | Significado |
|---|---|
| `uv init nombre` | Crea la carpeta con `pyproject.toml`, `.python-version`, `.gitignore`, `README.md`, `main.py` y un repositorio Git |
| `uv add paquete` | Instala y apunta en `dependencies` (crea `.venv` y `uv.lock`) |
| `uv add --dev paquete` | Dependencia solo de desarrollo (grupo `dev`) |
| `uv remove paquete` | Quita una dependencia |
| `uv sync` | Deja `.venv` exactamente como dicen `pyproject.toml` y `uv.lock` |
| `uv lock` | Actualiza `uv.lock` |
| `uv run comando` | Ejecuta un comando dentro del entorno del proyecto |
| `uv tree` | Árbol de dependencias |
| `uv python install 3.14` | Instala un intérprete concreto |

| Archivo | Qué es | ¿Se edita? | ¿Git? |
|---|---|---|---|
| `pyproject.toml` | **Manifiesto**: nombre, versión de Python, dependencias, configuración de herramientas | Sí (o con `uv add`) | Sí |
| `uv.lock` | **Bloqueo**: versión exacta de cada paquete | No | Sí |
| `.python-version` | Versión de Python del proyecto | Raramente | Sí |
| `.venv/` | **Entorno virtual**: copia aislada de Python y paquetes | No | **No** |

Un **entorno virtual** es una carpeta con un Python y paquetes propios del proyecto: evita que dos proyectos con versiones distintas de la misma librería se pisen. `uv` lo gestiona solo; no necesitas activarlo (`uv run` lo usa).

## 13. Una API REST con FastAPI

FastAPI valida con tipos de Python y genera documentación automática. La estructura sigue las mismas capas que la API de [Node.js](#node).

```text
tasks-py/
  pyproject.toml
  uv.lock
  app/
    __init__.py
    models.py        DTOs y validación (Pydantic)
    repository.py    datos (en memoria; luego una base de datos)
    service.py       reglas de negocio
    main.py          rutas HTTP y manejo de errores
  tests/
    test_api.py
```

```python
from pydantic import BaseModel, Field


class TaskCreate(BaseModel):
    """Datos que envía el cliente para crear una tarea (DTO de entrada)."""

    title: str = Field(min_length=2)


class TaskUpdate(BaseModel):
    """Cambios parciales: solo se aplican los campos que vengan informados."""

    title: str | None = Field(default=None, min_length=2)
    done: bool | None = None


class Task(BaseModel):
    """Lo que devuelve la API (DTO de salida)."""

    id: str
    title: str
    done: bool = False
```

```python
from app.models import Task


class TaskRepository:
    """Repositorio en memoria. Más adelante se sustituye por una base de datos."""

    def __init__(self) -> None:
        self._tasks: dict[str, Task] = {}
        self._next_id = 1

    def find_all(self) -> list[Task]:
        return list(self._tasks.values())

    def find_by_id(self, task_id: str) -> Task | None:
        return self._tasks.get(task_id)

    def create(self, title: str) -> Task:
        task = Task(id=str(self._next_id), title=title)
        self._tasks[task.id] = task
        self._next_id += 1
        return task

    def update(self, task_id: str, changes: dict) -> Task | None:
        current = self._tasks.get(task_id)
        if current is None:
            return None
        updated = current.model_copy(update=changes)
        self._tasks[task_id] = updated
        return updated

    def remove(self, task_id: str) -> bool:
        return self._tasks.pop(task_id, None) is not None
```

```python
from app.models import Task, TaskCreate, TaskUpdate
from app.repository import TaskRepository


class NotFoundError(Exception):
    """El recurso pedido no existe."""


class TaskService:
    """Reglas de negocio. No conoce HTTP."""

    def __init__(self, repository: TaskRepository) -> None:
        self._repository = repository

    def list(self) -> list[Task]:
        return self._repository.find_all()

    def get(self, task_id: str) -> Task:
        task = self._repository.find_by_id(task_id)
        if task is None:
            raise NotFoundError(f"No existe la tarea {task_id}")
        return task

    def create(self, data: TaskCreate) -> Task:
        return self._repository.create(data.title.strip())

    def update(self, task_id: str, data: TaskUpdate) -> Task:
        changes = data.model_dump(exclude_none=True)
        task = self._repository.update(task_id, changes)
        if task is None:
            raise NotFoundError(f"No existe la tarea {task_id}")
        return task

    def remove(self, task_id: str) -> None:
        if not self._repository.remove(task_id):
            raise NotFoundError(f"No existe la tarea {task_id}")
```

```python
from fastapi import FastAPI, Request, status
from fastapi.middleware.cors import CORSMiddleware
from fastapi.responses import JSONResponse

from app.models import Task, TaskCreate, TaskUpdate
from app.repository import TaskRepository
from app.service import NotFoundError, TaskService


def create_app() -> FastAPI:
    app = FastAPI(title="Tasks API")
    service = TaskService(TaskRepository())

    app.add_middleware(
        CORSMiddleware,
        allow_origins=["http://localhost:5173"],
        allow_methods=["GET", "POST", "PATCH", "DELETE"],
        allow_headers=["Content-Type"],
    )

    # Un único sitio donde las excepciones del negocio se convierten en respuestas HTTP
    @app.exception_handler(NotFoundError)
    async def not_found_handler(_: Request, error: NotFoundError) -> JSONResponse:
        return JSONResponse(status_code=404, content={"error": str(error)})

    @app.get("/api/tasks", response_model=list[Task])
    def list_tasks() -> list[Task]:
        return service.list()

    @app.get("/api/tasks/{task_id}", response_model=Task)
    def get_task(task_id: str) -> Task:
        return service.get(task_id)

    @app.post("/api/tasks", response_model=Task, status_code=status.HTTP_201_CREATED)
    def create_task(data: TaskCreate) -> Task:
        return service.create(data)

    @app.patch("/api/tasks/{task_id}", response_model=Task)
    def update_task(task_id: str, data: TaskUpdate) -> Task:
        return service.update(task_id, data)

    @app.delete("/api/tasks/{task_id}", status_code=status.HTTP_204_NO_CONTENT)
    def delete_task(task_id: str) -> None:
        service.remove(task_id)

    return app


app = create_app()
```

| Pieza | Significado |
|---|---|
| `class TaskCreate(BaseModel)` | Modelo Pydantic: valida y convierte el JSON entrante. Si no cumple, FastAPI responde `422` solo |
| `Field(min_length=2)` | Regla de validación |
| `response_model=Task` | Forma de la respuesta; filtra campos no deseados |
| `status_code=201` | Código de éxito para `POST` |
| `@app.exception_handler` | Convierte `NotFoundError` en `404` |
| `CORSMiddleware` | Permite al frontend (otro origen) llamar a la API; los métodos se enumeran |
| `create_app()` | Función fábrica: crea una aplicación nueva por prueba |

Arrancar y probar:

```bash
uv run uvicorn app.main:app --reload --port 3000
```

| Pieza | Significado |
|---|---|
| `uvicorn` | Servidor ASGI que ejecuta FastAPI |
| `app.main:app` | Módulo `app/main.py`, variable `app` |
| `--reload` | Reinicia al guardar (solo desarrollo) |
| `--port 3000` | Puerto |
| `--host 0.0.0.0` | Escuchar en todas las interfaces (por defecto solo `127.0.0.1`) |

Abre `http://localhost:3000/docs`: documentación interactiva generada automáticamente.

## 14. Pruebas, formato y análisis estático

```python
from fastapi.testclient import TestClient

from app.main import create_app


def test_crud_flow() -> None:
    client = TestClient(create_app())

    created = client.post("/api/tasks", json={"title": "Estudiar Python"})
    assert created.status_code == 201
    assert created.json() == {"id": "1", "title": "Estudiar Python", "done": False}

    assert len(client.get("/api/tasks").json()) == 1

    patched = client.patch("/api/tasks/1", json={"done": True})
    assert patched.json()["done"] is True

    assert client.post("/api/tasks", json={"title": "x"}).status_code == 422

    assert client.delete("/api/tasks/1").status_code == 204
    assert client.get("/api/tasks/1").status_code == 404
```

```toml
# pyproject.toml (parte de configuración de pytest)
[tool.pytest.ini_options]
pythonpath = ["."]
testpaths = ["tests"]
```

```bash
uv run pytest -q
uv run ruff check .
uv run ruff format .
```

| Comando | Significado |
|---|---|
| `pytest -q` | Ejecuta las pruebas en modo silencioso (`-v` detallado, `-k nombre` filtra, `-x` para en el primer fallo) |
| `ruff check .` | Analiza el código (errores y estilo); `--fix` corrige lo automático |
| `ruff format .` | Formatea el código |
| `assert condición` | Aserción de `pytest`: si es falsa, la prueba falla |

Depurar: `breakpoint()` abre el depurador en ese punto; también puntos de interrupción en VS Code.

## 15. Estructura recomendada para una aplicación real

```text
app/                 código de la aplicación (paquete)
tests/               pruebas
pyproject.toml
uv.lock
.env.example         variables de ejemplo
README.md
```

Configuración por variables de entorno: `os.environ.get("PORT", "3000")` o la librería `pydantic-settings`.

## 16. Crear, ejecutar, probar y empaquetar

| Paso | Comando |
|---|---|
| Crear | `uv init nombre` |
| Ejecutar | `uv run python -m app.main` o `uv run uvicorn …` |
| Probar | `uv run pytest` |
| Empaquetar una librería | `uv build` (genera `dist/` con `.whl` y `.tar.gz`) |
| Contenedor | Imagen `python:3.14-slim` + `uv sync --frozen` + `CMD ["uvicorn", …]` |

## 17. Errores habituales

| Mensaje | Causa | Solución |
|---|---|---|
| `Command 'python' not found` | Ubuntu solo trae `python3` | Usa `python3` o `uv run` |
| `No module named pip` | No hay pip en el sistema | Usa `uv add` en un proyecto |
| `ModuleNotFoundError: No module named 'app'` en pytest | Python no encuentra tu paquete | `pythonpath = ["."]` en `[tool.pytest.ini_options]`, o `uv run python -m pytest` |
| `IndentationError` | Mezcla de espacios y tabuladores o sangría incorrecta | 4 espacios |
| `ModuleNotFoundError: No module named 'fastapi'` | Ejecutas con otro Python (fuera del entorno) | `uv run …` |
| `externally-managed-environment` | Intentas `pip install` en el Python del sistema | Crea un proyecto con `uv` |
| `TypeError: 'NoneType' object is not subscriptable` | Una función devolvió `None` | Revisa el `return` |
| `KeyError: 'x'` | La clave no existe | `dic.get("x")` |
| `UnicodeDecodeError` | Archivo con otra codificación | `encoding="utf-8"` explícito |
| La lista cambia «sola» | Dos nombres apuntan al mismo objeto | `copy()` o `list(xs)` |

## 18. Ejercicios

> **Ejercicio:** resuelve los ejercicios de [lógica y programación](#ejercicios-basicos) 1–15 en Python y escribe pruebas con `pytest`.

> **Ejercicio:** reproduce el análisis de empleados (activos, masa salarial por departamento, mejor pagado) con comprensiones, `sum` y `defaultdict`.

> **Ejercicio:** añade a la API `GET /api/tasks?done=true`, con un parámetro de consulta tipado, y su prueba.

> **Ejercicio:** escribe un script con `argparse` que lea un CSV y muestre la suma de una columna indicada con `--column`.

## 19. Documentación oficial

- [Tutorial oficial de Python](https://docs.python.org/es/3/tutorial/)
- [Biblioteca estándar](https://docs.python.org/3/library/)
- [uv](https://docs.astral.sh/uv/)
- [FastAPI](https://fastapi.tiangolo.com/)
- [pytest](https://docs.pytest.org/)
- [Ruff](https://docs.astral.sh/ruff/)
