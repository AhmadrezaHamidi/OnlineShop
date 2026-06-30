# ╔══════════════════════════════════════════════════════════════════╗
# ║         PR / Merge — Ahmad OnlineShop                           ║
# ║  Merge برنچ feature → develop/main → push → حذف برنچ           ║
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
Set-Location $projectRoot

Write-Host ""
Write-Host "╔══════════════════════════════════════╗" -ForegroundColor Magenta
Write-Host "║    Ahmad OnlineShop — PR / Merge      ║" -ForegroundColor Magenta
Write-Host "╚══════════════════════════════════════╝" -ForegroundColor Magenta

# ── بررسی working tree تمیز ──────────────────────────────────────────
$status = git status --porcelain
if ($status) {
    Write-Warning "تغییرات commit نشده وجود دارد:"
    $status | ForEach-Object { Write-Info "  $_" }
    $cont = (Read-Host "`n  ادامه دهی؟ (y/n)").ToLower()
    if ($cont -ne "y") { exit 0 }
}

# ── وضعیت فعلی ────────────────────────────────────────────────────
$currentBranch = git rev-parse --abbrev-ref HEAD
Write-Header "وضعیت"
Write-Info "برنچ فعلی: $currentBranch"

# ── انتخاب برنچ source ────────────────────────────────────────────
Write-Header "برنچ Source (که می‌خوای merge کنی)"

$allBranches = git branch --list | ForEach-Object { $_.Trim() -replace '^\* ', '' }
$i = 1
$branchMap = @{}
$allBranches | ForEach-Object {
    $marker = if ($_ -eq $currentBranch) { " ← فعلی" } else { "" }
    Write-Host "  [$i] $_$marker"
    $branchMap["$i"] = $_
    $i++
}

do {
    $srcChoice = (Read-Host "  شماره برنچ source").Trim()
} while (-not $branchMap.ContainsKey($srcChoice))
$sourceBranch = $branchMap[$srcChoice]

# ── انتخاب برنچ target ────────────────────────────────────────────
Write-Header "برنچ Target (که merge می‌شه توش)"
Write-Host "  [1] develop  (پیشنهادی)"
Write-Host "  [2] main"
Write-Host "  [3] برنچ دیگر"

do {
    $tgtChoice = (Read-Host "  انتخاب [1/2/3]").Trim()
} while ($tgtChoice -notin @("1","2","3"))

$targetBranch = switch ($tgtChoice) {
    "1" { "develop" }
    "2" { "main"    }
    "3" { (Read-Host "  نام برنچ").Trim() }
}

# ── نمایش کامیت‌هایی که merge می‌شن ────────────────────────────────
Write-Header "کامیت‌هایی که merge می‌شوند"
$commitList = git log "$targetBranch..$sourceBranch" --oneline 2>$null
if (-not $commitList) {
    Write-Warning "هیچ کامیتی برای merge وجود ندارد — شاید از قبل merge شده"
    exit 0
}
$commitList | ForEach-Object { Write-Info "  $_" }

# ── تأیید ─────────────────────────────────────────────────────────
Write-Host ""
Write-Warning "Merge: $sourceBranch  →  $targetBranch"
$confirm = (Read-Host "  ادامه دهی؟ (yes/no)").ToLower()
if ($confirm -ne "yes") { Write-Info "لغو شد"; exit 0 }

# ── pull target ───────────────────────────────────────────────────
Write-Header "آپدیت $targetBranch"
Write-Step "checkout $targetBranch ..."
git checkout $targetBranch

Write-Step "pull origin $targetBranch ..."
git pull origin $targetBranch

# ── Merge ─────────────────────────────────────────────────────────
Write-Header "Merge"
Write-Step "git merge --no-ff $sourceBranch ..."

git merge --no-ff $sourceBranch -m "Merge branch '$sourceBranch' into $targetBranch"

if ($LASTEXITCODE -ne 0) {
    Write-Err "Merge با conflict مواجه شد!"
    Write-Info "فایل‌های conflict:"
    git diff --name-only --diff-filter=U | ForEach-Object { Write-Info "  $_" }
    Write-Info "Conflict ها را resolve کن و دستور زیر را اجرا کن:"
    Write-Info "  git merge --continue"
    exit 1
}
Write-Success "Merge موفق"

# ── Push ──────────────────────────────────────────────────────────
Write-Header "Push"
Write-Step "git push origin $targetBranch ..."
git push origin $targetBranch
Write-Success "Push موفق"

# ── حذف برنچ feature ─────────────────────────────────────────────
Write-Header "حذف برنچ"
$deleteBranch = (Read-Host "  برنچ '$sourceBranch' را حذف کنی؟ (y/n)").ToLower()

if ($deleteBranch -eq "y") {
    Write-Step "حذف local ..."
    git branch -d $sourceBranch

    $deleteRemote = (Read-Host "  از remote هم حذف شود؟ (y/n)").ToLower()
    if ($deleteRemote -eq "y") {
        Write-Step "حذف remote origin/$sourceBranch ..."
        git push origin --delete $sourceBranch
        Write-Success "برنچ از remote حذف شد"
    }
    Write-Success "برنچ '$sourceBranch' حذف شد"
}

# ── خلاصه ─────────────────────────────────────────────────────────
Write-Header "خلاصه"
Write-Host ""
Write-Host "  ┌──────────────────────────────────────────" -ForegroundColor DarkGray
Write-Host "  │  Source : $sourceBranch"                   -ForegroundColor White
Write-Host "  │  Target : $targetBranch"                   -ForegroundColor White
Write-Host "  │  Push   : ✅"                              -ForegroundColor Green
Write-Host "  │  حذف    : $(if ($deleteBranch -eq 'y') { '✅' } else { '⏸ نه' })" -ForegroundColor White
Write-Host "  └──────────────────────────────────────────" -ForegroundColor DarkGray
Write-Host ""
Write-Success "PR / Merge با موفقیت انجام شد! 🎯"
Write-Host ""
