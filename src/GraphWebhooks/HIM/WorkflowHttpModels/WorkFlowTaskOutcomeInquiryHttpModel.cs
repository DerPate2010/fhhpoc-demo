using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace HIM.Services.DataModels.WorkflowHttpModels
{
    public class WorkFlowTaskOutcomeInquiryHttpModel
    { 
        public Guid taskId { get; set; }
        public string timeStamp { get; set; }
        public UserHttpModel inquiryAssignee { get; set; }
        public string comment { get; set; }
        public Guid workflowId { get; set; }

        
        public WorkFlowTaskOutcomeInquiryHttpModel()
        {
        }
    }
}
