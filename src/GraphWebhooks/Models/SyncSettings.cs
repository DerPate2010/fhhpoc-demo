using System;
using System.ComponentModel.DataAnnotations;

namespace GraphWebhooks.Models
{
    public class SyncSettings
    {
        public DateTime LastRun { get; set; }

        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public string AdUser { get; set; }
    }
}
