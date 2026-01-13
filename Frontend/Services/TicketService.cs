using System.Net.Http.Json;
using System.Net.Http.Headers;
using Frontend.Models;

namespace Frontend.Services;

public class TicketService
{
    private readonly HttpClient _httpClient;
    private readonly AuthService _authService;

    public TicketService(HttpClient httpClient, AuthService authService)
    {
        _httpClient = httpClient;
        _authService = authService;
    }

    private async Task AddAuthorizationHeaderAsync()
    {
        var token = await _authService.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<List<TicketDto>> GetTicketsAsync()
    {
        try
        {
            var tickets = await _httpClient.GetFromJsonAsync<List<TicketDto>>("api/tickets");
            return tickets ?? new List<TicketDto>();
        }
        catch
        {
            return new List<TicketDto>();
        }
    }

    public async Task<bool> CreateTicketAsync(string title, string description)
    {
        try
        {
            await AddAuthorizationHeaderAsync();

            var response = await _httpClient.PostAsJsonAsync("api/tickets", new
            {
                Title = title,
                Description = description
            });

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateTicketStatusAsync(int ticketId, int statusId)
    {
        try
        {
            await AddAuthorizationHeaderAsync();

            var response = await _httpClient.PutAsJsonAsync($"api/tickets/{ticketId}", new
            {
                StatusID = statusId
            });

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteTicketAsync(int ticketId)
    {
        try
        {
            await AddAuthorizationHeaderAsync();

            var response = await _httpClient.DeleteAsync($"api/tickets/{ticketId}");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<TicketStatus>> GetStatusesAsync()
    {
        try
        {
            var statuses = await _httpClient.GetFromJsonAsync<List<TicketStatus>>("api/tickets/statuses");
            return statuses ?? new List<TicketStatus>();
        }
        catch
        {
            return new List<TicketStatus>();
        }
    }
}
