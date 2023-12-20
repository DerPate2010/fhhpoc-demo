using System;

namespace GraphWebhooks.Models
{
    public class ApprovalInfo
    {
        public Guid TaskId { get; set; }
        public string WorkflowTimeStamp { get; set; }
        public string Comment { get; set; }
        public Guid WorkflowId { get; set; }
        public int TaskOutcome { get; set; }
    }
}
