-- Lección 11: índices y planes de ejecución (EXPLAIN)

-- Tabla de prueba con 200 000 filas generadas
CREATE TABLE big_orders AS
SELECT g AS id, (g % 5000) + 1 AS customer_id, date '2024-01-01' + (g % 365) AS ordered_at, (random() * 500)::numeric(10,2) AS amount
FROM generate_series(1, 200000) AS g;
ANALYZE big_orders;                                       -- actualiza las estadísticas

-- Sin índice: la base lee TODA la tabla ("Seq Scan")
EXPLAIN (ANALYZE, COSTS OFF, TIMING OFF) SELECT * FROM big_orders WHERE customer_id = 1234;

CREATE INDEX idx_big_orders_customer ON big_orders (customer_id);

-- Con índice: salta directamente a las filas ("Index Scan" o "Bitmap")
EXPLAIN (ANALYZE, COSTS OFF, TIMING OFF) SELECT * FROM big_orders WHERE customer_id = 1234;

-- Índices que existen en la tabla
SELECT indexname, indexdef FROM pg_indexes WHERE tablename = 'big_orders';

DROP TABLE big_orders;
