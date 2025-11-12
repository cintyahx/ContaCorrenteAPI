#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Miotto.BankMore.Conta.App/Miotto.BankMore.Conta.App.csproj", "Miotto.BankMore.Conta.App/"]
COPY ["Miotto.BankMore.Conta.Domain/Miotto.BankMore.Conta.Domain.csproj", "Miotto.BankMore.Conta.Domain/"]
COPY ["Miotto.BankMore.Conta.Infra/Miotto.BankMore.Conta.Infra.csproj", "Miotto.BankMore.Conta.Infra/"]
COPY ["Miotto.BankMore.Conta.Service/Miotto.BankMore.Conta.Service.csproj", "Miotto.BankMore.Conta.Service/"]
RUN dotnet restore "./Miotto.BankMore.Conta.App/Miotto.BankMore.Conta.App.csproj"
COPY . .
WORKDIR "/src/Miotto.BankMore.Conta.App"
RUN dotnet build "./Miotto.BankMore.Conta.App.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Miotto.BankMore.Conta.App.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Miotto.BankMore.Conta.App.dll"]