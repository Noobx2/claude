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
