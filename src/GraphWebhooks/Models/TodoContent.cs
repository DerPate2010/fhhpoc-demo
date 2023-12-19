using System.ComponentModel.DataAnnotations;

namespace GraphWebhooks.Models
{
    public class TodoContent
    {
        [Required]
        public string Title { get; set; }
        public string WebUrl { get; set; }
        [Required]
        public string ApplicationName { get; set; }
        public string Body { get; set; }
        public bool IsHtml { get; set; }
        [Required]
        public string DisplayName { get; set; }
    }
}
