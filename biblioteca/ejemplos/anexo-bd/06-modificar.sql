-- Lección 6: modificar (UPDATE) y borrar (DELETE) con seguridad

BEGIN;                                              -- empieza una transacción para poder deshacer

UPDATE products SET price = price * 1.10 WHERE category_id = 4;    -- sube 10 % los cables

SELECT name, price FROM products WHERE category_id = 4;

ROLLBACK;                                           -- deshacemos: como si nada hubiera pasado

SELECT name, price FROM products WHERE category_id = 4;

-- RETURNING devuelve lo que se ha cambiado (PostgreSQL, SQLite 3.35+)
UPDATE products SET stock = stock + 20 WHERE name = 'Alfombrilla XL' RETURNING id, name, stock;

BEGIN;
DELETE FROM customers WHERE city IS NULL RETURNING id, name;       -- Sara no tiene pedidos
ROLLBACK;

-- Un DELETE sin WHERE borra TODO. Primero comprueba con un SELECT con el mismo WHERE.
