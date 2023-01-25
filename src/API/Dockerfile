FROM mcr.microsoft.com/dotnet/sdk:7.0 as build
EXPOSE 80
EXPOSE 443

WORKDIR /src

COPY ["API/API.csproj", "API/"]
COPY ["Shared/Shared.csproj", "Shared/"]

RUN dotnet restore "API/API.csproj"

COPY . .
WORKDIR "/src/API"
RUN dotnet build "API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "API.csproj" --no-restore -c Release -o /published

FROM mcr.microsoft.com/dotnet/aspnet:7.0 as runtime

# Uncomment the line below if running with HTTPS
# ENV ASPNETCORE_URLS=https://+:443

WORKDIR /app

COPY --from=publish /published .

ENTRYPOINT [ "dotnet", "API.dll" ]