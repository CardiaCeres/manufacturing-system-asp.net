using System.Threading.Tasks;

namespace ManufacturingSystem.Services
{
    public interface IEmailService
    {
        Task SendResetPasswordEmailAsync(string toEmail, string resetUrl);
    }
}
