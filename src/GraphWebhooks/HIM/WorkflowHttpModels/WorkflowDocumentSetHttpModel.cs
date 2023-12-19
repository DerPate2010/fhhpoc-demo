using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIM.Services.DataModels.WorkflowHttpModels
{
    /// <summary>
    /// Provides the reference to the document set
    /// </summary>
    public class WorkflowDocumentSetHttpModel
    {
        public Guid documentSetId { get; set; }
        public string? url { get; set; }
        public string timeStamp { get; set; }
        public WorkflowDocumentSetPermissionsHttpModel? permission { get; set; }
    }
}
