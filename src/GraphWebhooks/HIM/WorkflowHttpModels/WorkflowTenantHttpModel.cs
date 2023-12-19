namespace HIM.Services.DataModels.WorkflowHttpModels
{
    /// <summary>
    /// Workflow (SharePoint) tenant information
    /// </summary>
    public class WorkflowTenantHttpModel
    {
        public int id { get; set; }
        public string? name { get; set; }
        public string? siteCollectionUrl { get; set; }
    }
}
