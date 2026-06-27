# ╔══════════════════════════════════════════════════════════════════╗
# ║         Changelog Generator — Ahmad OnlineShop                  ║
# ║  از git log → دسته‌بندی → Markdown در Docs/Changelogs           ║
# ╚══════════════════════════════════════════════════════════════════╝

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Write-Header  ($msg) { Write-Host "`n══ $msg ══" -ForegroundColor Cyan  }
function Write-Step    ($msg) { Write-Host "  ➤ $msg"    -ForegroundColor White }
function Write-Success ($msg) { Write-Host "  ✅ $msg"    -ForegroundColor Green }
function Write-Info    ($msg) { Write-Host "     $msg"    -ForegroundColor Gray  }
function Write-Err     ($msg) { Write-Host "  ❌ $msg"    -ForegroundColor Red   }

$scriptDir    = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot  = Split-Path -Parent $scriptDir
$changelogDir = Join-Path $projectRoot "Docs\Changelogs"
$date         = Get-Date -Format "yyyy-MM-dd"
$timestamp    = Get-Date -Format "yyyyMMdd_HHmmss"

Write-Host ""
Write-Host "╔══════════════════════════════════════╗" -ForegroundColor Magenta
Write-Host "║  Ahmad OnlineShop — Changelog         ║" -ForegroundColor Magenta
Write-Host "╚══════════════════════════════════════╝" -ForegroundColor Magenta

# ── بررسی git ──────────────────────────────────────────────────────
if (-not (Test-Path (Join-Path $projectRoot ".git"))) {
    Write-Err "این پوشه git repository نیست"
    exit 1
}

Set-Location $projectRoot

# ── نمایش تگ‌ها و برنچ‌ها ──────────────────────────────────────────
Write-Header "وضعیت Repository"

$currentBranch = git rev-parse --abbrev-ref HEAD
Write-Info "برنچ فعلی: $currentBranch"

$tags = git tag --sort=-version:refname 2>$null | Select-Object -First 10
if ($tags) {
    Write-Info "آخرین تگ‌ها:"
    $tags | ForEach-Object { Write-Info "  $_" }
}

# ── محدوده کامیت‌ها ─────────────────────────────────────────────────
Write-Header "محدوده Changelog"
Write-Host "  [1] از آخرین تگ تا الان (پیشنهادی)"
Write-Host "  [2] بین دو تگ مشخص"
Write-Host "  [3] از برنچ‌ای تا برنچ دیگر"
Write-Host "  [4] N کامیت آخر"

do {
    $rangeChoice = (Read-Host "  انتخاب [1/2/3/4]").Trim()
} while ($rangeChoice -notin @("1","2","3","4"))

$fromRef = ""
$toRef   = "HEAD"
$version = "Unreleased"

switch ($rangeChoice) {
    "1" {
        $lastTag = git describe --tags --abbrev=0 2>$null
        if (-not $lastTag) {
            Write-Info "هیچ تگی وجود ندارد — از ابتدای history"
            $fromRef = ""
        } else {
            $fromRef = $lastTag
            Write-Info "از: $lastTag → تا: HEAD"
        }
    }
    "2" {
        $tags | ForEach-Object { Write-Info "  $_" }
        $fromRef = (Read-Host "  تگ شروع (از)").Trim()
        $toRef   = (Read-Host "  تگ پایان (تا) [Enter=HEAD]").Trim()
        if (-not $toRef) { $toRef = "HEAD" }
        $version = $toRef
    }
    "3" {
        $fromRef = (Read-Host "  برنچ شروع (مثال: develop)").Trim()
        $toRef   = (Read-Host "  برنچ پایان (مثال: main) [Enter=HEAD]").Trim()
        if (-not $toRef) { $toRef = "HEAD" }
    }
    "4" {
        $n       = (Read-Host "  چند کامیت آخر؟").Trim()
        $fromRef = "HEAD~$n"
    }
}

# ── نسخه Changelog ─────────────────────────────────────────────────
$releaseVersion = (Read-Host "`n  نسخه release (مثال: v1.2.0) [Enter=Unreleased]").Trim()
if (-not $releaseVersion) { $releaseVersion = "Unreleased" }

# ── گرفتن کامیت‌ها ──────────────────────────────────────────────────
Write-Header "خواندن کامیت‌ها از git"

