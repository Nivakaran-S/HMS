FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5002

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["DoctorService/DoctorService.csproj", "DoctorService/"]
COPY ["Common/Common.csproj", "Common/"]
RUN dotnet restore "DoctorService/DoctorService.csproj"
COPY . .
WORKDIR "/src/DoctorService"
RUN dotnet build "DoctorService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DoctorService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DoctorService.dll"]