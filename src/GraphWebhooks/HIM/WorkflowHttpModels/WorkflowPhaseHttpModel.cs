using System;
using System.Collections.Generic;

namespace HIM.Services.DataModels.WorkflowHttpModels
{
    /// <summary>
    /// Workflow phase
    /// </summary>
    public class WorkflowPhaseHttpModel
    {
        public Guid phaseId { get; set; }
        public string? title { get; set; }
        public Guid configurationPhaseId { get; set; }
        public string? processId { get; set; }
        public int status { get; set; }
        public string timeStamp { get; set; }
        public int conversionStatus { get; set; }
        
        public Guid? nextPhaseId { get; set; }
        public List<WorkflowPhaseStepHttpModel>? steps { get; set; }
    }
}