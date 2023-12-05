// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Microsoft.Graph;

using GraphWebhooks.Models;
using GraphWebhooks.Services;
using System.Collections.Generic;

namespace GraphWebhooks.Controllers;

/// <summary>
/// Implements subscription management endpoints
/// </summary>
public class TodoController : Controller
{
    private readonly GraphServiceClient _graphClient;
    private readonly SubscriptionStore _subscriptionStore;
    private readonly CertificateService _certificateService;
    private readonly ILogger<WatchController> _logger;
    private readonly string _notificationHost;

    public TodoController(
        GraphServiceClient graphClient,
        SubscriptionStore subscriptionStore,
        CertificateService certificateService,
        ILogger<WatchController> logger,
        IConfiguration configuration)
    {
        _graphClient = graphClient ?? throw new ArgumentException(nameof(graphClient));
        _subscriptionStore = subscriptionStore ?? throw new ArgumentException(nameof(subscriptionStore));
        _certificateService = certificateService ?? throw new ArgumentException(nameof(certificateService));
        _logger = logger ?? throw new ArgumentException(nameof(logger));
        _ = configuration ?? throw new ArgumentException(nameof(configuration));

        _notificationHost = configuration.GetValue<string>("NotificationHost");
        if (string.IsNullOrEmpty(_notificationHost) || _notificationHost == "YOUR_NGROK_PROXY")
        {
            throw new ArgumentException("You must configure NotificationHost in appsettings.json");
        }
    }

    /// <summary>
    /// GET /watch/delegated
    /// Creates a new subscription to the authenticated user's inbox and
    /// displays a page that updates with each received notification
    /// </summary>
    /// <returns></returns>
    [AuthorizeForScopes(ScopeKeySection = "GraphScopes")]
    public async Task<IActionResult> List()
    {
        try
        {
            // Get the user's ID and tenant ID from the user's identity
            var userId = User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
            _logger.LogInformation($"Authenticated user ID {userId}");
            var tenantId = User.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid")?.Value;

            // Get the user from Microsoft Graph
            var user = await _graphClient.Me.Request().GetAsync();

            _logger.LogInformation($"Authenticated user: {user.DisplayName} ({user.Mail ?? user.UserPrincipalName})");
            // Add the user's display name and email address to the user's
            // identity.
            User.AddUserGraphInfo(user);

            var todo = await _graphClient.Me.Todo.Lists.Request().GetAsync();

            return View(todo);
        }
        catch (Exception ex)
        {
            // Throw MicrosoftIdentityWebChallengeUserException to allow
            // Microsoft.Identity.Web to challenge the user for re-auth or consent
            if (ex.InnerException is MicrosoftIdentityWebChallengeUserException) throw;

            // Otherwise display the error
            return View().WithError($"Error creating subscription: {ex.Message}",
                ex.ToString());
        }
    }

    [AuthorizeForScopes(ScopeKeySection = "GraphScopes")]
    public async Task<IActionResult> Create()
    {
        try
        {
            // Get the user's ID and tenant ID from the user's identity
            var userId = User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
            _logger.LogInformation($"Authenticated user ID {userId}");
            var tenantId = User.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid")?.Value;

            // Get the user from Microsoft Graph
            var user = await _graphClient.Me
                .Request()
                .GetAsync();

            _logger.LogInformation($"Authenticated user: {user.DisplayName} ({user.Mail ?? user.UserPrincipalName})");
            // Add the user's display name and email address to the user's
            // identity.
            User.AddUserGraphInfo(user);

            var todo = await _graphClient.Me.Todo.Lists.Request().GetAsync();
            var taskListId = todo.CurrentPage[0].Id;

            var requestBody = new TodoTask
            {
                Title = "Test HIM 7 (z.d.A.)",
                //Categories = new List<string>
                //{
                //    "Important",
                //},
                LinkedResources = new TodoTaskLinkedResourcesCollectionPage()
                {
                    new LinkedResource
                    {
                        WebUrl = "https://dev.azure.com/MA-fhhsk/fhhsk-apps/_sprints/backlog/HIM/fhhsk-apps/HIM/Sprint%2021?workitem=1067",
                        ApplicationName = "Jira",
                        DisplayName = "Jira",
                    },
                },
            };

            // To initialize your graphClient, see https://learn.microsoft.com/en-us/graph/sdks/create-client?from=snippets&tabs=csharp
            var result = await _graphClient.Me.Todo.Lists[taskListId].Tasks.Request().AddAsync(requestBody);


            return View(todo);
        }
        catch (Exception ex)
        {
            // Throw MicrosoftIdentityWebChallengeUserException to allow
            // Microsoft.Identity.Web to challenge the user for re-auth or consent
            if (ex.InnerException is MicrosoftIdentityWebChallengeUserException) throw;

            // Otherwise display the error
            return View().WithError($"Error creating subscription: {ex.Message}",
                ex.ToString());
        }
    }
}
