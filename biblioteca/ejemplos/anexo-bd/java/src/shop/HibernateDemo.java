package shop;

import java.util.List;
import org.hibernate.SessionFactory;
import org.hibernate.cfg.Configuration;

/** ORM con Hibernate (implementación de JPA): clases Java <-> tablas. */
public class HibernateDemo {

    public static void main(String[] args) {
        try (SessionFactory factory = new Configuration()
                .addAnnotatedClass(Category.class)
                .addAnnotatedClass(Product.class)
                .setProperty("hibernate.connection.url", "jdbc:h2:mem:orm")     // otra base: cambiar URL y dialecto/controlador
                .setProperty("hibernate.hbm2ddl.auto", "create")                // crea las tablas a partir de las clases
                .setProperty("hibernate.generate_statistics", "true")           // contar las consultas SQL
                .buildSessionFactory()) {

            // Guardar: una transacción, un grafo de objetos
            factory.inTransaction(session -> {
                Category peripherals = new Category("Periféricos");
                peripherals.add(new Product("Teclado", "59.90"));
                peripherals.add(new Product("Ratón", "24.50"));
                Category cables = new Category("Cables");
                cables.add(new Product("HDMI", "7.50"));
                session.persist(peripherals);       // CascadeType.ALL guarda también los productos
                session.persist(cables);
            });

            // Consultar con HQL (lenguaje parecido a SQL pero sobre clases)
            factory.inSession(session -> {
                List<Object[]> expensive = session.createSelectionQuery(
                        "select p.name, p.price from Product p where p.price > :min order by p.price desc", Object[].class)
                        .setParameter("min", new java.math.BigDecimal("20"))
                        .getResultList();
                expensive.forEach(row -> System.out.println("caro: " + row[0] + " " + row[1]));

                List<Object[]> perCategory = session.createSelectionQuery(
                        "select c.name, count(p) from Category c join c.products p group by c.name order by c.name", Object[].class)
                        .getResultList();
                perCategory.forEach(row -> System.out.println("categoría: " + row[0] + " = " + row[1]));
            });

            // Modificar: se cambia el objeto y Hibernate genera el UPDATE al cerrar la transacción
            factory.inTransaction(session -> {
                Product keyboard = session.createSelectionQuery("from Product where name = 'Teclado'", Product.class).getSingleResult();
                keyboard.price = new java.math.BigDecimal("49.90");
            });

            // Problema N+1 y su solución con JOIN FETCH
            factory.getStatistics().clear();
            factory.inSession(session -> {
                for (Category c : session.createSelectionQuery("from Category", Category.class).getResultList()) {
                    c.products.size();             // dispara una consulta por cada categoría
                }
            });
            System.out.println("sin optimizar: " + factory.getStatistics().getPrepareStatementCount() + " consultas");

            factory.getStatistics().clear();
            factory.inSession(session -> {
                for (Category c : session.createSelectionQuery("select distinct c from Category c join fetch c.products", Category.class).getResultList()) {
                    c.products.size();
                }
            });
            System.out.println("con join fetch: " + factory.getStatistics().getPrepareStatementCount() + " consulta");
        }
    }
}
