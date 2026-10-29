# Curso de C# · Parte IX: escritorio con WPF y MVVM

Una aplicación de escritorio tiene el mismo problema que una web: **mucha lógica enredada con la pantalla**. La solución es el patrón **MVVM**, y lo bueno es que se puede **probar la lógica sin abrir ninguna ventana**. Esta parte construye el cliente de escritorio de TaskFlow con **WPF**.

*Verificado: 2026-10-29 con .NET SDK 10.0.113 **en Linux**. Aquí hay que ser muy claro con lo que se probó y lo que no:*

| Qué | Estado |
|---|---|
| **ViewModels** (toda la lógica: propiedades, comandos, errores, estado «ocupado») | **Ejecutado y probado**: 17 pruebas, y 4 mutaciones que las pruebas detectaron |
| El proyecto WPF (XAML, ventana, composición con DI) | **Compila** (`0 Error(s)`), pero **NO se ejecutó** |
| **Bindings** de XAML, aspecto, layout, comportamiento real de los botones | **No verificado**: WPF solo se ejecuta en **Windows** y aquí no hay Windows |

> **Un error de un binding en XAML no se detecta al compilar:** se descubre en ejecución, en la ventana *Salida* del depurador («BindingExpression path error»). Quien use este código **debe ejecutarlo en Windows y comprobar la ventana** antes de fiarse de él.

## Mapa de la Parte IX

| Lección | Tema | Verificado |
|---|---|---|
| 53 | MVVM con WPF: `INotifyPropertyChanged`, comandos, `ObservableCollection`, XAML | ViewModel ejecutado; XAML solo compilado |

---

## Lección 53 · MVVM con WPF

### Qué vas a aprender

- Qué es **WPF** y cuándo se elige frente a otras opciones de escritorio.
- El patrón **MVVM** (Modelo–Vista–ViewModel): qué pone cada capa.
- `INotifyPropertyChanged`, `ObservableCollection<T>` y `ICommand`, **a mano** (para entenderlos) y con **CommunityToolkit.Mvvm** (como se hace en la práctica).
- **Probar un ViewModel sin ventana**, y comprobar que las pruebas **detectan** los errores.
- **Compilar un proyecto de Windows desde Linux** (y por qué eso **no** basta para fiarse de él).

### La idea

#### Opciones de escritorio con .NET

| Tecnología | Plataformas | Cuándo |
|---|---|---|
| **WPF** | **Solo Windows** | Aplicaciones empresariales ricas; enlace de datos potente; muchísimo código existente |
| **WinForms** | Solo Windows | Utilidades internas rápidas; código antiguo |
| **.NET MAUI** | Windows, macOS, Android, iOS | Una base de código para móvil y escritorio. *No cubierto aquí* |
| **Avalonia** | Windows, macOS, Linux | Escritorio multiplataforma con XAML parecido a WPF. *No probado aquí* |

#### MVVM: tres piezas

```text
 ┌────────────┐  enlace de datos  ┌──────────────┐   usa    ┌─────────┐
 │   Vista    │ ◄───────────────► │  ViewModel   │ ───────► │ Modelo  │
 │ (XAML)     │   y comandos      │ (C# puro)    │          │ + API   │
 └────────────┘                   └──────────────┘          └─────────┘
  Solo dibuja.                     El estado y el             Los datos y
  No tiene lógica.                 comportamiento de          las reglas.
                                   la pantalla.
```

- La **Vista** (XAML) dibuja y **no decide nada**.
- El **ViewModel** guarda **el estado de la pantalla** (el texto escrito, si está cargando, el error) y **lo que ocurre al pulsar** (comandos). **No conoce** ventanas ni controles.
- La Vista y el ViewModel se conectan con **enlaces** (`{Binding Titulo}`): si la propiedad cambia, la pantalla se repinta **sola**; y si el usuario escribe, la propiedad cambia **sola**.

**Por qué importa:** como el ViewModel es C# normal, **se prueba con xUnit en milisegundos**, sin abrir una ventana, sin hacer clics. Es exactamente la idea de la capa `Core` de TaskFlow, aplicada a la pantalla.

#### Las tres piezas técnicas

