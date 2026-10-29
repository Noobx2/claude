using Generated;

var p = new Persona { Nombre = "Ana", Edad = 31 };
Console.WriteLine(p.Describir());                 // este método NO está escrito en ningún .cs de nuestro proyecto
Console.WriteLine(new Libro { Titulo = "C# a fondo", Paginas = 480, Autor = "Eva" }.Describir());

[Describir] public partial class Persona { public string Nombre { get; set; } = ""; public int Edad { get; set; } }
[Describir] public partial class Libro { public string Titulo { get; set; } = ""; public int Paginas { get; set; } public string Autor { get; set; } = ""; }
