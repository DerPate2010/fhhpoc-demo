using System.Collections.Generic;

namespace HIM.Services.DataModels.WorkflowHttpModels
{
    /// <summary>
    /// Root element for receiving workflows
    /// </summary>
    public class WorkflowBaseHttpModel
    {
        public string? timeStamp { get; set; }
        public List<WorkflowResultHttpModel>? workflowResults { get; set; }
        public List<UserHttpModel>? principals { get; set; }
        public List<WorkflowTenantHttpModel>? tenants { get; set; }
    }
}
