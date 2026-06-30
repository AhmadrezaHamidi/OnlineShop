# ╔══════════════════════════════════════════════════════════════════╗
# ║         Rollback Migration — Ahmad OnlineShop                   ║
# ║  لیست Migration ها → انتخاب هدف → Script SQL حذف → Rollback    ║
# ╚══════════════════════════════════════════════════════════════════╝

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Write-Header  ($msg) { Write-Host "`n══ $msg ══" -ForegroundColor Cyan   }
function Write-Step    ($msg) { Write-Host "  ➤ $msg"    -ForegroundColor White  }
function Write-Success ($msg) { Write-Host "  ✅ $msg"    -ForegroundColor Green  }
function Write-Warning ($msg) { Write-Host "  ⚠️  $msg"   -ForegroundColor Yellow }
function Write-Err     ($msg) { Write-Host "  ❌ $msg"    -ForegroundColor Red    }
function Write-Info    ($msg) { Write-Host "     $msg"    -ForegroundColor Gray   }

$scriptDir   = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Split-Path -Parent $scriptDir
$efProject   = Join-Path $projectRoot "Src\Infrastructure\Ahmad.OnlineShop.Persistence.EF\Ahmad.OnlineShop.Persistence.EF.csproj"
$startupProj = Join-Path $projectRoot "Src\Host\Ahmad.OnlineShop.ServiceHost\Ahmad.OnlineShop.ServiceHost.csproj"
$docsFolder  = Join-Path $projectRoot "Docs\Migrations"
$timestamp   = Get-Date -Format "yyyyMMdd_HHmmss"

Write-Host ""
Write-Host "╔══════════════════════════════════════╗" -ForegroundColor Red
Write-Host "║  Ahmad OnlineShop — Migration Rollback║" -ForegroundColor Red
Write-Host "╚══════════════════════════════════════╝" -ForegroundColor Red

# ── لیست Migration ها ───────────────────────────────────────────────
Write-Header "Migration های موجود"

$rawList = dotnet ef migrations list `
    --project $efProject `
    --startup-project $startupProj `
    --no-build 2>&1

if ($LASTEXITCODE -ne 0) {
    Write-Err "خطا در گرفتن لیست Migration — ابتدا Build کن"
    exit 1
}

# فیلتر خطوط واقعی Migration
$migrationLines = $rawList | Where-Object { $_ -match '^\d{14}_' -or $_ -match '^[A-Za-z]' }

if (-not $migrationLines -or $migrationLines.Count -eq 0) {
    Write-Warning "هیچ Migration ای وجود ندارد"
    exit 0
}

# نمایش با شماره
$migrations = @()
$i = 1
foreach ($line in $migrationLines) {
    $isApplied = $line -notmatch '\(Pending\)'
    $status    = if ($isApplied) { "✅ applied" } else { "⏸ pending" }
    $name      = ($line -replace '\s*\(Pending\)', '').Trim()
    Write-Host "  [$i] $name  $status" -ForegroundColor $(if ($isApplied) { "White" } else { "Yellow" })
    $migrations += $name
    $i++
}

Write-Host "  [0] حذف همه Migration ها (برگشت به ابتدا)" -ForegroundColor DarkRed

# ── انتخاب هدف ──────────────────────────────────────────────────────
Write-Header "انتخاب هدف Rollback"
Write-Warning "بعد از Rollback، آن Migration و همه بعد از آن حذف می‌شوند!"

do {
    $choice = (Read-Host "  شماره Migration هدف [0-$($migrations.Count)]").Trim()
    $valid  = $choice -match '^\d+$' -and [int]$choice -ge 0 -and [int]$choice -le $migrations.Count
    if (-not $valid) { Write-Err "شماره نامعتبر" }
} while (-not $valid)

$targetIdx = [int]$choice

if ($targetIdx -eq 0) {
    $targetMigration = "0"
    $targetName      = "ابتدا (حذف همه)"
} else {
    $targetMigration = $migrations[$targetIdx - 1]
    $targetName      = $targetMigration
}

# ── تأیید ──────────────────────────────────────────────────────────
Write-Host ""
Write-Warning "Rollback به: $targetName"

$migsToRemove = if ($targetIdx -eq 0) { $migrations } else { $migrations[$targetIdx..($migrations.Count - 1)] }
if ($migsToRemove.Count -gt 0) {
    Write-Warning "Migration هایی که حذف می‌شوند:"
    $migsToRemove | ForEach-Object { Write-Info "  - $_" }
}

$confirm = (Read-Host "`n  مطمئنی؟ (yes/no)").Trim().ToLower()
if ($confirm -ne "yes") { Write-Info "Rollback لغو شد"; exit 0 }

# ── Script SQL حذف ─────────────────────────────────────────────────
Write-Header "Script SQL حذف تغییرات"
New-Item -ItemType Directory -Path $docsFolder -Force | Out-Null
$scriptFile = Join-Path $docsFolder "${timestamp}_ROLLBACK_to_${targetIdx}.sql"

Write-Step "تولید SQL Script حذف ..."
dotnet ef migrations script $targetMigration `
    --project $efProject `
    --startup-project $startupProj `
    --idempotent `
    --output $scriptFile `
    --no-build

Write-Success "SQL Script ذخیره شد: $scriptFile"

# ── اعمال Rollback ─────────────────────────────────────────────────
Write-Header "اعمال Rollback روی Database"
Write-Step "dotnet ef database update $targetMigration ..."

dotnet ef database update $targetMigration `
    --project $efProject `
    --startup-project $startupProj `
    --no-build

if ($LASTEXITCODE -ne 0) { Write-Err "Rollback روی Database شکست خورد"; exit 1 }
Write-Success "Database به '$targetName' برگشت"

# ── حذف فایل‌های Migration ─────────────────────────────────────────
Write-Header "حذف فایل‌های Migration"
$migrationsDir = Join-Path $projectRoot "Src\Infrastructure\Ahmad.OnlineShop.Persistence.EF\Migrations"

foreach ($mig in $migsToRemove) {
    $files = Get-ChildItem $migrationsDir -Filter "*_${mig}*" -ErrorAction SilentlyContinue
    foreach ($f in $files) {
        Remove-Item $f.FullName -Force
        Write-Info "حذف شد: $($f.Name)"
    }
}
Write-Success "فایل‌های Migration پاکسازی شدند"

Write-Host ""
Write-Success "Rollback با موفقیت انجام شد! 🔄"
Write-Host ""
