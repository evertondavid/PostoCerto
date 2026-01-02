# 🏗️ Building Blocks - Domain Foundation

## 📌 Overview

The **Building Blocks** library provides the foundational classes for all microservices in the PostoCerto project. It implements core Domain-Driven Design (DDD) patterns and Clean Architecture principles that are shared across all bounded contexts.

### 🎯 Purpose in the Project Architecture

Building Blocks serve as the **shared kernel** across all microservices:

- **Consistency**: Ensures all services (Identity, Stations, Prices, Recommendations) follow the same base patterns
- **DRY Principle**: Avoids code duplication across microservices
- **Type Safety**: Provides generic base classes with strong typing
- **Best Practices**: Implements DDD tactical patterns correctly from the start
- **Evolution**: Changes here propagate to all services, maintaining architectural consistency

### 📦 Components

| Class | Description | Used By |
|-------|-------------|---------|
| `Entity<TId>` | Base class for all domain entities | User, Station, PriceSurvey, UserPreference |
| `ValueObject` | Base class for immutable value objects | Email, Cnpj, Coordinates, Price, Password |
| `Result<T>` | Railway-oriented programming pattern | Application layer operations (Phase 02) |
| `DomainEvent` | Base class for domain events | UserRegistered, StationCreated, PriceUpdated |
| `DomainException` | Base exception for domain rules | InvalidEmail, WeakPassword, InvalidCnpj |

### 🏛️ Architectural Role

```
┌─────────────────────────────────────────┐
│         Building Blocks Library          │
│  (Shared across all microservices)      │
└─────────────────┬───────────────────────┘
                  │
    ┌─────────────┼─────────────┬─────────────┐
    │             │             │             │
┌───▼───┐   ┌────▼────┐   ┌────▼────┐   ┌───▼────┐
│Identity│   │Stations │   │ Prices  │   │ Recomm.│
│Service │   │Service  │   │ Service │   │ Service│
└────────┘   └─────────┘   └─────────┘   └────────┘
```

Each microservice **depends on** Building Blocks but **never on each other**.

---

## 🧪 Entity<TId> Test Scenarios

This section documents all test scenarios for the `Entity<TId>` base class, following **Test-Driven Development (TDD)** approach.

### 🎯 What is Entity<TId>?

`Entity<TId>` is the abstract base class for all domain entities in the system. In DDD, an entity is defined by its **identity** (ID), not by its attributes. Two entities with the same ID are considered the same entity, even if other properties differ.

---

### 📋 Test Scenarios

#### 1. **Entity Should Have Id When Created**
**Purpose**: Verify that an entity is correctly initialized with a unique identifier.

**Business Rule**: Every entity must have a unique ID that identifies it throughout its lifecycle.

**Why Important**: 
- IDs are the foundation of entity identity in DDD
- Without proper ID initialization, entity tracking becomes impossible
- Critical for database persistence and entity comparison

---

#### 2. **Two Entities With Same Id Should Be Equal**
**Purpose**: Verify that entities are compared by their ID, not by reference.

**Business Rule**: Two entity instances with the same ID represent the same domain object, regardless of being different objects in memory.

**Why Important**:
- Core principle of DDD: identity-based equality
- Enables proper behavior in collections (List, HashSet, Dictionary)
- Essential for domain logic like "is this the same user?"
- Required for repository pattern to work correctly

---

#### 3. **Two Entities With Different Ids Should Not Be Equal**
**Purpose**: Verify that entities with different IDs are not considered equal.

**Business Rule**: Different IDs mean different entities, even if all other attributes are identical.

**Why Important**:
- Prevents false positives in entity comparison
- Ensures data integrity in collections
- Critical for business logic that depends on entity uniqueness

---

#### 4. **Entity Should Have Proper HashCode Based On Id**
**Purpose**: Verify that GetHashCode returns a consistent hash based on the ID.

**Business Rule**: Entities with the same ID must return the same hash code.

**Why Important**:
- Required for HashSet and Dictionary to work correctly
- Enables O(1) lookup performance in hash-based collections
- Prevents duplicate entities in HashSet
- Essential for .NET equality contract (Equals + GetHashCode must be consistent)

---

#### 5. **Entity Should Have CreatedAt Timestamp**
**Purpose**: Verify that entities automatically track their creation time.

**Business Rule**: All entities must record when they were created.

**Why Important**:
- Audit trail requirement
- Business analytics (user registration dates, station creation timeline)
- Sorting entities chronologically
- Debugging and troubleshooting

---

#### 6. **Entity Should Have UpdatedAt Timestamp**
**Purpose**: Verify that entities track when they were last modified.

**Business Rule**: All entities must record their last modification time.

