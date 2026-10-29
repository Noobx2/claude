Imports System.Runtime.InteropServices

Module Program
    ' Visual Basic .NET: la misma P/Invoke, con la palabra "Declare" (o con <DllImport>, igual que en C#)
    Declare Auto Function MessageBox Lib "user32.dll" Alias "MessageBoxW" (hWnd As IntPtr, text As String, caption As String, type As UInteger) As Integer

    <DllImport("libc", EntryPoint:="getpid")>
    Private Function GetPid() As Integer
    End Function

    Sub Main()
        If OperatingSystem.IsWindows() Then
            MessageBox(IntPtr.Zero, "Hola desde VB.NET", "P/Invoke", 0UI)
        ElseIf OperatingSystem.IsLinux() Then
            Console.WriteLine($"getpid() desde VB.NET = {GetPid()}   (Environment.ProcessId = {Environment.ProcessId})")
        End If
    End Sub
End Module
