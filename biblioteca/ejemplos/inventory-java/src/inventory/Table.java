package inventory;

import java.util.ArrayList;
import java.util.List;

/** Dibuja una tabla de texto con las columnas alineadas, calculando el ancho de cada una. */
public class Table {

    private final List<String> headers;
    private final List<Boolean> rightAligned;
    private final List<List<String>> rows = new ArrayList<>();

    public Table(List<String> headers, List<Boolean> rightAligned) {
        this.headers = headers;
        this.rightAligned = rightAligned;
    }

    public void addRow(List<String> cells) {
        rows.add(cells);
    }

    @Override
    public String toString() {
        int columns = headers.size();
        int[] widths = new int[columns];
        for (int c = 0; c < columns; c++) {
            widths[c] = headers.get(c).length();
            for (List<String> row : rows) {
                widths[c] = Math.max(widths[c], row.get(c).length());
            }
        }
        StringBuilder out = new StringBuilder();
        appendRow(out, headers, widths);
        for (int c = 0; c < columns; c++) {
            out.append(c == 0 ? "" : "-+-").append("-".repeat(widths[c]));
        }
        out.append(System.lineSeparator());
        for (List<String> row : rows) {
            appendRow(out, row, widths);
        }
        return out.toString();
    }

    private void appendRow(StringBuilder out, List<String> cells, int[] widths) {
        for (int c = 0; c < cells.size(); c++) {
            if (c > 0) {
                out.append(" | ");
            }
            // %-Ns alinea a la izquierda en N caracteres; %Ns a la derecha
            String format = rightAligned.get(c) ? "%" + widths[c] + "s" : "%-" + widths[c] + "s";
            out.append(String.format(format, cells.get(c)));
        }
        out.append(System.lineSeparator());
    }
}
