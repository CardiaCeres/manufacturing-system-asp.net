using System.Threading.Tasks;

namespace ManufacturingSystem.Services
{
    public interface IEmailService
    {
        // 發送重設密碼信
        Task SendResetPasswordEmailAsync(string toEmail, string resetUrl);

        // 發送一般通知信
        Task SendNotificationEmailAsync(string toEmail, string subject, string htmlContent);

        // 發送自訂內容信件
        Task SendCustomEmailAsync(string toEmail, string subject, string htmlContent);
    }
}