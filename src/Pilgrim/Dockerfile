FROM mcr.microsoft.com/dotnet/sdk:7.0 as build
EXPOSE 80
EXPOSE 443

WORKDIR /src

COPY ["Pilgrim/Pilgrim.csproj", "Pilgrim/"]
COPY ["Shared/Shared.csproj", "Shared/"]

RUN dotnet restore "Pilgrim/Pilgrim.csproj"

COPY . .
WORKDIR "/src/Pilgrim"
RUN dotnet build "Pilgrim.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Pilgrim.csproj" --no-restore -c Release -o /published

FROM mcr.microsoft.com/dotnet/aspnet:7.0 as runtime

# Uncomment the line below if running with HTTPS
# ENV ASPNETCORE_URLS=https://+:443
ENV ASPNETCORE_URLS=https://+:80

WORKDIR /app

COPY --from=publish /published .

ENTRYPOINT [ "dotnet", "Pilgrim.dll" ]