#:property PublishAot=false

using System.Collections;
using System.Diagnostics;
using System.Text;

// ================= 1) Tipos por valor frente a tipos por referencia =================
Console.WriteLine("=== Valor vs referencia ===");
Punto a = new(1, 2); Punto b = a; b.X = 99;                       // struct: b es una COPIA de a
Console.WriteLine($"struct: a.X={a.X}, b.X={b.X}   → cada variable tiene su propia copia");
Persona p1 = new() { Nombre = "Ana" }; Persona p2 = p1; p2.Nombre = "Luis";   // class: p2 apunta al MISMO objeto
Console.WriteLine($"class : p1={p1.Nombre}, p2={p2.Nombre}   → las dos variables apuntan al mismo objeto");
Cambiar(a, p1);
Console.WriteLine($"tras Cambiar(): a.X={a.X} (el struct no cambió), p1.Nombre={p1.Nombre} (la clase sí)");
static void Cambiar(Punto punto, Persona persona) { punto.X = -1; persona.Nombre = "Cambiado"; }

// ================= 2) Las colecciones y para qué sirve cada una =================
Console.WriteLine("\n=== Colecciones ===");
var pila = new Stack<string>();                                   // LIFO: último en entrar, primero en salir
foreach (var paso in new[] { "abrir", "editar", "guardar" }) pila.Push(paso);
Console.Write("Stack (deshacer): "); while (pila.Count > 0) Console.Write(pila.Pop() + " "); Console.WriteLine();

var cola = new Queue<string>();                                   // FIFO: primero en entrar, primero en salir
foreach (var cliente in new[] { "Ana", "Luis", "Eva" }) cola.Enqueue(cliente);
Console.Write("Queue (turnos)  : "); while (cola.Count > 0) Console.Write(cola.Dequeue() + " "); Console.WriteLine();

var edades = new Dictionary<string, int> { ["Ana"] = 31, ["Luis"] = 27 };   // clave → valor, búsqueda ~O(1)
edades["Eva"] = 45;
Console.WriteLine($"Dictionary: Ana={edades["Ana"]}, ¿existe Pedro? {edades.TryGetValue("Pedro", out _)}");

var etiquetas = new HashSet<string> { "c#", "net", "c#", "web" };          // sin duplicados, pertenencia ~O(1)
Console.WriteLine($"HashSet: {etiquetas.Count} elementos (el duplicado se ignoró); contiene 'net': {etiquetas.Contains("net")}");

var ordenado = new SortedDictionary<string, int> { ["pera"] = 3, ["manzana"] = 1, ["kiwi"] = 2 };
Console.WriteLine("SortedDictionary recorre ordenado por clave: " + string.Join(", ", ordenado.Keys));

var lista = new LinkedList<int>([1, 2, 3]); lista.AddFirst(0);
Console.WriteLine("LinkedList (insertar al inicio sin desplazar): " + string.Join(",", lista));

var prioridad = new PriorityQueue<string, int>();                 // saca primero el de MENOR prioridad numérica
prioridad.Enqueue("baja", 5); prioridad.Enqueue("urgente", 1); prioridad.Enqueue("media", 3);
Console.Write("PriorityQueue: "); while (prioridad.TryDequeue(out var t, out _)) Console.Write(t + " "); Console.WriteLine();

// ================= 3) Elegir bien importa: mediciones =================
Console.WriteLine("\n=== Mediciones (Release; los tiempos cambian en cada máquina, lo que importa es la proporción) ===");
const int N = 20_000;
var lista2 = Enumerable.Range(0, N).ToList();
var conjunto = lista2.ToHashSet();
int[] busquedas = Enumerable.Range(0, 2000).Select(i => N - 1 - i).ToArray();   // elementos del final de la lista

Medir("Buscar 2000 valores en List<int> (Contains) ", () => { int c = 0; foreach (int x in busquedas) if (lista2.Contains(x)) c++; return c; });
Medir("Buscar 2000 valores en HashSet<int>         ", () => { int c = 0; foreach (int x in busquedas) if (conjunto.Contains(x)) c++; return c; });

