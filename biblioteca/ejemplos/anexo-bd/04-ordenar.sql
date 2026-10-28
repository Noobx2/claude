-- Lección 4: ordenar, limitar, eliminar repetidos y calcular

SELECT name, price FROM products ORDER BY price DESC;               -- DESC = de mayor a menor

SELECT name, price FROM products ORDER BY price DESC LIMIT 3;       -- los 3 más caros

SELECT name, price FROM products ORDER BY price DESC LIMIT 3 OFFSET 3;   -- paginación: saltar 3

SELECT DISTINCT city FROM customers ORDER BY city;                  -- sin repetidos

SELECT name, price, stock, price * stock AS stock_value FROM products ORDER BY stock_value DESC LIMIT 5;

SELECT name,
       CASE
         WHEN price < 10  THEN 'cheap'
         WHEN price < 100 THEN 'medium'
         ELSE 'expensive'
       END AS price_range
FROM products
ORDER BY price;

SELECT name, coalesce(city, 'unknown') AS city FROM customers ORDER BY name;   -- coalesce: primer valor no nulo

SELECT upper(name), length(name), round(price * 1.21, 2) AS price_with_vat FROM products LIMIT 3;
