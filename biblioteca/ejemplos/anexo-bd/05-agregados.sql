-- Lección 5: agregar (COUNT, SUM, AVG…) y agrupar (GROUP BY, HAVING)

SELECT count(*) AS total_products, min(price) AS cheapest, max(price) AS most_expensive,
       round(avg(price), 2) AS average_price, sum(stock) AS total_units
FROM products;

SELECT count(*) AS all_rows, count(city) AS rows_with_city FROM customers;   -- count(col) ignora los NULL

SELECT city, count(*) AS customers
FROM customers
GROUP BY city
ORDER BY customers DESC, city;

SELECT category_id, count(*) AS products, round(avg(price), 2) AS avg_price
FROM products
GROUP BY category_id
ORDER BY category_id;

-- HAVING filtra grupos (WHERE filtra filas antes de agrupar)
SELECT city, count(*) AS customers
FROM customers
WHERE city IS NOT NULL
GROUP BY city
HAVING count(*) > 1;

SELECT order_id, sum(quantity * unit_price) AS order_total
FROM order_items
GROUP BY order_id
ORDER BY order_id;