| Pieza | Para qué | Si falta |
|---|---|---|
| **`INotifyPropertyChanged`** | La propiedad **avisa** de que cambió | La pantalla muestra datos viejos |
| **`ObservableCollection<T>`** | La colección avisa al **añadir/quitar** elementos (`List<T>` no lo hace) | La lista no se actualiza |
| **`ICommand`** | Un botón se enlaza a una acción y a su condición (`CanExecute`) | Tendrías que escribir código en la ventana |

### Código 1: lo básico, escrito a mano

`ObservableObject.cs`: la base que avisa de los cambios

```csharp
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Mvvm.Core.Manual;

/// <summary>
/// La base de todo ViewModel: avisa a la interfaz cuando una propiedad cambia (INotifyPropertyChanged).
/// Sin este aviso, la pantalla no se entera de que debe repintar.
/// </summary>
public abstract class ObservableObject : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propiedad = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propiedad));

    /// <summary>Asigna el campo SOLO si el valor es distinto, y avisa. Devuelve si hubo cambio.</summary>
    protected bool Set<T>(ref T campo, T valor, [CallerMemberName] string? propiedad = null)
    {
        if (EqualityComparer<T>.Default.Equals(campo, valor)) return false;
        campo = valor;
        OnPropertyChanged(propiedad);
        return true;
    }
}
```

`Comandos.cs`: `RelayCommand` y el comando asíncrono

```csharp
using System.Windows.Input;

namespace Mvvm.Core.Manual;

/// <summary>Un botón de la interfaz se enlaza a un ICommand: decide si está habilitado (CanExecute) y qué hace (Execute).</summary>
public sealed class RelayCommand(Action ejecutar, Func<bool>? puedeEjecutar = null) : ICommand
{
    public event EventHandler? CanExecuteChanged;
    public bool CanExecute(object? parameter) => puedeEjecutar?.Invoke() ?? true;
    public void Execute(object? parameter) => ejecutar();
    public void NotificarCambio() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);   // «vuelve a preguntar si me puedes pulsar»
}

public sealed class RelayCommand<T>(Action<T> ejecutar, Func<T, bool>? puedeEjecutar = null) : ICommand
{
    public event EventHandler? CanExecuteChanged;
    public bool CanExecute(object? parameter) => parameter is T valor && (puedeEjecutar?.Invoke(valor) ?? true);
    public void Execute(object? parameter) { if (parameter is T valor) ejecutar(valor); }
    public void NotificarCambio() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}

/// <summary>
/// Comando asíncrono: deshabilita el botón MIENTRAS corre (evita el doble clic) y captura las excepciones
/// (un 'async void' que lanzara una excepción tumbaría la aplicación entera).
/// </summary>
public sealed class AsyncRelayCommand(Func<Task> ejecutar, Func<bool>? puedeEjecutar = null, Action<Exception>? alError = null) : ICommand
{
    private bool _ejecutando;
    public event EventHandler? CanExecuteChanged;
    public bool EstaEjecutando => _ejecutando;
    public bool CanExecute(object? parameter) => !_ejecutando && (puedeEjecutar?.Invoke() ?? true);

    public async void Execute(object? parameter) => await EjecutarAsync();   // «async void» SOLO aquí, porque la interfaz ICommand lo exige

    public async Task EjecutarAsync()
    {
        if (!CanExecute(null)) return;
        _ejecutando = true; NotificarCambio();
        try { await ejecutar(); }
        catch (Exception ex) when (alError is not null) { alError(ex); }       // sin alError, la excepción se propaga
        finally { _ejecutando = false; NotificarCambio(); }
    }

    public void NotificarCambio() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
```

`Modelo.cs`: el modelo y lo que el ViewModel necesita del exterior

```csharp
namespace Mvvm.Core;

public sealed record Tarea(int Id, string Titulo, bool Hecha);

/// <summary>Lo que el ViewModel necesita del mundo exterior. En producción llamaría a la API de TaskFlow; en las pruebas es un doble.</summary>
public interface ITareasApi
{
    Task<IReadOnlyList<Tarea>> ListarAsync(CancellationToken ct = default);
    Task<Tarea> CrearAsync(string titulo, CancellationToken ct = default);
    Task CompletarAsync(int id, CancellationToken ct = default);
}
```

