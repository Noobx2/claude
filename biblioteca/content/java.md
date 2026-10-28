# Java (con Maven)

Java es un lenguaje **estático y orientado a objetos** que se compila a *bytecode* y se ejecuta en la JVM. Es el estándar en backends empresariales (Spring Boot), herramientas de datos y Android (junto con Kotlin). Tiene un ecosistema enorme y garantías fuertes de compatibilidad.

*Verificado: 2026-10-27. Los ejemplos se han compilado y ejecutado con JDK 21 y Maven 3.9; tu equipo tiene JDK 25.0.4.1 LTS y Maven 3.10.0, y todo lo que sigue funciona igual (cambia `maven.compiler.release` a 25). Referencia: [dev.java](https://dev.java/learn/) y [maven.apache.org](https://maven.apache.org/guides/).*

## 1. Para qué sirve

| Uso | Ejemplo |
|---|---|
| APIs y servicios | Spring Boot (ver [Spring Boot](#spring)) |
| Aplicaciones de escritorio | JavaFX, Swing |
| Android | Kotlin es lo recomendado, Java sigue siendo válido ([Android](#android)) |
| Procesamiento de datos | Kafka, Spark, Elasticsearch |

## 2. Instalación, versiones y ejecución

Tu JDK (Temurin 25.0.4.1) y Maven (3.10.0) están instalados con SDKMAN.

```bash
java -version
javac -version
mvn --version
```

| Comando | Significado |
|---|---|
| `java -version` | Versión de la JVM (un solo guion en `java`; `--version` también funciona en JDK recientes) |
| `javac Archivo.java` | Compila a `Archivo.class` (bytecode) |
| `java Archivo` | Ejecuta la clase compilada |
| `java Archivo.java` | **Modo archivo fuente**: compila y ejecuta en un paso (un solo archivo) |
| `jshell` | REPL interactivo de Java |
| `mvn --version` | Versión de Maven y del JDK que usa |

JDK frente a JRE: el **JDK** incluye compilador y herramientas; el **JRE** solo ejecuta. Para desarrollar necesitas el JDK. Cambiar de versión: `sdk use java <versión>` ([Gestores de versiones](#entorno-versiones)).

## 3. Archivos y convenciones de nombres

| Elemento | Convención |
|---|---|
| Archivo | `PascalCase.java`; **debe llamarse igual que la clase pública** |
| Clases, interfaces, records, enums | `PascalCase` |
| Métodos y variables | `camelCase` |
| Constantes | `UPPER_SNAKE_CASE` (`static final`) |
| Paquetes | minúsculas, dominio inverso: `com.example.tasks` |
| Carpetas | La estructura de carpetas **refleja los paquetes** |
| Llaves y `;` | Obligatorios |

## 4. Hola mundo

```java
// Hello.java
public class Hello {
    public static void main(String[] args) {
        System.out.println("Hola mundo");
    }
}
```

```bash
java Hello.java
```

| Pieza | Significado |
|---|---|
| `public class Hello` | Toda la lógica vive en clases |
| `public static void main(String[] args)` | Punto de entrada: público, sin objeto previo (`static`), sin valor de retorno (`void`), con los argumentos de la línea de comandos |
| `System.out.println` | Imprime con salto de línea (`print` sin salto, `printf` con formato) |

En Java 25 existe una forma abreviada (clases compactas con `void main()` y `IO.println`) pensada para aprender; no se ha podido probar aquí, así que consulta [dev.java](https://dev.java/) para tu versión. El estilo clásico de arriba funciona siempre.

## 5. Variables, tipos y conversiones

```java
// Basics.java: variables, tipos, condiciones, bucles y funciones
import java.util.List;

public class Basics {

    // Constante de clase
    static final int MAX_TRIES = 3;

    // Función (en Java: "método"). Debe declarar tipos de parámetros y de retorno
    static int add(int a, int b) {
        return a + b;
    }

    // Sobrecarga: mismo nombre, distintos parámetros
    static int add(int a, int b, int c) {
        return a + b + c;
    }

    static String grade(int score) {
        // switch como expresión (Java 14+): devuelve un valor, sin "break"
        return switch (score / 10) {
            case 10, 9 -> "A";
            case 8, 7 -> "B";
            default -> "C";
        };
    }

    public static void main(String[] args) {
        String name = "Ana";          // String (objeto, inmutable)
        int age = 31;                 // int: entero de 32 bits (primitivo)
        long big = 3_000_000_000L;    // long: 64 bits (la L final y los _ son opcionales y legibles)
        double height = 1.68;         // double: decimal de 64 bits
        boolean student = false;
        char letter = 'A';            // un solo carácter, comillas simples
        var inferred = 42;            // var: el compilador deduce int (sigue siendo tipado estático)

        System.out.println(name + " " + age + " " + big + " " + height + " " + student + " " + letter + " " + inferred);
        System.out.printf("%s mide %.1f m%n", name, height);   // %s texto, %.1f un decimal, %n salto

        // Conversiones
        int parsed = Integer.parseInt("42");
        double ratio = (double) 7 / 2;       // cast: 3.5 (sin cast, 7 / 2 sería 3)
        String text = String.valueOf(7) + "!";
        System.out.println(parsed + " " + ratio + " " + text + " " + 7 / 2 + " " + 7 % 2);

        // Comparar textos: equals, NUNCA ==
        String a = new String("hola");
        System.out.println(a.equals("hola") + " " + (a == "hola"));

        System.out.println(grade(95) + grade(72) + grade(40));
        System.out.println(add(2, 3) + " " + add(1, 2, 3));

        for (int i = 1; i <= 3; i++) {
            System.out.println("vuelta " + i);
        }
        for (String item : List.of("a", "b")) {       // for-each
            System.out.println(item);
        }
        int count = 3;
        while (count > 0) {
            count--;
        }

        // Bloque de texto (Java 15+)
        String json = """
                {
                  "name": "Ana",
                  "age": 31
                }
                """;
        System.out.print(json);

        // Valor ausente: null; Optional para expresarlo en el tipo
        String maybe = null;
        System.out.println(maybe == null ? "sin valor" : maybe.length());
    }
}
```

| Tipo primitivo | Tamaño | Ejemplo |
|---|---|---|
| `byte`, `short`, `int`, `long` | 8, 16, 32, 64 bits | `int n = 42;` `long big = 1L;` |
| `float`, `double` | 32, 64 bits | `double x = 1.5;` |
| `boolean` | | `true`, `false` |
| `char` | 16 bits | `'A'` (comillas simples) |
| `String` | Objeto | `"texto"` (comillas dobles), inmutable |

Los primitivos tienen su **envoltorio** para colecciones: `Integer`, `Long`, `Double`, `Boolean`. Java los convierte solo (*autoboxing*).

> **Ojo:** compara textos con `a.equals(b)`, nunca con `==` (que compara si son el *mismo objeto*). Y `7 / 2` entre enteros es `3`: la división entera descarta los decimales.

## 6. Colecciones y streams

```java
// CollectionsDemo.java: colecciones y streams con el ejemplo de empleados
import java.util.*;
import java.util.stream.*;

public class CollectionsDemo {

    record Employee(int id, String name, String department, int salary, int age, boolean active) {}

    public static void main(String[] args) {
        List<Employee> employees = List.of(
                new Employee(1, "Ana", "ventas", 2400, 31, true),
                new Employee(2, "Luis", "tecnología", 3600, 28, true),
                new Employee(3, "Marta", "tecnología", 4100, 41, false),
                new Employee(4, "Pablo", "ventas", 2100, 24, true),
                new Employee(5, "Sara", "recursos", 2800, 36, true),
                new Employee(6, "Iván", "tecnología", 3300, 33, true));

        System.out.println(employees.stream().filter(Employee::active).map(Employee::name).toList());

        int total = employees.stream().mapToInt(Employee::salary).sum();
        double average = employees.stream().mapToInt(Employee::salary).average().orElse(0);
        System.out.println(total + " " + average);

        System.out.println(employees.stream()
                .sorted(Comparator.comparingInt(Employee::salary).reversed())
                .limit(3).map(Employee::name).toList());

        System.out.println(employees.stream().filter(e -> e.age() > 40).findFirst().map(Employee::name).orElse("nadie"));
        System.out.println(employees.stream().allMatch(e -> e.salary() > 2000) + " "
                + employees.stream().anyMatch(e -> !e.active()));
        System.out.println(employees.stream().map(Employee::name).collect(Collectors.joining(", ")));

        // Agrupar: masa salarial por departamento (TreeMap para orden estable al imprimir)
        Map<String, Integer> payroll = employees.stream().collect(
                Collectors.groupingBy(Employee::department, TreeMap::new, Collectors.summingInt(Employee::salary)));
        System.out.println(payroll);

        // Colecciones clásicas
        List<Integer> numbers = new ArrayList<>(List.of(3, 1, 4, 1, 5, 9, 2, 6, 5, 3, 5));
        Set<Integer> unique = new TreeSet<>(numbers);              // sin repetidos y ordenado
        Map<Integer, Integer> frequency = new TreeMap<>();
        for (int n : numbers) {
            frequency.merge(n, 1, Integer::sum);                   // si existe suma 1, si no pone 1
        }
        System.out.println(unique + " " + frequency);
        Collections.sort(numbers);                                  // OJO: modifica la lista
        System.out.println(numbers);
    }
}
```

| Colección | Para qué |
|---|---|
| `List<T>` (`ArrayList`) | Lista ordenada con índices |
| `Set<T>` (`HashSet`, `TreeSet`) | Sin repetidos |
| `Map<K,V>` (`HashMap`, `TreeMap`, `LinkedHashMap`) | Diccionario |
| `Deque<T>` (`ArrayDeque`) | Pila y cola |
| `List.of(…)`, `Map.of(…)` | Colecciones **inmutables** (lanzan excepción si intentas modificarlas) |

| Operación | Streams |
|---|---|
| Filtrar | `.filter(e -> …)` |
| Transformar | `.map(Employee::name)` |
| Sumar | `.mapToInt(…).sum()` |
| Ordenar | `.sorted(Comparator.comparing(…))` |
| Alguno / todos | `anyMatch`, `allMatch` |
| Primero | `findFirst()` → `Optional` |
| Agrupar | `Collectors.groupingBy(…)` |
| Unir texto | `Collectors.joining(", ")` |
| A lista | `.toList()` |

Los genéricos (`List<String>`) indican qué tipo contiene la colección y lo comprueba el compilador. `::` (`Employee::name`) es una **referencia a método**.

## 7. Errores y excepciones

```java
// Errors.java: excepciones y archivos
import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.util.List;

public class Errors {

    static class InsufficientFundsException extends Exception {   // comprobada: hay que declararla o capturarla
        InsufficientFundsException(String message) {
            super(message);
        }
    }

    static void withdraw(double balance, double amount) throws InsufficientFundsException {
        if (amount > balance) {
            throw new InsufficientFundsException("Saldo insuficiente: " + balance);
        }
    }

    public static void main(String[] args) throws IOException {
        try {
            withdraw(100, 500);
        } catch (InsufficientFundsException e) {
            System.out.println("No se pudo: " + e.getMessage());
        }

        try {
            Integer.parseInt("abc");
        } catch (NumberFormatException e) {
            System.out.println("Error: " + e.getMessage());
        } finally {
            System.out.println("finally se ejecuta siempre");
        }

        // Archivos con NIO (Path y Files)
        Path path = Path.of("demo.txt");
        Files.writeString(path, "uno\ndos\ntres\n");
        List<String> lines = Files.readAllLines(path);
        System.out.println(lines + " " + Files.exists(path));

        // try-with-resources: cierra el recurso solo
        try (var stream = Files.lines(path)) {
            System.out.println(stream.filter(l -> l.contains("o")).count());
        }
        Files.delete(path);
    }
}
```

| Concepto | Significado |
|---|---|
| **Comprobadas** (`extends Exception`) | El compilador te obliga a capturarlas o declararlas con `throws`. Casos recuperables (IO) |
| **No comprobadas** (`extends RuntimeException`) | Errores de programación (`NullPointerException`, `IllegalArgumentException`) |
| `try-with-resources` | Cierra automáticamente lo que implementa `AutoCloseable` |
| `Optional<T>` | Expresa «puede no haber valor» en el tipo en lugar de `null` |

## 8. Programación orientada a objetos

El proyecto de abajo muestra los conceptos en un caso real:

- **Clase / record**: `Task` es un `record` (inmutable, con `equals`, `hashCode` y `toString` generados).
- **Interfaz**: `TaskRepository` define el contrato; el servicio no sabe qué implementación hay detrás.
- **Implementación**: `InMemoryTaskRepository implements TaskRepository`.
- **Encapsulación**: atributos `private final`; solo se accede por métodos.
- **Herencia**: `extends` (una clase hereda de otra); `RuntimeException` es la base de nuestra excepción.
- **Polimorfismo**: una variable de tipo `TaskRepository` puede guardar cualquier implementación.
- **Inyección por constructor**: `new TaskService(repository)`; así se cambia la implementación (por ejemplo, en pruebas).

| Modificador | Visible desde |
|---|---|
| `public` | Todos |
| `protected` | Mismo paquete y subclases |
| (nada) | Mismo paquete |
| `private` | Solo la propia clase |

## 9. Concurrencia

```java
import java.util.concurrent.Executors;

public class Concurrency {
    public static void main(String[] args) throws Exception {
        // Hilos virtuales (Java 21+): baratos, uno por tarea
        try (var executor = Executors.newVirtualThreadPerTaskExecutor()) {
            var future = executor.submit(() -> {
                Thread.sleep(200);
                return "listo";
            });
            System.out.println(future.get());
        }
    }
}
```

`ExecutorService` ejecuta tareas en hilos; `Future.get()` espera el resultado; `CompletableFuture` encadena operaciones asíncronas. Evita compartir estado mutable entre hilos sin sincronización (`synchronized`, `AtomicLong`, colecciones concurrentes).

## 10. Un proyecto Maven completo

**Maven** es el gestor de dependencias y de construcción. Su archivo es `pom.xml` (*Project Object Model*).

Estructura (la convención de Maven; no se negocia):

```text
tasks-java/
  pom.xml
  src/
    main/java/com/example/tasks/      código de producción
      Task.java
      TaskRepository.java
      InMemoryTaskRepository.java
      TaskNotFoundException.java
      TaskService.java
      Main.java
    test/java/com/example/tasks/      pruebas
      TaskServiceTest.java
  target/                              generado por Maven (no se versiona)
```

### `pom.xml`

```xml
<?xml version="1.0" encoding="UTF-8"?>
<project xmlns="http://maven.apache.org/POM/4.0.0"
         xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
         xsi:schemaLocation="http://maven.apache.org/POM/4.0.0 https://maven.apache.org/xsd/maven-4.0.0.xsd">
  <modelVersion>4.0.0</modelVersion>

  <!-- Coordenadas: identifican el proyecto de forma única (groupId:artifactId:version) -->
  <groupId>com.example</groupId>
  <artifactId>tasks-java</artifactId>
  <version>0.1.0</version>
  <packaging>jar</packaging>

  <properties>
    <!-- Versión de Java con la que se compila: pon la de tu JDK -->
    <maven.compiler.release>21</maven.compiler.release>
    <project.build.sourceEncoding>UTF-8</project.build.sourceEncoding>
  </properties>

  <dependencies>
    <dependency>
      <groupId>com.fasterxml.jackson.core</groupId>
      <artifactId>jackson-databind</artifactId>
      <version>2.22.4</version>
    </dependency>
    <dependency>
      <groupId>org.junit.jupiter</groupId>
      <artifactId>junit-jupiter</artifactId>
      <version>6.1.3</version>
      <scope>test</scope>
    </dependency>
  </dependencies>

  <build>
    <plugins>
      <plugin>
        <groupId>org.apache.maven.plugins</groupId>
        <artifactId>maven-compiler-plugin</artifactId>
        <version>3.15.0</version>
      </plugin>
      <plugin>
        <groupId>org.apache.maven.plugins</groupId>
        <artifactId>maven-surefire-plugin</artifactId>
        <version>3.6.1</version>
      </plugin>
      <plugin>
        <groupId>org.codehaus.mojo</groupId>
        <artifactId>exec-maven-plugin</artifactId>
        <version>3.6.4</version>
        <configuration>
          <mainClass>com.example.tasks.Main</mainClass>
        </configuration>
      </plugin>
    </plugins>
  </build>
</project>
```

| Elemento | Significado |
|---|---|
| `groupId:artifactId:version` | **Coordenadas**: identifican el proyecto o una dependencia en cualquier parte del mundo |
| `packaging` | Tipo de artefacto: `jar` (por defecto), `war`, `pom` |
| `properties` | Variables: `maven.compiler.release` fija la versión de Java |
| `dependencies` | Librerías necesarias; se descargan de Maven Central |
| `scope` | `compile` (por defecto), `test` (solo pruebas), `provided`, `runtime` |
| `build/plugins` | Plugins con versión fija; hacen el trabajo (compilar, probar, ejecutar) |

> **Ojo:** fija siempre la versión de los plugins y las dependencias. Versiones en rangos o sin fijar hacen las compilaciones irreproducibles.

### El código

```java
package com.example.tasks;

/**
 * Una tarea. Un "record" es una clase inmutable para guardar datos:
 * genera constructor, getters (title(), done()...), equals, hashCode y toString.
 */
public record Task(long id, String title, boolean done) {

    // Constructor compacto: valida al crear
    public Task {
        if (title == null || title.isBlank()) {
            throw new IllegalArgumentException("title no puede estar vacío");
        }
    }

    // Los records son inmutables: "modificar" es crear una copia con el cambio
    public Task withDone(boolean newDone) {
        return new Task(id, title, newDone);
    }
}
```

```java
package com.example.tasks;

import java.util.List;
import java.util.Optional;

/** Contrato de acceso a datos: el servicio depende de esta interfaz, no de una implementación. */
public interface TaskRepository {
    List<Task> findAll();

    Optional<Task> findById(long id);

    Task save(Task task);

    boolean deleteById(long id);

    long nextId();
}
```

```java
package com.example.tasks;

import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;
import java.util.Optional;

public class InMemoryTaskRepository implements TaskRepository {

    private final Map<Long, Task> tasks = new LinkedHashMap<>();
    private long lastId = 0;

    @Override
    public List<Task> findAll() {
        return new ArrayList<>(tasks.values()); // copia: nadie modifica nuestra colección interna
    }

    @Override
    public Optional<Task> findById(long id) {
        return Optional.ofNullable(tasks.get(id));
    }

    @Override
    public Task save(Task task) {
        tasks.put(task.id(), task);
        return task;
    }

    @Override
    public boolean deleteById(long id) {
        return tasks.remove(id) != null;
    }

    @Override
    public long nextId() {
        lastId += 1;
        return lastId;
    }
}
```

```java
package com.example.tasks;

/** Excepción propia (no comprobada): hereda de RuntimeException. */
public class TaskNotFoundException extends RuntimeException {
    public TaskNotFoundException(long id) {
        super("No existe la tarea " + id);
    }
}
```

```java
package com.example.tasks;

import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

/** Reglas de negocio. No sabe si los datos están en memoria, en una base de datos o en HTTP. */
public class TaskService {

    private final TaskRepository repository;

    public TaskService(TaskRepository repository) {
        this.repository = repository;
    }

    public Task create(String title) {
        return repository.save(new Task(repository.nextId(), title.strip(), false));
    }

    public Task get(long id) {
        return repository.findById(id).orElseThrow(() -> new TaskNotFoundException(id));
    }

    public Task setDone(long id, boolean done) {
        return repository.save(get(id).withDone(done));
    }

    public void delete(long id) {
        if (!repository.deleteById(id)) {
            throw new TaskNotFoundException(id);
        }
    }

    public List<Task> list() {
        return repository.findAll();
    }

    /** Streams: filtrar, transformar y recoger en una colección. */
    public List<String> pendingTitles() {
        return repository.findAll().stream()
                .filter(task -> !task.done())
                .map(Task::title)
                .toList();
    }

    /** Agrupar: true -> hechas, false -> pendientes. */
    public Map<Boolean, Long> countByDone() {
        return repository.findAll().stream()
                .collect(Collectors.partitioningBy(Task::done, Collectors.counting()));
    }
}
```

```java
package com.example.tasks;

import com.fasterxml.jackson.databind.ObjectMapper;

public class Main {

    public static void main(String[] args) throws Exception {
        TaskService service = new TaskService(new InMemoryTaskRepository());

        service.create("Estudiar Java");
        service.create("Probar Maven");
        service.setDone(1, true);

        System.out.println("Pendientes: " + service.pendingTitles());
        System.out.println("Conteo: " + service.countByDone());

        // JSON con Jackson: objeto -> texto
        ObjectMapper mapper = new ObjectMapper();
        String json = mapper.writeValueAsString(service.list());
        System.out.println(json);

        // texto -> objeto
        Task[] parsed = mapper.readValue(json, Task[].class);
        System.out.println(parsed.length + " tareas leídas; la primera: " + parsed[0]);
    }
}
```

### JSON con Jackson

`Main` usa `ObjectMapper` (Jackson): `writeValueAsString(objeto)` convierte a JSON y `readValue(texto, Clase.class)` reconstruye objetos. Los `record` se serializan sin configuración adicional en las versiones actuales.

### Pruebas con JUnit

```java
package com.example.tasks;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

class TaskServiceTest {

    private TaskService service;

    @BeforeEach
    void setUp() {
        service = new TaskService(new InMemoryTaskRepository());
    }

    @Test
    void createsTaskWithTrimmedTitle() {
        Task task = service.create("  Estudiar  ");
        assertEquals("Estudiar", task.title());
        assertEquals(false, task.done());
    }

    @Test
    void rejectsBlankTitle() {
        assertThrows(IllegalArgumentException.class, () -> service.create("   "));
    }

    @Test
    void throwsWhenTaskDoesNotExist() {
        assertThrows(TaskNotFoundException.class, () -> service.get(99));
    }

    @Test
    void marksTaskAsDone() {
        Task created = service.create("Probar");
        assertTrue(service.setDone(created.id(), true).done());
        assertEquals(0, service.pendingTitles().size());
    }
}
```

| Anotación / método | Significado |
|---|---|
| `@Test` | Método de prueba |
| `@BeforeEach` | Se ejecuta antes de cada prueba (prepara un estado limpio) |
| `assertEquals(esperado, real)` | Comprueba igualdad (el **esperado va primero**) |
| `assertThrows(Clase.class, () -> …)` | Comprueba que se lanza esa excepción |

## 11. Ciclo de vida y comandos de Maven

```bash
mvn -B test
mvn -B -DskipTests package
mvn -B -Dtest='TaskServiceTest#rejectsBlankTitle' test
mvn -B compile exec:java
mvn dependency:tree
```

| Comando / flag | Significado |
|---|---|
| `mvn clean` | Borra `target/` |
| `mvn compile` | Compila `src/main` |
| `mvn test` | Compila y ejecuta las pruebas |
| `mvn package` | Genera el `.jar` en `target/` (ejecuta antes `test`) |
| `mvn verify` | Ejecuta además las pruebas de integración |
| `mvn install` | Copia el artefacto a tu repositorio local `~/.m2` |
| `-B` | *Batch*: salida sin colores ni barras (ideal para CI y registros) |
| `-q` | *Quiet*: solo errores |
| `-DskipTests` | Compila pero no ejecuta pruebas |
| `-Dtest='Clase#método'` | Ejecuta solo esa prueba |
| `-o` | *Offline*: usa solo lo descargado |
| `-U` | Fuerza la actualización de dependencias |
| `-pl modulo` | Solo ese módulo (proyectos multimódulo) |
| `dependency:tree` | Árbol de dependencias (incluye las transitivas) |
| `help:effective-pom` | El POM final con todo heredado |

El **ciclo de vida** es una secuencia de fases: `validate → compile → test → package → verify → install → deploy`. Pedir una fase ejecuta **todas las anteriores**.

Ejecutar desde la línea de comandos sin plugin: `java -cp target/classes:<dependencias> com.example.tasks.Main`. Para un `.jar` ejecutable con dependencias se usa el plugin Shade o, en Spring Boot, `spring-boot-maven-plugin`.

**Wrapper** (`mvnw`): `mvn wrapper:wrapper` añade scripts que descargan la versión de Maven del proyecto: así cualquiera compila sin instalar Maven.

Dónde están las dependencias: `~/.m2/repository` (caché global). Maven Central es el repositorio por defecto.

## 12. Formato y análisis estático

| Herramienta | Uso |
|---|---|
| Formato | Google Java Format, Spotless (plugin Maven) |
| Análisis | Checkstyle, SpotBugs, Error Prone |
| IDE | IntelliJ IDEA, VS Code con la extensión de Java |

## 13. Estructura recomendada para una aplicación real

```text
src/main/java/com/example/app/
  domain/        entidades y reglas puras
  service/       casos de uso
  repository/    acceso a datos (interfaces + implementaciones)
  web/           controladores (en Spring Boot)
  config/        configuración
```

## 14. Errores habituales

| Mensaje | Causa | Solución |
|---|---|---|
| `class X is public, should be declared in a file named X.java` | El archivo no se llama como la clase | Renombra |
| `cannot find symbol` | Nombre mal escrito, falta `import` o dependencia | Revisa la ortografía y los imports |
| `incompatible types: possible lossy conversion from double to int` | Asignas un decimal a un entero | Cast explícito `(int)` |
| `NullPointerException` | Usas una referencia `null` | Comprueba antes, usa `Optional` |
| `error: release version 25 not supported` | Tu JDK es anterior al `release` del `pom.xml` | Ajusta `maven.compiler.release` o cambia de JDK con `sdk use` |
| `Could not resolve dependencies` | Coordenadas o versión erróneas, o sin red | Revisa `groupId`, `artifactId`, versión en [Maven Central](https://central.sonatype.com/) |
| `Unsupported class file major version` | JDK distinto del esperado por la herramienta | Alinea `JAVA_HOME` y la versión |
| `UnsupportedOperationException` | Modificas una colección creada con `List.of` | Copia a `new ArrayList<>(…)` |
| Tildes como `?` en consola | La consola no usa UTF-8 | Ejecuta con `-Dstdout.encoding=UTF-8` o configura el terminal en UTF-8 |
| `BUILD FAILURE` sin detalle | Se oculta con `-q` | Repite sin `-q`, o con `-e` para la traza completa |

## 15. Ejercicios

> **Ejercicio:** resuelve en Java los ejercicios 1–15 de [lógica y programación](#ejercicios-basicos) con un método estático cada uno y pruebas JUnit.

> **Ejercicio:** reproduce con streams el análisis de empleados: activos, masa salarial por departamento, mejor pagado, salario medio por departamento.

> **Ejercicio:** añade al proyecto la operación «buscar por texto» (`search(String)`), primero en el repositorio y luego en el servicio, con sus pruebas.

> **Ejercicio:** crea una segunda implementación de `TaskRepository` que guarde las tareas en un archivo JSON con Jackson. Cambia solo el `Main` para usarla.

> **Ejercicio:** ejecuta `mvn dependency:tree` y explica de dónde sale cada librería transitiva.

## 16. Documentación oficial

- [dev.java: aprender](https://dev.java/learn/)
- [Documentación de la API de Java](https://docs.oracle.com/en/java/javase/)
- [Maven: guías](https://maven.apache.org/guides/)
- [Maven Central: buscar dependencias](https://central.sonatype.com/)
- [JUnit](https://junit.org/)
