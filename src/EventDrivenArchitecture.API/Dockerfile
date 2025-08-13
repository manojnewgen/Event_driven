# Use the official .NET 8 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file and project files
COPY ["EventDrivenArchitecture.sln", "."]
COPY ["src/EventDrivenArchitecture.API/EventDrivenArchitecture.API.csproj", "src/EventDrivenArchitecture.API/"]
COPY ["src/EventDrivenArchitecture.Domain/EventDrivenArchitecture.Domain.csproj", "src/EventDrivenArchitecture.Domain/"]
COPY ["src/EventDrivenArchitecture.Publishers/EventDrivenArchitecture.Publishers.csproj", "src/EventDrivenArchitecture.Publishers/"]
COPY ["src/EventDrivenArchitecture.Consumers/EventDrivenArchitecture.Consumers.csproj", "src/EventDrivenArchitecture.Consumers/"]

# Restore NuGet packages
RUN dotnet restore "src/EventDrivenArchitecture.API/EventDrivenArchitecture.API.csproj"

# Copy the entire source code
COPY . .

# Build the application
WORKDIR "/src/src/EventDrivenArchitecture.API"
RUN dotnet build "EventDrivenArchitecture.API.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "EventDrivenArchitecture.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the official .NET 8 runtime image for running
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create a non-root user for security
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Copy published application
COPY --from=publish /app/publish .

# Change ownership to appuser
RUN chown -R appuser:appuser /app
USER appuser

# Expose port 8080 (default for .NET 8 in containers)
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=30s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

# Set the entry point
ENTRYPOINT ["dotnet", "EventDrivenArchitecture.API.dll"]