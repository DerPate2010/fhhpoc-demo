using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIM.Services.DataModels.WorkflowHttpModels
{
    /// <summary>
    /// Workflow process
    /// </summary>
    public class WorkflowProcessHttpModel
    {
        public string timeStamp { get; set; }
        public Guid processId { get; set; }
        public Guid workflowId { get; set; }

        public Guid configurationId { get; set; }
        public bool processChanged { get; set; }

        public List<WorkflowPhaseHttpModel>? phases { get; set; }
    }
}
