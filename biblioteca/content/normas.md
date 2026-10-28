# Normas y convenciones

Reglas permanentes para esta biblioteca y para los proyectos que construyas con ella.

## Idioma

> **Regla:** todo el **código** (nombres de variables, funciones, clases, archivos, carpetas, ramas) va en **inglés**. Los **comentarios** y la documentación van en **español**.

```js
// Calcula el total del carrito aplicando el descuento
function getCartTotal(items, discountRate) {
  const subtotal = items.reduce((sum, item) => sum + item.price * item.quantity, 0);
  return subtotal * (1 - discountRate);
}
```

Así el código se parece al de la documentación oficial, y tú entiendes la explicación sin esfuerzo.

## Estructura de cada ficha de lenguaje

Cada lenguaje sigue siempre las mismas secciones, para poder compararlos:

1. Para qué sirve.
2. Instalación, versiones y ejecución.
3. Archivos y convenciones de nombres.
4. Hola mundo.
5. Variables, tipos y conversiones.
6. Condiciones, bucles y funciones.
7. Colecciones y su manejo (recorrer, filtrar, transformar, ordenar, agrupar).
8. JSON.
9. Errores y excepciones.
10. Módulos y organización.
11. Orientación a objetos, si existe.
12. Entrada, salida y archivos.
13. Concurrencia, si existe.
14. Pruebas, depuración, formato y análisis estático.
15. Gestor de dependencias y manifiesto.
16. Estructura de una aplicación real.
17. Crear, ejecutar, probar, compilar y empaquetar.
18. Errores habituales y cómo diagnosticarlos.

Cada ficha marca qué es **esencial**, qué es **intermedio** y qué es **avanzado**. Si un lenguaje no tiene un concepto (por ejemplo, clases en Go), la ficha lo dice en lugar de forzarlo.

## Convenciones de nombres por lenguaje

Los idiomas se parecen en lo importante y difieren en detalles que conviene fijar desde el principio.

| Lenguaje | Archivos | Variables y funciones | Tipos / clases | Constantes |
|---|---|---|---|---|
| JavaScript / TypeScript | `kebab-case.ts` (componentes React: `PascalCase.tsx`) | `camelCase` | `PascalCase` | `UPPER_SNAKE_CASE` |
| Python | `snake_case.py` | `snake_case` | `PascalCase` | `UPPER_SNAKE_CASE` |
| Java | `PascalCase.java` (igual que la clase pública) | `camelCase` | `PascalCase` | `UPPER_SNAKE_CASE` |
| C# | `PascalCase.cs` | métodos `PascalCase`, variables `camelCase` | `PascalCase` | `PascalCase` |
| Go | `snake_case.go`, paquetes en minúscula | `camelCase` (privado) / `PascalCase` (exportado) | `PascalCase` | `PascalCase` o `camelCase` |
| Rust | `snake_case.rs` | `snake_case` | `PascalCase` | `UPPER_SNAKE_CASE` |
| Dart | `snake_case.dart` | `lowerCamelCase` | `UpperCamelCase` | `lowerCamelCase` |

> **Ojo:** en Java el nombre del archivo **debe** coincidir con la clase pública. En Go, que un nombre empiece por mayúscula decide si es visible desde otros paquetes.

## Estructura de proyecto

Estructura mínima para proyectos propios con frontend y backend:

```text
nombre-proyecto/
  README.md
  docs/
  app/
    backend/
    frontend/
```

Un programa de consola o una librería **no** necesitan fingir que tienen frontend y backend: usan una carpeta de código sencilla.

## Secretos

- Nunca se escriben credenciales reales en ejemplos.
- `.env` contiene secretos y **no se sube a Git**. `.env.example` sí, con valores falsos.
- Los ejemplos usan marcas como `CHANGE_ME` o `example`.

## Fechas de comprobación

Las versiones, comandos de instalación y recomendaciones que pueden caducar llevan la fecha de comprobación. Si pasa mucho tiempo, vuelve a comprobarlos en la documentación oficial.

## Método de aprendizaje

- Ejemplos pequeños y ejecutables, nunca aplicaciones enormes.
- Cada archivo y cada decisión viene con su porqué.
- Los ejercicios no traen solución inmediata: intenta primero.
- Cuando hay varias alternativas, se comparan ventajas, inconvenientes y casos de uso, sin presentarlas como equivalentes.

## Plantilla de `AGENT.md` para tus proyectos

Un archivo en la raíz de cada proyecto con las normas permanentes para ti, tu equipo y los asistentes de código. Cópialo y adáptalo:

```markdown
# Normas del proyecto

## Idioma
- Código (nombres de variables, funciones, archivos, ramas): inglés.
- Comentarios y documentación: español.

## Estructura
- `app/backend/` y `app/frontend/`; documentación en `docs/`.

## Cómo trabajar
- Cambios pequeños y verificables. Una pregunta o decisión por vez.
- Antes de dar algo por hecho: ejecutar las pruebas (`pnpm test`) y el análisis (`pnpm lint`).
- No añadir dependencias sin justificarlo; fijar versiones.
- No incluir nunca credenciales reales; usar `.env.example` con valores falsos.
- Explicar cada comando nuevo, incluidas sus opciones.

## Comandos habituales
- Instalar: `pnpm install --frozen-lockfile`
- Desarrollo: `pnpm dev`
- Pruebas: `pnpm test`
- Compilar: `pnpm build`

## Seguridad
- Solo se prueba lo propio o con autorización escrita.
- Los secretos nunca se versionan.
```

## Cómo se ha comprobado esta biblioteca

- Los ejemplos de código se **ejecutaron** en un entorno Linux real (Node, Python, Java, C#, Go, Rust, Dart/Flutter, PostgreSQL, OpenSSL, `tcpdump`) y las pruebas de cada proyecto pasan.
- La API de tareas se implementó **seis veces** (Node, Python, Java/Spring Boot, C#, Go, Rust). Las seis se probaron con la **misma interfaz de React en un navegador real** (Chromium automatizado) recorriendo validación, alta, marcado, filtro y borrado; la de Node también con la interfaz de Vue. La aplicación de Flutter pasa sus pruebas de widgets y, compilada para web, creó una tarea real en la API.
- **No se pudo ejecutar** lo que depende de hardware o servicios externos: dispositivos USB y serie reales, WireGuard, `ufw`/`fail2ban`/SSH, un emulador o teléfono Android, ni compilar Kotlin. Esas fichas lo dicen en su cabecera.
- Los comandos de instalación y las versiones concretas (por ejemplo, la de `go` para `mise`) deben confirmarse en tu equipo.
