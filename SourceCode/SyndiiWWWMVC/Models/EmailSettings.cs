using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyndiiWWWMVC.Models
{
    /// <summary>
    /// Email setting
    /// </summary>
    public class EmailSettings
    {
        public string UsernameEmail { get; set; }
        public string UsernamePassword { get; set; }
        public string DisplayName { get; set; }
        public string PrimaryDomain { get; set; }
        public string PrimaryPort { get; set; }
        public bool EnableSsl { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
