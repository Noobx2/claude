-- Lección 8: subconsultas y CTE (WITH)

-- Productos más caros que la media
SELECT name, price FROM products WHERE price > (SELECT avg(price) FROM products) ORDER BY price DESC;

-- Clientes que han comprado algo (IN con subconsulta)
SELECT name FROM customers WHERE id IN (SELECT customer_id FROM orders) ORDER BY name;

-- EXISTS: ¿hay al menos una fila? Suele ser más eficiente que IN con muchos datos
SELECT name FROM customers c
WHERE EXISTS (SELECT 1 FROM orders o WHERE o.customer_id = c.id AND o.status = 'pending');

-- CTE: una subconsulta con nombre, para leer la consulta de arriba abajo
WITH order_totals AS (
    SELECT order_id, sum(quantity * unit_price) AS total
    FROM order_items
    GROUP BY order_id
)
SELECT c.name, round(sum(t.total), 2) AS spent
FROM order_totals t
JOIN orders o    ON o.id = t.order_id AND o.status <> 'cancelled'
JOIN customers c ON c.id = o.customer_id
GROUP BY c.name
ORDER BY spent DESC;

-- CTE recursivo: contar del 1 al 5 (sirve para jerarquías y calendarios)
WITH RECURSIVE n(i) AS (SELECT 1 UNION ALL SELECT i + 1 FROM n WHERE i < 5)
SELECT i FROM n;
