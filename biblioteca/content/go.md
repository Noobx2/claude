# Go

Go es un lenguaje **compilado, estático y muy simple**, pensado para servidores y herramientas de línea de comandos. Compila rápido a un único binario sin dependencias, trae en la librería estándar casi todo lo necesario (HTTP, JSON, pruebas) y hace la concurrencia fácil.

*Verificado: 2026-10-27. Los ejemplos se han compilado y ejecutado con Go 1.24.7. Tu `go` hoy **no funciona** (shim de `mise` sin versión); sigue la sección 2. Referencia: [go.dev/doc](https://go.dev/doc/) y [Tour de Go](https://go.dev/tour/).*

## 1. Para qué sirve

| Uso | Ejemplo |
|---|---|
| APIs y microservicios | Servidores HTTP con solo la librería estándar |
| Herramientas de línea de comandos | Docker, kubectl y Terraform están hechos en Go |
| Redes y concurrencia | Servidores con miles de conexiones simultáneas |
| Binarios de despliegue sencillo | Un solo archivo, se copia y se ejecuta |

## 2. Instalación y versiones

El informe mostró:

```text
mise ERROR No version is set for shim: go
```

`mise` está instalado y existe el *shim* `go`, pero no hay ninguna versión elegida. Para activarla:

```bash
mise ls-remote go | tail
mise use -g go@<versión>
go version
```

| Pieza | Significado |
|---|---|
| `mise ls-remote go` | Lista las versiones disponibles de Go |
| `mise use -g go@X` | Instala (si hace falta) y activa la versión X **globalmente** (`-g` = *global*) |
| `mise use go@X` | Solo para la carpeta actual (crea `mise.toml`) |
| `go version` | Comprueba la versión activa |

El propio mensaje de tu sistema sugería `go@1.27.3`; verifica con `mise ls-remote go` que existe y elige la última estable. Alternativa que **ya tienes**: el contenedor `go-flutter-lab-backend` (Go dentro de Docker) — ver [Docker](#docker).

## 3. Archivos y convenciones de nombres

| Elemento | Convención |
|---|---|
| Archivos | `snake_case.go`; pruebas en `*_test.go` |
| Paquetes | Una palabra en minúsculas (`tasks`, `http`); el nombre de la carpeta |
| Visibilidad | **Mayúscula inicial = exportado** (público); minúscula = privado del paquete |
| Variables y funciones | `camelCase` (privadas) / `PascalCase` (exportadas) |
| Constructores | Funciones `NewXxx` |
| Interfaces de un método | Sufijo `-er` (`Reader`, `Stringer`) |
| Formato | **Único y obligatorio**: `gofmt` (tabuladores). No se discute |

## 4. Hola mundo

```go
// main.go
package main

import "fmt"

func main() {
	fmt.Println("Hola mundo")
}
```

```bash
mkdir -p ~/projects/hello-go && cd ~/projects/hello-go
go mod init example.com/hello
go run .
```

| Pieza | Significado |
|---|---|
| `package main` | Paquete ejecutable (solo `main` genera un programa) |
| `import "fmt"` | Importa un paquete de la librería estándar |
| `func main()` | Punto de entrada |
| `go mod init example.com/hello` | Crea `go.mod` y le da nombre al módulo |
| `go run .` | Compila y ejecuta el paquete de la carpeta actual |

Un import o una variable **sin usar son errores de compilación**, no avisos.

## 5. Variables, tipos y conversiones

```go
// basics/main.go: variables, tipos, condiciones, bucles y funciones
package main

import (
	"fmt"
	"strconv"
)

const MaxTries = 3 // constante

// Función con varios valores de retorno: lo normal en Go
func divide(a, b float64) (float64, error) {
	if b == 0 {
		return 0, fmt.Errorf("no se puede dividir %v entre cero", a)
	}
	return a / b, nil
}

// Parámetros variádicos
func sum(numbers ...int) int {
	total := 0
	for _, n := range numbers {
		total += n
	}
	return total
}

func grade(score int) string {
	switch { // switch sin expresión: cada case es una condición
	case score >= 90:
		return "A"
	case score >= 70:
		return "B"
	default:
		return "C"
	}
}

func main() {
	var name string = "Ana" // declaración completa
	age := 31               // := declara e infiere el tipo (solo dentro de funciones)
	height := 1.68
	var student bool // valor cero: false
	var count int    // valor cero: 0

	fmt.Println(name, age, height, student, count)
	fmt.Printf("%s mide %.1f m (%T)\n", name, height, height) // %T muestra el tipo

	// Conversiones: siempre explícitas
	n, err := strconv.Atoi("42")
	fmt.Println(n+1, err)
	_, err = strconv.Atoi("abc")
	fmt.Println(err)
	fmt.Println(float64(7)/2, 7/2, 7%2, strconv.Itoa(7)+"!")

	result, err := divide(8, 2)
	fmt.Println(result, err)
	_, err = divide(1, 0)
	fmt.Println("error:", err)

	fmt.Println(sum(1, 2, 3), grade(95), grade(72), grade(40))

	// Un único bucle: for
	for i := 1; i <= 3; i++ {
		fmt.Println("vuelta", i)
	}
	for index, letter := range "abc" { // range recorre slices, mapas, textos, canales
		fmt.Println(index, string(letter))
	}
	remaining := 3
	for remaining > 0 { // "while"
		remaining--
	}

	// if con sentencia de inicio
	if value, err := strconv.Atoi("7"); err == nil && value > 5 {
		fmt.Println("mayor que 5:", value)
	}

	// Funciones como valores
	square := func(x int) int { return x * x }
	fmt.Println(square(5))
}
```

| Tipo | Ejemplo | Notas |
|---|---|---|
| `int`, `int64`, `uint` | `42` | `int` suele ser de 64 bits |
| `float64` | `3.14` | |
| `string` | `"texto"` | Inmutable, UTF-8; `len(s)` cuenta **bytes** |
| `bool` | `true` | |
| `rune` | `'ñ'` | Un carácter Unicode (`int32`) |
| `[]T` (slice) | `[]int{1,2}` | Lista dinámica |
| `map[K]V` | `map[string]int{}` | Diccionario |
| `struct` | `type Task struct{…}` | Agrupa campos |
| `error` | `errors.New("x")` | Interfaz para fallos |

Los **valores cero**: toda variable declarada sin valor se inicializa sola (`0`, `""`, `false`, `nil`). `:=` solo funciona dentro de funciones. Las conversiones numéricas son siempre explícitas (`float64(n)`).

## 6. Colecciones: recorrer, filtrar, transformar, ordenar y agrupar

```go
// collections/main.go: slices, mapas, structs y ordenación
package main

import (
	"fmt"
	"slices"
	"sort"
	"strings"
)

type Employee struct {
	ID         int
	Name       string
	Department string
	Salary     int
	Age        int
	Active     bool
}

func main() {
	employees := []Employee{
		{1, "Ana", "ventas", 2400, 31, true},
		{2, "Luis", "tecnología", 3600, 28, true},
		{3, "Marta", "tecnología", 4100, 41, false},
		{4, "Pablo", "ventas", 2100, 24, true},
		{5, "Sara", "recursos", 2800, 36, true},
		{6, "Iván", "tecnología", 3300, 33, true},
	}

	// Go no tiene map/filter genéricos en la librería base: se escriben con bucles
	var activeNames []string
	total := 0
	for _, e := range employees {
		total += e.Salary
		if e.Active {
			activeNames = append(activeNames, e.Name)
		}
	}
	fmt.Println(activeNames, total, float64(total)/float64(len(employees)))

	// Ordenar una copia (para no modificar el original)
	sorted := slices.Clone(employees)
	sort.Slice(sorted, func(i, j int) bool { return sorted[i].Salary > sorted[j].Salary })
	fmt.Println(sorted[0].Name, sorted[1].Name, sorted[2].Name)

	// Agrupar con un mapa
	payroll := map[string]int{}
	for _, e := range employees {
		payroll[e.Department] += e.Salary // un valor ausente vale 0, así que += funciona
	}
	fmt.Println(payroll) // fmt imprime los mapas ordenados por clave

	// Recorrido ordenado de un mapa: el orden de range NO está garantizado
	keys := make([]string, 0, len(payroll))
	for department := range payroll {
		keys = append(keys, department)
	}
	slices.Sort(keys)
	fmt.Println(keys)

	// Comprobar si existe una clave: la forma "coma ok"
	if value, ok := payroll["ventas"]; ok {
		fmt.Println("ventas:", value)
	}
	delete(payroll, "ventas")

	// Slices: vistas sobre un array
	numbers := []int{3, 1, 4, 1, 5, 9, 2, 6, 5, 3, 5}
	fmt.Println(len(numbers), numbers[2:5], slices.Max(numbers), slices.Contains(numbers, 9))

	var names []string
	for _, e := range employees {
		names = append(names, e.Name)
	}
	fmt.Println(strings.Join(names, ", "))
}
```

Go no trae `map`/`filter`/`reduce` en la librería base; se escriben con bucles o con funciones genéricas propias:

```go
// generics/main.go: genéricos y funciones de orden superior (Map, Filter, Reduce propios)
package main

import "fmt"

// [T, U any]: parámetros de tipo. "any" significa cualquier tipo.
func Map[T, U any](items []T, fn func(T) U) []U {
	result := make([]U, 0, len(items))
	for _, item := range items {
		result = append(result, fn(item))
	}
	return result
}

func Filter[T any](items []T, keep func(T) bool) []T {
	var result []T
	for _, item := range items {
		if keep(item) {
			result = append(result, item)
		}
	}
	return result
}

func Reduce[T, A any](items []T, initial A, fn func(A, T) A) A {
	accumulator := initial
	for _, item := range items {
		accumulator = fn(accumulator, item)
	}
	return accumulator
}

func main() {
	numbers := []int{1, 2, 3, 4, 5}
	doubled := Map(numbers, func(n int) int { return n * 2 })
	evens := Filter(numbers, func(n int) bool { return n%2 == 0 })
	total := Reduce(numbers, 0, func(sum, n int) int { return sum + n })
	labels := Map(numbers, func(n int) string { return fmt.Sprintf("#%d", n) })
	fmt.Println(doubled, evens, total, labels)
}
```

| Operación | Go |
|---|---|
| Recorrer | `for i, x := range xs` |
| Filtrar / transformar | Bucle con `append`, o funciones genéricas |
| Ordenar | `slices.Sort`, `sort.Slice(xs, func(i, j int) bool {…})` |
| Máximo, contiene | `slices.Max`, `slices.Contains` |
| Copiar | `slices.Clone(xs)` |
| Agrupar | `m[clave] = append(m[clave], x)` |
| Existencia en un mapa | `valor, ok := m[clave]` |
| Borrar | `delete(m, clave)` |

> **Ojo:** el orden de `range` sobre un mapa **no está garantizado**. Para un orden estable, ordena las claves.

## 7. Errores: valores, no excepciones

```go
// errjson/main.go: errores como valores y JSON
package main

import (
	"encoding/json"
	"errors"
	"fmt"
	"os"
)

var ErrNotFound = errors.New("no encontrado") // error centinela

type ValidationError struct {
	Field string
}

func (e *ValidationError) Error() string { return "campo inválido: " + e.Field }

func find(id int) error {
	if id != 1 {
		return fmt.Errorf("tarea %d: %w", id, ErrNotFound) // %w envuelve el error original
	}
	return nil
}

func validate(title string) error {
	if len(title) < 2 {
		return &ValidationError{Field: "title"}
	}
	return nil
}

// Las etiquetas `json:"..."` definen los nombres en el JSON
type Task struct {
	ID    int      `json:"id"`
	Title string   `json:"title"`
	Done  bool     `json:"done"`
	Tags  []string `json:"tags,omitempty"` // omitempty: no se escribe si está vacío
}

func main() {
	err := find(7)
	fmt.Println(err, errors.Is(err, ErrNotFound)) // errors.Is recorre la cadena de errores envueltos

	err = validate("x")
	var validationError *ValidationError
	if errors.As(err, &validationError) { // errors.As busca un tipo concreto
		fmt.Println("campo:", validationError.Field)
	}

	task := Task{ID: 1, Title: "Estudiar Go", Tags: []string{"estudio"}}
	data, _ := json.MarshalIndent(task, "", "  ") // struct -> JSON
	fmt.Println(string(data))

	var parsed Task
	if err := json.Unmarshal([]byte(`{"id":2,"title":"Probar","done":true}`), &parsed); err != nil { // JSON -> struct
		fmt.Println(err)
	}
	fmt.Printf("%+v\n", parsed)

	if err := json.Unmarshal([]byte(`{"id":`), &parsed); err != nil {
		fmt.Println("JSON inválido:", err)
	}

	// Archivos
	_ = os.WriteFile("demo.json", data, 0o644)
	content, err := os.ReadFile("demo.json")
	fmt.Println(len(content) > 0, err)
	_ = os.Remove("demo.json")
	if _, err := os.ReadFile("no-existe.json"); errors.Is(err, os.ErrNotExist) {
		fmt.Println("el archivo no existe")
	}
}
```

El patrón omnipresente:

```go
value, err := doSomething()
if err != nil {
	return fmt.Errorf("contexto útil: %w", err) // %w conserva el error original
}
```

| Función | Para qué |
|---|---|
| `errors.New("texto")` | Crea un error simple |
| `fmt.Errorf("…: %w", err)` | Envuelve un error añadiendo contexto |
| `errors.Is(err, ErrX)` | ¿Es (o envuelve) este error concreto? |
| `errors.As(err, &miTipo)` | ¿Es de este tipo? Extrae el valor |
| `defer` | Aplaza una llamada hasta salir de la función (cerrar archivos) |
| `panic` / `recover` | Solo para fallos irrecuperables, no para el flujo normal |

## 8. Structs, métodos e interfaces (sin clases)

```go
// structs/main.go: structs, métodos, interfaces y composición (Go no tiene clases ni herencia)
package main

import (
	"errors"
	"fmt"
)

var ErrInsufficientFunds = errors.New("saldo insuficiente")

type Account struct {
	Owner   string
	balance float64 // minúscula: privado del paquete
}

// Constructor por convención: función NewXxx
func NewAccount(owner string, balance float64) *Account {
	return &Account{Owner: owner, balance: balance}
}

// Método con receptor puntero (*Account): puede modificar la cuenta
func (a *Account) Deposit(amount float64) error {
	if amount <= 0 {
		return errors.New("el importe debe ser positivo")
	}
	a.balance += amount
	return nil
}

func (a *Account) Withdraw(amount float64) error {
	if amount > a.balance {
		return fmt.Errorf("%w: %.2f", ErrInsufficientFunds, a.balance)
	}
	a.balance -= amount
	return nil
}

// Receptor valor: solo lee
func (a Account) String() string { return fmt.Sprintf("%s: %.2f €", a.Owner, a.balance) }

// Interfaz: se cumple IMPLÍCITAMENTE; basta con tener los métodos
type Shape interface {
	Area() float64
}

type Rectangle struct{ Width, Height float64 }
type Circle struct{ Radius float64 }

func (r Rectangle) Area() float64 { return r.Width * r.Height }
func (c Circle) Area() float64    { return 3.14159 * c.Radius * c.Radius }

// Composición en lugar de herencia: se "incrusta" otro tipo
type SavingsAccount struct {
	Account // incrustado: SavingsAccount obtiene sus campos y métodos
	Rate    float64
}

func main() {
	account := NewAccount("Ana", 1000)
	_ = account.Deposit(500)
	fmt.Println(account) // usa String()
	if err := account.Withdraw(5000); errors.Is(err, ErrInsufficientFunds) {
		fmt.Println("No se pudo:", err)
	}

	shapes := []Shape{Rectangle{3, 4}, Circle{1}}
	for _, shape := range shapes {
		fmt.Printf("%T %.2f\n", shape, shape.Area())
	}

	savings := SavingsAccount{Account: *NewAccount("Luis", 200), Rate: 0.02}
	_ = savings.Deposit(100) // método heredado por composición
	fmt.Println(savings.Owner, savings.Rate, savings.String())

	// Punteros: & toma la dirección, * accede al valor
	x := 5
	p := &x
	*p = 10
	fmt.Println(x)
}
```

- No hay clases ni herencia: hay **structs con métodos** y **composición** (incrustación).
- Las **interfaces** se cumplen implícitamente: si un tipo tiene los métodos, la cumple. No hay `implements`.
- Receptor puntero (`*Account`) para modificar el valor; receptor valor para solo leer.
- Define las interfaces **donde se usan** (el consumidor), pequeñas y de pocos métodos.

## 9. Concurrencia

```go
// concurrency/main.go: goroutines, canales y WaitGroup
package main

import (
	"context"
	"fmt"
	"sync"
	"time"
)

func worker(id int, jobs <-chan int, results chan<- string, wg *sync.WaitGroup) {
	defer wg.Done()
	for job := range jobs {
		time.Sleep(50 * time.Millisecond) // simula trabajo
		results <- fmt.Sprintf("worker %d: trabajo %d", id, job)
	}
}

func main() {
	jobs := make(chan int, 5)
	results := make(chan string, 5)
	var wg sync.WaitGroup

	for id := 1; id <= 3; id++ {
		wg.Add(1)
		go worker(id, jobs, results, &wg) // "go" lanza una goroutine
	}
	for job := 1; job <= 5; job++ {
		jobs <- job
	}
	close(jobs) // avisa de que no habrá más trabajos
	wg.Wait()   // espera a que terminen todos los workers
	close(results)

	count := 0
	for range results {
		count++
	}
	fmt.Println("resultados:", count)

	// Mutex: protege datos compartidos
	var mu sync.Mutex
	counter := 0
	var wg2 sync.WaitGroup
	for i := 0; i < 100; i++ {
		wg2.Add(1)
		go func() {
			defer wg2.Done()
			mu.Lock()
			counter++
			mu.Unlock()
		}()
	}
	wg2.Wait()
	fmt.Println("contador:", counter)

	// context: cancelación y plazos
	ctx, cancel := context.WithTimeout(context.Background(), 100*time.Millisecond)
	defer cancel()
	select {
	case <-time.After(time.Second):
		fmt.Println("terminó")
	case <-ctx.Done():
		fmt.Println("cancelado:", ctx.Err())
	}
}
```

| Concepto | Significado |
|---|---|
| `go f()` | Lanza `f` en una **goroutine** (hilo ligero) |
| `chan T` | Canal tipado para comunicar goroutines; `<-` envía y recibe |
| `sync.WaitGroup` | Espera a que terminen varias goroutines |
| `sync.Mutex` | Protege datos compartidos |
| `select` | Espera varios canales a la vez |
| `context` | Cancelación y plazos que se propagan entre llamadas |

Lema: *no comuniques compartiendo memoria; comparte memoria comunicando*. Prueba siempre con el detector de carreras: `go test -race ./...`.

## 10. Módulos, paquetes y dependencias

```bash
go mod init example.com/tasks
go get github.com/google/uuid@latest
go mod tidy
```

| Comando | Significado |
|---|---|
| `go mod init nombre` | Crea `go.mod` |
| `go get paquete@versión` | Añade o actualiza una dependencia |
| `go mod tidy` | Añade lo que falte, quita lo sobrante y actualiza `go.sum` |
| `go list -m all` | Todas las dependencias |
| `go mod download` | Descarga las dependencias |

| Archivo | Qué es | ¿Se edita? | ¿Git? |
|---|---|---|---|
| `go.mod` | **Manifiesto**: nombre del módulo, versión de Go, dependencias | Casi nunca a mano | Sí |
| `go.sum` | **Bloqueo / verificación**: hashes de las dependencias | No | Sí |
| `vendor/` | Copia local de dependencias (opcional) | No | Depende |

Un **paquete** es una carpeta; todos sus `.go` empiezan por `package nombre`. Se importa por su ruta de módulo: `import "example.com/tasks/internal/tasks"`. La carpeta `internal/` solo puede importarse desde dentro del módulo.

## 11. Una API REST con solo la librería estándar

Estructura:

```text
tasks-go/
  go.mod
  cmd/
    server/main.go          punto de entrada (puerto, CORS, arranque)
  internal/tasks/
    task.go                 tipos y DTOs
    repository.go           interfaz + implementación en memoria
    service.go              reglas de negocio y errores del dominio
    handler.go              capa HTTP
    middleware.go           CORS
    handler_test.go         pruebas
```

```go
// Package tasks contiene el dominio, el servicio, el repositorio y el manejador HTTP de las tareas.
package tasks

// Task es la entidad. Las etiquetas json definen cómo se escribe en la API.
type Task struct {
	ID    string `json:"id"`
	Title string `json:"title"`
	Done  bool   `json:"done"`
}

// CreateInput es el DTO de entrada para crear.
type CreateInput struct {
	Title string `json:"title"`
}

// UpdateInput son los cambios parciales: un puntero nil significa "no modificar".
type UpdateInput struct {
	Title *string `json:"title"`
	Done  *bool   `json:"done"`
}
```

```go
package tasks

import (
	"strconv"
	"sync"
)

// Repository es el contrato de acceso a datos. Se define donde se USA (en el servicio).
type Repository interface {
	FindAll() []Task
	FindByID(id string) (Task, bool)
	Save(task Task) Task
	Delete(id string) bool
	NextID() string
}

// MemoryRepository guarda las tareas en memoria; el mutex protege el acceso concurrente.
type MemoryRepository struct {
	mu     sync.Mutex
	tasks  map[string]Task
	order  []string
	lastID int
}

func NewMemoryRepository() *MemoryRepository {
	return &MemoryRepository{tasks: map[string]Task{}}
}

func (r *MemoryRepository) FindAll() []Task {
	r.mu.Lock()
	defer r.mu.Unlock()
	result := make([]Task, 0, len(r.order))
	for _, id := range r.order {
		result = append(result, r.tasks[id])
	}
	return result
}

func (r *MemoryRepository) FindByID(id string) (Task, bool) {
	r.mu.Lock()
	defer r.mu.Unlock()
	task, ok := r.tasks[id]
	return task, ok
}

func (r *MemoryRepository) Save(task Task) Task {
	r.mu.Lock()
	defer r.mu.Unlock()
	if _, exists := r.tasks[task.ID]; !exists {
		r.order = append(r.order, task.ID)
	}
	r.tasks[task.ID] = task
	return task
}

func (r *MemoryRepository) Delete(id string) bool {
	r.mu.Lock()
	defer r.mu.Unlock()
	if _, exists := r.tasks[id]; !exists {
		return false
	}
	delete(r.tasks, id)
	for i, existing := range r.order {
		if existing == id {
			r.order = append(r.order[:i], r.order[i+1:]...)
			break
		}
	}
	return true
}

func (r *MemoryRepository) NextID() string {
	r.mu.Lock()
	defer r.mu.Unlock()
	r.lastID++
	return strconv.Itoa(r.lastID)
}
```

```go
package tasks

import (
	"errors"
	"fmt"
	"strings"
)

// Errores del dominio: el manejador HTTP los traduce a códigos de estado.
var (
	ErrNotFound   = errors.New("no encontrado")
	ErrValidation = errors.New("datos inválidos")
)

// Service contiene las reglas de negocio. No sabe nada de HTTP.
type Service struct {
	repo Repository
}

func NewService(repo Repository) *Service {
	return &Service{repo: repo}
}

func validateTitle(title string) (string, error) {
	title = strings.TrimSpace(title)
	if len([]rune(title)) < 2 {
		return "", fmt.Errorf("%w: title debe tener al menos 2 caracteres", ErrValidation)
	}
	return title, nil
}

func (s *Service) List() []Task { return s.repo.FindAll() }

func (s *Service) Get(id string) (Task, error) {
	task, ok := s.repo.FindByID(id)
	if !ok {
		return Task{}, fmt.Errorf("tarea %s: %w", id, ErrNotFound)
	}
	return task, nil
}

func (s *Service) Create(input CreateInput) (Task, error) {
	title, err := validateTitle(input.Title)
	if err != nil {
		return Task{}, err
	}
	return s.repo.Save(Task{ID: s.repo.NextID(), Title: title}), nil
}

func (s *Service) Update(id string, input UpdateInput) (Task, error) {
	task, err := s.Get(id)
	if err != nil {
		return Task{}, err
	}
	if input.Title != nil {
		title, err := validateTitle(*input.Title)
		if err != nil {
			return Task{}, err
		}
		task.Title = title
	}
	if input.Done != nil {
		task.Done = *input.Done
	}
	return s.repo.Save(task), nil
}

func (s *Service) Delete(id string) error {
	if !s.repo.Delete(id) {
		return fmt.Errorf("tarea %s: %w", id, ErrNotFound)
	}
	return nil
}
```

```go
package tasks

import (
	"encoding/json"
	"errors"
	"net/http"
)

// Handler es la capa HTTP: traduce peticiones a llamadas al servicio y errores a códigos.
type Handler struct {
	service *Service
}

func NewHandler(service *Service) *Handler {
	return &Handler{service: service}
}

// Routes registra las rutas. Desde Go 1.22 el patrón admite método y parámetros: "GET /api/tasks/{id}".
func (h *Handler) Routes() http.Handler {
	mux := http.NewServeMux()
	mux.HandleFunc("GET /api/tasks", h.list)
	mux.HandleFunc("GET /api/tasks/{id}", h.get)
	mux.HandleFunc("POST /api/tasks", h.create)
	mux.HandleFunc("PATCH /api/tasks/{id}", h.update)
	mux.HandleFunc("DELETE /api/tasks/{id}", h.delete)
	return mux
}

func writeJSON(w http.ResponseWriter, status int, body any) {
	w.Header().Set("Content-Type", "application/json")
	w.WriteHeader(status)
	if body != nil {
		_ = json.NewEncoder(w).Encode(body)
	}
}

// writeError convierte un error del dominio en una respuesta HTTP.
func writeError(w http.ResponseWriter, err error) {
	switch {
	case errors.Is(err, ErrNotFound):
		writeJSON(w, http.StatusNotFound, map[string]string{"error": err.Error()})
	case errors.Is(err, ErrValidation):
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": err.Error()})
	default:
		writeJSON(w, http.StatusInternalServerError, map[string]string{"error": "Error interno"})
	}
}

func (h *Handler) list(w http.ResponseWriter, _ *http.Request) {
	writeJSON(w, http.StatusOK, h.service.List())
}

func (h *Handler) get(w http.ResponseWriter, r *http.Request) {
	task, err := h.service.Get(r.PathValue("id"))
	if err != nil {
		writeError(w, err)
		return
	}
	writeJSON(w, http.StatusOK, task)
}

func (h *Handler) create(w http.ResponseWriter, r *http.Request) {
	var input CreateInput
	if err := json.NewDecoder(r.Body).Decode(&input); err != nil {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "JSON inválido"})
		return
	}
	task, err := h.service.Create(input)
	if err != nil {
		writeError(w, err)
		return
	}
	writeJSON(w, http.StatusCreated, task)
}

func (h *Handler) update(w http.ResponseWriter, r *http.Request) {
	var input UpdateInput
	if err := json.NewDecoder(r.Body).Decode(&input); err != nil {
		writeJSON(w, http.StatusBadRequest, map[string]string{"error": "JSON inválido"})
		return
	}
	task, err := h.service.Update(r.PathValue("id"), input)
	if err != nil {
		writeError(w, err)
		return
	}
	writeJSON(w, http.StatusOK, task)
}

func (h *Handler) delete(w http.ResponseWriter, r *http.Request) {
	if err := h.service.Delete(r.PathValue("id")); err != nil {
		writeError(w, err)
		return
	}
	w.WriteHeader(http.StatusNoContent)
}
```

```go
package tasks

import "net/http"

// WithCORS permite que el navegador llame a la API desde otro origen.
// Un middleware es una función que envuelve a un manejador y añade comportamiento.
func WithCORS(origin string, next http.Handler) http.Handler {
	return http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		w.Header().Set("Access-Control-Allow-Origin", origin)
		w.Header().Set("Access-Control-Allow-Methods", "GET, POST, PATCH, DELETE")
		w.Header().Set("Access-Control-Allow-Headers", "Content-Type")
		if r.Method == http.MethodOptions { // petición previa (preflight) del navegador
			w.WriteHeader(http.StatusNoContent)
			return
		}
		next.ServeHTTP(w, r)
	})
}
```

```go
// Command server arranca la API de tareas.
package main

import (
	"log"
	"net/http"
	"os"

	"example.com/tasks/internal/tasks"
)

// getenv devuelve la variable de entorno o un valor por defecto.
func getenv(key, fallback string) string {
	if value := os.Getenv(key); value != "" {
		return value
	}
	return fallback
}

func main() {
	port := getenv("PORT", "3000")
	origin := getenv("CORS_ORIGIN", "http://localhost:5173")

	service := tasks.NewService(tasks.NewMemoryRepository())
	handler := tasks.WithCORS(origin, tasks.NewHandler(service).Routes())

	server := &http.Server{Addr: ":" + port, Handler: handler}
	log.Printf("escuchando en http://localhost:%s", port)
	log.Fatal(server.ListenAndServe())
}
```

| Pieza | Significado |
|---|---|
| `http.NewServeMux()` | Enrutador estándar |
| `"GET /api/tasks/{id}"` | Patrón con método y parámetro (Go 1.22+) |
| `r.PathValue("id")` | Lee el parámetro de ruta |
| `json.NewDecoder(r.Body).Decode(&x)` | Deserializa el JSON del cuerpo |
| `json.NewEncoder(w).Encode(x)` | Serializa a JSON en la respuesta |
| `*string` / `*bool` en `UpdateInput` | Puntero `nil` = campo no enviado |
| `errors.Is(err, ErrNotFound)` | Traduce errores del dominio a `404`/`400` |
| `WithCORS` | Middleware: función que envuelve al manejador |
| `os.Getenv` | Configuración por variables de entorno |

```bash
cd ~/projects/tasks-go
PORT=3000 go run ./cmd/server
```

Esta API es **intercambiable** con las de [Node.js](#node), [Python](#python) y [Spring Boot](#spring): mismas rutas y mismo JSON; el frontend no cambia.

## 12. Pruebas, depuración, formato y análisis

```go
package tasks

import (
	"net/http"
	"net/http/httptest"
	"strings"
	"testing"
)

func do(t *testing.T, handler http.Handler, method, path, body string) *httptest.ResponseRecorder {
	t.Helper()
	request := httptest.NewRequest(method, path, strings.NewReader(body))
	recorder := httptest.NewRecorder()
	handler.ServeHTTP(recorder, request)
	return recorder
}

func TestCRUDFlow(t *testing.T) {
	handler := NewHandler(NewService(NewMemoryRepository())).Routes()

	created := do(t, handler, "POST", "/api/tasks", `{"title":"Estudiar Go"}`)
	if created.Code != http.StatusCreated {
		t.Fatalf("crear: esperaba 201 y llegó %d", created.Code)
	}

	if invalid := do(t, handler, "POST", "/api/tasks", `{"title":"x"}`); invalid.Code != http.StatusBadRequest {
		t.Fatalf("validar: esperaba 400 y llegó %d", invalid.Code)
	}

	patched := do(t, handler, "PATCH", "/api/tasks/1", `{"done":true}`)
	if !strings.Contains(patched.Body.String(), `"done":true`) {
		t.Fatalf("actualizar: cuerpo inesperado %s", patched.Body.String())
	}

	if deleted := do(t, handler, "DELETE", "/api/tasks/1", ""); deleted.Code != http.StatusNoContent {
		t.Fatalf("eliminar: esperaba 204 y llegó %d", deleted.Code)
	}
	if missing := do(t, handler, "GET", "/api/tasks/1", ""); missing.Code != http.StatusNotFound {
		t.Fatalf("consultar: esperaba 404 y llegó %d", missing.Code)
	}
}

// Prueba de tabla: el patrón habitual en Go para probar varios casos
func TestValidateTitle(t *testing.T) {
	cases := []struct {
		name    string
		input   string
		want    string
		wantErr bool
	}{
		{"normal", "Estudiar", "Estudiar", false},
		{"con espacios", "  Estudiar  ", "Estudiar", false},
		{"muy corto", "x", "", true},
		{"vacío", "   ", "", true},
	}
	for _, tc := range cases {
		t.Run(tc.name, func(t *testing.T) {
			got, err := validateTitle(tc.input)
			if (err != nil) != tc.wantErr {
				t.Fatalf("error = %v, wantErr = %v", err, tc.wantErr)
			}
			if got != tc.want {
				t.Fatalf("got %q, want %q", got, tc.want)
			}
		})
	}
}
```

```bash
go test ./...
go test -v -run TestValidateTitle ./internal/tasks
go test -race -cover ./...
go vet ./...
gofmt -l .
go fmt ./...
```

| Comando / flag | Significado |
|---|---|
| `go test ./...` | Prueba todos los paquetes (`./...` = esta carpeta y todas las de dentro) |
| `-v` | Detallado |
| `-run Patrón` | Solo las pruebas cuyo nombre coincida |
| `-race` | Detector de condiciones de carrera |
| `-cover` | Porcentaje de cobertura |
| `go vet` | Busca errores sospechosos |
| `gofmt -l .` | Lista los archivos mal formateados; `go fmt ./...` los arregla |
| `httptest.NewRequest` / `NewRecorder` | Simulan petición y respuesta sin red |

Las pruebas de tabla (`TestValidateTitle`) son el patrón habitual: una lista de casos y un `t.Run` por cada uno. Linter popular: `golangci-lint`. Depurador: Delve (`dlv`), integrado en VS Code.

## 13. Compilar y empaquetar

```bash
go build -o bin/server ./cmd/server
GOOS=linux GOARCH=amd64 go build -o bin/server-linux ./cmd/server
CGO_ENABLED=0 go build -ldflags="-s -w" -o bin/server ./cmd/server
```

| Pieza | Significado |
|---|---|
| `go build -o ruta` | Genera el binario en esa ruta |
| `GOOS` / `GOARCH` | **Compilación cruzada** para otro sistema o arquitectura |
| `CGO_ENABLED=0` | Binario totalmente estático (sin librerías C del sistema) |
| `-ldflags="-s -w"` | Quita tablas de símbolos y depuración: binario más pequeño |

Imagen Docker mínima (construcción en dos etapas):

```dockerfile
FROM golang:1.24 AS build
WORKDIR /src
COPY go.mod ./
RUN go mod download
COPY . .
RUN CGO_ENABLED=0 go build -o /out/server ./cmd/server

FROM gcr.io/distroless/static-debian12
COPY --from=build /out/server /server
EXPOSE 3000
ENTRYPOINT ["/server"]
```

## 14. Errores habituales

| Mensaje | Causa | Solución |
|---|---|---|
| `mise ERROR No version is set for shim: go` | Ninguna versión activada | `mise use -g go@<versión>` |
| `declared and not used: x` / `"os" imported and not used` | Go prohíbe variables e imports sin usar | Úsalos o bórralos (o `_ =` para ignorar) |
| `go: go.mod file not found` | No hay módulo en esa carpeta | `go mod init nombre` |
| `package X is not in std` / `no required module provides package` | Falta la dependencia | `go get` o `go mod tidy` |
| `missing return` | Una rama de la función no devuelve valor | Añade el `return` |
| `cannot use x (variable of type int) as float64` | No hay conversiones implícitas | `float64(x)` |
| `assignment to entry in nil map` | Escribes en un mapa sin inicializar | `m := map[string]int{}` o `make(...)` |
| `invalid memory address or nil pointer dereference` | Usas un puntero `nil` | Comprueba `nil` antes |
| `all goroutines are asleep - deadlock!` | Todos esperan un canal que nadie atiende | Revisa envíos, recepciones y `close` |
| `fatal error: concurrent map writes` | Varias goroutines escriben el mismo mapa | `sync.Mutex` o `sync.Map` |
| `address already in use` | Puerto ocupado | `ss -ltnp \| grep 3000` |

## 15. Ejercicios

> **Ejercicio:** resuelve los ejercicios 1–15 de [lógica y programación](#ejercicios-basicos) en Go con pruebas de tabla.

> **Ejercicio:** implementa `Map`, `Filter` y `Reduce` genéricos (ya están arriba) y úsalos para el análisis de empleados.

> **Ejercicio:** añade `GET /api/tasks?done=true` leyendo `r.URL.Query().Get("done")`, con su prueba.

> **Ejercicio:** lanza 1000 goroutines que incrementen un contador, primero sin `Mutex` y ejecutando `go test -race`; después corrígelo.

> **Ejercicio:** añade un middleware de registro que escriba método, ruta y duración de cada petición.

## 16. Documentación oficial

- [Tour de Go](https://go.dev/tour/)
- [Effective Go](https://go.dev/doc/effective_go)
- [Librería estándar](https://pkg.go.dev/std)
- [net/http](https://pkg.go.dev/net/http)
- [Cómo escribir código Go](https://go.dev/doc/code)
