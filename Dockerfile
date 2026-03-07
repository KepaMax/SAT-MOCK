# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# 1. Copy GLOBAL configuration files
# These files tell the SDK that the project uses 'net10.0'
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]
COPY ["global.json", "./"]

# 2. Copy project files mirroring your specific folder structure
COPY ["src/Web/Web.csproj", "src/Web/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]

# 3. Restore dependencies
# This will now succeed because TargetFramework is found in Directory.Build.props
RUN dotnet restore "src/Web/Web.csproj"

# 4. Copy the rest of the source code
COPY . .

# 5. Build and Publish
WORKDIR "/src/src/Web"
RUN dotnet publish "Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080

# Essential for Render's dynamic port mapping
ENV ASPNETCORE_URLS=http://+:8080

# Copy the published output from the build stage
COPY --from=build /app/publish .

# 6. ENTRYPOINT - Uses your AssemblyName: EXAM_SYSTEM.Web
ENTRYPOINT ["dotnet", "EXAM_SYSTEM.Web.dll"]