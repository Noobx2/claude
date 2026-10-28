-- Lección 7: unir tablas (JOIN)

-- INNER JOIN: solo filas que tienen pareja en ambas tablas
SELECT o.id AS order_id, c.name AS customer, o.ordered_at, o.status
FROM orders AS o
INNER JOIN customers AS c ON c.id = o.customer_id
ORDER BY o.id;

-- LEFT JOIN: todos los clientes, tengan o no pedidos
SELECT c.name, count(o.id) AS orders
FROM customers c
LEFT JOIN orders o ON o.customer_id = c.id
GROUP BY c.id, c.name
ORDER BY orders DESC, c.name;

-- Clientes SIN pedidos: LEFT JOIN + IS NULL
SELECT c.name
FROM customers c
LEFT JOIN orders o ON o.customer_id = c.id
WHERE o.id IS NULL;

-- Varias tablas: detalle de cada pedido
SELECT o.id AS order_id, c.name AS customer, p.name AS product, i.quantity, i.unit_price,
       i.quantity * i.unit_price AS line_total
FROM orders o
JOIN customers   c ON c.id = o.customer_id
JOIN order_items i ON i.order_id = o.id
JOIN products    p ON p.id = i.product_id
WHERE o.id IN (1, 5)
ORDER BY o.id, p.name;

-- Ventas por categoría
SELECT cat.name AS category, sum(i.quantity) AS units, sum(i.quantity * i.unit_price) AS revenue
FROM order_items i
JOIN products   p   ON p.id = i.product_id
JOIN categories cat ON cat.id = p.category_id
JOIN orders     o   ON o.id = i.order_id AND o.status <> 'cancelled'
GROUP BY cat.name
ORDER BY revenue DESC;

-- CROSS JOIN: todas las combinaciones (rara vez se necesita)
SELECT c.name AS category, s.status
FROM categories c CROSS JOIN (VALUES ('pending'), ('paid')) AS s(status)
ORDER BY c.name, s.status LIMIT 4;
