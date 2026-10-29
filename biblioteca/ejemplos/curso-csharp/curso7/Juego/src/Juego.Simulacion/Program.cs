using System.Collections;
using Juego.Core;

// ╔══════════════════════════════════════════════════════════════════════════════════════════╗
// ║  ESTO NO ES UNITY. Es una SIMULACIÓN escrita para este curso que reproduce el ORDEN que   ║
// ║  Unity documenta para los mensajes de MonoBehaviour. Sirve para entender el orden, no     ║
// ║  para comprobar el comportamiento real del motor (hay que verificarlo en el propio Unity). ║
// ╚══════════════════════════════════════════════════════════════════════════════════════════╝

var registro = new List<string>();
void Log(string s) => registro.Add(s);

var mundo = new Mundo();
var jugador = new Comportamiento("Jugador", Log);
var enemigo = new Comportamiento("Enemigo", Log) { ActivoAlCrear = false };      // se crea desactivado

Log("— Fotograma 0: se crean los objetos —");
mundo.Crear(jugador); mundo.Crear(enemigo);

for (int f = 1; f <= 3; f++)
{
    Log($"— Fotograma {f} —");
    if (f == 2) { Log("(el script activa al Enemigo)"); mundo.Activar(enemigo); }
    if (f == 3) { Log("(el script destruye al Jugador)"); mundo.Destruir(jugador); }
    mundo.Fotograma(deltaTime: 0.03f);                                           // 30 ms por fotograma
}

Console.WriteLine("Orden de los mensajes (simulación del orden documentado por Unity):\n");
foreach (var linea in registro) Console.WriteLine(linea);

Console.WriteLine("\n--- Corrutinas: un iterador que se avanza un paso por fotograma ---\n");
var runner = new CorrutinaRunner(); int fot = 0;
IEnumerator Parpadeo()
{
    Console.WriteLine($"  fotograma {fot}: empieza el parpadeo");
    for (int i = 1; i <= 2; i++)
    {
        yield return new EsperarSegundos(0.1f);                                  // como WaitForSeconds(0.1f)
        Console.WriteLine($"  fotograma {fot}: parpadea {i}");
    }
    yield return null;                                                           // esperar UN fotograma
    Console.WriteLine($"  fotograma {fot}: fin");
}
runner.Empezar(Parpadeo());
for (fot = 0; runner.EnCurso > 0 && fot < 20; fot++) runner.Tick(0.04f);          // 40 ms por fotograma
Console.WriteLine("  (no hay hilos: es una máquina de estados que el motor avanza en el hilo principal)");

// ------------------------------------------------------------------------------------------------
sealed class Comportamiento(string nombre, Action<string> log)
{
    public bool ActivoAlCrear { get; init; } = true;
    public bool Activo, YaTuvoStart, Destruido;
    public string Nombre => nombre;
    public void Awake() => log($"{nombre}.Awake");               // una vez, al crearse (incluso desactivado)
    public void OnEnable() => log($"{nombre}.OnEnable");        // cada vez que se activa
    public void Start() => log($"{nombre}.Start");               // una vez, justo antes del PRIMER Update
    public void FixedUpdate() => log($"{nombre}.FixedUpdate");   // a intervalos fijos (la física)
    public void Update() => log($"{nombre}.Update");             // una vez por fotograma
    public void LateUpdate() => log($"{nombre}.LateUpdate");     // después de todos los Update (cámaras)
    public void OnDisable() => log($"{nombre}.OnDisable");
    public void OnDestroy() => log($"{nombre}.OnDestroy");
}

sealed class Mundo
{
    private readonly List<Comportamiento> _todos = new();
    private float _acumulado; private const float PasoFijo = 0.02f;   // 50 pasos de física por segundo (valor por defecto documentado)

    public void Crear(Comportamiento c)
    {
        _todos.Add(c); c.Awake();                                      // Awake se llama aunque esté desactivado
        if (c.ActivoAlCrear) Activar(c);
    }
    public void Activar(Comportamiento c) { if (c.Activo) return; c.Activo = true; c.OnEnable(); }
    public void Destruir(Comportamiento c) { if (c.Activo) { c.Activo = false; c.OnDisable(); } c.OnDestroy(); c.Destruido = true; }

    public void Fotograma(float deltaTime)
    {
        var vivos = _todos.Where(c => c.Activo && !c.Destruido).ToList();
        foreach (var c in vivos.Where(c => !c.YaTuvoStart)) { c.YaTuvoStart = true; c.Start(); }   // Start antes del primer Update
        _acumulado += deltaTime;
        while (_acumulado >= PasoFijo) { _acumulado -= PasoFijo; foreach (var c in vivos) c.FixedUpdate(); }
        foreach (var c in vivos) c.Update();
        foreach (var c in vivos) c.LateUpdate();
    }
}
