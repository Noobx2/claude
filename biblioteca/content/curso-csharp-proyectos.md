# Curso de C# · Parte XI: tres proyectos integradores

Leer no es aprender: **aprender es construir y que no te salga a la primera**. Estos tres proyectos juntan lo estudiado. Cada uno trabaja una habilidad distinta:

| Proyecto | Qué entrena | Cómo se entrega |
|---|---|---|
| **1. TaskFlow, de punta a punta** | Entender y **verificar un sistema completo** (consola, API, base de datos, seguridad, interfaz web) | Ya construido en las Partes II–VIII: aquí se **verifica con un solo comando** |
| **2. Reservas** | **Construir desde un enunciado, con pruebas primero (TDD)** | Un **esqueleto en rojo** (27 pruebas fallando) que **tú** pones en verde. La solución de referencia está aparte |
| **3. Combate por turnos** | **Diseñar lógica de juego** portable a Unity, con azar inyectado | Código y pruebas **ejecutados**; luego lo extiendes tú |

*Verificado: 2026-10-29 con .NET SDK 10.0.113 en Linux. Todo lo de esta parte se ejecutó. Lo que **no** se pudo comprobar aquí (Unity, WPF en Windows, Docker) se indica donde aparece.*

---

## Proyecto 1 · TaskFlow, de punta a punta

### Qué trabaja este proyecto

No es escribir código nuevo, sino lo que hace un profesional todos los días: **comprobar que un sistema con muchas piezas funciona junto**. TaskFlow tiene, a estas alturas:

```text
TaskFlow.Console  ┐
TaskFlow.Api      ├──► TaskFlow.Core  ◄── (las reglas del negocio; no conoce a nadie)
TaskFlow.Web  ────┘         ▲
 (Blazor, en el navegador)  │
TaskFlow.Infrastructure ────┘   (JSON, memoria, EF Core + PostgreSQL, hash de contraseñas)
```

| Pieza | Parte |
|---|---|
| `Core`: dominio y casos de uso, **22 pruebas** | II y III |
| `Console`: la aplicación de consola | II |
| `Infrastructure`: persistencia (memoria, JSON, PostgreSQL) y seguridad | II, VII |
| `Api`: Minimal API, JWT, validación, errores estándar | VII |
| `Api.Tests`: integración con PostgreSQL real, arquitectura, **18 pruebas** | VII |
| `Web`: Blazor WebAssembly con login y CRUD | VIII |
| `e2e/e2e.js`: el recorrido de **17 pasos** en Chromium real | VIII |
| `Dockerfile`, `docker-compose.yml`, `ci.yml` | VII (**escritos, no ejecutados**) |

### El script: todo, en un comando

`verificar-todo.sh` hace **seis pasos** y **se detiene en el primero que falle**:

```text
#!/usr/bin/env bash
# Verifica TaskFlow de punta a punta, en un solo comando:
#   compilar → pruebas unitarias e integración → migraciones → API → interfaz Blazor en un navegador real.
# Requisitos: .NET 10 SDK, PostgreSQL en localhost:5432 con el rol «taskflow», Node + Playwright (solo para el último paso).
set -uo pipefail
cd "$(dirname "$0")"

PUERTOS=(5099 5200)
# Apaga lo que escucha en los puertos que usó ESTE script. («dotnet run» lanza un proceso hijo: matar solo al padre dejaría al hijo vivo.)
limpiar() { for puerto in "${PUERTOS[@]}"; do for pid in $(ss -ltnpH "sport = :$puerto" 2>/dev/null | grep -oP 'pid=\K[0-9]+' | sort -u); do kill "$pid" 2>/dev/null; done; done; }
trap limpiar EXIT

paso() { printf '\n== %s ==\n' "$1"; }
falla() { printf '\nFALLO en el paso: %s\n' "$1"; exit 1; }
esperar() { for _ in $(seq 1 40); do curl -s -o /dev/null "$1" && return 0; sleep 0.5; done; return 1; }   # espera a que un servidor responda

export PGPASSWORD=taskflow_dev
PSQL=(psql -h localhost -U taskflow -d taskflow -qAt)

paso "1/6 Compilar toda la solución (Release)"
dotnet build -c Release --nologo -v:q 2>&1 | grep -E "error|Warn|Error" | head -3
dotnet build -c Release --nologo -v:q >/dev/null 2>&1 || falla "compilación"

paso "2/6 Pruebas unitarias y de integración (PostgreSQL real)"
dotnet test -c Release --no-build --nologo 2>&1 | grep -E "Passed!|Failed!" | sed -E 's/, Duration.*//'
dotnet test -c Release --no-build --nologo >/dev/null 2>&1 || falla "pruebas"

paso "3/6 Migraciones: la base se deja al día"
dotnet ef database update --project src/TaskFlow.Infrastructure --startup-project src/TaskFlow.Api --no-build 2>&1 | grep -E "Done|No migrations|already" | head -1 || true
"${PSQL[@]}" -c "select count(*) || ' migraciones aplicadas' from \"__EFMigrationsHistory\"" || falla "migraciones"
"${PSQL[@]}" -c "truncate tasks, users restart identity"          # base limpia para el recorrido

paso "4/6 Arrancar la API (entorno Development) y comprobar su estado"
# OJO: con «dotnet App.dll» la raíz de contenido es la carpeta ACTUAL (no la del programa) y ahí no estaría appsettings.Development.json.
# Por eso se entra primero en la carpeta de la aplicación. («dotnet run» no tiene este problema: usa la carpeta del proyecto.)
(cd src/TaskFlow.Api/bin/Release/net10.0 && ASPNETCORE_ENVIRONMENT=Development exec dotnet TaskFlow.Api.dll --urls http://localhost:5099 >/tmp/tf-api.log 2>&1) &
esperar http://localhost:5099/health || falla "la API no arrancó (mira /tmp/tf-api.log)"
printf 'GET /health → %s\n' "$(curl -s http://localhost:5099/health)"
printf 'GET /tasks sin token → HTTP %s\n' "$(curl -s -o /dev/null -w '%{http_code}' http://localhost:5099/tasks)"

paso "5/6 Arrancar la interfaz Blazor"
(cd src/TaskFlow.Web && dotnet run -c Release --urls http://localhost:5200 >/tmp/tf-web.log 2>&1) &
esperar http://localhost:5200/ || falla "la interfaz no arrancó (mira /tmp/tf-web.log)"
printf 'GET / → HTTP %s\n' "$(curl -s -o /dev/null -w '%{http_code}' http://localhost:5200/)"

paso "6/6 Recorrido de extremo a extremo en Chromium (Playwright)"
NODE_PATH="$(npm root -g)" node e2e/e2e.js || falla "recorrido E2E"

printf '\nTODO CORRECTO\n'
```

**Cómo se lee:**

1. **`set -uo pipefail`**: si usas una variable sin definir o una tubería falla, el script **no sigue ciegamente**.
2. **`trap limpiar EXIT`**: pase lo que pase (éxito, fallo o Ctrl+C), **apaga los servidores que arrancó**. Se localizan **por el puerto que escuchan** (`ss`), porque `dotnet run` lanza un proceso hijo y matar solo al padre dejaría al hijo vivo (me ocurrió en el primer intento).
3. **`esperar`**: en lugar de un `sleep 10` a ciegas, **consulta hasta que el servidor responde** (con límite de tiempo).
4. **Paso 3** deja la base de datos **al día con las migraciones** y la **vacía** para que el recorrido sea reproducible.
5. **Paso 4:** la API se lanza **desde su carpeta** (`cd …/net10.0 && exec dotnet TaskFlow.Api.dll`). **No es un detalle:** con `dotnet App.dll` la raíz de contenido es la **carpeta actual**; lanzada desde otra, **no encuentra `appsettings.Development.json`** y la API rechazaba al navegador por CORS. Lo encontró este mismo script.
6. **Paso 6** ejecuta el recorrido del navegador.

### Resultado real

