# 1️⃣ Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Копіюємо файли рішення і проектів
COPY *.sln ./
COPY answer_ua/*.csproj ./answer_ua/
RUN dotnet restore

# Копіюємо весь код проекту
COPY answer_ua/. ./answer_ua/
WORKDIR /app/answer_ua

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