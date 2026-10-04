using System.Net.Http.Json;
using ConsuCare.Shared.Dtos;

namespace ConsuCare.Client.Services;

/// <summary>Typed wrapper over the ConsuCare peer-support API.</summary>
public class Api
{
    private readonly HttpClient _http;
    public Api(HttpClient http) => _http = http;

    // ---- Auth ----
    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", request);
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<AuthResponse>() : null;
    }

    public async Task<(AuthResponse? Auth, string? Error)> SignupAsync(SignupRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/auth/signup", request);
        if (response.IsSuccessStatusCode)
            return (await response.Content.ReadFromJsonAsync<AuthResponse>(), null);
        return (null, await response.Content.ReadAsStringAsync());
    }

    public async Task<UserDto?> UpdateProfileAsync(int userId, UpdateProfileRequest request)
    {
        var response = await _http.PutAsJsonAsync($"api/users/{userId}", request);
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<UserDto>() : null;
    }

    public async Task<UserDto?> UpdatePreferencesAsync(int userId, UpdatePreferencesRequest request)
    {
        var response = await _http.PutAsJsonAsync($"api/users/{userId}/preferences", request);
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<UserDto>() : null;
    }

    public async Task<string?> ChangePasswordAsync(int userId, ChangePasswordRequest request)
    {
        var response = await _http.PostAsJsonAsync($"api/users/{userId}/change-password", request);
        return response.IsSuccessStatusCode ? null : (await response.Content.ReadAsStringAsync()).Trim('"');
    }

    // ---- Supporters ----
    public Task<List<SupporterCardDto>?> DiscoverSupportersAsync(string? search = null)
        => _http.GetFromJsonAsync<List<SupporterCardDto>>(
            string.IsNullOrWhiteSpace(search) ? "api/supporters" : $"api/supporters?search={Uri.EscapeDataString(search)}");

    public Task<SupporterProfileDto?> GetSupporterProfileAsync(int userId)
        => _http.GetFromJsonAsync<SupporterProfileDto>($"api/supporters/{userId}");

    public async Task<SupporterProfileDto?> AddReviewAsync(int supporterUserId, CreateReviewRequest request)
    {
        var response = await _http.PostAsJsonAsync($"api/supporters/{supporterUserId}/reviews", request);
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<SupporterProfileDto>() : null;
    }

    // ---- Goals ----
    public Task<List<GoalDto>?> GetGoalsAsync(int patientId)
        => _http.GetFromJsonAsync<List<GoalDto>>($"api/goals/patient/{patientId}");

    public async Task<GoalDto?> ToggleMilestoneAsync(int milestoneId)
    {
        var response = await _http.PostAsync($"api/goals/milestones/{milestoneId}/toggle", null);
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<GoalDto>() : null;
    }

    public async Task<GoalDto?> CreateGoalAsync(CreateGoalRequest request)
    {
        var response = await _http.PostAsJsonAsync("api/goals", request);
        return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<GoalDto>() : null;
    }

    // ---- Messaging ----
    public Task<List<ConversationDto>?> GetConversationsAsync(int userId)
        => _http.GetFromJsonAsync<List<ConversationDto>>($"api/messages/conversations/{userId}");

    public Task<List<ChatMessageDto>?> GetThreadAsync(int userId, int partnerId)
        => _http.GetFromJsonAsync<List<ChatMessageDto>>($"api/messages/thread/{userId}/{partnerId}");

    // ---- Peer-support requests and connections ----
    public async Task<bool> CreateRequestAsync(CreateSupportRequest request)
        => (await _http.PostAsJsonAsync("api/support-requests", request)).IsSuccessStatusCode;

    public async Task AcceptRequestAsync(int id) => await _http.PostAsync($"api/support-requests/{id}/accept", null);
    public async Task DeclineRequestAsync(int id) => await _http.PostAsync($"api/support-requests/{id}/decline", null);

    public Task<List<SupportConnectionDto>?> GetSupportConnectionsAsync(int patientId)
        => _http.GetFromJsonAsync<List<SupportConnectionDto>>($"api/support-connections/patient/{patientId}");

    // ---- Notifications ----
    public Task<List<NotificationDto>?> GetNotificationsAsync(int userId)
        => _http.GetFromJsonAsync<List<NotificationDto>>($"api/notifications/{userId}");

    public async Task MarkAllReadAsync(int userId)
        => await _http.PostAsync($"api/notifications/{userId}/read-all", null);

    // ---- Dashboards ----
    public Task<PatientDashboardDto?> GetPatientDashboardAsync(int patientId)
        => _http.GetFromJsonAsync<PatientDashboardDto>($"api/dashboard/patient/{patientId}");

    public Task<SupporterDashboardDto?> GetSupporterDashboardAsync(int supporterUserId)
        => _http.GetFromJsonAsync<SupporterDashboardDto>($"api/dashboard/supporter/{supporterUserId}");

    // ---- Admin ----
    public Task<List<VerificationDto>?> GetVerificationsAsync()
        => _http.GetFromJsonAsync<List<VerificationDto>>("api/admin/verifications");

    public async Task ApproveVerificationAsync(int profileId)
        => await _http.PostAsync($"api/admin/verifications/{profileId}/approve", null);

    public async Task RejectVerificationAsync(int profileId)
        => await _http.PostAsync($"api/admin/verifications/{profileId}/reject", null);
}