```text

== 1/6 Compilar toda la solución (Release) ==
    0 Warning(s)
    0 Error(s)

== 2/6 Pruebas unitarias y de integración (PostgreSQL real) ==
Passed!  - Failed:     0, Passed:    22, Skipped:     0, Total:    22
Passed!  - Failed:     0, Passed:    18, Skipped:     0, Total:    18

== 3/6 Migraciones: la base se deja al día ==
      No migrations were applied. The database is already up to date.
2 migraciones aplicadas

== 4/6 Arrancar la API (entorno Development) y comprobar su estado ==
GET /health → Healthy
GET /tasks sin token → HTTP 401

== 5/6 Arrancar la interfaz Blazor ==
GET / → HTTP 200

== 6/6 Recorrido de extremo a extremo en Chromium (Playwright) ==
 1. Sin sesión, «/» redirige a /login
 2. Formulario vacío → mensajes de validación («Escribe tu usuario»)
 3. Contraseña de 3 caracteres → «Mínimo 8 caracteres» (validado en el cliente)
 4. Registro + entrada → «Mis tareas», usuario «ana Admin» (primer usuario = Admin)
 5. Título de 2 letras → mensaje de validación en el formulario
 6. Fecha pasada → la API rechaza y la página muestra: «La fecha límite no puede estar en el pasado.»
 7. Tres tareas creadas → tabla con 3 filas
 8. Completar una tarea → estadísticas: Total 3 Pendientes 2 Hechas 1 Vencidas 0
 9. Filtro «Hechas» → 1 fila
10. Búsqueda «jwt» (sin distinguir mayúsculas) → solo «Revisar JWT»
11. Recarga (F5) → sigue la sesión y los datos
12. Eliminar como Admin → 2 filas
13. Usuario «luis» (rol User): la interfaz NO muestra «Eliminar»
14. Saltándose la interfaz: DELETE directo a la API con el token de «luis» → 403 (el servidor es quien protege)
15. Contraseña incorrecta → «Credenciales incorrectas o sesión caducada.»
16. Ancho de móvil (390 px): sin desplazamiento horizontal
17. Consola del navegador: 3 respuestas 400/401/403 (las provocadas a propósito en los pasos 6, 14 y 15) y 0 errores inesperados

RESULTADO: todos los pasos pasaron

TODO CORRECTO

real	0m47.147s
user	0m34.616s
sys	0m5.422s
procesos que quedan escuchando: 0
```

**Qué demuestra y qué no:**

- Demuestra que **las piezas funcionan juntas en esta máquina**: 22 + 18 pruebas, base migrada, API con autenticación, interfaz que consume la API y 17 pasos de usuario.
- **No demuestra:** que funcione en **Windows**, con **otros navegadores**, bajo **carga**, con **Docker**, ni que sea **segura frente a un atacante** (se probaron los casos que se pensaron, no un análisis de seguridad).

### Para practicar (sin solución dada)

> **P1.1:** añade un campo `Etiquetas` a las tareas, de extremo a extremo: dominio, migración, API, Blazor y una prueba nueva en cada nivel. Si te saltas uno, el script debe **detectarlo**.
>
> **P1.2:** haz que `GET /tasks` filtre **en la base de datos** (lección 47, «limitación honesta») en lugar de en memoria. Comprueba con el SQL que EF genera que aparece el `WHERE`.
>
> **P1.3:** si tienes Docker: construye la imagen, arranca `docker compose up` y **anota qué falló la primera vez**. Ese texto no pudo verificarse aquí.
>
> **P1.4:** añade al script un paso que arranque la API con una variable `Jwt__Key` de menos de 32 caracteres y **compruebe que no arranca**.

---

## Proyecto 2 · Reservas: construir desde un enunciado, con pruebas primero

### Qué trabaja este proyecto

El 90 % del trabajo real es **recibir un enunciado ambiguo y convertirlo en reglas exactas**. Aquí te doy el enunciado, **las pruebas ya escritas** (que **son** la especificación exacta) y un esqueleto. **Tu trabajo:** poner las 27 pruebas en verde, de una en una.

### El enunciado

> Un hotel pequeño necesita un servicio de reservas. Cada **habitación** tiene número, capacidad y precio por noche. Un **huésped** reserva una habitación para unas fechas.
>
> **Reservar** (`Reservar(habitacionId, huesped, entrada, salida, personas)`):
> 1. El huésped no puede estar vacío (los espacios no cuentan).
> 2. La **salida** debe ser **posterior** a la entrada.
> 3. La entrada **no puede estar en el pasado** (entrar **hoy** sí vale).
> 4. Como máximo **30 noches**.
> 5. La habitación debe **existir** (si no, «no encontrado»).
> 6. Las personas deben estar **entre 1 y la capacidad** de la habitación.
> 7. La habitación **no puede estar ocupada** en esas fechas por una reserva **activa**. **Salir el mismo día que otro entra es válido** (las estancias son «de la noche de entrada a la mañana de salida»).
> 8. **Precio** = noches × precio por noche; **10 % de descuento desde 7 noches**. Redondeado a 2 decimales.
> 9. El nombre del huésped se guarda **sin espacios sobrantes**.
>
> **Cancelar** (`Cancelar(reservaId)`):
> 1. La reserva debe existir, y **no estar ya cancelada**.
> 2. **Reembolso:** con **2 o más días** de antelación hasta la entrada: **100 %**; el día antes o el mismo día: **50 %**; si la estancia ya empezó: **0 %**.
> 3. Una reserva cancelada **libera las fechas**.
>
> **Disponibilidad** (`Disponibles(entrada, salida, personas)`): las habitaciones **libres** con capacidad suficiente, **ordenadas por precio** (y por número en caso de empate).

### El diseño que se te da hecho

```csharp
namespace Reservas.Core;

public sealed record Habitacion(int Id, string Numero, int Capacidad, decimal PrecioNoche);

public enum EstadoReserva { Activa, Cancelada }

public sealed record Reserva(
    int Id, int HabitacionId, string Huesped,
    DateOnly Entrada, DateOnly Salida, int Personas,
    decimal Total, EstadoReserva Estado = EstadoReserva.Activa, decimal Reembolso = 0m)
{
    public int Noches => Salida.DayNumber - Entrada.DayNumber;
}

/// <summary>Se incumple una regla del negocio: el mensaje se puede enseñar al usuario.</summary>
public sealed class ReglaDeNegocioException(string mensaje) : Exception(mensaje);
public sealed class NoEncontradoException(string mensaje) : Exception(mensaje);

// ---- Los puertos: lo que el servicio necesita del exterior ----
public interface ICatalogoHabitaciones
{
    Habitacion? Buscar(int id);
    IReadOnlyList<Habitacion> Todas();
}

public interface IRepositorioReservas
{
    IReadOnlyList<Reserva> ListarPorHabitacion(int habitacionId);
    Reserva? Buscar(int id);
    Reserva Guardar(Reserva reserva);       // asigna el Id
    void Actualizar(Reserva reserva);
}

public interface IReloj { DateOnly Hoy { get; } }
```

**Fíjate** en tres decisiones que repiten lo aprendido: el **reloj es una dependencia** (`IReloj`: sin él no se puede probar «no entrar en el pasado»), los **datos entran por interfaces** (`ICatalogoHabitaciones`, `IRepositorioReservas`) y los errores de negocio son **excepciones propias**.

### Tu punto de partida

```csharp
namespace Reservas.Core;

/// <summary>
/// TU TRABAJO: implementar estos tres métodos hasta que TODAS las pruebas pasen.
/// Reglas completas en el enunciado del proyecto. Consejo: ve prueba a prueba, de arriba abajo,
/// y escribe SOLO lo mínimo para que pase la siguiente (TDD).
/// </summary>
public sealed class ServicioReservas(ICatalogoHabitaciones habitaciones, IRepositorioReservas reservas, IReloj reloj)
{
    public const int MaximoNoches = 30;
    public const int NochesParaDescuento = 7;
    public const decimal Descuento = 0.10m;

    public Reserva Reservar(int habitacionId, string huesped, DateOnly entrada, DateOnly salida, int personas)
        => throw new NotImplementedException();

    public Reserva Cancelar(int reservaId)
        => throw new NotImplementedException();

    public IReadOnlyList<Habitacion> Disponibles(DateOnly entrada, DateOnly salida, int personas)
        => throw new NotImplementedException();
}
```

### Las pruebas: la especificación ejecutable

`Dobles.cs`: un reloj fijo, un catálogo y un repositorio en memoria

