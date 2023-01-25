FROM mcr.microsoft.com/dotnet/sdk:7.0 as build
EXPOSE 80
EXPOSE 443

WORKDIR /src

COPY ./OpenTelemetry.sln .
COPY ./src/API/API.csproj ./src/API/
COPY ./src/Shared/Shared.csproj ./src/Shared/

RUN dotnet restore

COPY . .

RUN dotnet publish --no-restore -c Release -o /published src/API/API.csproj

FROM mcr.microsoft.com/dotnet/aspnet:7.0 as runtime

# Uncomment the line below if running with HTTPS
# ENV ASPNETCORE_URLS=https://+:443

WORKDIR /app

COPY --from=build /published .

ENTRYPOINT [ "dotnet", "API.dll" ]