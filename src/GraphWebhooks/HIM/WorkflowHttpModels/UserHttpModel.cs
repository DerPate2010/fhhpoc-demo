namespace HIM.Services.DataModels.WorkflowHttpModels
{
    public class UserHttpModel
    {
        public int id { get; set; }
        public string? displayName { get; set; }
        public string? email { get; set; }
        public string? login { get; set; }
        public string? shortcut { get; set; }

        public string sid { get; set; }

        public string timeStamp { get; set; }
    }
}