```csharp
using Reservas.Core;

namespace Reservas.Tests;

/// <summary>Dobles de prueba: un reloj fijo, un catálogo y un repositorio en memoria.</summary>
public sealed class RelojFijo(DateOnly hoy) : IReloj { public DateOnly Hoy { get; set; } = hoy; }

public sealed class CatalogoFalso(params Habitacion[] datos) : ICatalogoHabitaciones
{
    public Habitacion? Buscar(int id) => datos.FirstOrDefault(h => h.Id == id);
    public IReadOnlyList<Habitacion> Todas() => datos;
}

public sealed class RepositorioEnMemoria : IRepositorioReservas
{
    private readonly List<Reserva> _todas = [];
    private int _siguiente = 1;
    public IReadOnlyList<Reserva> ListarPorHabitacion(int id) => _todas.Where(r => r.HabitacionId == id).ToList();
    public Reserva? Buscar(int id) => _todas.FirstOrDefault(r => r.Id == id);
    public Reserva Guardar(Reserva r) { var guardada = r with { Id = _siguiente++ }; _todas.Add(guardada); return guardada; }
    public void Actualizar(Reserva r) => _todas[_todas.FindIndex(x => x.Id == r.Id)] = r;
}
```

`ReservasTests.cs`

```csharp
using Reservas.Core;

namespace Reservas.Tests;

public class ReservasTests
{
    private static readonly DateOnly Hoy = new(2026, 11, 1);
    private readonly RelojFijo _reloj = new(Hoy);
    private readonly RepositorioEnMemoria _repo = new();
    private readonly ServicioReservas _servicio;

    private static DateOnly Dia(int desdeHoy) => Hoy.AddDays(desdeHoy);

    public ReservasTests()
    {
        var catalogo = new CatalogoFalso(
            new Habitacion(1, "101", Capacidad: 2, PrecioNoche: 100m),
            new Habitacion(2, "102", Capacidad: 4, PrecioNoche: 150m),
            new Habitacion(3, "201", Capacidad: 2, PrecioNoche: 80m));
        _servicio = new ServicioReservas(catalogo, _repo, _reloj);
    }

    // ---------------- Reservar: validaciones ----------------
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Reservar_SinHuesped_Falla(string huesped) =>
        Assert.Throws<ReglaDeNegocioException>(() => _servicio.Reservar(1, huesped, Dia(1), Dia(2), 1));

    [Fact]
    public void Reservar_SalidaNoPosteriorAEntrada_Falla()
    {
        Assert.Throws<ReglaDeNegocioException>(() => _servicio.Reservar(1, "Ana", Dia(3), Dia(3), 1));
        Assert.Throws<ReglaDeNegocioException>(() => _servicio.Reservar(1, "Ana", Dia(3), Dia(2), 1));
    }

    [Fact]
    public void Reservar_EntradaEnElPasado_Falla() =>
        Assert.Throws<ReglaDeNegocioException>(() => _servicio.Reservar(1, "Ana", Dia(-1), Dia(2), 1));

    [Fact]
    public void Reservar_EntrandoHoy_Funciona() =>
        Assert.Equal(Dia(0), _servicio.Reservar(1, "Ana", Dia(0), Dia(1), 1).Entrada);

    [Fact]
    public void Reservar_MasDeTreintaNoches_Falla()
    {
        Assert.Throws<ReglaDeNegocioException>(() => _servicio.Reservar(1, "Ana", Dia(1), Dia(32), 1));   // 31 noches
        Assert.Equal(30, _servicio.Reservar(1, "Ana", Dia(1), Dia(31), 1).Noches);                          // 30 noches: límite permitido
    }

    [Fact]
    public void Reservar_HabitacionInexistente_LanzaNoEncontrado() =>
        Assert.Throws<NoEncontradoException>(() => _servicio.Reservar(99, "Ana", Dia(1), Dia(2), 1));

    [Theory]
    [InlineData(0)]
    [InlineData(3)]       // la habitación 1 admite 2
    public void Reservar_PersonasFueraDeCapacidad_Falla(int personas) =>
        Assert.Throws<ReglaDeNegocioException>(() => _servicio.Reservar(1, "Ana", Dia(1), Dia(2), personas));

    // ---------------- Reservar: solapamientos ----------------
    [Fact]
    public void Reservar_FechasQueSeSolapan_Falla()
    {
        _servicio.Reservar(1, "Ana", Dia(10), Dia(15), 2);
        Assert.Throws<ReglaDeNegocioException>(() => _servicio.Reservar(1, "Luis", Dia(12), Dia(14), 1));   // dentro
        Assert.Throws<ReglaDeNegocioException>(() => _servicio.Reservar(1, "Luis", Dia(8), Dia(11), 1));    // pisa el inicio
        Assert.Throws<ReglaDeNegocioException>(() => _servicio.Reservar(1, "Luis", Dia(14), Dia(18), 1));   // pisa el final
        Assert.Throws<ReglaDeNegocioException>(() => _servicio.Reservar(1, "Luis", Dia(8), Dia(20), 1));    // la envuelve
    }

    [Fact]
    public void Reservar_UnaEstanciaAcabaElDiaQueOtraEmpieza_NoEsSolapamiento()
    {
        _servicio.Reservar(1, "Ana", Dia(10), Dia(15), 2);
        _servicio.Reservar(1, "Luis", Dia(15), Dia(17), 1);      // entra el día que ella sale
        _servicio.Reservar(1, "Eva", Dia(8), Dia(10), 1);        // sale el día que ella entra
        Assert.Equal(3, _repo.ListarPorHabitacion(1).Count);
    }

    [Fact]
    public void Reservar_OtraHabitacionLasMismasFechas_Funciona()
    {
        _servicio.Reservar(1, "Ana", Dia(10), Dia(15), 2);
        Assert.NotNull(_servicio.Reservar(3, "Luis", Dia(10), Dia(15), 2));
    }

    // ---------------- Precio ----------------
    [Theory]
    [InlineData(1, 100.00)]
    [InlineData(6, 600.00)]      // 6 noches: sin descuento
    [InlineData(7, 630.00)]      // 7 noches: 10 % de descuento → 700 × 0,9
    [InlineData(10, 900.00)]
    public void Reservar_CalculaElTotalConDescuentoDesdeSieteNoches(int noches, double esperado) =>
        Assert.Equal((decimal)esperado, _servicio.Reservar(1, "Ana", Dia(1), Dia(1 + noches), 2).Total);

    [Fact]
    public void Reservar_RecortaElNombreDelHuesped() =>
        Assert.Equal("Ana López", _servicio.Reservar(1, "  Ana López ", Dia(1), Dia(2), 1).Huesped);

    // ---------------- Cancelar ----------------
    [Theory]
    [InlineData(10, 1.00)]     // mucha antelación: reembolso completo
    [InlineData(2, 1.00)]      // exactamente 2 días: completo
    [InlineData(1, 0.50)]      // el día anterior: la mitad
    [InlineData(0, 0.50)]      // el mismo día de la entrada: la mitad
    [InlineData(-1, 0.00)]     // ya empezó: nada
    public void Cancelar_ReembolsaSegunLaAntelacion(int diasHastaEntrada, double proporcion)
    {
        // La reserva se crea con el reloj en otra fecha, y luego se mueve el reloj hasta el día de la cancelación
        _reloj.Hoy = Hoy;
        var reserva = _servicio.Reservar(1, "Ana", Dia(10), Dia(12), 2);          // total 200
        _reloj.Hoy = reserva.Entrada.AddDays(-diasHastaEntrada);

        var cancelada = _servicio.Cancelar(reserva.Id);

        Assert.Equal(EstadoReserva.Cancelada, cancelada.Estado);
        Assert.Equal(200m * (decimal)proporcion, cancelada.Reembolso);
    }

    [Fact]
    public void Cancelar_DosVeces_Falla()
    {
        var reserva = _servicio.Reservar(1, "Ana", Dia(10), Dia(12), 2);
        _servicio.Cancelar(reserva.Id);
        Assert.Throws<ReglaDeNegocioException>(() => _servicio.Cancelar(reserva.Id));
    }

    [Fact]
    public void Cancelar_ReservaInexistente_LanzaNoEncontrado() =>
        Assert.Throws<NoEncontradoException>(() => _servicio.Cancelar(123));

    [Fact]
    public void UnaReservaCancelada_LiberaLasFechas()
    {
        var reserva = _servicio.Reservar(1, "Ana", Dia(10), Dia(15), 2);
        _servicio.Cancelar(reserva.Id);
        Assert.NotNull(_servicio.Reservar(1, "Luis", Dia(10), Dia(15), 2));       // ahora sí se puede
    }

    // ---------------- Disponibilidad ----------------
    [Fact]
    public void Disponibles_FiltraPorCapacidadYOcupacion_OrdenadasPorPrecio()
    {
        _servicio.Reservar(3, "Ana", Dia(10), Dia(12), 2);                          // la 201 (la más barata) queda ocupada

        var libres = _servicio.Disponibles(Dia(10), Dia(12), personas: 2);
        Assert.Equal(new[] { "101", "102" }, libres.Select(h => h.Numero));          // 100 € luego 150 €

        var paraCuatro = _servicio.Disponibles(Dia(10), Dia(12), personas: 4);
        Assert.Equal(new[] { "102" }, paraCuatro.Select(h => h.Numero));             // solo la de capacidad 4
    }

    [Fact]
    public void Disponibles_ConFechasInvalidas_Falla() =>
        Assert.Throws<ReglaDeNegocioException>(() => _servicio.Disponibles(Dia(5), Dia(5), 1));
}
```

