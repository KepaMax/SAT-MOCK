# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# 1. Copy ALL .csproj files by mirroring the folder structure
# This ensures that when we restore, the SDK sees the entire dependency tree
COPY ["src/Web/Web.csproj", "src/Web/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]

# 2. Restore everything at once
# This solves the "TargetFramework not recognized" error
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

ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "EXAM_SYSTEM.Web.dll"]