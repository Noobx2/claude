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
