

using CleanAspire.Api.Client;
using CleanAspire.Api.Client.Models;
using CleanAspire.ClientApp.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CleanAspire.ClientApp.Components.Autocompletes;

public class MultiTenantAutocomplete<T> : MudAutocomplete<TenantDto>
{
    public MultiTenantAutocomplete()
    {
        SearchFunc = SearchKeyValues;
        ToStringFunc = dto => dto?.Name;
        Dense = true;
        ResetValueOnEmptyText = true;
        ShowProgressIndicator = true;
    }
    public List<TenantDto>? Tenants { get; set; } = new();
    [Inject] private ApiClient ApiClient { get; set; } = default!;
    [Inject] private ApiClientServiceProxy ApiClientServiceProxy { get; set; } = default!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Tenants = await ApiClientServiceProxy.QueryAsync("multitenant", () => ApiClient.Tenants.GetAsync(), tags: null, expiration: TimeSpan.FromMinutes(60));
            StateHasChanged(); // Trigger a re-render after the tenants are loaded
        }
    }
    private async Task<IEnumerable<TenantDto>> SearchKeyValues(string? value, CancellationToken cancellation)
    {
        IEnumerable<TenantDto> result;

        if (string.IsNullOrWhiteSpace(value))
            result = Tenants ?? new List<TenantDto>();
        else
            result = Tenants?
                .Where(x => x.Name?.Contains(value, StringComparison.InvariantCultureIgnoreCase) == true ||
                            x.Description?.Contains(value, StringComparison.InvariantCultureIgnoreCase) == true)
                .ToList() ?? new List<TenantDto>();

        return await Task.FromResult(result);
    }
}
