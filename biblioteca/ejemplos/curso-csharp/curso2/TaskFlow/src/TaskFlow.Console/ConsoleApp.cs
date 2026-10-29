using TaskFlow.Application;
using TaskFlow.Domain;

namespace TaskFlow.Console;

/// <summary>La interfaz de consola: pregunta, llama al servicio y muestra el resultado. No contiene reglas del negocio.</summary>
public sealed class ConsoleApp(TaskService service, IConsoleIO io, IClock clock)
{
    private readonly Prompts _ask = new(io);

    public void Run()
    {
        try { Loop(); }
        catch (EndOfInputException) { io.WriteLine(); io.WriteLine("(fin de la entrada)"); }   // Ctrl+D o archivo terminado: salir sin error
    }

    private void Loop()
    {
        while (true)
        {
            io.WriteLine();
            io.WriteLine("=== TASKFLOW ===");
            io.WriteLine("1) Nueva  2) Listar  3) Detalle  4) Editar  5) Completar  6) Borrar  7) Buscar  8) Resumen  0) Salir");
            int option = _ask.Int("Opción", 0, 8);
            if (option == 0) { io.WriteLine("Hasta luego."); return; }

            try
            {
                switch (option)
                {
                    case 1: Create(); break;
                    case 2: Show(service.List()); break;
                    case 3: Detail(); break;
                    case 4: Edit(); break;
                    case 5: Complete(); break;
                    case 6: Delete(); break;
                    case 7: Show(service.List(new TaskFilter(Text: _ask.Text("Texto a buscar", required: true)))); break;
                    case 8: Summary(); break;
                }
            }
            catch (ValidationException e) { io.WriteLine($"  ✗ {e.Message}"); }     // errores del negocio: se explican al usuario
            catch (NotFoundException e) { io.WriteLine($"  ✗ {e.Message}"); }
        }
    }

    private void Create()
    {
        var input = new NewTask(
            _ask.Text("Título", required: true),
            _ask.Text("Descripción (opcional)"),
            _ask.Priority("Prioridad"),
            _ask.Date("Fecha límite"));
        TaskItem task = service.Add(input);                   // si algo no cumple las reglas, lanza ValidationException
        io.WriteLine($"  ✓ Tarea {task.Id} creada.");
    }

    private void Detail()
    {
        TaskItem t = service.Get(_ask.Int("Id", 1, int.MaxValue));
        io.WriteLine($"  Id:          {t.Id}");
        io.WriteLine($"  Título:      {t.Title}");
        io.WriteLine($"  Descripción: {t.Description ?? "(sin descripción)"}");
        io.WriteLine($"  Prioridad:   {Label(t.Priority)}");
        io.WriteLine($"  Límite:      {(t.DueDate is { } d ? d.ToString("dd/MM/yyyy") : "(sin fecha)")}{(t.IsOverdue(clock.Today) ? "  ¡VENCIDA!" : "")}");
        io.WriteLine($"  Estado:      {(t.Done ? "hecha" : "pendiente")}");
        io.WriteLine($"  Creada:      {t.CreatedAt:dd/MM/yyyy HH:mm}");
    }

    private void Edit()
    {
        TaskItem current = service.Get(_ask.Int("Id a editar", 1, int.MaxValue));
        io.WriteLine("  (Enter conserva el valor actual)");
        var changes = new TaskChanges(
            _ask.Text("Título", current.Title),
            _ask.Text("Descripción", current.Description ?? ""),
            _ask.Priority("Prioridad", current.Priority),
            _ask.Date("Fecha límite", current.DueDate));
        service.Update(current.Id, changes);
        io.WriteLine("  ✓ Tarea actualizada.");
    }

    private void Complete()
    {
        TaskItem t = service.SetDone(_ask.Int("Id a completar", 1, int.MaxValue), true);
        io.WriteLine($"  ✓ «{t.Title}» marcada como hecha.");
    }

    private void Delete()
    {
        TaskItem t = service.Get(_ask.Int("Id a borrar", 1, int.MaxValue));
        if (_ask.YesNo($"¿Borrar «{t.Title}»?")) { service.Delete(t.Id); io.WriteLine("  ✓ Borrada."); }
        else io.WriteLine("  Cancelado.");
    }

    private void Summary()
    {
        TaskStats s = service.GetStats();
        io.WriteLine($"  Total {s.Total} · hechas {s.Done} · pendientes {s.Pending} · vencidas {s.Overdue}");
    }

    private void Show(IReadOnlyList<TaskItem> tasks)
    {
        if (tasks.Count == 0) { io.WriteLine("  (sin resultados)"); return; }
        io.WriteLine($"  {"Id",3}  {"",1} {"Título",-26} {"Prio",-6} {"Límite",-10}");
        foreach (var t in tasks)
            io.WriteLine($"  {t.Id,3}  {(t.Done ? "✓" : " ")} {Truncate(t.Title, 26),-26} {Label(t.Priority),-6} {(t.DueDate is { } d ? d.ToString("dd/MM/yyyy") : ""),-10}{(t.IsOverdue(clock.Today) ? " ¡vencida!" : "")}");
        io.WriteLine($"  {tasks.Count} tarea(s)");
    }

    private static string Label(Priority p) => p switch { Priority.High => "Alta", Priority.Medium => "Media", _ => "Baja" };
    private static string Truncate(string text, int max) => text.Length <= max ? text : text[..(max - 1)] + "…";
}
