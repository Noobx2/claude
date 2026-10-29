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
