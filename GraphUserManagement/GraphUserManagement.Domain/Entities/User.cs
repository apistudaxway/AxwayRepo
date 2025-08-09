using GraphUserManagement.Domain.ValueObjects;

namespace GraphUserManagement.Domain.Entities;

public class User
{
    public string Id { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public Email Email { get; private set; } = default!;
    public string GivenName { get; private set; } = string.Empty;
    public string Surname { get; private set; } = string.Empty;
    public string UserPrincipalName { get; private set; } = string.Empty;
    public string? JobTitle { get; private set; }
    public string? Department { get; private set; }
    public string? MobilePhone { get; private set; }
    public bool AccountEnabled { get; private set; } = true;
    public DateTime CreatedDateTime { get; private set; }
    public DateTime? LastModifiedDateTime { get; private set; }

    // Private constructor for EF Core
    private User() { }

    public User(
        string displayName,
        Email email,
        string givenName,
        string surname,
        string userPrincipalName,
        string? jobTitle = null,
        string? department = null,
        string? mobilePhone = null,
        bool accountEnabled = true)
    {
        DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        GivenName = givenName ?? throw new ArgumentNullException(nameof(givenName));
        Surname = surname ?? throw new ArgumentNullException(nameof(surname));
        UserPrincipalName = userPrincipalName ?? throw new ArgumentNullException(nameof(userPrincipalName));
        JobTitle = jobTitle;
        Department = department;
        MobilePhone = mobilePhone;
        AccountEnabled = accountEnabled;
        CreatedDateTime = DateTime.UtcNow;
    }

    public void UpdateProfile(
        string? displayName = null,
        string? givenName = null,
        string? surname = null,
        string? jobTitle = null,
        string? department = null,
        string? mobilePhone = null)
    {
        if (!string.IsNullOrWhiteSpace(displayName))
            DisplayName = displayName;
        
        if (!string.IsNullOrWhiteSpace(givenName))
            GivenName = givenName;
        
        if (!string.IsNullOrWhiteSpace(surname))
            Surname = surname;

        JobTitle = jobTitle;
        Department = department;
        MobilePhone = mobilePhone;
        LastModifiedDateTime = DateTime.UtcNow;
    }

    public void SetId(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("User ID cannot be null or empty", nameof(id));
        
        Id = id;
    }

    public void Enable()
    {
        AccountEnabled = true;
        LastModifiedDateTime = DateTime.UtcNow;
    }

    public void Disable()
    {
        AccountEnabled = false;
        LastModifiedDateTime = DateTime.UtcNow;
    }
}