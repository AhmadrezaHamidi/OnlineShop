#Requires -RunAsAdministrator

Write-Host "Repairing SQL Server LocalDB ..." -ForegroundColor Cyan

$candidates = @(
    "C:\Program Files\Microsoft SQL Server\150\LocalDB\Binn\",
    "C:\Program Files\Microsoft SQL Server\160\LocalDB\Binn\",
    "${env:ProgramFiles(x86)}\Microsoft SQL Server\150\LocalDB\Binn\"
)

$dataDir = $candidates | Where-Object { Test-Path "$_\sqlservr.exe" } | Select-Object -First 1

if (-not $dataDir) {
    Write-Host "LocalDB install not found. Install SQL Server Express LocalDB:" -ForegroundColor Red
    Write-Host "   https://aka.ms/localdbinstaller" -ForegroundColor Yellow
    exit 1
}

Write-Host "Found install path: $dataDir" -ForegroundColor Green

$regPaths = @(
    "HKLM:\SOFTWARE\Microsoft\Microsoft SQL Server Local DB\Installed Versions\15.0",
    "HKLM:\SOFTWARE\WOW6432Node\Microsoft\Microsoft SQL Server Local DB\Installed Versions\15.0"
)

foreach ($p in $regPaths) {
    if (Test-Path $p) {
        Set-ItemProperty -Path $p -Name "DataDirectory" -Value $dataDir -Force
        Write-Host "Set DataDirectory on $p" -ForegroundColor Green
    }
}

Write-Host "Cleaning old user instance ..." -ForegroundColor Cyan
sqllocaldb stop MSSQLLocalDB -i 2>$null | Out-Null
sqllocaldb delete MSSQLLocalDB 2>$null | Out-Null

$userDataFolder = "$env:LOCALAPPDATA\Microsoft\Microsoft SQL Server Local DB\Instances\MSSQLLocalDB"
if (Test-Path $userDataFolder) {
    Remove-Item -Path $userDataFolder -Recurse -Force -ErrorAction SilentlyContinue
}

$userRegPath = "HKCU:\SOFTWARE\Microsoft\Microsoft SQL Server Local DB\Instances\MSSQLLocalDB"
if (Test-Path $userRegPath) {
    Remove-Item -Path $userRegPath -Recurse -Force -ErrorAction SilentlyContinue
}

Write-Host "Creating new instance ..." -ForegroundColor Cyan
sqllocaldb create MSSQLLocalDB
Start-Sleep 2
sqllocaldb start MSSQLLocalDB
Start-Sleep 2

$info = sqllocaldb info MSSQLLocalDB
Write-Host ""
Write-Host $info

if ($info -match "State:\s*Running") {
    Write-Host ""
    Write-Host "SUCCESS - LocalDB is running!" -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host "STILL FAILING - check Windows Event Viewer:" -ForegroundColor Red
    Write-Host "   Get-WinEvent -LogName Application -MaxEvents 10" -ForegroundColor Yellow
}
