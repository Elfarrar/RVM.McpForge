FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src
COPY ["src/RVM.McpForge.Domain/RVM.McpForge.Domain.csproj", "src/RVM.McpForge.Domain/"]
COPY ["src/RVM.McpForge.Infrastructure/RVM.McpForge.Infrastructure.csproj", "src/RVM.McpForge.Infrastructure/"]
COPY ["src/RVM.McpForge.Application/RVM.McpForge.Application.csproj", "src/RVM.McpForge.Application/"]
COPY ["src/RVM.McpForge.API/RVM.McpForge.API.csproj", "src/RVM.McpForge.API/"]
RUN dotnet restore "src/RVM.McpForge.API/RVM.McpForge.API.csproj"
COPY . .
WORKDIR "/src/src/RVM.McpForge.API"
RUN dotnet build "RVM.McpForge.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RVM.McpForge.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
RUN mkdir -p /app/data/dataprotection
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RVM.McpForge.API.dll"]
