# PostoCerto - Documento de Arquitetura e Roadmap Completo

## 📋 Índice
1. [Visão Geral](#visão-geral)
2. [Stack Tecnológica (Versões LTS)](#stack-tecnológica)
3. [Arquitetura de Microserviços](#arquitetura)
4. [Estrutura de Pastas](#estrutura-de-pastas)
5. [Domain-Driven Design (DDD)](#domain-driven-design)
6. [Test-Driven Development (TDD)](#test-driven-development)
7. [Roadmap Completo](#roadmap-completo)

---

## Visão Geral

**PostoCerto** é uma plataforma de comparação de preços de combustíveis que consome dados da ANP (Agência Nacional do Petróleo). O sistema utiliza arquitetura de microserviços com Clean Architecture, DDD e TDD.

### Objetivos de Aprendizado
- Microserviços .NET com Clean Architecture
- Domain-Driven Design aplicado
- Test-Driven Development
- gRPC para comunicação interna
- REST API para clientes externos
- React (Web) e Flutter (Mobile)
- Docker e Docker Compose
- PostgreSQL e MongoDB
- Integração com IA (Anthropic Claude)

---

## Stack Tecnológica

### Backend (.NET)

| Tecnologia | Versão LTS | Justificativa |
|------------|------------|---------------|
| .NET SDK | 8.0.404 | LTS até nov/2026 |
| C# | 12.0 | Incluído no .NET 8 |
| ASP.NET Core | 8.0.11 | Runtime LTS |

### Pacotes NuGet (Versão Unificada: 8.0.11)

```xml
<!-- Todos os serviços usam as mesmas versões -->
<PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="8.0.11" />
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.11" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.11" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.11" />
<PackageReference Include="Grpc.AspNetCore" Version="2.66.0" />
<PackageReference Include="Grpc.Tools" Version="2.66.0" />
<PackageReference Include="FluentValidation" Version="11.10.0" />
<PackageReference Include="MediatR" Version="12.4.1" />
<PackageReference Include="Serilog.AspNetCore" Version="8.0.3" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.9.0" />

<!-- Testes -->
<PackageReference Include="xunit" Version="2.9.2" />
<PackageReference Include="xunit.runner.visualstudio" Version="2.8.2" />
<PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.11.1" />
<PackageReference Include="FluentAssertions" Version="6.12.2" />
<PackageReference Include="Moq" Version="4.20.72" />
<PackageReference Include="Testcontainers" Version="3.10.0" />
<PackageReference Include="Testcontainers.PostgreSql" Version="3.10.0" />
<PackageReference Include="Testcontainers.MongoDb" Version="3.10.0" />

<!-- MongoDB -->
<PackageReference Include="MongoDB.Driver" Version="2.29.0" />

<!-- Worker Service -->
<PackageReference Include="Quartz" Version="3.13.1" />
<PackageReference Include="Quartz.Extensions.Hosting" Version="3.13.1" />
<PackageReference Include="Polly" Version="8.4.2" />

<!-- IA -->
<PackageReference Include="Anthropic.SDK" Version="0.3.1" />
```

### Frontend

| Tecnologia | Versão | Justificativa |
|------------|--------|---------------|
| Node.js | 20.18.1 LTS | LTS até abr/2026 |
| React | 18.3.1 | Estável |
| TypeScript | 5.6.3 | Compatível React 18 |
| Vite | 5.4.11 | Build tool moderno |
| React Router | 6.28.0 | Routing |
| TanStack Query | 5.62.7 | Data fetching |
| Axios | 1.7.9 | HTTP client |
| Leaflet | 1.9.4 | Mapas |

### Mobile

| Tecnologia | Versão | Justificativa |
|------------|--------|---------------|
| Flutter | 3.27.1 | Stable channel |
| Dart | 3.6.0 | Incluído no Flutter |
| dio | 5.7.0 | HTTP client |
| flutter_bloc | 8.1.6 | State management |
| get_it | 8.0.2 | DI |
| freezed | 2.5.7 | Immutability |
| google_maps_flutter | 2.9.0 | Mapas |

### Infraestrutura

| Tecnologia | Versão | Justificativa |
|------------|--------|---------------|
| PostgreSQL | 16.6 | LTS até nov/2028 |
| MongoDB | 7.0.15 | LTS até nov/2026 |
| Docker | 27.4.0 | Stable |
| Docker Compose | 2.31.0 | Compatível |

---

## Arquitetura

### Diagrama de Comunicação

```
┌─────────────────────────────────────────────────────────────┐
│                         CLIENTES                             │
├─────────────────────────────────────────────────────────────┤
│  React Web (Port: 3000)  │  Flutter Mobile                  │
└──────────────┬───────────┴──────────────┬───────────────────┘
               │ HTTPS/JSON               │ HTTPS/JSON
               └──────────────┬───────────┘
                              │
               ┌──────────────▼──────────────┐
               │       API GATEWAY            │
               │  REST Port: 8080             │
               │  gRPC Port: 8081             │
               └──────────┬───────────────────┘
                          │ gRPC (HTTP/2)
         ┌────────────────┼────────────────┬──────────────┐
         │                │                │              │
    ┌────▼────┐     ┌────▼────┐     ┌────▼────┐   ┌────▼─────┐
    │Identity │     │Stations │     │ Prices  │   │Recommend.│
    │Service  │     │Service  │     │Service  │   │Service   │
    │gRPC:7001│     │gRPC:7002│     │gRPC:7003│   │gRPC:7004 │
    └────┬────┘     └────┬────┘     └────┬────┘   └────┬─────┘
         │               │               │              │
         │               │               │              │
    ┌────▼────┐     ┌───▼─────┐    ┌───▼──────┐  ┌───▼──────┐
    │Postgres │     │Postgres │    │ MongoDB  │  │ MongoDB  │
    │identity │     │stations │    │  prices  │  │recommend.│
    │Port:5432│     │Port:5432│    │Port:27017│  │Port:27017│
    └─────────┘     └─────────┘    └──────────┘  └──────────┘

    ┌─────────────────────────────────────────────────────────┐
    │              INGESTION WORKER                            │
    │  (Quartz Scheduled Jobs)                                 │
    └────┬────────────────────────────────────┬───────────────┘
         │ gRPC                               │ gRPC
         ▼                                    ▼
    Stations Service                    Prices Service
         ▲                                    ▲
         │ HTTP                               │ HTTP
         └────────────────┬───────────────────┘
                          │
                    ┌─────▼─────┐
                    │  ANP API  │
                    │dados.gov.br│
                    └───────────┘

    ┌─────────────────────────────────────────┐
    │    Recommendations Service              │
    └────┬────────────────────────────────────┘
         │ HTTPS
         ▼
    ┌────────────────┐
    │Anthropic Claude│
    │     API        │
    └────────────────┘
```

### Princípios Arquiteturais

#### 1. Clean Architecture (por serviço)
```
Domain (Entidades, Value Objects, Invariantes)
   ↑
Application (Commands, Queries, Handlers, Interfaces)
   ↑
Infrastructure (EF Core, Repositories, APIs externas)
   ↑
Api (Controllers REST, Services gRPC)
```

#### 2. Domain-Driven Design
- **Bounded Contexts:** Identity, Stations, Prices, Recommendations
- **Aggregates:** User, Station, PriceSurvey, UserPreference
- **Value Objects:** Email, Cnpj, Coordinates, Price
- **Domain Events:** UserRegistered, StationCreated, PriceUpdated

#### 3. SOLID Principles
- **S**ingle Responsibility: Um handler por use case
- **O**pen/Closed: Strategy pattern para tipos de combustível
- **L**iskov Substitution: Interfaces de repositório
- **I**nterface Segregation: IUserReader vs IUserWriter
- **D**ependency Inversion: DI nativo do .NET

#### 4. Comunicação
- **Externo → Gateway:** REST/HTTPS (JSON)
- **Gateway → Services:** gRPC (HTTP/2)
- **Services → Databases:** Cada serviço próprio DB

---

## Estrutura de Pastas

```text
PostoCerto/
├── .github/
│   └── workflows/
│       ├── backend-ci.yml
│       ├── web-ci.yml
│       └── mobile-ci.yml
│
├── docs/
│   ├── architecture.md              # Este arquivo
│   ├── adr/                         # Architecture Decision Records
│   └── api/
│       └── openapi.yaml
│
├── libs/
│   ├── building-blocks/
│   │   └── src/
│   │       ├── BuildingBlocks.Domain/
│   │       │   ├── Entity.cs
│   │       │   ├── ValueObject.cs
│   │       │   ├── Result.cs
│   │       │   └── DomainEvent.cs
│   │       └── BuildingBlocks.Application/
│   │           ├── ICommand.cs
│   │           ├── IQuery.cs
│   │           └── IUnitOfWork.cs
│   │
│   └── contracts/
│       ├── grpc/
│       │   ├── identity.proto
│       │   ├── stations.proto
│       │   ├── prices.proto
│       │   └── recommendations.proto
│       └── rest/
│           └── openapi.yaml
│
├── services/
│   ├── api-gateway/
│   │   ├── ApiGateway.sln
│   │   ├── src/
│   │   │   └── ApiGateway.Api/
│   │   │       ├── Endpoints/
│   │   │       │   ├── AuthEndpoints.cs
│   │   │       │   ├── StationsEndpoints.cs
│   │   │       │   ├── PricesEndpoints.cs
│   │   │       │   └── RecommendationsEndpoints.cs
│   │   │       ├── GrpcClients/
│   │   │       │   ├── IdentityGrpcClient.cs
│   │   │       │   ├── StationsGrpcClient.cs
│   │   │       │   ├── PricesGrpcClient.cs
│   │   │       │   └── RecommendationsGrpcClient.cs
│   │   │       ├── Middleware/
│   │   │       │   ├── AuthenticationMiddleware.cs
│   │   │       │   └── ExceptionHandlingMiddleware.cs
│   │   │       ├── Program.cs
│   │   │       ├── appsettings.json
│   │   │       └── Dockerfile
│   │   └── tests/
│   │       └── ApiGateway.Tests/
│   │
│   ├── identity-service/
│   │   ├── IdentityService.sln
│   │   ├── src/
│   │   │   ├── Identity.Domain/
│   │   │   │   ├── Entities/
│   │   │   │   │   ├── User.cs
│   │   │   │   │   └── Role.cs
│   │   │   │   ├── ValueObjects/
│   │   │   │   │   ├── Email.cs
│   │   │   │   │   ├── Password.cs
│   │   │   │   │   └── RefreshToken.cs
│   │   │   │   ├── Events/
│   │   │   │   │   └── UserRegisteredEvent.cs
│   │   │   │   └── Exceptions/
│   │   │   │       └── InvalidEmailException.cs
│   │   │   │
│   │   │   ├── Identity.Application/
│   │   │   │   ├── Common/
│   │   │   │   │   ├── Interfaces/
│   │   │   │   │   │   ├── IUserRepository.cs
│   │   │   │   │   │   ├── IPasswordHasher.cs
│   │   │   │   │   │   └── ITokenService.cs
│   │   │   │   │   └── Behaviors/
│   │   │   │   │       ├── ValidationBehavior.cs
│   │   │   │   │       └── LoggingBehavior.cs
│   │   │   │   ├── Features/
│   │   │   │   │   ├── Register/
│   │   │   │   │   │   ├── RegisterUserCommand.cs
│   │   │   │   │   │   ├── RegisterUserCommandHandler.cs
│   │   │   │   │   │   └── RegisterUserCommandValidator.cs
│   │   │   │   │   ├── Login/
│   │   │   │   │   │   ├── LoginCommand.cs
│   │   │   │   │   │   └── LoginCommandHandler.cs
│   │   │   │   │   └── GetUser/
│   │   │   │   │       ├── GetUserQuery.cs
│   │   │   │   │       └── GetUserQueryHandler.cs
│   │   │   │   └── DependencyInjection.cs
│   │   │   │
│   │   │   ├── Identity.Infrastructure/
│   │   │   │   ├── Persistence/
│   │   │   │   │   ├── IdentityDbContext.cs
│   │   │   │   │   ├── Configurations/
│   │   │   │   │   │   ├── UserConfiguration.cs
│   │   │   │   │   │   └── RoleConfiguration.cs
│   │   │   │   │   ├── Repositories/
│   │   │   │   │   │   └── UserRepository.cs
│   │   │   │   │   └── Migrations/
│   │   │   │   ├── Security/
│   │   │   │   │   ├── PasswordHasher.cs
│   │   │   │   │   └── TokenService.cs
│   │   │   │   └── DependencyInjection.cs
│   │   │   │
│   │   │   └── Identity.Api/
│   │   │       ├── Grpc/
│   │   │       │   └── Services/
│   │   │       │       └── IdentityGrpcService.cs
│   │   │       ├── Rest/
│   │   │       │   └── Controllers/
│   │   │       │       └── DebugController.cs
│   │   │       ├── Program.cs
│   │   │       ├── appsettings.json
│   │   │       └── Dockerfile
│   │   │
│   │   └── tests/
│   │       ├── Identity.Domain.Tests/
│   │       ├── Identity.Application.Tests/
│   │       ├── Identity.Infrastructure.Tests/
│   │       └── Identity.Api.Tests/
│   │
│   ├── stations-service/
│   │   ├── StationsService.sln
│   │   ├── src/
│   │   │   ├── Stations.Domain/
│   │   │   │   ├── Entities/
│   │   │   │   │   └── Station.cs
│   │   │   │   ├── ValueObjects/
│   │   │   │   │   ├── Cnpj.cs
│   │   │   │   │   ├── Coordinates.cs
│   │   │   │   │   └── Address.cs
│   │   │   │   └── Services/
│   │   │   │       └── GeoSearchService.cs
│   │   │   │
│   │   │   ├── Stations.Application/
│   │   │   │   ├── Common/Interfaces/
│   │   │   │   │   ├── IStationRepository.cs
│   │   │   │   │   └── IAnpApiClient.cs
│   │   │   │   └── Features/
│   │   │   │       ├── SearchStations/
│   │   │   │       ├── GetStation/
│   │   │   │       └── CreateStation/
│   │   │   │
│   │   │   ├── Stations.Infrastructure/
│   │   │   │   ├── Persistence/
│   │   │   │   │   ├── StationsDbContext.cs
│   │   │   │   │   └── Repositories/
│   │   │   │   └── ExternalApis/
│   │   │   │       └── AnpApiClient.cs
│   │   │   │
│   │   │   └── Stations.Api/
│   │   │       ├── Grpc/Services/
│   │   │       ├── Program.cs
│   │   │       └── Dockerfile
│   │   │
│   │   └── tests/
│   │
│   ├── prices-service/
│   │   ├── PricesService.sln
│   │   ├── src/
│   │   │   ├── Prices.Domain/
│   │   │   │   ├── Entities/
│   │   │   │   │   ├── PriceSurvey.cs
│   │   │   │   │   └── FuelPrice.cs
│   │   │   │   └── ValueObjects/
│   │   │   │       ├── FuelType.cs
│   │   │   │       ├── Price.cs
│   │   │   │       └── SurveyDate.cs
│   │   │   │
│   │   │   ├── Prices.Application/
│   │   │   │   ├── Common/Interfaces/
│   │   │   │   │   └── IPriceSurveyRepository.cs
│   │   │   │   └── Features/
│   │   │   │       ├── GetPriceHistory/
│   │   │   │       ├── GetPriceTrends/
│   │   │   │       └── CreatePriceSurvey/
│   │   │   │
│   │   │   ├── Prices.Infrastructure/
│   │   │   │   ├── Persistence/
│   │   │   │   │   ├── PricesMongoContext.cs
│   │   │   │   │   └── Repositories/
│   │   │   │   └── DependencyInjection.cs
│   │   │   │
│   │   │   └── Prices.Api/
│   │   │       ├── Grpc/Services/
│   │   │       ├── Program.cs
│   │   │       └── Dockerfile
│   │   │
│   │   └── tests/
│   │
│   ├── recommendations-service/
│   │   ├── RecommendationsService.sln
│   │   ├── src/
│   │   │   ├── Recommendations.Domain/
│   │   │   │   ├── Entities/
│   │   │   │   │   ├── UserPreference.cs
│   │   │   │   │   └── Recommendation.cs
│   │   │   │   └── ValueObjects/
│   │   │   │       └── PreferredFuelType.cs
│   │   │   │
│   │   │   ├── Recommendations.Application/
│   │   │   │   ├── Common/Interfaces/
│   │   │   │   │   ├── IRecommendationEngine.cs
│   │   │   │   │   └── IClaudeApiClient.cs
│   │   │   │   └── Features/
│   │   │   │       ├── GetRecommendations/
│   │   │   │       └── SaveUserPreference/
│   │   │   │
│   │   │   ├── Recommendations.Infrastructure/
│   │   │   │   ├── Persistence/
│   │   │   │   ├── AI/
│   │   │   │   │   └── ClaudeApiClient.cs
│   │   │   │   └── ML/
│   │   │   │       └── RecommendationEngine.cs
│   │   │   │
│   │   │   └── Recommendations.Api/
│   │   │       ├── Grpc/Services/
│   │   │       ├── Program.cs
│   │   │       └── Dockerfile
│   │   │
│   │   └── tests/
│   │
│   └── ingestion-worker/
│       ├── IngestionWorker.sln
│       ├── src/
│       │   ├── Worker/
│       │   │   ├── AnpIngestionWorker.cs
│       │   │   ├── Program.cs
│       │   │   └── Dockerfile
│       │   ├── Application/
│       │   │   └── Jobs/
│       │   │       ├── StationsIngestionJob.cs
│       │   │       └── PricesIngestionJob.cs
│       │   ├── Domain/
│       │   └── Infrastructure/
│       │       ├── AnpHttpClient.cs
│       │       └── GrpcClients/
│       └── tests/
│
├── frontend/
│   ├── web/
│   │   ├── public/
│   │   ├── src/
│   │   │   ├── api/
│   │   │   │   ├── client.ts
│   │   │   │   └── endpoints/
│   │   │   ├── components/
│   │   │   │   ├── common/
│   │   │   │   └── features/
│   │   │   ├── features/
│   │   │   │   ├── auth/
│   │   │   │   ├── stations/
│   │   │   │   ├── prices/
│   │   │   │   └── recommendations/
│   │   │   ├── hooks/
│   │   │   ├── contexts/
│   │   │   ├── App.tsx
│   │   │   └── main.tsx
│   │   ├── package.json
│   │   ├── vite.config.ts
│   │   └── Dockerfile
│   │
│   └── mobile/
│       ├── lib/
│       │   ├── core/
│       │   │   ├── api/
│       │   │   ├── di/
│       │   │   └── error/
│       │   ├── features/
│       │   │   ├── auth/
│       │   │   │   ├── data/
│       │   │   │   ├── domain/
│       │   │   │   └── presentation/
│       │   │   ├── stations/
│       │   │   ├── prices/
│       │   │   └── recommendations/
│       │   └── main.dart
│       ├── pubspec.yaml
│       └── Dockerfile
│
├── infra/
│   ├── db/
│   │   ├── postgres/
│   │   │   └── init.sql
│   │   └── mongodb/
│   │       └── init.js
│   └── docker/
│       └── docker-compose.yml
│
├── .gitignore
├── README.md
└── docker-compose.yml
```

---

## Domain-Driven Design

### Bounded Contexts

#### 1. Identity Context
**Aggregate Root:** User
```csharp
public class User : Entity<Guid>
{
    public Email Email { get; private set; }
    public Password Password { get; private set; }
    public string FullName { get; private set; }
    public List<Role> Roles { get; private set; }
    public RefreshToken? RefreshToken { get; private set; }
    
    // Factory method
    public static Result<User> Create(Email email, Password password, string fullName)
    {
        // Validações de negócio
        // Raise UserRegisteredEvent
    }
}
```

**Value Objects:**
- Email (validação regex)
- Password (hash, salt, regras de complexidade)
- RefreshToken (token, expiração)

#### 2. Stations Context
**Aggregate Root:** Station
```csharp
public class Station : Entity<Guid>
{
    public Cnpj Cnpj { get; private set; }
    public string TradeName { get; private set; }
    public string Brand { get; private set; }
    public Address Address { get; private set; }
    public Coordinates Coordinates { get; private set; }
    
    public bool IsWithinRadius(Coordinates center, int radiusMeters)
    {
        // Lógica de geo-search
    }
}
```

**Value Objects:**
- Cnpj (validação)
- Coordinates (lat, lng, validação)
- Address (street, city, state, zipCode)

#### 3. Prices Context
**Aggregate Root:** PriceSurvey
```csharp
public class PriceSurvey : Entity<Guid>
{
    public Guid StationId { get; private set; }
    public SurveyDate SurveyDate { get; private set; }
    public List<FuelPrice> FuelPrices { get; private set; }
    
    public void AddFuelPrice(FuelType fuelType, Price price)
    {
        // Validações
        FuelPrices.Add(new FuelPrice(fuelType, price));
    }
}
```

**Entities:**
- FuelPrice (fuelType, price, updateDate)

**Value Objects:**
- FuelType (Gasoline, Diesel, Ethanol)
- Price (value, currency)
- SurveyDate (weekStart, weekEnd)

#### 4. Recommendations Context
**Aggregate Root:** UserPreference
```csharp
public class UserPreference : Entity<Guid>
{
    public Guid UserId { get; private set; }
    public PreferredFuelType PreferredFuel { get; private set; }
    public Coordinates HomeLocation { get; private set; }
    public List<Coordinates> FrequentRoutes { get; private set; }
    
    public Recommendation GenerateRecommendation(
        List<Station> nearbyStations,
        Dictionary<Guid, decimal> currentPrices)
    {
        // Lógica de recomendação
    }
}
```

---

## Test-Driven Development

### Estratégia de Testes

#### 1. Pirâmide de Testes
```
       /\
      /  \    E2E (10%)
     /____\
    /      \  Integration (30%)
   /________\
  /          \ Unit (60%)
 /____________\
```

#### 2. Estrutura de Testes

```csharp
// tests/Identity.Domain.Tests/Entities/UserTests.cs
public class UserTests
{
    [Fact]
    public void Create_WithValidData_ShouldSucceed()
    {
        // Arrange
        var email = Email.Create("user@example.com").Value;
        var password = Password.Create("SecureP@ss123").Value;
        var fullName = "John Doe";
        
        // Act
        var result = User.Create(email, password, fullName);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Email.Should().Be(email);
    }
    
    [Theory]
    [InlineData("invalid-email")]
    [InlineData("")]
    [InlineData(null)]
    public void Create_WithInvalidEmail_ShouldFail(string invalidEmail)
    {
        // Arrange & Act
        var emailResult = Email.Create(invalidEmail);
        
        // Assert
        emailResult.IsSuccess.Should().BeFalse();
    }
}
```

#### 3. Tipos de Testes por Camada

**Domain (Unit Tests):**
- Validações de entidades
- Lógica de negócio em value objects
- Invariantes de agregados

**Application (Unit + Integration):**
- Handlers de commands/queries
- Validadores FluentValidation
- Behaviors de pipeline

**Infrastructure (Integration):**
- Repositórios (com Testcontainers)
- APIs externas (mocks)
- Migrations

**API (Integration + E2E):**
- Endpoints REST/gRPC
- Autenticação/autorização
- Fluxos completos

---

## Roadmap Completo

### Fase 0: Setup Inicial (1-2 dias)

#### ✅ Tarefa 0.1: Criar Repositório Git
**Tempo:** 30 minutos

```bash
# Criar repositório no GitHub
# Nome: PostoCerto
# Descrição: Plataforma de comparação de preços de combustíveis

# Local
git init
git remote add origin https://github.com/seu-usuario/PostoCerto.git
git branch -M main
```

#### ✅ Tarefa 0.2: Configurar .gitignore
**Tempo:** 15 minutos

```bash
# .gitignore
# .NET
bin/
obj/
*.user
*.suo
*.cache

# Node
node_modules/
dist/
.vite/

# Flutter
.dart_tool/
.flutter-plugins
build/

# IDEs
.vscode/
.idea/
*.swp

# Environment
.env
.env.local

# Docker
docker-compose.override.yml

# Databases
*.db
*.db-shm
*.db-wal
```

#### ✅ Tarefa 0.3: Criar Estrutura de Pastas
**Tempo:** 30 minutos

```bash
mkdir -p {docs/adr,libs/{building-blocks/src,contracts/{grpc,rest}},services,frontend/{web,mobile},infra/{db,docker}}
```

#### ✅ Tarefa 0.4: README.md Inicial
**Tempo:** 30 minutos

```markdown
# PostoCerto

Plataforma de comparação de preços de combustíveis.

## Stack
- Backend: .NET 8.0
- Frontend: React 18.3 + Flutter 3.27
- Databases: PostgreSQL 16.6 + MongoDB 7.0

## Arquitetura
- Microserviços com Clean Architecture
- DDD + TDD
- gRPC interno / REST externo

## Quick Start
```bash
docker-compose up
```

## Documentação
Ver [docs/architecture.md](docs/architecture.md)
```

#### ✅ Tarefa 0.5: Primeiro Commit
**Tempo:** 15 minutos

```bash
git add .
git commit -m "chore: initial project structure"
git push -u origin main
```

---

### Fase 1: Building Blocks (2-3 dias)

#### ✅ Tarefa 1.1: Criar Projeto BuildingBlocks.Domain
**Tempo:** 2 horas

```bash
cd libs/building-blocks/src
dotnet new classlib -n BuildingBlocks.Domain -f net8.0
cd BuildingBlocks.Domain
rm Class1.cs
```

**Arquivos a criar:**

```csharp
// Entity.cs
namespace BuildingBlocks.Domain;

public abstract class Entity<TId> : IEquatable<Entity<TId>>
    where TId : notnull
{
    public TId Id { get; protected set; }
    
    protected Entity(TId id)
    {
        Id = id;
    }
    
    public override bool Equals(object? obj)
    {
        return obj is Entity<TId> entity && Id.Equals(entity.Id);
    }
    
    public bool Equals(Entity<TId>? other)
    {
        return Equals((object?)other);
    }
    
    public static bool operator ==(Entity<TId> left, Entity<TId> right)
    {
        return Equals(left, right);
    }
    
    public static bool operator !=(Entity<TId> left, Entity<TId> right)
    {
        return !Equals(left, right);
    }
    
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}

// ValueObject.cs
namespace BuildingBlocks.Domain;

public abstract class ValueObject : IEquatable<ValueObject>
{
    protected abstract IEnumerable<object?> GetEqualityComponents();
    
    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
        {
            return false;
        }
        
        var other = (ValueObject)obj;
        
        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }
    
    public bool Equals(ValueObject? other)
    {
        return Equals((object?)other);
    }
    
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }
    
    public static bool operator ==(ValueObject left, ValueObject right)
    {
        return Equals(left, right);
    }
    
    public static bool operator !=(ValueObject left, ValueObject right)
    {
        return !Equals(left, right);
    }
}

// Result.cs
namespace BuildingBlocks.Domain;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error? Error { get; }
    
    protected Result(bool isSuccess, Error? error)
    {
        if (isSuccess && error != null)
            throw new InvalidOperationException();
        if (!isSuccess && error == null)
            throw new InvalidOperationException();
            
        IsSuccess = isSuccess;
        Error = error;
    }
    
    public static Result Success() => new(true, null);
    public static Result Failure(Error error) => new(false, error);
    
    public static Result<T> Success<T>(T value) => new(value, true, null);
    public static Result<T> Failure<T>(Error error) => new(default, false, error);
}

public class Result<T> : Result
{
    public T? Value { get; }
    
    protected internal Result(T? value, bool isSuccess, Error? error)
        : base(isSuccess, error)
    {
        Value = value;
    }
}

// Error.cs
namespace BuildingBlocks.Domain;

public record Error(string Code, string Message)
{
    public static Error None = new(string.Empty, string.Empty);
    public static Error NullValue = new("Error.NullValue", "Value cannot be null");
}

// DomainEvent.cs
namespace BuildingBlocks.Domain;

public abstract record DomainEvent(Guid Id, DateTime OccurredOn);
```

**Commit:**
```bash
git add .
git commit -m "feat: add BuildingBlocks.Domain with Entity, ValueObject, Result"
```

#### ✅ Tarefa 1.2: Criar Projeto BuildingBlocks.Application
**Tempo:** 1 hora

```bash
cd libs/building-blocks/src
dotnet new classlib -n BuildingBlocks.Application -f net8.0
cd BuildingBlocks.Application
rm Class1.cs
```

```csharp
// ICommand.cs
using MediatR;
using BuildingBlocks.Domain;

namespace BuildingBlocks.Application;

public interface ICommand : IRequest<Result>
{
}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}

// IQuery.cs
using MediatR;
using BuildingBlocks.Domain;

namespace BuildingBlocks.Application;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}

// IUnitOfWork.cs
namespace BuildingBlocks.Application;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

**Adicionar PackageReference:**
```xml
<PackageReference Include="MediatR" Version="12.4.1" />
```

**Commit:**
```bash
git add .
git commit -m "feat: add BuildingBlocks.Application with CQRS interfaces"
```

#### ✅ Tarefa 1.3: Criar Contratos gRPC
**Tempo:** 2 horas

```bash
cd libs/contracts/grpc
```

**Criar arquivos .proto (identity.proto, stations.proto, prices.proto, recommendations.proto)**

Ver seção completa de contratos gRPC no documento original.

**Commit:**
```bash
git add .
git commit -m "feat: add gRPC contracts for all services"
```

---

### Fase 2: Identity Service (5-7 dias) - TDD

#### ✅ Tarefa 2.1: Criar Estrutura de Projetos
**Tempo:** 1 hora

```bash
cd services
mkdir identity-service
cd identity-service

dotnet new sln -n IdentityService

# Criar projetos
dotnet new classlib -n Identity.Domain -f net8.0
dotnet new classlib -n Identity.Application -f net8.0
dotnet new classlib -n Identity.Infrastructure -f net8.0
dotnet new webapi -n Identity.Api -f net8.0

# Testes
dotnet new xunit -n Identity.Domain.Tests -f net8.0
dotnet new xunit -n Identity.Application.Tests -f net8.0
dotnet new xunit -n Identity.Infrastructure.Tests -f net8.0
dotnet new xunit -n Identity.Api.Tests -f net8.0

# Adicionar à solution
dotnet sln add **/*.csproj

# Referências
cd Identity.Application
dotnet add reference ../Identity.Domain/Identity.Domain.csproj
dotnet add reference ../../../libs/building-blocks/src/BuildingBlocks.Domain/BuildingBlocks.Domain.csproj
dotnet add reference ../../../libs/building-blocks/src/BuildingBlocks.Application/BuildingBlocks.Application.csproj

cd ../Identity.Infrastructure
dotnet add reference ../Identity.Domain/Identity.Domain.csproj
dotnet add reference ../Identity.Application/Identity.Application.csproj

cd ../Identity.Api
dotnet add reference ../Identity.Application/Identity.Application.csproj
dotnet add reference ../Identity.Infrastructure/Identity.Infrastructure.csproj
```

**Commit:**
```bash
git add .
git commit -m "feat: create Identity Service project structure"
```

#### ✅ Tarefa 2.2-2.7: Implementar Domain, Application, Infrastructure, API
**Tempo:** 4-6 dias

Ver detalhes completos no documento original com exemplos de TDD para:
- Value Objects (Email, Password)
- Entities (User)
- Commands/Queries (RegisterUser, Login)
- Repositories (EF Core)
- gRPC Services

---

### Fase 3: Docker Setup (1 dia)

#### ✅ Tarefa 3.1: Criar docker-compose.yml
**Tempo:** 2 horas

```yaml
# docker-compose.yml
version: '3.8'

services:
  postgres:
    image: postgres:16.6-alpine
    container_name: postocerto-postgres
    environment:
      POSTGRES_USER: postocerto
      POSTGRES_PASSWORD: dev123
      POSTGRES_DB: postgres
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data
      - ./infra/db/postgres/init.sql:/docker-entrypoint-initdb.d/init.sql
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postocerto"]
      interval: 10s
      timeout: 5s
      retries: 5
    networks:
      - postocerto-network

  mongodb:
    image: mongo:7.0.15
    container_name: postocerto-mongodb
    environment:
      MONGO_INITDB_ROOT_USERNAME: postocerto
      MONGO_INITDB_ROOT_PASSWORD: dev123
    ports:
      - "27017:27017"
    volumes:
      - mongo_data:/data/db
      - ./infra/db/mongodb/init.js:/docker-entrypoint-initdb.d/init.js
    healthcheck:
      test: echo 'db.runCommand("ping").ok' | mongosh localhost:27017/test --quiet
      interval: 10s
      timeout: 5s
      retries: 5
    networks:
      - postocerto-network

  identity-service:
    build:
      context: ./services/identity-service
      dockerfile: src/Identity.Api/Dockerfile
    container_name: postocerto-identity
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__IdentityDb=Host=postgres;Database=postocerto_identity;Username=postocerto;Password=dev123
    ports:
      - "7001:7001"
    depends_on:
      postgres:
        condition: service_healthy
    networks:
      - postocerto-network

volumes:
  postgres_data:
  mongo_data:

networks:
  postocerto-network:
    driver: bridge
```

**Criar scripts de inicialização de banco:**

```sql
-- infra/db/postgres/init.sql
CREATE DATABASE postocerto_identity;
CREATE DATABASE postocerto_stations;

\c postocerto_stations;
CREATE EXTENSION IF NOT EXISTS postgis;
```

```javascript
// infra/db/mongodb/init.js
db = db.getSiblingDB('admin');

db = db.getSiblingDB('postocerto_prices');
db.createCollection('price_surveys');

db = db.getSiblingDB('postocerto_recommendations');
db.createCollection('user_preferences');
db.createCollection('recommendations');
```

**Commit:**
```bash
git add .
git commit -m "feat: add Docker Compose setup"
```

---

### Fase 4: Stations Service (5-7 dias)

Seguir mesmo padrão do Identity Service:
1. Criar estrutura de projetos
2. Domain (TDD): Station entity, Value Objects (Cnpj, Coordinates, Address)
3. Application (TDD): Commands/Queries para busca geoespacial
4. Infrastructure: EF Core com PostGIS
5. API: gRPC Service
6. Testes completos

**Commit incremental após cada camada**

---

### Fase 5: Prices Service (5-7 dias)

Seguir padrão similar com MongoDB:
1. Criar estrutura de projetos
2. Domain (TDD): PriceSurvey aggregate
3. Application (TDD): Commands/Queries para histórico
4. Infrastructure: MongoDB Driver
5. API: gRPC Service
6. Testes completos

---

### Fase 6: Recommendations Service (7-10 dias)

Incluir integração com IA:
1. Criar estrutura de projetos
2. Domain (TDD): UserPreference aggregate
3. Application (TDD): Lógica de recomendação
4. Infrastructure: Claude API Client + ML.NET
5. API: gRPC Service
6. Testes (com mocks da API)

---

### Fase 7: API Gateway (3-5 dias)

1. Criar projeto Minimal API
2. Configurar gRPC clients para todos os serviços
3. Implementar endpoints REST públicos
4. Middleware de autenticação
5. Agregação de dados
6. OpenAPI/Swagger
7. Testes E2E

---

### Fase 8: Ingestion Worker (3-5 dias)

1. Criar projeto Worker Service
2. Configurar Quartz.NET
3. Implementar jobs de ingestão ANP
4. Políticas de retry com Polly
5. gRPC clients para Stations e Prices
6. Logging e observabilidade
7. Testes

---

### Fase 9: Frontend React (3-4 semanas)

#### ✅ Tarefa 9.1: Setup React com Vite
**Tempo:** 1 hora

```bash
cd frontend
npm create vite@latest web -- --template react-ts
cd web
npm install

# Dependências
npm install react-router-dom@6.28.0
npm install @tanstack/react-query@5.62.7
npm install axios@1.7.9
npm install leaflet@1.9.4 react-leaflet@4.2.1
npm install @types/leaflet -D
```

#### Estrutura de Features:
- Auth (login, registro, profile)
- Stations (mapa, lista, busca)
- Prices (histórico, comparação)
- Recommendations (dashboard personalizado)

**Tempo total: 3-4 semanas**

---

### Fase 10: Mobile Flutter (3-4 semanas)

#### ✅ Tarefa 10.1: Setup Flutter
**Tempo:** 1 hora

```bash
cd frontend
flutter create mobile
cd mobile

# Dependências
flutter pub add dio
flutter pub add flutter_bloc
flutter pub add get_it
flutter pub add freezed_annotation
flutter pub add json_annotation

flutter pub add --dev build_runner
flutter pub add --dev freezed
flutter pub add --dev json_serializable
```

#### Estrutura Clean Architecture:
- Core (API client, DI, Error handling)
- Features (Auth, Stations, Prices, Recommendations)
  - Data (DataSources, Models, Repositories)
  - Domain (Entities, Repositories, UseCases)
  - Presentation (BLoC, Pages, Widgets)

**Tempo total: 3-4 semanas**

---

## 📊 Cronograma Resumido

| Fase | Descrição | Tempo Estimado | Semanas |
|------|-----------|----------------|---------|
| 0 | Setup Inicial | 1-2 dias | 0.5 |
| 1 | Building Blocks | 2-3 dias | 0.5 |
| 2 | Identity Service (TDD) | 5-7 dias | 1.5 |
| 3 | Docker Setup | 1 dia | 0.2 |
| 4 | Stations Service (TDD) | 5-7 dias | 1.5 |
| 5 | Prices Service (TDD) | 5-7 dias | 1.5 |
| 6 | Recommendations (IA) | 7-10 dias | 2 |
| 7 | API Gateway | 3-5 dias | 1 |
| 8 | Ingestion Worker | 3-5 dias | 1 |
| 9 | React Web | 3-4 semanas | 3.5 |
| 10 | Flutter Mobile | 3-4 semanas | 3.5 |
| **Total** | | **16-20 semanas** | **~5 meses** |

---

## 🎯 Checklist de Progresso

### ⬜ Fase 0: Setup Inicial
- [ ] Criar repositório Git
- [ ] Configurar .gitignore
- [ ] Criar estrutura de pastas
- [ ] README.md inicial
- [ ] Primeiro commit

### ⬜ Fase 1: Building Blocks
- [ ] BuildingBlocks.Domain
- [ ] BuildingBlocks.Application
- [ ] Contratos gRPC

### ⬜ Fase 2: Identity Service
- [ ] Estrutura de projetos
- [ ] Domain (TDD)
- [ ] Application (TDD)
- [ ] Infrastructure (EF Core)
- [ ] API (gRPC)
- [ ] Testes

### ⬜ Fase 3: Docker
- [ ] docker-compose.yml
- [ ] Scripts de inicialização DB
- [ ] Dockerfiles
- [ ] Testes de integração

### ⬜ Fase 4: Stations Service
- [ ] Estrutura de projetos
- [ ] Domain (TDD)
- [ ] Application (TDD)
- [ ] Infrastructure (PostGIS)
- [ ] API (gRPC)
- [ ] Testes

### ⬜ Fase 5: Prices Service
- [ ] Estrutura de projetos
- [ ] Domain (TDD)
- [ ] Application (TDD)
- [ ] Infrastructure (MongoDB)
- [ ] API (gRPC)
- [ ] Testes

### ⬜ Fase 6: Recommendations Service
- [ ] Estrutura de projetos
- [ ] Domain (TDD)
- [ ] Application (TDD)
- [ ] Infrastructure (IA + ML)
- [ ] API (gRPC)
- [ ] Testes

### ⬜ Fase 7: API Gateway
- [ ] Projeto Minimal API
- [ ] gRPC Clients
- [ ] Endpoints REST
- [ ] Autenticação
- [ ] OpenAPI/Swagger
- [ ] Testes E2E

### ⬜ Fase 8: Ingestion Worker
- [ ] Worker Service
- [ ] Quartz.NET
- [ ] Jobs de ingestão
- [ ] Polly retry
- [ ] Testes

### ⬜ Fase 9: React Web
- [ ] Setup Vite + React
- [ ] Estrutura de features
- [ ] Auth feature
- [ ] Stations feature
- [ ] Prices feature
- [ ] Recommendations feature
- [ ] Testes

### ⬜ Fase 10: Flutter Mobile
- [ ] Setup Flutter
- [ ] Estrutura Clean Arch
- [ ] Auth feature
- [ ] Stations feature
- [ ] Prices feature
- [ ] Recommendations feature
- [ ] Testes

---

## 📝 Notas Importantes

### Convenções de Commit
```bash
# Tipos
feat: nova funcionalidade
fix: correção de bug
test: adicionar/modificar testes
refactor: refatoração de código
docs: documentação
chore: tarefas de manutenção
style: formatação de código

# Exemplos
git commit -m "feat: add Email value object"
git commit -m "test: add UserTests with TDD"
git commit -m "refactor: improve password hashing"
```

### Boas Práticas
1. **TDD**: Red → Green → Refactor
2. **Commits pequenos e frequentes**
3. **Testes antes de mergear**
4. **Code review (mesmo solo)**
5. **Documentar decisões arquiteturais**

### Recursos de Apoio
- **gRPC:** https://grpc.io/docs/languages/csharp/
- **EF Core:** https://learn.microsoft.com/ef/core/
- **Clean Architecture:** https://blog.cleancoder.com/
- **DDD:** https://domainlanguage.com/ddd/
- **TDD:** https://martinfowler.com/bliki/TestDrivenDevelopment.html

---

## 🚀 Como Usar Este Documento

1. **Clone o repositório**
2. **Abra este arquivo no VS Code**
3. **Use a extensão Markdown Preview**
4. **Marque checkboxes conforme progride**
5. **Siga a ordem das fases**
6. **Commit após cada tarefa**

---

**Autor:** Arquitetura PostoCerto  
**Versão:** 1.0.0  
**Data:** 2025-01-01  
**Status:** Pronto para Desenvolvimento

**Próximo Passo:** Começar pela Fase 0 - Tarefa 0.1 (Criar Repositório Git)
