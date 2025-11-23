FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5004

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["BillingService/BillingService.csproj", "BillingService/"]
COPY ["Common/Common.csproj", "Common/"]
RUN dotnet restore "BillingService/BillingService.csproj"
COPY . .
WORKDIR "/src/BillingService"
RUN dotnet build "BillingService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BillingService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BillingService.dll"]