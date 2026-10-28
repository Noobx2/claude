-- Lección 2: insertar datos (INSERT)

INSERT INTO categories (name) VALUES ('Periféricos'), ('Monitores'), ('Almacenamiento'), ('Cables');

INSERT INTO customers (name, email, city, created_at) VALUES
  ('Ana Ruiz',      'ana@example.com',    'Madrid',    '2025-01-10'),
  ('Luis Pérez',    'luis@example.com',  'Sevilla',   '2025-02-03'),
  ('Marta Gil',     'marta@example.com', 'Madrid',    '2025-02-20'),
  ('Pedro León',    'pedro@example.com', 'Valencia',  '2025-03-15'),
  ('Sara Núñez',    'sara@example.com',  NULL,        '2025-04-01'),
  ('Iván Soto',     'ivan@example.com',  'Sevilla',   '2025-04-22'),
  ('Elena Mora',    'elena@example.com', 'Bilbao',    '2025-05-05'),
  ('Raúl Vega',     'raul@example.com',  'Madrid',    '2025-06-30');

INSERT INTO products (name, price, stock, category_id) VALUES
  ('Teclado mecánico',   59.90, 25, 1),
  ('Ratón inalámbrico',  24.50, 60, 1),
  ('Webcam HD',          39.00, 15, 1),
  ('Monitor 24"',       139.00, 10, 2),
  ('Monitor 27" 4K',    329.00,  4, 2),
  ('SSD 1 TB',           79.90, 30, 3),
  ('Disco externo 2 TB', 89.00, 12, 3),
  ('Memoria USB 64 GB',   9.90, 100, 3),
  ('Cable HDMI 2 m',      7.50, 80, 4),
  ('Cable USB-C',         6.90, 120, 4),
  ('Alfombrilla XL',     14.00, 0, 1),
  ('Hub USB 4 puertos',  19.90, 35, 1);

INSERT INTO orders (customer_id, ordered_at, status) VALUES
  (1, '2025-03-01', 'shipped'),
  (1, '2025-04-12', 'paid'),
  (2, '2025-03-18', 'shipped'),
  (3, '2025-04-02', 'cancelled'),
  (3, '2025-05-10', 'paid'),
  (4, '2025-05-21', 'pending'),
  (6, '2025-06-02', 'shipped'),
  (7, '2025-06-15', 'paid'),
  (1, '2025-07-01', 'pending');

INSERT INTO order_items (order_id, product_id, quantity, unit_price) VALUES
  (1, 1, 1, 59.90), (1, 2, 1, 24.50),
  (2, 4, 2, 139.00),
  (3, 6, 1, 79.90), (3, 10, 3, 6.90),
  (4, 5, 1, 329.00),
  (5, 3, 1, 39.00), (5, 12, 2, 19.90), (5, 9, 2, 7.50),
  (6, 7, 1, 89.00),
  (7, 8, 5, 9.90), (7, 2, 2, 24.50),
  (8, 1, 1, 59.90), (8, 4, 1, 139.00), (8, 6, 1, 79.90),
  (9, 9, 4, 7.50);

SELECT 'customers' AS table_name, count(*) FROM customers
UNION ALL SELECT 'products', count(*) FROM products
UNION ALL SELECT 'orders', count(*) FROM orders
UNION ALL SELECT 'order_items', count(*) FROM order_items;
