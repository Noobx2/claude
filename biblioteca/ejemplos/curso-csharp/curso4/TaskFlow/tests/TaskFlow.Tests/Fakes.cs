using TaskFlow.Application;
using TaskFlow.Console;
using TaskFlow.Infrastructure;

namespace TaskFlow.Tests;

/// <summary>Reloj controlable: el "hoy" de las pruebas es siempre el mismo.</summary>
public sealed class FakeClock(DateTime now) : IClock
{
    public DateTime Now { get; set; } = now;
}

/// <summary>Consola falsa: la entrada está escrita de antemano y la salida se acumula para poder comprobarla.</summary>
public sealed class FakeConsole(params string[] input) : IConsoleIO
{
    private readonly Queue<string> _input = new(input);
    private readonly List<string> _lines = [];
    private string _pending = "";

    public string? ReadLine() => _input.Count > 0 ? _input.Dequeue() : null;
    public void Write(string text) => _pending += text;
    public void WriteLine(string text = "") { _lines.Add(_pending + text); _pending = ""; }

    public string Output => string.Join("\n", _lines) + _pending;
}

public static class TestFactory
{
    public static readonly DateTime Today = new(2026, 3, 10, 9, 0, 0);

    public static TaskService NewService(out FakeClock clock, ITaskRepository? repository = null)
    {
        clock = new FakeClock(Today);
        return new TaskService(repository ?? new InMemoryTaskRepository(), clock);
    }
}
