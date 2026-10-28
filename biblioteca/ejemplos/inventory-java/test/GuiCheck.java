import inventory.InventoryWindow;
import inventory.ProductRepository;
import java.awt.Robot;
import java.awt.image.BufferedImage;
import java.io.File;
import java.nio.file.Path;
import java.util.concurrent.atomic.AtomicReference;
import javax.imageio.ImageIO;
import javax.swing.SwingUtilities;

/** Comprueba la ventana en un servidor de pantalla virtual: rellena el formulario, pulsa «Añadir» y captura la imagen. */
public class GuiCheck {
    public static void main(String[] args) throws Exception {
        Path file = Path.of(args[0]);
        ProductRepository repository = new ProductRepository();
        repository.load(file);

        AtomicReference<InventoryWindow> ref = new AtomicReference<>();
        SwingUtilities.invokeAndWait(() -> {
            InventoryWindow window = new InventoryWindow(repository, file);
            window.setVisible(true);
            ref.set(window);
        });
        InventoryWindow window = ref.get();

        int before = window.rowCount();
        SwingUtilities.invokeAndWait(() -> {
            window.fillForm("Ratón inalámbrico", "Periféricos", "19,95", "25");
            window.addFromForm();
        });
        System.out.println("filas antes: " + before + ", después: " + window.rowCount());

        Thread.sleep(500);
        BufferedImage image = new Robot().createScreenCapture(window.getBounds());
        ImageIO.write(image, "png", new File(args[1]));
        System.exit(window.rowCount() == before + 1 ? 0 : 1);
    }
}
