using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIM.Services.DataModels.WorkflowHttpModels
{
    public class WorkflowNoticeHttpModel
    {
        public string notice { get; set; }
        public Guid workflowId { get; set; }
        public string workflowTimeStamp { get; set; }
    }
}
