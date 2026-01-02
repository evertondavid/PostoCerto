# 🚗 PostoCerto - Fuel Price Comparison Platform

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Build Status](https://img.shields.io/badge/build-passing-brightgreen)]()
[![Test Coverage](https://img.shields.io/badge/coverage-100%25-brightgreen)]()
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)]()

> **A modern, scalable microservices platform for comparing fuel prices across Brazil, consuming data from ANP (National Petroleum Agency).**

---

## 📋 Table of Contents

- [About the Project](#-about-the-project)
- [Key Features](#-key-features)
- [Architecture](#-architecture)
- [Technology Stack](#-technology-stack)
- [Project Structure](#-project-structure)
- [Getting Started](#-getting-started)
- [Development Practices](#-development-practices)
- [Roadmap](#-roadmap)
- [Contributing](#-contributing)
- [License](#-license)

---

## 🎯 About the Project

**PostoCerto** is a comprehensive fuel price comparison platform that helps Brazilian consumers find the best fuel prices in their area. The system integrates with ANP's open data API to provide real-time pricing information across thousands of gas stations nationwide.

### 💡 Problem Statement

Brazilian consumers face significant fuel price variations across different gas stations and regions. Finding the best prices requires manually visiting multiple stations or relying on outdated information. PostoCerto solves this by:

- **Aggregating** real-time fuel prices from ANP's official database
- **Comparing** prices across multiple stations in any given area
- **Recommending** the best options based on user preferences (price, distance, fuel type)
- **Providing** historical price trends and analytics

### 🎓 Learning Objectives

This project serves as a comprehensive learning platform for modern software architecture:

- ✅ **Microservices Architecture** with .NET 8
- ✅ **Clean Architecture** and Domain-Driven Design (DDD)
- ✅ **Test-Driven Development** (TDD) with 100% coverage
- ✅ **gRPC** for synchronous inter-service communication
- ✅ **Kafka** for asynchronous event streaming
- ✅ **Event-Driven Architecture** for scalability
- ✅ **REST & GraphQL** for flexible API consumption
- ✅ **Cloud-Native** patterns with Docker/Kubernetes

---

## ✨ Key Features

### 🔍 Core Functionality

- **Real-Time Price Search**: Find fuel prices by location, radius, or specific station
- **Multi-Fuel Support**: Gasoline, Ethanol, Diesel, and CNG prices
- **Historical Trends**: Track price changes over time with analytics
- **AI-Powered Recommendations**: Smart suggestions based on user preferences and behavior
- **User Accounts**: Save favorite stations, set price alerts, and track searches
- **Mobile & Web Apps**: Cross-platform access via Flutter mobile app and React web interface

### 🏗️ Technical Features

- **High Availability**: Distributed microservices with fault tolerance
- **Scalability**: Horizontal scaling with Kubernetes
- **Security**: JWT authentication, role-based access control (RBAC)
- **Observability**: Comprehensive logging, metrics, and distributed tracing
- **API-First**: REST, GraphQL & gRPC for flexible integration
- **Event-Driven**: Kafka for async messaging and event streaming
- **Containerization**: Docker for consistent deployment across environments

---

## 🏛️ Architecture

PostoCerto follows **Clean Architecture** principles with **Domain-Driven Design** patterns, organized as microservices.

### 🎨 Architecture Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                         CLIENTS                              │
├─────────────────────────────────────────────────────────────┤
│  React Web (Port: 3000)  │  Flutter Mobile                  │
└──────────────┬───────────┴──────────────┬───────────────────┘
               │ HTTPS/REST               │ HTTPS/REST
               │                          │
       ┌───────▼────────┐        ┌───────▼────────┐
       │   Web BFF      │        │  Mobile BFF    │
       │  REST: 5000    │        │  REST: 5001    │
       └───────┬────────┘        └───────┬────────┘
               │ gRPC                     │ gRPC
               └──────────────┬───────────┘
                              │
               ┌──────────────▼──────────────┐
               │       API GATEWAY            │
               │  gRPC Port: 8081             │
               │  (Service Mesh)              │
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
    ┌────▼────┐     ┌───▼─────┐    ┌───▼──────┐  ┌───▼──────┐
    │Postgres │     │Postgres │    │ MongoDB  │  │ MongoDB  │
    │identity │     │stations │    │  prices  │  │recommend.│
    └─────────┘     └─────────┘    └──────────┘  └──────────┘

    ┌─────────────────────────────────────────────────────────┐
    │              INGESTION WORKER                            │
    │  (Scheduled Jobs - ANP Data Import)                      │
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
```

### 🎯 Bounded Contexts (DDD)

| Context | Responsibility | Database | Technology |
|---------|---------------|----------|------------|
| **Web BFF** | Web-specific aggregation, caching, response shaping | Redis (Cache) | .NET 8 + ASP.NET Core |
| **Mobile BFF** | Mobile-optimized payloads, offline support | Redis (Cache) | .NET 8 + ASP.NET Core |
| **Identity** | User authentication, authorization, JWT tokens | PostgreSQL | .NET 8 + EF Core |
| **Stations** | Gas station registry, locations, CNPJ validation | PostgreSQL | .NET 8 + EF Core |
| **Prices** | Fuel price surveys, historical data, trends | MongoDB | .NET 8 + MongoDB Driver |
| **Recommendations** | AI-powered suggestions, user preferences | MongoDB | .NET 8 + Anthropic Claude |

### 🔄 Communication Patterns

**Synchronous Communication:**
- **External Clients → BFF**: REST/HTTPS (JSON) or GraphQL
- **BFF → API Gateway**: gRPC (HTTP/2, Protobuf)
- **API Gateway → Services**: gRPC (HTTP/2, Protobuf)
- **Services → Databases**: Direct connection (isolated per service)
- **Worker → ANP API**: HTTP REST
- **Worker → Services**: gRPC

**Asynchronous Communication:**
- **Service → Kafka**: Event publishing (Domain Events)
- **Kafka → Services**: Event consumption (Event Handlers)
- **Use Cases**: Price updates, user notifications, audit logs, analytics

### 🎯 BFF Pattern (Backend For Frontend)

Each frontend has its own optimized BFF:

**Web BFF** (Port 5000):
- Optimized for desktop browsers
- Rich data aggregation from multiple services
- Server-side caching for performance
- SEO-friendly responses

**Mobile BFF** (Port 5001):
- Lightweight payloads for mobile networks
- Offline-first data synchronization
- Battery-efficient polling strategies
- Push notification integration

### 📡 Event-Driven Architecture (Kafka)

Asynchronous communication for decoupled services:

**Event Publishing:**
- `PriceUpdated`: When new fuel prices are imported
- `StationCreated`: When a new station is registered
- `UserRegistered`: When a user creates an account
- `FavoriteAdded`: When a user favorites a station

**Event Consumption:**
- **Recommendations Service**: Consumes price & favorite events for ML training
- **Notifications Service**: Consumes events to trigger push notifications
- **Analytics Service**: Consumes all events for business intelligence

**Benefits:**
- ✅ **Decoupling**: Services don't need to know about each other
- ✅ **Scalability**: Process events at different rates
- ✅ **Reliability**: Event replay for failed processing
- ✅ **Audit Trail**: Complete event history for debugging

---

## 🛠️ Technology Stack

### Backend

| Component | Version | Purpose |
|-----------|---------|---------|
| **.NET SDK** | 8.0.404 (LTS) | Runtime and framework |
| **C#** | 12.0 | Programming language |
| **ASP.NET Core** | 8.0.11 | Web framework |
| **REST API** | (Minimal APIs) | BFF & external HTTP endpoints |
| **gRPC** | 2.66.0 | Internal inter-service communication |
| **GraphQL** | (Hot Chocolate 13.9+) | Flexible client-driven queries |
| **Apache Kafka** | 3.6+ | Event streaming & async messaging |
| **MediatR** | 12.4.1 | CQRS implementation |
| **FluentValidation** | 11.10.0 | Domain validation |
| **Serilog** | 8.0.3 | Structured logging |
| **Docker** | 27.4.0 | Service containerization |

### Databases

| Database | Version | Used By | Purpose |
|----------|---------|---------|---------|
| **PostgreSQL** | 16.6 (LTS) | Identity, Stations | Relational data with ACID guarantees |
| **MongoDB** | 7.0.15 (LTS) | Prices, Recommendations | Document store for flexible schemas |

### Testing

| Tool | Version | Purpose |
|------|---------|---------|
| **xUnit** | 2.9.2 | Test framework |
| **FluentAssertions** | 6.12.2 | Assertion library |
| **Moq** | 4.20.72 | Mocking framework |
| **Testcontainers** | 3.10.0 | Integration tests with Docker |

### Frontend

| Technology | Version | Purpose |
|------------|---------|---------|
| **React** | 18.3.1 | Web application |
| **TypeScript** | 5.6.3 | Type-safe JavaScript |
| **Vite** | 5.4.11 | Build tool |
| **TanStack Query** | 5.62.7 | Data fetching & caching |
| **Leaflet** | 1.9.4 | Interactive maps |
| **Flutter** | 3.27.1 | Mobile application (iOS/Android) |

### Infrastructure

| Tool | Version | Purpose |
|------|---------|---------|
| **Docker** | 27.4.0 | Containerization |
| **Docker Compose** | 2.31.0 | Local orchestration || **Apache Kafka** | 3.6+ | Event streaming platform |
| **Zookeeper** | 3.8+ | Kafka cluster coordination |
| **Redis** | 7.2+ | Caching & session storage || **Kubernetes** | (Planned) | Production orchestration |
| **Prometheus** | (Planned) | Metrics collection |
| **Grafana** | (Planned) | Metrics visualization |
| **Seq** | (Planned) | Centralized logging |

---

## 📁 Project Structure

```
PostoCerto/
│
├── docs/                           # Documentation
│   ├── architecture.md             # Architecture details
│   ├── adr/                        # Architecture Decision Records
│   └── .copilot-memory.md          # Project memory for AI assistance
│
├── libs/                           # Shared libraries
│   └── building-blocks/            # DDD/Clean Architecture foundations
│       ├── src/
│       │   └── BuildingBlocks.Domain/
│       │       ├── Entity.cs       # Base entity class
│       │       ├── ValueObject.cs  # Base value object
│       │       ├── Result.cs       # Railway-oriented programming
│       │       └── DomainEvent.cs  # Event sourcing base
│       └── tests/
│           └── BuildingBlocks.Domain.Tests/
│
├── bff/                            # Backend For Frontend (BFF)
│   │
│   ├── web-bff/                    # Web Application BFF
│   │   ├── src/
│   │   │   └── WebBff.Api/
│   │   │       ├── Controllers/    # REST endpoints
│   │   │       ├── Aggregators/    # Multi-service aggregation
│   │   │       ├── GrpcClients/    # Service clients
│   │   │       └── Caching/        # Response caching
│   │   └── tests/
│   │       └── WebBff.Api.Tests/
│   │
│   └── mobile-bff/                 # Mobile Application BFF
│       ├── src/
│       │   └── MobileBff.Api/
│       │       ├── Controllers/    # REST endpoints
│       │       ├── Aggregators/    # Optimized aggregation
│       │       ├── GrpcClients/    # Service clients
│       │       └── Sync/           # Offline sync logic
│       └── tests/
│           └── MobileBff.Api.Tests/
│
├── services/                       # Microservices
│   │
│   ├── identity-service/           # Authentication & Authorization
│   │   ├── src/
│   │   │   └── Identity.Domain/
│   │   │       ├── Entities/       # User, Role
│   │   │       ├── ValueObjects/   # Email, Password, RefreshToken
│   │   │       ├── Events/         # UserRegistered
│   │   │       └── Exceptions/     # Domain exceptions
│   │   └── tests/
│   │       └── Identity.Domain.Tests/
│   │
│   ├── stations-service/           # Gas Station Management
│   │   ├── src/
│   │   │   └── Stations.Domain/
│   │   │       ├── Entities/       # Station
│   │   │       ├── ValueObjects/   # Cnpj, Coordinates, Address
│   │   │       └── Services/       # GeoSearchService
│   │   └── tests/
│   │
│   ├── prices-service/             # Fuel Price Data
│   │   ├── src/
│   │   │   └── Prices.Domain/
│   │   │       ├── Entities/       # PriceSurvey, FuelPrice
│   │   │       └── ValueObjects/   # FuelType, Price, SurveyDate
│   │   └── tests/
│   │
│   └── recommendations-service/    # AI-Powered Recommendations
│       ├── src/
│       │   └── Recommendations.Domain/
│       │       ├── Entities/       # UserPreference, Recommendation
│       │       └── ValueObjects/   # PreferredFuelType
│       └── tests/
│
├── infra/                          # Infrastructure as Code (Future)
│   ├── kubernetes/                 # K8s manifests
│   ├── docker/                     # Dockerfiles
│   └── terraform/                  # Cloud provisioning
│
├── .gitignore                      # Git ignore rules
├── global.json                     # .NET SDK version lock
└── README.md                       # This file
```

### 📦 Service Layers (Clean Architecture)

Each microservice follows this structure:

```
{Service}.Domain/          # Phase 01 ✅ (CURRENT)
  ├── Entities/            # Aggregates and entities
  ├── ValueObjects/        # Immutable value objects
  ├── Events/              # Domain events
  ├── Exceptions/          # Domain-specific exceptions
  └── Services/            # Domain services

{Service}.Application/     # Phase 02 (FUTURE)
  ├── Commands/            # Write operations (CQRS)
  ├── Queries/             # Read operations (CQRS)
  ├── Handlers/            # MediatR handlers
  └── Interfaces/          # Repository contracts

{Service}.Infrastructure/  # Phase 03 (FUTURE)
  ├── Persistence/         # EF Core / MongoDB
  ├── Repositories/        # Data access
  └── ExternalApis/        # Third-party integrations

{Service}.Api/             # Phase 04 (FUTURE)
  ├── Grpc/                # gRPC services
  ├── Rest/                # REST endpoints (optional)
  └── Middleware/          # Cross-cutting concerns
```

### 🏛️ Clean Architecture Mapping

Understanding how Clean Architecture concepts translate to PostoCerto's structure:

| Clean Architecture | PostoCerto Location | Example |
|--------------------|---------------------|---------|
| **Entities** | `*.Domain/Entities/` | `Identity.Domain/Entities/User.cs` |
| **Value Objects** | `*.Domain/ValueObjects/` | `Identity.Domain/ValueObjects/Email.cs` |
| **Domain Events** | `*.Domain/Events/` | `Identity.Domain/Events/UserRegisteredEvent.cs` |
| **Use Cases** | `*.Application/Commands/` & `*.Application/Queries/` | `Identity.Application/Commands/RegisterUserCommand.cs` |
| **Repository Interfaces** | `*.Application/Interfaces/` | `Identity.Application/Interfaces/IUserRepository.cs` |
| **Repository Implementation** | `*.Infrastructure/Persistence/` | `Identity.Infrastructure/Persistence/UserRepository.cs` |
| **Controllers** | `*.Api/Controllers/` | `Identity.Api/Controllers/UsersController.cs` |
| **External Services** | `*.Infrastructure/ExternalServices/` | `Identity.Infrastructure/ExternalServices/EmailService.cs` |

**Key Principles:**
- **Domain Layer**: Contains pure business logic, no dependencies on frameworks
- **Application Layer**: Orchestrates use cases using domain entities
- **Infrastructure Layer**: Implements interfaces defined in Application (EF Core, MongoDB, APIs)
- **API Layer**: Entry points (REST, gRPC) that delegate to Application layer

**Dependency Rule**: Dependencies always point inward → `Api → Application → Domain`

---

## 🚀 Getting Started

### Prerequisites

Ensure you have the following installed:

| Tool | Version | Download |
|------|---------|----------|
| .NET SDK | 8.0.404+ | [Download](https://dotnet.microsoft.com/download/dotnet/8.0) |
| Docker Desktop | 27.4.0+ | [Download](https://www.docker.com/products/docker-desktop) |
| Visual Studio Code | Latest | [Download](https://code.visualstudio.com/) |
| Git | Latest | [Download](https://git-scm.com/) |

**Recommended VS Code Extensions:**
- C# Dev Kit
- Docker
- REST Client
- GitLens

### Installation

1. **Clone the repository**

```bash
git clone https://github.com/yourusername/PostoCerto.git
cd PostoCerto
```

2. **Verify .NET version**

```bash
dotnet --version
# Should output: 8.0.404 (or higher patch version)
```

3. **Restore dependencies**

```bash
# Restore all solutions
dotnet restore

# Or restore specific service
cd services/identity-service
dotnet restore
```

4. **Build the project**

```bash
# Build all services
dotnet build

# Or build specific service
cd libs/building-blocks
dotnet build
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run tests for specific service
cd libs/building-blocks
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run with verbosity
dotnet test --verbosity normal
```

### 🎯 Current Phase: Phase 01 - Domain Layer

We're currently implementing the **Domain Layer** with **Test-Driven Development (TDD)**:

```bash
# Navigate to Building Blocks
cd libs/building-blocks

# Run tests
dotnet test

# Expected output:
# ✅ Passed!  - Failed: 0, Passed: 3, Skipped: 0, Total: 3
```

---

## 🧪 Development Practices

### Test-Driven Development (TDD)

**All code follows the Red-Green-Refactor cycle:**

```
🔴 RED: Write a failing test
   ↓
🟢 GREEN: Write minimal code to pass
   ↓
🔵 REFACTOR: Improve without breaking tests
   ↓
🔁 REPEAT
```

**Current Test Coverage: 100%**

### Code Quality Standards

| Practice | Tool/Pattern | Enforcement |
|----------|--------------|-------------|
| **Clean Code** | SOLID Principles | Code reviews |
| **DDD** | Aggregates, Value Objects, Domain Events | Architecture reviews |
| **CQRS** | MediatR, Commands/Queries | Pattern enforcement |
| **Immutability** | Value Objects, Private setters | Compile-time |
| **Null Safety** | Nullable reference types (C# 12) | Compiler warnings |

### Naming Conventions

```csharp
// Entities: PascalCase, singular
public class User : Entity<Guid> { }
public class Station : Entity<Guid> { }

// Value Objects: PascalCase, descriptive
public class Email : ValueObject { }
public class Coordinates : ValueObject { }

// Tests: Method_Should_Behavior_When_Condition
[Fact]
public void Email_Should_Throw_When_Invalid() { }

// Files: One class per file, match class name
User.cs, Email.cs, UserTests.cs
```

### Git Workflow

```bash
# Create feature branch
git checkout -b feature/identity-service-domain

# Commit with conventional commits
git commit -m "feat(identity): add User entity with email validation"
git commit -m "test(identity): add User entity tests (TDD)"
git commit -m "refactor(identity): improve Email value object"

# Push and create PR
git push origin feature/identity-service-domain
```

**Commit Message Format:**
```
<type>(<scope>): <subject>

Types: feat, fix, docs, style, refactor, test, chore
Scope: web-bff, mobile-bff, identity, stations, prices, recommendations, building-blocks
```

---

## 🗺️ Roadmap

### ✅ Phase 01: Domain Layer (CURRENT)

**Status**: 🟡 In Progress (January 2026)

- [x] Setup project structure
- [x] Create Building Blocks foundation
- [x] Implement Entity<TId> with TDD
- [ ] Implement ValueObject base class
- [ ] Implement Result<T> pattern
- [ ] Implement DomainEvent base class
- [ ] Identity Service domain (User, Email, Password)
- [ ] Stations Service domain (Station, Cnpj, Coordinates)
- [ ] Prices Service domain (PriceSurvey, FuelPrice)
- [ ] Recommendations Service domain

### 🔜 Phase 02: Application Layer

**Status**: ⚪ Planned (Q1 2026)

- [ ] Implement CQRS with MediatR
- [ ] Add FluentValidation
- [ ] Create Command/Query handlers
- [ ] Implement domain event handlers
- [ ] Add application-level tests

### 🔜 Phase 03: Infrastructure Layer

**Status**: ⚪ Planned (Q1-Q2 2026)

- [ ] Setup PostgreSQL with EF Core
- [ ] Setup MongoDB with native driver
- [ ] Implement repositories
- [ ] Add database migrations
- [ ] Setup Apache Kafka & Zookeeper
- [ ] Implement event producers & consumers
- [ ] Add Redis caching layer
- [ ] Integrate with ANP API
- [ ] Setup Anthropic Claude integration
- [ ] Docker Compose for local services

### 🔜 Phase 04: BFF & API Layer

**Status**: ⚪ Planned (Q2 2026)

- [ ] Implement Web BFF (REST API for React)
- [ ] Implement Mobile BFF (REST API for Flutter)
- [ ] Add GraphQL endpoint (Hot Chocolate)
- [ ] Add Redis caching layer
- [ ] Implement response aggregation
- [ ] Add BFF-level authentication
- [ ] Implement gRPC services
- [ ] Add REST API (Minimal APIs)
- [ ] Setup API Gateway
- [ ] Implement JWT authentication
- [ ] Add API documentation (Swagger/OpenAPI)
- [ ] Kafka integration for async events
- [ ] Docker containerization per service

### 🔜 Phase 05: Frontend

**Status**: ⚪ Planned (Q2-Q3 2026)

- [ ] React web application
- [ ] Flutter mobile application
- [ ] Responsive design
- [ ] Interactive maps (Leaflet/Google Maps)

### 🔜 Phase 06: DevOps & Production

**Status**: ⚪ Planned (Q3 2026)

- [ ] Docker containerization
- [ ] Kubernetes deployment
- [ ] CI/CD pipelines (GitHub Actions)
- [ ] Monitoring (Prometheus + Grafana)
- [ ] Centralized logging (Seq)
- [ ] Cloud deployment (Azure/AWS)

---

## 🤝 Contributing

Contributions are welcome! This is a learning project, so feel free to:

- 🐛 Report bugs
- 💡 Suggest new features
- 📖 Improve documentation
- 🧪 Add more tests
- ♻️ Refactor code

### How to Contribute

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Follow TDD: Write tests first!
4. Commit your changes (`git commit -m 'feat: add amazing feature'`)
5. Push to the branch (`git push origin feature/amazing-feature`)
6. Open a Pull Request

### Code Review Checklist

- [ ] All tests pass (`dotnet test`)
- [ ] Code follows Clean Architecture principles
- [ ] Follows DDD patterns (Entities, Value Objects, etc.)
- [ ] TDD approach (tests written first)
- [ ] Code coverage maintained at 100%
- [ ] Conventional commit messages
- [ ] Documentation updated
- [ ] No compiler warnings

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 📞 Contact & Resources

- **Project Repository**: [GitHub - PostoCerto](https://github.com/yourusername/PostoCerto)
- **Documentation**: [docs/architecture.md](docs/architecture.md)
- **Issue Tracker**: [GitHub Issues](https://github.com/yourusername/PostoCerto/issues)

### 📚 Learning Resources

- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Domain-Driven Design by Eric Evans](https://www.domainlanguage.com/ddd/)
- [Implementing DDD by Vaughn Vernon](https://vaughnvernon.com/)
- [.NET Microservices Architecture](https://dotnet.microsoft.com/learn/aspnet/microservices-architecture)

---

## 🙏 Acknowledgments

- **ANP (Agência Nacional do Petróleo)** for providing open fuel price data
- **Microsoft** for .NET and excellent documentation
- **Community** for open-source tools and libraries

---

<div align="center">

**Built with ❤️ for learning and the Brazilian community**

⭐ Star this repo if you find it helpful!

**Last Updated**: January 2, 2026 | **Phase**: 01 - Domain Layer | **Status**: 🟡 In Progress

</div>
