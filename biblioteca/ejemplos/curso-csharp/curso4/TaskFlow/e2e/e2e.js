// Prueba de extremo a extremo (E2E): un navegador real usa la aplicación como lo haría una persona.
const { chromium } = require('playwright');
const assert = require('node:assert/strict');

const WEB = 'http://localhost:5200';
const paso = (n, texto) => console.log(`${String(n).padStart(2)}. ${texto}`);

(async () => {
  const navegador = await chromium.launch();
  const contexto = await navegador.newContext({ viewport: { width: 1000, height: 900 }, locale: 'es-ES' });
  const page = await contexto.newPage();
  const erroresConsola = [];
  page.on('console', m => { if (m.type() === 'error') erroresConsola.push(m.text()); });
  page.on('pageerror', e => erroresConsola.push('EXCEPCIÓN: ' + e.message));

  // 1) Sin sesión, la ruta protegida redirige a /login
  await page.goto(WEB + '/');
  await page.waitForURL('**/login');
  paso(1, 'Sin sesión, «/» redirige a /login');

  // 2) Validación del formulario en el navegador, sin llamar al servidor
  await page.getByTestId('enviar').click();
  await page.getByText('Escribe tu usuario').waitFor();
  paso(2, 'Formulario vacío → mensajes de validación («Escribe tu usuario»)');

  // 3) Registro con contraseña corta: lo rechaza el cliente
  await page.getByTestId('alternar').click();
  await page.getByTestId('usuario-campo').fill('ana');
  await page.getByTestId('clave-campo').fill('123');
  await page.getByTestId('enviar').click();
  await page.getByText('Mínimo 8 caracteres').waitFor();
  paso(3, 'Contraseña de 3 caracteres → «Mínimo 8 caracteres» (validado en el cliente)');

  // 4) Registro correcto: entra directamente
  await page.getByTestId('clave-campo').fill('clave-segura-1');
  await page.getByTestId('enviar').click();
  await page.getByRole('heading', { name: 'Mis tareas' }).waitFor();
  assert.match(await page.getByTestId('usuario').innerText(), /ana\s+Admin/);
  paso(4, 'Registro + entrada → «Mis tareas», usuario «ana Admin» (primer usuario = Admin)');
  await page.screenshot({ path: 'e2e-01-vacia.png' });

  // 5) Validación del formulario de tareas
  await page.getByTestId('titulo').fill('ab');
  await page.getByTestId('crear').click();
  await page.getByText('El título debe tener entre 3 y 80 caracteres').waitFor();
  paso(5, 'Título de 2 letras → mensaje de validación en el formulario');

  // 6) Regla del DOMINIO (el cliente no la conoce): fecha en el pasado → error que viene del servidor
  await page.getByTestId('titulo').fill('Tarea antigua');
  await page.getByTestId('fecha').fill('2020-01-01');
  await page.getByTestId('crear').click();
  await page.getByTestId('error').waitFor();
  assert.match(await page.getByTestId('error').innerText(), /pasado/);
  paso(6, 'Fecha pasada → la API rechaza y la página muestra: «' + (await page.getByTestId('error').innerText()).replace(/\s*cerrar\s*$/, '') + '»');

  // 7) Crear tareas válidas
  const crear = async (titulo, prioridad, fecha, descripcion) => {
    await page.getByTestId('titulo').fill(titulo);
    await page.getByTestId('descripcion').fill(descripcion ?? '');
    await page.getByTestId('prioridad').selectOption(prioridad);
    await page.getByTestId('fecha').fill(fecha ?? '');
    await page.getByTestId('crear').click();
    await page.getByRole('cell', { name: titulo }).first().waitFor();
  };
  await crear('Estudiar Blazor', 'High', '2030-01-15', 'Componentes y formularios');
  await crear('Leer sobre EF Core', 'Medium', '', '');
  await crear('Revisar JWT', 'Low', '2031-03-01', '');
  assert.equal(await page.getByTestId('fila').count(), 3);
  paso(7, 'Tres tareas creadas → tabla con 3 filas');
  await page.screenshot({ path: 'e2e-02-tareas.png' });

  // 8) Completar y comprobar estadísticas
  await page.getByTestId('completar').first().click();
  await page.waitForFunction(() => document.querySelector('[data-testid=estadisticas]')?.innerText.includes('Hechas\n1') || document.querySelector('[data-testid=estadisticas]')?.innerText.replace(/\s+/g,' ').includes('Hechas 1'));
  paso(8, 'Completar una tarea → estadísticas: ' + (await page.getByTestId('estadisticas').innerText()).replace(/\s+/g, ' '));

  // 9) Filtros (el servidor filtra: la tabla cambia)
  await page.getByTestId('estado').selectOption('hechas');
  await page.waitForFunction(() => document.querySelectorAll('[data-testid=fila]').length === 1);
  paso(9, 'Filtro «Hechas» → 1 fila');
  await page.getByTestId('estado').selectOption('');
  await page.getByTestId('buscar').fill('jwt');
  await page.waitForFunction(() => document.querySelectorAll('[data-testid=fila]').length === 1);
  assert.match(await page.getByTestId('fila').innerText(), /Revisar JWT/);
  paso(10, 'Búsqueda «jwt» (sin distinguir mayúsculas) → solo «Revisar JWT»');
  await page.getByTestId('buscar').fill('');
  await page.waitForFunction(() => document.querySelectorAll('[data-testid=fila]').length === 3);

  // 11) Sobrevivir a F5 (sessionStorage)
  await page.reload();
  await page.getByRole('heading', { name: 'Mis tareas' }).waitFor();
  paso(11, 'Recarga (F5) → sigue la sesión y los datos');

  // 12) Eliminar como admin
  await page.getByTestId('eliminar').last().click();
  await page.waitForFunction(() => document.querySelectorAll('[data-testid=fila]').length === 2);
  paso(12, 'Eliminar como Admin → 2 filas');

  // 13) Salir y entrar como usuario normal
  await page.getByTestId('salir').click();
  await page.waitForURL('**/login');
  await page.getByTestId('alternar').click();
  await page.getByTestId('usuario-campo').fill('luis');
  await page.getByTestId('clave-campo').fill('otra-clave-2');
  await page.getByTestId('enviar').click();
  await page.getByRole('heading', { name: 'Mis tareas' }).waitFor();
  assert.match(await page.getByTestId('usuario').innerText(), /luis\s+User/);
  assert.equal(await page.getByTestId('eliminar').count(), 0);
  paso(13, 'Usuario «luis» (rol User): la interfaz NO muestra «Eliminar»');

  // 14) Pero esconder un botón NO es seguridad: lo comprobamos atacando la API directamente desde el navegador
  const estado = await page.evaluate(async () => {
    const t = sessionStorage.getItem('taskflow.token');
    const r = await fetch('http://localhost:5099/tasks/1', { method: 'DELETE', headers: { Authorization: 'Bearer ' + t } });
    return r.status;
  });
  assert.equal(estado, 403);
  paso(14, 'Saltándose la interfaz: DELETE directo a la API con el token de «luis» → ' + estado + ' (el servidor es quien protege)');

  // 15) Credenciales incorrectas
  await page.getByTestId('salir').click();
  await page.waitForURL('**/login');
  await page.getByTestId('usuario-campo').fill('ana');
  await page.getByTestId('clave-campo').fill('contraseña-incorrecta');
  await page.getByTestId('enviar').click();
  await page.getByTestId('error').waitFor();
  paso(15, 'Contraseña incorrecta → «' + await page.getByTestId('error').innerText() + '»');

  // 16) Móvil
  await page.setViewportSize({ width: 390, height: 800 });
  await page.getByTestId('clave-campo').fill('clave-segura-1');
  await page.getByTestId('enviar').click();
  await page.getByRole('heading', { name: 'Mis tareas' }).waitFor();
  const desborde = await page.evaluate(() => document.documentElement.scrollWidth > document.documentElement.clientWidth);
  if (desborde) {
    const culpables = await page.evaluate(() => { const W = document.documentElement.clientWidth; return [...document.querySelectorAll('body *')].filter(e => e.getBoundingClientRect().right > W + 1).slice(0, 4).map(e => e.tagName + '.' + e.className); });
    console.log('   desborde por:', culpables);
  }
  assert.equal(desborde, false);
  paso(16, 'Ancho de móvil (390 px): sin desplazamiento horizontal');
  await page.screenshot({ path: 'e2e-03-movil.png', fullPage: true });

  // 17) API caída: mensaje claro
  const esperados = erroresConsola.filter(e => /status of (400|401|403)/.test(e));      // los provocamos a propósito en los pasos 6, 14 y 15
  const inesperados = erroresConsola.filter(e => !esperados.includes(e));
  assert.equal(inesperados.length, 0, 'errores inesperados: ' + inesperados.join(' | '));
  paso(17, `Consola del navegador: ${esperados.length} respuestas 400/401/403 (las provocadas a propósito en los pasos 6, 14 y 15) y 0 errores inesperados`);
  await navegador.close();
  console.log('\nRESULTADO: todos los pasos pasaron');
})().catch(e => { console.error('FALLO:', e.message.split('\n')[0]); process.exit(1); });
