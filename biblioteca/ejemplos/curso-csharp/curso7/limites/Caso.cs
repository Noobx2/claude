// Truco conocido en Unity: declarar tú el tipo que el compilador espera para 'init' y 'record'
namespace System.Runtime.CompilerServices { internal static class IsExternalInit { } }

public record Punto(int X, int Y);
public class P { public int X { get; init; } }
