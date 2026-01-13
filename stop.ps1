# Stoppa och ta bort alla containers
Write-Host "Stoppar alla tjänster..." -ForegroundColor Yellow
docker-compose down

Write-Host "Tjänster stoppade!" -ForegroundColor Green
