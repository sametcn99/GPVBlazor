# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["GPVBlazor.csproj", "."]
RUN dotnet restore "./GPVBlazor.csproj"
COPY . .
RUN dotnet publish "./GPVBlazor.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Serve Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "GPVBlazor.dll"]