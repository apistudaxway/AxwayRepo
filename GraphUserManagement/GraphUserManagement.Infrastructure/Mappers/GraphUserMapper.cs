using GraphUserManagement.Domain.Entities;
using GraphUserManagement.Domain.ValueObjects;
using Microsoft.Graph.Models;
using DomainUser = GraphUserManagement.Domain.Entities.User;
using GraphUser = Microsoft.Graph.Models.User;

namespace GraphUserManagement.Infrastructure.Mappers;

public static class GraphUserMapper
{
    public static GraphUser ToGraphUser(DomainUser domainUser)
    {
        return new GraphUser
        {
            DisplayName = domainUser.DisplayName,
            Mail = domainUser.Email.Value,
            GivenName = domainUser.GivenName,
            Surname = domainUser.Surname,
            UserPrincipalName = domainUser.UserPrincipalName,
            JobTitle = domainUser.JobTitle,
            Department = domainUser.Department,
            MobilePhone = domainUser.MobilePhone,
            AccountEnabled = domainUser.AccountEnabled,
            PasswordProfile = new PasswordProfile
            {
                ForceChangePasswordNextSignIn = true,
                Password = GenerateTemporaryPassword()
            }
        };
    }

    public static GraphUser ToGraphUserUpdate(DomainUser domainUser)
    {
        return new GraphUser
        {
            DisplayName = domainUser.DisplayName,
            GivenName = domainUser.GivenName,
            Surname = domainUser.Surname,
            JobTitle = domainUser.JobTitle,
            Department = domainUser.Department,
            MobilePhone = domainUser.MobilePhone,
            AccountEnabled = domainUser.AccountEnabled
            // Note: Mail and UserPrincipalName are typically not updated after creation
        };
    }

    public static DomainUser ToDomainUser(GraphUser graphUser)
    {
        var email = new Email(graphUser.Mail ?? graphUser.UserPrincipalName ?? string.Empty);
        
        var domainUser = new DomainUser(
            graphUser.DisplayName ?? string.Empty,
            email,
            graphUser.GivenName ?? string.Empty,
            graphUser.Surname ?? string.Empty,
            graphUser.UserPrincipalName ?? string.Empty,
            graphUser.JobTitle,
            graphUser.Department,
            graphUser.MobilePhone,
            graphUser.AccountEnabled ?? true);

        // Set the ID from Graph API
        if (!string.IsNullOrEmpty(graphUser.Id))
        {
            domainUser.SetId(graphUser.Id);
        }

        return domainUser;
    }

    private static string GenerateTemporaryPassword()
    {
        // Generate a secure temporary password
        // In production, consider using a more sophisticated password generation strategy
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
        var password = new string(Enumerable.Repeat(chars, 12)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        
        return password + "Temp1!"; // Ensure complexity requirements are met
    }
}