using Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();
builder.Services.AddOpenTelemetry().WithTracing(builder =>
{
    builder.AddSource("blazor.wasm.client")
           .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("blazor.wasm.client"))
           .AddHttpClientInstrumentation()
           .AddZipkinExporter(options =>
           {
               options.Endpoint = new Uri("http://localhost:9411/api/v2/spans");
           })
           .AddJaegerExporter(options =>
           {
               options.AgentHost = "localhost";
               options.AgentPort = 6831;
           });
}).StartWithHost();

await builder.Build().RunAsync();
