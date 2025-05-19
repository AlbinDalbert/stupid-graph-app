# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build-env /app/out .

# Expose the port the app runs on (default Kestrel port is 8080 in containers for .NET 8+)
# If you configure a different port or an older .NET version, adjust accordingly (e.g., EXPOSE 5000)
EXPOSE 8080
# For older .NET versions, ASPNETCORE_URLS might be http://+:80
# ENV ASPNETCORE_URLS=http://+:8080

RUN ls -la

ENTRYPOINT ["dotnet", "kiss-graph-api.dll"]