#:property PublishAot=false

using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

// ======== 1) Introspección: mirar el interior de un tipo en tiempo de ejecución ========
Type tipo = typeof(Cuenta);
Console.WriteLine($"Tipo: {tipo.FullName}  | ensamblado: {tipo.Assembly.GetName().Name}  | sellado: {tipo.IsSealed}");
Console.WriteLine("Constructores : " + string.Join(" | ", tipo.GetConstructors().Select(c => $"({string.Join(", ", c.GetParameters().Select(p => p.ParameterType.Name + " " + p.Name))})")));
Console.WriteLine("Propiedades   : " + string.Join(", ", tipo.GetProperties().Select(p => $"{p.PropertyType.Name} {p.Name}")));
Console.WriteLine("Métodos       : " + string.Join(", ", tipo.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Select(m => m.Name)));
Console.WriteLine("Campos privados: " + string.Join(", ", tipo.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Select(f => f.Name)));
Console.WriteLine("Interfaces    : " + string.Join(", ", tipo.GetInterfaces().Select(i => i.Name)));

// ======== 2) Crear objetos, leer/escribir propiedades e invocar métodos SIN conocer el tipo en compilación ========
object cuenta = Activator.CreateInstance(tipo, "Ana", 100m)!;
tipo.GetProperty("Titular")!.SetValue(cuenta, "Ana López");
tipo.GetMethod("Ingresar")!.Invoke(cuenta, [50m]);
Console.WriteLine($"\nCreada y usada por reflexión: {cuenta}");
FieldInfo privado = tipo.GetField("_saldo", BindingFlags.NonPublic | BindingFlags.Instance)!;
Console.WriteLine($"Leído un campo PRIVADO: _saldo = {privado.GetValue(cuenta)}  (reflexión salta la visibilidad: úsalo con mucha cautela)");

// ======== 3) Atributos: metadatos que tú defines y lees después ========
Console.WriteLine("\n--- Validador basado en atributos (como hacen DataAnnotations) ---");
foreach (var solicitud in new[] { new Registro("ana", 25, "ana@correo.com"), new Registro("", 150, "no-es-correo") })
{
    var errores = Validador.Validar(solicitud);
    Console.WriteLine(errores.Count == 0 ? $"'{solicitud.Nombre}' es válido" : "Errores: " + string.Join("; ", errores));
}

// ======== 4) Descubrir "plugins" por atributo, a lo largo de todo el ensamblado ========
Console.WriteLine("\n--- Comandos descubiertos por atributo [Comando] ---");
var comandos = Assembly.GetExecutingAssembly().GetTypes()
    .Where(t => t.GetCustomAttribute<ComandoAttribute>() is not null && typeof(IComando).IsAssignableFrom(t))
    .Select(t => (Atributo: t.GetCustomAttribute<ComandoAttribute>()!, Tipo: t))
    .OrderBy(x => x.Atributo.Nombre);
foreach (var (atributo, t) in comandos)
{
    var instancia = (IComando)Activator.CreateInstance(t)!;
    Console.WriteLine($"  {atributo.Nombre,-8} {atributo.Descripcion,-24} → {instancia.Ejecutar()}");
}

// ======== 5) Genéricos en tiempo de ejecución ========
Type listaDe = typeof(List<>).MakeGenericType(typeof(decimal));
object lista = Activator.CreateInstance(listaDe)!;
listaDe.GetMethod("Add")!.Invoke(lista, [9.5m]);
Console.WriteLine($"\nMakeGenericType → {listaDe.Name}[{listaDe.GetGenericArguments()[0].Name}] con Count={listaDe.GetProperty("Count")!.GetValue(lista)}");

// ======== 6) El precio de la reflexión y cómo evitarlo ========
Console.WriteLine("\n--- Coste (1.000.000 de lecturas de una propiedad) ---");
var c1 = new Cuenta("Ana", 10m); PropertyInfo prop = typeof(Cuenta).GetProperty("Saldo")!;
Func<Cuenta, decimal> compilado = (Func<Cuenta, decimal>)Delegate.CreateDelegate(typeof(Func<Cuenta, decimal>), prop.GetGetMethod()!);   // un delegado tipado
var param = Expression.Parameter(typeof(Cuenta));
Func<Cuenta, decimal> viaExpresion = Expression.Lambda<Func<Cuenta, decimal>>(Expression.Property(param, "Saldo"), param).Compile();
Medir("Acceso directo c1.Saldo                  ", () => { decimal s = 0; for (int i = 0; i < 1_000_000; i++) s += c1.Saldo; return s; });
Medir("Reflexión: prop.GetValue(c1) cada vez    ", () => { decimal s = 0; for (int i = 0; i < 1_000_000; i++) s += (decimal)prop.GetValue(c1)!; return s; });
Medir("Delegado creado UNA vez (CreateDelegate) ", () => { decimal s = 0; for (int i = 0; i < 1_000_000; i++) s += compilado(c1); return s; });
Medir("Expression Tree compilado UNA vez        ", () => { decimal s = 0; for (int i = 0; i < 1_000_000; i++) s += viaExpresion(c1); return s; });

