# ╔══════════════════════════════════════════════════════════════════╗
# ║         Migration Script — Ahmad OnlineShop                     ║
# ║  نام Migration → Add → Script SQL → ذخیره در Docs → Apply؟    ║
# ╚══════════════════════════════════════════════════════════════════╝

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# ── رنگ‌ها ─────────────────────────────────────────────────────────
function Write-Header  ($msg) { Write-Host "`n══ $msg ══" -ForegroundColor Cyan   }
function Write-Step    ($msg) { Write-Host "  ➤ $msg"    -ForegroundColor White  }
function Write-Success ($msg) { Write-Host "  ✅ $msg"    -ForegroundColor Green  }
function Write-Warning ($msg) { Write-Host "  ⚠️  $msg"   -ForegroundColor Yellow }
function Write-Err     ($msg) { Write-Host "  ❌ $msg"    -ForegroundColor Red    }
function Write-Info    ($msg) { Write-Host "     $msg"    -ForegroundColor Gray   }

# ── مسیرها ─────────────────────────────────────────────────────────
$scriptDir    = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot  = Split-Path -Parent $scriptDir

$efProject    = Join-Path $projectRoot "Src\Infrastructure\Ahmad.OnlineShop.Persistence.EF\Ahmad.OnlineShop.Persistence.EF.csproj"
$startupProj  = Join-Path $projectRoot "Src\Host\Ahmad.OnlineShop.ServiceHost\Ahmad.OnlineShop.ServiceHost.csproj"
$docsFolder   = Join-Path $projectRoot "Docs\Migrations"
$timestamp    = Get-Date -Format "yyyyMMdd_HHmmss"

# ─────────────────────────────────────────────────────────────────────
Write-Host ""
Write-Host "╔══════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║   Ahmad OnlineShop — Migration Tool  ║" -ForegroundColor Cyan
Write-Host "╚══════════════════════════════════════╝" -ForegroundColor Cyan

# ── بررسی dotnet-ef ─────────────────────────────────────────────────
Write-Header "بررسی ابزارها"
Write-Step "چک dotnet-ef ..."

$efVersion = dotnet ef --version 2>$null
if ($LASTEXITCODE -ne 0 -or -not $efVersion) {
    Write-Warning "dotnet-ef نصب نیست — در حال نصب ..."
    dotnet tool install --global dotnet-ef --verbosity quiet
    if ($LASTEXITCODE -ne 0) {
        Write-Err "نصب dotnet-ef شکست خورد"
        exit 1
    }
    Write-Success "dotnet-ef نصب شد"
} else {
    Write-Success "dotnet-ef موجود است  [$efVersion]"
}

# ── بررسی وجود EF project ───────────────────────────────────────────
if (-not (Test-Path $efProject)) {
    Write-Err "EF project پیدا نشد:`n  $efProject"
    exit 1
}

