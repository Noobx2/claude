#:property PublishAot=false

using System.Reflection;
using System.Reflection.Emit;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// 1) ¿Sobre qué runtime estoy corriendo?
Console.WriteLine($"Runtime ......: {RuntimeInformation.FrameworkDescription}");
Console.WriteLine($"SO / arquitec.: {RuntimeInformation.OSDescription} / {RuntimeInformation.ProcessArchitecture}");
Console.WriteLine($"Versión CLR ..: {Environment.Version}");
Console.WriteLine($"Ensamblado ...: {typeof(Calculadora).Assembly.GetName().Name} (ejecutable en memoria)");
Console.WriteLine($"Ensamblado de List<T>: {typeof(List<int>).Assembly.GetName().Name}");

// 2) El compilador NO genera código de máquina: genera IL (lenguaje intermedio).
//    Aquí lo leemos del propio programa y lo "desensamblamos" a mano.
Console.WriteLine("\n--- IL del método Calculadora.Sumar(int a, int b) ---");
MethodInfo sumar = typeof(Calculadora).GetMethod(nameof(Calculadora.Sumar))!;
byte[] il = sumar.GetMethodBody()!.GetILAsByteArray()!;
Console.WriteLine("Bytes: " + Convert.ToHexString(il));
Desensamblar(il);

Console.WriteLine("\n--- IL de un método con un bucle: Calculadora.SumaHasta(int n) ---");
Desensamblar(typeof(Calculadora).GetMethod(nameof(Calculadora.SumaHasta))!.GetMethodBody()!.GetILAsByteArray()!);

// 3) El JIT compila el IL a código de máquina la PRIMERA vez que se llama a un método.
Console.WriteLine("\n--- JIT: primera llamada frente a las siguientes ---");
long compilacionesAntes = JitInfo.GetCompiledMethodCount();
var t0 = System.Diagnostics.Stopwatch.GetTimestamp();
Calculadora.SumaHasta(10);                                     // aquí el JIT compila SumaHasta
double primera = System.Diagnostics.Stopwatch.GetElapsedTime(t0).TotalMicroseconds;
t0 = System.Diagnostics.Stopwatch.GetTimestamp();
Calculadora.SumaHasta(10);                                     // ya está compilado: solo se ejecuta
double segunda = System.Diagnostics.Stopwatch.GetElapsedTime(t0).TotalMicroseconds;
Console.WriteLine($"Métodos compilados por el JIT en este tramo: {JitInfo.GetCompiledMethodCount() - compilacionesAntes}");
Console.WriteLine($"Primera llamada (compila + ejecuta): {(primera > segunda ? "más lenta" : "similar")}");
Console.WriteLine($"Segunda llamada es {primera / Math.Max(segunda, 0.001):F0}x más rápida que la primera (los valores exactos cambian en cada máquina)");

// 4) Compilación por niveles (tiered compilation): primero rápido y sin optimizar, luego optimizado si el método es "caliente".
bool tieredActiva = !(AppContext.TryGetSwitch("System.Runtime.TieredCompilation", out bool valor) && !valor);
Console.WriteLine($"\nTiered compilation activa: {(tieredActiva ? "sí" : "no")}");

// 5) Código administrado (managed) frente a no administrado (unmanaged).
Console.WriteLine("\n--- Managed vs unmanaged ---");
int[] administrado = new int[1000];                             // lo gestiona el recolector de basura (GC)
Console.WriteLine($"Array administrado: generación del GC = {GC.GetGeneration(administrado)}");
IntPtr bloque = Marshal.AllocHGlobal(1024);                     // memoria NO administrada: el GC no la ve, hay que liberarla
Console.WriteLine($"Bloque no administrado en una dirección de memoria: {(bloque != IntPtr.Zero ? "reservado" : "falló")}");
Marshal.FreeHGlobal(bloque);                                    // si olvidas esta línea hay una fuga de memoria
Console.WriteLine($"PID desde una función C nativa (P/Invoke a libc): {NativeLibc.getpid() == Environment.ProcessId}");
Console.WriteLine($"Modo del GC: {(GCSettings.IsServerGC ? "servidor" : "estación de trabajo")}, latencia: {GCSettings.LatencyMode}");

// --- desensamblador mínimo: traduce bytes de IL a nombres de instrucciones ---
static void Desensamblar(byte[] il)
{
    // Tabla opcode → nombre, obtenida por reflexión de System.Reflection.Emit.OpCodes
    // Las instrucciones de 1 byte se identifican por ese byte; las de 2 bytes empiezan por 0xFE.
    var tabla = typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static)
        .Select(f => (OpCode)f.GetValue(null)!)
        .ToDictionary(o => (ushort)o.Value, o => o);
    for (int i = 0; i < il.Length;)
    {
        int inicio = i;
        OpCode op = il[i] == 0xFE ? tabla[(ushort)(il[i] << 8 | il[i + 1])] : tabla[il[i]];
        i += op.Size;
        string operando = "";
        switch (op.OperandType)
        {
            case OperandType.InlineNone: break;
            case OperandType.ShortInlineBrTarget: operando = $"IL_{i + 1 + (sbyte)il[i]:X4}"; i += 1; break;
            case OperandType.ShortInlineI: case OperandType.ShortInlineVar: operando = il[i].ToString(); i += 1; break;
            case OperandType.InlineI: operando = BitConverter.ToInt32(il, i).ToString(); i += 4; break;
            default: operando = "(…)"; i += 4; break;
        }
        Console.WriteLine($"  IL_{inicio:X4}: {op.Name,-10} {operando}");
    }
}

public static class Calculadora
{
    public static int Sumar(int a, int b) => a + b;

    public static int SumaHasta(int n)
    {
        int total = 0;
        for (int i = 1; i <= n; i++) total += i;
        return total;
    }
}

public static partial class NativeLibc
{
    [DllImport("libc")] public static extern int getpid();      // P/Invoke: llamar a código nativo del sistema
}
