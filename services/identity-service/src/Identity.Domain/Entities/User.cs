using BuildingBlocks.Domain;

namespace Identity.Domain.Entities;

public class User : Entity<Guid>
{
    public string? FirstName {get ; private set ; }
    public string? LastName {get ; private set ;}
    public string? FullName => 
    (FirstName, LastName) switch
    {
        (not null, not null) => $"{FirstName} {LastName}",
        (not null, null) => FirstName,
        (null, not null) => LastName,
        _ => string.Empty
    };
    public Email Email { get; }
    private readonly PasswordHash _passwordHash;
    
    private User(
        Guid id,
        Email email,
        string? firstName,
        string? lastName,
        PasswordHash passwordHash
    ) : base(id)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        _passwordHash = passwordHash;
    }

    public static User Create(
        Email? email,
        string? firstName,
        string? lastName,
        PasswordHash passwordHash
    )
    {
        ArgumentNullException.ThrowIfNull(email);
        return new User(Guid.NewGuid(), email, firstName, lastName, passwordHash);
    }

    public static User Create(
        Guid id,
        Email email,
        string? firstName,
        string? lastName,
        PasswordHash passwordHash
    )
    {
        ArgumentNullException.ThrowIfNull(email);
        return new User(id, email, firstName, lastName, passwordHash);
    }

    public void UpdateFirstName(string? firstName)
    {
        FirstName = firstName;
        MarkAsModified();
    }

    public void UpdateLastName(string? lastName)
    {
        LastName = lastName;
        MarkAsModified();
    }
}