# ╔══════════════════════════════════════════════════════════════════╗
# ║  Ahmad OnlineShop — Multi-stage Dockerfile                       ║
# ╚══════════════════════════════════════════════════════════════════╝

# ── Stage 1: Build ───────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# کپی فقط csproj ها برای cache بهتر
COPY ["Src/Host/Ahmad.OnlineShop.ServiceHost/Ahmad.OnlineShop.ServiceHost.csproj",                   "Src/Host/Ahmad.OnlineShop.ServiceHost/"]
COPY ["Src/Host/Ahmad.OnlineShop.Option/Ahmad.OnlineShop.Option.csproj",                             "Src/Host/Ahmad.OnlineShop.Option/"]
COPY ["Src/Domain/Ahmad.OnlineShop.Domain/Ahmad.OnlineShop.Domain.csproj",                           "Src/Domain/Ahmad.OnlineShop.Domain/"]
COPY ["Src/Domain/Ahmad.OnlineShop.Event/Ahmad.OnlineShop.Event.csproj",                             "Src/Domain/Ahmad.OnlineShop.Event/"]
COPY ["Src/Application/Ahmad.OnlineShop.Application/Ahmad.OnlineShop.Application.csproj",            "Src/Application/Ahmad.OnlineShop.Application/"]
COPY ["Src/Application/Ahmad.OnlineShop.Application.Contract/Ahmad.OnlineShop.Application.Contract.csproj", "Src/Application/Ahmad.OnlineShop.Application.Contract/"]
COPY ["Src/Application/Ahmad.OnlineShop.Application.Query/Ahmad.OnlineShop.Application.Query.csproj","Src/Application/Ahmad.OnlineShop.Application.Query/"]
COPY ["Src/Application/Ahmad.OnlineShop.Application.Query.Contract/Ahmad.OnlineShop.Application.Query.Contract.csproj","Src/Application/Ahmad.OnlineShop.Application.Query.Contract/"]
COPY ["Src/Application/Ahmad.OnlineShop.Event.Handler/Ahmad.OnlineShop.Event.Handler.csproj",        "Src/Application/Ahmad.OnlineShop.Event.Handler/"]
COPY ["Src/Infrastructure/Ahmad.OnlineShop.Persistence.EF/Ahmad.OnlineShop.Persistence.EF.csproj",   "Src/Infrastructure/Ahmad.OnlineShop.Persistence.EF/"]
COPY ["Src/Infrastructure/Ahmad.OnlineShop.Read.Dapper/Ahmad.OnlineShop.Read.Dapper.csproj",         "Src/Infrastructure/Ahmad.OnlineShop.Read.Dapper/"]
COPY ["Src/Infrastructure/Ahmad.OnlineShop.API.Config/Ahmad.OnlineShop.Config.csproj",               "Src/Infrastructure/Ahmad.OnlineShop.API.Config/"]
COPY ["Src/Rest/Ahmad.OnlineShop.API.Rest/Ahmad.OnlineShop.Rest.csproj",                             "Src/Rest/Ahmad.OnlineShop.API.Rest/"]

# NuGet config برای private source
COPY ["nuget.config", "."]

# Restore
RUN dotnet restore "Src/Host/Ahmad.OnlineShop.ServiceHost/Ahmad.OnlineShop.ServiceHost.csproj"

# کپی بقیه کدها
COPY . .

# Build & Publish
RUN dotnet publish "Src/Host/Ahmad.OnlineShop.ServiceHost/Ahmad.OnlineShop.ServiceHost.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

# ── Stage 2: Runtime ─────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# تنظیمات محیط
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080 \
    DOTNET_RUNNING_IN_CONTAINER=true \
    TZ=Asia/Tehran

# نصب tzdata برای timezone فارسی
RUN apt-get update && apt-get install -y --no-install-recommends tzdata \
    && rm -rf /var/lib/apt/lists/*

# کپی از build stage
COPY --from=build /app/publish .

# کاربر non-root برای امنیت
RUN adduser --disabled-password --gecos "" appuser \
    && chown -R appuser:appuser /app
USER appuser

EXPOSE 8080

ENTRYPOINT ["dotnet", "Ahmad.OnlineShop.ServiceHost.dll"]
