using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ManufacturingSystem.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpHost = "smtp.sendgrid.net"; // 可換成其他
        private readonly int _smtpPort = 587;
        private readonly string _smtpUser = "apikey";
        private readonly string _smtpPass = "YOUR_SENDGRID_APIKEY";

        public async Task SendResetPasswordEmailAsync(string toEmail, string resetUrl)
        {
            using var client = new SmtpClient(_smtpHost, _smtpPort)
            {
                Credentials = new NetworkCredential(_smtpUser, _smtpPass),
                EnableSsl = true
            };

            var message = new MailMessage();
            message.From = new MailAddress("no-reply@yourapp.com");
            message.To.Add(toEmail);
            message.Subject = "重設您的密碼";
            message.IsBodyHtml = true;
            message.Body = $@"
                <div style='font-family:Arial,sans-serif;line-height:1.6'>
                    <h2>🔐 重設密碼通知</h2>
                    <p>請點擊下方按鈕設定新密碼：</p>
                    <p><a href='{resetUrl}' style='display:inline-block;padding:10px 20px;background-color:#667eea;color:#fff;text-decoration:none;border-radius:8px;'>重設密碼</a></p>
                </div>";

            await client.SendMailAsync(message);
        }
    }
}