// ---- boxing: convertir un tipo por valor en object copia el valor al montón (heap) ----
var arrayList = new ArrayList();                                                  // guarda 'object'
Medir("Sumar 1.000.000 int en ArrayList (boxing)   ", () => { arrayList.Clear(); for (int i = 0; i < 1_000_000; i++) arrayList.Add(i); long s = 0; foreach (int x in arrayList) s += x; return s; });
var generica = new List<int>();
Medir("Sumar 1.000.000 int en List<int> (sin box)  ", () => { generica.Clear(); for (int i = 0; i < 1_000_000; i++) generica.Add(i); long s = 0; foreach (int x in generica) s += x; return s; });

// Ojo: .NET 10 es capaz de ELIMINAR un boxing cuando el objeto no "escapa" del método (el JIT lo detecta).
// Para ver el coste real hay que hacer que la caja se guarde en algún sitio, como aquí en un arreglo.
object[] guardadas = new object[1000];
long antes = GC.GetAllocatedBytesForCurrentThread();
for (int i = 0; i < guardadas.Length; i++) guardadas[i] = i;                      // boxing: cada int se copia a una caja en el heap
long bytes = GC.GetAllocatedBytesForCurrentThread() - antes;
Console.WriteLine($"1000 boxings guardados reservaron {bytes:N0} bytes en el heap (~{bytes / 1000} bytes por int de 4 bytes)");

// ---- strings: son inmutables, concatenar en bucle crea un string nuevo cada vez ----
Medir("Concatenar 20.000 veces con +=              ", () => { string s = ""; for (int i = 0; i < 20_000; i++) s += "x"; return s.Length; });
Medir("Concatenar 20.000 veces con StringBuilder   ", () => { var sb = new StringBuilder(); for (int i = 0; i < 20_000; i++) sb.Append('x'); return sb.Length; });

// ---- excepciones: sirven para lo EXCEPCIONAL, no para el flujo normal ----
string[] textos = Enumerable.Range(0, 5_000).Select(i => i % 2 == 0 ? "123" : "abc").ToArray();
Medir("Validar 5000 textos con try/catch           ", () => { int ok = 0; foreach (var t in textos) { try { ok += int.Parse(t) > 0 ? 1 : 0; } catch (FormatException) { } } return ok; });
Medir("Validar 5000 textos con int.TryParse        ", () => { int ok = 0; foreach (var t in textos) if (int.TryParse(t, out int v) && v > 0) ok++; return ok; });

// ---- bucles: for sobre array/List frente a foreach sobre IEnumerable y LINQ ----
int[] datos = Enumerable.Range(1, 5_000_000).ToArray();
Medir("Suma con for sobre array                    ", () => { long s = 0; for (int i = 0; i < datos.Length; i++) s += datos[i]; return s; });
Medir("Suma con foreach sobre array                ", () => { long s = 0; foreach (int x in datos) s += x; return s; });
Medir("Suma con LINQ (Sum de long)                 ", () => datos.Sum(x => (long)x));
Medir("Suma con Span<int> (sin copiar)             ", () => { long s = 0; foreach (int x in datos.AsSpan()) s += x; return s; });

// Medir: calienta una vez (para que el JIT compile) y toma la mejor de 5 pasadas.
static void Medir<T>(string nombre, Func<T> accion)
{
    accion();                                   // calentamiento: el JIT compila y las cachés se llenan
    double mejor = double.MaxValue;
    for (int i = 0; i < 5; i++)
    {
        long t0 = Stopwatch.GetTimestamp();
        accion();
        mejor = Math.Min(mejor, Stopwatch.GetElapsedTime(t0).TotalMilliseconds);
    }
    Console.WriteLine($"{nombre} {mejor,9:F3} ms");
}

public struct Punto(int x, int y) { public int X { get; set; } = x; public int Y { get; set; } = y; }
public class Persona { public string Nombre { get; set; } = ""; }
