using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using Microsoft.Extensions.Logging;
using MMA.Domain;

namespace MMA.Service
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string email, string subject, string htmlTemplate,
            string? fileTemplateName = null,
            object? replaceProperty = null,
            CEmailProviderType emailProviderType = CEmailProviderType.Gmail,
            CMicroserviceType serviceType = CMicroserviceType.None)
        {
            return Task.Run(async () =>
            {
                _logger.LogInformation("Starting email sending process.");

                var smtpSettings = emailProviderType switch
                {
                    CEmailProviderType.Gmail => (SmtpSetting)RuntimeContext.AppSettings.EmailConfig.MailKitConfig.GmailSmtp,
                    // CEmailProviderType.Outlook => (SmtpSetting)RuntimeContext.AppSettings.EmailSetting.MailKitSetting.OutlookSmtp,
                    // CEmailProviderType.Proton => (SmtpSetting)RuntimeContext.AppSettings.EmailSetting.MailKitSetting.ProtonMailSmtp,
                    // CEmailProviderType.Brevo => (SmtpSetting)RuntimeContext.AppSettings.EmailSetting.MailKitSetting.BrevoSmtp,
                    // CEmailProviderType.Elastice => (SmtpSetting)RuntimeContext.AppSettings.EmailSetting.MailKitSetting.ElasticeSmtp,
                    _ => throw new ArgumentOutOfRangeException(nameof(emailProviderType), "Loại nhà cung cấp dịch vụ email không được hỗ trợ.")
                };

                _logger.LogInformation("SMTP settings loaded successfully for {EmailProviderType}.", emailProviderType);

                var message = new MimeMessage();
                message.From.Add(MailboxAddress.Parse(smtpSettings.SenderEmail));
                message.To.Add(MailboxAddress.Parse(email));
                message.Subject = subject;

                if (!string.IsNullOrEmpty(fileTemplateName))
                {
                    string templatePath = DirectoryHelper.GetHtmlTemplatePath(templateFileName: fileTemplateName, serviceType: serviceType);
                    _logger.LogInformation("Loading email template from file: {TemplatePath}.", templatePath);

                    htmlTemplate = await File.ReadAllTextAsync(templatePath);
                    _logger.LogInformation("Email template loaded successfully.");

                    if (replaceProperty != null)
                    {
                        _logger.LogInformation("Replacing placeholders in the template with actual values.");

                        foreach (var prop in replaceProperty.GetType().GetProperties().ToList())
                        {
                            string propName = $"[{prop.Name}]";
                            string propValue = prop.GetValue(replaceProperty)?.ToString() ?? string.Empty;
                            htmlTemplate = htmlTemplate.Replace(propName, propValue);

                            _logger.LogDebug("Replaced placeholder {Placeholder} with value {Value}.", propName, propValue);
                        }

                        _logger.LogInformation("All placeholders have been replaced successfully.");
                    }
                }

                message.Body = new TextPart(TextFormat.Html)
                {
                    Text = htmlTemplate
                };

                using (var client = new SmtpClient())
                {
                    try
                    {
                        _logger.LogInformation("Connecting to SMTP server {Server} on port {Port}.", smtpSettings.Server, smtpSettings.Port);
                        // Use SecureSocketOptions.StartTls for port 587
                        client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
                        await client.ConnectAsync(smtpSettings.Server, smtpSettings.Port, SecureSocketOptions.StartTls);
                        _logger.LogInformation("Connected to SMTP server successfully.");

                        _logger.LogInformation("Authenticating with SMTP server using username {Username}.", smtpSettings.Username);
                        await client.AuthenticateAsync(smtpSettings.Username, smtpSettings.Password);
                        _logger.LogInformation("Authenticated with SMTP server successfully.");

                        _logger.LogInformation("Sending email to {Recipient}.", email);
                        await client.SendAsync(message);
                        _logger.LogInformation("Email sent successfully to {Recipient}.", email);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occurred while sending email to {Recipient}.", email);
                        throw new InvalidOperationException($"Đã xảy ra lỗi khi gửi email:: {ex.Message}");
                    }
                    finally
                    {
                        _logger.LogInformation("Disconnecting from SMTP server.");
                        await client.DisconnectAsync(true);
                        client.Dispose();
                        _logger.LogInformation("Disconnected from SMTP server and disposed resources.");
                    }
                }
            });
        }
    }
}