`TareasViewModel.cs`: el ViewModel

```csharp
using System.Collections.ObjectModel;

namespace Mvvm.Core.Manual;

/// <summary>
/// El ViewModel: el estado y el comportamiento de la pantalla, SIN saber nada de botones, ventanas ni XAML.
/// Por eso se puede probar sin abrir ninguna ventana.
/// </summary>
public sealed class TareasViewModel : ObservableObject
{
    private readonly ITareasApi _api;
    private string _nuevoTitulo = "";
    private string? _error;
    private bool _ocupado;

    public TareasViewModel(ITareasApi api)
    {
        _api = api;
        CargarCommand = new AsyncRelayCommand(CargarAsync, alError: MostrarError);
        CrearCommand = new AsyncRelayCommand(CrearAsync, () => PuedeCrear, MostrarError);
        // CompletarAsync captura sus propias excepciones (ver abajo), por eso aquí se puede lanzar sin esperar.
        CompletarCommand = new RelayCommand<Tarea>(t => _ = CompletarAsync(t), t => !t.Hecha);
    }

    // ObservableCollection avisa a la interfaz cuando se AÑADE o QUITA un elemento (List<T> no lo hace)
    public ObservableCollection<Tarea> Tareas { get; } = [];

    public string NuevoTitulo
    {
        get => _nuevoTitulo;
        set
        {
            if (!Set(ref _nuevoTitulo, value)) return;
            OnPropertyChanged(nameof(PuedeCrear));          // una propiedad CALCULADA también hay que avisarla
            CrearCommand.NotificarCambio();                 // y el botón «Crear» debe reevaluar si se puede pulsar
        }
    }

    public string? Error { get => _error; private set => Set(ref _error, value); }
    public bool Ocupado { get => _ocupado; private set => Set(ref _ocupado, value); }
    public bool PuedeCrear => NuevoTitulo.Trim().Length >= 3;
    public int Pendientes => Tareas.Count(t => !t.Hecha);

    public AsyncRelayCommand CargarCommand { get; }
    public AsyncRelayCommand CrearCommand { get; }
    public RelayCommand<Tarea> CompletarCommand { get; }

    private async Task CargarAsync()
    {
        Ocupado = true; Error = null;
        try
        {
            var lista = await _api.ListarAsync();
            Tareas.Clear();
            foreach (var t in lista) Tareas.Add(t);
            OnPropertyChanged(nameof(Pendientes));
        }
        finally { Ocupado = false; }
    }

    private async Task CrearAsync()
    {
        Error = null;
        var creada = await _api.CrearAsync(NuevoTitulo.Trim());
        Tareas.Add(creada);
        NuevoTitulo = "";
        OnPropertyChanged(nameof(Pendientes));
    }

    private async Task CompletarAsync(Tarea tarea)
    {
        try
        {
            await _api.CompletarAsync(tarea.Id);
            int i = Tareas.IndexOf(tarea);
            if (i >= 0) Tareas[i] = tarea with { Hecha = true };           // el record es inmutable: se reemplaza por una copia
            OnPropertyChanged(nameof(Pendientes));
        }
        catch (Exception ex) { MostrarError(ex); }
    }

    private void MostrarError(Exception ex) => Error = ex.Message;
}
```

### Cómo se lee, paso a paso

