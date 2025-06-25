# File: kiss-graph-api/Dockerfile

# --- Build Stage ---
# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

# Copy the project file and the nuget.config that points to our local feed.
# Using the specific project name is more reliable than a wildcard.
COPY kiss-graph-api.csproj ./
COPY nuget.config ./

# Copy the entire local-nuget-feed directory into the container.
# This contains all our pre-downloaded, offline packages.
COPY local-nuget-shit/ ./local-nuget-shit/

# Use a cache mount for the NuGet packages directory during restore.
RUN dotnet restore

# Copy the rest of your application source code.
COPY . ./

# Publish the application, creating the final output in the 'out' folder.
RUN dotnet publish -c Release -o out


# --- Build the final, smaller runtime image ---
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build-env /app/out .

# Expose the port the app runs on
EXPOSE 8080

ENTRYPOINT ["dotnet", "kiss-graph-api.dll"]
