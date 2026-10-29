// Lección 24 (b): tablas bonitas con una biblioteca (Spectre.Console)
// En .NET 10, un archivo único puede declarar paquetes NuGet con la directiva #:package
#:package Spectre.Console@0.57.2
using Spectre.Console;

// Sin terminal real (salida redirigida) Spectre no conoce el ancho: lo fijamos a mano
if (Console.IsOutputRedirected) AnsiConsole.Profile.Width = 80;

var table = new Table().RoundedBorder().Title("Inventario");
table.AddColumn("Id");
table.AddColumn("Producto");
table.AddColumn(new TableColumn("Precio").RightAligned());
table.AddColumn(new TableColumn("Stock").RightAligned());
table.AddRow("1", "Teclado mecánico", "59,90", "25");
table.AddRow("2", "Ratón inalámbrico", "24,50", "60");
table.AddRow("[red]3[/]", "[red]SSD 1 TB[/]", "79,90", "[red]AGOTADO[/]");   // [red]...[/] = marcado de color
AnsiConsole.Write(table);

AnsiConsole.MarkupLine("[bold green]Listo.[/] Con la salida redirigida los colores se omiten solos.");

// Preguntas con validación incluida (en un terminal real son interactivas)
// var name = AnsiConsole.Prompt(new TextPrompt<string>("Nombre?").Validate(n => n.Length >= 2 ? ValidationResult.Success() : ValidationResult.Error("Mínimo 2 letras")));
// var option = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Elige").AddChoices("Crear", "Listar", "Salir"));