1. **`Set(ref campo, valor)`** asigna **solo si cambió** y avisa con `PropertyChanged`. `[CallerMemberName]` rellena el nombre de la propiedad automáticamente (si te equivocas escribiendo un nombre a mano, **nada falla, pero la pantalla no se actualiza**).
2. **Una propiedad calculada también hay que avisarla.** `PuedeCrear` depende de `NuevoTitulo`; por eso su `set` hace `OnPropertyChanged(nameof(PuedeCrear))`. Es **el olvido más frecuente** en MVVM.
3. **`RelayCommand.CanExecute`** decide si el botón está habilitado; **`CanExecuteChanged`** es el aviso para que WPF **vuelva a preguntar** (por eso `NuevoTitulo` llama a `CrearCommand.NotificarCambio()`).
4. **`AsyncRelayCommand`** hace tres cosas importantes: **se bloquea mientras corre** (evita el doble clic), **captura las excepciones** (un `async void` que lanza hunde la aplicación entera) y **siempre** restablece su estado en el `finally`. El `async void` aparece **una sola vez**, en `Execute`, porque la interfaz `ICommand` lo exige.
5. **El ViewModel depende de `ITareasApi`** (una interfaz), no de `HttpClient`: **inyección de dependencias** (Parte III). En producción llega la implementación real; en las pruebas, un doble.
6. **`Tareas` es una `ObservableCollection`**: al hacer `Add`, WPF añade la fila **sin que escribas nada más**.
7. **Un `record` es inmutable**: para «completar» una tarea se **reemplaza** el elemento por una copia (`tarea with { Hecha = true }`).

### Código 2: lo mismo con CommunityToolkit.Mvvm

Escribir `OnPropertyChanged` en cada propiedad es repetitivo. **CommunityToolkit.Mvvm** (paquete oficial de Microsoft) usa **generadores de código** (lección 42): tú pones atributos y **el compilador escribe el resto**.

```csharp
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Mvvm.Core.Toolkit;

/// <summary>
/// El MISMO ViewModel con CommunityToolkit.Mvvm: los atributos hacen que un generador de código escriba
/// las propiedades, los avisos y los comandos (lo que arriba escribimos a mano).
/// </summary>
public sealed partial class TareasViewModelToolkit(ITareasApi api) : ObservableObject
{
    public ObservableCollection<Tarea> Tareas { get; } = [];

    [ObservableProperty]                                        // genera la propiedad pública «NuevoTitulo» y su aviso de cambio
    [NotifyCanExecuteChangedFor(nameof(CrearCommand))]          // y reevalúa el botón «Crear»
    [NotifyPropertyChangedFor(nameof(PuedeCrear))]              // y avisa también a la propiedad calculada
    private string _nuevoTitulo = "";

    [ObservableProperty] private string? _error;
    [ObservableProperty] private bool _ocupado;

    public bool PuedeCrear => NuevoTitulo.Trim().Length >= 3;
    public int Pendientes => Tareas.Count(t => !t.Hecha);

    [RelayCommand]                                              // genera «CargarCommand» (un IAsyncRelayCommand)
    private async Task CargarAsync()
    {
        Ocupado = true; Error = null;
        try
        {
            var lista = await api.ListarAsync();
            Tareas.Clear();
            foreach (var t in lista) Tareas.Add(t);
            OnPropertyChanged(nameof(Pendientes));
        }
        catch (Exception ex) { Error = ex.Message; }
        finally { Ocupado = false; }
    }

    [RelayCommand(CanExecute = nameof(PuedeCrear))]             // genera «CrearCommand» con su condición
    private async Task CrearAsync()
    {
        try
        {
            Tareas.Add(await api.CrearAsync(NuevoTitulo.Trim()));
            NuevoTitulo = "";
            OnPropertyChanged(nameof(Pendientes));
        }
        catch (Exception ex) { Error = ex.Message; }
    }
}
```

**Qué se ahorra:** `[ObservableProperty] private string _nuevoTitulo` **genera** la propiedad pública `NuevoTitulo` con su aviso; `[NotifyCanExecuteChangedFor]` reevalúa el botón; `[RelayCommand]` convierte `CargarAsync` en `CargarCommand`. El comportamiento **es el mismo**, y las pruebas lo confirman (siguiente sección).

> **Cuándo qué:** aprende **a mano** (para saber qué pasa debajo); trabaja **con el Toolkit** (menos código, menos errores de nombre).

### Código 3: la Vista (XAML) y la composición

`MainWindow.xaml`

