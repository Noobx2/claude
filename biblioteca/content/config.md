# Manifiestos y archivos de configuración

Cada proyecto está lleno de archivos con nombres que conviene reconocer a primera vista. Esta ficha responde, para cada uno: **qué es, quién lo lee, cuándo se genera, si se edita a mano, qué campos importan y cómo diagnosticar sus errores**. Los ejemplos son archivos reales de los proyectos probados en esta biblioteca.

*Verificado: 2026-10-27. Los archivos mostrados son los generados o escritos en los proyectos de las demás fichas, que se compilaron y ejecutaron. Sintaxis de formatos comprobada con intérpretes reales.*

## 1. Cuatro categorías que no hay que confundir

| Categoría | Qué dice | Ejemplos | ¿Se versiona? |
|---|---|---|---|
| **Manifiesto de dependencias** | Qué necesita el proyecto y con qué rangos de versión | `package.json`, `pom.xml`, `pyproject.toml`, `Cargo.toml`, `go.mod`, `*.csproj`, `pubspec.yaml` | **Sí** |
| **Archivo de bloqueo** (*lockfile*) | Las versiones **exactas** instaladas, para reproducir | `pnpm-lock.yaml`, `uv.lock`, `Cargo.lock`, `go.sum`, `pubspec.lock` | **Sí** (en aplicaciones) |
| **Configuración de herramientas** | Cómo se comporta una herramienta o el compilador | `tsconfig.json`, `vite.config.ts`, `.oxlintrc.json`, `.editorconfig`, `analysis_options.yaml` | **Sí** |
| **Entorno y secretos** | Valores que cambian por máquina o entorno, y secretos | `.env`, variables del sistema, `local.properties` | **No** (`.env.example` sí) |

Y dos más: **definición de ejecución** (`Dockerfile`, `compose.yaml`, `Makefile`, unidades de systemd) y **configuración de servidores y bases de datos** (`sshd_config`, `postgresql.conf`, `nginx.conf`).

> **Regla:** manifiesto = *qué quieres*; bloqueo = *qué tienes exactamente*. Se editan los manifiestos (o con comandos como `pnpm add`); los archivos de bloqueo **nunca a mano**.

### Variables del sistema frente a variables de aplicación

| | Del sistema / shell | De la aplicación |
|---|---|---|
| Ejemplos | `PATH`, `HOME`, `JAVA_HOME` | `PORT`, `DATABASE_URL`, `JWT_SECRET` |
| Dónde viven | `~/.bashrc`, `~/.profile`, `/etc/environment` | `.env`, variables del contenedor, panel del servidor |
| Quién las lee | La shell y todos los procesos | Tu código o su cargador |

