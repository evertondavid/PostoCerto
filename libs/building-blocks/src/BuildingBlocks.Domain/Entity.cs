namespace BuildingBlocks.Domain;

public abstract class Entity<TId> where TId : notnull
{
    public TId Id { get; protected set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    protected Entity(TId id)
    {
        Id = id;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
    
    // Protected constructor for EF Core
    protected Entity() { 
      Id = default!; // Default value for TId
    }

    public Entity<TId> MarkAsModified()
    {
        UpdatedAt = DateTime.UtcNow;
        return this;
    }


     public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> other)
            return false;
       
       if(ReferenceEquals(this, other))
            return true;
      
        return Id.Equals(other.Id);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}