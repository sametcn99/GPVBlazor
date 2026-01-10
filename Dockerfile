# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["GPVBlazor/GPVBlazor/GPVBlazor.csproj", "GPVBlazor/GPVBlazor/"]
RUN dotnet restore "GPVBlazor/GPVBlazor/GPVBlazor.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/GPVBlazor/GPVBlazor"
RUN dotnet publish "./GPVBlazor.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Serve Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GPVBlazor.dll"]