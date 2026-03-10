# STAGE 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# 1. Copy only .csproj files first for better caching
# This ensures that 'dotnet restore' only re-runs if you change dependencies
COPY ["src/Web/Web.csproj", "src/Web/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]

# 2. Restore dependencies
RUN dotnet restore "src/Web/Web.csproj"

# 3. Copy the rest of the source code
COPY . .

# 4. Build and Publish the Web project
WORKDIR "/src/src/Web"
RUN dotnet publish "Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# STAGE 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /app/publish .

# Expose the port (ASP.NET 8/10 defaults to 8080)
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "EXAM_SYSTEM.Web.dll"]