# Rust (con Cargo)

Rust es un lenguaje **compilado y estático** que garantiza seguridad de memoria **sin recolector de basura**. Su sistema de *propiedad y préstamos* elimina en tiempo de compilación errores clásicos (punteros colgantes, carreras de datos). Es rápido como C y se usa en herramientas, sistemas, servidores, WebAssembly y software que controla hardware.

*Verificado: 2026-10-27. Todos los ejemplos se han compilado y ejecutado con Rust 1.97 (edición 2024); tu equipo tiene 1.99.1 y la sintaxis es la misma. Referencia: [El libro de Rust](https://doc.rust-lang.org/book/) y [doc.rust-lang.org](https://doc.rust-lang.org/).*

> **Ojo:** Rust tiene la curva más pronunciada de tu ruta. Su compilador es muy estricto, pero sus mensajes de error son los mejores: léelos enteros, suelen proponer la solución.

## 1. Para qué sirve

| Uso | Ejemplo |
|---|---|
| Herramientas de línea de comandos | ripgrep, fd, bat |
| Servidores de alto rendimiento | axum, actix-web |
| Hardware y sistemas embebidos | Control de dispositivos con control fino de memoria |
| WebAssembly | Módulos rápidos para el navegador |
| Aplicaciones de escritorio | Tauri (fuera de tu prioridad por ahora) |

## 2. Instalación y versiones

Tu equipo ya tiene `rustup`, el toolchain `stable` activo, `rustc` y `cargo` 1.99.1.

```bash
rustc --version
cargo --version
rustup show
rustup update
rustup component add clippy rustfmt
```

| Comando | Significado |
|---|---|
| `rustc --version` | Versión del compilador |
| `cargo --version` | Versión del gestor de proyectos |
| `rustup show` | Toolchains, objetivos y componentes instalados |
| `rustup update` | Actualiza el toolchain estable |
| `rustup component add X` | Añade `clippy` (linter) o `rustfmt` (formateador) |
| `rustup target add X` | Añade un objetivo para compilar para otra plataforma |

## 3. Archivos y convenciones de nombres

| Elemento | Convención |
|---|---|
| Archivos y módulos | `snake_case.rs` |
| Funciones y variables | `snake_case` |
| Tipos, traits y enums | `PascalCase` |
| Constantes | `UPPER_SNAKE_CASE` |
| Crates | `kebab-case` en `Cargo.toml` (`mi-crate`), `snake_case` en el código |
| Formato | Único: `cargo fmt` |

## 4. Hola mundo

```bash
cd ~/projects
cargo new hello-rust
cd hello-rust
cargo run
```

```rust
// src/main.rs
fn main() {
    println!("Hola mundo");
}
```

| Pieza | Significado |
|---|---|
| `cargo new nombre` | Crea un proyecto binario (con repositorio Git). `--lib` crea una librería |
| `cargo run` | Compila y ejecuta |
| `fn main()` | Punto de entrada |
| `println!` | **Macro** (lleva `!`): imprime con formato; `{}` inserta un valor, `{:?}` formato de depuración |

Estructura generada:

```text
hello-rust/
  Cargo.toml      manifiesto
  Cargo.lock      bloqueo de versiones
  .gitignore      ignora target/
  src/main.rs
```

## 5. Variables, tipos y conversiones

```rust
// basics.rs: variables, tipos, condiciones, bucles y funciones
const MAX_TRIES: u32 = 3; // constante: tipo obligatorio

// Cada parámetro y el retorno llevan tipo. La última expresión SIN ; es el valor devuelto
fn add(a: i32, b: i32) -> i32 {
    a + b
}

fn grade(score: u32) -> &'static str {
    // if y match son expresiones: devuelven un valor
    match score {
        90..=100 => "A",
        70..=89 => "B",
        _ => "C", // _ = cualquier otro caso (match debe cubrirlos todos)
    }
}

fn main() {
    let name = "Ana"; // inmutable por defecto
    let mut count = 0; // mut: se puede modificar
    count += 1;
    let age: u8 = 31; // tipo explícito
    let height = 1.68_f64;
    let student = false;

    println!("{name} {age} {height} {student} {count} {MAX_TRIES}");
    println!("{name} mide {height:.1} m"); // {:.1}: un decimal

    // Sombreado (shadowing): una nueva variable con el mismo nombre
    let text = "42";
    let text: i32 = text.parse().unwrap(); // &str -> i32
    println!("{}", text + 1);

    // Conversiones: siempre explícitas con "as" o métodos
    let ratio = 7 as f64 / 2.0;
    println!("{ratio} {} {}", 7 / 2, 7 % 2);

    println!("{} {} {}", grade(95), grade(72), grade(40));
    println!("{}", add(2, 3));

    // Bucles
    for i in 1..=3 {
        println!("vuelta {i}"); // 1..=3 incluye el 3; 1..3 no
    }
    for (index, letter) in "abc".chars().enumerate() {
        println!("{index} {letter}");
    }
    let mut remaining = 3;
    while remaining > 0 {
        remaining -= 1;
    }
    let found = loop {
        remaining += 1;
        if remaining == 5 {
            break remaining * 2; // loop puede devolver un valor
        }
    };
    println!("{found}");

    // if let: atajo para un solo patrón
    let maybe: Option<i32> = Some(7);
    if let Some(value) = maybe {
        println!("tiene {value}");
    }

    // Tuplas y arrays
    let point = (3, 4);
    let (x, y) = point;
    let numbers = [1, 2, 3];
    println!("{x} {y} {} {}", numbers.len(), numbers[0]);
}
```

| Tipo | Ejemplo | Notas |
|---|---|---|
| `i8…i128`, `u8…u128`, `isize`, `usize` | `i32`, `u8` | Con o sin signo; `i32` por defecto; `usize` para índices |
| `f32`, `f64` | `1.5` | `f64` por defecto |
| `bool` | `true` | |
| `char` | `'ñ'` | Un valor Unicode (4 bytes) |
| `&str` / `String` | `"texto"` / `String::from("texto")` | `&str` es una vista prestada; `String` es dueño y crece |
| `(T, U)` | `(3, 4)` | Tupla |
| `[T; N]` | `[1, 2, 3]` | Array de tamaño fijo |
| `Vec<T>` | `vec![1, 2]` | Lista dinámica |
| `Option<T>` | `Some(1)` / `None` | Sustituye a `null` |
| `Result<T, E>` | `Ok(1)` / `Err(e)` | Sustituye a las excepciones |

> **Regla:** las variables son **inmutables** por defecto. Escribe `mut` solo cuando haga falta.

## 6. Propiedad y préstamos

```rust
// ownership.rs: propiedad y préstamos, la idea central de Rust

fn length(text: &String) -> usize {
    // &String: préstamo de solo lectura; no toma posesión
    text.len()
}

fn shout(text: &mut String) {
    // &mut: préstamo mutable; solo uno a la vez
    text.push_str("!");
}

fn consume(text: String) {
    // Toma posesión: al terminar la función, text se libera
    println!("consumido: {text}");
}

fn first_word(text: &str) -> &str {
    // &str: vista de texto (trozo prestado). Es el parámetro más flexible
    text.split_whitespace().next().unwrap_or("")
}

fn main() {
    let a = String::from("hola");
    let b = a; // MOVIMIENTO: a deja de ser válida
    // println!("{a}");  // error: value borrowed here after move
    println!("{b}");

    let c = b.clone(); // copia profunda explícita
    println!("{b} {c}");

    let mut message = String::from("hola");
    println!("{}", length(&message)); // se presta
    shout(&mut message); // se presta como mutable
    println!("{message}");

    consume(message); // se mueve a la función
    // println!("{message}"); // error: ya no es nuestra

    // Los tipos pequeños (enteros, bool, char) se COPIAN, no se mueven
    let x = 5;
    let y = x;
    println!("{x} {y}");

    println!("{}", first_word("Rust es genial"));

    // Reglas del préstamo: o varias referencias de lectura, o UNA mutable; nunca mezcladas
    let mut numbers = vec![1, 2, 3];
    let first = numbers[0]; // se copia el valor
    numbers.push(4); // correcto porque no hay una referencia viva a un elemento
    println!("{first} {numbers:?}");
}
```

Las tres reglas:

1. Cada valor tiene **un único dueño**.
2. Cuando el dueño sale del ámbito, el valor se libera (sin recolector).
3. Se puede **prestar** con referencias: o **varias de solo lectura** (`&T`), o **una sola mutable** (`&mut T`), nunca mezcladas.

Pasar un valor a otra variable o función lo **mueve** (el original deja de ser válido) salvo que el tipo sea `Copy` (números, `bool`, `char`) o que lo prestes con `&`.

## 7. Colecciones: recorrer, filtrar, transformar, ordenar y agrupar

```rust
// collections.rs: Vec, HashMap e iteradores con el ejemplo de empleados
use std::collections::{BTreeMap, HashMap, HashSet};

#[derive(Debug, Clone)]
struct Employee {
    name: &'static str,
    department: &'static str,
    salary: u32,
    age: u32,
    active: bool,
}

fn main() {
    let employees = vec![
        Employee {
            name: "Ana",
            department: "ventas",
            salary: 2400,
            age: 31,
            active: true,
        },
        Employee {
            name: "Luis",
            department: "tecnología",
            salary: 3600,
            age: 28,
            active: true,
        },
        Employee {
            name: "Marta",
            department: "tecnología",
            salary: 4100,
            age: 41,
            active: false,
        },
        Employee {
            name: "Pablo",
            department: "ventas",
            salary: 2100,
            age: 24,
            active: true,
        },
        Employee {
            name: "Sara",
            department: "recursos",
            salary: 2800,
            age: 36,
            active: true,
        },
        Employee {
            name: "Iván",
            department: "tecnología",
            salary: 3300,
            age: 33,
            active: true,
        },
    ];

    // Los iteradores son perezosos: no hacen nada hasta que se consumen (collect, sum, count...)
    let active: Vec<&str> = employees
        .iter()
        .filter(|e| e.active)
        .map(|e| e.name)
        .collect();
    println!("{active:?}");

    let total: u32 = employees.iter().map(|e| e.salary).sum();
    println!("{total} {}", total as f64 / employees.len() as f64);

    let mut sorted = employees.clone(); // ordenar una copia
    sorted.sort_by(|a, b| b.salary.cmp(&a.salary));
    let top3: Vec<&str> = sorted.iter().take(3).map(|e| e.name).collect();
    println!("{top3:?}");

    let first_over_40 = employees.iter().find(|e| e.age > 40).map(|e| e.name);
    println!("{first_over_40:?}");
    println!(
        "{} {}",
        employees.iter().all(|e| e.salary > 2000),
        employees.iter().any(|e| !e.active)
    );
    let names: Vec<&str> = employees.iter().map(|e| e.name).collect();
    println!("{}", names.join(", "));

    // Agrupar con HashMap y la API entry
    let mut payroll: BTreeMap<&str, u32> = BTreeMap::new(); // BTreeMap: ordenado por clave
    for e in &employees {
        *payroll.entry(e.department).or_insert(0) += e.salary;
    }
    println!("{payroll:?}");

    // Contar frecuencias y quitar repetidos
    let numbers = [3, 1, 4, 1, 5, 9, 2, 6, 5, 3, 5];
    let mut frequency: HashMap<i32, u32> = HashMap::new();
    for n in numbers {
        *frequency.entry(n).or_default() += 1;
    }
    let mut pairs: Vec<_> = frequency.into_iter().collect();
    pairs.sort();
    println!("{pairs:?}");
    let unique: HashSet<i32> = numbers.into_iter().collect();
    println!("{}", unique.len());

    let max = numbers.iter().max().unwrap();
    let min = numbers.iter().min().unwrap();
    println!("{max} {min}");
}
```

| Operación | Rust |
|---|---|
| Recorrer | `for x in &v` o `v.iter()` |
| Transformar | `.map(\|x\| …)` |
| Filtrar | `.filter(\|x\| …)` |
| Sumar | `.sum::<T>()` |
| Acumular | `.fold(inicial, \|acc, x\| …)` |
| Ordenar | `v.sort()`, `v.sort_by(\|a, b\| …)`, `v.sort_by_key(…)` |
| Primero que cumple | `.find(\|x\| …)` → `Option` |
| Alguno / todos | `.any(…)`, `.all(…)` |
| Agrupar y contar | `HashMap` con `.entry(clave).or_insert(0)` |
| Quitar repetidos | `HashSet` |
| Consumir el iterador | `.collect::<Vec<_>>()` |

Los iteradores son **perezosos**: `map` y `filter` no hacen nada hasta que un consumidor (`collect`, `sum`, `count`, `for`) los recorre.

## 8. Errores: `Option` y `Result`

```rust
// errors.rs: Option, Result, el operador ? y errores propios
use std::fmt;
use std::fs;
use std::num::ParseIntError;

// Un error propio: enum con los casos posibles
#[derive(Debug)]
enum ConfigError {
    Io(std::io::Error),
    Parse(ParseIntError),
    OutOfRange(i64),
}

impl fmt::Display for ConfigError {
    fn fmt(&self, f: &mut fmt::Formatter) -> fmt::Result {
        match self {
            ConfigError::Io(e) => write!(f, "error de lectura: {e}"),
            ConfigError::Parse(e) => write!(f, "número inválido: {e}"),
            ConfigError::OutOfRange(n) => write!(f, "{n} fuera de rango"),
        }
    }
}

// From permite que "?" convierta automáticamente los errores de la librería en el nuestro
impl From<std::io::Error> for ConfigError {
    fn from(e: std::io::Error) -> Self {
        ConfigError::Io(e)
    }
}
impl From<ParseIntError> for ConfigError {
    fn from(e: ParseIntError) -> Self {
        ConfigError::Parse(e)
    }
}

fn read_port(path: &str) -> Result<i64, ConfigError> {
    let text = fs::read_to_string(path)?; // si falla, devuelve Err y sale de la función
    let port: i64 = text.trim().parse()?;
    if !(1..=65535).contains(&port) {
        return Err(ConfigError::OutOfRange(port));
    }
    Ok(port)
}

fn divide(a: f64, b: f64) -> Option<f64> {
    // Option: puede haber valor (Some) o no (None). No existe null
    if b == 0.0 { None } else { Some(a / b) }
}

fn main() {
    println!("{:?} {:?}", divide(8.0, 2.0), divide(1.0, 0.0));
    println!("{}", divide(1.0, 0.0).unwrap_or(f64::NAN).is_nan());

    match "42".parse::<i32>() {
        Ok(n) => println!("número: {n}"),
        Err(e) => println!("error: {e}"),
    }
    println!("{:?}", "abc".parse::<i32>().is_err());

    fs::write("port.txt", "99999").unwrap();
    match read_port("port.txt") {
        Ok(p) => println!("puerto {p}"),
        Err(e) => println!("Error: {e}"),
    }
    fs::write("port.txt", "8080").unwrap();
    println!("{:?}", read_port("port.txt").ok());
    fs::remove_file("port.txt").unwrap();
    if let Err(e) = read_port("no-existe.txt") {
        println!("Error: {e}");
    }
    // unwrap() y expect() provocan un panic si hay error: solo en pruebas y prototipos
}
```

| Elemento | Significado |
|---|---|
| `Option<T>` | Puede no haber valor |
| `Result<T, E>` | Éxito o fallo con información |
| `?` | Si es `Err`, devuelve el error de la función actual; si es `Ok`, extrae el valor |
| `match` | Trata cada caso (el compilador exige cubrirlos todos) |
| `.unwrap()`, `.expect("…")` | **Provocan `panic`** (el programa aborta) si hay error: solo en pruebas o prototipos |
| `.unwrap_or(x)`, `.ok()`, `.map_err(…)` | Alternativas seguras |
| `impl From<A> for B` | Permite a `?` convertir errores automáticamente |

## 9. Structs, traits y genéricos

```rust
// traits.rs: structs, métodos, traits y genéricos
use std::fmt;

struct Account {
    owner: String,
    balance: f64,
}

impl Account {
    // Función asociada (como un constructor): se llama Account::new(...)
    fn new(owner: &str, balance: f64) -> Self {
        Account {
            owner: owner.to_string(),
            balance,
        }
    }

    // &mut self: puede modificar
    fn withdraw(&mut self, amount: f64) -> Result<(), String> {
        if amount > self.balance {
            return Err(format!("saldo insuficiente: {:.2}", self.balance));
        }
        self.balance -= amount;
        Ok(())
    }
}

impl fmt::Display for Account {
    fn fmt(&self, f: &mut fmt::Formatter) -> fmt::Result {
        write!(f, "{}: {:.2} €", self.owner, self.balance)
    }
}

// Un trait define un comportamiento; cada tipo lo implementa a su manera
trait Shape {
    fn area(&self) -> f64;
    fn describe(&self) -> String {
        format!("área {:.2}", self.area()) // implementación por defecto
    }
}

struct Rectangle {
    width: f64,
    height: f64,
}
struct Circle {
    radius: f64,
}

impl Shape for Rectangle {
    fn area(&self) -> f64 {
        self.width * self.height
    }
}
impl Shape for Circle {
    fn area(&self) -> f64 {
        std::f64::consts::PI * self.radius * self.radius
    }
}

// Genérico con restricción: T debe implementar Shape
fn print_area<T: Shape>(shape: &T) {
    println!("{}", shape.describe());
}

#[derive(Debug, Clone, PartialEq)]
enum Status {
    Todo,
    Doing { since: u32 }, // las variantes pueden llevar datos
    Done,
}

fn main() {
    let mut account = Account::new("Ana", 1000.0);
    println!("{account}");
    if let Err(e) = account.withdraw(5000.0) {
        println!("No se pudo: {e}");
    }

    print_area(&Rectangle {
        width: 3.0,
        height: 4.0,
    });
    print_area(&Circle { radius: 1.0 });

    // Polimorfismo dinámico: Box<dyn Trait>
    let shapes: Vec<Box<dyn Shape>> = vec![
        Box::new(Rectangle {
            width: 2.0,
            height: 5.0,
        }),
        Box::new(Circle { radius: 2.0 }),
    ];
    let total: f64 = shapes.iter().map(|s| s.area()).sum();
    println!("{total:.2}");

    let status = Status::Doing { since: 3 };
    match &status {
        Status::Todo => println!("pendiente"),
        Status::Doing { since } => println!("en curso desde {since}"),
        Status::Done => println!("hecha"),
    }
    println!("{:?} {}", status, status == Status::Done);
}
```

- No hay clases ni herencia. Se usan **structs con `impl`**, **traits** (comportamiento compartido) y **enums con datos**.
- `&self` solo lee; `&mut self` modifica; `self` consume.
- `#[derive(Debug, Clone, PartialEq)]` genera implementaciones automáticas.
- `Box<dyn Trait>` permite guardar tipos distintos que cumplen el mismo trait.

## 10. Concurrencia

```rust
// concurrency.rs: hilos, canales y estado compartido
use std::sync::{Arc, Mutex, mpsc};
use std::thread;
use std::time::Duration;

fn main() {
    // Canal: varios productores, un consumidor
    let (sender, receiver) = mpsc::channel();
    let mut handles = Vec::new();
    for id in 1..=3 {
        let sender = sender.clone();
        handles.push(thread::spawn(move || {
            // "move": el hilo toma posesión de lo que usa
            thread::sleep(Duration::from_millis(20));
            sender.send(format!("hilo {id} listo")).unwrap();
        }));
    }
    drop(sender); // cerramos nuestro emisor para que el bucle de abajo termine
    for message in receiver {
        let _ = message;
    }
    for handle in handles {
        handle.join().unwrap(); // espera a que termine el hilo
    }

    // Estado compartido: Arc (varios dueños) + Mutex (acceso exclusivo)
    let counter = Arc::new(Mutex::new(0));
    let mut workers = Vec::new();
    for _ in 0..100 {
        let counter = Arc::clone(&counter);
        workers.push(thread::spawn(move || {
            *counter.lock().unwrap() += 1;
        }));
    }
    for worker in workers {
        worker.join().unwrap();
    }
    println!("contador: {}", *counter.lock().unwrap());
}
```

El compilador **impide las carreras de datos**: no puedes compartir datos mutables entre hilos sin `Arc<Mutex<…>>` (o canales). Para programación asíncrona (servidores) se usa `async/await` con el runtime `tokio`.

## 11. Módulos y organización

```text
src/
  main.rs         punto de entrada; declara los módulos con `mod`
  models.rs       mod models;
  service.rs      mod service;
```

```rust
mod models;                       // incluye src/models.rs
use crate::models::Task;          // ruta absoluta dentro del crate
use std::collections::HashMap;    // de la librería estándar

pub struct Task { pub id: String } // pub = visible fuera del módulo; por defecto todo es privado
```

## 12. Cargo: manifiesto, dependencias y comandos

```toml
# Cargo.toml
[package]
name = "tasks-rs"
version = "0.1.0"
edition = "2024"

[dependencies]
axum = "0.8"
serde = { version = "1", features = ["derive"] }
tokio = { version = "1", features = ["full"] }

[dev-dependencies]
tower = { version = "0.5", features = ["util"] }
```

```bash
cargo add serde --features derive
cargo add --dev tower --features util
cargo remove nombre
cargo update
cargo tree
```

| Comando | Significado |
|---|---|
| `cargo add crate` | Añade una dependencia a `Cargo.toml`; `--features a,b` activa funciones opcionales; `--dev` solo para pruebas |
| `cargo remove crate` | La quita |
| `cargo update` | Actualiza `Cargo.lock` dentro de los rangos permitidos |
| `cargo tree` | Árbol de dependencias |
| `cargo check` | Comprueba que compila **sin generar el binario** (mucho más rápido) |
| `cargo build` | Compila en modo depuración (`target/debug/`) |
| `cargo build --release` | Compila optimizado (`target/release/`) |
| `cargo run -- args` | Ejecuta pasando argumentos a tu programa (lo que va tras `--`) |
| `cargo run --bin nombre` | Elige entre varios binarios (`src/bin/nombre.rs`) |
| `cargo test` | Ejecuta las pruebas; `cargo test nombre` filtra; `-- --nocapture` muestra los `println!` |
| `cargo fmt` | Formatea el código |
| `cargo clippy` | Linter con consejos |
| `cargo doc --open` | Genera y abre la documentación de tu proyecto y sus dependencias |

| Archivo | Qué es | ¿Se edita? | ¿Git? |
|---|---|---|---|
| `Cargo.toml` | **Manifiesto**: nombre, edición, dependencias, features | Sí (o con `cargo add`) | Sí |
| `Cargo.lock` | **Bloqueo** de versiones exactas | No | Sí en aplicaciones; opcional en librerías |
| `target/` | Compilados y caché | No | **No** (ya está en `.gitignore`) |

Versiones de dependencias: `"0.8"` equivale a `^0.8` (compatible con 0.8.x). *Features* son partes opcionales de un crate: actívalas solo si las necesitas (compilan menos código).

## 13. Una API REST con axum

Las mismas capas que en las demás tecnologías. Resultado intercambiable con las APIs de [Node.js](#node), [Python](#python), [Go](#go) y [Spring Boot](#spring).

```text
tasks-rs/
  Cargo.toml
  src/
    main.rs        arranque: puerto, CORS, servidor
    models.rs      entidad y DTOs
    error.rs       errores del dominio → respuestas HTTP
    service.rs     reglas de negocio y estado
    handlers.rs    rutas HTTP y pruebas
```

```rust
use serde::{Deserialize, Serialize};

/// Entidad. `Serialize` permite convertirla a JSON; `Clone` duplicarla.
#[derive(Debug, Clone, Serialize)]
pub struct Task {
    pub id: String,
    pub title: String,
    pub done: bool,
}

/// DTO de entrada para crear una tarea.
#[derive(Debug, Deserialize)]
pub struct CreateTask {
    pub title: String,
}

/// Cambios parciales: `Option` = puede venir o no.
#[derive(Debug, Deserialize)]
pub struct UpdateTask {
    pub title: Option<String>,
    pub done: Option<bool>,
}
```

```rust
use axum::{
    Json,
    http::StatusCode,
    response::{IntoResponse, Response},
};
use serde_json::json;

/// Errores del dominio. Un `enum` modela exactamente los casos posibles.
#[derive(Debug)]
pub enum AppError {
    NotFound(String),
    Validation(String),
}

/// Cómo se convierte cada error en una respuesta HTTP: un único lugar.
impl IntoResponse for AppError {
    fn into_response(self) -> Response {
        let (status, message) = match self {
            AppError::NotFound(message) => (StatusCode::NOT_FOUND, message),
            AppError::Validation(message) => (StatusCode::BAD_REQUEST, message),
        };
        (status, Json(json!({ "error": message }))).into_response()
    }
}
```

```rust
use std::sync::{Arc, Mutex};

use crate::error::AppError;
use crate::models::{CreateTask, Task, UpdateTask};

#[derive(Default)]
struct Store {
    tasks: Vec<Task>,
    last_id: u64,
}

/// Servicio con estado compartido. `Arc` permite varios dueños; `Mutex` garantiza un solo acceso a la vez.
#[derive(Clone, Default)]
pub struct TaskService {
    store: Arc<Mutex<Store>>,
}

fn validate_title(title: &str) -> Result<String, AppError> {
    let title = title.trim();
    if title.chars().count() < 2 {
        return Err(AppError::Validation(
            "title debe tener al menos 2 caracteres".to_string(),
        ));
    }
    Ok(title.to_string())
}

impl TaskService {
    pub fn list(&self) -> Vec<Task> {
        self.store.lock().unwrap().tasks.clone()
    }

    pub fn get(&self, id: &str) -> Result<Task, AppError> {
        let store = self.store.lock().unwrap();
        store
            .tasks
            .iter()
            .find(|task| task.id == id)
            .cloned()
            .ok_or_else(|| AppError::NotFound(format!("No existe la tarea {id}")))
    }

    pub fn create(&self, input: CreateTask) -> Result<Task, AppError> {
        let title = validate_title(&input.title)?;
        let mut store = self.store.lock().unwrap();
        store.last_id += 1;
        let task = Task {
            id: store.last_id.to_string(),
            title,
            done: false,
        };
        store.tasks.push(task.clone());
        Ok(task)
    }

    pub fn update(&self, id: &str, input: UpdateTask) -> Result<Task, AppError> {
        let new_title = input.title.as_deref().map(validate_title).transpose()?;
        let mut store = self.store.lock().unwrap();
        let task = store
            .tasks
            .iter_mut()
            .find(|task| task.id == id)
            .ok_or_else(|| AppError::NotFound(format!("No existe la tarea {id}")))?;
        if let Some(title) = new_title {
            task.title = title;
        }
        if let Some(done) = input.done {
            task.done = done;
        }
        Ok(task.clone())
    }

    pub fn delete(&self, id: &str) -> Result<(), AppError> {
        let mut store = self.store.lock().unwrap();
        let before = store.tasks.len();
        store.tasks.retain(|task| task.id != id);
        if store.tasks.len() == before {
            return Err(AppError::NotFound(format!("No existe la tarea {id}")));
        }
        Ok(())
    }
}
```

```rust
use axum::{
    Json, Router,
    extract::{Path, State},
    http::StatusCode,
    routing::get,
};

use crate::error::AppError;
use crate::models::{CreateTask, Task, UpdateTask};
use crate::service::TaskService;

/// Construye las rutas. `State` comparte el servicio con todos los manejadores.
pub fn router(service: TaskService) -> Router {
    Router::new()
        .route("/api/tasks", get(list).post(create))
        .route("/api/tasks/{id}", get(get_one).patch(update).delete(delete))
        .with_state(service)
}

async fn list(State(service): State<TaskService>) -> Json<Vec<Task>> {
    Json(service.list())
}

async fn get_one(
    State(service): State<TaskService>,
    Path(id): Path<String>,
) -> Result<Json<Task>, AppError> {
    Ok(Json(service.get(&id)?))
}

async fn create(
    State(service): State<TaskService>,
    Json(input): Json<CreateTask>,
) -> Result<(StatusCode, Json<Task>), AppError> {
    Ok((StatusCode::CREATED, Json(service.create(input)?)))
}

async fn update(
    State(service): State<TaskService>,
    Path(id): Path<String>,
    Json(input): Json<UpdateTask>,
) -> Result<Json<Task>, AppError> {
    Ok(Json(service.update(&id, input)?))
}

async fn delete(
    State(service): State<TaskService>,
    Path(id): Path<String>,
) -> Result<StatusCode, AppError> {
    service.delete(&id)?;
    Ok(StatusCode::NO_CONTENT)
}

#[cfg(test)]
mod tests {
    use super::*;
    use axum::{body::Body, http::Request};
    use http_body_util::BodyExt;
    use tower::ServiceExt; // aporta `oneshot`

    async fn send(app: &Router, method: &str, uri: &str, body: &str) -> (StatusCode, String) {
        let request = Request::builder()
            .method(method)
            .uri(uri)
            .header("content-type", "application/json")
            .body(Body::from(body.to_string()))
            .unwrap();
        let response = app.clone().oneshot(request).await.unwrap();
        let status = response.status();
        let bytes = response.into_body().collect().await.unwrap().to_bytes();
        (status, String::from_utf8(bytes.to_vec()).unwrap())
    }

    #[tokio::test]
    async fn crud_flow() {
        let app = router(TaskService::default());

        let (status, body) = send(&app, "POST", "/api/tasks", r#"{"title":"Estudiar Rust"}"#).await;
        assert_eq!(status, StatusCode::CREATED);
        assert!(body.contains(r#""title":"Estudiar Rust""#));

        let (status, _) = send(&app, "POST", "/api/tasks", r#"{"title":"x"}"#).await;
        assert_eq!(status, StatusCode::BAD_REQUEST);

        let (_, body) = send(&app, "PATCH", "/api/tasks/1", r#"{"done":true}"#).await;
        assert!(body.contains(r#""done":true"#));

        let (status, _) = send(&app, "DELETE", "/api/tasks/1", "").await;
        assert_eq!(status, StatusCode::NO_CONTENT);

        let (status, _) = send(&app, "GET", "/api/tasks/1", "").await;
        assert_eq!(status, StatusCode::NOT_FOUND);
    }
}
```

```rust
mod error;
mod handlers;
mod models;
mod service;

use std::env;

use axum::http::{HeaderValue, Method, header};
use tower_http::cors::CorsLayer;

#[tokio::main]
async fn main() {
    let port = env::var("PORT").unwrap_or_else(|_| "3000".to_string());
    let origin = env::var("CORS_ORIGIN").unwrap_or_else(|_| "http://localhost:5173".to_string());

    let cors = CorsLayer::new()
        .allow_origin(origin.parse::<HeaderValue>().expect("CORS_ORIGIN inválido"))
        .allow_methods([Method::GET, Method::POST, Method::PATCH, Method::DELETE])
        .allow_headers([header::CONTENT_TYPE]);

    let app = handlers::router(service::TaskService::default()).layer(cors);

    let listener = tokio::net::TcpListener::bind(format!("0.0.0.0:{port}"))
        .await
        .expect("no se pudo abrir el puerto");
    println!("escuchando en http://localhost:{port}");
    axum::serve(listener, app)
        .await
        .expect("error del servidor");
}
```

| Pieza | Significado |
|---|---|
| `#[tokio::main]` | Convierte `main` en asíncrona con el runtime tokio |
| `Router::new().route("/api/tasks/{id}", get(…).patch(…))` | Rutas por método; `{id}` es un parámetro (axum 0.8) |
| `State<TaskService>` | Estado compartido inyectado en cada manejador |
| `Path(id): Path<String>` | Extrae el parámetro de la URL |
| `Json(input): Json<CreateTask>` | Deserializa el cuerpo; si es inválido, axum responde `4xx` solo |
| `Result<Json<T>, AppError>` | Si hay error, `AppError::into_response` decide el código HTTP |
| `Arc<Mutex<Store>>` | Estado compartido entre las peticiones concurrentes |
| `Option<String>` en `UpdateTask` | Campo opcional del PATCH |
| `CorsLayer` | CORS: se enumeran métodos y cabeceras permitidos |

Ejecutar y probar:

```bash
PORT=3000 cargo run
cargo test
cargo clippy
```

La primera compilación descarga y compila las dependencias (es lenta); las siguientes son rápidas.

## 14. Pruebas, formato y análisis

- Pruebas unitarias en el mismo archivo, dentro de `#[cfg(test)] mod tests { … }` con `#[test]` (o `#[tokio::test]` para asíncronas).
- Pruebas de integración en la carpeta `tests/`.
- `cargo fmt --check` verifica el formato sin cambiarlo (útil en CI); `cargo clippy -- -D warnings` convierte los avisos en errores.
- Depuración: `dbg!(valor)` imprime el valor y su origen; `RUST_BACKTRACE=1 cargo run` muestra la traza de un `panic`; depurador con la extensión CodeLLDB en VS Code.

## 15. Compilar y empaquetar

```bash
cargo build --release
./target/release/tasks-rs
```

Imagen Docker en dos etapas:

```dockerfile
FROM rust:1 AS build
WORKDIR /src
COPY . .
RUN cargo build --release

FROM debian:stable-slim
COPY --from=build /src/target/release/tasks-rs /usr/local/bin/tasks-rs
EXPOSE 3000
CMD ["tasks-rs"]
```

## 16. Errores habituales

| Mensaje | Causa | Solución |
|---|---|---|
| `borrow of moved value: x` | Usaste `x` después de moverlo | Préstalo con `&x` o clónalo con `.clone()` |
| `cannot borrow x as mutable because it is also borrowed as immutable` | Referencias de lectura y de escritura a la vez | Reduce el alcance de las referencias |
| `cannot assign twice to immutable variable` | Falta `mut` | `let mut x` |
| `mismatched types: expected &str, found String` | Tipos distintos | `&texto` (de `String` a `&str`) o `.to_string()` |
| `the trait bound X: Y is not satisfied` | El tipo no implementa lo requerido | `#[derive(...)]` o `impl` del trait |
| `use of undeclared crate or module` | Falta `mod` o dependencia | Declara el módulo o `cargo add` |
| `thread 'main' panicked at … unwrap()` | `unwrap` sobre `None`/`Err` | Maneja el caso con `match` o `?` |
| `missing lifetime specifier` | Referencias devueltas sin saber de dónde vienen | Anota `'a` o devuelve un valor propio |
| `address already in use` | Puerto ocupado | `ss -ltnp \| grep 3000` |
| `error: linker 'cc' not found` | Falta compilador/enlazador del sistema | Ya tienes `gcc`/`clang` |

## 17. Ejercicios

> **Ejercicio:** resuelve los ejercicios 1–15 de [lógica y programación](#ejercicios-basicos) en Rust. Anota cada error de préstamo que te dé el compilador y por qué ocurrió.

> **Ejercicio:** reproduce el análisis de empleados con iteradores: activos, masa salarial por departamento, mejor pagado.

> **Ejercicio:** escribe una CLI `wordcount` con `std::env::args` que lea un archivo y cuente las palabras con un `HashMap`, devolviendo `Result` desde `main`.

> **Ejercicio:** añade `GET /api/tasks?done=true` a la API con `Query<Params>` de axum.

> **Ejercicio:** crea un error propio con `enum` y `From` para que `?` convierta errores de `std::io` y de `serde_json` en uno solo.

## 18. Documentación oficial

- [El libro de Rust](https://doc.rust-lang.org/book/)
- [Rust by Example](https://doc.rust-lang.org/rust-by-example/)
- [Rustlings: ejercicios](https://github.com/rust-lang/rustlings)
- [The Cargo Book](https://doc.rust-lang.org/cargo/)
- [axum](https://docs.rs/axum)
- [crates.io](https://crates.io/)
