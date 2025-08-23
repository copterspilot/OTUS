

using CleanAspire.Api.Client;
using CleanAspire.Api.Client.Models;
using CleanAspire.ClientApp.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CleanAspire.ClientApp.Components.Autocompletes;

public class ProductAutocomplete<T> : MudAutocomplete<ProductDto>
{
    public ProductAutocomplete()
    {
        SearchFunc = SearchKeyValues;
        ToStringFunc = dto => dto?.Name;
        Dense = true;
        ResetValueOnEmptyText = true;
        ShowProgressIndicator = true;
    }
    [Parameter] public string? DefaultProductId { get; set; }
    public List<ProductDto>? Products { get; set; } = new();
    [Inject] private ApiClient ApiClient { get; set; } = default!;
    [Inject] private ApiClientServiceProxy ApiClientServiceProxy { get; set; } = default!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Products = await ApiClientServiceProxy.QueryAsync("_allproducts", () => ApiClient.Products.GetAsync(), tags: null, expiration: TimeSpan.FromMinutes(60));
            if (!string.IsNullOrEmpty(DefaultProductId))
            {
                var defaultProduct = Products?.FirstOrDefault(p => p.Id == DefaultProductId);
                if (defaultProduct != null)
                {
                    Value = defaultProduct; 
                    await ValueChanged.InvokeAsync(Value);
                }
            }
            StateHasChanged(); // Trigger a re-render after the tenants are loaded
        }
    }
    private async Task<IEnumerable<ProductDto>> SearchKeyValues(string? value, CancellationToken cancellation)
    {
        IEnumerable<ProductDto> result;

        if (string.IsNullOrWhiteSpace(value))
            result = Products ?? new List<ProductDto>();
        else
            result = Products?
                .Where(x => x.Name?.Contains(value, StringComparison.InvariantCultureIgnoreCase) == true ||
                            x.Sku?.Contains(value, StringComparison.InvariantCultureIgnoreCase) == true ||
                            x.Description?.Contains(value, StringComparison.InvariantCultureIgnoreCase) == true)
                .ToList() ?? new List<ProductDto>();

        return await Task.FromResult(result);
    }
}
