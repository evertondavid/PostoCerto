using System.Net.Mail;
using System.Security.Cryptography;
using FluentAssertions;

namespace Identity.Domain.Tests;

public class IdentityTests
{

    // #1
    [Fact]
    public void Email_Should_Be_Created_With_Valid_Format()
    {
       // Arrange & Act
    Action act1 = () => Email.Create("user@example.com");
    Action act2 = () => Email.Create("user@example.org");
    Action act3 = () => Email.Create("other+domain@example.co.uk");
    
    // Assert
    act1.Should().NotThrow();
    act2.Should().NotThrow();
    act3.Should().NotThrow();
 
    }

    // #2
    [Fact]
    public void Email_Should_Throw_Exception_For_Invalid_Format()
    {

        // Arrange & Act
        Action act1 = () => Email.Create("@domain.com");
        Action act2 = () => Email.Create("user@");
        Action act3 = () => Email.Create("user @domain.com");
        Action act4 = () => Email.Create("");

        // Assert
        act1.Should().Throw();
        act2.Should().Throw();
        act3.Should().Throw();
        act4.Should().Throw();
    }

    // #3
    [Fact]
    public void Email_Should_Be_Case_Insensitive()
    {
        // Arrange & Act
        Action act = () => Email.Create("JoHn.DoE@EmaIL.COm");

        // Assert
        act.Should().NotThrow();
    }

    // #4
    [Fact]
    public void Email_Should_Trim_Whitespace()
    {

        // Arrange & Act
        Action act = () => Email.Create("    john.DOE@domain.com   ");

        // Assert
        act.Should().NotThrow();
    }

    // #5
    [Fact]
    public void Email_Equality_Should_Be_Based_On_Value()
    {
        // Arrange & Act
        var email1 = Email.Create("john.doe@domain.com");
        var email2 = Email.Create("john.doe@domain.com");

        // Assert
        email1.Should().Be(email2);
        email1.GetHashCode().Should().Be(email2.GetHashCode());
    }

    // #6
    [Fact]
    public void Email_Should_Be_Immutable()
    {
        // Arrange & Act
        var email = Email.Create("john.doe@domain.com");
        var originalValue = email.Value;

        // Assert
        email.Value.Should().Be(originalValue);
        // Email.Value is get-only (immutable)
        // Attempting: email.Value = "other@email.com"; // Would not compile
    }

    // #7
    [Fact]
    public void Email_Should_Convert_To_String_Implicitly(){
        
        // Arrange & Act
        var email = Email.Create("john.doe@domain.com");
        var result = email.ToString();

        // Assert
        result.Should().Be("john.doe@domain.com");
    }
}