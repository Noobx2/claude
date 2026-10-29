// Lección 23 (d): leer TECLAS sueltas (ReadKey): contraseñas ocultas, "pulsa una tecla", menús de una letra
// Console.ReadKey NO funciona con entrada redirigida (da error): solo con un teclado/terminal real.
if (Console.IsInputRedirected)
{
    Console.WriteLine("Esta lección necesita un teclado real (no una entrada redirigida).");
    return;
}

Console.Write("Contraseña: ");
var password = new System.Text.StringBuilder();
while (true)
{
    ConsoleKeyInfo key = Console.ReadKey(intercept: true);   // intercept: true = NO muestra la tecla en pantalla
    if (key.Key == ConsoleKey.Enter) break;
    if (key.Key == ConsoleKey.Backspace)
    {
        if (password.Length > 0)
        {
            password.Length--;                                // borrar el último carácter guardado
            Console.Write("\b \b");                           // y borrar el asterisco de la pantalla
        }
        continue;
    }
    if (!char.IsControl(key.KeyChar))                         // ignorar teclas especiales (flechas, F1...)
    {
        password.Append(key.KeyChar);
        Console.Write('*');                                   // mostramos un asterisco por cada carácter
    }
}
Console.WriteLine();
Console.WriteLine($"Longitud de la contraseña: {password.Length} (el contenido NO se muestra ni se guarda en el registro)");

Console.Write("¿Continuar? [s/n] ");
ConsoleKeyInfo answer = Console.ReadKey();                    // sin intercept: la tecla se ve
Console.WriteLine();
Console.WriteLine(char.ToLower(answer.KeyChar) == 's' ? "Continuamos." : "Cancelado.");
