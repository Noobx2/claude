using System.Net.Http.Headers;
using System.Text.Json;

namespace TaskFlow.Api.Tests;

/// <summary>Pruebas de integración: cada una hace peticiones HTTP reales contra la API y la base PostgreSQL.</summary>
public sealed class ApiTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;
    public ApiTests(ApiFactory factory) => _factory = factory;

    // Helpers: cada prueba crea un usuario con nombre único, así no dependen del orden de ejecución.
    private async Task<HttpClient> LoginAsync(string role = "User")
    {
        var client = _factory.CreateClient();
        string name = $"{role.ToLower()}-{Guid.NewGuid():N}"[..20];
        await client.PostAsJsonAsync("/auth/register", new { userName = name, password = "clave-segura-1" });
        var response = await client.PostAsJsonAsync("/auth/login", new { userName = name, password = "clave-segura-1" });
        var token = (await response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("accessToken").GetString();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    [Fact]
    public async Task Tasks_WithoutToken_Returns401()
    {
        var response = await _factory.CreateClient().GetAsync("/tasks");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithWrongPassword_Returns401()
    {
        var client = _factory.CreateClient();
        await client.PostAsJsonAsync("/auth/register", new { userName = "marta-wrong", password = "clave-segura-1" });
        var response = await client.PostAsJsonAsync("/auth/login", new { userName = "marta-wrong", password = "incorrecta" });
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Create_ThenGet_RoundTripsThroughPostgres()
    {
        var client = await LoginAsync();
        var created = await client.PostAsJsonAsync("/tasks", new { title = "Probar la API", priority = "High" });
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);

        var body = await created.Content.ReadFromJsonAsync<JsonElement>();
        int id = body.GetProperty("id").GetInt32();

        var fetched = await client.GetFromJsonAsync<JsonElement>($"/tasks/{id}");
        Assert.Equal("Probar la API", fetched.GetProperty("title").GetString());
        Assert.Equal("High", fetched.GetProperty("priority").GetString());
    }

    [Theory]
    [InlineData("{\"title\":\"ab\"}")]                                  // título demasiado corto (DataAnnotations)
    [InlineData("{\"title\":\"Tarea\",\"priority\":\"Urgente\"}")]       // enum inexistente (JSON)
    [InlineData("{\"title\":\"Tarea\",\"dueDate\":\"2001-01-01\"}")]     // fecha en el pasado (regla del dominio)
    public async Task Create_WithInvalidData_Returns400(string json)
    {
        var client = await LoginAsync();
        var response = await client.PostAsync("/tasks", new StringContent(json, System.Text.Encoding.UTF8, "application/json"));
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Get_Missing_Returns404WithProblemDetails()
    {
        var client = await LoginAsync();
        var response = await client.GetAsync("/tasks/999999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(404, problem.GetProperty("status").GetInt32());
    }

    [Fact]
    public async Task Delete_AsNormalUser_Returns403()
    {
        // El primer usuario de la base es Admin; nos aseguramos de que exista alguno antes del usuario normal.
        await LoginAsync("admin");
        var user = await LoginAsync("user");
        var created = await user.PostAsJsonAsync("/tasks", new { title = "No borrable" });
        int id = (await created.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetInt32();

        var response = await user.DeleteAsync($"/tasks/{id}");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Health_ReportsHealthy()
    {
        var response = await _factory.CreateClient().GetAsync("/health");
        Assert.Equal("Healthy", await response.Content.ReadAsStringAsync());
    }
}
