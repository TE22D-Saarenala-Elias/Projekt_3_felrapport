# Starta Frontend lokalt (kräver att Backend körs)
Write-Host "Startar Frontend på http://localhost:3000..." -ForegroundColor Green

Set-Location Frontend

Write-Host "Startar Frontend..." -ForegroundColor Green
dotnet run
