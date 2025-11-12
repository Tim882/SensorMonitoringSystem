FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем файл проекта
COPY ["SensorEmulatorWorker/SensorEmulatorWorker.csproj", "SensorEmulatorWorker/"]
RUN dotnet restore "SensorEmulatorWorker/SensorEmulatorWorker.csproj"

# Копируем весь исходный код
COPY . .

# Сборка и публикация
WORKDIR "/src/SensorEmulatorWorker"
RUN dotnet publish "SensorEmulatorWorker.csproj" -c Release -o /app/publish

# Финальный образ
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Копируем опубликованные файлы
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "SensorEmulatorWorker.dll"]