### Cómo trabajar (el método)

1. **Ejecuta `dotnet test` en `Esqueleto/`.** Verás **27 fallos** (`NotImplementedException`). **Es lo esperado.**
2. **Elige la prueba más fácil** (la de «sin huésped»). Escribe **solo** lo mínimo para que pase. Ejecuta.
3. **Siguiente prueba.** Cuando una prueba nueva te obligue a cambiar lo que ya tenías, **refactoriza con las anteriores como red**.
4. **No toques las pruebas** para que pasen. Si crees que una está mal, **discútelo contigo mismo con el enunciado en la mano** (a mí me pasó con una prueba de otro proyecto: tenía el error la prueba, no el código; pero lo decide el enunciado).
5. Cuando estén todas verdes, **mira la solución** y compara. No hay una única solución correcta.

### Lo que se ve al ejecutar

```text
$ cd Esqueleto && dotnet test   (el punto de partida del alumno: TODO en rojo)
Failed!  - Failed:    27, Passed:     0, Skipped:     0, Total:    27

$ cd Solucion && dotnet test   (la solución de referencia)
Passed!  - Failed:     0, Passed:    27, Skipped:     0, Total:    27
```

**El esqueleto da 27 fallos y la solución 27 aciertos con el mismo conjunto de pruebas.** Eso demuestra algo que no es obvio: **las pruebas se pueden satisfacer** (no se contradicen) y **no pasan por casualidad** (con el esqueleto vacío, todas fallan).

### ¿Valen esas pruebas? Mutación manual

Se rompió la **solución** de seis maneras distintas, todas **errores de «un día de más o de menos»**, los más típicos del código con fechas:

```text
$ MUTACIÓN: solapamiento con <= en vez de < (saldría 'ocupado' al enlazar estancias)
Failed Reservas.Tests.ReservasTests.Reservar_UnaEstanciaAcabaElDiaQueOtraEmpieza_NoEsSolapamiento
Failed!  - Failed:     1, Passed:    26, Skipped:     0, Total:    27

$ MUTACIÓN: reembolso completo solo con MÁS de 2 días (en vez de 2 o más)
Failed Reservas.Tests.ReservasTests.Cancelar_ReembolsaSegunLaAntelacion(diasHastaEntrada: 2, proporcion: 1)
Failed!  - Failed:     1, Passed:    26, Skipped:     0, Total:    27

$ MUTACIÓN: descuento desde 8 noches en vez de desde 7
Failed Reservas.Tests.ReservasTests.Reservar_CalculaElTotalConDescuentoDesdeSieteNoches(noches: 7, esperado: 630)
Failed!  - Failed:     1, Passed:    26, Skipped:     0, Total:    27

$ MUTACIÓN: las reservas canceladas siguen bloqueando la habitación
Failed Reservas.Tests.ReservasTests.UnaReservaCancelada_LiberaLasFechas [< 1 ms]
Failed!  - Failed:     1, Passed:    26, Skipped:     0, Total:    27

$ MUTACIÓN: el máximo es 29 noches en vez de 30
Failed Reservas.Tests.ReservasTests.Reservar_MasDeTreintaNoches_Falla
Failed!  - Failed:     1, Passed:    26, Skipped:     0, Total:    27

$ MUTACIÓN: no se puede entrar hoy
Failed Reservas.Tests.ReservasTests.Reservar_EntrandoHoy_Funciona [< 1 ms]
Failed!  - Failed:     1, Passed:    26, Skipped:     0, Total:    27
```

**Las seis fueron detectadas por la prueba correspondiente.** Fíjate en cuáles: son los **casos frontera** (`< / <=`, `>= 2 / > 2`, 7 frente a 8 noches, 30 frente a 29). Por eso las pruebas tienen `[Theory]` con valores **justo en el borde** y justo a ambos lados. **Un error de «uno de más» casi nunca se ve con un caso «normal».**

### La solución de referencia (míralo solo cuando termines)

```csharp
namespace Reservas.Core;

public sealed class ServicioReservas(ICatalogoHabitaciones habitaciones, IRepositorioReservas reservas, IReloj reloj)
{
    public const int MaximoNoches = 30;
    public const int NochesParaDescuento = 7;
    public const decimal Descuento = 0.10m;

    public Reserva Reservar(int habitacionId, string huesped, DateOnly entrada, DateOnly salida, int personas)
    {
        if (string.IsNullOrWhiteSpace(huesped)) throw new ReglaDeNegocioException("El huésped es obligatorio.");
        if (salida <= entrada) throw new ReglaDeNegocioException("La salida debe ser posterior a la entrada.");
        if (entrada < reloj.Hoy) throw new ReglaDeNegocioException("La entrada no puede estar en el pasado.");
        int noches = salida.DayNumber - entrada.DayNumber;
        if (noches > MaximoNoches) throw new ReglaDeNegocioException($"La estancia máxima es de {MaximoNoches} noches.");

        var habitacion = habitaciones.Buscar(habitacionId) ?? throw new NoEncontradoException($"No existe la habitación {habitacionId}.");
        if (personas < 1 || personas > habitacion.Capacidad)
            throw new ReglaDeNegocioException($"La habitación {habitacion.Numero} admite de 1 a {habitacion.Capacidad} personas.");
        if (EstaOcupada(habitacion.Id, entrada, salida))
            throw new ReglaDeNegocioException($"La habitación {habitacion.Numero} no está libre en esas fechas.");

        decimal total = CalcularTotal(habitacion.PrecioNoche, noches);
        return reservas.Guardar(new Reserva(0, habitacion.Id, huesped.Trim(), entrada, salida, personas, total));
    }

    public Reserva Cancelar(int reservaId)
    {
        var reserva = reservas.Buscar(reservaId) ?? throw new NoEncontradoException($"No existe la reserva {reservaId}.");
        if (reserva.Estado == EstadoReserva.Cancelada) throw new ReglaDeNegocioException("La reserva ya estaba cancelada.");

        int diasHastaLaEntrada = reserva.Entrada.DayNumber - reloj.Hoy.DayNumber;
        decimal porcentaje = diasHastaLaEntrada >= 2 ? 1.00m      // con 2 o más días de antelación: se devuelve todo
                           : diasHastaLaEntrada >= 0 ? 0.50m      // el día anterior o el mismo día: la mitad
                           : 0m;                                  // ya empezó la estancia: nada
        var cancelada = reserva with { Estado = EstadoReserva.Cancelada, Reembolso = Math.Round(reserva.Total * porcentaje, 2) };
        reservas.Actualizar(cancelada);
        return cancelada;
    }

    public IReadOnlyList<Habitacion> Disponibles(DateOnly entrada, DateOnly salida, int personas)
    {
        if (salida <= entrada) throw new ReglaDeNegocioException("La salida debe ser posterior a la entrada.");
        return habitaciones.Todas()
            .Where(h => h.Capacidad >= personas && !EstaOcupada(h.Id, entrada, salida))
            .OrderBy(h => h.PrecioNoche).ThenBy(h => h.Numero)
            .ToList();
    }

    public static decimal CalcularTotal(decimal precioNoche, int noches)
    {
        decimal bruto = precioNoche * noches;
        return Math.Round(noches >= NochesParaDescuento ? bruto * (1 - Descuento) : bruto, 2);
    }

    // Dos estancias [entrada, salida) se solapan si cada una empieza antes de que la otra termine.
    // Con intervalos semiabiertos, salir un día y entrar otro EL MISMO día NO es solapamiento.
    private bool EstaOcupada(int habitacionId, DateOnly entrada, DateOnly salida) =>
        reservas.ListarPorHabitacion(habitacionId)
            .Any(r => r.Estado == EstadoReserva.Activa && entrada < r.Salida && r.Entrada < salida);
}
```

