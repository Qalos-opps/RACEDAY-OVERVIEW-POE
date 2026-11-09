# RaceDay — Complete Part 3 Project

RaceDay is a .NET 8 race-event management solution with an ASP.NET Core Web API and an ASP.NET Core MVC website. The website calls the API for accounts, event data, enrolments, results and image uploads. The API remains the owner of database access, role checks and Azure Blob Storage operations.

> **Assignment note:** The separate Part 3 assignment brief was not present in the uploaded ZIP or the conversation snapshot used for this build. The implementation follows the API endpoint plan and Part 3 notes included in `docs/`.

## Contents

- [Project overview](#project-overview)
- [Features](#features)
- [Technology and architecture](#technology-and-architecture)
- [Requirements](#requirements)
- [Run in Visual Studio 2022](#run-in-visual-studio-2022)
- [Database setup](#database-setup)
- [Storage and image uploads](#storage-and-image-uploads)
- [Run and build from a terminal](#run-and-build-from-a-terminal)
- [Optional Docker setup](#optional-docker-setup)
- [GitHub Actions](#github-actions)
- [API reference](#api-reference)
- [Troubleshooting](#troubleshooting)
- [Known project notes](#known-project-notes)

## Project overview

RaceDay provides a public event catalogue and separate Participant and Organiser experiences. Participants can create accounts, enter events and review their race history. Organisers can create events, manage race categories and participant rosters, upload event banners, and record results.

The solution keeps the supplied Part 2 API and database schema. The Part 3 database update adds event type, event distance, event banner URL and user profile picture URL fields. The MVC project is a separate presentation layer and does not connect directly to SQL Server or Azure Storage.

## Features

### Public pages

- Browse events and filter by event name/description, event type, location and upcoming date.
- View event descriptions, dates, registration deadlines, location, status, banner, categories and available weather information.
- Review category distances, fees, participant capacity and taken places.

### Participant pages

- Register as a Participant and sign in or sign out.
- Update profile name and phone number; upload a profile picture.
- Enrol in an open category with an emergency contact.
- View and cancel eligible entries.
- Review personal results, finish time, position and category finisher count.

### Organiser pages

- View an organiser dashboard with event, category and entry counts.
- Create, edit and delete owned events.
- Upload or replace an event banner.
- Add, edit and delete event categories.
- Review the participant roster and emergency contacts for an owned event.
- Record or update finish times, positions and result statuses.

The API enforces roles and event ownership. Results can be recorded after the event date arrives or after an event is marked Completed. The API also checks result status, positive positions and unique finishing positions within a category.

### Images

- Profile picture uploads accept JPG, PNG or WEBP images up to 2 MB.
- Event banner uploads accept JPG, PNG or WEBP images up to 5 MB.
- The MVC app forwards uploads to the API; the API validates the image bytes and stores the image in Azure Blob Storage or Azurite.

## Technology and architecture

| Area | Implementation |
|---|---|
| Website | ASP.NET Core MVC, .NET 8, Razor views |
| Styling | Bootstrap 5 with a custom black and purple RaceDay theme |
| API | ASP.NET Core Web API, .NET 8 |
| Database | SQL Server with the supplied Part 2 schema and Part 3 update script |
| Image storage | Azure Blob Storage; Azurite is supported for local development |
| Website sign-in | Cookie authentication with server-side session state |
| API sign-in | Existing API cookie authentication |
| CI | GitHub Actions restore and build for both projects |
| Containers | Dockerfiles for API and website; Docker Compose also starts Azurite |

The MVC website establishes its own sign-in cookie for browser navigation and stores the API authentication cookie in server-side session state. It forwards the API cookie on authenticated API calls. This keeps the supplied API authentication model intact while allowing role-based website navigation.

## Project structure

```text
RaceDay.sln
├── RaceDay.API/                 Web API, SQL Server access, auth and Blob Storage
│   ├── Controllers/             API endpoints
│   ├── Data/                    Entity Framework database context
│   ├── DTOs/                    API request and response contracts
│   ├── Models/                  Database entities
│   └── Services/                Image validation and Blob Storage service
├── RaceDay.Web/                 ASP.NET Core MVC website
│   ├── Controllers/             Public, account, participant and organiser flows
│   ├── Models/                  Web forms, view models and API contracts
│   ├── Services/                API client and API-cookie forwarding
│   ├── Views/                   Razor pages and shared layout
│   └── wwwroot/                 CSS and static assets
├── docs/                        Database scripts and API/Part 3 notes
├── .github/workflows/           GitHub Actions build workflow
├── docker-compose.yml           Optional API, web and Azurite containers
└── README.md                    Setup, usage and troubleshooting guide
```

## Requirements

- Windows with Visual Studio 2022 and the **ASP.NET and web development** workload.
- .NET 8 SDK.
- SQL Server and SQL Server Management Studio (SSMS), or an existing Part 2 `RaceDay` database.
- Docker Desktop for Azurite or the optional Docker Compose setup.
- Internet access in the browser for the Bootstrap and client-side validation CDN assets. Server-side validation remains enabled if those CDN assets are unavailable.

## Run in Visual Studio 2022

### 1. Open the solution

Extract the project ZIP and open `RaceDay.sln` in Visual Studio. Allow Visual Studio to restore NuGet packages.

### 2. Prepare the database

If you already have the Part 2 `RaceDay` database, keep it and run `docs/RACEDAY_Part3_Update.sql` once in SSMS. The update script is safe to rerun.

For a new database, run `docs/RACEDAY_CLEAN_INSTALL.sql` in SSMS, followed by `docs/RACEDAY_Part3_Update.sql`. The clean-install script is based on the supplied original database script and adds a `GO` batch separator after `CREATE DATABASE`. The original `docs/RACEDAY_DATABASE.sql` is preserved unchanged.

### 3. Configure the API's SQL Server connection

In Visual Studio, right-click **RaceDay.API** and select **Manage User Secrets**. Add your own SQL Server connection string:

```json
{
  "ConnectionStrings": {
    "RaceDayConnection": "Server=YOUR_SQL_SERVER_INSTANCE;Database=RaceDay;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

Replace `YOUR_SQL_SERVER_INSTANCE` with the server/instance name shown in SSMS. Common examples are `localhost`, `localhost\\SQLEXPRESS` and `.`. For SQL authentication, use a SQL Server login and password in the connection string, and keep it in User Secrets rather than committing it.

The API's `appsettings.json` contains the `RaceDayConnection` key with a placeholder value. User Secrets override that value locally.

### 4. Start local image storage

For local development, start Docker Desktop and run this in PowerShell:

```powershell
docker run -d --name azurite -p 10000:10000 mcr.microsoft.com/azure-storage/azurite azurite-blob --blobHost 0.0.0.0 --skipApiVersionCheck
```

The API's default Azure Blob setting is `UseDevelopmentStorage=true`, which connects to the local Azurite endpoint. If the `azurite` container already exists, start it with `docker start azurite`.

For a real Azure Storage account, follow `docs/CHANGES_chunk2_azure.md`. Store the connection string in **RaceDay.API > Manage User Secrets** under `AzureBlob:ConnectionString`. The account must allow blob access for the public image URLs returned by the API to display in a browser. Never commit a real Azure connection string.

### 5. Configure both startup projects

In Visual Studio:

1. Right-click the solution and select **Configure Startup Projects…**.
2. Select **Multiple startup projects**.
3. Set `RaceDay.API` and `RaceDay.Web` to **Start**.
4. Apply the settings, then press **F5**.

The default addresses are:

- Website: `https://localhost:7143`
- API Swagger: `https://localhost:61424/swagger`

If the API HTTPS port changes in `RaceDay.API/Properties/launchSettings.json`, update `RaceDayApi:BaseUrl` in `RaceDay.Web/appsettings.json` to match. Trust the ASP.NET Core HTTPS development certificate if Visual Studio prompts you.

### 6. Create accounts and explore the roles

Use **Join RaceDay** to create a Participant account. Registration intentionally creates Participant accounts only; there is no public Organiser registration flow.

For local development, you can promote a registered account to Organiser in SSMS:

```sql
USE RaceDay;
UPDATE Users
SET Role = 'Organiser'
WHERE Email = 'your.registered@email.example';
```

Change the email to the account you created, sign out, and sign in again so the website loads the updated role. This is a development setup step; the API continues to enforce role and ownership checks.

The sample users in the original SQL file have literal placeholder password hashes and cannot sign in. Register a new account to create a usable password hash.

## Database setup

Database files are in `docs/`:

- `RACEDAY_DATABASE.sql` — original Part 2 database creation and sample data script.
- `RACEDAY_CLEAN_INSTALL.sql` — clean-install copy with the SQL Server batch separator needed after `CREATE DATABASE`.
- `RACEDAY_Part3_Update.sql` — adds the Part 3 fields to an existing database and updates existing event distances.
- `RaceDay_API_Endpoint_Plan.md` — planned API routes, roles and Part 3 changes.
- `CHANGES_chunk1_api.md` and `CHANGES_chunk2_azure.md` — API and Azure implementation notes.

Do not run the clean-install script against a database you want to preserve: it creates the `RaceDay` database and inserts sample records. For an existing database, use only the Part 3 update script.

## Storage and image uploads

The API owns all Blob Storage interaction. The website sends a multipart upload to the API; it does not receive storage credentials. The API returns a public URL, saves that URL to the existing RaceDay database, and removes the previous image after a successful replacement.

The default local settings are:

```json
{
  "AzureBlob": {
    "ConnectionString": "UseDevelopmentStorage=true",
    "ContainerName": "raceday-images"
  }
}
```

When storage is unavailable, the API reports an upload error while the rest of the event website remains available. The API writes the storage failure details to its logs.

## Run and build from a terminal

Open PowerShell in the folder containing `RaceDay.sln`:

```powershell
dotnet restore RaceDay.sln --disable-parallel --maxcpucount:1
dotnet build RaceDay.sln --configuration Release --no-restore --maxcpucount:1
```

The build compiles both `RaceDay.API` and `RaceDay.Web`. It does not connect to SQL Server or Azure Storage. Run the projects from Visual Studio or use the relevant `dotnet run --project ...` command after configuring the API connection string and starting storage.

No separate automated test project is included. A full user-flow check requires a configured SQL Server database and running API; image upload checks also require Azurite or Azure Blob Storage.

## Optional Docker setup

Docker Compose starts the API, MVC website and Azurite. It reuses an existing RaceDay SQL Server database; it does not create or replace the database. The SQL Server must be reachable from the containers and accept SQL authentication.

1. Copy `.env.example` to `.env`.
2. Set `RACEDAY_SQL_CONNECTION` to a SQL-authenticated connection string for the existing `RaceDay` database. For SQL Server on the same computer, use `host.docker.internal` and ensure TCP/IP is enabled.
3. The example includes Azurite's public emulator-only development account string with the Compose service endpoint. Replace it with an Azure connection string only when using real Azure Storage.
4. Run:

   ```powershell
   docker compose up --build
   ```

5. Open the website at `http://localhost:8081`. The API is available at `http://localhost:8080`.

The `.env` file can contain credentials and is excluded from the project ZIP and source control. The example Azurite account is for emulator development only; never use it for production data. Docker Compose uses named volumes for Azurite data and the website cookie-encryption key ring.

## GitHub Actions

`.github/workflows/build.yml` restores and builds both projects on pushes and pull requests to `main` or `master`. It uses the .NET 8 SDK and does not need SQL Server or Azure credentials to compile the solution.

## API reference

The API routes are documented in Swagger while the API runs in Development at `https://localhost:61424/swagger`.

| Area | Routes | Access |
|---|---|---|
| Authentication | `POST /api/auth/register`, `POST /api/auth/login`, `POST /api/auth/logout` | Register/login public; logout signed-in |
| User profile | `GET /api/users/me`, `PUT /api/users/me`, `POST /api/users/me/picture` | Signed-in |
| Events | `GET /api/events`, `GET /api/events/{id}`, `POST /api/events`, `GET /api/events/mine`, `PUT /api/events/{id}`, `DELETE /api/events/{id}`, `POST /api/events/{id}/banner` | Public read; organiser write/own events |
| Categories | `GET /api/events/{eventId}/categories`, `POST /api/events/{eventId}/categories`, `PUT /api/categories/{id}`, `DELETE /api/categories/{id}` | Public read; organiser write/own event |
| Enrolments | `POST /api/enrolments`, `GET /api/enrolments/me`, `DELETE /api/enrolments/{id}`, `GET /api/events/{eventId}/enrolments` | Participant for personal entries; organiser for own event roster |
| Results | `POST /api/results`, `PUT /api/results/{id}`, `GET /api/results/me`, `GET /api/events/{eventId}/results` | Organiser records/views event results; participant views personal results |
| Weather | `GET /api/events/{eventId}/weather` | Public |

The event list supports `search`, `eventType`, `location` and `upcomingOnly` query filters. Event types are Run, Walk and Cycle. Result statuses are Finished, DNF, DNS and Disqualified.

## Troubleshooting

### The website says it cannot reach the API

- Start `RaceDay.API` and `RaceDay.Web` together in Visual Studio.
- Check that the API's HTTPS port matches `RaceDayApi:BaseUrl` in the web app settings.
- Open the API Swagger URL to confirm the API is running.
- If the API's HTTPS certificate is not trusted, trust the ASP.NET Core development certificate.

### The API cannot connect to SQL Server

- Check the server/instance name in User Secrets.
- Confirm SQL Server is running and the `RaceDay` database exists.
- Confirm the Part 3 update script has been applied.
- For Docker, use a SQL-authenticated connection string and `host.docker.internal`; Windows integrated authentication from a Linux container is not configured by this Compose setup.

### Sign in fails for the sample users

The original database script uses placeholder hashes for sample accounts. Create an account through the website, then use the SQL role update above if you need to test Organiser features.

### Image upload fails

- Make sure Azurite or Azure Storage is running and the API's `AzureBlob` connection string points to it.
- Check the API console logs for the storage error.
- For Azure Storage, verify the account's blob access settings and that the stored image URL can be opened by the browser.
- Use JPG, PNG or WEBP and stay within the profile (2 MB) or banner (5 MB) limit.

### Organiser pages are not shown

Sign in with an account whose database `Role` is `Organiser`. If you changed the role directly in SQL, sign out and back in to refresh the website's role claims.

## Known project notes

- The supplied Part 2 API and original database SQL script are retained unchanged.
- The Part 3 database update is separate and safe to rerun.
- New public registrations are Participants; Organiser accounts must already exist or be assigned by an administrator/development setup.
- The website uses an in-memory session store. Signing out clears both the website login and its stored API cookie. A multi-instance deployment should use a shared distributed session store and shared Data Protection key storage.
- For production deployment, configure HTTPS, durable shared session/key storage, secure secret management, and an Azure Storage configuration appropriate for the production account.
