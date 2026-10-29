// Mide la PRIMERA carga (sin caché) de la misma clase de aplicación en Blazor Server y en Blazor WebAssembly
const { chromium } = require('playwright');
const fs = require('node:fs');

async function medir(nombre, url, listoSelector) {
  const navegador = await chromium.launch();
  const ctx = await navegador.newContext();
  const page = await ctx.newPage();
  const cdp = await ctx.newCDPSession(page);
  await cdp.send('Network.enable');
  let bytes = 0, peticiones = 0;
  cdp.on('Network.loadingFinished', e => { bytes += e.encodedDataLength; peticiones++; });
  const t0 = Date.now();
  await page.goto(url);
  await page.waitForSelector(listoSelector, { timeout: 60000 });
  const msListo = Date.now() - t0;
  await page.waitForTimeout(500);
  await navegador.close();
  return { nombre, kb: (bytes / 1024).toFixed(0), peticiones, msListo };
}

(async () => {
  const filas = [
    await medir('Blazor Server (servidor)', 'http://localhost:5244/contador', '#sumar'),
    await medir('Blazor WebAssembly (navegador)', 'http://localhost:5200/login', '[data-testid=enviar]'),
  ];
  console.log('Primera carga SIN caché, en local (sin latencia de red real):');
  console.log('  ' + 'Modelo'.padEnd(34) + 'Descarga'.padStart(10) + 'Peticiones'.padStart(12) + 'Listo en'.padStart(11));
  for (const f of filas) console.log('  ' + f.nombre.padEnd(34) + (f.kb + ' KB').padStart(10) + String(f.peticiones).padStart(12) + (f.msListo + ' ms').padStart(11));
})();
