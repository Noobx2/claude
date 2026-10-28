-- Lección 13: diseño de tablas, relaciones N:M, UPSERT y JSON

-- Relación muchos a muchos (N:M): un producto tiene varias etiquetas y una etiqueta, varios productos
CREATE TABLE tags (id integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY, name text NOT NULL UNIQUE);
CREATE TABLE product_tags (
    product_id integer REFERENCES products(id) ON DELETE CASCADE,
    tag_id     integer REFERENCES tags(id)     ON DELETE CASCADE,
    PRIMARY KEY (product_id, tag_id)               -- tabla intermedia
);
INSERT INTO tags (name) VALUES ('gaming'), ('office'), ('usb');
INSERT INTO product_tags VALUES (1, 1), (1, 2), (2, 2), (10, 3), (12, 3);

SELECT p.name, string_agg(t.name, ', ' ORDER BY t.name) AS tags
FROM products p
JOIN product_tags pt ON pt.product_id = p.id
JOIN tags t          ON t.id = pt.tag_id
GROUP BY p.name
ORDER BY p.name;

-- UPSERT: insertar, y si ya existe, actualizar
INSERT INTO customers (name, email, city) VALUES ('Ana Ruiz Gómez', 'ana@example.com', 'Madrid')
ON CONFLICT (email) DO UPDATE SET name = EXCLUDED.name
RETURNING id, name, email;

-- Columna JSON (jsonb): datos flexibles dentro de una base relacional
ALTER TABLE products ADD COLUMN specs jsonb;
UPDATE products SET specs = '{"color": "black", "wireless": false, "dims": {"w": 44, "h": 3}}' WHERE id = 1;
UPDATE products SET specs = '{"color": "white", "wireless": true}' WHERE id = 2;

SELECT name, specs ->> 'color' AS color, (specs ->> 'wireless')::boolean AS wireless
FROM products WHERE specs IS NOT NULL ORDER BY id;

SELECT name FROM products WHERE specs @> '{"wireless": true}';    -- @> : "contiene"