```xml
<Window x:Class="Mvvm.Wpf.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vm="clr-namespace:Mvvm.Core.Toolkit;assembly=Mvvm.Core"
        mc:Ignorable="d" xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        d:DataContext="{d:DesignInstance Type=vm:TareasViewModelToolkit}"
        Title="TaskFlow (escritorio)" Height="480" Width="560" MinWidth="420">
    <!-- DataContext = el ViewModel. Cada {Binding X} lee o escribe la propiedad X del ViewModel. -->
    <Grid Margin="16">
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>

        <TextBlock Grid.Row="0" FontSize="22" FontWeight="SemiBold" Margin="0,0,0,12">
            <Run Text="Pendientes: "/><Run Text="{Binding Pendientes, Mode=OneWay}"/>
        </TextBlock>

        <!-- Nueva tarea: el texto se enlaza en dos sentidos y se actualiza en cada tecla -->
        <DockPanel Grid.Row="1" Margin="0,0,0,12">
            <Button DockPanel.Dock="Right" Content="Añadir" Margin="8,0,0,0" Padding="14,4"
                    Command="{Binding CrearCommand}"/>
            <TextBox Text="{Binding NuevoTitulo, UpdateSourceTrigger=PropertyChanged}" Padding="6" />
        </DockPanel>

        <!-- La lista se rellena sola cuando cambia la ObservableCollection -->
        <ListView Grid.Row="2" ItemsSource="{Binding Tareas}">
            <ListView.ItemTemplate>
                <DataTemplate>
                    <DockPanel LastChildFill="True">
                        <Button DockPanel.Dock="Right" Content="Completar" Padding="10,2"
                                Command="{Binding DataContext.CompletarCommand, RelativeSource={RelativeSource AncestorType=ListView}}"
                                CommandParameter="{Binding}"/>
                        <TextBlock Text="{Binding Titulo}" VerticalAlignment="Center"/>
                    </DockPanel>
                </DataTemplate>
            </ListView.ItemTemplate>
        </ListView>

        <StackPanel Grid.Row="3" Margin="0,12,0,0">
            <ProgressBar IsIndeterminate="True" Height="4" Visibility="{Binding Ocupado, Converter={StaticResource BoolAVisibilidad}}"/>
            <TextBlock Text="{Binding Error}" Foreground="Firebrick" TextWrapping="Wrap"/>
            <Button Content="Recargar" HorizontalAlignment="Left" Margin="0,8,0,0" Padding="12,4" Command="{Binding CargarCommand}"/>
        </StackPanel>
    </Grid>
</Window>
```

**Cómo se lee:**

- **`Text="{Binding NuevoTitulo, UpdateSourceTrigger=PropertyChanged}"`**: enlaza el cuadro de texto con la propiedad del ViewModel **en los dos sentidos**; `UpdateSourceTrigger=PropertyChanged` actualiza **en cada tecla** (por defecto solo al perder el foco, y entonces el botón «Añadir» no se habilitaría hasta salir del campo: un fallo clásico).
- **`Command="{Binding CrearCommand}"`**: el botón se habilita y deshabilita **solo**, según `CanExecute`.
- **`ItemsSource="{Binding Tareas}"` + `DataTemplate`**: cómo se dibuja **cada elemento** de la colección.
- **`RelativeSource={RelativeSource AncestorType=ListView}`**: dentro de una plantilla de fila el `DataContext` es la **tarea**, no el ViewModel; para llegar al comando hay que «subir» hasta la lista.
- **`d:DataContext=…`**: solo para el diseñador de Visual Studio.

`MainWindow.xaml.cs`, `App.xaml`, `App.xaml.cs` (la **raíz de composición**: el único sitio donde se conectan las piezas) y `HttpTareasApi.cs` (la implementación real):

```csharp
using System.Windows;
using Mvvm.Core.Toolkit;

namespace Mvvm.Wpf;

public partial class MainWindow : Window
{
    // El code-behind queda casi vacío: solo recibe el ViewModel y lo pone como DataContext.
    public MainWindow(TareasViewModelToolkit viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        Loaded += (_, _) => viewModel.CargarCommand.Execute(null);      // carga al abrirse la ventana
    }
}
```

