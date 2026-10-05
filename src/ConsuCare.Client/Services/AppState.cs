using System.Text.Json;
using ConsuCare.Shared.Dtos;
using Microsoft.JSInterop;

namespace ConsuCare.Client.Services;

/// <summary>Client-side session state, persisted to localStorage so refreshes keep you signed in.</summary>
public class AppState
{
    private const string StorageKey = "consucare-session";
    private readonly IJSRuntime _js;
    private bool _loaded;

    public AppState(IJSRuntime js) => _js = js;

    public UserDto? CurrentUser { get; private set; }
    public string? Token { get; private set; }
    public event Action? OnChange;

    public async Task EnsureLoadedAsync()
    {
        if (_loaded) return;
        _loaded = true;
        try
        {
            var json = await _js.InvokeAsync<string?>("consuCareStore.get", StorageKey);
            if (!string.IsNullOrEmpty(json))
            {
                var session = JsonSerializer.Deserialize<StoredSession>(json);
                CurrentUser = session?.User;
                Token = session?.Token;
            }
        }
        catch { /* localStorage unavailable — stay signed out */ }
    }

    public async Task SignInAsync(UserDto user, string token)
    {
        CurrentUser = user;
        Token = token;
        await _js.InvokeVoidAsync("consuCareStore.set", StorageKey, JsonSerializer.Serialize(new StoredSession(user, token)));
        OnChange?.Invoke();
    }

    public async Task SignOutAsync()
    {
        CurrentUser = null;
        Token = null;
        await _js.InvokeVoidAsync("consuCareStore.del", StorageKey);
        OnChange?.Invoke();
    }

    private record StoredSession(UserDto User, string Token);
}
