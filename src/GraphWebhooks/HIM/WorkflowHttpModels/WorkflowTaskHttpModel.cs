using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIM.Services.DataModels.WorkflowHttpModels
{
    /// <summary>
    /// Workflow task
    /// </summary>
    public class WorkflowTaskHttpModel
    {
        public Guid taskId { get; set; }
        public bool assignedToCurrentPrincipal { get; set; }
        public int? assigneeId { get; set; }
        public Guid stepId { get; set; }
        public int? executorId { get; set; }
        public string? comment { get; set; }
        public int? outcome { get; set; }
        public int status { get; set; }
        public string timeStamp { get; set; }

        //Todo: Matthias -> Das ist neu!
        public bool canCompleteTask { get; set; }
        public string? activatedAt { get; set; }
        public string? completedAt { get; set; }

        public WorkflowTaskPersmissionsHttpModel? permissions { get; set; }
    }
}
