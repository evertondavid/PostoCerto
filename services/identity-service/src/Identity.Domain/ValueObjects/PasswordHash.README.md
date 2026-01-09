# PasswordHash ValueObject - Test Plan

## 🎯 Objective

Validate the `PasswordHash` ValueObject following **TDD** principles. PasswordHash is a fundamental security component that encapsulates hashed password values throughout the Identity domain.

---

## 📐 Design Decisions

**Why ValueObject?**
- PasswordHash has intrinsic value and behavior (validation)
- No identity - two hashes with same value are identical
- Immutable once created
- Equality based on value, not reference
- Prevents passing plain text passwords where hashes are expected

**Validation Strategy:**
- Accept pre-hashed passwords only (hashing happens in Application Layer)
- Validate format (BCrypt, Argon2, etc.) if needed
- Never store or expose plain text passwords
- Encapsulates password security concerns in a single type

---

## 📋 Essential Tests for PasswordHash ValueObject

### ✅ Test #1: PasswordHash_Should_Be_Created_With_Valid_Hash
**Objective:** Validate that PasswordHash accepts valid hash strings.

**Tips:**
- Test cases: BCrypt hash (`$2a$11$...`), Argon2 hash, generic hash string
- Should not throw exception
- Value property should return the exact hash provided
- No normalization needed (hash is already in final form)

**Example:**
```csharp
[Fact]
public void PasswordHash_Should_Be_Created_With_Valid_Hash()
{
    // Arrange & Act
    var hash = "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy";
    var passwordHash = PasswordHash.Create(hash);

    // Assert
    passwordHash.Value.Should().Be(hash);
}
```

---

### ✅ Test #2: PasswordHash_Should_Throw_Exception_For_Empty_Hash
**Objective:** Ensure empty or whitespace-only hashes are rejected.

**Tips:**
- Test cases: `""`, `null`, `"   "` (whitespace)
- Should throw `ArgumentException`
- Message should be descriptive: "Password hash cannot be empty"
- Use FluentAssertions: `.Should().Throw<ArgumentException>()`

**Example:**
```csharp
[Fact]
public void PasswordHash_Should_Throw_Exception_For_Empty_Hash()
{
    // Arrange & Act
    Action act1 = () => PasswordHash.Create("");
    Action act2 = () => PasswordHash.Create("   ");

    // Assert
    act1.Should().Throw<ArgumentException>();
    act2.Should().Throw<ArgumentException>();
}
```

---

### ✅ Test #3: PasswordHash_Should_Throw_Exception_For_Null_Hash
**Objective:** Ensure null values are rejected explicitly.

**Tips:**
- Test case: `null`
- Should throw `ArgumentException` or `ArgumentNullException`
- Protects against null reference issues

**Example:**
```csharp
[Fact]
public void PasswordHash_Should_Throw_Exception_For_Null_Hash()
{
    // Arrange & Act
    Action act = () => PasswordHash.Create(null);

    // Assert
    act.Should().Throw<ArgumentException>();
}
```

---

### ✅ Test #4: PasswordHash_Equality_Should_Be_Based_On_Value
**Objective:** Validate ValueObject equality semantics.

**Tips:**
- Two PasswordHash instances with same value are equal
- Test: `Equals()`, `GetHashCode()`
- Different references but same value → equal
- Should follow ValueObject pattern (record equality)

**Example:**
```csharp
[Fact]
public void PasswordHash_Equality_Should_Be_Based_On_Value()
{
    // Arrange & Act
    var hash1 = PasswordHash.Create("$2a$11$hashedpassword");
    var hash2 = PasswordHash.Create("$2a$11$hashedpassword");

    // Assert
    hash1.Should().Be(hash2);
    hash1.GetHashCode().Should().Be(hash2.GetHashCode());
}
```

---

### ✅ Test #5: PasswordHash_Should_Be_Immutable
**Objective:** Ensure PasswordHash cannot be modified after creation.

**Tips:**
- Value property should be `{ get; }` only
- No public setters
- No methods that mutate state
- To "change" hash, create new instance
- Using `record` ensures immutability

**Example:**
```csharp
[Fact]
public void PasswordHash_Should_Be_Immutable()
{
    // Arrange & Act
    var hash = PasswordHash.Create("$2a$11$hashedpassword");
    var originalValue = hash.Value;

    // Assert
    hash.Value.Should().Be(originalValue);
    // PasswordHash.Value is get-only (immutable)
    // Attempting: hash.Value = "other"; // Would not compile
}
```

---

### ✅ Test #6: PasswordHash_Should_Not_Be_Equal_For_Different_Hashes
**Objective:** Validate that different hashes are not considered equal.

**Tips:**
- Create two PasswordHash instances with different values
- Should NOT be equal
- Ensures equality logic works both ways

