using System;

namespace HIM.Services.DataModels.WorkflowHttpModels
{
    public class WorkflowDocumentHttpModel
    {
        public string? name { get; set; }
        public Guid documentId { get; set; }
        public string? type { get; set; }
        public string? lastModified { get; set; }
        public int? modifiedById { get; set; }
        public string? version { get; set; }
        public bool forceArchive { get; set; }
        public string? url { get; set; }
        public string? title { get; set; }
        public string? createdAt { get; set; }
        public int? createdById { get; set; }
        public string timeStamp { get; set; }
        public bool permanent { get; set; }
        public int? fileSize { get; set; }
        public int? contenttype { get; set; }
        public bool deleted { get; set; }
        public int sortPriority { get; set; }
    }
}
