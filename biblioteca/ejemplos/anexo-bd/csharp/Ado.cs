using Microsoft.Data.Sqlite;

// ADO.NET: acceso a datos SIN ORM. Cada proveedor (SQLite, SQL Server, PostgreSQL) ofrece las mismas piezas:
// Connection (conexión), Command (orden SQL), Parameter (dato aparte) y DataReader (lector de filas).
public static class Ado
{
    public static void Run()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        using (var create = connection.CreateCommand())
        {
            create.CommandText = """
                CREATE TABLE products (id INTEGER PRIMARY KEY, name TEXT NOT NULL, price REAL NOT NULL);
                """;
            create.ExecuteNonQuery();                    // ExecuteNonQuery: órdenes que no devuelven filas
        }

        // Transacción: o se guardan las tres filas o ninguna
        using (var transaction = connection.BeginTransaction())
        {
            foreach (var (name, price) in new[] { ("Teclado", 59.9), ("Ratón", 24.5), ("HDMI", 7.5) })
            {
                using var insert = connection.CreateCommand();
                insert.Transaction = transaction;
                insert.CommandText = "INSERT INTO products (name, price) VALUES ($name, $price)";
                insert.Parameters.AddWithValue("$name", name);      // parámetro: evita la inyección SQL
                insert.Parameters.AddWithValue("$price", price);
                insert.ExecuteNonQuery();
            }
            transaction.Commit();
        }

        using var select = connection.CreateCommand();
        select.CommandText = "SELECT id, name, price FROM products WHERE price > $min ORDER BY price DESC";
        select.Parameters.AddWithValue("$min", 20);
        using var reader = select.ExecuteReader();               // ExecuteReader: devuelve filas
        while (reader.Read())
            Console.WriteLine($"{reader.GetInt32(0)} | {reader.GetString(1),-8} | {reader.GetDouble(2),6:F2}");

        using var count = connection.CreateCommand();
        count.CommandText = "SELECT count(*) FROM products";
        Console.WriteLine($"total: {count.ExecuteScalar()}");    // ExecuteScalar: un único valor
    }
}
