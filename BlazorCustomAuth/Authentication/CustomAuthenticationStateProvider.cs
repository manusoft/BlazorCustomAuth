using BlazorCustomAuth.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;

namespace BlazorCustomAuth.Authentication;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ProtectedSessionStorage _protectedSessionStorage;
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

    public CustomAuthenticationStateProvider(ProtectedSessionStorage protectedSessionStorage)
    {
        _protectedSessionStorage = protectedSessionStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var userSessionStorageResult = await _protectedSessionStorage.GetAsync<UserSession>("UserSession");
            var userSession = userSessionStorageResult.Success ? userSessionStorageResult.Value : null;

            if (userSession == null)
                return await Task.FromResult(new AuthenticationState(_anonymous));

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.Name, userSession.Username),
                new Claim(ClaimTypes.Role, userSession.Role),
            }, "CustomAuth"));

            return await Task.FromResult(new AuthenticationState(claimsPrincipal));
        }
        catch (Exception)
        {
            return await Task.FromResult(new AuthenticationState(_anonymous));
        }
    }

    public async Task UpdateAuthenticationState(UserSession userSession)
    {
        ClaimsPrincipal claimsPrincipal;

        if (userSession != null)
        {
            await _protectedSessionStorage.SetAsync("UserSession", userSession);

            claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.Name, userSession.Username),
                new Claim(ClaimTypes.Role, userSession.Role),
            }));
        }
        else
        {
            await _protectedSessionStorage.DeleteAsync("UserSession");

            claimsPrincipal = _anonymous;
        }

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
    }
}