# ── نمایش آخرین Migration ها ────────────────────────────────────────
Write-Header "Migration های موجود"
try {
    $migrations = dotnet ef migrations list `
        --project $efProject `
        --startup-project $startupProj `
        --no-build 2>$null

    if ($migrations) {
        $migrations | Select-Object -Last 5 | ForEach-Object { Write-Info "  $_" }
    } else {
        Write-Info "هیچ Migration ای وجود ندارد"
    }
} catch {
    Write-Info "لیست Migration ها قابل نمایش نبود"
}

# ─────────────────────────────────────────────────────────────────────
# مرحله ۱: نام Migration
# ─────────────────────────────────────────────────────────────────────
Write-Header "مرحله ۱ — نام Migration"
Write-Info "فرمت پیشنهادی: Add_OtpRequest_Table | Update_User_AddPhoneNumber"

do {
    $migrationName = (Read-Host "  نام Migration").Trim()
    $valid = $migrationName -match '^[A-Za-z][A-Za-z0-9_]{2,}$'
    if (-not $valid) {
        Write-Err "نام Migration باید با حرف شروع شود و فقط شامل A-Z، 0-9 و _ باشد (حداقل ۳ کاراکتر)"
    }
} while (-not $valid)

# ─────────────────────────────────────────────────────────────────────
# مرحله ۲: Build
# ─────────────────────────────────────────────────────────────────────
Write-Header "مرحله ۲ — Build"
Write-Step "dotnet build ..."
dotnet build $efProject --configuration Debug --verbosity quiet
if ($LASTEXITCODE -ne 0) { Write-Err "Build شکست خورد"; exit 1 }
Write-Success "Build موفق"

# ─────────────────────────────────────────────────────────────────────
# مرحله ۳: Add Migration
# ─────────────────────────────────────────────────────────────────────
Write-Header "مرحله ۳ — Add Migration"
Write-Step "dotnet ef migrations add $migrationName ..."

dotnet ef migrations add $migrationName `
    --project $efProject `
    --startup-project $startupProj `
    --verbose

if ($LASTEXITCODE -ne 0) {
    Write-Err "ساخت Migration شکست خورد"
    exit 1
}
Write-Success "Migration '$migrationName' ساخته شد"

# ─────────────────────────────────────────────────────────────────────
# مرحله ۴: Script کردن تغییرات
# ─────────────────────────────────────────────────────────────────────
Write-Header "مرحله ۴ — Script SQL تغییرات"

New-Item -ItemType Directory -Path $docsFolder -Force | Out-Null
$scriptFile = Join-Path $docsFolder "${timestamp}_${migrationName}.sql"

Write-Step "تولید SQL Script ..."

dotnet ef migrations script `
    --project $efProject `
    --startup-project $startupProj `
    --idempotent `
    --output $scriptFile `
    --no-build

if ($LASTEXITCODE -ne 0) {
    Write-Err "تولید SQL Script شکست خورد"
    exit 1
}

Write-Success "SQL Script ذخیره شد:"
Write-Info    "  $scriptFile"

# نمایش پیش‌نمایش Script
Write-Host ""
Write-Host "  ── پیش‌نمایش SQL ──────────────────────" -ForegroundColor DarkGray
Get-Content $scriptFile | Select-Object -First 20 | ForEach-Object {
    Write-Host "  $_" -ForegroundColor DarkGray
}
$totalLines = (Get-Content $scriptFile | Measure-Object -Line).Lines
if ($totalLines -gt 20) {
    Write-Host "  ... ($($totalLines - 20) خط دیگر)" -ForegroundColor DarkGray
}
Write-Host "  ──────────────────────────────────────" -ForegroundColor DarkGray

# ─────────────────────────────────────────────────────────────────────
# مرحله ۵: اعمال روی Database؟
# ─────────────────────────────────────────────────────────────────────
Write-Header "مرحله ۵ — اعمال روی Database"

Write-Host ""
Write-Host "  آیا Migration را روی Database اعمال کنی؟" -ForegroundColor White
Write-Host "  [y] بله — dotnet ef database update اجرا شود" -ForegroundColor Green
Write-Host "  [n] خیر — فقط Script ذخیره شد، بعداً اعمال کن" -ForegroundColor Gray
Write-Host ""

$applyChoice = (Read-Host "  انتخاب (y/n)").Trim().ToLower()

if ($applyChoice -eq "y" -or $applyChoice -eq "yes") {

    Write-Header "اعمال Migration روی Database"
    Write-Step  "dotnet ef database update ..."

    dotnet ef database update `
        --project $efProject `
        --startup-project $startupProj `
        --no-build `
        --verbose

    if ($LASTEXITCODE -ne 0) {
        Write-Err "اعمال Migration شکست خورد"
        Write-Info "SQL Script ذخیره شده است و می‌توانی بعداً آن را اعمال کنی:"
        Write-Info "  $scriptFile"
        exit 1
    }

    Write-Success "Migration روی Database اعمال شد"

} else {
    Write-Warning "Migration اعمال نشد"
    Write-Info   "برای اعمال بعداً دستور زیر را اجرا کن:"
    Write-Info   "  dotnet ef database update --project $efProject"
    Write-Info   "یا از SQL Script استفاده کن:"
    Write-Info   "  $scriptFile"
}

# ─────────────────────────────────────────────────────────────────────
# خلاصه
# ─────────────────────────────────────────────────────────────────────
Write-Header "خلاصه"
Write-Host ""
Write-Host "  ┌─────────────────────────────────────────" -ForegroundColor DarkGray
Write-Host "  │  Migration : $migrationName"              -ForegroundColor White
Write-Host "  │  زمان      : $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" -ForegroundColor White
Write-Host "  │  SQL Script: $scriptFile"                 -ForegroundColor White
Write-Host "  │  اعمال     : $(if ($applyChoice -eq 'y' -or $applyChoice -eq 'yes') { 'بله ✅' } else { 'خیر ⏸' })" -ForegroundColor White
Write-Host "  └─────────────────────────────────────────" -ForegroundColor DarkGray
Write-Host ""
