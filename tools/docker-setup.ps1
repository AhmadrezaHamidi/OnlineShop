# ╔══════════════════════════════════════════════════════════════════╗
# ║  Ahmad OnlineShop — Docker Setup                                  ║
# ║  همه چیز رو از صفر راه می‌اندازد:                                ║
# ║  SQL Server · MinIO · Redis · API                                 ║
# ╚══════════════════════════════════════════════════════════════════╝
#
# اجرا:
#   .\tools\docker-setup.ps1              ← همه چیز
#   .\tools\docker-setup.ps1 -SkipBuild   ← بدون build
#   .\tools\docker-setup.ps1 -Down        ← خاموش کن

param(
    [switch]$SkipBuild,
    [switch]$Down,
    [switch]$Logs,
    [string]$Service = ""
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$root = Split-Path -Parent $PSScriptRoot

function W-Title { param($t) Write-Host "`n╔══ $t ══╗" -ForegroundColor Cyan  }
function W-OK    { param($t) Write-Host "  ✅ $t"    -ForegroundColor Green  }
function W-Step  { param($t) Write-Host "  ➤ $t"    -ForegroundColor White  }
function W-Err   { param($t) Write-Host "  ❌ $t"    -ForegroundColor Red    }
function W-Info  { param($t) Write-Host "     $t"    -ForegroundColor Gray   }

Set-Location $root

# ── .env موجود باشه ──────────────────────────────────────────────────────────
if (-not (Test-Path ".env")) {
    if (Test-Path ".env.example") {
        W-Step "ساخت .env از .env.example ..."
        Copy-Item ".env.example" ".env"
        W-Info "⚠️  مقادیر پیش‌فرض استفاده شد — لطفاً .env را ویرایش کنید"
    } else {
        W-Err ".env وجود ندارد — فایل .env.example یافت نشد"
        exit 1
    }
}

# ── Down ──────────────────────────────────────────────────────────────────────
if ($Down) {
    W-Title "خاموش کردن همه سرویس‌ها"
    docker compose down -v
    W-OK "همه container ها متوقف شدند"
    exit 0
}

# ── Logs ──────────────────────────────────────────────────────────────────────
if ($Logs) {
    if ($Service) { docker compose logs -f $Service }
    else          { docker compose logs -f }
    exit 0
}

# ── بررسی Docker ─────────────────────────────────────────────────────────────
W-Title "بررسی Docker"
try {
    docker info | Out-Null
    W-OK "Docker در حال اجرا است"
} catch {
    W-Err "Docker در حال اجرا نیست — لطفاً Docker Desktop را روشن کنید"
    exit 1
}

# ── Build Image ───────────────────────────────────────────────────────────────
if (-not $SkipBuild) {
    W-Title "Build کردن Image"
    W-Step "dotnet build ..."
    dotnet build "Src\Host\Ahmad.OnlineShop.ServiceHost\Ahmad.OnlineShop.ServiceHost.csproj" -c Release --nologo -q
    if ($LASTEXITCODE -ne 0) { W-Err "Build ناموفق"; exit 1 }
    W-OK "Build موفق"

    W-Step "docker compose build ..."
    docker compose build --no-cache
    if ($LASTEXITCODE -ne 0) { W-Err "Docker build ناموفق"; exit 1 }
    W-OK "Docker image آماده شد"
}

# ── Start Infrastructure (بدون API اول) ──────────────────────────────────────
W-Title "راه‌اندازی Infrastructure"
W-Step "شروع SQL Server + MinIO + Redis ..."
docker compose up -d sqlserver minio redis
if ($LASTEXITCODE -ne 0) { W-Err "خطا در شروع سرویس‌ها"; exit 1 }

# ── منتظر SQL Server ──────────────────────────────────────────────────────────
W-Step "منتظر آماده شدن SQL Server ..."
$maxWait = 60
$waited  = 0
do {
    Start-Sleep 3
    $waited += 3
    $status = docker inspect --format='{{.State.Health.Status}}' onlineshop-sql 2>$null
    W-Info "SQL Server: $status ($waited/$maxWait s)"
    if ($waited -ge $maxWait) { W-Err "SQL Server در $maxWait ثانیه آماده نشد"; exit 1 }
} while ($status -ne "healthy")
W-OK "SQL Server آماده است"

# ── MinIO Bucket ──────────────────────────────────────────────────────────────
W-Step "ساخت bucket محصولات در MinIO ..."
Start-Sleep 3
$env_content = Get-Content ".env" | Where-Object { $_ -match "=" }
$envVars = @{}
foreach ($line in $env_content) {
    $parts = $line -split "=", 2
    if ($parts.Count -eq 2) { $envVars[$parts[0].Trim()] = $parts[1].Trim() }
}

$minioUser = $envVars["MINIO_ACCESS_KEY"]
$minioPass = $envVars["MINIO_SECRET_KEY"]
if ([string]::IsNullOrEmpty($minioUser)) { $minioUser = "minioadmin" }
if ([string]::IsNullOrEmpty($minioPass)) { $minioPass = "minioadmin" }

# mc alias + bucket
docker exec onlineshop-minio sh -c "mc alias set local http://localhost:9000 $minioUser $minioPass && mc mb --ignore-existing local/products && mc anonymous set public local/products" 2>$null
W-OK "MinIO bucket 'products' آماده است"
W-Info "MinIO Console: http://localhost:9001  (user: $minioUser)"

# ── Start API ─────────────────────────────────────────────────────────────────
W-Title "راه‌اندازی API"
docker compose up -d api
if ($LASTEXITCODE -ne 0) { W-Err "خطا در شروع API"; exit 1 }

# ── منتظر API ─────────────────────────────────────────────────────────────────
W-Step "منتظر آماده شدن API ..."
$waited = 0
do {
    Start-Sleep 3
    $waited += 3
    try {
        $r = Invoke-RestMethod "http://localhost:8080/health" -ErrorAction SilentlyContinue
        break
    } catch { }
    W-Info "API: در حال شروع ... ($waited s)"
    if ($waited -ge 40) { W-Err "API در 40 ثانیه آماده نشد"; break }
} while ($true)
W-OK "API آماده است"

# ── خلاصه ────────────────────────────────────────────────────────────────────
Write-Host ""
Write-Host "╔═══════════════════════════════════════════════════╗" -ForegroundColor Green
Write-Host "║         ✅ همه سرویس‌ها فعال هستند                 ║" -ForegroundColor Green
Write-Host "╠═══════════════════════════════════════════════════╣" -ForegroundColor Green
Write-Host "║  🌐 API      : http://localhost:8080               ║" -ForegroundColor Green
Write-Host "║  📖 Swagger  : http://localhost:8080/swagger       ║" -ForegroundColor Green
Write-Host "║  🗄  SQL      : localhost:1433                      ║" -ForegroundColor Green
Write-Host "║  📦 MinIO    : http://localhost:9001               ║" -ForegroundColor Green
Write-Host "║  ⚡ Redis    : localhost:6379                       ║" -ForegroundColor Green
Write-Host "╠═══════════════════════════════════════════════════╣" -ForegroundColor Green
Write-Host "║  Dev Flow   : .\tools\run-dev-flow.ps1            ║" -ForegroundColor Cyan
Write-Host "║  Logs       : .\tools\docker-setup.ps1 -Logs      ║" -ForegroundColor Cyan
Write-Host "║  Stop       : .\tools\docker-setup.ps1 -Down      ║" -ForegroundColor Cyan
Write-Host "╚═══════════════════════════════════════════════════╝" -ForegroundColor Green
Write-Host ""
