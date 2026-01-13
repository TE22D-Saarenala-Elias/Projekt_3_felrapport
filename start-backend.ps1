# Starta Backend lokalt (kräver att SQL Server körs)
Write-Host "Startar Backend på http://localhost:7000..." -ForegroundColor Green

Set-Location Backend

# Kontrollera om migrationer behövs
$migrations = dotnet ef migrations list 2>$null
if ($LASTEXITCODE -ne 0 -or -not $migrations) {
    Write-Host "Skapar databas-migrationer..." -ForegroundColor Yellow
    dotnet ef migrations add InitialCreate
}

Write-Host "Uppdaterar databas..." -ForegroundColor Yellow
dotnet ef database update

Write-Host "Startar Backend..." -ForegroundColor Green
dotnet run
