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
