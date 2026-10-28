-- Lección 9: funciones de ventana (OVER): calcular sin colapsar filas

-- Ranking de productos por precio dentro de cada categoría
SELECT category_id, name, price,
       rank()       OVER (PARTITION BY category_id ORDER BY price DESC) AS rank_in_category,
       row_number() OVER (ORDER BY price DESC)                          AS overall_position
FROM products
ORDER BY category_id, rank_in_category;

-- Acumulado de ventas por pedido
SELECT o.id, o.ordered_at, t.total,
       sum(t.total) OVER (ORDER BY o.ordered_at, o.id) AS running_total
FROM orders o
JOIN (SELECT order_id, sum(quantity * unit_price) AS total FROM order_items GROUP BY order_id) t ON t.order_id = o.id
WHERE o.status <> 'cancelled'
ORDER BY o.ordered_at, o.id;

-- Comparar cada pedido con el anterior del mismo cliente
SELECT customer_id, id, ordered_at,
       lag(ordered_at) OVER (PARTITION BY customer_id ORDER BY ordered_at) AS previous_order
FROM orders
ORDER BY customer_id, ordered_at;
