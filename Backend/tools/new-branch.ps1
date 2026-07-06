# ╔══════════════════════════════════════════════════════════════════╗
# ║           Branch Creator — Ahmad OnlineShop                     ║
# ║  شماره Jira → اسم تسک → انتخاب برنچ پایه → ساخت برنچ          ║
# ╚══════════════════════════════════════════════════════════════════╝

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# ── رنگ‌ها ─────────────────────────────────────────────────────────
function Write-Header  ($msg) { Write-Host "`n$msg" -ForegroundColor Cyan   }
function Write-Success ($msg) { Write-Host "✅ $msg" -ForegroundColor Green  }
function Write-Warning ($msg) { Write-Host "⚠️  $msg" -ForegroundColor Yellow }
function Write-Err     ($msg) { Write-Host "❌ $msg" -ForegroundColor Red    }
function Write-Info    ($msg) { Write-Host "   $msg" -ForegroundColor Gray   }

# ── اطمینان از بودن در git repo ────────────────────────────────────
if (-not (Test-Path ".git")) {
    Set-Location (git rev-parse --show-toplevel 2>$null)
}

Write-Header "═══ Branch Creator ═══"

# ─────────────────────────────────────────────────────────────────────
# مرحله ۱: شماره تسک Jira
# فرمت مورد انتظار: XXX_1234  (حروف بزرگ، زیرخط، عدد)
# ─────────────────────────────────────────────────────────────────────
Write-Header "مرحله ۱ — شماره تسک Jira"
Write-Info   "فرمت: ABC_1234  (مثال: AP_1234 | SHOP_567 | ONL_89)"

do {
    $taskNumber = (Read-Host "  شماره تسک").Trim().ToUpper()
    $valid = $taskNumber -match '^[A-Z]{2,6}_\d{1,6}$'
    if (-not $valid) {
        Write-Err "فرمت اشتباه! باید مثل  AP_1234  باشد (حروف انگلیسی + زیرخط + عدد)"
    }
} while (-not $valid)

# ─────────────────────────────────────────────────────────────────────
# مرحله ۲: اسم تسک
# برنچ نهایی: AP_1234_fix_otp_login
# ─────────────────────────────────────────────────────────────────────
Write-Header "مرحله ۲ — اسم تسک"
Write-Info   "فاصله‌ها به _ تبدیل می‌شوند  (مثال: fix otp login → fix_otp_login)"

do {
    $rawName = (Read-Host "  اسم تسک").Trim()
    $valid = $rawName.Length -ge 2
    if (-not $valid) {
        Write-Err "اسم تسک نمی‌تواند خالی باشد"
    }
} while (-not $valid)

# نرمال‌سازی: فاصله/خط‌تیره → زیرخط، حروف کوچک
$taskName   = $rawName -replace '[\s\-]+', '_'
$taskName   = $taskName.ToLower() -replace '[^a-z0-9_]', ''
$branchName = "${taskNumber}_${taskName}"

Write-Info "برنچ: $branchName"

# ─────────────────────────────────────────────────────────────────────
# مرحله ۳: انتخاب برنچ پایه
# ─────────────────────────────────────────────────────────────────────
Write-Header "مرحله ۳ — برنچ پایه"

do {
    $base = (Read-Host "  از کدوم برنچ شروع کنی؟ [develop / main]").Trim().ToLower()
    $valid = $base -eq 'develop' -or $base -eq 'main'
    if (-not $valid) {
        Write-Err "فقط  develop  یا  main  قابل قبول است"
    }
} while (-not $valid)

# ─────────────────────────────────────────────────────────────────────
# مرحله ۴: چک تداخل develop با main
# اگر develop از main واگرایی داشت → main به عنوان مرجع
# ─────────────────────────────────────────────────────────────────────
$actualBase = $base

if ($base -eq 'develop') {
    Write-Header "بررسی تداخل develop با main ..."

    try {
        $mainTip    = git rev-parse --verify main    2>$null
        $developTip = git rev-parse --verify develop 2>$null
        $mergeBase  = git merge-base main develop    2>$null

        if (-not $mainTip -or -not $developTip) {
            Write-Warning "یکی از برنچ‌ها پیدا نشد → از develop ادامه می‌دهیم"
        }
        elseif ($mergeBase -eq $mainTip) {
            # develop از main جلوتر است یا برابر → مشکلی نیست
            Write-Success "develop به‌روز است — بدون تداخل با main"
        }
        else {
            # main کامیت‌هایی دارد که در develop نیست → خطر تداخل
            $aheadMain = (git rev-list develop..main | Measure-Object).Count
            Write-Warning "main شامل $aheadMain کامیت جدید است که در develop نیست"
            Write-Warning "احتمال تداخل → مرجع به main تغییر کرد"
            $actualBase = 'main'
        }
    }
    catch {
        Write-Warning "بررسی تداخل ممکن نشد → از develop ادامه می‌دهیم"
    }
}

# ─────────────────────────────────────────────────────────────────────
# مرحله ۵: pull و ساخت برنچ
# ─────────────────────────────────────────────────────────────────────
Write-Header "مرحله ۵ — pull و ساخت برنچ از  '$actualBase'"

try {
    Write-Info "سوئیچ به $actualBase ..."
    git checkout $actualBase

    Write-Info "pull آخرین تغییرات ..."
    git pull origin $actualBase

    Write-Info "ساخت برنچ جدید ..."
    git checkout -b $branchName

    Write-Host ""
    Write-Success "برنچ  '$branchName'  از  '$actualBase'  با موفقیت ساخته شد!"
    Write-Host ""
    Write-Host "  git push -u origin $branchName" -ForegroundColor DarkGray
    Write-Host ""
}
catch {
    Write-Err "خطا: $_"
    exit 1
}
