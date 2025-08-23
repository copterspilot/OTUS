
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CleanAspire.ClientApp.Components.Autocompletes;

public class PicklistAutocomplete<T> : MudAutocomplete<string>
{
    public PicklistAutocomplete()
    {
        MaxItems = 50;
        SearchFunc = SearchFunc_;
        Dense = true;
        ResetValueOnEmptyText = true;
    }

    [Parameter]
    public PicklistType Picklist { get; set; }

    public Dictionary<PicklistType, string[]> Data => PicklistDataSource.Data;

    private Task<IEnumerable<string>> SearchFunc_(string? value, CancellationToken cancellation = default)
    {
        if (!Data.ContainsKey(Picklist))
            return Task.FromResult(Enumerable.Empty<string>());

        var list = Data[Picklist];

        if (string.IsNullOrEmpty(value))
            return Task.FromResult(list.AsEnumerable());

        return Task.FromResult(list.Where(x =>
            x.Contains(value, StringComparison.InvariantCultureIgnoreCase)));
    }
}
