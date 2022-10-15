using OpenTelemetry;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using System.Diagnostics;

// Define some important constants to initialize tracing with
string serviceName = "Demo.Console";
string serviceVersion = "1.0.0";

// Configure important OpenTelemetry settings and the console exporter
using var traceProvider = Sdk.CreateTracerProviderBuilder()
    .AddSource(serviceName)
    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(serviceName, serviceVersion: serviceVersion))
    .AddConsoleExporter()
    .Build();

var activitySource = new ActivitySource(serviceName, serviceVersion);

using var activity = activitySource.StartActivity("Start tag activity", ActivityKind.Internal);
activity?.SetTag("event 1 resource", new { Id = 1245, IsValid = true });
activity?.SetTag("event 1 state", "Pending");
activity?.SetTag("event 1 succeeded", true);