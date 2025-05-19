#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src
COPY ["VantageView.Api/VantageView.Api.csproj", "VantageView.Api/"]
COPY ["./VantageView.Contracts/VantageView.Contracts.csproj", "VantageView.Contracts/"]
COPY ["./VantageView.Application/VantageView.Application.csproj", "VantageView.Application/"]
COPY ["./VantageView.Domain/VantageView.Domain.csproj", "VantageView.Domain/"]
COPY ["./VantageView.Infrastructure/VantageView.Infrastructure.csproj", "VantageView.Infrastructure/"]
COPY ["./VantageView.Infrastructure.Parser/VantageView.Infrastructure.Parser.csproj", "VantageView.Infrastructure.Parser/"]
COPY ["./VantageView.Infrastructure.Engine/VantageView.Infrastructure.Engine.csproj", "VantageView.Infrastructure.Engine/"]
COPY ["./VantageView.Infrastructure.TaskManager/VantageView.Infrastructure.TaskManager.csproj", "VantageView.Infrastructure.TaskManager/"]
COPY ["./VantageView.Tools.Messaging/VantageView.Tools.Messaging.csproj", "VantageView.Tools.Messaging/"]
RUN dotnet restore "VantageView.Api/VantageView.Api.csproj"
COPY . .
WORKDIR "/src/VantageView.Api"
RUN dotnet build "VantageView.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VantageView.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VantageView.Api.dll"]