param(
    [string]$ProjectName = (Read-Host "Enter project name (e.g. ahmad.userMangemnt)")
)

if (-not $ProjectName) { Write-Host "Error: Name required." -ForegroundColor Red; exit }

$root = Join-Path (Get-Location) $ProjectName
$sln = Join-Path $root "$ProjectName.sln"
$nexus = "http://nexus-ahmad-dev.ir/repository/nuget-hosted/"

function New-Project($path, $ahmad.onlineshop) {
    $full = Join-Path $root $path
    $name = Split-Path $full -Leaf
    dotnet new $ahmad.onlineshop -n $name -o $full --framework net8.0 | Out-Null
    dotnet sln $sln add (Join-Path $full "$name.csproj") | Out-Null
}

# 1. Clean & Create Solution
if(Test-Path $root){ Remove-Item $root -Recurse -Force }
New-Item -ItemType Directory -Path $root | Out-Null
dotnet new sln -n $ProjectName -o $root | Out-Null

Write-Host "Scaffolding folders like the image..." -ForegroundColor Cyan

# 2. Application Layer
New-Project "Src\Application\$ProjectName.Application" classlib
New-Project "Src\Application\$ProjectName.Application.Contract" classlib
New-Project "Src\Application\$ProjectName.Application.Query" classlib
New-Project "Src\Application\$ProjectName.Application.Query.Contract" classlib
New-Project "Src\Application\$ProjectName.Event.Handler" classlib

# 3. Domain Layer
New-Project "Src\Domain\$ProjectName.Domain" classlib
New-Project "Src\Domain\$ProjectName.Event" classlib

# 4. Infrastructure Layer
New-Project "Src\Infrastructure\$ProjectName.API.Config" classlib
New-Project "Src\Infrastructure\$ProjectName.Persistence.EF" classlib
New-Project "Src\Infrastructure\$ProjectName.Read.Dapper" classlib

# 5. Rest Layer
New-Project "Src\Rest\$ProjectName.API.Rest" classlib

# 6. Test Layer
New-Project "Test\$ProjectName.Application.Tests" xunit
New-Project "Test\$ProjectName.Domain.Tests" xunit
New-Project "Test\$ProjectName.Integration.Tests" xunit

# 7. Service Host (The Web API)
New-Project "$ProjectName.ServiceHost" webapi

Write-Host "Setting up internal references..." -ForegroundColor Yellow

# --- References Mapping ---
$app = "Src\Application\$ProjectName.Application\$ProjectName.Application.csproj"
$appContract = "Src\Application\$ProjectName.Application.Contract\$ProjectName.Application.Contract.csproj"
$domain = "Src\Domain\$ProjectName.Domain\$ProjectName.Domain.csproj"
$infraEf = "Src\Infrastructure\$ProjectName.Persistence.EF\$ProjectName.Persistence.EF.csproj"
$host = "$ProjectName.ServiceHost\$ProjectName.ServiceHost.csproj"

# Example of essential references (Add more as needed)
dotnet add (Join-Path $root $app) reference (Join-Path $root $domain)
dotnet add (Join-Path $root $app) reference (Join-Path $root $appContract)
dotnet add (Join-Path $root $host) reference (Join-Path $root $app)
dotnet add (Join-Path $root $host) reference (Join-Path $root $infraEf)

# 8. Injecting your private AhmadBase NuGets
Write-Host "Injecting AhmadBase packages from Nexus..." -ForegroundColor Green

# You can add/remove packages based on which project needs what:
dotnet add (Join-Path $root $domain) package AhmadBase.Domain --source $nexus
dotnet add (Join-Path $root $app) package AhmadBase.Application --source $nexus
dotnet add (Join-Path $root $infraEf) package AhmadBase.Persistence --source $nexus
dotnet add (Join-Path $root $host) package AhmadBase.IOC --source $nexus

Write-Host "Structure Ready! Check the $ProjectName folder." -ForegroundColor Cyan