Detalle: [PATH, variables y perfiles](#entorno-path).

## 2. Formatos de archivo

| Formato | Aspecto | Comentarios | Usos |
|---|---|---|---|
| **JSON** | `{"clave": "valor"}` | **No admite** (algunas herramientas aceptan `//` y lo llaman JSONC: `tsconfig.json`) | `package.json`, respuestas de API |
| **YAML** | Sangría con espacios, `clave: valor`, listas con `- ` | `# comentario` | `compose.yaml`, CI, `pubspec.yaml`, `pnpm-lock.yaml` |
| **TOML** | `[sección]` y `clave = valor` | `# comentario` | `Cargo.toml`, `pyproject.toml`, `mise.toml` |
| **INI** | `[sección]` y `clave = valor` | `;` o `#` | `.editorconfig`, `settings.ini`, `php.ini` |
| **.properties** | `clave=valor` | `#` | Java / Spring (`application.properties`) |
| **XML** | `<etiqueta>valor</etiqueta>` | `<!-- -->` | `pom.xml`, `*.csproj`, `AndroidManifest.xml` |
| **.env** | `CLAVE=valor` | `#` | Variables de entorno |

```yaml
# YAML: la SANGRÍA importa (2 espacios, nunca tabuladores)
services:
  api:
    ports:
      - "3000:3000"      # una lista
    environment:
      PORT: "3000"       # entre comillas para que sea texto, no número
```

```toml
# TOML
[package]
name = "tasks-rs"
version = "0.1.0"

[dependencies]
serde = { version = "1", features = ["derive"] }
```

> **Ojo (YAML):** `no`, `yes`, `on`, `off` y `1.10` pueden interpretarse como booleano o número. Pon comillas cuando quieras texto. Un tabulador o una sangría errónea rompe el archivo.

Validar sintaxis: `python3 -c "import json,sys; json.load(open('package.json'))"`, `python3 -c "import tomllib; tomllib.load(open('Cargo.toml','rb'))"`, `docker compose config` (YAML de Compose).

## 3. JavaScript y TypeScript

### `package.json`

```json
{
  "name": "app-react-ts",
  "private": true,
  "version": "0.0.0",
  "type": "module",
  "scripts": {
    "dev": "vite",
    "build": "tsc -b && vite build",
    "lint": "oxlint",
    "preview": "vite preview"
  },
  "dependencies": {
    "react": "^19.3.1",
    "react-dom": "^19.3.1"
  },
  "devDependencies": {
    "@types/node": "^24.19.2",
    "@types/react": "^19.3.0",
    "@types/react-dom": "^19.3.0",
    "@vitejs/plugin-react": "^6.1.2",
    "oxlint": "^1.89.0",
    "typescript": "~7.0.2",
    "vite": "^8.3.5"
  }
}
```

| Ficha | Detalle |
|---|---|
| **Qué es** | Manifiesto del proyecto Node/frontend |
| **Quién lo lee** | npm, pnpm, Vite, Node, herramientas |
| **Se genera** | `pnpm init`, `pnpm create vite@latest` |
| **¿Se edita?** | Sí (o con `pnpm add`/`remove`) |
| **Campos clave** | `name`, `version`, `private`, `type` (`module`), `scripts`, `dependencies`, `devDependencies`, `engines`, `packageManager` |
| **Diagnóstico** | `JSON.parse` falla: coma sobrante o comillas simples; `pnpm install` no encuentra el script: nombre mal escrito en `scripts` |

### `pnpm-lock.yaml` / `package-lock.json`

Bloqueo con la versión exacta, el origen y el hash de cada paquete. **Lo escribe la herramienta**, se versiona, y se instala con `pnpm install --frozen-lockfile` (falla si el manifiesto y el bloqueo discrepan). Mezclar dos gestores crea dos bloqueos que se contradicen. Si está corrupto o hay un conflicto de fusión: borra solo ese archivo y ejecuta `pnpm install` para regenerarlo.

### `tsconfig.json`

```json
{
  "compilerOptions": {
    "tsBuildInfoFile": "./node_modules/.tmp/tsconfig.app.tsbuildinfo",
    "target": "es2023",
    "lib": ["ES2023", "DOM"],
    "module": "esnext",
    "types": ["vite/client"],
    "allowArbitraryExtensions": true,
    "skipLibCheck": true,

    /* Bundler mode */
    "moduleResolution": "bundler",
    "allowImportingTsExtensions": true,
    "verbatimModuleSyntax": true,
    "moduleDetection": "force",
    "noEmit": true,
    "jsx": "react-jsx",

    /* Linting */
    "noUnusedLocals": true,
    "noUnusedParameters": true,
    "erasableSyntaxOnly": true,
    "noFallthroughCasesInSwitch": true
  },
  "include": ["src"]
}
```

| Ficha | Detalle |
|---|---|
| **Qué es** | Configuración de TypeScript (la plantilla de Vite la divide: `tsconfig.json` referencia a `tsconfig.app.json` para `src/` y `tsconfig.node.json` para `vite.config.ts`) |
| **Quién lo lee** | `tsc`, el editor, Vite (solo para algunas opciones) |
| **¿Se edita?** | Sí, con criterio |
| **Campos clave** | `target`, `lib`, `module`, `moduleResolution`, `strict`, `jsx`, `noEmit`, `types`, `verbatimModuleSyntax`, `include` |
| **Diagnóstico** | El editor y `tsc` disienten: están leyendo configuraciones distintas; `Cannot find name 'process'`: falta `types: ["node"]` |

Más detalle en [TypeScript](#ts).

### `vite.config.ts`

```ts
import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
})
```

Configuración de Vite: plugins (`react()`, `vue()`), `server` (puerto, *proxy* hacia la API), `build`, `resolve.alias`, variables. Es **código TypeScript**, no datos. El proxy evita CORS en desarrollo:

```ts
export default defineConfig({
  plugins: [react()],
  server: { proxy: { '/api': 'http://localhost:3000' } },
});
```

### `.oxlintrc.json`

```json
{
  "$schema": "./node_modules/oxlint/configuration_schema.json",
  "plugins": ["react", "typescript", "oxc"],
  "rules": {
    "react/rules-of-hooks": "error",
    "react/only-export-components": ["warn", { "allowConstantExport": true }]
  }
}
```

Reglas del linter Oxlint. `plugins` activa familias de reglas; `rules` cambia la gravedad (`off`, `warn`, `error`).

## 4. Python

### `pyproject.toml` y `uv.lock`

```toml
[project]
name = "tasks-py"
version = "0.1.0"
description = "Add your description here"
readme = "README.md"
requires-python = ">=3.13"
dependencies = [
    "fastapi>=0.162.0",
    "uvicorn>=0.54.0",
]

[dependency-groups]
dev = [
    "httpx>=0.28.1",
    "pytest>=9.1.1",
    "ruff>=0.17.3",
]

[tool.pytest.ini_options]
pythonpath = ["."]
testpaths = ["tests"]
```

| Ficha | Detalle |
|---|---|
| **Qué es** | Manifiesto estándar de Python (PEP 621) y configuración de herramientas (`[tool.pytest.ini_options]`, `[tool.ruff]`) |
| **Quién lo lee** | `uv`, `pip`, `pytest`, `ruff` |
| **Se genera** | `uv init` |
| **¿Se edita?** | Sí (o con `uv add`) |
| **Campos clave** | `[project] name, version, requires-python, dependencies`; `[dependency-groups]`; `[tool.*]` |
| **`uv.lock`** | Bloqueo; no se edita; se versiona |
| **Diagnóstico** | `No solution found when resolving dependencies`: dos dependencias exigen versiones incompatibles; revisa `requires-python` |

El entorno virtual `.venv/` **no** se versiona. `.python-version` fija la versión del intérprete.

## 5. Java

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

| Ficha | Detalle |
|---|---|
| **Qué es** | *Project Object Model*: identidad, dependencias, plugins y propiedades del proyecto Maven |
| **Quién lo lee** | Maven, IDEs |
| **Se genera** | A mano, desde un arquetipo o con Spring Initializr |
| **¿Se edita?** | Sí |
| **Campos clave** | `groupId:artifactId:version`, `properties`, `dependencies` (con `scope`), `build/plugins`, `parent` (Spring Boot) |
| **Diagnóstico** | `Could not resolve dependencies`: coordenadas mal escritas o sin red; `mvn help:effective-pom` muestra el POM final con la herencia aplicada; `mvn dependency:tree` explica de dónde viene cada librería |

### `build.gradle` / `build.gradle.kts` (Gradle)

Equivalente a `pom.xml` en Gradle (Groovy o Kotlin DSL). Se encuentra en **proyectos Android** ([Android nativo](#android)). Campos: `plugins { }`, `dependencies { implementation("grupo:artefacto:versión") }`, `android { compileSdk … }`. `settings.gradle.kts` lista los módulos; `gradle/libs.versions.toml` centraliza las versiones; `gradlew` es el *wrapper*. Se usa `./gradlew tarea`.

### `application.properties` (Spring Boot)

```properties
# Puerto: se puede sobrescribir con la variable de entorno SERVER_PORT o PORT
server.port=${PORT:3000}
# Origen permitido por CORS
app.cors.origin=${CORS_ORIGIN:http://localhost:5173}
```

Configuración de la aplicación Spring. `${NOMBRE:valor}` lee una variable de entorno con un valor por defecto. También existe `application.yml` (equivalente en YAML) y variantes por perfil (`application-prod.properties`).

## 6. C# / .NET

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

</Project>
```

| Ficha | Detalle |
|---|---|
| **Qué es** | Archivo de proyecto: SDK, framework, dependencias NuGet |
| **Se genera** | `dotnet new` |
| **¿Se edita?** | Sí, o con `dotnet add package` |
| **Campos clave** | `Sdk`, `TargetFramework`, `Nullable`, `ImplicitUsings`, `PackageReference`, `ProjectReference` |
| **Otros** | `*.sln`/`*.slnx` (lista de proyectos), `appsettings.json` (configuración de ASP.NET Core), `Properties/launchSettings.json` (puertos de desarrollo), `global.json` (fija la versión del SDK) |

## 7. Go

```go
module example.com/tasks

go 1.24
```

| Ficha | Detalle |
|---|---|
| **`go.mod`** | Manifiesto: ruta del módulo, versión de Go, dependencias (`require`) |
| **`go.sum`** | Hashes de verificación de las dependencias; se versiona; no se edita |
| **Se generan** | `go mod init`, `go get`, `go mod tidy` |
| **Diagnóstico** | `go: go.mod file not found`: estás fuera del módulo; `missing go.sum entry`: ejecuta `go mod tidy` |

## 8. Rust

```toml
[package]
name = "tasks-rs"
version = "0.1.0"
edition = "2024"

[dependencies]
axum = "0.8.9"
serde = { version = "1.0.229", features = ["derive"] }
serde_json = "1.0.154"
tokio = { version = "1.54.1", features = ["full"] }
tower-http = { version = "0.7.1", features = ["cors"] }

[dev-dependencies]
http-body-util = "0.1.5"
tower = { version = "0.5.3", features = ["util"] }
```

| Ficha | Detalle |
|---|---|
| **`Cargo.toml`** | Manifiesto: `[package]`, `[dependencies]`, `[dev-dependencies]`, `features`, `[profile.release]` |
| **`Cargo.lock`** | Bloqueo; se versiona en aplicaciones |
| **Se generan** | `cargo new`, `cargo add` |
| **Diagnóstico** | `failed to select a version for …`: versiones incompatibles; `cargo tree -d` muestra duplicados; `edition` inválida: actualiza `rustc` |

## 9. Dart / Flutter

```yaml
name: tasks_app
description: "A new Flutter project."
publish_to: 'none'
version: 0.1.0+1

environment:
  sdk: ^3.13.7

dependencies:
  flutter:
    sdk: flutter
  http: ^1.6.0

dev_dependencies:
  flutter_test:
    sdk: flutter
  flutter_lints: ^6.0.0

flutter:
  uses-material-design: true
```

| Ficha | Detalle |
|---|---|
| **`pubspec.yaml`** | Manifiesto: nombre, versión, entorno del SDK, dependencias, assets y fuentes |
| **`pubspec.lock`** | Bloqueo; se versiona en aplicaciones |
| **`analysis_options.yaml`** | Reglas del analizador (`include: package:flutter_lints/flutter.yaml`) |
| **`.fvmrc`** | Versión de Flutter del proyecto (FVM) |
| **Diagnóstico** | Error de sangría en YAML; `version solving failed`: restricciones incompatibles (`flutter pub outdated`) |
| **Android** | `android/app/src/main/AndroidManifest.xml`: permisos, actividades; `android/local.properties`: ruta del SDK (no se versiona) |

## 10. Entorno y secretos

### `.env` y `.env.example`

```bash
# .env.example  (se versiona: documenta qué variables existen, con valores FALSOS)
PORT=3000
DATABASE_URL=postgres://app:CHANGE_ME@localhost:5432/app
JWT_SECRET=CHANGE_ME
```

| Ficha | Detalle |
|---|---|
| **Qué es** | Variables de aplicación para este entorno |
| **Quién lo lee** | `node --env-file`, Vite, Docker Compose, `dotenv`, Python (`pydantic-settings`) |
| **Se genera** | `cp .env.example .env` |
| **¿Se edita?** | Sí, es su propósito |
| **Contiene secretos** | **Sí → `.env` va en `.gitignore`**; nunca credenciales reales en `.env.example` |
| **Diagnóstico** | La variable llega `undefined`: ¿está en el archivo correcto?, ¿reiniciaste?, ¿prefijo `VITE_`?; comillas y espacios en el valor; `#` dentro de un valor sin comillas se interpreta como comentario |

### Otros archivos que pueden contener secretos

| Archivo | Riesgo |
|---|---|
| `.env`, `.env.*` | Claves, contraseñas |
| `~/.ssh/id_*` (sin `.pub`) | Clave privada |
| `wg0.conf`, `*.pem`, `*.key` | Claves de VPN y TLS |
| `~/.pgpass`, `~/.aws/credentials`, `~/.npmrc` | Credenciales de servicios |
| `appsettings.Production.json`, `application-prod.properties` | Cadenas de conexión |
| `terraform.tfstate`, volcados de bases de datos | Datos sensibles |

**Nunca** los subas a Git. Si ya lo hiciste: revoca la credencial ([Git](#git)).

## 11. Contenedores

### `Dockerfile` y `.dockerignore`

```dockerfile
FROM node:24-slim
WORKDIR /app
COPY package.json pnpm-lock.yaml ./
RUN npm install -g pnpm && pnpm install --prod --frozen-lockfile
COPY src ./src
USER node
CMD ["node", "src/server.js"]
```

Receta para construir una imagen. Cada instrucción es una capa. `.dockerignore` evita copiar `node_modules`, `.git`, `.env`. Detalle completo en [Docker y Compose](#docker).

### `compose.yaml`

Define varios servicios (contenedores), redes y volúmenes. `docker compose config` muestra el archivo interpretado y es la forma de depurarlo; `${VAR:?mensaje}` exige variables. El nombre `docker-compose.yml` (antiguo) también funciona; la clave `version:` ya no se usa.

## 12. Git y editor

### `.gitignore`

Una línea por patrón: `node_modules/`, `*.log`, `.env`, `!.env.example`. No afecta a archivos **ya seguidos** (`git rm --cached`). `git check-ignore -v ruta` explica por qué se ignora un archivo.

### `.editorconfig`

```ini
root = true

[*]
charset = utf-8
end_of_line = lf
insert_final_newline = true
trim_trailing_whitespace = true
indent_style = space
indent_size = 2

[*.{py,rs}]
indent_size = 4

[Makefile]
indent_style = tab
```

Lo leen los editores (VS Code con la extensión *EditorConfig*, IntelliJ, Visual Studio) para unificar finales de línea, sangrías y codificación entre personas. Las secciones usan patrones de archivo.

### `.gitattributes`

Normaliza finales de línea (`* text=auto eol=lf`) y marca binarios. Útil si trabajas entre Windows y Linux (tu anfitrión y tu VM) para evitar los `^M` de CRLF en scripts.

## 13. `Makefile`

```makefile
# Makefile: atajos para las tareas habituales del proyecto
# Importante: las líneas de órdenes empiezan con un TABULADOR, no con espacios.

.DEFAULT_GOAL := help
.PHONY: help install dev test lint build clean

help: ## Muestra esta ayuda
	@grep -E '^[a-zA-Z_-]+:.*?## ' $(MAKEFILE_LIST) | awk 'BEGIN {FS = ":.*?## "}; {printf "  %-10s %s\n", $$1, $$2}'

install: ## Instala las dependencias
	pnpm install --frozen-lockfile

dev: ## Arranca el servidor de desarrollo
	pnpm dev

test: ## Ejecuta las pruebas
	pnpm test

lint: ## Analiza el código
	pnpm lint

build: lint test ## Compila (antes ejecuta lint y test)
	pnpm build

clean: ## Borra lo generado
	rm -rf dist node_modules/.cache
```

| Ficha | Detalle |
|---|---|
| **Qué es** | Recetas con nombre: `make test`, `make build` |
| **Quién lo lee** | `make` |
| **¿Se edita?** | Sí |
| **Campos clave** | `objetivo: dependencias` y debajo las órdenes (**con tabulador**); `.PHONY` (objetivos que no son archivos); `.DEFAULT_GOAL`; variables `$(VAR)` |
| **Diagnóstico** | `*** missing separator`: las órdenes llevan espacios en lugar de un tabulador; `make -n objetivo` muestra qué ejecutaría sin hacerlo |

Resultado real de `make help` y `make -n build`:

```text
  help       Muestra esta ayuda
  install    Instala las dependencias
  ...
$ make -n build
pnpm lint
pnpm test
pnpm build
```

Alternativas modernas: los `scripts` de `package.json`, `just`, `mise tasks`.

## 14. Configuración de servidores y sistema

| Archivo | Para qué | Formato | Comprobar |
|---|---|---|---|
| `/etc/ssh/sshd_config`, `/etc/ssh/sshd_config.d/*.conf` | Servidor SSH ([Endurecimiento](#hardening)) | `Directiva valor` | `sudo sshd -t`, `sudo sshd -T` |
| `/etc/fail2ban/jail.local` | `fail2ban` | INI | `fail2ban-client -t` |
| `/etc/wireguard/wg0.conf` | VPN ([VPN](#vpn)) | INI | `wg-quick strip wg0` |
| `/etc/systemd/system/*.service` | Servicios propios ([Honeypots](#honeypots)) | INI | `systemd-analyze verify unidad.service`; `systemctl daemon-reload` tras cambiar |
| `crontab -e` | Tareas programadas ([Recetario](#recetas)) | Cinco campos + orden | `crontab -l` |
| `postgresql.conf`, `pg_hba.conf` | PostgreSQL: parámetros y control de acceso | `clave = valor` / tabla de reglas | `SHOW ALL;` en `psql` |
| `mongod.conf` | MongoDB | YAML | `mongod --config … --validate` si existe |
| `nginx.conf`, `Caddyfile` | Proxy inverso | Propio | `sudo nginx -t`, `caddy validate` |
| `/etc/environment`, `~/.bashrc`, `~/.profile` | Variables y perfil de la shell | Shell | `source ~/.bashrc` |
| `/etc/hosts`, `/etc/resolv.conf` | Resolución de nombres | Texto | `getent hosts nombre` |

> **Regla:** antes de recargar un servicio, **valida** el archivo con su comando de prueba y mantén abierta una sesión de recuperación.

## 15. Documentación de proyecto

| Archivo | Para qué |
|---|---|
| `README.md` | Qué es el proyecto y cómo arrancarlo en menos de 10 líneas |
| `AGENT.md` / `CLAUDE.md` | Normas permanentes para asistentes y colaboradores ([Normas](#normas)) |
| `CHANGELOG.md` | Historial de cambios por versión |
| `LICENSE` | Licencia de uso |
| `docs/` | Decisiones, arquitectura, guías |

## 16. Diagnóstico general de configuración

1. **¿Qué archivo lee la herramienta?** (a veces hay varios: global, de usuario, de proyecto). `--help`, `--verbose`, `config show`/`effective` lo aclaran (`mvn help:effective-pom`, `sshd -T`, `docker compose config`, `git config --list --show-origin`).
2. **¿La sintaxis es válida?** Valida con el intérprete del formato.
3. **¿Se recargó?** Muchos servicios solo leen la configuración al arrancar.
4. **¿Precedencia?** Normalmente: argumentos > variables de entorno > archivo de proyecto > archivo global > valores por defecto.
5. **¿Qué cambió?** `git diff` del archivo.

## 17. Ejercicios

> **Ejercicio:** para cada proyecto de tu carpeta `~/projects`, clasifica sus archivos en las cuatro categorías de la sección 1. ¿Hay algún secreto versionado por error?

> **Ejercicio:** rompe adrede un `package.json` (coma sobrante), un `compose.yaml` (tabulador) y un `Makefile` (espacios) y anota el mensaje de error de cada uno y cómo lo diagnosticas.

> **Ejercicio:** escribe un `.editorconfig` para tu proyecto y comprueba que VS Code respeta la sangría de Python (4) y la de TypeScript (2).

> **Ejercicio:** crea un `Makefile` con los objetivos `install`, `test`, `lint`, `build` y `clean` para uno de tus proyectos, y que `make` sin argumentos muestre la ayuda.

> **Ejercicio:** averigua, para `git`, `ssh` y `npm`, en qué archivos busca su configuración y en qué orden.

## 18. Documentación oficial

- [package.json (npm)](https://docs.npmjs.com/cli/configuring-npm/package-json)
- [pyproject.toml (PyPA)](https://packaging.python.org/en/latest/specifications/pyproject-toml/)
- [Cargo.toml](https://doc.rust-lang.org/cargo/reference/manifest.html)
- [Maven: POM](https://maven.apache.org/pom.html)
- [go.mod](https://go.dev/ref/mod#go-mod-file)
- [pubspec.yaml](https://dart.dev/tools/pub/pubspec)
- [Compose file](https://docs.docker.com/reference/compose-file/)
- [EditorConfig](https://editorconfig.org/)
- [GNU Make](https://www.gnu.org/software/make/manual/make.html)
