param(
    [Parameter(Mandatory = $true)]
    [string]$MigrationName
)

$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$repoRoot = Resolve-Path (Join-Path $scriptDir "..\..")

$efProjectDir  = Join-Path $repoRoot "src\Infrastructure\Ahmad.OnlineShop.Persistence.EF"
$efProjectFile = Join-Path $efProjectDir "Ahmad.OnlineShop.Persistence.EF.csproj"

$baseScriptFolder = Join-Path $repoRoot "Docs\DataBaseScript"

$timestamp = Get-Date -Format "yyyyMMddHHmmss"
$fullName  = "${timestamp}_${MigrationName}"

if (!(Test-Path $baseScriptFolder)) {
    New-Item -ItemType Directory -Path $baseScriptFolder -Force | Out-Null
}

$existingFolders = Get-ChildItem $baseScriptFolder -Directory -ErrorAction SilentlyContinue |
    Where-Object { $_.Name -match '^\d+$' } |
    ForEach-Object { [int]$_.Name }

$nextNumber = if ($existingFolders.Count -gt 0) {
    ($existingFolders | Measure-Object -Maximum).Maximum + 1
} else {
    1
}

$outputFolder = Join-Path $baseScriptFolder $nextNumber
if (!(Test-Path $outputFolder)) {
    New-Item -ItemType Directory -Path $outputFolder -Force | Out-Null
}

$sqlFileName = Join-Path $outputFolder "${fullName}.sql"

Write-Host "Starting Migration Process: $fullName" -ForegroundColor Cyan
Write-Host "EF Project File: $efProjectFile" -ForegroundColor DarkGray
Write-Host "SQL Output Folder: $outputFolder" -ForegroundColor DarkGray

if (!(Test-Path $efProjectFile)) {
    Write-Host "EF project file not found: $efProjectFile" -ForegroundColor Red
    exit 1
}

Push-Location $efProjectDir

try {
    dotnet ef migrations add $fullName `
        --project $efProjectFile `
        --output-dir Migrations

    if ($LASTEXITCODE -ne 0) {
        Write-Host "Failed to create migration." -ForegroundColor Red
        exit 1
    }

    dotnet ef migrations script --idempotent `
        --project $efProjectFile `
        --output $sqlFileName

    if ($LASTEXITCODE -eq 0) {
        Write-Host "Migration and SQL Script created successfully." -ForegroundColor Green
        Write-Host "Migration Name: $fullName" -ForegroundColor Gray
        Write-Host "SQL Path: $sqlFileName" -ForegroundColor Gray
    }
    else {
        Write-Host "Failed to generate SQL script." -ForegroundColor Red
        exit 1
    }
}
finally {
    Pop-Location
}
