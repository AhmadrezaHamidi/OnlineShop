# ╔══════════════════════════════════════════════════════════════════╗
# ║           Deploy Script — Ahmad OnlineShop                      ║
# ║  انتخاب محیط → Build → Test → Publish → Deploy                  ║
# ╚══════════════════════════════════════════════════════════════════╝

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# ── رنگ‌ها ─────────────────────────────────────────────────────────
function Write-Header  ($msg) { Write-Host "`n══ $msg ══" -ForegroundColor Cyan   }
function Write-Step    ($msg) { Write-Host "  ➤ $msg"   -ForegroundColor White   }
function Write-Success ($msg) { Write-Host "  ✅ $msg"   -ForegroundColor Green   }
function Write-Warning ($msg) { Write-Host "  ⚠️  $msg"  -ForegroundColor Yellow  }
function Write-Err     ($msg) { Write-Host "  ❌ $msg"   -ForegroundColor Red     }
function Write-Info    ($msg) { Write-Host "     $msg"   -ForegroundColor Gray    }

# ── مسیر root پروژه ─────────────────────────────────────────────────
$scriptDir   = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Split-Path -Parent $scriptDir

$solution    = Join-Path $projectRoot "Ahmad.OnlineShop.sln"
$serviceHost = Join-Path $projectRoot "Src\Host\Ahmad.OnlineShop.ServiceHost\Ahmad.OnlineShop.ServiceHost.csproj"
$publishDir  = Join-Path $projectRoot "publish"

# ── محیط‌های موجود ──────────────────────────────────────────────────
$environments = @{
    "1" = @{ Name = "Development"; Short = "dev";  Color = "Cyan"   }
    "2" = @{ Name = "Staging";     Short = "stg";  Color = "Yellow" }
    "3" = @{ Name = "Production";  Short = "prod"; Color = "Red"    }
}

# ─────────────────────────────────────────────────────────────────────
Write-Host ""
Write-Host "╔══════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║    Ahmad OnlineShop — Deployer       ║" -ForegroundColor Cyan
Write-Host "╚══════════════════════════════════════╝" -ForegroundColor Cyan

# ── بررسی وجود solution ─────────────────────────────────────────────
if (-not (Test-Path $solution)) {
    Write-Err "Solution پیدا نشد: $solution"
    exit 1
}

# ─────────────────────────────────────────────────────────────────────
# مرحله ۱: انتخاب محیط
# ─────────────────────────────────────────────────────────────────────
Write-Header "مرحله ۱ — محیط Deploy"
Write-Host "  [1] Development" -ForegroundColor Cyan
Write-Host "  [2] Staging"     -ForegroundColor Yellow
Write-Host "  [3] Production"  -ForegroundColor Red

do {
    $choice = (Read-Host "  انتخاب کن [1/2/3]").Trim()
    $valid  = $environments.ContainsKey($choice)
    if (-not $valid) { Write-Err "فقط 1، 2 یا 3 قابل قبول است" }
} while (-not $valid)

$env      = $environments[$choice]
$envName  = $env.Name
$envShort = $env.Short

Write-Host ""
Write-Host "  محیط انتخابی: $envName" -ForegroundColor $env.Color

# ── هشدار Production ─────────────────────────────────────────────────
if ($envShort -eq "prod") {
    Write-Host ""
    Write-Warning "در حال Deploy به PRODUCTION هستی!"
    $confirm = (Read-Host "  مطمئنی؟ (yes/no)").Trim().ToLower()
    if ($confirm -ne "yes") {
        Write-Info "Deploy لغو شد."
        exit 0
    }
}

$timestamp    = Get-Date -Format "yyyyMMdd_HHmmss"
$outputFolder = Join-Path $publishDir "${envShort}_${timestamp}"

# ─────────────────────────────────────────────────────────────────────
# مرحله ۲: Restore
# ─────────────────────────────────────────────────────────────────────
Write-Header "مرحله ۲ — Restore"
Write-Step "dotnet restore ..."
try {
    dotnet restore $solution --verbosity quiet
    Write-Success "Restore موفق"
} catch {
    Write-Err "Restore شکست خورد: $_"; exit 1
}

# ─────────────────────────────────────────────────────────────────────
# مرحله ۳: Build
# ─────────────────────────────────────────────────────────────────────
Write-Header "مرحله ۳ — Build ($envName)"
Write-Step "dotnet build ..."
try {
    dotnet build $solution `
        --configuration Release `
        --no-restore `
        -p:EnvironmentName=$envName `
        --verbosity quiet
    Write-Success "Build موفق"
} catch {
    Write-Err "Build شکست خورد: $_"; exit 1
}

# ─────────────────────────────────────────────────────────────────────
# مرحله ۴: Test
# ─────────────────────────────────────────────────────────────────────
Write-Header "مرحله ۴ — Unit Tests"

$runTests = "y"
if ($envShort -eq "prod") {
    $runTests = (Read-Host "  تست‌ها اجرا شود؟ (y/n)").Trim().ToLower()
}

if ($runTests -eq "y" -or $runTests -eq "yes") {
    Write-Step "dotnet test ..."
    try {
        $testResult = dotnet test $solution `
            --configuration Release `
            --no-build `
            --verbosity quiet `
            --logger "console;verbosity=minimal" 2>&1

        $failed = $testResult | Select-String "Failed:"
        if ($failed) {
            Write-Err "تست‌ها شکست خوردند:"
            $failed | ForEach-Object { Write-Info $_.Line }
            $continueOnFail = (Read-Host "  با وجود شکست تست ادامه دهی؟ (yes/no)").ToLower()
            if ($continueOnFail -ne "yes") { exit 1 }
        } else {
            $passed = ($testResult | Select-String "Passed").Line
            Write-Success "همه تست‌ها پاس شدند  [$passed]"
        }
    } catch {
        Write-Warning "خطا در اجرای تست: $_"
    }
} else {
    Write-Warning "تست‌ها رد شدند"
}

# ─────────────────────────────────────────────────────────────────────
# مرحله ۵: Publish
# ─────────────────────────────────────────────────────────────────────
Write-Header "مرحله ۵ — Publish"
Write-Step "خروجی: $outputFolder"

try {
    New-Item -ItemType Directory -Path $outputFolder -Force | Out-Null

    dotnet publish $serviceHost `
        --configuration Release `
        --no-build `
        --output $outputFolder `
        -p:EnvironmentName=$envName `
        --verbosity quiet

    Write-Success "Publish موفق → $outputFolder"
} catch {
    Write-Err "Publish شکست خورد: $_"; exit 1
}

# ─────────────────────────────────────────────────────────────────────
# مرحله ۶: اطلاعات خروجی
# ─────────────────────────────────────────────────────────────────────
Write-Header "خلاصه Deploy"

$size = (Get-ChildItem $outputFolder -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB

Write-Host ""
Write-Host "  ┌─────────────────────────────────────────" -ForegroundColor DarkGray
Write-Host "  │  محیط    : $envName"                      -ForegroundColor White
Write-Host "  │  زمان    : $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" -ForegroundColor White
Write-Host "  │  مسیر    : $outputFolder"                 -ForegroundColor White
Write-Host "  │  حجم     : $([math]::Round($size, 1)) MB" -ForegroundColor White
Write-Host "  └─────────────────────────────────────────" -ForegroundColor DarkGray
Write-Host ""
Write-Success "Deploy به $envName با موفقیت انجام شد! 🚀"
Write-Host ""
