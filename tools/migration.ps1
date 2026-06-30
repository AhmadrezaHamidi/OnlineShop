# ╔══════════════════════════════════════════════════════════════════╗
# ║  Migration Manager — Ahmad OnlineShop                            ║
# ║  Add · Update · Script · List · Rollback · Drop · Reset         ║
# ╚══════════════════════════════════════════════════════════════════╝

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function W-Title  { param($t) Write-Host "`n╔══ $t ══╗"  -ForegroundColor Cyan  }
function W-OK     { param($t) Write-Host "  ✅ $t"       -ForegroundColor Green }
function W-Step   { param($t) Write-Host "  ➤ $t"       -ForegroundColor White }
function W-Err    { param($t) Write-Host "  ❌ $t"       -ForegroundColor Red   }
function W-Info   { param($t) Write-Host "     $t"       -ForegroundColor Gray  }

$root        = Split-Path -Parent $PSScriptRoot
$efProject   = "$root\Src\Infrastructure\Ahmad.OnlineShop.Persistence.EF\Ahmad.OnlineShop.Persistence.EF.csproj"
$startupProj = "$root\Src\Host\Ahmad.OnlineShop.ServiceHost\Ahmad.OnlineShop.ServiceHost.csproj"
$sqlDir      = "$root\Docs\Migrations"

New-Item -ItemType Directory -Path $sqlDir -Force | Out-Null

# ── انتخاب Context ────────────────────────────────────────────────────────────
Write-Host ""
Write-Host "╔══════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║    Ahmad OnlineShop — Migration Manager  ║" -ForegroundColor Cyan
Write-Host "╚══════════════════════════════════════════╝" -ForegroundColor Cyan

W-Title "انتخاب DbContext"
Write-Host "  [1] ApplicationDbContext  (Domain — Products, Orders, BNPL)"
Write-Host "  [2] IdentityAppDbContext  (Identity — Users, OTP, Tokens)"
Write-Host "  [3] هر دو"

do {
    $ctxChoice = (Read-Host "  Context [1-3]").Trim()
    $ctxValid  = $ctxChoice -in @('1','2','3')
    if (!$ctxValid) { W-Err "عدد ۱ تا ۳ وارد کنید" }
} while (!$ctxValid)

$contexts = @()
if ($ctxChoice -in @('1','3')) { $contexts += @{ Name = "ApplicationDbContext";  MigDir = "Migrations" } }
if ($ctxChoice -in @('2','3')) { $contexts += @{ Name = "IdentityAppDbContext";  MigDir = "Migrations/Identity" } }

# ── انتخاب عملیات ────────────────────────────────────────────────────────────
W-Title "عملیات"
Write-Host "  [1] افزودن Migration جدید"
Write-Host "  [2] اعمال روی DB (Update)"
Write-Host "  [3] ساخت SQL Script (ذخیره در Docs/Migrations)"
Write-Host "  [4] لیست Migration ها"
Write-Host "  [5] Rollback"
Write-Host "  [6] Reset کامل (Drop DB + حذف Migrations + ایجاد مجدد + اعمال)"

do {
    $op = (Read-Host "  عملیات [1-6]").Trim()
    $opValid = $op -in @('1','2','3','4','5','6')
    if (!$opValid) { W-Err "عدد ۱ تا ۶ وارد کنید" }
} while (!$opValid)

