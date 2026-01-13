# Ticket Reporting System

En .NET 10 Blazor-baserad webbapplikation för felrapportering med SQL Server-databas, Docker-stöd och rollbaserad åtkomst.

## Funktioner

- **Användarautentisering**: Registrera konto och logga in med JWT-tokens
- **Felrapportering**: Användare kan rapportera fel med titel och beskrivning
- **Status-hantering**: Admin kan uppdatera status på rapporter (Ny, Pågående, Löst)
- **Rapport-borttagning**: Admin kan ta bort felrapporter
- **3NF Databasschema**: Normaliserad databas med Users, TicketStatus och Tickets tabeller

## Projektstruktur

```
├── Backend/                 # REST API (.NET 10)
│   ├── Controllers/        # API-endpoints
│   ├── Models/             # Datamodeller och DTOs
│   ├── Data/               # Entity Framework DbContext
│   ├── Program.cs          # Konfiguration
│   └── Dockerfile          # Container-konfiguration
├── Frontend/               # Blazor Server App (.NET 10)
│   ├── Components/         # Razor-komponenter och sidor
│   ├── Services/           # Tjänster för API-kommunikation
│   ├── Models/             # Frontend modeller
│   ├── Program.cs          # Konfiguration
│   └── Dockerfile          # Container-konfiguration
├── docker-compose.yml      # Flerkontainer-orchestrering
└── global.json             # .NET version-konfiguration
```

## Databas-schema (3NF)

### Users
- **UserID** (Primärnyckel)
- **Username** (Unik)
- **Email** (Unik)
- **PasswordHash**
- **Role** (User/Admin)

### TicketStatus
- **StatusID** (Primärnyckel)
- **StatusName** (Ny, Pågående, Löst)

### Tickets
- **TicketID** (Primärnyckel)
- **Title**
- **Description**
- **DateCreated**
- **CreatorUserID** (Främmande nyckel → Users)
- **StatusID** (Främmande nyckel → TicketStatus)


## Installation och Körning

### Med Docker Compose (Rekommenderat)


1. Starta alla tjänster (SQL Server, Backend, Frontend):
```bash
docker-compose up --build
```

2. Öppna webbläsaren:
   - Frontend: http://localhost:3000
   - Backend API: http://localhost:7000
   - SQL Server: localhost:1433

3. Stoppa tjänsterna:
```bash
docker-compose down
```

### Lokal utveckling utan Docker

#### 1. Starta SQL Server
```bash
docker run -d -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourPassword123!" -p 1433:1433 --name sqlserver mcr.microsoft.com/mssql/server:2022-latest
```

#### 2. Starta Backend
```bash
cd Backend
dotnet restore
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
```

Backend körs nu på:
- HTTP: http://localhost:7000
- HTTPS: https://localhost:7001

#### 3. Starta Frontend (i en ny terminal)
```bash
cd Frontend
dotnet restore
dotnet run
```

Frontend körs nu på:
- HTTP: http://localhost:3000
- HTTPS: https://localhost:3001

## Användning

### Registrering
1. Öppna http://localhost:3000
2. Klicka "Skapa ett konto här"
3. Ange användarnamn, e-postadress och lösenord
4. Klicka "Registrera"

### Inloggning
1. Ange ditt användarnamn och lösenord
2. Klicka "Logga in"

### Skapa Felrapport
1. Efter inloggning, fyll i titel och beskrivning
2. Klicka "Skicka rapport"
3. Rapporten visas i tabellen nedan med status "Ny"

### Admin-funktioner

För att testa admin-funktioner, skapa en admin-användare direkt i databasen eller ändra en befintlig användares roll till "Admin".

**Via termenalen kan du ändra statusen på ett konto till admin genom att skriva det här:**
```sql
docker exec ticketreporting-sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourPassword123!" -C -Q "USE TicketReportingDB; UPDATE Users SET Role = 'Admin' WHERE Username = 'EliasSaarenala'; SELECT UserID, Username, Role FROM Users;"
```

Som Admin kan du:
- Ändra status på rapporter via dropdown-menyn
- Ta bort rapporter med "Ta bort"-knappen

### Tickets
- `GET /api/tickets` - Hämta alla rapporter
- `POST /api/tickets` - Skapa ny rapport (Kräver autentisering)
  ```json
  {
    "title": "string",
    "description": "string"
  }
  ```
- `GET /api/tickets/{id}` - Hämta specifik rapport
- `PUT /api/tickets/{id}` - Uppdatera rapport-status (Admin)
  ```json
  {
    "statusID": 1
  }
  ```
- `DELETE /api/tickets/{id}` - Ta bort rapport (Admin)
- `GET /api/tickets/statuses` - Hämta alla statusar

## Konfiguration

### Backend (appsettings.json)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=TicketReportingDB;User Id=sa;Password=YourPassword123!;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyForJWTTokenGeneration12345",
    "Issuer": "TicketReportingBackend",
    "Audience": "TicketReportingFrontend"
  }
}
```

### Frontend (appsettings.json)
```json
{
  "ApiBaseAddress": "http://localhost:7000"
}
```

## Felsökning

### Docker-problem
```bash
# Visa loggar
docker-compose logs

# Visa loggar för specifik tjänst
docker-compose logs backend
docker-compose logs frontend
docker-compose logs sqlserver

# Starta om en tjänst
docker-compose restart backend
```

### Databas-migrationer
```bash
cd Backend
dotnet ef migrations add MigrationName
dotnet ef database update
```

## Licens

Detta projekt är skapat för utbildningssyfte.
