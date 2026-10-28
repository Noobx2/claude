package inventory;

import java.math.BigDecimal;
import java.util.Scanner;

/**
 * Captura de datos por consola. Cada método REPITE la pregunta hasta recibir un dato válido,
 * así el programa nunca se rompe por lo que escriba el usuario.
 */
public class Input {

    private final Scanner scanner;

    public Input(Scanner scanner) {
        this.scanner = scanner;
    }

    /** Lee una línea de texto; si el usuario no escribe nada y hay valor por defecto, lo usa. */
    public String text(String prompt, String defaultValue) {
        System.out.print(defaultValue == null ? prompt + ": " : prompt + " [" + defaultValue + "]: ");
        String line = readLine().strip();
        return line.isEmpty() && defaultValue != null ? defaultValue : line;
    }

    /** Texto obligatorio: insiste mientras esté vacío. */
    public String requiredText(String prompt) {
        while (true) {
            String value = text(prompt, null);
            if (!value.isEmpty()) {
                return value;
            }
            System.out.println("  Este dato es obligatorio.");
        }
    }

    /** Entero dentro de un rango. */
    public int integer(String prompt, int min, int max) {
        return integer(prompt, min, max, null);
    }

    /** Entero dentro de un rango; si el usuario pulsa Enter y hay valor por defecto, lo usa. */
    public int integer(String prompt, int min, int max, Integer defaultValue) {
        while (true) {
            String raw = text(prompt, defaultValue == null ? null : String.valueOf(defaultValue));
            try {
                int value = Integer.parseInt(raw);
                if (value >= min && value <= max) {
                    return value;
                }
                System.out.printf("  Debe estar entre %d y %d.%n", min, max);
            } catch (NumberFormatException e) {
                System.out.println("  Escribe un número entero (por ejemplo 12).");
            }
        }
    }

    /** Número decimal; acepta coma o punto como separador. */
    public BigDecimal decimal(String prompt) {
        return decimal(prompt, null);
    }

    /** Número decimal con valor por defecto (Enter lo mantiene). */
    public BigDecimal decimal(String prompt, BigDecimal defaultValue) {
        while (true) {
            String raw = text(prompt, defaultValue == null ? null : defaultValue.toPlainString()).replace(',', '.');
            try {
                BigDecimal value = new BigDecimal(raw);
                if (value.signum() >= 0) {
                    return value;
                }
                System.out.println("  No puede ser negativo.");
            } catch (NumberFormatException e) {
                System.out.println("  Escribe un número (por ejemplo 12,50).");
            }
        }
    }

    /** Pregunta sí/no. */
    public boolean confirm(String prompt) {
        while (true) {
            String answer = text(prompt + " (s/n)", null).toLowerCase();
            if (answer.equals("s") || answer.equals("si") || answer.equals("sí")) {
                return true;
            }
            if (answer.equals("n") || answer.equals("no")) {
                return false;
            }
            System.out.println("  Responde s o n.");
        }
    }

    private String readLine() {
        if (!scanner.hasNextLine()) {
            // Fin de la entrada (Ctrl+D o tubería agotada): salir limpiamente
            System.out.println();
            throw new EndOfInputException();
        }
        return scanner.nextLine();
    }

    /** Se lanza cuando ya no hay más datos que leer. */
    @SuppressWarnings("serial")
    public static class EndOfInputException extends RuntimeException {
    }
}
