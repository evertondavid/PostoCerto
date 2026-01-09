# User Entity - Test Plan (MVP)

## 🎯 Objective

Implement the `User` entity following **TDD** with focus on MVP essential features. User is an **Entity** (not ValueObject) - identified by unique Id, mutable over time.

---

## 📐 Design Decisions

**Why Entity (not ValueObject)?**
- User has unique identity (Id: Guid)
- Mutable over time (can update FirstName, LastName, etc)
- Lifecycle managed by aggregate root (User itself)
- Inherits from `Entity<Guid>` (BuildingBlocks)

**Aggregate Structure:**
```
User (Aggregate Root)
├── Id: Guid (from Entity<TId>)
├── Email: Email ValueObject ✅
├── PasswordHash: PasswordHash ValueObject (stub for now)
├── FirstName: string? (optional)
├── LastName: string? (optional)
├── FullName: string (computed property)
├── Addresses: IReadOnlyCollection<Address> (stub for now)
├── CreatedAt: DateTime (from Entity<TId>)
└── UpdatedAt: DateTime (from Entity<TId>)
```

---

## 📋 5 Essential Tests for MVP

### ✅ Test #1: User_Should_Be_Created_With_Valid_Mandatory_Data
**Objective:** Validate User creation with minimum required fields.

**Tips:**
- Mandatory: Id (Guid), Email (ValueObject), PasswordHash (stub: just string for now)
- Optional: FirstName, LastName (can be null)
- User inherits from `Entity<Guid>` - test CreatedAt, UpdatedAt are set
- Addresses collection should be empty initially
- Use factory method: `User.Create()` or constructor

**Stub for PasswordHash:**
```csharp
// For now, just use string. Replace later with PasswordHash ValueObject
private User(Guid id, Email email, string passwordHash, ...)
```

---

### ✅ Test #2: User_Should_Not_Be_Created_With_Null_Email
**Objective:** Ensure Email is mandatory.

**Tips:**
- Pass `null` for email parameter
- Should throw `ArgumentNullException`
- Message should indicate "email" parameter
- Guards clause: `ArgumentNullException.ThrowIfNull(email);` (C# 11+)

---

### ✅ Test #3: User_FullName_Should_Combine_FirstName_And_LastName
**Objective:** Validate computed property `FullName`.

**Tips:**
- Scenario 1: FirstName="John", LastName="Doe" → "John Doe"
- Scenario 2: FirstName="John", LastName=null → "John"
- Scenario 3: FirstName=null, LastName="Doe" → "Doe"
- Scenario 4: Both null → string.Empty or null
- Implementation:
```csharp
public string FullName => 
    (FirstName, LastName) switch
    {
        (not null, not null) => $"{FirstName} {LastName}",
        (not null, null) => FirstName,
        (null, not null) => LastName,
        _ => string.Empty
    };
```

---

### ✅ Test #4: User_Should_Update_FirstName_And_MarkAsModified
**Objective:** Validate property mutation updates timestamp.

**Tips:**
- Create User with FirstName = "John"
- Call `UpdateFirstName("Jane")`
- Verify FirstName changed to "Jane"
- Verify `UpdatedAt > CreatedAt` (inherited from Entity<TId>)
- Method should call `MarkAsModified()` internally

**Implementation pattern:**
```csharp
public void UpdateFirstName(string? firstName)
{
    FirstName = firstName;
    MarkAsModified(); // From Entity<TId>
}
```

---

### ✅ Test #5: User_Equality_Should_Be_Based_On_Id
**Objective:** Validate entity equality (inherited behavior).

**Tips:**
- Two Users with same Id are equal (even if Email is different)
- Two Users with different Id are NOT equal (even if all properties are same)
- Test: `Equals()`, `GetHashCode()`, `operator==`, `operator!=`
- Behavior already implemented in `Entity<Guid>` - just validate inheritance works
- Create two Users with same Guid, verify equality

---

## 🚧 Stubs for Later Implementation

### PasswordHash Stub (for now)
```csharp
// User.cs
private readonly string _passwordHash; // Temporary - will be PasswordHash ValueObject

private User(Guid id, Email email, string passwordHash, ...)
{
    ArgumentNullException.ThrowIfNull(email);
    ArgumentException.ThrowIfNullOrWhiteSpace(passwordHash);
    
    Id = id;
    Email = email;
    _passwordHash = passwordHash;
    // ...
}
```

### Address Stub (for now)
```csharp
// User.cs
private readonly List<object> _addresses = new(); // Temporary - will be List<Address>

public IReadOnlyCollection<object> Addresses => _addresses.AsReadOnly();

// Methods AddAddress, RemoveAddress - implement later
```

---

## 🧪 Implementation Steps (TDD Cycle)

1. **Test #1 - Create User with valid data**
   - 🔴 Write test → fails (User class doesn't exist)
   - 🟢 Create User class inheriting Entity<Guid>
   - 🟢 Add properties: Email, _passwordHash, FirstName, LastName
   - 🟢 Add constructor with validation
   - ✅ Test passes

2. **Test #2 - Null email validation**
   - 🔴 Write test → fails (no null check)
   - 🟢 Add `ArgumentNullException.ThrowIfNull(email)` in constructor
   - ✅ Test passes

3. **Test #3 - FullName computed property**
   - 🔴 Write test → fails (FullName not implemented)
   - 🟢 Implement FullName property with switch expression
   - ✅ Test passes

4. **Test #4 - Update FirstName**
   - 🔴 Write test → fails (UpdateFirstName doesn't exist)
   - 🟢 Create UpdateFirstName method calling MarkAsModified()
   - ✅ Test passes

5. **Test #5 - Equality by Id**
   - 🔴 Write test → should pass already (inherited from Entity<Guid>)
   - ✅ Validates inheritance works correctly

---

## 📚 References

- [Martin Fowler - Aggregate Pattern](https://martinfowler.com/bliki/DDD_Aggregate.html)
- [Microsoft - Entities in DDD](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/microservice-domain-model#implement-entities)
- [Entity<TId> Implementation](../../../libs/building-blocks/src/BuildingBlocks.Domain/Entity.cs)

---

## 🎯 Next Steps After MVP

1. Replace `string _passwordHash` with `PasswordHash` ValueObject
2. Replace `List<object> _addresses` with `List<Address>` ValueObject
3. Implement remaining 5 tests from [full test plan](../../../tests/Identity.Domain.Tests/README.md)
4. Add domain events: `UserCreated`, `UserUpdated`

---

## 🏃 Quick Commands

```bash
# Run all User tests
dotnet test --filter "FullyQualifiedName~User"

# Run specific test
dotnet test --filter "User_Should_Be_Created_With_Valid_Mandatory_Data"

# Watch mode (re-run on file change)
dotnet watch test
```
