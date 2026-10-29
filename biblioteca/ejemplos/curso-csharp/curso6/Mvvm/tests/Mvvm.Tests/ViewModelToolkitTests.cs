using Mvvm.Core.Toolkit;

namespace Mvvm.Tests;

/// <summary>El ViewModel hecho con CommunityToolkit debe comportarse igual que el escrito a mano.</summary>
public class ViewModelToolkitTests
{
    private readonly ApiFalsa _api = new();

    [Fact]
    public void LaPropiedadGeneradaAvisaDeSuCambio_YDeLaCalculada()
    {
        var vm = new TareasViewModelToolkit(_api); var avisadas = new List<string?>();
        vm.PropertyChanged += (_, e) => avisadas.Add(e.PropertyName);

        vm.NuevoTitulo = "Comprar pan";

        Assert.Contains("NuevoTitulo", avisadas);
        Assert.Contains("PuedeCrear", avisadas);
    }

    [Fact]
    public void ElComandoGenerado_RespetaSuCondicion()
    {
        var vm = new TareasViewModelToolkit(_api);
        Assert.False(vm.CrearCommand.CanExecute(null));
        vm.NuevoTitulo = "Algo válido";
        Assert.True(vm.CrearCommand.CanExecute(null));
    }

    [Fact]
    public async Task Crear_ConElComandoGenerado_FuncionaDeExtremoAExtremo()
    {
        var vm = new TareasViewModelToolkit(_api); vm.NuevoTitulo = "  Nueva  ";

        await vm.CrearCommand.ExecuteAsync(null);

        Assert.Equal("Nueva", vm.Tareas.Single().Titulo);
        Assert.Equal("", vm.NuevoTitulo);
    }

    [Fact]
    public async Task SiLaApiFalla_ElErrorSeMuestra()
    {
        _api.FallarCon = new InvalidOperationException("Fallo de prueba");
        var vm = new TareasViewModelToolkit(_api);

        await vm.CargarCommand.ExecuteAsync(null);

        Assert.Equal("Fallo de prueba", vm.Error);
        Assert.False(vm.Ocupado);
    }
}
