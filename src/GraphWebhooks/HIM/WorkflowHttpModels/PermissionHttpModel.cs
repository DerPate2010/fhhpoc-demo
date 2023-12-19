namespace HIM.Services.DataModels.WorkflowHttpModels
{
    public class PermissionHttpModel
    {
        public bool hasRead { get; set; }
        public bool hasWrite { get; set; }
        public bool hasDelete { get; set; }
        public bool canEditDescription { get; set; }
        public bool canEditPriority { get; set; }
        public bool canEditTitle { get; set; }
        public bool hasInitiatorRole { get; set; }
    }
}