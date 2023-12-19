using System;

namespace HIM.Services.DataModels.WorkflowHttpModels
{
    public class WorkFlowTaskOutcomeHttpModel
    {
        public Guid taskId { get; set; }
        public string timeStamp { get; set; }
        public int outcome { get; set; }
        public string comment { get; set; }
        public Guid workflowId { get; set; }
    }
}