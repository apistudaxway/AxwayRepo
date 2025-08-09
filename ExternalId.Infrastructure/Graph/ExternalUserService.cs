using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.Identity;
using ExternalId.Application;
using ExternalId.Application.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace ExternalId.Infrastructure.Graph;

public sealed class ExternalUserService : IExternalUserService
{
    private readonly GraphServiceClient _graphClient;
    private readonly GraphSettings _settings;
    private readonly ILogger<ExternalUserService> _logger;

    public ExternalUserService(IOptions<GraphSettings> options, ILogger<ExternalUserService> logger)
    {
        _settings = options.Value;
        _logger = logger;

        var credential = new ClientSecretCredential(_settings.TenantId, _settings.ClientId, _settings.ClientSecret);
        _graphClient = new GraphServiceClient(credential, new[] { "https://graph.microsoft.com/.default" });
    }

    public async Task<string> CreateUserAsync(CreateExternalUserCommand command, CancellationToken cancellationToken)
    {
        var user = new User
        {
            AccountEnabled = command.AccountEnabled,
            DisplayName = command.DisplayName,
            GivenName = command.GivenName,
            Surname = command.Surname,
            Identities = new List<ObjectIdentity>
            {
                new ObjectIdentity
                {
                    SignInType = "emailAddress",
                    // For Azure Entra External ID (B2C/Customer), issuer is your tenant domain or tenantId
                    Issuer = _settings.TenantId,
                    IssuerAssignedId = command.Email
                }
            },
            PasswordProfile = new PasswordProfile
            {
                Password = command.Password,
                ForceChangePasswordNextSignIn = command.ForceChangePasswordNextSignIn
            },
            PasswordPolicies = "DisablePasswordExpiration"
        };

        try
        {
            var created = await _graphClient.Users.PostAsync(user, cancellationToken: cancellationToken);
            if (created is null || string.IsNullOrWhiteSpace(created.Id))
            {
                throw new InvalidOperationException("Microsoft Graph did not return a created user ID.");
            }
            return created.Id;
        }
        catch (ServiceException ex)
        {
            _logger.LogError(ex, "Graph service error while creating external user");
            throw;
        }
    }
}