-- Lección 3: consultar (SELECT) y filtrar (WHERE)

SELECT * FROM customers;                                -- todas las columnas

SELECT name, city FROM customers;                       -- solo algunas columnas

SELECT name AS customer, city AS town FROM customers;   -- alias con AS

SELECT name, price FROM products WHERE price > 50;      -- comparación

SELECT name, city FROM customers WHERE city = 'Madrid' AND created_at >= '2025-02-01';

SELECT name FROM customers WHERE city = 'Madrid' OR city = 'Sevilla';

SELECT name FROM customers WHERE city IN ('Madrid', 'Sevilla');          -- IN: una lista

SELECT name, price FROM products WHERE price BETWEEN 10 AND 40;           -- BETWEEN: incluye extremos

SELECT name FROM customers WHERE name LIKE 'M%';        -- LIKE: % = cualquier texto

SELECT name FROM customers WHERE name ILIKE '%ez%';     -- ILIKE: sin distinguir mayúsculas (PostgreSQL)

SELECT name FROM customers WHERE city IS NULL;          -- NULL se compara con IS NULL, nunca con =

SELECT name FROM customers WHERE city <> 'Madrid';      -- ¡ojo! no incluye a quien tiene city NULL

SELECT name FROM customers WHERE city IS DISTINCT FROM 'Madrid';   -- esta sí incluye los NULL

SELECT name FROM customers WHERE NOT (city = 'Madrid');
