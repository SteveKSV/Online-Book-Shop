#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Order/Presentation/Order.csproj", "Services/Order/Presentation/"]
COPY ["EventBusMessages/EventBusMessages.csproj", "EventBusMessages/"]
COPY ["Services/Order/Application/Application.csproj", "Services/Order/Application/"]
COPY ["Services/Order/Domain/Domain.csproj", "Services/Order/Domain/"]
COPY ["Services/Order/Infrastructure/Infrastructure.csproj", "Services/Order/Infrastructure/"]
RUN dotnet restore "./Services/Order/Presentation/Order.csproj"
COPY . .
WORKDIR "/src/Services/Order/Presentation"
RUN dotnet build "./Order.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Order.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Order.dll"]