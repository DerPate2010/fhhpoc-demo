using GraphWebhooks.Models;
using HIM.Services.DataModels.WorkflowHttpModels;
using HIM.Services.Transport;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GraphWebhooks.Controllers
{
    public class HIMController : Controller
    {
        private ILogger<DataExchange> _logger;
        private readonly GraphServiceClient _graphClient;
        private readonly IWebHostEnvironment _environment;

        public static string Username;
        public static string Password;

        public HIMController(ILogger<DataExchange> logger, GraphServiceClient graphClient, IWebHostEnvironment environment)
        {
            _logger = logger;
            _graphClient = graphClient;
            _environment = environment;
        }


        [AuthorizeForScopes(ScopeKeySection = "GraphScopes")]
        public async Task<IActionResult> Index()
        {
            // Get the user's ID and tenant ID from the user's identity
            var userId = User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
            _logger.LogInformation($"Authenticated user ID {userId}");
            var tenantId = User.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid")?.Value;

            // Get the user from Microsoft Graph
            var user = await _graphClient.Me.Request().GetAsync();

            _logger.LogInformation($"Authenticated user: {user.DisplayName} ({user.Mail ?? user.UserPrincipalName})");

            SyncSettings syncSettings = new SyncSettings();
            syncSettings.AdUser = user.UserPrincipalName;
            syncSettings.LastRun =DateTime.UtcNow;

            return View(syncSettings);
        }

        [HttpPost]
        [AuthorizeForScopes(ScopeKeySection = "GraphScopes")]
        public async Task<IActionResult> Index(SyncSettings syncSettings)
        {
            // Get the user's ID and tenant ID from the user's identity
            var userId = User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
            _logger.LogInformation($"Authenticated user ID {userId}");
            var tenantId = User.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid")?.Value;

            // Get the user from Microsoft Graph
            var user = await _graphClient.Me.Request().GetAsync();

            _logger.LogInformation($"Authenticated user: {user.DisplayName} ({user.Mail ?? user.UserPrincipalName})");
            syncSettings.AdUser = user.UserPrincipalName;

            if (string.IsNullOrWhiteSpace(syncSettings.Password))
            {
                syncSettings.Password = Password;
            }
            Username = syncSettings.Username;
            Password = syncSettings.Password;
            var dx = new DataExchange(_logger, new CredentialManager(syncSettings.Username, syncSettings.Password));
            var workflowHttpBaseModel = await dx.GetJsonAsync<WorkflowBaseHttpModel>("/workflows");

            foreach (var workflow in workflowHttpBaseModel.workflowResults)
            {

                foreach (var phase in workflow.process.phases)
                {
                    foreach (var step in phase.steps)
                    {
                        foreach (var task in step.tasks)
                        {
                            if (task.canCompleteTask)
                            {
                                var activatedAt = new DateTime(long.Parse(task.activatedAt));
                                if (activatedAt > syncSettings.LastRun)
                                {
                                    await SendCardForTask(task, step, phase, workflow, user);
                                }
                            }
                        }
                    }
                }
            }
            syncSettings.LastRun = DateTime.UtcNow;
            return View(syncSettings);
        }

        private async Task SendCardForTask(WorkflowTaskHttpModel task, WorkflowPhaseStepHttpModel step, WorkflowPhaseHttpModel phase, WorkflowResultHttpModel workflow, User user)
        {
            string contentPath = _environment.ContentRootPath;
            var htmlTemplate = System.IO.File.ReadAllText(Path.Combine(contentPath, "Templates\\mailcontent.html"));
            var cardTemplate = System.IO.File.ReadAllText(Path.Combine(contentPath, "Templates\\action.json"));
            DateTime dueDate = DateTime.UtcNow.AddDays(1);
            try
            {
                dueDate = new DateTime(long.Parse(workflow.workflow.activeStepDueDate));

            }
            catch (Exception)
            {
            }
            cardTemplate = cardTemplate.Replace("%taskTitle%", step.title);
            cardTemplate = cardTemplate.Replace("%taskAssignedTo%", user.DisplayName);
            cardTemplate = cardTemplate.Replace("%taskDueDate%", dueDate.ToLongDateString());
            cardTemplate = cardTemplate.Replace("%taskId%", task.taskId.ToString());
            cardTemplate = cardTemplate.Replace("%workflowId%", workflow.workflowId.ToString());
            cardTemplate = cardTemplate.Replace("%workflowTimeStamp%", workflow.timeStamp.ToString());
            cardTemplate = cardTemplate.Replace("%actionUrl%", "https://f10djr4h-44354.euw.devtunnels.ms/api/Actions/");
            cardTemplate = cardTemplate.Replace("%taskApprover%", user.UserPrincipalName);

            htmlTemplate = htmlTemplate.Replace("%cardcontent%", cardTemplate);

            var Message = new Message
            {
                Subject = "HIM - " + workflow.workflow.title,
                Body = new ItemBody
                {
                    ContentType = BodyType.Html,
                    Content = htmlTemplate,
                },
                ToRecipients = new List<Recipient>
                {
                    new Recipient
                    {
                        EmailAddress = new EmailAddress
                        {
                            Address = "steve@fhhpoc23.onmicrosoft.com",
                        },
                    },
                },
                From = new Recipient { EmailAddress = new EmailAddress { Address = "himworkflow@fhhpoc23.onmicrosoft.com" } },
            };


            // To initialize your graphClient, see https://learn.microsoft.com/en-us/graph/sdks/create-client?from=snippets&tabs=csharp
            await _graphClient.Users["himworkflow@fhhpoc23.onmicrosoft.com"].SendMail(Message).Request().PostAsync();
        }
    }
}
