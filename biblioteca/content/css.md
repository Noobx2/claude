# CSS moderno

CSS controla la **presentación**: color, tipografía, espacio y disposición. El CSS actual resuelve sin librerías cosas que antes exigían trucos o JavaScript.

*Verificado: 2026-10-27. Compatibilidad: consulta [MDN](https://developer.mozilla.org/es/docs/Web/CSS) y [Baseline](https://web.dev/baseline) antes de usar una función reciente.*

## 1. Archivos, nombres y enlace

| Elemento | Convención |
|---|---|
| Hoja de estilos | `styles.css` o `main.css`, en minúsculas |
| Clases | `kebab-case`: `task-list`, `btn-primary` |
| Enlazar | `<link rel="stylesheet" href="css/styles.css">` en el `<head>` |
| Con Vite | `import './styles.css'` desde un módulo JS/TS |
| Comentarios | `/* en español */` |

## 2. Sintaxis

```css
/* selector { propiedad: valor; } */
h1 {
  color: #0b6b58;
  font-size: 2rem;
}
```

### Selectores esenciales

| Selector | Elige | Ejemplo |
|---|---|---|
| Tipo | Por etiqueta | `p` |
| Clase | Por `class` | `.task` |
| Id | Por `id` (evítalo para estilos) | `#main-title` |
| Atributo | Por atributo | `input[type="email"]` |
| Descendiente | Dentro de otro | `.card p` |
| Hijo directo | Solo hijos | `ul > li` |
| Adyacente | El siguiente hermano | `h2 + p` |
| Pseudo-clase | Por estado | `a:hover`, `button:focus-visible`, `li:nth-child(odd)` |
| Pseudo-elemento | Parte generada | `p::first-line`, `.btn::after` |

### Selectores modernos

```css
/* :is() agrupa; :where() lo mismo sin sumar especificidad */
:is(h1, h2, h3) { text-wrap: balance; }

/* :not() excluye */
li:not(.done) { font-weight: 600; }

/* :has() = "selector padre": una tarjeta que contiene una imagen */
.card:has(img) { padding-top: 0; }

/* Un formulario con algún campo inválido tras interactuar */
form:has(:user-invalid) button[type="submit"] { opacity: .6; }
```

## 3. La cascada: por qué gana una regla

Cuando varias reglas afectan al mismo elemento, el navegador decide en este orden:

1. **Importancia** (`!important` gana; evítalo).
2. **Capa** (`@layer`): las capas declaradas más tarde ganan.
3. **Especificidad**: cuántos ids, clases y etiquetas tiene el selector.
4. **Orden**: a igualdad, gana la última.

| Selector | Especificidad (ids, clases, etiquetas) |
|---|---|
| `p` | 0, 0, 1 |
| `.task` | 0, 1, 0 |
| `.list .task` | 0, 2, 0 |
| `#title` | 1, 0, 0 |
| `:where(.a .b)` | 0, 0, 0 |

> **Regla:** usa clases, mantén la especificidad baja y nunca uses `!important` para «arreglar» un conflicto. Si hace falta, ordena con `@layer`.

```css
@layer reset, base, components, utilities;

@layer reset {
  *, *::before, *::after { box-sizing: border-box; }
  body { margin: 0; }
}
@layer components {
  .btn { padding: .5rem 1rem; border-radius: .5rem; }
}
```

## 4. Modelo de caja

Todo elemento es una caja: contenido, `padding`, `border` y `margin`.

```css
*, *::before, *::after { box-sizing: border-box; }   /* el ancho incluye padding y borde */
.card { padding: 1rem; border: 1px solid #ccc; margin-block: 1rem; }
```

- `margin-inline` / `margin-block` / `padding-inline` son **propiedades lógicas**: respetan el idioma y la dirección de escritura.
- Los márgenes verticales entre hermanos pueden **colapsar**. Si lo evitas con `gap` en flex o grid, no te afecta.

## 5. Unidades

| Unidad | Qué es | Cuándo |
|---|---|---|
| `rem` | Múltiplo del tamaño de fuente raíz | Tipografía y espacios; respeta la preferencia del usuario |
| `em` | Múltiplo de la fuente del elemento | Espacios que deben escalar con el texto |
| `%` | Relativo al padre | Anchos fluidos |
| `vw`, `vh`, `dvh` | Porcentaje de la ventana; `dvh` considera las barras del móvil | Pantallas completas |
| `ch` | Ancho del carácter «0» | Limitar líneas de lectura: `max-width: 65ch` |
| `fr` | Fracción del espacio libre en grid | Columnas flexibles |
| `px` | Píxel | Bordes y sombras finas |

Valores fluidos con `clamp(mínimo, ideal, máximo)`:

```css
h1 { font-size: clamp(1.75rem, 1rem + 3vw, 3rem); }
.container { width: min(100% - 2rem, 70rem); margin-inline: auto; }
```

## 6. Variables y color

```css
:root {
  --color-bg: #f4f6f3;
  --color-text: #17201c;
  --color-accent: #0b6b58;
  --space-m: 1rem;
  --radius: .5rem;
}

.btn {
  background: var(--color-accent);
  padding: var(--space-m);
  border-radius: var(--radius);
}
```

Las variables CSS se heredan, se pueden cambiar con JS (`el.style.setProperty('--color-accent', 'red')`) y son la base de los temas.

### Color moderno

```css
.a { color: oklch(55% 0.12 175); }                         /* percepción uniforme */
.b { background: color-mix(in oklch, var(--color-accent) 20%, white); }
.c { color: light-dark(#17201c, #e3ece7); }                /* sigue color-scheme */
```

### Modo claro y oscuro

```css
:root { color-scheme: light dark; }          /* controles nativos y light-dark() */

@media (prefers-color-scheme: dark) {
  :root { --color-bg: #0e1411; --color-text: #e3ece7; }
}
body { background: var(--color-bg); color: var(--color-text); }
```

## 7. Tipografía

```css
body {
  font-family: system-ui, -apple-system, "Segoe UI", sans-serif;
  line-height: 1.6;
}
code { font-family: ui-monospace, Menlo, Consolas, monospace; }
h1, h2 { text-wrap: balance; }      /* reparte el título en líneas equilibradas */
p { max-width: 65ch; text-wrap: pretty; }
```

Las fuentes de sistema no descargan nada y cargan al instante. Si usas fuentes web, declara siempre una pila de reserva.

## 8. Disposición

### Flexbox: una dimensión

```css
.toolbar {
  display: flex;
  gap: .75rem;
  align-items: center;          /* eje transversal */
  justify-content: space-between; /* eje principal */
  flex-wrap: wrap;
}
.toolbar .grow { flex: 1; }
```

### Grid: dos dimensiones

```css
/* Tarjetas que se reparten solas: tantas columnas como quepan, mínimo 16rem */
.cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(16rem, 1fr));
  gap: 1rem;
}

/* Maquetación de página */
.layout {
  display: grid;
  grid-template-columns: 16rem minmax(0, 1fr);
  grid-template-areas: "nav main";
}
```

| | Flexbox | Grid |
|---|---|---|
| Dimensión | Una (fila **o** columna) | Dos (filas **y** columnas) |
| Parte de | El contenido | El contenedor |
| Ideal | Barras, botones, centrar | Páginas, rejillas de tarjetas |

`subgrid` permite que un elemento hijo alinee sus filas con las del padre (útil en tarjetas con altura desigual).

### Otras piezas

```css
.thumb { aspect-ratio: 16 / 9; object-fit: cover; width: 100%; }
.sticky-header { position: sticky; top: 0; }
.center { display: grid; place-items: center; }
```

## 9. Diseño adaptable

Empieza por el móvil y añade reglas cuando el espacio crece (*mobile first*).

```css
.layout { display: grid; gap: 1rem; }

@media (min-width: 48rem) {
  .layout { grid-template-columns: 16rem 1fr; }
}
```

### Container queries: adaptarse al contenedor, no a la pantalla

```css
.card-wrapper { container-type: inline-size; }

.card { display: grid; gap: .5rem; }
@container (min-width: 30rem) {
  .card { grid-template-columns: 8rem 1fr; }
}
```

El componente decide su propia disposición según el espacio que le dan, y funciona igual en una barra lateral o en la página principal.

### Preferencias del usuario

```css
@media (prefers-reduced-motion: reduce) {
  * { animation: none !important; transition: none !important; }
}
```

## 10. Anidamiento nativo

```css
.card {
  padding: 1rem;
  border: 1px solid var(--color-border);

  & h2 { margin-block: 0; }
  &:hover { border-color: var(--color-accent); }
  @media (min-width: 48rem) { padding: 1.5rem; }
}
```

## 11. Transiciones y animación

```css
.btn {
  transition: background-color .15s ease, transform .15s ease;
}
.btn:hover { transform: translateY(-1px); }

@keyframes fade-in {
  from { opacity: 0; }
  to   { opacity: 1; }
}
.toast { animation: fade-in .2s ease-out; }
```

Anima `transform` y `opacity`: son baratas para el navegador. Evita animar `width`, `height` o `top`.

## 12. Funciones recientes: qué resuelven

| Función | Qué resuelve | Madurez |
|---|---|---|
| `@layer` | Orden explícito de la cascada | Estable |
| Anidamiento `&` | CSS más legible sin preprocesadores | Estable |
| `:has()` | Seleccionar por contenido o estado | Estable |
| Container queries | Componentes que se adaptan a su contenedor | Estable |
| `subgrid` | Alinear rejillas anidadas | Estable |
| `color-mix()`, `oklch()` | Paletas y variantes de color | Estable |
| `text-wrap: balance` | Títulos equilibrados | Reciente |
| `light-dark()` | Temas sin repetir variables | Reciente |
| `@property` | Variables tipadas y animables | Reciente |
| Popover y `dialog` (con CSS) | Capas superpuestas sin JS | Reciente |
| Animaciones guiadas por scroll, `@scope` | Efectos y alcance de estilos | Compatibilidad variable: comprueba antes |

> **Ojo:** «estable» o «reciente» no sustituye a comprobar. En MDN, cada propiedad muestra su tabla de compatibilidad y su estado Baseline.

## 13. Organizar el CSS

| Estrategia | Idea | Cuándo |
|---|---|---|
| CSS global con `@layer` | Un archivo por capa (reset, base, componentes) | Proyectos pequeños o JS puro |
| BEM | Nombres como `card__title--active` | Equipos y proyectos medianos |
| CSS Modules (`Card.module.css`) | Clases con alcance local; Vite lo soporta | Proyectos React/Vue |
| Utilidades (Tailwind) | Clases pequeñas en el HTML | Prototipos rápidos; es una elección de estilo, no un sustituto de saber CSS |

## 14. Depuración y errores habituales

Abre las DevTools (F12) → pestaña **Elementos**: muestra las reglas aplicadas (y las tachadas), la caja y el cálculo final.

| Síntoma | Causa probable |
|---|---|
| La regla no se aplica | Especificidad menor o `@layer` posterior; está tachada en DevTools |
| La página tiene scroll horizontal | Un hijo es más ancho que la ventana: busca `width` fijos, imágenes sin `max-width: 100%`, o falta `min-width: 0` en hijos de flex/grid |
| El contenido queda bajo la barra del móvil | Usas `100vh`; prueba `100dvh` |
| `z-index` no funciona | El elemento no está posicionado o pertenece a otro contexto de apilamiento |
| El elemento hijo no se centra | Olvidaste `display: flex/grid` en el padre |
| Texto ilegible en oscuro | Colores literales en lugar de variables |

## 15. Ejercicios

> **Ejercicio:** maqueta una lista de tareas (sin JS): cabecera, lista de tarjetas con grid `auto-fit`, y un botón. Debe verse bien a 360 px y a 1280 px.

> **Ejercicio:** define tokens en `:root` y haz que la página cambie automáticamente a modo oscuro. Después añade un botón con `data-theme` que fuerce uno u otro (con JS de tres líneas).

> **Ejercicio:** convierte una tarjeta en un componente con container query que cambie de vertical a horizontal según su ancho, y colócala en un contenedor estrecho y en uno ancho.

## 16. Documentación oficial

- [CSS en MDN](https://developer.mozilla.org/es/docs/Web/CSS)
- [Aprende CSS (web.dev)](https://web.dev/learn/css)
- [Baseline](https://web.dev/baseline)
