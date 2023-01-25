using System.Diagnostics;
using System.Net.Http.Json;

namespace Client.Pages;

public partial class Index
{
    private static readonly ActivitySource ActivitySource = new ActivitySource("blazor.wasm.client");
    bool _processing = false;
    public DummyModel Model { get; set; } = new();

    private async Task SubmitAsync()
    {
        _processing = true;
        using var activity = ActivitySource.StartActivity("send api request", ActivityKind.Client);

        activity?.AddEvent(new("client making api request..."));
        activity?.SetTag("client.user.username", Model.Username);

        await _httpClient.PostAsJsonAsync("https://localhost:7114/journey/start", Model);

        _processing = false;
    }
}

public class DummyModel
{
    public string Username { get; set; } = string.Empty;
}
