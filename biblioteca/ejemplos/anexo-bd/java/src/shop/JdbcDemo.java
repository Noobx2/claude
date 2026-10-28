package shop;

import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;

/** JDBC: acceso a datos SIN ORM, solo con la biblioteca estándar (el controlador de la base va aparte). */
public class JdbcDemo {

    public static void main(String[] args) throws SQLException {
        // La URL indica el controlador y la base. H2 en memoria: desaparece al cerrar la conexión.
        try (Connection connection = DriverManager.getConnection("jdbc:h2:mem:shop")) {

            try (Statement statement = connection.createStatement()) {
                statement.execute("CREATE TABLE products (id INT AUTO_INCREMENT PRIMARY KEY, name VARCHAR(100) NOT NULL, price DECIMAL(10,2) NOT NULL)");
            }

            // Transacción manual: desactivamos el autocommit para agrupar varias órdenes
            connection.setAutoCommit(false);
            try (PreparedStatement insert = connection.prepareStatement("INSERT INTO products (name, price) VALUES (?, ?)")) {
                insert.setString(1, "Teclado");  insert.setBigDecimal(2, new java.math.BigDecimal("59.90")); insert.addBatch();
                insert.setString(1, "Ratón");    insert.setBigDecimal(2, new java.math.BigDecimal("24.50")); insert.addBatch();
                insert.setString(1, "HDMI");     insert.setBigDecimal(2, new java.math.BigDecimal("7.50"));  insert.addBatch();
                insert.executeBatch();           // varias inserciones en un solo envío
            }
            connection.commit();

            // PreparedStatement con parámetro "?": evita la inyección SQL
            try (PreparedStatement select = connection.prepareStatement("SELECT id, name, price FROM products WHERE price > ? ORDER BY price DESC")) {
                select.setBigDecimal(1, new java.math.BigDecimal("20"));
                try (ResultSet rows = select.executeQuery()) {
                    while (rows.next()) {
                        System.out.printf("%d | %-8s | %6.2f%n", rows.getInt("id"), rows.getString("name"), rows.getBigDecimal("price"));
                    }
                }
            }
        }
    }
}