// ======== 7) Emitir IL: generar código en tiempo de ejecución ========
Console.WriteLine("\n--- Emisión de IL (System.Reflection.Emit) ---");
var metodo = new DynamicMethod("Potencia", typeof(long), [typeof(long), typeof(int)]);        // un método creado en memoria
ILGenerator il = metodo.GetILGenerator();
var inicio = il.DefineLabel(); var fin = il.DefineLabel();
LocalBuilder resultado = il.DeclareLocal(typeof(long));
il.Emit(OpCodes.Ldc_I8, 1L); il.Emit(OpCodes.Stloc, resultado);                              // resultado = 1
il.MarkLabel(inicio);
il.Emit(OpCodes.Ldarg_1); il.Emit(OpCodes.Ldc_I4_0); il.Emit(OpCodes.Ble, fin);              // si exp <= 0 → fin
il.Emit(OpCodes.Ldloc, resultado); il.Emit(OpCodes.Ldarg_0); il.Emit(OpCodes.Mul); il.Emit(OpCodes.Stloc, resultado);   // resultado *= base
il.Emit(OpCodes.Ldarg_1); il.Emit(OpCodes.Ldc_I4_1); il.Emit(OpCodes.Sub); il.Emit(OpCodes.Starg_S, (byte)1);          // exp--
il.Emit(OpCodes.Br, inicio);
il.MarkLabel(fin); il.Emit(OpCodes.Ldloc, resultado); il.Emit(OpCodes.Ret);
var potencia = (Func<long, int, long>)metodo.CreateDelegate(typeof(Func<long, int, long>));
Console.WriteLine($"Potencia(2, 40) generado en memoria = {potencia(2, 40):N0}");

// Un tipo entero creado en tiempo de ejecución
var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Dinamico"), AssemblyBuilderAccess.Run);
var tb = asm.DefineDynamicModule("m").DefineType("Saludador", TypeAttributes.Public);
var mb = tb.DefineMethod("Saludar", MethodAttributes.Public | MethodAttributes.Static, typeof(string), [typeof(string)]);
var g = mb.GetILGenerator();
g.Emit(OpCodes.Ldstr, "Hola, "); g.Emit(OpCodes.Ldarg_0); g.Emit(OpCodes.Call, typeof(string).GetMethod("Concat", [typeof(string), typeof(string)])!); g.Emit(OpCodes.Ret);
Type creado = tb.CreateType();
Console.WriteLine($"Tipo creado al vuelo: {creado.FullName}.Saludar(\"Eva\") = {creado.GetMethod("Saludar")!.Invoke(null, ["Eva"])}");

static void Medir<T>(string nombre, Func<T> f)
{
    f(); long t0 = Stopwatch.GetTimestamp(); f();
    Console.WriteLine($"{nombre} {Stopwatch.GetElapsedTime(t0).TotalMilliseconds,8:F2} ms");
}

// ------------------------------- tipos de apoyo -------------------------------
public sealed class Cuenta : IComparable<Cuenta>
{
    private decimal _saldo;
    public string Titular { get; set; }
    public decimal Saldo => _saldo;
    public Cuenta(string titular, decimal saldo) { Titular = titular; _saldo = saldo; }
    public void Ingresar(decimal cantidad) => _saldo += cantidad;
    public int CompareTo(Cuenta? otra) => _saldo.CompareTo(otra?._saldo);
    public override string ToString() => $"{Titular}: {_saldo} €";
}

[AttributeUsage(AttributeTargets.Property)]                                  // este atributo solo se puede poner en propiedades
public abstract class ReglaAttribute : Attribute { public abstract string? Comprobar(string campo, object? valor); }

public sealed class RequeridoAttribute : ReglaAttribute
{
    public override string? Comprobar(string campo, object? v) => v is null or "" ? $"{campo} es obligatorio" : null;
}
public sealed class RangoAttribute(int min, int max) : ReglaAttribute              // parámetros del atributo = argumentos del constructor
{
    public override string? Comprobar(string campo, object? v) => v is int n && (n < min || n > max) ? $"{campo} debe estar entre {min} y {max}" : null;
}
public sealed class CorreoAttribute : ReglaAttribute
{
    public override string? Comprobar(string campo, object? v) => v is string s && !s.Contains('@') ? $"{campo} no parece un correo" : null;
}

public record Registro(
    [property: Requerido] string Nombre,
    [property: Rango(18, 120)] int Edad,
    [property: Correo] string Email);

public static class Validador
{
    public static List<string> Validar(object objeto)
    {
        var errores = new List<string>();
        foreach (PropertyInfo p in objeto.GetType().GetProperties())
            foreach (ReglaAttribute regla in p.GetCustomAttributes<ReglaAttribute>())          // lee los atributos de cada propiedad
                if (regla.Comprobar(p.Name, p.GetValue(objeto)) is { } error) errores.Add(error);
        return errores;
    }
}

public interface IComando { string Ejecutar(); }
[AttributeUsage(AttributeTargets.Class)]
public sealed class ComandoAttribute(string nombre, string descripcion) : Attribute
{
    public string Nombre { get; } = nombre; public string Descripcion { get; } = descripcion;
}
[Comando("saludo", "Dice hola")] public class Saludo : IComando { public string Ejecutar() => "¡Hola!"; }
[Comando("fecha", "Muestra la fecha fija")] public class Fecha : IComando { public string Ejecutar() => "2026-10-29"; }
[Comando("version", "Versión del runtime")] public class Version : IComando { public string Ejecutar() => Environment.Version.ToString(); }
