using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TaskFlow.Web;
using TaskFlow.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// La URL de la API sale de wwwroot/appsettings.json (no está escrita en el código)
string apiUrl = builder.Configuration["ApiBaseUrl"] ?? throw new InvalidOperationException("Falta ApiBaseUrl en wwwroot/appsettings.json");
builder.Services.AddScoped<Sesion>();
builder.Services.AddScoped(sp => new ApiClient(new HttpClient { BaseAddress = new Uri(apiUrl) }, sp.GetRequiredService<Sesion>()));

await builder.Build().RunAsync();
