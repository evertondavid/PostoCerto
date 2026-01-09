using FluentAssertions;
using Identity.Domain.Entities; 

namespace Identity.Domain.Tests;

public class UserTests
{
  // #1
  [Fact]
  public void User_Should_Be_Created_With_Valid_Mandatory_Data()
  {
      // Arrange
      var email = Email.Create("john.doe@example.com"); 
      var firstName = "John";
      var lastName = "Doe";
      var passwordHash = PasswordHash.Create("hashed_password_here");

      // Act
      var user = User.Create(email, firstName, lastName, passwordHash);

      // Assert
      user.Id.Should().NotBeEmpty();
      user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
      user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
      user.FirstName.Should().Be(firstName);
      user.LastName.Should().Be(lastName);
      user.Email.Should().Be(email);
      user.FullName.Should().Be("John Doe");
  }

  // #2
  [Fact]
  public void User_Should_Not_Be_Created_With_Null_Email()
  {

     // Arrange
      var firstName = "John";
      var lastName = "Doe";
      var passwordHash = PasswordHash.Create("hashed_password_here");

      // Act
      Action user = () => User.Create(null, firstName, lastName, passwordHash);

      // Assert
      user.Should().Throw<ArgumentNullException>()
      .WithParameterName("email");
  }

  // #3
  [Fact]
  public void User_FullName_Should_Combine_FirstName_And_LastName()
  {
     // Arrange
      var email = Email.Create("john.doe@example.com"); 
      var firstName = "John";
      var lastName = "Doe";
      var passwordHash = PasswordHash.Create("hashed_password_here");

      // Act
      var user1 = User.Create(email, firstName, lastName, passwordHash);
      var user2 = User.Create(email, firstName, null, passwordHash);
      var user3 = User.Create(email, null, lastName, passwordHash);
      var user4 = User.Create(email, null, null, passwordHash);

      // Assert
      user1.FullName.Should().Be("John Doe");
      user2.FullName.Should().Be("John");
      user3.FullName.Should().Be("Doe");
      user4.FullName.Should().Be(string.Empty);
  }

  // #4
  [Fact]
  public void User_Should_Update_FirstName_And_MarkAsModified()
  {
     // Arrange
      var email = Email.Create("john.doe@example.com"); 
      var firstName = "John";
      var lastName = "Doe";
      var passwordHash = PasswordHash.Create("hashed_password_here");

      // Act
      var user = User.Create(email, firstName, lastName, passwordHash);
      var originalUpdatedAt = user.UpdatedAt;
      System.Threading.Thread.Sleep(1000);
      user.UpdateFirstName("Patrick");
      
      // Assert
      user.FirstName.Should().NotBe("John");
      user.UpdatedAt.Should().BeAfter(originalUpdatedAt);
  }

  // #5
  [Fact]
  public void User_Equality_Should_Be_Based_On_Id()
  {

    var email1 = Email.Create("john.doe@example.com"); 
    var email2 = Email.Create("patrick.malone@domain.co");
    var firstName = "John";
    var lastName = "Doe";
    var passwordHash = PasswordHash.Create("hashed_password_here");

    // Act
    var user1 = User.Create(email1, firstName, lastName, passwordHash);
    var user2 = User.Create(user1.Id, email2, firstName, lastName, passwordHash);

    // Assert
    user1.Id.Should().Be(user2.Id);
    user1.Should().Be(user2);
  }
}