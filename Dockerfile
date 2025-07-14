# Этап для PostgreSQL с инициализацией
FROM postgres:17.2 AS postgres-setup
RUN apt-get update && \
    apt-get install -y python3 python3-venv && \
    python3 -m venv /opt/venv && \
    /opt/venv/bin/pip install psycopg2-binary

COPY fill_db.py /docker-entrypoint-initdb.d/
RUN chmod +x /docker-entrypoint-initdb.d/fill_db.py && \
    sed -i '1s|^.*$|#!/opt/venv/bin/python3|' /docker-entrypoint-initdb.d/fill_db.py

# Этап для .NET runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 7249
ENV ASPNETCORE_URLS=http://+:7249

# Этап сборки .NET
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Wordlie/Wordlie.csproj", "Wordlie/"]
RUN dotnet restore "Wordlie/Wordlie.csproj"
COPY . .
RUN dotnet build "Wordlie/Wordlie.csproj" -c Release -o /app/build

# Этап публикации .NET
FROM build AS publish
RUN dotnet publish "Wordlie/Wordlie.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Финальный этап
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Wordlie.dll"]