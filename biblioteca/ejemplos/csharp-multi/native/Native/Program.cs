using System.Runtime.InteropServices;

// P/Invoke (Platform Invoke): llamar a una función de una biblioteca NATIVA (no .NET) desde C#.
// Es la puerta a "la API de Windows" (Win32) y también a cualquier .so/.dylib/.dll.

Console.WriteLine($"Sistema: {RuntimeInformation.OSDescription}");

if (OperatingSystem.IsWindows())
{
    // Estas llamadas SOLO se ejecutan en Windows (user32.dll y kernel32.dll no existen en Linux)
    Console.WriteLine($"GetTickCount64 (kernel32) = {Win32.GetTickCount64()} ms");
    Console.WriteLine($"Environment.TickCount64   = {Environment.TickCount64} ms   <- la biblioteca base ya lo envuelve");
    Console.WriteLine($"Ancho de pantalla (user32 GetSystemMetrics) = {Win32.GetSystemMetrics(0)} px");
    Win32.MessageBox(IntPtr.Zero, "Hola desde la API de Windows", "P/Invoke", 0);
}
else if (OperatingSystem.IsLinux())
{
    // El MISMO mecanismo, apuntando a la biblioteca de C de Linux
    Console.WriteLine($"getpid() de libc (P/Invoke) = {Libc.getpid()}");
    Console.WriteLine($"Environment.ProcessId       = {Environment.ProcessId}   <- la biblioteca base ya lo envuelve");
    Console.WriteLine($"getpid() estilo clásico (DllImport) = {LibcClassic.getpid()}");
}

// Estilo moderno (.NET 7+): LibraryImport. Un generador de código escribe el "pegamento" al compilar (más rápido, compatible con AOT).
static partial class Win32
{
    [LibraryImport("kernel32.dll")]
    internal static partial ulong GetTickCount64();

    [LibraryImport("user32.dll")]
    internal static partial int GetSystemMetrics(int index);

    // StringMarshalling.Utf16: cómo se convierte el string de .NET al texto que espera Windows ("W" = wide, UTF-16)
    [LibraryImport("user32.dll", EntryPoint = "MessageBoxW", StringMarshalling = StringMarshalling.Utf16, SetLastError = true)]
    internal static partial int MessageBox(IntPtr hWnd, string text, string caption, uint type);
}

static partial class Libc
{
    [LibraryImport("libc", EntryPoint = "getpid")]
    internal static partial int getpid();
}

// Estilo clásico (desde .NET Framework 1.0): DllImport con "extern". El "pegamento" se genera al ejecutar, por reflexión.
static class LibcClassic
{
    [DllImport("libc", EntryPoint = "getpid")]
    internal static extern int getpid();
}
