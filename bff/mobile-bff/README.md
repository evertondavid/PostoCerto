# Mobile BFF - Backend For Frontend

## Overview

The **Mobile BFF** is a specialized backend service optimized for the Flutter mobile application (iOS & Android). It provides a tailored API with lightweight payloads, offline-first synchronization, and battery-efficient data fetching strategies.

## Purpose

The Mobile BFF is specifically designed for mobile constraints:

- **Lightweight Payloads**: Minimal data transfer for cellular networks
- **Offline Support**: Sync strategies for intermittent connectivity
- **Battery Efficiency**: Reduced polling and smart caching
- **Push Notifications**: Mobile-specific notification integration
- **Bandwidth Optimization**: Compressed responses and pagination

## Architecture

```
Flutter Mobile App
     ↓ REST/HTTPS
Mobile BFF (Port 5001)
     ↓ gRPC
API Gateway → Microservices
```

## Key Features

### 📱 Mobile-Specific Optimizations

- **Minimal Payloads**: Only essential data to reduce bandwidth
- **Pagination**: Limit-offset pagination for lists
- **Image Optimization**: Responsive image URLs (thumbnail, medium, full)
- **Incremental Sync**: Delta updates for offline-first apps
- **Network Detection**: Adaptive strategies based on connection quality

### 🔄 Offline-First Support

- **Data Synchronization**: Queue-based sync for offline actions
- **Conflict Resolution**: Last-write-wins with server authority
- **Local Cache**: Client-side SQLite sync metadata
- **Background Sync**: Service worker for background updates

### ⚡ Performance Features

- **Response Compression**: Gzip for reduced payload size
- **Field Filtering**: GraphQL-like field selection
- **Connection Pooling**: Persistent gRPC connections
- **Request Batching**: Multiple queries in single roundtrip

### 🔔 Push Notifications

- **Firebase Cloud Messaging** (FCM) integration
- **APNS** (Apple Push Notification Service) support
- **Price Alerts**: Notify when fuel prices drop
- **Favorite Stations**: Updates for saved stations

## Technology Stack

| Component | Version | Purpose |
|-----------|---------|---------|
| .NET SDK | 8.0.404 | Runtime |
| ASP.NET Core | 8.0 | Web framework |
| gRPC.Net.Client | 2.66.0 | Service communication |
| Redis | 7.2+ | Session & caching |
| SignalR | 8.0 | Real-time updates |
| FirebaseAdmin | 3.0+ | Push notifications |

## Project Structure

```
mobile-bff/
├── src/
│   └── MobileBff.Api/
│       ├── Controllers/          # REST API endpoints
│       │   ├── StationsController.cs
│       │   ├── PricesController.cs
│       │   └── SyncController.cs
│       ├── Aggregators/          # Optimized aggregation
│       │   ├── MobileStationAggregator.cs
│       │   └── MobileDashboardAggregator.cs
│       ├── GrpcClients/          # gRPC client wrappers
│       │   ├── IdentityClient.cs
│       │   ├── StationsClient.cs
│       │   ├── PricesClient.cs
│       │   └── RecommendationsClient.cs
│       ├── Sync/                 # Offline sync logic
│       │   ├── SyncEngine.cs
│       │   ├── ConflictResolver.cs
│       │   └── DeltaCalculator.cs
│       ├── Notifications/        # Push notifications
│       │   ├── FcmService.cs
│       │   └── ApnsService.cs
│       ├── Models/               # Lightweight DTOs
│       │   ├── Requests/
│       │   └── Responses/
│       ├── Middleware/           # HTTP pipeline
│       │   ├── MobileAuthMiddleware.cs
│       │   └── CompressionMiddleware.cs
│       └── Program.cs            # Application entry point
└── tests/
    └── MobileBff.Api.Tests/
        ├── Controllers/
        ├── Sync/
        └── Integration/
```

## API Endpoints (Future)

### Nearby Stations (Mobile-Optimized)

```http
GET /api/stations/nearby?lat=-23.5505&lng=-46.6333&radius=5&limit=10
```

**Response**: Minimal station data with thumbnails

```json
{
  "stations": [
    {
      "id": "uuid",
      "name": "Station Name",
      "distance": 1.2,
      "thumbnail": "https://cdn.example.com/thumbnails/station.jpg",
      "lowestPrice": {
        "fuelType": "gasoline",
        "price": 5.49
      }
    }
  ],
  "hasMore": true,
  "nextCursor": "cursor_token"
}
```

### Incremental Sync

```http
POST /api/sync
Authorization: Bearer {token}
Content-Type: application/json

{
  "lastSyncTimestamp": "2026-01-02T10:00:00Z",
  "clientChanges": [
    { "type": "favorite_added", "stationId": "uuid" }
  ]
}
```

**Response**: Server changes since last sync

### Push Notification Registration

```http
POST /api/notifications/register
Authorization: Bearer {token}
Content-Type: application/json

{
  "deviceToken": "fcm_token_here",
  "platform": "android",
  "preferences": {
    "priceAlerts": true,
    "favoriteUpdates": true
  }
}
```

## Development Status

**Phase**: 🔴 Not Started (Planned for Q2 2026)

Current implementation is in **Phase 01: Domain Layer** only.

## Getting Started (Future)

Once implemented, you'll be able to run the Mobile BFF with:

```bash
cd bff/mobile-bff/src/MobileBff.Api
dotnet run
```

The API will be available at:
- HTTP: `http://localhost:5001`
- HTTPS: `https://localhost:5002`

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
    "DefaultExpiration": "00:10:00"
  },
  "Firebase": {
    "ProjectId": "postocerto",
    "CredentialsPath": "./firebase-adminsdk.json"
  },
  "MobileOptimization": {
    "MaxPayloadSize": 50000,
    "ImageThumbnailSize": 200,
    "PaginationLimit": 10
  }
}
```

## Offline Sync Strategy (Future)

| Data Type | Sync Frequency | Conflict Resolution |
|-----------|----------------|---------------------|
| Favorites | On change | Last write wins |
| Search History | On change | Merge |
| Price Alerts | On change | Server authority |
| User Profile | On change | Server authority |

## Battery Optimization (Future)

- **Adaptive Polling**: Reduce frequency when battery < 20%
- **Background Sync**: Use system background tasks
- **Data Compression**: Reduce network usage
- **Location Services**: Optimize GPS usage for nearby search

## Related Documentation

- [Web BFF](../web-bff/README.md)
- [Flutter Mobile App](../../frontend/mobile/README.md)
- [Architecture Overview](../../docs/architecture.md)

---

**Last Updated**: January 2, 2026
**Status**: Planning Phase
