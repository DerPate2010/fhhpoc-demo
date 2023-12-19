using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIM.Services.DataModels.WorkflowHttpModels
{
    /// <summary>
    /// Base information of a workflow
    /// </summary>
    public class WorkflowHttpModel
    {
        public string workflowId { get; set; }
        public string? tenantId { get; set; }
        public string? description { get; set; }
        public string? dueDate { get; set; }
        public string? activeStepDueDate { get; set; }
        public int? initiatorId { get; set; }
        public int? deletedById { get; set; }
        public int priority { get; set; }
        public int state { get; set; }
        public string? title { get; set; }

        public string? startedAt { get; set; }
        public string? createdAt { get; set; }


        public PermissionHttpModel permission { get; set; }
        public string? assignee { get; set; }
        public Guid? currentPhaseId { get; set; }
        public Guid? currentStepId { get; set; }
        public string? notice { get; set; }
        public int? noticeEditorId { get; set; }
        public string? noticeLastModified { get; set; }
        public bool retractWorkflowBlocked { get; set; }
        public bool? confidential { get; set; }
        public int origin { get; set; }
        //public string timeStamp { get; set; }
        public List<WorkflowCcParticipantShortHttpModel>? ccParticipants { get; set; }
    }
}
