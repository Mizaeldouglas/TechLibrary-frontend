using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using TechLibrary.Frontend.Models;

namespace TechLibrary.Frontend.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly AuthenticationStateProvider _authenticationStateProvider;
    private readonly ILocalStorageService _localStorage;

    public AuthService(HttpClient httpClient, AuthenticationStateProvider authenticationStateProvider, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _authenticationStateProvider = authenticationStateProvider;
        _localStorage = localStorage;
        _httpClient.BaseAddress = new Uri("http://localhost:5079/");
    }

    public async Task<bool> Register(RegisterModel registerModel)
    {
        var content = new StringContent(JsonSerializer.Serialize(registerModel), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("users", content);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> Login(LoginModel loginModel)
    {
        var content = new StringContent(JsonSerializer.Serialize(loginModel), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("login", content);

        if (!response.IsSuccessStatusCode)
            return false;

        var authResponse = JsonSerializer.Deserialize<AuthResponse>(await response.Content.ReadAsStringAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        await _localStorage.SetItemAsync("authToken", authResponse.AccessToken);
        await _localStorage.SetItemAsync("userEmail", loginModel.Email);
        ((CustomAuthenticationStateProvider)_authenticationStateProvider).NotifyUserAuthentication(authResponse.AccessToken);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResponse.AccessToken);

        return true;
    }

    public async Task ReserveBookAsync(Guid bookId)
    {
        var response = await _httpClient.PostAsync($"api/Checkouts/{bookId}", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task Logout()
    {
        await _localStorage.RemoveItemAsync("authToken");
        await _localStorage.RemoveItemAsync("userEmail");
        ((CustomAuthenticationStateProvider)_authenticationStateProvider).NotifyUserLogout();
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<bool> IsUserLoggedIn()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        return !string.IsNullOrWhiteSpace(token);
    }

    public async Task<string> GetUserEmail()
    {
        return await _localStorage.GetItemAsync<string>("userEmail");
    }
}