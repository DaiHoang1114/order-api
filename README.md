# OrderPOC

OrderPOC is a Proof of Concept (POC) API for managing orders, built with .NET 8 and Clean Architecture principles. It implements CQRS using MediatR and persists data in a PostgreSQL database.

## Architecture

The project follows the Clean Architecture pattern with the following layers:

*   **OrderPOC.Domain**: Core domain entities and business logic (e.g., `Order`, `Customer`).
*   **OrderPOC.Application**: Application use cases (CQRS Commands and Queries), interfaces, and DTOs.
*   **OrderPOC.Infrastructure**: Implementation of interfaces (e.g., Repositories, Database Context) and database migrations.
*   **OrderPOC.API**: The entry point of the application (Controllers, Program.cs, Configuration).
*   **OrderPOC.API.Infrastructure**: Specific infrastructure components for the API host (e.g., Global Exception Handling).

## Tech Stack

*   **Framework**: .NET 8
*   **Database**: PostgreSQL
*   **ORM**: Entity Framework Core
*   **Patterns**: CQRS (MediatR), Repository Pattern
*   **Containerization**: Docker Compose

## Prerequisites

*   [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
*   [Docker Desktop](https://www.docker.com/products/docker-desktop/) (or Docker Engine + Compose)
*   EF Core Global Tool (`dotnet tool install --global dotnet-ef`)

## Getting Started

### 1. Start Infrastructure

Use Docker Compose to start the PostgreSQL database.

```bash
docker-compose -f Infrastructure/docker-compose.yml up -d
```

### 2. Setup Database

Navigate to the project root directory and update the database structure.

> **Note**: Ensure `dotnet-ef` is in your PATH. If the command fails, you might need to run `export PATH="$PATH:$HOME/.dotnet/tools"` or add it to your shell profile.

```bash
# Create migration (if needed)
dotnet ef migrations add InitialCreate --project src/OrderPOC.Infrastructure --startup-project src/OrderPOC.API

# Update database
dotnet ef database update --project src/OrderPOC.Infrastructure --startup-project src/OrderPOC.API
```

### 3. Run the API

You can run the API directly using the `dotnet` CLI.

```bash
dotnet run
```

The API will be available at `http://localhost:5186` (or the port configured in `launchSettings.json`).

## API Endpoints

Once the application is running, you can access the Swagger UI documentation at:

```
http://localhost:5186/swagger
```

### Key Endpoints

*   `POST /api/v1/orders`: Create a new order.
*   `GET /api/v1/orders/{orderId}`: Retrieve an order by its ID.

## Project Structure

```
OrderPOC/
├── Infrastructure/       # Docker configuration
├── src/
│   ├── OrderPOC.API/               # Web API Layer
│   ├── OrderPOC.API.Infrastructure/ # API-specific Infra (Error Handling)
│   ├── OrderPOC.Application/       # CQRS, Interfaces
│   ├── OrderPOC.Domain/            # Entities
│   └── OrderPOC.Infrastructure/    # EF Core, Repositories
└── OrderPOC.sln
```
