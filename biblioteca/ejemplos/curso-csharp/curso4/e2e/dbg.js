const { chromium } = require('playwright');
(async () => {
  const b = await chromium.launch(); const p = await b.newPage({ viewport:{width:390,height:800} });
  await p.goto('http://localhost:5200/login'); await p.waitForTimeout(1200);
  await p.getByTestId('usuario-campo').fill('ana'); await p.getByTestId('clave-campo').fill('clave-segura-1');
  await p.getByTestId('alternar').click(); await p.getByTestId('enviar').click(); await p.getByRole('heading',{name:'Mis tareas'}).waitFor(); await p.getByTestId('titulo').fill('Tarea con nombre largo para probar'); await p.getByTestId('crear').click(); await p.getByTestId('fila').first().waitFor();
  const r = await p.evaluate(() => { const W = document.documentElement.clientWidth; return [...document.querySelectorAll('*')].filter(e => e.getBoundingClientRect().right > W + 1).map(e => e.tagName + '.' + e.className + ' → ' + Math.round(e.getBoundingClientRect().right) + 'px').slice(0,8) });
  console.log('ancho', 390, r);
  await p.screenshot({path:'dbg.png', fullPage:true}); await b.close();
})();