# ── اجرا ─────────────────────────────────────────────────────────────────────
foreach ($ctx in $contexts) {
    $ctxName = $ctx.Name
    $migDir  = $ctx.MigDir

    W-Title "$ctxName"

    if ($op -eq '1') {
        # ─── Migration جدید ────────────────────────────────────────────────
        $name = (Read-Host "  نام Migration (مثال: AddOrderTable)").Trim()
        if ([string]::IsNullOrWhiteSpace($name)) { W-Err "نام الزامی است"; continue }
        W-Step "اضافه کردن migration ..."
        dotnet ef migrations add $name `
            --project $efProject --startup-project $startupProj `
            --context $ctxName --output-dir $migDir
        W-OK "Migration '$name' اضافه شد"
    }
    elseif ($op -eq '2') {
        # ─── Update Database ───────────────────────────────────────────────
        $confirm = (Read-Host "  اعمال روی DB؟ (yes/no)").ToLower()
        if ($confirm -ne 'yes') { W-Info "لغو شد"; continue }
        W-Step "اعمال migration ها ..."
        dotnet ef database update `
            --project $efProject --startup-project $startupProj `
            --context $ctxName
        W-OK "DB به‌روز شد"
    }
    elseif ($op -eq '3') {
        # ─── SQL Script ────────────────────────────────────────────────────
        $ts   = Get-Date -Format "yyyyMMdd_HHmmss"
        $file = "$sqlDir\${ctxName}_${ts}.sql"
        W-Step "ساخت SQL Script ..."
        dotnet ef migrations script `
            --project $efProject --startup-project $startupProj `
            --context $ctxName --idempotent --output $file
        W-OK "SQL Script ذخیره شد:"
        W-Info "$file"
        # باز کردن در notepad اگر خواست
        $open = (Read-Host "  باز کردن در Notepad? (y/n)").ToLower()
        if ($open -eq 'y') { notepad $file }
    }
    elseif ($op -eq '4') {
        # ─── List ─────────────────────────────────────────────────────────
        W-Step "لیست migration ها:"
        dotnet ef migrations list `
            --project $efProject --startup-project $startupProj `
            --context $ctxName
    }
    elseif ($op -eq '5') {
        # ─── Rollback ─────────────────────────────────────────────────────
        W-Step "Migration های موجود:"
        dotnet ef migrations list `
            --project $efProject --startup-project $startupProj `
            --context $ctxName
        $target = (Read-Host "`n  به کدام Migration rollback کنی؟").Trim()
        if ([string]::IsNullOrWhiteSpace($target)) { W-Err "نام الزامی است"; continue }
        $confirm = (Read-Host "  ⚠️ rollback پاک می‌کند — مطمئنی؟ (yes/no)").ToLower()
        if ($confirm -ne 'yes') { W-Info "لغو شد"; continue }
        dotnet ef database update $target `
            --project $efProject --startup-project $startupProj `
            --context $ctxName
        W-OK "Rollback به '$target' انجام شد"
    }
    elseif ($op -eq '6') {
        # ─── Reset کامل ───────────────────────────────────────────────────
        $confirm = (Read-Host "  ⚠️  DB حذف و از نو ساخته می‌شود — مطمئنی؟ (yes/no)").ToLower()
        if ($confirm -ne 'yes') { W-Info "لغو شد"; continue }

        W-Step "حذف DB ..."
        dotnet ef database drop --force `
            --project $efProject --startup-project $startupProj `
            --context $ctxName

        W-Step "حذف Migration files ..."
        $migPath = "$root\Src\Infrastructure\Ahmad.OnlineShop.Persistence.EF\$migDir"
        if (Test-Path $migPath) { Remove-Item "$migPath\*.cs" -Force -ErrorAction SilentlyContinue }

        W-Step "ایجاد InitialCreate ..."
        $migName = if ($ctxName -eq "ApplicationDbContext") { "InitialCreate" } else { "InitialIdentity" }
        dotnet ef migrations add $migName `
            --project $efProject --startup-project $startupProj `
            --context $ctxName --output-dir $migDir

        W-Step "اعمال روی DB ..."
        dotnet ef database update `
            --project $efProject --startup-project $startupProj `
            --context $ctxName

        W-Step "ساخت SQL Script ..."
        $ts   = Get-Date -Format "yyyyMMdd_HHmmss"
        $file = "$sqlDir\${ctxName}_${ts}.sql"
        dotnet ef migrations script `
            --project $efProject --startup-project $startupProj `
            --context $ctxName --idempotent --output $file

        W-OK "Reset کامل انجام شد"
        W-Info "SQL Script: $file"
    }
}

Write-Host ""