**Qué notar en ella:**

- **Intervalos semiabiertos** `[entrada, salida)`: la condición de solapamiento es `entrada < r.Salida && r.Entrada < salida`. **Una sola fórmula** cubre «dentro», «envuelve», «pisa el inicio» y «pisa el final», y deja pasar «salgo el día que tú entras». Es la técnica estándar; apréndela.
- **`DayNumber`** para restar fechas: `DateOnly` no tiene resta directa, y así **no hay zonas horarias ni horas** de por medio.
- **La política de reembolso** es una cadena de `? :` con los tres escalones; si crece, merece una tabla o un patrón *Strategy*.
- **`Cancelar` crea una copia** con `with` (el record es inmutable) y la guarda con `Actualizar`.

### Para ampliar (te toca a ti: pruebas primero)

> **P2.1:** **recargo de fin de semana:** las noches de viernes y sábado cuestan un 20 % más. Escribe **primero** las pruebas (¿cuenta la noche del viernes al sábado?).
>
> **P2.2:** que el repositorio sea **PostgreSQL** con EF Core (reutiliza lo de la lección 48), con un **índice** o restricción que impida solapamientos **también en la base** (investiga `EXCLUDE USING gist` en PostgreSQL). Pregúntate: ¿qué pasa si dos personas reservan a la vez? (concurrencia, lección 43/48).
>
> **P2.3:** expón el servicio como **API** con Minimal API y añade pruebas de integración.
>
> **P2.4:** añade **hora de entrada/salida** (check-in a las 15:00, check-out a las 12:00) y comprueba cuántas reglas cambian. Esa es la medida de cuánto te ayudó tener el reloj y las fechas aislados.

---

## Proyecto 3 · Combate por turnos: lógica de juego portable a Unity

### Qué trabaja este proyecto

**Diseñar lógica de juego como C# puro**, con **el azar inyectado** para que sea **reproducible**. Combina lo de las Partes III y X: composición (`Health`, `Cooldown`), máquina de estados, eventos, inyección de dependencias y pruebas deterministas.

**Restricción real:** el proyecto se compila con **`netstandard2.1` y `LangVersion 9`** (los límites de Unity, lección 54), así que **se puede copiar tal cual a un proyecto de Unity**. (Esa copia **no se probó** en Unity.)

### El diseño

| Pieza | Papel |
|---|---|
| `IAleatorio` | **El azar como dependencia.** En el juego, aleatorio de verdad; en las pruebas, valores fijos |
| `Combatiente` | Compone un `Health`, estadísticas y una `Habilidad` opcional (con su `Cooldown`) |
| `StateMachine<EstadoCombate>` | `Preparando → EnCurso → Terminado`: no se puede ejecutar dos veces |
| `EventChannel<EventoCombate>` | El simulador **emite** eventos; la consola (o Unity) **los escucha**. El simulador no imprime nada |
| `SimuladorDeCombate` | Orquesta los turnos |

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using Juego.Core;

namespace Juego.Combate
{
    /// <summary>El azar como DEPENDENCIA: en el juego es aleatorio de verdad; en las pruebas, una secuencia fija.</summary>
    public interface IAleatorio
    {
        int Siguiente(int minInclusivo, int maxExclusivo);
    }

    public sealed class Habilidad
    {
        public string Nombre { get; }
        public int Multiplicador { get; }                // 2 = hace el doble de daño
        public Cooldown Recarga { get; }
        public Habilidad(string nombre, int multiplicador, float recargaEnTurnos)
        {
            Nombre = nombre; Multiplicador = multiplicador; Recarga = new Cooldown(recargaEnTurnos);
        }
    }

    public sealed class Combatiente
    {
        public string Nombre { get; }
        public Health Vida { get; }
        public int Ataque { get; }
        public int Defensa { get; }
        public int Velocidad { get; }
        public Habilidad Habilidad { get; }
        public bool EstaVivo => !Vida.IsDead;

        public Combatiente(string nombre, int vida, int ataque, int defensa, int velocidad, Habilidad habilidad = null)
        {
            Nombre = nombre; Vida = new Health(vida); Ataque = ataque; Defensa = defensa; Velocidad = velocidad; Habilidad = habilidad;
        }
    }

    public enum EstadoCombate { Preparando, EnCurso, Terminado }

    /// <summary>Lo que ocurre en el combate, como datos: la interfaz (consola, Unity…) decide cómo mostrarlo.</summary>
    public sealed class EventoCombate
    {
        public int Turno { get; }
        public string Texto { get; }
        public EventoCombate(int turno, string texto) { Turno = turno; Texto = texto; }
        public override string ToString() => $"[T{Turno}] {Texto}";
    }

    /// <summary>Un combate por turnos entre dos bandos. Sin consola, sin Unity, sin azar real: todo inyectado.</summary>
    public sealed class SimuladorDeCombate
    {
        public const int ProbabilidadCriticoPorCien = 10;
        public const int TurnosMaximos = 100;               // protección contra combates que no acaban nunca

        private readonly List<Combatiente> _heroes, _enemigos;
        private readonly IAleatorio _azar;
        private readonly StateMachine<EstadoCombate> _estado;

        public EventChannel<EventoCombate> Eventos { get; } = new EventChannel<EventoCombate>();
        public int Turno { get; private set; }
        public EstadoCombate Estado => _estado.Actual;
        public IReadOnlyList<Combatiente> Heroes => _heroes;
        public IReadOnlyList<Combatiente> Enemigos => _enemigos;
        public bool GananLosHeroes => _enemigos.All(e => !e.EstaVivo) && _heroes.Any(h => h.EstaVivo);

        public SimuladorDeCombate(IEnumerable<Combatiente> heroes, IEnumerable<Combatiente> enemigos, IAleatorio azar)
        {
            _heroes = heroes.ToList(); _enemigos = enemigos.ToList(); _azar = azar;
            if (_heroes.Count == 0 || _enemigos.Count == 0) throw new ArgumentException("Cada bando necesita al menos un combatiente.");
            _estado = new StateMachine<EstadoCombate>(EstadoCombate.Preparando)
                .Permitir(EstadoCombate.Preparando, EstadoCombate.EnCurso)
                .Permitir(EstadoCombate.EnCurso, EstadoCombate.Terminado);
        }

        /// <summary>Calcula el daño de un ataque. Público para poder probar la fórmula sola.</summary>
        public static int CalcularDano(int ataque, int defensa, int multiplicador, bool critico)
        {
            int baseDano = Math.Max(1, ataque - defensa / 2);          // siempre se hace al menos 1 de daño
            int dano = baseDano * multiplicador;
            return critico ? dano * 2 : dano;
        }

        public void Ejecutar()
        {
            if (!_estado.TryIr(EstadoCombate.EnCurso)) throw new InvalidOperationException("El combate ya empezó o terminó.");
            while (Turno < TurnosMaximos && !HayGanador())
            {
                Turno++;
                // Orden de actuación: del más rápido al más lento (desempate por nombre para que sea determinista)
                var orden = _heroes.Concat(_enemigos).Where(c => c.EstaVivo)
                                   .OrderByDescending(c => c.Velocidad).ThenBy(c => c.Nombre).ToList();
                foreach (var actor in orden)
                {
                    if (!actor.EstaVivo) continue;                      // pudo morir antes de que le tocara
                    if (HayGanador()) break;
                    Actuar(actor);
                }
                foreach (var c in orden) c.Habilidad?.Recarga.Tick(1f); // la recarga se mide en turnos
            }
            _estado.TryIr(EstadoCombate.Terminado);
            Eventos.Emitir(new EventoCombate(Turno, HayGanador()
                ? (GananLosHeroes ? "VICTORIA de los héroes" : "DERROTA: ganan los enemigos")
                : $"EMPATE: se alcanzó el máximo de {TurnosMaximos} turnos"));
        }

        private bool HayGanador() => _heroes.All(h => !h.EstaVivo) || _enemigos.All(e => !e.EstaVivo);

