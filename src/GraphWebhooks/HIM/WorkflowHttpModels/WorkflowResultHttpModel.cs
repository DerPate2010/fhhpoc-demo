using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIM.Services.DataModels.WorkflowHttpModels
{
    public class WorkflowResultHttpModel
    {
        public Guid workflowId { get; set; }
        public string timeStamp { get; set; }
        public WorkflowHttpModel workflow { get; set; }
        public WorkflowProcessHttpModel process { get; set; }

        //Todo: Matthias -> Eigenschaft ist neu!!!
        public WorkflowDocumentResultHttpModel documentResults { get; set; }
    }
}
