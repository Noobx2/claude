// Lección 8: LINQ (consultas sobre colecciones)
var employees = new List<Employee>
{
    new(1, "Ana",   "ventas",     2400, 31, true),
    new(2, "Luis",  "tecnología", 3600, 28, true),
    new(3, "Marta", "tecnología", 4100, 41, false),
    new(4, "Pablo", "ventas",     2100, 24, true),
    new(5, "Sara",  "recursos",   2800, 36, true),
    new(6, "Iván",  "tecnología", 3300, 33, true),
};

// Where = filter, Select = map, Aggregate = reduce
Console.WriteLine("activos: " + string.Join(", ", employees.Where(e => e.Active).Select(e => e.Name)));
Console.WriteLine($"suma {employees.Sum(e => e.Salary)}; media {employees.Average(e => e.Salary):F2}; max {employees.Max(e => e.Salary)}; min {employees.Min(e => e.Salary)}");
Console.WriteLine("top 3: " + string.Join(", ", employees.OrderByDescending(e => e.Salary).Take(3).Select(e => e.Name)));
Console.WriteLine("por departamento y edad descendente: " + string.Join(", ", employees.OrderBy(e => e.Department).ThenByDescending(e => e.Age).Select(e => $"{e.Department[..3]}:{e.Name}")));

// Primeros, existencia
Console.WriteLine($"{employees.First(e => e.Age > 30).Name} | {employees.FirstOrDefault(e => e.Age > 60)?.Name ?? "nadie"} | Any {employees.Any(e => !e.Active)} | All {employees.All(e => e.Salary > 2000)} | Count {employees.Count(e => e.Department == "ventas")}");

// GroupBy
foreach (var group in employees.GroupBy(e => e.Department).OrderBy(g => g.Key))
    Console.WriteLine($"{group.Key,-11} n={group.Count()} total={group.Sum(e => e.Salary)} media={group.Average(e => e.Salary):F0}");

// ToDictionary, ToLookup, Distinct, Zip, SelectMany
var byId = employees.ToDictionary(e => e.Id, e => e.Name);
Console.WriteLine($"byId[3] = {byId[3]}");
var departments = employees.Select(e => e.Department).Distinct().Order().ToList();
Console.WriteLine(string.Join("/", departments));
var matrix = new[] { new[] { 1, 2 }, new[] { 3 }, new[] { 4, 5, 6 } };
Console.WriteLine("SelectMany: " + string.Join(",", matrix.SelectMany(row => row)));
Console.WriteLine("Zip: " + string.Join(", ", new[] { "a", "b", "c" }.Zip(new[] { 1, 2, 3 }, (letter, number) => $"{letter}{number}")));
Console.WriteLine("Chunk: " + string.Join(" | ", Enumerable.Range(1, 7).Chunk(3).Select(c => string.Join(",", c))));
Console.WriteLine("Aggregate: " + new[] { 1, 2, 3, 4 }.Aggregate((acc, x) => acc * x));

// JOIN
var departmentHeads = new[] { new Head("ventas", "Carla"), new Head("tecnología", "Diego") };
var joined = employees.Join(departmentHeads, e => e.Department, h => h.Department, (e, h) => $"{e.Name}->{h.Name}");
Console.WriteLine("join: " + string.Join(", ", joined));

// EJECUCIÓN DIFERIDA: la consulta no se ejecuta al escribirla, sino al recorrerla
var counter = 0;
var query = employees.Where(e => { counter++; return e.Active; });
Console.WriteLine($"tras definir la consulta: counter = {counter}");
var list = query.ToList();                         // aquí se ejecuta
Console.WriteLine($"tras ToList: counter = {counter}; resultado {list.Count}");
var again = query.Count();                         // y se vuelve a ejecutar entera
Console.WriteLine($"tras Count: counter = {counter}  (cada recorrido repite el trabajo; usa ToList si lo necesitas varias veces)");

// Sintaxis de consulta (alternativa; casi todo el mundo usa la de métodos)
var q = from e in employees where e.Salary > 3000 orderby e.Name select e.Name;
Console.WriteLine("sintaxis de consulta: " + string.Join(", ", q));

record Employee(int Id, string Name, string Department, int Salary, int Age, bool Active);
record Head(string Department, string Name);