        private void Actuar(Combatiente actor)
        {
            var bandoContrario = _heroes.Contains(actor) ? _enemigos : _heroes;
            var objetivo = bandoContrario.Where(c => c.EstaVivo).OrderBy(c => c.Vida.Current).ThenBy(c => c.Nombre).First();   // ataca al más débil

            bool usaHabilidad = actor.Habilidad != null && actor.Habilidad.Recarga.TryUsar();
            int multiplicador = usaHabilidad ? actor.Habilidad.Multiplicador : 1;
            bool critico = _azar.Siguiente(0, 100) < ProbabilidadCriticoPorCien;
            int dano = CalcularDano(actor.Ataque, objetivo.Defensa, multiplicador, critico);

            string accion = usaHabilidad ? $"usa {actor.Habilidad.Nombre} sobre" : "ataca a";
            objetivo.Vida.TakeDamage(dano);
            Eventos.Emitir(new EventoCombate(Turno,
                $"{actor.Nombre} {accion} {objetivo.Nombre}: {dano} de daño{(critico ? " ¡CRÍTICO!" : "")} ({objetivo.Vida.Current}/{objetivo.Vida.Max})"));
            if (!objetivo.EstaVivo) Eventos.Emitir(new EventoCombate(Turno, $"{objetivo.Nombre} cae derrotado"));
        }
    }
}
```

### Cómo se lee, paso a paso

1. **`CalcularDano` es estática y pública**: así **la fórmula se prueba sola**, sin montar un combate.
2. **Daño mínimo 1** (`Math.Max(1, …)`): sin esa regla, un enemigo con mucha defensa **haría daño 0 y el combate no acabaría nunca**. Por eso existe `TurnosMaximos`: **protección de último recurso** contra bucles infinitos, que acaba en empate.
3. **El orden de actuación se calcula al inicio del turno** (del más rápido al más lento, con desempate por nombre para ser **determinista**). Pero alguien puede **morir durante el turno**: por eso `Actuar` comprueba `EstaVivo` antes de dejarle jugar (**esa comprobación tuvo su prueba solo después de que una mutación sobreviviera**; ver abajo).
4. **La recarga de la habilidad se mide en turnos**: se hace `Tick(1f)` al final de cada turno. `Cooldown` no sabe qué es un «turno»: **solo recibe un número**. En Unity recibiría `Time.deltaTime`.
5. **`Eventos.Emitir`**: el simulador **no usa `Console.WriteLine`**. La interfaz se suscribe y decide qué hacer.
6. **Todo el azar sale de `_azar.Siguiente(0, 100)`**: un solo punto, inyectado. Con la misma semilla, **el mismo combate**.

### El ejecutable de consola

```csharp
using Juego.Combate;

// El «adaptador» entre el azar real de .NET y la interfaz IAleatorio que usa el simulador.
// Con una SEMILLA (seed) la secuencia es siempre la misma: mismos números → mismo combate.
int semilla = args.Length > 0 && int.TryParse(args[0], out int s) ? s : 42;
var azar = new AzarReal(semilla);

var heroes = new[]
{
    new Combatiente("Aria",  vida: 60, ataque: 14, defensa: 6, velocidad: 12, new Habilidad("Tajo doble", multiplicador: 2, recargaEnTurnos: 3)),
    new Combatiente("Bruno", vida: 80, ataque: 10, defensa: 10, velocidad: 6),
};
var enemigos = new[]
{
    new Combatiente("Goblin",  vida: 35, ataque: 9, defensa: 3, velocidad: 9),
    new Combatiente("Orco",    vida: 70, ataque: 13, defensa: 8, velocidad: 4, new Habilidad("Golpe brutal", 2, 4)),
};

var combate = new SimuladorDeCombate(heroes, enemigos, azar);
using var suscripcion = combate.Eventos.Suscribir(e => Console.WriteLine(e));    // la interfaz solo ESCUCHA los eventos

Console.WriteLine($"Semilla: {semilla}\n");
combate.Ejecutar();
Console.WriteLine($"\nEstado final: {combate.Estado}, turnos: {combate.Turno}");
foreach (var c in heroes.Concat(enemigos)) Console.WriteLine($"  {c.Nombre,-7} {c.Vida.Current,3}/{c.Vida.Max}");

sealed class AzarReal : IAleatorio
{
    private readonly Random _r;
    public AzarReal(int semilla) { _r = new Random(semilla); }
    public int Siguiente(int min, int max) => _r.Next(min, max);
}
```

**Con la semilla 42:**

```text
Semilla: 42

[T1] Aria usa Tajo doble sobre Goblin: 26 de daño (9/35)
[T1] Goblin ataca a Aria: 6 de daño (54/60)
[T1] Bruno ataca a Goblin: 9 de daño (0/35)
[T1] Goblin cae derrotado
[T1] Orco usa Golpe brutal sobre Aria: 20 de daño (34/60)
[T2] Aria ataca a Orco: 10 de daño (60/70)
[T2] Bruno ataca a Orco: 6 de daño (54/70)
[T2] Orco ataca a Aria: 10 de daño (24/60)
[T3] Aria ataca a Orco: 10 de daño (44/70)
[T3] Bruno ataca a Orco: 6 de daño (38/70)
[T3] Orco ataca a Aria: 10 de daño (14/60)
[T4] Aria usa Tajo doble sobre Orco: 20 de daño (18/70)
[T4] Bruno ataca a Orco: 6 de daño (12/70)
[T4] Orco ataca a Aria: 10 de daño (4/60)
[T5] Aria ataca a Orco: 10 de daño (2/70)
[T5] Bruno ataca a Orco: 6 de daño (0/70)
[T5] Orco cae derrotado
[T5] VICTORIA de los héroes

Estado final: Terminado, turnos: 5
  Aria      4/60
  Bruno    80/80
  Goblin    0/35
  Orco      0/70
```

**Con la semilla 7** (distinta: aparecen críticos y el combate dura más):

```text
Semilla: 7

[T1] Aria usa Tajo doble sobre Goblin: 26 de daño (9/35)
[T1] Goblin ataca a Aria: 6 de daño (54/60)
[T1] Bruno ataca a Goblin: 9 de daño (0/35)
[T1] Goblin cae derrotado
[T1] Orco usa Golpe brutal sobre Aria: 40 de daño ¡CRÍTICO! (14/60)
[T2] Aria ataca a Orco: 10 de daño (60/70)
[T2] Bruno ataca a Orco: 6 de daño (54/70)
[T2] Orco ataca a Aria: 20 de daño ¡CRÍTICO! (0/60)
[T2] Aria cae derrotado
[T3] Bruno ataca a Orco: 6 de daño (48/70)
[T3] Orco ataca a Bruno: 8 de daño (72/80)
[T4] Bruno ataca a Orco: 6 de daño (42/70)
[T4] Orco ataca a Bruno: 8 de daño (64/80)
[T5] Bruno ataca a Orco: 6 de daño (36/70)
[T5] Orco usa Golpe brutal sobre Bruno: 16 de daño (48/80)
[T6] Bruno ataca a Orco: 6 de daño (30/70)
[T6] Orco ataca a Bruno: 8 de daño (40/80)
[T7] Bruno ataca a Orco: 6 de daño (24/70)
[T7] Orco ataca a Bruno: 8 de daño (32/80)
[T8] Bruno ataca a Orco: 6 de daño (18/70)
[T8] Orco ataca a Bruno: 8 de daño (24/80)
[T9] Bruno ataca a Orco: 12 de daño ¡CRÍTICO! (6/70)
[T9] Orco usa Golpe brutal sobre Bruno: 16 de daño (8/80)
[T10] Bruno ataca a Orco: 6 de daño (0/70)
[T10] Orco cae derrotado
[T10] VICTORIA de los héroes

Estado final: Terminado, turnos: 10
  Aria      0/60
  Bruno     8/80
  Goblin    0/35
  Orco      0/70
```

**Qué se ve:** **el mismo programa con distinta semilla da distinto combate, y con la misma semilla da siempre exactamente el mismo.** Eso es lo que permite **reproducir un error** («en la semilla 7, turno 9, el crítico hizo…») y hacer **repeticiones** de partidas.

### Las pruebas

```csharp
using Juego.Combate;

namespace Juego.Tests;

/// <summary>Un «azar» que devuelve siempre lo que la prueba decide: sin aleatoriedad, sin pruebas intermitentes.</summary>
internal sealed class AzarFijo : IAleatorio
{
    private readonly Queue<int> _valores; private readonly int _porDefecto;
    public AzarFijo(int porDefecto = 99, params int[] primeros) { _porDefecto = porDefecto; _valores = new Queue<int>(primeros); }
    public int Siguiente(int min, int max) => _valores.Count > 0 ? _valores.Dequeue() : _porDefecto;   // 99 = nunca crítico (crítico si < 10)
}

