const { chromium } = require('playwright');
const fs = require('node:fs');
const archivo = '../ServerDemo/Components/Pages/Contador.razor';
(async () => {
  const original = fs.readFileSync(archivo, 'utf8');
  const navegador = await chromium.launch(); const page = await navegador.newPage();
  await page.goto('http://localhost:5244/contador');
  await page.waitForSelector('#sumar');
  await page.waitForTimeout(1000);                         // espera a que el circuito SignalR esté conectado
  for (let i = 0; i < 3; i++) await page.click('#sumar');
  await page.waitForFunction(() => document.querySelector('#cuenta').textContent === '3');
  await page.evaluate(() => { window.__marca = 'sin-recargar'; });
  console.log('1. Tras 3 clics: cuenta =', await page.textContent('#cuenta'), '| título =', await page.textContent('h1'));

  fs.writeFileSync(archivo, original.replace('Contador (Blazor Server)', 'Contador EDITADO EN CALIENTE'));
  console.log('2. Se editó el archivo .razor en disco (cambio del título)…');
  const t0 = Date.now();
  await page.waitForFunction(() => document.querySelector('h1').textContent.includes('EDITADO'), null, { timeout: 30000 });
  console.log(`3. La página mostró el cambio a los ${Date.now() - t0} ms | título = ${await page.textContent('h1')}`);
  console.log('   ¿Se recargó la página? ', (await page.evaluate(() => window.__marca)) === 'sin-recargar' ? 'No (la marca de la ventana sigue ahí)' : 'Sí');
  console.log('   Cuenta tras el cambio:', await page.textContent('#cuenta'), '(el estado del componente se conserva si el cambio es solo de marcado)');

  // Cambio de código: ahora Sumar suma 10
  fs.writeFileSync(archivo, original.replace('Contador (Blazor Server)', 'Contador EDITADO EN CALIENTE').replace('_cuenta++;', '_cuenta += 10;'));
  await page.waitForTimeout(6000);
  await page.click('#sumar');
  await page.waitForTimeout(500);
  console.log('4. Se editó también el CÓDIGO C# (Sumar suma 10). Un clic más → cuenta =', await page.textContent('#cuenta'), '(3 + 10 = 13 si el cambio se aplicó sin reiniciar)');
  fs.writeFileSync(archivo, original);                       // restauramos el archivo
  await navegador.close();
})().catch(e => { console.error('FALLO', e.message.split('\n')[0]); fs.writeFileSync(archivo, fs.readFileSync(archivo, 'utf8')); process.exit(1); });