$gitRange = if ($fromRef) { "$fromRef..$toRef" } else { $toRef }
$format   = "--pretty=format:%H|%s|%an|%ad"
$commits  = git log $gitRange $format --date=short 2>$null

if (-not $commits) {
    Write-Err "کامیتی در این محدوده پیدا نشد"
    exit 0
}

$allCommits = $commits | ForEach-Object {
    $parts = $_ -split '\|', 4
    [PSCustomObject]@{
        Hash    = $parts[0].Substring(0, 7)
        Message = $parts[1]
        Author  = $parts[2]
        Date    = $parts[3]
    }
}

Write-Info "$($allCommits.Count) کامیت پیدا شد"

# ── دسته‌بندی کامیت‌ها (Conventional Commits) ──────────────────────
$categories = [ordered]@{
    "✨ Features"       = @()
    "🐛 Bug Fixes"      = @()
    "⚡ Performance"    = @()
    "♻️  Refactoring"   = @()
    "🧪 Tests"          = @()
    "📖 Docs"           = @()
    "🔧 Config/Chore"   = @()
    "🗄️  Database"      = @()
    "🔒 Security"       = @()
    "📦 Dependencies"   = @()
    "🔀 Other"          = @()
}

$prefixMap = @{
    "feat"     = "✨ Features"
    "feature"  = "✨ Features"
    "fix"      = "🐛 Bug Fixes"
    "bugfix"   = "🐛 Bug Fixes"
    "perf"     = "⚡ Performance"
    "refactor" = "♻️  Refactoring"
    "test"     = "🧪 Tests"
    "tests"    = "🧪 Tests"
    "docs"     = "📖 Docs"
    "doc"      = "📖 Docs"
    "chore"    = "🔧 Config/Chore"
    "ci"       = "🔧 Config/Chore"
    "build"    = "🔧 Config/Chore"
    "style"    = "🔧 Config/Chore"
    "db"       = "🗄️  Database"
    "migration"= "🗄️  Database"
    "security" = "🔒 Security"
    "deps"     = "📦 Dependencies"
}

foreach ($commit in $allCommits) {
    $msg     = $commit.Message
    $matched = $false

    foreach ($prefix in $prefixMap.Keys) {
        if ($msg -match "^$prefix(\(.+?\))?[:\s]" -or $msg -match "^\[$prefix\]") {
            $clean   = $msg -replace "^$prefix(\(.+?\))?[:\s]+", "" -replace "^\[$prefix\]\s*", ""
            $scope   = if ($msg -match "\((.+?)\)") { " **[$($matches[1])]**" } else { "" }
            $categories[$prefixMap[$prefix]] += "- $clean$scope  \`[$($commit.Hash)\`] _$($commit.Date)_"
            $matched = $true
            break
        }
    }

    if (-not $matched) {
        $categories["🔀 Other"] += "- $msg  \`[$($commit.Hash)\`] _$($commit.Date)_"
    }
}

# ── تولید Markdown ──────────────────────────────────────────────────
Write-Header "تولید Changelog"
New-Item -ItemType Directory -Path $changelogDir -Force | Out-Null
$outputFile = Join-Path $changelogDir "${timestamp}_${releaseVersion -replace '[^a-zA-Z0-9._-]','_'}.md"

$md = @"
# Changelog — $releaseVersion

> تاریخ: $date
> محدوده: ``$gitRange``
> تعداد کامیت: $($allCommits.Count)

---

"@

foreach ($cat in $categories.Keys) {
    if ($categories[$cat].Count -gt 0) {
        $md += "`n## $cat`n`n"
        $md += ($categories[$cat] -join "`n") + "`n"
    }
}

$md += "`n---`n_Generated by Ahmad.OnlineShop tools/changelog.ps1 on $date_`n"

$md | Out-File $outputFile -Encoding utf8
Write-Success "Changelog ذخیره شد: $outputFile"

# ── نمایش خلاصه ────────────────────────────────────────────────────
Write-Host ""
foreach ($cat in $categories.Keys) {
    if ($categories[$cat].Count -gt 0) {
        Write-Host "  $cat : $($categories[$cat].Count) مورد" -ForegroundColor White
    }
}

# ── باز کردن فایل ──────────────────────────────────────────────────
$open = (Read-Host "`n  فایل را باز کنی؟ (y/n)").ToLower()
if ($open -eq "y") { Start-Process $outputFile }

Write-Host ""
Write-Success "Changelog آماده است! 📋"
Write-Host ""
