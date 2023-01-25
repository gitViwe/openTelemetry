FROM mcr.microsoft.com/dotnet/sdk:7.0 as build
EXPOSE 80
EXPOSE 443

WORKDIR /src

COPY ["Orchestrator/Orchestrator.csproj", "Orchestrator/"]
COPY ["Shared/Shared.csproj", "Shared/"]

RUN dotnet restore "Orchestrator/Orchestrator.csproj"

COPY . .
WORKDIR "/src/Orchestrator"
RUN dotnet build "Orchestrator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Orchestrator.csproj" --no-restore -c Release -o /published

FROM mcr.microsoft.com/dotnet/aspnet:7.0 as runtime

# Uncomment the line below if running with HTTPS
# ENV ASPNETCORE_URLS=https://+:443

WORKDIR /app

COPY --from=publish /published .

ENTRYPOINT [ "dotnet", "Orchestrator.dll" ]