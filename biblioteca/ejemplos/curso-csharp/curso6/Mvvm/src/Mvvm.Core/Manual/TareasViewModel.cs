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
