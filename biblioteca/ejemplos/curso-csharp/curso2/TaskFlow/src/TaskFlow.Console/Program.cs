using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Application;
using TaskFlow.Console;
using TaskFlow.Infrastructure;

// "Raíz de composición": el ÚNICO sitio que conoce todas las piezas y las conecta.
// Para cambiar el almacenamiento (JSON -> base de datos) se cambia solo una línea de aquí.
string dataFile = Environment.GetEnvironmentVariable("TASKFLOW_FILE") ?? "tasks.json";

var services = new ServiceCollection();
services.AddSingleton<IClock, SystemClock>();
services.AddSingleton<ITaskRepository>(_ => new JsonTaskRepository(dataFile));
services.AddSingleton<IConsoleIO, SystemConsoleIO>();
services.AddSingleton<TaskService>();
services.AddSingleton<ConsoleApp>();

using var provider = services.BuildServiceProvider();
provider.GetRequiredService<ConsoleApp>().Run();

file sealed class SystemClock : IClock                     // "file": visible solo en este archivo
{
    public DateTime Now => DateTime.Now;
}
