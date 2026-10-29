using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace TaskFlow.Api;

/// <summary>Opciones del JWT, enlazadas a la sección "Jwt" de la configuración (patrón Options).</summary>
public sealed class JwtOptions
{
    public string Issuer { get; set; } = "TaskFlow";
    public string Audience { get; set; } = "TaskFlow.Clients";
    public string Key { get; set; } = "";            // SECRETO: viene de user-secrets / variable de entorno, nunca del repo
    public int ExpiresMinutes { get; set; } = 30;
}

public sealed class JwtTokenService(IOptions<JwtOptions> options)
{
    private readonly JwtOptions _o = options.Value;

    public string Create(int userId, string userName, string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_o.Key));
        var token = new JwtSecurityToken(
            issuer: _o.Issuer, audience: _o.Audience,
            claims: [new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                     new Claim(ClaimTypes.Name, userName),
                     new Claim(ClaimTypes.Role, role)],
            expires: DateTime.UtcNow.AddMinutes(_o.ExpiresMinutes),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
