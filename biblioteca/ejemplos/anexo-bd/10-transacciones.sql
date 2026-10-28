-- Lección 10: transacciones (todo o nada)

-- Vender: crear el pedido, la línea y descontar el stock. Si algo falla, no se hace nada.
BEGIN;
INSERT INTO orders (customer_id, ordered_at, status) VALUES (2, '2025-07-10', 'paid') RETURNING id;
INSERT INTO order_items (order_id, product_id, quantity, unit_price) VALUES (10, 6, 2, 79.90);
UPDATE products SET stock = stock - 2 WHERE id = 6;
COMMIT;                                                  -- confirma todo a la vez

SELECT id, name, stock FROM products WHERE id = 6;

-- Un error dentro de una transacción la anula
BEGIN;
UPDATE products SET stock = stock - 500 WHERE id = 6;    -- viola CHECK (stock >= 0)
ROLLBACK;

-- SAVEPOINT: deshacer solo una parte
BEGIN;
UPDATE products SET price = 1 WHERE id = 1;
SAVEPOINT before_second_change;
UPDATE products SET price = 2 WHERE id = 2;
ROLLBACK TO SAVEPOINT before_second_change;              -- deshace solo el segundo cambio
COMMIT;
SELECT id, name, price FROM products WHERE id IN (1, 2) ORDER BY id;

-- Restauramos el precio del teclado para las siguientes lecciones
UPDATE products SET price = 59.90 WHERE id = 1;
