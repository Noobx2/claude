# Un Core, muchos clientes

Mismo `Inventory.Core` (C#) reutilizado, sin modificarlo, por:

| Proyecto | Qué es | Cómo se ejecuta |
|---|---|---|
| `Inventory.Console` | Consola (C#) | `dotnet run --project src/Inventory.Console` |
| `Inventory.WinForms` | Ventana clásica (solo Windows) | Visual Studio, F5 |
| `Inventory.Wpf` | Ventana con XAML y MVVM (solo Windows) | Visual Studio, F5 |
| `Inventory.Api` + `web/` | API ASP.NET Core + interfaz Vite | ver abajo |
| `Inventory.FSharp` | Cliente en F# | `dotnet run --project src/Inventory.FSharp` |
| `Inventory.VB` | Cliente en Visual Basic | `dotnet run --project src/Inventory.VB` |
| `native/` | P/Invoke (API de Windows / libc) | `dotnet run --project native/Native` |

## API + Vite
```bash
cd web && npm install && npx vite build          # deja la interfaz en src/Inventory.Api/wwwroot
cd .. && dotnet run --project src/Inventory.Api --urls http://localhost:5080
```
En desarrollo: la API en :5080 y `cd web && npx vite` en :5173 (proxy de `/api`).

Comprobado en Linux con .NET 10 y Vite 8.3.5: todo salvo WinForms y WPF (solo compilan) y la rama Windows de `native/`.
