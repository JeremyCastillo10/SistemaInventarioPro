using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

public class ApiAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly AuthenticationService _authenticationService;
    private readonly IJSRuntime _jsRuntime;

    public ApiAuthenticationStateProvider(AuthenticationService authenticationService, IJSRuntime jsRuntime)
    {
        _authenticationService = authenticationService;
        _jsRuntime = jsRuntime;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _authenticationService.GetTokenAsync();

        if (string.IsNullOrEmpty(token))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, _authenticationService.GetUserNameFromToken(token)),
        new Claim(ClaimTypes.NameIdentifier, _authenticationService.GetUserIdFromToken(token)) 
    };

        var identity = new ClaimsIdentity(claims, "jwt");
        var user = new ClaimsPrincipal(identity);

        return new AuthenticationState(user);
    }




}
