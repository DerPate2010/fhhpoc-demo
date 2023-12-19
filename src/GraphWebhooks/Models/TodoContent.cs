namespace GraphWebhooks.Models
{
    public class TodoContent
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string WebUrl { get; set; }
        public string ApplicationName { get; set; }
        public string Body { get; set; }
        public bool IsHtml { get; set; }
        public string DisplayName { get; internal set; }
    }
}
