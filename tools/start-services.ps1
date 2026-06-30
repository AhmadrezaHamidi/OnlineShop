# ╔══════════════════════════════════════════════════════════════════╗
# ║  Start Dev Services — Redis + MinIO                              ║
# ║  اجرا: .\tools\start-services.ps1                               ║
# ╚══════════════════════════════════════════════════════════════════╝

$toolsDir  = "C:\Tools\OnlineShop"
$redisExe  = "$toolsDir\Redis\redis-server.exe"
$miniExe   = "$toolsDir\minio.exe"
$minioData = "$toolsDir\minio-data"

New-Item -ItemType Directory -Path $minioData -Force | Out-Null

function W-OK  { param($t) Write-Host "  ✅ $t" -ForegroundColor Green  }
function W-Err { param($t) Write-Host "  ❌ $t" -ForegroundColor Red    }
function W-Step{ param($t) Write-Host "  ➤ $t"  -ForegroundColor Cyan   }

Write-Host ""
Write-Host "╔══════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║     Ahmad OnlineShop — Dev Services      ║" -ForegroundColor Cyan
Write-Host "╚══════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

# ── Redis ─────────────────────────────────────────────────────────────────────
W-Step "شروع Redis روی port 6379 ..."
if (-not (Test-Path $redisExe)) { W-Err "redis-server.exe پیدا نشد: $redisExe"; exit 1 }

$redisRunning = Get-NetTCPConnection -LocalPort 6379 -ErrorAction SilentlyContinue
if ($redisRunning) {
    W-OK "Redis قبلاً در حال اجراست (port 6379)"
} else {
    Start-Process -FilePath $redisExe -WindowStyle Minimized
    Start-Sleep 2
    $check = Get-NetTCPConnection -LocalPort 6379 -ErrorAction SilentlyContinue
    if ($check) { W-OK "Redis بالا آمد ✓" }
    else         { W-Err "Redis شروع نشد" }
}

# ── MinIO ─────────────────────────────────────────────────────────────────────
W-Step "شروع MinIO روی port 9000 (Console: 9001) ..."
if (-not (Test-Path $miniExe)) { W-Err "minio.exe پیدا نشد: $miniExe"; exit 1 }

$minioRunning = Get-NetTCPConnection -LocalPort 9000 -ErrorAction SilentlyContinue
if ($minioRunning) {
    W-OK "MinIO قبلاً در حال اجراست (port 9000)"
} else {
    $env:MINIO_ROOT_USER     = "minioadmin"
    $env:MINIO_ROOT_PASSWORD = "minioadmin123"

    Start-Process -FilePath $miniExe `
        -ArgumentList "server $minioData --console-address :9001" `
        -WindowStyle Minimized
    Start-Sleep 3

    $check = Get-NetTCPConnection -LocalPort 9000 -ErrorAction SilentlyContinue
    if ($check) { W-OK "MinIO بالا آمد ✓" }
    else         { W-Err "MinIO شروع نشد" }
}

Write-Host ""
Write-Host "╔══════════════════════════════════════════╗" -ForegroundColor Green
Write-Host "║  ✅ سرویس‌ها فعال هستند                   ║" -ForegroundColor Green
Write-Host "╠══════════════════════════════════════════╣" -ForegroundColor Green
Write-Host "║  Redis  : localhost:6379                  ║" -ForegroundColor Green
Write-Host "║  MinIO  : http://localhost:9000           ║" -ForegroundColor Green
Write-Host "║  MinIO  : http://localhost:9001  (UI)     ║" -ForegroundColor Green
Write-Host "║  User   : minioadmin                      ║" -ForegroundColor Green
Write-Host "║  Pass   : minioadmin123                   ║" -ForegroundColor Green
Write-Host "╚══════════════════════════════════════════╝" -ForegroundColor Green
Write-Host ""
