FROM postgres:17.2

# Устанавливаем Python и создаем виртуальное окружение
RUN apt-get update && \
    apt-get install -y python3 python3-venv python3-pip && \
    python3 -m venv /opt/venv && \
    /opt/venv/bin/pip install --upgrade pip

# Копируем скрипт и устанавливаем зависимости
COPY fill_db.py /docker-entrypoint-initdb.d/
RUN /opt/venv/bin/pip install psycopg2-binary && \
    chmod +x /docker-entrypoint-initdb.d/fill_db.py

# Убедимся, что скрипт использует правильный Python
RUN sed -i '1s|^.*$|#!/opt/venv/bin/python3|' /docker-entrypoint-initdb.d/fill_db.py

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 7249
ENV ASPNETCORE_URLS=http://+:7249

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Wordlie/Wordlie.csproj", "Wordlie/"]
RUN dotnet restore "Wordlie/Wordlie.csproj"
COPY . .
RUN dotnet build "Wordlie/Wordlie.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Wordlie/Wordlie.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=db /docker-entrypoint-initdb.d/fill_db.py /tmp/
ENTRYPOINT ["dotnet", "Wordlie.dll"]