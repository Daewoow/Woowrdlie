# First stage: PostgreSQL with initialization
FROM postgres:17.2 AS db
RUN apt-get update && \
    apt-get install -y python3 python3-pip && \
    pip3 install psycopg2-binary
COPY fill_db.py /docker-entrypoint-initdb.d/

# Second stage: ASP.NET Core runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 7249
ENV ASPNETCORE_URLS=http://+:7249

# Third stage: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Wordlie/Wordlie.csproj", "Wordlie/"]
RUN dotnet restore "Wordlie/Wordlie.csproj"
COPY . .
RUN dotnet build "Wordlie/Wordlie.csproj" -c Release -o /app/build

# Fourth stage: Publish
FROM build AS publish
RUN dotnet publish "Wordlie/Wordlie.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=db /docker-entrypoint-initdb.d/fill_db.py /tmp/
ENTRYPOINT ["dotnet", "Wordlie.dll"]