# Spring Boot

Spring Boot construye aplicaciones Java listas para ejecutar: un servidor web embebido, configuración automática y un modelo de **inyección de dependencias** que organiza el código en capas. Es el framework de backend más extendido en Java.

*Verificado: 2026-10-27. El proyecto de este módulo (Spring Boot 4.1.2, Java 21, Maven) se ha compilado, probado con una prueba de integración y ejecutado contra la interfaz React real del módulo de [React](#react). Referencia: [spring.io/projects/spring-boot](https://spring.io/projects/spring-boot).*

> **Ojo:** antes de empezar, domina [Java](#java) y [Maven](#java). Spring añade muchas anotaciones: es más sencillo si el lenguaje ya no te estorba.

## 1. Ideas clave

| Concepto | Significado |
|---|---|
| **Bean** | Objeto creado y gestionado por Spring |
| **Inyección de dependencias** | En lugar de hacer `new`, pides lo que necesitas por el constructor y Spring te lo da |
| **Anotación** | Marca (`@Service`, `@RestController`…) que le dice a Spring qué hacer con una clase o método |
| **Starter** | Dependencia que agrupa todo lo necesario para una función (web, validación, datos) |
| **Autoconfiguración** | Spring Boot configura sola cada starter según lo que encuentra en el classpath |
| **Perfil** | Configuración distinta por entorno (`dev`, `prod`) |

## 2. Crear el proyecto

Dos formas:

1. **Spring Initializr** ([start.spring.io](https://start.spring.io/)): eliges Maven, Java, versión de Spring Boot y dependencias, y descargas un `.zip`.
2. Escribir el `pom.xml` a mano, como en este módulo.

Dependencias iniciales: *Spring Web*, *Validation* y, más adelante, *Spring Data MongoDB* o *Spring Data JPA*.

## 3. Estructura

```text
tasks-spring/
  pom.xml
  src/
    main/
      java/com/example/tasksapi/
        TasksApiApplication.java     punto de entrada
        Task.java                    modelo de dominio
        CreateTaskRequest.java       DTO de entrada + validación
        UpdateTaskRequest.java       DTO de entrada
        TaskRepository.java          contrato de datos
        InMemoryTaskRepository.java  implementación en memoria
        TaskService.java             reglas de negocio
        TaskController.java          capa HTTP
        ApiExceptionHandler.java     errores → respuestas HTTP
        TaskNotFoundException.java
        WebConfig.java               CORS
      resources/
        application.properties       configuración
    test/java/com/example/tasksapi/
      TasksApiTest.java              prueba de integración
```

### `pom.xml`

```xml
<?xml version="1.0" encoding="UTF-8"?>
<project xmlns="http://maven.apache.org/POM/4.0.0"
         xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
         xsi:schemaLocation="http://maven.apache.org/POM/4.0.0 https://maven.apache.org/xsd/maven-4.0.0.xsd">
  <modelVersion>4.0.0</modelVersion>

  <!-- El "parent" aporta versiones coherentes de cientos de librerías y la configuración de plugins -->
  <parent>
    <groupId>org.springframework.boot</groupId>
    <artifactId>spring-boot-starter-parent</artifactId>
    <version>4.1.2</version>
    <relativePath/>
  </parent>

  <groupId>com.example</groupId>
  <artifactId>tasks-spring</artifactId>
  <version>0.1.0</version>

  <properties>
    <java.version>21</java.version>
  </properties>

  <dependencies>
    <dependency>
      <groupId>org.springframework.boot</groupId>
      <artifactId>spring-boot-starter-web</artifactId>
    </dependency>
    <dependency>
      <groupId>org.springframework.boot</groupId>
      <artifactId>spring-boot-starter-validation</artifactId>
    </dependency>
    <dependency>
      <groupId>org.springframework.boot</groupId>
      <artifactId>spring-boot-starter-test</artifactId>
      <scope>test</scope>
    </dependency>
  </dependencies>

  <build>
    <plugins>
      <plugin>
        <groupId>org.springframework.boot</groupId>
        <artifactId>spring-boot-maven-plugin</artifactId>
      </plugin>
    </plugins>
  </build>
</project>
```

| Pieza | Significado |
|---|---|
| `spring-boot-starter-parent` | Padre: fija versiones compatibles de todas las librerías (no las pones tú) y configura plugins |
| `spring-boot-starter-web` | Servidor web embebido (Tomcat), Spring MVC y JSON con Jackson |
| `spring-boot-starter-validation` | Validación con anotaciones (`@NotBlank`, `@Size`) |
| `spring-boot-starter-test` | JUnit, Mockito, utilidades de prueba |
| `spring-boot-maven-plugin` | Genera un `.jar` ejecutable con todo dentro y permite `mvn spring-boot:run` |

## 4. Las capas

```text
HTTP ─▶ Controller ─▶ Service ─▶ Repository ─▶ datos
         (HTTP)      (negocio)   (acceso)
```

> **Regla:** el controlador **no** contiene lógica de negocio. Recibe, delega en el servicio y devuelve. Las reglas viven en el servicio; el repositorio solo habla con los datos.

### Aplicación

```java
package com.example.tasksapi;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;

@SpringBootApplication // = configuración + exploración de componentes + autoconfiguración
public class TasksApiApplication {

    public static void main(String[] args) {
        SpringApplication.run(TasksApiApplication.class, args);
    }
}
```

`@SpringBootApplication` activa la exploración de componentes (busca clases anotadas en este paquete y sus subpaquetes), la autoconfiguración y la configuración propia.

### Modelo y DTO

```java
package com.example.tasksapi;

/** Entidad / modelo de dominio. */
public record Task(String id, String title, boolean done) {
    public Task withTitle(String newTitle) {
        return new Task(id, newTitle, done);
    }

    public Task withDone(boolean newDone) {
        return new Task(id, title, newDone);
    }
}
```

```java
package com.example.tasksapi;

import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.Size;

/** DTO de entrada: lo que el cliente envía para crear. Las anotaciones declaran la validación. */
public record CreateTaskRequest(@NotBlank @Size(min = 2, max = 120) String title) {}
```

```java
package com.example.tasksapi;

import jakarta.validation.constraints.Size;

/** Cambios parciales: un campo null significa "no modificar". */
public record UpdateTaskRequest(@Size(min = 2, max = 120) String title, Boolean done) {}
```

**Entidad / modelo frente a DTO.** El modelo representa los datos de tu dominio; el DTO (*Data Transfer Object*) representa lo que viaja por la API. Separarlos permite que la API cambie sin romper el dominio y evita exponer campos internos.

### Repositorio

```java
package com.example.tasksapi;

import java.util.List;
import java.util.Optional;

public interface TaskRepository {
    List<Task> findAll();

    Optional<Task> findById(String id);

    Task save(Task task);

    boolean deleteById(String id);

    String nextId();
}
```

```java
package com.example.tasksapi;

import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;
import java.util.Optional;
import java.util.concurrent.atomic.AtomicLong;
import org.springframework.stereotype.Repository;

@Repository // Spring crea una instancia y la inyecta donde se pida un TaskRepository
public class InMemoryTaskRepository implements TaskRepository {

    private final Map<String, Task> tasks = new LinkedHashMap<>();
    private final AtomicLong counter = new AtomicLong();

    @Override
    public synchronized List<Task> findAll() {
        return new ArrayList<>(tasks.values());
    }

    @Override
    public synchronized Optional<Task> findById(String id) {
        return Optional.ofNullable(tasks.get(id));
    }

    @Override
    public synchronized Task save(Task task) {
        tasks.put(task.id(), task);
        return task;
    }

    @Override
    public synchronized boolean deleteById(String id) {
        return tasks.remove(id) != null;
    }

    @Override
    public String nextId() {
        return String.valueOf(counter.incrementAndGet());
    }
}
```

### Servicio

```java
package com.example.tasksapi;

import java.util.List;
import org.springframework.stereotype.Service;

@Service // capa de reglas de negocio
public class TaskService {

    private final TaskRepository repository;

    // Inyección por constructor: Spring pasa aquí el repositorio
    public TaskService(TaskRepository repository) {
        this.repository = repository;
    }

    public List<Task> list() {
        return repository.findAll();
    }

    public Task get(String id) {
        return repository.findById(id).orElseThrow(() -> new TaskNotFoundException(id));
    }

    public Task create(CreateTaskRequest request) {
        return repository.save(new Task(repository.nextId(), request.title().strip(), false));
    }

    public Task update(String id, UpdateTaskRequest request) {
        Task task = get(id);
        if (request.title() != null) {
            task = task.withTitle(request.title().strip());
        }
        if (request.done() != null) {
            task = task.withDone(request.done());
        }
        return repository.save(task);
    }

    public void delete(String id) {
        if (!repository.deleteById(id)) {
            throw new TaskNotFoundException(id);
        }
    }
}
```

### Controlador

```java
package com.example.tasksapi;

import jakarta.validation.Valid;
import java.util.List;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.*;

@RestController // devuelve JSON directamente
@RequestMapping("/api/tasks")
public class TaskController {

    private final TaskService service;

    public TaskController(TaskService service) {
        this.service = service;
    }

    @GetMapping
    public List<Task> list() {
        return service.list();
    }

    @GetMapping("/{id}")
    public Task get(@PathVariable String id) {
        return service.get(id);
    }

    @PostMapping
    @ResponseStatus(HttpStatus.CREATED) // 201
    public Task create(@Valid @RequestBody CreateTaskRequest request) {
        return service.create(request);
    }

    @PatchMapping("/{id}")
    public Task update(@PathVariable String id, @Valid @RequestBody UpdateTaskRequest request) {
        return service.update(id, request);
    }

    @DeleteMapping("/{id}")
    @ResponseStatus(HttpStatus.NO_CONTENT) // 204
    public void delete(@PathVariable String id) {
        service.delete(id);
    }
}
```

| Anotación | Significado |
|---|---|
| `@RestController` | Los métodos devuelven datos (JSON), no vistas |
| `@RequestMapping("/api/tasks")` | Prefijo común de las rutas |
| `@GetMapping`, `@PostMapping`, `@PatchMapping`, `@DeleteMapping` | Método HTTP y ruta |
| `@PathVariable` | Valor del trozo `{id}` de la URL |
| `@RequestBody` | Convierte el JSON del cuerpo al objeto Java (deserialización) |
| `@Valid` | Ejecuta las validaciones del DTO |
| `@ResponseStatus(HttpStatus.CREATED)` | Código de estado de la respuesta |
| Valor devuelto | Se convierte a JSON automáticamente (serialización) |

### Errores en un solo lugar

```java
package com.example.tasksapi;

import java.util.Map;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.MethodArgumentNotValidException;
import org.springframework.web.bind.annotation.ExceptionHandler;
import org.springframework.web.bind.annotation.RestControllerAdvice;

/** Un único lugar donde las excepciones se traducen a respuestas HTTP. */
@RestControllerAdvice
public class ApiExceptionHandler {

    @ExceptionHandler(TaskNotFoundException.class)
    public ResponseEntity<Map<String, String>> notFound(TaskNotFoundException error) {
        return ResponseEntity.status(HttpStatus.NOT_FOUND).body(Map.of("error", error.getMessage()));
    }

    @ExceptionHandler(MethodArgumentNotValidException.class)
    public ResponseEntity<Map<String, String>> invalid(MethodArgumentNotValidException error) {
        String message = error.getBindingResult().getFieldErrors().stream()
                .map(fieldError -> fieldError.getField() + ": " + fieldError.getDefaultMessage())
                .findFirst()
                .orElse("Petición inválida");
        return ResponseEntity.status(HttpStatus.BAD_REQUEST).body(Map.of("error", message));
    }
}
```

`@RestControllerAdvice` captura excepciones lanzadas desde cualquier controlador y las traduce a respuestas: `404` para «no existe», `400` para datos inválidos.

### Configuración y CORS

```java
package com.example.tasksapi;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Configuration;
import org.springframework.web.servlet.config.annotation.CorsRegistry;
import org.springframework.web.servlet.config.annotation.WebMvcConfigurer;

@Configuration
public class WebConfig implements WebMvcConfigurer {

    // Valor leído de application.properties o de la variable de entorno CORS_ORIGIN
    @Value("${app.cors.origin}")
    private String origin;

    @Override
    public void addCorsMappings(CorsRegistry registry) {
        registry.addMapping("/api/**")
                .allowedOrigins(origin)
                .allowedMethods("GET", "POST", "PATCH", "DELETE");
    }
}
```

```properties
# Puerto: se puede sobrescribir con la variable de entorno SERVER_PORT o PORT
server.port=${PORT:3000}
# Origen permitido por CORS
app.cors.origin=${CORS_ORIGIN:http://localhost:5173}
```

| Pieza | Significado |
|---|---|
| `application.properties` | Configuración externa de la aplicación |
| `${PORT:3000}` | Usa la variable de entorno `PORT` o, si no existe, `3000` |
| `@Value("${app.cors.origin}")` | Inyecta un valor de configuración |
| `allowedMethods(…)` | Hay que enumerar `PATCH` y `DELETE` si los usa tu frontend |

Orden de prioridad de la configuración (de mayor a menor): argumentos de línea de comandos → variables de entorno → `application.properties`. Perfiles: `application-prod.properties` se carga con `SPRING_PROFILES_ACTIVE=prod`. Los **secretos** (contraseñas, claves) van en variables de entorno, nunca en `application.properties` versionado.

## 5. Ejecutar y probar

```bash
mvn -B test
mvn spring-boot:run
mvn -B -DskipTests package
java -jar target/tasks-spring-0.1.0.jar
```

| Comando | Significado |
|---|---|
| `mvn spring-boot:run` | Arranca la aplicación en desarrollo |
| `mvn package` | Genera el `.jar` ejecutable (incluye Tomcat y dependencias) |
| `java -jar archivo.jar` | Ejecuta la aplicación; admite `--server.port=8081` |

```bash
curl -s -X POST -H 'Content-Type: application/json' -d '{"title":"Estudiar Spring"}' http://localhost:3000/api/tasks
curl -s http://localhost:3000/api/tasks
curl -s -X PATCH -H 'Content-Type: application/json' -d '{"done":true}' http://localhost:3000/api/tasks/1
curl -s -o /dev/null -w "%{http_code}\n" -X DELETE http://localhost:3000/api/tasks/1
```

Esta API es **intercambiable** con la de [Node.js](#node): mismas rutas, mismos códigos y mismo formato JSON. La interfaz de React o Vue funciona con cualquiera de las dos sin cambios.

### Prueba de integración

```java
package com.example.tasksapi;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertTrue;

import java.net.URI;
import java.net.http.HttpClient;
import java.net.http.HttpRequest;
import java.net.http.HttpResponse;
import org.junit.jupiter.api.Test;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.boot.test.web.server.LocalServerPort;

// Arranca la aplicación completa en un puerto aleatorio y la llama como un cliente real
@SpringBootTest(webEnvironment = SpringBootTest.WebEnvironment.RANDOM_PORT)
class TasksApiTest {

    @LocalServerPort
    int port;

    private final HttpClient client = HttpClient.newHttpClient();

    private HttpResponse<String> send(String method, String path, String body) throws Exception {
        HttpRequest.Builder builder = HttpRequest.newBuilder(URI.create("http://localhost:" + port + path));
        if (body == null) {
            builder.method(method, HttpRequest.BodyPublishers.noBody());
        } else {
            builder.header("Content-Type", "application/json").method(method, HttpRequest.BodyPublishers.ofString(body));
        }
        return client.send(builder.build(), HttpResponse.BodyHandlers.ofString());
    }

    @Test
    void crudFlow() throws Exception {
        HttpResponse<String> created = send("POST", "/api/tasks", "{\"title\":\"Estudiar Spring\"}");
        assertEquals(201, created.statusCode());
        assertTrue(created.body().contains("\"title\":\"Estudiar Spring\""));

        assertEquals(400, send("POST", "/api/tasks", "{\"title\":\"x\"}").statusCode());

        HttpResponse<String> patched = send("PATCH", "/api/tasks/1", "{\"done\":true}");
        assertTrue(patched.body().contains("\"done\":true"));

        assertEquals(204, send("DELETE", "/api/tasks/1", null).statusCode());
        assertEquals(404, send("GET", "/api/tasks/1", null).statusCode());
    }
}
```

`@SpringBootTest(webEnvironment = RANDOM_PORT)` arranca la aplicación completa en un puerto libre; `@LocalServerPort` te dice cuál. Se llama con el cliente HTTP estándar de Java, como haría un cliente real. Mockito puede mostrar avisos de agente dinámico con JDK recientes; son informativos.

## 6. Datos reales: MongoDB o SQL

Se añade el starter correspondiente y se sustituye `InMemoryTaskRepository`:

| Base | Starter | Idea |
|---|---|---|
| MongoDB | `spring-boot-starter-data-mongodb` | `interface TaskMongoRepository extends MongoRepository<TaskDocument, String>` |
| PostgreSQL | `spring-boot-starter-data-jpa` + driver `postgresql` | `interface TaskJpaRepository extends JpaRepository<TaskEntity, Long>` |

Spring Data **genera la implementación** a partir de la interfaz, incluidas consultas por nombre del método (`findByDoneFalse()`). La conexión se declara en `application.properties` (`spring.data.mongodb.uri=…`, `spring.datasource.url=…`) y las credenciales llegan por variables de entorno. Para SQL, las migraciones del esquema se gestionan con Flyway o Liquibase (ver [SQL](#sql)).

## 7. Seguridad (visión general)

- **Autenticación**: quién eres (usuario y contraseña, token JWT, OAuth2).
- **Autorización**: qué puedes hacer (roles y permisos).
- Spring Security se añade con `spring-boot-starter-security`; al hacerlo, **todo queda protegido por defecto** y debes declarar qué rutas son públicas.
- Contraseñas: siempre con `BCryptPasswordEncoder` o equivalente; nunca en claro ([criptografía](#cripto)).

## 8. Logging y producción

- Spring Boot trae logging configurado (`logging.level.com.example=DEBUG` en propiedades).
- `spring-boot-starter-actuator` añade `/actuator/health` y métricas.
- Contenedor: `FROM eclipse-temurin:21-jre` + `COPY target/*.jar app.jar` + `ENTRYPOINT ["java","-jar","/app.jar"]`.
- Ajusta la memoria con `-Xmx` en contenedores pequeños.

## 9. Errores habituales

| Mensaje | Causa | Solución |
|---|---|---|
| `Web server failed to start. Port 3000 was already in use` | Otro proceso usa el puerto | `ss -ltnp \| grep 3000`; cambia `PORT` |
| `No qualifying bean of type …` | Falta la anotación (`@Service`, `@Repository`) o la clase está fuera del paquete explorado | Anota la clase y colócala bajo el paquete de `@SpringBootApplication` |
| `Field error in object … on field 'title'` / `400` | La validación del DTO falla | Es el comportamiento esperado; revisa el cuerpo enviado |
| `HttpMessageNotReadableException` | JSON mal formado o tipos incorrectos | Revisa el cuerpo y la cabecera `Content-Type` |
| Error de CORS en el navegador con `PATCH`/`DELETE` | No están en `allowedMethods` | Añádelos |
| `Could not resolve placeholder 'app.cors.origin'` | Falta la propiedad | Defínela en `application.properties` o con valor por defecto `${…:valor}` |
| `UnsupportedClassVersionError` | JDK de ejecución anterior al de compilación | Ejecuta con el mismo JDK o superior |
| `Circular dependency` | Dos beans se necesitan entre sí | Rediseña la dependencia |

## 10. Ejercicios

> **Ejercicio:** añade `GET /api/tasks?done=true` con `@RequestParam(required = false) Boolean done`, y su prueba.

> **Ejercicio:** añade paginación con `?page=0&size=5` y devuelve en la respuesta el total.

> **Ejercicio:** sustituye `InMemoryTaskRepository` por una implementación con MongoDB (starter de Spring Data) o PostgreSQL y comprueba que el servicio y el controlador no cambian.

> **Ejercicio:** impide títulos duplicados: lanza una excepción propia y tradúcela a `409 Conflict` en `ApiExceptionHandler`.

> **Ejercicio:** haz que el frontend React use esta API en lugar de la de Node.js, cambiando solo `VITE_API_URL`.

## 11. Documentación oficial

- [Spring Boot: referencia](https://docs.spring.io/spring-boot/reference/)
- [Spring Initializr](https://start.spring.io/)
- [Guías de Spring](https://spring.io/guides)
- [Spring Data](https://spring.io/projects/spring-data)
