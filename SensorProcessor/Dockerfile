FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем файл проекта
COPY ["SensorProcessor/SensorProcessor.csproj", "SensorProcessor/"]
RUN dotnet restore "SensorProcessor/SensorProcessor.csproj"

# Копируем весь исходный код
COPY . .

# Сборка и публикация
WORKDIR "/src/SensorProcessor"
RUN dotnet publish "SensorProcessor.csproj" -c Release -o /app/publish

# Финальный образ
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Копируем опубликованные файлы
COPY --from=build /app/publish .

EXPOSE 5000
EXPOSE 5001

ENTRYPOINT ["dotnet", "SensorProcessor.dll"]