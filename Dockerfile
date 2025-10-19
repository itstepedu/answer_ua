# 1️⃣ Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Встановлюємо додаткові утиліти
RUN apt-get update && \
    apt-get install -y curl net-tools iproute2 && \
    rm -rf /var/lib/apt/lists/*

# Копіюємо файли рішення і проектів
COPY answer_ua.sln ./
COPY AnswerUA.csproj ./
RUN dotnet restore answer_ua.sln

# Копіюємо весь код проекту
COPY . ./
WORKDIR /app

# Публікуємо додаток
RUN dotnet publish answer_ua.sln -c Release -o /app/publish --no-restore

# 2️⃣ Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Додаємо змінну середовища для порту
ENV ASPNETCORE_URLS=http://+:5000

# Копіюємо опубліковані файли з build stage
COPY --from=build /app/publish .

# Volume для бази SQLite
#VOLUME ["/app/app.db"]

# Відкриваємо порт для сайту
EXPOSE 5000

# Запуск додатку
ENTRYPOINT ["dotnet", "AnswerUA.dll"]