```csharp
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Mvvm.Core;
using Mvvm.Core.Toolkit;

namespace Mvvm.Wpf;

public partial class App : Application
{
    private ServiceProvider? _servicios;

    // Raíz de composición: el ÚNICO sitio donde se conectan las piezas (como Program.cs en la API)
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var servicios = new ServiceCollection();
        servicios.AddHttpClient<ITareasApi, HttpTareasApi>(c =>
        {
            c.BaseAddress = new Uri("http://localhost:5099/");
            // La API exige un token. Para este ejemplo se toma de una variable de entorno (una pantalla de login sería el siguiente paso).
            string? token = Environment.GetEnvironmentVariable("TASKFLOW_TOKEN");
            if (token is not null) c.DefaultRequestHeaders.Authorization = new("Bearer", token);
        });
        servicios.AddTransient<TareasViewModelToolkit>();
        servicios.AddTransient<MainWindow>();
        _servicios = servicios.BuildServiceProvider();

        _servicios.GetRequiredService<MainWindow>().Show();
    }

    protected override void OnExit(ExitEventArgs e) { _servicios?.Dispose(); base.OnExit(e); }
}
```

```csharp
using System.Net.Http;
using System.Net.Http.Json;
using Mvvm.Core;

namespace Mvvm.Wpf;

/// <summary>
/// La implementación REAL de ITareasApi: habla con la API de TaskFlow (lecciones 47–50).
/// Traduce entre el JSON de la API (title/done) y el modelo del ViewModel (Titulo/Hecha).
/// </summary>
public sealed class HttpTareasApi(HttpClient http) : ITareasApi
{
    private sealed record TareaDto(int Id, string Title, bool Done);

    public async Task<IReadOnlyList<Tarea>> ListarAsync(CancellationToken ct = default) =>
        (await http.GetFromJsonAsync<List<TareaDto>>("tasks", ct) ?? []).Select(Convertir).ToList();

    public async Task<Tarea> CrearAsync(string titulo, CancellationToken ct = default)
    {
        var r = await http.PostAsJsonAsync("tasks", new { title = titulo }, ct);
        r.EnsureSuccessStatusCode();
        return Convertir((await r.Content.ReadFromJsonAsync<TareaDto>(ct))!);
    }

    public async Task CompletarAsync(int id, CancellationToken ct = default) =>
        (await http.PostAsync($"tasks/{id}/done", null, ct)).EnsureSuccessStatusCode();

    private static Tarea Convertir(TareaDto d) => new(d.Id, d.Title, d.Done);
}
```

**Detalles que importan:**

- **El code-behind** (`MainWindow.xaml.cs`) queda **casi vacío**: solo recibe el ViewModel. Si lo vas llenando de lógica, has abandonado MVVM.
- **`HttpTareasApi` traduce** entre el JSON de la API (`title`, `done`) y el modelo del ViewModel (`Titulo`, `Hecha`): **la API y la pantalla pueden evolucionar por separado**.
- **El token JWT** se toma de la variable de entorno `TASKFLOW_TOKEN`: una pantalla de inicio de sesión sería el paso siguiente (no se hizo).
- **Hilos en WPF:** solo el hilo de la interfaz puede tocar controles. Con `async/await` la continuación vuelve a ese hilo sola. **Si usaras `Task.Run` o un temporizador y tocaras `Tareas` desde otro hilo**, saltaría una excepción: se resuelve con `Dispatcher.InvokeAsync` (lección 44, el contexto de sincronización).

### Resultado real

```text
$ dotnet build src/Mvvm.Wpf    (desde Linux; el proyecto es net10.0-windows con EnableWindowsTargeting=true)
    0 Error(s)
    0 Warning(s)
  Mvvm.Wpf -> /tmp/claude-0/-home-user-claude/ddd43a70-8f87-5490-9d3f-755ecbba3013/scratchpad/curso6/Mvvm/src/Mvvm.Wpf/bin/Debug/net10.0-windows/Mvvm.Wpf.dll

$ dotnet test   (toda la solución: el ViewModel se prueba en Linux)
Passed!  - Failed:     0, Passed:    17, Skipped:     0, Total:    17, Duration: 142 ms
```

**Y las pruebas del ViewModel** (se ejecutan en Linux, sin ventana):

