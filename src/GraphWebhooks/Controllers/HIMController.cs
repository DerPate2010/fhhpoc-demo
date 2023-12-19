using GraphWebhooks.Models;
using HIM.Services.DataModels.WorkflowHttpModels;
using HIM.Services.Transport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using System;
using System.Threading.Tasks;

namespace GraphWebhooks.Controllers
{
    public class HIMController : Controller
    {
        private ILogger<DataExchange> _logger;

        public HIMController(ILogger<DataExchange> logger)
        {
            _logger = logger;
        }


        [AuthorizeForScopes(ScopeKeySection = "GraphScopes")]
        public IActionResult Index()
        {
            var userId = User.FindFirst("preferred_username")?.Value;
            SyncSettings syncSettings = new SyncSettings();
            syncSettings.AdUser = userId;
            syncSettings.LastRun =DateTime.UtcNow;

            return View(syncSettings);
        }

        [HttpPost]
        [AuthorizeForScopes(ScopeKeySection = "GraphScopes")]
        public async Task<IActionResult> Index(SyncSettings syncSettings)
        {
            var userId = User.FindFirst("preferred_username")?.Value;
            syncSettings.AdUser = userId;


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
                                }
                            }
                        }
                    }
                }
            }
            syncSettings.LastRun = DateTime.UtcNow;
            return View(syncSettings);
        }
    }
}
