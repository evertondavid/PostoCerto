using FluentAssertions;

namespace BuildingBlocks.Domain.Tests;

public class EntityTests
{
    // #1
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

    // #2
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

    // #3
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

    // #4
    [Fact]
    public void Entity_Should_Have_Proper_HashCode_Based_On_Id(){

        // Arrange
        var id = Guid.NewGuid();
        var entity = new FakeEntity(id);

        // Act & Assert
        entity.GetHashCode().Should().Be(id.GetHashCode());

    }

    // #5
    [Fact]
    public void Entity_Should_Have_CreatedAt_Timestamp()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity = new FakeEntity(id);
    
        // Act & Assert
        entity.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    // #6
    [Fact]
    public void Entity_Should_Have_UpdatedAt_Timestamp(){
        // Arrange
        var id = Guid.NewGuid();
        var entity = new FakeEntity(id);

        // Act & Assert
        entity.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    // #7
    [Fact]
    public void UpdatedAt_Should_Change_When_Entity_Is_Modified(){

        // Arrange
        var id = Guid.NewGuid();
        var entity = new FakeEntity(id);

        // Act & Assert
        var originalUpdated = entity.UpdatedAt;
        System.Threading.Thread.Sleep(1000); // Ensure time difference
        entity.MarkAsModified();
        entity.UpdatedAt.Should().BeAfter(originalUpdated);

    }

    // #8
    [Fact]
    public void CreatedAt_Should_Never_Change_After_Creation()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity = new FakeEntity(id);
        var originalCreatedAt = entity.CreatedAt; 
        
        // Act
        Thread.Sleep(10); 
        entity.MarkAsModified(); 
        
        // Assert
        entity.CreatedAt.Should().Be(originalCreatedAt); 
    }

    // #9
    [Fact]
    public void Equality_Operator_EqualEqual_Should_Work_Correctly(){

        // Arrange
        var id = Guid.NewGuid();
        var entity1 = new FakeEntity(id);
        var entity2 = new FakeEntity(id);

        // Act
        var compareEntityID = entity1.Id == entity2.Id;

        // Assert
        compareEntityID.Should().Be(true);
    }

    // #10
    [Fact]
    public void Inequality_Operator_Not_Equal_ShouldWorkCorrectly(){

        //Arrange
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var entity1 = new FakeEntity(id1);
        var entity2 = new FakeEntity(id2);

        // Act
        var compareEntityID = entity1.Id != entity2.Id;

        // Assert
        compareEntityID.Should().Be(true);
    }
    
    // Fake class to test the abstract Entity
    private class FakeEntity : Entity<Guid>
    {
        public FakeEntity(Guid id) : base(id) { }
    }

   
}