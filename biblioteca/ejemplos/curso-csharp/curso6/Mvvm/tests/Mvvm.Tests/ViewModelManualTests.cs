using System.Collections.Specialized;
using System.ComponentModel;
using Mvvm.Core;
using Mvvm.Core.Manual;

namespace Mvvm.Tests;

public class ViewModelManualTests
{
    private readonly ApiFalsa _api = new();
    private TareasViewModel Crear() => new(_api);

    [Fact]
    public void CambiarUnaPropiedad_AvisaALaInterfaz()
    {
        var vm = Crear(); var avisadas = new List<string?>();
        vm.PropertyChanged += (_, e) => avisadas.Add(e.PropertyName);

        vm.NuevoTitulo = "Comprar pan";

        Assert.Contains(nameof(vm.NuevoTitulo), avisadas);
        Assert.Contains(nameof(vm.PuedeCrear), avisadas);            // la propiedad calculada también
    }

    [Fact]
    public void AsignarElMismoValor_NoAvisa()
    {
        var vm = Crear(); vm.NuevoTitulo = "Igual";
        int avisos = 0; vm.PropertyChanged += (_, _) => avisos++;

        vm.NuevoTitulo = "Igual";

        Assert.Equal(0, avisos);
    }

    [Theory]
    [InlineData("", false)]
    [InlineData("ab", false)]
    [InlineData("  ab  ", false)]       // los espacios no cuentan
    [InlineData("abc", true)]
    public void BotonCrear_SoloSeHabilitaConTituloValido(string titulo, bool esperado)
    {
        var vm = Crear();
        vm.NuevoTitulo = titulo;
        Assert.Equal(esperado, vm.CrearCommand.CanExecute(null));
    }

    [Fact]
    public void BotonCrear_AvisaCuandoCambiaSuEstado()
    {
        var vm = Crear(); int veces = 0;
        vm.CrearCommand.CanExecuteChanged += (_, _) => veces++;
        vm.NuevoTitulo = "Algo válido";
        Assert.True(veces > 0, "sin este aviso el botón se quedaría deshabilitado en pantalla");
    }

    [Fact]
    public async Task Cargar_LlenaLaColeccionYAvisaDeCadaElemento()
    {
        _api.Datos.AddRange([new Tarea(1, "A", false), new Tarea(2, "B", true)]);
        var vm = Crear(); var cambios = new List<NotifyCollectionChangedAction>();
        ((INotifyCollectionChanged)vm.Tareas).CollectionChanged += (_, e) => cambios.Add(e.Action);

        await vm.CargarCommand.EjecutarAsync();

        Assert.Equal(2, vm.Tareas.Count);
        Assert.Equal(1, vm.Pendientes);
        Assert.Contains(NotifyCollectionChangedAction.Add, cambios);
    }

    [Fact]
    public async Task MientrasCarga_EstaOcupado_YElBotonNoSePuedePulsarDeNuevo()
    {
        _api.Puerta = new TaskCompletionSource();              // la API se queda «pensando»
        var vm = Crear();

        var carga = vm.CargarCommand.EjecutarAsync();

        Assert.True(vm.Ocupado);
        Assert.False(vm.CargarCommand.CanExecute(null));       // protege contra el doble clic
        await vm.CargarCommand.EjecutarAsync();                // un segundo intento se ignora
        Assert.Equal(1, _api.Llamadas);

        _api.Puerta.SetResult(); await carga;
        Assert.False(vm.Ocupado);
        Assert.True(vm.CargarCommand.CanExecute(null));
    }

    [Fact]
    public async Task Crear_AnadeLaTareaYLimpiaElCampo()
    {
        var vm = Crear(); vm.NuevoTitulo = "  Estudiar MVVM  ";

        await vm.CrearCommand.EjecutarAsync();

        Assert.Equal("Estudiar MVVM", vm.Tareas.Single().Titulo);   // recortado
        Assert.Equal("", vm.NuevoTitulo);
        Assert.False(vm.CrearCommand.CanExecute(null));             // el campo quedó vacío
    }

    [Fact]
    public async Task SiLaApiFalla_ElErrorSeMuestra_YElBotonVuelveAHabilitarse()
    {
        _api.FallarCon = new HttpRequestException("Sin conexión con el servidor");
        var vm = Crear(); vm.NuevoTitulo = "Tarea válida";

        await vm.CrearCommand.EjecutarAsync();                      // NO lanza: el comando captura la excepción

        Assert.Equal("Sin conexión con el servidor", vm.Error);
        Assert.Empty(vm.Tareas);
        Assert.True(vm.CrearCommand.CanExecute(null));              // se puede reintentar
    }

    [Fact]
    public void Completar_NoSePuedeEnUnaTareaYaHecha()
    {
        var vm = Crear();
        Assert.True(vm.CompletarCommand.CanExecute(new Tarea(1, "x", false)));
        Assert.False(vm.CompletarCommand.CanExecute(new Tarea(1, "x", true)));
        Assert.False(vm.CompletarCommand.CanExecute(null));
    }

    [Fact]
    public async Task Completar_ReemplazaLaTareaPorSuCopiaHecha()
    {
        _api.Datos.Add(new Tarea(1, "A", false));
        var vm = Crear(); await vm.CargarCommand.EjecutarAsync();

        vm.CompletarCommand.Execute(vm.Tareas[0]);
        await Task.Delay(50);                                       // CompletarAsync no se espera; dejamos que termine

        Assert.True(vm.Tareas[0].Hecha);
        Assert.Equal(0, vm.Pendientes);
    }
}
