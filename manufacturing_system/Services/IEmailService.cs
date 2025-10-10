using System.Threading.Tasks;

namespace ManufacturingSystem.Services
{
    public interface IEmailService
    {
        /// <summary>
        /// 發送重設密碼信
        /// </summary>
        Task SendResetPasswordEmailAsync(string toEmail, string resetUrl);

        /// <summary>
        /// 發送一般通知信
        /// </summary>
        Task SendNotificationEmailAsync(string toEmail, string subject, string htmlContent);

        /// <summary>
        /// 發送自訂內容信件
        /// </summary>
        Task SendCustomEmailAsync(string toEmail, string fromEmail, string subject, string htmlContent);
    }
}