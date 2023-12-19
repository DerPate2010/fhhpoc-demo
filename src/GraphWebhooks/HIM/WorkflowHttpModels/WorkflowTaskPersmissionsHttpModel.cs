using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIM.Services.DataModels.WorkflowHttpModels
{
    public class WorkflowTaskPersmissionsHttpModel
    {
        public bool canApprove { get; set; }
        public bool canReject { get; set; }
        public bool canSetToDone { get; set; }
        public bool canPostpone { get; set; }
        public bool canDelegate { get; set; }
        public bool canStartInquiry { get; set; }
        public bool canEditDocuments { get; set; }
        public bool canAlterWorkflowProcess { get; set; }
        public bool canStartIntermediateProvision { get; set; }
    }
}
