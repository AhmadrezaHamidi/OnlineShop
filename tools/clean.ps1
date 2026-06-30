# ╔══════════════════════════════════════════════════════════════════╗
# ║         Clean — Ahmad OnlineShop                                ║
# ║  حذف bin/obj در کل solution + publish های قدیمی               ║
# ╚══════════════════════════════════════════════════════════════════╝

Set-StrictMode -Version Latest

function Write-Header  ($msg) { Write-Host "`n══ $msg ══" -ForegroundColor Cyan  }
function Write-Step    ($msg) { Write-Host "  ➤ $msg"    -ForegroundColor White }
function Write-Success ($msg) { Write-Host "  ✅ $msg"    -ForegroundColor Green }
function Write-Info    ($msg) { Write-Host "     $msg"    -ForegroundColor Gray  }

$scriptDir   = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Split-Path -Parent $scriptDir

Write-Host ""
Write-Host "╔══════════════════════════════════════╗" -ForegroundColor DarkGray
Write-Host "║    Ahmad OnlineShop — Clean           ║" -ForegroundColor DarkGray
Write-Host "╚══════════════════════════════════════╝" -ForegroundColor DarkGray

# ── انتخاب سطح ─────────────────────────────────────────────────────
Write-Header "سطح پاکسازی"
Write-Host "  [1] فقط bin/obj  (سریع)"
Write-Host "  [2] bin/obj + publish قدیمی"
Write-Host "  [3] همه + NuGet cache local"

do {
    $choice = (Read-Host "  انتخاب [1/2/3]").Trim()
} while ($choice -notin @("1","2","3"))

$totalSize = 0
$count     = 0

# ─── تابع حذف ────────────────────────────────────────────────────────
function Remove-Folder($path, $label) {
    if (Test-Path $path) {
        $size = (Get-ChildItem $path -Recurse -ErrorAction SilentlyContinue |
                 Measure-Object -Property Length -Sum -ErrorAction SilentlyContinue).Sum
        Remove-Item $path -Recurse -Force -ErrorAction SilentlyContinue
        if ($null -eq $size) { $size = 0 }
        $script:totalSize += $size
        $script:count++
        Write-Info "حذف شد: $label  ($([math]::Round($size/1MB, 1)) MB)"
    }
}

# ── bin / obj در کل Src و Test ──────────────────────────────────────
Write-Header "پاکسازی bin و obj"

foreach ($folder in @("Src", "Test")) {
    $base = Join-Path $projectRoot $folder
    Get-ChildItem $base -Recurse -Directory -ErrorAction SilentlyContinue |
        Where-Object { $_.Name -eq "bin" -or $_.Name -eq "obj" } |
        ForEach-Object { Remove-Folder $_.FullName "$folder\...\$($_.Name)" }
}

Write-Step "dotnet clean ..."
dotnet clean "$projectRoot\Ahmad.OnlineShop.sln" --verbosity quiet 2>$null
Write-Success "bin/obj پاک شد"

# ── publish های قدیمی ───────────────────────────────────────────────
if ($choice -in @("2","3")) {
    Write-Header "پاکسازی publish قدیمی"
    $publishDir = Join-Path $projectRoot "publish"

    if (Test-Path $publishDir) {
        $folders = Get-ChildItem $publishDir -Directory |
                   Sort-Object CreationTime -Descending

        if ($folders.Count -le 2) {
            Write-Info "فقط $($folders.Count) publish وجود دارد — حذف نشد (آخرین‌ها نگه داشته می‌شوند)"
        } else {
            # نگه داشتن ۲ تای آخر
            $folders | Select-Object -Skip 2 | ForEach-Object {
                Remove-Folder $_.FullName "publish\$($_.Name)"
            }
            Write-Success "publish های قدیمی حذف شدند (۲ آخرین نگه داشته شد)"
        }
    } else {
        Write-Info "پوشه publish وجود ندارد"
    }
}

# ── NuGet cache local ───────────────────────────────────────────────
if ($choice -eq "3") {
    Write-Header "پاکسازی NuGet Cache"
    $nugetTemp = Join-Path $projectRoot ".nuget"
    if (Test-Path $nugetTemp) {
        Remove-Folder $nugetTemp ".nuget (local cache)"
    }
    Write-Info "NuGet global cache حذف نشد (مشترک بین پروژه‌هاست)"
}

# ── خلاصه ───────────────────────────────────────────────────────────
Write-Header "خلاصه"
Write-Host ""
Write-Host "  ┌──────────────────────────────────" -ForegroundColor DarkGray
Write-Host "  │  پوشه‌های حذف‌شده: $count"        -ForegroundColor White
Write-Host "  │  فضای آزادشده   : $([math]::Round($totalSize/1MB, 1)) MB" -ForegroundColor White
Write-Host "  └──────────────────────────────────" -ForegroundColor DarkGray
Write-Host ""
Write-Success "پاکسازی تموم شد! 🧹"
Write-Host ""
