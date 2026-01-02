using System.Data.Common;
using System.Runtime.InteropServices;
using FluentAssertions;
using Xunit;

namespace BuildingBlocks.Domain.Tests;

public class EntityTests
{
    // 🔴 RED - This test will FAIL because Entity doesn't exist yet
    [Fact]
    public void Entity_Should_Have_Id_When_Created()
    {
        // Arrange
        var id = Guid.NewGuid();
        
        // Act
        var entity = new FakeEntity(id);
        
        // Assert
        entity.Id.Should().Be(id);
    }

    [Fact]
    public void Two_Entities_With_Same_Id_Should_Be_Equal()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity1 = new FakeEntity(id);
        var entity2 = new FakeEntity(id);
        
        // Act & Assert
        entity1.Should().Be(entity2); // This test should fail until equality is implemented
    }

    [Fact] 
    public void Two_Entities_With_Different_Id_Should_Not_Be_Equal()
    {
        // Arrange
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var entity1 = new FakeEntity(id1);
        var entity2 = new FakeEntity(id2);
        
        // Act & Assert
        entity1.Should().NotBe(entity2); 
    }

    [Fact]
    public void Entity_Should_Be_Equal_To_Itself()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity = new FakeEntity(id);

        // Act & Assert
        entity.Should().Be(entity);
    }

    [Fact]
    public void Entity_Should_Have_CreatedAt_Timestamp()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity = new FakeEntity(id);
    
        // Act & Assert
        entity.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Entity_Should_Have_UpdatedAt_Timestamp(){
        // Arrange
        var id = Guid.NewGuid();
        var entity = new FakeEntity(id);

        // Act & Assert
        entity.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
    
    // Fake class to test the abstract Entity
    private class FakeEntity : Entity<Guid>
    {
        public FakeEntity(Guid id) : base(id) { }
    }

   
}