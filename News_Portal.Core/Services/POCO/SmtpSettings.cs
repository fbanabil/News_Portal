using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.Services.POCO
{
    public class SmtpSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool UseSsl { get; set; } = false;
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = "News Portal";
    }
}
