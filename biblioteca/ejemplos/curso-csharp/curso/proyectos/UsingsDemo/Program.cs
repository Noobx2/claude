using Totally.Different.Name;              // trae Thing
using UsingsDemo.Models;                   // trae Product (el record)
using UsingsDemo.Services;                 // trae PriceService
using static System.Console;               // using static: usar WriteLine sin escribir "Console."
using Legacy = UsingsDemo.Legacy.Product;  // alias: un nombre corto para un tipo (y evita el choque de nombres)
using Dict = System.Collections.Generic.Dictionary<string, int>;   // alias de un tipo genérico completo

// Console, List, Math, StringBuilder... no necesitan using aquí:
//   - System, System.Linq, System.IO, System.Collections.Generic... vienen de ImplicitUsings (ver obj/.../GlobalUsings.g.cs)
//   - System.Text viene de GlobalUsings.cs
//   - System.Globalization viene de <Using Include="..."/> en el .csproj

var product = new Product("Teclado", 59.90m);                  // Models.Product (el record)
var old = new Legacy();                                        // UsingsDemo.Legacy.Product, gracias al alias
WriteLine($"{product} | {old.Name}");
WriteLine(new PriceService().WithVat(product));
WriteLine(new Thing());

var sb = new StringBuilder("global using");                    // sin "using System.Text;" en este archivo
WriteLine(sb);
WriteLine(CultureInfo.InvariantCulture.Name == "" ? "InvariantCulture (de <Using> en el csproj)" : "otra");
WriteLine(new Dict { ["a"] = 1 }.Count);

// Nombre completo: siempre se puede escribir sin using
WriteLine(System.IO.Path.GetExtension("archivo.cs"));
WriteLine(typeof(Product).Namespace + " | " + typeof(Legacy).Namespace + " | " + typeof(Thing).Namespace);
