using System.Net.Mail;

namespace Identity.Domain;

public sealed record Email
{
    public string Value { get;}

    private Email(string value) => Value = value;

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty");
            
        var normalized = email.Trim().ToLowerInvariant();
        
        try
        {
            var mailAddress = new MailAddress(normalized);
            if (mailAddress.Address != normalized)
                throw new ArgumentException($"Invalid email format: {normalized}");
        }
        catch (FormatException ex)
        {
            throw new ArgumentException($"Invalid email format: {email}", ex);
        }
        
        return new Email(normalized);
    }

  public override string ToString() => Value;
}