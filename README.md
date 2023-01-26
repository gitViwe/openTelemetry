<!-- ABOUT THE PROJECT -->
# OpenTelemetry
OpenTelemetry, also known as OTel for short, is a vendor-neutral open-source Observability framework for instrumenting, generating, collecting, and exporting telemetry data such as traces, metrics, logs. As an industry-standard it is natively supported by a number of vendors.


<!-- GETTING STARTED -->
## Getting Started

This is an example of how you may give instructions on setting up your project locally.
To get a local copy up and running follow these simple example steps.

### Prerequisites

Things you need to use the software and how to install them.
* [Visual Studio / Visual Studio Code](https://visualstudio.microsoft.com/)
* [.NET 7](https://devblogs.microsoft.com/dotnet/announcing-dotnet-7/)
* [Docker](https://www.docker.com/)

### Installation

1. Clone the repo
   ```sh
   git clone https://github.com/gitViwe/openTelemetry.git
   ```
2. Run via Docker
   ```
   cd opentelemetry
   docker compose up -d
   ```

* Then navigate to [http://localhost:5043/swagger](http://localhost:5043/swagger) and send a request.
* View traces via Jaeger at [http://localhost:16686](http://localhost:16686) or Zipkin at [http://localhost:9411](http://localhost:9411)
* Access RabittMQ UI at [http://localhost:515672](http://localhost:15672) Username: `guest` Password: `guest`
* Access SEQ UI at [http://localhost:5555](http://localhost:5555)
