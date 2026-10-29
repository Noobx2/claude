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
