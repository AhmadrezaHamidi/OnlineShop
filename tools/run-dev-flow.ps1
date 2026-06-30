# ╔══════════════════════════════════════════════════════════════════╗
# ║  Ahmad OnlineShop — Dev Flow Runner                               ║
# ║  Project رو Run می‌کنه + همه Flow های اصلی رو تست می‌کنه          ║
# ║                                                                    ║
# ║  پیش‌نیاز: SQL Server محلی فعال باشه                              ║
# ║  OTP در Development همیشه: 00000                                  ║
# ╚══════════════════════════════════════════════════════════════════╝

param(
    [string]$BaseUrl    = "http://localhost:5000",
    [switch]$StartServer                          # سرور رو هم Start کنه
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$root       = Split-Path -Parent $PSScriptRoot
$serverProj = "$root\Src\Host\Ahmad.OnlineShop.ServiceHost\Ahmad.OnlineShop.ServiceHost.csproj"

# ── رنگ‌ها ──────────────────────────────────────────────────────────────────
function W-Header { param($t) Write-Host "`n╔══ $t ══╗" -ForegroundColor Cyan }
function W-OK     { param($t) Write-Host "  ✅ $t"     -ForegroundColor Green  }
function W-Err    { param($t) Write-Host "  ❌ $t"     -ForegroundColor Red    }
function W-Step   { param($t) Write-Host "  ➤ $t"     -ForegroundColor White  }
function W-Info   { param($t) Write-Host "     $t"     -ForegroundColor Gray   }

# ── Start Server ─────────────────────────────────────────────────────────────
if ($StartServer) {
    W-Header "شروع سرور"
    $env:ASPNETCORE_ENVIRONMENT = "Development"
    $serverJob = Start-Job -ScriptBlock {
        param($proj)
        dotnet run --project $proj --environment Development 2>&1
    } -ArgumentList $serverProj

    W-Step "منتظر آماده شدن سرور (10 ثانیه) ..."
    Start-Sleep 10
}

# ── Helper: API Call ──────────────────────────────────────────────────────────
function Call {
    param(
        [string]$Method,
        [string]$Url,
        [object]$Body,
        [string]$Token,
        [string]$Label
    )
    W-Step $Label
    $headers = @{ "Content-Type" = "application/json" }
    if ($Token) { $headers["Authorization"] = "Bearer $Token" }

    try {
        $bodyJson = if ($Body) { $Body | ConvertTo-Json -Compress } else { $null }
        $resp = Invoke-RestMethod -Method $Method -Uri "$BaseUrl/$Url" `
                    -Headers $headers -Body $bodyJson -ErrorAction Stop
        W-OK "$Label → OK"
        return $resp
    } catch {
        $code = $_.Exception.Response.StatusCode.value__
        W-Err "$Label → $code : $($_.Exception.Message)"
        return $null
    }
}

Write-Host ""
Write-Host "╔══════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║  Ahmad OnlineShop — Dev Flow Tester      ║" -ForegroundColor Cyan
Write-Host "║  Base URL: $BaseUrl" -ForegroundColor Cyan
Write-Host "╚══════════════════════════════════════════╝" -ForegroundColor Cyan

# ═══════════════════════════════════════════════════════════════════════════════
W-Header "Flow 1: مشتری جدید — ثبت‌نام و ورود"
# ───────────────────────────────────────────────────────────────────────────────

$newPhone = "091$(Get-Random -Minimum 10000000 -Maximum 99999999)"
W-Info "موبایل جدید: $newPhone"

# ۱. ارسال OTP
$r = Call POST "api/v1/Identity/Auth/Login" @{ phoneNumber = $newPhone } -Label "ارسال OTP (مشتری جدید)"

# ۲. تأیید OTP → JWT
$login = Call POST "api/v1/Identity/Auth/Login/verify" @{
    phoneNumber = $newPhone; code = "00000"
} -Label "تأیید OTP با کد 00000"

$customerToken = $login?.accessToken
if ($customerToken) {
    W-OK "JWT دریافت شد ✓"
    W-Info "UserId: $($login.userId)"
} else {
    W-Err "Login ناموفق — ادامه ممکن نیست"
    exit 1
}

# ۳. ثبت نام (نام)
Call PATCH "api/v1/Identity/Users/$($login.userId)/profile" @{ fullName = "مشتری تست" } `
    -Token $customerToken -Label "ثبت نام کامل (UpdateProfile)"

# ═══════════════════════════════════════════════════════════════════════════════
W-Header "Flow 2: ادمین — ورود"
# ───────────────────────────────────────────────────────────────────────────────

Call POST "api/v1/Identity/Auth/Login" @{ phoneNumber = "09000000001" } -Label "ارسال OTP (ادمین)"
$adminLogin = Call POST "api/v1/Identity/Auth/Login/verify" @{
    phoneNumber = "09000000001"; code = "00000"
} -Label "تأیید OTP ادمین"

$adminToken = $adminLogin?.accessToken
if ($adminToken) { W-OK "JWT ادمین دریافت شد ✓" }

# ═══════════════════════════════════════════════════════════════════════════════
W-Header "Flow 3: فروشنده — ورود + محصول جدید"
# ───────────────────────────────────────────────────────────────────────────────

Call POST "api/v1/Identity/Auth/Login" @{ phoneNumber = "09000000002" } -Label "ارسال OTP (فروشنده)"
$sellerLogin = Call POST "api/v1/Identity/Auth/Login/verify" @{
    phoneNumber = "09000000002"; code = "00000"
} -Label "تأیید OTP فروشنده"

$sellerToken = $sellerLogin?.accessToken
$sellerId    = $sellerLogin?.userId

if ($sellerToken) {
    W-OK "JWT فروشنده دریافت شد ✓"

    # ایجاد محصول
    $product = Call POST "api/v1/Seller/Products" @{
        sellerId    = $sellerId
        name        = "محصول تست $(Get-Random -Max 999)"
        description = "محصول تست برای Dev Flow"
        price       = 150000
        categoryId  = 1
        quantity    = 100
    } -Token $sellerToken -Label "ایجاد محصول جدید"

    $productId = $product?.id ?? $product?.productId
    W-Info "ProductId: $productId"
}

# ═══════════════════════════════════════════════════════════════════════════════
W-Header "Flow 4: مشتری — سفارش"
# ───────────────────────────────────────────────────────────────────────────────

if ($customerToken -and $productId) {
    $order = Call POST "api/v1/Orders" @{
        userId        = $login.userId
        paymentMethod = 1
    } -Token $customerToken -Label "ایجاد سفارش"

    $orderId = $order?.id ?? $order?.orderId
    W-Info "OrderId: $orderId"

    if ($orderId) {
        Call POST "api/v1/Orders/$orderId/items" @{
            productId = $productId
            quantity  = 2
            unitPrice = 150000
        } -Token $customerToken -Label "افزودن آیتم به سفارش"

        Call POST "api/v1/Orders/$orderId/place" $null `
            -Token $customerToken -Label "ثبت نهایی سفارش (Place)"
    }
}

# ═══════════════════════════════════════════════════════════════════════════════
W-Header "Flow 5: Token Management"
# ───────────────────────────────────────────────────────────────────────────────

if ($login?.refreshToken) {
    $refreshed = Call POST "api/v1/Identity/Auth/Refresh" @{
        accessToken  = $customerToken
        refreshToken = $login.refreshToken
    } -Label "Refresh Token"

    if ($refreshed?.accessToken) { W-OK "توکن جدید دریافت شد ✓" }

    Call POST "api/v1/Identity/Auth/Logout" @{
        refreshToken = $refreshed?.refreshToken ?? $login.refreshToken
    } -Token ($refreshed?.accessToken ?? $customerToken) -Label "Logout"
}

# ═══════════════════════════════════════════════════════════════════════════════
Write-Host ""
Write-Host "╔══════════════════════════════════════════╗" -ForegroundColor Green
Write-Host "║         ✅ همه Flow ها تست شدند           ║" -ForegroundColor Green
Write-Host "║  Swagger: $BaseUrl/swagger               ║" -ForegroundColor Green
Write-Host "╚══════════════════════════════════════════╝" -ForegroundColor Green
Write-Host ""

if ($StartServer -and $serverJob) {
    Write-Host "سرور در حال اجراست — Ctrl+C برای خروج" -ForegroundColor Yellow
    Wait-Job $serverJob | Out-Null
}
