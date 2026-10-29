// Lección 7: arrays y colecciones
// ---- Array: tamaño fijo ----
int[] fixedSize = new int[3];                    // [0, 0, 0]
int[] numbers = [5, 3, 8, 1];                    // expresión de colección (C# 12)
numbers[0] = 50;
Console.WriteLine($"{numbers.Length} elementos; primero {numbers[0]}; último {numbers[^1]}; trozo {string.Join(",", numbers[1..3])}");
Array.Sort(numbers);
Console.WriteLine($"ordenado: {string.Join(",", numbers)}; índice del 8: {Array.IndexOf(numbers, 8)}");
int[,] grid = { { 1, 2, 3 }, { 4, 5, 6 } };      // array bidimensional (matriz)
Console.WriteLine($"grid[1,2] = {grid[1, 2]}; filas {grid.GetLength(0)}, columnas {grid.GetLength(1)}");
int[][] jagged = [[1], [2, 3], [4, 5, 6]];       // array de arrays (filas de distinto tamaño)
Console.WriteLine($"jagged[2][1] = {jagged[2][1]}");

// ---- List<T>: tamaño variable ----
var names = new List<string> { "Ana", "Luis" };
names.Add("Marta");
names.AddRange(["Pablo", "Sara"]);
names.Insert(1, "Iván");
names.Remove("Luis");
names.RemoveAt(0);
Console.WriteLine($"{string.Join(", ", names)} | Count = {names.Count} | ¿Marta? {names.Contains("Marta")} | índice de Pablo {names.IndexOf("Pablo")}");
names.Sort();
names.Reverse();
Console.WriteLine($"ordenada inversa: {string.Join(", ", names)}");

// ---- Dictionary<K,V>: clave -> valor ----
var ages = new Dictionary<string, int> { ["Ana"] = 31, ["Luis"] = 28 };
ages["Sara"] = 36;                               // añadir o reemplazar
ages.TryAdd("Ana", 99);                          // no hace nada: la clave ya existe
if (ages.TryGetValue("Pedro", out int pedroAge)) Console.WriteLine(pedroAge); else Console.WriteLine("Pedro no está");
foreach (var (key, value) in ages) Console.Write($"{key}={value}; ");
Console.WriteLine();

// ---- HashSet<T>: sin repetidos, búsqueda rápida ----
var set = new HashSet<int> { 1, 2, 3, 3, 3 };
Console.WriteLine($"set: {string.Join(",", set)} ({set.Count}); Add(2) devuelve {set.Add(2)}; Add(9) devuelve {set.Add(9)}");
var other = new HashSet<int> { 3, 9, 10 };
set.IntersectWith(other);
Console.WriteLine($"intersección: {string.Join(",", set)}");

// ---- Pila (LIFO) y cola (FIFO) ----
var stack = new Stack<string>();
stack.Push("a"); stack.Push("b"); stack.Push("c");
Console.WriteLine($"pila: Pop = {stack.Pop()}, Peek = {stack.Peek()}");
var queue = new Queue<string>();
queue.Enqueue("a"); queue.Enqueue("b"); queue.Enqueue("c");
Console.WriteLine($"cola: Dequeue = {queue.Dequeue()}, Peek = {queue.Peek()}");

// ---- Tuplas y listas de tuplas ----
var people = new List<(string Name, int Age)> { ("Ana", 31), ("Luis", 28), ("Marta", 41) };
foreach (var (n, a) in people.OrderBy(p => p.Age)) Console.Write($"{n}({a}) ");
Console.WriteLine();

// ---- Colecciones inmutables y de solo lectura ----
IReadOnlyList<string> readOnly = names.AsReadOnly();
Console.WriteLine($"solo lectura: {readOnly.Count} elementos; no tiene Add");

// ---- Qué usar ----
Console.WriteLine("""
    Array: tamaño fijo conocido.   List<T>: lo normal.   Dictionary<K,V>: buscar por clave.
    HashSet<T>: sin repetidos.     Queue<T>/Stack<T>: orden de proceso.
    """);

// ---- Recorrer con índice, y modificar mientras se recorre ----
var values = new List<int> { 1, 2, 3, 4, 5, 6 };
try
{
    foreach (var v in values) if (v % 2 == 0) values.Remove(v);   // MAL: modifica la lista mientras se recorre
}
catch (InvalidOperationException e)
{
    Console.WriteLine($"error esperado: {e.Message}");
}
values.RemoveAll(v => v % 2 == 0);                                 // BIEN
Console.WriteLine(string.Join(",", values));
