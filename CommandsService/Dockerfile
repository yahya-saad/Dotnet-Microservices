# See https://aka.ms/customizecontainer to learn how to customize your debug container 
# and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# ===== Base Stage =====
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# ===== Build Stage =====
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project
COPY ["CommandsService/CommandsService.csproj", "CommandsService/"]

# Restore dependencies
RUN dotnet restore "CommandsService/CommandsService.csproj"

# Copy everything and build
COPY . .
WORKDIR "/src/CommandsService"
RUN dotnet build "CommandsService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ===== Publish Stage =====
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CommandsService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ===== Final Stage =====
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CommandsService.dll"]
