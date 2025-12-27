using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using News_Portal.Core.ServiceContracts;
using News_Portal.Core.Services.POCO;
using System;
using System.Threading.Tasks;

namespace News_Portal.Core.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpSettings _smtp;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<SmtpSettings> smtpOptions, ILogger<EmailService> logger)
        {
            _smtp = smtpOptions?.Value ?? throw new ArgumentNullException(nameof(smtpOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SendEmailAsync(string to, string subject, string htmlMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtp.FromName, _smtp.FromEmail));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var builder = new BodyBuilder { HtmlBody = htmlMessage };
            message.Body = builder.ToMessageBody();

            using var client = new MailKit.Net.Smtp.SmtpClient();
            try
            {
                var secureOption = _smtp.UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;
                await client.ConnectAsync(_smtp.Host, _smtp.Port, secureOption).ConfigureAwait(false);

                if (!string.IsNullOrWhiteSpace(_smtp.UserName))
                    await client.AuthenticateAsync(_smtp.UserName, _smtp.Password).ConfigureAwait(false);

                await client.SendAsync(message).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}", to);
                throw;
            }
            finally
            {
                if (client.IsConnected)
                    await client.DisconnectAsync(true).ConfigureAwait(false);
            }
        }

        public Task SendEmailConfirmationAsync(string email, string callbackUrl)
        {
            var html = $"Please confirm your account by <a href='{System.Net.WebUtility.HtmlEncode(callbackUrl)}'>clicking here</a>.";
            return SendEmailAsync(email, "Confirm your email", html);
        }
    }
}