using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace News_Portal.Core.ServiceContracts
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string htmlMessage);
        Task SendEmailConfirmationAsync(string email, string callbackUrl);
    }
}
