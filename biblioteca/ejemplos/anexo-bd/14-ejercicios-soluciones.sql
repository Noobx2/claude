-- Soluciones de los ejercicios del curso (intenta primero resolverlos tú)

-- Ej. 1: nombre y precio de los productos con stock 0
SELECT name, price FROM products WHERE stock = 0;

-- Ej. 2: los 3 clientes que más han gastado (sin contar pedidos cancelados)
SELECT c.name, sum(i.quantity * i.unit_price) AS spent
FROM customers c
JOIN orders o ON o.customer_id = c.id AND o.status <> 'cancelled'
JOIN order_items i ON i.order_id = o.id
GROUP BY c.name ORDER BY spent DESC LIMIT 3;

-- Ej. 3: productos que nunca se han vendido
SELECT p.name FROM products p LEFT JOIN order_items i ON i.product_id = p.id WHERE i.product_id IS NULL ORDER BY p.name;

-- Ej. 4: número de pedidos por estado, incluyendo estados con 0 pedidos
SELECT s.status, count(o.id) AS orders
FROM (VALUES ('pending'), ('paid'), ('shipped'), ('cancelled')) AS s(status)
LEFT JOIN orders o ON o.status = s.status
GROUP BY s.status ORDER BY s.status;

-- Ej. 5: para cada ciudad, el cliente más antiguo
SELECT DISTINCT ON (city) city, name, created_at FROM customers WHERE city IS NOT NULL ORDER BY city, created_at;

-- Ej. 6: ticket medio (importe medio por pedido no cancelado)
SELECT round(avg(total), 2) AS average_order
FROM (SELECT o.id, sum(i.quantity * i.unit_price) AS total
      FROM orders o JOIN order_items i ON i.order_id = o.id WHERE o.status <> 'cancelled' GROUP BY o.id) t;

-- Ej. 7: productos de la categoría con más unidades vendidas
WITH sold AS (
  SELECT p.category_id, sum(i.quantity) AS units FROM order_items i JOIN products p ON p.id = i.product_id GROUP BY p.category_id
)
SELECT p.name FROM products p WHERE p.category_id = (SELECT category_id FROM sold ORDER BY units DESC LIMIT 1) ORDER BY p.name;
