# 1️⃣ Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Копіюємо файли рішення і проектів
COPY *.sln ./
COPY *.csproj ./
COPY NuGet.Config ./
RUN dotnet restore

# Копіюємо весь код проекту
COPY . ./
WORKDIR /app
RUN dotnet restore

# Публікуємо додаток
RUN dotnet publish -c Release -o /app/publish --no-restore

# 2️⃣ Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Копіюємо опубліковані файли з build stage
COPY --from=build /app/publish .

# Копіюємо SQLite базу (якщо є у репозиторії)
COPY app.db .

# Відкриваємо порт для сайту
EXPOSE 5000

# Запуск додатку
ENTRYPOINT ["dotnet", "answer_ua.dll"]
