using System.Reflection;
using TaskFlow.Application;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Api.Tests;

/// <summary>
/// «Pruebas de arquitectura»: hacen cumplir la regla de dependencias. Si alguien añade por error
/// EF Core al dominio, la prueba falla ANTES de que el diseño se degrade.
/// </summary>
public class ArquitecturaTests
{
    private static string[] Referencias(Assembly a) => a.GetReferencedAssemblies().Select(r => r.Name!).ToArray();

    [Fact]
    public void Core_NoDependeDeInfraestructuraNiDeFrameworks()
    {
        var referencias = Referencias(typeof(TaskService).Assembly);
        Assert.DoesNotContain(referencias, r => r.StartsWith("Microsoft.EntityFrameworkCore"));
        Assert.DoesNotContain(referencias, r => r.StartsWith("Npgsql"));
        Assert.DoesNotContain(referencias, r => r.StartsWith("Microsoft.AspNetCore"));
        Assert.DoesNotContain(referencias, r => r == "TaskFlow.Infrastructure" || r == "TaskFlow.Api");
    }

    [Fact]
    public void ElDetector_SiVeEfCoreDondeSiEstaPresente()      // control: si esto fallara, las pruebas anteriores serían vacías
    {
        Assert.Contains(Referencias(typeof(TaskFlowDbContext).Assembly), r => r.StartsWith("Microsoft.EntityFrameworkCore"));
    }

    [Fact]
    public void Infraestructura_NoDependeDeLaApi()
    {
        Assert.DoesNotContain(Referencias(typeof(TaskFlowDbContext).Assembly), r => r == "TaskFlow.Api");
    }

    [Fact]
    public void Api_ConoceTodasLasCapas_PorqueEsLaRaizDeComposicion()
    {
        var referencias = Referencias(typeof(Program).Assembly);
        Assert.Contains("TaskFlow.Core", referencias);
        Assert.Contains("TaskFlow.Infrastructure", referencias);
    }
}
