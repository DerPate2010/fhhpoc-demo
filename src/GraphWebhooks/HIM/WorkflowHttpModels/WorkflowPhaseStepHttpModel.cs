using System;
using System.Collections.Generic;

namespace HIM.Services.DataModels.WorkflowHttpModels
{
    /// <summary>
    /// Workflow step (contained in phase)
    /// </summary>
    public class WorkflowPhaseStepHttpModel
    {
        public Guid stepId { get; set; }
        public int type { get; set; }
        public string? title { get; set; }
        public string? description { get; set; }
        public string? dueDate { get; set; }
        public int priority { get; set; }
        public int status { get; set; }
        public Guid configurationStepId { get; set; }
        public Guid phaseId { get; set; }
        public string timeStamp { get; set; }
        public string? provisionComment { get; set; }
        public int? provisionStatus { get; set; }
        public Guid? nextStepId { get; set; }
        public List<WorkflowTaskHttpModel>? tasks { get; set; }
    }
}