public class CombateTests
{
    private static Combatiente C(string n, int vida = 50, int atq = 10, int def = 0, int vel = 5, Habilidad? h = null) =>
        new(n, vida, atq, def, vel, h!);

    private static List<string> Registrar(SimuladorDeCombate combate)
    {
        var log = new List<string>();
        combate.Eventos.Suscribir(e => log.Add(e.Texto));
        return log;
    }

    [Theory]
    [InlineData(10, 0, 1, false, 10)]     // sin defensa: todo el ataque
    [InlineData(10, 6, 1, false, 7)]      // la defensa cuenta a la mitad: 10 - 3
    [InlineData(2, 20, 1, false, 1)]      // mínimo 1 de daño, siempre
    [InlineData(10, 0, 2, false, 20)]     // habilidad ×2
    [InlineData(10, 0, 1, true, 20)]      // crítico ×2
    [InlineData(10, 0, 2, true, 40)]      // los dos se multiplican
    public void FormulaDeDano(int ataque, int defensa, int mult, bool critico, int esperado) =>
        Assert.Equal(esperado, SimuladorDeCombate.CalcularDano(ataque, defensa, mult, critico));

    [Fact]
    public void ActuanDelMasRapidoAlMasLento()
    {
        var combate = new SimuladorDeCombate(new[] { C("Lento", vel: 1, vida: 500) }, new[] { C("Rapido", vel: 9, vida: 500), C("Medio", vel: 5, vida: 500) }, new AzarFijo());
        var log = Registrar(combate);
        combate.Ejecutar();
        var primerosTres = log.Take(3).Select(l => l.Split(' ')[0]).ToList();
        Assert.Equal(new[] { "Rapido", "Medio", "Lento" }, primerosTres);
    }

    [Fact]
    public void AtacaAlEnemigoMasDebil()
    {
        var combate = new SimuladorDeCombate(new[] { C("Heroe", atq: 5, vel: 9, vida: 999) },
                                             new[] { C("Fuerte", vida: 100, atq: 0, vel: 1), C("Debil", vida: 10, atq: 0, vel: 1) }, new AzarFijo());
        var log = Registrar(combate); combate.Ejecutar();
        Assert.StartsWith("Heroe ataca a Debil", log[0]);
    }

    [Fact]
    public void LosHeroesGananCuandoNoQuedanEnemigos()
    {
        var combate = new SimuladorDeCombate(new[] { C("Heroe", atq: 100, vel: 9) }, new[] { C("Debil", vida: 10, vel: 1) }, new AzarFijo());
        combate.Ejecutar();
        Assert.True(combate.GananLosHeroes);
        Assert.Equal(EstadoCombate.Terminado, combate.Estado);
        Assert.Equal(1, combate.Turno);
    }

    [Fact]
    public void LosEnemigosGananCuandoNoQuedanHeroes()
    {
        var combate = new SimuladorDeCombate(new[] { C("Heroe", vida: 5, vel: 1) }, new[] { C("Bestia", atq: 100, vel: 9) }, new AzarFijo());
        var log = Registrar(combate); combate.Ejecutar();
        Assert.False(combate.GananLosHeroes);
        Assert.Contains(log, l => l.StartsWith("DERROTA"));
    }

    [Fact]
    public void UnMuertoNoActuaEnELMismoTurno()
    {
        // El héroe (más rápido) mata al enemigo antes de que le toque: el enemigo no debe atacar
        var combate = new SimuladorDeCombate(new[] { C("Heroe", atq: 100, vel: 9) }, new[] { C("Debil", vida: 1, atq: 50, vel: 1) }, new AzarFijo());
        var log = Registrar(combate); combate.Ejecutar();
        Assert.DoesNotContain(log, l => l.StartsWith("Debil ataca"));
    }

    [Fact]
    public void UnMuertoNoActuaAunqueElCombateSigaEnCurso()
    {
        // El orden de actuación se calcula al empezar el turno, así que «Debil1» sigue en esa lista aunque muera.
        // El combate NO termina (queda «Debil2» vivo), por eso aquí la comprobación de «¿sigue vivo?» es la que importa.
        var combate = new SimuladorDeCombate(new[] { C("Heroe", atq: 100, vel: 9, vida: 999) },
                                             new[] { C("Debil1", vida: 1, atq: 50, vel: 5), C("Debil2", vida: 999, atq: 1, vel: 1) }, new AzarFijo());
        var log = Registrar(combate); combate.Ejecutar();
        Assert.Contains(log, l => l.Contains("Debil1 cae derrotado"));
        Assert.DoesNotContain(log, l => l.StartsWith("Debil1 ataca"));
    }

    [Fact]
    public void ElCriticoDependeDelAzarInyectado()
    {
        // Primer valor del azar = 5 (< 10) → crítico; los siguientes = 99 → no crítico
        var combate = new SimuladorDeCombate(new[] { C("Heroe", atq: 10, vel: 9) }, new[] { C("Blanco", vida: 1000, atq: 0, vel: 1) }, new AzarFijo(99, 5));
        var log = Registrar(combate); combate.Ejecutar();
        Assert.Contains("¡CRÍTICO!", log[0]);
        Assert.DoesNotContain("¡CRÍTICO!", log[1]);
    }

    [Fact]
    public void LaHabilidadTieneRecargaEnTurnos()
    {
        var habilidad = new Habilidad("Golpe", multiplicador: 3, recargaEnTurnos: 2);
        var combate = new SimuladorDeCombate(new[] { C("Heroe", atq: 10, vel: 9, vida: 999, h: habilidad) },
                                             new[] { C("Blanco", vida: 1000, atq: 0, vel: 1) }, new AzarFijo());
        var eventos = new List<EventoCombate>();
        combate.Eventos.Suscribir(eventos.Add);
        combate.Ejecutar();
        var turnosConHabilidad = eventos.Where(e => e.Texto.Contains("usa Golpe")).Select(e => e.Turno).Take(3).ToList();
        Assert.Equal(new[] { 1, 3, 5 }, turnosConHabilidad);          // recarga de 2 turnos: se usa en el 1, queda lista en el 3, en el 5…
    }

    [Fact]
    public void SiNadieCaeSeTerminaPorLimiteDeTurnosEnEmpate()
    {
        var combate = new SimuladorDeCombate(new[] { C("A", vida: 100000, atq: 1) }, new[] { C("B", vida: 100000, atq: 1) }, new AzarFijo());
        var log = Registrar(combate); combate.Ejecutar();
        Assert.Equal(SimuladorDeCombate.TurnosMaximos, combate.Turno);
        Assert.Contains(log, l => l.StartsWith("EMPATE"));
    }

    [Fact]
    public void NoSePuedeEjecutarDosVeces()
    {
        var combate = new SimuladorDeCombate(new[] { C("A", atq: 100, vel: 9) }, new[] { C("B", vida: 1) }, new AzarFijo());
        combate.Ejecutar();
        Assert.Throws<InvalidOperationException>(() => combate.Ejecutar());
    }

    [Fact]
    public void ExigeAlMenosUnCombatientePorBando()
    {
        Assert.Throws<ArgumentException>(() => new SimuladorDeCombate(new List<Combatiente>(), new[] { C("B") }, new AzarFijo()));
    }

