# ╔══════════════════════════════════════════════════════════════════╗
# ║         Seed Database — Ahmad OnlineShop                        ║
# ║  محیط → انتخاب داده‌های اولیه → Insert مستقیم به DB            ║
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

# ── Connection String از appsettings ────────────────────────────────
function Get-ConnectionString($env) {
    $appsettings = Join-Path $projectRoot "Src\Host\Ahmad.OnlineShop.ServiceHost\appsettings.$env.json"
    if (-not (Test-Path $appsettings)) {
        $appsettings = Join-Path $projectRoot "Src\Host\Ahmad.OnlineShop.ServiceHost\appsettings.json"
    }
    $json = Get-Content $appsettings | ConvertFrom-Json
    return $json.ConnectionStrings.DefaultConnection
}

# ── اجرای SQL روی SQL Server ───────────────────────────────────────
function Invoke-Sql($connectionString, $sql, $description) {
    Write-Step $description
    try {
        $conn = New-Object System.Data.SqlClient.SqlConnection($connectionString)
        $conn.Open()
        $cmd         = $conn.CreateCommand()
        $cmd.CommandText = $sql
        $affected    = $cmd.ExecuteNonQuery()
        $conn.Close()
        Write-Success "$description  ($affected ردیف)"
        return $true
    } catch {
        Write-Err "$description شکست خورد: $($_.Exception.Message)"
        return $false
    }
}

# ─────────────────────────────────────────────────────────────────────
Write-Host ""
Write-Host "╔══════════════════════════════════════╗" -ForegroundColor Green
Write-Host "║    Ahmad OnlineShop — Database Seeder ║" -ForegroundColor Green
Write-Host "╚══════════════════════════════════════╝" -ForegroundColor Green

# ── انتخاب محیط ────────────────────────────────────────────────────
Write-Header "محیط"
Write-Host "  [1] Development  ← پیشنهادی" -ForegroundColor Cyan
Write-Host "  [2] Staging"                  -ForegroundColor Yellow
Write-Host "  [3] Production   ← ممنوع!"    -ForegroundColor Red

do {
    $envChoice = (Read-Host "  انتخاب [1/2/3]").Trim()
} while ($envChoice -notin @("1","2","3"))

$envName = @{ "1"="Development"; "2"="Staging"; "3"="Production" }[$envChoice]

if ($envChoice -eq "3") {
    Write-Err "Seed روی Production ممنوع است!"
    exit 1
}

# ── گرفتن Connection String ─────────────────────────────────────────
$connStr = Get-ConnectionString $envName
if (-not $connStr) {
    Write-Warning "ConnectionString پیدا نشد در appsettings — دستی وارد کن:"
    $connStr = Read-Host "  Connection String"
}
Write-Info "محیط: $envName"

# ── انتخاب داده‌های Seed ────────────────────────────────────────────
Write-Header "انتخاب داده‌های Seed"
Write-Host "  [1] همه (پیشنهادی)"         -ForegroundColor Cyan
Write-Host "  [2] فقط ادمین اولیه"
Write-Host "  [3] فقط دسته‌بندی‌ها"
Write-Host "  [4] فقط نقش‌های سیستم"

do {
    $dataChoice = (Read-Host "  انتخاب [1/2/3/4]").Trim()
} while ($dataChoice -notin @("1","2","3","4"))

$seedAdmin    = $dataChoice -in @("1","2")
$seedCategory = $dataChoice -in @("1","3")
$seedRoles    = $dataChoice -in @("1","4")

# ── SQL های Seed ────────────────────────────────────────────────────

$sqlRoles = @"
IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'SuperAdmin')
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (1, 'SuperAdmin', 'SUPERADMIN', NEWID());

IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'Customer')
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (2, 'Customer', 'CUSTOMER', NEWID());

IF NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Name = 'Seller')
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp)
    VALUES (3, 'Seller', 'SELLER', NEWID());
"@

$adminPhone = "09120000000"
$sqlAdmin = @"
-- ادمین اولیه در BackOffice
IF NOT EXISTS (SELECT 1 FROM AdminUsers WHERE Email = 'admin@onlineshop.ir')
BEGIN
    INSERT INTO AdminUsers (Id, FullName, Email, Role, Status, CreatedAt)
    VALUES (1, 'ادمین اول', 'admin@onlineshop.ir', 0, 0, GETUTCDATE());
    PRINT 'ادمین اولیه ساخته شد — admin@onlineshop.ir';
END
ELSE
    PRINT 'ادمین از قبل وجود دارد';
"@

$sqlCategories = @"
IF NOT EXISTS (SELECT 1 FROM Categories WHERE Name = 'الکترونیک')
BEGIN
    INSERT INTO Categories (Id, Name, ParentId) VALUES (1, 'الکترونیک', NULL);
    INSERT INTO Categories (Id, Name, ParentId) VALUES (2, 'موبایل',    1);
    INSERT INTO Categories (Id, Name, ParentId) VALUES (3, 'لپ‌تاپ',    1);
    INSERT INTO Categories (Id, Name, ParentId) VALUES (4, 'لوازم خانگی', NULL);
    INSERT INTO Categories (Id, Name, ParentId) VALUES (5, 'پوشاک',    NULL);
END
"@

# ── اجرا ────────────────────────────────────────────────────────────
Write-Header "اجرای Seed"

if ($seedRoles)    { Invoke-Sql $connStr $sqlRoles    "ایجاد نقش‌های سیستم" | Out-Null }
if ($seedAdmin)    { Invoke-Sql $connStr $sqlAdmin    "ایجاد ادمین اولیه"   | Out-Null }
if ($seedCategory) { Invoke-Sql $connStr $sqlCategories "ایجاد دسته‌بندی‌ها" | Out-Null }

Write-Host ""
Write-Success "Seed با موفقیت انجام شد! 🌱"
Write-Host ""