**Why Important**:
- Change tracking for audit purposes
- Optimistic concurrency control (Phase 03)
- Cache invalidation strategies
- Data synchronization between services

---

#### 7. **UpdatedAt Should Change When Entity Is Modified**
**Purpose**: Verify that calling `MarkAsModified()` updates the UpdatedAt timestamp.

**Business Rule**: Any modification to an entity must update its modification timestamp.

**Why Important**:
- Ensures audit trail accuracy
- Enables change detection mechanisms
- Required for event sourcing patterns
- Critical for distributed systems synchronization

---

#### 8. **CreatedAt Should Never Change After Creation**
**Purpose**: Verify that CreatedAt remains immutable throughout entity lifecycle.

**Business Rule**: Creation timestamp is immutable and should never be modified.

**Why Important**:
- Data integrity guarantee
- Historical accuracy for analytics
- Prevents data tampering
- Regulatory compliance (LGPD, GDPR)

---

#### 9. **Equality Operator (==) Should Work Correctly**
**Purpose**: Verify that the `==` operator correctly compares entities by ID.

**Business Rule**: `entity1 == entity2` should return true if they have the same ID.

**Why Important**:
- Idiomatic C# code: developers expect `==` to work
- Cleaner syntax in business logic
- Consistency with built-in types
- Reduces cognitive load for developers

---

#### 10. **Inequality Operator (!=) Should Work Correctly**
**Purpose**: Verify that the `!=` operator correctly identifies different entities.

**Business Rule**: `entity1 != entity2` should return true if they have different IDs.

**Why Important**:
- Complement to `==` operator
- Required for complete equality implementation
- Prevents subtle bugs in conditional logic
- Makes null checks more intuitive

---

#### 11. **Entity Should Handle Null Comparisons Gracefully**
**Purpose**: Verify that comparing an entity with null doesn't throw exceptions.

**Business Rule**: `entity.Equals(null)` should return false, not crash.

**Why Important**:
- Defensive programming practice
- Prevents NullReferenceException in production
- Required by .NET equality contract
- Common scenario in repository queries

---

#### 12. **Entity Should Not Equal Object Of Different Type**
**Purpose**: Verify that comparing an entity with a non-entity object returns false.

**Business Rule**: An entity cannot be equal to a string, number, or other type.

**Why Important**:
- Type safety enforcement
- Prevents runtime type errors
- Follows .NET equality best practices
- Essential for polymorphic scenarios

---

#### 13. **GetHashCode Should Be Consistent Across Calls**
**Purpose**: Verify that calling GetHashCode multiple times returns the same value.

**Business Rule**: Hash code must remain stable as long as the ID doesn't change.

**Why Important**:
- Required by .NET equality contract
- Hash-based collections would break otherwise
- Prevents "lost items" in Dictionary/HashSet
- Performance guarantee for lookups

---

#### 14. **Entities Can Be Used In HashSet Without Duplicates**
**Purpose**: Integration test to verify HashSet correctly identifies duplicate entities.

**Business Rule**: Adding the same entity twice (same ID) should result in only one entry.

**Why Important**:
- Real-world usage scenario
- Validates Equals + GetHashCode implementation together
- Critical for domain collections (unique users, unique stations)
- Prevents data duplication bugs

---

#### 15. **Entities Can Be Used As Dictionary Keys**
**Purpose**: Integration test to verify entities work correctly as dictionary keys.

**Business Rule**: Entities with the same ID should map to the same dictionary entry.

**Why Important**:
- Common pattern for caching (Dictionary<User, Data>)
- Lookup by entity instead of by ID
- Validates complete equality implementation
- Enables advanced domain patterns

---

## 🔄 TDD Workflow

All tests follow the **Red-Green-Refactor** cycle:

1. **🔴 RED**: Write a failing test
2. **🟢 GREEN**: Write minimal code to pass the test
3. **🔵 REFACTOR**: Improve code without breaking tests
4. **✅ VERIFY**: Run `dotnet test` to ensure all tests pass

---

## 🚀 Getting Started

### Prerequisites

- .NET SDK 8.0.404 (LTS)
- xUnit 2.9.2
- FluentAssertions 6.12.2

### Running Tests

```bash
cd libs/building-blocks
dotnet test
```

### Building the Project

```bash
dotnet build
```

---

## 📚 References

- [Domain-Driven Design by Eric Evans](https://www.domainlanguage.com/ddd/)
- [Implementing Domain-Driven Design by Vaughn Vernon](https://vaughnvernon.com/)
- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [.NET Equality Guidelines](https://learn.microsoft.com/en-us/dotnet/api/system.object.equals)

---

**Last Updated**: January 2, 2026  
**Status**: Phase 01 - Domain Layer + Tests (TDD)  
**Current Progress**: Entity equality implementation