    [Fact]
    public void MismoAzar_MismoCombate()
    {
        List<string> Jugar()
        {
            var combate = new SimuladorDeCombate(new[] { C("A", atq: 12, vel: 7, vida: 80) }, new[] { C("B", atq: 11, vel: 6, vida: 80) }, new AzarFijo(40));
            var log = Registrar(combate); combate.Ejecutar(); return log;
        }
        Assert.Equal(Jugar(), Jugar());                               // reproducible: imprescindible para depurar y para repeticiones
    }
}
```

```text
Passed Juego.Tests.CombateTests.UnMuertoNoActuaAunqueElCombateSigaEnCurso
Passed Juego.Tests.CombateTests.UnMuertoNoActuaEnELMismoTurno [< 1 ms]
Passed Juego.Tests.CombateTests.AtacaAlEnemigoMasDebil [< 1 ms]
Passed Juego.Tests.CombateTests.ActuanDelMasRapidoAlMasLento
Passed Juego.Tests.CombateTests.LosHeroesGananCuandoNoQuedanEnemigos
Passed Juego.Tests.CombateTests.MismoAzar_MismoCombate [< 1 ms]
Passed Juego.Tests.CombateTests.FormulaDeDano(ataque: 10, defensa: 0, mult: 1, critico: True, esperado: 20) [< 1 ms]
Passed Juego.Tests.CombateTests.FormulaDeDano(ataque: 10, defensa: 6, mult: 1, critico: False, esperado: 7) [< 1 ms]
Passed Juego.Tests.CombateTests.FormulaDeDano(ataque: 10, defensa: 0, mult: 1, critico: False, esperado: 10) [< 1 ms]
Passed Juego.Tests.CombateTests.FormulaDeDano(ataque: 10, defensa: 0, mult: 2, critico: False, esperado: 20) [< 1 ms]
Passed Juego.Tests.CombateTests.FormulaDeDano(ataque: 10, defensa: 0, mult: 2, critico: True, esperado: 40) [< 1 ms]
Passed Juego.Tests.CombateTests.FormulaDeDano(ataque: 2, defensa: 20, mult: 1, critico: False, esperado: 1) [< 1 ms]
Passed Juego.Tests.CombateTests.NoSePuedeEjecutarDosVeces [< 1 ms]
Passed Juego.Tests.CombateTests.SiNadieCaeSeTerminaPorLimiteDeTurnosEnEmpate
Passed Juego.Tests.CombateTests.LaHabilidadTieneRecargaEnTurnos
Passed Juego.Tests.CombateTests.LosEnemigosGananCuandoNoQuedanHeroes [< 1 ms]
Passed Juego.Tests.CombateTests.ElCriticoDependeDelAzarInyectado
Passed Juego.Tests.CombateTests.ExigeAlMenosUnCombatientePorBando [< 1 ms]
```

### Mutaciones: y un hueco que se encontró

```text
$ MUTACIÓN: el daño mínimo pasa de 1 a 0 (combates que no acaban)
Failed Juego.Tests.CombateTests.FormulaDeDano(ataque: 2, defensa: 20, mult: 1, critico: False, esperado: 1)
Failed!  - Failed:     1, Passed:    33, Skipped:     0, Total:    34

$ MUTACIÓN: actúa primero el más lento
Failed Juego.Tests.CombateTests.UnMuertoNoActuaEnELMismoTurno
Failed Juego.Tests.CombateTests.AtacaAlEnemigoMasDebil
Failed Juego.Tests.CombateTests.ActuanDelMasRapidoAlMasLento

$ MUTACIÓN: ataca al más fuerte en vez de al más débil
Failed Juego.Tests.CombateTests.AtacaAlEnemigoMasDebil
Failed!  - Failed:     1, Passed:    33, Skipped:     0, Total:    34

$ MUTACIÓN: un muerto sigue actuando en su turno
Passed!  - Failed:     0, Passed:    34, Skipped:     0, Total:    34      ← ¡SOBREVIVIÓ! Hueco en las pruebas (ver abajo)

$ MUTACIÓN: la condición del crítico está invertida
Failed Juego.Tests.CombateTests.ElCriticoDependeDelAzarInyectado
Failed!  - Failed:     1, Passed:    33, Skipped:     0, Total:    34


--- Se añadió la prueba que faltaba y se repitió ESA mutación ---
$ (original)
Passed!  - Failed:     0, Passed:    35, Skipped:     0, Total:    35

$ MUTACIÓN: un muerto sigue actuando en su turno   (el mutante que SOBREVIVIÓ la primera vez)
Failed Juego.Tests.CombateTests.UnMuertoNoActuaAunqueElCombateSigaEnCurso
Failed!  - Failed:     1, Passed:    34, Skipped:     0, Total:    35
```

**La lección más importante de este proyecto está aquí.** De cinco mutaciones, **cuatro** fueron detectadas a la primera. **Una sobrevivió:** quitar la comprobación «el muerto no actúa». Eso significaba que **mis pruebas no verificaban esa regla**, aunque **creía que sí**: mi prueba `UnMuertoNoActuaEnELMismoTurno` terminaba el combate antes de que la comprobación importara (el combate se acababa y salía por otra vía).

**Qué se hizo:** se escribió una prueba (`UnMuertoNoActuaAunqueElCombateSigaEnCurso`) con **dos enemigos**, de modo que muera uno y el combate **continúe**; se **repitió esa mutación** y **entonces sí falló la prueba**. **Fíjate en lo que esto enseña sobre la «cobertura de código»:** mide qué líneas **se ejecutan**, no si hay una **aserción** que dependa de ellas. Una línea puede ejecutarse en las pruebas y, aun así, **nadie compruebe lo que hace**.

> **Pruebas que pasan no significan pruebas que protegen.** La forma práctica de saberlo es **romper el código a propósito y ver si alguien se queja**.

### Para ampliar (el diseño aguanta estos cambios si lo hiciste bien)

> **P3.1:** añade **veneno** (daño por turno durante N turnos). ¿Dónde lo pones: en `Combatiente`, en el simulador o en un componente nuevo? Escribe **primero** la prueba.
>
> **P3.2:** que la **elección de objetivo** sea una **estrategia intercambiable** (`IElegirObjetivo`: el más débil, el más fuerte, aleatorio) en lugar de estar escrita dentro de `Actuar`. Es el patrón **Strategy** (Parte III); comprueba que **las pruebas existentes siguen pasando sin tocarlas**.
>
> **P3.3:** añade un **bando de 3 contra 3** y una **barra de vida en texto** (`[██████░░░░] 60 %`) que se actualice al suscribirse a los eventos.
>
> **P3.4 (necesita Unity):** copia `Juego.Core` y `Juego.Combate` a `Assets/Scripts/`, escribe un `MonoBehaviour` fino que dibuje la barra de vida y **anota qué no compiló o no se comportó como aquí se dice**.

---

## Cierre del curso

### Lo que has construido, y de qué parte viene cada cosa

| Habilidad | Dónde |
|---|---|
| Sintaxis, tipos, colecciones, LINQ, errores | Parte I |
| Capturar y mostrar datos, **CRUD** de verdad, capas | Parte II |
| Clases abstractas, interfaces, **singleton**, **inyección**, patrones, SOLID | Parte III |
| Cómo funciona .NET, soluciones, NuGet, Git, depuración, estructuras de datos | Parte IV |
| C# 8–14, reflexión, generadores, concurrencia, asincronía | Parte V |
| Pruebas, TDD, dobles, BDD | Parte VI |
| API web, EF Core + PostgreSQL, seguridad, configuración, despliegue | Parte VII |
| Interfaz web con Blazor | Parte VIII |
| Escritorio con WPF y MVVM | Parte IX |
| Unity | Parte X |
| **Todo junto** | Parte XI |

### La regla que atraviesa todo el curso

**No te creas nada —tampoco este texto— que no hayas visto fallar.** Cada vez que algo parecía funcionar a la primera, se **rompió a propósito** para comprobar que las pruebas se quejaban; cada vez que un resultado contradecía la teoría, se **investigó en lugar de ajustarlo**; y lo que no se pudo ejecutar **se dijo**. Es la costumbre que más te va a servir: la de desconfiar con método.

### Qué NO cubre este curso (para no dar falsas expectativas)

- **Seguridad de verdad:** hay buenas prácticas, no un análisis de amenazas. Para producción: ASP.NET Core Identity o un proveedor de identidad, revisión por alguien con experiencia.
- **Rendimiento a escala:** mediciones en una máquina de 4 núcleos con datos de juguete.
- **Nube y operación:** Docker y CI **se escribieron pero no se ejecutaron**; no hay Kubernetes, observabilidad, ni despliegues.
- **Bases de datos avanzadas:** índices, planes de ejecución, particionado.
- **Móvil (.NET MAUI), microservicios, mensajería, DDD en profundidad.**
- **Windows y macOS:** todo se probó en Linux; WPF, solo compilado.
- **Unity:** la lógica en C# puro sí; los scripts, solo compilados contra un molde.

### Y ahora

1. **Haz los ejercicios** que no te hayan salido solos. Los que no podías hacer son los que más te enseñan.
2. **Repite el Proyecto 2** (Reservas) **desde cero** sin mirar la solución, **a la semana**. Si no te sale, vuelve a la lección que te falte.
3. **Elige un proyecto tuyo**, pequeño, y constrúyelo **con pruebas primero**.
4. **Lee código ajeno**: el repositorio de .NET (`dotnet/runtime`, `dotnet/aspnetcore`) es público.