```text
Passed Mvvm.Tests.ViewModelManualTests.BotonCrear_AvisaCuandoCambiaSuEstado
Passed Mvvm.Tests.ViewModelToolkitTests.Crear_ConElComandoGenerado_FuncionaDeExtremoAExtremo
Passed Mvvm.Tests.ViewModelToolkitTests.ElComandoGenerado_RespetaSuCondicion
Passed Mvvm.Tests.ViewModelToolkitTests.LaPropiedadGeneradaAvisaDeSuCambio_YDeLaCalculada
Passed Mvvm.Tests.ViewModelManualTests.MientrasCarga_EstaOcupado_YElBotonNoSePuedePulsarDeNuevo
Passed Mvvm.Tests.ViewModelToolkitTests.SiLaApiFalla_ElErrorSeMuestra
Passed Mvvm.Tests.ViewModelManualTests.Completar_ReemplazaLaTareaPorSuCopiaHecha
Passed Mvvm.Tests.ViewModelManualTests.Completar_NoSePuedeEnUnaTareaYaHecha [< 1 ms]
Passed Mvvm.Tests.ViewModelManualTests.BotonCrear_SoloSeHabilitaConTituloValido(titulo: "abc", esperado: True)
Passed Mvvm.Tests.ViewModelManualTests.BotonCrear_SoloSeHabilitaConTituloValido(titulo: "  ab  ", esperado: False) [< 1 ms]
Passed Mvvm.Tests.ViewModelManualTests.BotonCrear_SoloSeHabilitaConTituloValido(titulo: "ab", esperado: False) [< 1 ms]
Passed Mvvm.Tests.ViewModelManualTests.BotonCrear_SoloSeHabilitaConTituloValido(titulo: "", esperado: False) [< 1 ms]
Passed Mvvm.Tests.ViewModelManualTests.Cargar_LlenaLaColeccionYAvisaDeCadaElemento
Passed Mvvm.Tests.ViewModelManualTests.SiLaApiFalla_ElErrorSeMuestra_YElBotonVuelveAHabilitarse
Passed Mvvm.Tests.ViewModelManualTests.AsignarElMismoValor_NoAvisa [< 1 ms]
Passed Mvvm.Tests.ViewModelManualTests.CambiarUnaPropiedad_AvisaALaInterfaz [< 1 ms]
Passed Mvvm.Tests.ViewModelManualTests.Crear_AnadeLaTareaYLimpiaElCampo [< 1 ms]
```

### ¿Estas pruebas detectan algo? Mutación manual

Una prueba que **nunca ha fallado** no demuestra nada. Se rompió **a propósito** el código de cuatro maneras y se comprobó que **cada una** la detecta **una prueba distinta**:

```text
$ MUTACIÓN: (ninguna: el código original)
Passed!  - Failed:     0, Passed:    17, Skipped:     0, Total:    17, Duration: 162 ms

$ MUTACIÓN: se quita CrearCommand.NotificarCambio() del setter de NuevoTitulo
Failed Mvvm.Tests.ViewModelManualTests.BotonCrear_AvisaCuandoCambiaSuEstado
Failed!  - Failed:     1, Passed:    16, Skipped:     0, Total:    17, Duration: 172 ms

$ MUTACIÓN: se quita el aviso de la propiedad calculada PuedeCrear
Failed Mvvm.Tests.ViewModelManualTests.CambiarUnaPropiedad_AvisaALaInterfaz
Failed!  - Failed:     1, Passed:    16, Skipped:     0, Total:    17, Duration: 181 ms

$ MUTACIÓN: AsyncRelayCommand deja de bloquearse mientras se ejecuta (el doble clic pasaría)
Failed Mvvm.Tests.ViewModelManualTests.MientrasCarga_EstaOcupado_YElBotonNoSePuedePulsarDeNuevo
Failed!  - Failed:     1, Passed:    16, Skipped:     0, Total:    17, Duration: 169 ms

$ MUTACIÓN: Set() avisa SIEMPRE, aunque el valor no cambie
Failed Mvvm.Tests.ViewModelManualTests.AsignarElMismoValor_NoAvisa
Failed!  - Failed:     1, Passed:    16, Skipped:     0, Total:    17, Duration: 185 ms
```

