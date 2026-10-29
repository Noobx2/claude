using TaskFlow.Application;
using TaskFlow.Domain;

namespace TaskFlow.Tests;

public class TaskServiceTests
{
    [Fact]
    public void Add_AssignsIdTrimsTitleAndStartsPending()
    {
        var service = TestFactory.NewService(out _);

        var task = service.Add(new NewTask("  Comprar pan  ", "  ", Priority.Medium, null));

        Assert.Equal(1, task.Id);
        Assert.Equal("Comprar pan", task.Title);
        Assert.Null(task.Description);                    // una descripción en blanco se guarda como "sin descripción"
        Assert.False(task.Done);
        Assert.Equal(TestFactory.Today, task.CreatedAt);
    }

    [Theory]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("ab")]
    public void Add_RejectsShortTitles(string title)
    {
        var service = TestFactory.NewService(out _);
        var error = Assert.Throws<ValidationException>(() => service.Add(new NewTask(title, null, Priority.Low, null)));
        Assert.Contains("3 caracteres", error.Message);
    }

    [Fact]
    public void Add_RejectsDueDateInThePast()
    {
        var service = TestFactory.NewService(out _);
        Assert.Throws<ValidationException>(() => service.Add(new NewTask("Tarea", null, Priority.Low, new DateOnly(2026, 3, 9))));
        service.Add(new NewTask("Tarea", null, Priority.Low, new DateOnly(2026, 3, 10)));    // hoy sí vale
    }

    [Fact]
    public void List_OrdersPendingFirstThenByPriority()
    {
        var service = TestFactory.NewService(out _);
        service.Add(new NewTask("Baja", null, Priority.Low, null));
        service.Add(new NewTask("Alta", null, Priority.High, null));
        var hecha = service.Add(new NewTask("Hecha alta", null, Priority.High, null));
        service.SetDone(hecha.Id, true);

        var titles = service.List().Select(t => t.Title);

        Assert.Equal(["Alta", "Baja", "Hecha alta"], titles);
    }

    [Fact]
    public void List_FiltersByTextAndOverdue()
    {
        var service = TestFactory.NewService(out var clock);
        service.Add(new NewTask("Pagar luz", null, Priority.Low, new DateOnly(2026, 3, 11)));
        service.Add(new NewTask("Llamar al banco", "por la luz", Priority.Low, null));
        Assert.Equal(2, service.List(new TaskFilter(Text: "LUZ")).Count);        // mayúsculas y descripción incluidas

        clock.Now = clock.Now.AddDays(5);                                        // el tiempo pasa
        var overdue = service.List(new TaskFilter(OnlyOverdue: true));
        Assert.Single(overdue);
        Assert.Equal("Pagar luz", overdue[0].Title);
    }

    [Fact]
    public void Update_ChangesOnlyWhatIsGiven()
    {
        var service = TestFactory.NewService(out _);
        var task = service.Add(new NewTask("Original", "nota", Priority.Low, null));

        var updated = service.Update(task.Id, new TaskChanges(Priority: Priority.High));

        Assert.Equal(Priority.High, updated.Priority);
        Assert.Equal("Original", updated.Title);
        Assert.Equal("nota", updated.Description);
    }

    [Fact]
    public void MissingTask_ThrowsNotFound()
    {
        var service = TestFactory.NewService(out _);
        Assert.Throws<NotFoundException>(() => service.Get(99));
        Assert.Throws<NotFoundException>(() => service.Delete(99));
        Assert.Throws<NotFoundException>(() => service.SetDone(99, true));
    }

    [Fact]
    public void Stats_CountsDoneAndOverdue()
    {
        var service = TestFactory.NewService(out var clock);
        var a = service.Add(new NewTask("Primera", null, Priority.Low, new DateOnly(2026, 3, 11)));
        service.Add(new NewTask("Segunda", null, Priority.Low, null));
        service.SetDone(a.Id, true);
        clock.Now = clock.Now.AddDays(30);

        Assert.Equal(new TaskStats(2, 1, 1, 0), service.GetStats());           // la vencida está hecha: no cuenta
    }
}
