# -----------------------------------------------------------------------------
# Stage 1: Build the application using .NET 8.0 SDK container
# -----------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project definition and restore dependencies first to leverage Docker layer caching
COPY ["ProgressGateway.Api.csproj", "./"]
RUN dotnet restore "ProgressGateway.Api.csproj"

# Copy all remaining source files and compile Release binaries
COPY . .
RUN dotnet publish "ProgressGateway.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# -----------------------------------------------------------------------------
# Stage 2: Create lightweight production runtime image
# -----------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy compiled artifacts from the build stage
COPY --from=build /app/publish .

# Expose default internal ASP.NET Core port
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

# Define container entrypoint
ENTRYPOINT ["dotnet", "ProgressGateway.Api.dll"]