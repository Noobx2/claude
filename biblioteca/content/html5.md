# HTML5 moderno

HTML describe la **estructura y el significado** de una página. CSS decide cómo se ve y JavaScript cómo se comporta. Un buen HTML funciona incluso sin los otros dos.

*Verificado: 2026-10-27. Compatibilidad: consulta siempre [MDN](https://developer.mozilla.org/es/docs/Web/HTML) y el estado Baseline de cada función.*

## 1. Archivos y nombres

| Elemento | Convención |
|---|---|
| Página principal | `index.html` (los servidores la sirven por defecto) |
| Otras páginas | `kebab-case.html`, por ejemplo `contact-form.html` |
| Codificación | UTF-8; el `<meta charset>` va el primero |
| Etiquetas y atributos | Minúsculas, atributos entre comillas dobles |
| Carpetas típicas | `css/`, `js/`, `img/` (o `public/` y `src/` si usas Vite) |

## 2. Hola mundo

Crea `index.html`:

```html
<!doctype html>
<html lang="es">
  <head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>Hola mundo</title>
  </head>
  <body>
    <h1>Hola mundo</h1>
  </body>
</html>
```

Para verlo, ábrelo en el navegador o sírvelo desde la terminal (útil por SSH):

```bash
python3 -m http.server 8000     # sirve la carpeta actual en http://localhost:8000
```

VS Code Remote SSH reenvía el puerto automáticamente; si no, ábrelo desde su panel *Puertos*.

## 3. Qué hace cada pieza del documento

| Pieza | Para qué |
|---|---|
| `<!doctype html>` | Activa el modo estándar del navegador. Sin él, se comporta como en los años 90 |
| `<html lang="es">` | Idioma del contenido: lectores de pantalla, traducción y corrección ortográfica dependen de él |
| `<meta charset="utf-8">` | Evita que las tildes salgan como símbolos raros |
| `<meta name="viewport" ...>` | Sin esto, el móvil renderiza la página como si fuera de escritorio |
| `<title>` | Pestaña, historial y resultados de búsqueda |
| `<link rel="stylesheet" href="css/styles.css">` | Enlaza los estilos |
| `<script type="module" src="js/main.js"></script>` | Carga JavaScript como módulo; se difiere por defecto, no bloquea el HTML |

## 4. Estructura semántica

Usa la etiqueta que **describe el contenido**, no la que «se ve bien».

```html
<body>
  <header>
    <h1>Mis tareas</h1>
    <nav aria-label="Principal">
      <a href="/">Inicio</a>
      <a href="/about.html">Acerca de</a>
    </nav>
  </header>

  <main>
    <section aria-labelledby="pending-title">
      <h2 id="pending-title">Pendientes</h2>
      <ul>
        <li>Estudiar HTML</li>
      </ul>
    </section>

    <aside>Consejo del día</aside>
  </main>

  <footer><small>© 2026</small></footer>
</body>
```

| Etiqueta | Significado |
|---|---|
| `header`, `footer` | Cabecera y pie (de la página o de una sección) |
| `nav` | Bloque de navegación |
| `main` | Contenido principal; **uno por página** |
| `section` | Sección temática con encabezado |
| `article` | Contenido autónomo (una entrada, una tarjeta) |
| `aside` | Contenido complementario |
| `search` | Región de búsqueda (contenedor de un formulario de búsqueda) |
| `h1`…`h6` | Jerarquía de títulos: un `h1` y sin saltar niveles |
| `figure` / `figcaption` | Imagen o código con su pie |
| `div`, `span` | Sin significado; solo cuando nada semántico encaja |

> **Regla:** si algo hace clic y cambia de página, es un `<a href>`; si hace clic y ejecuta una acción, es un `<button type="button">`. Nunca un `<div onclick>`: pierde teclado, foco y accesibilidad.

## 5. Texto, enlaces e imágenes

```html
<p>Texto con <strong>importancia</strong> y <em>énfasis</em>.</p>
<a href="https://developer.mozilla.org" target="_blank" rel="noopener">MDN</a>

<img src="img/team.jpg" alt="Equipo en la oficina" width="800" height="450" loading="lazy">

<!-- Imágenes adaptables: el navegador elige la mejor -->
<img
  src="img/photo-800.jpg"
  srcset="img/photo-400.jpg 400w, img/photo-800.jpg 800w, img/photo-1600.jpg 1600w"
  sizes="(min-width: 800px) 800px, 100vw"
  alt="Paisaje de montaña">
```

- `alt` es obligatorio. Si la imagen es decorativa, `alt=""` (vacío, pero presente).
- `width` y `height` evitan que la página «salte» al cargar.
- `loading="lazy"` retrasa las imágenes fuera de pantalla.

## 6. Formularios

Un formulario bien hecho valida **sin JavaScript**.

```html
<form id="signup-form" method="post" action="/api/signup">
  <p>
    <label for="email">Correo</label>
    <input id="email" name="email" type="email" required autocomplete="email">
  </p>
  <p>
    <label for="age">Edad</label>
    <input id="age" name="age" type="number" min="18" max="120" inputmode="numeric">
  </p>
  <p>
    <label for="username">Usuario</label>
    <input id="username" name="username" required minlength="3" maxlength="20"
           pattern="[a-z0-9_]+" title="Minúsculas, números y guion bajo">
  </p>
  <p>
    <label for="plan">Plan</label>
    <select id="plan" name="plan">
      <option value="free">Gratis</option>
      <option value="pro">Pro</option>
    </select>
  </p>
  <button type="submit">Registrarme</button>
</form>
```

| Atributo | Función |
|---|---|
| `name` | Nombre con el que viaja el dato al servidor |
| `id` + `<label for>` | Asocia etiqueta y campo: clic en la etiqueta enfoca el campo, y lo leen los lectores de pantalla |
| `type` | `email`, `url`, `number`, `date`, `tel`, `password`, `search`, `checkbox`, `radio`, `file`… determina teclado y validación |
| `required`, `min`, `max`, `minlength`, `maxlength`, `pattern` | Reglas de validación nativas |
| `autocomplete` | Permite al navegador rellenar el campo |
| `inputmode` | Teclado que aparece en móvil |

Control del lado del cliente con JavaScript (la **Constraint Validation API**):

```js
const form = document.querySelector('#signup-form');

form.addEventListener('submit', (event) => {
  if (!form.checkValidity()) {
    event.preventDefault();
    form.reportValidity();      // muestra los mensajes nativos
  }
});

// Mensaje propio para una regla propia
const username = document.querySelector('#username');
username.addEventListener('input', () => {
  username.setCustomValidity(username.value === 'admin' ? 'Nombre reservado' : '');
});
```

En CSS, `:user-invalid` marca como inválido un campo **solo después de que el usuario lo toque**, a diferencia de `:invalid`, que aparece desde el principio.

> **Ojo:** la validación del navegador mejora la experiencia, pero **no es seguridad**. El servidor debe volver a validar todo.

## 7. Interactividad sin JavaScript

Estos elementos modernos sustituyen a mucho código manual.

```html
<!-- Acordeón; el mismo "name" hace que solo uno esté abierto -->
<details name="faq">
  <summary>¿Qué es HTML?</summary>
  <p>Un lenguaje de marcado.</p>
</details>
<details name="faq">
  <summary>¿Qué es CSS?</summary>
  <p>El lenguaje de estilos.</p>
</details>

<!-- Diálogo modal -->
<dialog id="confirm-dialog">
  <form method="dialog">
    <p>¿Eliminar la tarea?</p>
    <button value="cancel">Cancelar</button>
    <button value="ok">Eliminar</button>
  </form>
</dialog>
<button type="button" id="open-dialog">Eliminar</button>

<!-- Popover: se abre y cierra solo con atributos -->
<button popovertarget="help">Ayuda</button>
<div id="help" popover>Texto de ayuda</div>
```

```js
const dialog = document.querySelector('#confirm-dialog');
document.querySelector('#open-dialog').addEventListener('click', () => dialog.showModal());
dialog.addEventListener('close', () => {
  console.log(dialog.returnValue);   // "ok" o "cancel"
});
```

`<dialog>` gestiona el foco, la tecla Esc y el fondo oscurecido. `popover` es más reciente: compruébalo en MDN antes de usarlo en producción.

## 8. Atributos globales útiles

| Atributo | Uso |
|---|---|
| `id` | Identificador **único** en la página |
| `class` | Etiqueta reutilizable para CSS y JS |
| `data-*` | Datos propios: `data-task-id="7"` se lee con `element.dataset.taskId` |
| `hidden` | Oculta el elemento |
| `inert` | Hace que una zona no reciba foco ni clics |
| `lang` | Cambia el idioma de un fragmento |
| `tabindex="0"` | Hace enfocable un elemento (úsalo con muy poca frecuencia) |

## 9. Plantillas y componentes

```html
<template id="task-template">
  <li><input type="checkbox"> <span></span></li>
</template>
```

`<template>` guarda HTML inerte que JS clona (`template.content.cloneNode(true)`). Los *custom elements* (`customElements.define`) permiten crear etiquetas propias; son avanzados y React/Vue cubren esa necesidad en proyectos grandes.

## 10. Accesibilidad básica

1. Usa etiquetas semánticas: ya traen rol, foco y teclado.
2. Todo campo tiene su `<label>`; toda imagen informativa, su `alt`.
3. Comprueba que puedes usar la página **solo con teclado** (Tab, Enter, Esc) y que el foco se ve.
4. Contraste suficiente entre texto y fondo.
5. **ARIA solo si no hay etiqueta nativa**: un `<button>` siempre es mejor que `<div role="button">`.

## 11. Cuándo usar cada enfoque

No son alternativas equivalentes; resuelven problemas distintos.

| Enfoque | Cómo funciona | Cuándo | Límite |
|---|---|---|---|
| HTML tradicional + formulario | El navegador envía el formulario y el servidor devuelve una página nueva | Sitios de contenido, formularios simples, mejor SEO, funciona sin JS | Recarga completa de página |
| `fetch` (AJAX) | JS pide datos y actualiza una parte de la página | Listas que cambian, validar sin recargar, interfaces con partes independientes | Tú escribes la sincronización entre datos y pantalla |
| Framework reactivo (React, Vue) | Describes la interfaz como función del estado; el framework la actualiza | Aplicaciones con mucho estado, muchas vistas y componentes reutilizables | Complejidad, herramientas de compilación, mayor curva |

Orden recomendado para aprender: HTML con formularios, luego `fetch` con JS puro y, solo entonces, un framework. Así entenderás qué automatiza cada capa.

## 12. Errores habituales

| Síntoma | Causa probable | Diagnóstico |
|---|---|---|
| Tildes como `Ã³` | Falta `<meta charset="utf-8">` o el archivo no es UTF-8 | Revisa la codificación del archivo en VS Code |
| Diseño minúsculo en móvil | Falta la etiqueta `viewport` | Inspecciona `<head>` |
| El estilo no se aplica | Ruta del `<link>` incorrecta | Pestaña *Red* de las DevTools: busca un 404 |
| El script no hace nada | Ruta incorrecta o error de JS | Pestaña *Consola* |
| Clic en la etiqueta no enfoca el campo | `for` e `id` no coinciden | Comprueba que el `id` es único |
| El diseño se rompe sin motivo | Etiqueta sin cerrar | Valida en [validator.w3.org/nu](https://validator.w3.org/nu/) |
| `type="module"` falla desde `file://` | Los módulos exigen un servidor | Usa `python3 -m http.server` o Vite |

## 13. Ejercicios

> **Ejercicio:** crea una página de perfil con `header`, `main`, `footer`, una foto con `alt`, y una lista de tus herramientas favoritas. Sin CSS ni JS. Valídala en el validador de W3C.

> **Ejercicio:** añade un formulario de contacto con nombre (obligatorio, mínimo 3 caracteres), correo y mensaje. Comprueba que el navegador lo valida solo. Después, añade con `setCustomValidity` una regla propia.

> **Ejercicio:** reproduce un acordeón de tres preguntas con `<details name>` y un `<dialog>` de confirmación sin escribir más de diez líneas de JS.

## 14. Documentación oficial

- [HTML en MDN](https://developer.mozilla.org/es/docs/Web/HTML)
- [Formularios en MDN](https://developer.mozilla.org/es/docs/Learn_web_development/Extensions/Forms)
- [Validador HTML de W3C](https://validator.w3.org/nu/)
- [Estado Baseline en web.dev](https://web.dev/baseline)
