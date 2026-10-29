using System.Text;
using Inventory;

// Con UTF-8 se ven bien las tildes y el símbolo € en cualquier consola
Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

string file = args.Length > 0 ? args[0] : "products.json";
var app = new App(new ConsoleInput(Console.In, Console.Out), new ProductRepository(), file, Console.Out);
app.Run();
