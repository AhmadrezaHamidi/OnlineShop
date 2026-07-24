#Requires -Version 5.1
<#
.SYNOPSIS
    Deploy آخرین نسخه پروژه به سرور از طریق SSH

.DESCRIPTION
    با یک دستور:  .\tools\publish.ps1 -Server 1.2.3.4 -User deploy -Password s3cr3t
    اتصال SSH می‌گیره، آخرین نسخه git رو pull می‌کنه،
    build می‌کنه و سرویس رو restart می‌کنه.

.EXAMPLE
    # Docker (پیشفرض)
    .\tools\publish.ps1 -Server 1.2.3.4 -User root -Password Passw0rd

    # Docker با branch خاص
    .\tools\publish.ps1 -Server 1.2.3.4 -User deploy -Password Passw0rd -Branch release/v2

    # Native (systemctl)
    .\tools\publish.ps1 -Server 1.2.3.4 -User deploy -Password Passw0rd -Mode native -ServiceName onlineshop-api

    # Key-based (no password)
    .\tools\publish.ps1 -Server 1.2.3.4 -User deploy -KeyFile ~/.ssh/id_rsa
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory)] [string] $Server,
    [Parameter(Mandatory)] [string] $User,
    [string]  $Password    = "",       # خالی = key-based SSH
    [string]  $KeyFile     = "",       # مسیر SSH private key
    [int]     $Port        = 22,
    [string]  $Branch      = "main",
    [string]  $RemoteDir   = "/opt/onlineshop",
    [ValidateSet("docker","native")]
    [string]  $Mode        = "docker",
    [string]  $ServiceName = "onlineshop-api",  # فقط در mode=native
    [string]  $ComposeFile = "docker-compose.yml",
    [switch]  $DryRun      # فقط نمایش دستورات — اجرا نمی‌کنه
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

# ── رنگ‌ها ──────────────────────────────────────────────────────────────────
function Step   { param($m) Write-Host "`n▶  $m" -ForegroundColor Cyan    }
function OK     { param($m) Write-Host "   ✅ $m" -ForegroundColor Green   }
function Warn   { param($m) Write-Host "   ⚠️  $m" -ForegroundColor Yellow  }
function Fail   { param($m) Write-Host "   ❌ $m" -ForegroundColor Red     }
function Info   { param($m) Write-Host "   ℹ️  $m" -ForegroundColor Gray    }

# ── بنر ─────────────────────────────────────────────────────────────────────
Write-Host ""
Write-Host "╔══════════════════════════════════════════════════════════╗" -ForegroundColor Magenta
Write-Host "║          Ahmad OnlineShop — Remote Deploy               ║" -ForegroundColor Magenta
Write-Host "╚══════════════════════════════════════════════════════════╝" -ForegroundColor Magenta
Write-Host "  سرور  : $User@$Server`:$Port" -ForegroundColor White
Write-Host "  Branch : $Branch"              -ForegroundColor White
Write-Host "  Mode   : $Mode"                -ForegroundColor White
Write-Host "  Dir    : $RemoteDir"           -ForegroundColor White
if ($DryRun) { Write-Host "  [DRY RUN — هیچ دستوری اجرا نمی‌شود]" -ForegroundColor Yellow }
Write-Host ""

# ── تشخیص SSH ابزار ─────────────────────────────────────────────────────────
Step "تشخیص SSH client..."

$sshExe   = $null
$plinkExe = $null

if ($Password -ne "") {
    # با password: به plink نیاز داریم (PuTTY CLI) یا sshpass
    $plinkExe = Get-Command "plink.exe" -ErrorAction SilentlyContinue |
                Select-Object -ExpandProperty Source

    if ($null -eq $plinkExe) {
        $plinkExe = Get-Command "plink" -ErrorAction SilentlyContinue |
                    Select-Object -ExpandProperty Source
    }

    if ($null -ne $plinkExe) {
        OK "plink یافت شد: $plinkExe"
    } else {
        Warn "plink یافت نشد — تلاش با ssh (احتمالاً پسورد رد می‌شود)"
        Warn "برای password SSH روی Windows: PuTTY نصب کنید  →  https://www.putty.org"
        $sshExe = Get-Command "ssh" -ErrorAction SilentlyContinue |
                  Select-Object -ExpandProperty Source
        if ($null -eq $sshExe) {
            Fail "هیچ SSH client یافت نشد. لطفاً OpenSSH یا PuTTY نصب کنید."
            exit 1
        }
        OK "ssh یافت شد: $sshExe"
    }
} else {
    $sshExe = Get-Command "ssh" -ErrorAction SilentlyContinue |
              Select-Object -ExpandProperty Source
    if ($null -eq $sshExe) {
        Fail "OpenSSH نصب نیست. در PowerShell بزنید: Add-WindowsCapability -Online -Name OpenSSH.Client~~~~0.0.1.0"
        exit 1
    }
    OK "ssh یافت شد: $sshExe"
}

