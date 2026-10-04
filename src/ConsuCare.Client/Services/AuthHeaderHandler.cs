using System.Net.Http.Headers;

namespace ConsuCare.Client.Services;

/// <summary>Attaches the signed-in user's JWT to every outgoing API request.</summary>
public class AuthHeaderHandler : DelegatingHandler
{
    private readonly AppState _state;
    public AuthHeaderHandler(AppState state) => _state = state;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        await _state.EnsureLoadedAsync();
        if (!string.IsNullOrEmpty(_state.Token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _state.Token);
        return await base.SendAsync(request, cancellationToken);
    }
}
