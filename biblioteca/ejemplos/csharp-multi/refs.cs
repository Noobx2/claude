// Lista los ensamblados (.dll) de los que depende cada proyecto compilado, sea cual sea su lenguaje.
using System.Reflection;

var root = args[0];
foreach (var (label, path) in new[]
{
    ("C#  (Inventory.Core)",   "src/Inventory.Core/bin/Debug/net10.0/Inventory.Core.dll"),
    ("F#  (Inventory.FSharp)", "src/Inventory.FSharp/bin/Debug/net10.0/Inventory.FSharp.dll"),
    ("VB  (Inventory.VB)",     "src/Inventory.VB/bin/Debug/net10.0/Inventory.VB.dll"),
})
{
    var assembly = Assembly.LoadFrom(Path.Combine(root, path));
    var names = assembly.GetReferencedAssemblies().Select(a => a.Name).Order().ToList();
    Console.WriteLine($"{label}: {string.Join(", ", names)}");
}
