using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace HIM.Services.DataModels.WorkflowHttpModels
{
    /// <summary>
    /// Provides information about the location of the documents structure
    /// </summary>
    public class WorkflowDocumentResultHttpModel
    {
        public Guid workflowId { get; set; }
        public string timeStamp { get; set; }
        public WorkflowDocumentSetHttpModel documentSet { get; set; }
        public List<WorkflowDocumentHttpModel>? documents { get; set; }
    }
}
