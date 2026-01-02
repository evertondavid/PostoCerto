# Web BFF - Backend For Frontend

## Overview

The **Web BFF** is a specialized backend service optimized for the React web application. It provides a tailored API that aggregates data from multiple microservices and shapes responses specifically for web browser consumption.

## Purpose

The BFF pattern solves the "one size fits all" API problem by:

- **Aggregating** multiple microservice calls into single endpoints
- **Optimizing** responses for desktop browser capabilities
- **Caching** frequently accessed data for better performance
- **Shaping** data structures to match web UI requirements
- **Reducing** network round-trips from client to server

## Architecture

```
React Web App
     ↓ REST/HTTPS
  Web BFF (Port 5000)
     ↓ gRPC
API Gateway → Microservices
```

## Key Features

### 🎯 Web-Specific Optimizations

- **Rich Data Payloads**: Full-featured responses leveraging desktop bandwidth
- **Server-Side Caching**: Redis integration for frequently accessed data
- **SEO Support**: Pre-rendered responses for search engine crawlers
- **Session Management**: Web-specific authentication flows
- **GraphQL Support** (Future): Flexible client-driven queries

### 🔄 Service Aggregation

Single endpoint calls that combine:
- User identity from Identity Service
- Station locations from Stations Service
- Price data from Prices Service
- Recommendations from Recommendations Service

### ⚡ Performance Features

- **Response Caching**: Redis-backed caching strategy
- **Parallel Requests**: Concurrent gRPC calls to microservices
- **Compression**: Gzip/Brotli response compression
- **CDN Integration**: Static asset optimization

## Technology Stack

| Component | Version | Purpose |
|-----------|---------|---------|
| .NET SDK | 8.0.404 | Runtime |
| ASP.NET Core | 8.0 | Web framework |
| gRPC.Net.Client | 2.66.0 | Service communication |
| Redis | 7.2+ | Response caching |
| Refit | 7.0+ | HTTP client generation |
| Polly | 8.0+ | Resilience & fault tolerance |

## Project Structure

```
web-bff/
├── src/
│   └── WebBff.Api/
│       ├── Controllers/          # REST API endpoints
│       │   ├── StationsController.cs
│       │   ├── PricesController.cs
│       │   └── RecommendationsController.cs
│       ├── Aggregators/          # Multi-service aggregation
│       │   ├── StationPriceAggregator.cs
│       │   └── UserDashboardAggregator.cs
│       ├── GrpcClients/          # gRPC client wrappers
│       │   ├── IdentityClient.cs
│       │   ├── StationsClient.cs
│       │   ├── PricesClient.cs
│       │   └── RecommendationsClient.cs
│       ├── Caching/              # Cache strategies
│       │   ├── CacheService.cs
│       │   └── CacheKeys.cs
│       ├── Models/               # DTOs & ViewModels
│       │   ├── Requests/
│       │   └── Responses/
│       ├── Middleware/           # HTTP pipeline
│       │   ├── AuthenticationMiddleware.cs
│       │   └── ExceptionMiddleware.cs
│       └── Program.cs            # Application entry point
└── tests/
    └── WebBff.Api.Tests/
        ├── Controllers/
        ├── Aggregators/
        └── Integration/
```

## API Endpoints (Future)

### Station & Price Search

```http
GET /api/stations/search?lat=-23.5505&lng=-46.6333&radius=5
```

**Response**: Aggregated station info with current prices

### User Dashboard

```http
GET /api/dashboard
Authorization: Bearer {token}
```

**Response**: User preferences + favorite stations + price alerts

### Price History

```http
GET /api/prices/history?stationId={id}&fuelType=gasoline&days=30
```

**Response**: Historical price trends with analytics

## Development Status

**Phase**: 🔴 Not Started (Planned for Q2 2026)

Current implementation is in **Phase 01: Domain Layer** only.

## Getting Started (Future)

Once implemented, you'll be able to run the Web BFF with:

```bash
cd bff/web-bff/src/WebBff.Api
dotnet run
```

The API will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`

## Configuration (Future)

**appsettings.json**:
```json
{
  "ServiceEndpoints": {
    "Identity": "https://localhost:7001",
    "Stations": "https://localhost:7002",
    "Prices": "https://localhost:7003",
    "Recommendations": "https://localhost:7004"
  },
  "Redis": {
    "ConnectionString": "localhost:6379",
    "DefaultExpiration": "00:05:00"
  }
}
```

## Caching Strategy (Future)

| Data Type | TTL | Invalidation |
|-----------|-----|--------------|
| Station Info | 1 hour | On station update |
| Current Prices | 5 minutes | On price update |
| Price History | 15 minutes | On new survey |
| User Preferences | 30 minutes | On user update |

## Related Documentation

- [Mobile BFF](../mobile-bff/README.md)
- [Architecture Overview](../../docs/architecture.md)
- [API Gateway Documentation](../../docs/api-gateway.md)

---

**Last Updated**: January 2, 2026
**Status**: Planning Phase
