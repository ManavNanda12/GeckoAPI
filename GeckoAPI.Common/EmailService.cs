using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;

namespace GeckoAPI.Common
{
    public class EmailService
    {
        private readonly SmtpSettings _settings;

        public EmailService(IOptions<SmtpSettings> settings)
        {
            _settings = settings.Value;
        }

        #region Error Email Method
        public async Task SendErrorEmailAsync(string subject, string htmlBody)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));

            foreach (var recipient in _settings.Recipients)
            {
                message.To.Add(MailboxAddress.Parse(recipient));
            }

            message.Subject = subject;
            message.Body = new TextPart("html") { Text = htmlBody };

            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.Server, _settings.Port, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_settings.Username, _settings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        #endregion

        #region Common Email Methods

        /// <summary>
        /// Send a single email to one recipient
        /// </summary>
        public async Task<bool> SendEmailAsync(string toEmail, string toName, string subject, string htmlBody)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
                message.To.Add(new MailboxAddress(toName, toEmail));
                message.Subject = subject;
                message.Body = new TextPart("html") { Text = htmlBody };

                using var client = new SmtpClient();
                await client.ConnectAsync(_settings.Server, _settings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_settings.Username, _settings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Send email with CC and BCC support
        /// </summary>
        public async Task<bool> SendEmailWithCCBCCAsync(
            string toEmail,
            string toName,
            string subject,
            string htmlBody,
            List<string> ccEmails = null,
            List<string> bccEmails = null)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
                message.To.Add(new MailboxAddress(toName, toEmail));

                // Add CC recipients
                if (ccEmails != null && ccEmails.Any())
                {
                    foreach (var cc in ccEmails)
                    {
                        message.Cc.Add(MailboxAddress.Parse(cc));
                    }
                }

                // Add BCC recipients
                if (bccEmails != null && bccEmails.Any())
                {
                    foreach (var bcc in bccEmails)
                    {
                        message.Bcc.Add(MailboxAddress.Parse(bcc));
                    }
                }

                message.Subject = subject;
                message.Body = new TextPart("html") { Text = htmlBody };

                using var client = new SmtpClient();
                await client.ConnectAsync(_settings.Server, _settings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_settings.Username, _settings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Send email to multiple recipients
        /// </summary>
        public async Task<bool> SendBulkEmailAsync(List<string> toEmails, string subject, string htmlBody)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));

                foreach (var email in toEmails)
                {
                    message.To.Add(MailboxAddress.Parse(email));
                }

                message.Subject = subject;
                message.Body = new TextPart("html") { Text = htmlBody };

                using var client = new SmtpClient();
                await client.ConnectAsync(_settings.Server, _settings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_settings.Username, _settings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Send individual emails to multiple recipients (personalized)
        /// </summary>
        public async Task<EmailBatchResult> SendIndividualEmailsAsync(List<EmailRecipient> recipients, string subject, Func<EmailRecipient, string> htmlBodyGenerator)
        {
            var result = new EmailBatchResult();

            using var client = new SmtpClient();

            try
            {
                // Connect once for all emails
                await client.ConnectAsync(_settings.Server, _settings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_settings.Username, _settings.Password);

                foreach (var recipient in recipients)
                {
                    try
                    {
                        var message = new MimeMessage();
                        message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
                        message.To.Add(new MailboxAddress(recipient.Name, recipient.Email));
                        message.Subject = subject;

                        // Generate personalized body
                        string htmlBody = htmlBodyGenerator(recipient);
                        message.Body = new TextPart("html") { Text = htmlBody };

                        await client.SendAsync(message);
                        result.SuccessCount++;

                        // Small delay to avoid overwhelming server
                        await Task.Delay(100);
                    }
                    catch (Exception ex)
                    {
                        result.FailureCount++;
                        result.FailedEmails.Add(new FailedEmail
                        {
                            Email = recipient.Email,
                            Error = ex.Message
                        });
                    }
                }

                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                result.FailureCount = recipients.Count - result.SuccessCount;
                result.GeneralError = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// Send email with attachment
        /// </summary>
        public async Task<bool> SendEmailWithAttachmentAsync(
            string toEmail,
            string toName,
            string subject,
            string htmlBody,
            List<EmailAttachment> attachments)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
                message.To.Add(new MailboxAddress(toName, toEmail));
                message.Subject = subject;

                var builder = new BodyBuilder
                {
                    HtmlBody = htmlBody
                };

                // Add attachments
                if (attachments != null && attachments.Any())
                {
                    foreach (var attachment in attachments)
                    {
                        builder.Attachments.Add(attachment.FileName, attachment.Content);
                    }
                }

                message.Body = builder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(_settings.Server, _settings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_settings.Username, _settings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Send Welcome Email to Customer
        /// </summary>
        public async Task<bool> SendWelcomeEmailAsync(string customerEmail, string customerName, string htmlTemplate)
        {
            try
            {
                return await SendEmailAsync(customerEmail, customerName, "Welcome to Gecko!", htmlTemplate);
            }
            catch (Exception)
            {
                return false;
            }
        }


        /// <summary>
        /// Send Monthly Sales Report to Admins
        /// </summary>
        public async Task<bool> SendMonthlyReportEmailAsync(string toEmail, string toName, string subject, string htmlTemplate)
        {
            try
            {
                return await SendEmailAsync(toEmail, toName,subject, htmlTemplate);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SendCustomerGeneratedPasswordMail(string customerEmail, string customerName, string htmlTemplate)
        {
            try
            {
                return await SendEmailAsync(customerEmail, customerName, "Your Password For Gecko!", htmlTemplate);
            }
            catch (Exception)
            {
                return false;
            }
        }


        #endregion
    }

    #region Supporting Classes

    public class SmtpSettings
    {
        public string Server { get; set; }
        public int Port { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<string> Recipients { get; set; } // For error emails
    }

    public class EmailRecipient
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> CustomData { get; set; } = new Dictionary<string, string>();
    }

    public class EmailBatchResult
    {
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public List<FailedEmail> FailedEmails { get; set; } = new List<FailedEmail>();
        public string GeneralError { get; set; }
    }

    public class FailedEmail
    {
        public string Email { get; set; }
        public string Error { get; set; }
    }

    public class EmailAttachment
    {
        public string FileName { get; set; }
        public byte[] Content { get; set; }
    }

    #endregion
}