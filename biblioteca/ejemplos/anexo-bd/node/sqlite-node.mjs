// Acceso a datos SIN librerías externas: módulo node:sqlite incluido en Node (22.5+; estable en versiones recientes)
import { DatabaseSync } from 'node:sqlite';

const db = new DatabaseSync(':memory:');                  // base en memoria
db.exec(`
  CREATE TABLE products (id INTEGER PRIMARY KEY, name TEXT NOT NULL, price REAL NOT NULL);
`);

const insert = db.prepare('INSERT INTO products (name, price) VALUES (?, ?)');   // consulta preparada
for (const [name, price] of [['Teclado', 59.9], ['Ratón', 24.5], ['HDMI', 7.5]]) {
  insert.run(name, price);
}

const expensive = db.prepare('SELECT id, name, price FROM products WHERE price > ? ORDER BY price DESC').all(20);
console.log(expensive);          // array de objetos

const one = db.prepare('SELECT count(*) AS total FROM products').get();
console.log('total:', one.total);
db.close();
