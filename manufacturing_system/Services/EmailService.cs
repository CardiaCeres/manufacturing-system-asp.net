using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ManufacturingSystem.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _apiKey = Environment.GetEnvironmentVariable("RESEND_API_KEY")
                                           ?? throw new InvalidOperationException("RESEND_API_KEY not set.");
        private readonly HttpClient _httpClient;

        public EmailService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.resend.com")
            };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        // 發送重設密碼信
        public async Task SendResetPasswordEmailAsync(string toEmail, string resetUrl)
        {
            string htmlContent = $@"
                <div style='font-family:Arial,sans-serif;line-height:1.6'>
                    <h2>🔐 重設密碼通知</h2>
                    <p>請點擊下方按鈕設定新密碼：</p>
                    <p>
                        <a href='{resetUrl}' 
                           style='display:inline-block;padding:10px 20px;background-color:#667eea;color:#fff;text-decoration:none;border-radius:8px;'>
                           重設密碼
                        </a>
                    </p>
                </div>";

            await SendCustomEmailAsync(toEmail, "no-reply@yourapp.com", "重設您的密碼", htmlContent);
        }

        // 發送一般通知信
        public async Task SendNotificationEmailAsync(string toEmail, string subject, string htmlContent)
        {
            await SendCustomEmailAsync(toEmail, "no-reply@yourapp.com", subject, htmlContent);
        }

        // 發送自訂內容信件
        public async Task SendCustomEmailAsync(string toEmail, string fromEmail, string subject, string htmlContent)
        {
            var payload = new
            {
                from = fromEmail,
                to = toEmail,
                subject = subject,
                html = htmlContent
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/v1/emails", content);

            if (!response.IsSuccessStatusCode)
            {
                var respContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Resend API 發信失敗: {response.StatusCode}, {respContent}");
            }
        }
    }
}
