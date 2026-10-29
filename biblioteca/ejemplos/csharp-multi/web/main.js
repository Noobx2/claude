// Interfaz web que habla con el backend de C# solo por HTTP + JSON.
// No sabe nada de C#: podría ser Node, Java o Python detrás.
const euro = new Intl.NumberFormat('es-ES', { style: 'currency', currency: 'EUR' });
const tbody = document.querySelector('#table tbody');

async function load() {
  const q = document.querySelector('#q').value;
  const response = await fetch('/api/products' + (q ? `?q=${encodeURIComponent(q)}` : ''));
  const products = await response.json();
  tbody.replaceChildren(...products.map((p) => {
    const row = document.createElement('tr');
    row.innerHTML = `<td></td><td></td><td>${euro.format(p.price)}</td><td>${p.stock}</td>`;
    row.children[0].textContent = p.name;       // textContent: evita inyectar HTML (XSS)
    row.children[1].textContent = p.category;
    return row;
  }));
  document.querySelector('#summary').textContent = await (await fetch('/api/summary')).text();
}

document.querySelector('#search').addEventListener('submit', (e) => { e.preventDefault(); load(); });
document.querySelector('#q').addEventListener('input', load);

document.querySelector('#add').addEventListener('submit', async (e) => {
  e.preventDefault();
  const form = new FormData(e.target);
  const response = await fetch('/api/products', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({
      name: form.get('name'), category: form.get('category'),
      price: Number(form.get('price')), stock: Number(form.get('stock')),
    }),
  });
  // Las reglas de validación viven en el Core de C#; aquí solo se muestra el mensaje
  document.querySelector('#error').textContent = response.ok ? '' : (await response.json()).error;
  if (response.ok) { e.target.reset(); load(); }
});

load();
