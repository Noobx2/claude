-- Lección 12: vistas, funciones y triggers

-- Vista: una consulta guardada con nombre
CREATE VIEW order_totals AS
SELECT o.id AS order_id, c.name AS customer, o.status, sum(i.quantity * i.unit_price) AS total
FROM orders o
JOIN customers c   ON c.id = o.customer_id
JOIN order_items i ON i.order_id = o.id
GROUP BY o.id, c.name, o.status;

SELECT * FROM order_totals ORDER BY total DESC LIMIT 3;

-- Función: lógica reutilizable dentro de la base
CREATE FUNCTION price_with_vat(price numeric, vat numeric DEFAULT 0.21) RETURNS numeric
LANGUAGE sql IMMUTABLE AS $$ SELECT round(price * (1 + vat), 2) $$;

SELECT name, price, price_with_vat(price) AS with_vat FROM products ORDER BY id LIMIT 3;

-- Trigger: código que se dispara solo ante un cambio. Ejemplo: registro de cambios de precio.
CREATE TABLE price_log (
    product_id integer, old_price numeric, new_price numeric, changed_at timestamptz DEFAULT now()
);

CREATE FUNCTION log_price_change() RETURNS trigger LANGUAGE plpgsql AS $$
BEGIN
    IF NEW.price <> OLD.price THEN
        INSERT INTO price_log (product_id, old_price, new_price) VALUES (OLD.id, OLD.price, NEW.price);
    END IF;
    RETURN NEW;
END $$;

CREATE TRIGGER products_price_log AFTER UPDATE ON products
FOR EACH ROW EXECUTE FUNCTION log_price_change();

UPDATE products SET price = 62.90 WHERE id = 1;
SELECT product_id, old_price, new_price FROM price_log;
UPDATE products SET price = 59.90 WHERE id = 1;     -- lo dejamos como estaba (y queda registrado)
SELECT count(*) AS log_rows FROM price_log;
