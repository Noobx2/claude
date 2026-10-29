using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Inventory.Wpf;

/// <summary>
/// ViewModel (patrón MVVM): lo que la ventana MUESTRA y puede hacer, sin conocer ningún control.
/// La vista (XAML) se enlaza a estas propiedades con {Binding ...}.
/// </summary>
public sealed class ProductsViewModel : INotifyPropertyChanged
{
    private readonly ProductRepository _repository = new();
    private string _query = "";

    public ProductsViewModel()
    {
        _repository.Add("Teclado mecánico", "Periféricos", 59.90m, 25);
        _repository.Add("Ratón inalámbrico", "Periféricos", 24.50m, 60);
        _repository.Add("Monitor 24\"", "Monitores", 139.00m, 10);
        Refresh();
    }

    // ObservableCollection avisa a la tabla cuando cambia: no hay que redibujar a mano
    public ObservableCollection<Product> Products { get; } = [];

    public string Query
    {
        get => _query;
        set { _query = value; OnPropertyChanged(); Refresh(); }
    }

    public string SummaryText => $"{Products.Count} productos";

    private void Refresh()
    {
        Products.Clear();
        foreach (var p in string.IsNullOrWhiteSpace(_query) ? _repository.FindAll() : _repository.Search(_query))
            Products.Add(p);
        OnPropertyChanged(nameof(SummaryText));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
