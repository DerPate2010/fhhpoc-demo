using GraphWebhooks.Models;
using HIM.Services.Transport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using HIM.Services.DataModels.WorkflowHttpModels;
using Microsoft.O365.ActionableMessages.Utilities;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Build.Framework;
using Microsoft.VisualBasic;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GraphWebhooks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActionsController : ControllerBase
    {
        private readonly ILogger<DataExchange> _logger;
        private readonly IWebHostEnvironment _environment;

        public ActionsController(ILogger<DataExchange> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        // GET: api/<ActionsController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ActionsController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<ActionsController>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ApprovalInfo value)
        {
            var auth = HttpContext.Request.Headers.Authorization[0].Trim().Split(' ');
            if (auth.Length == 2 && auth[0].ToLowerInvariant() == "bearer")
            {
                var token = auth[1];

                var validator = new ActionableMessageTokenValidator();

                var validationResult = await validator.ValidateTokenAsync(token, "https://" + Request.Host.Value);
                if (!validationResult.ValidationSucceeded)
                {
                    return Unauthorized(validationResult.Exception.Message);
                }
            }
            else
            {
                return Unauthorized("Invalid auth header");
            }

            var dx = new DataExchange(_logger, new CredentialManager(HIMController.Username, HIMController.Password));

            var workflowHttpBaseModel = await dx.GetJsonAsync<WorkflowBaseHttpModel>("/workflows");

            foreach (var workflow in workflowHttpBaseModel.workflowResults)
            {

                if (workflow.workflowId == value.WorkflowId)
                {
                    value.WorkflowTimeStamp = workflow.timeStamp;
                    break;
                }
            }

            var httpModel = new WorkFlowTaskOutcomeHttpModel()
            {
                taskId = value.TaskId,
                timeStamp = value.WorkflowTimeStamp,
                outcome = value.TaskOutcome,
                comment = value.Comment,
                workflowId = value.WorkflowId
            };

            await dx.PatchAsync($"/tasks/{value.TaskId}", httpModel, CancellationToken.None);

            string contentPath = _environment.ContentRootPath;

            var template = value.TaskOutcome == 1 ? "approved" : "declined";

            var cardTemplate = System.IO.File.ReadAllText(Path.Combine(contentPath, "Templates\\" + template + ".json"));

            //cardTemplate = cardTemplate.Replace("%taskTitle%", step.title);
            //cardTemplate = cardTemplate.Replace("%taskAssignedTo%", user.DisplayName);
            //cardTemplate = cardTemplate.Replace("%taskDueDate%", dueDate.ToLongDateString());
            //cardTemplate = cardTemplate.Replace("%taskId%", task.taskId.ToString());
            //cardTemplate = cardTemplate.Replace("%workflowId%", workflow.workflowId.ToString());
            //cardTemplate = cardTemplate.Replace("%workflowTimeStamp%", workflow.timeStamp.ToString());
            //cardTemplate = cardTemplate.Replace("%taskApprover%", user.UserPrincipalName);
            cardTemplate = cardTemplate.Replace("%actionUrl%", "https://" + Request.Host.Value + "/api/Actions/");
            cardTemplate = cardTemplate.Replace("%originator%", HIMController.HostOriginator[Request.Host.Value]);

            Response.Headers.Add("Content-Type", "application/json");
            Response.Headers.Add("CARD-UPDATE-IN-BODY", "true");
            
            return Content(cardTemplate);
        }

        // PUT api/<ActionsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ActionsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
