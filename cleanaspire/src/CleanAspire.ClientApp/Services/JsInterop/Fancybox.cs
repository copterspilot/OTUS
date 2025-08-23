using Microsoft.JSInterop;

namespace CleanAspire.ClientApp.Services.JsInterop;
public sealed class Fancybox
{
    private readonly IJSRuntime _jsRuntime;

    public Fancybox(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<ValueTask> Preview(string defaultUrl, IEnumerable<string> images)
    {
        var jsmodule = await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/fancybox.js").ConfigureAwait(false);
        return jsmodule.InvokeVoidAsync("filepreview", defaultUrl,
            images);
    }
}
