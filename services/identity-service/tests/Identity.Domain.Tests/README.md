# Identity.Domain.Tests - Test Plan

## 🎯 Objective

Validate the behavior of the `User` entity and its value objects (`Email`, `PasswordHash`, `Address`) following **TDD** (Test-Driven Development) with focus on the MVP.

---

## 📋 Essential Tests for User Entity

### ✅ Test #1: User_Should_Be_Created_With_Valid_Mandatory_Data
**Objective:** Validate that a User can be created with minimum mandatory data.

**Tips:**
- Mandatory data: `Id` (Guid), `Email`, `PasswordHash`
- User inherits from `Entity<Guid>`, so test `Id`, `CreatedAt`, `UpdatedAt`
- Email and PasswordHash must be ValueObjects (create stubs for the test)
- FirstName/LastName are optional, can be null
- Addresses collection should be empty initially

---

### ✅ Test #2: User_Should_Not_Be_Created_With_Null_Email
**Objective:** Ensure that Email is mandatory.

**Tips:**
- Passing `null` in constructor should throw `ArgumentNullException`
- Message should indicate "email" as invalid parameter
- Use `FluentAssertions`: `.Should().Throw<ArgumentNullException>()`

---

### ✅ Test #3: User_Should_Not_Be_Created_With_Null_PasswordHash
**Objective:** Ensure that PasswordHash is mandatory.

**Tips:**
- Passing `null` in constructor should throw `ArgumentNullException`
- Message should indicate "passwordHash" as invalid parameter
- Similar to Test #2, but for password

---

### ✅ Test #4: User_FullName_Should_Combine_FirstName_And_LastName
**Objective:** Validate computed property `FullName`.

**Tips:**
- Scenario 1: FirstName + LastName → "John Doe"
- Scenario 2: Only FirstName → "John"
- Scenario 3: Only LastName → "Doe"
- Scenario 4: Both null → empty string or null
- FullName is NOT persisted, it's calculated (get-only property)

---

### ✅ Test #5: User_Should_Add_Address_To_Collection
**Objective:** Validate adding an address to the Addresses collection.

**Tips:**
- User.Addresses should be a collection (IReadOnlyCollection or List)
- Method `AddAddress(Address address)` adds to collection
- Collection should have Count = 1 after adding
- Address is ValueObject (Street, City, State, Type: "Home"/"Work")

---

### ✅ Test #6: User_Should_Remove_Address_From_Collection
**Objective:** Validate removing an address from the collection.

**Tips:**
- Add Address, then remove it
- Method `RemoveAddress(Address address)` removes from collection
- Collection should return to Count = 0
- Removing non-existent Address should not throw exception (idempotent)

---

### ✅ Test #7: User_Should_Not_Add_Duplicate_Address
**Objective:** Prevent duplicate addresses (ValueObject equality).

**Tips:**
- Add same Address twice
- Collection should have Count = 1 (ignore duplicate)
- ValueObject compares by value, not by reference
- Or throw exception when trying to add duplicate (you decide)

---

### ✅ Test #8: User_Should_Update_FirstName_And_MarkAsModified
**Objective:** Validate property change and timestamp update.

**Tips:**
- Create User with FirstName = "John"
- Method `UpdateFirstName(string firstName)` changes property
- `MarkAsModified()` should be called internally
- `UpdatedAt` should be greater than initial value
- Test inherited behavior from Entity<TId>

---

### ✅ Test #9: User_Equality_Should_Be_Based_On_Id
**Objective:** Validate entity equality (inherited from Entity<TId>).

**Tips:**
- Two Users with same Id are equal (even if Email is different)
- Two Users with different Id are NOT equal
- Test Equals(), GetHashCode(), operator==, operator!=
- Behavior already implemented in Entity<TId>, but validates inheritance

---

### ✅ Test #10: User_Should_Allow_Multiple_Addresses_With_Different_Types
**Objective:** Validate multiple addresses (Home, Work, etc).

**Tips:**
- Add Address type "Home"
- Add Address type "Work"
- Collection should have Count = 2
- Allow same address with different type (you decide)

---

## 🧪 Next Steps

1. **Create ValueObjects first:**
   - `Email.cs` (format validation)
   - `PasswordHash.cs` (BCrypt hashing)
   - `Address.cs` (Street, City, State, Type)

2. **Implement User.cs:**
   - Inherit from `Entity<Guid>`
   - Properties: Email, PasswordHash, FirstName, LastName
   - Collection: private List<Address> + IReadOnlyCollection
   - Methods: AddAddress, RemoveAddress, UpdateFirstName/LastName

3. **Run tests:** `dotnet test`

4. **TDD Cycle:**
   - 🔴 Red: Write failing test
   - 🟢 Green: Implement minimum code to pass
   - 🔵 Refactor: Improve code keeping tests green

---

## 📚 References

- [Martin Fowler - ValueObject](https://martinfowler.com/bliki/ValueObject.html)
- [Microsoft - Aggregate Pattern](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/microservice-domain-model#the-aggregate-pattern)
- [xUnit Documentation](https://xunit.net/)
- [FluentAssertions](https://fluentassertions.com/)
- [BCrypt.Net-Next](https://github.com/BcryptNet/bcrypt.net)