| Mutación (error introducido) | Qué hubiera pasado en pantalla | Prueba que la detectó |
|---|---|---|
| Se quita `CrearCommand.NotificarCambio()` | El botón «Añadir» **se quedaría deshabilitado** aunque escribas | `BotonCrear_AvisaCuandoCambiaSuEstado` |
| Se quita el aviso de `PuedeCrear` | Un indicador visual dependiente de `PuedeCrear` no se actualizaría | `CambiarUnaPropiedad_AvisaALaInterfaz` |
| El comando async deja de bloquearse | **El doble clic lanzaría dos cargas** | `MientrasCarga_EstaOcupado_YElBotonNoSePuedePulsarDeNuevo` |
| `Set()` avisa siempre | Repintados innecesarios y **bucles de actualización** | `AsignarElMismoValor_NoAvisa` |

**Esto es lo que ninguna comprobación visual da en un minuto**, y es la razón de ser de MVVM. *Las herramientas de pruebas de mutación automáticas (por ejemplo, Stryker.NET) hacen esto a escala; aquí se hizo a mano para ver la idea.*

### Un tropiezo del proceso, por honestidad

Al restaurar el código tras las mutaciones, mi primer intento de «deshacer» **dejó el código roto** (el `Set` sin su comparación) y una prueba fallaba. **Lo vi porque volví a ejecutar las pruebas después de restaurar**: hazlo siempre. En un repositorio real, `git checkout -- archivo` es la forma segura de deshacer una mutación.

### Errores típicos

| Síntoma | Causa | Solución |
|---|---|---|
| La pantalla no se actualiza | Falta `OnPropertyChanged` (o nombre mal escrito) | `[CallerMemberName]` o el Toolkit |
| El botón no se habilita al escribir | Falta `CanExecuteChanged` o `UpdateSourceTrigger=PropertyChanged` | Notifica el cambio; actualiza en cada tecla |
| La lista no se refresca | Se usa `List<T>` | `ObservableCollection<T>` |
| `The calling thread cannot access this object` | Se toca la interfaz desde otro hilo | `await` sin `ConfigureAwait(false)` en el ViewModel, o `Dispatcher` |
| Binding que «no hace nada» | Ruta mal escrita: **no hay error de compilación** | Mira la ventana *Salida* (errores de `BindingExpression`) |
| Lógica en el code-behind | Atajo | Muévela al ViewModel |
| Doble clic dispara dos veces | Comando sin bloqueo | `AsyncRelayCommand` que se deshabilita al ejecutarse |
| El ViewModel referencia `MessageBox` o `Window` | Mezcla de capas | Una interfaz (`IDialogService`) que se pueda sustituir |
| «Compila, luego funciona» | WPF compilado en Linux | **Ejecútalo en Windows** antes de fiarte |

> **Ejercicio 53.1:** añade `FiltroTexto` al ViewModel y una propiedad `TareasFiltradas`. Escribe **primero** la prueba.
>
> **Ejercicio 53.2:** introduce una mutación propia (por ejemplo, que `Crear` no limpie el campo) y comprueba qué prueba la detecta; si ninguna, escríbela.
>
> **Ejercicio 53.3 (necesita Windows):** ejecuta la ventana, abre la ventana *Salida* de Visual Studio y busca errores de `BindingExpression`.

---

## Resumen de la Parte IX

| Si quieres… | Usa… |
|---|---|
| Escritorio solo Windows con enlace de datos potente | WPF |
| Separar pantalla y lógica | **MVVM** |
| Avisar a la pantalla de que algo cambió | `INotifyPropertyChanged` / `[ObservableProperty]` |
| Listas que se actualizan solas | `ObservableCollection<T>` |
| Botones sin código en la ventana | `ICommand` / `[RelayCommand]` |
| Evitar el doble clic | Comando asíncrono que se bloquea mientras corre |
| Probar la lógica de la pantalla | xUnit sobre el ViewModel |
| Saber si tus pruebas valen | Romper el código a propósito |
| Comprobar la parte visual | **Ejecutarla** (en Windows) |

**Sigue con la Parte X:** C# en **Unity**, el complemento para videojuegos.
