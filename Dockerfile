# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# 1. Copy project files individually to cache layers
# Note: Casing must match your folders (Application, Domain, Infrastructure, Web)
COPY ["src/Web/Web.csproj", "src/Web/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]

# 2. Restore dependencies
RUN dotnet restore "src/Web/Web.csproj"

# 3. Copy the rest of the source code
COPY . .

# 4. Build and Publish
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

# 5. ENTRYPOINT - Uses your specific AssemblyName from the screenshot
ENTRYPOINT ["dotnet", "EXAM_SYSTEM.Web.dll"]
