-- Lección 1: crear las tablas de una tienda online
-- (una sentencia SQL termina siempre en punto y coma)

CREATE TABLE categories (
    id    integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,  -- clave primaria autonumérica
    name  text NOT NULL UNIQUE                               -- obligatorio y sin repetidos
);

CREATE TABLE customers (
    id          integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name        text NOT NULL,
    email       text NOT NULL UNIQUE,
    city        text,                                        -- puede quedar vacío (NULL)
    created_at  date NOT NULL DEFAULT CURRENT_DATE           -- valor por defecto
);

CREATE TABLE products (
    id           integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    name         text NOT NULL,
    price        numeric(10,2) NOT NULL CHECK (price >= 0),  -- regla: no negativo
    stock        integer NOT NULL DEFAULT 0 CHECK (stock >= 0),
    category_id  integer NOT NULL REFERENCES categories(id)  -- clave foránea
);

CREATE TABLE orders (
    id           integer GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    customer_id  integer NOT NULL REFERENCES customers(id),
    ordered_at   date NOT NULL,
    status       text NOT NULL DEFAULT 'pending'
                 CHECK (status IN ('pending', 'paid', 'shipped', 'cancelled'))
);

CREATE TABLE order_items (
    order_id    integer NOT NULL REFERENCES orders(id) ON DELETE CASCADE,
    product_id  integer NOT NULL REFERENCES products(id),
    quantity    integer NOT NULL CHECK (quantity > 0),
    unit_price  numeric(10,2) NOT NULL,                      -- precio en el momento de la compra
    PRIMARY KEY (order_id, product_id)                       -- clave compuesta
);

\dt
\d products
