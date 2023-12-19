using System;

namespace HIM.Services.DataModels.WorkflowHttpModels
{
    public class WorkflowModifyPropertiesHttpModel
    {
        public Guid documentId { get; set; }
        public string newFilename { get; set; }
        public string comment { get; set; }
        public bool forceArchive { get; set; }
    }
}