# ── تابع اجرای SSH ──────────────────────────────────────────────────────────
function Invoke-Remote {
    param([string]$Command, [string]$Description = "")

    if ($Description) { Info "  → $Description" }

    if ($DryRun) {
        Write-Host "      [DRY] $Command" -ForegroundColor DarkGray
        return
    }

    if ($null -ne $plinkExe) {
        # plink با password
        $args = @("-ssh", "-P", $Port, "-l", $User, "-pw", $Password,
                  "-batch", "-no-antispoof", "$Server", $Command)
        & $plinkExe @args
    } elseif ($KeyFile -ne "") {
        # ssh با key file
        $args = @("-i", $KeyFile, "-p", $Port, "-o", "StrictHostKeyChecking=no",
                  "$User@$Server", $Command)
        & $sshExe @args
    } else {
        # ssh بدون password (key-based از agent یا config)
        $args = @("-p", $Port, "-o", "StrictHostKeyChecking=no",
                  "$User@$Server", $Command)
        & $sshExe @args
    }

    if ($LASTEXITCODE -ne 0 -and !$DryRun) {
        Fail "دستور ناموفق بود (exit: $LASTEXITCODE): $Command"
        exit $LASTEXITCODE
    }
}

# ── ۱. تست اتصال ────────────────────────────────────────────────────────────
Step "[1/5] تست اتصال به سرور..."
Invoke-Remote "echo 'SSH OK: '\$(hostname)" "تست اتصال"
OK "اتصال SSH برقرار است"

# ── ۲. بررسی پوشه پروژه ─────────────────────────────────────────────────────
Step "[2/5] بررسی پوشه پروژه روی سرور..."
Invoke-Remote "test -d '$RemoteDir' || { echo 'ERROR: $RemoteDir وجود ندارد'; exit 1; }" "بررسی مسیر"
Invoke-Remote "cd '$RemoteDir' && git status --short" "وضعیت git"

# ── ۳. Pull آخرین نسخه ──────────────────────────────────────────────────────
Step "[3/5] Pull آخرین نسخه از branch $Branch..."
Invoke-Remote @"
cd '$RemoteDir' && \
git fetch origin && \
git checkout $Branch && \
git reset --hard origin/$Branch && \
git log -1 --format='commit: %H%nتاریخ: %ci%nپیام: %s'
"@ "git pull --force"
OK "کد سرور به روز شد"

# ── ۴. Deploy ────────────────────────────────────────────────────────────────
if ($Mode -eq "docker") {
    Step "[4/5] Docker Compose — rebuild و restart..."

    # .env فایل باید روی سرور وجود داشته باشد
    Invoke-Remote @"
cd '$RemoteDir' && \
if [ ! -f .env ]; then
  if [ -f .env.example ]; then
    cp .env.example .env
    echo 'WARN: .env از .env.example کپی شد — مقادیر را تنظیم کنید'
  else
    echo 'ERROR: فایل .env وجود ندارد'
    exit 1
  fi
fi
"@ "بررسی .env"

    Invoke-Remote @"
cd '$RemoteDir' && \
docker compose -f $ComposeFile pull --ignore-pull-failures 2>/dev/null || true && \
docker compose -f $ComposeFile up -d --build $ServiceName && \
docker compose -f $ComposeFile ps
"@ "docker compose up"

} else {
    # ── Native deploy: dotnet publish → copy → restart ───────────────────────
    Step "[4/5] dotnet publish روی سرور..."

    Invoke-Remote @"
cd '$RemoteDir' && \
export DOTNET_CLI_TELEMETRY_OPTOUT=1 && \
dotnet restore Ahmad.OnlineShop.sln --verbosity quiet && \
dotnet publish Src/Host/Ahmad.OnlineShop.ServiceHost/Ahmad.OnlineShop.ServiceHost.csproj \
  -c Release -o /var/www/onlineshop --no-restore --verbosity quiet
"@ "dotnet publish"

    Invoke-Remote @"
systemctl restart $ServiceName && \
sleep 2 && \
systemctl status $ServiceName --no-pager -l
"@ "restart سرویس"
}

OK "Deploy کامل شد"

# ── ۵. Health Check ──────────────────────────────────────────────────────────
Step "[5/5] Health Check..."

if ($Mode -eq "docker") {
    Invoke-Remote @"
cd '$RemoteDir' && \
sleep 3 && \
docker compose -f $ComposeFile ps --format 'table {{.Name}}\t{{.Status}}\t{{.Ports}}'
"@ "وضعیت container ها"
} else {
    Invoke-Remote @"
sleep 2 && \
curl -sf http://localhost:8080/health 2>/dev/null && echo 'Health: OK' || echo 'Health endpoint در دسترس نیست'
"@ "health check"
}

# ── نتیجه ───────────────────────────────────────────────────────────────────
Write-Host ""
Write-Host "══════════════════════════════════════════════════════════" -ForegroundColor Green
Write-Host "  ✅  Deploy با موفقیت تکمیل شد!" -ForegroundColor Green
Write-Host "══════════════════════════════════════════════════════════" -ForegroundColor Green
Write-Host "  سرور  : $User@$Server"
Write-Host "  Branch : $Branch"
Write-Host "  Mode   : $Mode"
Write-Host ""
if ($Mode -eq "docker") {
    Write-Host "  برای لاگ‌های زنده روی سرور:"
    Write-Host "  ssh $User@$Server `"cd $RemoteDir && docker compose logs -f api`"" -ForegroundColor Gray
} else {
    Write-Host "  برای لاگ‌های سرویس:"
    Write-Host "  ssh $User@$Server `"journalctl -u $ServiceName -f`"" -ForegroundColor Gray
}
Write-Host ""
