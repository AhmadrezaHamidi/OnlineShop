# ╔══════════════════════════════════════════════════════════════════╗
# ║         Test Runner — Ahmad OnlineShop                          ║
# ║  انتخاب project → اجرا → Coverage → گزارش HTML در Docs         ║
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
$testDir     = Join-Path $projectRoot "Test"
$reportsDir  = Join-Path $projectRoot "Docs\TestResults"
$timestamp   = Get-Date -Format "yyyyMMdd_HHmmss"

$projects = @{
    "1" = @{ Name = "Domain Tests";      Path = "$testDir\Ahmad.OnlineShop.Domain.Tests\Ahmad.OnlineShop.Domain.Tests.csproj" }
    "2" = @{ Name = "Application Tests"; Path = "$testDir\Ahmad.OnlineShop.Application.Tests\Ahmad.OnlineShop.Application.Tests.csproj" }
    "3" = @{ Name = "Integration Tests"; Path = "$testDir\Ahmad.OnlineShop.Integration.Tests\Ahmad.OnlineShop.Integration.Tests.csproj" }
}

Write-Host ""
Write-Host "╔══════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║    Ahmad OnlineShop — Test Runner     ║" -ForegroundColor Cyan
Write-Host "╚══════════════════════════════════════╝" -ForegroundColor Cyan

# ── انتخاب project ─────────────────────────────────────────────────
Write-Header "انتخاب Test Project"
Write-Host "  [1] Domain Tests      ← تست‌های Domain (سریع)"   -ForegroundColor White
Write-Host "  [2] Application Tests ← تست‌های Handler"          -ForegroundColor White
Write-Host "  [3] Integration Tests ← تست‌های یکپارچه"          -ForegroundColor White
Write-Host "  [4] همه"                                           -ForegroundColor Cyan

do {
    $choice = (Read-Host "  انتخاب [1/2/3/4]").Trim()
} while ($choice -notin @("1","2","3","4"))

$toRun = if ($choice -eq "4") { $projects.Values } else { @($projects[$choice]) }

# ── انتخاب Coverage ────────────────────────────────────────────────
Write-Header "گزارش Coverage"
$withCoverage = (Read-Host "  گزارش Coverage هم بگیری؟ (y/n)").Trim().ToLower()
$coverageEnabled = $withCoverage -eq "y" -or $withCoverage -eq "yes"

# ── اجرای تست‌ها ────────────────────────────────────────────────────
$totalPassed  = 0
$totalFailed  = 0
$totalSkipped = 0
$results      = @()

foreach ($proj in $toRun) {
    Write-Header "اجرا — $($proj.Name)"

    if (-not (Test-Path $proj.Path)) {
        Write-Warning "پیدا نشد: $($proj.Path)"
        continue
    }

    $reportFolder = Join-Path $reportsDir "${timestamp}_$($proj.Name -replace ' ','_')"
    New-Item -ItemType Directory -Path $reportFolder -Force | Out-Null

    $coverageArgs = if ($coverageEnabled) {
        @("--collect:XPlat Code Coverage", "--results-directory:$reportFolder")
    } else {
        @("--results-directory:$reportFolder")
    }

    Write-Step "dotnet test ..."
    $output = dotnet test $proj.Path `
        --configuration Release `
        --logger "console;verbosity=minimal" `
        --logger "trx;LogFileName=results.trx" `
        @coverageArgs `
        2>&1

    # parse نتیجه
    $passLine  = $output | Select-String "Passed:\s+(\d+)"  | Select-Object -Last 1
    $failLine  = $output | Select-String "Failed:\s+(\d+)"  | Select-Object -Last 1
    $skipLine  = $output | Select-String "Skipped:\s+(\d+)" | Select-Object -Last 1
    $totalLine = $output | Select-String "Total:\s+(\d+)"   | Select-Object -Last 1

    $passed  = if ($passLine)  { [int]($passLine.Matches[0].Groups[1].Value)  } else { 0 }
    $failed  = if ($failLine)  { [int]($failLine.Matches[0].Groups[1].Value)  } else { 0 }
    $skipped = if ($skipLine)  { [int]($skipLine.Matches[0].Groups[1].Value)  } else { 0 }
    $total   = if ($totalLine) { [int]($totalLine.Matches[0].Groups[1].Value) } else { 0 }

    $totalPassed  += $passed
    $totalFailed  += $failed
    $totalSkipped += $skipped

    if ($failed -gt 0) {
        Write-Err   "$($proj.Name): $failed شکست / $total کل"
        # نمایش تست‌های fail شده
        $output | Where-Object { $_ -match 'Failed\s+\w' } |
            Select-Object -First 10 |
            ForEach-Object { Write-Info "  $_" }
    } else {
        Write-Success "$($proj.Name): $passed/$total پاس ✓"
    }

    $results += [PSCustomObject]@{
        Project  = $proj.Name
        Passed   = $passed
        Failed   = $failed
        Skipped  = $skipped
        Total    = $total
        Report   = $reportFolder
    }
}

# ── Coverage Report با ReportGenerator ─────────────────────────────
if ($coverageEnabled) {
    Write-Header "تولید گزارش HTML"

    $rgInstalled = dotnet tool list -g | Select-String "reportgenerator"
    if (-not $rgInstalled) {
        Write-Step "نصب ReportGenerator ..."
        dotnet tool install --global dotnet-reportgenerator-globaltool --verbosity quiet
    }

    $coverageFiles = Get-ChildItem $reportsDir -Recurse -Filter "coverage.cobertura.xml" |
                     Where-Object { $_.FullName -like "*$timestamp*" } |
                     Select-Object -ExpandProperty FullName

    if ($coverageFiles) {
        $htmlReport = Join-Path $reportsDir "${timestamp}_Coverage"
        reportgenerator `
            -reports:($coverageFiles -join ";") `
            -targetdir:$htmlReport `
            -reporttypes:Html `
            -verbosity:Error

        Write-Success "گزارش HTML: $htmlReport\index.html"

        # باز کردن در browser
        $openBrowser = (Read-Host "  گزارش را در browser باز کنی؟ (y/n)").ToLower()
        if ($openBrowser -eq "y") {
            Start-Process "$htmlReport\index.html"
        }
    }
}

# ── خلاصه نهایی ────────────────────────────────────────────────────
Write-Header "نتیجه نهایی"
Write-Host ""

$results | ForEach-Object {
    $icon = if ($_.Failed -gt 0) { "❌" } else { "✅" }
    Write-Host "  $icon  $($_.Project.PadRight(25)) Passed: $($_.Passed)  Failed: $($_.Failed)  Total: $($_.Total)"
}

Write-Host ""
Write-Host "  ┌──────────────────────────────────────" -ForegroundColor DarkGray
Write-Host "  │  جمع Passed : $totalPassed"            -ForegroundColor Green
Write-Host "  │  جمع Failed : $totalFailed"            -ForegroundColor $(if ($totalFailed -gt 0) { "Red" } else { "Green" })
Write-Host "  │  جمع Skipped: $totalSkipped"           -ForegroundColor Yellow
Write-Host "  │  گزارش‌ها   : $reportsDir"             -ForegroundColor Gray
Write-Host "  └──────────────────────────────────────" -ForegroundColor DarkGray
Write-Host ""

if ($totalFailed -gt 0) {
    Write-Err   "$totalFailed تست شکست خورد!"
    exit 1
} else {
    Write-Success "همه تست‌ها پاس شدند! 🎉"
}
