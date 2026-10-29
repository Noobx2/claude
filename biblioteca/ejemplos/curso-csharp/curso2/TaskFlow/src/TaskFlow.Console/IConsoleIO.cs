namespace TaskFlow.Console;

/// <summary>
/// La consola como dependencia. En producción se usa la real; en las pruebas, una falsa con
/// la entrada escrita de antemano y la salida guardada en una lista.
/// </summary>
/// <summary>Se acabó la entrada (Ctrl+D, archivo terminado): hay que salir, no seguir preguntando.</summary>
public sealed class EndOfInputException : Exception;

public interface IConsoleIO
{
    string? ReadLine();
    void Write(string text);
    void WriteLine(string text = "");
}

public sealed class SystemConsoleIO : IConsoleIO
{
    public string? ReadLine()
    {
        string? line = System.Console.ReadLine();
        if (System.Console.IsInputRedirected && line is not null) System.Console.WriteLine(line);   // eco (solo con entrada simulada)
        return line;
    }
    public void Write(string text) => System.Console.Write(text);
    public void WriteLine(string text = "") => System.Console.WriteLine(text);
}
