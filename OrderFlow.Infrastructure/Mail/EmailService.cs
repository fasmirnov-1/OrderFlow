using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace OrderFlow.Infrastructure.Mail
{
    public class EmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings?.Value ?? throw new ArgumentNullException(nameof(emailSettings));
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException("Адрес получателя не может быть пустым.", nameof(toEmail));

            var emailMessage = CreateEmailMessage(toEmail, subject, body);
            string decryptedPassword = GetDecryptedPassword();

            using var client = new SmtpClient();

            // Устанавливаем таймаут соединения (например, 10 секунд), чтобы приложение не зависало наглухо при проблемах с сетью
            client.Timeout = 10000;

            // Подключаемся к SMTP-серверу
            await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);

            // Аутентифицируемся с расшифрованным паролем
            await client.AuthenticateAsync(_emailSettings.Username, decryptedPassword);

            // Отправляем сообщение
            await client.SendAsync(emailMessage);

            // Корректно разрываем соединение
            await client.DisconnectAsync(true);
        }

        private MimeMessage CreateEmailMessage(string toEmail, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            message.To.Add(new MailboxAddress(string.Empty, toEmail));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };
            return message;
        }

        private string GetDecryptedPassword()
        {
            // Пытаемся расшифровать пароль. Если он не был зашифрован (например, передан в открытом виде для теста), 
            // метод Decrypt вернет исходную строку или пустую при ошибке.
            string decrypted = AppSettingsCrypto.Decrypt(_emailSettings.Password);

            if (string.IsNullOrEmpty(decrypted))
            {
                // Если расшифровка вернула пустоту, возможно, пароль хранится в чистом виде
                return _emailSettings.Password;
            }

            return decrypted;
        }
    }
}