**Example:**
```csharp
[Fact]
public void PasswordHash_Should_Not_Be_Equal_For_Different_Hashes()
{
    // Arrange & Act
    var hash1 = PasswordHash.Create("$2a$11$hash1");
    var hash2 = PasswordHash.Create("$2a$11$hash2");

    // Assert
    hash1.Should().NotBe(hash2);
}
```

---

### ✅ Test #7: PasswordHash_ToString_Should_Return_Value
**Objective:** Validate string conversion returns the hash value.

**Tips:**
- Implement `ToString()` override to return Value
- Useful for logging (but be careful not to log hashes in production!)
- Makes debugging easier

**Example:**
```csharp
[Fact]
public void PasswordHash_ToString_Should_Return_Value()
{
    // Arrange & Act
    var hashValue = "$2a$11$hashedpassword";
    var passwordHash = PasswordHash.Create(hashValue);
    var result = passwordHash.ToString();

    // Assert
    result.Should().Be(hashValue);
}
```

---

## 🧪 Implementation Guidelines

### Class Structure
```csharp
namespace Identity.Domain;

public sealed record PasswordHash
{
    public string Value { get; }

    private PasswordHash(string value) => Value = value;

    public static PasswordHash Create(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            throw new ArgumentException("Password hash cannot be empty");

        // Optional: Add format validation for specific hash types
        // ValidateBCryptFormat(hash);
        
        return new PasswordHash(hash);
    }

    public override string ToString() => Value;
}
```

### Optional: BCrypt Format Validation
```csharp
private static void ValidateBCryptFormat(string hash)
{
    // BCrypt format: $2a$10$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy
    // Pattern: $2[a/b/x/y]$[cost]$[salt][hash]
    if (!hash.StartsWith("$2"))
        throw new ArgumentException("Invalid BCrypt hash format");
        
    var parts = hash.Split('$');
    if (parts.Length != 4)
        throw new ArgumentException("Invalid BCrypt hash format");
}
```

---

## 🔮 Security Considerations

### ⚠️ Important: Domain Layer Responsibilities

The Domain Layer (PasswordHash ValueObject) should:
- ✅ Store hash values securely
- ✅ Validate hash format (structure)
- ✅ Ensure immutability
- ✅ Provide type safety

The Domain Layer should **NOT**:
- ❌ Hash plain text passwords (Application Layer responsibility)
- ❌ Verify passwords against hashes (Application Layer responsibility)
- ❌ Log hash values in production
- ❌ Expose plain text passwords (never exists in Domain)

---

## 🔐 Password Hashing (Application Layer)

Password hashing should be implemented in the **Application Layer** using:

### Recommended Libraries:
1. **BCrypt.Net-Next** (Most common)
   ```csharp
   var hash = BCrypt.Net.BCrypt.HashPassword("plainPassword");
   var isValid = BCrypt.Net.BCrypt.Verify("plainPassword", hash);
   ```

2. **Argon2** (Most secure, OWASP recommended)
   ```csharp
   // Using Konscious.Security.Cryptography.Argon2
   var hash = Argon2.Hash("plainPassword");
   var isValid = Argon2.Verify(hash, "plainPassword");
   ```

3. **PBKDF2** (Built-in .NET)
   ```csharp
   // Using Microsoft.AspNetCore.Cryptography.KeyDerivation
   var hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(...));
   ```

### Application Layer Structure:
```csharp
// Application/Services/IPasswordHasher.cs
public interface IPasswordHasher
{
    PasswordHash Hash(string plainPassword);
    bool Verify(string plainPassword, PasswordHash passwordHash);
}

// Infrastructure/Security/BCryptPasswordHasher.cs
public class BCryptPasswordHasher : IPasswordHasher
{
    public PasswordHash Hash(string plainPassword)
    {
        var hash = BCrypt.Net.BCrypt.HashPassword(plainPassword);
        return PasswordHash.Create(hash);
    }

    public bool Verify(string plainPassword, PasswordHash passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(plainPassword, passwordHash.Value);
    }
}
```

---

## 📚 References

- [OWASP Password Storage Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html)
- [BCrypt.Net-Next on GitHub](https://github.com/BcryptNet/bcrypt.net)
- [Argon2 - OWASP Recommended](https://github.com/P-H-C/phc-winner-argon2)
- [Martin Fowler - ValueObject](https://martinfowler.com/bliki/ValueObject.html)

---

## 🏃 TDD Workflow

1. 🔴 **Red**: Write Test #1, watch it fail
2. 🟢 **Green**: Create minimal PasswordHash class to pass Test #1
3. 🔵 **Refactor**: Improve implementation without breaking test
4. ♻️ **Repeat**: Move to Test #2, repeat cycle

Run tests: `dotnet test --filter "FullyQualifiedName~PasswordHash"`
