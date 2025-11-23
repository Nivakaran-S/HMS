FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5003

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["AppointmentService/AppointmentService.csproj", "AppointmentService/"]
COPY ["Common/Common.csproj", "Common/"]
RUN dotnet restore "AppointmentService/AppointmentService.csproj"
COPY . .
WORKDIR "/src/AppointmentService"
RUN dotnet build "AppointmentService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AppointmentService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AppointmentService.dll"]