# Email ValueObject - Test Plan

## 🎯 Objective

Validate the `Email` ValueObject following **TDD** principles. Email is a fundamental building block that ensures valid email addresses throughout the Identity domain.

---

## 📐 Design Decisions

**Why ValueObject?**
- Email has intrinsic value and behavior (validation)
- No identity - two emails with same value are identical
- Immutable once created
- Equality based on value, not reference

**Validation Strategy:**
- Use .NET's native `MailAddress` class (RFC 5322 compliant)
- No regex - MailAddress is battle-tested and covers edge cases
- Normalizes email automatically (trims, lowercase)

---

## 📋 Essential Tests for Email ValueObject

### ✅ Test #1: Email_Should_Be_Created_With_Valid_Format
**Objective:** Validate that Email accepts standard valid email formats.

**Tips:**
- Test cases: `user@example.com`, `john.doe@company.co.uk`, `user+tag@domain.org`
- Use `MailAddress` constructor to parse
- Should not throw exception
- Value property should return the normalized email

---

### ✅ Test #2: Email_Should_Throw_Exception_For_Invalid_Format
**Objective:** Ensure invalid formats are rejected.

**Tips:**
- Test cases: `invalid`, `@domain.com`, `user@`, `user @domain.com`, empty string
- Should throw `ArgumentException` or custom `InvalidEmailException`
- Use `FluentAssertions`: `.Should().Throw<ArgumentException>()`
- Message should be descriptive

---

### ✅ Test #3: Email_Should_Be_Case_Insensitive
**Objective:** Validate that email comparison ignores case.

**Tips:**
- Create `user@EXAMPLE.com` and `USER@example.com`
- Should be considered equal (Equals(), ==, GetHashCode())
- Normalize to lowercase internally
- RFC 5321: local part is technically case-sensitive, but practically treated as case-insensitive

---

### ✅ Test #4: Email_Should_Trim_Whitespace
**Objective:** Ensure leading/trailing spaces are removed.

**Tips:**
- Input: `"  user@example.com  "`
- Output: `"user@example.com"`
- Trim before validation
- Spaces inside should still be invalid

---

### ✅ Test #5: Email_Equality_Should_Be_Based_On_Value
**Objective:** Validate ValueObject equality semantics.

**Tips:**
- Two Email instances with same value are equal
- Test: Equals(), operator==, operator!=, GetHashCode()
- Different references but same value → equal
- Should follow ValueObject pattern from Building Blocks (when implemented)

---

### ✅ Test #6: Email_Should_Be_Immutable
**Objective:** Ensure Email cannot be modified after creation.

**Tips:**
- Value property should be `{ get; private set; }` or `{ get; init; }`
- No public setters
- No methods that mutate state
- To "change" email, create new instance

---

### ✅ Test #7: Email_Should_Convert_To_String_Implicitly
**Objective:** Validate convenient string conversion.

**Tips:**
- Implement `ToString()` override to return Value
- Optional: Implicit conversion operator `public static implicit operator string(Email email)`
- Makes usage easier: `string emailStr = emailObject;`
- Decide if you want implicit or explicit conversion

---

## 🧪 Implementation Guidelines

### Class Structure
```csharp
public class Email : ValueObject // Inherit from BuildingBlocks when ready
{
    public string Value { get; private set; }
    
    private Email(string value) 
    { 
        // Private constructor - use factory method
    }
    
    public static Email Create(string email)
    {
        // Validate using MailAddress
        // Normalize (trim, lowercase)
        // Return new Email(normalized)
    }
    
    // Equality members (if not using ValueObject base)
    // ToString() override
}
```

### Validation Logic
```csharp
public static Email Create(string email)
{
    if (string.IsNullOrWhiteSpace(email))
        throw new ArgumentException("Email cannot be empty");
        
    var normalized = email.Trim().ToLowerInvariant();
    
    try
    {
        var mailAddress = new MailAddress(normalized);
        // Ensure normalization didn't change format
        if (mailAddress.Address != normalized)
            throw new ArgumentException("Invalid email format");
    }
    catch (FormatException ex)
    {
        throw new ArgumentException($"Invalid email format: {email}", ex);
    }
    
    return new Email(normalized);
}
```

---

## 🔮 Future Enhancements (Post-MVP)

These validations should be implemented in the **Application Layer** (Use Cases), not in the Domain ValueObject:

### 1. DNS MX Record Validation
- Verify that email domain has valid MX records
- Ensures domain can receive emails
- Requires async DNS lookup
- Library: `DnsClient.NET`

### 2. Disposable Email Blacklist
- Block temporary email services (temp-mail.org, guerrillamail.com, etc)
- Maintain curated blacklist or use service like `mailcheck.ai`
- Prevents spam signups

### 3. Corporate Domain Whitelist
- For B2B scenarios: only allow company domains
- Example: Only `@company.com` allowed
- Configurable per tenant

### 4. Email Deliverability Check
- Integration with services like SendGrid, Mailgun
- Verify email exists without sending
- Prevents typos: `user@gmai.com` → suggest `gmail.com`

**Why Application Layer?**
- Domain should only validate structure (format)
- Infrastructure concerns (DNS, external APIs) belong in Application/Infrastructure
- Keeps Domain pure and testable

---

## 📚 References

- [RFC 5322 - Internet Message Format](https://datatracker.ietf.org/doc/html/rfc5322)
- [MailAddress Class - Microsoft Docs](https://learn.microsoft.com/en-us/dotnet/api/system.net.mail.mailaddress)
- [Martin Fowler - ValueObject](https://martinfowler.com/bliki/ValueObject.html)
- [DDD - Value Objects](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/implement-value-objects)

---

## 🏃 TDD Workflow

1. 🔴 **Red**: Write Test #1, watch it fail (Email class doesn't exist)
2. 🟢 **Green**: Create minimal Email class to pass Test #1
3. 🔵 **Refactor**: Improve implementation without breaking test
4. ♻️ **Repeat**: Move to Test #2, repeat cycle

Run tests: `dotnet test --filter "FullyQualifiedName~Email"`
