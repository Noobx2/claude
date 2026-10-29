using TaskFlow.Console;
using TaskFlow.Infrastructure;

namespace TaskFlow.Tests;

/// <summary>Pruebas de la interfaz: se "teclea" una conversación entera y se comprueba lo que se muestra.</summary>
public class ConsoleAppTests
{
    private static (string Output, FakeClock Clock, InMemoryTaskRepository Repo) Run(params string[] input)
    {
        var repo = new InMemoryTaskRepository();
        var service = TestFactory.NewService(out var clock, repo);
        var io = new FakeConsole(input);
        new ConsoleApp(service, io, clock).Run();
        return (io.Output, clock, repo);
    }

    [Fact]
    public void CreateAndList_ShowsTheTask()
    {
        var (output, _, repo) = Run("1", "Estudiar C#", "", "3", "", "2", "0");   // nueva: título, sin descripción, prioridad alta, sin fecha; listar; salir

        Assert.Single(repo.GetAll());
        Assert.Contains("✓ Tarea 1 creada", output);
        Assert.Contains("Estudiar C#", output);
        Assert.Contains("Alta", output);
    }

    [Fact]
    public void InvalidInput_IsRetriedWithoutCrashing()
    {
        var (output, _, repo) = Run("abc", "9", "1", "", "Título válido", "", "7", "2", "", "0");
        // "abc" y "9" no son opciones; "1" crea; título vacío se rechaza; descripción vacía;
        // prioridad "7" no válida y "2" sí; fecha vacía; "0" sale

        Assert.Contains("Escribe un número entero", output);
        Assert.Contains("Debe estar entre 0 y 8", output);
        Assert.Contains("Es obligatorio", output);
        Assert.Contains("Escribe 1, 2 o 3", output);
        Assert.Single(repo.GetAll());
    }

    [Fact]
    public void EndOfInput_ExitsCleanlyInsteadOfLooping()
    {
        var (output, _, _) = Run("1", "Título");                 // la entrada se acaba a mitad de un formulario

        Assert.Contains("(fin de la entrada)", output);          // sin esta protección el programa preguntaría eternamente
    }

    [Fact]
    public void BusinessErrors_AreShownNotThrown()
    {
        var (output, _, repo) = Run("1", "ab", "", "1", "01/01/2020", "0");        // título de 2 letras: la regla del negocio lo rechaza

        Assert.Contains("✗ El título necesita al menos 3 caracteres.", output);
        Assert.Empty(repo.GetAll());
        Assert.Contains("Hasta luego", output);                                       // y el programa sigue funcionando
    }

    [Fact]
    public void Delete_AsksForConfirmation()
    {
        var (output, _, repo) = Run(
            "1", "Borrar esto", "", "1", "",     // crear
            "6", "1", "n",                        // borrar y decir que no
            "6", "1", "s",                        // borrar y decir que sí
            "0");

        Assert.Contains("Cancelado", output);
        Assert.Contains("✓ Borrada", output);
        Assert.Empty(repo.GetAll());
    }

    [Fact]
    public void Edit_KeepsValuesOnEnter()
    {
        var (output, _, repo) = Run(
            "1", "Original", "nota", "1", "",     // crear
            "4", "1", "", "", "3", "",            // editar: Enter, Enter, prioridad 3, Enter
            "0");

        var task = repo.GetAll().Single();
        Assert.Equal("Original", task.Title);
        Assert.Equal("nota", task.Description);
        Assert.Equal(TaskFlow.Domain.Priority.High, task.Priority);
    }
}
