using System.Text;
using System.Text.Json;
using Microsoft.JSInterop;

namespace TaskFlow.Web.Services;

/// <summary>
/// Quién está conectado. Guarda el token en memoria y en sessionStorage (para sobrevivir a un F5, pero no al cerrar la pestaña).
/// Aviso de seguridad: ni sessionStorage ni localStorage son inmunes a XSS; cada opción tiene su compromiso.
/// </summary>
public sealed class Sesion(IJSRuntime js)
{
    private const string Clave = "taskflow.token";
    public string? Token { get; private set; }
    public string? Usuario { get; private set; }
    public string? Rol { get; private set; }
    public bool EstaAutenticado => Token is not null;
    public bool EsAdmin => Rol == "Admin";
    public event Action? Cambio;                       // los componentes se suscriben para repintarse

    public async Task RestaurarAsync()
    {
        string? guardado = await js.InvokeAsync<string?>("sessionStorage.getItem", Clave);
        if (guardado is not null) Aplicar(guardado);
    }

    public async Task IniciarAsync(string token) { Aplicar(token); await js.InvokeVoidAsync("sessionStorage.setItem", Clave, token); }
    public async Task CerrarAsync() { Token = Usuario = Rol = null; await js.InvokeVoidAsync("sessionStorage.removeItem", Clave); Cambio?.Invoke(); }

    private void Aplicar(string token)
    {
        // Leer el contenido del JWT en el cliente SOLO sirve para mostrar el nombre o esconder botones.
        // La seguridad real la impone el servidor: aquí cualquiera podría editar esto desde las herramientas del navegador.
        string contenido = token.Split('.')[1].Replace('-', '+').Replace('_', '/');
        contenido = contenido.PadRight(contenido.Length + (4 - contenido.Length % 4) % 4, '=');
        using var doc = JsonDocument.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(contenido)));
        Token = token;
        Usuario = doc.RootElement.GetProperty("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name").GetString();
        Rol = doc.RootElement.GetProperty("http://schemas.microsoft.com/ws/2008/06/identity/claims/role").GetString();
        Cambio?.Invoke();
    }
}
