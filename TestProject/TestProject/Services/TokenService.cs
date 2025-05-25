using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TestProject.Models;
using TestProject.Models.JWT;

namespace TestProject.Services;

public class TokenService(JwtSettings jwtSettings, UserManager<User> userManager)
{
    private readonly JwtSettings _jwtSettings = jwtSettings;
    private readonly UserManager<User> _userManager = userManager;

    public async Task<string> GenerateToken(User user)
    {
        var userRoles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName!)
        };

        foreach (var userRole in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, userRole));
        }

        var token = GenerateAccessToken(claims);

        await _userManager.AddClaimsAsync(user, claims);

        var response = new JwtSecurityTokenHandler().WriteToken(token);

        return response;
    }

    private JwtSecurityToken GenerateAccessToken(IList<Claim> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        return new JwtSecurityToken(
           _jwtSettings.Issuer,
            _jwtSettings.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_jwtSettings.DurationInMinutes)),
            signingCredentials: credentials
        );
    }

    private TokenValidationParameters GetValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
            ValidateLifetime = true
        };
    }
}
