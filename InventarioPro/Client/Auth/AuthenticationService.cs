using Microsoft.JSInterop;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

public class AuthenticationService
{
    private readonly IJSRuntime _jsRuntime;

    public AuthenticationService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    public async Task<string> GetTokenAsync()
    {
        return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "jwt_token");
    }
    public string GetUserNameFromToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return null;

        var jwtToken = new JwtSecurityToken(token);
        var userNameClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == "email");
        return userNameClaim?.Value;
    }
    public async Task RemoveTokenAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "jwt_token");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error eliminando el token: {ex.Message}");
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            await RemoveTokenAsync();

        }
        catch (Exception ex)
        {
        }
    }
}
